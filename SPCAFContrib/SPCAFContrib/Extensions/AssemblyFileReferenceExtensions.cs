using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Mono.Cecil;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Common;
using SPCAFContrib.Entities.Consts;

namespace SPCAFContrib.Extensions
{
    public static class AssemblyFileReferenceExtensions
    {
        #region methods

        public static void OnMethodMatch(this AssemblyFileReference assembly,
                                         MultiValueDictionary<string, string> typeMethodMap,
                                         Action<CodeInstruction> onMethodMatch)
        {
            if (onMethodMatch == null) return;

            foreach (string typeName in typeMethodMap.Keys)
            {
                foreach (string methodName in typeMethodMap[typeName])
                {
                    foreach (CodeInstruction methodCall in assembly.MethodInvocationInstructions(typeName, methodName).OrderBy(m => m.MethodDefinition.Name))
                    {
                        if (onMethodMatch != null)
                            onMethodMatch(methodCall);
                    }
                }
            }
        }

        public static void OnPropertyMatch(this AssemblyFileReference assembly,
                                         MultiValueDictionary<string, string> typePropertyMap,
                                         Action<CodeInstruction> onPropertyMatch)
        {
            if (onPropertyMatch == null) return;

            foreach (string typeName in typePropertyMap.Keys)
            {
                foreach (string propertyName in typePropertyMap[typeName])
                {
                    foreach (CodeInstruction propertySet in assembly.PropertySettings(typeName, propertyName).OrderBy(m => m.MethodDefinition.Name))
                    {
                        if (onPropertyMatch != null)
                            onPropertyMatch(propertySet);
                    }
                }
            }
        }

        public static void OnPropertyUsageMatch(this AssemblyFileReference assembly,
                                         MultiValueDictionary<string, string> typePropertyMap,
                                         Action<CodeInstruction> onPropertyMatch)
        {
            if (onPropertyMatch == null) return;

            foreach (string typeName in typePropertyMap.Keys)
            {
                foreach (string propertyName in typePropertyMap[typeName])
                {
                    foreach (CodeInstruction propertySet in assembly.PropertyUsages(typeName, propertyName).OrderBy(m => m.MethodDefinition.Name))
                    {
                        if (onPropertyMatch != null)
                            onPropertyMatch(propertySet);
                    }
                }
            }
        }

        public static bool AssemblyHasExcluded(this AssemblyFileReference assembly)
        {
            return ExcludedItems.Assemblies.Any(s => new Wildcard(s, RegexOptions.IgnoreCase).IsMatch(assembly.ReadableElementName));
        }

        public static bool AssemblyHasExcluded(this AssemblyDefinition assembly)
        {
            return ExcludedItems.Assemblies.Any(s => new Wildcard(s, RegexOptions.IgnoreCase).IsMatch(assembly.MainModule.Name));
        }

        public static IEnumerable<TypeDefinition> ResolveAllowedWebControls(this AssemblyFileReference assembly)
        {
            foreach (TypeDefinition type in assembly.ResolveTypesByBaseTypes(TypeInfo.WebControlAndPages))
            {
                yield return type;
            }

            foreach (TypeDefinition type in assembly.TypesThatImplementInterface(TypeKeys.IHttpHandler))
            {
                yield return type;
            }

            foreach (TypeDefinition type in assembly.TypesThatImplementInterface(TypeKeys.IHttpModule))
            {
                yield return type;
            }
        }

        public static IEnumerable<TypeDefinition> ResolveOutOfHttpContextTypes(this AssemblyFileReference assembly)
        {
            foreach (TypeDefinition type in assembly.ResolveTypesByBaseTypes(TypeInfo.SPTimerJobs))
            {
                yield return type;
            }

            foreach (TypeDefinition type in assembly.ResolveTypesByBaseType(TypeKeys.SPFeatureReceiver))
            {
                yield return type;
            }
            
            foreach (TypeDefinition type in assembly.ResolveTypesByBaseTypes(TypeInfo.SPEventReceivers))
            {
                // need only async methods check like ItemAdded
                if ((type.IsDerivedFromType(TypeKeys.SPItemEventReceiver) ||
                     type.IsDerivedFromType(TypeKeys.SPListEventReceiver) ||
                     type.IsDerivedFromType(TypeKeys.SPWebEventReceiver)) && type.HasMethods)
                {
                    if (
                        !type.Methods.Any(
                            method =>
                                TypeInfo.SPItemEventReceiverAsynchronousEvents.Contains(method.Name) ||
                                TypeInfo.SPListEventReceiverAsynchronousEvents.Contains(method.Name) ||
                                TypeInfo.SPWebEventReceiverAsynchronousEvents.Contains(method.Name)))
                        continue;
                }
                yield return type;
            }

            foreach (TypeDefinition type in assembly.ResolveTypesByBaseTypes(TypeInfo.SPWFActivities))
            {
                yield return type;
            }
        }
        #endregion
    }
}
