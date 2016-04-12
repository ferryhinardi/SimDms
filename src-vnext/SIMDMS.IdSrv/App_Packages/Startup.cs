/*
 * Copyright 2014 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using WebHost.IdSvr;
using IdentityManager.Configuration;
using IdentityServer3.Core.Configuration;
using WebHost.IdMgr;

using Serilog;
using IdentityManager.Logging;
using IdentityManager.Core.Logging;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin;
using System;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Security.Claims;
using WebHost.AspId;
using System.Collections.Generic;
using System.DirectoryServices;
using Owin;
using System.Web.Cors;

namespace SIMDMS.IdSrv
{
    internal class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            LogProvider.SetCurrentLogProvider(new DiagnosticsTraceLogProvider());
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.Trace()
               .CreateLogger();

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
 
            app.Map("/admin", adminApp =>
            {
                var factory = new IdentityManagerServiceFactory();
                factory.ConfigureSimpleIdentityManagerService("AspId");

                adminApp.UseIdentityManager(new IdentityManagerOptions()
                {
                    Factory = factory ,
                    SecurityConfiguration =
                    {
                        RequireSsl = false 
                    }
                });
            });

            app.Map("/core", core =>
            {
                var idSvrFactory = Factory.Configure();
                idSvrFactory.ConfigureUserService("AspId");
                  
                var options = new IdentityServerOptions
                {
                    SiteName = "IdentityServer3 - AspNetIdentity 2FA",
                    SigningCertificate = Certificate.Get(),
                    Factory = idSvrFactory,                     
                    RequireSsl = false,
                    
                };

                core.UseIdentityServer(options);

            });

            
            ConfigureOAuth(app);
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

                UserManager u_man = new UserManager(new UserStore(new Context("AspId")));

                var userLogin = await u_man.FindAsync(context.UserName, context.Password);

                if (userLogin != null)
                {
                    VerificationSucceeded = true;
                    accessDenied = false;
                    identity.AddClaim(new Claim("sub", userLogin.Id));
                    IList<Claim> rolesForUser = await u_man.GetClaimsAsync(userLogin.Id);
                    foreach (var item in rolesForUser)
                    {
                        identity.AddClaim(item);
                    }
                }

                if (!VerificationSucceeded)
                {
                    var UseDomain = System.Configuration.ConfigurationManager.AppSettings["UseDomain"].ToString();
                    var domain = System.Configuration.ConfigurationManager.AppSettings["Domain"].ToString();

                    if (UseDomain == "true" && !string.IsNullOrEmpty(context.UserName) && !string.IsNullOrEmpty(context.Password))
                    {
                        var dse = new DirectoryEntry(@"LDAP://" + domain + @"/rootDSE", context.UserName, context.Password);
                        try
                        {
                            var name = dse.Name;
                            if (dse.Username.ToLower() == context.UserName.ToLower() && name.Length > 0)
                            {
                                accessDenied = false;
                                //using (var db = new DataAccessContext())
                                //{
                                //    var userinfo = db.Users.Where(x => x.UserId == context.UserName).FirstOrDefault();
                                //    if (userinfo != null)
                                //    {
                                //        identity.AddClaim(new Claim("user_role", userinfo.RoleId));
                                //        identity.AddClaim(new Claim("role", userinfo.RoleId));
                                //        userinfo.LoginDate = DateTime.Now;
                                //        userinfo.IsLogin = 1;
                                //        db.Entry(userinfo).State = System.Data.Entity.EntityState.Modified;
                                //        db.SaveChanges();
                                //    }
                                //}

                                var UserInfo = await u_man.FindByNameAsync(context.UserName);
                                if (UserInfo != null)
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

                    var header = context.OwinContext.Response.Headers.ContainsKey("Access-Control-Allow-Origin");

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
}