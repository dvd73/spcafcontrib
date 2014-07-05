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
    public class ListTemplateCache : SPXmlEntityCache<ListTemplateXmlEntity>
    {
        #region Properties
        protected override string XmlSchemaContainerXPath
        {
            get { return "Elements"; }
        }

        protected override string XmlEntityXPath
        {
            get { return "Elements/ListTemplate"; }
        }

        protected override string CacheDirectoryName
        {
            get { return "ListTemplates"; }
        }

        public override IEqualityComparer<ListTemplateXmlEntity> ItemsEqualityComparer
        {
            get { return new ListTemplateXmlEntityEqualityComparer(); }
        }

        public static ListTemplateCache GetInstance(ISolution solution)
        {
            return solution.GetComponent<ListTemplateCache>();
        } 
        #endregion

        public ListTemplateCache(IShellLocks locks, ISolution solution, ChangeManager changeManager, Lifetime lifetime, IPsiConfiguration psiConfiguration, LanguageManager languageManager, IPersistentIndexManager persistentIndexManager)
            : base(locks, solution, changeManager, lifetime, psiConfiguration, languageManager, persistentIndexManager)
        {
        }

        #region Methods

        #region Overrided methods

        protected override IList<ListTemplateXmlEntity> BuildData(IPsiSourceFile sourceFile)
        {
            JetHashSet<ListTemplateXmlEntity> jetHashSet = null;
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
                            jetHashSet = new JetHashSet<ListTemplateXmlEntity>(ItemsEqualityComparer);
                        
                        jetHashSet.Add(new ListTemplateXmlEntity()
                        {
                            Name = xmlTag.AttributeExists("Name") ? xmlTag.GetAttribute("Name").UnquotedValue.Trim() : String.Empty,
                            DisplayName = xmlTag.AttributeExists("DisplayName") ? xmlTag.GetAttribute("DisplayName").UnquotedValue.Trim() : String.Empty,
                            Type = xmlTag.AttributeExists("Type") ? xmlTag.GetAttribute("Type").UnquotedValue.Trim() : String.Empty,
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
    public class ListTemplateXmlEntity : SPXmlEntity
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Type { get; set; }

        public override string GetPropertyValue(string attributeName)
        {
            switch (attributeName)
            {
                case "DisplayName":
                    return DisplayName;
                case "Name":
                    return Name;
                case "Type":
                    return Type;
                default:
                    throw new ArgumentOutOfRangeException("attributeName");
            }
        }
    }

    public class ListTemplateXmlEntityEqualityComparer : IEqualityComparer<ListTemplateXmlEntity>
    {
        public bool Equals(ListTemplateXmlEntity x, ListTemplateXmlEntity y)
        {
            return x.Offset.Equals(y.Offset) &&
                   String.Equals(x.Name.Trim(), y.Name.Trim(), StringComparison.OrdinalIgnoreCase) &&
                   String.Equals(x.DisplayName.Trim(), y.DisplayName.Trim(), StringComparison.OrdinalIgnoreCase) &&
                   String.Equals(x.Type.Trim(), y.Type.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(ListTemplateXmlEntity obj)
        {
            return (obj.Name.Trim() + obj.DisplayName.Trim() + obj.Type.Trim()).GetHashCode() ^ obj.Offset.GetHashCode();
        }
    }
}
