using System;
using System.Linq;
using SPCAF.Engine;
using SPCAF.Engine.ArgumentValidation;
using SPCAF.Sdk;

namespace SPCAFContrib.Test.Common
{
    public class TestAnalyzerResult
    {
        public IAnalysisReport Report { get; set; }
        public ValidationErrorCollection ValidationErrors { get; set; }
        public NotificationCollection Notifications { get; set; }
        public Exception FatalError { get; set; }

        public bool HasFatalError 
        {
            get { return FatalError != null; }
        }

        public bool HasValidationError
        {
            get { return ValidationErrors != null && ValidationErrors.Any(); }
        }

        public TestAnalyzerResult()
        {
            Notifications = new NotificationCollection();
        }
    }
}
