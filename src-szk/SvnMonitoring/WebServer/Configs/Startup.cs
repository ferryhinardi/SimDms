using EventScheduler;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using Ormikon.Owin.Static;
using Owin;
using SQLWatcher;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

[assembly: OwinStartup(typeof(SVNMON.WebServer.Configs.Startup))]
namespace SVNMON.WebServer.Configs
{
    public class Startup
    {

        

        private string HomeDir
        {
            get
            {
                string Url = System.Configuration.ConfigurationManager.AppSettings["home"].ToString();
                return Url.Replace(@"{APPPATH}", Environment.CurrentDirectory);
            }
        }

        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            string contentPath = HomeDir;
            MyShared.DebugInfo("Local Home Directory: " + contentPath);

            app.UseStatic(new StaticSettings(contentPath) { DefaultFile = "index.html" })
                .MapStatic("/js", new StaticSettings(contentPath + "\\resources\\js") { Expires = DateTimeOffset.Now.AddDays(1), Cached = true, Include = "*.js" })
                .MapStatic("/css", new StaticSettings(contentPath + "\\resources\\css") { Expires = DateTimeOffset.Now.AddDays(1), Cached = true, Include = "*.css" })
                .MapStatic("/fonts", new StaticSettings(contentPath + "\\resources\\fonts") { Expires = DateTimeOffset.Now.AddDays(1), Cached = true })
                .MapStatic("/icons", new StaticSettings(contentPath + "\\resources\\icons") { Expires = DateTimeOffset.Now.AddDays(1), Cached = true })
                .MapStatic("/images", new StaticSettings(contentPath + "\\resources\\images") { Expires = DateTimeOffset.Now.AddDays(1), Cached = true })
                .UseWelcomePage("/welcome")
                .UseErrorPage();       
  
            ConfigureOAuth(app);  
            WebApiConfig.Register(config);            
            app.UseWebApi(config);
            app.Map("/signalr", map =>
            {
                map.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration
                {
                    EnableDetailedErrors = true,
                    EnableJSONP = true
                };
                map.RunSignalR(hubConfiguration);
            });



        }



        public void ConfigureOAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new SimpleAuthorizationServerProvider()
            };
            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }

    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //SMSDB db = new SMSDB();
            //Repository<SVNMON.Models.User> _repo = new Repository<SVNMON.Models.User>(db);
            //{
            //    SVNMON.Models.User user = await _repo.FindAsync(u => u.UserId == context.UserName && u.Password == context.Password);

            //    if (user == null)
            //    {
            //        context.Response.StatusCode = 200;
            //        context.Response.ContentType = "application/json";
            //        await context.Response.WriteAsync("{success:false, errorDescription:'The user name or password is incorrect.'}");
            //        return;
            //    }
            //}

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("sub", context.UserName));
            identity.AddClaim(new Claim("role", "user"));

            context.Validated(identity);

        }
    }


}
