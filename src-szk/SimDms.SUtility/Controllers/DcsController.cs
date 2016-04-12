using SimDms.SUtility.Models.Dcs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SimDms.SUtility.Controllers
{
    public class DcsController : BaseController
    {
        public string Download()
        {
            if (User.Identity.IsAuthenticated)
            {
                return HtmlRender("dcs/dnload.js");
            }

            return HtmlRender("general/unauthorized.js");
        }

        public string Upload()
        {
            return HtmlRender("dcs/upload.js");
        }

        /// <summary>
        /// Method ini digunakan untuk handle SDMS Apabila penarikan data DCS tidak berhasil melalui SDMSLink di SQL Server
        /// </summary>
        /// <returns></returns>
        public string DownloadDCSToSDMS()
        {
            var dcsLink = ctx.DcsDownloads;
            var data = dcsLink.Where(m => m.ID > 453686 && m.DataID == "SHIST" && (m.ProductType == "4W" || m.ProductType == "4")).OrderBy(m => m.ID);
            foreach (DcsDownload row in data)
            {
                if (row.DataID == "SHIST")
                {
                    DcsDownloadClone clone = new DcsDownloadClone();
                    clone.Contents = row.ClobContent;
                    clone.CreatedDate = row.CreatedDate;
                    clone.CustomerCode = row.CustomerCode;
                    clone.DataID = row.DataID;
                    clone.Header = row.ClobContent.Length > 286 ? row.ClobContent.Substring(0, 286) : row.ClobContent;
                    clone.ID = row.ID;
                    clone.ProductType = "4";
                    clone.Status = row.Status;
                    clone.UpdateDate = DateTime.Now;
                    ctxSdms.DcsDownloadClones.Add(clone);
                    ctxSdms.SaveChanges();
                }
            }
            
            return "Complete";
        }
    }
}
