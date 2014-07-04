using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using SPCAF.Sdk.Model;
using MethodDefinition = Mono.Cecil.MethodDefinition;

namespace SPCAFContrib.Extensions
{
    public static class CachedOperationExtensions
    {
        #region methods

        public static IEnumerable<TypeDefinition> GetAllTypeDefinitions(AssemblyFileReference assembly)
        {
            return CecilCacheExtensions.AssemblyCache.ResolveAllAssemblyTypes(assembly);
        }

        public static IEnumerable<MethodDefinition> GetAllTypeNotEmptyMethods(TypeDefinition type)
        {
            return CecilCacheExtensions.TypeCache.ResolveNonEmptyMethods(type);
        }

        public static void SearchMethodStringInstructions(AssemblyFileReference assembly,
            Action<MethodDefinition, Instruction> searchCallback)
        {
            IEnumerable<TypeDefinition> types = GetAllTypeDefinitions(assembly);

            foreach (TypeDefinition type in types)
            {
                IEnumerable<MethodDefinition> methods = GetAllTypeNotEmptyMethods(type);

                foreach (MethodDefinition targetMethod in methods)
                {
                    SearchMethodStringInstructions(targetMethod, searchCallback);
                }
            }
        }

        public static void SearchMethodStringInstructions(MethodDefinition method,
                                    Action<MethodDefinition, Instruction> searchCallback)
        {
            SearchMethodInstructions(method, new[] { OpCodes.Ldstr }, searchCallback);
        }

        public static void SearchMethodInstructions(MethodDefinition method,
                                      IEnumerable<OpCode> opCodes,
                                      Action<MethodDefinition, Instruction> searchCallback)
        {
            List<Instruction> instructions = CecilCacheExtensions.MethodCache.ResolveInstructions(method, opCodes);

            foreach (Instruction instruction in instructions)
                searchCallback(method, instruction);
        }

        public static Dictionary<Mono.Cecil.FieldDefinition, string> SearchNotEmptyOrNullTypeConsts(TypeDefinition type)
        {
            return CecilCacheExtensions.TypeCache.ResolveNotNullOrEmptyFieldConstStrings(type);
        }

        public static List<Mono.Cecil.FieldDefinition> SearchPublicNotEmptyOrNullTypeConsts(IEnumerable<AssemblyFileReference> assemblies)
        {
            return CecilCacheExtensions.AssemblyCache.SearchPublicNotEmptyOrNullTypeConsts(assemblies);
        }

        #endregion
    }
}
