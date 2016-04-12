using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AuthSvr.Auth;
using IdentityServer3.Admin.MongoDb;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;
using IdentityServer3.Core.Services.InMemory;
using IdentityServer3.MongoDb;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AuthSvr
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIISPlatformHandler();

            app.UseWelcomePage("/welcome");

            app.Map("/core", core =>
            {

                //var settings = StoreSettings.DefaultSettings();
                
                var settings = new StoreSettings
                {
                    ConnectionString = "mongodb://localhost",
                    Database = "identityserver3",
                    ClientCollection = "clients",
                    ScopeCollection = "scopes",
                    ConsentCollection = "consents",
                    AuthorizationCodeCollection = "authorizationCodes",
                    RefreshTokenCollection = "refreshtokens",
                    TokenHandleCollection = "tokenhandles"
                };


                SetupDatabase(settings).Wait();

                var factory = new IdentityServer3.MongoDb.ServiceFactory(
                        new Registration<IUserService>(new InMemoryUserService(Users.Get())),
                        settings
                    );

                factory.ConfigureDefaultViewService(new DefaultViewServiceOptions()
                {
                    CacheViews = false
                });

                //factory.ViewService = new Registration<IViewService>(typeof(CustomViewService));
                factory.UserService = new Registration<IUserService, ActiveDirectoryUserService>();
                factory.CorsPolicyService = new Registration<ICorsPolicyService>(new DefaultCorsPolicyService { AllowAll = true });

                var idsrvOptions = new IdentityServerOptions
                {

                    //Factory = new IdentityServerServiceFactory()
                    //                .UseInMemoryUsers(Users.Get())
                    //                .UseInMemoryClients(Clients.Get())
                    //                .UseInMemoryScopes(Scopes.Get()),

                    Factory = factory,


                    // SigningCertificate = new X509Certificate2(certFile, "idsrv3test"),
                    AuthenticationOptions = new AuthenticationOptions
                    {
                        EnablePostSignOutAutoRedirect = true
                    },

                    RequireSsl = false,

                    CspOptions = new CspOptions()
                    {
                        ScriptSrc = "'unsafe-eval'"
                    }
                };

                

                core.UseIdentityServer(idsrvOptions);
            });

            app.UseStaticFiles();


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                 
            });
        }


        async Task SetupDatabase(StoreSettings settings)
        {
            //This setup script should really be run as a job during deployment and is
            //only here to illustrate how the database can be setup from code
            var adminService = AdminServiceFactory.Create(settings);
            await adminService.CreateDatabase();
            foreach (var client in Clients.Get())
            {
                await adminService.Save(client);
            }

            foreach (var scope in Scopes.Get())
            {
                await adminService.Save(scope);
            }
        }


        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
