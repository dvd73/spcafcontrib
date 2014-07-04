using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Metrics;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Common;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Metrics.Code.Base
{
    public abstract class SearchMethodMetricBase : Metric<AssemblyFileReference>
    {
        #region properties

        protected MultiValueDictionary<string, string> TargetTypeMap { get; set; }
        protected int Number { get; set; }

        #endregion

        #region api

        protected abstract void PopulateTypeMap();

        protected virtual void OnMatch(AssemblyFileReference assembly,
                                             CodeInstruction instruction)
        {
            Number++;
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
                this.OnMatch(assembly, _instruction);
            });

            if (Number > 0)
                Notify(assembly, Number, notifications, x => x.AssemblyName);
            Number = 0;
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
