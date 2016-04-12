using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Service.Models;

namespace SimDms.Service.Controllers.Api
{
    public class UnitInfoController : BaseController
    {
        //
        // GET: /UnitInfo/

        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchCode = BranchCode,
                BranchName = BranchName,
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year
            });
        }

        public JsonResult GetUnitInfo(int month, int year)
        {
            object[] parameters = { CompanyCode, BranchCode, ProductType, year, month };
            var query = "exec uspfn_SvInqGetVehicleInfo {0},{1},{2},{3},{4}";

            var data = ctx.Database.SqlQuery<UnitInfo>(query, parameters);

            return Json(data);
        }

        private DataSet GetDetail(int month, int year, string jobtype)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;

            cmd.CommandText = "uspfn_SvInqGetDtlUnitInfo";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@month", month);
            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@JobType", jobtype);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            return ds;
        }

        public JsonResult GetUnitDetail(int month, int year, string jobtype)
        {
            var unitdtl = GetDetail(month, year, jobtype).Tables[0];
            return Json(GetJson(unitdtl));
        }

        public JsonResult GetInvoiceDetail(int month, int year, string jobtype)
        {
            var invoicedtl = GetDetail(month, year, jobtype).Tables[1];
            return Json(GetJson(invoicedtl));
        }

        public JsonResult GetTaskDetail(int month, int year, string jobtype)
        {
            var taskdtl = GetDetail(month, year, jobtype).Tables[2];
            return Json(GetJson(taskdtl));
        }
    }
}
