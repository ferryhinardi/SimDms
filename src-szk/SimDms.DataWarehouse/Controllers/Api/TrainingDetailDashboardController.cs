using OfficeOpenXml;
using OfficeOpenXml.Style;
using SimDms.DataWarehouse.Helpers;
using EP = SimDms.DataWarehouse.Helpers.EPPlusHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Drawing;
using SimDms.DataWarehouse.Models;
using GeLang;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class TrainingDetailDashboardController : BaseController
    {
        public JsonResult QueryTrainingDetail(string area, string comp, string outl, string post, string tprg, string flt1, string flt2)
        {            
            var end = DateTime.Now.ToString("MM/dd/yyyy");
            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            //var data = ctx.Database.SqlQuery<SfmTrainingDetail>("exec uspfn_TrainingDetailDashBoard @AreaCode=@p0, @DealerCode=@p1, @End=@p2,@OutletCode=@p3,@Position=@p4,@TrainingCode=@p5", area, comp, end, outl, post, tprg).AsQueryable();
            var data = ctx.Database.SqlQuery<SfmTrainingDetail>("exec usprpt_mpTrainingDetail @areaGroup=@p0, @DealerCode=@p1,@OutletCode=@p2,@Position=@p3,@TrainingCode=@p4,@FilterOutlet=@p5,@FilterCol=@p6",area, comp, outl, post, tprg, flt1, flt2).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult QueryTrainingDetailNew(string GroupArea, string CompanyCode, string outl, string post, string tprg, string flt1, string flt2)
        {
            var end = DateTime.Now.ToString("MM/dd/yyyy");
            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<SfmTrainingDetail>("exec usprpt_mpTrainingDetailNew @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7",
                GroupArea, ParamDealerCode, outl, post, tprg, flt1, flt2, ParamGroupNoNew).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult QueryTrainingDetailData(string area, string comp, string outl, string post, string tprg, string flt1, string flt2)
        {
            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<TrainingDetailData>("exec usprpt_mpTrainingDetail @areaGroup=@p0, @DealerCode=@p1, @OutletCode=@p2,@Position=@p3,@TrainingCode=@p4,@FilterOutlet=@p5,@FilterCol=@p6",area, comp, outl, post, tprg, flt1, flt2).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult QueryTrainingDetailDataNew(string GroupArea, string CompanyCode, string outl, string post, string tprg, string flt1, string flt2)
        {
            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<TrainingDetailData>("exec usprpt_mpTrainingDetailNew @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7", 
                GroupArea, ParamDealerCode, outl, post, tprg, flt1, flt2, ParamGroupNoNew).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportTrainingDetail(string area, string comp, string outl, string post, string tprg, string flt1, string flt2)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "usprpt_mpTrainingDetail";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@areaGroup", area);
            cmd.Parameters.AddWithValue("@DealerCode", comp);
            cmd.Parameters.AddWithValue("@OutletCode", outl);
            cmd.Parameters.AddWithValue("@Position", post);
            cmd.Parameters.AddWithValue("@TrainingCode", tprg);
            cmd.Parameters.AddWithValue("@FilterOutlet", flt1);
            cmd.Parameters.AddWithValue("@FilterCol",flt2);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            var header = new List<List<dynamic>>() { };
            header.Add(new List<dynamic> { "Dealer : ", (comp == "") ? "ALL" : ctx.DealerInfos.Find(new string[] { comp }).DealerCode +" - "+ ctx.DealerInfos.Find(new string[] { comp }).DealerName });

            return GenerateReportXls(dt, "Training Detail", "TrainingDetail", header);
        }
        
        public JsonResult ExportTrainingDetailNew(string GroupArea, string CompanyCode, string outl, string post, string tprg, string flt1, string flt2)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "usprpt_mpTrainingDetailNew";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@areaGroup", GroupArea);
            cmd.Parameters.AddWithValue("@DealerCode", ParamDealerCode);
            cmd.Parameters.AddWithValue("@OutletCode", outl);
            cmd.Parameters.AddWithValue("@Position", post);
            cmd.Parameters.AddWithValue("@TrainingCode", tprg);
            cmd.Parameters.AddWithValue("@FilterOutlet", flt1);
            cmd.Parameters.AddWithValue("@FilterCol", flt2);
            cmd.Parameters.AddWithValue("@GroupNoNew", ParamGroupNoNew);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            var header = new List<List<dynamic>>() { };
            header.Add(new List<dynamic> { "Dealer : ", (ParamDealerCode == "") ? "ALL" : ctx.DealerInfos.Find(new string[] { ParamDealerCode }).DealerCode + " - " + ctx.DealerInfos.Find(new string[] { ParamDealerCode }).DealerName });

            return GenerateReportXls(dt, "Training Detail", "TrainingDetail", header);
        }
    }
}