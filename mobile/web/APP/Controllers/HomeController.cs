using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Thinktecture.IdentityModel.Mvc;
using eXpressAPI;
using eXpressAPI.Controllers;
using eXpressAPI.Models;

namespace eXpressAPP.Controllers
{
    [Authorize]
    public class HomeController : DefaultController
    {
        public ActionResult Index()
        {
            //var listCompany = db.Database.SqlQuery<ComboList>("select companyid value, name [text] from dbo.Get_Company('" + UserId + "')").ToList();
            // auto setup if user not found
            try
            {
                if (!db.Users.Any())
                {
                    HttpContext.Response.Redirect("appconfig/system/setup");
                }
                else
                {
                    ViewBag.Companies = ListCompanyByRole();
                    ViewBag.UserName = UserName;
                    ViewBag.Company = Settings.Company;
                    ViewBag.Developer = false;
                    ViewBag.Admin = User.IsInRole("Administrator");

                    var user = db.Users.Find(UserId);
                    if (user != null)
                    {
                        var company = db.CompanyProfile.Where(x => x.Code == user.Code).FirstOrDefault();
                        if (company != null)
                        {
                            ViewBag.Company = company.Name;
                        }
                    }

                    return View(GenerateNavigationMenu());
                }
            } catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }

            return View();
        }

        public ActionResult About()
        {
            throw new Exception("A test exception for ELMAH dff dfd");

            return View((User as ClaimsPrincipal).Claims);
        }

        [HandleForbidden]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult Logout()
        {
            UserLogout();
            Request.GetOwinContext().Authentication.SignOut();
            return Redirect("/");
        }
    }
}