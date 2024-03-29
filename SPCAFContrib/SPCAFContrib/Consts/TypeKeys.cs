﻿using System.Globalization;

namespace SPCAFContrib.Consts
{
    public static class TypeKeys
    {
        #region UserProfiles
        public const string ProfileManagerBase = "Microsoft.Office.Server.UserProfiles.ProfileManagerBase";
        public const string UserProfile = "Microsoft.Office.Server.UserProfiles.UserProfile";
        #endregion
        #region System.Data
        public const string DataContext = "System.Data.Linq.DataContext";
        public const string EFDataContext = "System.Data.Entity.DbContext";
        public const string SqlConnection = "System.Data.SqlClient.SqlConnection";
        public const string OleDbConnection = "System.Data.OleDb.OleDbConnection";
        public const string OdbcConnection = "System.Data.Odbc.OdbcConnection";
        public const string OracleConnection = "System.Data.OracleClient.OracleConnection"; 
        #endregion
        #region System.Web
        public const string HttpRequest = "System.Web.HttpRequest";
        public const string IHttpHandler = "System.Web.IHttpHandler";
        public const string IHttpModule = "System.Web.IHttpModule";
        public const string ScriptManager = "System.Web.UI.ScriptManager";
        public const string ClientScriptManager = "System.Web.UI.ClientScriptManager";
        #endregion
        #region Microsoft.SharePoint
        public const string SPContext = "Microsoft.SharePoint.SPContext";
        public const string SPSite = "Microsoft.SharePoint.SPSite";
        public const string SPWeb = "Microsoft.SharePoint.SPWeb";
        public const string SPFeatureCollection = "Microsoft.SharePoint.SPFeatureCollection";
        public const string SPFile = "Microsoft.SharePoint.SPFile";
        public const string SPList = "Microsoft.SharePoint.SPList";
        public const string SPListItem = "Microsoft.SharePoint.SPListItem";
        public const string SPListCollection = "Microsoft.SharePoint.SPListCollection";
        public const string SPRoleDefinitionCollection = "Microsoft.SharePoint.SPRoleDefinitionCollection";
        public const string SPSecurableObject = "Microsoft.SharePoint.SPSecurableObject";
        public const string SPContentType = "Microsoft.SharePoint.SPContentType";
        public const string SPPrincipal = "Microsoft.SharePoint.SPPrincipal";
        public const string SPQuery = "Microsoft.SharePoint.SPQuery";
        public const string SPView = "Microsoft.SharePoint.SPView";
        public const string SPDataSourceView = "Microsoft.SharePoint.WebControls.SPDataSourceView";
        public const string SPDataSource = "Microsoft.SharePoint.WebControls.SPDataSource";
        public const string SPSiteDataQuery = "Microsoft.SharePoint.SPSiteDataQuery";
        public const string SPViewCollection = "Microsoft.SharePoint.SPViewCollection";
        public const string ListFieldIterator = "Microsoft.SharePoint.WebControls.ListFieldIterator";
        public const string SPFeatureReceiver = "Microsoft.SharePoint.SPFeatureReceiver";
        public const string SPSecurity = "Microsoft.SharePoint.SPSecurity";
        public const string SPItemEventReceiver = "Microsoft.SharePoint.SPItemEventReceiver";
        public const string PortalLog = "Microsoft.Office.Server.Diagnostics.PortalLog";
        #endregion
        #region Configuration
        public const string ConfigurationManager = "System.Configuration.ConfigurationManager"; 
        #endregion
        #region Search
        public const string KeywordQuery = "Microsoft.Office.Server.Search.Query.KeywordQuery";
        public const string FullTextSqlQuery = "Microsoft.Office.Server.Search.Query.FullTextSqlQuery"; 
        #endregion
        #region Administration
        public const string SPJobDefinition = "Microsoft.SharePoint.Administration.SPJobDefinition";
        public const string SPPersistedObject = "Microsoft.SharePoint.Administration.SPPersistedObject";
        public const string SPDiagnosticsServiceBase = "Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase";
        public const string SPDiagnosticsService = "Microsoft.SharePoint.Administration.SPDiagnosticsService";
        public const string IPersonalPage = "Microsoft.SharePoint.Portal.WebControls.IPersonalPage";
        #endregion
        #region Publishing
        public const string PublishingWeb = "Microsoft.SharePoint.Publishing.PublishingWeb";
        public const string PageLayout = "Microsoft.SharePoint.Publishing.PageLayout";
        public const string PortalSiteMapProvider = "Microsoft.SharePoint.Publishing.Navigation.PortalSiteMapProvider";
        public const string CrossListQueryCache = "Microsoft.SharePoint.Publishing.CrossListQueryCache";
        #endregion
        #region Utilities
        public const string SPPropertyBag = "Microsoft.SharePoint.Utilities.SPPropertyBag";
        public const string SPMonitoredScope = "Microsoft.SharePoint.Utilities.SPMonitoredScope"; 
        #endregion
        #region Taxonomy
        public const string TaxonomyTerm = "Microsoft.SharePoint.Taxonomy.Term";
        public const string TaxonomyGroup = "Microsoft.SharePoint.Taxonomy.Group"; 
        #endregion
        #region Logging
        public const string EventLog = "System.Diagnostics.EventLog";
        /// <summary>
        /// NLog is a simple .NET logging library designed to be flexible. It supports processing diagnostic messages with any .NET languages and supports multiple targets.
        /// </summary>
        public const string NLog = "NLog.Logger";
        /// <summary>
        /// Log4net is a tool to help programmers output log statements to different types of output targets. Log4net is a port of the log4j Apache project.
        /// </summary>
        public const string log4net = "log4net.ILog";
        /// <summary>
        /// Common.Logging is a library to introduce a simple abstraction to allow you to select a specific logging implementation at runtime. There are a variety of logging implementations for .NET currently in use, log4net, Enterprise Library Logging, NLog, to name the most popular. They do not share a common interface and therefore impose a particular logging implementation on the users of your library. Common.Logging solves this problem.
        /// </summary>
        public const string CommonLogging = "Common.Logging.ILog";
        /// <summary>
        /// Microsoft's Enterprise Library comes with a .NET logging application block to write messages to the Windows event log, text files, message queue and more.
        /// </summary>
        public const string EL_Logging_Application_Block = "Microsoft.Practices.EnterpriseLibrary.Logging.Logger";
        /// <summary>
        /// ObjectGuy Logging Framework for .NET supports logging to the system Console, a file on disk, TCP/IP and memory
        /// </summary>
        public const string ObjectGuy = "BitFactory.Logging.Logger";
        /// <summary>
        /// C# Logger is a logging tool that supports sending events and messages to the Windows event log. The API is similar to Apache's log4j.
        /// </summary>
        public const string CSharpLogger = "com.sporadicism.util.logger.ILogger";
        /// <summary>
        /// Log4net is a tool to help programmers output log statements to different types of output targets. Log4net is a port of the log4j Apache project.
        /// </summary>
        public const string LoggerNET = "TerWoord.Diagnostics.LogFactory";
        /// <summary>
        /// The LogThis C# logging framework supports custom profiles, dates in log file names and logging to the Windows event log.
        /// </summary>
        public const string LogThis = "LogThis.Log";
        /// <summary>
        /// C# .NET Logger is an extensible logging framework written in C# and comes with message queuing and asynchronous logging capabilities.
        /// </summary>
        public const string CSharpDotNETLogger = "VS.Logger.Logger";
        /// <summary>
        /// NetTrace is a simple debug tracer that comes with its own tracing class and a built-in dialog that allows developers to configure the tracing output.
        /// </summary>
        public const string NetTrace = "NetTrace.Tracer";
        /// <summary>
        /// The NSpring framework includes a logging library that supports log files and log file archiving. It also supports formatting data as XML.
        /// </summary>
        public const string NSpring = "NSpring.Logging.Logger";
        /// <summary>
        /// Using Loggr’s fluent-style logging library you can easily send your events to Loggr and immediately take advantage of cloud-based event logging.
        /// </summary>
        public const string Loggr = "Loggr.Events";
        /// <summary>
        /// ELMAH (Error Logging Modules and Handlers) is an application-wide error logging facility that is completely pluggable. It can be dynamically added to a running ASP.NET web application, or even all ASP.NET web applications on a machine, without any need for re-compilation or re-deployment.
        /// </summary>
        public const string ELMAH_SqlErrorLog = "Elmah.ErrorLog.SqlErrorLog";
        public const string ELMAH_SqlServerCompactErrorLog = "Elmah.ErrorLog.SqlServerCompactErrorLog";
        public const string ELMAH_SQLiteErrorLog = "Elmah.ErrorLog.SQLiteErrorLog";
        public const string ELMAH_MemoryErrorLog = "Elmah.ErrorLog.MemoryErrorLog";
        public const string ELMAH_OracleErrorLog = "Elmah.ErrorLog.OracleErrorLog";
        public const string ELMAH_MySqlErrorLog = "Elmah.ErrorLog.MySqlErrorLog";
        public const string ELMAH_XmlFileErrorLog = "Elmah.ErrorLog.XmlFileErrorLog";
        public const string ELMAH_PgsqlErrorLog = "Elmah.ErrorLog.PgsqlErrorLog";
        public const string ELMAH_AccessErrorLog = "Elmah.ErrorLog.AccessErrorLog";
        #endregion
        #region Web parts
        public const string ContentEditorWebPart = "Microsoft.SharePoint.WebPartPages.ContentEditorWebPart";
        #endregion
        
        public const string DirectorySearcher = "System.DirectoryServices.DirectorySearcher";
        public const string Thread = "System.Threading.Thread";
        public const string SystemString = "System.String";

        public const string CamlexIQuery = "CamlexNET.Interfaces.IQuery";
    }
}
