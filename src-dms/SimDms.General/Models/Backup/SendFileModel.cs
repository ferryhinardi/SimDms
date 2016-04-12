using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Models
{
    public class UtlActivityReport
    {
        public Int32? SeqNo { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public String Period { get; set; }
        public Decimal? BOM { get; set; }
        public Decimal? _1 { get; set; }
        public Decimal? _2 { get; set; }
        public Decimal? _3 { get; set; }
        public Decimal? _4 { get; set; }
        public Decimal? _5 { get; set; }
        public Decimal? _6 { get; set; }
        public Decimal? _7 { get; set; }
        public Decimal? _8 { get; set; }
        public Decimal? _9 { get; set; }
        public Decimal? _10 { get; set; }
        public Decimal? _11 { get; set; }
        public Decimal? _12 { get; set; }
        public Decimal? _13 { get; set; }
        public Decimal? _14 { get; set; }
        public Decimal? _15 { get; set; }
        public Decimal? _16 { get; set; }
        public Decimal? _17 { get; set; }
        public Decimal? _18 { get; set; }
        public Decimal? _19 { get; set; }
        public Decimal? _20 { get; set; }
        public Decimal? _21 { get; set; }
        public Decimal? _22 { get; set; }
        public Decimal? _23 { get; set; }
        public Decimal? _24 { get; set; }
        public Decimal? _25 { get; set; }
        public Decimal? _26 { get; set; }
        public Decimal? _27 { get; set; }
        public Decimal? _28 { get; set; }
        public Decimal? _29 { get; set; }
        public Decimal? _30 { get; set; }
        public Decimal? _31 { get; set; }
    }

    public class UtlStock
    {
        public Decimal? FiscalYear { get; set; }
        public Int32? Year { get; set; }
        public Int32? Month { get; set; }
        public Decimal? PeriodeNum { get; set; }
        public Int32? PlanStock { get; set; }
        public Decimal? CurrYearStockAmt { get; set; }
        public Int32? PctVsPlan { get; set; }
        public Decimal? BefYearStockAmt { get; set; }
        public Int32? PctVsBefYear { get; set; }
        public Int32? PlanITO { get; set; }
        public Int32? CurrYearITO { get; set; }
        public Int32? BefYearITO { get; set; }
        public Decimal? CurrYearStockAmt4N5 { get; set; }
        public Decimal? BefYearStockAmt4N5 { get; set; }
        public Int32? PctAmountMC { get; set; }
        public Int32? Ratio4N5CurrYear { get; set; }
        public Int32? Ratio4N5BefYear { get; set; }
        public Decimal? CurrYearStockQty { get; set; }
        public Decimal? BefYearStockQty { get; set; }
        public Int32? PctStockQty { get; set; }
        public Decimal? CurrScrappedAmt { get; set; }
        public Decimal? BefScrappedAmt { get; set; }
        public Int32? PctScrappedAmt { get; set; }
    }

    public class UtlBackOrderManifest
    {
        public Decimal? PeriodeNum { get; set; }
        public Decimal? FiscalYear { get; set; }
        public Int32? Month { get; set; }
        public Int32? CurrLineOrder { get; set; }
        public Int32? BefLineOrder { get; set; }
        public Int32? PctLineOrder { get; set; }
        public Int32? CurrLineBO { get; set; }
        public Int32? BefLineBO { get; set; }
        public Int32? PctLineBO { get; set; }
        public Int32? PlanBORatio { get; set; }
        public Int32? PctCurrBORatio { get; set; }
        public Int32? PctBefBORatio { get; set; }
        public Decimal? CurrAmountOrder { get; set; }
        public Decimal? BefAmountOrder { get; set; }
        public Int32? PctAmountOrder { get; set; }
        public Decimal? CurrAmountBO { get; set; }
        public Decimal? BefAmountBO { get; set; }
        public Int32? PctAmountBO { get; set; }
        public Int32? PctAmountCurrBORatio { get; set; }
        public Int32? PctAmountBefBORatio { get; set; }
    }

    public class UtlPlanRealization
    {
        public Decimal? FiscalYear { get; set; }
        public Int32? FiscalMonth { get; set; }
        public Int32? PlanSalesAmt { get; set; }
        public Decimal? SalesAmt { get; set; }
        public Int32? SalesAmtPct { get; set; }
        public Int32? PlanReceivingAmt { get; set; }
        public Decimal? ReceivingAmt { get; set; }
        public Int32? ReceivingAmtPct { get; set; }
        public Int32? PlanSalesCost { get; set; }
        public Decimal? SalesCost { get; set; }
        public Int32? SalesCostPct { get; set; }
        public Int32? PlanProfit { get; set; }
        public Decimal? Profit { get; set; }
        public Int32? ProfitPct { get; set; }
        public Int32? PlanStockAmt { get; set; }
        public Decimal? StockAmt { get; set; }
        public Int32? StockAmtPct { get; set; }
        public Int32? PlanStockMonth { get; set; }
        public Decimal? StockMonth { get; set; }
        public Int32? StockMonthPct { get; set; }
        public Int32? OrderLine { get; set; }
        public Int32? SupplyLine { get; set; }
        public Int32? AllocationPct { get; set; }
        public Decimal? BOAmt { get; set; }
    }

    public class UtlSales
    {
        public String RecordID { get; set; }
        public Int32? FiscalMonth { get; set; }
        public Int32? PlanGenuineAmt { get; set; }
        public Decimal? NewGenuineAmt { get; set; }
        public Int32? PlanGenuinePct { get; set; }
        public Decimal? OldGenuineAmt { get; set; }
        public Int32? GenuinePct { get; set; }
        public Decimal? NewNonGenuineAmt { get; set; }
        public Decimal? OldNonGenuineAmt { get; set; }
        public Int32? NonGenuinePct { get; set; }
        public Decimal? NewTotal { get; set; }
        public Decimal? OldTotal { get; set; }
        public Int32? TotalPct { get; set; }
        public Int32? PlanCostSales { get; set; }
        public Decimal? NewCostSales { get; set; }
        public Int32? PlanCostSalesPct { get; set; }
        public Decimal? OldCostSales { get; set; }
        public Int32? CostSalesPct { get; set; }
        public Decimal? NewProfit { get; set; }
        public Decimal? OldProfit { get; set; }
        public Int32? ProfitPct { get; set; }
        public Int32? PlanProfitRate { get; set; }
        public Int32? NewProfitRate { get; set; }
        public Int32? OldProfitRate { get; set; }
    }

    public class UtlLeadTime
    {
        public String RecordID { get; set; }
        public String CustomerCode { get; set; }
        public String CustomerName { get; set; }
        public String CityCode { get; set; }
        public String CityName { get; set; }
        public String CategoryCode { get; set; }
        public String CustomerRank { get; set; }
        public String OrderNo { get; set; }
        public String PartNo { get; set; }
        public Decimal? QtyOrder { get; set; }
        public DateTime? KeyInOrder { get; set; }
        public String FPJNo { get; set; }
        public DateTime? PickDate { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public Int32? LTINT { get; set; }
    }

    public class SendFile
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string StandardCode { get; set; }
        public bool Daily { get; set; }
        public bool Stock { get; set; }
        public bool Sales { get; set; }
        public bool PlanRealization { get; set; }
        public bool BackOrder { get; set; }
        public bool LeadTime { get; set; }
        public DateTime FirstPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
    }
}
