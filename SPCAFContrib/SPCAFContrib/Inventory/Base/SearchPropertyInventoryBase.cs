using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Inventory;
using SPCAF.Sdk.Model;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Inventory.Base
{
    public abstract class SearchPropertyInventoryBase : SearchMethodInventoryBase
    {
        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            EnsureTypeMap();

            assembly.OnPropertyMatch(TargetTypeMap, (_methodInstruction) =>
            {
                this.OnMatch(assembly, _methodInstruction, notifications);
            });
        }
    }
}
