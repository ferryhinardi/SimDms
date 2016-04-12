using SimDms.Absence.Models.Results;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Absence.Controllers.Api
{
    public class AbsenceController : BaseController
    {
        public JsonResult UploadFile(HttpPostedFileBase file)
        {
            ResultModel result = InitializeResult();

            if (file != null)
            {
                Stream stream = file.InputStream;

                byte[] rawData = new byte[file.ContentLength];
                file.InputStream.Read(rawData, 0, file.ContentLength);

                string fileHash = ComputeHash(stream);

                byte[] newRawData = rawData;

            }
            
            return Json(result);
        }

        private string ComputeHash(Stream inputStream)
        {
            HashAlgorithm ha = System.Security.Cryptography.SHA256.Create();
            return BitConverter.ToString(ha.ComputeHash(inputStream)).Replace("-", "");
        }
    }
}
