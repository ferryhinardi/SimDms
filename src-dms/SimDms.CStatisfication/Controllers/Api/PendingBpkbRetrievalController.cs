using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.CStatisfication.Models;
using SimDms.CStatisfication.Models.General;

namespace SimDms.CStatisfication.Controllers.Api
{
    public class PendingBpkbRetrievalController : BaseController
    {
        [HttpPost]
        public JsonResult Save(CsBpkbRetrievalInformation model, string Chassis)
        {
            ResultModel result = InitializeResultModel();
            DateTime currentTime = DateTime.Now;
            var currentUser = CurrentUser.UserId;

            if (model.RetrievalEstimationDate == null)
            {
                result.message = "Tanggal penundaan pengambilan BPKB harus diisi.";

                return Json(result);
            }

            var data = ctx.CsBpkbRetrievalInformations.Find(CompanyCode, model.CustomerCode, model.RetrievalEstimationDate);
            if (data == null)
            {
                data = new CsBpkbRetrievalInformation();
                data.CompanyCode = CompanyCode;
                data.CustomerCode = model.CustomerCode;
                data.RetrievalEstimationDate = model.RetrievalEstimationDate;
                data.CreatedBy = currentUser;
                data.CreatedDate = currentTime;

                ctx.CsBpkbRetrievalInformations.Add(data);
            }

            data.Notes = model.Notes;
            data.UpdatedBy = currentUser;
            data.UpdatedDate = currentTime;
            data.IsDeleted = false;

            try
            {
                ctx.SaveChanges();
                var result3 = ctx.Database.ExecuteSqlCommand("exec uspfn_CRUDBPKPReminderResource  @CompanyCode=@p0, @CustomerCode=@p1, @Chassis=@p2", CompanyCode, model.CustomerCode, Chassis);

                result.success = true;
                result.message = "Data penundaan pengambilan BPKB berhasil disimpan ke dalam database.";
            }
            catch (Exception)
            {
                result.message = "Data penundaan pengambilan BPKB tidak dapat disimpan.";
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult Delete(CsBpkbRetrievalInformation model, string Chassis)
        {
            ResultModel result = InitializeResultModel();

            var data = ctx.CsBpkbRetrievalInformations.Find(CompanyCode, model.CustomerCode, model.RetrievalEstimationDate);
            if (data != null)
            {
                data.IsDeleted = true;
            }

            try
            {
                ctx.SaveChanges();
                var result3 = ctx.Database.ExecuteSqlCommand("exec uspfn_CRUDBPKPReminderResource  @CompanyCode=@p0, @CustomerCode=@p1, @Chassis=@p2", CompanyCode, model.CustomerCode, Chassis);

                result.success = true;
                result.message = "Data penundaan pengambilan BPKB berhasil dihapus.";
            }
            catch (Exception)
            {
                result.message = "Data penundaan pengambilan BPKB gagal dihapus.";
            }

            return Json(result);
        }
    }
}
