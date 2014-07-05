using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Markup;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.LinqTools;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Xml.Tree;
using SPCAFContrib.ReSharper.Common.Attributes;
using SPCAFContrib.ReSharper.Common.Extensions;
using XmlLanguage = JetBrains.ReSharper.Psi.Xml.XmlLanguage;

namespace SPCAFContrib.ReSharper.Common.DataCache
{
    [PsiComponent]
    [Applicability(
       IDEProjectType.SP2010FarmSolution |
       IDEProjectType.SPSandbox |
       IDEProjectType.SP2013FarmSolution)]
    public class FeatureCache : SPXmlEntityCache<FeatureXmlEntity>
    {
        #region Properties
        protected override string XmlSchemaContainerXPath
        {
            get { return "feature"; }
        }

        protected override string XmlEntityXPath
        {
            get { return "feature"; }
        }

        protected override string CacheDirectoryName
        {
            get { return "Features"; }
        }

        protected override string XmlSchemaName
        {
            get { return "http://schemas.microsoft.com/VisualStudio/2008/SharePointTools/FeatureModel"; }
        }

        public override IEqualityComparer<FeatureXmlEntity> ItemsEqualityComparer
        {
            get { return new FeatureXmlEntityEqualityComparer(); }
        }

        public static FeatureCache GetInstance(ISolution solution)
        {
            return solution.GetComponent<FeatureCache>();
        } 
        #endregion

        public FeatureCache(IShellLocks locks, ISolution solution, ChangeManager changeManager, Lifetime lifetime, IPsiConfiguration psiConfiguration, LanguageManager languageManager, IPersistentIndexManager persistentIndexManager)
            : base(locks, solution, changeManager, lifetime, psiConfiguration, languageManager, persistentIndexManager)
        {
        }

        #region Methods
        
        #region Overrided methods

        protected override IList<FeatureXmlEntity> BuildData(IPsiSourceFile sourceFile)
        {
            JetHashSet<FeatureXmlEntity> jetHashSet = null;
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
                            jetHashSet = new JetHashSet<FeatureXmlEntity>(ItemsEqualityComparer);
                        
                        jetHashSet.Add(new FeatureXmlEntity()
                        {
                            Id = xmlTag.AttributeExists("featureId") ? new Guid(xmlTag.GetAttribute("featureId").UnquotedValue.Trim()) : Guid.Empty,
                            Title = xmlTag.AttributeExists("title") ? xmlTag.GetAttribute("title").UnquotedValue.Trim() : String.Empty,
                            Scope = ResolveScope(xmlTag.AttributeExists("scope") ? xmlTag.GetAttribute("scope").UnquotedValue.Trim() : String.Empty),
                            AlwaysForceInstall = xmlTag.AttributeExists("alwaysForceInstall") ? Convert.ToBoolean(xmlTag.GetAttribute("alwaysForceInstall").UnquotedValue.Trim()) : false,
                            ImageUrl = xmlTag.AttributeExists("imageUrl") ? xmlTag.GetAttribute("imageUrl").UnquotedValue.Trim() : String.Empty,
                            ReceiverAssembly = ResolveReceiverAssembly(sourceFile.GetProject(), xmlTag.AttributeExists("receiverAssembly") ? xmlTag.GetAttribute("receiverAssembly").UnquotedValue.Trim() : String.Empty),
                            ReceiverClass = xmlTag.AttributeExists("receiverClass") ? xmlTag.GetAttribute("receiverClass").UnquotedValue.Trim() : String.Empty,
                            ReceiverClassDeclaration = ResolveReceiverClass(sourceFile.GetProject(), xmlTag.AttributeExists("receiverClass") ? xmlTag.GetAttribute("receiverClass").UnquotedValue.Trim() : String.Empty),
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

        private SPFeatureScope ResolveScope(string s)
        {
            switch (s.ToLower().Trim())
            {
                case "farm":
                    return SPFeatureScope.Farm;
                case "webapplication":
                    return SPFeatureScope.WebApplication;
                case "site":
                    return SPFeatureScope.Site;
                default:
                    return SPFeatureScope.Web;
            }
        }

        private IClassDeclaration ResolveReceiverClass(IProject project, string s)
        {
            IClassDeclaration result = null;

            if (!String.IsNullOrEmpty(s))
            {
                const string regexp = @"[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}";
                if (Regex.IsMatch(s, regexp))
                {
                    string guid = Regex.Match(s, regexp).Value;
                    foreach (IProjectFile projectFile in project.GetAllProjectFiles())
                    {
                        if (projectFile.IsValid() && projectFile.LanguageType.Is<CSharpProjectFileType>())
                        {
                            var sourceFile = projectFile.ToSourceFile();
                            if (sourceFile != null && !sourceFile.Properties.IsGeneratedFile)
                            {
                                foreach (ICSharpFile csharpFile in sourceFile.GetPsiFiles<CSharpLanguage>())
                                {
                                    if (csharpFile != null)
                                    {
                                        foreach (IClassDeclaration classDeclaration in csharpFile.EnumerateSubTree().OfType<IClassDeclaration>())
                                        {
                                            if (
                                                classDeclaration.Attributes.Any(
                                                    attribute =>
                                                        attribute.Name.ShortName == "Guid" &&
                                                        attribute.Arguments.Any(
                                                            argument =>
                                                                argument.Value.IsConstantValue() &&
                                                                argument.Value.ConstantValue.Value != null &&
                                                                String.Equals(
                                                                    argument.Value.ConstantValue.Value.ToString(), guid,
                                                                    StringComparison.OrdinalIgnoreCase))))
                                            {
                                                result = classDeclaration;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        private string ResolveReceiverAssembly(IProject project, string s)
        {
            string result = String.Empty;

            if (!String.IsNullOrEmpty(s) && String.Equals(s, "$SharePoint.Project.AssemblyFullName$", StringComparison.OrdinalIgnoreCase))
            {
                result = project.GetOutputAssemblyFullName();
            }
            else
            {
                result = s;
            }

            return result;
        }

        #endregion

        public IEnumerable<FeatureXmlEntity> GetReceivers(SPFeatureScope scope)
        {
            return Items.Where(item => item.Scope == scope);
        }

        #endregion
    }

    [Serializable()]
    public class FeatureXmlEntity : SPXmlEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public SPFeatureScope Scope { get; set; }
        public bool AlwaysForceInstall { get; set; }
        public string ImageUrl { get; set; }
        public string ReceiverAssembly { get; set; }
        public string ReceiverClass { get; set; }
        
        public IClassDeclaration ReceiverClassDeclaration 
        {
            set
            {
                if (value != null)
                {
                    ReceiverClass = value.CLRName;
                    ReceiverClassReferences = value.EnumerateSubTree()
                        .OfType<IReferenceExpression>()
                        .Where(referenceExpression => referenceExpression.ReferenceExpressionTarget() is IMethod)
                        .Select(referenceExpression => referenceExpression.ReadableName())
                        .Where(s => !String.IsNullOrEmpty(s) && !s.Contains("Microsoft.") && !s.Contains("System."))
                        .Distinct()
                        .ToArray();
                }
            } 
        }
        public IEnumerable<string> ReceiverClassReferences { get; set; }

        public override string GetPropertyValue(string attributeName)
        {
            switch (attributeName)
            {
                case "Id":
                    return Id.ToString();
                case "Title":
                    return Title;
                case "Scope":
                    return Scope.ToString();
                case "AlwaysForceInstall":
                    return Convert.ToInt32(AlwaysForceInstall).ToString();
                case "ImageUrl":
                    return ImageUrl;
                case "ReceiverAssembly":
                    return ReceiverAssembly;
                case "ReceiverClass":
                    return ReceiverClass;
                default:
                    throw new ArgumentOutOfRangeException("attributeName");
            }
        }
    }

    public class FeatureXmlEntityEqualityComparer : IEqualityComparer<FeatureXmlEntity>
    {
        public bool Equals(FeatureXmlEntity x, FeatureXmlEntity y)
        {
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(FeatureXmlEntity obj)
        {
            return (obj.Id).GetHashCode();
        }
    }
}
