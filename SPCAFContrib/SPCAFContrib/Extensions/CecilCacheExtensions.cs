using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using FieldDefinition = Mono.Cecil.FieldDefinition;
using MethodDefinition = Mono.Cecil.MethodDefinition;

namespace SPCAFContrib.Extensions
{
    public static class CecilCacheExtensions
    {
        public static class AssemblyCache
        {
            #region properties

            private static Dictionary<AssemblyFileReference, List<TypeDefinition>> AllAssemblyTypes =
                new Dictionary<AssemblyFileReference, List<TypeDefinition>>();

            #endregion

            #region methods

            public static List<TypeDefinition> ResolveAllAssemblyTypes(AssemblyFileReference assembly)
            {
                if (!AllAssemblyTypes.ContainsKey(assembly))
                {
                    AllAssemblyTypes.Add(assembly, new List<TypeDefinition>());
                    AllAssemblyTypes[assembly].AddRange(assembly.AllTypeDefinitions());
                }

                return AllAssemblyTypes[assembly];
            }

            #endregion

            private static Dictionary<AssemblyFileReference, List<FieldDefinition>> PublicNotEmptyOrNullTypeConstsCache = new Dictionary<AssemblyFileReference, List<FieldDefinition>>();

            public static List<FieldDefinition> SearchPublicNotEmptyOrNullTypeConsts(IEnumerable<AssemblyFileReference> assemblies)
            {
                List<FieldDefinition> result = new List<FieldDefinition>();

                foreach (AssemblyFileReference assembly in assemblies)
                {
                    if (!PublicNotEmptyOrNullTypeConstsCache.ContainsKey(assembly))
                    {
                        List<FieldDefinition> assemblyConstsCache = new List<FieldDefinition>();
                        List<TypeDefinition> allTypes = ResolveAllAssemblyTypes(assembly);

                        foreach (TypeDefinition type in allTypes)
                        {
                            Dictionary<FieldDefinition, string> fields = TypeCache.ResolveNotNullOrEmptyFieldConstStrings(type);

                            foreach (KeyValuePair<FieldDefinition, string> field in fields)
                            {
                                if (field.Key.IsPublic)
                                    assemblyConstsCache.Add(field.Key);
                            }
                        }

                        PublicNotEmptyOrNullTypeConstsCache.Add(assembly, assemblyConstsCache);
                    }

                    result.AddRange(PublicNotEmptyOrNullTypeConstsCache[assembly]);
                }

                return result;
            }
        }

        public static class TypeCache
        {
            #region properties

            private static Dictionary<TypeDefinition, List<MethodDefinition>> NonEmptyMethods = new Dictionary<TypeDefinition, List<MethodDefinition>>();

            private static Dictionary<TypeDefinition, Dictionary<Mono.Cecil.FieldDefinition, string>> AllFieldConsts = new Dictionary<TypeDefinition, Dictionary<Mono.Cecil.FieldDefinition, string>>();
            private static Dictionary<TypeDefinition, Dictionary<Mono.Cecil.FieldDefinition, string>> NotNullOrEmptyFieldConsts = new Dictionary<TypeDefinition, Dictionary<Mono.Cecil.FieldDefinition, string>>();

            #endregion

            #region methods

            public static Dictionary<Mono.Cecil.FieldDefinition, string> ResolveNotNullOrEmptyFieldConstStrings(TypeDefinition type)
            {
                EnsureFieldConstStrings(type);

                return NotNullOrEmptyFieldConsts[type];
            }

            public static Dictionary<Mono.Cecil.FieldDefinition, string> ResolveAllFieldConstStrings(TypeDefinition type)
            {
                EnsureFieldConstStrings(type);

                return AllFieldConsts[type];
            }

            private static void EnsureFieldConstStrings(TypeDefinition type)
            {
                if (!AllFieldConsts.ContainsKey(type))
                {
                    Dictionary<FieldDefinition, string> allFieldDictDict = new Dictionary<FieldDefinition, string>();
                    Dictionary<FieldDefinition, string> notEmptyFieldDict = new Dictionary<FieldDefinition, string>();

                    foreach (FieldDefinition field in type.Fields.Where(field => field.HasConstant))
                    {
                        string value = field.Constant as string;

                        allFieldDictDict.Add(field, value);

                        if (!string.IsNullOrEmpty(value))
                            notEmptyFieldDict.Add(field, value);
                    }

                    AllFieldConsts.Add(type, allFieldDictDict);
                    NotNullOrEmptyFieldConsts.Add(type, notEmptyFieldDict);
                }
            }

            public static List<MethodDefinition> ResolveNonEmptyMethods(TypeDefinition type)
            {
                if (!NonEmptyMethods.ContainsKey(type))
                {
                    NonEmptyMethods.Add(type, new List<MethodDefinition>());
                    NonEmptyMethods[type].AddRange(type.Methods.Where(method => !method.IsAbstract && method.HasBody));
                }

                return NonEmptyMethods[type];
            }

            #endregion
        }

        public static class MethodCache
        {
            #region properties

            private static Dictionary<MethodDefinition, Dictionary<OpCode, List<Instruction>>> MethodOpCodeToInstructionMap = new Dictionary<MethodDefinition, Dictionary<OpCode, List<Instruction>>>();

            #endregion

            public static List<Instruction> ResolveInstructions(MethodDefinition method, IEnumerable<OpCode> opCodes)
            {
                List<Instruction> result = new List<Instruction>();

                foreach (OpCode opCode in opCodes)
                {
                    if (!MethodOpCodeToInstructionMap.ContainsKey(method))
                        MethodOpCodeToInstructionMap.Add(method, new Dictionary<OpCode, List<Instruction>>());

                    Dictionary<OpCode, List<Instruction>> cachedInstructionMap = MethodOpCodeToInstructionMap[method];

                    if (!cachedInstructionMap.ContainsKey(opCode))
                    {
                        List<Instruction> cachedOpCodes = new List<Instruction>();

                        cachedOpCodes.AddRange(method.Body.Instructions.Where(i => i.OpCode == opCode));
                        cachedInstructionMap.Add(opCode, cachedOpCodes);
                    }

                    result.AddRange(cachedInstructionMap[opCode]);
                }

                return result;
            }
        }
    }
}
