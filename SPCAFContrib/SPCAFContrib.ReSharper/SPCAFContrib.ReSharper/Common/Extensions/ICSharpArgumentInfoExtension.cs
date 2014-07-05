using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl.Resolve;

namespace SPCAFContrib.ReSharper.Common.Extensions
{
    public static class ICSharpArgumentInfoExtension
    {
        public static bool IsReferenceOfPropertyUsage(this ICSharpArgumentInfo argument, IClrTypeName typeName, IEnumerable<string> propertyNames)
        {
            bool result = false;
            
            if (argument is ExpressionArgumentInfo)
            {
                result = (argument as ExpressionArgumentInfo).Expression.IsResolvedAsPropertyUsage(typeName, propertyNames);
            }

            return result;
        }
    }
}
