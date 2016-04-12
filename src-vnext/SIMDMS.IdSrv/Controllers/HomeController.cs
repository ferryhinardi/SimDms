using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIMDMS.IdSrv.Controllers
{
    public class HomeController : Controller
    {

        //[AllowAnonymous]
        //public async Task<string> GetToken(LogOnModel data)
        //{

        //    using (HttpClient httpClient = new HttpClient())
        //    {
        //        var Url = Settings.TokenURL;
        //        var response = await httpClient.PostAsync(new Uri(Url), new StringContent("grant_type=password&UserName=" + data.UserName + "&Password=" + data.Password));
        //        var result = await response.Content.ReadAsStringAsync();
        //        return result.ToString();
        //    }

        //    return "{success=false, access_token=null}";
        //}

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}