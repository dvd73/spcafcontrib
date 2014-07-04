using System;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Inventory;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Common;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Inventory.Base
{
    public abstract class SearchMethodInventoryBase : Inventory<AssemblyFileReference>
    {
        #region properties

        protected MultiValueDictionary<string, string> TargetTypeMap { get; set; }
        protected string[] InventoryFields { get; set; }

        #endregion

        #region api

        protected abstract void PopulateTypeMap();

        protected abstract void GetInventoryData(AssemblyFileReference assembly, CodeInstruction instruction,
            ref string[] inventoryValues, ref string message);

        
        protected virtual void OnMatch(AssemblyFileReference assembly,
                                             CodeInstruction instruction, NotificationCollection notifications)
        {
            string[] inventoryValues = new string[]{};
            string message = String.Empty;

            GetInventoryData(assembly, instruction, ref inventoryValues, ref message);

            if (InventoryFields.Length > 0 && inventoryValues.Length > 0)
                Notify(assembly, InventoryFields, inventoryValues, message, notifications);
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
                this.OnMatch(assembly, _instruction, notifications);
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
