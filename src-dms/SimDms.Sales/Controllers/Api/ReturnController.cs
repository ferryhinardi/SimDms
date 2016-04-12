using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Sales.Models;
using System.Transactions;

namespace SimDms.Sales.Controllers.Api
{
    public class ReturnController : BaseController
    {
        public JsonResult Save(omTrSalesReturn mdl)
        {
            try
            {
                string serr = "";
                if (!DateTransValidation((DateTime)mdl.ReturnDate, ref serr))
                {
                    return Json(new { success = false, message = serr });
                }


                if (ctx.Database.SqlQuery<omSlsInvcLkp>(string.Format("exec uspfn_omSlsReturnLkpInvoice {0},{1}",
                          CompanyCode, BranchCode))
                 .Where(x => x.InvoiceNo == mdl.InvoiceNo)
                 .FirstOrDefault() == null)
                {
                    return Json(new { success = false, message = "Invalid Invoice No" });
                }


                if (ctx.LookUpDtls
                .Where(x => x.CompanyCode == CompanyCode &&
                            x.CodeID == "MPWH" &&
                            x.ParaValue == BranchCode &&
                            x.LookUpValue == mdl.WareHouseCode)
               .FirstOrDefault() == null)
                {
                    return Json(new { success = false, message = "Invalid Invoice No" });
                }

                var hdr = ctx.omTrSalesReturn.Find(CompanyCode, BranchCode, mdl.ReturnNo); 
                if(hdr==null)
                {
                    hdr = new omTrSalesReturn();
                    hdr.CompanyCode = CompanyCode;
                    hdr.BranchCode = BranchCode;
                    hdr.ReturnDate = mdl.ReturnDate;
                    hdr.InvoiceNo = mdl.InvoiceNo;
                    hdr.FakturPajakNo = mdl.FakturPajakNo;
                    hdr.FakturPajakDate = mdl.FakturPajakDate;
                    hdr.CustomerCode = mdl.CustomerCode;
                    hdr.InvoiceDate = mdl.InvoiceDate;
                    hdr.ReturnNo = GetNewDocumentNo("RTS", (DateTime)mdl.ReturnDate);
                    hdr.CreatedBy = CurrentUser.UserId;
                    hdr.CreatedDate = ctx.CurrentTime;
                    hdr.isLocked = false;
                    ctx.omTrSalesReturn.Add(hdr);
                }

                hdr.WareHouseCode = mdl.WareHouseCode;
                hdr.Remark = mdl.Remark.ToUpper();
                hdr.Status = "0";

                hdr.LastUpdateBy = CurrentUser.UserId;
                hdr.LastUpdateDate = ctx.CurrentTime;

                ctx.SaveChanges();
                return Json(new { success = true, ReturnNo = hdr.ReturnNo, Status = hdr.Status, StatusDsc = getStringStatus(hdr.Status) });                
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult deleteDetail(string ReturnNo, string InvoiceNo, InquiryTrSalesReturnDetailModelView mdlDet)
        {
            bool result = false;
            try
            {
                using (var tranScope = new TransactionScope(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout }))
                {
                    result = DeleteDetailModel(ctx, CompanyCode, BranchCode, ReturnNo, InvoiceNo, mdlDet.BPKNo, mdlDet.SalesModelCode, mdlDet.SalesModelYear,
                                                mdlDet.ChassisCode, (decimal) mdlDet.ChassisNo, CurrentUser.UserId);
                    if (result)
                    {
                        tranScope.Complete();
                        return Json(new { success = true, message = "Delete detail return berhasil" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Delete detail return gagal!" });
                    }
                }
            }catch(Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private static bool DeleteDetailModel(DataContext ctxx, string companyCode, string branchCode,
            string returnNo, string InvoiceNo, string BPKNo, string salesModelCode, decimal salesModelYear,
            string chassisCode, decimal chassisNo, string usrid)
        {
            bool result = false;
            var record = ctxx.omTrSalesReturn.Find(companyCode, branchCode, returnNo);

            if (record != null)
            {
                record.Status = "0";
                record.LastUpdateBy = usrid;
                record.LastUpdateDate = DateTime.Now;

                if (ctxx.SaveChanges() > 0)
                {
                    if (DeleteVINRecord(ctxx, companyCode, branchCode, returnNo, BPKNo, salesModelCode,
                       salesModelYear, chassisCode, chassisNo, InvoiceNo, usrid))
                        if (DeleteDetailModelRecord(ctxx, companyCode, branchCode, returnNo, BPKNo, salesModelCode, salesModelYear))
                            if (DeleteBPKRecord(ctxx, companyCode, branchCode, returnNo, BPKNo))
                                result = true;
                }

            }
            return result;
        }

        private static bool DeleteBPKRecord(DataContext ctxx, string companyCode, string branchCode, string returnNo, string BPKNo)
        {
            bool result = false;
            //first check whether record has detailModel
            if (!OtherDetailModelExists(ctxx, companyCode, branchCode, returnNo, BPKNo))
            {
                var qry = String.Format(@"
                DELETE
                  FROM omTrSalesReturnBPK
                 WHERE CompanyCode = '{0}'
                       AND BranchCode = '{1}'
                       AND ReturnNo = '{2}'
                       AND BPKNo = '{3}'
                ", companyCode, branchCode, returnNo, BPKNo);
                
                if (ctxx.Database.ExecuteSqlCommand(qry) >= 0 )
                {
                    result = true;
                }               
            }
            else result = true;

            return result;
        }

        private static bool DeleteDetailModelRecord(DataContext ctxx, string companyCode, string branchCode,
            string returnNo, string BPKNo, string salesModelCode, decimal salesModelYear)
        {
            bool result = false;
            //first check whether record has vin
            if (!OtherVinExists(ctxx, companyCode, branchCode, returnNo, BPKNo, salesModelCode, salesModelYear))
            {
                var slsRtrnDetailModel = ctxx.omTrSalesReturnDetailModel.Find(companyCode, branchCode, returnNo, BPKNo, salesModelCode, salesModelYear);
                if (DeleteOtherRecord(ctxx, companyCode, branchCode, returnNo, BPKNo, salesModelCode, salesModelYear))
                    if (slsRtrnDetailModel != null)
                    {
                        ctxx.omTrSalesReturnDetailModel.Remove(slsRtrnDetailModel);
                        result = true;
                    }
            }
            else
            {
                var detailModel = ctxx.omTrSalesReturnDetailModel.Find(companyCode, branchCode, returnNo, BPKNo, salesModelCode, salesModelYear);
                if (detailModel != null)
                {
                    detailModel.Quantity = detailModel.Quantity - 1;
                    if (ctxx.SaveChanges() > 0)
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        private static bool OtherDetailModelExists(DataContext ctxx, string companyCode, string branchCode,
            string returnNo, string BPKNo)
        {
            var qry = String.Format(@"SELECT Count(*) 
                  FROM omTrSalesReturnDetailModel a
                 WHERE a.CompanyCode = '{0}'
                       AND a.BranchCode = '{1}'
                       AND a.ReturnNo = '{2}'
                       AND a.BPKNo = '{3}'
            ", companyCode, branchCode, returnNo, BPKNo);
            Int32 rCount = ctxx.Database.SqlQuery<Int32>(qry).FirstOrDefault();
            if (rCount > 0) return true;
            else return false;
        }

        private static bool DeleteOtherRecord(DataContext ctxx, string companyCode, string branchCode,
            string returnNo, string BPKNo, string salesModelCode, decimal salesModelYear)
        {
            var qry = String.Format(@"
                DELETE
                  FROM omTrSalesReturnOther
                 WHERE CompanyCode = '{0}'
                       AND BranchCode = '{1}'
                       AND ReturnNo = '{2}'
                       AND BPKNo = '{3}'
                       AND SalesModelCode = @SalesModelCode
                       AND SalesModelYear = @SalesModelYear
            ", companyCode, branchCode, returnNo, BPKNo, salesModelCode, salesModelYear);

            if (ctxx.Database.ExecuteSqlCommand(qry) >= 0) return true; else return false;
        }

        private static bool OtherVinExists(DataContext ctxx, string companyCode, string branchCode,
            string returnNo, string BPKNo, string salesModelCode, decimal salesModelYear)
        {
            var qry = String.Format(@"SELECT COUNT(*) 
                  FROM omTrSalesReturnVIN a
                 WHERE a.CompanyCode = '{0}'
                       AND a.BranchCode = '{1}'
                       AND a.ReturnNo = '{2}'
                       AND a.BPKNo = '{3}'
                       AND a.SalesModelCode = '{4}'
                       AND a.SalesModelYear = '{5}'
            ", companyCode, branchCode, returnNo, BPKNo, salesModelCode, salesModelYear);

            Int32 rCount = ctxx.Database.SqlQuery<Int32>(qry).FirstOrDefault();
            if (rCount > 0) return true;
            else return false;
        }

        private static bool DeleteVINRecord(DataContext ctxx, string companyCode, string branchCode, string returnNo, string BPKNo, string salesModelCode, 
                                            decimal salesModelYear, string chassisCode, decimal chassisNo, string invoiceNo, string usrid)
        {
            bool result = false;
            var VINRecord = ctxx.omTrSalesReturnVIN.Where(x => x.CompanyCode == companyCode && x.BranchCode == branchCode && x.ReturnNo == returnNo && x.BPKNo == BPKNo &&
                                                          x.SalesModelCode == salesModelCode && x.SalesModelYear == salesModelYear && x.ChassisCode == chassisCode && x.ChassisNo == chassisNo).FirstOrDefault();

            if (VINRecord != null)
                ctxx.omTrSalesReturnVIN.Remove(VINRecord);
            if (UpdateInvoice(ctxx, companyCode, branchCode, invoiceNo, BPKNo, salesModelCode, salesModelYear, chassisNo, false, usrid))
                result = true;

            return result;
        }

        public JsonResult printPreview(string ReturnNo)
        {
            string infoBranch = "";
            if (checkDetailModel(ctx, CompanyCode, BranchCode, ReturnNo))
            {
                var record = ctx.omTrSalesReturn.Find(CompanyCode, BranchCode, ReturnNo);
                if (record != null)
                {
                    if (record.Status == "0" || record.Status.Trim() == "") record.Status = "1";
                    record.LastUpdateBy = CurrentUser.UserId;
                    record.LastUpdateDate = DateTime.Now;
                    ctx.SaveChanges();

                    string ib = ctx.Database.SqlQuery<string>("SELECT ParaValue FROM GnMstLookUpDtl WHERE CompanyCode='" + CompanyCode + "' AND CodeID = 'RETUR_BRANCH'").FirstOrDefault();
                    if (ib != "" || ib != null)
                    {
                        if (ib == "1")
                        {
                            infoBranch = BranchCode;
                        }
                    }
                    return Json(new { success = true, infoBranch = infoBranch });
                }
            }
            return Json(new { success = false, message = "Print Preview gagal!" });
        }

        private static bool checkDetailModel(DataContext ctxx, string companyCode, string branchCode, string returnNo)
        {
            string RetNo = "";
            string PartNo = "";
            var qry = String.Format (@"
                SELECT a.ReturnNo
                  FROM omTrSalesReturnDetailModel a
                       INNER JOIN
                          omTrSalesReturnVIN b
                       ON a.CompanyCode = b.CompanyCode
                          AND a.BranchCode = b.BranchCode
                          AND a.BPKNo = b.BPKNo
                          AND a.ReturnNo = b.ReturnNo
                          AND a.SalesModelCode = b.SalesModelCode
                          AND a.SalesModelYear = b.SalesModelYear
                 WHERE a.CompanyCode = '{0}'
                       AND a.BranchCode = '{1}'
                       AND a.ReturnNo = '{2}'
            ", companyCode, branchCode, returnNo);

            RetNo = ctxx.Database.SqlQuery<string>(qry).FirstOrDefault() == null ? "" : ctxx.Database.SqlQuery<string>(qry).FirstOrDefault();

            qry = String.Format(@"
                SELECT a.PartNo
                  FROM omTrSalesReturnAccs a
                 WHERE a.CompanyCode = '{0}'
                       AND a.BranchCode = '{1}'
                       AND a.ReturnNo = '{2}'", companyCode, branchCode, returnNo);

            PartNo = ctxx.Database.SqlQuery<string>(qry).FirstOrDefault() == null ? "" : ctxx.Database.SqlQuery<string>(qry).FirstOrDefault();
            if (RetNo == ""  && PartNo == "")
            {
                return false;
            }else{
                return true; 
            }
        }

        public JsonResult approve(string returnNo)
        {
            bool result = false;
            string Qry = "";
            string msg = "";
            string dbMD = ctx.Database.SqlQuery<string>("SELECT DbMD from gnMstCompanyMapping WHERE CompanyCode='" + CompanyCode + "' AND BranchCode='" + BranchCode + "'").FirstOrDefault();

            if (dbMD == "" || dbMD == null)
            {
                return Json(new { success = false, message = "Approved return unit gagal!! Database MD tidak ada silahkan cek gnMstCompanyMapping" });
            }

            try
            {
                using (var tranScope = new TransactionScope(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout }))
                {
                    if (ctx.Database.ExecuteSqlCommand("EXEC uspfn_OmApproveSalesReturn '" + CompanyCode + "','" + BranchCode + "','" + returnNo + "','" + CurrentUser.UserId + "'") > 0 )
                    {
                        var record = ctx.omTrSalesReturn.Find(CompanyCode, BranchCode, returnNo);
                        if (record != null) 
                        {
                            record.LastUpdateDate = DateTime.Now;
                            record.LastUpdateBy = CurrentUser.UserId;
                            record.Status = "2";

                            result = ctx.SaveChanges() > 0;

                            //Insert omSdMovement
                            ctx.omTrSalesReturnVIN.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.ReturnNo == returnNo).ToList()
                            .ForEach(y =>
                            {
                                if (result)
                                {
                                    Qry = "INSERT INTO " + dbMD + "..omSDMovement(CompanyCode, BranchCode, DocNo, DocDate, Seq, SalesModelCode, SalesModelYear, ChassisCode, ChassisNo, EngineCode," +
                                          "EngineNo, ColourCode, WarehouseCode, CustomerCode, QtyFlag, CompanyMD, BranchMD, WarehouseMD, Status, ProcessStatus, ProcessDate, CreatedBy," +
                                          "CreatedDate, LastUpdateBy, LastUpdateDate) Values('" +
                                          y.CompanyCode + "','" + y.BranchCode + "','" + y.ReturnNo + "','" + y.CreatedDate + "','" + y.ReturnSeq + "','" + y.SalesModelCode +
                                          "','" + y.SalesModelYear + "','" + y.ChassisCode + "','" + y.ChassisNo + "','" + y.EngineCode + "','" + y.EngineNo +
                                          "','" + y.ColourCode + "','" + record.WareHouseCode + "','" + record.CustomerCode + "','-','" + CompanyMD + "','" + UnitBranchMD + "','" + WarehouseMD +
                                          "','" + record.Status + "','0','" + DateTime.Now + "','" + CurrentUser.UserId + "','" + DateTime.Now + "','" + CurrentUser.UserId + "','" + DateTime.Now + "')";

                                    result = ctx.Database.ExecuteSqlCommand(Qry) > 0;
                                }
                                else
                                {
                                    msg = "Proses insert omSdMovement untuk ChassisNo: " + y.ChassisNo;
                                    return;
                                }
                            });
                        }                        
                    }

                    if (result)
                    {
                        tranScope.Complete();
                        return Json(new { success = true, message = "Approve return unit berhasil!", Status = "2" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Approve return unit gagal!, " + msg });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult addDetail(omTrSalesReturn mdlHdr, InquiryTrSalesReturnDetailModelView mdlDet)
        {
            bool result = false;

            try
            {
                using (var tranScope = new TransactionScope(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout }))
                {
                    if (SaveDetailModel(ctx, CompanyCode, BranchCode, mdlHdr.ReturnNo, mdlHdr.InvoiceNo, mdlDet.BPKNo, mdlDet.SalesModelCode, mdlDet.SalesModelYear,
                                    mdlDet.ChassisCode, (decimal)mdlDet.ChassisNo, mdlDet.Remark, CurrentUser.UserId))
                    {
                        result = true;
                    }

                    if (result)
                    {
                        tranScope.Complete();
                        return Json(new { success = true, message = "Simpan return unit berhasil" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Simpan return unit gagal!" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        private static bool SaveDetailModel(DataContext ctxx, string companyCode, string branchCode, string returnNo, string invNo, string BPKNo, string salesModelCode,
                            decimal salesModelYear, string chassisCode, decimal chassisNo, string remarkDtl, string usrid)
        {
            bool result = false;
            omTrSalesReturn record = ctxx.omTrSalesReturn.Find(companyCode, branchCode, returnNo);
            record.Status = "0";
            record.LastUpdateBy = usrid;
            record.LastUpdateDate = DateTime.Now;

            if (ctxx.SaveChanges() > 0)
            {
                if (SaveBPKRecord(ctxx, companyCode, branchCode, returnNo, BPKNo, usrid))
                    if (SaveDetailModelRecord(ctxx, companyCode, branchCode, invNo, returnNo, BPKNo, salesModelCode, salesModelYear, usrid))
                        if (SaveVINRecord(ctxx, companyCode, branchCode, invNo, returnNo, BPKNo, salesModelCode, salesModelYear, chassisCode, chassisNo, remarkDtl, usrid))
                            result = true;
            }
            return result;
        }

        private static bool SaveVINRecord(DataContext ctxx, string companyCode, string branchCode,
            string invoiceNo, string returnNo, string BPKNo, string salesModelCode, decimal salesModelYear,
            string chassisCode, decimal chassisNo, string remark, string usrid)
        {
            bool result = false;
            bool isNew = false;
            //var VINRecord = ctxx.omTrSalesReturnVIN.Find(companyCode, branchCode, returnNo, BPKNo, salesModelCode, salesModelYear, chassisCode, chassisNo);
            
            var VINRecord = ctxx.omTrSalesReturnVIN.Where(x => x.CompanyCode == companyCode && x.BranchCode == branchCode && x.ReturnNo == returnNo && x.BPKNo == BPKNo
                                                            && x.SalesModelCode == salesModelCode && x.SalesModelYear == salesModelYear &&
                                                            x.ChassisCode == chassisCode && x.ChassisNo == chassisNo).FirstOrDefault();

            var invoiceVIN = ctxx.omTrSalesInvoiceVin.Where(x => x.CompanyCode == companyCode && x.BranchCode == branchCode && x.InvoiceNo == invoiceNo && x.BPKNo == BPKNo
                                             && x.SalesModelCode == salesModelCode && x.SalesModelYear == salesModelYear &&
                                             x.ChassisCode == chassisCode && x.ChassisNo == chassisNo).FirstOrDefault();
            if (invoiceVIN != null)
            {
                if (VINRecord == null)
                {
                    isNew = true;

                    VINRecord = new omTrSalesReturnVIN();
                    VINRecord.CompanyCode = companyCode;
                    VINRecord.BranchCode = branchCode;
                    VINRecord.ReturnNo = returnNo;
                    VINRecord.BPKNo = BPKNo;
                    VINRecord.SalesModelCode = salesModelCode;
                    VINRecord.SalesModelYear = salesModelYear;
                    VINRecord.ReturnSeq = Convert.ToDecimal(invoiceVIN.InvoiceSeq == null ? 0 : invoiceVIN.InvoiceSeq);
                    VINRecord.ColourCode = invoiceVIN.ColourCode;
                    VINRecord.ChassisCode = invoiceVIN.ChassisCode;
                    VINRecord.ChassisNo = invoiceVIN.ChassisNo;
                    VINRecord.EngineCode = invoiceVIN.EngineCode;
                    VINRecord.EngineNo = invoiceVIN.EngineNo;
                    VINRecord.CreatedBy = usrid;
                    VINRecord.CreatedDate = DateTime.Now;

                    ctxx.omTrSalesReturnVIN.Add(VINRecord);

                }

                VINRecord.Remark = remark == null ? "" : remark;
                VINRecord.LastUpdateBy = usrid;
                VINRecord.LastUpdateDate = DateTime.Now;

                if (isNew)
                {
                    if (ctxx.SaveChanges() > 0)
                    {
                        result = UpdateInvoice(ctxx, companyCode, branchCode, invoiceNo, BPKNo, salesModelCode, salesModelYear, chassisNo, true, usrid);
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    result = ctxx.SaveChanges() > 0;
                }
            }
            return result;
        }

        public static bool UpdateInvoice(DataContext ctxx, string companyCode, string branchCode, string invoiceNo,
                            string BPKNo, string salesModelCode, decimal salesModelYear, decimal chassisNo, bool isSaving, string usrid)
        {
            bool isReturn = true;
            decimal qty = 1;
            if (!isSaving)
            {
                isReturn = false;
                qty = -1;
            }

            bool result = false;
            var qry = String.Format (@"UPDATE omTrSalesInvoiceVin
                   SET isReturn = '{0}'
                 WHERE CompanyCode = '{1}'
                       AND BranchCode = '{2}'
                       AND InvoiceNo = '{3}'
                       AND BPKNo = '{4}'
                       AND ChassisNo = '{5}'", isReturn, companyCode, branchCode, invoiceNo, BPKNo, chassisNo);

            if(ctxx.Database.ExecuteSqlCommand(qry) > 0 )
            {
                qry = String.Format(@"
                    UPDATE omTrSalesInvoiceModel
                       SET quantityReturn = quantityReturn + '{0}'
                     WHERE CompanyCode = '{1}'
                           AND BranchCode = '{2}'
                           AND InvoiceNo = '{3}'
                           AND BPKNo = '{4}'
                           AND SalesModelCode = '{5}'
                           AND SalesModelYear = '{6}'
                ", qty, companyCode, branchCode, invoiceNo, BPKNo, salesModelCode, salesModelYear);

                if (ctxx.Database.ExecuteSqlCommand(qry) > 0)
                    result = true;
            }

            return result;
        }

        private static bool SaveBPKRecord(DataContext ctxx, string companyCode, string branchCode, string returnNo, string BPKNo, string usrid)
        {
            bool result = false;
            var returnBPKRecord = ctxx.omTrSalesReturnBPKs.Find(companyCode, branchCode, returnNo, BPKNo);
            if (returnBPKRecord == null)
            {
                returnBPKRecord = new omTrSalesReturnBPK();
                returnBPKRecord.CompanyCode = companyCode;
                returnBPKRecord.BranchCode = branchCode;
                returnBPKRecord.ReturnNo = returnNo;
                returnBPKRecord.BPKNo = BPKNo;

                returnBPKRecord.CreatedBy = usrid;
                returnBPKRecord.CreatedDate = DateTime.Now;

                ctxx.omTrSalesReturnBPKs.Add(returnBPKRecord);
            }
            returnBPKRecord.LastUpdateBy = usrid;
            returnBPKRecord.LastUpdateDate = DateTime.Now;

            result = ctxx.SaveChanges() > 0;
            return result;
        }

        private static bool SaveDetailModelRecord(DataContext ctxx, string companyCode, string branchCode,
            string invoiceNo, string returnNo, string BPKNo, string salesModelCode, decimal salesModelYear, string usrid)
        {
            bool result = false;
            bool isNew = false;

            var detailModelRecord = ctxx.omTrSalesReturnDetailModel.Find(companyCode, branchCode,
                returnNo, BPKNo, salesModelCode, salesModelYear);
            
            if (detailModelRecord == null)
            {
                isNew = true;
                var invoiceModel = ctxx.omTrSalesInvoiceModel.Find(companyCode, branchCode,
                    invoiceNo, BPKNo, salesModelCode, salesModelYear);

                detailModelRecord = new omTrSalesReturnDetailModel();
                detailModelRecord.CompanyCode = companyCode;
                detailModelRecord.BranchCode = branchCode;
                detailModelRecord.ReturnNo = returnNo;
                detailModelRecord.BPKNo = BPKNo;
                detailModelRecord.SalesModelCode = salesModelCode;
                detailModelRecord.SalesModelYear = salesModelYear;
                detailModelRecord.BeforeDiscDPP = invoiceModel.BeforeDiscDPP;
                detailModelRecord.DiscExcludePPn = invoiceModel.DiscExcludePPn;
                detailModelRecord.AfterDiscDPP = invoiceModel.AfterDiscDPP;
                detailModelRecord.AfterDiscPPn = invoiceModel.AfterDiscPPn;
                detailModelRecord.AfterDiscPPnBM = invoiceModel.AfterDiscPPnBM;
                detailModelRecord.AfterDiscTotal = invoiceModel.AfterDiscTotal;
                detailModelRecord.OthersDPP = invoiceModel.OthersDPP;
                detailModelRecord.OthersPPn = invoiceModel.OthersPPn;
                detailModelRecord.CreatedBy = usrid;
                detailModelRecord.CreatedDate = DateTime.Now;
                detailModelRecord.Quantity = 0;

                ctxx.omTrSalesReturnDetailModel.Add(detailModelRecord);

            }

            detailModelRecord.Quantity = detailModelRecord.Quantity + 1;
            detailModelRecord.LastUpdateBy = usrid;
            detailModelRecord.LastUpdateDate = DateTime.Now;

            if (isNew)
            {
                if (ctxx.SaveChanges() > 0)
                {
                    result = SaveOtherRecord(ctxx, companyCode, branchCode, invoiceNo, returnNo, BPKNo, salesModelCode, salesModelYear, usrid);
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                result = ctxx.SaveChanges() > 0;
            }

            return result;
        }

        private static bool SaveOtherRecord(DataContext ctxx, string companyCode, string branchCode,
            string invoiceNo, string returnNo, string BPKNo, string salesModelCode, decimal salesModelYear, string usrid)
        {
            bool result = true;
            ctxx.omTrSalesInvoiceOthers.Where(x => x.CompanyCode == companyCode && x.BranchCode == branchCode &&
                    x.InvoiceNo == invoiceNo && x.BPKNo == BPKNo && x.SalesModelCode == salesModelCode && x.SalesModelYear == salesModelYear)
                    .ToList().ForEach( y => {

                        omTrSalesReturnOther OtherRecord = new omTrSalesReturnOther();
                        OtherRecord.CompanyCode = companyCode;
                        OtherRecord.BranchCode = branchCode;
                        OtherRecord.ReturnNo = returnNo;
                        OtherRecord.BPKNo = y.BPKNo;
                        OtherRecord.SalesModelCode = y.SalesModelCode;
                        OtherRecord.SalesModelYear = y.SalesModelYear;
                        OtherRecord.OtherCode = y.OtherCode;
                        OtherRecord.BeforeDiscDPP = y.BeforeDiscDPP == null ? 0 : y.BeforeDiscDPP ;
                        OtherRecord.BeforeDiscPPn = y.BeforeDiscPPn == null ? 0 : y.BeforeDiscPPn;
                        OtherRecord.BeforeDiscTotal = y.BeforeDiscTotal == null ? 0 : y.BeforeDiscTotal;
                        OtherRecord.DiscExcludePPn = y.DiscExcludePPn == null ? 0 : y.DiscExcludePPn;
                        OtherRecord.DiscIncludePPn = y.DiscIncludePPn == null ? 0 : y.DiscIncludePPn;
                        OtherRecord.AfterDiscDPP = y.AfterDiscDPP == null ? 0 : y.AfterDiscDPP;
                        OtherRecord.AfterDiscPPn = y.AfterDiscPPn == null ? 0 : y.AfterDiscPPn;
                        OtherRecord.AfterDiscTotal = y.AfterDiscTotal == null ? 0 : y.AfterDiscTotal;
                        OtherRecord.DPP = y.DPP == null ? 0 : y.DPP;
                        OtherRecord.PPn = y.PPn == null ? 0 : y.PPn;
                        OtherRecord.Total = y.Total == null ? 0 : y.Total;
                        OtherRecord.CreatedBy = usrid;
                        OtherRecord.CreatedDate = DateTime.Now;
                        OtherRecord.LastUpdateBy = usrid;
                        OtherRecord.LastUpdateDate = DateTime.Now;

                        ctxx.omTrSalesReturnOthers.Add(OtherRecord);

                        result = ctxx.SaveChanges() > 0;
            });
            return result;
        }



        public JsonResult Delete(OmTrSalesInvoice mdl)
        {

            return Json("");
            //try
            //{
            //    var record = ctx.OmTrSalesInvoices.Find(CompanyCode, BranchCode, mdl.InvoiceNo);
            //    if (record != null)
            //    {
            //        if (record.Status != "0" && record.Status != "1")
            //        {
            //            return Json(new { success = false, message = "Invalid Status" });
            //        }
            //    }
            //    if (record == null)
            //    {
            //        return Json(new { success = false, message = "Invalid Status" });
            //    }

            //    OmTrSalesBLL.Instance(CurrentUser.UserId).DeleteInv(ctx, record);

            //    return Json(new { success = true, InvoiceNo = record.InvoiceNo, Status = record.Status, StatusDsc = getStringStatus(record.Status) });
            //}
            //catch (Exception ex)
            //{
            //    return Json(new { success = false, message = ex.Message });
            //}

        }

    }
}
