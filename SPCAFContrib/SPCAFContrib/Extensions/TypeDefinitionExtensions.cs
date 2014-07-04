using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Common;
using FieldDefinition = Mono.Cecil.FieldDefinition;
using MethodDefinition = Mono.Cecil.MethodDefinition;

namespace SPCAFContrib.Extensions
{
    public static class TypeDefinitionExtensions
    {
        #region methods

        public static void SearchConstStrings(this TypeDefinition type, Action<FieldDefinition> searchCallback)
        {
            if (searchCallback == null) return;

            IOrderedEnumerable<FieldDefinition> constFields = type.Fields.Where(field => field.HasConstant).OrderBy(field=>field.Name);

            foreach (FieldDefinition field in constFields)
                searchCallback(field);
        }

        public static void SearchMethodInstructions(this MethodDefinition method,
                                               IEnumerable<OpCode> opCodes,
                                               Action<MethodDefinition, Instruction> searchCallback)
        {
            IEnumerable<Instruction> instructions = method.Body.Instructions.Where(i => opCodes.Contains(i.OpCode));

            foreach (Instruction instruction in instructions)
                searchCallback(method, instruction);
        }

        public static void SearchMethodStrings(this TypeDefinition type, Action<MethodDefinition, Instruction> searchCallback)
        {
            if (searchCallback == null) return;

            IOrderedEnumerable<MethodDefinition> methods = type.Methods.Where(method => !method.IsAbstract && method.HasBody).OrderBy(method => method.Name);

            foreach (MethodDefinition method in methods)
                method.SearchMethodInstructions(new[] { OpCodes.Ldstr }, searchCallback);
        }

        public static void SearchMethodNumbers(this TypeDefinition type, Action<MethodDefinition, Instruction> searchCallback)
        {
            if (searchCallback == null) return;

            IEnumerable<MethodDefinition> methods = type.Methods.Where(method => !method.IsAbstract && method.HasBody);

            foreach (MethodDefinition method in methods)
                method.SearchMethodInstructions(new[] { 
                    // int
                    OpCodes.Ldc_I4,
                    OpCodes.Ldc_I4_0,
                    OpCodes.Ldc_I4_1,
                    OpCodes.Ldc_I4_2,
                    OpCodes.Ldc_I4_3,
                    OpCodes.Ldc_I4_4,
                    OpCodes.Ldc_I4_5,
                    OpCodes.Ldc_I4_6,
                    OpCodes.Ldc_I4_8,
                    OpCodes.Ldc_I4_S,

                    //
                    OpCodes.Ldc_R4,
                    OpCodes.Ldc_R8,
                }, searchCallback);
        }

        public static void InsideMethodCalls(this TypeDefinition type, MultiValueDictionary<string, string> methodList, HashSet<string> handledMethods, bool ignoreSystemCalls, Action<MethodDefinition> report)
        {
            foreach (MethodDefinition methodDefinition in type.Methods.OrderBy(method => method.Name))
            {
                if (!methodList.TryAdd(methodDefinition.DeclaringType.FullName, methodDefinition.Name))
                    continue;
                else
                {
                    if (report != null)
                        report(methodDefinition);
                }

                methodDefinition.InsideMethodsCalls(methodList, handledMethods, ignoreSystemCalls, _ => { });
            }
        }

        #endregion

        public static IEnumerable<TypeDefinition> ResolveTypesByBaseType(this AssemblyFileReference assembly, string typeName)
        {
            return assembly.TypesThatDerivesFromType(typeName).OrderBy(td => td.Name);
        }

        public static IEnumerable<TypeDefinition> ResolveTypesByBaseTypes(this AssemblyFileReference assembly, IEnumerable<string> typeNames)
        {
            return typeNames.SelectMany(t => assembly.ResolveTypesByBaseType(t)).OrderBy(td => td.Name);
        }
    }
}
