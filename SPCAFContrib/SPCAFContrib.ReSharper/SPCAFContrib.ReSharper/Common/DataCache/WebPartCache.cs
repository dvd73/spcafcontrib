using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Xml;
using JetBrains.ReSharper.Psi.Xml.Tree;
using SPCAFContrib.ReSharper.Common.Attributes;
using SPCAFContrib.ReSharper.Common.Extensions;

namespace SPCAFContrib.ReSharper.Common.DataCache
{
    [PsiComponent]
    [Applicability(
       IDEProjectType.SP2010FarmSolution |
       IDEProjectType.SPSandbox |
       IDEProjectType.SP2013FarmSolution)]
    public class WebPartCache : SPXmlEntityCache<WebPartXmlEntity>
    {
        #region Properties
        protected override string XmlSchemaContainerXPath
        {
            get { return "webPart"; }
        }
        protected override string XmlSchemaName
        {
            get { return "http://schemas.microsoft.com/WebPart/v3"; }
        }
        protected override string XmlEntityXPath
        {
            get { return "webParts/webPart"; }
        }

        protected override string CacheDirectoryName
        {
            get { return "WebParts"; }
        }

        public override IEqualityComparer<WebPartXmlEntity> ItemsEqualityComparer
        {
            get { return new WebPartXmlEntityEqualityComparer(); }
        }

        public static WebPartCache GetInstance(ISolution solution)
        {
            return solution.GetComponent<WebPartCache>();
        } 
        #endregion

        public WebPartCache(IShellLocks locks, ISolution solution, ChangeManager changeManager, Lifetime lifetime, IPsiConfiguration psiConfiguration, LanguageManager languageManager, IPersistentIndexManager persistentIndexManager)
            : base(locks, solution, changeManager, lifetime, psiConfiguration, languageManager, persistentIndexManager)
        {
        }

        #region Methods
        
        #region Overrided methods

        protected override IList<WebPartXmlEntity> BuildData(IPsiSourceFile sourceFile)
        {
            JetHashSet<WebPartXmlEntity> jetHashSet = null;
            IFile file = sourceFile.GetDominantPsiFile<XmlLanguage>();

            if (file != null)
            {
                IXmlFile xmlFile = file as IXmlFile;
               
                IXmlTag validatedTag = xmlFile.GetNestedTags<IXmlTag>(XmlSchemaContainerXPath).FirstOrDefault();

                if (validatedTag != null)
                {
                    IXmlTag titleTag =
                        xmlFile.GetNestedTags<IXmlTag>("webParts/webPart/data/properties/property")
                            .FirstOrDefault(t => t.CheckAttributeValue("name", "Title", true));

                    if (titleTag != null)
                    {
                        if (jetHashSet == null)
                            jetHashSet = new JetHashSet<WebPartXmlEntity>(ItemsEqualityComparer);

                        jetHashSet.Add(new WebPartXmlEntity()
                        {
                            Title = titleTag.InnerText.Trim(),
                            Offset = validatedTag.GetTreeStartOffset()
                        });
                    }
                }
            }

            if (jetHashSet == null)
                return null;
            else
                return jetHashSet.ToArray();
        }

        #endregion

        #endregion
    }

    [Serializable()]
    public class WebPartXmlEntity : SPXmlEntity
    {
        public string Title { get; set; }
        public override string GetPropertyValue(string attributeName)
        {
            switch (attributeName)
            {
                case "Title":
                    return Title;
                default:
                    throw new ArgumentOutOfRangeException("attributeName");
            }
        }
    }

    public class WebPartXmlEntityEqualityComparer : IEqualityComparer<WebPartXmlEntity>
    {
        public bool Equals(WebPartXmlEntity x, WebPartXmlEntity y)
        {
            return x.Offset.Equals(y.Offset) &&
                   String.Equals(x.Title.Trim(), y.Title.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(WebPartXmlEntity obj)
        {
            return (obj.Title.Trim()).GetHashCode() ^ obj.Offset.GetHashCode();
        }
    }
}
