using SimDms.Tax.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Tax.Controllers.Api
{
    public class ConsolidationTaxOutController : BaseController
    {
        [HttpPost]
        public JsonResult Default()
        {
            return Json(new
            {
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year
            });

        }

        public JsonResult GetTaxOut(int periodYear, int periodMonth)
        {
            var isFPJCentral = IsFPJCentral();
            var data = ctx.GnTaxOuts.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == (isFPJCentral ? a.BranchCode : BranchCode)
                     && a.ProductType == ProductType && a.PeriodYear == periodYear && a.PeriodMonth == periodMonth).OrderBy(a => a.SeqNo);
            
            return Json(data);
        }
        
        public JsonResult GetGrandTotal(int periodYear, int periodMonth)
        {
            object[] parameters = { CompanyCode, (IsFPJCentral()) ? "" : BranchCode, ProductType, periodYear, periodMonth };

            string query = "";

            if (IsFPJCentral())
            {
                query = @"
                select  isnull(sum(a.DPPAmt),0) DPPAmt
                        ,isnull(sum(a.PPNAmt),0) PPNAmt
                        ,(
                            select isnull(sum(DPPAmt),0)
                            from	gnTaxOut WITH(NOLOCK, NOWAIT)
                            where	CompanyCode= a.CompanyCode
	                                and ProductType= a.ProductType
	                                and PeriodYear= a.PeriodYear
	                                and PeriodMonth= a.PeriodMonth
	                                and IsPKP= 1
                        ) SumDPPStd
                        ,(
                            select isnull(sum(DPPAmt),0)
                            from	gnTaxOut WITH(NOLOCK, NOWAIT)
                            where	CompanyCode= a.CompanyCode
	                                and ProductType= a.ProductType
	                                and PeriodYear= a.PeriodYear
	                                and PeriodMonth= a.PeriodMonth
	                                and IsPKP= 0
                        ) SumDPPSdh
                        ,(
                            select	isnull(sum(PPNAmt),0)
                            from	gnTaxOut WITH(NOLOCK, NOWAIT)
                            where	CompanyCode= a.CompanyCode
	                                and ProductType= a.ProductType
	                                and PeriodYear= a.PeriodYear
	                                and PeriodMonth= a.PeriodMonth
	                                and IsPKP= 1
                        ) SumPPNStd
                        ,(
                            select	isnull(sum(PPNAmt),0)
                            from	gnTaxOut WITH(NOLOCK, NOWAIT)
                            where	CompanyCode= a.CompanyCode
	                                and ProductType= a.ProductType
	                                and PeriodYear= a.PeriodYear
	                                and PeriodMonth= a.PeriodMonth
	                                and IsPKP= 0
                        ) SumPPNSdh
                        ,isnull(sum(a.PPNBmAmt),0) SumPPnBMAmt
                from	gnTaxOut a WITH(NOLOCK, NOWAIT)
                where	a.CompanyCode= @p0
		                and a.BranchCode= case @p1 when '' then a.BranchCode else @p1 end 
		                and a.ProductType= @p2
		                and a.PeriodYear= @p3
		                and a.PeriodMonth= @p4
                group by
		                a.CompanyCode,a.ProductType,a.PeriodYear,a.PeriodMonth";


            }
            else
            {
                query = @"
                select  isnull(sum(a.DPPAmt),0) DPPAmt
                        ,isnull(sum(a.PPNAmt),0) PPNAmt
                        ,(
                            select isnull(sum(DPPAmt),0)
                            from	gnTaxOut WITH(NOLOCK, NOWAIT)
                            where	CompanyCode= a.CompanyCode
	                                and BranchCode= a.BranchCode
	                                and ProductType= a.ProductType
	                                and PeriodYear= a.PeriodYear
	                                and PeriodMonth= a.PeriodMonth
	                                and IsPKP= 1
                        ) SumDPPStd
                        ,(
                            select isnull(sum(DPPAmt),0)
                            from	gnTaxOut WITH(NOLOCK, NOWAIT)
                            where	CompanyCode= a.CompanyCode
	                                and BranchCode= a.BranchCode
	                                and ProductType= a.ProductType
	                                and PeriodYear= a.PeriodYear
	                                and PeriodMonth= a.PeriodMonth
	                                and IsPKP= 0
                        ) SumDPPSdh
                        ,(
                            select	isnull(sum(PPNAmt),0)
                            from	gnTaxOut WITH(NOLOCK, NOWAIT)
                            where	CompanyCode= a.CompanyCode
	                                and BranchCode= a.BranchCode
	                                and ProductType= a.ProductType
	                                and PeriodYear= a.PeriodYear
	                                and PeriodMonth= a.PeriodMonth
	                                and IsPKP= 1
                        ) SumPPNStd
                        ,(
                            select	isnull(sum(PPNAmt),0)
                            from	gnTaxOut WITH(NOLOCK, NOWAIT)
                            where	CompanyCode= a.CompanyCode
	                                and BranchCode= a.BranchCode
	                                and ProductType= a.ProductType
	                                and PeriodYear= a.PeriodYear
	                                and PeriodMonth= a.PeriodMonth
	                                and IsPKP= 0
                        ) SumPPNSdh
                        ,isnull(sum(a.PPNBmAmt),0) SumPPnBMAmt
                from	gnTaxOut a WITH(NOLOCK, NOWAIT)
                where	a.CompanyCode= @p0
		                and a.BranchCode= case @p1 when '' then a.BranchCode else @p1 end 
		                and a.ProductType= @p2
		                and a.PeriodYear= @p3
		                and a.PeriodMonth= @p4
                group by
		                a.CompanyCode,a.BranchCode,a.ProductType,a.PeriodYear,a.PeriodMonth";
            }

            var data = ctx.Database.SqlQuery<GetGrandTotal>(query, parameters);
            return Json(data);
        }
        
        public JsonResult Query(int PeriodYear, int PeriodMonth)
        {
            DataSet ds = new DataSet();
           
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "usprpt_GnInquiryTaxOut";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 3600;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", IsFPJCentral() ? "" : BranchCode);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@PeriodYear", PeriodYear);
            cmd.Parameters.AddWithValue("@PeriodMonth", PeriodMonth);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count <= 0)
            {
                var msg = "Tidak ada data yang ditampilkan";
                return Json(new { success = false, message = msg }); ;
            }
            else
            {
                return Json(new { success = true, taxOut = ds.Tables[0], grandTotal = ds.Tables[1] }); ;
            }

        }

        public JsonResult Save(GetTaxOut model)
        {
            bool periodExist = PeriodExsist((int)model.PeriodYear, (int)model.PeriodMonth, IsFPJCentral());
            string errMsg = string.Empty;
            string msg = string.Empty;
            int countNewData = 0;

            if (periodExist == model.RePosting)
            {
                if (PostingAll((int)model.PeriodYear, (int)model.PeriodMonth, ref errMsg, ref countNewData, periodExist, IsFPJCentral()))
                {
                    var grandTotal = GetGrandTotal((int)model.PeriodYear, (int)model.PeriodMonth);
                    msg = string.Format("Proses Transfer Pajak Berhasil \nSebanyak {0} data berhasil ditambahkan",
                        countNewData);
                    return Json(new { success = true, message = msg });
                }
                else
                    msg = errMsg;
            }
            else
            {
                if (periodExist)
                    msg ="Data Pajak sudah pernah di-posting, Save dapat dilanjutkan dengan Re-Posting";
                else
                    msg ="Tidak dapat Re-Posting, Data Pajak belum pernah di-posting";
            }

            return Json(new {success = false, message = msg});
        }

        public JsonResult ValidateDetail(GetTaxOut model)
        {

            var recordTax = ctx.GnTaxOuts.Find(model.CompanyCode, model.BranchCode, model.ProductType,
                model.PeriodYear, model.PeriodMonth, model.SeqNo);

            if (recordTax == null)
            {
                var msg = "Perubahan tidak bisa dilakukan, adanya perubahan basis data";
                return Json(new { success = false, message = msg});
            }

            return Json(new { success = true });
        }

        public JsonResult SaveDetail(GetTaxOut model)
        {
            bool result = false;
            string msg = "";
            GnTaxOutHistory recordHistory = ctx.GnTaxOutHistories.Find(model.CompanyCode, model.BranchCode,
                    model.ProductType, model.PeriodYear, model.PeriodMonth, model.SeqNo);
            GnTaxOut recordTaxOut = ctx.GnTaxOuts.Find(model.CompanyCode, model.BranchCode, model.ProductType,
                        model.PeriodYear, model.PeriodMonth, model.SeqNo);

            if (recordTaxOut != null)
            {
                if (recordHistory == null)
                {
                    recordHistory = new GnTaxOutHistory()
                    {
                        CompanyCode = recordTaxOut.CompanyCode,
                        BranchCode = recordTaxOut.BranchCode,
                        ProductType = recordTaxOut.ProductType,
                        PeriodYear = recordTaxOut.PeriodYear,
                        PeriodMonth = recordTaxOut.PeriodMonth,
                        SeqNo = recordTaxOut.SeqNo,
                        ProfitCenter = recordTaxOut.ProfitCenter,
                        TypeOfGoods = recordTaxOut.TypeOfGoods,
                        TaxCode = recordTaxOut.TaxCode,
                        TransactionCode = recordTaxOut.TransactionCode,
                        StatusCode = recordTaxOut.StatusCode,
                        DocumentCode = recordTaxOut.DocumentCode,
                        DocumentType = recordTaxOut.DocumentType,
                        CustomerCode = recordTaxOut.CustomerCode,
                        CustomerName = recordTaxOut.CustomerName,
                        IsPKP = recordTaxOut.IsPKP,
                        NPWP = recordTaxOut.NPWP,
                        FPJNo = recordTaxOut.FPJNo,
                        FPJDate = recordTaxOut.FPJDate,
                        ReferenceNo = recordTaxOut.ReferenceNo,
                        ReferenceDate = recordTaxOut.ReferenceDate,
                        TaxNo = recordTaxOut.TaxNo,
                        TaxDate = recordTaxOut.TaxDate,
                        SubmissionDate = recordTaxOut.SubmissionDate,
                        DPPAmt = recordTaxOut.DPPAmt,
                        PPNAmt = recordTaxOut.PPNAmt,
                        PPNBmAmt = recordTaxOut.PPNBmAmt,
                        Description = recordTaxOut.Description,
                        Quantity = recordTaxOut.Quantity,
                        IsDeleted = false,
                        CreatedBy = CurrentUser.UserId,
                        CreatedDate = DateTime.Now
                    };
                    ctx.GnTaxOutHistories.Add(recordHistory);
                }
                else
                {
                    msg = string.Format("Data pajak dengan no. referensi {0} belum tersimpan", model.ReferenceNo);
                    return Json(new { success = false, message = msg });
                }
            }
            recordHistory.LastupdateBy = CurrentUser.UserId;
            recordHistory.LastupdateDate = DateTime.Now;

            result = ctx.SaveChanges() > 0;
            msg = result ? string.Empty : "Data pajak tidak dapat disimpan di Daftar Riwayat Pajak Masukan";

            if (result)
            {
                recordTaxOut.LastupdateBy = CurrentUser.UserId;
                recordTaxOut.LastupdateDate = DateTime.Now;
                result = ctx.SaveChanges() > 0;

                msg = result ? string.Empty : string.IsNullOrEmpty(msg) ?
                    "Data pajak tidak dapat disimpan di Daftar Pajak Masukan" : msg;

                // Update PPN GnTaxPPN
                if (result)
                {
                    result = RecalculatePPN(model.CompanyCode, model.BranchCode, model.ProductType, (int)model.PeriodYear, (int)model.PeriodMonth, model.ProfitCenter, model.DocumentType);
                    // Update GrandTotal gnTaxPPN
                    if (result)
                    {
                        result = UpdateTotal(model.CompanyCode, (IsFPJCentral() ? "" : model.BranchCode), model.ProductType, (int)model.PeriodYear, (int)model.PeriodMonth);
                        if (!result) msg = "Perhitungan GrandTotal gagal disimpan";
                    }
                    else
                        msg = "Perhitungan PPN gagal disimpan";
                }
            }
            return Json(new { success = result, message = msg });
        }

        public JsonResult DeleteDetail(GetTaxIn model)
        {
            var msg = "";
            var result = false;
            // Insert/Update GnTaxInHistory
            GnTaxOutHistory recordHistory = ctx.GnTaxOutHistories.Find(model.CompanyCode, model.BranchCode,
                model.ProductType, model.PeriodYear, model.PeriodMonth, model.SeqNo);
            GnTaxOut recordTaxOut = ctx.GnTaxOuts.Find(model.CompanyCode, model.BranchCode, model.ProductType, model.PeriodYear, model.PeriodMonth, model.SeqNo);

            if (recordHistory == null)
            {
                recordHistory = new GnTaxOutHistory();

                if (recordTaxOut != null)
                {
                    recordHistory.CompanyCode = recordTaxOut.CompanyCode;
                    recordHistory.BranchCode = recordTaxOut.BranchCode;
                    recordHistory.ProductType = recordTaxOut.ProductType;
                    recordHistory.PeriodYear = recordTaxOut.PeriodYear;
                    recordHistory.PeriodMonth = recordTaxOut.PeriodMonth;
                    recordHistory.SeqNo = recordTaxOut.SeqNo;
                    recordHistory.ProfitCenter = recordTaxOut.ProfitCenter;
                    recordHistory.TypeOfGoods = recordTaxOut.TypeOfGoods;
                    recordHistory.TaxCode = recordTaxOut.TaxCode;
                    recordHistory.TransactionCode = recordTaxOut.TransactionCode;
                    recordHistory.StatusCode = recordTaxOut.StatusCode;
                    recordHistory.DocumentCode = recordTaxOut.DocumentCode;
                    recordHistory.DocumentType = recordTaxOut.DocumentType;
                    recordHistory.CustomerCode = recordTaxOut.CustomerCode;
                    recordHistory.CustomerName = recordTaxOut.CustomerName;
                    recordHistory.IsPKP = recordTaxOut.IsPKP;
                    recordHistory.NPWP = recordTaxOut.NPWP;
                    recordHistory.FPJNo = recordTaxOut.FPJNo;
                    recordHistory.FPJDate = recordTaxOut.FPJDate;
                    recordHistory.ReferenceNo = recordTaxOut.ReferenceNo;
                    recordHistory.ReferenceDate = recordTaxOut.ReferenceDate;
                    recordHistory.TaxNo = recordTaxOut.TaxNo;
                    recordHistory.TaxDate = recordTaxOut.TaxDate;
                    recordHistory.SubmissionDate = recordTaxOut.SubmissionDate;
                    recordHistory.DPPAmt = recordTaxOut.DPPAmt;
                    recordHistory.PPNAmt = recordTaxOut.PPNAmt;
                    recordHistory.PPNBmAmt = recordTaxOut.PPNBmAmt;
                    recordHistory.Description = recordTaxOut.Description;
                    recordHistory.Quantity = recordTaxOut.Quantity;
                    recordHistory.CreatedBy = CurrentUser.UserId;
                    recordHistory.CreatedDate = DateTime.Now;
                    ctx.GnTaxOutHistories.Add(recordHistory);
                }
                else
                {
                    msg = string.Format("Data pajak dengan no. referensi {0} belum tersimpan", model.ReferenceNo);
                    return Json(new { success = false, message = msg });
                }
            }

            recordHistory.IsDeleted = true;
            recordHistory.LastupdateBy = CurrentUser.UserId;
            recordHistory.LastupdateDate = DateTime.Now;

            result = ctx.SaveChanges() > 0;

            msg = result ? string.Empty : "Data pajak tidak dapat disimpan di Daftar Riwayat Pajak Masukan";

            // Delete GnTaxIn
            if (result)
            {
                ctx.GnTaxOuts.Remove(recordTaxOut);
                result = ctx.SaveChanges() > 0;

                msg = result ? string.Empty : "Data pajak tidak dapat dihapus di Daftar Pajak Masukan";

                // Update PPN GnTaxPPN
                if (result)
                {
                    result = RecalculatePPN(model.CompanyCode, model.BranchCode, model.ProductType, (int)model.PeriodYear, (int)model.PeriodMonth, model.ProfitCenter, model.DocumentType);

                    // Update GrandTotal gnTaxPPN
                    if (result)
                    {
                        result = UpdateTotal(model.CompanyCode, (IsFPJCentral() ? "" : model.BranchCode), model.ProductType, (int)model.PeriodYear, (int)model.PeriodMonth);
                        if (!result) msg = "Perhitungan GrandTotal gagal disimpan";
                    }
                    else
                        msg = "Perhitungan PPN gagal disimpan";
                }
            }

            return Json(new { success = result, message = msg });
        }

        public bool PostingAll(int periodYear, int periodMonth, ref string msg, ref int countNewData, bool periodExist, bool isFPJCentral)
        {
            bool result = false;
            try
            {
                int countOldData = CountByPeriod(periodYear, periodMonth, isFPJCentral);

                var seqNoTaxOut = ctx.GnTaxOuts.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == (isFPJCentral ? a.BranchCode : BranchCode)
                     && a.ProductType == ProductType && a.PeriodYear == periodYear && a.PeriodMonth == periodMonth).Max(a => a.SeqNo) ?? 0;

                var seqNoTaxOutHst = ctx.GnTaxOutHistories.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == (isFPJCentral ? a.BranchCode : BranchCode)
                     && a.ProductType == ProductType && a.PeriodYear == periodYear && a.PeriodMonth == periodMonth && a.IsDeleted == true).Max(a => a.SeqNo) ?? 0;

                var lastSeqNo = (seqNoTaxOut > seqNoTaxOutHst) ? seqNoTaxOut : seqNoTaxOutHst;

                object[] postingParams = { CompanyCode, (IsFPJCentral() ? "" : BranchCode), periodMonth, periodYear, ProductType, lastSeqNo, CurrentUser.UserId };
                var postingQuery = "exec usprpt_GnPostingTaxOut @p0,@p1,@p2,@p3,@p4,@p5,@p6";

                result = ctx.Database.ExecuteSqlCommand(postingQuery, postingParams) >= 0 ? true : false;

                if (result)
                {
                    countNewData = CountByPeriod(periodYear, periodMonth, isFPJCentral);
                    DateTime date = new DateTime(periodYear, periodMonth, 1);
                    countNewData -= countOldData;

                    object[] PPnParams = { CompanyCode, BranchCode, ProductType, CurrentUser.UserId, date };
                    var PPnQuery = IsFPJCentral() ? "exec usprpt_GnInsertTaxPPNWoBranch @p0,@p1,@p2,@p3,@p4" : "exec usprpt_GnInsertTaxPPN @p0,@p1,@p2,@p3,@p4";
                    try
                    {
                        result = ctx.Database.ExecuteSqlCommand(PPnQuery, PPnParams) >= 0 ? true : false;
                    }
                    catch
                    {
                        result = false;
                    }


                    if (result)
                    {
                        if (periodExist && countNewData > 0)
                        {
                            var newRecord = ctx.GnTaxOuts.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == (isFPJCentral ? a.BranchCode : BranchCode)
                        && a.ProductType == ProductType && a.PeriodYear == periodYear && a.PeriodMonth == periodMonth)
                        .Select(c => new ConsolidationTaxNewRecord { BranchCode = c.BranchCode, ProfitCenter = c.ProfitCenter, DocumentType = c.DocumentType }).Take(countNewData);

                            
                            foreach (var row in newRecord)
                            {
                                result = RecalculatePPN(CompanyCode, row.BranchCode, ProductType, periodYear, periodMonth, row.ProfitCenter, row.DocumentType);
                                  
                                if (!result)
                                {
                                    msg = "Proses update PPN gagal";
                                    break;
                                }
                            }
                        }

                        if (result)
                        {
                            result = UpdateTotal(CompanyCode, (IsFPJCentral() ? "" : BranchCode), ProductType, periodYear, periodMonth);
                            if (!result) msg = "Proses update Grand Total gagal";
                        }
                    }
                    else
                        msg = "Proses save PPN gagal";
                }
                else
                    msg = "Proses save Pajak Masukan gagal";
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        private bool IsFPJCentral()
        {
            return ctx.CoProfiles.Where(c => c.CompanyCode == CompanyCode)
                .Join(ctx.OrganizationDtls.Where(d => d.IsBranch == false), x => new { x.CompanyCode, x.BranchCode }, y => new { y.CompanyCode, y.BranchCode }, (x, y) => new { x, y }).Select(a => a.x.IsFPJCentralized.Value).FirstOrDefault();
        }

        private bool PeriodExsist(int periodYear, int periodMonth, bool isFPJCentral)
        {
            var data = ctx.GnTaxOuts.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == (isFPJCentral ? a.BranchCode : BranchCode) && a.PeriodYear == periodYear && a.PeriodMonth == periodMonth && a.ProductType == ProductType).Take(1);

            return data.Count() == 1 ? true : false;
        }

        private int CountByPeriod(int periodYear, int periodMonth, bool isFPJCentral)
        {
            return ctx.GnTaxOuts.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == (isFPJCentral ? a.BranchCode : BranchCode)
                 && a.ProductType == ProductType && a.PeriodYear == periodYear && a.PeriodMonth == periodMonth).Count();
        }
       
        private bool RecalculatePPN(string companyCode, string branchCode, string productType, int year, int month, string profitCenter, string docType)
        {
            var result = false;
            object[] recPPnParams = { companyCode, branchCode, productType, year, month, profitCenter, docType };
            var recPPnQuery = "exec usprpt_GnRecalculatePPN @p0,@p1,@p2,@p3,@p4,@p5,@p6";
            result = ctx.Database.ExecuteSqlCommand(recPPnQuery, recPPnParams) >= 0 ? true : false;

            return result;
        }

        private bool UpdateTotal(string companyCode, string branchCode, string productType, int year, int month)
        {
            var result = false;
            DateTime date = new DateTime(year, month, 1);
            object[] updateParams = { companyCode, (IsFPJCentral() ? "" : branchCode), productType, date };
            var updateQuery = "exec uspfn_TaxUpdateTotal @p0,@p1,@p2,@p3";
            result = ctx.Database.ExecuteSqlCommand(updateQuery, updateParams) >= 0 ? true : false;
            return result;
        }
    
    }
}
