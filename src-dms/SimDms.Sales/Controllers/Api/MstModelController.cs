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
    public class MstModelController : BaseController
    {
        public JsonResult ModelCode(MstModel model)
        {
            var record = ctx.MstModels.Find(CompanyCode, model.SalesModelCode);

            return Json(new { success = record != null, data = record });
        }

        public JsonResult Save(MstModel model)
        {
            string msg = "";
            var record = ctx.MstModels.Find(CompanyCode, model.SalesModelCode);

            if (record == null)
            {
                record = new MstModel
                {
                    CompanyCode = CompanyCode,
                    SalesModelCode = model.SalesModelCode,
                    SalesModelDesc = model.SalesModelDesc,
                    FakturPolisiDesc = model.FakturPolisiDesc,
                    EngineCode = model.EngineCode,
                    PpnBmCodeBuy = model.PpnBmCodeSell,
                    PpnBmPctBuy = model.PpnBmPctBuy,
                    PpnBmCodeSell = model.PpnBmCodeSell,
                    PpnBmPctSell = model.PpnBmPctSell,
                    PpnBmCodePrincipal = model.PpnBmCodePrincipal,
                    PpnBmPctPrincipal = model.PpnBmPctPrincipal,
                    Remark = model.Remark == null ? "" : model.Remark,
                    BasicModel = model.BasicModel,
                    TechnicalModelCode = model.TechnicalModelCode,
                    ProductType = model.ProductType,
                    TransmissionType = model.TransmissionType,
                    IsChassis = model.IsChassis,
                    IsCbu = model.IsCbu,
                    SMCModelCode = model.SMCModelCode == null ? "" : model.SMCModelCode,
                    GroupCode = model.GroupCode == null ? "" : model.GroupCode,
                    TypeCode = model.TypeCode == null ? "" : model.TypeCode,
                    CylinderCapacity = model.CylinderCapacity == null ? 0 : model.CylinderCapacity ,
                    fuel = model.fuel == null ? "" : model.fuel,
                    ModelPrincipal = model.ModelPrincipal == null ? "" : model.ModelPrincipal,
                    Specification = model.Specification == null ? "" : model.Specification,
                    ModelLine = model.ModelLine == null ? "" : model.ModelLine,
                    Status = model.Status,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = ctx.CurrentTime,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = ctx.CurrentTime,
                    IsLocked = false,
                    LockedBy = "",
                    LockedDate = Convert.ToDateTime("1900-01-01"),
                    MarketModelCode = "",
                    GroupMarketModel = "",
                    ColumnMarketModel = ""
                };
                ctx.MstModels.Add(record);
                msg = "New Refference added...";
            }
            else
            {
                    record.SalesModelDesc = model.SalesModelDesc;
                    record.FakturPolisiDesc = model.FakturPolisiDesc;
                    record.EngineCode = model.EngineCode;
                    record.PpnBmCodeBuy = model.PpnBmCodeSell;
                    record.PpnBmPctBuy = model.PpnBmPctBuy;
                    record.PpnBmCodeSell = model.PpnBmCodeSell;
                    record.PpnBmPctSell = model.PpnBmPctSell;
                    record.PpnBmCodePrincipal = model.PpnBmCodePrincipal;
                    record.PpnBmPctPrincipal = model.PpnBmPctPrincipal;
                    record.Remark = model.Remark == null ? "" : model.Remark;
                    record.BasicModel = model.BasicModel;
                    record.TechnicalModelCode = model.TechnicalModelCode;
                    record.ProductType = model.ProductType;
                    record.TransmissionType = model.TransmissionType;
                    record.IsChassis = model.IsChassis;
                    record.IsCbu = model.IsCbu;
                    record.SMCModelCode = model.SMCModelCode == null ? "" : model.SMCModelCode;
                    record.GroupCode = model.GroupCode == null ? "" : model.GroupCode;
                    record.TypeCode = model.TypeCode == null ? "" : model.TypeCode;
                    record.CylinderCapacity = model.CylinderCapacity == null ? 0 : model.CylinderCapacity;
                    record.fuel = model.fuel == null ? "" : model.fuel;
                    record.ModelPrincipal = model.ModelPrincipal == null ? "" : model.ModelPrincipal;
                    record.Specification = model.Specification == null ? "" : model.Specification;
                    record.ModelLine = model.ModelLine == null ? "" : model.ModelLine;
                    record.Status = model.Status;
                    record.LastUpdateBy = CurrentUser.UserId;
                    record.LastUpdateDate = ctx.CurrentTime;
                    record.IsLocked = false;
                    record.LockedBy = "";
                    record.LockedDate = Convert.ToDateTime("1900-01-01");
                    record.MarketModelCode = "";
                    record.GroupMarketModel = "";
                    record.ColumnMarketModel = "";
                //ctx.MstRefferences.Add(record);
                msg = "Refference updated";
            }

            //record.IsLocked = false;
            //record.LockedBy = CurrentUser.UserId;
            //record.LockedDate = ctx.CurrentTime;

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, message = msg, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Delete(MstModel model)
        {
            var record = ctx.MstModels.Find(CompanyCode, model.SalesModelCode);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.MstModels.Remove(record);
            }

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
