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
    public class CSIScoreController : BaseController
    {
        public JsonResult ReloadCSIScore()
        {
            var PeriodYear = Request["PeriodYear"];
            var PeriodMonth = Request["PeriodMonth"];

            decimal Year = Convert.ToInt32(PeriodYear != "" ? PeriodYear : "0");
            decimal Month = Convert.ToInt32(PeriodMonth != "" ? PeriodMonth : "0");
            try
            {
                string qry = String.Format("SELECT c.*," +
                    " (SELECT TOP 1 ServiceName FROM svMstDealerAndOutletServiceMapping WHERE ServiceCode = c.ServiceCode) as ServiceName" +
                    " FROM svCustomerSatisfactionScore c" + 
                    " WHERE c.PeriodYear = @p0 AND c.PeriodMonth = @p1");
                
                var data = ctx.Database.SqlQuery<CSIScoreGridModel>(qry, Year, Month);

                return Json(new { message = "Success", data = data });
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error", data = ex.InnerException });
            }
        }

        public JsonResult ReloadScorePerformance()
        {
            var PeriodYear = Request["PeriodYear"];
            var PeriodMonthF = Request["PeriodMonthF"];
            var PeriodMonthT = Request["PeriodMonthT"];

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
                string qry = String.Format("select PeriodMonth" +
                                    ", isnull(AVG(ServiceInitiation)	, 0)	as ServiceInitiation           " +
                                    ", isnull(AVG(ServiceAdvisor)	    , 0)	as ServiceAdvisor              " +
                                    ", isnull(AVG(ServiceFaciltiy)	    , 0)	as ServiceFaciltiy             " +
                                    ", isnull(AVG(VehiclePickup)		, 0)	as VehiclePickup               " +
                                    ", isnull(AVG(ServiceQuality)	    , 0)	as ServiceQuality              " +
                                    "FROM svCustomerSatisfactionScore c                               " +
                                    "WHERE PeriodYear = @p0                                           " +
                                    "AND PeriodMonth >= @p1 AND PeriodMonth <= @p2                    " +
                                    "GROUP BY PeriodMonth                                             ");

                var data = ctx.Database.SqlQuery<CSIScoreChartModel>(qry, Year, MonthF, MonthT);

                for (int i = 0; i < 12; i++)
                {
                    if (data.Where(x=>x.PeriodMonth == i + 1).Count() > 0)
                    {
                        var score = data.FirstOrDefault(x => x.PeriodMonth == i + 1);
                        SIseries[i] = (decimal)score.ServiceInitiation;
                        SAseries[i] = (decimal)score.ServiceAdvisor;
                        SFseries[i] = (decimal)score.ServiceFaciltiy;
                        VPseries[i] = (decimal)score.VehiclePickup;
                        SQseries[i] = (decimal)score.ServiceQuality;
                    }
                }

                return Json(new
                {
                    SIseries,
                    SAseries,
                    SFseries,
                    VPseries,
                    SQseries
                });
                //return Json(new { message = "Success", data = data });
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error", data = ex.Message });
            }

        }

        public JsonResult ReloadCSIScoreTopFive()
        {
            var PeriodYear = Request["PeriodYear"];
            var PeriodMonthF = Request["PeriodMonthF"];
            var PeriodMonthT = Request["PeriodMonthT"];
            var Sort = Request["Sort"];

            decimal Year = Convert.ToInt32(PeriodYear != "" ? PeriodYear : "0");
            decimal MonthF = Convert.ToInt32(PeriodMonthF != "" ? PeriodMonthF : "0");
            decimal MonthT = Convert.ToInt32(PeriodMonthT != "" ? PeriodMonthT : "0");
            
            try
            {
                string qry = string.Format("exec uspfn_ReloadCSIScoreTopFive {0}, {1}, {2}, '{3}'", MonthF, MonthT, PeriodYear, Sort);
                //string qry = String.Format("SELECT Top 5 SUM(Score) as value," +
                //    " (SELECT TOP 1 ServiceAbbreviation FROM svMstDealerAndOutletServiceMapping WHERE ServiceCode = c.ServiceCode) as text" +
                //    " FROM svCustomerSatisfactionScore c" +
                //    " WHERE c.PeriodYear = @p0 AND c.PeriodMonth > @p1 AND c.PeriodMonth < @p2" +
                //    " group by ServiceCode" +
                //    " order by value " + (Sort == "" ? "desc" : Sort));

                var data = ctx.Database.SqlQuery<UnitIntakeFiveModel>(qry);

                return Json(new { message = "Success", data = data });
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error", data = ex.Message });
            }
        }
    }

}