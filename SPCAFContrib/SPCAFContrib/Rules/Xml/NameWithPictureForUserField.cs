using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;

namespace SPCAFContrib.Rules.Xml
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.FieldTemplate.NameWithPictureForUserField,
     Help = CheckIDs.Rules.FieldTemplate.NameWithPictureForUserField_HelpUrl,

     DisplayName = "ShowField attribute value NameWithPicture is not recommended.",
     Message = "Definition of field '{0}'. NameWithPicture attribute value is not recommended.",
     Description = "Looks like NameWithPicture value is no longer available via sharePoint GUI. It might mean that this value is depricated.",
     Resolution = "Replace attribute value with NameWithPictureAndDetails",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "15" },

     Links = new []
     {
         "Field Element (Field)",
         "http://msdn.microsoft.com/en-us/library/office/aa979575.aspx"
     }
     )]
    public class NameWithPictureForUserField : Rule<FieldDefinition>
    {
        public override void Visit(FieldDefinition target, NotificationCollection notifications)
        {
            if (target.Type.Equals("User", StringComparison.OrdinalIgnoreCase) && !String.IsNullOrEmpty(target.ShowField))
            {
                if (target.ShowField.Equals("NameWithPicture", StringComparison.OrdinalIgnoreCase))
                    Notify(target, String.Format(this.MessageTemplate(), target.Name), notifications);
            }
        }
    }
}
