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
    public class ConsolidationTaxInController : BaseController
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

        public JsonResult GetTaxIn(int periodYear, int periodMonth)
        {
            var query = string.Format(@"SELECT ROW_NUMBER() OVER(ORDER BY a.SeqNo ASC) RowNumber, 
                                	a.CompanyCode,
                                	a.BranchCode,
                                	a.ProductType,
                                	(select lookupvaluename from gnMstLookUpDtl b where b.LookUpValue = a.ProductType and b.CodeID ='PRDT' ) ProductDesc,
                                	a.PeriodYear,a.PeriodMonth,a.SeqNo,a.ProfitCenter,a.TypeOfGoods,
                                	(select lookupvaluename from gnMstLookUpDtl b where b.LookUpValue = a.TypeOfGoods and b.CodeID ='TPGO' )TypeOfGoodsDesc,
                                	a.TaxCode,
                                	(select lookupvaluename from gnMstLookUpDtl b where b.LookUpValue = a.TaxCode and b.CodeID ='TXCI' ) TaxDesc,
                                	a.TransactionCode,
                                	(select lookupvaluename from gnMstLookUpDtl b where b.LookUpValue = a.TransactionCode and b.CodeID ='TRCI' ) TransactionDesc,
                                	a.StatusCode,
                                	(select lookupvaluename from gnMstLookUpDtl b where b.LookUpValue = a.StatusCode and b.CodeID ='TSCI' ) StatusDesc,
                                	a.DocumentCode,
                                	(select lookupvaluename from gnMstLookUpDtl b where b.LookUpValue = a.DocumentCode and b.CodeID ='TDCD' ) DocumentDesc,
                                	a.DocumentType, a.SupplierCode, a.SupplierName, a.IsPKP, a.NPWP, a.FPJNo, a.FPJDate, a.ReferenceNo, a.ReferenceDate,
                                	a.TaxNo, a.TaxDate, a.SubmissionDate, a.DPPAmt, a.PPNAmt, a.PPNBmAmt, a.Description, a.Quantity 
                                FROM
                                	GnTaxIn a
                                WHERE
                                	CompanyCode = '{0}'
                                	AND	BranchCode = case '{1}' when '' then BranchCode else '{1}' end 
                                	AND ProductType = '{2}'
                                	AND PeriodYear = {3}
                                	AND PeriodMonth = {4}
                                ORDER BY SeqNo ASC", CompanyCode, (IsFPJCentral()) ? "" : BranchCode, ProductType, periodYear, periodMonth);

            var data = ctx.Database.SqlQuery<GetTaxIn>(query);

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
                            from	gnTaxIn WITH(NOLOCK, NOWAIT)
			                where	CompanyCode= a.CompanyCode					                
					                and ProductType= a.ProductType
					                and PeriodYear= a.PeriodYear
					                and PeriodMonth= a.PeriodMonth
					                and IsPKP= 1
                        ) SumDPPStd
                        ,(
                            select isnull(sum(DPPAmt),0)
                            from	gnTaxIn WITH(NOLOCK, NOWAIT)
			                where	CompanyCode= a.CompanyCode
					                and ProductType= a.ProductType
					                and PeriodYear= a.PeriodYear
					                and PeriodMonth= a.PeriodMonth
					                and IsPKP= 0
                        ) SumDPPSdh
		                ,(
			                select	isnull(sum(PPNAmt),0)
			                from	gnTaxIn WITH(NOLOCK, NOWAIT)
			                where	CompanyCode= a.CompanyCode
					                and ProductType= a.ProductType
					                and PeriodYear= a.PeriodYear
					                and PeriodMonth= a.PeriodMonth
					                and IsPKP= 1
		                ) SumPPNStd
		                ,(
			                select	isnull(sum(PPNAmt),0)
			                from	gnTaxIn WITH(NOLOCK, NOWAIT)
			                where	CompanyCode= a.CompanyCode
					                and ProductType= a.ProductType
					                and PeriodYear= a.PeriodYear
					                and PeriodMonth= a.PeriodMonth
					                and IsPKP= 0
		                ) SumPPNSdh
		                ,isnull(sum(a.PPNBmAmt),0) SumPPnBMAmt
                from	gnTaxIn a WITH(NOLOCK, NOWAIT)
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
                            from	gnTaxIn WITH(NOLOCK, NOWAIT)
			                where	CompanyCode= a.CompanyCode
					                and BranchCode= a.BranchCode
					                and ProductType= a.ProductType
					                and PeriodYear= a.PeriodYear
					                and PeriodMonth= a.PeriodMonth
					                and IsPKP= 1
                        ) SumDPPStd
                        ,(
                            select isnull(sum(DPPAmt),0)
                            from	gnTaxIn WITH(NOLOCK, NOWAIT)
			                where	CompanyCode= a.CompanyCode
					                and BranchCode= a.BranchCode
					                and ProductType= a.ProductType
					                and PeriodYear= a.PeriodYear
					                and PeriodMonth= a.PeriodMonth
					                and IsPKP= 0
                        ) SumDPPSdh
		                ,(
			                select	isnull(sum(PPNAmt),0)
			                from	gnTaxIn WITH(NOLOCK, NOWAIT)
			                where	CompanyCode= a.CompanyCode
					                and BranchCode= a.BranchCode
					                and ProductType= a.ProductType
					                and PeriodYear= a.PeriodYear
					                and PeriodMonth= a.PeriodMonth
					                and IsPKP= 1
		                ) SumPPNStd
		                ,(
			                select	isnull(sum(PPNAmt),0)
			                from	gnTaxIn WITH(NOLOCK, NOWAIT)
			                where	CompanyCode= a.CompanyCode
					                and BranchCode= a.BranchCode
					                and ProductType= a.ProductType
					                and PeriodYear= a.PeriodYear
					                and PeriodMonth= a.PeriodMonth
					                and IsPKP= 0
		                ) SumPPNSdh
		                ,isnull(sum(a.PPNBmAmt),0) SumPPnBMAmt
                from	gnTaxIn a WITH(NOLOCK, NOWAIT)
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
            //object[] parameters = { CompanyCode, BranchCode, ProductType, Year, Month };
            //var query = "exec uspfn_GetTaxInPeriod @p0,@p1,@p2,@p3,@p4";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_GetTaxInPeriod";
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
                return Json(new { success = true, taxIn = ds.Tables[0], grandTotal = ds.Tables[1] }); ;
            }

        }

        public JsonResult Save(GetTaxIn model)
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

        public JsonResult ValidateDetail(GetTaxIn model)
        {

            var recordTax = ctx.gnTaxIn.Find(model.CompanyCode, model.BranchCode, model.ProductType,
                model.PeriodYear, model.PeriodMonth, model.SeqNo);

            if (recordTax == null)
            {
                var msg = "Perubahan tidak bisa dilakukan, adanya perubahan basis data";
                return Json(new { success = false, message = msg});
            }

            return Json(new { success = true });
        }

        public JsonResult SaveDetail(GetTaxIn model)
        {
            bool result = false;
            string msg = "";
            GnTaxInHistory recordHistory = ctx.GnTaxInHistories.Find(model.CompanyCode, model.BranchCode,
                    model.ProductType, model.PeriodYear, model.PeriodMonth, model.SeqNo);
            gnTaxIn recordTaxIn = ctx.gnTaxIn.Find(model.CompanyCode, model.BranchCode, model.ProductType,
                        model.PeriodYear, model.PeriodMonth, model.SeqNo);

            if (recordTaxIn != null)
            {
                if (recordHistory == null)
                {
                    recordHistory = new GnTaxInHistory()
                    {
                        CompanyCode = recordTaxIn.CompanyCode,
                        BranchCode = recordTaxIn.BranchCode,
                        ProductType = recordTaxIn.ProductType,
                        PeriodYear = recordTaxIn.PeriodYear,
                        PeriodMonth = recordTaxIn.PeriodMonth,
                        SeqNo = recordTaxIn.SeqNo,
                        ProfitCenter = recordTaxIn.ProfitCenter,
                        TypeOfGoods = recordTaxIn.TypeOfGoods,
                        TaxCode = recordTaxIn.TaxCode,
                        TransactionCode = recordTaxIn.TransactionCode,
                        StatusCode = recordTaxIn.StatusCode,
                        DocumentCode = recordTaxIn.DocumentCode,
                        DocumentType = recordTaxIn.DocumentType,
                        SupplierCode = recordTaxIn.SupplierCode,
                        SupplierName = recordTaxIn.SupplierName,
                        IsPKP = recordTaxIn.IsPKP,
                        NPWP = recordTaxIn.NPWP,
                        FPJNo = recordTaxIn.FPJNo,
                        FPJDate = recordTaxIn.FPJDate,
                        ReferenceNo = recordTaxIn.ReferenceNo,
                        ReferenceDate = recordTaxIn.ReferenceDate,
                        TaxNo = recordTaxIn.TaxNo,
                        TaxDate = recordTaxIn.TaxDate,
                        SubmissionDate = recordTaxIn.SubmissionDate,
                        DPPAmt = recordTaxIn.DPPAmt,
                        PPNAmt = recordTaxIn.PPNAmt,
                        PPNBmAmt = recordTaxIn.PPNBmAmt,
                        Description = recordTaxIn.Description,
                        Quantity = recordTaxIn.Quantity,
                        IsDeleted = false,
                        CreatedBy = CurrentUser.UserId,
                        CreatedDate = DateTime.Now
                    };
                    ctx.GnTaxInHistories.Add(recordHistory);
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
                recordTaxIn.LastupdateBy = CurrentUser.UserId;
                recordTaxIn.LastupdateDate = DateTime.Now;
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
            GnTaxInHistory recordHistory = ctx.GnTaxInHistories.Find(model.CompanyCode, model.BranchCode,
                model.ProductType, model.PeriodYear, model.PeriodMonth, model.SeqNo);
            gnTaxIn recordTaxIn = ctx.gnTaxIn.Find(model.CompanyCode, model.BranchCode, model.ProductType, model.PeriodYear, model.PeriodMonth, model.SeqNo);

            if (recordHistory == null)
            {
                recordHistory = new GnTaxInHistory();
               
                if (recordTaxIn != null)
                {
                    recordHistory.CompanyCode = recordTaxIn.CompanyCode;
                    recordHistory.BranchCode = recordTaxIn.BranchCode;
                    recordHistory.ProductType = recordTaxIn.ProductType;
                    recordHistory.PeriodYear = recordTaxIn.PeriodYear;
                    recordHistory.PeriodMonth = recordTaxIn.PeriodMonth;
                    recordHistory.SeqNo = recordTaxIn.SeqNo;
                    recordHistory.ProfitCenter = recordTaxIn.ProfitCenter;
                    recordHistory.TypeOfGoods = recordTaxIn.TypeOfGoods;
                    recordHistory.TaxCode = recordTaxIn.TaxCode;
                    recordHistory.TransactionCode = recordTaxIn.TransactionCode;
                    recordHistory.StatusCode = recordTaxIn.StatusCode;
                    recordHistory.DocumentCode = recordTaxIn.DocumentCode;
                    recordHistory.DocumentType = recordTaxIn.DocumentType;
                    recordHistory.SupplierCode = recordTaxIn.SupplierCode;
                    recordHistory.SupplierName = recordTaxIn.SupplierName;
                    recordHistory.IsPKP = recordTaxIn.IsPKP;
                    recordHistory.NPWP = recordTaxIn.NPWP;
                    recordHistory.FPJNo = recordTaxIn.FPJNo;
                    recordHistory.FPJDate = recordTaxIn.FPJDate;
                    recordHistory.ReferenceNo = recordTaxIn.ReferenceNo;
                    recordHistory.ReferenceDate = recordTaxIn.ReferenceDate;
                    recordHistory.TaxNo = recordTaxIn.TaxNo;
                    recordHistory.TaxDate = recordTaxIn.TaxDate;
                    recordHistory.SubmissionDate = recordTaxIn.SubmissionDate;
                    recordHistory.DPPAmt = recordTaxIn.DPPAmt;
                    recordHistory.PPNAmt = recordTaxIn.PPNAmt;
                    recordHistory.PPNBmAmt = recordTaxIn.PPNBmAmt;
                    recordHistory.Description = recordTaxIn.Description;
                    recordHistory.Quantity = recordTaxIn.Quantity;
                    recordHistory.CreatedBy = CurrentUser.UserId;
                    recordHistory.CreatedDate = DateTime.Now;
                    ctx.GnTaxInHistories.Add(recordHistory);
                }
                else
                {
                    msg = string.Format("Data pajak dengan no. referensi {0} belum tersimpan", model.ReferenceNo);
                    return Json(new { success = false, message = msg});
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
                ctx.gnTaxIn.Remove(recordTaxIn);
                result = ctx.SaveChanges() > 0;

                msg = result ? string.Empty : "Data pajak tidak dapat dihapus di Daftar Pajak Masukan";

                // Update PPN GnTaxPPN
                if (result)
                {
                    result = RecalculatePPN(model.CompanyCode, model.BranchCode, model.ProductType, (int)model.PeriodYear, (int)model.PeriodMonth, model.ProfitCenter, model.DocumentType);

                    // Update GrandTotal gnTaxPPN
                    if (result)
                    {
                        result = UpdateTotal(model.CompanyCode, (IsFPJCentral() ? "" : model.BranchCode), model.ProductType,(int)model.PeriodYear, (int)model.PeriodMonth);
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

                var seqNoTaxIn = ctx.gnTaxIn.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == (isFPJCentral ? a.BranchCode : BranchCode)
                     && a.ProductType == ProductType && a.PeriodYear == periodYear && a.PeriodMonth == periodMonth).Max(a => a.SeqNo) ?? 0;

                var seqNoTaxInHst = ctx.GnTaxInHistories.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == (isFPJCentral ? a.BranchCode : BranchCode)
                     && a.ProductType == ProductType && a.PeriodYear == periodYear && a.PeriodMonth == periodMonth && a.IsDeleted == true).Max(a => a.SeqNo) ?? 0;

                var lastSeqNo = (seqNoTaxIn > seqNoTaxInHst) ? seqNoTaxIn : seqNoTaxInHst;

                object[] postingParams = { CompanyCode, (IsFPJCentral() ? "" : BranchCode), ProductType, periodYear, periodMonth, lastSeqNo, CurrentUser.UserId };
                var postingQuery = "exec uspfn_PostingTaxIn @p0,@p1,@p2,@p3,@p4,@p5,@p6";

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
                            var newRecord = ctx.gnTaxIn.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == (isFPJCentral ? a.BranchCode : BranchCode)
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
            var data = ctx.gnTaxIn.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == (isFPJCentral ? a.BranchCode : BranchCode) && a.PeriodYear == periodYear && a.PeriodMonth == periodMonth && a.ProductType == ProductType).Take(1);

            return data.Count() == 1 ? true : false;
        }

        private int CountByPeriod(int periodYear, int periodMonth, bool isFPJCentral)
        {
            return ctx.gnTaxIn.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == (isFPJCentral ? a.BranchCode : BranchCode)
                 && a.ProductType == ProductType && a.PeriodYear == periodYear && a.PeriodMonth == periodMonth).Count();
        }

        private bool RecalculatePPN(string companyCode, string branchCode, string productType, int year, int month, string profitCenter, string docType)
        {
            var result = false;
            object[] recPPnParams = { companyCode, branchCode, productType, year, month, profitCenter, docType };
            var recPPnQuery = "exec uspfn_TaxRecalculatePPN @p0,@p1,@p2,@p3,@p4,@p5,@p6";
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
