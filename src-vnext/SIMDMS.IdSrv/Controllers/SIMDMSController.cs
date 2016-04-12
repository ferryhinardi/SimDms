using eXpressAPI;
using eXpressAPI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RestSharp;
using SIMDMS.IdSrv;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebHost.AspId;

namespace SIMDMS.IdSrv.Controllers
{
    [RoutePrefix("suzuki/simdms")]
    [Route("{action=index}")]
    [Authorize]
    public class SIMDMSController : Controller
    {
        private CodeEditorRepository myCfg = new CodeEditorRepository();

        [AllowAnonymous]
        public string GetToken(LoginViewModel data)
        {
            RestClient restClient = new RestClient(ConfigurationManager.AppSettings["TokenUrl"].ToString());
            if (restClient == null)
                return "{success=false, access_token=null}";
            RestRequest restRequest = new RestRequest((Method)1);
            restRequest.AddParameter("grant_type", (object)"password");
            restRequest.AddParameter("UserName", (object)data.UserName);
            restRequest.AddParameter("Password", (object)data.Password);
            return restClient.Execute((IRestRequest)restRequest).Content.ToString();
        }

        [Route("avatar/{param}")]
        [AllowAnonymous]
        [HttpGet]
        public FileResult Avatar(string param)
        {
            try
            {
                this.Response.Headers.Remove("Content-Disposition");
                FileSystems fileInfo3_1 = this.myCfg.GetFileInfo3("apps/mobile/img/" + param);
                if (fileInfo3_1 != null)
                    return (FileResult)this.File(this.myCfg.GetFile3(fileInfo3_1.Fileid), fileInfo3_1.FileType ?? "application/octet-stream");
                FileSystems fileInfo3_2 = this.myCfg.GetFileInfo3("apps/mobile/img/unknown.png");
                if (fileInfo3_2 != null)
                    return (FileResult)this.File(this.myCfg.GetFile3(fileInfo3_2.Fileid), fileInfo3_2.FileType ?? "application/octet-stream");
            }
            catch (Exception ex)
            {
            }
            return (FileResult)null;
        }

        [AllowAnonymous]
        [Route("m/{moduleId?}/{param?}/{param1?}/{param2?}/{param3?}/{param4?}/{param5?}/{param6?}/{param7?}/{param8?}/{param9?}")]
        [HttpGet]
        public virtual object loadModules(string moduleId, string param, string param1, string param2, string param3, string param4, string param5, string param6, string param7, string param8, string param9)
        {
            string str1 = "apps/index";
            if (!string.IsNullOrEmpty(moduleId))
                str1 = "apps/" + moduleId;
            string str2 = string.IsNullOrEmpty(param) ? str1 + "/index.html" : str1 + "/" + param;
            if (!string.IsNullOrEmpty(param1))
                str2 = str2 + "/" + param1;
            if (!string.IsNullOrEmpty(param2))
                str2 = str2 + "/" + param2;
            if (!string.IsNullOrEmpty(param3))
                str2 = str2 + "/" + param3;
            if (!string.IsNullOrEmpty(param4))
                str2 = str2 + "/" + param4;
            if (!string.IsNullOrEmpty(param5))
                str2 = str2 + "/" + param5;
            if (!string.IsNullOrEmpty(param6))
                str2 = str2 + "/" + param6;
            if (!string.IsNullOrEmpty(param7))
                str2 = str2 + "/" + param7;
            if (!string.IsNullOrEmpty(param8))
                str2 = str2 + "/" + param8;
            if (!string.IsNullOrEmpty(param9))
                str2 = str2 + "/" + param9;
            string str3 = str2.ToLower();
            string codeByFileName = this.myCfg.GetCodeByFileName(str3);
            string fileTypeDesc = this.myCfg.GetFileTypeDesc(str3);
            this.Response.ContentType = fileTypeDesc;
            if (fileTypeDesc == "application/octet-stream" && Enumerable.Contains<char>((IEnumerable<char>)str3, '.'))
            {
                if (!string.IsNullOrEmpty(codeByFileName))
                    return (object)this.File(Encoding.UTF8.GetBytes(codeByFileName), this.myCfg.GetFileTypeDesc(str3));
                FileSystems fileInfo3 = this.myCfg.GetFileInfo3(str3);
                if (fileInfo3 != null)
                    return (object)this.File(this.myCfg.GetFile3(fileInfo3.Fileid), this.myCfg.GetFileTypeDesc(str3));
                return (object)null;
            }
            if (!string.IsNullOrEmpty(codeByFileName))
                return (object)this.File(Encoding.UTF8.GetBytes(codeByFileName), this.myCfg.GetFileTypeDesc(str3));
            FileSystems fileInfo3_1 = this.myCfg.GetFileInfo3(str3);
            if (fileInfo3_1 != null)
                return (object)this.File(this.myCfg.GetFile3(fileInfo3_1.Fileid), this.myCfg.GetFileTypeDesc(str3));
            return (object)null;
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            JsonNetResult jsonNetResult = new JsonNetResult();
            jsonNetResult.Data = data;
            jsonNetResult.ContentType = contentType;
            jsonNetResult.ContentEncoding = contentEncoding;
            jsonNetResult.JsonRequestBehavior = behavior;
            return (JsonResult)jsonNetResult;
        }

        protected bool HaveRole(string rolename)
        {
            if (!this.User.Identity.IsAuthenticated)
                return false;
            IEnumerable<Claim> claims = (this.User as ClaimsPrincipal).Claims;
            bool flag = false;
            foreach (Claim claim in claims)
            {
                if (claim.Type == "role" && claim.Value.ToLower() == rolename.ToLower())
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        public string CurrentCompany()
        {
            if (!this.User.Identity.IsAuthenticated)
                return "";
            IEnumerable<Claim> claims = (this.User as ClaimsPrincipal).Claims;
            string str = "";
            foreach (Claim claim in claims)
            {
                if (claim.Type == "DealerCode")
                {
                    str = claim.Value.ToLower();
                    break;
                }
            }
            return str;
        }

        public string CurrentUserName()
        {
            if (!this.User.Identity.IsAuthenticated)
                return "";
            IEnumerable<Claim> claims = (this.User as ClaimsPrincipal).Claims;
            string str = "";
            foreach (Claim claim in claims)
            {
                if (claim.Type == "user_name")
                {
                    str = claim.Value.ToLower();
                    break;
                }
            }
            return str;
        }

        public string CurrentUserId()
        {
            if (!this.User.Identity.IsAuthenticated)
                return "";
            IEnumerable<Claim> claims = (this.User as ClaimsPrincipal).Claims;
            string str = "";
            foreach (Claim claim in claims)
            {
                if (claim.Type == "sub")
                {
                    str = claim.Value.ToLower();
                    break;
                }
            }
            return str;
        }

        public string Index()
        {
            return "Welcome to SIMDMS";
        }

        public JsonResult UserInfo()
        {
            SaveResult saveResult = new SaveResult(0, false);

            User user = new Context("AspId").Users.Find(CurrentUserId());

            if (user != null)
            {
                IEnumerable<Claim> claims = (this.User as ClaimsPrincipal).Claims;
                string str1 = "";
                string userName = user.UserName;
                string str2 = "";
                string str3 = "unknown.png";
                foreach (Claim claim in claims)
                {
                    if (claim.Type == "role")
                        str1 = str1 + ", " + claim.Value;
                    else if (claim.Type == "name")
                        userName = claim.Value;
                    else if (claim.Type == "DealerCode")
                        str2 = claim.Value;
                    else if (claim.Type == "avatar")
                        str3 = claim.Value;
                }
                DataTable dataTable = !(str2 != "") ? MyGlobalVar.SqlQueryDB("EXEC uspfn_GetTotalInquiryAndSpk2").Table() : MyGlobalVar.SqlQueryDB("EXEC uspfn_getexecutivesummary '" + str2 + "', 1").Table();
                saveResult.data =  new
                {
                    UserId = user.Id,
                    Name = userName,
                    Company = user.BranchCode,
                    CompanyCode = user.CompanyCode,
                    DealerCode = str2,
                    Roles = str1.Substring(2),
                    InternalUser = (user.CompanyCode == "0000000"),
                    Summary = dataTable,
                    Image = str3
                };
                saveResult.success = true;
            }
            return this.Json((object)saveResult, JsonRequestBehavior.AllowGet);
        }

        public string Logout()
        {
      //      HttpContextBaseExtensions.GetOwinContext(this.Request).get_Authentication().SignOut(new string[1]
      //{
      //  "ExternalCookie"
      //});
            return "Logoff";
        }

        public async Task<JsonResult> ChangePassword(LoginViewModel data)
        {
            SaveResult ret = new SaveResult(0, false);
            UserManager u_man = new UserManager(new UserStore(new Context("AspId")));
            if (this.CurrentUserId() != "")
            {
                string UserId = this.CurrentUserId();
                IdentityResult retX = await u_man.ChangePasswordAsync(UserId, data.UserName, data.Password);
                if (!retX.Succeeded)
                {
                    ret.message = Enumerable.Aggregate<string, string>(retX.Errors, "", (Func<string, string, string>)((c, e) => c + e.ToString() + Environment.NewLine));
                }
                else
                {
                    ret.success = true;
                    ret.message = "success";
                }
            }
            return this.Json((object)ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListEvents()
        {
            SaveResult saveResult = new SaveResult(0, false);
            if (this.HaveRole("Exhibition"))
            {
                saveResult = MyGlobalVar.SqlQueryDB("SELECT Id, ExhibitionName TEXT FROM SysExhibitionInfo ORDER BY StartPeriod desc");
                if (saveResult.Count > 0)
                    saveResult.data = (object)saveResult.Table();
            }
            else
            {
                saveResult.success = false;
                saveResult.message = "Access Denied !!!";
            }
            return this.Json((object)saveResult, JsonRequestBehavior.AllowGet);
        }

        [Route("ExhibitionResult/{id}")]
        public JsonResult ExhibitionResult(int id)
        {
            SaveResult saveResult = new SaveResult(0, false);
            if (!this.HaveRole("Exhibition"))
                saveResult.message = "Access Denied !!!";
            else
                saveResult = MyGlobalVar.SqlQueryDB("EXEC uspfn_SysExhibitionQuery " + id.ToString());
            return this.Json((object)saveResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DovsFPvsITS()
        {
            SaveResult saveResult = new SaveResult(0, false);
            if (!this.HaveRole("ITS"))
                saveResult.message = "Access Denied !!!";
            else
                saveResult = MyGlobalVar.SqlQueryDB("EXEC uspfn_GetTotalInquiryAndSpk");
            return this.Json((object)saveResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DovsFPvsITS2()
        {
            SaveResult saveResult = new SaveResult(0, false);
            if (!this.HaveRole("ITS"))
                saveResult.message = "Access Denied !!!";
            else
                saveResult = MyGlobalVar.SqlQueryDB("EXEC uspfn_GetTotalInquiryAndSpk2");
            return this.Json((object)saveResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListOutletDealer()
        {
            SaveResult saveResult = new SaveResult(0, false);
            if (!this.HaveRole("ITS"))
                saveResult.message = "Access Denied !!!";
            else
                saveResult = MyGlobalVar.SqlQueryDB("EXEC uspfn_getlistoutletdealers");
            return this.Json((object)saveResult, JsonRequestBehavior.AllowGet);
        }

        [Route("DealerExecutiveSummary/{id}")]
        public JsonResult DealerExecutiveSummary(string id)
        {
            SaveResult saveResult = new SaveResult(0, false);
            if (!this.HaveRole("ITS"))
            {
                saveResult.message = "Access Denied !!!";
            }
            else
            {
                string str = this.CurrentCompany();
                if (str != "")
                    id = str;
                saveResult = MyGlobalVar.SqlQueryDB("EXEC uspfn_getexecutivesummary '" + id + "'");
            }
            return this.Json((object)saveResult, JsonRequestBehavior.AllowGet);
        }

        //[AllowAnonymous]
        //public JsonResult GenerateUsers()
        //{
        //    SaveResult saveResult = MyGlobalVar.SqlQueryUDB("select * from tempUser");
        //    UserManager userManager = new UserManager(new UserStore(new Context("AspId")));
        //    DataTable dataTable = saveResult.Table();
        //    int count = dataTable.Rows.Count;
        //    if (count > 0)
        //    {
        //        for (int index = 0; index < count; ++index)
        //        {
        //            string str = dataTable.Rows[index][0].ToString();
        //            User result1 = userManager.FindByNameAsync(str).Result;
        //            if (result1 != null)
        //            {
        //                IdentityUserClaim identityUserClaim = new IdentityUserClaim();
        //                ((IdentityUserClaim<string>)identityUserClaim).set_ClaimType("DealerCode");
        //                ((IdentityUserClaim<string>)identityUserClaim).set_ClaimValue(dataTable.Rows[index][1].ToString());
        //                ((IdentityUser<string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>)result1).get_Claims().Add(identityUserClaim);
        //                IdentityResult result2 = userManager.UpdateAsync(result1).Result;
        //            }
        //        }
        //    }
        //    return this.Json((object)saveResult, JsonRequestBehavior.AllowGet);
        //}
    }
}
