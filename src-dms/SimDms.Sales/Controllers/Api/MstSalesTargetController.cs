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
    public class MstSalesTargetController : BaseController
    {
        public JsonResult SalesTarget(SalesTargetBrowse model)
        {
            var query = string.Format(@"SELECT a.DealerCode, a.OutletCode, a.Year, a.Month,d.EmployeeName SalesmanName, a.MarketModel, a.TargetUnit, a.isActive, a.CreatedBy, a.CreatedDate,b.CompanyName, c.BranchName, c.IsBranch
                                FROM omMstSalesTarget a 
                                left join gnMstOrganizationHdr b on b.CompanyCode = a.DealerCode
                                left join gnMstOrganizationDtl c on c.CompanyCode = a.DealerCode and c.BranchCode = a.OutletCode
                                left join GnMstEmployee d on d.CompanyCode = a.DealerCode and  a.SalesmanID = d.EmployeeId
             WHERE 
             a.DealerCode = '{0}' AND a.OutletCode = '{1}' AND a.Year = '{2}' AND a.Month = '{3}'
             order by a.OutletCode
            ", model.DealerCode, model.OutletCode, model.Year, model.Month);

            var sqlstr = ctx.Database.SqlQuery<SalesTargetBrowse>(query).AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult SalesmanID(omMstSalesTarget model)
        {
            var record = ctx.Employees.Find(CompanyCode, BranchCode, model.SalesmanID);

            return Json(new { success = record != null, data = record });
        }

        public JsonResult Save(omMstSalesTarget model)
        {
            string msg = "";
            var record = ctx.omMstSalesTarget.Find(model.DealerCode, model.OutletCode, model.Year, model.Month, model.SalesmanID, model.MarketModel);

            if (record == null)
            {
                record = new omMstSalesTarget
                {
                    DealerCode = model.DealerCode,
                    OutletCode = model.OutletCode,
                    Month = model.Month,
                    Year = model.Year,
                    SalesmanID = model.SalesmanID,
                    MarketModel = model.MarketModel,
                    TargetUnit = model.TargetUnit,
                    isActive = model.isActive,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = ctx.CurrentTime
                };
                ctx.omMstSalesTarget.Add(record);
                msg = "New Others Inventory added...";
            }
            else
            {
                record.TargetUnit = model.TargetUnit;
                record.isActive = model.isActive;
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = ctx.CurrentTime;
                msg = "Others Inventory updated";
            }



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

        public JsonResult Delete(omMstSalesTarget model)
        {
            var record = ctx.omMstSalesTarget.Find(model.DealerCode, model.OutletCode, model.Year, model.Month, model.SalesmanID, model.MarketModel);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.omMstSalesTarget.Remove(record);
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
