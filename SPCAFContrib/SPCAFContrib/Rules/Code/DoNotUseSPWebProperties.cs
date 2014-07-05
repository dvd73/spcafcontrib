using System;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Groups;
using SPCAFContrib.Rules.Code.Base;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.DoNotUseSPWebProperties,
     Help = CheckIDs.Rules.Assembly.DoNotUseSPWebProperties_HelpUrl,

     Message = "Do not use SPWeb.Properties collection. Method: [{0}], Class:[{1}]",
     DisplayName = "Do not use SPWeb.Properties collection.",
     Description = "SPWeb.Properties is a StringDictionary, and doesn’t support casing for keys/values (everything gets converted to fully lowercase).",
     Resolution = "The recommendation is to use the SPWeb.AllProperties collection.",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "14", "15" },
     Links = new[]
     {
         "SPWeb.AllProperties property",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.spweb.allproperties.aspx"
     }
     )]
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
