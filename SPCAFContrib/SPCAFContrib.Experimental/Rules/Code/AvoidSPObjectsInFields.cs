using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Experimental.Rules.Code
{
     [RuleMetadata(typeof(ContribBestPracticesGroup),
     CheckId = CheckIDs.Rules.Assembly.AvoidSPObjectsInFields,
     Help = CheckIDs.Rules.Assembly.AvoidSPObjectsInFields_HelpUrl,
     DisplayName = "Avoid using SP-Objects In Fields and Properties",
     Description = "Having SP-Objects as a properties/fields are quite dangerous",
     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" },
     Message = "Avoid using SP-Objects In Fields and Properties In Class: {0}",
     Resolution = "Pass SP-Objects as a methods parameters most of the time")]
    public class AvoidSPObjectsInFields : Rule<AssemblyFileReference>
    {
        #region fields

        private static readonly string[] _unsafeObjects = { "SPWeb", "SPSIte", "SPLimitedWebPartManager", "UserProfileManager", "PublishingWeb" };

        protected static List<string> RestrictedSharePointClasses = new List<string>
        {
                "Microsoft.SharePoint.SPSite",
                "Microsoft.SharePoint.SPWeb",
                "Microsoft.SharePoint.SPList"
        };

        protected static List<string> TargetUIElementClasses = new List<string>
        {
                "System.Web.UI.UserControl",
                "System.Web.UI.Page",
                "System.Web.UI.WebControls.WebParts.WebPart"
        };

        #endregion

        #region methods

         /// <summary>
        /// There are 2 implementation as result of code refactoring. Second implementation is commented out (CheckSPFeatureReceivers).
         /// </summary>
         /// <param name="assembly"></param>
         /// <param name="notifications"></param>
        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            //CheckSPFeatureReceivers(assembly, notifications);

            IEnumerable<TypeDefinition> types = assembly.AllTypeDefinitions().Where(z=>z.IsClass && z.BaseType!=null);

            foreach (TypeDefinition type in types)
            {
                foreach (Mono.Cecil.FieldDefinition field in type.Fields)
                {
                    if(!IsSafeType(field.FieldType) && !field.IsPublic)
                        Notify(assembly, string.Format(this.MessageTemplate(), field.DeclaringType.Name), field.ImproveSummary(assembly.GetSummary()), notifications);
                }

                foreach (Mono.Cecil.PropertyDefinition property in type.Properties)
                {
                    if (!IsSafeType(property.PropertyType) && ((property.GetMethod != null && !property.GetMethod.IsPublic) || (property.SetMethod != null && !property.SetMethod.IsPublic)))
                        Notify(assembly, string.Format(this.MessageTemplate(), property.DeclaringType.Name), property.ImproveSummary(assembly.GetSummary()), notifications);
                }
            }
        }

        private bool IsSafeType(TypeReference type)
        {
            return !_unsafeObjects.Contains(type.Name);
        }

        private static bool IsRestrictedType(string type)
        {
            return RestrictedSharePointClasses.Any(t => type.IndexOf(t, StringComparison.CurrentCultureIgnoreCase) >= 0);
        }

        private static string GetRestrictedSharePointClassesString()
        {
            return RestrictedSharePointClasses.Aggregate((a, b) => string.Format("[{0}], [{1}]", a, b));
        }

        private static string GetTargetUIElementClassesString()
        {
            return TargetUIElementClasses.Aggregate((a, b) => string.Format("[{0}], [{1}]", a, b));
        }

        private void CheckSPFeatureReceivers(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            IEnumerable<TypeDefinition> uiElementTypes = TargetUIElementClasses.SelectMany(assembly.TypesThatDerivesFromType);

            foreach (TypeDefinition userControl in uiElementTypes)
            {
                foreach (Mono.Cecil.FieldDefinition field in userControl.Fields)
                {
                    if (IsRestrictedType(field.FieldType.FullName))
                    {
                        Notify(assembly,
                           string.Format("Avoid introducing SharePoint objects {2} as class fields for the following classes {3}. Field: [{0}] Type:[{1}]",
                                field.Name,
                                field.FieldType.FullName,
                                GetRestrictedSharePointClassesString(),
                                GetTargetUIElementClassesString()),
                           field.ImproveSummary(assembly.GetSummary()),
                           notifications);
                    }
                }

                foreach (Mono.Cecil.PropertyDefinition property in userControl.Properties)
                {
                    if (IsRestrictedType(property.DeclaringType.FullName))
                    {
                        Notify(assembly,
                           string.Format("Avoid introducing SharePoint objects {2} as class properties for the following classes {3}. Propety: [{0}] Type:[{1}]",
                                property.Name,
                                property.DeclaringType.FullName,
                                GetRestrictedSharePointClassesString(),
                                GetTargetUIElementClassesString()),
                           property.ImproveSummary(assembly.GetSummary()),
                           notifications);
                    }
                }
            }
        }

        #endregion
    }
}
