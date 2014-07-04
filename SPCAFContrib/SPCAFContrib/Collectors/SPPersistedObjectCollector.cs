using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Collectors.Base;
using SPCAFContrib.Common;
using SPCAFContrib.Common.Statistics;
using SPCAFContrib.Common.Statistics.Base;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Collectors
{
    public class SPPersistedObjectCollector : CollectorBase<SolutionDefinition>
    {
        public override StatisticBase Visit(SolutionDefinition solution) 
        {
            lock (syncRoot)
            {
                SPPersistedObjectStatistic result = base.Visit(solution) as SPPersistedObjectStatistic;

                if (!result.Collected)
                {
                    List<TypeDefinition> customTypes = new List<TypeDefinition>();
                    result.AllowedRecivers = new List<string>();
                    result.AllowedReciverCalls = new MultiValueDictionary<string, string>();

                    foreach (AssemblyFileReference assembly in solution.Assemblies)
                    {
                        if (assembly.AssemblyDefinition == null)
                            assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);
                        
                        if (assembly.AssemblyHasExcluded()) continue;

                        customTypes.AddRange(assembly.ResolveTypesByBaseTypes(new[] { TypeKeys.SPPersistedObject, TypeKeys.SPJobDefinition }));
                    }
                    
                    foreach (FeatureManifestReference featureManifestReference in solution.FeatureManifests)
                    {
                        if ((featureManifestReference.FeatureDefinition.Scope == FeatureScope.WebApplication ||
                             featureManifestReference.FeatureDefinition.Scope == FeatureScope.Farm) &&
                            !String.IsNullOrEmpty(featureManifestReference.FeatureDefinition.ReceiverClass))
                        {
                            result.AllowedRecivers.Add(featureManifestReference.FeatureDefinition.ReceiverClass);

                            AssemblyFileReference assembly =
                                solution.Assemblies.FirstOrDefault(
                                    a =>
                                        a.AssemblyFullName ==
                                        featureManifestReference.FeatureDefinition.ReceiverAssembly);
                            if (assembly != null)
                                FillAllowedReciverCalls(assembly, featureManifestReference.FeatureDefinition.ReceiverClass, result.AllowedReciverCalls);

                            AssemblyDefinition ad =
                                result.ResolvedLibs.FirstOrDefault(
                                    a => a.FullName == featureManifestReference.FeatureDefinition.ReceiverAssembly);
                            if (ad != null)
                            {
                                assembly = new AssemblyFileReference() {AssemblyDefinition = ad};
                                FillAllowedReciverCalls(assembly, featureManifestReference.FeatureDefinition.ReceiverClass, result.AllowedReciverCalls);
                            }
                        }
                    }

                    foreach (AssemblyDefinition resolvedLib in result.ResolvedLibs)
                    {
                        if (resolvedLib.AssemblyHasExcluded()) continue;

                        AssemblyFileReference assembly = new AssemblyFileReference() { AssemblyDefinition = resolvedLib };

                        IEnumerable<TypeDefinition> typesThatUseSPPersistedObject = assembly.ResolveTypesByBaseTypes(new[] { TypeKeys.SPPersistedObject, TypeKeys.SPJobDefinition });
                        customTypes.AddRange(typesThatUseSPPersistedObject);
                        foreach (TypeDefinition customType in typesThatUseSPPersistedObject)
                        {
                            customTypes.AddRange(
                                assembly.MethodsUsingType(customType.FullName).Select(md => md.DeclaringType).ToList());
                        }
                    }
                    
                    //remove duplicates from collection
                    result.CustomTypes = customTypes.GroupBy(td => td.FullName).Select(g => g.First().FullName).ToList();
                    result.SetCollected(true);
                }

                return result;
            }
        }

        private static void FillAllowedReciverCalls(AssemblyFileReference assembly, string receiverClass, MultiValueDictionary<string, string> list)
        {
            // get receiver method
            Mono.Cecil.MethodDefinition methodDefinition = assembly.AllMethodDefinitions()
                .FirstOrDefault(
                    md => md.DeclaringType.FullName == receiverClass);

            if (methodDefinition != null)
            {
                HashSet<string> handledMethods = new HashSet<string>();
                methodDefinition.InsideMethodsCalls(list, handledMethods, true, null);
            }
        }

        protected override StatisticBase GetStatisticObject()
        {
            return SPPersistedObjectStatistic.Instance;
        }
    }
}
