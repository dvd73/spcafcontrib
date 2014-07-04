using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Rules.Code.Base;
using Severity = SPCAF.Sdk.Severity;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.SPDataSourceScopeDoesNotDefined,
     Help = CheckIDs.Rules.Assembly.SPDataSourceScopeDoesNotDefined_HelpUrl,

     DisplayName = "SPDataSource scope is not defined.",
     Message = "SPDataSource scope is not defined.",
     Description = "All SPViewScope enumeration values are covered all possible developer's intentions. If not specified SharePoint will use Default value.",
     Resolution = "Specify Scope property for SPDataSource object.",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" },
     Links = new[]
     {
         "SPViewScope enumeration",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.spviewscope.aspx",
         "SPDataSource.Scope property",
         "http://msdn.microsoft.com/EN-US/library/microsoft.sharepoint.webcontrols.spdatasource.scope.aspx",
     }
     )]
    public class SPDataSourceScopeDoesNotDefined : SearchMethodRuleBase
    {
        private class SPDataSourceScopeVisitor : DepthFirstAstVisitor
        {
            public bool AllIsOK{
                get { return SPDataSource_variables.Count == 0; }
            }

            List<string> SPDataSource_variables = new List<string>(); 

            public override void VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement)
            {
                if (variableDeclarationStatement.Type is ICSharpCode.NRefactory.CSharp.SimpleType && 
                    ((variableDeclarationStatement.Type as ICSharpCode.NRefactory.CSharp.SimpleType).Identifier.Contains("SPDataSource")) &&
                    variableDeclarationStatement.Variables.Count > 0)
                {
                    SPDataSource_variables.AddRange(variableDeclarationStatement.Variables.Select(v => v.Name).ToArray());
                }

                // Call the base method to traverse into nested invocations
                base.VisitVariableDeclarationStatement(variableDeclarationStatement);
            }

            public override void VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression)
            {
                if (memberReferenceExpression.MemberName == "set_Scope")
                {
                    SPDataSource_variables.RemoveAll(
                        s =>
                            s ==
                            (memberReferenceExpression.Target as ICSharpCode.NRefactory.CSharp.IdentifierExpression)
                                .Identifier);
                }
                base.VisitMemberReferenceExpression(memberReferenceExpression);
            }
        }

        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.SPDataSource, new List<string>() { ".ctor" });
        }

        protected override void OnMatch(AssemblyFileReference assembly, CodeInstruction instruction, NotificationCollection notifications,
             Func<string> getNotificationMessage, Func<ElementSummary> getSummary)
        {
            Mono.Cecil.MethodDefinition method = instruction.MethodDefinition;
            if (method.AllMethodReferencesTo(TypeKeys.SPDataSource, "set_Scope").Count() == 0)
            {
                base.OnMatch(assembly, instruction, notifications, GetNotificationMessage, () =>
                {
                    return GetSummary(assembly, instruction);
                });
            }
            else
            {
                SPDataSourceScopeVisitor SPDataSourceScopeVisitor = new SPDataSourceScopeVisitor();
                method.Visit(SPDataSourceScopeVisitor);

                if (!SPDataSourceScopeVisitor.AllIsOK)
                {
                    base.OnMatch(assembly, instruction, notifications, GetNotificationMessage, () =>
                    {
                        return GetSummary(assembly, instruction);
                    });
                }
            }
        }
        
    }
}
