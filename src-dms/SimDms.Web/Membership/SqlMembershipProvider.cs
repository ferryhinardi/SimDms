using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Security;
using SimDms.Web.Models;

namespace SimDms.Web
{
    public class SqlMembershipProvider : MembershipProvider
    {
        #region Properties

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

        public override int MaxInvalidPasswordAttempts
        {
            get { return 5; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return 0; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 6; }
        }

        public override int PasswordAttemptWindow
        {
            get { return 0; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return MembershipPasswordFormat.Hashed; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return String.Empty; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

        #endregion

        #region Functions

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            if (string.IsNullOrEmpty(username))
            {
                status = MembershipCreateStatus.InvalidUserName;
                return null;
            }
            if (string.IsNullOrEmpty(password))
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }
            if (string.IsNullOrEmpty(email))
            {
                status = MembershipCreateStatus.InvalidEmail;
                return null;
            }

            string HashedPassword = Crypto.HashPassword(password);
            if (HashedPassword.Length > 128)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            using (LayoutContext ctx = new LayoutContext())
            {
                if (ctx.SysUsers.Where(Usr => Usr.Email == email).Any())
                {
                    status = MembershipCreateStatus.DuplicateEmail;
                    return null;
                }

                SysUser NewUser = new SysUser
                {
                    UserId = username,
                    Password = HashedPassword,
                    FullName = username,
                    Email = email,
                    IsActive = isApproved,
                };

                ctx.SysUsers.Add(NewUser);
                ctx.SaveChanges();
                status = MembershipCreateStatus.Success;
                return new MembershipUser(Membership.Provider.Name, NewUser.UserId, NewUser.UserId, NewUser.Email, "", "", NewUser.IsActive, !NewUser.IsActive, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
            }
        }

        public override bool ValidateUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            if (string.IsNullOrEmpty(password))
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
                if (!User.IsActive)
                {
                    return false;
                }

                Boolean VerificationSucceeded = (User.Password == (FormsAuthentication.HashPasswordForStoringInConfigFile(password, "md5")));
                if (VerificationSucceeded)
                {
                    Console.WriteLine("Validation Success");
                }
                else
                {
                    Console.WriteLine("Validation Not Success");
                }
                Context.SaveChanges();
                if (VerificationSucceeded)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
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
                    if (userIsOnline)
                    {
                        Context.SaveChanges();
                    }
                    return new MembershipUser(Membership.Provider.Name, User.UserId, User.UserId, User.Email, null, null, User.IsActive, !User.IsActive, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
                }
                else
                {
                    return null;
                }
            }
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            if (providerUserKey is Guid) { }
            else
            {
                return null;
            }

            using (LayoutContext Context = new LayoutContext())
            {
                SysUser User = null;
                User = Context.SysUsers.Find(providerUserKey);
                if (User != null)
                {
                    if (userIsOnline)
                    {
                        Context.SaveChanges();
                    }
                    return new MembershipUser(Membership.Provider.Name, User.UserId, User.UserId, User.Email, null, null, User.IsActive, !User.IsActive, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
                }
                else
                {
                    return null;
                }
            }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            if (string.IsNullOrEmpty(oldPassword))
            {
                return false;
            }
            if (string.IsNullOrEmpty(newPassword))
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
                Boolean VerificationSucceeded = (User.Password == FormsAuthentication.HashPasswordForStoringInConfigFile(oldPassword, "md5"));
                if (VerificationSucceeded)
                {
                    Console.WriteLine("Validation Success");
                }
                else
                {
                    Console.WriteLine("Validation Not Success");
                    return false;
                }
                User.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(newPassword, "md5");
                Context.SaveChanges();
                return true;
            }
        }

        public override bool UnlockUser(string userName)
        {
            using (LayoutContext Context = new LayoutContext())
            {
                SysUser User = null;
                User = Context.SysUsers.Find(userName);
                if (User != null)
                {
                    User.IsActive = true;
                    Context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override int GetNumberOfUsersOnline()
        {
            DateTime DateActive = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(Convert.ToDouble(Membership.UserIsOnlineTimeWindow)));
            using (LayoutContext Context = new LayoutContext())
            {
                return Context.SysUsers.Where(Usr => Usr.IsActive).Count();
            }
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            using (LayoutContext Context = new LayoutContext())
            {
                SysUser User = null;
                User = Context.SysUsers.Find(username);
                if (User != null)
                {
                    Context.SysUsers.Remove(User);
                    Context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override string GetUserNameByEmail(string email)
        {
            using (LayoutContext Context = new LayoutContext())
            {
                SysUser User = null;
                User = Context.SysUsers.FirstOrDefault(Usr => Usr.Email == email);
                if (User != null)
                {
                    return User.UserId;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection MembershipUsers = new MembershipUserCollection();
            using (LayoutContext Context = new LayoutContext())
            {
                totalRecords = Context.SysUsers.Where(Usr => Usr.Email == emailToMatch).Count();
                IQueryable<SysUser> Users = Context.SysUsers.Where(Usr => Usr.Email == emailToMatch).OrderBy(Usrn => Usrn.UserId).Skip(pageIndex * pageSize).Take(pageSize);
                foreach (SysUser user in Users)
                {
                    MembershipUsers.Add(new MembershipUser(Membership.Provider.Name, user.UserId, user.UserId, user.Email, null, null, user.IsActive, !user.IsActive, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now));
                }
            }
            return MembershipUsers;
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection MembershipUsers = new MembershipUserCollection();
            using (LayoutContext Context = new LayoutContext())
            {
                totalRecords = Context.SysUsers.Where(Usr => Usr.UserId == usernameToMatch).Count();
                IQueryable<SysUser> Users = Context.SysUsers.Where(Usr => Usr.UserId == usernameToMatch).OrderBy(Usrn => Usrn.UserId).Skip(pageIndex * pageSize).Take(pageSize);
                foreach (SysUser user in Users)
                {
                    MembershipUsers.Add(new MembershipUser(Membership.Provider.Name, user.UserId, user.UserId, user.Email, null, null, user.IsActive, !user.IsActive, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now));
                }
            }
            return MembershipUsers;
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection MembershipUsers = new MembershipUserCollection();
            using (LayoutContext Context = new LayoutContext())
            {
                totalRecords = Context.SysUsers.Count();
                IQueryable<SysUser> Users = Context.SysUsers.OrderBy(Usrn => Usrn.UserId).Skip(pageIndex * pageSize).Take(pageSize);
                foreach (SysUser user in Users)
                {
                    MembershipUsers.Add(new MembershipUser(Membership.Provider.Name, user.UserId, user.UserId, user.Email, null, null, user.IsActive, !user.IsActive, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now));
                }
            }
            return MembershipUsers;
        }

        #endregion

        #region Not Supported

        //CodeFirstMembershipProvider does not support password retrieval scenarios.
        public override bool EnablePasswordRetrieval
        {
            get { return false; }
        }
        public override string GetPassword(string username, string answer)
        {
            throw new NotSupportedException("Consider using methods from WebSecurity module.");
        }

        //CodeFirstMembershipProvider does not support password reset scenarios.
        public override bool EnablePasswordReset
        {
            get { return false; }
        }
        public override string ResetPassword(string username, string answer)
        {
            throw new NotSupportedException("Consider using methods from WebSecurity module.");
        }

        //CodeFirstMembershipProvider does not support question and answer scenarios.
        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotSupportedException("Consider using methods from WebSecurity module.");
        }

        //CodeFirstMembershipProvider does not support UpdateUser because this method is useless.
        public override void UpdateUser(MembershipUser user)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}