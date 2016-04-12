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
    public class InfoSPKController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {
                JobOrderDateBegin = DateTime.Now,
                JobOrderDateEnd = DateTime.Now,
            });
        }

        public JsonResult Save(svTrnInvClaim model, HelpSave Help, string InvoiceNo)
        {
            //var record = ctx.svTrnInvClaims.Find(CompanyCode, ProductType, BranchCode, model.InvoiceNo);
            var record = ctx.svTrnInvClaims.Where(a=>a.CompanyCode==CompanyCode && a.ProductType==ProductType && a.BranchCode==BranchCode && a.InvoiceNo==InvoiceNo).SingleOrDefault();
            if (record == null)
            {
                record = new svTrnInvClaim
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    ProductType = ProductType,
                    InvoiceNo = InvoiceNo,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                };
                ctx.svTrnInvClaims.Add(record);
            }
            record.IsCbu = model.IsCbu;
            record.CategoryCode = Help.RefferenceCode;
            record.ComplainCode = Help.ComCode;
            record.DefectCode = Help.DefCod;
            record.OperationNo = Help.OperationNoDtl;
            record.CausalPartNo = Help.PartNo;
            record.TroubleDescription = Help.TroubleDesc;
            record.ProblemExplanation = Help.ProblemExp;
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;
            record.IsLocked = false;
            record.LockingBy = CurrentUser.UserId;
            record.LockingDate = DateTime.Now;

            try
            {
                ctx.SaveChanges();

                var query = "exec uspfn_SvInfoClaimSave @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12";
                object[] parameters = {record.CompanyCode, record.BranchCode, record.ProductType, record.InvoiceNo, 
                                  record.IsCbu, record.CategoryCode, record.ComplainCode, record.DefectCode, record.OperationNo, 
                                  record.CausalPartNo, record.TroubleDescription, record.ProblemExplanation, CurrentUser.UserId};

                ctx.Database.ExecuteSqlCommand(query, parameters);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        
        public JsonResult getTableJR(string JobOrderNo, string JobOrderNoEnd, DateTime JobOrderDateBegin, DateTime JobOrderDateEnd)
        {
            var CheckDate = '0';
            var IsSprClaim = Request["ClaimType"].ToString() == "0" ? false : true;  //'0';
            DataSet dt = new DataSet();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_SvInqClaimFromSpk";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@JobOrderNo1", JobOrderNo);
            cmd.Parameters.AddWithValue("@JobOrderNo2", JobOrderNoEnd);
            cmd.Parameters.AddWithValue("@CheckDate", CheckDate);
            cmd.Parameters.AddWithValue("@JobOrderDate1", JobOrderDateBegin);
            cmd.Parameters.AddWithValue("@JobOrderDate2", JobOrderDateEnd);
            cmd.Parameters.AddWithValue("@IsSprClaim", IsSprClaim);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            var list = GetJson(dt.Tables[1]);

            if (list != null)
            {
                return Json(list);
            }
            else
            {
                return Json(new { success = false }); ;
            }
        }
        public JsonResult getTableDC(string JobOrderNo, string JobOrderNoEnd, DateTime JobOrderDateBegin, DateTime JobOrderDateEnd)
        {
            var CheckDate = '0';
            var IsSprClaim = Request["ClaimType"].ToString() == "0" ? false : true;  //'0';
            DataSet dt = new DataSet();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_SvInqClaimFromSpk";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@JobOrderNo1", JobOrderNo);
            cmd.Parameters.AddWithValue("@JobOrderNo2", JobOrderNoEnd);
            cmd.Parameters.AddWithValue("@CheckDate", CheckDate);
            cmd.Parameters.AddWithValue("@JobOrderDate1", JobOrderDateBegin);
            cmd.Parameters.AddWithValue("@JobOrderDate2", JobOrderDateEnd);
            cmd.Parameters.AddWithValue("@IsSprClaim", IsSprClaim);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            var list = GetJson(dt.Tables[0]);
            int jum = list.Count();
            if (list != null)
            {
                return Json(new { list, jum });
            }
            else
            {
                return Json(new { success = false }); ;
            }
        }

        public JsonResult getTablePart(string JobOrderNo, string CausalPartNo)
        {
            DataSet dt = new DataSet();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_SvInqCausalPart";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@InvoiceNo", JobOrderNo);
            cmd.Parameters.AddWithValue("@CausalPartNo", CausalPartNo);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            
                var list = GetJson(dt.Tables[0]);
                if (list != null)
                {
                    return Json(list);
                }
                else
                {
                    return Json(new { success = false }); ;
                }
            
           // int jum = list.Count();
        }
    }
}
