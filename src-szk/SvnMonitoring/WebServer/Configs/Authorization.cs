using JSMSCentreSvc.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JSMSCentreSvc.WebServer.Configs
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            SMSDB db = new SMSDB();
            Repository<JSMSCentreSvc.Models.User> _repo = new Repository<JSMSCentreSvc.Models.User>(db);
            {
                JSMSCentreSvc.Models.User user = await _repo.FindAsync(u => u.UserId == context.UserName && u.Password == context.Password);

                if (user == null)
                {
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{success:false, error_description:'The user name or password is incorrect.'}");
                    return;
                }
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("sub", context.UserName));
            identity.AddClaim(new Claim("role", "user"));

            context.Validated(identity);

        }
    }
}
