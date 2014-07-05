using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace SPCAFContrib.ReSharper.Common.Extensions
{
    public static class ICSharpTreeNodeExtension
    {
        public static string ContainerReadableName(this ICSharpTreeNode element)
        {
            return String.Format("{0}: {1}", element.GetContainingTypeDeclaration().CLRName,
                element.GetContainingTypeMemberDeclaration().DeclaredName);
        }
    }
}
