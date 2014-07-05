using System;
using System.Linq;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using SPCAF.Sdk;
using SPCAF.Sdk.Inventory;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using Mono.Cecil;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Inventory
{
    [InventoryMetadata(typeof(ContribInventoryGroup),
        CheckId = CheckIDs.Inventory.Assembly.ListOfConsts,
        Help = CheckIDs.Inventory.Assembly.ListOfConsts_HelpUrl,

        Message = "Added constant declaration instruction [{0}] to inventory",
        DisplayName = "Constants",
        Description = "Provides information all the \"consts\" in the solutions.",
        
        SharePointVersion = new string[] { "12", "14", "15" })]
    public class ListOfConsts : Inventory<AssemblyFileReference>
    {
        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            string[] inventoryFields = new string[] { "Assembly", "Class", "Name", "Type", "Value" };

            foreach (TypeDefinition typeDefinition in assembly.AllTypeDefinitions().Where(td => !td.IsEnum).OrderBy(td => td.FullName))
            {
                typeDefinition.SearchConstStrings(field =>
                {
                    string constValue = field.Constant != null ? field.Constant.ToString() : String.Empty;

                    string[] inventoryValues = new string[] { assembly.AssemblyName, typeDefinition.FullName, field.Name, field.FieldType.FullName, constValue };
                    string message = String.Format(this.MessageTemplate(), constValue);

                    Notify(assembly, inventoryFields, inventoryValues, message, notifications);
                });
            }
        }
    }
}
