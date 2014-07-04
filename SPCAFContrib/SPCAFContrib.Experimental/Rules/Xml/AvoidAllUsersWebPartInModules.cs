using System;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;

namespace SPCAFContrib.Experimental.Rules.Xml
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
      CheckId = CheckIDs.Rules.Module.AvoidAllUsersWebPartInModules,
      DisplayName = "Avoid AllUsersWebPart in modules",
      Message = "Avoid AllUsersWebPart definitions in the module [{0}].",
      Description = "AvoidAllUsersWebPartInModules",
      DefaultSeverity = Severity.Warning,
      SharePointVersion = new[] { "12", "14", "15" },
      Resolution = "")]
    public class AvoidAllUsersWebPartInModules : Rule<WebPartDefinition>
    {
        #region methods

        public override void Visit(WebPartDefinition target, NotificationCollection notifications)
        {
            if (target.Parent != null)
            {
                Notify(target,
                    string.Format(this.MessageTemplate(), target.Parent.Parent.ReadableElementName),
                    target.GetSummary(String.Empty), notifications);
            }
        }

        #endregion
    }
}
