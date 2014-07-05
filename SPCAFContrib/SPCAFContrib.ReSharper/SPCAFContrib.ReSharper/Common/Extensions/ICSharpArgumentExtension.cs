using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace SPCAFContrib.ReSharper.Common.Extensions
{
    public static class ICSharpArgumentExtension
    {
        public static bool IsReferenceOfPropertyUsage(this ICSharpArgument argument, IClrTypeName typeName, IEnumerable<string> propertyNames)
        {
            bool result = false;

            if (argument.Value is IReferenceExpression)
            {
                result = (argument.Value as IReferenceExpression).IsResolvedAsPropertyUsage(typeName, propertyNames);
            }

            return result;
        }
    }
}
