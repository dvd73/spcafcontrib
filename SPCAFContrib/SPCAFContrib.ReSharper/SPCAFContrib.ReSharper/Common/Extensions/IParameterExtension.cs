using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;

namespace SPCAFContrib.ReSharper.Common.Extensions
{
    public static class IParameterExtension
    {
        public static bool IsOneOfTheTypes(this IParameter element, IEnumerable<IClrTypeName> typeNames)
        {
            bool result = false;

            IDeclaredType scalarType = element.Type.GetScalarType();
            if (scalarType != null && !scalarType.IsUnknown)
            {
                result = typeNames.Any(clrTypeName => scalarType.GetClrName().Equals(clrTypeName));
            }

            return result;
        }
    }
}
