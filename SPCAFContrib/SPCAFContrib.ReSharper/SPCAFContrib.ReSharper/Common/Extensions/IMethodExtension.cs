using System;
using JetBrains.ReSharper.Psi;
using System.Collections.Generic;
using System.Linq;
using SPCAFContrib.ReSharper.Common.CodeAnalysis;

namespace SPCAFContrib.ReSharper.Common.Extensions
{
    public static class IMethodExtension
    {
        /// <summary>
        /// Checks that method has same parameter list
        /// </summary>
        /// <param name="method">Method instance</param>
        /// <param name="parameters">Pass null if you don't want check parameters and not null else. Empty list means method has no parameters.</param>
        /// <returns></returns>
        public static bool HasSameParameters(this IMethod method, IEnumerable<ParameterCriteria> parameters)
        {
            bool result = true;

            if (parameters != null)
            {
                result =
                    parameters.Any(
                        p =>
                            method.Parameters.All(
                                mp =>
                                    mp.Kind == p.Kind &&
                                    string.Equals(mp.Type.ToString(), p.ParameterType,
                                        StringComparison.OrdinalIgnoreCase)));
            }

            return result;
        }
    }
}
