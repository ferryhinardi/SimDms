using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
using SimDms.DataWarehouse.Models;
using System.Web.Script.Serialization;
using System.Data;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class UnitRevenueTargetController : BaseController
    {
        public JsonResult LoadTable(string PeriodYear)
        {
            decimal PeriodYearParam;
            if (!Decimal.TryParse(PeriodYear, out PeriodYearParam))
                PeriodYearParam = 0;

            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<svMstUnitRevenueTarget>(string.Format("EXEC uspfn_svGetUnitRevenueTarget {0}", PeriodYearParam)).ToList();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            var dataUnit = data.Where(x => x.TargetFlag == "U").ToList();
            var dataRevenue = data.Where(x => x.TargetFlag == "R").ToList();
            return Json(new { success = true, dataUnit = dataUnit, dataRevenue = dataRevenue }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Save(string PeriodYear, string TargetFlag, string Data)
        {
            var items = Data;
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<svMstUnitRevenueTarget> listTarget = ser.Deserialize<List<svMstUnitRevenueTarget>>(Data);

            decimal PeriodYearParam;

            if (!Decimal.TryParse(PeriodYear, out PeriodYearParam))
                PeriodYearParam = 0;

            foreach (var item in listTarget)
            {
                ctx.Database.ExecuteSqlCommand(string.Format("EXEC uspfn_svInsertUnitRevenueTarget {0}, {1}, '{2}', {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, '{15}'", item.GroupNo, PeriodYearParam, item.TargetFlag, item.Target01, item.Target02, item.Target03, item.Target04, item.Target05, item.Target06, item.Target07, item.Target08, item.Target09, item.Target10, item.Target11, item.Target12, CurrentUser.UserId));
            }
            return Json(new { success = true });
        }
    }
}