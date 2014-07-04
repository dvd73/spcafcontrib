using System;
using System.Runtime.Serialization;

namespace SPCAFContrib.Scripting
{
    [Serializable]
    public class ScriptingRuntimeException : Exception
    {
        #region contructors

        public ScriptingRuntimeException() { }
        public ScriptingRuntimeException(string message) : base(message) { }
        public ScriptingRuntimeException(string message, Exception inner) : base(message, inner) { }

        protected ScriptingRuntimeException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        #endregion
    }
}
