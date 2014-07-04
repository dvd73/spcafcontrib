using System;
using Mono.Cecil;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAF.Sdk.Rules;
using SPCAF.Sdk;
using SPCAFContrib.Common;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Rules.Code.Base
{
    public abstract class SearchMethodRuleBase : Rule<AssemblyFileReference>
    {
        #region properties

        protected MultiValueDictionary<string, string> TargetTypeMap { get; set; }

        #endregion

        #region api

        protected abstract void PopulateTypeMap();

        protected virtual void OnMatch(AssemblyFileReference assembly,
            CodeInstruction instruction, NotificationCollection notifications,
            Func<string> getNotificationMessage,
            Func<ElementSummary> getSummary)
        {
            Notify(assembly, getNotificationMessage(), getSummary(), notifications);
        }

        protected virtual void OnMatch(Mono.Cecil.MethodDefinition method,
            NotificationCollection notifications,
            Func<string> getNotificationMessage,
            Func<ElementSummary> getSummary)
        {
            Notify(method, getNotificationMessage(), getSummary(), notifications);
        }

        protected virtual string GetNotificationMessage()
        {
            return this.MessageTemplate();
        }

        protected virtual ElementSummary GetSummary(AssemblyFileReference assembly, CodeInstruction instruction)
        {
            return instruction.ImproveSummary(assembly.GetSummary());
        }

        #endregion

        #region methods

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            EnsureTypeMap();

            assembly.OnMethodMatch(TargetTypeMap, (_instruction) =>
            {
                this.OnMatch(assembly, _instruction, notifications, GetNotificationMessage, ()=>
                {
                    return GetSummary(assembly, _instruction);
                });
            });
        }

        protected virtual void EnsureTypeMap()
        {
            if (TargetTypeMap != null) return;

            TargetTypeMap = new MultiValueDictionary<string, string>();
            PopulateTypeMap();
        }

        #endregion
    }
}
