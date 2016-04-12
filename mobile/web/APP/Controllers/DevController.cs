using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eXpressAPI;
using eXpressAPI.Controllers;
using eXpressAPI.Models;

namespace eXpressAPP.Controllers
{
    [Authorize]
    public class DevController : DefaultController
    {

        // GET: Dev
        public ActionResult Index()
        {
           // var listCompany = db.Database.SqlQuery<ComboList>("select companyid value, name [text] from dbo.Get_Company('" + UserId + "')").ToList();
            ViewBag.Companies = ListCompanyByRole();
            ViewBag.UserName = UserName;
            ViewBag.Developer = true;
            return View(GenerateNavigationMenu(true));
        }



    }
}