using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Absence.Controllers.Api
{
    public class TrainingDetailDashboardController : BaseController
    {
        public ActionResult TrainingPrograms(string dept = "")
        {
            //var qry = ctx.Positions.Where(p => p.CompanyCode == comp && (p.DeptCode.Equals(dept) || p.DeptCode.Equals("COM"))).ToList();
            var qry = ctx.HrMstTrainings.Where(p => p.CompanyCode == CompanyCode).ToList();
            var list = qry.OrderBy(p => p.TrainingCode).Select(p => new { value = p.TrainingCode, text = (p.TrainingDescription.ToUpper() + string.Format("  ({0})", p.TrainingCode)) }).ToList();
            return Json(list);
        }

        public JsonResult QueryTrainingDetail(string post, string tprg, string flt1, string flt2)
        {
            var end = DateTime.Now.ToString("MM/dd/yyyy");
            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            //var data = ctx.Database.SqlQuery<SfmTrainingDetail>("exec uspfn_TrainingDetailDashBoard @AreaCode=@p0, @DealerCode=@p1, @End=@p2,@OutletCode=@p3,@Position=@p4,@TrainingCode=@p5", area, comp, end, outl, post, tprg).AsQueryable();
            var data = ctx.Database.SqlQuery<SfmTrainingDetail>("exec usprpt_mpTrainingDetailDealer @DealerCode=@p0,@OutletCode=@p1,@Position=@p2,@TrainingCode=@p3,@FilterOutlet=@p4,@FilterCol=@p5", CompanyCode, BranchCode, post, tprg, flt1, flt2).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(new { data = data });
        }

        public JsonResult QueryTrainingDetailData(string post, string tprg, string filter1, string filter2)
        {
            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<TrainingDetailData>("exec usprpt_mpTrainingDetailDealer @DealerCode=@p0, @OutletCode=@p1,@Position=@p2,@TrainingCode=@p3,@FilterOutlet=@p4,@FilterCol=@p5", CompanyCode, BranchCode, post, tprg, filter1, filter2).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(new { data = data });
        }

        public JsonResult ExportTrainingDetail(string post, string tprg, string flt1, string flt2)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "usprpt_mpTrainingDetailDealer";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@areaGroup", area);
            cmd.Parameters.AddWithValue("@DealerCode", CompanyCode);
            cmd.Parameters.AddWithValue("@OutletCode", BranchCode);
            cmd.Parameters.AddWithValue("@Position", post);
            cmd.Parameters.AddWithValue("@TrainingCode", tprg);
            cmd.Parameters.AddWithValue("@FilterOutlet", flt1);
            cmd.Parameters.AddWithValue("@FilterCol", flt2);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            var header = new List<List<dynamic>>() { };
            var dealer = ctx.GnMstDealerMappings.FirstOrDefault(x => x.DealerCode == CompanyCode && x.isActive.Value);
            header.Add(new List<dynamic> { "Dealer : ", CompanyCode + " - " + dealer != null ? dealer.DealerAbbreviation : CompanyName });
            return GenerateReportXls(dt, "Training Detail", "TrainingDetail", header);
        }
    }

    public class SfmTrainingDetail
    {
        public string OutletCode { get; set; }
        public string OutletAbbr { get; set; }
        public int? Jml { get; set; }
        public int? T { get; set; }
        public int? NT { get; set; }
    }

    public class TrainingDetailData
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string OutletAbbr { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Position { get; set; }
        public string PosName { get; set; }
        public string Grade { get; set; }
        public string GradeName { get; set; }
        public DateTime? JoinDate { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
    }
}