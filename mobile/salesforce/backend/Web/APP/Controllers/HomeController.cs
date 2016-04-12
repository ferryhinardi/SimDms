using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Common.Helper;
using eXpressAPI.Controllers;
using eXpressAPI.Models;
using Thinktecture.IdentityModel.Mvc;

namespace eXpressAPP.Controllers
{
    [Authorize]
    public class HomeController : DefaultController
    {
        private CodeEditorRepository myCfg = new CodeEditorRepository();

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
                    ViewBag.IsDeveloper = User.IsInRole("Developer");
                    ViewBag.IsSuperuser = User.IsInRole("Superuser");

                    ViewBag.DevelopmentMode = Request.Url.ToString().ToLower().Contains("localhost:");
                    ViewBag.BaseAddress = Request.Url.ToString().ToLower();

                    var user = db.Users.Find(UserId);
                    if (user != null)
                    {
                        var company = db.CompanyProfile.FirstOrDefault(x => x.Code == user.Code);
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

        [Route("dev")]
        [Auth(Roles = "Developer")]
        [HandleForbidden]
        public ActionResult Dev()
        {
            try
            {
                ViewBag.Companies = ListCompanyByRole();
                ViewBag.UserName = UserName;
                ViewBag.Company = Settings.Company;
                ViewBag.Developer = true;
                ViewBag.Code = true;
                ViewBag.Admin = User.IsInRole("Administrator");
                ViewBag.IsDeveloper = User.IsInRole("Developer");
                ViewBag.IsSuperuser = User.IsInRole("Superuser");

                var user = db.Users.Find(UserId);
                if (user != null)
                {
                    var company = db.CompanyProfile.FirstOrDefault(x => x.Code == user.Code);
                    if (company != null)
                    {
                        ViewBag.Company = company.Name;
                    }
                }

                return View(GenerateNavigationMenu(true));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }

            return View();
        }

        [Route("myreports")]
        public ActionResult Reports(string id)
        {
            var report = db.ReportSessions.Find(id);

            if (report != null)
            {
                ViewBag.reportid = report.ReportId;
                ViewBag.Parameters = report.Parameters;
            }

            return View();
        }

        [Route("app/changecompany/{id}")]
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
        [Route("install")]
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




        public ActionResult Logout()
        {
            UserLogout();
            Request.GetOwinContext().Authentication.SignOut();
            return Redirect("/");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("setmyaddress")]
        public ActionResult SetupMyURL(string id)
        {
            var cfg = new CodeEditorRepository();
            if (id == cfg.GetSetting("DefaultPassword"))
            {
                var url = Request.Url.ToString().ToLower();
                var iPos = url.LastIndexOf('/');
                var item1 = new KeyValues();
                item1.key = "Home";
                item1.value = url.Substring(0, iPos+1);
                cfg.AddOrUpdateConfig(item1);

                item1.key = "IssueUrl";
                item1.value = url.Substring(0, iPos+1) + "core";
                cfg.AddOrUpdateConfig(item1);
            }

            return Redirect("/");
        }
    }


    [RoutePrefix("error")]
    public sealed class ErrorController : Controller
    {
        #region Public Methods

        /// <summary>
        /// Returns a HTTP 400 Bad Request error view. Returns a partial view if the request is an AJAX call.
        /// </summary>
        /// <returns>The partial or full bad request view.</returns>
        [Route("badrequest", Name = ErrorControllerRoute.GetBadRequest)]
        public ActionResult BadRequest()
        {
            return this.GetErrorView(HttpStatusCode.BadRequest, ErrorControllerAction.BadRequest);
        }

        /// <summary>
        /// Returns a HTTP 403 Forbidden error view. Returns a partial view if the request is an AJAX call.
        /// Unlike a 401 Unauthorized response, authenticating will make no difference.
        /// </summary>
        /// <returns>The partial or full forbidden view.</returns>
        [Route("forbidden", Name = ErrorControllerRoute.GetForbidden)]
        public ActionResult Forbidden()
        {
            return this.GetErrorView(HttpStatusCode.Forbidden, ErrorControllerAction.Forbidden);
        }

        /// <summary>
        /// Returns a HTTP 500 Internal Server Error error view. Returns a partial view if the request is an AJAX call.
        /// </summary>
        /// <returns>The partial or full internal server error view.</returns>
        [Route("internalservererror", Name = ErrorControllerRoute.GetInternalServerError)]
        public ActionResult InternalServerError()
        {
            return this.GetErrorView(HttpStatusCode.InternalServerError, ErrorControllerAction.InternalServerError);
        }

        /// <summary>
        /// Returns a HTTP 405 Method Not Allowed error view. Returns a partial view if the request is an AJAX call.
        /// </summary>
        /// <returns>The partial or full method not allowed view.</returns>
        [Route("methodnotallowed", Name = ErrorControllerRoute.GetMethodNotAllowed)]
        public ActionResult MethodNotAllowed()
        {
            return this.GetErrorView(HttpStatusCode.MethodNotAllowed, ErrorControllerAction.MethodNotAllowed);
        }

        /// <summary>
        /// Returns a HTTP 404 Not Found error view. Returns a partial view if the request is an AJAX call.
        /// </summary>
        /// <returns>The partial or full not found view.</returns>
        [Route("notfound", Name = ErrorControllerRoute.GetNotFound)]
        public ActionResult NotFound()
        {
            return this.GetErrorView(HttpStatusCode.NotFound, ErrorControllerAction.NotFound);
        }

        /// <summary>
        /// Returns a HTTP 401 Unauthorized error view. Returns a partial view if the request is an AJAX call.
        /// </summary>
        /// <returns>The partial or full unauthorized view.</returns>
        [Route("unauthorized", Name = ErrorControllerRoute.GetUnauthorized)]
        public ActionResult Unauthorized()
        {
            return this.GetErrorView(HttpStatusCode.Unauthorized, ErrorControllerAction.Unauthorized);
        }

        #endregion

        #region Private Methods

        private ActionResult GetErrorView(HttpStatusCode statusCode, string viewName)
        {
            this.Response.StatusCode = (int)statusCode;

            // Don't show IIS custom errors.
            this.Response.TrySkipIisCustomErrors = true;

            ActionResult result;
            if (this.Request.IsAjaxRequest())
            {
                // This allows us to show errors even in partial views.
                result = this.PartialView(viewName);
            }
            else
            {
                result = this.View(viewName);
            }

            return result;
        }

        #endregion
    }
}