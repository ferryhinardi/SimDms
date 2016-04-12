using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;
using SimDms.Common.DcsWs;

namespace SimDms.Sales.Controllers.Api
{
    public class InquiryInventoryController : BaseController
    {
        //Inquiry TransferOut

        public JsonResult searchTRansferOut(string Status, DateTime TransferOutDate, DateTime TransferOutDateTo, string TrasferOutNo, string TrasferOutNoTo, string WareHouseCodeFrom, string WareHouseCodeFromTo, string BranchCodeTo, string BranchCodeToLast, string WareHouseCodeTo, string WareHouseCodeToLast)
        {
            var sqlstr = ctx.Database.SqlQuery<InquiryTrTransferOutView>("sp_InquiryTransferOut '" + CompanyCode + "','" + BranchCode + "','" + Status + "','" + TransferOutDate + "','" + TransferOutDateTo + "','" + TrasferOutNo + "','" + TrasferOutNoTo + "','" + WareHouseCodeFrom + "','" + WareHouseCodeFromTo + "','" + BranchCodeTo + "','" + BranchCodeToLast + "','" + WareHouseCodeTo + "','" + WareHouseCodeToLast + "'").AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult GetTRansferOutDetail(string TransferOutNo)
        {
            var sqlstr = ctx.Database.SqlQuery<TransferOutDetailView>("sp_InquiryTransferOutDetail '" + CompanyCode + "','" + BranchCode + "','" + TransferOutNo + "'").AsQueryable();
            return Json(sqlstr);
        }

        //Inquiry TransferIn

        public JsonResult searchTRansferIn(string Status, DateTime TransferInDate, DateTime TransferInDateTo, string TransferInNo, string TransferInNoTo, string TransferOutNo, string TransferOutNoTo, string WareHouseCodeTo, string WareHouseCodeToLast)
        {
            var sqlstr = ctx.Database.SqlQuery<InquiryTrTransferInView>("sp_InquiryTransferIn '" + CompanyCode + "','" + BranchCode + "','" + Status + "','" + TransferInDate + "','" + TransferInDateTo + "','" + TransferInNo + "','" + TransferInNoTo + "','" + TransferOutNo + "','" + TransferOutNoTo + "','" + WareHouseCodeTo + "','" + WareHouseCodeToLast + "'").AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult GetTRansferInDetail(string TransferInNo)
        {
            var sqlstr = ctx.Database.SqlQuery<TransferInDetailView>("sp_InquiryTransferInDetail '" + CompanyCode + "','" + BranchCode + "','" + TransferInNo + "'").AsQueryable();
            return Json(sqlstr);
        }

        //Inquiry Stok Kendaraan

        public JsonResult searchStokKendaraan(string Year, string Month, string WarehouseCode, string WarehouseCodeTo, string SalesModelCode, string SalesModelCodeTo, string SalesModelYear, string SalesModelYearTo, string ColourCode, string ColourCodeTo)
        {
            bool isMainDealer = DealerCode() == "MD";
            var DB = ctx.CompanyMappings.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode).FirstOrDefault().DbMD;

            var sqlstr = isMainDealer ? ctx.Database.SqlQuery<InquiryInventQtyVehicleView>("sp_InquiryStokKendaraan '" + CompanyCode + "','" + BranchCode + "','" 
                + Year + "','" + Month + "','" + WarehouseCode + "','" + WarehouseCodeTo + "','" + SalesModelCode + "','" + SalesModelCodeTo + "','" + SalesModelYear + "','" 
                + SalesModelYearTo + "','" + ColourCode + "','" + ColourCodeTo + "','" + DB + "'").AsQueryable() : 
                ctx.Database.SqlQuery<InquiryInventQtyVehicleView>("sp_InquiryStokKendaraan '" + CompanyMD + "','" + BranchMD + "','" 
                + Year + "','" + Month + "','" + WarehouseCode + "','" + WarehouseCodeTo + "','" + SalesModelCode + "','" + SalesModelCodeTo + "','" + SalesModelYear + "','"
                + SalesModelYearTo + "','" + ColourCode + "','" + ColourCodeTo + "','" + DB + "'").AsQueryable();
            return Json(sqlstr);
        }

        //Inquiry Stok perlengkapan

        public JsonResult searchStokPerlengkapan(string Status, string Year, string Month, string PerlengkapanCode, string PerlengkapanCodeTo)
        {
            var sqlstr = ctx.Database.SqlQuery<InquiryInventQtyPerlengkapanView>("sp_InquiryStokPerlengkapan '" + CompanyCode + "','" + BranchCode + "','" + Status + "','" + Year + "','" + Month + "','" + PerlengkapanCode + "','" + PerlengkapanCodeTo + "'").AsQueryable();
            return Json(sqlstr);
        }

        //Inquiry Data Kendaraan

        public JsonResult searchDataKendaraan(string Status, 
            string WareHouseCode, 
            string WareHouseCodeTo, 
            string SalesModelCode, 
            string SalesModelCodeTo, 
            string SalesModelYear, 
            string SalesModelYearTo, 
            string ChassisCode, 
            string ChassisCodeTo, 
            string ChassisNo, 
            string ChassisNoTo)
        {
            bool ismd = DealerCode() == "MD";
            bool otomatis=cekOtomatis();
            var scomp = !otomatis?CurrentUser.CompanyCode : ismd ? CurrentUser.CompanyCode : CompanyMD;
            var sbranch = !otomatis ? CurrentUser.BranchCode :ismd  ? CurrentUser.BranchCode : BranchMD;

            var sqlstr = otomatis ? 
                ctxMD.Database.SqlQuery<InquiryMstVehicleView>("exec sp_InquirDataKendaraan  @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12",
               scomp, sbranch, Status, WareHouseCodeTo, WareHouseCodeTo, SalesModelCode, SalesModelCodeTo,
               SalesModelYear, SalesModelYearTo, ChassisCode, ChassisCodeTo, ChassisNo, ChassisNoTo).AsQueryable() :

               ctx.Database.SqlQuery<InquiryMstVehicleView>("exec sp_InquirDataKendaraan  @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12",
               scomp, sbranch, Status, WareHouseCodeTo, WareHouseCodeTo, SalesModelCode, SalesModelCodeTo,
               SalesModelYear, SalesModelYearTo, ChassisCode, ChassisCodeTo, ChassisNo, ChassisNoTo).AsQueryable();

           
            return Json(sqlstr);
        }

        public JsonResult GetDetailDataKendaraan(string ChassisCode, string ChassisNo)
        {
            //bool isMainDealer = DealerCode() == "MD";
            //var sqlstr = isMainDealer ? ctx.Database.SqlQuery<InquiryDetailDataKendaraan>("sp_InquirDetailDataKendaraan '" + CompanyCode + "','" + BranchCode + "','" +
            //    ChassisCode + "','" + ChassisNo + "'").AsQueryable() : ctxMD.Database.SqlQuery<InquiryDetailDataKendaraan>("sp_InquirDetailDataKendaraan '" + CompanyMD + "','" 
            //    + BranchMD + "','" + ChassisCode + "','" + ChassisNo + "'").AsQueryable();

            var sqlstr = ctx.Database.SqlQuery<InquiryDetailDataKendaraan>("exec sp_InquirDetailDataKendaraan '" + CompanyCode + "','"
                + BranchCode + "','" + ChassisCode + "','" + ChassisNo + "'").AsQueryable();

            return Json(sqlstr.FirstOrDefault());
        }

        //Inquiry Data DCS

        public JsonResult ValidateDCS()
        {
            DcsWsSoapClient ws = new DcsWsSoapClient();

            var Stat = 0;
            var Status = false;
            if (ws.IsValid())
            {
                Stat = 1;
                Status = true;
            }
            else
            {
                Stat = -1;
                Status = false;
            }
            ws = null;
            return Json(new { success = true, status = 1 });
        }
    }
}
                                                                         