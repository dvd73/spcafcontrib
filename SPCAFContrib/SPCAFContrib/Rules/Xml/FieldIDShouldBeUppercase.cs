using System;
using SPCAF.Sdk;
using SPCAF.Sdk.Rules;
using SPCAF.Sdk.Model;
using SPCAFContrib.Consts;

namespace SPCAFContrib.Rules.Xml
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.FieldTemplate.FieldIdShouldBeUppercase,
     Help = CheckIDs.Rules.FieldTemplate.FieldIdShouldBeUppercase_HelpUrl,

     DisplayName = "Field ID attribute must be upper-case.",
     Message = "Definition of field '{0}'. ID attribute must be in capital letters.",
     Description = "List scoped field MUST HAVE \"ID\" (not \"Id\") attribute.",
     Resolution = "Ensure ID attribute (not \"Id\").",

     DefaultSeverity = Severity.Error,
     SharePointVersion = new[] { "12", "14", "15" },

     Links = new[]
     {
         "Field Element (Field)",
         "http://msdn.microsoft.com/en-us/library/office/aa979575.aspx"   
     }
     )]
    public class FieldIdShouldBeUppercase : Rule<FieldDefinition>
    {
        #region methods

        public override void Visit(FieldDefinition target, NotificationCollection notifications)
        {
            if (target.Id != null)
            {
                Notify(target, String.Format(this.MessageTemplate(), target.Name), notifications);
            }
        }

        #endregion
    }
}
