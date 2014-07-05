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
    public class ContentTypeCache : SPXmlEntityCache<ContentTypeXmlEntity>
    {
        #region Properties
        protected override string XmlSchemaContainerXPath
        {
            get { return "Elements"; }
        }

        protected override string XmlEntityXPath
        {
            get { return "Elements/ContentType"; }
        }

        protected override string CacheDirectoryName
        {
            get { return "ContentTypes"; }
        }

        public override IEqualityComparer<ContentTypeXmlEntity> ItemsEqualityComparer
        {
            get { return new ContentTypeXmlEntityEqualityComparer(); }
        }

        public static ContentTypeCache GetInstance(ISolution solution)
        {
            return solution.GetComponent<ContentTypeCache>();
        } 
        #endregion

        public ContentTypeCache(IShellLocks locks, ISolution solution, ChangeManager changeManager, Lifetime lifetime, IPsiConfiguration psiConfiguration, LanguageManager languageManager, IPersistentIndexManager persistentIndexManager)
            : base(locks, solution, changeManager, lifetime, psiConfiguration, languageManager, persistentIndexManager)
        {
        }

        #region Methods

        #region Overrided methods

        protected override IList<ContentTypeXmlEntity> BuildData(IPsiSourceFile sourceFile)
        {
            JetHashSet<ContentTypeXmlEntity> jetHashSet = null;
            IFile file = sourceFile.GetDominantPsiFile<XmlLanguage>();

            if (file is IXmlFile)
            {
                IXmlFile xmlFile = file as IXmlFile;
                IXmlTag validatedTag = xmlFile.GetNestedTags<IXmlTag>(XmlSchemaContainerXPath).FirstOrDefault();

                if (validatedTag != null)
                {
                    foreach (IXmlTag xmlTag in xmlFile.GetNestedTags<IXmlTag>(XmlEntityXPath))
                    {
                        if (jetHashSet == null)
                            jetHashSet = new JetHashSet<ContentTypeXmlEntity>(ItemsEqualityComparer);
                        
                        jetHashSet.Add(new ContentTypeXmlEntity()
                        {
                            Id = xmlTag.AttributeExists("ID") ? xmlTag.GetAttribute("ID").UnquotedValue.Trim() : String.Empty,
                            Name = xmlTag.AttributeExists("Name") ? xmlTag.GetAttribute("Name").UnquotedValue.Trim() : String.Empty,
                            Offset = xmlTag.GetTreeStartOffset()
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
    public class ContentTypeXmlEntity : SPXmlEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public override string GetPropertyValue(string attributeName)
        {
            switch (attributeName)
            {
                case "ID":
                    return Id;
                case "Name":
                    return Name;
                default:
                    throw new ArgumentOutOfRangeException("attributeName");
            }
        }
    }

    public class ContentTypeXmlEntityEqualityComparer : IEqualityComparer<ContentTypeXmlEntity>
    {
        public bool Equals(ContentTypeXmlEntity x, ContentTypeXmlEntity y)
        {
            return x.Offset.Equals(y.Offset) &&
                   String.Equals(x.Id.Trim(), y.Id.Trim(), StringComparison.OrdinalIgnoreCase) &&
                   String.Equals(x.Name.Trim(), y.Name.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(ContentTypeXmlEntity obj)
        {
            return (obj.Id.Trim() + obj.Name.Trim()).GetHashCode() ^ obj.Offset.GetHashCode();
        }
    }
}
