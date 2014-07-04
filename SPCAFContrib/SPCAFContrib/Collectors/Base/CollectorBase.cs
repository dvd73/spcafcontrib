using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Common.Statistics.Base;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Collectors.Base
{
    public class CollectorBase<T> where T : SolutionDefinition
    {
        protected static object syncRoot = new object();

        public virtual StatisticBase Visit(T target)
        {
            lock (syncRoot)
            {
                StatisticBase result = GetStatisticObject();

                if (!result.Collected)
                {
                    List<AssemblyDefinition> localLibs = new List<AssemblyDefinition>();
                    List<string> referencedLibs = new List<string>();
                    DefaultAssemblyResolver assemblyResolver = new DefaultAssemblyResolver();
                    List<AssemblyDefinition> assemblyDefinitions = new List<AssemblyDefinition>();

                    foreach (AssemblyFileReference assembly in target.Assemblies)
                    {
                        if (assembly.AssemblyHasExcluded()) continue;

                        if (assembly.AssemblyDefinition == null)
                            assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

                        localLibs.Add(assembly.AssemblyDefinition);
                        referencedLibs.AddRange(assembly.ReferencedAssemblies().Select(a => a.FullName));
                    }

                    result.UnresolvedLibs = new List<string>();
                    referencedLibs = referencedLibs.Distinct().ToList();

                    foreach (string referencedLib in referencedLibs)
                    {
                        try
                        {
                            assemblyDefinitions.Add(assemblyResolver.Resolve(referencedLib));
                        }
                        catch
                        {
                            result.UnresolvedLibs.Add(referencedLib);
                        }
                    }

                    result.SolutionLibs = localLibs;
                    result.ResolvedLibs = assemblyDefinitions;

                    result.Collected = true;
                }

                return result;
            }
        }

        protected virtual StatisticBase GetStatisticObject()
        {
            return StatisticBase.Instance;
        }
    }
}
