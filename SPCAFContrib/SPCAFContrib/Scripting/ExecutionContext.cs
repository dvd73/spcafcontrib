using System;
using System.Collections.Generic;

namespace SPCAFContrib.Scripting
{
    public class ExecutionContext
    {
        #region contructors

        public ExecutionContext()
        {
            Parameters = new List<ScriptMethodParameter>();
            GlobalParameters = new List<ScriptMethodParameter>();

            ReferencedAssemblies = new List<string>();
        }

        #endregion

        #region events

        public Func<ResolveStartEntriesContext, ResolveStartEntriesResult> OnStartEntriesResolving;

        #endregion

        #region properties

        public string Script { get; set; }

        public string StartObjectName { get; set; }
        public string StartMethodName { get; set; }

        public List<string> ReferencedAssemblies { get; set; }

        public List<ScriptMethodParameter> Parameters { get; set; }
        public List<ScriptMethodParameter> GlobalParameters { get; set; }

        #endregion


    }
}
