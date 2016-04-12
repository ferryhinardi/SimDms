using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Service.Models;
using System.Data.SqlClient;

namespace SimDms.Service.Controllers.Api
{
    public class ClosingMonthController : BaseController
    {
        public JsonResult Default()
        {
            var data = getPeriode();
            
            var msg = "";
            switch (data.StatusService)
            {
                case 0:
                    msg = "Ada masalah dengan tutup bulan sebelumnya, sehingga proses tutup bulan untuk yang sekarang tidak dapat berlanjut";
                    break;
                case 1:
                    break;
                default:
                    msg = "Maaf sudah dilakukan tutup bulan";
                    break;
            }

            if (data == null)
            {
               msg = "Periode Fiscal belum diseting di Master Company...";
               return Json(new { message = msg });
            }

            return Json(new { 
            FiscalYear = data.FiscalYear,
            FiscalMonth = data.FiscalMonth,
            Periode = data.PeriodeNum,
            PeriodeName = data.PeriodeName,
            status = data.StatusService,
            message = msg
            });
        }

        public JsonResult ValidateClosing()
        {

            var query = string.Format("exec uspfn_SvUtlCheckB4Closing {0},{1},'{2}','{3}'", CompanyCode, BranchCode, ProductType, CurrentUser.UserId);
            var msg = "";
            try
            {
                ctx.Database.ExecuteSqlCommand(query);
            }
            catch (SqlException ex)
            {
                msg = ex.Message;
                var grid = GetOutstandingGrid();
                return Json(new { success = false, message = msg, grid = grid });
            }
            
            return Json(new { success = true });
        }

        public JsonResult Closing()
        {
            var data = getPeriode();
            var result = false;
            object[] parameters = {CompanyCode, BranchCode, data.FiscalYear, data.FiscalMonth, data.PeriodeNum, (data.PeriodeNum < 12) ? data.FiscalYear : data.FiscalYear + 1
                                      , (data.PeriodeNum < 12) ? data.PeriodeNum + 1 : 1, ProfitCenter, data.FromDate.Value.Month};

            var query = "exec uspfn_SvClosingMonth @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8";

            result = ctx.Database.ExecuteSqlCommand(query, parameters) > 0;

            if(result)
            {
                object[] paramSSOuts = {CompanyCode, BranchCode, ProductType, data.EndDate, CurrentUser.UserId ,DateTime.Now};
            var querySSOuts = "exec uspfn_SpHstSSOutstanding @p0,@p1,@p2,@p3,@p4,@p5";

            result = ctx.Database.ExecuteSqlCommand(querySSOuts,paramSSOuts) > -1;
            }

            return Json(new { success = result });
        }

        public List<OutstandingGrid> GetOutstandingGrid()
        {
            var query = string.Format("exec uspfn_SvUtlOutstandingData {0},{1},'{2}'", CompanyCode, BranchCode, ProductType);
            var data = ctx.Database.SqlQuery<OutstandingGrid>(query).ToList();

            return data;
        }

        public Periode getPeriode()
        {
            var data = (from a in ctx.Periodes
             join b in ctx.CoProfileServices
             on new { a.CompanyCode, a.BranchCode, a.FiscalYear, a.FiscalMonth, a.PeriodeNum }
             equals new { b.CompanyCode, b.BranchCode, b.FiscalYear, b.FiscalMonth, PeriodeNum = b.FiscalPeriod }
             where a.CompanyCode == CompanyCode && a.BranchCode == BranchCode
             select a).FirstOrDefault();

            return data;
        }

        public class OutstandingGrid
        {
            public String Info { get; set; }
            public String BranchCode { get; set; }
            public String InvoiceNo { get; set; }
            public DateTime? InvoiceDate { get; set; }
            public String JobOrderNo { get; set; }
            public DateTime? JobOrderDate { get; set; }
            public String FPJNo { get; set; }
            public DateTime? FPJDate { get; set; }
        }
    }
}