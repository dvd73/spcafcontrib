using System;
using System.Linq;
using SPCAF.Sdk;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;
using SPCAF.Sdk.Model;

namespace SPCAFContrib.Rules.Xml
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.FieldTemplate.ConsiderOverwriteAttributeForFields,
     Help = CheckIDs.Rules.FieldTemplate.ConsiderOverwriteAttributeForFields_HelpUrl,

     DisplayName = "Consider Overwrite=\"TRUE\" for fields.",
     Message = "Consider Overwrite=\"TRUE\" for field '{0}'.",
     Description = "Fields with Overwrite=\"TRUE\" are deployed directly to Content Database and not subject to ghosting issues.",
     Resolution = "Add Overwrite=\"TRUE\" attribute.",

     DefaultSeverity = Severity.CriticalWarning,
     SharePointVersion = new[] { "14", "15" },

     Links = new []
     {
         "Field Element (Field)",
         "http://msdn.microsoft.com/en-us/library/office/aa979575.aspx"
     }
     )]
    public class ConsiderOverwriteAttributeForFields : Rule<FieldDefinition>
    {
        #region methods

        public override void Visit(FieldDefinition target, NotificationCollection notifications)
        {
            if (target.GetXPath().StartsWith("/ns:List/ns:MetaData"))
            {
                //Skip fields in List Schema
                return;
            }

            string overwrite = "FALSE"; 
            if(target.AnyAttr != null)
            {
                overwrite = (from attr in target.AnyAttr
                             where attr.Name == "Overwrite"
                             select attr.Value).FirstOrDefault() ?? overwrite;
            }

            if(overwrite != "TRUE")
            {
                string message = string.Format(this.MessageTemplate(), target.Name);
                Notify(target, message, notifications);
            }
        }

        #endregion
    }
}
