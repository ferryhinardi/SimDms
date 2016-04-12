using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.Sparepart.Models;

namespace SimDms.Sparepart.Controllers.Api
{
    public class Pem_MaintenanceSubtitusiController : BaseController
    {
        public JsonResult POSLookup()
        {

            var queryable = ctx.Database.SqlQuery<spTrnPOrderBalanceLookup>(string.Format("uspfn_spTrnPOrderBalance '{0}', '{1}'", CompanyCode, BranchCode)).AsQueryable();
            return Json(queryable.KGrid());
        }
        public JsonResult PartLookup(string PosNo)
        {

            var queryable = ctx.Database.SqlQuery<spTrnPOrderBalancePartLookup>(string.Format("uspfn_spTrnPOrderBalancePart '{0}', '{1}', '{2}'", CompanyCode, BranchCode, PosNo)).AsQueryable();
            return Json(queryable.KGrid());
        }
 

        public JsonResult Save(spMstMovingCode model)
        {
            string msg = "";
            var record = ctx.spMstMovingCodes.Find(CompanyCode, model.MovingCode);

            if (record == null)
            {
                record = new spMstMovingCode
                {
                    CompanyCode = CompanyCode
                };

                ctx.spMstMovingCodes.Add(record);
                msg = "New item price added...";
            }
            else
            {
                ctx.spMstMovingCodes.Attach(record);
                msg = "Item price updated";
            }

            record.MovingCode = model.MovingCode;
            record.MovingCodeName = model.MovingCodeName;
            record.Param1 = model.Param1;
            record.Sign1 = model.Sign1;
            record.Variable = model.Variable;
            record.Sign2 = model.Sign2;
            record.Param2 = model.Param2;
            record.LockingBy = CurrentUser.UserId;
            record.LockingDate = ctx.CurrentTime;

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




        public JsonResult Delete(spMstMovingCode model)
        {
            var record = ctx.spMstMovingCodes.Find(CompanyCode, model.MovingCode);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.spMstMovingCodes.Remove(record);
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
