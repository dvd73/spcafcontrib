using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Metrics.Code.Base
{
    public abstract class SearchPropertyMetricBase : SearchMethodMetricBase
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
                this.OnMatch(assembly, _instruction);
            });

            Notify(assembly, Number, notifications, x => x.AssemblyName);
        }
        #endregion
    }
}
