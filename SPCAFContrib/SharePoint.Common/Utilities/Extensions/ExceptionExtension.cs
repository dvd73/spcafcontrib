using System;
using Microsoft.SharePoint.Administration;
using SharePoint.Common.Enums;

namespace SharePoint.Common.Utilities.Extensions
{
    public static class ExceptionExtension
    {
        /// <summary>
        /// Log the exception in the SharePoint list as an error with high severity
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        public static void LogError(this Exception ex)
        {
            Logger.Instance.LogException(ex, String.Empty, IssuePriority.High, IssueSeverity.Error);
        }

        /// <summary>
        /// Log the exception in the SharePoint list as an error with high severity
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <param name="message">Custom message text</param>
        public static void LogError(this Exception ex, string message)
        {
            Logger.Instance.LogException(ex, message, IssuePriority.High, IssueSeverity.Error);
        }

        /// <summary>
        /// Log the exception in the SharePoint list as an warning with medium severity
        /// </summary>
        /// <param name="ex"></param>        
        public static void LogWarning(this Exception ex)
        {
            Logger.Instance.LogException(ex, String.Empty, IssuePriority.Normal, IssueSeverity.Warning);
        }

        /// <summary>
        /// Log the exception in the SharePoint list as an warning with medium severity
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message">Custom message text</param>
        public static void LogWarning(this Exception ex, string message)
        {
            Logger.Instance.LogException(ex, message, IssuePriority.Normal, IssueSeverity.Warning);            
        }

        /// <summary>
        /// Log the exception in the SharePoint list as an information with medium severity
        /// </summary>
        /// <param name="ex"></param>        
        public static void LogInformation(this Exception ex)
        {
            Logger.Instance.LogException(ex, String.Empty, IssuePriority.Normal, IssueSeverity.Information);
        }

        /// <summary>
        /// Log the exception in the SharePoint list as an warning with medium severity
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message">Custom message text</param>
        public static void LogInformation(this Exception ex, string message)
        {
            Logger.Instance.LogException(ex, message, IssuePriority.Normal, IssueSeverity.Information);
        }
        
    }
}
