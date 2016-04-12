using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Entity;
using System.Web.Security;
using SimDms.Web.Models;

namespace SimDms.Web
{
    public class SqlRoleProvider : RoleProvider
    {
        public override string ApplicationName
        {
            get
            {
                return this.GetType().Assembly.GetName().Name.ToString();
            }
            set
            {
                this.ApplicationName = this.GetType().Assembly.GetName().Name.ToString();
            }
        }

        public override bool RoleExists(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return false;
            }
            using (LayoutContext ctx = new LayoutContext())
            {
                SysRole Role = null;
                Role = ctx.SysRoles.FirstOrDefault(Rl => Rl.GroupId == roleName);
                if (Role != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            if (string.IsNullOrEmpty(roleName))
            {
                return false;
            }
            using (LayoutContext Context = new LayoutContext())
            {
                SysUser User = null;
                User = Context.SysUsers.Find(username);
                if (User == null)
                {
                    return false;
                }
                SysRole Role = Context.SysRoles.Find(roleName);
                if (Role == null)
                {
                    return false;
                }
                //return User.Roles.Contains(Role);
                return false;
            }
        }

        public override string[] GetAllRoles()
        {
            using (LayoutContext Context = new LayoutContext())
            {
                return Context.SysRoles.Select(Rl => Rl.GroupName).ToArray();
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return null;
            }
            using (LayoutContext Context = new LayoutContext())
            {
                SysRole Role = null;
                Role = Context.SysRoles.Find(roleName);
                if (Role != null)
                {
                    //return Role.Users.Select(Usr => Usr.UserId).ToArray();
                    return null;
                }
                else
                {
                    return null;
                }
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }
            using (LayoutContext Context = new LayoutContext())
            {
                SysUser User = null;
                User = Context.SysUsers.Find(username);
                if (User != null)
                {
                    return null;
                    //return User.Roles.Select(Rl => Rl.GroupId).ToArray();
                }
                else
                {
                    return null;
                }
            }
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return null;
            }

            if (string.IsNullOrEmpty(usernameToMatch))
            {
                return null;
            }

            //using (LayoutContext Context = new LayoutContext())
            //{
            //    return (from Rl in Context.SysRoles from Usr in Rl.Users where Rl.GroupId == roleName && Usr.UserId.Contains(usernameToMatch) select Usr.UserId).ToArray();
            //}

            return null;
        }

        public override void CreateRole(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                using (LayoutContext Context = new LayoutContext())
                {
                    SysRole Role = null;
                    Role = Context.SysRoles.Find(roleName);
                    if (Role == null)
                    {
                        SysRole NewRole = new SysRole
                        {
                            GroupId = roleName,
                            GroupName = roleName,
                            IsActive = true,
                            IsAdmin = false
                        };
                        Context.SysRoles.Add(NewRole);
                        Context.SaveChanges();
                    }
                }
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return false;
            }
            using (LayoutContext Context = new LayoutContext())
            {
                SysRole Role = null;
                Role = Context.SysRoles.Find(roleName);
                if (Role == null)
                {
                    return false;
                }
                //if (throwOnPopulatedRole)
                //{
                //    if (Role.Users.Any())
                //    {
                //        return false;
                //    }
                //}
                //else
                //{
                //    Role.Users.Clear();
                //}
                //Context.SysRoles.Remove(Role);
                //Context.SaveChanges();
                return true;
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            using (LayoutContext Context = new LayoutContext())
            {
                List<SysUser> Users = Context.SysUsers.Where(Usr => usernames.Contains(Usr.UserId)).ToList();
                List<SysRole> Roles = Context.SysRoles.Where(Rl => roleNames.Contains(Rl.GroupId)).ToList();
                //foreach (SysUser user in Users)
                //{
                //    foreach (SysRole role in Roles)
                //    {
                //        if (!user.Roles.Contains(role))
                //        {
                //            user.Roles.Add(role);
                //        }
                //    }
                //}
                Context.SaveChanges();
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            using (LayoutContext Context = new LayoutContext())
            {
                foreach (String username in usernames)
                {
                    String us = username;
                    SysUser user = Context.SysUsers.Find(us);
                    //if (user != null)
                    //{
                    //    foreach (String roleName in roleNames)
                    //    {
                    //        String rl = roleName;
                    //        SysRole role = user.Roles.FirstOrDefault(R => R.GroupId == rl);
                    //        if (role != null)
                    //        {
                    //            user.Roles.Remove(role);
                    //        }
                    //    }
                    //}
                }
                Context.SaveChanges();
            }
        }
    }
}