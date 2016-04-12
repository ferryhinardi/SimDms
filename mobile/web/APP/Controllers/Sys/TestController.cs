using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using eXpressAPI;
using eXpressAPI.Controllers;
using eXpressAPI.Models;
//using MvcCodeRouting.Web.Mvc;

namespace eXpressAPP.Controllers.Sys
{
    [Authorize]
    public class TestController : DefaultController
    {
        // GET: App
        public ActionResult Index()
        {
            //var listCompany = db.Database.SqlQuery<ComboList>("select companyid value, name [text] from dbo.Get_Company('" + UserId + "')").ToList();
            ViewBag.Companies = ListCompanyByRole();
            ViewBag.UserName = UserName;
            ViewBag.Company = Settings.Company;

            var user = db.Users.Find(UserId);
            if (user != null)
            {
                var company = db.CompanyProfile.Find(user.Code);
                if (company != null)
                {
                    ViewBag.Company = company.Name;
                }
            }
            return View(GenerateNavigationMenu());
        }
        
    }
}