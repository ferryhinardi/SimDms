using SimDms.Common;
using SimDms.Sparepart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.BLL
{
    public class SalesPerformanceBLL : BaseBLL
    {
        public static DataContext ctx = new DataContext(MyHelpers.GetConnString("DataContext"));

        public static List<UtlActivityReport> GetActivityReportData(string companyCode, string branchCode, int month, int year)
        {
            object[] parameters = { companyCode, branchCode, month, year };
            var query = "exec usprpt_SpUtlActivityReport @p0,@p1,@p2,@p3";
            var data = ctx.Database.SqlQuery<UtlActivityReport>(query, parameters).ToList();
            return data;
        }

        public static List<UtlStock> GetStockData(string companyCode, string branchCode, int month, int year)
        {
            object[] parameters = { companyCode, branchCode, year, month};
            var query = "exec usprpt_SpUtlStock @p0,@p1,@p2,@p3";
            var data = ctx.Database.SqlQuery<UtlStock>(query, parameters).ToList();
            return data;
        }

        public static List<UtlBackOrderManifest> GetBackOrderManifest(string companyCode, string branchCode, int month, int year)
        {
            object[] parameters = { companyCode, branchCode, year, month };
            var query = "exec usprpt_SpUtlBackOrderManifest @p0,@p1,@p2,@p3";

            var data = ctx.Database.SqlQuery<UtlBackOrderManifest>(query, parameters).ToList();
            return data;
        }

        public static List<UtlPlanRealization> GetPlanRealization(string companyCode, string branchCode, int month, int year)
        {
            object[] parameters = { companyCode, branchCode, year, month };

            var query = "exec usprpt_SpUtlPlanRealization @p0,@p1,@p2,@p3";

            var data = ctx.Database.SqlQuery<UtlPlanRealization>(query, parameters).ToList();
            return data;
        }

        public static List<UtlSales> GetUtilitySales(string companyCode, string branchCode, int month, int year)
        {
            object[] parameters = { companyCode, branchCode, month, year };

            var query = "exec usprpt_SpUtlSales @p0,@p1,@p2,@p3";

            var data = ctx.Database.SqlQuery<UtlSales>(query, parameters).ToList();
            return data;
        }

        public static List<UtlLeadTime> GetUtilityLeadTime(string companyCode, string branchCode, DateTime dateFrom, DateTime dateTo)
        {
            object[] parameters = { companyCode, branchCode, dateFrom, dateTo };

            var query = "exec usprpt_SpUtlLeadTime @p0,@p1,@p2,@p3";

            var data = ctx.Database.SqlQuery<UtlLeadTime>(query, parameters).ToList();
            return data;
        }

        public static string GenPDLYR(string companyCode, string standardCode, string companyName, List<UtlActivityReport> dt, int month, int year)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("H");
            sb.Append("PDLYR");
            sb.Append(GetField(companyCode, 10, ' ', false));
            sb.Append(GetField(standardCode, 10, ' ', false));
            sb.Append(GetField(companyName, 50, ' ', false));
            sb.Append(GetField(dt.Count, 6, '0', true));
            sb.Append(GetField(year, 4, '0', true));
            sb.Append(GetField(month, 2, '0', true));
            sb.Append(GetField("", 429, ' ', false));
            foreach (var row in dt)
            {
                sb.AppendLine();
                sb.Append("1");
                sb.Append(GetField(row.Title, 50, ' ', false));
                sb.Append(GetField(row.Description, 50, ' ', false));
                sb.Append(GetField(row.BOM, 13, '0', true));
                sb.Append(GetField(row._1, 13, '0', true));
                sb.Append(GetField(row._2, 13, '0', true));
                sb.Append(GetField(row._3, 13, '0', true));
                sb.Append(GetField(row._4, 13, '0', true));
                sb.Append(GetField(row._5, 13, '0', true));
                sb.Append(GetField(row._6, 13, '0', true));
                sb.Append(GetField(row._7, 13, '0', true));
                sb.Append(GetField(row._8, 13, '0', true));
                sb.Append(GetField(row._9, 13, '0', true));
                sb.Append(GetField(row._10, 13, '0', true));
                sb.Append(GetField(row._11, 13, '0', true));
                sb.Append(GetField(row._12, 13, '0', true));
                sb.Append(GetField(row._13, 13, '0', true));
                sb.Append(GetField(row._14, 13, '0', true));
                sb.Append(GetField(row._15, 13, '0', true));
                sb.Append(GetField(row._16, 13, '0', true));
                sb.Append(GetField(row._17, 13, '0', true));
                sb.Append(GetField(row._18, 13, '0', true));
                sb.Append(GetField(row._19, 13, '0', true));
                sb.Append(GetField(row._20, 13, '0', true));
                sb.Append(GetField(row._21, 13, '0', true));
                sb.Append(GetField(row._22, 13, '0', true));
                sb.Append(GetField(row._23, 13, '0', true));
                sb.Append(GetField(row._24, 13, '0', true));
                sb.Append(GetField(row._25, 13, '0', true));
                sb.Append(GetField(row._26, 13, '0', true));
                sb.Append(GetField(row._27, 13, '0', true));
                sb.Append(GetField(row._28, 13, '0', true));
                sb.Append(GetField(row._29, 13, '0', true));
                sb.Append(GetField(row._30, 13, '0', true));
                sb.Append(GetField(row._31, 13, '0', true));
            }
            return sb.ToString();
        }

        public static string GenPSTCK(string companyCode, string standardCode, string companyName, List<UtlStock> dt, int month, int year)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("H");
            sb.Append("PSTCK");
            sb.Append(GetField(companyCode, 10, ' ', false));
            sb.Append(GetField(standardCode, 10, ' ', false));
            sb.Append(GetField(companyName, 50, ' ', false));
            sb.Append(GetField(dt.Count, 6, '0', true));
            sb.Append(GetField(dt.First().FiscalYear, 4, '0', true));
            sb.Append(GetField("", 76, ' ', false));
            foreach (var row in dt)
            {
                sb.AppendLine();
                sb.Append("1");
                sb.Append(GetField(row.Month, 2, '0', true));
                sb.Append(GetField(row.PlanStock, 13, '0', true));
                sb.Append(GetField(row.CurrYearStockAmt, 13, '0', true));
                sb.Append(GetField(row.PctVsPlan, 5, '0', true));
                sb.Append(GetField(row.BefYearStockAmt, 13, '0', true));
                sb.Append(GetField(row.PctVsBefYear, 5, '0', true));
                sb.Append(GetField(row.PlanITO, 5, '0', true));
                sb.Append(GetField(row.CurrYearITO, 5, '0', true));
                sb.Append(GetField(row.BefYearITO, 5, '0', true));
                sb.Append(GetField(row.CurrYearStockAmt4N5, 13, '0', true));
                sb.Append(GetField(row.BefYearStockAmt4N5, 13, '0', true));
                sb.Append(GetField(row.PctAmountMC, 5, '0', true));
                sb.Append(GetField(row.Ratio4N5CurrYear, 5, '0', true));
                sb.Append(GetField(row.Ratio4N5BefYear, 5, '0', true));
                sb.Append(GetField(row.CurrYearStockQty, 9, '0', true));
                sb.Append(GetField(row.BefYearStockQty, 9, '0', true));
                sb.Append(GetField(row.PctStockQty, 5, '0', true));
                sb.Append(GetField(row.CurrScrappedAmt, 13, '0', true));
                sb.Append(GetField(row.BefScrappedAmt, 13, '0', true));
                sb.Append(GetField(row.PctScrappedAmt, 5, '0', true));
            }
            return sb.ToString();
        }

        public static string GenPBORD(string companyCode, string standarCode, string companyName, List<UtlBackOrderManifest> dtGenerate)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("H");
            sb.Append("PBORD");
            sb.Append(GetField(companyCode, 10, ' ', false));
            sb.Append(GetField(standarCode, 10, ' ', false));
            sb.Append(GetField(companyName, 50, ' ', false));
            string rowCount = dtGenerate.Count.ToString();
            sb.Append(GetField(rowCount, 6, '0', true));
            sb.Append(GetField(dtGenerate.First().FiscalYear, 4, '0', true));
            sb.Append(GetField("", 53, ' ', false));
            foreach (var row in dtGenerate)
            {
                sb.AppendLine();
                sb.Append("1");
                sb.Append(GetField(row.Month, 2, '0', true));
                sb.Append(GetField(row.CurrLineOrder, 9, '0', true));
                sb.Append(GetField(row.BefLineOrder, 9, '0', true));
                sb.Append(GetField(row.PctLineOrder, 5, '0', true));
                sb.Append(GetField(row.CurrLineBO, 9, '0', true));
                sb.Append(GetField(row.BefLineBO, 9, '0', true));
                sb.Append(GetField(row.PctLineBO, 5, '0', true));
                sb.Append(GetField(row.PlanBORatio, 5, '0', true));
                sb.Append(GetField(row.PctCurrBORatio, 5, '0', true));
                sb.Append(GetField(row.PctBefBORatio, 5, '0', true));
                sb.Append(GetField(row.CurrAmountOrder, 13, '0', true));
                sb.Append(GetField(row.BefAmountOrder, 13, '0', true));
                sb.Append(GetField(row.PctAmountOrder, 5, '0', true));
                sb.Append(GetField(row.CurrAmountBO, 13, '0', true));
                sb.Append(GetField(row.BefAmountBO, 13, '0', true));
                sb.Append(GetField(row.PctAmountBO, 5, '0', true));
                sb.Append(GetField(row.PctAmountCurrBORatio, 5, '0', true));
                sb.Append(GetField(row.PctAmountBefBORatio, 5, '0', true));
            }
            return sb.ToString();
        }

        public static string GenPLRLD(string companyCode, string standarCode, string companyName, List<UtlPlanRealization> dtGenerate)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("H");
            sb.Append("PLRLD");
            sb.Append(GetField(companyCode, 10, ' ', false));
            sb.Append(GetField(standarCode, 10, ' ', false));
            sb.Append(GetField(companyName, 50, ' ', false));
            string rowCount = dtGenerate.Count.ToString();
            sb.Append(GetField(rowCount, 6, '0', true));
            sb.Append(GetField("", 112, ' ', false));
            foreach (var row in dtGenerate)
            {
                sb.AppendLine();
                sb.Append("1");
                sb.Append(GetField(row.FiscalMonth, 2, '0', true));
                sb.Append(GetField(row.PlanSalesAmt, 13, '0', true));
                sb.Append(GetField(row.SalesAmt, 13, '0', true));
                sb.Append(GetField(row.SalesAmtPct, 5, '0', true));
                sb.Append(GetField(row.PlanReceivingAmt, 13, '0', true));
                sb.Append(GetField(row.ReceivingAmt, 13, '0', true));
                sb.Append(GetField(row.ReceivingAmtPct, 5, '0', true));
                sb.Append(GetField(row.PlanSalesCost, 13, '0', true));
                sb.Append(GetField(row.SalesCost, 13, '0', true));
                sb.Append(GetField(row.SalesCostPct, 5, '0', true));
                sb.Append(GetField(row.PlanProfit, 13, '0', true));
                sb.Append(GetField(row.Profit, 13, '0', true));
                sb.Append(GetField(row.ProfitPct, 5, '0', true));
                sb.Append(GetField(row.PlanStockAmt, 13, '0', true));
                sb.Append(GetField(row.StockMonth, 13, '0', true));
                sb.Append(GetField(row.StockMonthPct, 5, '0', true));
                sb.Append(GetField(row.OrderLine, 9, '0', true));
                sb.Append(GetField(row.SupplyLine, 9, '0', true));
                sb.Append(GetField(row.AllocationPct, 5, '0', true));
                sb.Append(GetField(row.BOAmt, 13, '0', true));
            }
            return sb.ToString();
        }

        public static string GenPMSPD(string companyCode, string receivedDealerCode, string companyName, List<UtlSales> dtGenerate)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("H");
            sb.Append("PMSPD");
            sb.Append(GetField(companyCode, 10, ' ', false));
            sb.Append(GetField(receivedDealerCode, 10, ' ', false));
            sb.Append(GetField(companyName, 50, ' ', false));
            sb.Append(GetField(dtGenerate.Count, 6, '0', false));
            sb.Append(GetField("", 127, ' ', false));

            foreach (var row in dtGenerate)
            {
                sb.AppendLine();
                sb.Append("1");
                sb.Append(GetField(row.FiscalMonth, 2, '0', true));
                sb.Append(GetField(row.PlanGenuineAmt, 13, '0', true));
                sb.Append(GetField(row.NewGenuineAmt, 13, '0', true));
                sb.Append(GetField(row.PlanGenuinePct, 5, '0', true));
                sb.Append(GetField(row.OldGenuineAmt, 13, '0', true));
                sb.Append(GetField(row.GenuinePct, 5, '0', true));
                sb.Append(GetField(row.NewNonGenuineAmt, 13, '0', true));
                sb.Append(GetField(row.OldNonGenuineAmt, 13, '0', true));
                sb.Append(GetField(row.NonGenuinePct, 5, '0', true));
                sb.Append(GetField(row.NewTotal, 13, '0', true));
                sb.Append(GetField(row.OldTotal, 13, '0', true));
                sb.Append(GetField(row.TotalPct, 5, '0', true));
                sb.Append(GetField(row.PlanCostSales, 13, '0', true));
                sb.Append(GetField(row.NewCostSales, 13, '0', true));
                sb.Append(GetField(row.PlanCostSalesPct, 5, '0', true));
                sb.Append(GetField(row.OldCostSales, 13, '0', true));
                sb.Append(GetField(row.CostSalesPct, 5, '0', true));
                sb.Append(GetField(row.NewProfit, 13, '0', true));
                sb.Append(GetField(row.OldProfit, 13, '0', true));
                sb.Append(GetField(row.ProfitPct, 5, '0', true));
                sb.Append(GetField(row.PlanProfitRate, 5, '0', true));
                sb.Append(GetField(row.NewProfitRate, 5, '0', true));
                sb.Append(GetField(row.OldProfitRate, 5, '0', true));
            }

            return sb.ToString();
        }

        public static string GenPMLTD(string companyCode, string receivedDealerCode, string companyName, List<UtlLeadTime> dtGenerate)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("H");
            sb.Append("PMLTD");
            sb.Append(GetField(companyCode, 10, ' ', false));
            sb.Append(GetField(receivedDealerCode, 10, ' ', false));
            sb.Append(GetField(companyName, 50, ' ', false));
            sb.Append(GetField(dtGenerate.Count, 6, '0', false));
            sb.Append(GetField("", 374, ' ', false));

            foreach (var row in dtGenerate)
            {
                sb.AppendLine();
                sb.Append("1");
                sb.Append(GetField(row.CustomerCode, 15, ' ', false));
                sb.Append(GetField(row.CustomerName, 100, ' ', false));
                sb.Append(GetField(row.CityCode, 15, ' ', false));
                sb.Append(GetField(row.CityName, 100, ' ', false));
                sb.Append(GetField(row.CategoryCode, 15, ' ', false));
                sb.Append(GetField(row.CustomerRank, 100, ' ', false));
                sb.Append(GetField(row.OrderNo, 15, ' ', false));
                sb.Append(GetField(row.PartNo, 20, ' ', false));
                sb.Append(GetField(row.QtyOrder, 9, '0', true));
                sb.Append(GetField(Convert.ToDateTime(row.KeyInOrder).ToString("YYYYMMDD hh:mm"), 15, ' ', true));
                sb.Append(GetField(row.FPJNo, 15, ' ', false));
                sb.Append(GetField(Convert.ToDateTime(row.PickDate).ToString("YYYYMMDD hh:mm"), 15, ' ', false));
                sb.Append(GetField(Convert.ToDateTime(row.ShipmentDate).ToString("YYYYMMDD hh:mm"), 15, ' ', false));
                sb.Append(GetField(row.LTINT, 6, '0', true));
            }

            return sb.ToString();
        }

        private static string GetField(object field, int len, char kar, bool isPadLeft)
        {
            if (field == null) return "";
            if (isPadLeft)
                return field.ToString().PadLeft(len, kar).Substring(0, len);
            else
                return field.ToString().PadRight(len, kar).Substring(0, len);
        }
    }
}
