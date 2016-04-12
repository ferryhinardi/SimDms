using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class MobileController : BaseController
    {
        [AllowCrossSiteJson]
        [HttpPost]
        public string Upload(HttpPostedFileBase file)
        {
            return "Done";
        }

        [HttpPost]
        public JsonResult UploadFile()
        {
            HttpFileCollectionBase files = Request.Files;
            bool fileSaved = false;

            foreach (string h in files.AllKeys)
            {
                if (files[h].ContentLength > 0)
                {
                    string fileName = files[h].FileName;
                    int fileSize = files[h].ContentLength;

                    string serverPath = Path.Combine(Server.MapPath("~/ImageUploads"));

                    if (!Directory.Exists(serverPath))
                    {
                        Directory.CreateDirectory(serverPath);
                    }
                    //Get & Save the File
                    Request.Files.Get(h).SaveAs(serverPath + fileName);
                    fileSaved = true;
                }
            }
            return Json(new { FileSaved = fileSaved });
        }
    }
}
