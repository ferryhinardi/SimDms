﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thinktecture.IdentityManager;
using Thinktecture.IdentityManager.AspNetIdentity;
using Thinktecture.IdentityManager.Configuration;

namespace eXpressAPP
{


    public static class SimpleIdentityManagerServiceExtensions
    {
        public static void ConfigureSimpleIdentityManagerService(this IdentityManagerServiceFactory factory, string connectionString)
        {
            factory.Register(new Registration<Context>(resolver => new Context(connectionString)));
            factory.Register(new Registration<UserStore>());
            factory.Register(new Registration<RoleStore>());
            factory.Register(new Registration<UserManager>());
            factory.Register(new Registration<RoleManager>());
            factory.IdentityManagerService = new Registration<IIdentityManagerService, SimpleIdentityManagerService>();
        }
    }

    public class SimpleIdentityManagerService : AspNetIdentityManagerService<User, string, Role, string>
    {
        public SimpleIdentityManagerService(UserManager userMgr, RoleManager roleMgr)
            : base(userMgr, roleMgr)
        {
        }
    }

}