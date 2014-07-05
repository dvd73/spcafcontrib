using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace SPCAFContrib.ReSharper.Common.Extensions
{
    public static class IReferenceExtension
    {
        public static bool IsResolvedAs(this IReference element, IEnumerable<string> fullReferenceNames)
        {
            bool result = false;

            if (element != null &&
                element.CurrentResolveResult != null &&
                element.CurrentResolveResult.DeclaredElement != null &&
                element.CurrentResolveResult.ResolveErrorType == ResolveErrorType.OK)
            {
                string method = element.CurrentResolveResult.DeclaredElement.ToString();
                result = fullReferenceNames.Any(m => String.Equals(method, m, StringComparison.OrdinalIgnoreCase));
            }

            return result;
        }

        public static bool HasReferencesFromType(this IReference method,
            ITypeDeclaration classDeclaration)
        {
            return
                classDeclaration.EnumerateSubTree()
                    .OfType<IReferenceExpression>()
                    .Any(referenceExpression => referenceExpression.ContainsReference(method));
        }
    }
}
