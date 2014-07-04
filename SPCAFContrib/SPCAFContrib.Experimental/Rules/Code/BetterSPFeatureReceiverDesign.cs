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
using FieldDefinition = Mono.Cecil.FieldDefinition;
using MethodDefinition = Mono.Cecil.MethodDefinition;
using PropertyDefinition = Mono.Cecil.PropertyDefinition;

namespace SPCAFContrib.Experimental.Rules.Code
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
      CheckId = CheckIDs.Rules.Assembly.BetterSPFeatureReceiverDesign,
      DisplayName = "SPFeatureReceiver class design might be improved.",
      Description = "Consider improvements of SPFeatureReceiver class implementation following recommended practices.",
      DefaultSeverity = Severity.CriticalWarning,
      SharePointVersion = new[] { "12", "14", "15" },
      Message = "SPFeatureReceiver class design might be improved.",
      Resolution = "Improve SPFeatureReceiver design following recommended practices.")]
    public class BetterSPFeatureReceiverDesign : Rule<AssemblyFileReference>
    {
        #region properties

        protected const int MaxMethodLength = 50;

        protected List<string> AllowedSPFeatureReceiverMethods = new List<string>
            {
             "FeatureActivated",
             "FeatureDeactivating",
             "FeatureInstalled",
             "FeatureUninstalling",
             "FeatureUpgrading",
             ".ctor"
        };

        #endregion

        #region methods

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            CheckSPFeatureReceivers(assembly, notifications);
        }

        private void CheckSPFeatureReceivers(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            IEnumerable<TypeDefinition> featureReceivers = assembly.TypesThatDerivesFromType("Microsoft.SharePoint.SPFeatureReceiver");

            foreach (TypeDefinition receiver in featureReceivers)
            {
                CheckFields(assembly, notifications, receiver);
                CheckProperties(assembly, notifications, receiver);
                CheckMethods(assembly, notifications, receiver);
            }
        }

        private void CheckFields(AssemblyFileReference assembly, NotificationCollection notifications, TypeDefinition type)
        {
            foreach (FieldDefinition field in type.Fields)
            {
                Notify(assembly,
                       string.Format("Avoid introducing new fields for SPFeatureReceiver class. Field: [{0}]", field.Name),
                       field.ImproveSummary(assembly.GetSummary()),
                       notifications);
            }
        }

        private void CheckProperties(AssemblyFileReference assembly, NotificationCollection notifications, TypeDefinition type)
        {
            foreach (PropertyDefinition field in type.Properties)
            {
                Notify(assembly,
                       string.Format("Avoid introducing new properties for SPFeatureReceiver class. Property: [{0}]", field.Name),
                       field.ImproveSummary(assembly.GetSummary()),
                       notifications);
            }
        }

        private void CheckMethods(AssemblyFileReference assembly, NotificationCollection notifications, TypeDefinition type)
        {
            foreach (MethodDefinition method in type.Methods)
            {
                // skip get_/set_ signatures as they are covered by properties
                if (method.Name.StartsWith("get_", StringComparison.CurrentCultureIgnoreCase) ||
                    method.Name.StartsWith("set_", StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }

                // check total method size
                // it's different to one you see in the Visual Studio, so tried to implement several good-looling methods and it comes to 45-50
                if (method.IsOversized(MaxMethodLength))
                {
                    Notify(assembly,
                        string.Format("Avoid introducing long methods for SPFeatureReceiver classes. Method: [{0}]", method.Name),
                        method.ImproveSummary(assembly.GetSummary()),
                        notifications);
                }

                // finally, check to additional methods which better be removed outside of SPFeatureReceiver
                if (!AllowedSPFeatureReceiverMethods.Contains(method.Name, StringComparer.CurrentCultureIgnoreCase))
                {
                    Notify(assembly,
                        string.Format("Avoid introducing new methods for SPFeatureReceiver classes. Method: [{0}]", method.Name),
                        method.ImproveSummary(assembly.GetSummary()),
                        notifications);
                }
            }
        }

        #endregion
    }
}
