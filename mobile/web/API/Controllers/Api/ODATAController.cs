using Breeze.WebApi2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using eXpressAPI.Models;
using Breeze.ContextProvider.EF6;

namespace eXpressAPI.Controllers.Api
{
    [BreezeController]
    public class ODATAController : ApiController
    {
        readonly EFContextProvider<DataAccessContext> ctx = new EFContextProvider<DataAccessContext>();

        [HttpGet]
        public String Metadata()
        {
            return ctx.Metadata();
        }

        [HttpGet]
        public CompanyProfile CurrentCompany()
        {
            var user = ctx.Context.CompanyProfile.FirstOrDefault();
            return user;
        }

        [HttpGet]
        public IQueryable<Menus> ListMenus()
        {
            return ctx.Context.Menus;
        }
        
    }
}
