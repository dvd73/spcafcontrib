using System;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Properties.Managed;
using SPCAFContrib.ReSharper.Common.Attributes;
using SPCAFContrib.ReSharper.Common.DataCache;

namespace SPCAFContrib.ReSharper.Common.Extensions
{
    public static class IProjectExtension
    {
        public static bool IsApplicableFor(this IProject project, object actionObject)
        {
            bool result = false;
            IManagedProjectBuildSettings managedProjectBuildSettings = project.ProjectProperties.BuildSettings as IManagedProjectBuildSettings;
            ApplicabilityAttribute oAttribute = actionObject.GetType().GetCustomAttributes(typeof(ApplicabilityAttribute), true).FirstOrDefault() as ApplicabilityAttribute;

            if (managedProjectBuildSettings != null && oAttribute != null)
            {
                #region Current project type variables
                bool hasSPServerReference = project.HasSPServerReference() &&
                                                    (managedProjectBuildSettings.OutputType == ProjectOutputType.LIBRARY ||
                                                     managedProjectBuildSettings.OutputType == ProjectOutputType.CONSOLE_EXE ||
                                                     managedProjectBuildSettings.OutputType == ProjectOutputType.WIN_EXE);
                bool isSP2010FarmSolution = /*hasSPServerReference &&*/ managedProjectBuildSettings.OutputType == ProjectOutputType.LIBRARY && !project.IsSandboxed() &&
                    project.ProjectProperties.ProjectTypeGuids.All(t => Consts.Consts.SP2010SolutionProjectTypeGuids.Contains(t));
                bool isSP2013FarmSolution = /*hasSPServerReference &&*/ managedProjectBuildSettings.OutputType == ProjectOutputType.LIBRARY && !project.IsSandboxed() &&
                    project.ProjectProperties.ProjectTypeGuids.All(t => Consts.Consts.SP2013SolutionProjectTypeGuids.Contains(t));
                bool isSP2013AppSolution = project.HasSPClientReference() && managedProjectBuildSettings.OutputType == ProjectOutputType.LIBRARY &&
                    project.ProjectProperties.ProjectTypeGuids.All(t => Consts.Consts.SP2013AppsSolutionProjectTypeGuids.Contains(t));
                bool isSandboxProject = project.IsSharepointWorkflow() && project.IsSandboxed(); 
                #endregion

                if (isSP2010FarmSolution)
                {
                    result = (oAttribute.ProjectType & IDEProjectType.SP2010FarmSolution) ==
                             IDEProjectType.SP2010FarmSolution;
                }
                else if (isSP2013FarmSolution)
                {
                    result = (oAttribute.ProjectType & IDEProjectType.SP2013FarmSolution) ==
                             IDEProjectType.SP2013FarmSolution;
                }
                else if (isSP2013AppSolution)
                {
                    result = (oAttribute.ProjectType & IDEProjectType.SP2013App) ==
                             IDEProjectType.SP2013App;
                }
                else if (isSandboxProject)
                {
                    result = (oAttribute.ProjectType & IDEProjectType.SPSandbox) ==
                                IDEProjectType.SPSandbox;   
                }
                else if (hasSPServerReference)
                {
                    result = (oAttribute.ProjectType & IDEProjectType.SPServerAPIReferenced) ==
                                IDEProjectType.SPServerAPIReferenced;  
                }
                
            }

            return result;
        }

        public static bool IsClassicSolution(this IProject project)
        {
            bool result = false;

            IManagedProjectBuildSettings managedProjectBuildSettings = project.ProjectProperties.BuildSettings as IManagedProjectBuildSettings;

            if (managedProjectBuildSettings != null)
            {
                result = /*project.HasSPServerReference() &&*/
                         managedProjectBuildSettings.OutputType == ProjectOutputType.LIBRARY && (
                             project.ProjectProperties.ProjectTypeGuids.All(
                                 t => Consts.Consts.SP2010SolutionProjectTypeGuids.Contains(t) ||
                                      Consts.Consts.SP2013SolutionProjectTypeGuids.Contains(t)));
            }

            return result;
        }

        public static bool HasSPClientReference(this IProject project)
        {
            return project.GetAssemblyReferences().Any(r => r.Name == "Microsoft.SharePoint.Client.Runtime");
        }

        public static bool HasSPServerReference(this IProject project)
        {
            return project.GetAssemblyReferences().Any(r => r.Name == "Microsoft.SharePoint");
        }

        public static bool HasSecureStoreServiceReference(this IProject project)
        {
            return project.GetAssemblyReferences().Any(r => r.Name == "Microsoft.Office.SecureStoreService");
        }

        public static bool IsSandboxed(this IProject project)
        {
            bool result = false;

            ISolution solution = project.GetSolution();
            var solutionComponent = solution.GetComponent<SandboxedSolutionProvider>();
            result = solutionComponent.IsSandboxed(project);

            //if (project.ProjectFile != null)
            //    using (Stream stream = project.ProjectFile.CreateReadStream())
            //    {
            //        XDocument document = XDocument.Load(stream);
            //        XElement sandboxedSolutionNode = document.Descendants().FirstOrDefault(p => p.Name.LocalName == "SandboxedSolution");

            //        if (sandboxedSolutionNode != null && !String.IsNullOrEmpty(sandboxedSolutionNode.Value))
            //            result = sandboxedSolutionNode.Value.ToLower() == "true";
            //    }

            return result;
        }

        public static string GetOutputAssemblyFullName(this IProject project)
        {
            string result = String.Empty;

            var outputAssemblyInfo = project.GetOutputAssemblyInfo();
            if (outputAssemblyInfo != null)
                result = outputAssemblyInfo.AssemblyNameInfo.FullName;
            else
                result = project.Guid.ToString();

            return result;
        }
    }
}
