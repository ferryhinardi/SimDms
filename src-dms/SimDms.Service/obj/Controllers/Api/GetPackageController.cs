using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class GetPackageController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                BranchCode = BranchCode,
                GenerateDate = DateTime.Now,
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now
            });
        }

        public JsonResult GetPackage(string generateNo)
        {
            try
            {
                var cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = string.Format("exec uspfn_SvTrnPackageGet '{0}', '{1}', '{2}'",
                    CompanyCode, BranchCode, generateNo);
                var ds = new DataSet();
                new SqlDataAdapter(cmd).Fill(ds);

                var table1 = GetJson(ds.Tables[0]);
                var table2 = GetJson(ds.Tables[1]);
                var table3 = GetJson(ds.Tables[2]);

                return Json(new { message = "", header = table1, details = table2, rpInfo = table3 });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        public JsonResult Query(string branchFrom, string branchTo, string dateFrom, string dateTo)
        {
            try
            {
                var from = DateTime.Parse(dateFrom);
                var to = DateTime.Parse(dateTo);
                var cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = string.Format("exec uspfn_SvInqGetPackage '{0}', '{1}', '{2}', '{3}', '{4}'",
                    CompanyCode, branchFrom, branchTo, from, to);
                var ds = new DataSet();
                new SqlDataAdapter(cmd).Fill(ds);

                var table1 = GetJson(ds.Tables[0]);
                var table2 = GetJson(ds.Tables[1]);

                return Json(new { message = "", details = table1, rpInfo = table2, total = table1.Count });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        public JsonResult Save(string branchFrom, string branchTo, string dateFrom, string dateTo)
        {
            try
            {
                var from = DateTime.Parse(dateFrom);
                var to = DateTime.Parse(dateTo);
                var sql = "exec uspfn_SvTrnPackageSave '{0}', '{1}', '{2}', '{3}', {4}, {5}, '{6}'";
                var result = ctx.Database.SqlQuery<string>(sql, 
                    CompanyCode, BranchCode, branchFrom, branchTo, from, to, CurrentUser.UserId);
                return Json(new { message = "", generateNo = result.FirstOrDefault() });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        public JsonResult Delete(string generateNo)
        {
            try
            {
                var sql = "exec uspfn_SvTrnPackageDelete '{0}', '{1}', '{2}'";
                ctx.Database.ExecuteSqlCommand(sql, CompanyCode, BranchCode, generateNo);
                return Json(new { message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }
    }
}
