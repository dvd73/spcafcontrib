using System;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace SPCAFContrib.ReSharper.Common.Extensions
{
    public static class ICSharpTypeMemberDeclarationExtension
    {
        public static bool HasPropertySet(this ICSharpTypeMemberDeclaration method, IClrTypeName typeName,
            string propertyName, string qualifier)
        {
            bool result = false;

            foreach (ITreeNode node in method.EnumerateSubTree())
            {
                if (node is IAssignmentExpression)
                {
                    IAssignmentExpression assignmentExpression = node as IAssignmentExpression;
                    if (assignmentExpression.Dest is IReferenceExpression)
                    {
                        IReferenceExpression referenceExpression = assignmentExpression.Dest as IReferenceExpression;
                        bool propertyIsUsed = referenceExpression.IsResolvedAsPropertyUsage(typeName,
                            new[] {propertyName});
                        ICSharpExpression extensionQualifier = referenceExpression.GetExtensionQualifier();
                        bool qualifierIsUsed = extensionQualifier != null && String.Equals(extensionQualifier.GetText(), qualifier, StringComparison.OrdinalIgnoreCase);

                        result = qualifierIsUsed && propertyIsUsed;
                        
                        if (result)
                            break;
                    }
                }
            }

            return result;
        }
    }
}
