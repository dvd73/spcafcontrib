using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Experimental.Rules.Xml
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
      CheckId = CheckIDs.Rules.ListInstance.AvoidDataRowsInListInstances,
      Help = CheckIDs.Rules.ListInstance.AvoidDataRowsInListInstances_HelpUrl,

      Message = "Avoid Rows in the List instances.",
      DisplayName = "AvoidDataRowsInListInstances",
      Description = "AvoidDataRowsInListInstances",
      Resolution = "",

      DefaultSeverity = Severity.Warning,
      SharePointVersion = new[] { "12", "14", "15" },

      Links = new[]
       {
          "http://msdn.microsoft.com/en-us/library/office/ms478860.aspx"
       }
      )]
    public class AvoidDataRowsInListInstances : Rule<ListInstanceDefinition>
    {
        #region methods

        public override void Visit(ListInstanceDefinition target, NotificationCollection notifications)
        {
            DataDefinition dataDefinition = target.Item as DataDefinition;

            if (dataDefinition != null && dataDefinition.Rows != null && dataDefinition.Rows.Count() > 0)
            {
                Notify(target,
                       string.Format(this.MessageTemplate()),
                       notifications);
            }
        }

        #endregion
    }
}
