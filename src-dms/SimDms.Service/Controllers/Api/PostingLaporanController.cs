using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class PostingLaporanController : BaseController
    {
        //
        // GET: /PostingLaporan/
        private struct Month
        {
            public string value { get; set; }
            public string text { get; set; }
        }

        public JsonResult GetMonthNamesCombo()
        {
            var months = new List<Month>();

            for (int i = 1; i <= 12; i++)
            {
                var date = new DateTime(2000, i, 1);
                months.Add(new Month { value = i.ToString(), text = date.ToString("MMMM") });
            }
            
            return Json(months);
        }

        public JsonResult PostingHistory(int year, int month)
        {
            var message = "";

            try
            {
                var result = InsertHstBeResPerformance(year, month);
                result += InsertHstBeResActivity(year, month);
                if (result <= 0) throw new Exception("Transaksi Rollback");
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(new { message = message });
        }

        private int InsertHstBeResPerformance(int year, int month)
        {
            var query = "exec uspfn_SvHstBeResPerformance @p0, @p1, @p2, @p3, @p4, @p5";
            var result = ctx.Database.ExecuteSqlCommand(query, 
                CompanyCode, BranchCode, ProductType, year, month, CurrentUser.UserId);
            return result;
        }

        private int InsertHstBeResActivity(int year, int month)
        {
            var query = "uspfn_SvHstBeResActivity @p0, @p1, @p2, @p3, @p4, @p5";
            var result = ctx.Database.ExecuteSqlCommand(query,
                CompanyCode, BranchCode, ProductType, year, month, CurrentUser.UserId);
            return result;
        }
    }
}
