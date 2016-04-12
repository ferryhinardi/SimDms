using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Sales.Controllers.Api
{
    public class InquirySalesController : BaseController
    {
        //Inquiry SO

        public JsonResult searchSO(string Status, DateTime SODate, DateTime SODateTo, string SONo, string SONoTo, string CustomerCode, string CustomerCodeTo, string Salesman, string SalesmanTo)
        {
            var sqlstr = ctx.Database.SqlQuery<InquiryTrSalesSOView>("sp_InquirySalesOrder '" + CompanyCode + "','" + BranchCode + "','" + Status + "','" + SODate + "','" + SODateTo + "','" + SONo + "','" + SONoTo + "','" + CustomerCode + "','" + CustomerCodeTo + "','" + Salesman + "','" + SalesmanTo + "'").AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult DetailSOModel(string SONo)
        {
            var gridDetail = ctx.OmTrSalesSOModels.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.SONo == SONo);
            return Json(new { success = true, detail = gridDetail });
        }

        public JsonResult DetailSOModelColour(string SONo, string SalesModelCode)
        {
            var gridDetail = ctx.OmTrSalesSOModelColours.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.SONo == SONo && a.SalesModelCode == SalesModelCode);
            return Json(new { success = true, detail = gridDetail });
        }

        public JsonResult DetailSOVin(string SONo, string SalesModelCode, string ColourCode)
        {
            var gridDetail = ctx.omTrSalesSOVins.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.SONo == SONo && a.SalesModelCode == SalesModelCode && a.ColourCode == ColourCode);
            return Json(new { success = true, detail = gridDetail });
        }

        //Inquiry DO

        public JsonResult searchDO(string Status, DateTime DODate, DateTime DODateTo, string SONo, string SONoTo, string DONo, string DONoTo)
        {
            var sqlstr = ctx.Database.SqlQuery<InquiryTrSalesDOView>("sp_InquiryDeliveryOrder '" + CompanyCode + "','" + BranchCode + "','" + Status + "','" + DODate + "','" + DODateTo + "','" + SONo + "','" + SONoTo + "','" + DONo + "','" + DONoTo + "'").AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult DetailDO(string DONo)
        {
            var sqlstr = ctx.Database.SqlQuery<InquiryTrSalesDODetailView>("sp_InquiryDeliveryOrderDetail '" + CompanyCode + "','" + BranchCode + "','" + DONo + "'").AsQueryable();
            return Json(sqlstr);
        }

        //Inquiry BPK

        public JsonResult searchBPK(string Status, DateTime BPKDate, DateTime BPKDateTo, string SONo, string SONoTo, string DONo, string DONoTo, string BPKNo, string BPKNoTo)
        {
            var sqlstr = ctx.Database.SqlQuery<InquiryTrSalesBPKView>("sp_InquiryBPK'" + CompanyCode + "','" + BranchCode + "','" + Status + "','" + BPKDate + "','" + BPKDateTo + "','" + SONo + "','" + SONoTo + "','" + DONo + "','" + DONoTo + "','" + BPKNo + "','" + BPKNoTo + "'").AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult DetailBPK(string BPKNo)
        {
            var sqlstr = ctx.Database.SqlQuery<InquiryTrSalesBPKDetailView>("sp_InquiryBPKDetail '" + CompanyCode + "','" + BranchCode + "','" + BPKNo + "'").AsQueryable();
            return Json(sqlstr);
        }

        //Inquiry Invoice

        public JsonResult searchInvoice(string Status, DateTime InvoiceDate, DateTime InvoiceDateTo, string SONo, string SONoTo, string CustomerCode, string CustomerCodeTo, string InvoiceNo, string InvoiceNoTo, string SKPKNo, string SKPKNoTo)
        {
            var sqlstr = ctx.Database.SqlQuery<InquiryTrSalesInvoiceView>("sp_InquiryInvoice'" + CompanyCode + "','" + BranchCode + "','" + Status + "','" + InvoiceDate + "','" + InvoiceDateTo + "','" + SONo + "','" + SONoTo + "','" + CustomerCode + "','" + CustomerCodeTo + "','" + InvoiceNo + "','" + InvoiceNoTo + "','" + SKPKNo + "','" + SKPKNoTo + "'").AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult searchDN(string Status, DateTime InvoiceDate, DateTime InvoiceDateTo, string SONo, string SONoTo, string CustomerCode, string CustomerCodeTo, string InvoiceNo, string InvoiceNoTo, string SKPKNo, string SKPKNoTo)
        {
            var sqlstr = ctx.Database.SqlQuery<InquiryTrSalesInvoiceView>("sp_InquiryInvoice'" + CompanyCode + "','" + BranchCode + "','" + Status + "','" + InvoiceDate + "','" + InvoiceDateTo + "','" + SONo + "','" + SONoTo + "','" + CustomerCode + "','" + CustomerCodeTo + "','" + InvoiceNo + "','" + InvoiceNoTo + "','" + InvoiceNo + "','" + InvoiceNoTo + "'").AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult BPK(string InvoiceNo)
        {
            var gridDetail = ctx.omTrSalesInvoiceBPK.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.InvoiceNo == InvoiceNo);
            return Json(new { success = true, detail = gridDetail });
        }

        public JsonResult InvoiceVin(string InvoiceNo, string BPKNo)
        {
            var gridDetail = ctx.omTrSalesInvoiceModel.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.InvoiceNo == InvoiceNo && a.BPKNo == BPKNo);
            return Json(new { success = true, detail = gridDetail });
        }

        //Inquiry Return

        public JsonResult searchSalesReturn(string Status, DateTime ReturnDate, DateTime ReturnDateTo, string InvoiceNo, string InvoiceNoTo, string ReturnNo, string ReturnNoTo, string CustomerCode, string CustomerCodeTo, string WareHouseCode, string WareHouseCodeTo)
        {
            var sqlstr = ctx.Database.SqlQuery<InquiryTrSalesReturnView>("sp_InquirySalesReturn'" + CompanyCode + "','" + BranchCode + "','" + Status + "','" + ReturnDate + "','" + ReturnDateTo + "','" + InvoiceNo + "','" + InvoiceNoTo + "','" + ReturnNo + "','" + ReturnNoTo + "','" + CustomerCode + "','" + CustomerCodeTo + "','" + WareHouseCode + "','" + WareHouseCodeTo + "'").AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult ReturnDetailModel(string ReturnNo)
        {
            var sqlstr = ctx.Database.SqlQuery<InquiryTrSalesReturnDetailModelView>("sp_InquirySalesReturnDetailModel '" + CompanyCode + "','" + BranchCode + "','" + ReturnNo + "'").AsQueryable();
            return Json(sqlstr);
        }

        //Inquiry PerlengkapanOut

        public JsonResult searchPerlengkapanOut(string Status, string PerlengkapanType, DateTime PerlengkapanDate, DateTime PerlengkapanDateTo, string NoPerlengkapan, string NoPerlengkapanTo)
        {
            var sqlstr = ctx.Database.SqlQuery<inquiryTrPerlengkapanOutView>
                ("sp_InquiryPerlengkapanOut '" + CompanyCode + "','" + BranchCode + "','" + Status + "','" + PerlengkapanType + "','" + PerlengkapanDate + "','" + PerlengkapanDateTo + "','" + NoPerlengkapan + "','" + NoPerlengkapanTo + "'").AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult GetDetailPerlengkapanOut(string PerlengkapanNo)
        {
            var gridDetail = ctx.OmTrSalesPerlengkapanOutModels.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.PerlengkapanNo == PerlengkapanNo);
            return Json(new { success = true, detail = gridDetail });
        }

        //Inquiry Permohonan Faktur Polis

        public JsonResult searchPermohonan(string Status, string Jenis, DateTime ReqDate, DateTime ReqDateTo, string ReqNo, string ReqNoTo, string CustomerCode, string CustomerCodeTo)
        {
            var sqlstr = ctx.Database.SqlQuery<inquiryTrSalesReqView>
                ("sp_InquirySalesReq '" + CompanyCode + "','" + BranchCode + "','" + Status + "','" + Jenis + "','" + ReqDate + "','" + ReqDateTo + "','" + ReqNo + "','" + ReqNoTo + "','" + CustomerCode + "','" + CustomerCodeTo + "'").AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult GetDetailPermohonan(string ReqNo)
        {
            var gridDetail = ctx.omTrSalesReqDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ReqNo == ReqNo);
            return Json(new { success = true, detail = gridDetail });
        }

        //Inquiry Permohonan Faktur Polis

        public JsonResult searchSPK(string Status, DateTime SPKDate, DateTime SPKDateTo, string SPKNo, string SPKNoTo, string ChassisNo, string ChassisNoTo, string SupplierCode, string SupplierCodeTo)
        {
            var sqlstr = ctx.Database.SqlQuery<inquiryTrSPKView>
                ("sp_InquirySPK '" + CompanyCode + "','" + BranchCode + "','" + Status + "','" + SPKDate + "','" + SPKDateTo + "','" + SPKNo + "','" + SPKNoTo + "','" + ChassisNo + "','" + ChassisNoTo + "','" + SupplierCode + "','" + SupplierCodeTo + "'").AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult GetDetailSPK(string SPKNo)
        {
            var gridDetail = ctx.omTrSalesSPKDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.SPKNo == SPKNo);
            return Json(new { success = true, detail = gridDetail });
        }

        public JsonResult InqProdSales(DateTime StartDate, DateTime EndDate, string Area, string DealerID, string OutletID, string BranchHeadID, string SalesHeadID, string SalesCoordinatorID, string SalesmanID)
        {
            var query = string.Format(@"usprpt_OmInqProductivitySalesPivot '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}'
                    ", StartDate, EndDate, Area, DealerID, OutletID, BranchHeadID, SalesHeadID, SalesCoordinatorID, SalesmanID);
            var queryable = ctx.Database.SqlQuery<InqProdSales>(query).AsQueryable();
            return Json(new { success = true, grid = queryable });
        }

        public JsonResult ITS(DateTime StartDate, DateTime EndDate, string Area, string DealerID, string OutletID)
        {
            var query = string.Format(@"uspfn_InquiryITS '{0}','{1}','{2}','{3}','{4}'
                    ", StartDate, EndDate, Area, DealerID, OutletID);
            var queryable = ctx.Database.SqlQuery<InqProdSales>(query).AsQueryable();
            return Json(new { success = true, grid = queryable });
        }

        public JsonResult GetReportOmRpSalRgs039Web(string startDate, string endDate, string area, string dealer, string outlet, string branchHead, string salesHead, string salesCoordinator, string salesman, string SalesType)
        {
            var query = string.Format(@"usprpt_OmRpSalRgs039Web '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'
                    ", startDate, endDate, area, dealer, outlet, branchHead, salesHead, salesCoordinator, salesman, SalesType);
            var queryable = ctx.Database.SqlQuery<InqSales>(query).AsQueryable();
            
            if (queryable != null)
            {
                return Json(new { success = true, grid = queryable });
            }

            return Json(new { success = false });
        }

        public JsonResult GetOmRpSalRgs039PivotWeb(string startDate, string endDate, string area, string dealer, string outlet, string branchHead, string salesHead, string salesCoordinator, string salesman, string SalesType)
        {
            var query = string.Format(@"usprpt_OmRpSalRgs039PivotWeb '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'
                    ", startDate, endDate, area, dealer, outlet, branchHead, salesHead, salesCoordinator, salesman, SalesType);
            var queryable = ctx.Database.SqlQuery<InqSales>(query).AsQueryable();
            return Json(new { success = true, grid = queryable });
        }

        public JsonResult GetProdType()
        {
            var data = ctx.CoProfiles.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode).FirstOrDefault();
            return Json(new { success = true, pType = data.ProductType });
        }
    }
}

