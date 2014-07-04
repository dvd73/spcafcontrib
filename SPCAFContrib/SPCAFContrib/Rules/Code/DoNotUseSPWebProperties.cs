using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAFContrib.Consts;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Rules.Code.Base;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.DoNotUseSPWebProperties,
     Help = CheckIDs.Rules.Assembly.DoNotUseSPWebProperties_HelpUrl,
     DisplayName = "Do not use SPWeb.Properties collection.",
     Description = "SPWeb.Properties is a StringDictionary, and doesn’t support casing for keys/values (everything gets converted to fully lowercase).",
     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "14", "15" },
     Message = "Do not use SPWeb.Properties collection. Method: [{0}], Class:[{1}]",
     Links = new[]
     {
         "SPWeb.AllProperties property",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.spweb.allproperties.aspx"
     },
     Resolution = "The recommendation is to use the SPWeb.AllProperties collection.")]
    public class DoNotUseSPWebProperties : SearchPropertyRuleBase
    {
        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.SPWeb, "Properties");
        }

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            EnsureTypeMap();

            assembly.OnPropertyUsageMatch(TargetTypeMap, (_instruction) =>
            {
                Mono.Cecil.MethodDefinition method = _instruction.MethodDefinition;
                this.OnMatch(assembly, _instruction, notifications, 
                    () =>
                {
                    return String.Format(this.MessageTemplate(), method.Name, method.DeclaringType.FullName);
                }, 
                    () =>
                {
                    return GetSummary(assembly, _instruction);
                });
            });
        }
    }
}
