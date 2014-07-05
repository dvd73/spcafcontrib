using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;
using Mono.Cecil.Cil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using System;
using SPCAFContrib.Groups;
using SPCAFContrib.Rules.Code.Base;
using MethodDefinition = Mono.Cecil.MethodDefinition;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.DoNotSuppressExceptions,
     Help = CheckIDs.Rules.Assembly.DoNotSuppressExceptions_HelpUrl,

     Message = "Do not suppress general exceptions. Method: [{0}], Class:[{1}]",
     DisplayName = "Do not suppress general exceptions.",
     Description = "Rethrow exception in catch(Exception) block using throw or catch specific exception.",
     Resolution = "Rethrow exception in catch(Exception) block using throw or catch specific exception.",

     DefaultSeverity = SPCAF.Sdk.Severity.Error,
     SharePointVersion = new[] { "12", "14", "15" }
     )]
    public class DoNotSuppressExceptions : Rule<AssemblyFileReference>
    {
        private class EmptyCatchExceptionVisitor : BooleanOrVisitor<Action<AstNode>>
        {
            public override bool VisitCatchClause(CatchClause catchClause, Action<AstNode> report)
            {
                if (((catchClause.Type.IsNull) ||
                    ((catchClause.Type is SimpleType)
                    && ((catchClause.Type as SimpleType).Identifier == "Exception")))
                    && !CheckCatchBlock.Check(catchClause.Body, catchClause.VariableName))
                {
                    report(catchClause);
                    return true;
                }
                return false;
            }
        }

        private class CheckCatchBlock : BooleanOrVisitor
        {
            string variableName;
            public CheckCatchBlock(string variableName)
            {
                this.variableName = variableName;
            }

            public override bool VisitThrowStatement(ThrowStatement throwStatement)
            {
                return throwStatement.Expression.IsNull;
            }

            public override bool VisitCatchClause(CatchClause catchClause)
            {
                return false;
            }

            public static bool Check(AstNode node, string variableName)
            {
                return node.AcceptVisitor(new CheckCatchBlock(variableName));
            }
        }

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            IEnumerable<MethodDefinition> methodsWithTryCatch = from m in assembly.AllMethodDefinitions()
                                      where m.HasBody
                                      where m.Body.HasExceptionHandlers
                                      select m;

            foreach (MethodDefinition method in methodsWithTryCatch)
            {
                method.Visit(new EmptyCatchExceptionVisitor(),
                    catchNode =>
                    {
                        ElementSummary summary = assembly.GetSummary(method, catchNode, catchNode.Parent);

                        if (summary.LineNumber == 0 && catchNode is CatchClause)
                        {
                            Instruction instr = null;
                            if (
                                method.Body.ExceptionHandlers.Where(e => e.HandlerType == ExceptionHandlerType.Catch)
                                    .Count() == 1)
                            {
                                instr =
                                    method.Body.ExceptionHandlers.Where(
                                        e => e.HandlerType == ExceptionHandlerType.Catch)
                                        .First()
                                        .HandlerStart;
                            }
                            else
                            {
                                instr = method.Body.Instructions[0];
                            }

                            CodeInstruction instruction = new CodeInstruction(method, instr);
                            summary = instruction.ImproveSummary(assembly.GetSummary());
                        }

                        Notify(method, String.Format(this.MessageTemplate(), method.Name, method.DeclaringType.FullName),
                            summary, notifications);
                    });
            }
        }
    }
}
