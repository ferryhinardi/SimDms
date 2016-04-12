using SimDms.Sales.BLL;
using SimDms.Sales.Models;
//using SimDms.Sales.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;
using System.Data;
using System.Transactions;

namespace SimDms.Sales.Controllers.Api
{
    public class HPPController : BaseController
    {
        public JsonResult POView(string PONo)
        {
            var titleName = from a in ctx.OmTrPurchasePOs
                            join b in ctx.Supplier
                            on a.SupplierCode equals b.SupplierCode
                            where a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.PONo == PONo
                            select new
                            {
                                a.PONo,
                                a.RefferenceNo,
                                a.BillTo,
                                b.SupplierName,
                                b.SupplierCode
                            };

            if (titleName != null)
            {
                return Json(new
                {
                    success = true,
                    data = titleName.FirstOrDefault()
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    success = true,
                    TitleName = ""
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult BPUDetailLoad(string HPPNo)
        {
            var query = string.Format(@"
                SELECT a.BPUNo, a.RefferenceDONo, a.RefferenceSJNo, a.PONo,a.Status
                FROM    dbo.omTrPurchaseBPU a
                INNER JOIN
                        dbo.omTrPurchaseHPPDetail b
                        ON a.CompanyCode = b.CompanyCode
                        AND a.BranchCode = b.BranchCode
                        AND a.BPUNo = b.BPUNo
                WHERE   a.CompanyCode = '{0}'
                        AND a.BranchCode = '{1}'
                        AND b.HPPNo = '{2}'
                       ", CompanyCode, BranchCode, HPPNo);

            var uQry = string.Format(@"SELECT isLocked FROM omTrPurchaseHPP WHERE CompanyCode = '{0}' AND BranchCode='{1}' AND HPPNo = '{2}'", CompanyCode, BranchCode, HPPNo);
            bool bUpload = ctx.Database.SqlQuery<bool>(uQry).FirstOrDefault();
            return Json(new { data = ctx.Database.SqlQuery<BPUView>(query).AsQueryable(), bUpload = bUpload });
        }

        public JsonResult Approve(omTrPurchaseHPP mdlHdr)
        {
            bool result;
            string msg = "";
            result = OmTrPurchaseHPPBLL.Instance(CurrentUser.UserId).validSubDetail(mdlHdr.HPPNo);

            if (!result)
            {
                return Json(new { success = false, message = "Jumlah Model dengan Sub Detail tidak sama" });
            }

            ctx.omTrPurchaseHPPDetail.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.HPPNo == mdlHdr.HPPNo).ToList()
            .ForEach(y =>
            {
                ctx.omTrPurchaseHPPDetailModel.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.HPPNo == y.HPPNo && x.BPUNo == y.BPUNo)
                .ToList().ForEach(z =>
                {
                    decimal totalPPN = 0;
                    decimal totalDPP = 0;
                    ctx.omTrPurchaseHPPDetailModelOthers.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.HPPNo == z.HPPNo && x.BPUNo == z.BPUNo
                    && x.SalesModelCode == z.SalesModelCode && x.SalesModelYear == z.SalesModelYear).ToList().ForEach(w =>
                    {
                        totalDPP += w.OthersDPP == null ? 0 : (decimal)w.OthersDPP;
                        totalPPN += w.OthersPPN == null ? 0 : (decimal)w.OthersPPN;

                        if (w.OthersPPN.ToString() != totalPPN.ToString())
                        {
                            msg = "Nilai PPN Others untuk : " + "\n" + "No.BPU : " + z.BPUNo + "\n" + "Sales Model Code : " + z.SalesModelCode + "\n" + "Sales Model Year : " + z.SalesModelYear + "\n" + " tidak sama. Silahkan check kembali kendaraan tersebut";
                            return;
                        }
                        if (w.OthersDPP.ToString() != totalDPP.ToString())
                        {
                            msg = "Nilai DPP Others untuk : " + "\n" + "No.BPU : " + z.BPUNo + "\n" + "Sales Model Code : " + z.SalesModelCode + "\n" + "Sales Model Year : " + z.SalesModelYear + "\n" + " tidak sama. Silahkan check kembali kendaraan tersebut";
                            return;
                        }
                    });
                });
            });

            result = OmTrPurchaseHPPBLL.Instance(CurrentUser.UserId).ApproveHPP(mdlHdr.HPPNo);

            if (result)
            {
                var dstatus = "";
                var dtHPP = ctx.omTrPurchaseHPP.Find(CompanyCode, BranchCode, mdlHdr.HPPNo);
                if (dtHPP != null)
                {
                    dstatus = dtHPP.Status;
                }
                return Json(new { success = true, message = "Approve HPP berhasil!", status = dstatus });
            }
            return Json(new { success = false, message = "Approve HPP gagal!" });
        }


        public JsonResult SalesModelLoad(string HPPNo, string BPUNo)
        {
            var query = string.Format(@"
                SELECT * 
                FROM dbo.omTrPurchaseHPPDetailModel a
                WHERE a.CompanyCode = '{0}'
                    AND a.BranchCode = '{1}'
                    AND a.HPPNo = '{2}'
                    AND a.BPUNo = '{3}'
                       ", CompanyCode, BranchCode, HPPNo, BPUNo);
            return Json(ctx.Database.SqlQuery<SalesModelView>(query).AsQueryable());
        }

        public JsonResult SalesModelOthersLoad(omTrPurchaseHPPDetailModelOthers model)
        {
            var query = string.Format(@"
                SELECT * 
                FROM dbo.OmTrPurchaseHPPDetailModelOthers a
                WHERE a.CompanyCode = '{0}'
                    AND a.BranchCode = '{1}'
                    AND a.HPPNo = '{2}'
                    AND a.BPUNo ='{3}'
                    AND a.SalesModelCode = '{4}'
                    AND a.SalesModelYear = '{5}'
                       ", CompanyCode, BranchCode, model.HPPNo, model.BPUNo, model.SalesModelCode, model.SalesModelYear);
            return Json(ctx.Database.SqlQuery<omTrPurchaseHPPDetailModelOthers>(query).AsQueryable());
        }

        public JsonResult SubSalesModelLoad(omTrPurchaseHPPSubDetail model)
        {
            var query = string.Format(@"
                SELECT * 
                FROM dbo.omTrPurchaseHPPSubDetail a
                WHERE a.CompanyCode = '{0}'
                    AND a.BranchCode = '{1}'
                    AND a.HPPNo = '{2}'
                    AND a.BPUNo ='{3}'
                    AND a.SalesModelCode = '{4}'
                    AND a.SalesModelYear = '{5}'
                       ", CompanyCode, BranchCode, model.HPPNo, model.BPUNo, model.SalesModelCode, model.SalesModelYear);
            return Json(ctx.Database.SqlQuery<SubSalesModelView>(query).AsQueryable());
        }

        public JsonResult updateHdr(omTrPurchaseHPP model)
        {
            var me = ctx.omTrPurchaseHPP.Find(CompanyCode, BranchCode, model.HPPNo);
            if (me != null)
            {
                var meBPU = ctx.omTrPurchaseHPPDetail.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.HPPNo == model.HPPNo).FirstOrDefault();
                var data = new omTrPurchaseHPP();

                if (meBPU != null && me.Status == "0")
                {
                    var meDSM = ctx.omTrPurchaseHPPDetailModel
                        .Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.HPPNo == model.HPPNo && p.BPUNo == meBPU.BPUNo)
                        .FirstOrDefault();
                    if (meDSM != null)
                    {
                        //                        var query = string.Format(@"
                        //                Update dbo.omTrPurchaseHPP 
                        //                set Status = 1 
                        //                WHERE CompanyCode = '{0}'
                        //                    AND BranchCode = '{1}'
                        //                    AND HPPNo = '{2}'
                        //                       ", CompanyCode, BranchCode, model.HPPNo);
                        //                        ctx.Database.ExecuteSqlCommand(query);

                        me.Status = "1";
                        ctx.SaveChanges();
                        data = ctx.omTrPurchaseHPP.Find(CompanyCode, BranchCode, model.HPPNo);

                    }
                    else
                    {
                        return Json(new { success = false, message = meBPU.BPUNo + " : do not have table detail model in HPP!" });
                    }
                }
                else if (meBPU != null && me.Status == "1")
                {
                    me.Status = "2";
                    ctx.SaveChanges();
                    data = ctx.omTrPurchaseHPP.Find(CompanyCode, BranchCode, model.HPPNo);
                    var query = string.Format(@"uspfn_OmApprovePurchaseHPP
                       ", CompanyCode, BranchCode, model.HPPNo, CurrentUser.UserId);
                    var queryable = ctx.Database.SqlQuery<string>(query).AsQueryable();
                }

                return Json(new { success = true, data = data });
            }
            else
            {
                return Json(new { success = false, message = "You must fill table detail!" });
            }
        }

        public JsonResult ValidateSalesModelYear(string PONo, string BPUNo, string HPPNo, string salesModelCode, decimal salesModelYear)
        {
            bool success = true;
            string message = "";
            decimal poSalesModelYear = salesModelYear;
            string bpuType = "";
            omTrPurchaseBPU oOmTrPurchaseBPU = ctx.omTrPurchaseBPU.Find(CompanyCode, BranchCode, PONo, BPUNo);
            if (oOmTrPurchaseBPU != null) bpuType = oOmTrPurchaseBPU.BPUType;

            if (ProductType.Equals("2W") && bpuType.Equals("2"))
            {
                var dtPO = ctx.OmTrPurchasePOModels.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.PONo == PONo && p.SalesModelCode == salesModelCode).ToList();
                if (dtPO.Count > 0)
                    poSalesModelYear = dtPO[0].SalesModelYear;

                if (poSalesModelYear != salesModelYear)
                {
                    omTrPurchaseHPPDetailModel oOmTrPurchaseHPPDetailModel = ctx.omTrPurchaseHPPDetailModel.Find(CompanyCode, BranchCode, HPPNo, BPUNo, salesModelCode, salesModelYear);
                    if (oOmTrPurchaseHPPDetailModel == null)
                    {
                        success = false;
                        message = "Tahun dan harga tidak sama dengan PO, apakah ingin dilanjutkan?";
                    }
                }
            }

            return Json(new { success = success, message = message, data = poSalesModelYear });
        }

        public JsonResult PriceListBeli(string PONo, string salesModelCode, decimal salesModelYear, string BPUNo)
        {
            var record = ctx.OmTrPurchasePOModels.Find(CompanyCode, BranchCode, PONo, salesModelCode, salesModelYear);
            if(!string.IsNullOrEmpty(BPUNo)) {
                var recordBPU = ctx.OmTrPurchaseBPUDetailModels.Find(CompanyCode, BranchCode, PONo, BPUNo, salesModelCode, salesModelYear);
                record.QuantityBPU = recordBPU.QuantityBPU - recordBPU.QuantityHPP;
            }
            
            return Json(new { success = record != null, data = record });
        }

        [HttpPost]
        public JsonResult Save(omTrPurchaseHPP model)
        {
            //ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            //string branchCode = BranchCode;
            DateTime currentTime = DateTime.Now;

            var me = ctx.omTrPurchaseHPP.Find(companyCode, BranchCode, model.HPPNo);
            try
            {
                if (me == null)
                {
                    me = new omTrPurchaseHPP();
                    me.CreatedDate = currentTime;
                    me.LastUpdateDate = currentTime;
                    me.CreatedBy = userID;
                    var hppno = GetNewDocumentNo("HPU", model.HPPDate.Value);
                    me.HPPNo = hppno;
                    ctx.omTrPurchaseHPP.Add(me);
                }
                else
                {
                    me.LastUpdateDate = currentTime;
                    me.LastUpdateBy = userID;
                }
                me.CompanyCode = CompanyCode;
                me.BranchCode = BranchCode;
                me.HPPDate = model.HPPDate != null ? model.HPPDate : Convert.ToDateTime("1900/01/01");
                me.PONo = model.PONo;
                me.SupplierCode = model.SupplierCode;
                me.BillTo = model.BillTo != null ? model.BillTo : "";
                me.RefferenceInvoiceNo = model.RefferenceInvoiceNo.ToUpper();
                me.RefferenceInvoiceDate = model.RefferenceInvoiceDate != null ? model.RefferenceInvoiceDate : Convert.ToDateTime("1900/01/01");
                me.RefferenceFakturPajakNo = model.RefferenceFakturPajakNo != null ? model.RefferenceFakturPajakNo.ToUpper() : "";
                me.RefferenceFakturPajakDate = model.RefferenceFakturPajakDate != null ? model.RefferenceFakturPajakDate : Convert.ToDateTime("1900/01/01");
                me.DueDate = model.DueDate;
                me.Remark = model.Remark != null ? model.Remark.ToUpper() : "";
                me.Status = "0";
                me.isLocked = false;
                me.LockingBy = "";
                me.LockingDate = model.LockingDate;

                ctx.SaveChanges();
                return Json(new { success = true, data = me });
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SaveUpl(omTrPurchaseHPP model, string BPUNo, string BatchNo)
        {
            //ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;
            string standardCode = "";
            string msg = "";
            if (model.SupplierCode == null) { model.SupplierCode = ""; }
            
            var supplier = ctx.Supplier.Find(CompanyCode, model.SupplierCode);
            if (supplier != null)
            standardCode = ctx.Supplier.Find(CompanyCode, model.SupplierCode).StandardCode.ToString();

            var batchNo = ctx.OmUtlSHPOKDtl1s.Where(c => c.CompanyCode == CompanyCode && c.BranchCode == BranchCode && c.InvoiceNo == model.RefferenceInvoiceNo).FirstOrDefault();
            if (batchNo == null)
            {
                msg = "Data upload tidak ditemukan";
                return Json(new { success = false, message = msg });
            }

            if (string.IsNullOrEmpty(model.HPPNo))
            {
                if (p_IsExistInvNo(model.RefferenceInvoiceNo))
                {
                    msg = "No. Ref. Inv sudah ada di HPP";
                    return Json(new { success = false, message = msg });
                }

                if (!DateTransValidation(model.HPPDate.Value.Date, ref msg))
                {
                    return Json(new { success = false, message = msg });
                }
            }

            OmUtlSHPOKHdr utl = ctx.OmUtlSHPOKHdrs.Find(CompanyCode, BranchCode, standardCode, CompanyCode, batchNo.BatchNo);//OmUtlSHPOKHdrBLL.GetRecord(CompanyCode, BranchCode, standardCode,CompanyCode, batchNo);
            if (utl == null)
            {
                msg = "Data upload tidak ditemukan";
                
                return Json(new { success = false, message = msg });
            }
           
            using (var tran = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    model.CreatedDate = currentTime;
                    model.LastUpdateDate = currentTime;
                    model.CreatedBy = userID;
                    model.LastUpdateDate = currentTime;
                    model.LastUpdateBy = userID;
                    model.CompanyCode = CompanyCode;
                    model.BranchCode = BranchCode;
                    model.HPPDate = model.HPPDate != null ? model.HPPDate : Convert.ToDateTime("1900/01/01");
                    model.PONo = model.PONo;
                    model.SupplierCode = model.SupplierCode;
                    model.BillTo = model.BillTo != null ? model.BillTo : "";
                    model.RefferenceInvoiceNo = model.RefferenceInvoiceNo;
                    model.RefferenceInvoiceDate = model.RefferenceInvoiceDate != null ? model.RefferenceInvoiceDate : Convert.ToDateTime("1900/01/01");
                    model.RefferenceFakturPajakNo = model.RefferenceFakturPajakNo != null ? model.RefferenceFakturPajakNo : "";
                    model.RefferenceFakturPajakDate = model.RefferenceFakturPajakDate != null ? model.RefferenceFakturPajakDate : Convert.ToDateTime("1900/01/01");
                    model.DueDate = model.DueDate;
                    model.Remark = model.Remark != null ? model.Remark : "";
                    model.Status = "0";
                    model.isLocked = true;
                    model.LockingBy = "";
                    model.LockingDate = model.LockingDate ?? DateTime.Parse("1900/01/01");
                    
                    if (p_SaveHPPUpload(model, BatchNo, ref msg))
                    {
                        tran.Commit();
                    }
                    else{
                        tran.Rollback();
                    }
                        //}
                    //}


                    var me = ctx.omTrPurchaseHPP.Find(CompanyCode, BranchCode, model.HPPNo);
                    return Json(new { success = true, data = me, message = msg });
                }
                catch (Exception Ex)
                {
                    tran.Rollback();

                    return Json(new { success = false, message = Ex.Message });
                }
            }
        }

        private bool p_SaveHPPUpload(omTrPurchaseHPP record, string batchNo, ref string msg)
        {
            bool result = false;

            record.HPPNo = GetNewDocumentNo("HPU", record.HPPDate.Value);
            if (record.HPPNo.EndsWith("X")) throw new ApplicationException(string.Format(GetMessage(SysMessages.MSG_5046), GnMstDocumentConstant.HPU));

            Helpers.ReplaceNullable(record);
            ctx.omTrPurchaseHPP.Add(record);

            if (ctx.SaveChanges() > 0)
                if (p_InserHPPDetail(batchNo, record, ref msg))
                    //if (InserHPPDetailModel(ctx, companyCode, branchCode, batchNo, record.HPPNo, record.PONo, record.RefferenceInvoiceNo))
                    //    if (InserHPPSubDetail(ctx, companyCode, branchCode, batchNo, record.HPPNo, record.RefferenceInvoiceNo))
                    //    {
                    result = true;

            
            return result;
        }


        private bool p_InserHPPDetail(string batchNo, omTrPurchaseHPP record, ref string errMsg)
        {
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            string HPPNo = record.HPPNo;
            string PONo = record.PONo;
            string invoiceNo = record.RefferenceInvoiceNo;

            bool result = false; errMsg = "";
            string UserId = CurrentUser.UserId;
            OmUtlSHPOKDtl1 utlDtl1 = ctx.OmUtlSHPOKDtl1s.Find(companyCode, branchCode, batchNo, invoiceNo);
            if (utlDtl1 != null)
            {
                utlDtl1.Status = "1";
                utlDtl1.LastUpdateBy = UserId;
                utlDtl1.LastUpdateDate = DateTime.Now;

                if (ctx.SaveChanges() > 0)
                {
                    var recHPPDtl = p_GetHPPDetail(companyCode, branchCode, batchNo, invoiceNo);
                    if (recHPPDtl.Count() > 0)
                    {
                        foreach (var row in recHPPDtl)
                        {
                            string BPUNo = "";
                            if (row.DocumentType == "1")
                            {
                                var recBpu = ctx.omTrPurchaseBPU.Where(x => x.CompanyCode == companyCode && x.BranchCode == branchCode 
                                    && x.RefferenceDONo == row.DocumentNo && x.Status != "3").FirstOrDefault();
                                BPUNo = recBpu != null ? recBpu.BPUNo : "";
                            }
                            else
                            {
                                var recBpu = ctx.omTrPurchaseBPU.Where(x => x.CompanyCode == companyCode && x.BranchCode == branchCode 
                                    && x.RefferenceSJNo == row.DocumentNo && x.Status != "3").FirstOrDefault();
                                BPUNo = recBpu != null ? recBpu.BPUNo : "";
                            }
                            if (BPUNo == "")
                            {
                                //result = false; errMsg = "Quantity HPP tidak sama dengan quantity BPU"; break; 
                                throw new Exception("Quantity HPP tidak sama dengan quantity BPU");
                            }

                            var detail = new omTrPurchaseHPPDetail();
                            detail.CompanyCode = companyCode;
                            detail.BranchCode = branchCode;
                            detail.HPPNo = HPPNo;
                            detail.BPUNo = BPUNo;
                            detail.Remark = "";
                            detail.CreatedBy = UserId;
                            detail.CreatedDate = DateTime.Now;
                            detail.LastUpdateBy = UserId;
                            detail.LastUpdateDate = DateTime.Now;
                            
                            Helpers.ReplaceNullable(detail);
                            ctx.omTrPurchaseHPPDetail.Add(detail);
                            
                            if (ctx.SaveChanges() < 1)
                            {
                                //result = false;
                                //errMsg = "Gagal save HPP Detail Model";
                                //break;
                                throw new Exception("Gagal save HPP Detail Model");
                            }
                            else
                            {
                                var dtDtl3 = p_GetHPPDetailModel(companyCode, branchCode, batchNo, invoiceNo, row.DocumentNo);
                                if (dtDtl3.Count() > 0)
                                {
                                    foreach (var rowDtl3 in dtDtl3)
                                    {
                                        var detailModel = new omTrPurchaseHPPDetailModel();
                                        detailModel.CompanyCode = CompanyCode;
                                        detailModel.BranchCode = BranchCode;
                                        detailModel.HPPNo = HPPNo;
                                        detailModel.BPUNo = BPUNo;
                                        detailModel.SalesModelCode = rowDtl3.SalesModelCode;
                                        detailModel.SalesModelYear = rowDtl3.SalesModelYear;
                                        detailModel.Quantity = rowDtl3.Quantity;
                                        detailModel.BeforeDiscDPP = rowDtl3.BeforeDiscountDPP;
                                        detailModel.DiscExcludePPn = rowDtl3.DiscountExcludePPN;
                                        detailModel.AfterDiscDPP = rowDtl3.AfterDiscountDPP;
                                        detailModel.AfterDiscPPn = rowDtl3.AfterDiscountPPN;
                                        detailModel.AfterDiscPPnBM = rowDtl3.AfterDiscountPPNBM;
                                        detailModel.AfterDiscTotal = rowDtl3.AfterDiscountTotal;
                                        detailModel.PPnBMPaid = rowDtl3.PPNBMPaid;
                                        detailModel.OthersDPP = rowDtl3.OthersDPP;
                                        detailModel.OthersPPn = rowDtl3.OthersPPN;

                                        detailModel.Remark = "";
                                        detailModel.CreatedBy = UserId;
                                        detailModel.CreatedDate = DateTime.Now;
                                        detailModel.LastUpdateBy = UserId;
                                        detailModel.LastUpdateDate = DateTime.Now;

                                        Helpers.ReplaceNullable(detailModel);
                                        ctx.omTrPurchaseHPPDetailModel.Add(detailModel);

                                        if (ctx.SaveChanges() < 1)
                                        {
                                            //result = false;
                                            //errMsg = "Gagal save HPP Detail Model";
                                            //break;
                                            throw new Exception("Gagal save HPP Detail Model");
                                        }
                                        else
                                        {
                                            var BPUDetailModel = ctx.OmTrPurchaseBPUDetailModels.Find(
                                                companyCode, branchCode, PONo, detailModel.BPUNo, detailModel.SalesModelCode,
                                                detailModel.SalesModelYear);
                                            if (BPUDetailModel != null)
                                            {
                                                BPUDetailModel.QuantityHPP = detailModel.Quantity ?? 0;
                                                if ((ctx.SaveChanges() < 0))
                                                {
                                                    //errMsg = "Gagal update HPP Detail Model";
                                                    //result = false; break;
                                                    throw new Exception("Gagal update HPP Detail Model");
                                                }
                                                else
                                                {
                                                    if (BPUDetailModel.QuantityHPP > BPUDetailModel.QuantityBPU)
                                                    {
                                                        //errMsg = "Quantity HPP tidak sama dengan quantity BPU";
                                                        //result = false; break;
                                                        throw new Exception("Quantity HPP tidak sama dengan quantity BPU");
                                                    }
                                                    else
                                                    {
                                                        //Add new Data into OmTrPurchaseHPPDetailModelOthers
                                                        var dtO = p_GetHPPDetailModelOthers(companyCode, branchCode, batchNo, invoiceNo, rowDtl3.DocumentNo,
                                                                            rowDtl3.SalesModelCode, (int)rowDtl3.SalesModelYear);
                                                        if (dtO.Count() > 0)
                                                        {
                                                            foreach (var rowDtlO in dtO)
                                                            {
                                                                var oOmTrPurchaseHPPDetailModelOthers = new omTrPurchaseHPPDetailModelOthers();
                                                                oOmTrPurchaseHPPDetailModelOthers.CompanyCode = CompanyCode;
                                                                oOmTrPurchaseHPPDetailModelOthers.BranchCode = BranchCode;
                                                                oOmTrPurchaseHPPDetailModelOthers.BPUNo = BPUNo;
                                                                oOmTrPurchaseHPPDetailModelOthers.HPPNo = HPPNo;
                                                                oOmTrPurchaseHPPDetailModelOthers.SalesModelCode = rowDtlO.SalesModelCode;
                                                                oOmTrPurchaseHPPDetailModelOthers.SalesModelYear = rowDtlO.SalesModelYear;
                                                                oOmTrPurchaseHPPDetailModelOthers.OthersCode = rowDtlO.OthersCode;
                                                                oOmTrPurchaseHPPDetailModelOthers.OthersDPP = rowDtlO.OthersDPP;
                                                                oOmTrPurchaseHPPDetailModelOthers.OthersPPN = rowDtlO.OthersPPN;
                                                                oOmTrPurchaseHPPDetailModelOthers.Remark = string.Empty;
                                                                oOmTrPurchaseHPPDetailModelOthers.CreatedBy = UserId;
                                                                oOmTrPurchaseHPPDetailModelOthers.CreatedDate = DateTime.Now;
                                                                oOmTrPurchaseHPPDetailModelOthers.LastUpdateBy = UserId;
                                                                oOmTrPurchaseHPPDetailModelOthers.LastUpdateDate = DateTime.Now;

                                                                Helpers.ReplaceNullable(oOmTrPurchaseHPPDetailModelOthers);
                                                                ctx.omTrPurchaseHPPDetailModelOthers.Add(oOmTrPurchaseHPPDetailModelOthers);

                                                                if (ctx.SaveChanges() < 1)
                                                                {
                                                                    //result = false;
                                                                    //errMsg = "Gagal Save HPP Detail Model Others";
                                                                    //break;
                                                                    throw new Exception("Gagal Save HPP Detail Model Others");
                                                                }
                                                            }
                                                        }

                                                        //TODO insert sub detail
                                                        var dtDtl4 = p_GetHPPSubDetail(companyCode, branchCode, batchNo, invoiceNo, rowDtl3.DocumentNo,
                                                            rowDtl3.SalesModelCode, rowDtl3.SalesModelYear);
                                                        if (dtDtl4.Count() > 0)
                                                        {
                                                            foreach (var rowDtl4 in dtDtl4)
                                                            {
                                                                //OmMstVehicle oOmMstVehicle = OmMstVehicleBLL.GetRecord(ctx, CompanyCode, rowDtl4.ChassisCode,
                                                                //    decimal.Parse(rowDtl4.ChassisNo));

                                                                //if(oOmMstVehicle != null)
                                                                //    throw new Exception("Data sudah di Master Vehicle");

                                                                var subDetail = new omTrPurchaseHPPSubDetail();
                                                                subDetail.CompanyCode = CompanyCode;
                                                                subDetail.BranchCode = BranchCode;
                                                                subDetail.HPPNo = HPPNo;
                                                                subDetail.BPUNo = BPUNo;
                                                                subDetail.HPPSeq = GetSeq(ctx, companyCode, branchCode, HPPNo, subDetail.BPUNo);
                                                                subDetail.SalesModelCode = rowDtl4.SalesModelCode;
                                                                subDetail.SalesModelYear = rowDtl4.SalesModelYear;

                                                                subDetail.ColourCode = rowDtl4.ColourCode;
                                                                subDetail.ChassisCode = rowDtl4.ChassisCode;
                                                                subDetail.ChassisNo = rowDtl4.ChassisNo;
                                                                subDetail.EngineCode = rowDtl4.EngineCode;
                                                                subDetail.EngineNo = rowDtl4.EngineNo;

                                                                var rowBPUDetail = ctx.omTrPurchaseBPUDetail.Where(x => x.CompanyCode == companyCode && x.BranchCode == branchCode
                                                                    && x.BPUNo == BPUNo).ToList();

                                                                if (rowBPUDetail.Count() > 0)
                                                                {
                                                                    var rowBPUDetail_1 = rowBPUDetail.FirstOrDefault();
                                                                    if (rowBPUDetail_1 != null) {
                                                                        if (rowBPUDetail_1.ChassisNo != 0)
                                                                        {
                                                                            if (subDetail.ChassisNo != 0)
                                                                            {
                                                                                if (rowBPUDetail_1.ChassisNo != subDetail.ChassisNo)
                                                                                {
                                                                                    //result = false;
                                                                                    //errMsg = "Gagal save karena perbedaan ChassisNo di BPU dan SubDetail HPP";
                                                                                    //break;
                                                                                    throw new Exception("Gagal save karena perbedaan ChassisNo di BPU dan SubDetail HPP");
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                //subDetail.ChassisCode = rowBPUDetail.ChassisCode;
                                                                                subDetail.ChassisNo = rowBPUDetail_1.ChassisNo;
                                                                                //subDetail.EngineCode = rowBPUDetail.EngineCode;
                                                                                subDetail.EngineNo = rowBPUDetail_1.EngineNo;
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        throw new Exception("Gagal save, Purchase BPU Detail tidak ditemukan");
                                                                    }
                                                                }

                                                                subDetail.Remark = "";
                                                                subDetail.CreatedBy = UserId;
                                                                subDetail.CreatedDate = DateTime.Now;
                                                                subDetail.LastUpdateBy = UserId;
                                                                subDetail.LastUpdateDate = DateTime.Now;

                                                                Helpers.ReplaceNullable(subDetail);
                                                                ctx.omTrPurchaseHPPSubDetail.Add(subDetail);

                                                                if (ctx.SaveChanges() < 1)
                                                                {
                                                                    //result = false;
                                                                    //errMsg = "Gagal Save HPP Sub Detail";
                                                                    //break;
                                                                    throw new Exception("Gagal Save HPP Sub Detail");
                                                                }
                                                                else
                                                                {
                                                                    //todo : update master vehicle
                                                                    var BPU = ctx.omTrPurchaseBPU.Find(companyCode, branchCode, PONo, BPUNo);
                                                                    string BPUType = "";
                                                                    if (BPU != null)
                                                                    { BPUType = BPU.BPUType; }
                                                                    if (!BPUType.Equals("0"))
                                                                    {
                                                                        if (!p_UpdateOmMstVehicle(companyCode, subDetail.ChassisCode,
                                                                            subDetail.ChassisNo, detailModel))
                                                                        {
                                                                            //result = false;
                                                                            //errMsg = "Gagal update Master Vehicle";
                                                                            //break; 
                                                                            throw new Exception("Gagal update Master Vehicle");
                                                                        }
                                                                        else result = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (!p_UpdateOmMstVehicleTemp(companyCode, subDetail.ChassisCode,
                                                                            subDetail.ChassisNo, detailModel, int.Parse(subDetail.HPPSeq.ToString())))
                                                                        {
                                                                            //result = false;
                                                                            //errMsg = "Gagal update Master Vehicle Temp";
                                                                            //break; 
                                                                            throw new Exception("Gagal update Master Vehicle Temp");
                                                                        }
                                                                        else result = true;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        private static int GetSeq(DataContext ctxx, string companyCode, string branchCode, string HPPNo, string BPUNo)
        {
            var qry = @"SELECT ISNULL (MAX (HPPSeq), 0)  AS BPUSeq FROM omTrPurchaseHPPSubDetail WITH (NOLOCK, NOWAIT)
                         WHERE CompanyCode = '" + companyCode + "' AND BranchCode = '" + branchCode + "' AND HPPNo = '" + HPPNo + "' AND BPUNo = '" + BPUNo + "'";
            decimal No = ctxx.Database.SqlQuery<decimal>(qry).FirstOrDefault();
            return Convert.ToInt32(No) + 1;
        }

        public JsonResult Delete(omTrPurchaseHPP model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            //string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.omTrPurchaseHPP.Find(companyCode, BranchCode, model.HPPNo);
                    if (me != null)
                    {
                        me.LastUpdateDate = DateTime.Now;
                        me.LastUpdateBy = CurrentUser.UserId;
                        me.Status = "3";

                        //DeleteSubDetail
                        ctx.omTrPurchaseHPPSubDetail
                        .Where(x => x.CompanyCode == CompanyCode
                                && x.BranchCode == BranchCode
                                && x.HPPNo == model.HPPNo)
                        .ToList()
                        .ForEach(x => ctx.omTrPurchaseHPPSubDetail.Remove(x));

                        ctx.omTrPurchaseBPUDetail
                          .Where(x => x.CompanyCode == CompanyCode
                                    && x.BranchCode == BranchCode
                                    && x.PONo == model.PONo)
                          .ToList()
                          .ForEach(x => x.StatusHPP = "0");

                        //DeleteDetailModel
                        ctx.omTrPurchaseHPPDetailModel
                            .Where(x => x.CompanyCode == CompanyCode
                                       && x.BranchCode == BranchCode
                                       && x.HPPNo == model.HPPNo)
                            .ToList()
                            .ForEach(x => ctx.omTrPurchaseHPPDetailModel.Remove(x));

                        //DeleteDetailBPU
                        ctx.omTrPurchaseHPPDetail
                            .Where(x => x.CompanyCode == CompanyCode
                                       && x.BranchCode == BranchCode
                                       && x.HPPNo == model.HPPNo)
                            .ToList()
                            .ForEach(x => ctx.omTrPurchaseHPPDetail.Remove(x));

                        //UpdateModelBPU
                        ctx.OmTrPurchaseBPUDetailModels
                          .Where(x => x.CompanyCode == CompanyCode
                                    && x.BranchCode == BranchCode
                                    && x.PONo == model.PONo)
                          .ToList()
                          .ForEach(x => x.QuantityHPP = 0);


                        //UpdateUtlHPP
                        ctx.OmUtlSHPOKDtl1s
                          .Where(x => x.CompanyCode == CompanyCode
                                    && x.BranchCode == BranchCode
                                    && x.InvoiceNo == me.RefferenceInvoiceNo)
                          .ToList()
                          .ForEach(x => x.Status = "0");


                        //ctx.omTrPurchaseHPP.Remove(me);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data HPP berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete HPP, Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete HPP, Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }

        [HttpPost]
        public JsonResult Save2(omTrPurchaseHPPDetail model)
        {
            //ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            var me = ctx.omTrPurchaseHPPDetail.Find(CompanyCode, BranchCode, model.HPPNo, model.BPUNo);

            if (me == null)
            {
                me = new omTrPurchaseHPPDetail();
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreatedBy = userID;
                ctx.omTrPurchaseHPPDetail.Add(me);
            }
            else
            {
                me.LastUpdateDate = currentTime;
                me.LastUpdateBy = userID;
            }
            me.CompanyCode = CompanyCode;
            me.BranchCode = BranchCode;
            me.HPPNo = model.HPPNo;
            me.BPUNo = model.BPUNo;
            me.Remark = model.Remark;

            try
            {
                Helpers.ReplaceNullable(model);
                ctx.SaveChanges();

                return Json(new { success = true, data = me });
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }

        public JsonResult Delete2(omTrPurchaseHPPDetail model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            //string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.omTrPurchaseHPPDetail.Find(CompanyCode, BranchCode, model.HPPNo, model.BPUNo);
                    if (me != null)
                    {
                        ctx.omTrPurchaseHPPDetail.Remove(me);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data HPP berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete HPP, Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete HPP, Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }

        [HttpPost]
        public JsonResult Save3(omTrPurchaseHPPDetailModel model)
        {
            //ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            var me = ctx.omTrPurchaseHPPDetailModel.Find(CompanyCode, BranchCode, model.HPPNo, model.BPUNo, model.SalesModelCode, model.SalesModelYear);

            if (me == null)
            {
                me = new omTrPurchaseHPPDetailModel();
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreatedBy = userID;
                ctx.omTrPurchaseHPPDetailModel.Add(me);
            }
            else
            {
                me.LastUpdateDate = currentTime;
                me.LastUpdateBy = userID;
            }
            me.CompanyCode = CompanyCode;
            me.BranchCode = BranchCode;
            me.HPPNo = model.HPPNo;
            me.BPUNo = model.BPUNo;
            me.SalesModelCode = model.SalesModelCode;
            me.SalesModelYear = model.SalesModelYear;
            me.Quantity = model.Quantity;
            me.BeforeDiscDPP = model.BeforeDiscDPP;
            me.DiscExcludePPn = model.DiscExcludePPn != null ? model.DiscExcludePPn : 0;
            me.AfterDiscDPP = model.AfterDiscDPP;
            me.AfterDiscPPn = model.AfterDiscPPn;
            me.AfterDiscPPnBM = model.AfterDiscPPnBM;
            me.AfterDiscTotal = model.AfterDiscTotal;
            me.PPnBMPaid = model.PPnBMPaid;
            me.OthersDPP = model.OthersDPP != null ? model.OthersDPP : 0;
            me.OthersPPn = model.OthersPPn != null ? model.OthersPPn : 0;
            me.Remark = model.Remark;

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, data = me });
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }

        public JsonResult Delete3(omTrPurchaseHPPDetailModel model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            //string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.omTrPurchaseHPPDetailModel.Find(CompanyCode, BranchCode, model.HPPNo, model.BPUNo, model.SalesModelCode, model.SalesModelYear);
                    if (me != null)
                    {
                        ctx.omTrPurchaseHPPDetailModel.Remove(me);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data HPP Detil Sales Model berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete HPP Detil Sales Model, Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete HPP Detil Sales Model, Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }

        [HttpPost]
        public JsonResult Save4(omTrPurchaseHPPSubDetail model)
        {
            //ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            var hdr=ctx.omTrPurchaseHPP.Find(CompanyCode, BranchCode,model.HPPNo);
            
            var me = ctx.omTrPurchaseHPPSubDetail.Find(CompanyCode, BranchCode, model.HPPNo, model.BPUNo, model.HPPSeq);
            var my = ctx.omTrPurchaseBPUDetail
                .Where(m => m.CompanyCode==CompanyCode
                            && m.BranchCode == BranchCode
                            && m.BPUNo == model.BPUNo                            
                            && m.SalesModelCode == model.SalesModelCode
                            && m.SalesModelYear==model.SalesModelYear
                            && m.ChassisCode == model.ChassisCode
                            && m.ChassisNo == model.ChassisNo
                            && m.EngineNo == model.EngineNo
                            ).FirstOrDefault();
            var bp = ctx.omTrPurchaseBPUDetail.Find(CompanyCode, BranchCode, hdr.PONo, model.BPUNo,my.BPUSeq);
            bp.StatusHPP = "1";
            if (me == null)
            {
                me = new omTrPurchaseHPPSubDetail();
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreatedBy = userID;
                me.HPPSeq = my.BPUSeq;
                ctx.omTrPurchaseHPPSubDetail.Add(me);
            }
            else
            {
                me.LastUpdateDate = currentTime;
                me.LastUpdateBy = userID;
            }
            me.CompanyCode = CompanyCode;
            me.BranchCode = BranchCode;
            me.HPPNo = model.HPPNo;
            me.BPUNo = model.BPUNo;
         
            me.SalesModelCode = model.SalesModelCode;
            me.SalesModelYear = model.SalesModelYear;
            me.ColourCode = my.ColourCode;
            me.ChassisCode = my.ChassisCode;
            me.ChassisNo = my.ChassisNo;
            me.EngineCode = my.EngineCode;
            me.EngineNo = my.EngineNo;
            me.Remark = my.Remark;
            me.isReturn = my.isReturn;

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, data = me });
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SaveHppSubFromAllBpuDetail(omTrPurchaseHPPSubDetail model)
        {
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;


            try
            {
               var hdr=ctx.omTrPurchaseHPP.Find(CompanyCode, BranchCode,model.HPPNo);
                ctx.omTrPurchaseBPUDetail
                       .Where(m => m.BPUNo == model.BPUNo && m.SalesModelCode == model.SalesModelCode && m.SalesModelYear == model.SalesModelYear)
                       .ToList()
                       .ForEach(x =>
                       {
                           var me = ctx.omTrPurchaseHPPSubDetail.Find(CompanyCode, BranchCode, model.HPPNo, model.BPUNo, x.BPUSeq);
                           if (me == null)
                           {
                               me = new omTrPurchaseHPPSubDetail();
                               me.CreatedDate = currentTime;
                               me.LastUpdateDate = currentTime;
                               me.CreatedBy = userID;
                               ctx.omTrPurchaseHPPSubDetail.Add(me);
                           }
                           else
                           {
                               me.LastUpdateDate = currentTime;
                               me.LastUpdateBy = userID;
                           }
                           me.CompanyCode = CompanyCode;
                           me.BranchCode = BranchCode;
                           me.HPPNo = model.HPPNo;
                           me.BPUNo = model.BPUNo;
                           me.HPPSeq = x.BPUSeq;
                           me.SalesModelCode = model.SalesModelCode;
                           me.SalesModelYear = model.SalesModelYear;
                           me.ColourCode = x.ColourCode;
                           me.ChassisCode = x.ChassisCode;
                           me.ChassisNo = x.ChassisNo;
                           me.EngineCode = x.EngineCode;
                           me.EngineNo = x.EngineNo;
                           me.Remark = x.Remark;
                           me.isReturn = x.isReturn;

                           var bp = ctx.omTrPurchaseBPUDetail.Find(CompanyCode, BranchCode, hdr.PONo, model.BPUNo, x.BPUSeq);
                           bp.StatusHPP = "1";

                       });            
                


                ctx.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }
        
        public JsonResult DeleteAllbyHppDetail(omTrPurchaseHPPDetailModel model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            //string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    ctx.omTrPurchaseHPPSubDetail
                        .Where(x => x.CompanyCode == model.CompanyCode
                                && x.BranchCode == model.BranchCode
                                && x.HPPNo == model.HPPNo
                                && x.BPUNo == model.BPUNo
                                && x.SalesModelCode == model.SalesModelCode
                                && x.SalesModelYear == model.SalesModelYear)
                        .ToList()
                        .ForEach(x => ctx.omTrPurchaseHPPSubDetail.Remove(x));
                    ctx.SaveChanges();
                    returnObj = new { success = true, message = "Data HPP Sub Detil Model berhasil di delete." };
                    trans.Complete();                    
                    trans.Dispose();
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete HPP Sub Detil Model, Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }
        
        public JsonResult Delete4(omTrPurchaseHPPSubDetail model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            //string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.omTrPurchaseHPPSubDetail.Find(model.CompanyCode, model.BranchCode, model.HPPNo, model.BPUNo, model.HPPSeq);
                    if (me != null)
                    {
                        var hdr = ctx.omTrPurchaseHPP.Find(CompanyCode, BranchCode, model.HPPNo);
                        var bp = ctx.omTrPurchaseBPUDetail.Find(CompanyCode, BranchCode, hdr.PONo, model.BPUNo, me.HPPSeq);
                        bp.StatusHPP = "0";
                        ctx.omTrPurchaseHPPSubDetail.Remove(me);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data HPP Sub Detil Model berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete  HPP Sub Detil Model, Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete HPP Sub Detil Model, Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }

        [HttpPost]
        public JsonResult Save5(omTrPurchaseHPPDetailModelOthers model)
        {
            //ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            var me = ctx.omTrPurchaseHPPDetailModelOthers.Find(CompanyCode, BranchCode, model.HPPNo, model.BPUNo, model.SalesModelCode, model.SalesModelYear, model.OthersCode);

            if (me == null)
            {
                me = new omTrPurchaseHPPDetailModelOthers();
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreatedBy = userID;
                ctx.omTrPurchaseHPPDetailModelOthers.Add(me);
            }
            else
            {
                me.LastUpdateDate = currentTime;
                me.LastUpdateBy = userID;
            }
            me.CompanyCode = CompanyCode;
            me.BranchCode = BranchCode;
            me.HPPNo = model.HPPNo;
            me.BPUNo = model.BPUNo;
            me.SalesModelCode = model.SalesModelCode;
            me.SalesModelYear = model.SalesModelYear;
            me.OthersCode = model.OthersCode;
            me.OthersDPP = model.OthersDPP;
            me.OthersPPN = model.OthersPPN;
            me.Remark = model.Remark;
            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, data = me });
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }

        public JsonResult Delete5(omTrPurchaseHPPDetailModelOthers model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            //string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.omTrPurchaseHPPDetailModelOthers.Find(CompanyCode, BranchCode, model.HPPNo, model.BPUNo, model.SalesModelCode, model.SalesModelYear, model.OthersCode);
                    if (me != null)
                    {
                        ctx.omTrPurchaseHPPDetailModelOthers.Remove(me);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data HPP Sub Detil Model berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete  HPP Sub Detil Model, Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete HPP Sub Detil Model, Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }


        public JsonResult PrintValidate(string HPPNo)
        {
            try
            {
                //check whether detail HPP data doesn't exist
                var dtDetail = ctx.omTrPurchaseHPPDetail.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.HPPNo == HPPNo);
                if (dtDetail.Count() < 1)
                {
                    return Json(new { success = false, message = GetMessage(SysMessages.MSG_5047) });
                }

                // check if BPU doesn't have detail in HPP
                var dtBPU = (from a in ctx.omTrPurchaseHPPDetail
                             join b in ctx.omTrPurchaseHPPDetailModel on new { a.CompanyCode, a.BranchCode, a.BPUNo } equals new { b.CompanyCode, b.BranchCode, b.BPUNo } into _b
                             from b in _b.DefaultIfEmpty()
                             where a.BranchCode == BranchCode
                                 && a.HPPNo == HPPNo
                                 && b.Quantity == 0
                             select new { a.HPPNo, a.BPUNo, b.Quantity }).ToList();


                if (dtBPU.Count() > 0)
                {
                    string msg = "Terdapat BPU: ";
                    foreach (var row in dtBPU)
                        msg += row.BPUNo + ", ";
                    msg += "\nyang belum mempunyai detail model di HPP.";
                    
                    return Json(new { success = false, message = msg });
                }

                var record = ctx.omTrPurchaseHPP.Find(CompanyCode, BranchCode, HPPNo);
                if (record == null)
                    return Json(new { success = false, message = "Data HPP tidak ditemukan" });
                
              
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        #region == Private Method ==

        private bool p_IsExistInvNo(string reffInvNo)
        {
            var rec = ctx.omTrPurchaseHPP.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.Status != "3" && x.RefferenceInvoiceNo == reffInvNo);
            if (rec.Count() > 0)
                return true;
            else return false;
        }

        private List<OmUtlSHPOKDtl2> p_GetHPPDetail(string companyCode, string branchCode, string batchNo, string invoiceNo)
        {
            var records = ctx.OmUtlSHPOKDtl2s.Where(x => BranchCode == branchCode && x.CompanyCode == companyCode 
                && x.BatchNo == batchNo && x.InvoiceNo == invoiceNo);

            return records.ToList();
        }

        private List<OmUtlSHPOKDtl3> p_GetHPPDetailModel(string companyCode, string branchCode, string batchNo, string invoiceNo, string documentNo)
        {
            var records = ctx.OmUtlSHPOKDtl3s.Where(x => BranchCode == branchCode && x.CompanyCode == companyCode
                && x.BatchNo == batchNo && x.InvoiceNo == invoiceNo && x.DocumentNo == documentNo);

            return records.ToList();
        }

        private List<OmUtlSHPOKDtlO> p_GetHPPDetailModelOthers(string companyCode, string branchCode,
            string batchNo, string invoiceNo, string documentNo,string salesModelCode,int salesModelYear)
        {
            var records = ctx.OmUtlSHPOKDtlOs.Where(x => BranchCode == branchCode && x.CompanyCode == companyCode
                && x.BatchNo == batchNo && x.InvoiceNo == invoiceNo && x.DocumentNo == documentNo 
                && x.SalesModelCode == salesModelCode && x.SalesModelYear == salesModelYear);

            return records.ToList();
        }

        private List<OmUtlSHPOKDtl4> p_GetHPPSubDetail(string companyCode, string branchCode,
            string batchNo, string invoiceNo, string documentNo, string salesModelCode, decimal salesModelYear)
        {
             var records = ctx.OmUtlSHPOKDtl4s.Where(x => BranchCode == branchCode && x.CompanyCode == companyCode
                && x.BatchNo == batchNo && x.InvoiceNo == invoiceNo && x.DocumentNo == documentNo 
                && x.SalesModelCode == salesModelCode && x.SalesModelYear == salesModelYear);

            return records.ToList();
        }

        private bool p_UpdateOmMstVehicle(string companyCode, string chassisCode, decimal chassisNo,
            omTrPurchaseHPPDetailModel detailModelRecord)
        {
            return true;
            #region -- This process was moved to Approve --
            //bool result = false;
            //OmMstVehicleDao oOmMstVehicleDao = new OmMstVehicleDao(ctx);
            //OmMstVehicle oOmMstVehicle = oOmMstVehicleDao.GetRecord(companyCode, chassisCode, chassisNo);
            //if (oOmMstVehicle != null)
            //{
            //    oOmMstVehicle.COGSUnit = detailModelRecord.AfterDiscDPP;
            //    oOmMstVehicle.COGSOthers = detailModelRecord.OthersDPP;
            //    oOmMstVehicle.PpnBmBuyPaid = detailModelRecord.PPnBMPaid;
            //    oOmMstVehicle.PpnBmBuy = detailModelRecord.AfterDiscPPnBM;
            //    oOmMstVehicle.HPPNo = detailModelRecord.HPPNo;

            //    if (oOmMstVehicleDao.Update(oOmMstVehicle) >= 0)
            //        result = true;
            //}
            //return result; 
            #endregion
        }

        private bool p_UpdateOmMstVehicleTemp(string companyCode, string chassisCode, decimal chassisNo,
            omTrPurchaseHPPDetailModel detailModelRecord, int HPPSeq)
        {
            return true;
            #region -- This Process was moved to Approve --
            //            bool result = false;
            //            OmMstVehicleTempDao oOmMstVehicleTempDao = new OmMstVehicleTempDao(ctx);
            //            ctx.CommandText = @"select RefferenceDONo from omTrPurchaseBPU where 
            //                    companyCode = @CompanyCode and branchCode = @BranchCode and
            //                    BPUNo = @BPUNo";
            //            ctx.Add("@CompanyCode", detailModelRecord.CompanyCode);
            //            ctx.Add("@BranchCode", detailModelRecord.BranchCode);
            //            ctx.Add("@BPUNo", detailModelRecord.BPUNo);
            //            string reffDONo = "";
            //            object obj = DaoBase.ExecuteScalar(ctx);
            //            if (obj != null)
            //                reffDONo = obj.ToString() + "-" + HPPSeq.ToString();

            //            OmMstVehicleTemp oOmMstVehicleTemp = oOmMstVehicleTempDao.GetRecord(companyCode, chassisCode, reffDONo);
            //            if (oOmMstVehicleTemp != null)
            //            {
            //                oOmMstVehicleTemp.COGSUnit = detailModelRecord.AfterDiscDPP;
            //                oOmMstVehicleTemp.COGSOthers = detailModelRecord.OthersDPP;
            //                oOmMstVehicleTemp.PpnBmBuyPaid = detailModelRecord.PPnBMPaid;
            //                oOmMstVehicleTemp.PpnBmBuy = detailModelRecord.AfterDiscPPnBM;
            //                oOmMstVehicleTemp.HPPNo = detailModelRecord.HPPNo;

            //                if (oOmMstVehicleTempDao.Update(oOmMstVehicleTemp) >= 0)
            //                    result = true;
            //            }
            //            return result; 
            #endregion
        }

        #endregion
    }
}
