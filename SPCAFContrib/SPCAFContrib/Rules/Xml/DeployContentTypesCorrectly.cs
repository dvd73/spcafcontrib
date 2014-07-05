using SPCAF.Sdk;
using SPCAF.Sdk.Rules;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Rules.Xml
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.ContentType.DeployContentTypesCorrectly,
     Help = CheckIDs.Rules.ContentType.DeployContentTypesCorrectly_HelpUrl,

     Message = "Deploy Content Types correctly.",
     DisplayName = "Deploy Content Types correctly.",
     Description = "Checks Content Type definitions.",
     Resolution = "Do not deploy Content Type with Overwrite=\"TRUE\" and Inherits=\"False\" or not specified.",

     DefaultSeverity = Severity.Error,
     SharePointVersion = new[] { "14", "15" },
     Links = new []
     {
         "ContentType Element (ContentType)",
         "http://msdn.microsoft.com/en-us/library/office/aa544268.aspx"
     }
     )]
    public class DeployContentTypesCorrectly : Rule<ContentTypeDefinition>
    {
        #region methods

        public override void Visit(ContentTypeDefinition target, NotificationCollection notifications)
        {
            if (target.GetXPath().StartsWith("/ns:List/ns:MetaData"))
            {
                //Skip content types in List Schema
                return;
            }

            if (target.OverwriteSpecified && target.Overwrite.IsTrue() && (!target.InheritsSpecified || target.Inherits.IsFalse()))
            {
                string message = string.Format("Do not deploy Content Type [{0}] with Overwrite=\"TRUE\" and Inherits=\"False\" or not specified.", target.Name);
                Notify(target, message, notifications);
            }
        }

        #endregion
    }
}
