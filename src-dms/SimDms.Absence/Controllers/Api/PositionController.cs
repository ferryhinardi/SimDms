using SimDms.Absence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Absence.Controllers.Api
{
    public class PositionController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new { CompanyCode = CompanyCode, PosLevel = 0 });
        }

        public JsonResult Save(Position model)
        {
            var record = ctx.Positions.Find(CompanyCode, model.DeptCode, model.PosCode);
            var userID = CurrentUser.UserId;
            var currentDate = DateTime.Now;
            if (record == null)
            {
                record = new Position();
                record.CompanyCode = CompanyCode;
                record.DeptCode = model.DeptCode;
                record.PosCode = model.PosCode;
                record.CreatedBy = userID;
                record.CreatedDate = currentDate;
                ctx.Positions.Add(record);
            }
            record.PosName = model.PosName;
            record.PosHeader = model.PosHeader ?? "";
            record.PosLevel = model.PosLevel;
            record.StartDate = model.StartDate;
            record.FinishDate = model.FinishDate;
            record.UpdatedBy = userID;
            record.UpdatedDate = currentDate;

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

        public JsonResult Delete(Position model)
        {
            var record = ctx.Positions.Find(CompanyCode, model.DeptCode, model.PosCode);
            if (record != null)
            {
                ctx.Positions.Remove(record);

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
