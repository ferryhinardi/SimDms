using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
using SimDms.DataWarehouse.Models;
using System.Web.Script.Serialization;
using System.Data;
using SimDms.DataWarehouse.Helpers;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class URDashboardController : BaseController
    {
        public JsonResult YearUR()
        {
            var data = ctx.Database.SqlQuery<svMstUnitRevenueTarget>("exec [uspfn_svListYearUnitRevenue]").Select(x => new { text = x.PeriodYear, value = x.PeriodYear }).ToList();
            return Json(data);
        }

        public JsonResult ChartUnitRevenue()
        {
            var PeriodYear = Request["PeriodYear"];
            var PeriodMonthF = Request["StartMonth"];
            var PeriodMonthT = Request["EndMonth"];
            var DashType = Request["DashType"];
            var Sort = Request["Sort"];

            decimal Year = Convert.ToInt32(PeriodYear != "" ? PeriodYear : "0");
            decimal MonthF = Convert.ToInt32(PeriodMonthF != "" ? PeriodMonthF : "0");
            decimal MonthT = Convert.ToInt32(PeriodMonthT != "" ? PeriodMonthT : "0");

            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<UnitRevenue>(string.Format("EXEC uspfn_svGetUnitRevenueChart {0}, {1}, {2}", PeriodYear, MonthF, MonthT)).ToList();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;
            return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReloadUnitTopFive()
        {
            var PeriodYear = Request["PeriodYear"];
            var PeriodMonthF = Request["PeriodMonthF"];
            var PeriodMonthT = Request["PeriodMonthT"];
            var DashType = Request["DashType"];
            var Sort = Request["Sort"];
            var BodyRepair = Request["BodyRepair"];

            decimal Year = Convert.ToInt32(PeriodYear != "" ? PeriodYear : "0");
            decimal MonthF = Convert.ToInt32(PeriodMonthF != "" ? PeriodMonthF : "0");
            decimal MonthT = Convert.ToInt32(PeriodMonthT != "" ? PeriodMonthT : "0");

            try
            {
                string qry = string.Format("exec uspfn_ReloadUnitTopFive '{0}', {1}, {2}, {3}, '{4}', '{5}'", DashType, MonthF, MonthT, Year, Sort, BodyRepair);
                //if (DashType == "Invoice")
                //{
                //    qry += "select top 5 OutletAbbreviation as text, CONVERT(decimal, count(chassis)) as value FROM                           " +
                //    "(                                                                                                                      " +
                //    "	select distinct companycode, branchcode, CAST(InvoiceDate as date) as invdate, (ChassisCode + CONVERT(varchar(10), ChassisNo)) AS chassis    " +
                //    "	from svTrnInvoice                                                                                                                 " +
                //    "	where 1 = 1                                                                                                                       " +
                //    "		and YEAR(InvoiceDate) = @p2                                                                                                   " +
                //    "		and MONTH(InvoiceDate) BETWEEN @p0 AND @p1                                                                                    " +
                //    ") a                                                                                                                                  " +
                //    "join gnMstDealerOutletMapping s                                                                                                      " +
                //    "on a.companycode = s.DealerCode                                                                                                      " +
                //    "	and a.branchcode = s.OutletCode                                                                                                   " +
                //    "group by companycode, branchcode, OutletAbbreviation                                                                                         " +
                //    "order by value " + (Sort == "" ? "desc" : Sort) + "                                                                                  " +
                //    "";
                //}
                //else
                //{
                //    qry +=
                //        "select top 5 OutletAbbreviation as text, CONVERT(decimal, count(chassis)) as value FROM                                                                        " +
                //        "(                                                                                                                                                              " +
                //        "	select distinct companycode, branchcode, (ChassisCode + CONVERT(varchar(10), ChassisNo)) AS chassis                                                         " +
                //        " , CASE WHEN (jobOrderClosed is null OR JobOrderClosed = '1990-01-01') AND ServiceStatus > 4                                                                   " +
                //        "       THEN CAST(JobOrderDate as date) ELSE CAST(JobOrderClosed as date) END as jobdate                                                                        " +
                //        "	from svTrnService                                                                                                                                           " +
                //        "	where 1 = 1                                                                                                                                                 " +
                //        "		and YEAR(CASE WHEN (jobOrderClosed is null OR JobOrderClosed = '1990-01-01') AND ServiceStatus > 4 THEN JobOrderDate ELSE JobOrderClosed END) = @p2     " +
                //        "		and MONTH(CASE WHEN (jobOrderClosed is null OR JobOrderClosed = '1990-01-01') AND ServiceStatus > 4 THEN JobOrderDate ELSE JobOrderClosed END)          " +
                //        "		    BETWEEN @p0 AND @p1                                                                                                                                 " +
                //        ") a                                                                                                                                                            " +
                //        "join gnMstDealerOutletMapping s                                                                                                                                " +
                //        "on a.companycode = s.DealerCode                                                                                                                                " +
                //        "	and a.branchcode = s.OutletCode                                                                                                                             " +
                //        "group by companycode, branchcode, OutletAbbreviation                                                                                                           " +
                //        "order by value " + (Sort == "" ? "desc" : Sort) + "                                                                                                            " +
                //        "";
                //}
                ctx.Database.CommandTimeout = 3600;
                var data = ctx.Database.SqlQuery<UnitIntakeFiveModel>(qry);

                return Json(new { message = "Success", data = data });
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error", data = ex.Message });
            }

        }

        public JsonResult ReloadRevenueTopFive()
        {
            var PeriodYear = Request["PeriodYear"];
            var PeriodMonthF = Request["PeriodMonthF"];
            var PeriodMonthT = Request["PeriodMonthT"];
            var DashType = Request["DashType"];
            var Sort = Request["Sort"];
            var BodyRepair = Request["BodyRepair"];

            decimal Year = Convert.ToInt32(PeriodYear != "" ? PeriodYear : "0");
            decimal MonthF = Convert.ToInt32(PeriodMonthF != "" ? PeriodMonthF : "0");
            decimal MonthT = Convert.ToInt32(PeriodMonthT != "" ? PeriodMonthT : "0");

            try
            {
                string qry = string.Format("exec uspfn_ReloadRevenueTopFive '{0}', {1}, {2}, {3}, '{4}', '{5}'", DashType, MonthF, MonthT, Year, Sort, BodyRepair);
                //if (DashType == "Invoice")
                //{
                //    qry += "" +
                //        "select top 5 OutletAbbreviation as text, SUM(TotalDppAmt) as value FROM                                    " +
                //        "(                                                                                                  " +
                //        "	select distinct companycode, branchcode, InvoiceDate, TotalDppAmt      " +
                //        ", (ChassisCode + CONVERT(varchar(10), ChassisNo)) AS chassis                                       " +
                //        "	from svTrnInvoice                                                                               " +
                //        "	where 1 = 1                                                                                     " +
                //        "		and YEAR(InvoiceDate) = @p2                                                                 " +
                //        "		and MONTH(InvoiceDate) BETWEEN @p0 AND @p1                                                  " +
                //        ") a                                                                                                " +
                //        "join gnMstDealerOutletMapping s                                                                    " +
                //        "on a.companycode = s.DealerCode                                                                    " +
                //        "	and a.branchcode = s.OutletCode                                                                 " +
                //        "group by companycode, branchcode, OutletAbbreviation                                                       " +
                //        "order by value " + (Sort == "" ? "desc" : Sort) +
                //        "";

                //}
                //else
                //{
                //    qry +=
                //        "select top 5 OutletAbbreviation as text, SUM(TotalDppAmount) as value FROM                                                                                   " +
                //        "(                                                                                                                                                            " +
                //        "	select distinct companycode, branchcode, (ChassisCode + CONVERT(varchar(10), ChassisNo)) AS chassis, TotalDppAmount                                       " +
                //        " , CASE WHEN (jobOrderClosed is null OR JobOrderClosed = '1990-01-01') AND ServiceStatus > 4 THEN JobOrderDate ELSE JobOrderClosed END as jobdate            " +
                //        "	from svTrnService                                                                                                                                         " +
                //        "	where 1 = 1                                                                                                                                               " +
                //        "		and YEAR(CASE WHEN (jobOrderClosed is null OR JobOrderClosed = '1990-01-01') AND ServiceStatus > 4 THEN JobOrderDate ELSE JobOrderClosed END) = @p2   " +
                //        "		and MONTH(CASE WHEN (jobOrderClosed is null OR JobOrderClosed = '1990-01-01') AND ServiceStatus > 4 THEN JobOrderDate ELSE JobOrderClosed END)        " +
                //        "		    BETWEEN @p0 AND @p1                                                                                                                               " +
                //        ") a                                                                                                                                                          " +
                //        "join gnMstDealerOutletMapping s                                                                                                                              " +
                //        "on a.companycode = s.DealerCode                                                                                                                              " +
                //        "	and a.branchcode = s.OutletCode                                                                                                                           " +
                //        "group by companycode, branchcode, OutletAbbreviation                                                                                                         " +
                //        "order by value " + (Sort == "" ? "desc" : Sort) + "                                                                                                          " +
                //        "";
                //}
                ctx.Database.CommandTimeout = 3600;
                var data = ctx.Database.SqlQuery<UnitIntakeFiveModel>(qry);

                return Json(new { message = "Success", data = data });
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error", data = ex.Message });
            }

        }

        public JsonResult ReloadUnitIntake()
        {
            var PeriodYear = Request["PeriodYear"];
            var PeriodMonthF = Request["PeriodMonthF"];
            var PeriodMonthT = Request["PeriodMonthT"];
            var DashType = Request["DashType"];
            var Sort = Request["Sort"];

            decimal Year = Convert.ToInt32(PeriodYear != "" ? PeriodYear : "0");
            decimal MonthF = Convert.ToInt32(PeriodMonthF != "" ? PeriodMonthF : "0");
            decimal MonthT = Convert.ToInt32(PeriodMonthT != "" ? PeriodMonthT : "0");

            var SIseries = new decimal[12];
            var SAseries = new decimal[12];
            var SFseries = new decimal[12];
            var VPseries = new decimal[12];
            var SQseries = new decimal[12];

            try
            {
                string qry = "select top 5 (select top 1 AreaDealer from GroupArea where GroupNo = s.GroupNo) as text, SUM(";

                while (MonthF < MonthT)
                {
                    qry += "Target" + ("0" + MonthF).Substring(("0" + MonthF).Length - 2) + " + ";
                    MonthF++;
                }
                qry += "Target" + ("0" + MonthT).Substring(("0" + MonthT).Length - 2);
                qry += ") as value from [dbo].[svMstUnitRevenueTarget] s" +
                       " where PeriodYear = @p0" +
                       " and TargetFlag = @p1" + 
                       " group by groupno" +
                       " order by value " + (Sort == "" ? "desc" : Sort);
                ctx.Database.CommandTimeout = 3600;
                var data = ctx.Database.SqlQuery<UnitIntakeFiveModel>(String.Format(qry), Year, DashType);

                return Json(new { message = "Success", data = data });
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error", data = ex.Message });
            }
        }

        public JsonResult ReloadKeyIndicatorUnitIntake()
        {
            var PeriodYear = Request["PeriodYear"];
            var PeriodMonthF = Request["PeriodMonthF"];
            var PeriodMonthT = Request["PeriodMonthT"];

            decimal Year = Convert.ToInt32(PeriodYear != "" ? PeriodYear : "0");
            decimal MonthF = Convert.ToInt32(PeriodMonthF != "" ? PeriodMonthF : "0");
            decimal MonthT = Convert.ToInt32(PeriodMonthT != "" ? PeriodMonthT : "0");

            try
            {
                ctx.Database.CommandTimeout = 3600;
                var data = ctx.Database.SqlQuery<decimal>(string.Format("EXEC uspfn_svGetKeyIndicatorUnitIntake {0}, {1}, {2}", Year, MonthF, MonthT)).ToList();
                return Json(new { message = "Success", data = data });
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error", data = ex.Message });
            }
        }

        public JsonResult ReloadKeyIndicatorServiceCoverageCombine()
        {
            var PeriodYear = Request["PeriodYear"];
            var PeriodMonthF = Request["PeriodMonthF"];
            var PeriodMonthT = Request["PeriodMonthT"];

            decimal Year = Convert.ToInt32(PeriodYear != "" ? PeriodYear : "0");
            decimal MonthF = Convert.ToInt32(PeriodMonthF != "" ? PeriodMonthF : "0");
            decimal MonthT = Convert.ToInt32(PeriodMonthT != "" ? PeriodMonthT : "0");

            try
            {
                ctx.Database.CommandTimeout = 3600;
                var data = ctx.Database.SqlQuery<decimal>(string.Format("EXEC uspfn_svGetKeyIndicatorServiceCoverageCombine {0}, {1}, {2}", Year, MonthF, MonthT)).ToList();
                return Json(new { message = "Success", data = data });
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error", data = ex.Message });
            }
        }

        public JsonResult ReloadKeyIndicatorPassanger()
        {
            var PeriodYear = Request["PeriodYear"];
            var PeriodMonthF = Request["PeriodMonthF"];
            var PeriodMonthT = Request["PeriodMonthT"];

            decimal Year = Convert.ToInt32(PeriodYear != "" ? PeriodYear : "0");
            decimal MonthF = Convert.ToInt32(PeriodMonthF != "" ? PeriodMonthF : "0");
            decimal MonthT = Convert.ToInt32(PeriodMonthT != "" ? PeriodMonthT : "0");

            try
            {
                ctx.Database.CommandTimeout = 3600;
                var data = ctx.Database.SqlQuery<decimal>(string.Format("EXEC uspfn_svGetKeyIndicatorPassanger {0}, {1}, {2}", Year, MonthF, MonthT)).ToList();
                return Json(new { message = "Success", data = data });
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error", data = ex.Message });
            }
        }

        public JsonResult ReloadKeyIndicatorCommercial()
        {
            var PeriodYear = Request["PeriodYear"];
            var PeriodMonthF = Request["PeriodMonthF"];
            var PeriodMonthT = Request["PeriodMonthT"];

            decimal Year = Convert.ToInt32(PeriodYear != "" ? PeriodYear : "0");
            decimal MonthF = Convert.ToInt32(PeriodMonthF != "" ? PeriodMonthF : "0");
            decimal MonthT = Convert.ToInt32(PeriodMonthT != "" ? PeriodMonthT : "0");

            try
            {
                ctx.Database.CommandTimeout = 3600;
                var data = ctx.Database.SqlQuery<decimal>(string.Format("EXEC uspfn_svGetKeyIndicatorCommercial {0}, {1}, {2}", Year, MonthF, MonthT)).ToList();
                return Json(new { message = "Success", data = data });
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error", data = ex.Message });
            }
        }

        public JsonResult GetUnitIntakeNational()
        {
            var PeriodYear = Request["PeriodYear"];
            var PeriodMonthF = Request["PeriodMonthF"];
            var PeriodMonthT = Request["PeriodMonthT"];
            decimal Year = Convert.ToInt32(PeriodYear != "" ? PeriodYear : "0");
            decimal MonthF = Convert.ToInt32(PeriodMonthF != "" ? PeriodMonthF : "0");
            decimal MonthT = Convert.ToInt32(PeriodMonthT != "" ? PeriodMonthT : "0");
            try
            {
                ctx.Database.CommandTimeout = 3600;
                var data = ctx.Database.SqlQuery<UnitRevenue>(string.Format("EXEC uspfn_svGetUnitIntakeNational {0}, {1}, {2}", Year, MonthF, MonthT)).ToList();
                return Json(new { message = "Success", data = data });
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error", data = ex.Message });
            }
        }

        public JsonResult GetRevenueNational()
        {
            var PeriodYear = Request["PeriodYear"];
            var PeriodMonthF = Request["PeriodMonthF"];
            var PeriodMonthT = Request["PeriodMonthT"];
            decimal Year = Convert.ToInt32(PeriodYear != "" ? PeriodYear : "0");
            decimal MonthF = Convert.ToInt32(PeriodMonthF != "" ? PeriodMonthF : "0");
            decimal MonthT = Convert.ToInt32(PeriodMonthT != "" ? PeriodMonthT : "0");
            try
            {
                ctx.Database.CommandTimeout = 3600;
                var data = ctx.Database.SqlQuery<UnitRevenue>(string.Format("EXEC uspfn_svGetRevenueNational {0}, {1}, {2}", Year, MonthF, MonthT)).ToList();
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
            decimal Year = Convert.ToInt32(PeriodYear != "" ? PeriodYear : "0");
            try
            {
                ctx.Database.CommandTimeout = 3600;
                var data = ctx.Database.SqlQuery<UnitRevenue>(string.Format("EXEC uspfn_svGetTargetNational {0}", Year)).ToList();
                var dataUnit = data.Where(x => x.TargetFlag == "U");
                var dataRevenue = data.Where(x => x.TargetFlag == "R");
                return Json(new { message = "Success", dataUnit = dataUnit, dataRevenue = dataRevenue });
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error", data = ex.Message });
            }
        }
    }
}
