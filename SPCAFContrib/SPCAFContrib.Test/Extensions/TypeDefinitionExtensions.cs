using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using System.Diagnostics;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;

namespace SPCAFContrib.Test.Extensions
{
    public static class TypeDefinitionExtensions
    {
        #region methods

        public static void TraceTypes(this AssemblyDefinition assembly)
        {
            var types = assembly.MainModule.GetTypes();

            foreach (var type in types)
            {
                Trace.WriteLine(string.Format("     type:[{0}]", type.Name));
            }
        }

        public static void TraceFields(this TypeDefinition type)
        {
            Trace.WriteLine(string.Format("Type:[{0}]", type.Name));

            foreach (var field in type.Fields)
            {
                Trace.WriteLine(string.Format("     Field:[{0}] InitialValue:[{1}] Constant:[{2}]", field.Name, field.InitialValue, field.Constant));
            }
        }

        public static void TraceMethodInstructions(this TypeDefinition type)
        {
            Trace.WriteLine(string.Format("Type:[{0}]", type.Name));

            foreach (var method in type.Methods)
            {
                Trace.WriteLine(string.Format(" Method:[{0}]", method.Name));

                if (method.HasBody)
                {
                    foreach (var instruction in method.Body.Instructions)
                    {
                        Trace.WriteLine(string.Format("     Instruction:[{0}] OpCode:[{1}] Operand:[{2}]",
                                instruction,
                                instruction.OpCode,
                                instruction.Operand));
                    }
                }
            }
        }

        #endregion
    }
}
