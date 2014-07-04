using System;
using System.Text;
using SharePoint.Common.Utilities;

namespace MOSS.Common.Utilities
{
    /// <summary>
    /// Support ULS and Event Log writing
    /// </summary>
    public class MOSSLogger : Logger
    {
        public new static MOSSLogger Instance
        {
            get { return GetInstance<MOSSLogger>(); }
        }        

        /// <summary>
        /// Write to Windows Application Event Log
        /// </summary>
        /// <param name="diagnosticsService"></param>
        /// <param name="message"></param>
        public void LogError(string message)
        {            
            LoggingService.Instance.LogIssue(message);
            LastException = message;
        }

        /// <summary>
        /// Write to Windows Application Event Log
        /// </summary>
        /// <param name="diagnosticsService"></param>
        /// <param name="message"></param>
        public void LogError(Exception ex, Uri requestUrl)
        {
            StringBuilder buffer = new StringBuilder();
            if (requestUrl == null)
            {
                buffer.Append(
                    "An error occurred in the application.");
            }
            else
            {
                buffer.Append(
                    "An error occurred during the execution of the"
                    + " web request.");
            }

            buffer.Append(
                " Please review the stack trace for more information about the"
                + " error and where it originated in the code.");

            buffer.Append(Environment.NewLine);
            buffer.Append(Environment.NewLine);

            if (requestUrl != null)
            {
                buffer.Append("Request URL: ");
                buffer.Append(requestUrl.AbsoluteUri);

                buffer.Append(Environment.NewLine);
                buffer.Append(Environment.NewLine);
            }

            buffer.Append(BuildExceptionDetails(ex, false));

            string message = buffer.ToString();
            LastException = message;
            LoggingService.Instance.LogIssue(message);            
        }          

        /// <summary>
        /// Write to Windows Application Event Log
        /// </summary>
        /// <param name="message">Log message</param>
        public void Trace(string message)
        {
            LoggingService.Instance.LogInfo(message);
        }                     
    }
}