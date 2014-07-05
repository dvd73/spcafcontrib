using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.DataFlow;
using JetBrains.DocumentManagers.impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util.Caches;
using JetBrains.ReSharper.Psi.Xml;
using JetBrains.ReSharper.Psi.Xml.Tree;
using JetBrains.Util;
using SPCAFContrib.ReSharper.Common.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace SPCAFContrib.ReSharper.Common.DataCache
{
    public abstract class SPXmlEntityCache<T> : ICache where T : SPXmlEntity
    {
        #region Fields
        private readonly ISolution _solution;
        private readonly IShellLocks _locks;
        private readonly IPsiConfiguration _psiConfiguration;
        private readonly LanguageManager _languageManager;
        private readonly IPersistentIndexManager _persistentIndexManager;
        private SimplePersistentCache<IList<T>> _persistentCache;

        protected readonly object LockObject = new object();
        protected readonly JetHashSet<IPsiSourceFile> DirtySourceFiles;
        protected readonly JetHashSet<IPsiSourceFile> FilesWithValidData;
        protected readonly OneToSetMap<IPsiSourceFile, T> ProjectFileToItems;
        protected readonly OneToSetMap<T, IPsiSourceFile> ItemsToProjectFiles; 
        #endregion

        #region Properties
        protected virtual string XmlSchemaName
        {
            get { return "http://schemas.microsoft.com/sharepoint"; }
        }
        protected abstract string XmlSchemaContainerXPath { get; }
        protected abstract string XmlEntityXPath { get; }
        protected abstract string CacheDirectoryName { get; }
        public abstract IEqualityComparer<T> ItemsEqualityComparer { get; } 
        #endregion

        #region Ctor
        public SPXmlEntityCache(IShellLocks locks, ISolution solution, ChangeManager changeManager,
            Lifetime lifetime, IPsiConfiguration psiConfiguration, LanguageManager languageManager,
            IPersistentIndexManager persistentIndexManager)
        {
            _locks = locks;
            _psiConfiguration = psiConfiguration;
            _languageManager = languageManager;
            _solution = solution;
            _persistentIndexManager = persistentIndexManager;

            DirtySourceFiles = new JetHashSet<IPsiSourceFile>();
            FilesWithValidData = new JetHashSet<IPsiSourceFile>();

            ProjectFileToItems = new OneToSetMap<IPsiSourceFile, T>();
            ItemsToProjectFiles = new OneToSetMap<T, IPsiSourceFile>(ItemsEqualityComparer);
        }
        #endregion

        #region Methods

        #region Public contract

        [IndexerName("SourceFiles")]
        public virtual IEnumerable<IPsiSourceFile> this[T item]
        {
            get
            {
                lock (LockObject)
                {
                    return ItemsToProjectFiles[item].ToArray();
                }
            }
        }

        #region Public contract
        public IEnumerable<T> Items
        {
            get { return ItemsToProjectFiles.Keys; }
        }

        public IEnumerable<IPsiSourceFile> Files
        {
            get { return ProjectFileToItems.Keys; }
        }

        public virtual IEnumerable<IPsiSourceFile> GetDuplicates(IXmlTag element, string attributeName)
        {
            List<IPsiSourceFile> result = new List<IPsiSourceFile>();
            IXmlAttribute attribute = element.GetAttribute(attributeName);
            string attValue = attribute.UnquotedValue.Trim();
            IPsiSourceFile sourceFile = element.GetSourceFile();
            TreeOffset offset = element.GetTreeStartOffset();

            lock (LockObject)
            {
                IEnumerable<T> keys =
                    ItemsToProjectFiles.Keys.Where(
                        key =>
                            String.Equals(key.GetPropertyValue(attributeName), attValue,
                                StringComparison.OrdinalIgnoreCase));

                foreach (T key in keys)
                {
                    ICollection<IPsiSourceFile> files = ItemsToProjectFiles.GetValuesSafe(key);
                    result.AddRange(
                        files.Where(
                            file =>
                                !file.Equals(sourceFile) ||
                                (file.Equals(sourceFile) && !key.Offset.Equals(offset))));
                }
            }

            if (result.Count == 0)
                return EmptyList<IPsiSourceFile>.InstanceList;
            else
                return result;
        }

        #endregion
        #endregion

        #region ICache members
        void ICache.MarkAsDirty(IPsiSourceFile sourceFile)
        {
            if (!ShouldBeProcessed(sourceFile))
                return;

            lock (LockObject)
            {
                DirtySourceFiles.Add(sourceFile);
                FilesWithValidData.Remove(sourceFile);
            }
        }

        object ICache.Load(IProgressIndicator progress, bool enablePersistence)
        {
            if (!enablePersistence)
                return null;

            Assertion.Assert(_persistentCache == null, "_persistentCache == null");

            // FormatVersion = this is the version of the format of the cache on disk. 
            // If the version in the on-disk serialisation of the cache matches the implementation’s version, the cache is loaded. 
            // If the versions are different, the implementation won’t be able to de-serialise the cache, and the on-disk cache is discarded, meaning the cache is rebuilt.
            _persistentCache = new SimplePersistentCache<IList<T>>(_locks, 1, CacheDirectoryName, _psiConfiguration);

            if (_persistentCache.Load(progress, _persistentIndexManager,
                (file, reader) => // read data handler
                {
                    int itemsCount = reader.ReadInt32();
                    if (itemsCount == 0)
                        return EmptyList<T>.InstanceList;

                    List<T> list = new List<T>(itemsCount);

                    for (int i = 0; i < itemsCount; ++i)
                    {
                        int sizeofT = reader.ReadInt32();
                        list.Add(DeserializeFromBytes(reader.ReadBytes(sizeofT)) as T);
                    }

                    return (IList<T>)list;
                },
                (projectFile, data) => // data loaded handler
                {
                    lock (LockObject)
                        ((ICache)this).Merge(projectFile, data);
                }) != LoadResult.OK)
            {
                // load failed
                lock (LockObject)
                {
                    ItemsToProjectFiles.Clear();
                    ProjectFileToItems.Clear();
                    DirtySourceFiles.Clear();
                    FilesWithValidData.Clear();
                }
            }

            // set up the in-memory cache and avoid ICache.MergeLoaded
            return null; 
        }

        void ICache.Save(IProgressIndicator progress, bool enablePersistence)
        {
            if (!enablePersistence)
                return;

            lock (LockObject)
            {
                Assertion.Assert(_persistentCache != null, "_persistentCache != null");

                _persistentCache.Save(progress, _persistentIndexManager, (writer, file, data) =>
                {
                    writer.Write(data.Count);
                    foreach (T item in data)
                    {
                        byte[] rawdata = SerializeToBytes(item);
                        writer.Write(rawdata.Length);
                        writer.Write(rawdata);
                    }
                });

                _persistentCache.Dispose();
                _persistentCache = null;
            }
        }

        void ICache.MergeLoaded(object data)
        {

        }

        bool ICache.UpToDate(IPsiSourceFile sourceFile)
        {
            if (!ShouldBeProcessed(sourceFile))
                return true;

            if (DirtySourceFiles.Contains(sourceFile))
                return false;

            return FilesWithValidData.Contains(sourceFile);
        }

        object ICache.Build(IPsiSourceFile sourceFile, bool isStartup)
        {
            if (ShouldBeProcessed(sourceFile))
                return BuildData(sourceFile);
            else
            {
                return null;
            }
        }

        void ICache.Merge(IPsiSourceFile sourceFile, object builtPart)
        {
            if (builtPart == null)
                return;

            lock (LockObject)
            {
                FilesWithValidData.Add(sourceFile);
                DirtySourceFiles.Remove(sourceFile);
                RemoveData(sourceFile);
                IList<T> newItems = builtPart as IList<T>;

                Assertion.Assert((newItems != null ? 1 : 0) != 0, "newItems != null: {0}", new[] { builtPart });

                AddData(sourceFile, newItems);

                if (_persistentCache == null)
                    return;

                _persistentCache.AddDataToSave(sourceFile, ProjectFileToItems[sourceFile].ToArray());
            }
        }

        void ICache.Drop(IPsiSourceFile sourceFile)
        {
            if (!ShouldBeProcessed(sourceFile))
                return;

            lock (LockObject)
            {
                FilesWithValidData.Remove(sourceFile);
                DirtySourceFiles.Remove(sourceFile);
                RemoveData(sourceFile);
            }
        }

        void ICache.OnPsiChange(ITreeNode elementContainingChanges, PsiChangedElementType type)
        {
            if (elementContainingChanges != null)
            {
                IPsiSourceFile sourceFile = elementContainingChanges.GetSourceFile();
                ((ICache)this).MarkAsDirty(sourceFile);
            }
        }

        void ICache.OnDocumentChange(IPsiSourceFile sourceFile, ProjectFileDocumentCopyChange change)
        {
            ((ICache)this).MarkAsDirty(sourceFile);
        }

        void ICache.SyncUpdate(bool underTransaction)
        {
            lock (LockObject)
            {
                // Make a copy of the enumeration because collection is goig to be changed
                foreach (IPsiSourceFile file in DirtySourceFiles.ToList())
                {
                    ((ICache)this).Merge(file, BuildData(file));
                }
            }
        }

        bool ICache.HasDirtyFiles
        {
            get
            {
                lock (LockObject)
                {
                    return !DirtySourceFiles.IsEmpty();
                }
            }
        } 
        #endregion

        #region Implementation details
        
        protected virtual void RemoveData(IPsiSourceFile sourceFile)
        {
            lock (LockObject)
            {
                ICollection<T> items = ProjectFileToItems[sourceFile];
                ProjectFileToItems.RemoveKey(sourceFile);

                foreach (T item in items)
                    ItemsToProjectFiles.Remove(item, sourceFile);
            }
        }

        protected virtual void AddData(IPsiSourceFile sourceFile, [NotNull] IList<T> newItems)
        {
            lock (LockObject)
            {
                if (!newItems.Any())
                    return;

                ProjectFileToItems.AddRange(sourceFile, newItems);

                foreach (T item in newItems)
                    ItemsToProjectFiles.Add(item, sourceFile);
            }
        }

        protected virtual byte[] SerializeToBytes(T item)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, item);
                stream.Seek(0, SeekOrigin.Begin);
                return stream.ToArray();
            }
        }

        protected virtual object DeserializeFromBytes(byte[] bytes)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(bytes))
            {
                return formatter.Deserialize(stream);
            }
        }

        protected virtual bool ShouldBeProcessed(IPsiSourceFile sourceFile)
        {
            bool result = false;

            IFile file = sourceFile.GetDominantPsiFile<XmlLanguage>();
            if (file != null)
            {
                if (file is IXmlFile)
                {
                    IXmlFile xmlFile = file as IXmlFile;

                    IXmlTag validatedTag = xmlFile.GetNestedTags<IXmlTag>(XmlSchemaContainerXPath).FirstOrDefault();

                    if (validatedTag != null)
                    {
                        // check xmlns="http://schemas.microsoft.com/sharepoint/" to be sure this is xml with sharepoint schema
                        result = validatedTag.CheckAttributeValue("xmlns", XmlSchemaName);
                        
                        if (result)
                            result = xmlFile.GetNestedTags<IXmlTag>(XmlEntityXPath).Any();
                    }
                }
            }

            return result;
        }

        protected abstract IList<T> BuildData(IPsiSourceFile sourceFile);

        #endregion

        #endregion
    }

    [Serializable()]
    public abstract class SPXmlEntity
    {
        public TreeOffset Offset { get; set; }

        public abstract string GetPropertyValue(string attributeName);
    }
}
