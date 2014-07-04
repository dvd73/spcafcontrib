namespace SPCAFContrib.Scripting
{
    public class ResolveStartEntriesContext
    {
        #region contructors

        #endregion

        #region properties

        public string CurrentScript { get; set; }
        public object CurrentCompiledScript { get; set; }

        public string CurrentStartObjectName { get; set; }
        public string CurrentStartMethodName { get; set; }

        public ExecutionContext Context { get; set; }

        #endregion
    }
}
