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
    public class ParameterOrderController : BaseController
    {
        public JsonResult loadtest()
        {
            var queryable = ctx.Database.SqlQuery<spMstItemInfoExport>("select CompanyCode ,PartNo ,PartName , SupplierCode ,PartCategory  from spMstItemInfo where partname like'CYLINDER'").AsQueryable();
            return Json(queryable, JsonRequestBehavior.AllowGet);
        }



        public JsonResult ParameterOrderBrowse(string refcode)
        {
            //var queryable = ctx.spMstOrderParamViews.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode);
            var queryable = ctx.Database.SqlQuery<spMstOrderParamView>("exec sp_spMstOrderParamView '" + CompanyCode + "','" + BranchCode + "'").AsQueryable();
            return Json(GeLang.DataTables<spMstOrderParamView>.Parse(queryable, Request),JsonRequestBehavior.AllowGet);
        }


        public JsonResult ParameterOrderLookup(string id)
        {
            return Json(eXecSp<spMstOrderParamLookup>("sp_spMstOrderParamLookup", "'" + id + "'"));
        }


        public JsonResult Save(spMstOrderParam model)
        {
            string msg = "";
            var record = ctx.spMstOrderParams.Find(CompanyCode, BranchCode, model.SupplierCode,model.MovingCode);

            if (record == null)
            {
                record = new spMstOrderParam
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    SupplierCode = model.SupplierCode,
                    MovingCode = model.MovingCode,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };

                ctx.spMstOrderParams.Add(record);
                msg = "New item price added...";
            }
            else
            {
                ctx.spMstOrderParams.Attach(record);
                msg = "Item price updated";
            }

           
            record.LeadTime = model.LeadTime;
            record.OrderCycle = model.OrderCycle;
            record.SafetyStock = model.SafetyStock;
 
 
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;

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




        public JsonResult Delete(spMstOrderParam model)
        {
            var record = ctx.spMstOrderParams.Find(CompanyCode, BranchCode, model.SupplierCode, model.MovingCode);

 
            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.spMstOrderParams.Remove(record);
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

        public JsonResult UpdateAll(spMstOrderParam model)
        {

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_spUpdateOrderParams";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@SupplierCode", model.SupplierCode);
            cmd.Parameters.AddWithValue("@MovingCode", model.MovingCode);
            cmd.Parameters.AddWithValue("@LeadTime", model.LeadTime);
            cmd.Parameters.AddWithValue("@OrderCycle", model.OrderCycle);
            cmd.Parameters.AddWithValue("@SafetyStock", model.SafetyStock);
            cmd.Parameters.AddWithValue("@UserId", CurrentUser.UserId);
           
            try
            {
                cmd.Connection.Open();
                var i = cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                cmd.Connection.Dispose();

                return Json(new { success = true, total= i });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        public string CheckRecord(string SupplierCode)
        {
            var data = ctx.spMstOrderParams.Find(CompanyCode, BranchCode, SupplierCode);
            if (data == null)
            {
                return "0";
            }
            return "1";
        }


        public JsonResult GetListMovingCode()
        {
            var data = ctx.spMstMovingCodes.Where(X => X.CompanyCode == CompanyCode);
            return Json(GeLang.DataTables<spMstMovingCode>.Parse(data, Request));
        }

 
    }
}
