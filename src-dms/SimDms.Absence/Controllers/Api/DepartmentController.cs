using SimDms.Absence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Absence.Controllers.Api
{
    public class DepartmentController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new { CompanyCode = CompanyCode, OrgGroupCode = "DEPT", OrgSeq = 0 });
        }

        public JsonResult Save(OrgGroup model)
        {
            var record = ctx.OrgGroups.Find(CompanyCode, "DEPT", model.OrgCode);
            if (record == null)
            {
                record = new OrgGroup();
                record.CompanyCode = CompanyCode;
                record.OrgGroupCode = "DEPT";
                record.OrgCode = model.OrgCode;
                record.CreatedBy = CurrentUser.UserId;
                record.CreatedDate = DateTime.Now;
                ctx.OrgGroups.Add(record);
            }
            record.OrgName = model.OrgName;
            record.OrgSeq = model.OrgSeq;
            record.OrgHeader = model.OrgHeader ?? "";
            record.StartDate = model.StartDate;
            record.FinishDate = model.FinishDate;
            record.UpdatedBy = CurrentUser.UserId;
            record.UpdatedDate = DateTime.Now;

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

        public JsonResult Delete(OrgGroup model)
        {
            var record = ctx.OrgGroups.Find(CompanyCode, "DEPT", model.OrgCode);
            if (record != null)
            {
                ctx.OrgGroups.Remove(record);

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
            return Json(new { success = false, message = "data not valid" });
        }
    }
}
