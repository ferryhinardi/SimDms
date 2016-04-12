using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.Service.Models;
using System.Transactions;
using SimDms.Service.BLL;

namespace SimDms.Service.Controllers.Api
{
    public class GetWarrantyClaimController : BaseController
    {
        //
        // GET: /GetWarrantyClaim/

        public JsonResult Default()
        {
            /* Check Claim Mode */
            var oLookup = ctx.GnMstLookUpDtls.Find(CompanyCode, "SRV_FLAG", "CLM_MODE");
            var claimMode = "";
            if (oLookup != null && oLookup.ParaValue == "1")
            {
                claimMode = "INV";
            }
            else
            {
                claimMode = "SPK";
            }

            /* Validate Holding or Branch */
            var recHolding = ctx.OrganizationDtls.Where(x => x.CompanyCode == CompanyCode && x.IsBranch == false).Count();
            var oGnMstLookUpDtl = ctx.GnMstLookUpDtls.Find(CompanyCode, "SRV_FLAG", "CLM_HOLDING");

            //DateTime gDate = Convert.ToDateTime(ctx.CoProfileServices.Find(CompanyCode, BranchCode).PeriodEnd);
            DateTime defaultTime = DateTime.Now;
            var recPeriod = ctx.CoProfileServices.Find(CompanyCode, BranchCode);
            if (recPeriod != null)
            {
                DateTime periodEndDate = Convert.ToDateTime(recPeriod.PeriodEnd).Date;
                if (defaultTime.Date.CompareTo(periodEndDate) == 1)
                {
                    defaultTime = periodEndDate.AddHours(defaultTime.Hour).AddMinutes(defaultTime.Minute);
                }
            }

            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchFrom = BranchCode,
                BranchTo = BranchCode,
                ProductType = ProductType,
                GenerateDate = defaultTime, //new DateTime(gDate.Year, gDate.Month, gDate.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second),
                ClaimType = 0,
                IsBranch = IsBranch,
                OtherCompensationAmt = 0,
                OGnMstLookUpDtl = oGnMstLookUpDtl,
                ClaimMode = claimMode,
                RecHoding = recHolding,
                PeriodFrom = DateTime.Now.Date,
                PeriodTo = DateTime.Now.Date
            });
        }

        public JsonResult GetClaim(string generateNo)
        {
            var ds = p_GetClaim(generateNo);

            return Json(new
            {
                ClaimInfo = GetJson(ds.Tables[0]),
                CostInfo = GetJson(ds.Tables[1]),
                HeaderInfo = GetJson(ds.Tables[2])
            });
        }

        public JsonResult InquiryClaim(string invoiceFrom, string invoiceTo, string branchFrom, string branchTo, string claimType)
        {
            string invFrom = "";
            string invTo = "";
            string temp = invoiceFrom.Substring(0, 3).ToString();
            if (temp == "SPK" || temp == "INW")
            {
                invFrom = invoiceFrom;
                invTo = invoiceTo;
            }
            else
            {
                invFrom = Convert.ToDateTime(invoiceFrom).ToString("yyyMMdd");
                invTo = Convert.ToDateTime(invoiceTo).ToString("yyyMMdd");
            }

            //invoiceFrom = Convert.ToDateTime(invoiceFrom).ToString("yyyMMdd");
            //invoiceTo = Convert.ToDateTime(invoiceTo).ToString("yyyMMdd");
            //var ds = p_InquiryClaim(invoiceFrom, invoiceTo, branchFrom, branchTo, claimType);
            var ds = p_InquiryClaim(invFrom, invTo, branchFrom, branchTo, claimType);

            return Json(new { ClaimInfo = GetJson(ds.Tables[0]), CostInfo = GetJson(ds.Tables[1]) });
        }

        public JsonResult ClaimData(Claim model)
        {
            var ds = p_GetClaim(model.GenerateNo);

            return Json(GetJson(ds.Tables[0]));
        }

        public JsonResult TotalAmount(Claim model)
        {
            var ds = p_GetClaim(model.GenerateNo);

            return Json(GetJson(ds.Tables[1]));
        }

        public JsonResult CasualParts(string branchCode, string invoiceNo, string causalPartNo)
        {
            DataSet ds = new DataSet();
            SqlConnection con = (SqlConnection)ctx.Database.Connection;
            SqlCommand cmd = con.CreateCommand();
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            cmd.CommandText = "uspfn_SvInqCausalPart";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", branchCode);
            cmd.Parameters.AddWithValue("@InvoiceNo", invoiceNo);
            cmd.Parameters.AddWithValue("@CausalPartNo", causalPartNo);

            da.Fill(ds);

            return Json(GetJson(ds.Tables[0]));
        }

        public JsonResult SaveClaim(string InvoiceFrom, string InvoiceTo, decimal OtherCompensationAmt, string ClaimType)
        {
            string msg = string.Empty;
            bool stat = false;
            string GenNo = string.Empty;
            bool isSprClaim = (ClaimType == "0") ? false : true;
            using (var tranScope = new TransactionScope(TransactionScopeOption.RequiresNew,
            new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                try
                {
                    GenNo = (string)ctx.Database.SqlQuery<string>("uspfn_SvSaveClaim @p0, @p1, @p2, @p3, @p4, @p5, @p6",
                        CompanyCode, BranchCode, InvoiceFrom, InvoiceTo, OtherCompensationAmt, CurrentUser.UserId, isSprClaim).FirstOrDefault();

                    ctx.SaveChanges();
                    tranScope.Complete();
                    msg = "Data has been Saved";
                    stat = true;
                }
                catch (Exception ex)
                {
                    string innerEx = (ex.InnerException == null) ? ex.Message :
                    (ex.InnerException.InnerException == null) ? ex.Message : ex.InnerException.InnerException.Message;
                    msg = (ex.InnerException == null) ? ex.Message : innerEx;
                }
            }

            return Json(new { success = stat, message = msg, data = new { GenerateNo = GenNo } });
        }

        public JsonResult SaveClaimHolding(DateTime PeriodFrom, DateTime PeriodTo, decimal OtherCompensationAmt, string ClaimType)
        {
            string msg = string.Empty;
            bool stat = false;
            string GenNo = string.Empty;
            bool isSprClaim = (ClaimType == "0") ? false : true;
            using (var tranScope = new TransactionScope(TransactionScopeOption.RequiresNew,
            new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                try
                {
                    GenNo = (string)ctx.Database.SqlQuery<string>("uspfn_SvSaveClaimHolding @p0, @p1, @p2, @p3, @p4, @p5",
                        CompanyCode, OtherCompensationAmt, PeriodFrom.ToString("yyyMMdd"), PeriodTo.ToString("yyyMMdd"), isSprClaim, CurrentUser.UserId).FirstOrDefault();

                    ctx.SaveChanges();
                    tranScope.Complete();
                    msg = "Data has been Saved";
                    stat = true;
                }
                catch (Exception ex)
                {
                    string innerEx = (ex.InnerException == null) ? ex.Message :
                    (ex.InnerException.InnerException == null) ? ex.Message : ex.InnerException.InnerException.Message;
                    msg = (ex.InnerException == null) ? ex.Message : innerEx;
                }
            }

            return Json(new { success = stat, message = msg, data = new { GenerateNo = GenNo } });
        }

        public JsonResult DeleteClaim(string GenerateNo)
        {
            string msg = string.Empty;
            bool stat = false;
            using (var tranScope = new TransactionScope(TransactionScopeOption.RequiresNew,
            new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                try
                {
                    ctx.Database.ExecuteSqlCommand("uspfn_SvUtlClaimDelete @p0, @p1, @p2, @p3, @p4",
                        CompanyCode, BranchCode, ProductType, GenerateNo, CurrentUser.UserId);

                    tranScope.Complete();
                    msg = "Data has been Deleted";
                    stat = true;
                }
                catch (Exception ex)
                {
                    string innerEx = (ex.InnerException == null) ? ex.Message :
                    (ex.InnerException.InnerException == null) ? ex.Message : ex.InnerException.InnerException.Message;
                    msg = (ex.InnerException == null) ? ex.Message : innerEx;
                }
            }

            return Json(new { success = stat, message = msg });
        }

        public JsonResult SvRpTrn013(string GenerateNo, string GenerateNoEnd, string SourceData)
        {
            string sql = @"
                select 
                 a.GenerateNo
                ,a.GenerateDate
                ,b.GenerateSeq
                ,case a.SourceData
                  when '0' then 'Internal Faktur Penjualan'
                  when '1' then 'Manual Input'
                  when '2' then 'Sub Dealer / Branches'
                 end as SourceData
                ,a.SenderDealerCode
                ,a.SenderDealerName
                ,a.FpjNo
                ,a.FpjDate
                ,a.FpjGovNo
                ,a.RefferenceNo
                ,a.RefferenceDate
                ,coalesce(b.PaymentNo, '') as PaymentNo
                ,b.PaymentDate
                ,coalesce(b.InvoiceNo,'') as InvoiceNo
                ,b.IssueNo
                ,b.IssueDate
                ,b.ServiceBookNo
                ,b.BasicModel
                ,b.TechnicalModel
                ,b.ChassisCode
                ,b.ChassisNo
                ,b.EngineCode
                ,b.EngineNo
                ,b.RegisteredDate
                ,b.RepairedDate
                ,b.Odometer
                ,b.IsCbu
                ,b.ComplainCode
                ,b.DefectCode
                ,b.CategoryCode
                ,b.OperationNo
                ,b.OperationHour
                ,b.SubletHour
                ,b.OperationAmt
                ,b.SubletAmt
                ,b.PartAmt
                ,b.ClaimAmt
                --,b.ReceivedDate
                --,b.SuzukiRefferenceNo
                --,b.DivisionCode
                --,b.JudgementCode
                --,b.PaymentOprHour
                --,b.PaymentSubletHour
                --,b.PaymentOprAmt
                --,b.PaymentSubletAmt
                ,b.TroubleDescription
                ,b.ProblemExplanation

                from svTrnClaim a
                left join svTrnClaimApplication b on b.CompanyCode = a.CompanyCode
                 and b.BranchCode = a.BranchCode
                 and b.ProductType = a.ProductType
                 and b.GenerateNo = a.GenerateNo
                where 1 = 1
                  and a.CompanyCode = @CompanyCode
                  and a.BranchCode = @BranchCode
                  and a.ProductType = @ProductType
                  and a.GenerateNo between @ClaimNoFrom and @ClaimNoTo
                  and a.SourceData = @sourceData
                order by a.GenerateNo, b.GenerateSeq

                select
                 a.GenerateNo
                ,a.GenerateDate
                ,b.GenerateSeq
                ,a.SenderDealerCode
                ,a.SenderDealerName
                ,a.FpjNo
                ,a.FpjDate
                ,a.FpjGovNo
                ,a.RefferenceNo
                ,a.RefferenceDate
                ,coalesce(b.PaymentNo, '') as PaymentNo
                ,b.PaymentDate
                ,coalesce(b.InvoiceNo,'') as InvoiceNo
                ,b.IssueNo
                ,b.IssueDate
                ,c.PartSeq
                ,case c.IsCausal
                  when '0' then 'No'
                  when '1' then 'Yes'
                 end as IsCausal
                ,c.PartNo
                ,c.ProcessedPartNo
                ,c.Quantity
                ,c.TotalPrice
                ,c.PaymentQuantity
                ,c.PaymentTotalPrice
                from svTrnClaim a
                left join svTrnClaimApplication b on b.CompanyCode = a.CompanyCode
                 and b.BranchCode = a.BranchCode
                 and b.ProductType = a.ProductType
                 and b.GenerateNo = a.GenerateNo
                left join svTrnClaimPart c on c.CompanyCode = b.CompanyCode
                 and c.BranchCode = b.BranchCode
                 and c.ProductType = b.ProductType
                 and c.GenerateNo = b.GenerateNo
                 and c.GenerateSeq = b.GenerateSeq
                where 1 = 1
                  and a.CompanyCode = @CompanyCode
                  and a.BranchCode = @BranchCode
                  and a.ProductType = @ProductType
                  and a.GenerateNo between @ClaimNoFrom and @ClaimNoTo
                  and a.SourceData = @sourceData
                order by a.GenerateNo, b.GenerateSeq, c.PartSeq
                ";

            DataSet ds = new DataSet();
            SqlConnection con = (SqlConnection)ctx.Database.Connection;
            SqlCommand cmd = con.CreateCommand();
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);

            cmd.Parameters.AddWithValue("@ClaimNoFrom", GenerateNo);
            cmd.Parameters.AddWithValue("@ClaimNoTo", GenerateNoEnd);
            cmd.Parameters.AddWithValue("@sourceData", SourceData);
            da.Fill(ds);

            var Sheet = new List<string> { };
            var Prefix = (WarrantyClaimBLL.WarrantyReportSource)Convert.ToInt32(SourceData);

            Sheet.Add(Prefix + "_WARRANTY_HDR");
            Sheet.Add(Prefix + "_WARRANTY_DTL");
            var FileName = "SvRpTrn013";

            return GenerateReportXls(ds, Sheet, FileName);
        }

        #region Private Method
        private DataSet p_GetClaim(string generateNo)
        {
            DataSet ds = new DataSet();
            SqlConnection con = (SqlConnection)ctx.Database.Connection;
            SqlCommand cmd = con.CreateCommand();
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            cmd.CommandText = "uspfn_SvGetClaim";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@GenerateNo", string.IsNullOrEmpty(generateNo) ? "" : generateNo);

            da.Fill(ds);

            return ds;
        }

        private DataSet p_InquiryClaim(string invoiceFrom, string invoiceTo, string branchFrom, string branchTo, string claimType)
        {
            bool isSprClaim = (claimType == "0") ? false : true;
            DataSet ds = new DataSet();
            SqlConnection con = (SqlConnection)ctx.Database.Connection;
            SqlCommand cmd = con.CreateCommand();
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            cmd.CommandText = "uspfn_SvInqGetClaim";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@InvoiceFrom", invoiceFrom);
            cmd.Parameters.AddWithValue("@InvoiceTo", invoiceTo);
            cmd.Parameters.AddWithValue("@BranchFrom", branchFrom);
            cmd.Parameters.AddWithValue("@BranchTo", branchTo);
            cmd.Parameters.AddWithValue("@IsSprClaim", isSprClaim);
            da.Fill(ds);

            return ds;
        }
        #endregion
    }
}
