using eXpressAPI;
using Microsoft.AspNet.Identity.EntityFramework;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebHost.AspId;

namespace SIMDMS.IdSrv.Controllers
{
    public class AppController : Controller
    {

        [AllowAnonymous]
        public string GetToken(LoginViewModel data)
        {
            var simdmsUrl = System.Configuration.ConfigurationManager.AppSettings["TokenUrl"].ToString();
            var client = new RestClient(simdmsUrl);
            
            if (client != null)
            {

                // client.Authenticator = new HttpBasicAuthenticator(username, password);

                var request = new RestRequest(Method.POST);
                request.AddParameter("grant_type", "password"); // adds to POST or URL querystring based on Method
                request.AddParameter("UserName", data.UserName); // adds to POST or URL querystring based on Method
                request.AddParameter("Password", data.Password); // adds to POST or URL querystring based on Method

                // execute the request
                IRestResponse response = client.Execute(request);

                return response.Content.ToString();

            }

            return "{success=false, access_token=null}";
        }

    }
    

        public class LoginViewModel
        {
            public string UserName { get; set; }
            public string Password { get; set; }
            public string grant_type { get; set; }
            public bool RememberMe { get; set; }
        }

        public class ListNewUser
        {
            public string UserId { get; set; }
            public string RoleId { get; set; } 
        }


        //[Authorize]
        //[RoutePrefix("suzuki/simdms")]
        //[Route("{action=index}")]
        //public class SIMDMSController : Controller
        //{
            
        //    protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        //    {
        //        return new JsonNetResult
        //        {
        //            Data = data,
        //            ContentType = contentType,
        //            ContentEncoding = contentEncoding,
        //            JsonRequestBehavior = behavior
        //        };
        //    }

        //    protected bool HaveRole(string rolename)
        //    {
        //        if (!User.Identity.IsAuthenticated)
        //        {
        //            return false;
        //        }
        //        var myclaims = (User as ClaimsPrincipal).Claims;
        //        bool allow = false;

        //        foreach (var item in myclaims)
        //        {
        //            if (item.Type == "role" && item.Value.ToLower() == rolename.ToLower())
        //            {
        //                allow = true;
        //                break;
        //            }
        //        }

        //        return allow;
        //    }

        //    public string CurrentCompany()
        //    {
        //        if (!User.Identity.IsAuthenticated)
        //        {
        //            return "";
        //        }

        //        var myclaims = (User as ClaimsPrincipal).Claims;
        //        string co = "";
                
        //        foreach (var item in myclaims)
        //        {
        //            if (item.Type == "DealerCode")
        //            {
        //                co = item.Value.ToLower();
        //                break;
        //            }
        //        }

        //        return co;
        //    }
             
        //    public string CurrentUserName()
        //    {
        //        if (!User.Identity.IsAuthenticated)
        //        {
        //            return "";
        //        }

        //        var myclaims = (User as ClaimsPrincipal).Claims;
        //        string co = "";

        //        foreach (var item in myclaims)
        //        {
        //            if (item.Type == "user_name")
        //            {
        //                co = item.Value.ToLower();
        //                break;
        //            }
        //        }

        //        return co;
        //    }

        //    public string CurrentUserId()
        //    {
        //        if (!User.Identity.IsAuthenticated)
        //        {
        //            return "";
        //        }

        //        var myclaims = (User as ClaimsPrincipal).Claims;
        //        string co = "";

        //        foreach (var item in myclaims)
        //        {
        //            if (item.Type == "sub")
        //            {
        //                co = item.Value.ToLower();
        //                break;
        //            }
        //        }

        //        return co;
        //    }



        //    public string Index()
        //    {
        //        return "Welcome to SIMDMS";
        //    }

        //    public JsonResult ChangePassword(LoginViewModel data)
        //    {

        //        SaveResult ret = new SaveResult(0, false);          
        //        UserManager u_man = new UserManager(new UserStore(new Context("AspId")));

        //        if (CurrentUserId() != "")
        //        {
        //            var retX = u_man.ChangePasswordAsync(CurrentUserId(), data.UserName, data.Password).Result;

        //           if (!retX.Succeeded )
        //           {
        //               var msg   = "";
        //               ret.message = retX.Errors.Aggregate(msg, (c, e) => c + (e.ToString() + Environment.NewLine));                        
        //           }
        //           else
        //           {                        
        //               ret.success = true;
        //               ret.message = "success";
        //           }
        //        }

        //        return Json(ret, JsonRequestBehavior.AllowGet);
        //    }

        //    public JsonResult ListEvents()
        //    {
        //        SaveResult ret = new SaveResult(0, false);

        //        if (HaveRole("Exhibition"))
        //        {
        //            ret = MyGlobalVar.SqlQueryDB("SELECT Id, ExhibitionName TEXT FROM SysExhibitionInfo ORDER BY StartPeriod desc");
        //            if (ret.Count > 0)
        //            {
        //                ret.data = ret.Table();
        //            }
        //        }
        //        else
        //        {
        //            ret.success = false;
        //            ret.message = "Access Denied !!!";
        //        }

        //        return Json(ret, JsonRequestBehavior.AllowGet);
        //    }

        //    public JsonResult ExhibitionResult(int id)
        //    {
        //        SaveResult ret = new SaveResult(0, false);
        //        if (!HaveRole("Exhibition"))
        //        {
        //            ret.message = "Access Denied !!!";
        //        }
        //        else
        //        {
        //            ret = MyGlobalVar.SqlQueryDB("EXEC uspfn_SysExhibitionQuery " + id.ToString());
        //        }
        //        return Json(ret, JsonRequestBehavior.AllowGet);
        //    }

        //    public JsonResult DovsFPvsITS()
        //    {
        //        SaveResult ret = new SaveResult(0, false);
        //        if (!HaveRole("ITS"))
        //        {
        //            ret.message = "Access Denied !!!";
        //        }
        //        else
        //        {
        //            ret = MyGlobalVar.SqlQueryDB("EXEC uspfn_GetTotalInquiryAndSpk");
        //        }
        //        return Json(ret, JsonRequestBehavior.AllowGet);
        //    }

        //    public JsonResult DovsFPvsITS2()
        //    {
        //        SaveResult ret = new SaveResult(0, false);
        //        if (!HaveRole("ITS"))
        //        {
        //            ret.message = "Access Denied !!!";
        //        }
        //        else
        //        {
        //            ret = MyGlobalVar.SqlQueryDB("EXEC uspfn_GetTotalInquiryAndSpk2");
        //        }
        //        return Json(ret, JsonRequestBehavior.AllowGet);
        //    }


        //    public JsonResult ListOutletDealer()
        //    {
        //        SaveResult ret = new SaveResult(0, false);
        //        if (!HaveRole("ITS"))
        //        {
        //            ret.message = "Access Denied !!!";
        //        }
        //        else
        //        {
        //            ret = MyGlobalVar.SqlQueryDB("EXEC uspfn_getlistoutletdealers");
        //        }
        //        return Json(ret, JsonRequestBehavior.AllowGet);
        //    }


        //    public JsonResult DealerExecutiveSummary(string id)
        //    {
        //        SaveResult ret = new SaveResult(0, false);
        //        if (!HaveRole("ITS"))
        //        {
        //            ret.message = "Access Denied !!!";
        //        }
        //        else
        //        {
        //            var xd = (CurrentCompany());
        //            if (xd != "")
        //            {
        //                id = xd;
        //            }
        //            ret = MyGlobalVar.SqlQueryDB("EXEC uspfn_getexecutivesummary '" + id + "'");
        //        }
        //        return Json(ret, JsonRequestBehavior.AllowGet);
        //    }

        //    [AllowAnonymous]
        //    public JsonResult GenerateUsers()
        //    {

        //        var ret = MyGlobalVar.SqlQueryUDB("select * from tempUser");
        //        UserManager u_man = new UserManager(new UserStore(new Context("AspId")));

                 
        //        var dt = ret.Table();

        //        int n = dt.Rows.Count;
                
        //        if (n > 0)
        //        {
        //            for(int i=0;i<n;i++){
        //                var userId = dt.Rows[i][0].ToString();
        //                var x = u_man.FindByNameAsync(userId).Result;
        //                if (x != null)
        //                {
        //                    var y = new IdentityUserClaim();
        //                    y.ClaimType = "DealerCode";
        //                    y.ClaimValue = dt.Rows[i][1].ToString();
        //                    x.Claims.Add(y);
        //                    var z = u_man.UpdateAsync(x).Result;
 
        //                }
        //            }
        //        }

        //        return Json(ret, JsonRequestBehavior.AllowGet);
        //    }

        //}

}