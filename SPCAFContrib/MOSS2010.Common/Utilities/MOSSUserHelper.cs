using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.SharePoint;
using Microsoft.Office.Server.Search.Query;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Administration;
using Microsoft.Office.Server.UserProfiles;
using SharePoint.Common.Utilities;

namespace MOSS.Common.Utilities
{
    public class MOSSUserHelper : UserHelper
    {
        public new static MOSSUserHelper Instance
        {
            get { return GetInstance<MOSSUserHelper>(); }
        }        

        const string UPQuery1 = "SELECT  AccountName, FirstName, LastName, WorkEmail " +
                                    "FROM Scope() " +
                                    "WHERE ( (\"SCOPE\" = 'People') )  AND (CONTAINS (FirstName, '{0}') OR CONTAINS (LastName, '{0}') OR CONTAINS (UserName, '{0}'))" +
                                    "ORDER BY \"Rank\" DESC";

        const string UPQuery2 = "SELECT  AccountName, FirstName, LastName, WorkEmail " +
                                    "FROM Scope() " +
                                    "WHERE ( (\"SCOPE\" = 'People') )  AND (CONTAINS (FirstName, '{0}') AND CONTAINS (LastName, '{1}') OR CONTAINS (FirstName, '{1}') AND CONTAINS (LastName, '{0}') )" +
                                    "ORDER BY \"Rank\" DESC";


        /// <summary>
        /// Use FullTextSqlQuery to find people
        /// </summary>
        /// <param name="web"></param>
        /// <param name="recipient"></param>
        /// <returns></returns>
        public UserResolvedInfo FindPerson(SPWeb web, string recipient)
        {
            ResultTableCollection resultTables = null;
            UserResolvedInfo result = new UserResolvedInfo(recipient);

            try
            {
                using (FullTextSqlQuery qRequest = new FullTextSqlQuery(web.Site))
                {
                    string[] names = recipient.Trim().Split(' ');
                    if (names.Length > 1)
                        qRequest.QueryText = String.Format(UPQuery2, names[0], names[1]);
                    else
                        qRequest.QueryText = String.Format(UPQuery1, recipient);
                    qRequest.ResultTypes = ResultType.RelevantResults;
                    qRequest.TrimDuplicates = false;
                    qRequest.RowLimit = 500;

                    resultTables = qRequest.Execute();
                }
            }
            catch (Exception ex)
            {
                result.ResolveInfo = ex.Message;
            }

            if (resultTables != null && resultTables.Count > 0)
            {
                ResultTable relevantResults = resultTables[ResultType.RelevantResults];

                if (relevantResults.RowCount == 0)
                {
                    result.Resolved = false;
                    result.ResolveInfo = "External";
                }
                else if (relevantResults.RowCount > 1)
                {
                    string email_tmp = String.Empty;
                    bool differentUsers = false;

                    foreach (DataRow row in relevantResults.Table.Rows)
                    {
                        if (String.IsNullOrEmpty(row[3].ToString())) continue;

                        if (String.IsNullOrEmpty(email_tmp))
                            email_tmp = row[3].ToString();
                        else if (email_tmp != row[3].ToString())
                            differentUsers = true;
                    }

                    if (differentUsers)
                    {
                        result.Resolved = false;
                        result.ResolveInfo = "Too many users";
                    }
                    else
                    {
                        DataRow row = relevantResults.Table.Rows[relevantResults.RowCount - 1];

                        result.Resolved = true;
                        result.LoginName = row[0].ToString();
                        result.DisplayName = String.Format("{0} {1}", row[1], row[2]);
                        if (String.IsNullOrEmpty(result.DisplayName))
                            result.DisplayName = result.LoginName;
                        result.Email = email_tmp;
                    }
                }
                else
                {
                    DataRow row = relevantResults.Table.Rows[0];

                    result.Resolved = true;
                    result.LoginName = row[0].ToString();
                    result.DisplayName = String.Format("{0} {1}", row[1], row[2]);
                    if (String.IsNullOrEmpty(result.DisplayName))
                        result.DisplayName = result.LoginName;
                    result.Email = row[3].ToString();
                }
            }

            return result;
        }

        /// <summary>
        /// Use SPUtility.SearchPrincipals
        /// </summary>
        /// <param name="web"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public IList<UserResolvedInfo> ResoveUsersForWebByName(SPWeb web, string userName)
        {
            List<UserResolvedInfo> result = new List<UserResolvedInfo>();
            bool reachedMaxCount = false;
            IList<SPPrincipalInfo> users = SPUtility.SearchPrincipals(web, userName, SPPrincipalType.User, SPPrincipalSource.All, null, 100, out reachedMaxCount);
            if (users.Count > 0)
            {
                foreach (SPPrincipalInfo principalInfo in users)
                {
                    result.Add(new UserResolvedInfo(principalInfo));
                }
            }

            return result;
        }

        /// <summary>
        /// Return a SPPrincipalInfo object based on a string that contains the user name (DOMAIN\User_Alias).
        /// </summary>
        /// <param name="login_name">Name of the login.</param>
        public SPPrincipalInfo GetPrincipalInfo(SPWebApplication app, string login_name)
        {
            return SPUtility.ResolveWindowsPrincipal(app, login_name, SPPrincipalType.User, false);
        }
    }

    public class UserResolvedInfo
    {
        public int Id { get; set; }
        public string LoginName;
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public bool Resolved { get; set; }
        public string ResolveInfo { get; set; }

        public UserResolvedInfo()
        {
            Resolved = false;
        }

        public UserResolvedInfo(string name)
        {
            this.DisplayName = name;
        }

        public UserResolvedInfo(SPPrincipalInfo principalInfo)
        {
            Id = principalInfo.PrincipalId;
            DisplayName = principalInfo.DisplayName;
            LoginName = principalInfo.LoginName;
            Email = principalInfo.Email;
            Resolved = true;
        }
    }
}
