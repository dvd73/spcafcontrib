using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Psi.Asp.Parsing;
using JetBrains.ReSharper.Psi.Asp.Tree;
using JetBrains.ReSharper.Psi.Html.Tree;

namespace SPCAFContrib.ReSharper.Common.Extensions
{
    public static class AspElementFactoryExtension
    {
        public static ITagAttribute CreateAttributeForTag(this AspElementFactory elementFactory, IAspTag src, string attName, string attValue)
        {
            return
                elementFactory.CreateHtmlTag(String.Format("<dummy {0}=\"{1}\" />", attName, attValue), src)
                    .Attributes.First();
        }
    }
}
