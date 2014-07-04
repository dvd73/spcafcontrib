using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAF.Sdk.Model;
using SPCAFContrib.Extensions;
using SPCAFContrib.Test.Common;
using SPCAFContrib.Test.Extensions;
using SPCAFContrib.Test.UnsafeStringComporation;

namespace SPCAFContrib.Test
{
    public static class AppKeys
    {
        public const string Security_ServiceAccount = "Security.ServiceAccount";
        public const string Search_Scope_Global = "Search.Scope.Global";

        private const string TRACECATEGORY = "SP.SharePointUtility";
        private const string MasterPageUrlTemplate = "/_catalogs/masterpage/{0}";
    }

    [TestClass]
    public class ICSharpCodeTest : CodeTestBase
    {
        #region properties

        protected UnsafeStringComporationImpl Instance = new UnsafeStringComporationImpl();

        #endregion

        [TestMethod]
        public void Negative_GetDate_ToString()
        {
            WithTargetType(typeof(AppKeys).Name, (assembly, type) =>
            {

                var tt = CachedOperationExtensions.SearchPublicNotEmptyOrNullTypeConsts(new[] { new AssemblyFileReference() { AssemblyDefinition = assembly }, })
                                                                 .Select(s => s.Constant as string)
                                                                 .Distinct()
                                                                 .ToDictionary(s => s);


                assembly.TraceTypes();
                type.TraceFields();
                type.TraceMethodInstructions();
            });

            WithTargetMethod(() => Instance.Negative_Linq_Expression(), (assembly, type, method) =>
            {
                type.TraceFields();
                type.TraceMethodInstructions();


                //var astBuilder = new AstBuilder(new ICSharpCode.Decompiler.DecompilerContext(assembly.MainModule));

                //astBuilder.AddType(type);

                //if (method.IsPublic && !method.IsGetter && !method.IsSetter && !method.IsConstructor)
                //{
                //    Console.WriteLine("M:{0}", method.Name);
                //    using (var output = new StringWriter())
                //    {
                //        astBuilder.GenerateCode(new PlainTextOutput(output));
                //        Trace.Write(output.ToString());
                //    }
                //}
                //var unsafeCasts = method.GetUnsafeSPListItemCastInstructions();
                //Assert.AreEqual(1, unsafeCasts.Count());
            });
        }
    }
}
