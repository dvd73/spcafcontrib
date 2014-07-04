using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCAFContrib.Test.UnsafeSPListItem
{
    public class BadSPListItemServiceImpl
    {
        #region methods

        #region int

        public int Negative_GetInt_ToString(SPList list)
        {
            return int.Parse(list.Items[0]["id"].ToString());
        }

        public int Negative_GetInt_Cast(SPList list)
        {
            return (int)list.Items[0]["id"];
        }

        public int? Negative_GetInt_CastNullable(SPList list)
        {
            return (int?)list.Items[0]["id"];
        }

        public int? Positive_GetDate_ConvertToInt16(SPList list)
        {
            return Convert.ToInt16(list.Items[0]["id"]);
        }

        public int? Positive_GetDate_ConvertToInt32(SPList list)
        {
            return Convert.ToInt32(list.Items[0]["id"]);
        }

        public long? Positive_GetDate_ConvertToInt64(SPList list)
        {
            return Convert.ToInt64(list.Items[0]["id"]);
        }

        public int? Positive_GetInt_As(SPList list)
        {
            return list.Items[0]["id"] as int?;
        }

        public long? Positive_GetLong_As(SPList list)
        {
            return list.Items[0]["id"] as long?;
        }

        #endregion

        #region DateTime

        public DateTime Negative_GetDate_ToString(SPList list)
        {
            return DateTime.Parse(list.Items[0]["Date"].ToString());
        }

        public DateTime Negative_GetDate_Cast(SPList list)
        {
            return (DateTime)list.Items[0]["Date"];
        }

        public DateTime? Negative_GetDate_CastNullable(SPList list)
        {
            return (DateTime?)list.Items[0]["Date"];
        }

        public DateTime? Positive_GetDate_ConvertToDateTime(SPList list)
        {
            return Convert.ToDateTime(list.Items[0]["Date"]);
        }

        public DateTime? Positive_GetDate_As(SPList list)
        {
            return list.Items[0]["Date"] as DateTime?;
        }

        #endregion

        #region user value

        public int Negative_GetSPFieldUserValue_Cast(SPList list)
        {
            return ((SPFieldUserValue)list.Items[0]["User"]).LookupId;
        }

        #endregion

        #endregion
    }
}
