using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;
using Mono.Cecil.Cil;
using SPCAF.Sdk.Utilities;
using SPCAFContrib.Extensions;
using SPCAFContrib.Test.Common;
using SPCAFContrib.Test.TryCatch;

namespace SPCAFContrib.Test.CecilCache
{
    public static class CecilCacheExtensions
    {
       
    }

    [TestClass]
    public class CecilCacheImplTest : CodeTestBase
    {
        #region properties

        protected BadTryCatchImpl Instance = new BadTryCatchImpl();

        #endregion

        #region tests

        [TestMethod]
        public void CanEnumerateMethods()
        {
            long rawResult = 0;
            long cachedResult = 0;

            long stringCalbackHits = 0;

            const string JavaScriptTag = "javascript:";

            Action<MethodDefinition, Instruction> stringSearchCallBack = (method, instruction) =>
            {
                stringCalbackHits++;

                var value = instruction.Operand as string;

                if (!string.IsNullOrEmpty(value))
                {
                    if (value.IndexOf(JavaScriptTag, StringComparison.OrdinalIgnoreCase) >= 0)
                    {

                    }
                }
            };

            var asmFolder = @"C:\_cdplx\spcafcontrib\trunk\SPCAFContrib\_3rd part\SharePoint";
            var assemblies = Directory.GetFiles(asmFolder, "*.dll").Select(assemblyPath =>
            {
                using (var stream = new StreamReader(assemblyPath))
                {
                    return AssemblyDefinition.ReadAssembly((stream.BaseStream));
                }
            });

            assemblies = assemblies.Take(1);

            foreach (var assembly in assemblies)
            {
                Trace.WriteLine(string.Format("Run assembly:[{0}]", assembly.FullName));

                rawResult += WithScopeWatch(() =>
                {
                    foreach (var type in assembly.MainModule.GetTypes())
                    {
                        type.SearchMethodStrings(stringSearchCallBack);
                    }
                });

                cachedResult += WithScopeWatch(() =>
                {
                    foreach (var type in assembly.MainModule.GetTypes())
                    {
                        //type.SearchMethodStringsWithCache(stringSearchCallBack);
                    }
                });
            }

            Trace.WriteLine(string.Format("Raw result:[{0}]", rawResult));
            Trace.WriteLine(string.Format("Cached result:[{0}]", cachedResult));

            Trace.WriteLine(string.Format("Profit:[{0}] or [{1}%]", rawResult - cachedResult, (double)(rawResult - cachedResult) / (double)rawResult * 100));
            Trace.WriteLine(string.Format("String callback hits:[{0}]", stringCalbackHits));
        }

        private long WithScopeWatch(Action action)
        {
            var rawWatch = new Stopwatch();

            rawWatch.Start();

            action();

            rawWatch.Stop();

            return rawWatch.ElapsedMilliseconds;
        }

        #endregion

    }
}
