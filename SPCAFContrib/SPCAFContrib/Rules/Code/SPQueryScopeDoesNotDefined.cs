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
     CheckId = CheckIDs.Rules.Assembly.SPQueryScopeDoesNotDefined,
     Help = CheckIDs.Rules.Assembly.SPQueryScopeDoesNotDefined_HelpUrl,

     Message = "SPQuery scope is not defined.",
     DisplayName = "SPQuery scope is not defined.",
     Description = "All SPViewScope enumeration values are covered all possible developer's intentions. If not specified SharePoint will use Default value.",
     Resolution = "Specify ViewAttributes property for SPQuery object.",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" },
     Links = new[]
     {
         "SPViewScope enumeration",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.spviewscope.aspx",
         "SPQuery.ViewAttributes property",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.spquery.viewattributes.aspx"
     }
     )]
    public class SPQueryScopeDoesNotDefined : SearchMethodRuleBase
    {
        private class SPQueryScopeVisitor : DepthFirstAstVisitor
        {
            public bool AllIsOK{
                get { return SPQuery_variables.Count == 0; }
            }

            List<string> SPQuery_variables = new List<string>(); 

            public override void VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement)
            {
                if (variableDeclarationStatement.Type is ICSharpCode.NRefactory.CSharp.SimpleType && 
                    ((variableDeclarationStatement.Type as ICSharpCode.NRefactory.CSharp.SimpleType).Identifier.Contains("SPQuery")) &&
                    variableDeclarationStatement.Variables.Count > 0)
                {
                    SPQuery_variables.AddRange(variableDeclarationStatement.Variables.Select(v => v.Name).ToArray());
                }

                // Call the base method to traverse into nested invocations
                base.VisitVariableDeclarationStatement(variableDeclarationStatement);
            }

            public override void VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression)
            {
                if (memberReferenceExpression.MemberName == "set_ViewAttributes")
                {
                    SPQuery_variables.RemoveAll(
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
            TargetTypeMap.Add(TypeKeys.SPQuery, new List<string>() { ".ctor" });
        }

        protected override void OnMatch(AssemblyFileReference assembly, CodeInstruction instruction, NotificationCollection notifications,
             Func<string> getNotificationMessage, Func<ElementSummary> getSummary)
        {
            Mono.Cecil.MethodDefinition method = instruction.MethodDefinition;
            if (method.AllMethodReferencesTo(TypeKeys.SPQuery, "set_ViewAttributes").Count() == 0)
            {
                base.OnMatch(assembly, instruction, notifications, GetNotificationMessage, () =>
                {
                    return GetSummary(assembly, instruction);
                });
            }
            else
            {
                SPQueryScopeVisitor sPQueryScopeVisitor = new SPQueryScopeVisitor();
                method.Visit(sPQueryScopeVisitor);

                if (!sPQueryScopeVisitor.AllIsOK)
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
