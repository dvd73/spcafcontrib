using Microsoft.SharePoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Extensions;
using SPCAFContrib.Test.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Test.Extensions;

namespace SPCAFContrib.Test.MagicStrings.Tests
{
    public class ItemTest
    {
        public void Test(SPList list)
        {
            var item = list.Items[0];

            var idValue = (int)item["id"];
            var titleValue = item["title"].ToString();
            var timeValue = (DateTime)item["time"];
        }
    }

    [TestClass]
    public class MagicStringsTest : CodeTestBase
    {
        #region properties

        protected MagicStringsClassImpl Instance = new MagicStringsClassImpl();

        #endregion

        #region tests


        [TestMethod]
        public void TraceSPListItem()
        {
            WithTargetType(typeof(ItemTest).Name, (assembly, type) =>
            {
                type.TraceMethodInstructions();
            });
        }

        [TestMethod]
        public void CanFindAllMethodMagicStringStuff()
        {
            var magicStringCount = 0;

            WithTargetType(Instance.GetType().Name, (assembly, type) =>
            {
                type.TraceMethodInstructions();

                var typeCosts = new List<string>();
                type.SearchConstStrings(field => typeCosts.Add(field.Constant as string));

                type.SearchMethodStrings((method, instruction) =>
                {
                    if (!method.IsConstructor)
                    {
                        Trace.WriteLine(string.Format("Method:[{0}]", method.Name));
                        Trace.WriteLine(string.Format(" Op:[{0}] Value:[{1}]", instruction.OpCode,
                                                      instruction.Operand));

                        var value = instruction.Operand as string;

                        // check if it is const inside class
                        if (!string.IsNullOrEmpty(value) && !typeCosts.Contains(value))
                        {
                            magicStringCount++;
                        }

                    }
                });
            });

            Assert.AreEqual(2, magicStringCount);
        }

        [TestMethod]
        public void CanFindAllMethodMagicNumberStuff()
        {
            WithTargetType(Instance.GetType().Name, (assembly, type) =>
            {
                type.TraceMethodInstructions();

                var dict = new Dictionary<string, int>();

                type.SearchMethodNumbers((method, instruction) =>
                {
                    Trace.WriteLine(string.Format("Method:[{0}]", method.Name));
                    Trace.WriteLine(string.Format(" Op:[{0}] Value:[{1}]", instruction.OpCode, instruction.Operand));

                    if (!dict.ContainsKey(method.Name))
                        dict.Add(method.Name, 0);

                    dict[method.Name] = dict[method.Name] + 1;
                });

                Assert.AreEqual(dict["MagicStringMethod"], 4);
                Assert.AreEqual(dict["DefaultMagicZero"], 4);
                Assert.AreEqual(dict["DefaultMagicOne"], 4);
                Assert.AreEqual(dict.ContainsKey("DefaultMagicNull"), false);
                Assert.AreEqual(dict["DefaultMagicNullable"], 4);
            });
        }

        #endregion
    }
}
