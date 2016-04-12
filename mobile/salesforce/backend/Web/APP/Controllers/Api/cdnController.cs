using System;
using System.Linq;
using System.Security.Claims;
using eXpressAPI.Models;
using System.Web.Mvc;
using System.Web.UI;

namespace eXpressAPP.Controllers
{
    public class cdnController : Controller
    {
        CodeEditorRepository _fsRepo = new CodeEditorRepository();

        [AllowAnonymous]
        [HttpGet]
        [MyOutputCache(Location = OutputCacheLocation.ServerAndClient)]
        [Route("cdn/{param}/{param1?}/{param2?}/{param3?}/{param4?}/{param5?}/{param6?}/{param7?}/{param8?}/{param9?}")]
        public FileResult Download(string param, string param1, string param2, string param3, string param4, string param5, string param6, string param7, string param8, string param9)
        {
            try
            {
                string filename = param;
                string fn = param;
                Response.Headers.Remove("Content-Disposition");

                if (!string.IsNullOrEmpty(param1))
                {
                    filename += "/" + param1;
                    fn = param1;
                }
                else
                {
                    Response.AppendHeader("Content-Disposition", "inline; filename=" + fn);

                    filename = Settings.GetSetting(fn);

                    if (filename.Contains("cdn/"))
                    {
                        filename = filename.Substring(4).ToLower();
                        var fi = _fsRepo.GetFileInfo3(filename);
                        return File(_fsRepo.GetFile3(fi.Fileid), fi.FileType ?? "application/octet-stream");
                    }
                    else
                    {
                        return File(Server.MapPath(filename), "image/jpeg");
                    }
                }

                
                if (!string.IsNullOrEmpty(param2))
                {
                    filename += "/" + param2;
                    fn = param2;
                }
                if (!string.IsNullOrEmpty(param3))
                {
                    filename += "/" + param3;
                    fn = param3;
                }
                if (!string.IsNullOrEmpty(param4))
                {
                    filename += "/" + param4;
                    fn = param4;
                }
                if (!string.IsNullOrEmpty(param5))
                {
                    filename += "/" + param5;
                    fn = param5;
                }
                if (!string.IsNullOrEmpty(param6))
                {
                    filename += "/" + param6;
                    fn = param6;
                }
                if (!string.IsNullOrEmpty(param7))
                {
                    filename += "/" + param7;
                    fn = param7;
                }
                if (!string.IsNullOrEmpty(param8))
                {
                    filename += "/" + param8;
                    fn = param8;
                }
                if (!string.IsNullOrEmpty(param9))
                {
                    filename += "/" + param9;
                    fn = param9;
                }

                filename = filename.ToLower();


                var ce = _fsRepo.GetCodeByFileName(filename);
                Response.AppendHeader("Content-Disposition", "inline; filename=" + fn);

                if ( !string.IsNullOrEmpty (ce))
                {
                    return File(System.Text.Encoding.UTF8.GetBytes(ce),  _fsRepo.GetFileTypeDesc(fn));
                }
                else
                {
                    var myFileInfo = _fsRepo.GetFileInfo3(filename);
                    if (myFileInfo != null)
                    {
                        return File(_fsRepo.GetFile3(myFileInfo.Fileid), myFileInfo.FileType ?? "application/octet-stream");
                    }
                }
            }
            catch (Exception) { }

            return null;
        }        

        [HttpPost]
        [Authorize]
        [Route("fileupload")]
        public virtual string Upload(FileSystemsParam data)
        {
            var length = Request.ContentLength;

            var paramData = new FileSystems();
            paramData.FileName = data.Attachment.FileName;
            paramData.Folder = data.Folder;
            paramData.FileType = data.FileType;
            paramData.UploadBy = Settings.CurrentUser();
            paramData.AutoNo = (data.FileType + "/" + paramData.Folder + "/" + paramData.FileName).ToLower();

            if (User.Identity.IsAuthenticated)
            {
                var myclaims = (User as ClaimsPrincipal).Claims;
                var _UserName = myclaims.First(x => x.Type == "name").Value;
                paramData.UploadBy = Settings.CurrentUser();
            }

            paramData.UploadDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            _fsRepo.AddOrUpdateFiles3(paramData, data.Attachment.InputStream, data.Attachment.ContentType);

            return string.Format("{0} bytes uploaded", length);
        }
    }
}