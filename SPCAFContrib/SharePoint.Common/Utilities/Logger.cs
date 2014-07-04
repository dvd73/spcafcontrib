using System;
using System.Text;
using Microsoft.SharePoint;
using SharePoint.Common.Enums;
using System.Diagnostics;

namespace SharePoint.Common.Utilities
{   
    public class Logger : SingletoneHelper
    {
        public virtual string LastException { get; set; }

        public SPWeb TargetWeb = null;

        #region Singeton interface        

        public static Logger Instance
        {
            get { return GetInstance<Logger>(); }
        }

        #endregion        

        /// <summary>
        /// Save exception info into web internal list
        /// </summary>
        /// <param name="ex">Exception object</param>
        /// <param name="message">Additional provided text</param>
        /// <param name="priority">The priority of issue</param>
        /// <param name="severity">The severity of issue</param>
        public virtual void LogException(Exception ex, string message, IssuePriority priority, IssueSeverity severity)
        {            
            // inside feature receiver the Context is always null
            SPWeb web   = SPContext.Current != null ? SPContext.Current.Web : TargetWeb;
            
            if (web != null)
            {
                bool allowUnsafeUpdates = web.AllowUnsafeUpdates;
                try
                {
                    web.AllowUnsafeUpdates = true;
                    SPUser curUsr = UserHelper.Instance.CurrentUser;

                    //Insert the exception message into the Issues list and mark it for Triage.                    
                    SPList issuesList = web.Lists.TryGetList(Consts.LOGGER_LIST_NAME);

                    if (issuesList != null && issuesList.DoesUserHavePermissions(SPBasePermissions.AddListItems))
                    {
                        SPListItem newIssue = issuesList.AddItem();
                        newIssue[SPBuiltInFieldId.Title] = GetSource();
                        if (curUsr != null)
                            newIssue[SPBuiltInFieldId.AssignedTo] = new SPFieldUserValue(web, curUsr.ID, UserHelper.Instance.CurrentUserLoginName);
                        newIssue[SPBuiltInFieldId.Priority] = (int)priority;
                        newIssue["Severity"] = severity.ToString();
                        LastException = BuildExceptionDetails(ex, true);
                        newIssue[SPBuiltInFieldId.Comment] = LastException.Replace(Environment.NewLine, "<br/>");
                        newIssue[SPBuiltInFieldId.V3Comments] = message;
                        newIssue.Update();
                    }
                }
                finally
                {
                    web.AllowUnsafeUpdates = allowUnsafeUpdates;
                }
            }
        }

        /// <summary>
        /// Save provided text into web internal list with  warning severity
        /// </summary>
        /// <param name="message">provided text</param>
        public virtual void LogWarning(string message)
        {
            // inside feature receiver the Context is always null
            SPWeb web = SPContext.Current != null ? SPContext.Current.Web : TargetWeb;

            if (web != null)
            {
                bool allowUnsafeUpdates = web.AllowUnsafeUpdates;
                try
                {
                    web.AllowUnsafeUpdates = true;
                    SPUser curUsr = UserHelper.Instance.CurrentUser;

                    //Insert the exception message into the Issues list and mark it for Triage.
                    SPList issuesList = web.Lists.TryGetList(Consts.LOGGER_LIST_NAME);
                    if (issuesList != null && issuesList.DoesUserHavePermissions(SPBasePermissions.AddListItems))
                    {                        
                        SPListItem newIssue = issuesList.AddItem();
                        newIssue[SPBuiltInFieldId.Title] = GetSource();
                        if (curUsr != null)
                            newIssue[SPBuiltInFieldId.AssignedTo] = new SPFieldUserValue(web, curUsr.ID, UserHelper.Instance.CurrentUserLoginName);
                        newIssue[SPBuiltInFieldId.Priority] = (int)IssuePriority.Normal;
                        newIssue["Severity"] = IssueSeverity.Warning.ToString();
                        newIssue[SPBuiltInFieldId.V3Comments] = message;
                        newIssue.Update();
                    }
                }
                finally
                {
                    web.AllowUnsafeUpdates = allowUnsafeUpdates;
                }
            }
        }

        /// <summary>
        /// Save provided text into web internal list with Information severity
        /// </summary>
        /// <param name="message">provided text</param>
        public virtual void LogInformation(string message)
        {
            // inside feature receiver the Context is always null
            SPWeb web = SPContext.Current != null ? SPContext.Current.Web : TargetWeb;
            
            if (web != null)
            {
                bool allowUnsafeUpdates = web.AllowUnsafeUpdates;
                try
                {
                    web.AllowUnsafeUpdates = true;
                    SPUser curUsr = UserHelper.Instance.CurrentUser;

                    //Insert the exception message into the Issues list and mark it for Triage.
                    SPList issuesList = web.Lists.TryGetList(Consts.LOGGER_LIST_NAME);
                    if (issuesList != null && issuesList.DoesUserHavePermissions(SPBasePermissions.AddListItems))
                    {

                        SPListItem newIssue = issuesList.AddItem();
                        newIssue[SPBuiltInFieldId.Title] = String.IsNullOrEmpty(message) ? GetSource() : message;
                        if (curUsr != null)
                            newIssue[SPBuiltInFieldId.AssignedTo] = new SPFieldUserValue(web, curUsr.ID, UserHelper.Instance.CurrentUserLoginName);
                        newIssue[SPBuiltInFieldId.Priority] = (int)IssuePriority.Low;
                        newIssue["Severity"] = IssueSeverity.Information.ToString();
                        newIssue[SPBuiltInFieldId.V3Comments] = message;
                        newIssue.Update();
                    }
                }
                finally
                {
                    web.AllowUnsafeUpdates = allowUnsafeUpdates;
                }
            }
        }

        protected string BuildExceptionDetails(Exception ex, bool format)
        {
            //Create a formatted string containing a formatted exception
            StringBuilder retString = new StringBuilder();
            if (format)
                retString.Append("<strong>Message:</strong> ");
            else
                retString.Append("Message: ");
            retString.Append(ComposeFullExceptionMessage(ex));
            if (format)
                retString.Append("<br/>");
            else
                retString.Append(Environment.NewLine);
            if (format)
                retString.Append("<strong>StackTrace:</strong> ");
            else
                retString.Append("StackTrace: ");
            retString.Append(ex.StackTrace);
            if (format)
                retString.Append("<br/>");
            else
                retString.Append(Environment.NewLine);
            if (format)
                retString.Append("<strong>Source:</strong> ");
            else
                retString.Append("Source: ");
            retString.Append(ex.Source);
            if (format)
                retString.Append("<br/>");
            else
                retString.Append(Environment.NewLine);

            return retString.ToString();
        }

        protected string ComposeFullExceptionMessage(Exception ex)
        {
            StringBuilder builder = new StringBuilder();
            Exception innerException = ex;
            do
            {
                builder.Append(innerException.GetType().ToString() + ": ").Append(innerException.Message).Append("; ");
                innerException = innerException.InnerException;
            }
            while (innerException != null);            
            return builder.ToString();
        }

        protected string GetSource()
        {
            // Office 365 has a restriction of Reflection usage, but regular sandbox - no.
            // TODO: need to replace by other solution
            return new StackFrame(3, true).GetMethod().ReflectedType.Assembly.FullName.Split(new char[] { ',' })[0];
        }
        
    }
}
