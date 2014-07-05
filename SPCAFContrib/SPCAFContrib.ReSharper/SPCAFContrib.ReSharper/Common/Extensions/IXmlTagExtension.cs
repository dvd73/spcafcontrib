using System;
using System.Linq;
using JetBrains.ReSharper.Psi.Xml.Impl.Tree;
using JetBrains.ReSharper.Psi.Xml.Parsing;
using JetBrains.ReSharper.Psi.Xml.Tree;
using JetBrains.ReSharper.Psi.Xml.Util;

namespace SPCAFContrib.ReSharper.Common.Extensions
{
    public static class IXmlTagExtension
    {
        public static bool AttributeExists(this IXmlTag tag, string attName)
        {
            IXmlAttribute attribute = tag.GetAttribute(attName);
            return attribute != null && attribute.Value != null;
        }

        public static bool CheckAttributeValue(this IXmlTag tag, string attName, string attValue, bool exactly = false)
        {
            IXmlAttribute attribute = tag.GetAttribute(attName);
            return attribute != null && attribute.Value != null &&
                   (exactly && attribute.UnquotedValue.ToLower() == attValue.ToLower() ||
                    !exactly && attribute.UnquotedValue.ToLower().Contains(attValue.ToLower()));
        }

        public static bool AttributeValueIsGuid(this IXmlTag tag, string attName)
        {
            IXmlAttribute attribute = tag.GetAttribute(attName);
            Guid dummy;
            return attribute !=null && attribute.Value != null && Guid.TryParse(attribute.UnquotedValue, out dummy);
        }

        public static void EnsureAttribute(this IXmlTag tag, string attName, string attValue)
        {
            if (!tag.AttributeExists(attName))
            {
                XmlElementFactory elementFactory = XmlElementFactory.GetInstance(tag);
                IXmlAttribute anchor = tag.GetAttributes().LastOrDefault();

                if (anchor != null)
                {
                    IXmlAttribute attOverwrite = elementFactory.CreateAttributeForTag(tag,
                        String.Format("{0}=\"{1}\"", attName, attValue));
                    tag.AddAttributeAfter(attOverwrite, anchor);
                }
            }
            else
            {
                XmlAttributeUtil.SetValue(tag.GetAttribute(attName), attValue);
            }
        }

        public static bool IsFieldDefinition(this IXmlTag tag)
        {
            IXmlTag parentTag = XmlTagContainerNavigator.GetByTag(tag) as IXmlTag;

            return tag.Header.ContainerName == "Field" && (parentTag == null ||
                                                           parentTag.Header.ContainerName != "Row");
        }
    }
}
