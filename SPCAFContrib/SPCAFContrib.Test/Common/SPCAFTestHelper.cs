using System;
using System.Collections.Generic;
using System.Threading;
using SPCAF.Engine;
using SPCAF.Sdk;
using SPCAFContrib.Rules.Code.Base;

namespace SPCAFContrib.Test.Common
{
    public class SPCAFTestHelper
    {
        static string outputfile = @"C:\Temp";
        static string[] inputfiles = new[] { @"C:\temp\SPCAFDebug\WSP\SPCAFContrib.Demo.wsp" };

        public static bool ExecuteTest(IVisitor[] visitors, string wspfile = null)
        {
            TestAnalyzerResult testResult = new TestAnalyzerResult();
            
            var spcafrunner = new TestAnalyzer(visitors, false);

            spcafrunner.OutputFile = outputfile;
            spcafrunner.SourceFiles = String.IsNullOrEmpty(wspfile) ? inputfiles : new[] {wspfile};
            //spcafrunner.SettingsFile = settingsfile;

            //spcafrunner.ReportGeneratorTypes = new ReportGeneratorType[] { ReportGeneratorType.HTML, ReportGeneratorType.XML, ReportGeneratorType.DGML, ReportGeneratorType.CSV };
            //spcafrunner.LicenseKey = "your licence key";
            //spcafrunner.RequiredLicense = "PROFESSIONAL|SERVER|ENTERPRISE";

            using (var eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                spcafrunner.AnalysisCompleted += recievedReport =>
                {
                    testResult.Report = recievedReport;
                    eventWaitHandle.Set();
                };

                spcafrunner.ArgumentValidationFailed += recievedValidationErrors =>
                {
                    testResult.ValidationErrors = recievedValidationErrors;
                    eventWaitHandle.Set();
                };

                spcafrunner.FatalErrorOccured += recievedFatalError =>
                {
                    testResult.FatalError = recievedFatalError;
                    eventWaitHandle.Set();
                };

                spcafrunner.NotificationAdded += notification => 
                {
                    testResult.Notifications.Add(notification);
                    eventWaitHandle.Set();
                };

                spcafrunner.Run();

                eventWaitHandle.WaitOne(new TimeSpan(0, 0, 30));

                if (testResult.Report == null && testResult.ValidationErrors == null && testResult.FatalError == null)
                {
                    throw new InvalidOperationException("Execution of the rule took too long");
                }
            }

            return !(testResult.HasFatalError || testResult.HasValidationError);
        }
    }
}
