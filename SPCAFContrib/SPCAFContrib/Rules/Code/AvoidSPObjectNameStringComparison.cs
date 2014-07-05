using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Groups;
using SPCAFContrib.Rules.Code.Base;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.AvoidSPObjectNameStringComparison,
     Help = CheckIDs.Rules.Assembly.AvoidSPObjectNameStringComparison_HelpUrl,

     Message = "Avoid {0}.Name == <string> comparison. Method: [{1}], Class:[{2}]",
     DisplayName = "Avoid SPObject.Name == <string> comparison.",
     Description = "Depending on the case, SPObject.Name string-based comparison is quite unsafe and might lead to the potential issues.",
     Resolution = "Use SPObject.ID instead or other non-string key property.",

     DefaultSeverity = Severity.Information,
     SharePointVersion = new[] { "12", "14", "15" }
     )]
    public class AvoidSPObjectNameStringComparison : SearchMethodRuleBase
    {
        private string[] _types =
        {
            TypeKeys.SPPersistedObject,
            TypeKeys.SPContentType,
            TypeKeys.PageLayout,
            TypeKeys.SPListItem,
            TypeKeys.TaxonomyItem,
            TypeKeys.SPWeb,
            TypeKeys.SPPrincipal
        };

        #region methods

        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.SystemString,
                new List<string> { "Equals", "Compare", "CompareTo", "op_Equality", "op_Inequality" });
        }

        protected override void OnMatch(AssemblyFileReference assembly, CodeInstruction instruction, NotificationCollection notifications, Func<string> getNotificationMessage, Func<ElementSummary> getSummary)
        {
            Instruction comporationInstruction = instruction.Instruction;
            MethodReference methodReference = comporationInstruction.Operand as MethodReference;

            if (methodReference != null && comporationInstruction.Previous != null)
            {
                int numberOfParams = methodReference.Parameters.Count;
                int i = 1;
                Instruction prevInstruction = comporationInstruction.Previous;

                do
                {
                    MethodReference m_reference = prevInstruction.Operand as MethodReference;
                    if (IsRequiredType(m_reference))
                    {
                        base.OnMatch(assembly, instruction, notifications, 
                            ()=>
                        {
                            return String.Format(this.MessageTemplate(), m_reference.DeclaringType.Name, m_reference.Name, m_reference.DeclaringType.FullName);
                        }, () =>
                        {
                            return GetSummary(assembly, instruction);
                        });
                        break;
                    }
                    i++;
                    prevInstruction = prevInstruction.Previous;
                } while (i <= numberOfParams);
            }
        }

        private bool IsRequiredType(MethodReference m_reference)
        {
            if (m_reference == null || m_reference.Name != "get_Name")
                return false;

            return _types.Any(t => String.Equals(m_reference.DeclaringType.FullName, t, StringComparison.OrdinalIgnoreCase));
        }
        
        #endregion
    }
}
