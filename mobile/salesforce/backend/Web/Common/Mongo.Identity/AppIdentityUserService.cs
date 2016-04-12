using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using AspNet.Identity.MongoDB;
using IdentityServer3.Core.Extensions;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services.Default;

namespace Common.Mongo.Identity
{
    public class LocalRegistrationUserService : UserServiceBase
    {

        private ApplicationUserManager _userManager;
        private ApplicationIdentityContext _appDB;

        public LocalRegistrationUserService()
        {
            _appDB = ApplicationIdentityContext.Create();
            _userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_appDB.Users));
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager;
            }
            private set
            {
                _userManager = value;
            }
        }

        public override Task  AuthenticateLocalAsync(LocalAuthenticationContext context)
        {

            var user =  UserManager.FindByNameAsync(context.UserName).Result;
            //if (user == null)
            //{
            //    return SignInStatus.Failure;
            //}
            //if (await UserManager.IsLockedOutAsync(user.Id))
            //{
            //    return SignInStatus.LockedOut;
            //}
            //if (await UserManager.CheckPasswordAsync(user, password))
            //{
            //    return await SignInOrTwoFactor(user, isPersistent);
            //}
            //if (shouldLockout)
            //{
            //    // If lockout is requested, increment access failed count which might lock out the user
            //    await UserManager.AccessFailedAsync(user.Id);
            //    if (await UserManager.IsLockedOutAsync(user.Id))
            //    {
            //        return SignInStatus.LockedOut;
            //    }
            //}
            //return SignInStatus.Failure;

            if (user != null)
            {
                context.AuthenticateResult = new AuthenticateResult(user.Id, user.UserName);
            }

            return Task.FromResult(0);
        }

        public override Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            // issue the claims for the user
            //var user = Users.SingleOrDefault(x => x.Subject == context.Subject.GetSubjectId());
            //if (user != null)
            //{
            //    context.IssuedClaims = user.Claims.Where(x => context.RequestedClaimTypes.Contains(x.Type));
            //}

            return Task.FromResult(0);
        } 

    }
}
