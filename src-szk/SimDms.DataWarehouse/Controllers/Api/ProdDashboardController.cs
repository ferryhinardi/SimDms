using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using GeLang;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using SimDms.DataWarehouse.Models;
using System.Data.SqlClient;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class ProdDashboardController : BaseController
    {
        public JsonResult GetStallNational()
        {
            var PeriodYear = Request["PeriodYear"];
            var PeriodMonthF = Request["PeriodMonthF"];
            var PeriodMonthT = Request["PeriodMonthT"];
            decimal Year = Convert.ToInt32(PeriodYear != "" ? PeriodYear : "0");
            decimal MonthF = Convert.ToInt32(PeriodMonthF != "" ? PeriodMonthF : "0");
            decimal MonthT = Convert.ToInt32(PeriodMonthT != "" ? PeriodMonthT : "0");
            try
            {
                var data = ctx.Database.SqlQuery<UnitRevenue>(string.Format("EXEC uspfn_svGetStallNational {0}, {1}, {2}", Year, MonthF, MonthT)).ToList();
                return Json(new { message = "Success", data = data });
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error", data = ex.Message });
            }
        }

        public JsonResult GetTechnicianNational()
        {
            var PeriodYear = Request["PeriodYear"];
            var PeriodMonthF = Request["PeriodMonthF"];
            var PeriodMonthT = Request["PeriodMonthT"];
            decimal Year = Convert.ToInt32(PeriodYear != "" ? PeriodYear : "0");
            decimal MonthF = Convert.ToInt32(PeriodMonthF != "" ? PeriodMonthF : "0");
            decimal MonthT = Convert.ToInt32(PeriodMonthT != "" ? PeriodMonthT : "0");
            try
            {
                var data = ctx.Database.SqlQuery<UnitRevenue>(string.Format("EXEC uspfn_svGetStallNational {0}, {1}, {2}", Year, MonthF, MonthT)).ToList();
                return Json(new { message = "Success", data = data });
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error", data = ex.Message });
            }
        }

        public JsonResult GetTargetNational()
        {
            var PeriodYear = Request["PeriodYear"];
            var PeriodMonthF = Request["PeriodMonthF"];
            var PeriodMonthT = Request["PeriodMonthT"];
            decimal Year = Convert.ToInt32(PeriodYear != "" ? PeriodYear : "0");
            decimal MonthF = Convert.ToInt32(PeriodMonthF != "" ? PeriodMonthF : "0");
            decimal MonthT = Convert.ToInt32(PeriodMonthT != "" ? PeriodMonthT : "0");
            try
            {
                var data = ctx.Database.SqlQuery<UnitRevenue>(string.Format("EXEC uspfn_svGetStallNational {0}, {1}, {2}", Year, MonthF, MonthT)).ToList();
                return Json(new { message = "Success", data = data });
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error", data = ex.Message });
            }
        }

    }
}