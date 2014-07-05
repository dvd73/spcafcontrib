using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using Mono.Cecil;
using Mono.Cecil.Cil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Collectors;
using SPCAFContrib.Common;
using SPCAFContrib.Common.Statistics;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Groups;
using SPCAFContrib.Rules.Code.Base;
using MethodDefinition = Mono.Cecil.MethodDefinition;
using Severity = SPCAF.Sdk.Severity;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
     CheckId = CheckIDs.Rules.Assembly.ULSLoggingInCatchBlock,
     Help = CheckIDs.Rules.Assembly.ULSLoggingInCatchBlock_HelpUrl,

     Message = "Catch block should include ULS logging output or re-throw in Method [{0}]  Class [{1})].",
     DisplayName = "Not logged exception found.",
     Description = "Catch block should include ULS logging output or re-throw",
     Resolution = "Log exception to ULS.",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" },
     Links = new []
     {
         "SPDiagnosticsService class",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.administration.spdiagnosticsservice.aspx"
     })]
    public class ULSLoggingInCatchBlock : SearchMethodRuleBase
    {
        private CatchVisitorData catchVisitorData;
        
        struct CatchVisitorData
        {
            public IProjectContent Project { get; set; }
            public MethodDefinition Method { get; set; }
            public bool UseSPDiagnosticsService { get; set; }
        }

        private class NoLoggingCatchExceptionVisitor : BooleanOrVisitor<Action<AstNode>>
        {
            private CheckCatchVisitor checkCatchVisitor;
            public NoLoggingCatchExceptionVisitor(CatchVisitorData data)
            {
                checkCatchVisitor = new CheckCatchVisitor(data);
            }

            public override bool VisitCatchClause(CatchClause catchClause, Action<AstNode> report)
            {
                if (!catchClause.Body.AcceptVisitor(checkCatchVisitor.Check(catchClause.VariableName)))
                {
                    report(catchClause);
                    return true;
                }
                // Call the base method to traverse into nested invocations
                return base.VisitCatchClause(catchClause, report);
            }
        }

        private class CheckCatchVisitor : BooleanOrVisitor
        {
            private CompilationUnit syntaxTree;
            private string _variableName;
            private CatchVisitorData validationData;

            public CheckCatchVisitor(CatchVisitorData data)
            {
                this.validationData = data;

                if (validationData.Project != null)
                {
                    NamespaceDeclaration FirstChild = validationData.Method.DeclaringType.DecompileType();
                    if (FirstChild != null)
                    {
                        //syntaxTree = FirstChild.Parent as CompilationUnit;
                        //syntaxTree.FileName = validationData.Method.Name + ".cs";
                        ICSharpCode.NRefactory.CSharp.CSharpParser parser = new ICSharpCode.NRefactory.CSharp.CSharpParser();
                        syntaxTree = parser.Parse(FirstChild.Parent.GetText(), validationData.Method.Name + ".cs");
                    }
                }
            }

            public CheckCatchVisitor Check(string variableName)
            {
                this._variableName = variableName;
                return this;
            }

            public override bool VisitInvocationExpression(InvocationExpression invocationExpression)
            {
                IEnumerable<MethodDefinition> annotations = invocationExpression.Annotations.OfType<MethodDefinition>();
                bool hasCustomLoggerCall = annotations.Any(a => IsCustomLoggerCall(a));
                bool hasSPDiagnosticsServiceCall = false;

                if (!hasCustomLoggerCall && validationData.UseSPDiagnosticsService)
                {
                    hasSPDiagnosticsServiceCall = annotations.Any(a => IsSPDiagnosticsServiceCall(a));
                }

                // revolve c# code to get semantic
                if (syntaxTree != null)
                {
                    CSharpParsedFile unresolvedTypes = null;
                    try
                    {
                        unresolvedTypes = syntaxTree.ToTypeSystem();
                        if (unresolvedTypes != null)
                        {
                            ICompilation compilation = validationData.Project.CreateCompilation();
                            CSharpAstResolver resolver = new CSharpAstResolver(compilation, syntaxTree, unresolvedTypes);
                            InvocationResolveResult resolveResult = resolver.Resolve(invocationExpression) as InvocationResolveResult;
                            if (resolveResult != null)
                            {
                                //ToDo: something to find required invocation
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                if (hasCustomLoggerCall || hasSPDiagnosticsServiceCall)
                    return true;
                else
                    return base.VisitInvocationExpression(invocationExpression);
            }

            public override bool VisitThrowStatement(ThrowStatement throwStatement)
            {
                return true;
            }   
            
            private bool IsCustomLoggerCall(MethodDefinition methodDefinition)
            {
                MultiValueDictionary<string, string> MethodSubCalls = new MultiValueDictionary<string, string>();
                HashSet<string> handledMethods = new HashSet<string>();
                methodDefinition.InsideMethodsCalls(MethodSubCalls, handledMethods, true, _ => { });

                return LoggingStatistic.Instance.CustomTypes.Any(ct => MethodSubCalls.ContainsKey(ct));
            }

            private bool IsSPDiagnosticsServiceCall(MethodDefinition methodDefinition)
            {
                MultiValueDictionary<string, string> MethodSubCalls = new MultiValueDictionary<string, string>();
                HashSet<string> handledMethods = new HashSet<string>();
                methodDefinition.InsideMethodsCalls(MethodSubCalls, handledMethods, false, _ => { });

                return MethodSubCalls.ContainsKey(TypeKeys.SPDiagnosticsService);
            }
        }

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            SolutionDefinition solution = assembly.ParentSolution as SolutionDefinition;

            bool hasSharePointReference = assembly.AssemblyReferencesSharePointAssembly();
            if (!hasSharePointReference) return;

            if (!LoggingStatistic.Instance.Collected)
            {
                new LoggingCollector().Visit(solution);
            }

            if (LoggingStatistic.Instance.Collected)
            {
                catchVisitorData = new CatchVisitorData();
                catchVisitorData.UseSPDiagnosticsService = assembly.UsesType(TypeKeys.SPDiagnosticsService);

                // revolve c# code to get semantic
                //CecilLoader loader = new CecilLoader();
                //catchVisitorData.Project= new CSharpProjectContent();
                //catchVisitorData.Project = catchVisitorData.Project.AddAssemblyReferences(StatisticBase.ResolvedLibs.Select(s => loader.LoadAssembly(s)));
                //catchVisitorData.Project = catchVisitorData.Project.AddAssemblyReferences(StatisticBase.SolutionLibs.Select(s => loader.LoadAssembly(s)));

                IEnumerable<MethodDefinition> methodsWithTryCatch = from m in assembly.AllMethodDefinitions()
                                          where m.HasBody
                                          where m.Body.HasExceptionHandlers
                                          select m;

                foreach (MethodDefinition method in methodsWithTryCatch)
                {
                    HandleMethod(assembly, method, notifications);
                }
            }
        }

        protected virtual ElementSummary GetAstNodeSummary(AssemblyFileReference assembly, MethodDefinition method, AstNode node)
        {
            Instruction instr = null;
            IEnumerable<ExceptionHandler> t = method.Body.ExceptionHandlers.Where(e => e.HandlerType == ExceptionHandlerType.Catch);
            if (t.Count() == 1)
            {
                instr = t.First().HandlerStart;
            }
            else
            {
                instr = method.Body.Instructions[0];
            }

            CodeInstruction instruction = new CodeInstruction(method, instr);
            return instruction.ImproveSummary(assembly.GetSummary());
        }

        protected virtual void HandleMethod(AssemblyFileReference assembly, MethodDefinition method, NotificationCollection notifications)
        {
            catchVisitorData.Method = method;
            method.Visit(GetAstVisitor(), catchNode =>
            {
                this.OnMatch(method, notifications,
                    () =>
                    {
                        return string.Format(this.MessageTemplate(),
                            method.Name,
                            method.DeclaringType.FullName);
                    }, () =>
                    {
                        return GetAstNodeSummary(assembly, method, catchNode);
                    });
            });
        }

        protected virtual BooleanOrVisitor<Action<AstNode>> GetAstVisitor()
        {
            return new NoLoggingCatchExceptionVisitor(catchVisitorData);
        }

        protected override void PopulateTypeMap()
        {
            throw new NotImplementedException();
        }
    }
}
