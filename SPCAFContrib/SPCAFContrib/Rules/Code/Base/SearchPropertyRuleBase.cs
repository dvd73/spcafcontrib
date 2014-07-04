using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Rules.Code.Base
{
    public abstract class SearchPropertyRuleBase : SearchMethodRuleBase
    {
        #region methods

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            EnsureTypeMap();

            assembly.OnPropertyMatch(TargetTypeMap, (_instruction) =>
            {
                this.OnMatch(assembly, _instruction, notifications, GetNotificationMessage, () =>
                {
                    return GetSummary(assembly, _instruction);
                });
            });
        }
        #endregion
    }
}
