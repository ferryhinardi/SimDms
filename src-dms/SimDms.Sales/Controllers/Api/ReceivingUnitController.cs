using SimDms.Sales.Models;
using SimDms.Sales.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.Common.Models;
using SimDms.Common;
namespace SimDms.Sales.Controllers.Api
{
    public class ReceivingUnitController : BaseController
    {
        public JsonResult GeData(string Vehicle)
        {
            if (Vehicle.Length < 17)
            {
                return Json(new { success = false, msg = "Silakan Scan Ulang!" });
            }

            var ChassisCode = Vehicle.Substring(0, 11);
            var ChassisNo = Vehicle.Substring(11);

            var record = ctx.OmMstVehicles.Find(CompanyCode, ChassisCode, Convert.ToInt32(ChassisNo));

            string Colour = "";

            var ColourX = ctx.MstRefferences.Where(a => a.CompanyCode == CompanyCode && a.RefferenceType == "COLO" && a.RefferenceCode == record.ColourCode)
                .FirstOrDefault();

            if (ColourX !=null) Colour = ColourX.RefferenceDesc1;

            var DealerAbbreviation = ctx.GnMstDealerMapping.Where(a => a.DealerCode == CompanyCode).FirstOrDefault().DealerAbbreviation;

            return Json(new { success = record != null, data = record, Colour = Colour, DealerAbbreviation = DealerAbbreviation });
        }

        public JsonResult Save(string Vehicle, string Driver)
        {
            var WarehouseCode = "";
            var ChassisCode = Vehicle.Substring(0, 11);
            var ChassisNo = Vehicle.Substring(11);
            var OmMstVehicles = ctx.OmMstVehicles.Find(CompanyCode, ChassisCode, Convert.ToInt32(ChassisNo));
            var Warehouse = ctx.LookUpDtls.Where(a => a.CompanyCode == CompanyCode && a.CodeID == "UNWH" && a.LookUpValue == CompanyCode).FirstOrDefault();

            if (Warehouse != null) { 
                WarehouseCode = Warehouse.LookUpValueName;
            } 
            else {
                return Json(new { succcess = false, message = "Save gagal! \n Silakan SetUp WarehouseCode di Master LookUpDtl dengan CodeID = 'UNWH' terlebih dahulu!" }); 
            }

            var record = ctx.omTrInventWH.Find(CompanyCode, ChassisCode, Convert.ToInt32(ChassisNo), WarehouseCode);

            if (record == null)
            {
                record = new omTrInventWH
                {
                    CompanyCode = CompanyCode,
                    ChassisCode = ChassisCode,
                    ChassisNo = Convert.ToInt32(ChassisNo),
                    WarehouseCode = WarehouseCode,
                    LocationCode = "",
                    ReceivingDate = ctx.CurrentTime,
                    DeliveryDate = Convert.ToDateTime("1900-01-01"),
                    Remark = "",
                    ProcessStatus = "0",
                    Status = "I",
                    IsActive = true,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = ctx.CurrentTime,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = ctx.CurrentTime,
                    isLocked = false,
                    LockingBy = "",
                    LockingDate = Convert.ToDateTime("1900-01-01")
                };
                ctx.omTrInventWH.Add(record);
            }

            var record2 = ctx.omTrInventWHHistory.Find(CompanyCode, ChassisCode, Convert.ToInt32(ChassisNo), 1);

            if (record2 == null)
            {
                record2 = new omTrInventWHHistory
                {
                    CompanyCode = CompanyCode,
                    ChassisCode = ChassisCode,
                    ChassisNo = Convert.ToInt32(ChassisNo),
                    SeqNo = 1,
                    BranchCode = BranchCode,
                    Driver = Driver,
                    DeliveryTo = "",
                    Remark = "",
                    Status = "I",
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = ctx.CurrentTime,
                };
                ctx.omTrInventWHHistory.Add(record2);
            }

            var record3 = ctx.omTrInventMovement.Find(CompanyCode, WarehouseCode, ctx.CurrentTime.Year, ctx.CurrentTime.Month, OmMstVehicles.SalesModelCode, OmMstVehicles.SalesModelYear, OmMstVehicles.ColourCode);
            int Year = ctx.CurrentTime.Month == 1 ? (ctx.CurrentTime.AddYears(-1)).Year : DateTime.Today.Year;
            int Month = ctx.CurrentTime.Month == 1 ? (ctx.CurrentTime.AddMonths(-1)).Month : DateTime.Today.Month;
            var omTrInventMovement = ctx.omTrInventMovement
                .Where(a => a.CompanyCode == CompanyCode && a.WarehouseCode == WarehouseCode && a.Year == Year && a.Month == Month && a.SaleModelCode == OmMstVehicles.SalesModelCode && a.SalesModelYear == OmMstVehicles.SalesModelYear && a.ColourCode == OmMstVehicles.ColourCode)
                .FirstOrDefault();
            int BOM = 0;
            if (omTrInventMovement != null) { BOM = Convert.ToInt32(omTrInventMovement.BOM); }

            if (record3 == null)
            {
                record3 = new omTrInventMovement
                {
                    CompanyCode = CompanyCode,
                    WarehouseCode = WarehouseCode,
                    Year = ctx.CurrentTime.Year,
                    Month = ctx.CurrentTime.Month,
                    SaleModelCode = OmMstVehicles.SalesModelCode,
                    SalesModelYear = Convert.ToInt32(OmMstVehicles.SalesModelYear),
                    ColourCode = OmMstVehicles.ColourCode,
                    BOM = BOM,
                    IN = 1,
                    OUT = 0,
                    EOM = 1,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = ctx.CurrentTime,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = ctx.CurrentTime,
                };
                ctx.omTrInventMovement.Add(record3);
            }
            else
            {
                record3.IN = record3.IN + 1;
                record3.EOM = record3.EOM + 1;
                record3.LastUpdateBy = CurrentUser.UserId;
                record3.LastUpdateDate = ctx.CurrentTime;
            }

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, data = record, message = "Data Saved!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}