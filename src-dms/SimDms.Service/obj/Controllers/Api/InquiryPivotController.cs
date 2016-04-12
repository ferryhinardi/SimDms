using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SimDms.Service.Models;
using System.Text;
using System.IO;

namespace SimDms.Service.Controllers.Api
{
    public class InquiryPivotController : BaseController
    {
        public JsonResult ServiceTransaction()
        {

            string StartDate = Request["StartDate"] ?? "";
            string EndDate = Request["EndDate"] ?? "";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = Request["PivotId"];

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@StartDate", StartDate);
            cmd.Parameters.AddWithValue("@EndDate", EndDate);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("datTable1");
            DataSet ds = new DataSet();
            da.Fill(ds);
            dt = ds.Tables[1];

            return Json(new { success = true, data = dt }, JsonRequestBehavior.AllowGet);
        }

    }
}
