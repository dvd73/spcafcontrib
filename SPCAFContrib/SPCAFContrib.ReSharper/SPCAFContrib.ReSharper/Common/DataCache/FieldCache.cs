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
    public class FieldCache : SPXmlEntityCache<FieldXmlEntity>
    {
        #region Properties
        protected override string XmlSchemaContainerXPath
        {
            get { return "Elements"; }
        }

        protected override string XmlEntityXPath
        {
            get { return "Elements/Field"; }
        }

        protected override string CacheDirectoryName
        {
            get { return "Fields"; }
        }

        public override IEqualityComparer<FieldXmlEntity> ItemsEqualityComparer
        {
            get { return new FieldXmlEntityEqualityComparer(); }
        }

        public static FieldCache GetInstance(ISolution solution)
        {
            return solution.GetComponent<FieldCache>();
        } 
        #endregion

        public FieldCache(IShellLocks locks, ISolution solution, ChangeManager changeManager, Lifetime lifetime, IPsiConfiguration psiConfiguration, LanguageManager languageManager, IPersistentIndexManager persistentIndexManager)
            : base(locks, solution, changeManager, lifetime, psiConfiguration, languageManager, persistentIndexManager)
        {
        }

        #region Methods

        #region Overrided methods

        protected override IList<FieldXmlEntity> BuildData(IPsiSourceFile sourceFile)
        {
            JetHashSet<FieldXmlEntity> jetHashSet = null;
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
                            jetHashSet = new JetHashSet<FieldXmlEntity>(ItemsEqualityComparer);
                        
                        jetHashSet.Add(new FieldXmlEntity()
                        {
                            Id = xmlTag.AttributeExists("ID") ? xmlTag.GetAttribute("ID").UnquotedValue.Trim() : String.Empty,
                            Name = xmlTag.AttributeExists("Name") ? xmlTag.GetAttribute("Name").UnquotedValue.Trim() : String.Empty,
                            StaticName = xmlTag.AttributeExists("StaticName") ? xmlTag.GetAttribute("StaticName").UnquotedValue.Trim() : String.Empty,
                            DisplayName = xmlTag.AttributeExists("DisplayName") ? xmlTag.GetAttribute("DisplayName").UnquotedValue.Trim() : String.Empty,
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
    public class FieldXmlEntity : SPXmlEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string StaticName { get; set; }
        public string DisplayName { get; set; }

        public override string GetPropertyValue(string attributeName)
        {
            switch (attributeName)
            {
                case "ID":
                    return Id;
                case "Name":
                    return Name;
                case "StaticName":
                    return StaticName;
                case "DisplayName":
                    return DisplayName;
                default:
                    throw new ArgumentOutOfRangeException("attributeName");
            }
        }
        
    }

    public class FieldXmlEntityEqualityComparer : IEqualityComparer<FieldXmlEntity>
    {
        public bool Equals(FieldXmlEntity x, FieldXmlEntity y)
        {
            return x.Offset.Equals(y.Offset) &&
                   String.Equals(x.Id.Trim(), y.Id.Trim(), StringComparison.OrdinalIgnoreCase) &&
                   String.Equals(x.Name.Trim(), y.Name.Trim(), StringComparison.OrdinalIgnoreCase) &&
                   String.Equals(x.StaticName.Trim(), y.StaticName.Trim(), StringComparison.OrdinalIgnoreCase) &&
                   String.Equals(x.DisplayName.Trim(), y.DisplayName.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(FieldXmlEntity obj)
        {
            return (obj.Id.Trim() + obj.Name.Trim() + obj.StaticName + obj.DisplayName).GetHashCode() ^ obj.Offset.GetHashCode();
        }
    }
}
