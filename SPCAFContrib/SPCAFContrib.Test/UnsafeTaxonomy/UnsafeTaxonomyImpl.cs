using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Taxonomy;

namespace SPCAFContrib.Test.UnsafeTaxonomy
{
    public class UnsafeTaxonomyImpl
    {
        #region GroupCollection

        public void Negative_GroupIndex_String()
        {
            var group = (GroupCollection)null;
            var targetGroup = group["testGroup"];
        }

        public void Positive_GroupIndex_Int()
        {
            var group = (GroupCollection)null;
            var targetGroup = group[1];
        }

        #endregion

        #region TermSetCollection

        public void Negative_TermSetCollectionIndex_String()
        {
            var group = (TermSetCollection)null;
            var targetSet = group["testGroup"];
        }

        public void Positive_TermSetCollectionIndex_Int()
        {
            var group = (TermSetCollection)null;
            var targetSet = group[1];
        }

        public void Positive_TermSetCollectionIndex_Guid()
        {
            var group = (TermSetCollection)null;
            var targetSet = group[Guid.NewGuid()];
        }

        #endregion
    }
}
