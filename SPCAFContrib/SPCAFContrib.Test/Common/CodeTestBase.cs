using System.Diagnostics;
using Mono.Cecil;
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SPCAFContrib.Test.Common
{
    public class CodeTestBase
    {
        #region utils

        protected virtual void WithTargetType(string typeName, Action<AssemblyDefinition, TypeDefinition> action)
        {
            WithCurrentAssembly(assembly =>
            {
                var types = assembly.MainModule.Types.Where(t => String.Compare(t.Name, typeName, StringComparison.OrdinalIgnoreCase) == 0);

                foreach (var type in types)
                {
                    action(assembly, type);
                }
            });
        }

        protected virtual void WithTargetMethod(string typeName, string methodName, Action<AssemblyDefinition, TypeDefinition, MethodDefinition> action)
        {
            WithTargetType(typeName, (assembly, type) =>
            {
                var methods = type.Methods.Where(m => String.Compare(m.Name, methodName, StringComparison.OrdinalIgnoreCase) == 0);

                foreach (var method in methods)
                    action(assembly, type, method);
            });
        }

        protected virtual void WithTargetAssembly(Assembly assembly, Action<AssemblyDefinition> action)
        {
            WithTargetAssembly(assembly.Location, action);
        }

        protected virtual void WithTargetAssembly(string assemblyPath, Action<AssemblyDefinition> action)
        {
            using (var stream = new StreamReader(assemblyPath))
            {
                action(AssemblyDefinition.ReadAssembly((stream.BaseStream)));
            }
        }

        protected virtual void WithCurrentAssembly(Action<AssemblyDefinition> action)
        {
            var path = Assembly.GetAssembly(GetType()).Location;

            using (var stream = new StreamReader(path))
            {
                var assembly = AssemblyDefinition.ReadAssembly(stream.BaseStream);

                action(assembly);
            }
        }

        protected virtual void WithTargetMethod(Expression<Action> expr, Action<AssemblyDefinition, TypeDefinition, MethodDefinition> action)
        {
            var methodCall = (MethodCallExpression)expr.Body;

            Debug.Assert(methodCall.Method.DeclaringType != null, "methodCall.Method.DeclaringType != null");

            var typeName = methodCall.Method.DeclaringType.Name;
            var methodName = methodCall.Method.Name;

            WithTargetMethod(typeName, methodName, action);
        }

        #endregion
    }
}
