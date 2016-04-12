using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class MainKSGController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchCode = BranchCode,
                BranchName = BranchName,
                UserID = CurrentUser.UserId
            });
        }

        public JsonResult Get(KsgSpkView model)
        {
            DataSet dt = new DataSet();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_SvUtlKsgClaimList";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@KsgClaim", "Ksg");
            cmd.Parameters.AddWithValue("@BatchNo", model.NoBatch);
            cmd.Parameters.AddWithValue("@UserID", CurrentUser.UserId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            var list = GetJson(dt.Tables[1]);

            if (list != null)
            {
                return Json(new { success = true, list = list, header = model });
            }
            else
            {
                return Json(new { success = false, message = "data not found" }); ;
            }
        }

        public JsonResult MainKSGLookups()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_SvUtlKsgClaimBatchList";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@KsgClaim", "Ksg");
            cmd.Parameters.AddWithValue("@UserID", CurrentUser.UserId);
            
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            
            var list = ds.Tables[1].AsEnumerable().ToList().Select(o =>
                new KsgSpkView()
                {
                    BranchCode = o.ItemArray[0].ToString(),
                    NoBatch = o.ItemArray[1].ToString(),
                    BatchDate = o.ItemArray[2].ToString(),
                    NoReceipt = o.ItemArray[3].ToString(),
                    ReceiptDate = o.ItemArray[4].ToString(),
                    FPJNo = o.ItemArray[5].ToString(),
                    FPJDate = o.ItemArray[6].ToString(),
                    FPJGovNo = o.ItemArray[7].ToString()
                });

            if (list != null)
            {
                //return Json(new { success = true, list = list.FirstOrDefault() });
                return Json(GeLang.DataTables<KsgSpkView>.Parse(list.AsQueryable(), Request));
            }
            else
            {
                return Json(new { success = false, message = "data not found" }); ;
            }
        }

        public JsonResult delDetail(string NoBatch)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            SqlTransaction trans = ctx.Database.Connection.BeginTransaction() as SqlTransaction;
            cmd.Transaction = trans;
            cmd.CommandText = "uspfn_SvUtlKsgClaimDelete";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@KsgClaim", "Ksg");
            cmd.Parameters.AddWithValue("@BatchNo", NoBatch);
            cmd.Parameters.AddWithValue("@UserID", CurrentUser.UserId);

            try
            {
                cmd.ExecuteNonQuery();
                trans.Commit();
                return Json(new { Message = "" });
            }
            catch (Exception ex)
            {
                trans.Rollback();                
                return Json(new { Message = ex.Message });
            }
        }

        public class KsgSpkView
        {
            public string BranchCode { get; set; }
            public string NoBatch { get; set; }
            public string BatchDate { get; set; }
            public string NoReceipt { get; set; }
            public string ReceiptDate { get; set; }
            public string FPJNo { get; set; }
            public string FPJDate { get; set; }
            public string FPJGovNo { get; set; }
        }
    }
}
