using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using Mono.Cecil;
using Mono.Cecil.Cil;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using System.Collections.Generic;
using SPCAFContrib.Consts;
using SPCAF.Sdk;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Extensions;
using SPCAFContrib.Utils;
using System.IO;
using FieldDefinition = Mono.Cecil.FieldDefinition;
using MethodDefinition = Mono.Cecil.MethodDefinition;

namespace SPCAFContrib.Experimental.Rules.Code
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
        CheckId = CheckIDs.Rules.Assembly.RepeatableMagicStringShouldNotBeUsed,
        DisplayName = "Repeatable magic string should not be used inside methods.",
        Description = "Consider a better place to define a magic string.",
        DefaultSeverity = Severity.Information,
        SharePointVersion = new[] { "12", "14", "15" },
        Message = "Repeatable magic string is detected [{0}].",
        Resolution = "Consider a better place to define string.")]
    public class RepeatableMagicStringShouldNotBeUsed : Rule<AssemblyFileReference>
    {
        #region classes

        private class LocalMethodStringStat
        {
            public LocalMethodStringStat()
            {
                Instruction = new List<Instruction>();
            }

            public int Count { get; set; }
            public List<Instruction> Instruction { get; set; }
        }

        #endregion

        #region methods

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            IEnumerable<AssemblyFileReference> allAssemblies = TryLoadAssemblyWithReferencies(assembly);
            Dictionary<string, string> publicGlobalStrings = CachedOperationExtensions.SearchPublicNotEmptyOrNullTypeConsts(allAssemblies)
                                                               .Select(s => s.Constant as string)
                                                               .Distinct()
                                                               .ToDictionary(s => s);

            IEnumerable<TypeDefinition> types = CachedOperationExtensions.GetAllTypeDefinitions(assembly);

            foreach (TypeDefinition type in types)
            {
                Dictionary<FieldDefinition, string> typeConsts = CachedOperationExtensions.SearchNotEmptyOrNullTypeConsts(type);
                IEnumerable<MethodDefinition> methods = CachedOperationExtensions.GetAllTypeNotEmptyMethods(type);

                foreach (MethodDefinition targetMethod in methods)
                {
                    Dictionary<string, LocalMethodStringStat> localMethodStrings = new Dictionary<string, LocalMethodStringStat>();

                    if (targetMethod.IsConstructor)
                        continue;

                    CachedOperationExtensions.SearchMethodStringInstructions(targetMethod, (method, instruction) =>
                    {
                        Dictionary<FieldDefinition, string> _localTypeConsts = typeConsts;
                        Dictionary<string, LocalMethodStringStat> _localMethodString = localMethodStrings;

                        if (method.IsConstructor) return;

                        string value = instruction.Operand as string;

                        if (string.IsNullOrEmpty(value))
                            return;

                        // possible global/local const
                        if (_localTypeConsts.ContainsValue(value) || publicGlobalStrings.ContainsKey(value))
                            return;

                        if (!_localMethodString.ContainsKey(value))
                            _localMethodString.Add(value, new LocalMethodStringStat());
                        else
                        {
                            _localMethodString[value].Count++;
                            _localMethodString[value].Instruction.Add(instruction);

                            if (_localMethodString[value].Count > 2)
                            {
                                Notify(method,
                                   string.Format(this.MessageTemplate(), value),
                                   new CodeInstruction(method, instruction).ImproveSummary(assembly.GetSummary()),
                                   notifications);
                            }
                        }
                    });
                }
            }
        }

        private static IEnumerable<AssemblyFileReference> TryLoadAssemblyWithReferencies(AssemblyFileReference assembly)
        {
            List<AssemblyFileReference> allAssemblies = new List<AssemblyFileReference> { assembly };

            IEnumerable<AssemblyNameReference> assemblyNameRefs = assembly.ReferencedAssemblies();
            DefaultAssemblyResolver resolver = new DefaultAssemblyResolver();

            foreach (AssemblyNameReference asmName in assemblyNameRefs)
            {
                try
                {
                    allAssemblies.Add(new AssemblyFileReference { AssemblyDefinition = resolver.Resolve(asmName) });
                }
                catch (Exception e)
                {
                    // TODO :)
                    // it might be better to submit ticket to SPCAF to understand the best choice for cross-wsp assemblies
                }
            }

            return allAssemblies;
        }

        #endregion
    }
}
