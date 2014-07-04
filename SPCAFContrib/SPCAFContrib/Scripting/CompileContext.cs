using System.Collections.Generic;

namespace SPCAFContrib.Scripting
{
    public class CompileContext
    {
        #region contructors

        public CompileContext()
        {
            ReferencedAssemblies = new List<string>();
        }

        #endregion

        #region properties

        public string Script { get; set; }
        public string StartEntryName { get; set; }

        public List<string> ReferencedAssemblies { get; set; }

        #endregion
    }
}
