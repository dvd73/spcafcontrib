using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Groups;
using SPCAFContrib.Rules.Code.Base;
using Severity = SPCAF.Sdk.Severity;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.SPViewScopeDoesNotDefined,
     Help = CheckIDs.Rules.Assembly.SPViewScopeDoesNotDefined_HelpUrl,

     Message = "SPView scope is not defined.",
     DisplayName = "SPView scope is not defined.",
     Description = "All SPViewScope enumeration values are covered all possible developer's intentions. If not specified SharePoint will use Default value.",
     Resolution = "Specify Scope property for SPView object.",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" },
     Links = new[]
     {
         "SPViewScope enumeration",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.spviewscope.aspx",
         "SPView.Scope property",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.spview.scope.aspx"
     }
     )]
    public class SPViewScopeDoesNotDefined : SearchMethodRuleBase
    {
        private class SPViewScopeVisitor : DepthFirstAstVisitor
        {
            public bool AllIsOK{
                get { return SPView_variables.Count == 0; }
            }

            List<string> SPView_variables = new List<string>(); 

            public override void VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement)
            {
                if (variableDeclarationStatement.Type is ICSharpCode.NRefactory.CSharp.SimpleType && 
                    ((variableDeclarationStatement.Type as ICSharpCode.NRefactory.CSharp.SimpleType).Identifier.Contains("SPView")) &&
                    variableDeclarationStatement.Variables.Count > 0)
                {
                    SPView_variables.AddRange(variableDeclarationStatement.Variables.Select(v => v.Name).ToArray());
                }

                // Call the base method to traverse into nested invocations
                base.VisitVariableDeclarationStatement(variableDeclarationStatement);
            }

            public override void VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression)
            {
                if (memberReferenceExpression.MemberName == "set_Scope")
                {
                    SPView_variables.RemoveAll(
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
            TargetTypeMap.Add(TypeKeys.SPView, new List<string>() { ".ctor", "Clone" });
            TargetTypeMap.Add(TypeKeys.SPViewCollection, new List<string>() { "Add" });
        }

        protected override void OnMatch(AssemblyFileReference assembly, CodeInstruction instruction, NotificationCollection notifications,
             Func<string> getNotificationMessage, Func<ElementSummary> getSummary)
        {
            Mono.Cecil.MethodDefinition method = instruction.MethodDefinition;
            //var t = method.DecompileMethod().GetText();
            if (method.AllMethodReferencesTo(TypeKeys.SPView, "set_Scope").Count() == 0)
            {
                base.OnMatch(assembly, instruction, notifications, GetNotificationMessage, () =>
                {
                    return GetSummary(assembly, instruction);
                });
            }
            else
            {
                SPViewScopeVisitor SPViewScopeVisitor = new SPViewScopeVisitor();
                method.Visit(SPViewScopeVisitor);

                if (!SPViewScopeVisitor.AllIsOK)
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
