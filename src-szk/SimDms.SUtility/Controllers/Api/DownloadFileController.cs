using SimDms.SUtility.Models.Dcs;
using SimDms.SUtility.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SimDms.SUtility.Controllers.Api
{
    public class DownloadFileController : BaseController
    {
        [HttpPost]
        [Authorize]
        public JsonResult UpdateDcsDownloadFileStatus(string DealerCode, string DataID, string Status, DateTime? StartDate, DateTime? EndDate, string UpdatedStatus)
        {
            ResultModel result = new ResultModel()
            {
                status = false,
                message = "",
                innerMessage = "",
                data = null
            };

            if (string.IsNullOrEmpty(UpdatedStatus) == false)
            {
                IQueryable<DcsDownload> list = ctx.DcsDownloads;

                if (string.IsNullOrEmpty(DealerCode) == false)
                {
                    list = list.Where(x => x.CustomerCode.Equals(DealerCode));
                }

                if (string.IsNullOrEmpty(DataID) == false)
                {
                    list = list.Where(x => x.DataID.Equals(DataID));
                }

                if (string.IsNullOrEmpty(Status) == false)
                {
                    list = list.Where(x => x.Status.Equals(Status));
                }

                if (StartDate != null)
                {
                    list = list.Where(x => x.CreatedDate >= StartDate.Value);
                }

                if (EndDate != null)
                {
                    list = list.Where(x => x.CreatedDate <= EndDate.Value);
                }

                list = list.OrderBy(x => x.CreatedDate);

                foreach (DcsDownload item in list)
                {
                    item.Status = UpdatedStatus;
                }

                try
                {
                    ctx.SaveChanges();
                    result.status = true;
                    result.message = "Data DCS berhasil disimpan.";
                }
                catch (Exception)
                {
                    result.status = false;
                    result.message = "Data DCS gagal diupdate.";
                }
            }

            return Json(result);
        }
    }
}
