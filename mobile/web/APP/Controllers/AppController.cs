using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using eXpressAPI;
using eXpressAPI.Controllers;
using eXpressAPI.Models;
using MongoDB.Driver.Builders;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace eXpressAPP.Controllers
{
    [Authorize]
    public class AppController : DefaultController
    {
        // GET: App
        public ActionResult Index()
        {
            //var listCompany = db.Database.SqlQuery<ComboList>("select companyid value, name [text] from dbo.Get_Company('" + UserId  + "')").ToList();
            ViewBag.Companies = ListCompanyByRole();
            ViewBag.UserName = UserName;
            ViewBag.Company = Settings.Company;
            ViewBag.Developer = User.IsInRole("Developer");
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

        public JsonResult ChangeCompany(string id)
        {
            SaveResult ret = new SaveResult(0, false);
            var user = db.Users.Find(UserId);
            if (user != null)
            {
                user.Code = id;
                ret = EntryUpdate(user, "Change company success");
            }
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Setup()
        {
            if (!db.Menus.Any())
            {
                string uri = Settings.Home + "appconfig/system/setup";
                using (HttpClient httpClient = new HttpClient())
                {
                    Task<String> response = httpClient.GetStringAsync(uri);
                    var resData = response.Result;
                }
            }
            return Redirect("/");
        }

    }
}