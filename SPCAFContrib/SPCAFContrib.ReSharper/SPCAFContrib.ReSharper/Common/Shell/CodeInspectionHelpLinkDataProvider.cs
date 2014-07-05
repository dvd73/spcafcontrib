using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.Threading;
using JetBrains.Util;
using JetBrains.Util.Logging;
using SPCAFContrib.ReSharper.Common.HelpLink;

namespace SPCAFContrib.ReSharper.Common.Shell
{
    [ShellComponent]
    public class CodeInspectionHelpLinkDataProvider : ICodeInspectionHelpLinkDataProvider
    {
        private readonly Lifetime myLifetime;
        private Dictionary<string, string> myData;

        public CodeInspectionHelpLinkDataProvider(Lifetime lifetime, IThreading threading)
        {
          this.myLifetime = lifetime;
          this.LoadData(CodeInspectionHelpLinkResources.CodeInspectionHelpLink);
          //this.StartDownloading(lifetime, threading);
        }

        //protected virtual void StartDownloading(Lifetime lifetime, IThreading threading)
        //{
        //    threading.ThreadManager.ExecuteTask((Action)(() =>
        //    {
        //        WebClient webClient = new WebClient();
        //        webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(this.DownloadStringCompleted);
        //        webClient.DownloadStringAsync(new Uri("http://www.jetbrains.com/resharper/updates/CodeInspectionWiki.xml"));
        //    }), ApartmentState.Unknown);
        //}

        //private void DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        //{
        //    if (this.myLifetime.IsTerminated)
        //        return;
        //    if (e.Error == null)
        //        this.LoadData(e.Result);
        //    else
        //        Logger.LogExceptionSilently(e.Error);
        //}

        protected void LoadData(string content)
        {
            if (!String.IsNullOrEmpty(content))
            {
                try
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(content);
                    this.myData =
                        Enumerable.ToDictionary<XmlNode, string, string>(
                            Enumerable.Cast<XmlNode>((IEnumerable) xmlDocument.DocumentElement.ChildNodes),
                            (Func<XmlNode, string>) (key => key.Attributes["Id"].Value),
                            (Func<XmlNode, string>) (val => val.Attributes["Url"].Value));
                }
                catch (Exception ex)
                {
                    InvalidOperationException exception =
                        new InvalidOperationException("Failed to load code inspection wiki XML.", ex);
                    ExceptionEx.AddData<InvalidOperationException>(exception, "XmlContent",
                        (Func<object>) (() => (object) content));
                    Logger.LogExceptionSilently((Exception) exception);
                }
            }
        }

        public bool TryGetValue(string attributeId, out string url)
        {
            if (this.myData != null)
                return this.myData.TryGetValue(attributeId, out url);
            url = (string)null;
            return false;
        }
    }

    public interface ICodeInspectionHelpLinkDataProvider
    {
        bool TryGetValue(string attributeId, out string url);
    }
}
