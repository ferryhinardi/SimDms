using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Absence.Controllers.Utilities
{
    public class Settings: Controller
    {
        public string UploadPath 
        {
            get
            {
                return ConfigurationManager.AppSettings["UploadPath"];
            }
        }

        public string UploadedFilePath(string uploadedFileName)
        {
            return Path.Combine(Server.MapPath(UploadPath), uploadedFileName);
        }
    }
}