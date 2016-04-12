﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core;
using Thinktecture.IdentityServer.Core.Services;
using Thinktecture.IdentityServer.EntityFramework;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.AspNetIdentity;


namespace eXpressAPP
{
    class Factory
    {
        public static IdentityServerServiceFactory Configure(string connString)
        {
            var efConfig = new EntityFrameworkServiceOptions
            {
                ConnectionString = connString,
                Schema = "IdSrv"
            };

            ConfigureClients(Clients.Get(), efConfig);
            ConfigureScopes(Scopes.Get(), efConfig);

            var factory = new IdentityServerServiceFactory();
            
            factory.RegisterConfigurationServices(efConfig);
            factory.RegisterOperationalServices(efConfig);

            //var userService = new Thinktecture.IdentityServer.Core.Services.InMemory.InMemoryUserService(Users.Get());
            //factory.UserService = new Registration<IUserService>(resolver => userService);

            //factory.UserService = new Registration<IUserService>(resolver => MembershipRebootUserServiceFactory.Factory(connString));
            //factory.UserService = new Registration<IUserService>(resolver => AspNetIdentityUserServiceFactory.Factory(connString));

            return factory;
        }

        public static void ConfigureClients(IEnumerable<Client> clients, EntityFrameworkServiceOptions options)
        {
            using (var db = new ClientConfigurationDbContext(options.ConnectionString, options.Schema))
            {
                if (!db.Clients.Any())
                {
                    foreach (var c in clients)
                    {
                        var e = c.ToEntity();
                        db.Clients.Add(e);
                    }
                    db.SaveChanges();
                }
            }
        }

        public static void ConfigureScopes(IEnumerable<Scope> scopes, EntityFrameworkServiceOptions options)
        {
            using (var db = new ScopeConfigurationDbContext(options.ConnectionString, options.Schema))
            {
                if (!db.Scopes.Any())
                {
                    foreach (var s in scopes)
                    {
                        var e = s.ToEntity();
                        db.Scopes.Add(e);
                    }
                    db.SaveChanges();
                }
            }
        }

    }




}
