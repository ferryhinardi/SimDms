using eXpressAPI.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace eXpressAPP.Controllers.Sys
{
    [Authorize]
    public class AppEngineController : Controller
    {
        CodeEditorRepository _codeRepo = new CodeEditorRepository();
        
        [HttpGet]
        public async Task<string> getfile(string name, string type)
        {
            CodeEditor ce = _codeRepo.GetByFileName(name);
            string content = "";

            try
            {
                if (type == "html")
                {
                    if (ce != null)
                    {
                        content = ce.html;
                    }
                } else if (type=="htmlx")
                {
                    if ( ce != null)
                    {
                        content = ce.html;
                    }
                    ce = _codeRepo.GetByFileName("ribbon");
                    content = ce.html + content;                     
                } else if (type=="css")
                {
                    if (ce != null)
                    {
                        content = ce.css;
                    }
                } else
                {
                    if (ce != null)
                    {
                        content = ce.cfg + ce.js;
                    }
                }
            }
            catch (Exception ex)
            {
                content = ex.Message;
            }

            return content;
        }
                
        [HttpGet]
        public  JsonResult  LoadContent(string filename)
        {
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

            return Json(new { success = success, data = ce, message = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public virtual object execute(string data)
        {
            return CSharpScriptEngine.Execute(data);
        }


        [HttpPost]
        public virtual string UploadFiles(FileSystemsParam data)
        {
            var length = Request.ContentLength;

            var paramData = new FileSystems();
            paramData.FileName = data.Attachment.FileName;
            paramData.Folder = data.Folder;
            paramData.FileType = data.FileType;
            paramData.UploadBy = "system";
  
            if (User.Identity.IsAuthenticated)
            {
                var myclaims = (User as ClaimsPrincipal).Claims;
                var _UserName = myclaims.First(x => x.Type == "name").Value;
                paramData.UploadBy = User.Identity.Name;
            }
            paramData.UploadDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            _codeRepo.AddOrUpdateFiles(paramData, data.Attachment.InputStream);

            return string.Format("{0} bytes uploaded", length);
        }
 

    }
    
}