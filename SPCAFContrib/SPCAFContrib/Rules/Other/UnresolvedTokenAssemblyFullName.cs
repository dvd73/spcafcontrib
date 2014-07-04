using System;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;

namespace SPCAFContrib.Rules.Other
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.TemplateFiles.UnresolvedTokenAssemblyFullName,
     Help = CheckIDs.Rules.TemplateFiles.UnresolvedTokenAssemblyFullName_HelpUrl,

     DisplayName = "Unresolved token $SharePoint.Project.AssemblyFullName$.",
     Message = "Unresolved token $SharePoint.Project.AssemblyFullName$ in file [{0}].",
     Description = "Token $SharePoint.Project.AssemblyFullName$ have to be replaced to strong name of the containing project’s output assembly. For Visual Studio project it is handled automatically depends on file extension.",
     Resolution = "Add required extension to the either .csproj project file or Microsoft.VisualStudio.SharePoint.targets file.",

     DefaultSeverity = Severity.Error,
     SharePointVersion = new[] { "12", "14", "15" },
     Links = new[]
     {
         "Replaceable Parameters",
         "http://msdn.microsoft.com/en-us/library/ee231545.aspx"
     }
     )]
    public class UnresolvedTokenAssemblyFullName : Rule<SolutionDefinition>
    {
        public override void Visit(SolutionDefinition target, NotificationCollection notifications)
        {
            foreach (InternalSourceFile internalSourceFile in target.AllFiles)
            {
                int lineNumber = 0;
                foreach (string fileContentLine in internalSourceFile.FileContentLines)
                {
                    int linePosition = fileContentLine.ToLower().IndexOf("sharePoint.project.assemblyfullname", StringComparison.OrdinalIgnoreCase);
                        
                    if (linePosition >=0)
                    {
                        Notify(target, String.Format(this.MessageTemplate(), internalSourceFile.FileName),
                            internalSourceFile.GetSummaryWithLineInfo(lineNumber, linePosition),
                            notifications);
                    }

                    lineNumber++;
                }
            }
        }
    }
}
