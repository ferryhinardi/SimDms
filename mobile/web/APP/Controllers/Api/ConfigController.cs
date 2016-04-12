using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Thinktecture.IdentityModel.Client;
using eXpressAPI;
using eXpressAPI.Models;
using MongoDB.Driver.Builders;

namespace eXpressAPP.Controllers.Api
{
    [Authorize]
    public class ConfigController : ApiController
    {

        [HttpPost]
        public async Task<IHttpActionResult> SaveContent(CodeEditor ce)
        {

            var MyClaims = (User as ClaimsPrincipal).Claims;
            var UserId = MyClaims.GetValue("sub");

            CodeEditorRepository _codeRepo = new CodeEditorRepository();

             _codeRepo.AddOrUpdateCodeEditor(ce);

            return Ok(ce.filename);
        }

        [HttpGet]
        public async Task<IHttpActionResult> LoadContent([FromUri] SearchParam SP)
        {
            string filename = SP.filename;
            CodeEditorRepository _codeRepo = new CodeEditorRepository();
            CodeEditor ce = null;
            string result = "success";
            bool success = false;

            try
            {
                ce = _codeRepo.GetByFileName(filename);
                success = true;
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return Ok(new { success = success, data = ce, message = result });
        }

        [HttpGet]
        //[CustomRoute("api/config/loadfile/{id}/{ext}")]
        public async Task<IHttpActionResult> loadfile(string id, string ext)
        {
            return Ok(new { id = id, ext = ext });
        }

        [HttpGet]
        //[CustomRoute("api/config/routes/{id}/{ext}")]
        public async Task<HttpResponseMessage> routes(string id, string ext)
        {
            var myclaims = (User as ClaimsPrincipal).Claims;
            var _RoleId = myclaims.First(x => x.Type == "user_role").Value;

            string strRoute = " '/dashboard': { " +
                              "   templateUrl: '" + @Settings.Home + "api/files/templates/dashboard/html', " +
                              "   controllerUrl: '" + @Settings.Home + "api/files/templates/dashboard/js', " +
                              "   css: '" + @Settings.Home + "api/files/templates/dashboard/css' " +
                              " }";

                         strRoute += ", '/': { " +
                              "   templateUrl: '" + @Settings.Home + "api/files/templates/home/html', " +
                              "   controllerUrl: '" + @Settings.Home + "api/files/templates/home/js', " +
                              "   css: '" + @Settings.Home + "api/files/templates/home/css' " +
                              " }";

            DataAccessContext db = new DataAccessContext();
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

            var listmenus = db.Database.SqlQuery<Menus>("exec sp_role_checkmenu '" + _RoleId + "'").ToList();

            foreach(var item in listmenus)
            {
                if (!string.IsNullOrEmpty(item.Link))
                {
                    strRoute += ", '/" + item.Link  + "': { " +
                                  "   templateUrl: '" + @Settings.Home + "api/files/templates/" + item.Link + "/html', " +
                                  "   controllerUrl: '" + @Settings.Home + "api/files/templates/" + item.Link + "/js', " +
                                  "   css: '" + @Settings.Home + "api/files/templates/" + item.Link + "/css' " +
                                  " }";
                    string lnkForm = item.Link + "-form";
                    strRoute += ", '/" + lnkForm + "/:ID1?/:ID2?/:ID3?/:ID4?/:ID5?/:ID6?/:ID7?': { " +
                              "   templateUrl: '" + @Settings.Home + "api/files/templates/" + lnkForm + "/html', " +
                              "   controllerUrl: '" + @Settings.Home + "api/files/templates/" + lnkForm + "/js', " +
                              "   css: '" + @Settings.Home + "api/files/templates/" + lnkForm + "/css' " +
                              " }";
                }

            }

            strRoute = "define([], function() { return { defaultRoutePath: '/', routes: { " + strRoute + " }};});";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(strRoute ?? ""));
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/javascript");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "routes.js" };

            return  (result);
        }
    }
}
