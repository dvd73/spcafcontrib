using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SPCAFContrib.Test.ItemBindingEvents
{
    public class BadItemBindingImpl
    {
        #region constuctors

        public BadItemBindingImpl()
        {
            ConstructorAssignedView.ItemCreated += Positive_ConstructorAssignedView_ItemDataBound;
            ConstructorAssignedView.ItemDataBound += Positive_ConstructorAssignedView_ItemCreated;

            ConstructorAssignedDataListView.ItemCreated += Positive_ConstructorAssignedDataListView_ItemCreated;
            ConstructorAssignedDataListView.ItemDataBound += Positive_ConstructorAssignedDataListView_ItemDataBound;
        }

        public void Positive_ConstructorAssignedDataListView_ItemDataBound(object sender, DataListItemEventArgs e)
        {

        }

        public void Positive_ConstructorAssignedDataListView_ItemCreated(object sender, DataListItemEventArgs e)
        {

        }

        public void Positive_ConstructorAssignedView_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

        }

        public void Positive_ConstructorAssignedView_ItemCreated(object sender, RepeaterItemEventArgs e)
        {

        }

        #endregion

        #region properties

        protected Repeater ConstructorAssignedView = new Repeater();
        protected Repeater IsPostBackAssignedView = new Repeater();

        protected DataList ConstructorAssignedDataListView = new DataList();
        protected DataList IsPostBackAssignedDataListView = new DataList();

        #endregion

        #region methods

        public void RenderOnPade(Page page)
        {
            if (page.IsPostBack)
            {
                IsPostBackAssignedView.ItemCreated += Positive_IsPostBackAssignedView_ItemCreated;
                IsPostBackAssignedView.ItemDataBound += Positive_IsPostBackAssignedView_ItemDataBound;

                IsPostBackAssignedDataListView.ItemCreated += Positive_IsPostBackAssignedDataListView_ItemCreated;
                IsPostBackAssignedDataListView.ItemDataBound += Positive_IsPostBackAssignedDataListView_ItemDataBound;
            }
        }

        public void Positive_IsPostBackAssignedDataListView_ItemDataBound(object sender, DataListItemEventArgs e)
        {

        }

        public void Positive_IsPostBackAssignedDataListView_ItemCreated(object sender, DataListItemEventArgs e)
        {

        }

        public void Positive_IsPostBackAssignedView_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

        }

        public void Positive_IsPostBackAssignedView_ItemCreated(object sender, RepeaterItemEventArgs e)
        {

        }


        #endregion

        #region action

        #endregion
    }
}
