using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Service.Models;


namespace SimDms.Service.Controllers.Api
{
    public class InputWarrantyClaimController : BaseController
    {
        //
        // GET: /InputWarrantyClaim/

        public JsonResult Default()
        {
            DateTime gDate = Convert.ToDateTime(ctx.CoProfileServices.Find(CompanyCode, BranchCode).PeriodEnd);
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchFrom = BranchCode,
                BranchTo = BranchCode,
                ProductType = ProductType,
                //GenerateDate = new DateTime(gDate.Year, gDate.Month, gDate.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second),
                GenerateDate = DateTime.Now,
                RefferenceDate = DateTime.Now,
                FPJDate = DateTime.Now,
                IssueDate = DateTime.Now,
                RegisteredDate = DateTime.Now,
                RepairedDate = DateTime.Now
            });
        }

        public JsonResult Save(Claim model)
        {
            DateTime gDate = Convert.ToDateTime(model.GenerateDate);
            gDate = new DateTime(gDate.Year, gDate.Month, gDate.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            try
            {
                var dtv = DateTransValidation(Convert.ToDateTime(model.GenerateDate));
                if (dtv != "") throw new Exception(dtv);

                var record = ctx.Claims.Find(CompanyCode, BranchCode, ProductType, model.GenerateNo);

                if (record == null)
                {
                    record = new Claim();
                    record.CompanyCode = CompanyCode;
                    record.BranchCode = BranchCode;
                    record.ProductType = ProductType;
                    record.GenerateNo = GetNewDocumentNo("CLA", model.GenerateDate.Value);
                    record.GenerateDate = gDate;
                    record.CreatedBy = CurrentUser.UserId;
                    record.CreatedDate = DateTime.Now;
                    ctx.Claims.Add(record);
                }

                record.SourceData = "1";
                record.FPJNo = model.FPJNo;
                record.FPJDate = model.FPJDate;
                record.FPJGovNo = model.FPJGovNo;
                record.RefferenceNo = model.RefferenceNo;
                record.RefferenceDate = model.RefferenceDate;

                var coProfile = ctx.CoProfiles.Find(CompanyCode, BranchCode);

                record.ReceiveDealerCode = coProfile.CompanyCode;
                record.SenderDealerCode = model.SenderDealerCode;
                record.SenderDealerName = model.SenderDealerName;
                record.TotalNoOfItem = 0;
                record.LotNo = 0;
                record.PostingFlag = "2";
                record.OtherCompensationAmt = 0;
                record.TotalOperationPayHour = 0;
                record.TotalSubletPayHour = 0;
                record.TotalOperationPaymentAmt = 0;
                record.TotalSubletPaymentAmt = 0;
                record.TotalPartPaymentAmt = 0;
                record.TotalClaimPaymentAmt = 0;
                record.OtherCompensationPaymentAmt = 0;
                record.LastupdateBy = CurrentUser.UserId;
                record.LastupdateDate = DateTime.Now;
                record.IsSparepartClaim = false;

                ctx.SaveChanges();
                return Json(new { success = true, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult SaveClaim(ClaimApplication model)
        {
            var record = new ClaimApplication();

            var laborrate = ctx.SvMstTarifJasas.FirstOrDefault(a => a.CompanyCode == CompanyCode
                && a.BranchCode == BranchCode && a.LaborCode.Equals("SUZUKI")
                && a.IsActive == true && a.EffectiveDate <= DateTime.Now).LaborPrice;

            var claimApplist = ctx.ClaimApplications.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ProductType == ProductType && a.GenerateNo == model.GenerateNo);
            var genSeq = claimApplist.Count() == 0 ? 1 : claimApplist.Max(a => a.GenerateSeq) + 1;

            if (model.IssueNo == null)
            {
                record.CompanyCode = CompanyCode;
                record.BranchCode = BranchCode;
                record.ProductType = ProductType;
                record.CreatedBy = CurrentUser.UserId;
                record.CreatedDate = DateTime.Now;
                record.GenerateNo = model.GenerateNo;
                record.GenerateSeq = genSeq;

                var docIssue = ctx.Documents.Find(CompanyCode, BranchCode, "ISU");
                docIssue.DocumentSequence = docIssue.DocumentSequence + 1;
                record.IssueNo = DateTime.Today.ToString("yyMM") + "-"
                    + docIssue.DocumentSequence.ToString().PadLeft(4, '0') + "-"
                    + BranchCode.Substring(BranchCode.Trim().Length - 2, 2);

                ctx.ClaimApplications.Add(record);
            }
            else
            {
                record = ctx.ClaimApplications.Find(CompanyCode, BranchCode, ProductType, model.GenerateNo, model.GenerateSeq);
            }

            record.IssueDate = model.IssueDate;
            record.ClaimStatus = "0";
            record.ServiceBookNo = model.ServiceBookNo;
            record.BasicModel = model.BasicModel;
            var techModel = ctx.Models.FirstOrDefault(a=>a.CompanyCode==CompanyCode && a.BasicModel == model.BasicModel).TechnicalModelCode;
            record.TechnicalModel = techModel != null ? techModel : "";
            record.ChassisCode = model.ChassisCode;
            record.ChassisNo = model.ChassisNo;
            record.EngineCode = model.EngineCode;
            record.EngineNo = model.EngineNo;
            record.RegisteredDate = model.RegisteredDate;
            record.RepairedDate = model.RepairedDate;
            record.Odometer = model.Odometer;
            record.IsCbu = model.IsCbu;
            record.ComplainCode = model.ComplainCode;
            record.DefectCode = model.DefectCode;
            record.CategoryCode = model.CategoryCode;
            record.OperationNo = model.OperationNo;
            record.OperationHour = model.OperationHour;
            record.SubletHour = model.SubletHour;
            record.OperationAmt = model.OperationHour * laborrate;
            record.SubletAmt = model.SubletHour * laborrate;
            record.TroubleDescription = model.TroubleDescription;
            record.ProblemExplanation = model.ProblemExplanation;
            record.LastupdateBy = CurrentUser.UserId;
            record.LastupdateDate = DateTime.Now;

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult DeleteClaim(ClaimApplication model)
        {
            var record = ctx.ClaimApplications.Find(CompanyCode, BranchCode, ProductType, model.GenerateNo, model.GenerateSeq);

            if (record != null)
            {
                ctx.ClaimApplications.Remove(record);
            }
            try
            {
                ctx.SaveChanges();
                return Json(new { success = true,  message = "Data berhasil dihapus." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult SavePart(ClaimPart model, int tblpart)
        {
            var record = ctx.ClaimParts.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ProductType == ProductType && a.GenerateNo == model.GenerateNo && a.GenerateSeq == model.GenerateSeq && a.PartSeq == model.PartSeq);

            var partseq = ctx.ClaimParts.Where(c => c.CompanyCode == CompanyCode && c.BranchCode == BranchCode && c.ProductType == ProductType && c.GenerateNo == model.GenerateNo && c.GenerateSeq == model.GenerateSeq).Max(a => a.PartSeq);

            if (record == null)
            {
                record = new ClaimPart();
                record.CompanyCode = CompanyCode;
                record.BranchCode = BranchCode;
                record.ProductType = ProductType;
                record.GenerateNo = model.GenerateNo;
                record.GenerateSeq = model.GenerateSeq;
                record.PartSeq = partseq == null ? 1 : partseq + 1;
                record.CreatedBy = CurrentUser.UserId;
                record.CreatedDate = DateTime.Now;
                ctx.ClaimParts.Add(record);
            }

            record.PartNo = model.PartNo;
            record.ProcessedPartNo = model.PartNo;
            record.Quantity = Convert.ToDecimal(model.Quantity);
            record.UnitPrice = GetClaimPartPrice(model.PartNo);
            record.TotalPrice = record.Quantity * record.UnitPrice;
            record.PaymentQuantity = 0;
            record.PaymentTotalPrice = 0;
            record.IsActive = true;
            record.LastupdateBy = CurrentUser.UserId;
            record.LastupdateDate = DateTime.Now;

            if (tblpart > 0)
                record.IsCausal = false;
            else
                record.IsCausal = true;

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult DeletePart(ClaimPart model, int tblpart)
        {
            try
            {

                var record = ctx.ClaimParts.Find(CompanyCode, BranchCode, ProductType, model.GenerateNo, model.GenerateSeq, model.PartSeq);

                if (record != null)
                {
                    if (record.IsCausal == true) {
                        if (tblpart > 1)
                        {
                            throw new Exception("Causal Part tidak boleh dihapus !!");
                        }
                    }
                    ctx.ClaimParts.Remove(record);
                }

                ctx.SaveChanges();
                return Json(new { success = true, message="Data berhasil dihapus."});
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult GetClaimAppData(string generateNo, decimal? generateseq)
        {
            var query = "exec uspfn_SvGetClaimApp {0},{1},{2},{3}";
            object[] parameters = { CompanyCode, BranchCode, ProductType, generateNo };

            var data = ctx.Database.SqlQuery<ClaimAppData>(query, parameters);
            if (generateseq != null) {
                data = data.Where(c => c.GenerateSeq == generateseq);
            }

            return Json(data);
        }

        public JsonResult GetPartData(string generateNo, decimal? generateseq)
        {
            var query = @"select (row_number() over (order by a.IsCausal desc, a.PartNo)) as No,
                    a.IsCausal, 
                    a.PartNo, 
                    a.Quantity, 
                    (select b.partname from spMstItemInfo b where b.PartNo = a.PartNo) as PartName,
                    a.PartSeq,
                    a.UnitPrice
                from
                    SvTrnClaimPart a
               where
	                a.CompanyCode = {0} 
	                and a.BranchCode = {1}
	                and a.ProductType = {2}
	                and a.GenerateNo = {3}
                    and a.GenerateSeq = {4}
                order by a.IsCausal desc, a.PartNo";

            object[] parameters = { CompanyCode, BranchCode, ProductType, generateNo, generateseq };

            var data = ctx.Database.SqlQuery<PartData>(query, parameters);

            return Json(new { data = data, count = data.Count() });
        }

        public JsonResult InformationCost(string generateNo)
        {
            var query = @"
            select 
                (row_number() over (order by BasicModel)) as No,
	            BasicModel ,
	            sum(ClaimAmt) as Total
            from svTrnClaimApplication
            where 
                CompanyCode = {0} and
                BranchCode = {1} and
                ProductType = {2} and
                GenerateNo = {3}
            group by 
	            BasicModel
            ";

            object[] parameters = { CompanyCode, BranchCode, ProductType, generateNo };

            var data = ctx.Database.SqlQuery<InfoCost>(query,parameters);

            return Json(data);
        }



        private decimal GetClaimPartPrice(string partNo)
        {
            DataSet ds = new DataSet();
            SqlConnection con = (SqlConnection)ctx.Database.Connection;
            SqlCommand cmd = con.CreateCommand();
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            Decimal result = 0;
            cmd.CommandText = "uspfn_SvGetClaimPartPrice";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@PartNo", partNo);

            da.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0 && !ds.Tables[0].Rows[0]["ClaimPrice"].Equals(DBNull.Value))
            {
                result = Convert.ToDecimal(ds.Tables[0].Rows[0]["ClaimPrice"]);
            }
            return result;
        }

        public JsonResult ValidatePart(ClaimPart model)
        {
            var claimPart = ctx.ClaimParts.Where(c => c.CompanyCode == CompanyCode && c.BranchCode == BranchCode && c.ProductType == ProductType && c.GenerateNo == model.GenerateNo && c.GenerateSeq == model.GenerateSeq && c.PartNo == model.PartNo);

            return Json(claimPart.Count());
        }
    }
}
