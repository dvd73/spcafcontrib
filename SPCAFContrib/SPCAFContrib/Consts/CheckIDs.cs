namespace SPCAFContrib.Consts
{
    public static class CheckIDs
    {
        #region properties

        public const string SPCAFContribRulePrefix = "CSC";
        public const string SPCAFContribMetricPrefix = "CSM";
        public const string SPCAFContribInventoryPrefix = "CSI";

        public const string SPCAFContribCategory = "51";

        #endregion

        public static class Rules
        {
            public static class TemplateFiles
            {
                private const string Prefix = SPCAFContribRulePrefix + SPCAFContribCategory + InspectedSPElement.TemplateFiles;

                public const string UnresolvedTokenAssemblyFullName = Prefix + "01";
                public const string UnresolvedTokenAssemblyFullName_HelpUrl = HelpSite.HelpPageUrl + UnresolvedTokenAssemblyFullName + "_UnresolvedTokenAssemblyFullName";
            }

            public static class ListInstance
            {
                private const string Prefix = SPCAFContribRulePrefix + SPCAFContribCategory + InspectedSPElement.ListInstance;

                public const string AvoidDataRowsInListInstances = Prefix + "01";
                public const string AvoidDataRowsInListInstances_HelpUrl = HelpSite.HelpPageUrl + AvoidDataRowsInListInstances + "_AvoidDataRowsInListInstances";

                public const string UniqueListInstanceUrl = Prefix + "02";
                public const string UniqueListInstanceUrl_HelpUrl = HelpSite.HelpPageUrl + UniqueListInstanceUrl + "_UniqueListInstanceUrl";

                public const string DoNotUseSystemListNames = Prefix + "03";
                public const string DoNotUseSystemListNames_HelpUrl = HelpSite.HelpPageUrl + DoNotUseSystemListNames + "_DoNotUseSystemListNames";

            }

            public static class ASPXMasterPage
            {
                private const string Prefix = SPCAFContribRulePrefix + SPCAFContribCategory + InspectedSPElement.MasterPage;

                public const string AvoidDollarGlobalVariableInMasterPage = Prefix + "01";
                public const string SPDataSourceScopeDoesNotDefinedInMasterPage = Prefix + "02";
                public const string AvoidJQueryDocumentReadyInMasterPage = Prefix + "03";
            }

            public static class ASPXPage
            {
                private const string Prefix = SPCAFContribRulePrefix + SPCAFContribCategory + InspectedSPElement.ASPXPage;
                
                public const string AvoidDollarGlobalVariableInPage = Prefix + "01";
                public const string SPDataSourceScopeDoesNotDefinedInPage = Prefix + "02";
                public const string AvoidJQueryDocumentReadyInPage = Prefix + "03";
            }

            public static class ASCXFile
            {
                private const string Prefix = SPCAFContribRulePrefix + SPCAFContribCategory + InspectedSPElement.ASCXControl;

                public const string AvoidDollarGlobalVariableInControl = Prefix + "01";
                public const string SPDataSourceScopeDoesNotDefinedInControl = Prefix + "02";
                
                public const string AvoidUsingRenderingTemplate = Prefix + "03";
                public const string AvoidUsingRenderingTemplate_HelpUrl = HelpSite.HelpPageUrl + AvoidUsingRenderingTemplate + "_AvoidUsingRenderingTemplate";

                public const string AvoidJQueryDocumentReadyInControl = Prefix + "04";
            }

            public static class General
            {
                private const string Prefix = SPCAFContribRulePrefix + SPCAFContribCategory + InspectedSPElement.General;

                public const string PowerShellHostAssemblyFileReferenceRule = Prefix + "01";
                public const string PowerShellHostFeatureDefinitionRule = Prefix + "02";

                public const string LoggerStatisticCollector = Prefix + "03";
                public const string LoggerStatisticCollector_HelpUrl = HelpSite.HelpPageUrl + LoggerStatisticCollector + "_LoggerStatisticCollector";

                public const string AvoidDollarGlobalVariable_HelpUrl = HelpSite.HelpPageUrl + "AvoidDollarGlobalVariable";
                public const string SPDataSourceScopeDoesNotDefined_HelpUrl = HelpSite.HelpPageUrl + "SPDataSourceScopeDoesNotDefined";
                public const string AvoidJQueryDocumentReady_HelpUrl = HelpSite.HelpPageUrl + "AvoidJQueryDocumentReady"; 
            }

            public static class ContentType
            {
                private const string Prefix = SPCAFContribRulePrefix + SPCAFContribCategory + InspectedSPElement.ContentType;

                public const string AvoidListContentTypes = Prefix + "01";
                public const string AvoidListContentTypes_HelpUrl = HelpSite.HelpPageUrl + AvoidListContentTypes + "_AvoidListContentTypes";

                public const string DoNotDefineMultipleContentTypeGroupInOneElementFile = Prefix + "02";
                public const string DoNotDefineMultipleContentTypeGroupInOneElementFile_HelpUrl = HelpSite.HelpPageUrl + DoNotDefineMultipleContentTypeGroupInOneElementFile + "_DoNotDefineMultipleContentTypeGroupInOneElementFile";
                
                public const string DeployContentTypesCorrectly = Prefix + "03";
                public const string DeployContentTypesCorrectly_HelpUrl = HelpSite.HelpPageUrl + DeployContentTypesCorrectly + "_DeployContentTypesCorrectly";

                public const string ConsiderOverwriteAttributeForContentType = Prefix + "04";
                public const string ConsiderOverwriteAttributeForContentType_HelpUrl = HelpSite.HelpPageUrl + ConsiderOverwriteAttributeForContentType + "_ConsiderOverwriteAttributeForContentType";
            }

            public static class ListTemplate
            {
                private const string Prefix = SPCAFContribRulePrefix + SPCAFContribCategory + InspectedSPElement.ListTemplate;

                public const string ConsiderHiddenListTemplates = Prefix + "01";
                public const string ConsiderHiddenListTemplates_HelpUrl = HelpSite.HelpPageUrl + ConsiderHiddenListTemplates + "_ConsiderHiddenListTemplates";

                public const string DeclareEmptyFieldsElement = Prefix + "02";
                public const string DeclareEmptyFieldsElement_HelpUrl = HelpSite.HelpPageUrl + DeclareEmptyFieldsElement + "_DeclareEmptyFieldsElement";

                public const string DoNotDeployTaxonomyFieldsInList = Prefix + "03";
                public const string DoNotDeployTaxonomyFieldsInList_HelpUrl = HelpSite.HelpPageUrl + DoNotDeployTaxonomyFieldsInList + "_DoNotDeployTaxonomyFieldsInListDefinition";

                public const string EnsureFolderContentTypeInListDefinition = Prefix + "04";
                public const string EnsureFolderContentTypeInListDefinition_HelpUrl = HelpSite.HelpPageUrl + EnsureFolderContentTypeInListDefinition + "_EnsureFolderContentTypeInListDefinition";

                public const string DoNotAllowDeletionForList = Prefix + "05";
                public const string DoNotAllowDeletionForList_HelpUrl = HelpSite.HelpPageUrl + DoNotAllowDeletionForList + "_DoNotAllowDeletionForList";

            }

            public static class FieldTemplate
            {
                private const string Prefix = SPCAFContribRulePrefix + SPCAFContribCategory + InspectedSPElement.Field;

                public const string DeployTaxonomyFieldsCorrectly = Prefix + "01";
                public const string DeployTaxonomyFieldsCorrectly_HelpUrl = HelpSite.HelpPageUrl + DeployTaxonomyFieldsCorrectly + "_DeployTaxonomyFieldsCorrectly";

                public const string DoNotDefineMultipleFieldGroupInOneElementFile = Prefix + "02";
                public const string DoNotDefineMultipleFieldGroupInOneElementFile_HelpUrl = HelpSite.HelpPageUrl + DoNotDefineMultipleFieldGroupInOneElementFile + "_DoNotDefineMultipleFieldGroupInOneElementFile";

                public const string DeployFieldsCorrectly = Prefix + "03";
                public const string DeployFieldsCorrectly_HelpUrl = HelpSite.HelpPageUrl + DeployFieldsCorrectly + "_DeployFieldsCorrectly";

                public const string ConsiderOverwriteAttributeForFields = Prefix + "04";
                public const string ConsiderOverwriteAttributeForFields_HelpUrl = HelpSite.HelpPageUrl + ConsiderOverwriteAttributeForFields + "_ConsiderOverwriteAttributeForFields";

                public const string FieldIdShouldBeUppercase = Prefix + "05";
                public const string FieldIdShouldBeUppercase_HelpUrl = HelpSite.HelpPageUrl + FieldIdShouldBeUppercase + "_FieldIdShouldBeUppercase";

                public const string NameWithPictureForUserField = Prefix + "06";
                public const string NameWithPictureForUserField_HelpUrl = HelpSite.HelpPageUrl + NameWithPictureForUserField + "_NameWithPictureForUserField";

                public const string DoNotAllowDeletionForField = Prefix + "07";
                public const string DoNotAllowDeletionForField_HelpUrl = HelpSite.HelpPageUrl + DoNotAllowDeletionForField + "_DoNotAllowDeletionForColumn";

            }

            public static class WebPart
            {
                private const string Prefix = SPCAFContribRulePrefix + SPCAFContribCategory + InspectedSPElement.WebPart;

                public const string WebPartDefinitionMightBeImproved = Prefix + "01";
                public const string WebPartDefinitionMightBeImproved_HelpUrl = HelpSite.HelpPageUrl + WebPartDefinitionMightBeImproved + "_WebPartDefinitionMightBeImproved";

                public const string WebPartModuleDefinitionMightbeImproved = Prefix + "02";
                public const string WebPartModuleDefinitionMightbeImproved_HelpUrl = HelpSite.HelpPageUrl + WebPartModuleDefinitionMightbeImproved + "_WebPartModuleDefinitionMightbeImproved";

                public const string AvoidDollarGlobalVariableInWebPart = Prefix + "03";
                public const string AvoidJQueryDocumentReadyInWebPart = Prefix + "04";
            }

            public static class Assembly
            {
                private const string Prefix = SPCAFContribRulePrefix + SPCAFContribCategory + InspectedSPElement.Assembly;

                public const string ThreadSleepShouldNotBeUsed = Prefix + "01";
                public const string ThreadSleepShouldNotBeUsed_HelpUrl = HelpSite.HelpPageUrl + ThreadSleepShouldNotBeUsed + "_ThreadSleepShouldNotBeUsed";

                public const string ConfigurationManagerShouldNotBeUsed = Prefix + "02";
                public const string ConfigurationManagerShouldNotBeUsed_HelpUrl = HelpSite.HelpPageUrl + ConfigurationManagerShouldNotBeUsed + "_ConfigurationManagerShouldNotBeUsed";

                public const string AvoidUsingListFieldIterator = Prefix + "03";
                public const string AvoidUsingListFieldIterator_HelpUrl = HelpSite.HelpPageUrl + AvoidUsingListFieldIterator + "_AvoidUsingListFieldIterator";

                public const string AvoidEnumeratingAllUserProfiles = Prefix + "04";
                public const string AvoidEnumeratingAllUserProfiles_HelpUrl = HelpSite.HelpPageUrl + AvoidEnumeratingAllUserProfiles + "_AvoidEnumeratingAllUserProfiles";

                public const string SPViewScopeDoesNotDefined = Prefix + "05";
                public const string SPViewScopeDoesNotDefined_HelpUrl = HelpSite.HelpPageUrl + SPViewScopeDoesNotDefined + "_SPViewScopeDoesNotDefined";

                public const string UseSecureStoreService = Prefix + "06";
                public const string UseSecureStoreService_HelpUrl = HelpSite.HelpPageUrl + UseSecureStoreService + "_UseSecureStoreService";

                public const string BetterSPFeatureReceiverDesign = Prefix + "07";
                public const string BetterSPFeatureReceiverDesign_HelpUrl = HelpSite.HelpPageUrl + BetterSPFeatureReceiverDesign + "_BetterSPFeatureReceiverDesign";

                public const string SPMonitoredScopeShouldBeUsed = Prefix + "08";
                public const string SPMonitoredScopeShouldBeUsed_HelpUrl = HelpSite.HelpPageUrl + SPMonitoredScopeShouldBeUsed + "_SPMonitoredScopeShouldBeUsed";

                public const string InappropriateUsageOfSPListCollection = Prefix + "09";
                public const string InappropriateUsageOfSPListCollection_HelpUrl = HelpSite.HelpPageUrl + InappropriateUsageOfSPListCollection + "_InappropriateUsageOfSPListCollection";

                public const string SPQueryScopeDoesNotDefined = Prefix + "10";
                public const string SPQueryScopeDoesNotDefined_HelpUrl = HelpSite.HelpPageUrl + SPQueryScopeDoesNotDefined + "_SPQueryScopeDoesNotDefined";

                public const string AvoidUsingAjaxControlToolkit = Prefix + "11";
                public const string AvoidUsingAjaxControlToolkit_HelpUrl = HelpSite.HelpPageUrl + AvoidUsingAjaxControlToolkit + "_AvoidUsingAjaxControlToolkit";

                public const string DoNotUseDirectorySearcher = Prefix + "12";
                public const string DoNotUseDirectorySearcher_HelpUrl = HelpSite.HelpPageUrl + DoNotUseDirectorySearcher + "_DoNotUseDirectorySearcher";

                public const string DoNotUseUnsafeTypeConversionOnSPListItem = Prefix + "13";
                public const string DoNotUseUnsafeTypeConversionOnSPListItem_HelpUrl = HelpSite.HelpPageUrl + DoNotUseUnsafeTypeConversionOnSPListItem + "_DoNotUseUnsafeTypeConversionOnSPListItem";

                public const string SPWebEnsureUserMethodUsage = Prefix + "14";
                public const string SPWebEnsureUserMethodUsage_HelpUrl = HelpSite.HelpPageUrl + SPWebEnsureUserMethodUsage + "_SPWebEnsureUserMethodUsage";

                public const string SPWebRequestAccessEmailPropertyUsage = Prefix + "15";
                public const string SPWebRequestAccessEmailPropertyUsage_HelpUrl = HelpSite.HelpPageUrl + SPWebRequestAccessEmailPropertyUsage + "_SPWebRequestAccessEmailPropertyUsage";

                public const string AvoidHeavyOperationsInsideRepeaterItemEventHandlers = Prefix + "16";
                public const string AvoidHeavyOperationsInsideRepeaterItemEventHandlers_HelpUrl = HelpSite.HelpPageUrl + AvoidHeavyOperationsInsideRepeaterItemEventHandlers + "_AvoidHeavyOperationsInsideRepeaterItemEventHandlers";

                public const string AvoidInappropriateDataAccess = Prefix + "17";
                public const string AvoidInappropriateDataAccess_HelpUrl = HelpSite.HelpPageUrl + AvoidInappropriateDataAccess + "_AvoidInappropriateDataAccess";

                public const string SPDataSourceScopeDoesNotDefined = Prefix + "18";
                public const string SPDataSourceScopeDoesNotDefined_HelpUrl = HelpSite.HelpPageUrl + SPDataSourceScopeDoesNotDefined + "_SPDataSourceScopeDoesNotDefined";

                public const string AvoidInlineJavaScriptCode = Prefix + "19";
                public const string AvoidInlineJavaScriptCode_HelpUrl = HelpSite.HelpPageUrl + AvoidInlineJavaScriptCode + "_AvoidInlineJavaScriptCode";

                public const string AvoidObsoleteWebServicesInCode = Prefix + "20";
                public const string AvoidObsoleteWebServicesInCode_HelpUrl = HelpSite.HelpPageUrl + AvoidObsoleteWebServicesInCode + "_AvoidObsoleteWebServicesInCode";

                public const string AvoidObsoleteWebServicesInJavaScriptFiles = Prefix + "21";
                public const string AvoidObsoleteWebServicesInJavaScriptFiles_HelpUrl = HelpSite.HelpPageUrl + AvoidObsoleteWebServicesInJavaScriptFiles + "_AvoidObsoleteWebServicesInJavaScriptFiles";

                public const string AvoidUsingSPContextOutsideOfWebContext = Prefix + "22";
                public const string AvoidUsingSPContextOutsideOfWebContext_HelpUrl = HelpSite.HelpPageUrl + AvoidUsingSPContextOutsideOfWebContext + "_AvoidUsingSPContextOutsideOfWebContext";

                public const string AvoidJQueryDocumentReadyInCode = Prefix + "23";

                public const string CamlexQueryDoubleWhere = Prefix + "24";
                public const string CamlexQueryDoubleWhere_HelpUrl = HelpSite.HelpPageUrl + CamlexQueryDoubleWhere + "_CamlexQueryDoubleWhere";

                public const string MagicStringShouldNotBeUsed = Prefix + "25";
                public const string MagicStringShouldNotBeUsed_HelpUrl = HelpSite.HelpPageUrl + MagicStringShouldNotBeUsed + "_MagicStringShouldNotBeUsed";

                public const string AvoidUsingSPListItemFile = Prefix + "26";
                public const string AvoidUsingSPListItemFile_HelpUrl = HelpSite.HelpPageUrl + AvoidUsingSPListItemFile + "_AvoidUsingSPListItemFile";

                public const string InappropriateUsageOfTaxonomyGroupCollection = Prefix + "27";
                public const string InappropriateUsageOfTaxonomyGroupCollection_HelpUrl = HelpSite.HelpPageUrl + InappropriateUsageOfTaxonomyGroupCollection + "_InappropriateUsageOfTaxonomyGroupCollection";

                public const string AvoidUnsafeUrlConcatenations = Prefix + "28";
                public const string AvoidUnsafeUrlConcatenations_HelpUrl = HelpSite.HelpPageUrl + AvoidUnsafeUrlConcatenations + "_AvoidUnsafeUrlConcatenations";

                public const string LoadJavaScriptWithinSandbox = Prefix + "29";
                public const string LoadJavaScriptWithinSandbox_HelpUrl = HelpSite.HelpPageUrl + LoadJavaScriptWithinSandbox + "_LoadJavaScriptWithinSandbox";

                public const string RepeatableMagicStringShouldNotBeUsed = Prefix + "30";

                public const string ListModificationFromJob = Prefix + "31";
                public const string ListModificationFromJob_HelpUrl = HelpSite.HelpPageUrl + ListModificationFromJob + "_ListModificationFromJob";

                public const string GetPublishingPages = Prefix + "32";
                public const string GetPublishingPages_HelpUrl = HelpSite.HelpPageUrl + GetPublishingPages + "_GetPublishingPages";

                public const string AvoidSPObjectsInFields = Prefix + "33";
                public const string AvoidSPObjectsInFields_HelpUrl = HelpSite.HelpPageUrl + AvoidSPObjectsInFields + "_AvoidSPObjectsInFields";

                public const string AvoidStaticSPObjectsInFields = Prefix + "34";
                public const string AvoidStaticSPObjectsInFields_HelpUrl = HelpSite.HelpPageUrl + AvoidStaticSPObjectsInFields + "_AvoidStaticSPObjectsInFields";

                public const string AvoidSPObjectNameStringComparison = Prefix + "35";
                public const string AvoidSPObjectNameStringComparison_HelpUrl = HelpSite.HelpPageUrl + AvoidSPObjectNameStringComparison + "_AvoidSPObjectNameStringComparison";

                public const string DoNotSuppressExceptions = Prefix + "36";
                public const string DoNotSuppressExceptions_HelpUrl = HelpSite.HelpPageUrl + DoNotSuppressExceptions + "_DoNotSuppressExceptions";

                public const string ULSLoggingShouldBeUsed = Prefix + "37";
                public const string ULSLoggingShouldBeUsed_HelpUrl = HelpSite.HelpPageUrl + ULSLoggingShouldBeUsed + "_ULSLoggingShouldBeUsed";

                public const string SpecifySPZoneInSPSite = Prefix + "38";
                public const string SpecifySPZoneInSPSite_HelpUrl = HelpSite.HelpPageUrl + SpecifySPZoneInSPSite + "_SpecifySPZoneInSPSite";

                public const string NoCustomLogging = Prefix + "39";
                public const string NoCustomLogging_HelpUrl = HelpSite.HelpPageUrl + NoCustomLogging + "_NoCustomLogging";

                public const string DoNotUseSPWebProperties = Prefix + "40";
                public const string DoNotUseSPWebProperties_HelpUrl = HelpSite.HelpPageUrl + DoNotUseSPWebProperties + "_DoNotUseSPWebProperties";

                public const string ULSLoggingInCatchBlock = Prefix + "41";
                public const string ULSLoggingInCatchBlock_HelpUrl = HelpSite.HelpPageUrl + ULSLoggingInCatchBlock + "_ULSLoggingInCatchBlock";

                public const string AvoidDollarGlobalVariableInCode = Prefix + "42";

                public const string DoNotChangeSPPersistedObject = Prefix + "43";
                public const string DoNotChangeSPPersistedObject_HelpUrl = HelpSite.HelpPageUrl + DoNotChangeSPPersistedObject + "_DoNotChangeSPPersistedObject";

                public const string OutOfContextRWEP = Prefix + "44";
                public const string OutOfContextRWEP_HelpUrl = HelpSite.HelpPageUrl + OutOfContextRWEP + "_OutOfContextRWEP";

                public const string DoNotUsePortalLog = Prefix + "45";
                public const string DoNotUsePortalLog_HelpUrl = HelpSite.HelpPageUrl + DoNotUsePortalLog + "_DoNotUsePortalLog";

                public const string DoNotDisposePersonalSiteWeb = Prefix + "46";
                public const string DoNotDisposePersonalSiteWeb_HelpUrl = HelpSite.HelpPageUrl + DoNotDisposePersonalSiteWeb + "_DoNotDisposePersonalSiteWeb";

                public const string WrongSPViewUsage = Prefix + "47";
                public const string WrongSPViewUsage_HelpUrl = HelpSite.HelpPageUrl + WrongSPViewUsage + "_WrongSPViewUsage";
            }

            public static class Feature
            {
                private const string Prefix = SPCAFContribRulePrefix + SPCAFContribCategory + InspectedSPElement.Feature;

                public const string SPSiteFeatureShouldNotBeActivatedFromCode = Prefix + "01";
                public const string SPSiteFeatureShouldNotBeActivatedFromCode_HelpUrl = HelpSite.HelpPageUrl + SPSiteFeatureShouldNotBeActivatedFromCode + "_SPSiteFeatureShouldNotBeActivatedFromCode";

                public const string FeatureShouldHaveNotEmptyImageUrl = Prefix + "02";
                public const string FeatureShouldHaveNotEmptyImageUrl_HelpUrl = HelpSite.HelpPageUrl + FeatureShouldHaveNotEmptyImageUrl + "_FeatureShouldHaveNotEmptyImageUrl";


                /* fill the gap 03 and 04 */

                public const string MagicStringShouldNotBeUsed = Prefix + "05";
                public const string MagicStringShouldNotBeUsed_HelpUrl = HelpSite.HelpPageUrl + MagicStringShouldNotBeUsed + "_MagicStringShouldNotBeUsed";

                public const string FeatureAlwaysForceInstall = Prefix + "06";
                public const string FeatureAlwaysForceInstall_HelpUrl = HelpSite.HelpPageUrl + FeatureAlwaysForceInstall + "_FeatureAlwaysForceInstall";
            }

            public static class JavaScriptFile
            {
                private const string Prefix = SPCAFContribRulePrefix + SPCAFContribCategory + InspectedSPElement.JavaScriptFile;

                public const string AvoidDollarGlobalVariableInJSFile = Prefix + "01";
                public const string AvoidJQueryDocumentReadyInJSFile = Prefix + "02";
            }

            public static class Module
            {
                private const string Prefix = SPCAFContribRulePrefix + SPCAFContribCategory + InspectedSPElement.Module;

                public const string AvoidAllUsersWebPartInModules = Prefix + "01";
                public const string AvoidAllUsersWebPartInModules_HelpUrl = HelpSite.HelpPageUrl + AvoidAllUsersWebPartInModules + "_AvoidAllUsersWebPartInModules";

                public const string AvoidXsltForSharePoint2013 = Prefix + "02";
                public const string AvoidXsltForSharePoint2013_HelpUrl = HelpSite.HelpPageUrl + AvoidXsltForSharePoint2013 + "_AvoidXsltForSharePoint2013";

                public const string AvoidInfoPathForms = Prefix + "03";
                public const string AvoidInfoPathForms_HelpUrl = HelpSite.HelpPageUrl + AvoidInfoPathForms + "_AvoidInfoPathForms";
            }
        }

        public static class Metrics
        {
            public static class Assembly
            {
                private const string Prefix = SPCAFContribMetricPrefix + SPCAFContribCategory + InspectedSPElement.Assembly;

                public const string NumberOfControlTemplates = Prefix + "01";
                public const string NumberOfControlTemplates_HelpUrl = HelpSite.HelpPageUrl + NumberOfControlTemplates + "_NumberOfControlTemplates";

                public const string NumberOfExternalDlls = Prefix + "02";
                public const string NumberOfExternalDlls_HelpUrl = HelpSite.HelpPageUrl + NumberOfExternalDlls + "_NumberOfExternalDlls";

                public const string NumberOfLayoutsPages = Prefix + "03";
                public const string NumberOfLayoutsPages_HelpUrl = HelpSite.HelpPageUrl + NumberOfLayoutsPages + "_NumberOfLayoutsPages";

                public const string NumberOfMasterPages = Prefix + "04";
                public const string NumberOfMasterPages_HelpUrl = HelpSite.HelpPageUrl + NumberOfMasterPages + "_NumberOfMasterPages";

                public const string NumberOfXsltFiles = Prefix + "05";
                public const string NumberOfXsltFiles_HelpUrl = HelpSite.HelpPageUrl + NumberOfXsltFiles + "_NumberOfXsltFiles";

                public const string SPMonitoredScopeMetric = Prefix + "06";
                public const string SPMonitoredScopeMetric_HelpUrl = HelpSite.HelpPageUrl + SPMonitoredScopeMetric + "_SPMonitoredScopeMetric";

                public const string NumberOfBreakInheritances = Prefix + "07";
                public const string NumberOfBreakInheritances_HelpUrl = HelpSite.HelpPageUrl + NumberOfBreakInheritances + "_NumberOfBreakInheritanceMetric";

                public const string SearchQueryIsUsed = Prefix + "08";
                public const string SearchQueryIsUsed_HelpUrl = HelpSite.HelpPageUrl + SearchQueryIsUsed + "_SearchQueryIsUsed";

            }
        }

        public static class Inventory
        {
            public static class Assembly
            {
                private const string Prefix = SPCAFContribInventoryPrefix + SPCAFContribCategory + InspectedSPElement.Assembly;

                public const string SearchProperties = Prefix + "01";
                public const string SearchProperties_HelpUrl = HelpSite.HelpPageUrl + SearchProperties + "_SearchProperties";

                public const string UserProfileProperties = Prefix + "02";
                public const string UserProfileProperties_HelpUrl = HelpSite.HelpPageUrl + UserProfileProperties + "_UserProfileProperties";

                public const string CustomMasterPages = Prefix + "03";
                public const string CustomMasterPages_HelpUrl = HelpSite.HelpPageUrl + CustomMasterPages + "_CustomMasterPages";

                public const string PropertyBagUsage = Prefix + "04";
                public const string PropertyBagUsage_HelpUrl = HelpSite.HelpPageUrl + PropertyBagUsage + "_PropertyBagUsage";

                public const string QueryStringUsage = Prefix + "05";
                public const string QueryStringUsage_HelpUrl = HelpSite.HelpPageUrl + QueryStringUsage + "_QueryStringUsage";

                public const string ListOfConsts = Prefix + "06";
                public const string ListOfConsts_HelpUrl = HelpSite.HelpPageUrl + ListOfConsts + "_ListOfConsts";

                public const string ListOfStrings = Prefix + "07";
                public const string ListOfStrings_HelpUrl = HelpSite.HelpPageUrl + ListOfStrings + "_ListOfStrings";
            }

            public static class Controls
            {
                private const string Prefix = SPCAFContribInventoryPrefix + SPCAFContribCategory + InspectedSPElement.Control;

                public const string DelegateControls = Prefix + "01";
                public const string DelegateControls_HelpUrl = HelpSite.HelpPageUrl + DelegateControls + "_NumberOfDelegateControls";
            }
        }
    }
}
