using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.BusinessData.Infrastructure.SecureStore;
using Microsoft.Office.SecureStoreService.Server;

namespace MOSS.Common.Utilities
{
    /// <summary>
    /// Read account credential programmatically
    /// <see href="http://www.projectserver2010blog.com/2012/02/how-to-use-secure-store-id-for.html">How to use Secure Store Id for Impersonation programmatically</see>
    /// </summary>
    public class SecureStoreHelper
    {
        public SSIdentityInfo GetCredentials(SPSite site, string targetApplicationId)
        {
            var credentials = GetWindowsCredentials(site, targetApplicationId);
            return ParseSecureStoreCredentials(credentials);
        }

        private SSIdentityInfo ParseSecureStoreCredentials(Dictionary<SecureStoreCredentialType, string> credentials)
        {
            if (!credentials.ContainsKey(SecureStoreCredentialType.WindowsUserName))
                throw new ArgumentNullException("credentials", "WindowsUserName field not defined");

            if (!credentials.ContainsKey(SecureStoreCredentialType.WindowsPassword))
                throw new ArgumentNullException("credentials", "WindowsPassword field not defined");

            string windowsUserName = credentials[SecureStoreCredentialType.WindowsUserName];
            string windowsPassword = credentials[SecureStoreCredentialType.WindowsPassword];

            SSIdentityInfo result = new SSIdentityInfo();
            result.Password = windowsPassword;

            if (string.IsNullOrEmpty(windowsUserName))
                throw new ArgumentNullException("credentials", "windowsUserName not defined");

            // treat domain\username
            if (windowsUserName.Contains("\\"))
            {
                var parts = windowsUserName.Split('\\');

                result.Domain = parts[0];
                result.Username = parts[1];
            }
            // treat username@domain
            else if (windowsUserName.Contains("@"))
            {
                var parts = windowsUserName.Split('@');

                result.Username = parts[0];
                result.Domain = parts[1];
            }
            else
            {
                result.Username = windowsUserName;
            }

            return result;
        }

        private Dictionary<SecureStoreCredentialType, string> GetWindowsCredentials(SPSite site, string targetApplicationId)
        {
            Dictionary<SecureStoreCredentialType, string> result = new Dictionary<SecureStoreCredentialType, string>();

            using (var tmpSite = new SPSite(site.ID))
            {
                SPServiceContext serviceContext = SPServiceContext.GetContext(tmpSite.WebApplication.ServiceApplicationProxyGroup, SPSiteSubscriptionIdentifier.Default);
                SecureStoreProvider secureStoreProvider = new SecureStoreProvider { Context = serviceContext };

                using (SecureStoreCredentialCollection credentials = secureStoreProvider.GetCredentials(targetApplicationId))
                {
                    ReadOnlyCollection<ITargetApplicationField> fields =
                        secureStoreProvider.GetTargetApplicationFields(targetApplicationId);
                    for (var i = 0; i < fields.Count; i++)
                    {
                        ITargetApplicationField field = fields[i];

                        ISecureStoreCredential credential = credentials[i];
                        string decryptedCredential = ToClrString(credential.Credential);

                        result.Add(field.CredentialType, decryptedCredential);
                    }
                }
            }

            return result;
        }

        private string ToClrString(SecureString secureString)
        {
            var ptr = Marshal.SecureStringToBSTR(secureString);

            try
            {
                return Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                Marshal.FreeBSTR(ptr);
            }
        }
    }

    public class SSIdentityInfo
    {
        public string Domain { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
