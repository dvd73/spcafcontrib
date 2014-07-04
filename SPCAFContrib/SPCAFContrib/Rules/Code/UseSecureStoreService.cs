using System;
using System.Collections.Generic;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Consts;
using SPCAFContrib.Rules.Code.Base;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
     CheckId = CheckIDs.Rules.Assembly.UseSecureStoreService,
     Help = CheckIDs.Rules.Assembly.UseSecureStoreService_HelpUrl,
     DisplayName = "Consider Secure Store Service instead of direct db connection.",
     Description = "At some point, it is better to use Secure Store Provider to store credentials/connection strins intead of saving them into web.config, property bags or lists.",
     DefaultSeverity = Severity.Information,
     SharePointVersion = new[] { "12", "14", "15" },
     Message = "Consider Secure Store Service instead of direct connection.",
     Resolution = "Conside Security Store Service as a safe storage for the credentials and connection strings.",
     Links = new []
     {
         "How to use Secure Store Id for Impersonation programmatically",
         "http://www.projectserver2010blog.com/2012/02/how-to-use-secure-store-id-for.html",
         "Read Credentials from Secure Store Service Programatically in SharePoint 2010",
         "http://saiabhilash.blogspot.ru/2011/12/read-credentials-from-secure-store.html",
         "How to: Use Secure Store Service to Connect to an External System",
         "http://msdn.microsoft.com/en-us/library/office/ee554863.aspx"
     })]
    public class UseSecureStoreService : SearchMethodRuleBase
    {
        private bool hasSecureStoreService = false;

        #region methods

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            SolutionDefinition solution = assembly.ParentSolution as SolutionDefinition;
            if (solution != null)
                hasSecureStoreService = assembly.ReferencesAssembly("Microsoft.Office.SecureStoreService");
            
            base.Visit(assembly, notifications);
        }

        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.DataContext, new List<string>{
                    ".ctor"
                });
            TargetTypeMap.Add(TypeKeys.EFDataContext, new List<string>{
                    ".ctor"
                });
            TargetTypeMap.Add(TypeKeys.SqlConnection, new List<string>{
                    ".ctor"
                });
            TargetTypeMap.Add(TypeKeys.OleDbConnection, new List<string>{
                    ".ctor"
                });
            TargetTypeMap.Add(TypeKeys.OdbcConnection, new List<string>{
                    ".ctor"
                });
            TargetTypeMap.Add(TypeKeys.OracleConnection, new List<string>{
                    ".ctor"
                });
        }

        protected override void OnMatch(AssemblyFileReference assembly, CodeInstruction instruction,
            NotificationCollection notifications, Func<string> getNotificationMessage, Func<ElementSummary> getSummary)
        {
            if (!hasSecureStoreService && !instruction.MethodDefinition.HasSPDatabase())
            {
                if (!String.IsNullOrEmpty(instruction.Document) &&
                    !instruction.Document.Contains("designer.cs") &&
                    !instruction.Document.Contains("Context.cs"))
                {
                    base.OnMatch(assembly, instruction, notifications, GetNotificationMessage, () =>
                    {
                        return GetSummary(assembly, instruction);
                    });
                }
            }
        }
        #endregion
    }
}
