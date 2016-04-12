using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using IdentityModel.Client;
using IdentityServer3.Core;
using IdentityServer3.Core.Configuration;
using System.Linq;
using System.Web.Helpers;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Facebook;
using eXpressAPI.Models;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Common;
using IdentityManager.Configuration;
using Microsoft.Owin.Infrastructure;
using IdentityServer3.Core.Services.Default;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using AuthenticationOptions = IdentityServer3.Core.Configuration.AuthenticationOptions;
using IdentityAdmin.Configuration;
using Hangfire;

[assembly: Microsoft.Owin.OwinStartup(typeof(eXpressAPP.Startup))]

namespace eXpressAPP
{
    public partial class Startup
    {
        private void ConfigureIdentityProviders(IAppBuilder app, string signInAsType)
        {
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                AuthenticationType = "Google",
                Caption = "btn btn-warning btn-circle~fa fa-google",
                SignInAsAuthenticationType = signInAsType,
                ClientId = Settings.GoogleClientId,
                ClientSecret = Settings.GoogleClientKey
            });

            app.UseFacebookAuthentication(new FacebookAuthenticationOptions
            {
                AuthenticationType = "Facebook",
                Caption = "btn btn-primary btn-circle~fa fa-facebook",
                SignInAsAuthenticationType = signInAsType,
                AppId = Settings.FacebookClientId,
                AppSecret = Settings.FacebookClientKey
            });
        }

        public void Configuration(IAppBuilder app)
        {
            //LogProvider.SetCurrentLogProvider(new DiagnosticsTraceLogProvider());
            AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.ClaimTypes.Subject;
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            var strApp = HttpContext.Current.Request.Url;

            app.Map("/idsrvadm", adminApp =>
            {
                var factory = new IdentityAdminServiceFactory();
                factory.Configure();
                adminApp.UseIdentityAdmin(new IdentityAdminOptions
                {
                    Factory = factory,
                    AdminSecurityConfiguration =
                    {
                        RequireSsl = false
                    }
                });
            });

            app.Map("/admin", adminApp =>
            {
                var factory = new IdentityManagerServiceFactory();
                factory.ConfigureSimpleIdentityManagerService(Settings.Connection);

                adminApp.UseIdentityManager(new IdentityManagerOptions
                {
                    Factory = factory,
                    DisableUserInterface = Convert.ToBoolean(Settings.DisabledUI),
                    SecurityConfiguration =
                    {
                        RequireSsl = false
                    }
                });
            });

            //GlobalConfiguration.Configuration.UseSqlServerStorage("IdentityDB");

            //app.UseHangfireServer();
            //app.UseHangfireDashboard();

            var idSvrFactory = Common.Factory.Configure(Settings.Connection);
            idSvrFactory.ConfigureDefaultViewService(new DefaultViewServiceOptions()
            {
                CacheViews = false
            });

            app.Map("/core", idsrvApp =>
            {
                idsrvApp.UseIdentityServer(new IdentityServerOptions
                {
                    SiteName = Settings.AppIdentity,
                    SigningCertificate = Certificate.Get(),
                    //EnableWelcomePage = false,
                    Factory = idSvrFactory,
                    RequireSsl = Convert.ToBoolean(Settings.UseSSL),
                    AuthenticationOptions = new AuthenticationOptions
                    {
                        IdentityProviders = ConfigureIdentityProviders,
                        EnablePostSignOutAutoRedirect = true,
                    },
                    CspOptions = new CspOptions()
                    {
                        ScriptSrc = "'unsafe-eval'"
                    }
                });
            });

            app.UseResourceAuthorization(new AuthorizationManager());
            
            app.UseKentorOwinCookieSaver();

            app.UseCookieAuthentication(new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies",
                CookieName = CookieAuthenticationDefaults.CookiePrefix + Settings.Project,
                ExpireTimeSpan = TimeSpan.FromHours(2),
                CookieManager = new SystemWebCookieManager()
            });

            var openid_options = new OpenIdConnectAuthenticationOptions
            {
                Authority = Settings.IssueUrl,
                RedirectUri = Settings.Home,
                PostLogoutRedirectUri = Settings.Home,

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

                        if (userInfo.Claims != null)
                        {
                            userInfo.Claims.ToList().ForEach(ui =>
                            {
                                if (ui.Item1 == "sub")
                                {
                                    subId = ui.Item2;
                                }
                                nid.AddClaim(new Claim(ui.Item1, ui.Item2));

                            });
                        }

                        // Custom User Info from User database
                        DataAccessContext db = new DataAccessContext();
                        var userinfo = db.Users.FirstOrDefault(x => x.Pass == subId);

                        if (userinfo != null)
                        {
                            nid.AddClaim(new Claim("user_id", userinfo.UserId));
                            nid.AddClaim(new Claim("user_code", userinfo.Code));
                            nid.AddClaim(new Claim("user_name", userinfo.Name));
                            nid.AddClaim(new Claim("user_role", userinfo.RoleId ?? ""));
                            nid.AddClaim(new Claim("sales_code", userinfo.Contact ?? ""));

                            var myRole = db.Roles.Find(userinfo.RoleId);

                            nid.AddClaim(myRole != null
                                ? new Claim("user_sales", myRole.IsSalary.ToString())
                                : new Claim("user_sales", "1"));

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
                            nid.AddClaim(new Claim("sales_code", ""));
                            nid.AddClaim(new Claim("user_sales", "1"));
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
    }

    public class SystemWebCookieManager : ICookieManager
    {
        public string GetRequestCookie(IOwinContext context, string key)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var webContext = context.Get<HttpContextBase>(typeof (HttpContextBase).FullName);
            var cookie = webContext.Request.Cookies[key];
            return cookie == null ? null : cookie.Value;
        }

        public void AppendResponseCookie(IOwinContext context, string key, string value,
            Microsoft.Owin.CookieOptions options)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            var webContext = context.Get<HttpContextBase>(typeof (HttpContextBase).FullName);

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

    public class ErrorHandlingControllerFactory : DefaultControllerFactory
    {
        public override IController CreateController(
            RequestContext requestContext,
            string controllerName)
        {

            if (requestContext == null)
            {
                return null;
            }

            try
            {
                var controller =
                    base.CreateController(requestContext,
                        controllerName);

                var c = controller as Controller;

                if (c != null)
                {
                    c.ActionInvoker =
                        new ErrorHandlingActionInvoker(
                            new HandleErrorWithELMAHAttribute());
                }

                return controller;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}