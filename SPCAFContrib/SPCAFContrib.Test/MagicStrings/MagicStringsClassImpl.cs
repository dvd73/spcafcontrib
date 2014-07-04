using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCAFContrib.Test.MagicStrings
{
    public class MagicStringsClassImpl
    {
        #region contructions

        public MagicStringsClassImpl()
        {

        }

        #endregion

        #region properties

        private const string CustomerListTitle = "Customers";
        public string NonMagicField = "NonMagicField";

        public string Tmp
        {
            get { return "tmp.magic"; }
        }

        #endregion

        #region methods

        public void MagicStringMethodCall()
        {
            var tt = CustomerListTitle;

            TestCall(CustomerListTitle);
            TestCall("Customers");
        }

        public void TestCall(string tmp)
        {

        }

        public void MagicStringMethod()
        {
            var tmpString = "MagicString";
            var tmpInt = 1500;
            var tmpFloat = 1700f;
            var tmpDouble = 42.42d;
            var tmpLong = 100500ul;
        }

        public void DefaultMagicZero()
        {
            var tmpString = string.Empty;
            var tmpInt = 0;
            var tmpFloat = 0f;
            var tmpDouble = 0d;
            var tmpLong = 0ul;
        }

        public void DefaultMagicOne()
        {
            var tmpString = "";
            var tmpInt = 1;
            var tmpFloat = 1f;
            var tmpDouble = 1d;
            var tmpLong = 1ul;
        }

        public void DefaultMagicNull()
        {
            var tmpString = (string)null;
            var tmpInt = (int?)null;
            var tmpFloat = (float?)null;
            var tmpDouble = (double?)null;
            var tmpLong = (long?)null;
        }

        public void DefaultMagicNullable()
        {
            var tmpString = (string)null;
            var tmpInt = (int?)12;
            var tmpFloat = (float?)13;
            var tmpDouble = (double?)14;
            var tmpLong = (long?)15;
        }

        #endregion
    }
}
