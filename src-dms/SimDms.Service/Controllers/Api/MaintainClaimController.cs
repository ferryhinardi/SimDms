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
    public class MaintainClaimController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {

               // BatchDate = DateTime.Now,
               // ReceiptDate = DateTime.Now,
               // FPJDate = DateTime.Now,

            });
        }


        public JsonResult deleteData(string BatchNo)
        {
            var KsgClaim = "CLM";
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_SvUtlKsgClaimDelete";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@KsgClaim", KsgClaim);
            cmd.Parameters.AddWithValue("@BatchNo", BatchNo);
            cmd.Parameters.AddWithValue("@UserID", CurrentUser.UserId);
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

        public JsonResult GetDataTable(string BatchNo)
        {
            var KsgClaim = "CLM";
            DataSet dt = new DataSet();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_SvUtlKsgClaimList";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@KsgClaim", KsgClaim);
            cmd.Parameters.AddWithValue("@BatchNo", BatchNo);
            cmd.Parameters.AddWithValue("@UserID", CurrentUser.UserId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            var list = GetJson(dt.Tables[1]);

            if (list != null)
            {
                return Json(list);
            }
            else
            {
                return Json(new { success = false, message = "data not found" }); ;
            }
            
        }

    }
}
