using Owin;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Thinktecture.IdentityModel.Client;
using Thinktecture.IdentityServer.Core;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Logging;
using System.Linq;
using Thinktecture.IdentityServer.Core.Configuration.Hosting;
using System.Web.Helpers;
using Thinktecture.IdentityManager.Configuration;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Facebook;
using eXpressAPI.Models;
using eXpressAPI;
using System.Web.Security;
using System.Web;
using Microsoft.Owin.Infrastructure;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.Owin.Security.OAuth;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.DirectoryServices;

[assembly: OwinStartupAttribute(typeof(eXpressAPP.Startup))]
namespace eXpressAPP
{

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            LogProvider.SetCurrentLogProvider(new DiagnosticsTraceLogProvider());
            AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.ClaimTypes.Subject;
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();
            
            app.Map("/admin", adminApp =>
            {
                var factory = new IdentityManagerServiceFactory();
                factory.ConfigureSimpleIdentityManagerService(Settings.Connection);
                adminApp.UseIdentityManager(new IdentityManagerOptions
                {
                    Factory = factory,
                    DisableUserInterface = Convert.ToBoolean(Settings.DisabledUI)
                });
            });

            var idSvrFactory = Factory.Configure(Settings.Connection);
            idSvrFactory.ConfigureUserService(Settings.Connection);

            app.Map("/core", idsrvApp =>
            {
                idsrvApp.UseIdentityServer(new IdentityServerOptions
                {
                    SiteName = Settings.AppIdentity,
                    SigningCertificate = Certificate.LoadCertificate(Settings.CertFile, Settings.CertPwd),
                    Factory = idSvrFactory,
                    CorsPolicy = CorsPolicy.AllowAll,
                    RequireSsl = Convert.ToBoolean(Settings.UseSSL),
                    AuthenticationOptions = new Thinktecture.IdentityServer.Core.Configuration.AuthenticationOptions
                    {
                        IdentityProviders = ConfigureIdentityProviders
                    }
                });
            });

            app.UseResourceAuthorization(new AuthorizationManager());

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies",
                CookieManager = new SystemWebCookieManager()                  
            });

            app.UseKentorOwinCookieSaver();

            var openid_options = new OpenIdConnectAuthenticationOptions
            {
                Authority = Settings.IssueUrl,
                RedirectUri = Settings.Home,

                ClientId = Settings.Project,
                Scope = "openid profile roles api",
                ResponseType = "id_token token",

                SignInAsAuthenticationType = "Cookies",
                UseTokenLifetime = false,

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = async n =>
                    {
                        var nid = new ClaimsIdentity(
                            n.AuthenticationTicket.Identity.AuthenticationType,
                            Constants.ClaimTypes.GivenName,
                            Constants.ClaimTypes.Role);

                        // get userinfo data
                        var userInfoClient = new UserInfoClient(
                            new Uri(n.Options.Authority + "/connect/userinfo"),
                            n.ProtocolMessage.AccessToken);

                        string subId = "";


                        var userInfo = await userInfoClient.GetAsync();
                        userInfo.Claims.ToList().ForEach(ui =>
                        {
                            if (ui.Item1 == "sub")
                            {
                                subId = ui.Item2;
                            }
                            nid.AddClaim(new Claim(ui.Item1, ui.Item2));

                        });

                        // Custom User Info from User database
                        DataAccessContext db = new DataAccessContext();
                        var userinfo = db.Users.Where(x => x.Pass == subId).FirstOrDefault();
                        if (userinfo != null)
                        {
                            nid.AddClaim(new Claim("user_id", userinfo.UserId));
                            nid.AddClaim(new Claim("user_code", userinfo.Code));
                            nid.AddClaim(new Claim("user_name", userinfo.Name));
                            nid.AddClaim(new Claim("user_role", userinfo.RoleId ?? ""));
                            nid.AddClaim(new Claim("role", userinfo.RoleId ?? ""));
                            userinfo.LoginDate = DateTime.Now;
                            userinfo.IsLogin = 1;
                            db.Entry(userinfo).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            nid.AddClaim(new Claim("user_id", n.ProtocolMessage.UserId ?? ""));
                            nid.AddClaim(new Claim("user_code", n.ProtocolMessage.Username ?? ""));
                            nid.AddClaim(new Claim("user_name", n.ProtocolMessage.Username ?? ""));
                            nid.AddClaim(new Claim("user_role", ""));
                        }

                        // keep the id_token for logout
                        nid.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

                        // add access token for sample API
                        nid.AddClaim(new Claim("access_token", n.ProtocolMessage.AccessToken));

                        // keep track of access token expiration
                        nid.AddClaim(new Claim("expires_at", DateTimeOffset.Now.AddHours(2).ToString()));

                        n.AuthenticationTicket = new AuthenticationTicket(
                            nid,
                            n.AuthenticationTicket.Properties);
                    },

                    RedirectToIdentityProvider = async n =>
                    {
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                        {
                            var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

                            if (idTokenHint != null)
                            {
                                n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                            }
                        }
                    }
                }
            };

            app.UseOpenIdConnectAuthentication(openid_options);
            
        }
        
        private void ConfigureIdentityProviders(IAppBuilder app, string signInAsType)
        {
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                AuthenticationType = "Google",
                Caption = "Sign-in with Google",
                SignInAsAuthenticationType = signInAsType,
                ClientId = Settings.GoogleClientId,
                ClientSecret = Settings.GoogleClientKey
            });

            app.UseFacebookAuthentication(new FacebookAuthenticationOptions
            {
                AuthenticationType = "Facebook",
                SignInAsAuthenticationType = signInAsType,
                AppId = Settings.FacebookClientId,
                AppSecret = Settings.FacebookClientKey
            });
        }    
        
        public void ConfigureOAuth(IAppBuilder app)
        {
            app.UseExternalSignInCookie(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalCookie);
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,                
                TokenEndpointPath = new PathString("/app/gettoken_app"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new SimpleAuthorizationServerProvider()
            };
            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
        
        public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
        {
            public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
            {
                context.Validated();
            }

            public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
            {
                var accessDenied = true;
                Boolean VerificationSucceeded = false;
                var jss = new JavaScriptSerializer();

                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                identity.AddClaim(new Claim("user_name", context.UserName));
                identity.AddClaim(new Claim("user_id", context.UserName));

                string cn_Str = System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDB"].ToString();
                UserManager u_man = new UserManager(new UserStore(new Context(cn_Str)));

                var userLogin = await u_man.FindAsync(context.UserName, context.Password);

                if (userLogin != null)
                {
                    VerificationSucceeded = true;
                    accessDenied = false;
                    identity.AddClaim(new Claim("sub", userLogin.Id ));
                    IList<Claim> rolesForUser = await u_man.GetClaimsAsync(userLogin.Id);
                    foreach(var item in rolesForUser){
                        identity.AddClaim(item);                    
                    }

                    using (var db = new DataAccessContext())
                    {
                        var userinfo = db.Users.Where(x => x.Pass == userLogin.Id).FirstOrDefault();
                        if (userinfo != null)
                        {
                            identity.AddClaim(new Claim("user_role", userinfo.RoleId));
                            identity.AddClaim(new Claim("role", userinfo.RoleId));

                            userinfo.LoginDate = DateTime.Now;
                            userinfo.IsLogin = 1;
                            db.Entry(userinfo).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }                          
                    }
                }
                
                if (!VerificationSucceeded)
                {
                    if (Settings.LDAP == "true" && !string.IsNullOrEmpty(context.UserName) && !string.IsNullOrEmpty(context.Password))
                    {
                        var dse = new DirectoryEntry(@"LDAP://" + Settings.Domain + @"/rootDSE", context.UserName, context.Password);
                        try
                        {
                            var name = dse.Name;
                            if (dse.Username.ToLower() == context.UserName.ToLower() && name.Length > 0)
                            {
                                accessDenied = false;
                                using (var db = new DataAccessContext())
                                {
                                    var userinfo = db.Users.Where(x => x.UserId == context.UserName).FirstOrDefault();
                                    if (userinfo != null)
                                    {
                                        identity.AddClaim(new Claim("user_role", userinfo.RoleId));
                                        identity.AddClaim(new Claim("role", userinfo.RoleId));
                                        userinfo.LoginDate = DateTime.Now;
                                        userinfo.IsLogin = 1;
                                        db.Entry(userinfo).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }

                                var UserInfo = await u_man.FindByNameAsync(context.UserName);
                                if ( UserInfo != null)
                                {
                                    identity.AddClaim(new Claim("sub", UserInfo.Id));
                                    IList<Claim> rolesForUser = await u_man.GetClaimsAsync(UserInfo.Id);
                                    foreach (var item in rolesForUser)
                                    {
                                        identity.AddClaim(item);
                                    }
                                }                             
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }

                if (accessDenied)
                {
                    var response = context.Response;
                    response.ContentType = "application/json";
                    response.StatusCode = 200;
                    response.Write("ERROR" + (new string(' ', 64)).ToString());
                    return;                   
                }
                else
                {
                    var header = context.OwinContext.Response.Headers.SingleOrDefault(h => h.Key == "Access-Control-Allow-Origin");
                    if (header.Equals(default(KeyValuePair<string, string[]>)))
                    {
                        context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
                    }
                    context.OwinContext.Authentication.SignOut(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalCookie);
                    context.Validated(identity);                     
                    context.OwinContext.Authentication.SignIn(identity);
                }
            }
        }
    }

    public class LogOnModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string grant_type { get; set; }
        public bool RememberMe { get; set; }
    }

    class LoginResult
    {
        public string role { get; set; }
        public bool success { get; set; }
    }


    class TokenResult
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
    }


    public class SystemWebCookieManager : ICookieManager
    {
        public string GetRequestCookie(IOwinContext context, string key)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var webContext = context.Get<HttpContextBase>(typeof(HttpContextBase).FullName);
            var cookie = webContext.Request.Cookies[key];
            return cookie == null ? null : cookie.Value;
        }

        public void AppendResponseCookie(IOwinContext context, string key, string value, Microsoft.Owin.CookieOptions options)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            var webContext = context.Get<HttpContextBase>(typeof(HttpContextBase).FullName);

            bool domainHasValue = !string.IsNullOrEmpty(options.Domain);
            bool pathHasValue = !string.IsNullOrEmpty(options.Path);
            bool expiresHasValue = options.Expires.HasValue;

            var cookie = new HttpCookie(key, value);
            if (domainHasValue)
            {
                cookie.Domain = options.Domain;
            }
            if (pathHasValue)
            {
                cookie.Path = options.Path;
            }
            if (expiresHasValue)
            {
                cookie.Expires = options.Expires.Value;
            }
            if (options.Secure)
            {
                cookie.Secure = true;
            }
            if (options.HttpOnly)
            {
                cookie.HttpOnly = true;
            }

            webContext.Response.AppendCookie(cookie);
        }

        public void DeleteCookie(IOwinContext context, string key, Microsoft.Owin.CookieOptions options)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            AppendResponseCookie(
                context,
                key,
                string.Empty,
                new Microsoft.Owin.CookieOptions
                {
                    Path = options.Path,
                    Domain = options.Domain,
                    Expires = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                });
        }
    }

    public class MyRestrictiveAuthorizationFilter : IAuthorizationFilter
    {
        public bool Authorize(IDictionary<string, object> owinEnvironment)
        {
            var context = new OwinContext(owinEnvironment);

            if (!context.Authentication.User.Identity.IsAuthenticated)
            {
                return false;
            }

            var myclaims = (context.Authentication.User as ClaimsPrincipal).Claims;
            bool allow = false;

            foreach (var item in myclaims)
            {
                if (item.Type == "role" && item.Value.ToLower() == Settings.JobViewer)
                {
                    allow = true;
                    break;
                }
            }
            return allow;
        }
    }
}
