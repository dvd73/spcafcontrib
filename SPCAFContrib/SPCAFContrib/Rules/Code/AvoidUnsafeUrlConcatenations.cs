using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using ICSharpCode.NRefactory.CSharp;
using SPCAFContrib.Groups;
using MethodDefinition = Mono.Cecil.MethodDefinition;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.AvoidUnsafeUrlConcatenations,
     Help = CheckIDs.Rules.Assembly.AvoidUnsafeUrlConcatenations_HelpUrl,

     Message = "Avoid unsafe Url concatinations. Method: [{0}], Class:[{1}]. Use SPUtility.ConcatUrls/SPUrlUtility.CombineUrl methods instead.",
     DisplayName = "Avoid unsafe url concatenations.",
     Description = "Url property for SPSite, SPWeb and SPFolder may return string with or without triling slash.",
     Resolution = "Use SPUtility.ConcatUrls/SPUrlUtility.CombineUrl methods instead.",

     DefaultSeverity = SPCAF.Sdk.Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" },
     Links = new []
     {
         "SPUtility.ConcatUrls method",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.utilities.sputility.concaturls.aspx",
         "SPUrlUtility.CombineUrl method",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.utilities.spurlutility.combineurl.aspx"
     })]
    public class AvoidUnsafeUrlConcatenations : Rule<AssemblyFileReference>
    {
        private static readonly HashSet<string> UrlProperties = new HashSet<string>() {
                "System.String Microsoft.SharePoint.SPWeb::get_Url()",
                "System.String Microsoft.SharePoint.SPWeb::get_ServerRelativeUrl()",
                "System.String Microsoft.SharePoint.SPSite::get_Url()",
                "System.String Microsoft.SharePoint.SPSite::get_ServerRelativeUrl()",
                "System.String Microsoft.SharePoint.SPFolder::get_Url()",
                "System.String Microsoft.SharePoint.SPFolder::get_ServerRelativeUrl()"
        };
        private class FindConcatWithUrlPropertyVisitor : BooleanOrVisitor<Action<AstNode>>
        {

            public override bool VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, Action<AstNode> report)
            {
                if (binaryOperatorExpression.Operator == BinaryOperatorType.Add)
                {
                    InvocationExpression left = binaryOperatorExpression.Left as InvocationExpression;
                    if (left != null)
                    {
                        MethodReference methodReference = left.Annotation<MethodReference>();
                        if (methodReference != null)
                        {
                            if (UrlProperties.Contains(methodReference.FullName))
                            {
                                report(left);
                            }
                        }
                    }
                }
                return base.VisitBinaryOperatorExpression(binaryOperatorExpression, report);
            }
        }
        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            IEnumerable<MethodDefinition> methods = from method in assembly.AllMethodDefinitions()
                          where UrlProperties.Any(p => method.AllMethodReferencesTo(p).Any())
                          select method;
            FindConcatWithUrlPropertyVisitor visitor = new FindConcatWithUrlPropertyVisitor();
            foreach (MethodDefinition method in methods)
            {
                method.Visit(visitor, node =>
                {
                    ElementSummary summary = assembly.GetSummary(method, node);

                    if (summary.LineNumber == 0 && node is InvocationExpression)
                    {
                        MemberReference mr = node.Annotations.OfType<MemberReference>().FirstOrDefault();
                        if (mr != null)
                        {
                            summary = method.ImproveSummary(summary);
                            CodeInstruction codeInstruction = method.AllMethodReferencesTo(mr.FullName).FirstOrDefault();
                            if (codeInstruction != null)
                                summary = codeInstruction.ImproveSummary(summary);
                        }
                        else
                        {
                            var codeInstruction = new CodeInstruction(method, method.Body.Instructions[0]);
                            summary = codeInstruction.ImproveSummary(summary);
                        }
                    }
                    
                    this.Notify(method,
                        string.Format(
                            this.MessageTemplate(),
                            method.Name,
                            method.DeclaringType.FullName),
                        summary,
                        notifications);
                });
            }
        }

    }
}
