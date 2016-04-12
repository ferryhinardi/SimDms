using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;
using SimDms.Sales.Models;

namespace SimDms.Sales.Controllers.Api
{
    public class FakturPolisiController : BaseController
    {
        public JsonResult getDetail(OmTrSalesFakturPolisi model)
        {
            var record = ctx.OmTrSalesFakturPolisi.Find(CompanyCode, BranchCode, model.FakturPolisiNo);
            return Json(new { data = record, status = getStringStatus(record.Status) });
        }

        public JsonResult Save(OmTrSalesFakturPolisi model)
        {
            var record = ctx.OmTrSalesFakturPolisi.Find(CompanyCode, BranchCode, model.FakturPolisiNo);
            if (record == null)
            {
                record = new OmTrSalesFakturPolisi()
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    FakturPolisiNo = model.FakturPolisiNo.ToUpper(),
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };
                ctx.OmTrSalesFakturPolisi.Add(record);
            }

            var query = string.Format(@"select a.chassisCode, a.ChassisNo, a.SalesModelCode, a.SalesModelYear, a.ColourCode, a.EngineCode, a.EngineNo,
                                        c.RefferenceDONo, c.RefferenceSJNo, isnull(e.ReqNo,'') ReqNo
                                        from omMstVehicle a 
                                        inner join omTrPurchaseBPUDetail b on  a.companyCode = b.companyCode and 
                                    	    a.chassisCode = b.chassisCode and a. chassisNo = b.chassisNo
                                    	inner join omTrPurchaseBPU c on b.companyCode = c.companyCode
                                    	    and b.branchCode = c.branchCode and b.BPUNo = c.BPUNo
                                    	left join omTrSalesReqDetail e on b.companyCode = e.companyCode and
                                    	    b.chassisCode = e.chassisCode and
                                    	    b.chassisNo = e.chassisNo
                                        where a.companyCode = '{0}'
                                    	and a.chassisCode = '{1}' and a.ChassisNo = '{2}'", CompanyCode, model.ChassisCode, model.ChassisNo);

            var detailVehicle = ctx.Database.SqlQuery<DetailVehicle>(query).FirstOrDefault();

            if (detailVehicle != null)
            {

                record.SalesModelCode = detailVehicle.SalesModelCode;
                record.SalesModelYear = detailVehicle.SalesModelYear;
                record.ColourCode = detailVehicle.ColourCode;
                record.EngineCode = detailVehicle.EngineCode;
                record.EngineNo = detailVehicle.EngineNo;
                record.SJImniNo = detailVehicle.RefferenceSJNo;
                record.DOImniNo = detailVehicle.RefferenceDONo;
                record.ReqNo = detailVehicle.ReqNo;
            }

                record.FakturPolisiDate = model.FakturPolisiDate;
                record.IsManual = true;

                record.ChassisCode = model.ChassisCode;
                record.ChassisNo = model.ChassisNo;

                record.IsBlanko = model.IsBlanko;

                record.Status = "0";
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = DateTime.Now;

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, message = "Data berhasil disimpan!!!" ,status = getStringStatus(record.Status)});
            }
            catch(Exception e)
            {
                return Json(new { success = false, message = "Data gagal disimpan!!!" });

            }
        }

        public JsonResult Delete(OmTrSalesFakturPolisi model)
        {
            var record = ctx.OmTrSalesFakturPolisi.Find(CompanyCode, BranchCode, model.FakturPolisiNo);
            if(record != null)
            {
                ctx.OmTrSalesFakturPolisi.Remove(record);
            }
            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, message = "Data berhasil dihapus!!!" });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Data gagal dihapus!!!", err = e.Message });

            }
        }

        public JsonResult Approve(OmTrSalesFakturPolisi model)
        {
            using (var tran = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    var msg = "";
                    var record = ctx.OmTrSalesFakturPolisi.Find(CompanyCode, BranchCode, model.FakturPolisiNo);
                    var perFktrPol = ctx.omTrSalesReqDetail.Where(a => a.CompanyCode == CompanyCode && a.ChassisCode == model.ChassisCode && a.ChassisNo == model.ChassisNo).FirstOrDefault();

                    record.LastUpdateBy = CurrentUser.UserId;
                    record.LastUpdateDate = DateTime.Now;
                    record.FakturPolisiDate = perFktrPol.FakturPolisiDate;
                    record.Status = "2";

                    bool success = ctx.SaveChanges() >= 0;
                    if (success)
                    {
                        perFktrPol.FakturPolisiNo = record.FakturPolisiNo;
                        ctx.SaveChanges();
                        tran.Commit();

                        msg = string.Format(ctx.SysMsgs.Find("5038").MessageCaption, "Approve Faktur Polisi", "");
                        return Json(new { success = success, message = msg, status = getStringStatus(record.Status) });
                    }
                    else
                    {
                        tran.Rollback();
                        msg = string.Format(ctx.SysMsgs.Find("5039").MessageCaption, "Approve Faktur Polisi", "");
                        return Json(new { success = success, message = msg });
                    }
                }
                catch (Exception ex)
                {
                    tran.Rollback();

                    //Helpers.GetOverLengthInputCollections( "omHstInquirySales");
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }
    }
}