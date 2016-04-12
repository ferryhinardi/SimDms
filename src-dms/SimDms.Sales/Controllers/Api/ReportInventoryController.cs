using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Sales.Controllers.Api
{
    public class ReportInventoryController : BaseController
    {
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

        public JsonResult StockVehicleDate()
        {
            return Json(new
            {
                Month = GetPeriod().PeriodBeg.Month - 1,
                Year = GetPeriod().PeriodBeg.Year
            });
        }

        public JsonResult Transfer()
        {
            return Json(new
            {
                IsBranch = IsBranch,
                DateFrom = GetPeriod().PeriodBeg,
                DateTo = GetPeriod().PeriodEnd
            });
        }

        public JsonResult ValidatePrintPerincianStok(string CodeFrom, string CodeTo)
        {
            int comparison = string.Compare(CodeFrom, CodeTo, false);

            if (comparison > 0)
            {
                var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "Kode Awal", "Kode Akhir");
                return Json(new { success = false, message = msg });
            }
            return Json(new { success = true, message = "" });
        }


        public JsonResult ValidatePrintStockInventory(bool isGudang, string CodeFrom, string CodeTo)
        {
            string awal = isGudang ? "Gudang Awal" : "Model Awal";
            string akhir = isGudang ? "Gudang Akhir" : "Model Akhir";

            int comparison = string.Compare(CodeFrom, CodeTo, false);

            if (comparison > 0)
            {
                var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, awal, akhir);
                return Json(new { success = false, message = msg });
            }
            return Json(new { success = true, message = "" });
        }

        public JsonResult ValidatePrint(string validate, string CodeFrom, string CodeTo)
        {
            string awal = validate == "Gudang" ? "Gudang Awal" : validate == "Model" ? "Model Awal" : "Warna Awal";
            string akhir = validate == "Gudang" ? "Gudang Akhir" : validate == "Model" ? "Model Akhir" : "Warna Akhir";

            int comparison = string.Compare(CodeFrom, CodeTo, false);

            if (comparison > 0)
            {
                var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, awal, akhir);
                return Json(new { success = false, message = msg });
            }
            return Json(new { success = true, message = "" });
        }

        public GetPeriod GetPeriod()
        {
            var data = ctx.CoProfileFinances.Select(a => new GetPeriod
            {
                Code = "AP",
                CompanyCode = a.CompanyCode,
                BranchCode = a.BranchCode,
                PeriodBeg = a.PeriodBeg.Value,
                PeriodEnd = a.PeriodEnd.Value
            }).Union(ctx.CoProfileFinances.Select(a => new GetPeriod
            {
                Code = "AR",
                CompanyCode = a.CompanyCode,
                BranchCode = a.BranchCode,
                PeriodBeg = a.PeriodBegAR.Value,
                PeriodEnd = a.PeriodEndAR.Value
            })).Union(ctx.CoProfileFinances.Select(a => new GetPeriod
            {
                Code = "GL",
                CompanyCode = a.CompanyCode,
                BranchCode = a.BranchCode,
                PeriodBeg = a.PeriodBegGL.Value,
                PeriodEnd = a.PeriodEndGL.Value
            })).Union(ctx.CoProfileSaleses.Select(a => new GetPeriod
            {
                Code = "SALES",
                CompanyCode = a.CompanyCode,
                BranchCode = a.BranchCode,
                PeriodBeg = a.PeriodBeg.Value,
                PeriodEnd = a.PeriodEnd.Value
            })).FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.Code.Equals("SALES"));

//            var query = string.Format(@"select gb.* from (
//            select 'AP' as Code, CompanyCode, BranchCode, PeriodBeg, PeriodEnd from gnMstCoProfileFinance UNION ALL
//            select 'AR' as Code, CompanyCode, BranchCode, PeriodBegAR, PeriodEndAR from gnMstCoProfileFinance UNION ALL
//            select 'GL' as Code, CompanyCode, BranchCode, PeriodBegGL, PeriodEndGL from gnMstCoProfileFinance UNION ALL
//            select 'SALES' as Code, CompanyCode, BranchCode, PeriodBeg, PeriodEnd from gnMstCoProfileSales)gb
//            where gb.CompanyCode = '{0}'
//            and gb.BranchCode = '{1}'
//            and gb.Code = '{2}'", CompanyCode, BranchCode, "SALES");

//            var datax = ctx.Database.SqlQuery<GetPeriod>(query).FirstOrDefault();

            return data;
        }

    }
}
    
