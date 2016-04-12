using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using SimDms.Sparepart.Models;
using System.Transactions;

namespace SimDms.Sparepart.Controllers.Api
{
    public class MaintenanceFakturPenjualanController : BaseController
    {
        //public JsonResult LookUpFPJNo()
        //{
        //    var ShowAll = Request["ShowAll"] ?? "1";

        //    var uid = CurrentUser;
        //    string sql = string.Format("exec uspfn_GetSpTrnSFPJHdr '{0}', '{1}'",  CompanyCode, BranchCode);
        //    var queryable = ctx.Database.SqlQuery<FPJLookup>(sql);

        //    //var records = ctx.JobOrderViews
        //    //    .Where(p => p.CompanyCode == uid.CompanyCode
        //    //    && p.BranchCode == uid.BranchCode && p.ServiceType == "2"
        //    //    && new string[] { "0", "1", "2", "3", "4", "5" }
        //    //    .Contains(p.ServiceStatus)).OrderByDescending(p => p.ServiceNo);

        //    //if (ShowAll == "0")
        //    //{
        //    //    records = ctx.JobOrderViews
        //    //    .Where(p => p.CompanyCode == uid.CompanyCode
        //    //    && p.BranchCode == uid.BranchCode && p.ServiceType == "2")
        //    //    .OrderByDescending(p => p.ServiceNo);
        //    //}

        //    return Json(records.toKG());
        //}

        public JsonResult GetSpTrnSFPJDtl(string FPJNo)
        {
            string sql = string.Format("exec uspfn_spGetSpTrnSFPJDtl '{0}', '{1}','{2}'", CompanyCode, BranchCode, FPJNo);
            var data = ctx.Database.SqlQuery<FPJLookup>(sql);
            decimal TotalDPP=0,TotalPPN=0, TotalAmount=0, DPP=0, PPN=0, Disc = 0;
            //foreach (var FPJ in data.ToList())
            //{
            //    Disc = Math.Round((FPJ.SalesAmt * FPJ.DiscPct) / 100, 1, MidpointRounding.AwayFromZero);
            //    DPP = FPJ.SalesAmt - Disc;
            //    TotalDPP += DPP;
            //}
            //TotalPPN = (TotalDPP * 10) / 100;
            //TotalAmount = TotalDPP + TotalPPN;
            var FPJHdr = ctx.SpTrnSFPJHdrs.Find(CompanyCode, BranchCode, FPJNo);
            TotalDPP = FPJHdr.TotDPPAmt;
            TotalPPN = FPJHdr.TotPPNAmt;
            TotalAmount = FPJHdr.TotFinalSalesAmt;
            var queryable = data.AsQueryable();
            return Json(new { data = queryable, TotalDPP = TotalDPP, TotalPPN = TotalPPN, TotalAmount = TotalAmount });

        }

        public JsonResult Save(FPJLookup model)
        {
            var SFPJHdr = ctx.SpTrnSFPJHdrs.Find(CompanyCode, BranchCode, model.FPJNo);
            var SFPJHdrLog = new SpTrnSFPJHdrLog();
            if (SFPJHdr != null)
            {
                var ArInterface = ctx.ArInterfaces.Find(CompanyCode, BranchCode, model.FPJNo);
                if (ArInterface != null && ArInterface.StatusFlag != "0")
                {
                    return Json(new { success = false, message = "Data tidak dapat diganti karena sudah masuk Ar Finance !" });
                }

                try
                {
                    SFPJHdr.CustomerCodeBill = SFPJHdr.CustomerCodeShip = SFPJHdr.CustomerCode = model.CustomerCode;
                    SFPJHdr.LastUpdateBy = CurrentUser.UserId;
                    SFPJHdr.LastUpdateDate = DateTime.Now;

                    SFPJHdrLog.CompanyCode = SFPJHdr.CompanyCode;
                    SFPJHdrLog.BranchCode = SFPJHdr.BranchCode;
                    SFPJHdrLog.CustomerCodeBill = SFPJHdr.BranchCode;
                    SFPJHdrLog.FPJNo = SFPJHdr.BranchCode;
                    SFPJHdrLog.InvoiceNo = SFPJHdr.BranchCode;
                    SFPJHdrLog.LastUpdateBy = CurrentUser.UserId;
                    SFPJHdrLog.LastUpdateDate = DateTime.Now;
                    SFPJHdrLog.TotDiscAmt = SFPJHdr.TotDiscAmt;
                    SFPJHdrLog.TotDPPAmt = SFPJHdr.TotDPPAmt;
                    SFPJHdrLog.TotFinalSalesAmt = SFPJHdr.TotFinalSalesAmt;
                    SFPJHdrLog.TotPPNAmt = SFPJHdr.TotPPNAmt;
                    ctx.SpTrnSFPJHdrLogs.Add(SFPJHdrLog);
                    ctx.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Proses update SpTrnSFPJHdr dan Insert SpTrnSFPJHdrLog Gagal" });
                }

                try
                {
                    var info = ctx.SpTrnSFPJInfos.Find(CompanyCode, BranchCode, model.FPJNo);
                    info.CustomerName = model.CustomerName;
                    info.Address1 = model.Address1;
                    info.Address2 = model.Address2;
                    info.Address3 = model.Address3;
                    info.Address4 = model.Address4;
                    info.LastUpdateBy = CurrentUser.UserId;
                    info.LastUpdateDate = DateTime.Now;
                    ctx.SaveChanges();
                }
                catch (Exception)
                {
                    return Json(new { success = false, message = "Proses Insert SpTrnSFPJInfo Gagal" });
                }

                try
                {
                    SpTrnSInvoiceHdr invHdr = ctx.SpTrnSInvoiceHdrs.Find(CompanyCode, BranchCode, SFPJHdr.InvoiceNo);
                    invHdr.CustomerCode = model.CustomerCode;
                    invHdr.LastUpdateBy = CurrentUser.UserId;
                    invHdr.LastUpdateDate = DateTime.Now;

                    var invHdrLog = new SpTrnSInvoiceHdrLog();
                    invHdrLog.CompanyCode = invHdr.CompanyCode;
                    invHdrLog.BranchCode = invHdr.BranchCode;
                    invHdrLog.CustomerCodeBill = invHdr.CustomerCodeBill;
                    invHdrLog.InvoiceNo = invHdr.InvoiceNo;
                    invHdrLog.LastUpdateBy = CurrentUser.UserId;
                    invHdrLog.LastUpdateDate = DateTime.Now;
                    invHdrLog.TotDiscAmt = invHdr.TotDiscAmt;
                    invHdrLog.TotDPPAmt = invHdr.TotDPPAmt;
                    invHdrLog.TotFinalSalesAmt = invHdr.TotFinalSalesAmt;
                    invHdrLog.TotPPNAmt = invHdr.TotPPNAmt;

                    ctx.SpTrnSInvoiceHdrLogs.Add(invHdrLog);
                    ctx.SaveChanges();
                }
                catch (Exception)
                {
                    return Json(new { success = false, message = "Proses update SpTrnSInvoiceHdr dan Insert SpTrnSInvoiceHdrLog Gagal" });
                }
            }
            return Json(new { success = true, message = "" });
        }

        public JsonResult SaveFPJDtl(FPJLookup model)
        {
            var fpjHdr = ctx.SpTrnSFPJHdrs.Find(CompanyCode, BranchCode, model.FPJNo);
            if (fpjHdr != null)
            {
                //check ar Interface
                var ArInterface = ctx.ArInterfaces.Find(CompanyCode, BranchCode, model.FPJNo);
                if (ArInterface != null && ArInterface.StatusFlag != "0")
                {
                    return Json(new { success = false, message = "Data tidak dapat diganti karena sudah masuk Ar Finance !" });
                }
                               
                //Get FPJ Details
                var fpjDtl = ctx.SpTrnSFPJDtls.Find(CompanyCode, BranchCode, model.FPJNo, model.WarehouseCode, model.PartNo, model.PartNoOriginal, model.DocNo);

                if (fpjDtl != null)
                {

                    SpTrnSFPJDtlLog SpTrnSFPJDtlLog = new SpTrnSFPJDtlLog();
                    try
                    {
                        //Update FPJDetails
                        fpjDtl.DiscPct = model.DiscPct;
                        decimal discAmt = model.SalesAmt * (fpjDtl.DiscPct / 100);
                        fpjDtl.DiscAmt = discAmt;
                        fpjDtl.NetSalesAmt = fpjDtl.SalesAmt - discAmt;
                        fpjDtl.TotSalesAmt = fpjDtl.SalesAmt - discAmt;
                        //ctx.
                        //Insert FPJ Details Log
                        SpTrnSFPJDtlLog.CompanyCode = fpjDtl.CompanyCode;
                        SpTrnSFPJDtlLog.BranchCode = fpjDtl.BranchCode;
                        SpTrnSFPJDtlLog.FPJNo = fpjDtl.FPJNo;
                        SpTrnSFPJDtlLog.WarehouseCode = fpjDtl.Warehousecode;
                        SpTrnSFPJDtlLog.PartNo = fpjDtl.PartNo;
                        SpTrnSFPJDtlLog.PartNoOriginal = fpjDtl.PartNoOriginal;
                        SpTrnSFPJDtlLog.DocNo = fpjDtl.DocNo;
                        SpTrnSFPJDtlLog.DiscPct = fpjDtl.DiscPct;
                        SpTrnSFPJDtlLog.DiscAmt = fpjDtl.DiscAmt;
                        SpTrnSFPJDtlLog.NetSalesAmt = fpjDtl.NetSalesAmt;
                        SpTrnSFPJDtlLog.TotSalesAmt = fpjDtl.TotSalesAmt;
                        SpTrnSFPJDtlLog.LastUpdateBy = CurrentUser.UserId;
                        SpTrnSFPJDtlLog.LastUpdateDate = DateTime.Now;
                        ctx.SpTrnSFPJDtlLogs.Add(SpTrnSFPJDtlLog);
                        ctx.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, message = "Proses Simpan SpTrnSFPJDtl dan SpTrnSFPJDtlLog Gagal." });
                    }

                    SpTrnSInvoiceDtlLog invDtlLog = new SpTrnSInvoiceDtlLog();
                    //Get Invoice Details
                    SpTrnSInvoiceDtl invDtl = ctx.SpTrnSInvoiceDtls.Find(CompanyCode, BranchCode, fpjHdr.InvoiceNo, model.WarehouseCode,
                        model.PartNo, model.PartNo, model.DocNo);
                    try
                    {
                        if (invDtl != null)
                        {
                            invDtl.DiscPct = fpjDtl.DiscPct;
                            invDtl.DiscAmt = fpjDtl.DiscAmt;
                            invDtl.NetSalesAmt = fpjDtl.NetSalesAmt;
                            invDtl.TotSalesAmt = fpjDtl.TotSalesAmt;
                            invDtl.LastUpdateBy = CurrentUser.UserId;
                            invDtl.LastUpdateDate = DateTime.Now;

                            invDtlLog.CompanyCode = CompanyCode;
                            invDtlLog.BranchCode = BranchCode;
                            invDtlLog.DiscAmt = invDtl.DiscAmt;
                            invDtlLog.DiscPct = invDtl.DiscPct;
                            invDtlLog.InvoiceNo = invDtl.InvoiceNo;
                            invDtlLog.LastUpdateBy = CurrentUser.UserId;
                            invDtlLog.LastUpdateDate = DateTime.Now;
                            invDtlLog.NetSalesAmt = invDtl.NetSalesAmt;
                            invDtlLog.PartNo = invDtl.PartNo;
                            invDtlLog.PartNoOriginal = invDtl.PartNoOriginal;
                            invDtlLog.TotSalesAmt = invDtl.TotSalesAmt;

                            ctx.SpTrnSInvoiceDtlLogs.Add(invDtlLog);
                            ctx.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, message = "Error pada saat Update SpTrnsInvoiceDtl dan Insert SpTrnsInvoiceDtlLog" });
                    }
                    
                    var query = "exec uspfn_SvTrnUpdateFPJHeader @p0,@p1,@p2,@p3,@p4";
                    object[] parameters = { CompanyCode, BranchCode, fpjHdr.FPJNo, fpjHdr.InvoiceNo, CurrentUser.UserId };
                    ctx.Database.ExecuteSqlCommand(query, parameters);

                }

            }
            return Json(new { success = true, message = "" });
        }

    }
}
