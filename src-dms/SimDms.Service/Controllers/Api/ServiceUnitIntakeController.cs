using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class ServiceUnitIntakeController : BaseController
    {
        public JsonResult Default()
        {
            int year = DateTime.Today.Year;
            int month = DateTime.Today.Month;
            int day = DateTime.Today.Day;

            return Json(new
            {
                FirstPeriod = new DateTime(year, month, 1),
                EndPeriod = new DateTime(year, month, day),
            });
        }

        public JsonResult SrvUnitIntakeGrid()
        {
            string companycode = CompanyCode;
            string branchcode = BranchCode;
            string startdate = Request["FirstPeriod"];
            string enddate = Request["EndPeriod"];
            string novin = Request["NoVin"];
            string nopol = Request["NoPol"];
            string custname = Request["CustName"];
            string rework = Request["Rework"];
            string filterby="";

            if (novin != null)
            {
                filterby += " and VinNo like " + "'%" + novin + "%'";
            }
            if (nopol != null || nopol == "undefined")
            {
                filterby += " and PoliceRegNo like " + "'%" + nopol + "%'";
            }
            if (custname != null || custname == "undefined")
            {
                filterby += " and CustomerName like " + "'%" + custname + "%'";
            }
            if (rework != "")
            {
                filterby += " and JobType = " + "'" + rework + "'";
            }

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_SvUnitIntakeGrid";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", companycode);
            cmd.Parameters.AddWithValue("@BranchCode", branchcode);
            cmd.Parameters.AddWithValue("@StartDate", startdate);
            cmd.Parameters.AddWithValue("@EndDate", enddate);
            cmd.Parameters.AddWithValue("@filterby", filterby);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();            
            DataTable dt = new DataTable();

            da.Fill(ds);
            dt = ds.Tables[0];           

            return Json(GetJson(dt));
        }
    }
}