using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace SimDms.Sparepart.Controllers.Api
{
    public class ReportRegisterController : BaseController
    {
        private bool isFiscalExist;
         
        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchCode = BranchCode,
                BranchName = BranchName,
                ProductType = ProductType,
                IsBranch = IsBranch,
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year
            });
        }

        public JsonResult SupplySlip()
        {
            var status = "Holding";
            var report = "SpRpRgs028";
            if (IsBranch)
            {
                status = "Cabang";
                report = "SpRpRgs001";
            }
            return Json(new { status = status , report = report});
        }

        public JsonResult PendingDocumentDefault()
        {
            var oCoProfileSpare = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode);
            
            if (oCoProfileSpare != null)
            {
                isFiscalExist = Convert.ToDecimal(oCoProfileSpare.FiscalMonth.Equals(string.Empty) ? 0 : oCoProfileSpare.FiscalMonth) > 0 &&
                                Convert.ToDecimal(oCoProfileSpare.FiscalYear.Equals(string.Empty) ? 0 : oCoProfileSpare.FiscalYear) > 0 &&
                                Convert.ToDecimal(oCoProfileSpare.FiscalPeriod.Equals(string.Empty) ? 0 : oCoProfileSpare.FiscalPeriod) > 0;
            }

            if (isFiscalExist)
            {
                var oPeriode = ctx.Periodes.Find(CompanyCode,BranchCode, oCoProfileSpare.FiscalYear, oCoProfileSpare.FiscalMonth, oCoProfileSpare.FiscalPeriod);
                var fiscalYear = oCoProfileSpare.FiscalYear.ToString();
                var fiscalMonth = oCoProfileSpare.FiscalMonth.ToString();
                var fiscalPeriod = oCoProfileSpare.FiscalPeriod.ToString();
                var periodName = oPeriode == null ? string.Empty : oPeriode.PeriodeName.ToString();
                return Json(new { success = true, FiscalYear = fiscalYear , FiscalMonth = fiscalMonth, FiscalPeriod = fiscalPeriod, PeriodName = periodName, PeriodBeg = oCoProfileSpare.PeriodBeg, PeriodEnd = oCoProfileSpare.PeriodEnd});
            }
            else
                return Json(new { success = false, message = "Periode Fiskal belum diseting di Master Company" }); 
        }

        public JsonResult SpRpRgs019()
        {
            DataTable dt = new DataTable();
            var month = Request["month"];
            var year = Request["year"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "usprpt_SpRpRgs019";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 3600;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@Month", month);
            cmd.Parameters.AddWithValue("@Year", year);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            return GenerateReportXls(dt, "usprpt_SpRpRgs019", "usprpt_SpRpRgs019");
        }


    }
}
