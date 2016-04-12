using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.Sparepart.Models;
using SimDms.Common;
using SimDms.Common.Models;

namespace SimDms.Sparepart.Controllers.Api
{
    public class EntryHPPController : BaseController
    {
        public string getData()
        {
           // var transdate = ctx.CoProfileServices.Find(CompanyCode, BranchCode).TransDate;
            try
            {
                return GetNewDocumentNo("HPP", DateTime.Now);
            }
            catch (System.Data.SqlClient.SqlException e) {
                return e.Message;
            }
        }

        private bool CheckNomorPajakInvalid(string supplierCode, string fakturNo, out string msg)
        {
            var rec = (from a in ctx.SpTrnPHPPs
                       join b in ctx.SpTrnPRcvHdrs on new { a.CompanyCode, a.BranchCode, a.WRSNo }
                         equals new { b.CompanyCode, b.BranchCode, b.WRSNo }
                       where a.CompanyCode == CompanyCode
                       && a.BranchCode == BranchCode
                       && b.SupplierCode == supplierCode
                       select new
                       {
                           b.SupplierCode,
                           a,
                           b
                       }).ToList();

            if (rec.Count() == 0)
            {
                msg = "";
                return false;
            }
            else
            {
                msg = "Proses Gagal ! Terdapat duplikasi nomor pajak untuk supplier yang sama ! \n"
                        + "Nomor HPP : " + rec.FirstOrDefault().a.HPPNo + "\n"
                        + "Nomor Tax : " + rec.FirstOrDefault().a.TaxNo;
                return true;
            }
        }

        public JsonResult Save(SaveHPP model)
        {
            if (model.TaxNo.Trim().Length != 19 || model.TaxNo.ToString().Contains(" "))
            {
                return Json(new { success = false, message = "Format Nomor Faktur Pajak yang Anda ketik tidak benar" });
            }
            else
            {
                var recDtl = ctx.LookUpDtls.Find(CompanyCode, "STHP", "STAT");
                if (recDtl != null)
                {
                    if (recDtl.ParaValue == "1")
                    {
                        var msg = "";
                        if (this.CheckNomorPajakInvalid(model.SupplierCode, model.TaxNo, out msg))
                        {
                            return Json(new { success = false, message = msg });
                        }
                    }
                }
            }
            
            var record = ctx.SpTrnPHPPs.Find(CompanyCode, BranchCode, model.HPPNo);
            using (var trans = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    if (record == null)
                    {
                        var code = getData();
                        if (code.Length > 20)
                        {
                            return Json(new { success = false, message = code });
                        }

                        record = new SpTrnPHPP
                        {
                            CompanyCode = CompanyCode,
                            BranchCode = BranchCode,
                            HPPNo = code,
                            CreatedBy = CurrentUser.UserId,
                            CreatedDate = DateTime.Now,
                            TypeOfGoods = TypeOfGoods

                        };
                        ctx.SpTrnPHPPs.Add(record);
                    }
                    else
                    {
                        if (record.Status == "2")
                        {
                            return Json(new
                            {
                                success = false,
                                message = "Save HPP Gagal, Status Record tidak memperbolehkan!!!"
                            });
                        }
                    }
                    record.HPPDate = model.HPPDate;
                    record.WRSNo = model.WRSNo;
                    record.WRSDate = model.WRSDate;
                    record.TotNetPurchAmt = model.TotNetPurchAmt;
                    record.TotTaxAmt = model.TotTaxAmt;
                    record.TotPurchAmt = record.TotNetPurchAmt + record.TotTaxAmt;
                    record.PrintSeq = 0;
                    record.TaxNo = model.TaxNo;
                    record.TaxDate = model.TaxDate;
                    record.ReferenceNo = model.ReferenceNo;
                    record.ReferenceDate = model.ReferenceDate;
                    record.MonthTax = model.MonthTax;
                    record.YearTax = model.YearTax;
                    record.DueDate = model.DueDate;
                    record.DiffNetPurchAmt = model.DiffNetPurchAmt;
                    record.DiffTaxAmt = model.DiffTaxAmt;
                    record.TotHPPAmt = model.TotHPPAmt;
                    record.Status = "0";
                    record.LastUpdateBy = CurrentUser.UserId;
                    record.LastUpdateDate = DateTime.Now;

                    Helpers.ReplaceNullable(record);
                    ctx.SaveChanges();

                    // prepare spTrnPRcvHdr data
                    SpTrnPRcvHdr oSpTrnPRcvHdr = ctx.SpTrnPRcvHdrs.Find(record.CompanyCode, record.BranchCode, record.WRSNo);
                    oSpTrnPRcvHdr.Status = "4";
                    oSpTrnPRcvHdr.LastUpdateBy = CurrentUser.UserId;
                    oSpTrnPRcvHdr.LastUpdateDate = DateTime.Now;

                    // prepare spTrnPPOSHdr data
                    spTrnPPOSHdr oSpTrnPPOSHdr = null;
                    List<string> strPOSNo = new List<string>();

                    var dtSpTrnPRcvDtl = ctx.SpTrnPRcvDtls.Where(x => x.CompanyCode == record.CompanyCode
                        && x.BranchCode == record.BranchCode && x.WRSNo == record.WRSNo).ToList();

                    var result = true;
                    if (dtSpTrnPRcvDtl.Count() > 0)
                    {
                        foreach (SpTrnPRcvDtl row in dtSpTrnPRcvDtl)
                        {
                            string docNo = row.DocNo;
                            if (!strPOSNo.Contains(docNo))
                            {
                                oSpTrnPPOSHdr =  ctx.spTrnPPOSHdrs.Find(record.CompanyCode, record.BranchCode, docNo);

                                if (oSpTrnPPOSHdr == null) { result = false; break; }

                                oSpTrnPPOSHdr.Status = "6";
                                oSpTrnPPOSHdr.LastUpdateBy = CurrentUser.UserId;
                                oSpTrnPPOSHdr.LastUpdateDate = DateTime.Now;

                                result = ctx.SaveChanges() >= 0;

                                if (!result) break;
                                else strPOSNo.Add(docNo);
                            }
                        };
                    }

                    if (result)
                    {
                        trans.Commit();
                        return Json(new { success = true, clm = record.HPPNo });
                    }
                    else
                    {
                        trans.Rollback();
                        return Json(new { success = false, message = "Gagal Simpan No HPP, cek Order Sparepart!" });
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();

                    return Json(new { success = false, message = "Gagal Simpan No HPP !!!", error_log = ex.Message });
                }
            }
        }

        public JsonResult CloseHPP(SaveHPP model)
        {
            if (model.TaxNo.Trim().Length != 19 || model.TaxNo.ToString().Contains(" "))
            {
                return Json(new { success = false, message = "Format Nomor Faktur Pajak yang Anda ketik tidak benar" });
            }
            else
            {
                var recDtl = ctx.LookUpDtls.Find(CompanyCode, "STHP", "STAT");
                if (recDtl != null)
                {
                    if (recDtl.ParaValue == "1")
                    {
                        var msg = "";
                        if (this.CheckNomorPajakInvalid(model.SupplierCode, model.TaxNo, out msg))
                        {
                            return Json(new { success = false, message = msg });
                        }
                    }
                }
            }

            var record = ctx.SpTrnPHPPs.Find(CompanyCode, BranchCode, model.HPPNo);
            using (var trans = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    var query1 = string.Format(@"
                            SELECT TOP 1 * FROM SpTrnPHPP WITH (updLock, ReadPast)
                            WHERE CompanyCode = '{0}' AND BranchCode = '{1}'
                            AND HPPNo = '{2}'
                            ", CompanyCode, BranchCode, model.HPPNo);
                    var recordHPP = ctx.Database.SqlQuery<SpTrnPHPP>(query1).ToList();

                    if (record == null)
                    {
                        return Json(new { success = false, message = "No. HPP: \"" + record.HPPNo + "\" tidak ditemukan" });
                    }
                    else if (record.Status == "2") {
                        return Json(new { success = false, message = "Closing HPP gagal !!! \n No. HPP: \""+ record.HPPNo +"\" telah di close" });
                    }
                    else if (record.Status.Equals("0"))
                    {
                        return Json(new { success = false, message = "Closing tidak dapat dilakukan karena data header belum di print" });
                    }

                    //Validation Transaction Date 
                    //var validTranMsg = DateTransValidation(model.WRSDate.Value);
                    //if (!string.IsNullOrEmpty(validTranMsg))
                    //{
                    //    return Json(new { success = false, message = validTranMsg });
                    //}

                    record.PrintSeq = 0;
                    record.Status = "2";
                    record.LastUpdateBy = CurrentUser.UserId;
                    record.LastUpdateDate = DateTime.Now;


            
                    bool retVal = JournalHPP(record.HPPNo);
                    var pesan = "";

                    if (retVal)
                    {
                        // update status pos menjadi 7
                        spTrnPPOSHdr oSpTrnPPOSHdr = null;
                        List<string> strPOSNo = new List<string>();

                        var dtSpTrnPRcvDtl = ctx.SpTrnPRcvDtls.Where(x => x.CompanyCode == record.CompanyCode 
                            && x.BranchCode == record.BranchCode && x.WRSNo == record.WRSNo).ToList();
                            
                        if (dtSpTrnPRcvDtl.Count() > 0)
                        {
                            foreach (SpTrnPRcvDtl row in dtSpTrnPRcvDtl)
                            {
                                string docNo = row.DocNo;
                                if (!strPOSNo.Contains(docNo))
                                {
                                    oSpTrnPPOSHdr = ctx.spTrnPPOSHdrs.Find(record.CompanyCode, record.BranchCode, docNo);
                                    if (oSpTrnPPOSHdr == null)
                                    {
                                        pesan = "Record POS Header Tidak ditemukan";
                                        retVal = false;
                                        break;
                                    }

                                    oSpTrnPPOSHdr.Status = "7";
                                    oSpTrnPPOSHdr.LastUpdateBy = CurrentUser.UserId;
                                    oSpTrnPPOSHdr.LastUpdateDate = DateTime.Now;

                                    retVal = ctx.SaveChanges() >= 0;
                                    if (!retVal)
                                    {
                                        pesan = "Proses Update POS Header Gagal";
                                        break;
                                    }
                                    else strPOSNo.Add(docNo);
                                }
                            }
                        }

                        if (retVal)
                        {
                            ctx.SaveChanges();
                            trans.Commit();

                            return Json(new { success = true, data = record });
                        }
                        else{
                            trans.Rollback();
                            return Json(new { success = false, message = pesan });
                        }
                    }
                    else
                    {
                        trans.Rollback();
                        return Json(new { success = false, message = "Proses Posting jurnal gagal" });
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return Json(new { success = false, message = "No HPP : " + record.HPPNo + " gagal close...!" });
                }
            }
        }

        private bool JournalHPP(string HppNo)
        {
            var recHPP = ctx.SpTrnPHPPs.Find(CompanyCode, BranchCode, HppNo);
            var recWRS = ctx.SpTrnPRcvHdrs.Find(CompanyCode, BranchCode, (recHPP != null) ? recHPP.WRSNo : "");
            if (recWRS == null) {
                return false;
            }
            var recordProf = ctx.MstSupplierProfitCenters.Find(CompanyCode, BranchCode, recWRS.SupplierCode, ProfitCenter);
          //  if (recordProf != null)
            var oSupplierClass = ctx.GnMstSupplierClasses.Find(CompanyCode, BranchCode, recordProf.SupplierClass, recWRS.TypeOfGoods);
            if (oSupplierClass == null) {
                return false;
            }

            var oAccount = ctx.spMstAccounts.Find(CompanyCode, BranchCode, recHPP.TypeOfGoods);
            var oJurnal = ctx.GLInterfaces.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.DocNo == recHPP.HPPNo).FirstOrDefault();
            int seqno = 0;
            if (oJurnal == null) {

                if (recHPP.TotNetPurchAmt > 0) {
                    oJurnal = new GLInterface()
                    {
                        CompanyCode = CompanyCode,
                        BranchCode = BranchCode,
                        DocNo = recHPP.HPPNo,
                        SeqNo  = seqno += 1,
                        DocDate = recHPP.HPPDate,
                        ProfitCenterCode = ProfitCenter,
                        AccDate = DateTime.Now,
                        AccountNo = oAccount.InventoryAccNo,
                        JournalCode = "SPAREPART",
                        TypeJournal = "PURCHASE",
                        ApplyTo = recHPP.HPPNo,
                        AmountDb = recHPP.TotNetPurchAmt,
                        AmountCr = decimal.Zero,
                        TypeTrans = "INVENTORY",
                        BatchNo = string.Empty,
                        BatchDate = new DateTime(1900,1,1),
                        StatusFlag = "0",
                        CreateBy = CurrentUser.UserId,
                        CreateDate = DateTime.Now,
                        LastUpdateBy = CurrentUser.UserId,
                        LastUpdateDate = DateTime.Now

                    };
                    
                    ctx.GLInterfaces.Add(oJurnal);

                    //try
                    //{
                    //    ctx.SaveChanges();
                    //}
                    //catch (Exception e) {
                    //    Console.WriteLine(e);
                    //}
                }

                if (recHPP.DiffNetPurchAmt > 0)
                {
                    oJurnal = new GLInterface()
                    {
                        CompanyCode = CompanyCode,
                        BranchCode = BranchCode,
                        DocNo = recHPP.HPPNo,
                        SeqNo = seqno += 1,
                        DocDate = recHPP.HPPDate,
                        ProfitCenterCode = ProfitCenter,
                        AccDate = DateTime.Now,
                        AccountNo = oAccount.COGSAccNo,
                        JournalCode = "SPAREPART",
                        TypeJournal = "PURCHASE",
                        ApplyTo = recHPP.HPPNo,
                        AmountDb = recHPP.DiffNetPurchAmt,
                        AmountCr = decimal.Zero,
                        TypeTrans = "COGS",
                        BatchNo = string.Empty,
                        BatchDate = new DateTime(1900,1,1),
                        StatusFlag = "0",
                        CreateBy = CurrentUser.UserId,
                        CreateDate = DateTime.Now,
                        LastUpdateBy = CurrentUser.UserId,
                        LastUpdateDate = DateTime.Now

                    };

                    ctx.GLInterfaces.Add(oJurnal);

                    //try
                    //{
                    //    ctx.SaveChanges();
                    //}
                    //catch (Exception e)
                    //{
                    //    Console.WriteLine(e);
                    //}
                }
                else if (recHPP.DiffNetPurchAmt < 0) {
                    oJurnal = new GLInterface()
                    {
                        CompanyCode = CompanyCode,
                        BranchCode = BranchCode,
                        DocNo = recHPP.HPPNo,
                        SeqNo = seqno += 1,
                        DocDate = recHPP.HPPDate,
                        ProfitCenterCode = ProfitCenter,
                        AccDate = DateTime.Now,
                        AccountNo = oAccount.COGSAccNo,
                        JournalCode = "SPAREPART",
                        TypeJournal = "PURCHASE",
                        ApplyTo = recHPP.HPPNo,
                        AmountDb = decimal.Zero,
                        AmountCr = Math.Abs(Convert.ToDecimal(recHPP.DiffNetPurchAmt)),
                        TypeTrans = "COGS",
                        BatchNo = string.Empty,
                        BatchDate = new DateTime(1900,1,1),
                        StatusFlag = "0",
                        CreateBy = CurrentUser.UserId,
                        CreateDate = DateTime.Now,
                        LastUpdateBy = CurrentUser.UserId,
                        LastUpdateDate = DateTime.Now

                    };

                    ctx.GLInterfaces.Add(oJurnal);

                    //try
                    //{
                    //    ctx.SaveChanges();
                    //}
                    //catch (Exception e)
                    //{
                    //    Console.WriteLine(e);
                    //}
                }

                if (recHPP.TotTaxAmt + recHPP.DiffTaxAmt > 0)
                {

                    oJurnal = new GLInterface()
                    {
                        CompanyCode = CompanyCode,
                        BranchCode = BranchCode,
                        DocNo = recHPP.HPPNo,
                        SeqNo = seqno += 1,
                        DocDate = recHPP.HPPDate,
                        ProfitCenterCode = ProfitCenter,
                        AccDate = DateTime.Now,
                        AccountNo = oSupplierClass.TaxInAccNo,
                        JournalCode = "SPAREPART",
                        TypeJournal = "PURCHASE",
                        ApplyTo = recHPP.HPPNo,
                        AmountDb = recHPP.TotTaxAmt + recHPP.DiffTaxAmt,
                        AmountCr = decimal.Zero,
                        TypeTrans = "TAX IN",
                        BatchNo = string.Empty,
                        BatchDate = new DateTime(1900,1,1),
                        StatusFlag = "0",
                        CreateBy = CurrentUser.UserId,
                        CreateDate = DateTime.Now,
                        LastUpdateBy = CurrentUser.UserId,
                        LastUpdateDate = DateTime.Now

                    };

                    ctx.GLInterfaces.Add(oJurnal);

                    //try
                    //{
                    //    ctx.SaveChanges();
                    //}
                    //catch (Exception e)
                    //{
                    //    Console.WriteLine(e);
                    //}
                   
                }

                oJurnal = new GLInterface()
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    DocNo = recHPP.HPPNo,
                    SeqNo = seqno += 1,
                    DocDate = recHPP.HPPDate,
                    ProfitCenterCode = ProfitCenter,
                    AccDate = DateTime.Now,
                    AccountNo = oSupplierClass.PayableAccNo,
                    JournalCode = "SPAREPART",
                    TypeJournal = "PURCHASE",
                    ApplyTo = recHPP.HPPNo,
                    AmountDb = decimal.Zero,
                    AmountCr = recHPP.TotNetPurchAmt + recHPP.DiffNetPurchAmt + recHPP.TotTaxAmt + recHPP.DiffTaxAmt,
                    TypeTrans = "AP",
                    BatchNo = string.Empty,
                    BatchDate = new DateTime(1900,1,1),
                    StatusFlag = "0",
                    CreateBy = CurrentUser.UserId,
                    CreateDate = DateTime.Now,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = DateTime.Now

                };

                ctx.GLInterfaces.Add(oJurnal);

                //try
                //{
                //    ctx.SaveChanges();
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine(e);
                //}

            }

            var oSpTrnPRcvHdr = ctx.SpTrnPRcvHdrs.Find(CompanyCode, BranchCode, recHPP.WRSNo);

            string refNo = (recHPP.WRSNo != null) ? Convert.ToString(recHPP.WRSNo) : string.Empty;
            DateTime refDate = (recHPP.WRSDate != null) ? Convert.ToDateTime(recHPP.WRSDate) : DateTime.Now;
            string suppCode = (oSpTrnPRcvHdr.SupplierCode != null) ? Convert.ToString(oSpTrnPRcvHdr.SupplierCode) : string.Empty;
            DateTime termDate = (recHPP.DueDate != null) ? Convert.ToDateTime(recHPP.DueDate) : DateTime.Now;
            string taxNo = (recHPP.TaxNo != null) ? Convert.ToString(recHPP.TaxNo) : string.Empty;
            DateTime taxDate = (recHPP.TaxDate != null) ? Convert.ToDateTime(recHPP.TaxDate) : DateTime.Now;


            GenerateAP(recHPP.HPPNo, recHPP.HPPDate, refNo, refDate, recHPP.TotNetPurchAmt + recHPP.DiffNetPurchAmt, 0, suppCode,
                    recHPP.TotTaxAmt + recHPP.DiffTaxAmt, 0, oSupplierClass.PayableAccNo, termDate, string.Empty, taxNo, taxDate);
            return true;
        }

        private void GenerateAP(string docno, DateTime? docdate, string refNo, DateTime refDate, decimal? netAmt, int pphAmt, string suppCode, decimal? ppnAmt, int ppnBM, string accNo, DateTime termDate, string termName, string taxNo, DateTime taxDate)
        {
            var oApInterface = ctx.ApInterfaces.Find(CompanyCode, BranchCode, docno);
            if (oApInterface == null) { 
                oApInterface = new ApInterface{
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    DocNo = docno
                };
                ctx.ApInterfaces.Add(oApInterface);
            }

            oApInterface.DocDate = docdate;
            oApInterface.ProfitCenterCode = ProfitCenter;
            oApInterface.Reference = refNo;
            oApInterface.ReferenceDate = refDate;
            oApInterface.NetAmt = netAmt;
            oApInterface.PPHAmt = pphAmt;
            oApInterface.SupplierCode = suppCode;
            oApInterface.PPNAmt = ppnAmt;
            oApInterface.PPnBM = ppnBM;
            oApInterface.AccountNo = accNo;
            oApInterface.TermsDate = termDate;
            oApInterface.TermsName = termName;
            oApInterface.ReceiveAmt = 0;
            oApInterface.TotalAmt = oApInterface.NetAmt + oApInterface.PPNAmt;
            oApInterface.StatusFlag = "0";
            oApInterface.FakturPajakNo = taxNo;
            oApInterface.FakturPajakDate = taxDate;
            oApInterface.CreateBy = CurrentUser.UserId;
            oApInterface.CreateDate = DateTime.Now;

            var recHPP = ctx.SpTrnPHPPs.Find(CompanyCode, BranchCode, docno);
            if (recHPP != null) oApInterface.RefNo = recHPP.ReferenceNo;

            //return true;
        }

        public JsonResult Print(SpTrnPHPP model)
        {
            var record = ctx.SpTrnPHPPs.Find(CompanyCode, BranchCode, model.HPPNo);
            if (record != null)
            {
                record.Status = "1";
                record.PrintSeq += 1;
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = DateTime.Now;
            }
            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, status = record.Status });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Gagal print No HPP !!!" });
            }


        }

        public JsonResult loadData(string HPPNo) 
        {
            var record = ctx.SpLoadEntryHPPs.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.TypeOfGoods == TypeOfGoods && a.HPPNo == HPPNo).FirstOrDefault();
            return Json(record);
        }

        public JsonResult getHppByWrs(string wrsNO)
        {
            var coProfilePart = ctx.Database.SqlQuery<GnMstCoProfileSpare>(" Select * from GnMstCoProfileSpare WITH(NoLock,NoWait) where CompanyCode =  '" + CompanyCode + "' and BranchCode = '" + BranchCode + "'").FirstOrDefault();
            var sql = string.Format("exec sp_EntryHPPBrowse '{0}','{1}', {2}, {3}, {4}", CompanyCode, BranchCode, TypeOfGoods, coProfilePart.PeriodBeg.ToString("yyyyMMdd"), coProfilePart.PeriodEnd.ToString("yyyyMMdd"));
            var records = ctx.Database.SqlQuery<EntryHPPBrowse>(sql)
                .Where(x => x.WRSNo == wrsNO).ToList();
            return Json(records);
        }

        public JsonResult getWrsByNo(string wrsNO)
        {
            var records = ctx.Database.SqlQuery<SpWRSHpp>(" Select * from SpWRSHpp where CompanyCode =  '" + CompanyCode + "' and BranchCode = '" + BranchCode + "' and TypeOfGoods = '" + TypeOfGoods + "'")
                .Where(x => x.WRSNo == wrsNO).ToList();
            return Json(records);
        }

        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                Today = DateTime.Now,
            });
        }
    }
}
