using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services.Default;

namespace AuthSvr.Auth
{
    public class ActiveDirectoryUserService : UserServiceBase
    {
        private const string DOMAIN = "suzuki.co.id";

        public override Task AuthenticateExternalAsync(ExternalAuthenticationContext context)
        {
            return Task.FromResult<AuthenticateResult>(null);
        }

        public override Task AuthenticateLocalAsync(LocalAuthenticationContext context)
        {
            string username = context.UserName;
            string password = context.Password;

            try
            {
                using (var pc = new PrincipalContext(ContextType.Domain, DOMAIN))
                {
                    var dse = new DirectoryEntry("LDAP://suzuki.co.id/rootDSE", username, password);
                    try
                    {
                        var name = dse.Name;
                        if (dse.Name.Length > 0)
                        {
                            
                            context.AuthenticateResult = new AuthenticateResult(subject: dse.Guid.ToString(), name: username);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
            
                    // if (pc.ValidateCredentials(username, password))
                    // {
                    //     using (var user = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, username))
                    //     {
                    //         if (user != null)
                    //         {
                    //             context.AuthenticateResult = new AuthenticateResult(subject: user.Guid.ToString(), name: username);
                    //         }
                    //     }
                    // }
                }
            }
            catch
            {

            }

            return Task.FromResult(0);
        }

        public override Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            List<Claim> _claims = new List<Claim>();

            Claim _subject = context.Subject.Claims.FirstOrDefault();

            if (_subject != null)
            {
                string username = GetUserIDFromSubject(_subject);

                _claims = this.BuildProfileForUserName(username);

                //Filter out the claims that weren't requested
                if (context.RequestedClaimTypes != null)
                {
                    _claims = _claims.Where(c => context.RequestedClaimTypes.Contains(c.Type)).ToList();
                }
            }

            context.IssuedClaims = _claims.AsEnumerable();

            return Task.FromResult(0);
        }

        private string GetUserIDFromSubject(Claim subject)
        {
            if (subject != null)
            {
                string _ldapPath = String.Format("LDAP://<GUID={0}>", subject.Value);
                var user = new DirectoryEntry(_ldapPath);

                if (user != null)
                {
                    return user.Properties["SamAccountName"].Value.ToString();
                }
            }

            return null;
        }

        public List<GroupPrincipal> GetGroups(string userName)
        {
            List<GroupPrincipal> result = new List<GroupPrincipal>();
            
            // establish domain context
            PrincipalContext yourDomain = new PrincipalContext(ContextType.Domain);
            
            // find your user
            UserPrincipal user = UserPrincipal.FindByIdentity(yourDomain, userName);
            
            // if found - grab its groups
            if(user != null)
            {
                PrincipalSearchResult<Principal> groups = user.GetAuthorizationGroups();
            
                // iterate over all groups
                foreach(Principal p in groups)
                {
                    // make sure to add only group principals
                    if(p is GroupPrincipal)
                    {
                        result.Add((GroupPrincipal)p);
                    }
                }
            }
            
            return result;
        }
        private List<Claim> BuildProfileForUserName(string username)
        {
            List<Claim> _claims = new List<Claim>();

            if (!String.IsNullOrEmpty(username))
            {
                using (var pc = new PrincipalContext(ContextType.Domain, DOMAIN))
                {

                    using (var user = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, username))
                    {
                        if (user != null)
                        {

                            _claims.Add(new Claim(Constants.ClaimTypes.Subject, user.Guid.ToString()));
                            _claims.Add(new Claim(Constants.ClaimTypes.GivenName, user.GivenName));
                            _claims.Add(new Claim(Constants.ClaimTypes.FamilyName, user.Surname));
                            _claims.Add(new Claim(Constants.ClaimTypes.Email, user.EmailAddress));
                            _claims.Add(new Claim(Constants.ClaimTypes.IdentityProvider, "ActiveDirectory"));

                            foreach (string role in GetRolesForUser(username))
                            {
                                _claims.Add(new Claim(Constants.ClaimTypes.Role, role));
                            }
                        }
                    }
                }
            }

            return _claims;
        }

        public override Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.FromResult(0);
        }

        public override Task PostAuthenticateAsync(PostAuthenticationContext context)
        {
            return Task.FromResult(0);
        }

        public override Task PreAuthenticateAsync(PreAuthenticationContext context)
        {
            return Task.FromResult(0);
        }

        public override Task SignOutAsync(SignOutContext context)
        {
            return Task.FromResult(0);
        }

        private List<string> GetRolesForUser(string username)
        {
            List<string> result = new List<string>();

            // establish domain context
            PrincipalContext _curDomain = new PrincipalContext(ContextType.Domain, DOMAIN);

            // find your user
            UserPrincipal user = UserPrincipal.FindByIdentity(_curDomain, username);

            // if found - grab its groups
            if (user != null)
            {
                PrincipalSearchResult<Principal> groups = user.GetAuthorizationGroups();

                // iterate over all groups
                foreach (Principal p in groups)
                {
                    // make sure to add only group principals
                    if (p is GroupPrincipal)
                    {
                        result.Add(((GroupPrincipal)p).Name);
                    }
                }
            }

            return result;
        }
    }
}