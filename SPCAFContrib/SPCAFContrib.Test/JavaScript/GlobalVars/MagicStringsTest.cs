using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.JavaScript.GlobalVars
{
    [TestClass]
    public class JavaScriptGlobalVarTest : CodeTestBase
    {
        #region tests

        [TestMethod]
        public void CangetGlobalVariables()
        {
            var scriptContent = @"
                $.test();
            ";

            var engine = new Jurassic.ScriptEngine();
            var result = engine.Evaluate(scriptContent);
        }

        #endregion
    }
}
