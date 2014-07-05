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
    public class ListInstanceCache : SPXmlEntityCache<ListInstanceXmlEntity>
    {
        #region Properties
        protected override string XmlSchemaContainerXPath
        {
            get { return "Elements"; }
        }

        protected override string XmlEntityXPath
        {
            get { return "Elements/ListInstance"; }
        }

        protected override string CacheDirectoryName
        {
            get { return "ListInstances"; }
        }

        public override IEqualityComparer<ListInstanceXmlEntity> ItemsEqualityComparer
        {
            get { return new ListInstanceXmlEntityEqualityComparer(); }
        }

        public static ListInstanceCache GetInstance(ISolution solution)
        {
            return solution.GetComponent<ListInstanceCache>();
        } 
        #endregion

        public ListInstanceCache(IShellLocks locks, ISolution solution, ChangeManager changeManager, Lifetime lifetime, IPsiConfiguration psiConfiguration, LanguageManager languageManager, IPersistentIndexManager persistentIndexManager)
            : base(locks, solution, changeManager, lifetime, psiConfiguration, languageManager, persistentIndexManager)
        {
        }

        #region Methods
        
        #region Overrided methods

        protected override IList<ListInstanceXmlEntity> BuildData(IPsiSourceFile sourceFile)
        {
            JetHashSet<ListInstanceXmlEntity> jetHashSet = null;
            IFile file = sourceFile.GetDominantPsiFile<XmlLanguage>();

            if (file != null)
            {
                IXmlFile xmlFile = file as IXmlFile;
                IXmlTag validatedTag = xmlFile.GetNestedTags<IXmlTag>(XmlSchemaContainerXPath).FirstOrDefault();

                if (validatedTag != null)
                {
                    foreach (IXmlTag xmlTag in xmlFile.GetNestedTags<IXmlTag>(XmlEntityXPath))
                    {
                        if (jetHashSet == null)
                            jetHashSet = new JetHashSet<ListInstanceXmlEntity>(ItemsEqualityComparer);
                        
                        jetHashSet.Add(new ListInstanceXmlEntity()
                        {
                            Title = xmlTag.AttributeExists("Title") ? xmlTag.GetAttribute("Title").UnquotedValue.Trim() : String.Empty,
                            Url = xmlTag.AttributeExists("Url") ? xmlTag.GetAttribute("Url").UnquotedValue.Trim() : String.Empty,
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
    public class ListInstanceXmlEntity : SPXmlEntity
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public override string GetPropertyValue(string attributeName)
        {
            switch (attributeName)
            {
                case "Url":
                    return Url;
                case "Title":
                    return Title;
                default:
                    throw new ArgumentOutOfRangeException("attributeName");
            }
        }
    }

    public class ListInstanceXmlEntityEqualityComparer : IEqualityComparer<ListInstanceXmlEntity>
    {
        public bool Equals(ListInstanceXmlEntity x, ListInstanceXmlEntity y)
        {
            return x.Offset.Equals(y.Offset) &&
                   String.Equals(x.Title.Trim(), y.Title.Trim(), StringComparison.OrdinalIgnoreCase) &&
                   String.Equals(x.Url.Trim(), y.Url.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(ListInstanceXmlEntity obj)
        {
            return (obj.Title.Trim() + obj.Url.Trim()).GetHashCode() ^ obj.Offset.GetHashCode();
        }
    }
}
