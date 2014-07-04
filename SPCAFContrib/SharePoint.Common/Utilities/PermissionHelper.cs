using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using Microsoft.SharePoint;
using SharePoint.Common.Utilities.Extensions;

namespace SharePoint.Common.Utilities
{
    public class PermissionHelper : SingletoneHelper
    {
        #region Singeton interface

        public static PermissionHelper Instance
        {
            get { return GetInstance<PermissionHelper>(); }
        }

        #endregion 

        public bool IsAuthenticated(HttpContext ctx)
        {
            return ctx.User.Identity.IsAuthenticated;
        }        

        public bool IsSiteCollAdmin(SPWeb web)
        {
            return web.CurrentUser != null ? web.CurrentUser.IsSiteAdmin : false;
        }        

        /// <summary>
        /// Проверяет, есть ли указанная группа в списке групп безопастности сайта
        /// </summary>
        /// <param name="web"></param>
        /// <param name="group_name"></param>
        /// <returns></returns>
        public bool CheckSiteGroup(SPWeb web, String group_name)
        {
            bool groupExists = false;
            foreach (SPGroup oGroup in web.SiteGroups)
            {
                if (String.Equals(oGroup.Name, group_name, StringComparison.CurrentCultureIgnoreCase))
                {
                    groupExists = true;
                    break;
                }
            }

            return groupExists;
        }

        public void RemoveSiteGroup(SPWeb web, String siteGroupName)
        {
            if (String.IsNullOrEmpty(siteGroupName))
            {
                throw new ArgumentNullException("groupName");
            }

            bool allowUnsafeUpdates = web.AllowUnsafeUpdates;
            web.AllowUnsafeUpdates = true;

            try
            {
                web.SiteGroups.Remove(siteGroupName);
            }
            catch (SPException e)
            {
                if (e.ErrorCode != -2146232832)
                {
                    throw e;
                }
            }
            finally
            {
                web.AllowUnsafeUpdates = allowUnsafeUpdates;
            }
        }
        
        public void RemoveGroup(SPWeb web, String groupName)
        {
            if (String.IsNullOrEmpty(groupName))
            {
                throw new ArgumentNullException("groupName");
            }

            bool allowUnsafeUpdates = web.AllowUnsafeUpdates;
            web.AllowUnsafeUpdates = true;

            try
            {
                web.Groups.Remove(groupName);
            }
            catch (SPException e)
            {
                if (e.ErrorCode != -2146232832)
                {
                    throw e;
                }
            }
            finally
            {
                web.AllowUnsafeUpdates = allowUnsafeUpdates;
            }
        }

        public void RemoveRole(SPWeb web, SPRoleDefinition role)
        {
            bool allowUnsafeUpdates = web.AllowUnsafeUpdates;
            web.AllowUnsafeUpdates = true;

            try
            {
                web.RoleDefinitions.Delete(role.Name);

            }
            catch (SPException e)
            {
                if (e.ErrorCode != -2146232832)
                {
                    throw e;
                }
            }
            finally
            {
                web.AllowUnsafeUpdates = allowUnsafeUpdates;
            }
        }

        public SPGroup AddSiteGroup(SPWeb web, String groupName, String groupDescription)
        {
            if (String.IsNullOrEmpty(groupName))
            {
                throw new ArgumentNullException("groupName");
            }

            foreach (SPGroup group in web.SiteGroups)
            {
                if (String.Compare(group.Name, groupName) == 0)
                {
                    return group;
                }
            }

            bool allowUnsafeUpdates = web.AllowUnsafeUpdates;
            web.AllowUnsafeUpdates = true;
            try
            {
                if (String.IsNullOrEmpty(groupDescription))
                {
                    web.SiteGroups.Add(groupName, web.CurrentUser, web.CurrentUser, groupName);
                }
                else
                {
                    web.SiteGroups.Add(groupName, web.CurrentUser, web.CurrentUser, groupDescription);
                }
            }
            finally
            {
                web.AllowUnsafeUpdates = allowUnsafeUpdates;
            }

            return web.SiteGroups[groupName];
        }       

        public SPGroup GetGroupByName(SPWeb web, string groupName)
        {
            if (CheckSiteGroup(web, groupName))
            {
                SPGroup group = web.SiteGroups[groupName];
                return group;
            }
            return null;
        }

        public SPRoleDefinition EnsureRoleForWeb(SPWeb web, SPRoleDefinition role)
        {
            string roleName = role.Name;
            SPRoleDefinition result = web.RoleDefinitions.OfType<SPRoleDefinition>().
                Where(r => String.Equals(r.Name, roleName, StringComparison.CurrentCultureIgnoreCase)).SingleOrDefault();

            if (result == null)
            {
                bool allowUnsafeUpdates = web.AllowUnsafeUpdates;
                web.AllowUnsafeUpdates = true;

                if (!(web.HasUniqueRoleDefinitions || web.IsRootWeb))
                {
                    web.BreakRoleInheritance(true);
                    web.RoleDefinitions.BreakInheritance(true, true);
                    web.Update();
                }

                web.RoleDefinitions.Add(role);
                web.Update();
                web.AllowUnsafeUpdates = allowUnsafeUpdates;
                result = web.RoleDefinitions.OfType<SPRoleDefinition>().
                Where(r => String.Equals(r.Name, roleName, StringComparison.CurrentCultureIgnoreCase)).SingleOrDefault();
            }

            return result;
        }

        public void RemovePermission(SPWeb web, SPSecurableObject obj, SPPrincipal principal, SPRoleDefinition role)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            if (principal == null)
            {
                throw new ArgumentNullException("principal");
            }

            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            bool allowUnsafeUpdates = web.AllowUnsafeUpdates;
            web.AllowUnsafeUpdates = true;
            try
            {
                SPRoleAssignment roleAssing = null;
                try
                {
                    roleAssing = obj.RoleAssignments.GetAssignmentByPrincipal(principal);
                }
                catch (ArgumentException)
                {
                    return;
                }

                if (roleAssing.RoleDefinitionBindings.Contains(role))
                {
                    roleAssing.RoleDefinitionBindings.Remove(role);
                    roleAssing.Update();
                }
            }
            finally
            {
                web.AllowUnsafeUpdates = allowUnsafeUpdates;
            }
        }

        /// <summary>
        /// Remember that workflow is executed in system account context, so you can manage permissions without any problems, 
        /// but event handler is executed in current user context, so you need to elevate user context first.            
        /// </summary>
        /// <param name="web"></param>
        /// <param name="secObj">like SPList, SPListItem</param>
        /// <param name="principal">like SPUser, SPGroup</param>
        /// <param name="role"></param>
        /// <param name="saveAssigments">true = break inheritance with copy assigments</param>
        public void AddRole(SPWeb web, SPSecurableObject secObj, IEnumerable<SPPrincipal> principals, IEnumerable<SPRoleDefinition> roles, bool saveAssigments)
        {
            if (secObj == null)
                throw new ArgumentNullException("obj");

            if (principals == null)
                throw new ArgumentNullException("principals");

            if (roles == null)
                throw new ArgumentNullException("role");

            bool allowUnsafeUpdates = web.AllowUnsafeUpdates;

            try
            {
                web.AllowUnsafeUpdates = true;

                if (!secObj.HasUniqueRoleAssignments)
                {
                    if (saveAssigments)
                        secObj.BreakRoleInheritance(true);
                    else
                        secObj.BreakRoleInheritance(false, true);
                }

                foreach (SPPrincipal principal in principals)
                {
                    SPRoleAssignment roleAssingment = new SPRoleAssignment(principal);
                    foreach (SPRoleDefinition role in roles)
                    {
                        bool roleAssingmentExists = false;

                        foreach (SPRoleAssignment objRoleAssignment in secObj.RoleAssignments)
                        {
                            if (objRoleAssignment.Member.ID == principal.ID &&
                                String.Equals(objRoleAssignment.Member.Name, principal.Name, StringComparison.CurrentCultureIgnoreCase) &&
                                String.Equals(objRoleAssignment.Member.LoginName, principal.LoginName, StringComparison.CurrentCultureIgnoreCase))
                            {
                                roleAssingment = objRoleAssignment;
                                roleAssingmentExists = true;
                                break;
                            }
                        }

                        if (!roleAssingment.RoleDefinitionBindings.Contains(role))
                            roleAssingment.RoleDefinitionBindings.Add(role);

                        if (!roleAssingmentExists)
                            secObj.RoleAssignments.Add(roleAssingment);
                        else
                            roleAssingment.Update();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogError();
            }
            finally
            {
                web.AllowUnsafeUpdates = allowUnsafeUpdates;
            }
        }

        public void ClearPermissions(SPWeb web, SPSecurableObject secObj, SPPrincipal principal, bool saveAssigments)
        {
            if (secObj == null)
            {
                throw new ArgumentNullException("secObj");
            }

            if (principal == null)
            {
                throw new ArgumentNullException("principal");
            }

            bool allowUnsafeUpdates = web.AllowUnsafeUpdates;
            web.AllowUnsafeUpdates = true;
            try
            {
                if (!secObj.HasUniqueRoleAssignments)
                {
                    if (saveAssigments)
                        secObj.BreakRoleInheritance(true);
                    else
                        secObj.BreakRoleInheritance(false, true);
                }
                secObj.RoleAssignments.Remove(principal);

            }
            catch (Exception ex)
            {
                ex.LogError();
            }
            finally
            {
                web.AllowUnsafeUpdates = allowUnsafeUpdates;
            }
        }

        public IEnumerable<SPRoleAssignment> ClearPermissions(SPSecurableObject obj, IEnumerable<SPPrincipal> principals)
        {
            List<SPRoleAssignment> assignmentsToDelete = new List<SPRoleAssignment>();
            foreach (SPPrincipal pr in principals)
            {
                SPRoleAssignment ra = obj.RoleAssignments.GetAssignmentByPrincipal(pr);
                if (ra != null)
                {
                    assignmentsToDelete.Add(ra);
                    obj.RoleAssignments.Remove(pr);
                }
            }
            return assignmentsToDelete;
        }

        public IEnumerable<SPRoleAssignment> ClearPermissions(SPSecurableObject obj, SPBasePermissions exceptPermissions)
        {
            var assignmentsToDelete = new List<SPRoleAssignment>();
            foreach (SPRoleAssignment roleAssignment in obj.RoleAssignments)
            {
                bool hasExceptPermissions = false;
                foreach (SPRoleDefinition roleDefinition in roleAssignment.RoleDefinitionBindings)
                {
                    var mask = roleDefinition.BasePermissions & exceptPermissions;
                    if (mask != SPBasePermissions.EmptyMask)
                    {
                        hasExceptPermissions = true;
                        break;
                    }
                }
                if (hasExceptPermissions == false)
                {
                    assignmentsToDelete.Add(roleAssignment);
                }
            }

            foreach (SPRoleAssignment assignmentToDelete in assignmentsToDelete)
            {
                obj.RoleAssignments.Remove(assignmentToDelete.Member);
            }
            return assignmentsToDelete;
        }

        public IEnumerable<SPRoleAssignment> ClearPermissions(SPSecurableObject obj)
        {
            return ClearPermissions(obj, SPBasePermissions.EmptyMask);
        }                    

        /// <summary>
        /// Use SPSecurity.RunWithElevatedPrivileges if required
        /// </summary>
        /// <param name="web"></param>
        /// <param name="secObj"></param>
        /// <param name="RoleName"></param>
        public void RemoveRoleForAllPrincipals(SPWeb web, SPSecurableObject secObj, SPRoleDefinition role, bool saveAssigments)
        {            
            if (!secObj.HasUniqueRoleAssignments)
            {
                if (saveAssigments)
                    secObj.BreakRoleInheritance(true);
                else
                    secObj.BreakRoleInheritance(false, true);
            }
            foreach (SPRoleAssignment ra in secObj.RoleAssignments)
            {
                if (ra.RoleDefinitionBindings.Contains(role))
                {
                    bool allowUnsafeUpdates = web.AllowUnsafeUpdates;
                    web.AllowUnsafeUpdates = true;                    
                    ra.RoleDefinitionBindings.Remove(role);
                    ra.Update();
                    web.AllowUnsafeUpdates = allowUnsafeUpdates;
                }
            }
        }

        /// <summary>
        ///  Use SPSecurity.RunWithElevatedPrivileges if required
        /// </summary>
        /// <param name="web"></param>
        /// <param name="obj"></param>
        /// <param name="loginName"></param>
        /// <param name="Role"></param>        
        /// <returns></returns>
        public bool DoesUserHaveRoleOnThisObject(SPWeb web, SPSecurableObject obj, string loginName, SPRoleDefinition role)
        {
            bool result = false;

            SPUser usr = null;
            SPGroup grp = null;
            bool isMember = false;

            foreach (SPRoleAssignment ra in obj.RoleAssignments)
            {
                if(ra.Member is SPUser) 
                {
                    usr = ra.Member as SPUser;
                    grp = null;
                }
                else if (ra.Member is SPGroup) 
                {
                    usr = null;
                    grp = ra.Member as SPGroup;
                }

                if (usr == null && grp == null)
                    continue;

                if (usr != null)
                    isMember = String.Equals(usr.LoginName, loginName, StringComparison.CurrentCultureIgnoreCase);

                if (grp != null)
                {
                    SPUser foundUser = grp.Users.OfType<SPUser>().Where(u => String.Equals(u.LoginName, loginName, StringComparison.CurrentCultureIgnoreCase)).SingleOrDefault();                    
                    isMember = foundUser != null;                    
                }

                if (isMember)
                {                    
                    if (ra.RoleDefinitionBindings.Contains(role))
                    {
                        result = true;
                        break;
                    }
                }

                isMember = false;
                usr = null;
                grp = null;
            }

            return result;
        }

        public void SafeResetRoleInheritance(SPWeb web, SPSecurableObject secObj)
        {
            bool oldAllowUnsafeUpdate = web.AllowUnsafeUpdates;
            secObj.ResetRoleInheritance();
            if (web.AllowUnsafeUpdates != oldAllowUnsafeUpdate)
                web.AllowUnsafeUpdates = oldAllowUnsafeUpdate;
        }

        public void SafeBreakRoleInheritance(SPWeb web, SPSecurableObject secObj, bool copyRoleAssignments)
        {
            bool oldAllowUnsafeUpdate = web.AllowUnsafeUpdates;
            secObj.BreakRoleInheritance(copyRoleAssignments, true);
            if (web.AllowUnsafeUpdates != oldAllowUnsafeUpdate)
                web.AllowUnsafeUpdates = oldAllowUnsafeUpdate;
        }
    }
}
