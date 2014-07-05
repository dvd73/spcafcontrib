using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Collectors.Base;
using SPCAFContrib.Common.Statistics;
using SPCAFContrib.Common.Statistics.Base;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Collectors
{
    public class LoggingCollector : CollectorBase<SolutionDefinition>
    {
        public override StatisticBase Visit(SolutionDefinition solution) 
        {
            lock (syncRoot)
            {
                LoggingStatistic result = base.Visit(solution) as LoggingStatistic;

                if (!result.Collected)
                {
                    List<TypeDefinition> customTypes = new List<TypeDefinition>();

                    foreach (AssemblyFileReference assembly in solution.Assemblies)
                    {
                        if (assembly.AssemblyDefinition == null)
                            assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);
                        
                        if (assembly.AssemblyHasExcluded()) continue;
                        
                        customTypes.AddRange(assembly.ResolveTypesByBaseTypes(new[] {TypeKeys.SPDiagnosticsServiceBase}));
                    }
                    
                    foreach (AssemblyDefinition resolvedLib in result.ResolvedLibs)
                    {
                        if (resolvedLib.AssemblyHasExcluded()) continue;

                        AssemblyFileReference assembly = new AssemblyFileReference() { AssemblyDefinition = resolvedLib };

                        IEnumerable<TypeDefinition> typesThatUseLogger = assembly.ResolveTypesByBaseTypes(new[] {TypeKeys.SPDiagnosticsServiceBase});
                        customTypes.AddRange(typesThatUseLogger);
                        foreach (TypeDefinition customType in typesThatUseLogger)
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

        protected override StatisticBase GetStatisticObject()
        {
            return LoggingStatistic.Instance;
        }
    }
}
