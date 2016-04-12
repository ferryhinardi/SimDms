using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using eXpressAPI.Models;
using MongoDB.Driver.Builders;

namespace eXpressAPP.Controllers.Api
{

    [Authorize]
    public class FilesController : ApiController
    {

        [HttpGet]
        //[CustomRoute("api/files/templates/{id}/{ext}")]
        public async Task<IHttpActionResult> templates(string id, string ext)
        {
            return new FileActionResult(id, ext);
        }


    }

    public class FileActionResult : IHttpActionResult
    {
        public FileActionResult(string id, string ext)
        {
            this.FileId = id;
            this.FileExt = ext ;
        }

        public string FileId { get; private set; }
        public string FileExt { get; private set; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            string content = "", extention = "text/plain";

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            CodeEditorRepository _codeRepo = new CodeEditorRepository();
            CodeEditor ce = _codeRepo.GetByFileName(FileId);

            try
            {
               
                if (FileExt == "html")
                {
                    extention = ("text/html");                    
                    if (ce != null)
                    {
                        content = ce.html;
                    }
                    else
                    {
                        content = "<table width=100% height=100%>" +
                                "    <tr>" +
                                "        <td style=\"text-align: center; vertical-align: middle;\">" +
                                "            <img title=\"Website_under_construction\" alt=\"Under Construction\" src=\"" + Settings.UnderContraction  +  "\">" +
                                "        </td>" +
                                "    </tr>" +
                                "</table>";
                    }
                }
                else if (FileExt == "htmlx")
                {
                    extention = ("text/html");
                    if (ce != null)
                    {
                        content = ce.html;
                    }
                    ce = _codeRepo.GetByFileName("ribbon");
                    content = ce.html + content; // +System.Environment.NewLine + "<script src=\"appengine/getfile?name=" + name + "\"></script>";

                }
                else if (FileExt == "css")
                {
                    extention = ("text/css");
                    if (ce != null)
                    {
                        content = ce.css;
                    }
                }
                else
                {
                    extention = ("application/javascript");
                    if (ce != null)
                    {
                        content = ce.cfg + System.Environment.NewLine + ce.js;
                    }
                }
            }
            catch (Exception ex)
            {
                content = ex.Message;
            }


            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content ?? ""));
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(extention);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = FileId + "." + FileExt };
            
            return Task.FromResult(result);

        }

    }


}