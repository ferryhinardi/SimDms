using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class TarifJasaController : BaseController
    {
        //
        // GET: /TarifJasa/

        public JsonResult Default()
        {
            return Json(new
            {

                CompanyCode = CompanyCode,
                EffectiveDate = DateTime.Now,
                ProductType = ProductType,
                LaborPrice = 0,
            });
        }

        public JsonResult Save(SvMstTarifJasa model)
        {
            var record = ctx.SvMstTarifJasas.Find(CompanyCode, BranchCode, model.LaborCode, ProductType, model.EffectiveDate);

            if (record == null)
            {
                record = new SvMstTarifJasa
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    LaborCode = model.LaborCode,
                    ProductType = ProductType,
                    EffectiveDate = model.EffectiveDate,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                    
                };
                ctx.SvMstTarifJasas.Add(record);
            }

            record.Description = model.Description;
            record.LaborPrice = model.LaborPrice;
            record.IsActive = model.IsActive;
            record.LastupdateBy = CurrentUser.UserId;
            record.LastupdateDate = DateTime.Now;
            record.IsLocked = false;
            record.LockingBy = CurrentUser.UserId;
            record.LockingDate = DateTime.Now;

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

        public JsonResult deleteData(SvMstTarifJasa model)
        {

            var record = ctx.SvMstTarifJasas.Find(CompanyCode, BranchCode, model.LaborCode, ProductType, model.EffectiveDate);
            if (record != null)
            {
                ctx.SvMstTarifJasas.Remove(record);
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

        public JsonResult UpdateAll()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_SvMstLbrPriceUpdInMstJob";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@UserId", CurrentUser.UserId);
            cmd.Parameters.AddWithValue("@Now", DateTime.Now);
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                cmd.Connection.Dispose();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}