using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Rules.Code.Base;
using MethodDefinition = Mono.Cecil.MethodDefinition;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.DoNotDisposePersonalSiteWeb,
     Help = CheckIDs.Rules.Assembly.DoNotDisposePersonalSiteWeb_HelpUrl,

     DisplayName = "Do not dispose PersonalSite or PersonalWeb.",
     Message = "Do not dispose PersonalSite or PersonalWeb.",
     Description = "My Site pages, which implement IPersonalPage, implement these properties as shared instances that should not be disposed of by controls that use them.",
     Resolution = "",

     DefaultSeverity = SPCAF.Sdk.Severity.Error,
     SharePointVersion = new[] { "12", "14", "15" },
     Links = new[]
     {
         "IPersonalPage.PersonalSite property",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.portal.webcontrols.ipersonalpage.personalsite.aspx",
         "IPersonalPage.PersonalWeb property",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.portal.webcontrols.ipersonalpage.personalweb.aspx"
     })]
    public class DoNotDisposePersonalSiteWeb : SearchPropertyRuleBase
    {
        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.IPersonalPage, new List<string>() { "PersonalSite", "PersonalWeb" });
        }

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            EnsureTypeMap();

            assembly.OnPropertyUsageMatch(TargetTypeMap, (_instruction) =>
            {
                MethodDefinition method = _instruction.MethodDefinition;
                FindSPWebDisposeVisitor findSPWebDisposeVisitor = new FindSPWebDisposeVisitor(_instruction);
                method.Visit(findSPWebDisposeVisitor);
                
                if (!findSPWebDisposeVisitor.AllIsOK)
                {
                    this.OnMatch(assembly, _instruction, notifications, GetNotificationMessage, () =>
                    {
                        return GetSummary(assembly, _instruction);
                    });
                }
            });
        }

        public class FindSPWebDisposeVisitor : DepthFirstAstVisitor
        {
            public bool AllIsOK
            {
                get { return SPWebDispose_variables.Count == 0; }
            }

            private MethodReference methodReference = null;
            private List<string> SPSiteWeb_variables = new List<string>();
            private List<string> SPWebDispose_variables = new List<string>(); 

            public FindSPWebDisposeVisitor(CodeInstruction _instruction)
            {
                methodReference = _instruction.Instruction.Operand as MethodReference;
            }

            public override void VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement)
            {
                if (variableDeclarationStatement.Type is SimpleType)
                {
                    SimpleType variableDeclaration = variableDeclarationStatement.Type as SimpleType;

                    if (variableDeclaration.Identifier.Contains(methodReference.ReturnType.Name) && variableDeclarationStatement.Variables.Count > 0)
                    {
                        SPSiteWeb_variables.AddRange(variableDeclarationStatement.Variables.Select(v => v.Name).ToArray());
                    }
                }
                // Call the base method to traverse into nested invocations
                base.VisitVariableDeclarationStatement(variableDeclarationStatement);
            }

            public override void VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression)
            {
                if (memberReferenceExpression.MemberName == "Dispose")
                {
                    string v = SPSiteWeb_variables.Where(
                        s =>
                            s ==
                            (memberReferenceExpression.Target as IdentifierExpression).Identifier).FirstOrDefault();

                    if (!String.IsNullOrEmpty(v))
                        SPWebDispose_variables.Add(v);
                }
                base.VisitMemberReferenceExpression(memberReferenceExpression);
            }

            public override void VisitExpressionStatement(ExpressionStatement expressionStatement)
            {
                string s = expressionStatement.Expression.GetText();
                if (s.Contains("get_PersonalWeb ().Dispose ()") || s.Contains("get_PersonalSite ().Dispose ()"))
                {
                    SPWebDispose_variables.Add(s);
                }
                base.VisitExpressionStatement(expressionStatement);
            }
        }
    }
}
