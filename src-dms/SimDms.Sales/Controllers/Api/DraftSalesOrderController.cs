using SimDms.Sales.BLL;
using SimDms.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common.Models;
using SimDms.Sales.Models;
using System.Transactions;

namespace SimDms.Sales.Controllers.Api
{
    public class DraftSalesOrderController : BaseController
    {
        #region -- Public Method --
        #region ---- Populate Data ----
        public JsonResult PopulateRecord(string draftSONo)
        {
            try
            {
                var uid = CurrentUser.UserId;
                var omTrSalesDraftSOBLL = new OmTrSalesDraftSOBLL(ctx, uid);
                var recDraftSO = omTrSalesDraftSOBLL.GetRecord(draftSONo);
                omTrSalesDraftSOBLL = null;

                var omTrSalesDraftSOModelBLL = new OmTrSalesDraftSOModelBLL(ctx, uid);
                var recDraftSOModel = omTrSalesDraftSOModelBLL.Select4Table(draftSONo);
                var CheckingQtySO = omTrSalesDraftSOModelBLL.CheckingQtySO(draftSONo);
                omTrSalesDraftSOModelBLL = null;

                return Json(new {success = true,
                                 data = new { 
                                     DraftSO = recDraftSO, 
                                     DraftSOModel = recDraftSOModel, 
                                     isCheckingQtySO = CheckingQtySO
                                 }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Tidak ada data untuk ditampilkan", error_log = ex.Message });
            }
        }

        public JsonResult PopulateRecordModel(string draftSONo = "", string salesModelCode = "", decimal salesModelYear = 0, string groupPriceCode = "")
        {
            try
            {
                var uid = CurrentUser.UserId;
                var omTrSalesDraftSOModelBLL = new OmTrSalesDraftSOModelBLL(ctx, uid);
                var recordDraftSOModel = omTrSalesDraftSOModelBLL.GetRecord(draftSONo, salesModelCode, salesModelYear);
                omTrSalesDraftSOModelBLL = null;

                var omTrSalesDraftSOModelColourBLL = new OmTrSalesDraftSOModelColourBLL(ctx, uid);
                var recordDraftSOColour = omTrSalesDraftSOModelColourBLL.Select4Table(draftSONo, salesModelCode, salesModelYear);
                omTrSalesDraftSOModelColourBLL = null;

                var omTrSalesDraftSOModelOthersBLL = new OmTrSalesDraftSOModelOthersBLL(ctx, uid);
                var recordDraftSOOthers = omTrSalesDraftSOModelOthersBLL.Select4Table(draftSONo, salesModelCode, salesModelYear);
                omTrSalesDraftSOModelOthersBLL = null;


                var salesModelDesc = p_ModelDescription(groupPriceCode, salesModelCode, salesModelYear);

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        recDraftSOModel = recordDraftSOModel,
                        recDraftSOColour = recordDraftSOColour ,
                        recDraftSOOthers = recordDraftSOOthers
                    },
                    SalesModelDesc = (salesModelDesc != null) ? salesModelDesc : ""
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Tidak ada data untuk ditampilkan", error_log = ex.Message });
            }
        }

        public JsonResult PopulateRecordModelColour(string draftSONo = "", string salesModelCode = "", decimal salesModelYear = 0, string colourCode = "")
        {
            try
            {
                var uid = CurrentUser.UserId;
                var omTrSalesDraftSOModelColourBLL = new OmTrSalesDraftSOModelColourBLL(ctx, uid);
                var recordDraftSOColour = omTrSalesDraftSOModelColourBLL.GetRecord(draftSONo, salesModelCode, salesModelYear, colourCode);
                omTrSalesDraftSOModelColourBLL = null;

                var omTrSalesDraftSOVinBLL = new OmTrSalesDraftSOVinBLL(ctx, uid);
                var recordDraftSOVin = omTrSalesDraftSOVinBLL.Select4Table(draftSONo, salesModelCode, salesModelYear, colourCode);
                omTrSalesDraftSOVinBLL = null;

                var colourDesc = p_ColourName(salesModelCode, colourCode);

                return Json(new { 
                    success = true, 
                    data = new {
                        recDraftSOColour = recordDraftSOColour ,
                        recDraftSOVin = recordDraftSOVin
                    }, 
                    ColourDesc = (colourDesc != null) ? colourDesc : "" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Tidak ada data untuk ditampilkan", error_log = ex.Message });
            }
        }

        public JsonResult PopulateRecordVin(string draftSONo = "", string salesModelCode = "", decimal salesModelYear = 0, string colourCode = "", int sOSeq = 0)
        {
            try
            {
                var uid = CurrentUser.UserId;
                var omTrSalesDraftSOVinBLL = new OmTrSalesDraftSOVinBLL(ctx, uid);
                var recordDraftSOVin = omTrSalesDraftSOVinBLL.GetRecord(draftSONo, salesModelCode, salesModelYear, colourCode, sOSeq);
                omTrSalesDraftSOVinBLL = null;

                var cityName = p_CityName(recordDraftSOVin.SupplierBBN, salesModelCode, salesModelYear, recordDraftSOVin.CityCode);
                var supplierBBNName = p_BNNDesc(salesModelCode, salesModelYear, recordDraftSOVin.SupplierBBN);

                return Json(new { 
                    success = true, 
                    data = new {
                        recDraftSOVin = recordDraftSOVin
                    },
                    CityName = (cityName != null) ? cityName  : "",
                    SupplierBBNName = (supplierBBNName != null) ? supplierBBNName : ""
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Tidak ada data untuk ditampilkan", error_log = ex.Message });
            }
        }

        public JsonResult PopulateRecordOthers(string draftSONo = "", string salesModelCode = "", decimal salesModelYear = 0, string otherCode = "")
        {
            try
            {
                var uid = CurrentUser.UserId;
                var omTrSalesDraftSOModelOthersBLL = new OmTrSalesDraftSOModelOthersBLL(ctx, uid);
                var recordDraftSOOthers = omTrSalesDraftSOModelOthersBLL.GetRecord(draftSONo, salesModelCode, salesModelYear, otherCode);
                omTrSalesDraftSOModelOthersBLL = null;

                var accsName = p_AccesoriesDesc(otherCode);

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        recDraftSOOthers = recordDraftSOOthers
                    },
                    AccsName = (accsName != null) ? accsName : ""
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Tidak ada data untuk ditampilkan", error_log = ex.Message });
            }
        }
        #endregion

        #region ----LookUp Data ----
        public JsonResult BrowseITS()
        {
            try
            {
                return Json(p_ITS().toKG());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Tidak ada data untuk ditampilkan", error_log = ex.Message });
            }
        }

        public JsonResult BrowseCustomer(bool chkType)
        {
            try
            {
                if (chkType)
                {
                    return Json(p_Customer2().toKG());
                }
                else
                {
                    return Json(p_Customer().toKG());
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Tidak ada data untuk ditampilkan", error_log = ex.Message });
            }
        }

        public JsonResult BrowseSalesman()
        {
            try
            {
                return Json(p_Salesman().toKG());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Tidak ada data untuk ditampilkan", error_log = ex.Message });
            }
        }

        public JsonResult BrowseTOPC()
        {
            try
            {
                return Json(p_TOPCList().toKG());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Tidak ada data untuk ditampilkan", error_log = ex.Message });
            }
        }

        public string TOPCInterval(string TOPCode)
        {
            return p_TOPCInterval(TOPCode);
        }

        public JsonResult BrowseLeasing()
        {
            try
            {
                return Json(p_Leasing().toKG());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Tidak ada data untuk ditampilkan", error_log = ex.Message });
            }
        }
        
        public JsonResult BrowseModel(string inquiryNumber = "")
        {
            try
            {
                if (inquiryNumber != ""){
                    var inqNumber = Convert.ToInt32(inquiryNumber);
                    var records = from a in ctx.OmMstModels
                                  join b in ctx.PmKdps on new { a.CompanyCode, BranchCode = BranchCode, a.GroupCode, a.TransmissionType, a.TypeCode }
                                  equals new { b.CompanyCode, b.BranchCode, GroupCode = b.TipeKendaraan, TransmissionType = b.Transmisi, TypeCode = b.Variant }
                                  where a.CompanyCode == CompanyCode && b.InquiryNumber == inqNumber
                                  orderby a.SalesModelCode
                                  select new { 
                                    a.SalesModelCode,
                                    a.SalesModelDesc
                                  };

                    return Json(records.toKG());
                }
                else{
                    var listStatus = "1,2".Split(',');

                    var records = (from a in ctx.MstModelYear
                                  where a.CompanyCode == CompanyCode && listStatus.Contains(a.Status)
                                  orderby a.SalesModelCode
                                  select new
                                  {
                                      a.SalesModelCode,
                                      a.SalesModelDesc
                                  }).Distinct();

                    return Json(records.toKG());
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Tidak ada data untuk ditampilkan", error_log = ex.Message });
            }
        }

        public JsonResult BrowseModelYear(string salesModelCode = "", string groupPriceCode = "")
        {
            try{
                var records = (from a in ctx.MstModelYear
                               join b in ctx.OmMstPricelistSells on new { a.CompanyCode, a.SalesModelCode, a.SalesModelYear, GroupPriceCode = groupPriceCode }
                               equals new { b.CompanyCode, b.SalesModelCode, b.SalesModelYear, b.GroupPriceCode } into _b
                               from b in _b.DefaultIfEmpty()
                               where a.CompanyCode == CompanyCode && a.SalesModelCode == salesModelCode && a.Status == "1"
                               orderby a.SalesModelYear
                               select a ).Distinct()
                               .Select(p => new {
                                    p.SalesModelYear,
                                    p.SalesModelDesc,
                                    p.ChassisCode
                               }).ToList();

                return Json(records.AsQueryable().toKG());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Tidak ada data untuk ditampilkan", error_log = ex.Message });
            }
        }

        public JsonResult BrowseColour(string salesModelCode = "")
        {
            try
            {
                var records = (from a in ctx.OmMstModelColours
                               where a.CompanyCode == CompanyCode && a.SalesModelCode == salesModelCode && a.Status == "1"
                               select new
                               {
                                    a.ColourCode,
                                    ColourDesc = (from b in ctx.MstRefferences where b.RefferenceCode ==  a.ColourCode 
                                                && b.CompanyCode ==  a.CompanyCode && b.RefferenceType == OmMstRefferenceConstant.CLCD 
                                                select b.RefferenceDesc1).FirstOrDefault(),
                                    a.Remark
                               });

                return Json(records.toKG());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Tidak ada data untuk ditampilkan", error_log = ex.Message });
            }
        }

        public JsonResult BrowseBBN(string salesModelCode = "", decimal salesModelYear = 0)
        {
            try
            {
                var records = (from a in ctx.Supplier
                               from b in ctx.SupplierProfitCenter
                               from c in ctx.MstBBNKIR
                               where a.CompanyCode == b.CompanyCode
                               && b.CompanyCode == c.CompanyCode
                               && a.SupplierCode == b.SupplierCode
                               && b.SupplierCode == c.SupplierCode
                               && a.CompanyCode == CompanyCode
                               && c.BranchCode == BranchCode
                               && b.ProfitCenterCode == ProfitCenter
                               && c.Status == "1"
                               && c.SalesModelCode == salesModelCode
                               && c.SalesModelYear == salesModelYear
                               orderby a.SupplierCode
                               select new
                               {
                                   SupplierBBN = a.SupplierCode,
                                   SupplierBBNName = a.SupplierName
                               }
                            ).Distinct();

                return Json(records.toKG());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Tidak ada data untuk ditampilkan", error_log = ex.Message });
            }
        }

        public JsonResult BrowseCity(string supplierCode = "", string salesModelCode = "", decimal salesModelYear = 0)
        {
            try
            {
                var records = (from a in ctx.LookUpDtls
                               join b in ctx.MstBBNKIR
                               on new
                               {
                                   a.CompanyCode, a.LookUpValue, BranchCode = BranchCode, a.CodeID, Status = "1", 
                                   SupplierCode = supplierCode, SalesModelCode = salesModelCode, SalesModelYear = salesModelYear
                               }
                               equals new
                               {
                                   b.CompanyCode, LookUpValue = b.CityCode, b.BranchCode, CodeID = "CITY", b.Status,
                                   b.SupplierCode, b.SalesModelCode, b.SalesModelYear
                               }
                               orderby a.LookUpValue
                               select new
                               {
                                  CityCode = a.LookUpValue,
                                   CityName = a.LookUpValueName,
                                   b.BBN,
                                   b.KIR
                               }).Distinct();

                return Json(records.toKG());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Tidak ada data untuk ditampilkan", error_log = ex.Message });
            }
        }

        public JsonResult BrowseAccesories()
        {
            try
            {
                var records = from a in ctx.MstRefferences
                              where a.CompanyCode == CompanyCode
                              && a.RefferenceType == OmMstRefferenceConstant.OTHS
                              && a.Status != "0"
                              select new
                              {
                                  a.RefferenceCode,
                                  a.RefferenceDesc1
                              };

                return Json(records.toKG());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Tidak ada data untuk ditampilkan", error_log = ex.Message });
            }
        }

        public string RetriveModelDescription(string inquiryNumber = "", string salesModelCode = "")
        {
            var SalesModelDesc = p_ModelDescription(inquiryNumber, salesModelCode);

            return (SalesModelDesc != null) ? SalesModelDesc : "";
        }

        public string RetriveModelColourDescription(string salesModelCode, string colourCode)
        {
            var colourDesc = p_ColourName(salesModelCode, colourCode);

            return (colourDesc != null) ? colourDesc : "";
        }
        
        public JsonResult RetrieveNameOrDescription(string customerCode = "", string salesmanCode = "", string topCode = "", string salesModelCode = "",
            decimal salesModelYear = 0, string colourCode = "", string supplierBBN = "", string cityCode = "", string accesoriesCode = "", bool chkType = false,
            string inquiryNumber = "")
        {
            try
            {
                var customerName = string.Empty;
                var groupPriceDesc =  string.Empty;
                if (chkType)
                {
                    var rec = p_Customer2().Where(p => p.CustomerCode == customerCode).FirstOrDefault();
                    if (rec != null){
                        customerName =  rec.CustomerName;
                        groupPriceDesc = rec.GroupPriceDesc;
                    }
                    rec = null;
                }
                else
                {
                    var rec = p_Customer().Where(p => p.CustomerCode == customerCode).FirstOrDefault();
                    if (rec != null)
                    {
                        customerName = rec.CustomerName;
                        groupPriceDesc = rec.GroupPriceDesc;
                    }
                    rec = null;
                }

                var recSales = p_Salesman().Where(p => p.EmployeeID == salesmanCode).FirstOrDefault();
                var salesmanName = (recSales != null) ? recSales.EmployeeName : "";
                recSales = null;
                var topInterval = p_TOPCInterval(topCode);
                var modelDesc = p_ModelDescription(inquiryNumber, salesModelCode);
                var colourName = p_ColourName(salesmanCode, colourCode);
                var bbnDesc = p_BNNDesc(salesModelCode, salesModelYear, supplierBBN);
                var cityName = p_CityName(supplierBBN, salesModelCode, salesModelYear, cityCode);
                var acceDesc = p_AccesoriesDesc(accesoriesCode);

                return Json(new { success = true, 
                    data = new { 
                        CustomerName = customerName,
                        SalesName = salesmanName,
                        GroupPriceDesc = groupPriceDesc,
                        TOPInterval = topInterval,
                        SalesModelDesc = modelDesc,
                        ColourDesc = colourName,
                        BNNDesc = bbnDesc,
                        CityName = cityName,
                        AccesoriesDesc = acceDesc
                    } 
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Tidak ada data untuk ditampilkan", error_log = ex.Message });
            }
        }
        #endregion

        #region ---- Business Logic ----
        public JsonResult SaveDraftSO(OmTrSalesDraftSO model, bool chkType, bool dtpSO, bool dtpReff, bool chkLeasing, bool dtpRequest)
        {
            try
            {
                    // validation of date transaction
                    string errMsg = string.Empty;
                    var SODate = (DateTime)model.DraftSODate;
                    if (dtpSO && !DateTransValidation(SODate, ref errMsg))
                    {
                        return ThrowException(errMsg);
                    }

                    var records = ctx.LookUpDtls.Where(p => p.CompanyCode == CompanyCode && p.CodeID == GnMstLookUpHdr.TermOfPayment)
                        .OrderBy(p => p.SeqNo);
                    if (records != null)
                    {
                        var records2 = records.Where(p => p.LookUpValue == model.TOPCode).ToList();
                        if (records2 != null)
                        {
                            if (records2.Count > 0)
                            {
                                model.TOPCode = records2[0].LookUpValue ?? "";
                                model.TOPDays = (records2[0].ParaValue != null) ? Convert.ToDecimal(records2[0].ParaValue) : 0;
                            }
                            else
                            {
                                model.TOPCode = "";
                                model.TOPDays = 0;
                                return ThrowException("TOP tidak ada dalam database");
                            }
                        }
                    }
                    
                    var omTrSalesDraftSOBLL = new OmTrSalesDraftSOBLL(ctx, CurrentUser.UserId);
                    OmTrSalesDraftSO record;
                    bool isNew = p_PrepareRecord(model, chkType, dtpReff, chkLeasing, dtpRequest, omTrSalesDraftSOBLL, out record);
                    bool result = false;

                    result = omTrSalesDraftSOBLL.SaveDraftSO(record, isNew);
                    omTrSalesDraftSOBLL = null;
                    if (result)
                    {
                        return Json(new { success = true, message = "Simpan Draft Sales Order berhasil", DraftSONo = record.DraftSONo, Status = record.Status });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Simpan Draft Sales Order tidak berhasil. Periksa kembali inputan anda" });
                    }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Simpan Draft Sales Order tidak berhasil. Periksa kembali inputan anda atau hubungi SDMS support", error_log = ex.Message });
            }
        }

        public JsonResult SaveDraftSOModel(OmTrSalesDraftSOModel model, string draftSONo, string groupPriceCode, 
            string customerCode, decimal discount, bool chkType)
        {
            try
            {
                using (var tranScope = new TransactionScope(TransactionScopeOption.RequiresNew,
                    new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    var uid = CurrentUser.UserId;
                    var omTrSalesDraftSOBLL = new OmTrSalesDraftSOBLL(ctx, uid);
                    var record = omTrSalesDraftSOBLL.GetRecord(draftSONo);
                    if (record != null)
                    {
                        if (record.Status != "0" && record.Status != "1") {
                            return Json(new { success = true, isExistDraftSO = true }); 
                        }
                    }

                    decimal minStaff = 0;
                    OmMstPricelistSell sell = ctx.OmMstPricelistSells.Find(CompanyCode, BranchCode, 
                        groupPriceCode, model.SalesModelCode, model.SalesModelYear);
                    if (sell != null)
                        minStaff = sell.TotalMinStaff ?? 0;
                    else
                    {
                        return ThrowException("Pricelist Jual belum ada");
                    }
                    string msgWarning = string.Empty;
                    string msgWarning2 = string.Empty;
                    var omTrSalesDraftSOModelBLL = new OmTrSalesDraftSOModelBLL(ctx, uid);
                    OmTrSalesDraftSOModel recordModel;
                    bool isNew = p_PrepareRecordModel(omTrSalesDraftSOModelBLL, model, draftSONo, groupPriceCode, customerCode, discount, 
                        out recordModel, out msgWarning, out msgWarning2);
                    if (recordModel == null) return Json(new { success = true, isNull = true });

                    // to make sure that if new detail is inserted, status PO back to 0
                    record = omTrSalesDraftSOBLL.GetRecord(draftSONo);
                    if (record == null) return Json(new { success = true, isNull = true });
                    else
                    {
                        record.Status = "0";
                        record.LastUpdateBy = uid;
                        record.LastUpdateDate = DateTime.Now;
                    }
                    bool result = omTrSalesDraftSOModelBLL.SaveDetailDraftSO(recordModel, record, isNew, chkType);
                    var recDraftSOModel = omTrSalesDraftSOModelBLL.Select4Table(draftSONo);
                    omTrSalesDraftSOModelBLL = null;

                    if (result)
                        tranScope.Complete();

                    omTrSalesDraftSOBLL = null;
                    
                    return Json(new { success = true, message = "Simpan Draft Sales Order - Model berhasil",
                                      DraftSOModel = recDraftSOModel,  
                                    Status = record.Status,
                                    messageWarning = msgWarning,
                                    messageWarning2 = msgWarning2
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Simpan Draft Sales Order - Model tidak berhasil. Periksa kembali inputan anda atau hubungi SDMS support", error_log = ex.Message });
            }
        }

        public JsonResult SaveDraftSOModelColour(OmTrSalesDraftSOModelColour model, string draftSONo, string salesModelCode, decimal salesModelYear)
        {
            bool save = false;
            string msg = "Simpan Draft Sales Order - Model Colour tidak berhasil";
            try
            {
                using (var tranScope = new TransactionScope(TransactionScopeOption.RequiresNew,
                    new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    model.DraftSONo = draftSONo;
                    model.SalesModelCode = salesModelCode;
                    model.SalesModelYear = salesModelYear;

                    var uid = CurrentUser.UserId;
                    var omTrSalesDraftSOBLL = new OmTrSalesDraftSOBLL(ctx, uid);
                    var record = omTrSalesDraftSOBLL.GetRecord(draftSONo);
                    if (record != null)
                    {
                        if (record.Status != "0" && record.Status != "1")
                        {
                            return Json(new { success = true, isNull = false, allowInput = false });
                        }
                    }

                    var omTrSalesDraftSOModelColourBLL = new OmTrSalesDraftSOModelColourBLL(ctx, uid);
                    OmTrSalesDraftSOModelColour recordModelColour;
                    bool isNew = p_PrepareRecordModelColour(omTrSalesDraftSOModelColourBLL, model, out recordModelColour);

                    // to make sure that if new detail is inserted, status PO back to 0
                    record = omTrSalesDraftSOBLL.GetRecord(draftSONo);
                    if (record == null) return Json(new { success = true, isNull = true, allowInput = false });
                    else
                    {
                        record.Status = "0";
                        record.LastUpdateBy = uid;
                        record.LastUpdateDate = DateTime.Now;
                    }

                    var omTrSalesDraftSOModelBLL = new OmTrSalesDraftSOModelBLL(ctx, uid);
                    bool result = omTrSalesDraftSOModelColourBLL.SaveDetailModelColour(omTrSalesDraftSOModelBLL, recordModelColour, isNew);
                    
                    var recDraftSOColour = omTrSalesDraftSOModelColourBLL.Select4Table(draftSONo, salesModelCode, salesModelYear);
                    var recDraftSOModel = omTrSalesDraftSOModelBLL.Select4Table(draftSONo);
                    if (result)
                    {
                        tranScope.Complete();
                        save = true;
                        msg = "Simpan Draft Sales Order - Model Colour berhasil";
                    }

                    omTrSalesDraftSOModelColourBLL = null;
                    omTrSalesDraftSOModelBLL = null;
                    omTrSalesDraftSOBLL = null;
                    uid = null;

                    return Json(new
                    {
                        success = save,
                        allowInput = true,
                        message = msg, 
                        data = new {
                            DraftSOModel = recDraftSOModel,
                            DraftSOColour = recDraftSOColour 
                        },
                        Status = record.Status
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Simpan Draft Sales - Order Model tidak berhasil. Periksa kembali inputan anda atau hubungi SDMS support", 
                    error_log = ex.Message });
            }
        }

        public JsonResult SaveDraftSOVin(OmTrSalesDraftSOVin model, string draftSONo, string salesModelCode, decimal salesModelYear, string colourCode, int seqNo)
        {
            bool save = false;
            string msg = "Simpan Draft Sales Order - Detil Lain-lain tidak berhasil";
            try
            {
                using (var tranScope = new TransactionScope(TransactionScopeOption.RequiresNew,
                    new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    model.DraftSONo = draftSONo;
                    model.SalesModelCode = salesModelCode;
                    model.SalesModelYear = salesModelYear;
                    model.ColourCode = colourCode;
                    model.SOSeq = seqNo;

                    var uid = CurrentUser.UserId;
                    var omTrSalesDraftSOBLL = new OmTrSalesDraftSOBLL(ctx, uid);
                    var record = omTrSalesDraftSOBLL.GetRecord(draftSONo);
                    if (record != null)
                    {
                        if (record.Status != "0" && record.Status != "1")
                        {
                            return Json(new { success = true, isNull = false, allowInput = false });
                        }
                    }

                    var omTrSalesDraftSOModelColourBLL = new OmTrSalesDraftSOModelColourBLL(ctx, uid);
                    var omTrSalesDraftSOVinBLL = new OmTrSalesDraftSOVinBLL(ctx, uid);
                    OmTrSalesDraftSOVin recordSOVin;
                    bool isNew = p_PreparerecordSOVin(omTrSalesDraftSOVinBLL, model, draftSONo, salesModelCode, salesModelYear, colourCode, seqNo, out recordSOVin);

                    if (isNew)
                    {
                        if (omTrSalesDraftSOBLL.QuantityCheck(draftSONo, salesModelCode, salesModelYear, colourCode))
                        {
                            return ThrowException("Jumlah Detail Lain-lain tidak boleh lebih dari jumlah Detil Warna");
                        }
                    }

                    bool result = false;
                    // to make sure that if new detail is inserted, status PO back to 0
                    record = omTrSalesDraftSOBLL.GetRecord(draftSONo);
                    if (record == null) return Json(new { success = true, isNull = true, allowInput = false });
                    else
                    {
                        record.Status = "0";
                        record.LastUpdateBy = uid;
                        record.LastUpdateDate = DateTime.Now;
                        Helpers.ReplaceNullable(record);
                        result = ctx.SaveChanges() >= 0;
                    }

                    result = omTrSalesDraftSOVinBLL.SaveVin(record, recordSOVin, isNew);
                    var recDraftSOVin =omTrSalesDraftSOVinBLL.Select4Table(draftSONo, salesModelCode, salesModelYear, colourCode);
                    if (result)
                    {
                        tranScope.Complete();
                        save = true;
                        msg = "Simpan Draft Sales Order - Detil Lain-lain berhasil";
                    }

                    omTrSalesDraftSOModelColourBLL = null;
                    omTrSalesDraftSOVinBLL = null;
                    omTrSalesDraftSOBLL = null;
                    uid = null;

                    return Json(new
                    {
                        success = save,
                        allowInput = true,
                        message = msg,
                        data = new
                        {
                            DraftSOVin = recDraftSOVin
                        },
                        Status = record.Status
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Simpan Draft Sales Order - Detil Lain-lain tidak berhasil. Periksa kembali inputan anda atau hubungi SDMS support",
                    error_log = ex.Message
                });
            }
        }

        public JsonResult SaveDraftSOOthers(OmTrSalesDraftSOModelOthers model, string draftSONo, string salesModelCode, decimal salesModelYear, string otherCode)
        {
            bool save = false;
            string msg = "Simpan Draft Sales Order - Detil Aksesoris tidak berhasil";
            try
            {
                using (var tranScope = new TransactionScope(TransactionScopeOption.RequiresNew,
                    new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    model.DraftSONo = draftSONo;
                    model.SalesModelCode = salesModelCode;
                    model.SalesModelYear = salesModelYear;

                    var uid = CurrentUser.UserId;
                    var omTrSalesDraftSOBLL = new OmTrSalesDraftSOBLL(ctx, uid);
                    var record = omTrSalesDraftSOBLL.GetRecord(draftSONo);
                    if (record != null)
                    {
                        if (record.Status != "0" && record.Status != "1")
                        {
                            return Json(new { success = true, isNull = false, allowInput = false });
                        }
                    }

                    var omTrSalesDraftSOModelOthersBLL = new OmTrSalesDraftSOModelOthersBLL(ctx, uid);
                    OmTrSalesDraftSOModelOthers recordOthers;
                    bool isNew = p_PrepareRecordOthers(omTrSalesDraftSOModelOthersBLL, model, draftSONo, salesModelCode, salesModelYear, otherCode, 
                        record.CustomerCode, out recordOthers);

                    bool result = false;
                    // to make sure that if new detail is inserted, status PO back to 0
                    record = omTrSalesDraftSOBLL.GetRecord(draftSONo);
                    if (record == null) return Json(new { success = true, isNull = true, allowInput = false });
                    else
                    {
                        record.Status = "0";
                        record.LastUpdateBy = uid;
                        record.LastUpdateDate = ctx.CurrentTime;
                        Helpers.ReplaceNullable(record);
                        result = ctx.SaveChanges() >= 0;
                    }

                    result = omTrSalesDraftSOModelOthersBLL.SaveAccOthers(record, recordOthers, isNew);
                    var recDraftSOOthers = omTrSalesDraftSOModelOthersBLL.Select4Table(draftSONo, salesModelCode, salesModelYear);
                    var omTrSalesDraftSOModelBLL = new OmTrSalesDraftSOModelBLL(ctx, uid);
                    var recDraftSOModel = omTrSalesDraftSOModelBLL.Select4Table(record.DraftSONo);
                    omTrSalesDraftSOModelBLL = null;

                    if (result)
                    {
                        tranScope.Complete();
                        save = true;
                        msg = "Simpan Draft Sales Order - Detil Aksesoris berhasil";
                    }

                    omTrSalesDraftSOModelOthersBLL = null;
                    omTrSalesDraftSOBLL = null;
                    uid = null;

                    return Json(new
                    {
                        success = save,
                        allowInput = true,
                        message = msg,
                        data = new
                        {
                            DraftSOModel = recDraftSOModel,
                            DraftSOOthers = recDraftSOOthers
                        },
                        Status = record.Status
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Simpan Draft Sales Order - Detil Aksesoris tidak berhasil. Periksa kembali inputan anda atau hubungi SDMS support",
                    error_log = ex.Message
                });
            }
        }

        public JsonResult DeleteDraftSO(string draftSONo)
        {
            try
            {
                var json = Json(new { });
                var uid = CurrentUser.UserId;

                var omTrSalesDraftSOBLL = new OmTrSalesDraftSOBLL(ctx, uid);
                bool result = omTrSalesDraftSOBLL.DeleteDraftSO(draftSONo);
                if (result)
                {
                    var record = omTrSalesDraftSOBLL.GetRecord(draftSONo);
                    if (record == null) return Json(new { success = true, isExistDraftSO = false });

                    var status = record.Status;
                    record = null;
                    uid = null;
                    omTrSalesDraftSOBLL = null;

                    json = Json(new
                    {
                        success = true,
                        isExistDraftSO = true,
                        message = "Delete Draft Sales Order berhasil",
                        Status = status
                    });
                }
                else
                {

                    json = Json(new { success = false, message = "Delete Gagal" });
                }
                omTrSalesDraftSOBLL = null;

                return json;
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Delete Draft Sales Order - Detil Aksesoris tidak berhasil. Periksa kembali inputan anda atau hubungi SDMS support",
                    error_log = ex.Message
                });
            }
        }

        public JsonResult DeleteDraftSO_Validated(string draftSONo)
        {
            try
            {
                bool isAllowDelete = false;
                var omTrSalesDraftSOBLL = new OmTrSalesDraftSOBLL(ctx, CurrentUser.UserId);
                var record = omTrSalesDraftSOBLL.GetRecord(draftSONo);
                omTrSalesDraftSOBLL = null;
                if (record != null)
                {
                    if (record.Status != "0" && record.Status != "1")
                    {
                        isAllowDelete = false;
                    }
                    else
                    {
                        if (Convert.ToInt32(record.Status) > 1)
                        {
                            isAllowDelete = false;
                        }
                        else{
                            isAllowDelete = true;
                        }
                    }
                }
                record = null;

                return Json(new { success = true, allowDelete = isAllowDelete });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Terjadi kesalahan pada saat validasi hapus data, silahkan hubungi SDMS support",
                    error_log = ex.Message
                });
            }
        }

        public JsonResult DeleteDraftSOModel(string draftSONo, string salesModelCode, decimal salesModelYear)
        {
            try
            {
                var json = Json(new { });
                var uid = CurrentUser.UserId;
                var omTrSalesDraftSOBLL = new OmTrSalesDraftSOBLL(ctx, uid);
                bool result = omTrSalesDraftSOBLL.DeleteDraftSO(draftSONo, salesModelCode, salesModelYear);

                if (result)
                {
                    var record = omTrSalesDraftSOBLL.GetRecord(draftSONo);
                    if (record == null) return Json(new { success = true, isExistDraftSO = false });

                    var omTrSalesDraftSOModelBLL = new OmTrSalesDraftSOModelBLL(ctx, uid);
                    var recDraftSOModel = omTrSalesDraftSOModelBLL.Select4Table(record.DraftSONo);
                    omTrSalesDraftSOModelBLL = null;
                    var status = record.Status;
                    record = null;
                    uid = null;

                    json = Json(new
                    {
                        success = true,
                        isExistDraftSO = true,
                        message = "Delete Draft Sales Order - Model berhasil",
                        Status = status,
                        DraftSOModel = recDraftSOModel
                    });
                }
                else
                {
                    json = Json(new { success = false, message = "Delete Gagal" });
                }
                omTrSalesDraftSOBLL = null;
                
                return json;
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Delete Draft Sales Order - Model tidak berhasil. Periksa kembali inputan anda atau hubungi SDMS support",
                    error_log = ex.Message
                });
            }
        }

        public JsonResult DeleteDraftSOModel_Validated(string draftSONo)
        {
            try
            {
                bool isAllowDelete = false;
                var omTrSalesDraftSOBLL = new OmTrSalesDraftSOBLL(ctx, CurrentUser.UserId);
                var record = omTrSalesDraftSOBLL.GetRecord(draftSONo);
                omTrSalesDraftSOBLL = null;
                if (record != null)
                {
                    if (record.Status != "0" && record.Status != "1")
                    {
                        isAllowDelete = false;
                    }
                    else
                    {
                        isAllowDelete = true;
                    }
                }
                record = null;

                return Json(new { success = true, allowDelete = isAllowDelete });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Terjadi kesalahan pada saat validasi hapus data, silahkan hubungi SDMS support",
                    error_log = ex.Message
                });
            }
        }

        public JsonResult DeleteDraftSOModelColour(string draftSONo, string salesModelCode, decimal salesModelYear, string colourCode)
        {
            try
            {
                var json = Json(new { });
                var uid = CurrentUser.UserId;
                var omTrSalesDraftSOBLL = new OmTrSalesDraftSOBLL(ctx, uid);
                bool result = omTrSalesDraftSOBLL.DeleteDraftSO(draftSONo, salesModelCode, salesModelYear, colourCode);

                if (result)
                {
                    var record = omTrSalesDraftSOBLL.GetRecord(draftSONo);
                    if (record == null) return Json(new { success = true, isExistDraftSO = false });

                    var omTrSalesDraftSOModelColourBLL = new OmTrSalesDraftSOModelColourBLL(ctx, uid);
                    var recDraftSOModelColour = omTrSalesDraftSOModelColourBLL.Select4Table(draftSONo, salesModelCode, salesModelYear);
                    omTrSalesDraftSOModelColourBLL = null;

                    var omTrSalesDraftSOModelBLL = new OmTrSalesDraftSOModelBLL(ctx, uid);
                    var recDraftSOModel = omTrSalesDraftSOModelBLL.Select4Table(record.DraftSONo);
                    omTrSalesDraftSOModelBLL = null;


                    var status = record.Status;
                    record = null;
                    uid = null;
                    omTrSalesDraftSOBLL = null;

                    json = Json(new
                    {
                        success = true,
                        isExistDraftSO = true,
                        message = "Delete Draft Sales Order - Model Colour berhasil",
                        Status = status,
                        DraftSOModel = recDraftSOModel,
                        DraftSOModelColour = recDraftSOModelColour
                    });
                }
                else
                {

                    json = Json(new { success = false, message = "Delete Gagal" });
                }
                omTrSalesDraftSOBLL = null;

                return json;
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Delete Draft Sales Order - Model Colour tidak berhasil. Periksa kembali inputan anda atau hubungi SDMS support",
                    error_log = ex.Message
                });
            }
        }

        public JsonResult DeleteDraftSOModelColour_Validated(string draftSONo)
        {
            try
            {
                bool isAllowDelete = false;
                var omTrSalesDraftSOBLL = new OmTrSalesDraftSOBLL(ctx, CurrentUser.UserId);
                var record = omTrSalesDraftSOBLL.GetRecord(draftSONo);
                omTrSalesDraftSOBLL = null;
                if (record != null)
                {
                    if (record.Status != "0" && record.Status != "1")
                    {
                        isAllowDelete = false;
                    }
                    else
                    {
                        isAllowDelete = true;
                    }
                }
                record = null;

                return Json(new { success = true, allowDelete = isAllowDelete });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Terjadi kesalahan pada saat validasi hapus data, silahkan hubungi SDMS support",
                    error_log = ex.Message
                });
            }
        }

        public JsonResult DeleteDraftSOVin(string draftSONo, string salesModelCode, decimal salesModelYear, string colourCode, int seqNo)
        {
            try
            {
                var json = Json(new { });
                var uid = CurrentUser.UserId;

                var omTrSalesDraftSOBLL = new OmTrSalesDraftSOBLL(ctx, uid);
                bool result = omTrSalesDraftSOBLL.DeleteDraftSO(draftSONo, salesModelCode, salesModelYear, colourCode, seqNo);
                if (result)
                {
                    var record = omTrSalesDraftSOBLL.GetRecord(draftSONo);
                    if (record == null) return Json(new { success = true, isExistDraftSO = false });

                    var omTrSalesDraftSOVinBLL = new OmTrSalesDraftSOVinBLL(ctx, uid);
                    var recordDraftSOVin = omTrSalesDraftSOVinBLL.Select4Table(draftSONo, salesModelCode, salesModelYear, colourCode);
                    omTrSalesDraftSOVinBLL = null;

                    var omTrSalesDraftSOModelBLL = new OmTrSalesDraftSOModelBLL(ctx, uid);
                    var recDraftSOModel = omTrSalesDraftSOModelBLL.Select4Table(record.DraftSONo);
                    omTrSalesDraftSOModelBLL = null;

                    var status = record.Status;
                    record = null;
                    uid = null;
                    omTrSalesDraftSOBLL = null;

                    json = Json(new
                    {
                        success = true,
                        isExistDraftSO = true,
                        message = "Delete Draft Sales Order - Detil Lain-lain berhasil",
                        Status = status,
                        DraftSOModel = recDraftSOModel,
                        DraftSOVin = recordDraftSOVin
                    });
                }
                else
                {

                    json = Json(new { success = false, message = "Delete Gagal" });
                }
                omTrSalesDraftSOBLL = null;

                return json;
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Delete Draft Sales Order - Detil Lain-lain tidak berhasil. Periksa kembali inputan anda atau hubungi SDMS support",
                    error_log = ex.Message
                });
            }
        }

        public JsonResult DeleteDraftSOVin_Validated(string draftSONo)
        {
            try
            {
                bool isAllowDelete = false;
                var omTrSalesDraftSOBLL = new OmTrSalesDraftSOBLL(ctx, CurrentUser.UserId);
                var record = omTrSalesDraftSOBLL.GetRecord(draftSONo);
                omTrSalesDraftSOBLL = null;
                if (record != null)
                {
                    if (record.Status != "0" && record.Status != "1")
                    {
                        isAllowDelete = false;
                    }
                    else
                    {
                        isAllowDelete = true;
                    }
                }
                record = null;

                return Json(new { success = true, allowDelete = isAllowDelete });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Terjadi kesalahan pada saat validasi hapus data, silahkan hubungi SDMS support",
                    error_log = ex.Message
                });
            }
        }

        public JsonResult DeleteDraftSOOthers(string draftSONo, string salesModelCode, decimal salesModelYear, string otherCode)
        {
            try
            {
                var json = Json(new { });
                var uid = CurrentUser.UserId;

                var omTrSalesDraftSOBLL = new OmTrSalesDraftSOBLL(ctx, uid);
                bool result = omTrSalesDraftSOBLL.DeleteDraftSoOthers(draftSONo, salesModelCode, salesModelYear, otherCode);
                if (result)
                {
                    var record = omTrSalesDraftSOBLL.GetRecord(draftSONo);
                    if (record == null) return Json(new { success = true, isExistDraftSO = false });

                    var omTrSalesDraftSOModelOthersBLL = new OmTrSalesDraftSOModelOthersBLL(ctx, uid);
                    var recordDraftSOOthers = omTrSalesDraftSOModelOthersBLL.Select4Table(draftSONo, salesModelCode, salesModelYear);
                    omTrSalesDraftSOModelOthersBLL = null;

                    var omTrSalesDraftSOModelBLL = new OmTrSalesDraftSOModelBLL(ctx, uid);
                    var recDraftSOModel = omTrSalesDraftSOModelBLL.Select4Table(record.DraftSONo);
                    omTrSalesDraftSOModelBLL = null;

                    var status = record.Status;
                    record = null;
                    uid = null;
                    omTrSalesDraftSOBLL = null;

                    json = Json(new
                    {
                        success = true,
                        isExistDraftSO = true,
                        message = "Delete Draft Sales Order - Detil Aksesoris berhasil",
                        Status = status,
                        DraftSOModel = recDraftSOModel,
                        DraftSOOthers = recordDraftSOOthers
                    });
                }
                else
                {

                    json = Json(new { success = false, message = "Delete Gagal" });
                }
                omTrSalesDraftSOBLL = null;

                return json;
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Delete Draft Sales Order - Detil Aksesoris tidak berhasil. Periksa kembali inputan anda atau hubungi SDMS support",
                    error_log = ex.Message
                });
            }
        }

        public JsonResult DeleteDraftSOOthers_Validated(string draftSONo)
        {
            try
            {
                bool isAllowDelete = false;
                var omTrSalesDraftSOBLL = new OmTrSalesDraftSOBLL(ctx, CurrentUser.UserId);
                var record = omTrSalesDraftSOBLL.GetRecord(draftSONo);
                omTrSalesDraftSOBLL = null;
                if (record != null)
                {
                    if (record.Status != "0" && record.Status != "1")
                    {
                        isAllowDelete = false;
                    }
                    else
                    {
                        isAllowDelete = true;
                    }
                }
                record = null;

                return Json(new { success = true, allowDelete = isAllowDelete });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Terjadi kesalahan pada saat validasi hapus data, silahkan hubungi SDMS support",
                    error_log = ex.Message
                });
            }
        }

        public JsonResult PrintDraftSO(string draftSONo)
        {
            try
            {
                //check whether detail SO data doesn't exist
                var uid = CurrentUser.UserId;
                var omTrSalesDraftSOModelBLL = new OmTrSalesDraftSOModelBLL(ctx, uid);
                var recModel = omTrSalesDraftSOModelBLL.Select4Table(draftSONo);
                if (recModel.Count < 1)
                {
                    return Json(new { success = true, message = GetMessage(SysMessages.MSG_5047), isDataExist = false });
                }

                var omTrSalesDraftSOBLL = new OmTrSalesDraftSOBLL(ctx, uid);
                var record = omTrSalesDraftSOBLL.GetRecord(draftSONo);
                if (record == null)
                    return Json(new { success = true, message = "", isDataExist = false });
                else
                {
                    if (record.Status == "0" || record.Status.Trim() == "") record.Status = "1";
                }

                record.LastUpdateBy = uid;
                record.LastUpdateDate = ctx.CurrentTime;
                Helpers.ReplaceNullable(record);
                bool result = ctx.SaveChanges() > 0;

                if (result)
                {
                    return Json(new { success = true, Status = record.Status, isDataExist = true });
                }
                else
                {
                    return Json(new { success = true, message = "", isDataExist = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Terjadi kesalahan pada saat Print data, silahkan hubungi SDMS support",
                    error_log = ex.Message
                });
            }
        }

        public JsonResult ApproveDraftSO(string draftSONo, bool isLinkToITS)
        {
            try
            {
                using (var tranScope = new TransactionScope(TransactionScopeOption.RequiresNew,
                    new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    var json = Json(new { });
                    var uid = CurrentUser.UserId;
                    var omTrSalesDraftSOBLL = new OmTrSalesDraftSOBLL(ctx, uid);
                    var record = omTrSalesDraftSOBLL.GetRecord(draftSONo);
                    if (record == null)
                        return Json(new { success = true, isExistDraftSO = false });
                    else
                    {
                        record.ApproveBy = uid;
                        record.ApproveDate = ctx.CurrentTime;
                        record.LastUpdateDate = ctx.CurrentTime;
                        record.LastUpdateBy = uid;
                        record.Status = "2";
                    }

                    bool result = omTrSalesDraftSOBLL.ApproveDraftSO(record, isLinkToITS);
                    if (result)
                    {
                        record = omTrSalesDraftSOBLL.GetRecord(draftSONo);
                        if (record != null) json = Json(new { success = true, message = "Proses Approve SO berhasil.", isExistDraftSO = true });
                    }
                    else
                    {
                        record = omTrSalesDraftSOBLL.GetRecord(draftSONo);
                        if (record != null) json = Json(new { success = true, message = "", isExistDraftSO = true });
                    }
                    tranScope.Complete();
                    omTrSalesDraftSOBLL = null;

                    return json;
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Approved Draft Sales Order tidak berhasil. Periksa kembali inputan anda atau hubungi SDMS support",
                    error_log = ex.Message
                });
            }
        }

        public JsonResult UnApproveDraftSO(string draftSONo, bool isLinkToITS)
        {
            try
            {
                var json = Json(new { });
                using (var tranScope = new TransactionScope(TransactionScopeOption.RequiresNew,
                    new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    var omTrSalesDraftSOBLL = new OmTrSalesDraftSOBLL(ctx, CurrentUser.UserId);
                    var record = omTrSalesDraftSOBLL.GetRecord(draftSONo);
                    if (record == null)
                        return Json(new { success = true, isExistDraftSO = false });
                    else
                    {
                        record.ApproveBy = "";
                        record.LastUpdateDate = ctx.CurrentTime;
                        record.LastUpdateBy = CurrentUser.UserId;
                        record.Status = "0";
                    }

                    bool result = omTrSalesDraftSOBLL.UnApprovedDraftSO(record, isLinkToITS);
                    if (result)
                    {
                        record = omTrSalesDraftSOBLL.GetRecord(draftSONo);
                        tranScope.Complete();
                        if (record != null) json = Json(new { success = true, message = "Proses Unproved SO berhasil.", isExistDraftSO = true, isUnproved = true });
                    }
                    else
                    {
                        record = omTrSalesDraftSOBLL.GetRecord(draftSONo);
                        if (record != null) json = Json(new { success = true, message = "Proses Unproved SO Gagal.", isExistDraftSO = true, isUnproved = false });
                    }
                    omTrSalesDraftSOBLL = null;

                    return json;
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Unproved Draft Sales Order - Detil Aksesoris tidak berhasil. Periksa kembali inputan anda atau hubungi SDMS support",
                    error_log = ex.Message
                });
            }
        }

        public JsonResult UnApproveDraftSO_Validated(string draftSONo)
        {
            try
            {
                var omTrSalesDraftSOModelBLL = new OmTrSalesDraftSOModelBLL(ctx, CurrentUser.UserId);
                if (!omTrSalesDraftSOModelBLL.CheckingQtySO(draftSONo))
                {
                    return Json(new { success = true, message = "Dokumen ini tidak bisa di UnApprove karena sudah dilakuakan SO. \nSilahkan delete detail model di SO terlebih dahulu" });
                }
                else
                {
                    return Json(new { success = true, message = "" });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Validasi UnApproved Draft Sales Order tidak berhasil. Periksa kembali inputan anda atau hubungi SDMS support",
                    error_log = ex.Message
                });
            }
        }
        #endregion

        #region ---- Validation Function ----
        public decimal TotalPricelistSell(string groupPriceCode = "", string salesModelCode = "", decimal salesModelYear = 0)
        {
            decimal totalPricelistSell = 0;
            OmMstPricelistSell priceSellRecord = ctx.OmMstPricelistSells.Find(CompanyCode, BranchCode,
                            groupPriceCode, salesModelCode, salesModelYear);
            if (priceSellRecord != null)
            {
                totalPricelistSell = priceSellRecord.Total ?? 0;
            }

            return totalPricelistSell;
        }

        public JsonResult TotalChecked(OmTrSalesDraftSOModel model, bool chkTotal, string groupPriceCode, string customerCode)
        {
            decimal TotAftDisc = 0;
            decimal DPPAftDisc = 0;
            decimal PPnAftDisc = 0;
            decimal PPnBMAftDisc = 0;
            decimal DiscPPn = 0;
            decimal TotBfrDisc = model.BeforeDiscTotal ?? 0;

            decimal ppnPct = 0;
            decimal ppnBmPct = 0;
            decimal totalPriceBeforeDisc = 0;

            var omTrSalesSOBLL = new OmTrSalesSOBLL(ctx, CurrentUser.UserId);
            var rec = omTrSalesSOBLL.Select4Tax(customerCode).FirstOrDefault();
            if (rec != null) ppnPct = rec.TaxPct ?? 0;
            else ppnPct = 0;
            omTrSalesSOBLL = null;

            OmMstModel recModel = ctx.OmMstModels.Find(CompanyCode, model.SalesModelCode);
            if (recModel != null) ppnBmPct = recModel.PpnBmPctSell ?? 0;
            else ppnBmPct = 0;

            totalPriceBeforeDisc = model.BeforeDiscTotal ?? 0;

            if (chkTotal == true)
            {
                model.AfterDiscTotal = model.AfterDiscTotal ?? 0;
                model.AfterDiscDPP = model.AfterDiscDPP ?? 0;
                if (model.AfterDiscTotal != null)
                {
                    decimal totalPrice = model.AfterDiscTotal ?? 0;
                    decimal dpp = totalPrice / ((100 + ppnPct + ppnBmPct) / 100);
                    decimal ppnBm = dpp * (ppnBmPct / 100);
                    decimal ppn = totalPrice - dpp - ppnBm;
                    decimal disc = totalPriceBeforeDisc - totalPrice;

                    // return to Client
                    TotAftDisc = Convert.ToDecimal(totalPrice.ToString("n0"));
                    DPPAftDisc = Convert.ToDecimal(dpp.ToString("n0"));
                    PPnAftDisc = Convert.ToDecimal(ppn.ToString("n0"));
                    PPnBMAftDisc = Convert.ToDecimal(ppnBm.ToString("n0"));
                    DiscPPn = (disc < 0) ? 0 : Convert.ToDecimal(disc.ToString("n0"));
                    if (disc < 0)
                    {
                        decimal totBfrDiscOld = 0;
                        OmMstPricelistSell priceSellRecord = ctx.OmMstPricelistSells.Find(CompanyCode, BranchCode,
                                groupPriceCode, model.SalesModelCode, model.SalesModelYear);
                        if (priceSellRecord != null)
                        {
                            totBfrDiscOld = priceSellRecord.Total ?? 0;
                        }
                        decimal totBfrDiscNew = 0;
                        totBfrDiscNew = totBfrDiscOld + Math.Abs(disc);
                        TotBfrDisc = Convert.ToDecimal(totBfrDiscNew.ToString("n0"));
                    }
                }
            }
            else
            {
                if (model.AfterDiscDPP != null)
                {
                    decimal dpp = model.AfterDiscDPP ?? 0;
                    decimal ppn = dpp * (ppnPct / 100);
                    decimal ppnBm = dpp * (ppnBmPct / 100);
                    decimal totalPrice = dpp + ppn + ppnBm;
                    decimal disc = totalPriceBeforeDisc - totalPrice;

                    DPPAftDisc = Convert.ToDecimal(dpp.ToString("n0"));
                    PPnAftDisc = Convert.ToDecimal(ppn.ToString("n0"));
                    PPnBMAftDisc = Convert.ToDecimal(ppnBm.ToString("n0"));
                    TotAftDisc = Convert.ToDecimal(totalPrice.ToString("n0"));
                    DiscPPn = (disc < 0) ? 0 : Convert.ToDecimal(disc.ToString("n0"));
                    if (disc < 0)
                    {
                        decimal totBfrDiscOld = 0;
                        OmMstPricelistSell priceSellRecord = ctx.OmMstPricelistSells.Find(CompanyCode, BranchCode,
                                groupPriceCode, model.SalesModelCode, model.SalesModelYear);
                        if (priceSellRecord != null)
                        {
                            totBfrDiscOld = priceSellRecord.Total ?? 0;
                        }
                        decimal totBfrDiscNew = 0;
                        totBfrDiscNew = totBfrDiscOld + Math.Abs(disc);
                        TotBfrDisc = Convert.ToDecimal(totBfrDiscNew.ToString("n0"));
                    }
                }
            }

            return Json(new
            {
                AfterDiscTotal = TotAftDisc,
                AfterDiscDPP = DPPAftDisc,
                AfterDiscPPn = PPnAftDisc,
                AfterDiscPPnBM = PPnBMAftDisc,
                DiscIncludePPn = DiscPPn,
                BeforeDiscTotal = TotBfrDisc
            });
        }

        public JsonResult CalculateDiscountOthers(OmTrSalesDraftSOModelOthers model, string customerCode)
        {
            decimal ppnPct = 0;
            decimal beforeDiscTotal = 0;
            decimal afterDiscTotal = 0;
            decimal afterDiscDPP = 0;
            decimal afterDiscPPn = 0;


            var omTrSalesSOBLL = new OmTrSalesSOBLL(ctx, CurrentUser.UserId);
            var rec = omTrSalesSOBLL.Select4Tax(customerCode).FirstOrDefault();
            if (rec != null) ppnPct = rec.TaxPct ?? 0;
            else ppnPct = 0;
            omTrSalesSOBLL = null;

            decimal totBfrDisc = model.BeforeDiscTotal ?? 0;
            decimal totAftDisc = model.AfterDiscTotal ?? 0;
            if (totBfrDisc < totAftDisc)
            {
                totBfrDisc = totAftDisc;
                beforeDiscTotal = Convert.ToDecimal(totAftDisc.ToString("n0"));
            }

            decimal othersDPPBfrDisc = totBfrDisc / ((100 + ppnPct) / 100);
            decimal othersPPnBfrDisc = totBfrDisc - othersDPPBfrDisc;
            decimal othersDPPAftDisc = totAftDisc / ((100 + ppnPct) / 100);
            decimal othersPPnAftDisc = totAftDisc - othersDPPAftDisc;

            afterDiscTotal = Convert.ToDecimal(totAftDisc.ToString("n0"));
            afterDiscDPP = Convert.ToDecimal(othersDPPAftDisc.ToString("n0"));
            afterDiscPPn = Convert.ToDecimal(othersPPnAftDisc.ToString("n0"));

            return Json(new
            {
                AfterDiscTotal = afterDiscTotal,
                AfterDiscDPP = afterDiscDPP,
                AfterDiscPPn = afterDiscPPn
            });
        }

        public JsonResult CheckingQtySO(string draftSONo)
        {
            try
            {
                var omTrSalesDraftSOModelBLL = new OmTrSalesDraftSOModelBLL(ctx, CurrentUser.UserId);
                var isCheckingQtySO = omTrSalesDraftSOModelBLL.CheckingQtySO(draftSONo);
                omTrSalesDraftSOModelBLL = null;

                return Json(new { success = true, CheckingQtySO = isCheckingQtySO });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Terjadi Kesalahan, Hubungi SDMS Support",
                    error_log = ex.Message
                });
            }
        }
        
        public JsonResult CheckBottomPrice(string draftSONo, string groupPriceCode)
        {
            try
            {
                bool status = true;
                string msg = "";
                decimal minStaff = 0;
                decimal afterDiscTotal = 0;

                var uid = CurrentUser.UserId;

                var omTrSalesSOModelBLL = new OmTrSalesSOModelBLL(ctx, uid);

                var records = omTrSalesSOModelBLL.Select4Table(draftSONo);
                if (records.Count > 0)
                {
                    foreach (OmTrSalesSOModel rec in records)
                    {
                        OmMstPricelistSell sell = ctx.OmMstPricelistSells.Find(CompanyCode, BranchCode, groupPriceCode, rec.SalesModelCode, rec.SalesModelYear);
                        if (sell != null)
                        {
                            minStaff = sell.TotalMinStaff ?? 0;
                            afterDiscTotal = rec.AfterDiscTotal ?? 0;
                            if (afterDiscTotal < minStaff)
                            {
                                // Equal Method GnMstApprovalBLL.getRecordByUserID(GnMstDocument.SOR, user.UserId);
                                var oGnMstApproval = ctx.GnMstApprovals.Where(p => p.CompanyCode == CompanyCode
                                    && p.BranchCode == BranchCode && p.DocumentType == GnMstDocumentConstant.SOR
                                    && p.UserID == uid).ToList();
                                
                                if (oGnMstApproval != null)
                                {
                                }
                                else
                                {
                                    msg += "Total harga model " + rec.SalesModelCode + " lebih kecil dari harga batas bawah" + "\n";
                                    status = false;
                                }

                                oGnMstApproval = null;
                            }
                            else
                            {
                            }
                        }

                        sell = null;
                    }

                }
                return Json(new { success = true, message = msg, allowApprove = status });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Terjadi Kesalahan, Hubungi SDMS Support",
                    error_log = ex.Message
                });
            }
        }
        #endregion
        #endregion

        #region -- Prepare Data --
        private bool p_PrepareRecord(OmTrSalesDraftSO model, bool chkType, bool dtpReff, bool chkLeasing, bool dtpRequest
            , OmTrSalesDraftSOBLL omTrSalesDraftSOBLL, out OmTrSalesDraftSO record)
        {
            var uid = CurrentUser.UserId;
            string salescode = "";
            var omTrSalesSOBLL = new OmTrSalesSOBLL(ctx, uid);
            omTrSalesSOBLL.SelectGroupAR(model.CustomerCode, "100", out salescode);
            omTrSalesSOBLL = null;

            bool isNew = false;
            record = omTrSalesDraftSOBLL.GetRecord(model.DraftSONo);
            if (record == null)
            {
                record = new OmTrSalesDraftSO();
                isNew = true;
                record.CompanyCode = CompanyCode;
                record.BranchCode = BranchCode;
                record.DraftSONo = model.DraftSONo;
                record.StatusDraftSO = model.SalesType;

                record.CreatedBy = uid;
                record.CreatedDate = DateTime.Now;
            }
            record.DraftSODate = model.DraftSODate;
            record.SalesType =  (chkType) ? "1" : "0";
            record.ProspectNo = model.ProspectNo ;

            record.RefferenceNo = model.RefferenceNo;
            if (dtpReff)
                record.RefferenceDate = model.RefferenceDate;
            
            record.CustomerCode = model.CustomerCode;
            record.TOPCode = model.TOPCode;
            record.TOPDays = model.TOPDays.Equals(string.Empty) ? 0 : model.TOPDays;

            record.Salesman = model.Salesman;
            record.SalesCode = salescode;

            record.GroupPriceCode = model.GroupPriceCode;

            record.isLeasing = model.isLeasing;
            if (chkLeasing)
            {
                record.LeasingCo = model.LeasingCo;
                record.Installment = model.Installment;
                record.FinalPaymentDate = model.FinalPaymentDate;
            }
            else
            {
                record.LeasingCo = "";
                record.Installment = 0;
                record.FinalPaymentDate = Convert.ToDateTime("1900-01-01 00:00:00.000");
            }

            record.PrePaymentAmt = (model.PrePaymentAmt == null) ? 0 : model.PrePaymentAmt;
            record.CommissionAmt = model.CommissionAmt.Equals(string.Empty) ? 0 : model.CommissionAmt; 

            if (dtpRequest)
                record.RequestDate = model.RequestDate;

            if (model.Remark != null)
                model.Remark = model.Remark.Trim();

            record.Remark = model.Remark;
            record.Status = "0";

            record.LastUpdateBy = uid;
            record.LastUpdateDate = DateTime.Now;

            return isNew;
        }

        private bool p_PrepareRecordModel(OmTrSalesDraftSOModelBLL omTrSalesDraftSOModelBLL, OmTrSalesDraftSOModel model
            , string draftSONo, string groupPriceCode, string customerCode, decimal discount,
            out OmTrSalesDraftSOModel recordModel, out string msgWarning, out string msgWarning2)
        {
            string ex = string.Empty;
            string ex2 = string.Empty;
            msgWarning = ex;
            msgWarning2 = ex2;
            decimal salesModelYear = model.SalesModelYear ?? 0;
            recordModel = omTrSalesDraftSOModelBLL.GetRecord(draftSONo, model.SalesModelCode, salesModelYear);

            bool isNew = false;
            if (recordModel == null)
            {
                isNew = true;
                recordModel = new OmTrSalesDraftSOModel();
                recordModel.CompanyCode = CompanyCode;
                recordModel.BranchCode = BranchCode;
                recordModel.DraftSONo = draftSONo;
                recordModel.SalesModelCode = model.SalesModelCode;
                recordModel.SalesModelYear = model.SalesModelYear;
                recordModel.QuantityDraftSO = 0;
                recordModel.CreatedBy = CurrentUser.UserId;
                recordModel.CreatedDate = DateTime.Now;
            }
            MstModelYear year = ctx.MstModelYear.Find(CompanyCode, model.SalesModelCode, model.SalesModelYear);
            if (year != null)
            {
                recordModel.ChassisCode = year.ChassisCode;
            }

            OmMstPricelistSell sell = ctx.OmMstPricelistSells.Find(CompanyCode, BranchCode,
                groupPriceCode, model.SalesModelCode, model.SalesModelYear);
            if (sell != null)
            {
                if (sell.Total <= Convert.ToDecimal(model.AfterDiscTotal))
                {
                    recordModel.BeforeDiscTotal = Convert.ToDecimal(model.AfterDiscTotal);
                    recordModel.BeforeDiscDPP = Convert.ToDecimal(model.AfterDiscDPP);
                    recordModel.BeforeDiscPPn = Convert.ToDecimal(model.AfterDiscPPn);
                }
                else
                {
                    recordModel.BeforeDiscTotal = sell.Total;
                    CustomerProfitCenter oGnMstCustomerProfitCenter = ctx.CustomerProfitCenters.Find(CompanyCode, BranchCode, customerCode, ProfitCenter);
                    if (oGnMstCustomerProfitCenter == null)
                    {
                        ex = "ProfitCenter pelanggan belum ada";
                        recordModel = null;
                    }
                    if (oGnMstCustomerProfitCenter.TaxCode.Equals("PPN"))
                    {
                        recordModel.BeforeDiscDPP = sell.DPP;
                        recordModel.BeforeDiscPPn = sell.PPn;
                    }
                    else
                    {
                        recordModel.BeforeDiscDPP = sell.Total;
                        recordModel.BeforeDiscPPn = 0;
                    }
                }
                recordModel.BeforeDiscPPnBM = sell.PPnBM;
            }

            try
            {
                recordModel.AfterDiscTotal = (model.AfterDiscTotal == null) ? 0 : model.AfterDiscTotal;
                recordModel.AfterDiscDPP = (model.AfterDiscDPP == null) ? 0 : model.AfterDiscDPP;
                recordModel.AfterDiscPPn = (model.AfterDiscPPn == null) ? 0 : model.AfterDiscPPn;
                recordModel.AfterDiscPPnBM = (model.AfterDiscPPnBM == null) ? 0 : model.AfterDiscPPnBM;

                recordModel.DiscExcludePPn = (recordModel.BeforeDiscDPP == null) ? 0 : recordModel.BeforeDiscDPP - recordModel.AfterDiscDPP;
                recordModel.DiscIncludePPn =  (discount == null) ? 0 : discount ;
            }
            catch
            {
                ex2 = "Input tidak valid";
                recordModel = null;
            }
            recordModel.OthersPPn = 0;
            recordModel.OthersDPP = 0;
            recordModel.QuantitySO = 0;

            recordModel.Remark = (model.Remark != null) ? model.Remark.Trim() : "";

            recordModel.LastUpdateBy = CurrentUser.UserId;
            recordModel.LastUpdateDate = DateTime.Now;

            return isNew;
        }

        private bool p_PrepareRecordModelColour(OmTrSalesDraftSOModelColourBLL omTrSalesDraftSOModelColourBLL, OmTrSalesDraftSOModelColour model,
            out OmTrSalesDraftSOModelColour record)
        {
            bool isNew = false;
            var salesModelYear = model.SalesModelYear ?? 0;
            record = omTrSalesDraftSOModelColourBLL.GetRecord(model.DraftSONo, model.SalesModelCode, salesModelYear, model.ColourCode);
            if (record == null)
            {
                isNew = true;
                record = new OmTrSalesDraftSOModelColour();
                record.CompanyCode = CompanyCode;
                record.BranchCode = BranchCode;
                record.DraftSONo = model.DraftSONo;
                record.SalesModelCode = model.SalesModelCode;
                record.SalesModelYear = salesModelYear;
                record.ColourCode = model.ColourCode;

                record.CreatedBy = CurrentUser.UserId;
                record.CreatedDate = DateTime.Now;
            }

            record.Quantity = model.Quantity ?? 0;
            record.Remark = (model.Remark != null) ? model.Remark.Trim() : "";

            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;

            return isNew;
        }

        private bool p_PreparerecordSOVin(OmTrSalesDraftSOVinBLL omTrSalesDraftSOVinBLL, OmTrSalesDraftSOVin model, string draftSONo, string salesModelCode, 
            decimal salesModelYear, string colourCode, int seqNo, out OmTrSalesDraftSOVin recordSOVin)
        {
            bool isNew = false;
            recordSOVin = omTrSalesDraftSOVinBLL.GetRecord(draftSONo, salesModelCode, salesModelYear, colourCode, seqNo);
            if (recordSOVin == null)
            {
                isNew = true;
                recordSOVin = new OmTrSalesDraftSOVin();
                recordSOVin.CompanyCode = CompanyCode;
                recordSOVin.BranchCode = BranchCode;
                recordSOVin.DraftSONo = draftSONo;
                recordSOVin.SalesModelCode = salesModelCode;
                recordSOVin.SalesModelYear = salesModelYear;
                recordSOVin.ColourCode = colourCode;

                recordSOVin.CreatedBy = CurrentUser.UserId;
                recordSOVin.CreatedDate = DateTime.Now;
            }

            recordSOVin.EndUserName = model.EndUserName;
            recordSOVin.SupplierBBN = model.SupplierBBN;
            recordSOVin.CityCode = model.CityCode;
            recordSOVin.BBN = model.BBN;
            recordSOVin.KIR = model.KIR;
            recordSOVin.Remark = model.Remark;
            recordSOVin.StatusReq = "0";

            recordSOVin.LastUpdateBy = CurrentUser.UserId;
            recordSOVin.LastUpdateDate = ctx.CurrentTime;

            return isNew;
        }


        private bool p_PrepareRecordOthers(OmTrSalesDraftSOModelOthersBLL omTrSalesDraftSOModelOthersBLL, OmTrSalesDraftSOModelOthers model, 
            string draftSONo, string salesModelCode, decimal salesModelYear, string otherCode, string customerCode, out OmTrSalesDraftSOModelOthers recordOthers)
        {
            bool isNew = false;

            var uid = CurrentUser.UserId;
            recordOthers = omTrSalesDraftSOModelOthersBLL.GetRecord(draftSONo, salesModelCode, salesModelYear, otherCode);
            if (recordOthers == null)
            {
                isNew = true;
                recordOthers = new OmTrSalesDraftSOModelOthers();
                recordOthers.CompanyCode = CompanyCode;
                recordOthers.BranchCode = BranchCode;
                recordOthers.DraftSONo = draftSONo;
                recordOthers.SalesModelCode = salesModelCode;
                recordOthers.SalesModelYear = salesModelYear;
                recordOthers.OtherCode = otherCode;

                recordOthers.CreatedBy = uid;
                recordOthers.CreatedDate = ctx.CurrentTime;
            }
            var omTrSalesSOBLL = new OmTrSalesSOBLL(ctx, uid);
            decimal ppnPct = 0;
            var recSalesSO = omTrSalesSOBLL.Select4Tax(customerCode);
            if (recSalesSO.Count > 0) ppnPct = recSalesSO.FirstOrDefault().TaxPct ?? 0;

            decimal totBfrDisc = model.BeforeDiscTotal ?? 0; //decimal.Parse(txtOthersTotBfrDisc.Text);
            decimal totAftDisc = model.AfterDiscTotal ?? 0; //decimal.Parse(txtOthersTotAftDisc.Text);
            recordOthers.AfterDiscTotal = totAftDisc;
            recordOthers.BeforeDiscTotal = totBfrDisc;

            decimal othersDPPBfrDisc = totBfrDisc / ((100 + ppnPct) / 100);
            recordOthers.BeforeDiscDPP = othersDPPBfrDisc;
            decimal othersPPnBfrDisc = totBfrDisc - othersDPPBfrDisc;
            recordOthers.BeforeDiscPPn = othersPPnBfrDisc;
            decimal othersDPPAftDisc = totAftDisc / ((100 + ppnPct) / 100);
            recordOthers.AfterDiscDPP = othersDPPAftDisc;
            decimal othersPPnAftDisc = totAftDisc - othersDPPAftDisc;
            recordOthers.AfterDiscPPn = othersPPnAftDisc;
            recordOthers.DiscExcludePPn = recordOthers.BeforeDiscDPP - recordOthers.AfterDiscDPP;
            recordOthers.DiscIncludePPn = totBfrDisc - totAftDisc;
            recordOthers.Remark = (model.Remark != null) ? model.Remark.Trim() : "";
            recordOthers.LastUpdateBy = uid;
            recordOthers.LastUpdateDate = ctx.CurrentTime;

            return isNew;
        }

        #endregion

        #region -- Private Method --
        private IQueryable<Select4LookupCustomer> p_Customer()
        {
            var uid = CurrentUser.UserId;
            var omTrSalesDraftSOBLL = new OmTrSalesDraftSOBLL(ctx, uid);
            var records = omTrSalesDraftSOBLL.Select4LookupCustomer(ProfitCenter);
            omTrSalesDraftSOBLL = null;

            return records;
        }

        private IQueryable<Select4LookupCustomer> p_Customer2()
        {
            var uid = CurrentUser.UserId;
            var omTrSalesDraftSOBLL = new OmTrSalesDraftSOBLL(ctx, uid);
            var records = omTrSalesDraftSOBLL.Select4LookupCustomer2(ProfitCenter);
            omTrSalesDraftSOBLL = null;

            return records;
        }
        
        private IQueryable<Select4LookupSalesman> p_Salesman()
        {
            var records = from a in ctx.Employees
                          where a.CompanyCode == CompanyCode && a.BranchCode == BranchCode
                          select new Select4LookupSalesman
                          {
                              EmployeeID = a.EmployeeID,
                              EmployeeName = a.EmployeeName,
                              TitleCode = a.TitleCode,
                              TitleName = (from b in ctx.LookUpDtls
                                           where b.CompanyCode == a.CompanyCode
                                               && b.CodeID == GnMstLookUpHdr.TitleCode && b.LookUpValue == a.TitleCode
                                           select b.LookUpValueName).FirstOrDefault()
                          };

            return records;
        }

        private IQueryable<LookUpDtl> p_TOPCList()
        {
            var records = ctx.LookUpDtls.Where(p => p.CompanyCode == CompanyCode && p.CodeID == "TOPC")
                .OrderBy(p => p.SeqNo);

            return records;
        }

        private IQueryable<Leasing> p_Leasing()
        {
            var omTrSalesBLL = OmTrSalesBLL.Instance(CurrentUser.UserId);
            var records = omTrSalesBLL.Select4Leasing();
            omTrSalesBLL = null;

            return records;
        }

        private IQueryable<ITSNoDraftSO> p_ITS()
        {
            var omTrSalesBLL = OmTrSalesBLL.Instance(CurrentUser.UserId);
            var records = omTrSalesBLL.SelectITSNoDraftSO();
            omTrSalesBLL = null;

            return records;
        }

        private string p_TOPCInterval(string TOPCode)
        {
            var rec = p_TOPCList().Where(p => p.LookUpValue == TOPCode).FirstOrDefault();
            
            return (rec != null) ? rec.ParaValue : "";
        }

        private string p_ModelDescription(string groupPriceCode = "" , string salesModelCode = "", decimal salesModelYear = 0)
        {
            var records = (from a in ctx.MstModelYear
                            join b in ctx.OmMstPricelistSells on new { a.CompanyCode, a.SalesModelCode, a.SalesModelYear, GroupPriceCode = groupPriceCode }
                            equals new { b.CompanyCode, b.SalesModelCode, b.SalesModelYear, b.GroupPriceCode } into _b
                            from b in _b.DefaultIfEmpty()
                            where a.CompanyCode == CompanyCode && a.SalesModelCode == salesModelCode && a.Status == "1"
                            orderby a.SalesModelYear
                            select a ).Distinct()
                            .Select(p => new {
                                p.SalesModelYear,
                                p.SalesModelDesc,
                                p.ChassisCode
                            }).ToList();

            var rec = records.Where(p => p.SalesModelYear == salesModelYear).FirstOrDefault(); 
            return (rec != null) ? rec.SalesModelDesc : "";


            //if (inquiryNumber != ""){
            //    var inqNumber = Convert.ToInt32(inquiryNumber);
            //    var records = from a in ctx.OmMstModels
            //                    join b in ctx.PmKdps on new { a.CompanyCode, BranchCode = BranchCode, a.GroupCode, a.TransmissionType, a.TypeCode }
            //                    equals new { b.CompanyCode, b.BranchCode, GroupCode = b.TipeKendaraan, TransmissionType = b.Transmisi, TypeCode = b.Variant }
            //                    where a.CompanyCode == CompanyCode && b.InquiryNumber == inqNumber
            //                    orderby a.SalesModelCode
            //                    select new { 
            //                    a.SalesModelCode,
            //                    a.SalesModelDesc
            //                    };

            //    var rec = records.Where(p => p.SalesModelCode == salesModelCode).FirstOrDefault();
            //    return (rec != null) ? rec.SalesModelDesc : "";
            //}
            //else{
            //    var listStatus = "1,2".Split(',');

            //    var records = (from a in ctx.MstModelYear
            //                    where a.CompanyCode == CompanyCode && listStatus.Contains(a.Status)
            //                    orderby a.SalesModelCode
            //                    select new
            //                    {
            //                        a.SalesModelCode,
            //                        a.SalesModelDesc
            //                    }).Distinct();

            //    var rec = records.Where(p => p.SalesModelCode == salesModelCode).FirstOrDefault(); 
            //    return (rec != null) ? rec.SalesModelDesc : "";
            //}
        }

        private string p_ColourName(string salesModelCode = "", string colourCode = "")
        {
                var records = (from a in ctx.OmMstModelColours
                               where a.CompanyCode == CompanyCode && a.SalesModelCode == salesModelCode && a.Status == "1"
                               select new
                               {
                                   a.ColourCode,
                                   colourDesc = (from b in ctx.MstRefferences
                                                 where b.RefferenceCode == a.ColourCode
                                                     && b.CompanyCode == a.CompanyCode && b.RefferenceType == OmMstRefferenceConstant.CLCD
                                                 select b.RefferenceDesc1).FirstOrDefault(),
                                   a.Remark
                               });
                var rec = records.Where(p => p.ColourCode == colourCode).FirstOrDefault(); 
                return (rec != null) ? rec.colourDesc : "";
        }

        private string p_BNNDesc(string salesModelCode = "", decimal salesModelYear = 0, string bnnCode = "")
        {
            var records = (from a in ctx.Supplier
                           from b in ctx.SupplierProfitCenter
                           from c in ctx.MstBBNKIR
                           where a.CompanyCode == b.CompanyCode
                           && b.CompanyCode == c.CompanyCode
                           && a.SupplierCode == b.SupplierCode
                           && b.SupplierCode == c.SupplierCode
                           && a.CompanyCode == CompanyCode
                           && c.BranchCode == BranchCode
                           && b.ProfitCenterCode == ProfitCenter
                           && c.Status == "1"
                           && c.SalesModelCode == salesModelCode
                           && c.SalesModelYear == salesModelYear
                           orderby a.SupplierCode
                           select new
                           {
                               a.SupplierCode,
                               a.SupplierName
                           }
                            ).Distinct();
            var rec = records.Where(p => p.SupplierCode == bnnCode).FirstOrDefault(); 
            return (rec != null) ? rec.SupplierName : "";
        }

        private string p_CityName(string supplierCode = "", string salesModelCode = "", decimal salesModelYear = 0, string cityCode = "")
        {
            var records = (from a in ctx.LookUpDtls
                join b in ctx.MstBBNKIR
                on new
                {
                    a.CompanyCode,
                    a.LookUpValue,
                    BranchCode = BranchCode,
                    a.CodeID,
                    Status = "1",
                    SupplierCode = supplierCode,
                    SalesModelCode = salesModelCode,
                    SalesModelYear = salesModelYear
                }
                equals new
                {
                    b.CompanyCode,
                    LookUpValue = b.CityCode,
                    b.BranchCode,
                    CodeID = "CITY",
                    b.Status,
                    b.SupplierCode,
                    b.SalesModelCode,
                    b.SalesModelYear
                }
                orderby a.LookUpValue
                select new
                {
                    a.LookUpValue,
                    a.LookUpValueName,
                    b.BBN,
                    b.KIR
                }).Distinct();

            var rec = records.Where(p => p.LookUpValue == cityCode).FirstOrDefault(); 
            return (rec != null) ? rec.LookUpValueName : "";
        }

        private string p_AccesoriesDesc(string accesoriesCode)
        {
            var records = from a in ctx.MstRefferences
                          where a.CompanyCode == CompanyCode
                          && a.RefferenceType == OmMstRefferenceConstant.OTHS
                          && a.Status != "0"
                          select new
                          {
                              a.RefferenceCode,
                              a.RefferenceDesc1
                          };

            var rec = records.Where(p => p.RefferenceCode == accesoriesCode).FirstOrDefault(); 
            return (rec != null) ? rec.RefferenceDesc1 : "";
        }
        #endregion
    }
}
