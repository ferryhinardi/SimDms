using SimDms.Sales.Models;
using SimDms.Sales.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.Common.Models;
using SimDms.Common;

namespace SimDms.Sales.Controllers.Api
{
    public class MstPerlengkapanController : BaseController
    {
        public JsonResult Save(MstPerlengkapan model)
        {
            string msg = "";
            var record = ctx.MstPerlengkapan.Find(CompanyCode, BranchCode, model.PerlengkapanCode);

            if (record == null)
            {
                record = new MstPerlengkapan
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    PerlengkapanCode = model.PerlengkapanCode,
                    PerlengkapanName = model.PerlengkapanName,
                    Remark = model.Remark == null ? "" : model.Remark,
                    Status = model.Status == "True" ? "1" : "0",
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = ctx.CurrentTime,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = ctx.CurrentTime,
                    isLocked = false,
                    LockingBy = "",
                    LockingDate = Convert.ToDateTime("1900-01-01")

                };
                ctx.MstPerlengkapan.Add(record);
                msg = "New Perlengkapan added...";
            }
            else
            {
                record.PerlengkapanName = model.PerlengkapanName;
                record.Remark = model.Remark == null ? "" : model.Remark;
                record.Status = model.Status == "True" ? "1" : "0";
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = ctx.CurrentTime;
                record.isLocked = false;
                record.LockingBy = "";
                record.LockingDate = Convert.ToDateTime("1900-01-01");
                msg = "Perlnegkapan updated";
            }



            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, message = msg, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Delete(MstPerlengkapan model)
        {
            var record = ctx.MstPerlengkapan.Find(CompanyCode, BranchCode, model.PerlengkapanCode);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.MstPerlengkapan.Remove(record);
            }

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
