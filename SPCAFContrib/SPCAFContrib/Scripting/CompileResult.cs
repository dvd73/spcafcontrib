using System;

namespace SPCAFContrib.Scripting
{
    public class CompileResult
    {
        #region properties

        public bool ResultValue { get; set; }

        public string ResultMessage { get; set; }
        public Exception ResultException { get; set; }

        #endregion
    }
}
