using SPCAF.Sdk;
using SPCAF.Sdk.Rules;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Rules.Xml
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.ContentType.ConsiderOverwriteAttributeForContentType,
     Help = CheckIDs.Rules.ContentType.ConsiderOverwriteAttributeForContentType_HelpUrl,

     Message = "Consider Overwrite=\"TRUE\" for content type [{0}] ({1}).",
     DisplayName = "Consider Overwrite=\"TRUE\" for content types.",
     Description = "Content types with Overwrite=\"TRUE\" are deployed directly to Content Database and not subject to ghosting issues.",
     Resolution = "Add Overwrite=\"TRUE\" attribute.",

     DefaultSeverity = Severity.CriticalWarning,
     SharePointVersion = new[] { "14", "15" },
     Links = new[]
     {
         "ContentType Element (ContentType)",
         "http://msdn.microsoft.com/en-us/library/office/aa544268.aspx"
     }
     )]
    public class ConsiderOverwriteAttributeForContentType : Rule<ContentTypeDefinition>
    {
        #region methods

        public override void Visit(ContentTypeDefinition target, NotificationCollection notifications)
        {
            if (target.GetXPath().StartsWith("/ns:List/ns:MetaData"))
            {
                //Skip content types in List Schema
                return;
            }

            if (!target.OverwriteSpecified || target.Overwrite.IsFalse())
            {
                        string message = string.Format(this.MessageTemplate(), target.Name, target.ID);
                        Notify(target, message, notifications);
            }
        }

        #endregion
    }
}
