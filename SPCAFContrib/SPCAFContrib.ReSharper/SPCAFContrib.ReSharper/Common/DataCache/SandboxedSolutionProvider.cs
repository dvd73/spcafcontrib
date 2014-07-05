using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Caches;
using SPCAFContrib.ReSharper.Common.Extensions;

namespace SPCAFContrib.ReSharper.Common.DataCache
{
    [SolutionComponent]
    public class SandboxedSolutionProvider : IProjectFileDataCache
    {
        private readonly IShellLocks myLocks;
        private readonly ProjectFileDataCache myCache;

        public SandboxedSolutionProvider(Lifetime lifetime, IShellLocks locks, ProjectFileDataCache cache)
        {
            myLocks = locks;
            myCache = cache;
            cache.RegisterCache(lifetime, this);
        }

        /// <summary>
        /// ReSharper will also call Read method at the appropriate time, and you should use the Binary Reader to recreate the object that you need to cache. 
        /// Again, don’t store anything in class fields here.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public object Read(BinaryReader reader)
        {
            return reader.ReadBoolean();
        }

        /// <summary>
        /// ReSharper will call your Write method, passing in the cached object. 
        /// You serialise this to the BinaryWriter however you want to.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="data"></param>
        public void Write(BinaryWriter writer, object data)
        {
            writer.Write((bool)data);
        }

        public bool CanHandle(IProjectFile projectFile)
        {
            bool result = false;

            IProject project = projectFile.GetProject();
            if (project != null)
                result = project.IsClassicSolution();

            return result;
        }

        /// <summary>
        /// Return an object that represents your cached value(s).
        /// This could be a boolean, or a class. Don’t store anything in your class fields. ReSharper will cache this value.
        /// If the object instance is different to the previously cached instance, your OnDataChanged method is called.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public object BuildData(XmlDocument doc)
        {
            bool result = false;

            XDocument document = doc.ToXDocument();
            XElement sandboxedSolutionNode = document.Descendants().FirstOrDefault(p => p.Name.LocalName == "SandboxedSolution");

            if (sandboxedSolutionNode != null && !String.IsNullOrEmpty(sandboxedSolutionNode.Value))
                result = sandboxedSolutionNode.Value.Trim().ToLower() == "true";

            return result;
        }

        /// <summary>
        /// You should implement OnDataChanged only if you need to do something when the data changes. 
        /// If there’s no way for the user to change this value while the project is open, 
        /// then I suspect it’ll be fine to ignore the method and just return null.
        /// </summary>
        /// <param name="projectFile"></param>
        /// <param name="oldData"></param>
        /// <param name="newData"></param>
        /// <returns>You can decide what needs to happen in response to this change, and you should return it as an action that will get executed later (after all processing of the file completes).</returns>
        public Action OnDataChanged(IProjectFile projectFile, object oldData, object newData)
        {
            /*
            if (oldData != null)
            {
                bool oldValue = (bool)oldData;
                bool newValue = (bool)newData;

                if (newValue != oldValue)
                    return () => myLocks.ExecuteWithWriteLock(() =>
                    {
                        IProject project = projectFile.GetProject();
                        Assertion.AssertNotNull(project, "project must not be null");

                        // ProjectModelChangeUtil.OnChange is just a helper function that will batch up changes to the change manager. 
                        // It ensures that all changes are "propagated", e.g. if given a change notification for a project item, 
                        // it makes sure that gets propagated up to the project itself. 
                        // It then uses IProjectModelBatchChangeManager to post the notification, 
                        // which will either add the change to a current transaction, or execute it in its own, new transaction. 
                        // I.e. the change happens as part of a new batch, of gets added to the existing batch. 
                        // The batch manager then posts it to the change manager, which is an event bus that components can subscribe to in order to be notified of changes.
                        // You can see the change manager subscriptions as a graph from the internal menu (ReSharper -> Internal -> View Change Manager Graph). 
                        ProjectModelChangeUtil.OnChange(projectFile.GetSolution().BatchChangeManager,
                            new ProjectItemChange(EmptyList<ProjectModelChange>.InstanceList, project,
                                project.ParentFolder, ProjectModelChangeType.PROPERTIES, project.Location,
                                project.GetPersistentID()));
                    });
            }
            */
            return null;
        }

        public int Version {
            get { return 0; }
        }

        public bool IsSandboxed(IProject project)
        {
            return myCache.GetData(this, project, false);
        }
    }
}
