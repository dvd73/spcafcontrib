using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace SPCAFContrib.Test.UnsafeSPListCollectionCalls
{
    public class UnsafeSPListCollectionCall
    {
        #region negative

        public void Negative_EnumeratorCall()
        {
            SPWeb web = null;

            foreach (var list in web.Lists)
            {
                //Console.WriteLine(list);
            }
        }

        public void Negative_EnumeratorLinqCastCall()
        {
            SPWeb web = null;
            var lists = web.Lists.Cast<SPList>();
        }

        public void Negative_EnumeratorLinqOfTypeCall()
        {
            SPWeb web = null;
            var lists = web.Lists.OfType<SPList>();
        }

        public void Negative_StringIndexCall()
        {
            SPWeb web = null;
            string title = "1";

            var list = web.Lists[title];
        }

        #endregion

        #region povitive

        public void Positive_IntIndexCall()
        {
            SPWeb web = null;
            var index = 1;

            var list = web.Lists[index];
        }

        public void Positive_GuidIndexCall()
        {
            SPWeb web = null;
            var id = Guid.NewGuid();

            var list = web.Lists[id];
        }

        #endregion
    }
}
