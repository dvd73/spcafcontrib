using System.Collections.Concurrent;
using System.IO;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Consts;
using MethodDefinition = Mono.Cecil.MethodDefinition;
using ICSharpCode.Decompiler.Ast.Transforms;

namespace SPCAFContrib.Extensions
{
    public static class DecompilationExtensions
    {

        static readonly ConcurrentDictionary<string, BlockStatement> DecompilerMethodBodyCache = new ConcurrentDictionary<string, BlockStatement>();
        static readonly ConcurrentDictionary<string, EntityDeclaration> DecompilerMethodCache = new ConcurrentDictionary<string, EntityDeclaration>();
        static readonly ConcurrentDictionary<string, NamespaceDeclaration> DecompilerTypeCache = new ConcurrentDictionary<string, NamespaceDeclaration>();

        public static BlockStatement DecompileMethodBody(this MethodDefinition method)
        {
            return DecompilerMethodBodyCache.GetOrAdd(method.FullName, _ => DecompileMethodBodyAux(method));
        }

        private static BlockStatement DecompileMethodBodyAux(MethodDefinition method)
        {
            try
            {
                EntityDeclaration m = method.DecompileMethod();
                if (m is MethodDeclaration)
                {
                    return (m as MethodDeclaration).Body;
                }
                else if (m is ConstructorDeclaration)
                {
                    return (m as ConstructorDeclaration).Body;
                }
                else if (m is OperatorDeclaration)
                {
                    return (m as OperatorDeclaration).Body;
                }
                else if (m is Accessor)
                {
                    return (m as Accessor).Body;
                }
                else if (m is DestructorDeclaration)
                {
                    return (m as DestructorDeclaration).Body;
                }
                else
                {
                   throw new InvalidOperationException();
                }
            }
            catch (Mono.Cecil.ResolutionException)
            {
                return DecompileMethodBodyAlt(method);
            }
        }
        private static BlockStatement DecompileMethodBodyAlt(MethodDefinition method)
        {
            DecompilerContext context = GetDecompilerContext(method);

            BlockStatement body = AstMethodBodyBuilder.CreateMethodBody(method, context, AstBuilder.MakeParameters(method));
            TransformationPipeline.RunTransformationsUntil(body, null, context);
            return body;
        }

        private static DecompilerContext GetDecompilerContext(MethodDefinition method)
        {
            DecompilerSettings decompilerSettings = new DecompilerSettings();
            decompilerSettings.FullyQualifyAmbiguousTypeNames = true;
            decompilerSettings.UsingStatement = false;
            decompilerSettings.UsingDeclarations = false;
            DecompilerContext context = new DecompilerContext(method.Module)
            {
                CurrentType = method.DeclaringType,
                Settings = decompilerSettings
            };
            return context;
        }

        private static DecompilerContext GetDecompilerContext(TypeDefinition typeDef)
        {
            DecompilerSettings decompilerSettings = new DecompilerSettings();
            decompilerSettings.FullyQualifyAmbiguousTypeNames = true;
            decompilerSettings.UsingStatement = true;
            decompilerSettings.UsingDeclarations = false;
            DecompilerContext context = new DecompilerContext(typeDef.Module)
            {
                CurrentType = typeDef,
                Settings = decompilerSettings
            };
            return context;
        }

        public static EntityDeclaration DecompileMethod(this MethodDefinition method)
        {
            return DecompilerMethodCache.GetOrAdd(method.FullName, _ => DecompileMethodAux(method));
        }

        public static NamespaceDeclaration DecompileType(this TypeDefinition typeDef)
        {
            return DecompilerTypeCache.GetOrAdd(typeDef.FullName, _ => DecompileTypeAux(typeDef));

        }

        private static EntityDeclaration DecompileMethodAux(MethodDefinition method)
        {
            AstBuilder builder = new AstBuilder(GetDecompilerContext(method));
            builder.AddMethod(method);
            builder.RunTransformations();
            return builder.CompilationUnit.FirstChild as EntityDeclaration;
        }

        private static NamespaceDeclaration DecompileTypeAux(TypeDefinition typeDef)
        {
            AstBuilder builder = new AstBuilder(GetDecompilerContext(typeDef));
            builder.AddType(typeDef);
            builder.RunTransformations();
            return builder.CompilationUnit.FirstChild as NamespaceDeclaration;
        }

        public static void Visit(this MethodDefinition method, IAstVisitor visitor)
        {
            method.DecompileMethodBody().AcceptVisitor(visitor);
        }

        public static T Visit<T>(this MethodDefinition method, IAstVisitor<T> visitor)
        {
            return method.DecompileMethodBody().AcceptVisitor(visitor);
        }

        public static S Visit<T, S>(this MethodDefinition method, IAstVisitor<T, S> visitor, T data)
        {
            return method.DecompileMethodBody().AcceptVisitor(visitor, data);
        }

        public static ElementSummary GetSummary(this AssemblyFileReference assembly, MethodDefinition method, AstNode target, AstNode codeBlock = null)
        {
            ElementSummary summary = assembly.GetSummary();

            if (method.HasBody && method.Body.Instructions[0].SequencePoint != null)
            {
                summary.WspRelativeFilename = method.Body.Instructions[0].SequencePoint.Document.Url;
            }

            /* It is required to upgrade ICSharpCode library, because now we have that problem: https://github.com/icsharpcode/NRefactory/issues/172 */
            summary.LineNumber = target.StartLocation.Line;
            summary.LinePosition = target.StartLocation.Column;

            codeBlock = GetCodeBlock(method, target, codeBlock);

            if (codeBlock == null)
            {
                CodeInstruction instr = new CodeInstruction(method, method.Body.Instructions[0]);
                return instr.ImproveSummary(summary);
            }

            string code;
            summary.SourceCodeLocation = GetSourceLocation(method, codeBlock, out code);
            summary.SourceCodeLineNumber = GetSourceCodeLine(target, code);

            return summary;
        }

        static readonly ConcurrentDictionary<string, string> SourceFileMap = new ConcurrentDictionary<string, string>();

        private static string GetSourceLocation(MethodDefinition method, AstNode codeBlock, out string text)
        {
            if (codeBlock is EntityDeclaration)
            {
                string text1 = null;
                string fileName = SourceFileMap.GetOrAdd(method.FullName, _ => GetSourceLocationAux(codeBlock,out text1));
                text = text1 ?? File.ReadAllText(fileName);
                return fileName;
            }
            else
            {
                return GetSourceLocationAux(codeBlock, out text);
            }
        }

        private static string GetSourceLocationAux(AstNode codeBlock, out string text)
        {
            string fileName = Path.GetTempFileName();
            text = codeBlock.GetText();
            File.WriteAllText(fileName, text);
            return fileName;
        }

        private static int GetSourceCodeLine(AstNode target, string code)
        {
            string highlightCode = target.GetText();
            // TODO: does not work. Always return -1. Please fix.
            int highlightPosition = code.IndexOf(highlightCode, StringComparison.Ordinal);
            int line = 0;
            int position = -1;
            do
            {
                position = code.IndexOf("\n", position + 1, StringComparison.Ordinal);
                line++;
            } while (position != -1 && position < highlightPosition);
            return line;
        }

        private static AstNode GetCodeBlock(MethodDefinition method, AstNode target, AstNode codeBlock)
        {
            if (codeBlock == null)
            {
                BlockStatement nearestBlock = target.AncestorsAndSelf.OfType<BlockStatement>().First();
                BlockStatement block;
                if (!DecompilerMethodBodyCache.TryGetValue(method.FullName, out block))
                {
                    return null;
                }

                if (block != nearestBlock)
                {
                    codeBlock = nearestBlock.Parent;
                }
                else
                {
                    EntityDeclaration decompiledMethod;
                    if (DecompilerMethodCache.TryGetValue(method.FullName, out decompiledMethod))
                    {
                        codeBlock = decompiledMethod;
                    }
                    else
                    {
                        codeBlock = block;
                    }
                }

            }
            
            return codeBlock;
        }
    }
}
