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
using GeLang;
using System.Data.Entity.Core.Objects;
using System.Transactions;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class VORController : BaseController
    {
        public JsonResult VORTable()
        {
            string groupArea = Request["Area"];
            string companyCode = Request["Dealer"];
            string branchCode = Request["Outlet"];
            string ReffProblem = Request["ProblemService"];
            string ReffModel = Request["Model"];
            string dateFrom = Request["DateFrom"];
            string dateTo = Request["DateTo"];
            string WIP = Request["WIP"];
            int status = (Request["Open"] != null ? 1 : 0) + (Request["Closed"] != null ? 2 : 0);

            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 10800;
            var data = ctx.Database.SqlQuery<VORPart>("exec usprpt_SvRpTrn021ver2 @GroupNo=@p0, @CompanyCode=@p1, @BranchCode=@p2, @RefferenceCodeDelay=@p3, @RefferenceCodeModel=@p4, @StartDate=@p5, @EndDate=@p6, @WIP=@p7, @Status=@p8", groupArea, companyCode, branchCode, ReffProblem, ReffModel, dateFrom, dateTo, WIP, status).AsQueryable();
            
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult YearVOR()
        {
            try
            {
                string qry = String.Format("SELECT DISTINCT YEAR(CreatedDate) AS [Year] FROM svTrnSrvVOR");
                var data = ctx.Database.SqlQuery<int>(qry).OrderByDescending(x => x).Select(x => new { text = x, value = x }).ToList();
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error", data = ex.InnerException });
            }
        }

        [HttpPost]
        public JsonResult CancelVOR(List<VORPart> VORs)
        {
            try
            {
                using (var tranScope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted, Timeout = new TimeSpan(0, 15, 0) }))
                {
                    foreach (var item in VORs)
                    {
                        var data = ctx.svTrnSrvVORs.Find(item.GNDealerCode, item.GNOutletCode, item.ServiceNo);
                        data.StatusVOR = 2;

                        string sqls = "UPDATE svTrnSrvVOR SET StatusVOR = 2 WHERE CompanyCode = '" + item.GNDealerCode + "'" +
                        " AND BranchCode = '" + item.GNOutletCode + "'" +
                        " AND ServiceNo = '" + item.ServiceNo + "'" + System.Environment.NewLine;

                        GenerateSQL(new SysSQLGateway() { TaskNo = DateTime.Now.ToString("yyyyMMddHHmmss") + "_CancelVOR_" + item.ServiceNo, TaskName = item.GNDealerCode + "_" + item.GNOutletCode + "_" + item.ServiceNo, SQL = sqls, DealerCode = item.GNDealerCode });
                    }
                    ctx.SaveChanges();
                    tranScope.Complete();
                }
                return Json("Success");
            }
            catch (Exception) { return Json("Fail"); }

        }
    }
}
