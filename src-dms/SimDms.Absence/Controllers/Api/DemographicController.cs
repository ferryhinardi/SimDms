using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Absence.Controllers.Api
{
    public class DemographicController : BaseController
    {
        public JsonResult CalculateWorkPeriod()
        {
            //var r1 = Request.Params["Range1"];
            //var r2 = Request.Params["Range2"];
            var dealer = Request.Params["CompanyCode"] ?? "";
            var outlet = Request.Params["BranchCode"] ?? BranchCode;
            var position = Request.Params["Position"] ?? "";
            var from = Request.Params["DateFrom"];

            var data1 = ctx.Database.SqlQuery<PieModel>("exec uspfn_CalculateWorkingPeriod @p0, @p1, @p2, @p3, 0, 3, 1", dealer, outlet, position, from);
            var data2 = ctx.Database.SqlQuery<PieModel>("exec uspfn_CalculateWorkingPeriod @p0, @p1, @p2, @p3, 3, 6, 1", dealer, outlet, position, from);
            var data3 = ctx.Database.SqlQuery<PieModel>("exec uspfn_CalculateWorkingPeriod @p0, @p1, @p2, @p3, 6, 12, 1", dealer, outlet, position, from);
            var data4 = ctx.Database.SqlQuery<PieModel>("exec uspfn_CalculateWorkingPeriod @p0, @p1, @p2, @p3, 12, 24, 1", dealer, outlet, position, from);
            var data5 = ctx.Database.SqlQuery<PieModel>("exec uspfn_CalculateWorkingPeriod @p0, @p1, @p2, @p3, 24, 48, 1", dealer, outlet, position, from);
            var data6 = ctx.Database.SqlQuery<PieModel>("exec uspfn_CalculateWorkingPeriod @p0, @p1, @p2, @p3, 48, 60, 1", dealer, outlet, position, from);
            var data7 = ctx.Database.SqlQuery<PieModel>("exec uspfn_CalculateWorkingPeriod @p0, @p1, @p2, @p3, 60, 0, 0", dealer, outlet, position, from);
            var result = data1.Concat(data2).Concat(data3).Concat(data4).Concat(data5).Concat(data6).Concat(data7);

            return Json(new { message = "Success", data = result });
        }

        public JsonResult CalculateAge()
        {
            //var r1 = Request.Params["Range1"];
            //var r2 = Request.Params["Range2"];
            var dealer = Request.Params["CompanyCode"] ?? "";
            var outlet = Request.Params["BranchCode"] ?? BranchCode;
            var position = Request.Params["Position"] ?? "";
            var from = Request.Params["DateFrom"];

            var data1 = ctx.Database.SqlQuery<PieModel>("exec uspfn_CalculateAge @p0, @p1, @p2, @p3, 0, 17", dealer, outlet, position, from);
            var data2 = ctx.Database.SqlQuery<PieModel>("exec uspfn_CalculateAge @p0, @p1, @p2, @p3, 17, 25", dealer, outlet, position, from);
            var data3 = ctx.Database.SqlQuery<PieModel>("exec uspfn_CalculateAge @p0, @p1, @p2, @p3, 25, 12", dealer, outlet, position, from);
            var data4 = ctx.Database.SqlQuery<PieModel>("exec uspfn_CalculateAge @p0, @p1, @p2, @p3, 33, 24", dealer, outlet, position, from);
            var data5 = ctx.Database.SqlQuery<PieModel>("exec uspfn_CalculateAge @p0, @p1, @p2, @p3, 41, 48", dealer, outlet, position, from);
            var data6 = ctx.Database.SqlQuery<PieModel>("exec uspfn_CalculateAge @p0, @p1, @p2, @p3, 49, 999", dealer, outlet, position, from);
            var result = data1.Concat(data2).Concat(data3).Concat(data4).Concat(data5).Concat(data6);

            return Json(new { message = "Success", data = result });

        }

        public JsonResult CalculateEducation()
        {
            //var r1 = Request.Params["Range1"];
            //var r2 = Request.Params["Range2"];
            var dealer = Request.Params["CompanyCode"] ?? "";
            var outlet = Request.Params["BranchCode"] ?? BranchCode;
            var position = Request.Params["Position"] ?? "";
            var from = Request.Params["DateFrom"];

            var data1 = ctx.Database.SqlQuery<PieModel>("exec uspfn_CalculateEducation @p0, @p1, @p2, @p3, @p4", dealer, outlet, position, from, "3, SMP");
            var data2 = ctx.Database.SqlQuery<PieModel>("exec uspfn_CalculateEducation @p0, @p1, @p2, @p3, @p4", dealer, outlet, position, from, "4, SMA, SMK");
            var data3 = ctx.Database.SqlQuery<PieModel>("exec uspfn_CalculateEducation @p0, @p1, @p2, @p3, @p4", dealer, outlet, position, from, "5, 6, 7, D1, D2, D3");
            var data4 = ctx.Database.SqlQuery<PieModel>("exec uspfn_CalculateEducation @p0, @p1, @p2, @p3, @p4", dealer, outlet, position, from, "8, S1");
            var data5 = ctx.Database.SqlQuery<PieModel>("exec uspfn_CalculateEducation @p0, @p1, @p2, @p3, @p4", dealer, outlet, position, from, "9, S2");
            var data6 = ctx.Database.SqlQuery<PieModel>("exec uspfn_CalculateEducation @p0, @p1, @p2, @p3, @p4", dealer, outlet, position, from, "10, S3");
            var result = data1.Concat(data2).Concat(data3).Concat(data4).Concat(data5).Concat(data6);

            return Json(new { message = "Success", data = result });

        }

        public JsonResult CalculateGender()
        {
            //var r1 = Request.Params["Range1"];
            //var r2 = Request.Params["Range2"];
            var dealer = Request.Params["CompanyCode"] ?? "";
            var outlet = Request.Params["BranchCode"] ?? BranchCode;
            var position = Request.Params["Position"] ?? "";
            var from = Request.Params["DateFrom"];

            var data1 = ctx.Database.SqlQuery<PieModel>("exec uspfn_CalculateGender @p0, @p1, @p2, @p3", dealer, outlet, position, from);
            var data2 = ctx.Database.SqlQuery<PieModel>("exec uspfn_CalculateGender @p0, @p1, @p2, @p3", dealer, outlet, position, from);
            var result = data1;

            return Json(new { message = "Success", data = result });

        }

        public class PieModel
        {
            public int value1 { get; set; }
            public int value2 { get; set; }
        }
    }
}