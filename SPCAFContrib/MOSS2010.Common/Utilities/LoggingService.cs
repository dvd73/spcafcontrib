using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using SharePoint.Common.Utilities.Extensions;

namespace MOSS.Common.Utilities
{    
    /// <summary>
    /// When implementing custom Logging Service you have to provide a list of categories and logging levels supported by the service. 
    /// You can do this by overriding the ProvideAreas method. In my example I implemented only one category but depending on your solution you could add multiple categories and severity levels.
    /// You need to grant App pool account to read from the event log registry entries:
    ///     1. Select Start - Run, then enter: regedt32
    ///     2. Navigate/expand to the following key:
    ///     3. HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Eventlog\Security
    ///     4. Right click on this entry and select Permissions
    ///     5. Add the ASPNET user
    ///     6. Give it Read permission or create DiagnosticAreaName mannually
    /// 2nd way(not implemented here): is needed if you want to make your custom logging service configurable via Central administration > Monitoring > Configure diagnostic logging. I.e. if you want to specify different trace and event severities for different categories via UI as you can do for standard categories, you should use 2nd way.    
    /// http://sadomovalex.blogspot.ru/2012/07/several-ways-to-implement-custom-uls.html?utm_source=feedburner&utm_medium=feed&utm_campaign=Feed:+SadomovalexsBlog+(sadomovalex's+blog)    
    /// </summary>
    public class LoggingService : SPDiagnosticsServiceBase
    {
        private const string DiagnosticAreaName = "MOSS.Common";
        private const string InfoCategoryName = "Trace";
        private const string IssueCategoryName = "Issue";
        private const string LoggingServiceName = "MOSS Logging Service";

        private const TraceSeverity DefaultTraceSeverityForIssue = TraceSeverity.Unexpected;
        private const TraceSeverity DefaultTraceSeverityForInfo = TraceSeverity.Medium;
        private const EventSeverity DefaultEventSeverityForIssue = EventSeverity.Error;
        private const EventSeverity DefaultEventSeverityForInfo = EventSeverity.Information;

        private SPDiagnosticsCategory DefaultInfoTraceCategory
        {
            get { return new SPDiagnosticsCategory(InfoCategoryName, DefaultTraceSeverityForInfo, DefaultEventSeverityForInfo); }
        }

        private SPDiagnosticsCategory DefaultIssueTraceCategory
        {
            get { return new SPDiagnosticsCategory(IssueCategoryName, DefaultTraceSeverityForIssue, DefaultEventSeverityForIssue); }
        }

        private static LoggingService _instance;
        public static LoggingService Instance
        {
            get
            {
                if (_instance == null)
                {

                    //if (SPContext.Current != null)
                    //{
                    //    SPContext.Current.Web.AllowUnsafeUpdates = true;
                    //}
                    _instance = GetLocal<LoggingService>();                    
                }

                return _instance;                
            }
        }

        public LoggingService(): base(DiagnosticAreaName, SPFarm.Local) 
        {                 
        }
     
        public LoggingService(string name, SPFarm farm) : base(name, farm)
        {
        }

        /// <summary>
        /// Registers the class for logging. Run in a Feature Receiver at the farm level  
        /// </summary>
        public void Register()
        {
            Update(true);
        }

        /// <summary>
        /// Unregisters the class. Run in a Feature Receiver at the farm level
        /// </summary>
        public void Unregister()
        {
            Delete();
            //Unprovision();
            //Uncache();
        }

        protected override bool HasAdditionalUpdateAccess() 
        {
           // Without this SPDiagnosticsServiceBase.GetLocal<MyDiagnosticsService>()
           // throws a SecurityException, see
           // http://share2010.wordpress.com/tag/sppersistedobject/
           return true;
        } 

        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            List<SPDiagnosticsArea> areas = new List<SPDiagnosticsArea>
            {
                new SPDiagnosticsArea(DiagnosticAreaName, new List<SPDiagnosticsCategory>
                {
                    DefaultInfoTraceCategory, DefaultIssueTraceCategory
                })
            };

            return areas;
        }

        /// <summary>
        /// Write issue message to Windows Application Event Log
        /// </summary>
        /// <param name="message">Log message</param>
        public void LogIssue(string message)
        {
            SPSecurity.RunWithElevatedPrivileges(
                delegate()
                {
                    try
                    {
                        SPDiagnosticsCategory category = Areas[DiagnosticAreaName].Categories[IssueCategoryName];
                        WriteEvent(0, category, DefaultEventSeverityForIssue, message);
                    }
                    catch (System.Exception ex)
                    {
                        // write exception with message to internal list
                        ex.LogError(message);
                    }                    
                });
        }

        /// <summary>
        /// Write info message to Windows Application Event Log
        /// </summary>
        /// <param name="message">Log message</param>
        public void LogInfo(string message)
        {
            SPSecurity.RunWithElevatedPrivileges(
                delegate()
                {
                    try
                    {
                        SPDiagnosticsCategory category = Areas[DiagnosticAreaName].Categories[InfoCategoryName];
                        WriteEvent(0, category, DefaultEventSeverityForInfo, message);
                    }
                    catch (System.Exception ex)
                    {                    
                        // write exception with message to internal list
                        ex.LogError(message);
                    }
                });
        }
        
    }
}
