using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Experimental.Rules.Code
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
        CheckId = CheckIDs.Rules.Assembly.AvoidInappropriateDataAccess,
        DisplayName = "AvoidInappropriateDataAccess",
        Description = "",
        DefaultSeverity = Severity.CriticalWarning,
        SharePointVersion = new[] { "12", "14", "15" },
        Message = "AvoidInappropriateDataAccess",
        Resolution = "")]
    public class AvoidInappropriateDataAccess : Rule<AssemblyFileReference>
    {
        #region static

        static AvoidInappropriateDataAccess()
        {
            RestrictedParentWebControls.AddRange(TypeInfo.WebControlAndPages);
            RestrictedParentWebControls.AddRange(TypeInfo.SPTimerJobs);
            RestrictedParentWebControls.AddRange(TypeInfo.SPEventReceivers);

            // probably, will include just by mask, like
            // get_*, set_* and so on
            // let see how it works now and will adjust later

            DataAccessMethods.Add(TypeKeys.SPQuery, new List<string>()
            {
                ".ctor",
                "get_Query",
                "set_Query",
                "get_RowLimit",
                "set_RowLimit",
                "get_ViewFields",
                "set_ViewFields"
            });

            DataAccessMethods.Add(TypeKeys.PortalSiteMapProvider, new List<string>()
            {
                ".ctor",
                "GetChildNodes",
                "GetCachedListItemsByQuery",
                "FindSiteMapNode",
                "GetCachedList",
                "GetCachedListItemsByQuery",
                "GetCachedSiteDataQuery",
                "get_CurrentSite",
                "set_CurrentSite",
                "get_CurrentWeb",
                "set_CurrentWeb"
            });

            DataAccessMethods.Add(TypeKeys.CrossListQueryCache, new List<string>()
            {
                ".ctor",
                "GetSiteData",
                "GetSiteDataResults"
            });

            DataAccessMethods.Add(TypeKeys.SPDataSourceView, new List<string>()
            {
                ".ctor",
                "Select"
            });

            DataAccessMethods.Add(TypeKeys.SPDataSource, new List<string>()
            {
                ".ctor",
                "get_List",
                "set_List",
                "get_DataSourceMode",
                "set_DataSourceMode",
                "get_Scope",
                "set_Scope"
            });

            DataAccessMethods.Add(TypeKeys.SPSiteDataQuery, new List<string>()
            {
                ".ctor"
            });

            DataAccessMethods.Add(TypeKeys.FullTextSqlQuery, new List<string>()
            {
                ".ctor"
            });

            DataAccessMethods.Add(TypeKeys.KeywordQuery, new List<string>()
            {
                ".ctor"
            });

            DataAccessMethods.Add(TypeKeys.SPSite, new List<string>()
            {
                "OpenWeb",
                "get_Features",
                "get_Cache",
                "get_AllWebs",
                "get_RootWeb",
            });

            DataAccessMethods.Add(TypeKeys.SPWeb, new List<string>() {
                "get_Files",
                "get_Lists",
                "GetFile",
                "get_Webs",
                "get_Workflows",
                "get_Users",
                "get_Alerts",
                "get_AllUsers",
                "get_Properties",
                "get_AllProperties",
                "get_Modules",
                "get_ListTemplates",
                "get_IsRoot",
                "get_Features",
            });

            DataAccessMethods.Add(TypeKeys.SPList, new List<string>() {
                "get_Items",
                "GetItemById",
                "GetItemByIdAllFields",
                "GetItemByIdSelectedFields",
                "GetItemByUniqueId",
                "GetItems",
                "GetItemsWithUniquePermissions",
                "get_Forms",
                "get_Folders",
                "get_Fields"
            });
        }

        #endregion

        #region properties

        protected static List<string> RestrictedParentWebControls = new List<string>();

        private static readonly object RestrictedWebControlsLock = new object();
        private static List<TypeDefinition> _restrictedWebControls;

        protected static Dictionary<string, List<string>> DataAccessMethods = new Dictionary<string, List<string>>();

        #endregion

        #region methods

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            TypeDefinition[] restrictedControlTypes = ResolveRestrictedWebControls(assembly).ToArray();

            foreach (string dataAccessClass in DataAccessMethods.Keys)
            {
                foreach (string dataAccessMethod in DataAccessMethods[dataAccessClass])
                {
                    string targetTypeName = dataAccessClass;
                    string targetMethodName = dataAccessMethod;

                    foreach (CodeInstruction methodCall in assembly.MethodInvocationInstructions(targetTypeName, targetMethodName))
                    {
                        string methodHostType = methodCall.MethodDefinition.DeclaringType.FullName;
                        bool isRestrictedTypeCall = restrictedControlTypes.Count(c => string.Compare(c.FullName, methodHostType, true) == 0) > 0;

                        if (isRestrictedTypeCall)
                        {
                            Notify(assembly,
                                string.Format("Inappropriate data access in class:[{0}] using:[{1}]", methodHostType, targetMethodName),
                                methodCall.ImproveSummary(assembly.GetSummary()), notifications);
                        }
                    }
                }
            }
        }

        #region utils

        private static IEnumerable<TypeDefinition> ResolveRestrictedWebControls(AssemblyFileReference assembly)
        {
            lock (RestrictedWebControlsLock)
            {
                if (_restrictedWebControls == null)
                {
                    _restrictedWebControls = new List<TypeDefinition>();

                    foreach (string allowedParentType in RestrictedParentWebControls)
                    {
                        _restrictedWebControls.AddRange(assembly.TypesThatDerivesFromType(allowedParentType));
                    }
                }
            }

            return _restrictedWebControls;
        }

        #endregion

        #endregion
    }
}
