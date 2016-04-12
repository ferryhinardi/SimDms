using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Models
{
    public class DocumentNoView
    {
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }

    }

    public class CustomerViewLookUp
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string ProfitCenter { get; set; }
        public string Salesman { get; set; }
        public string CompanyName { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
    }

    public class PaymentLookUp
    {
        public string PaymentCode { get; set; }
        public string PaymentDesc { get; set; }
    }

    public class SalesPartLookUp
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
    }

    public class PartNoLookUp
    {
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public string ABCClass { get; set; }
        public decimal? AvailQty { get; set; }
        public decimal? OnOrder { get; set; }
        public decimal? ReservedSP { get; set; }
        public decimal? ReservedSR { get; set; }
        public decimal? ReservedSL { get; set; }
        public string MovingCode { get; set; }
        public string SupplierCode { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? RetailPriceInclTax { get; set; }
        public decimal? PurchasePrice { get; set; }
    }

    public class JobOrderLookUp
    {
        public string JobOrderNo { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerCodeBill { get; set; }
    }

    public class SONoLookUp
    {
        public string SONo { get; set; }
        public DateTime? SODate { get; set; }
        public string CustomerCode { get; set; }
        public string BillTo { get; set; }
        public string ShipTo { get; set; }
    }

    public class OrderNoLookUp
    {
        public string OrderNo { get; set; }
        public DateTime? OrderDate { get; set; }
    }

    public class PartTable
    {
        public long? No { get; set; }
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public decimal? QtyOrder { get; set; }
        public decimal? SalesAmt { get; set; }
        public decimal? DiscAmt { get; set; }
        public decimal? NetSalesAmt { get; set; }
        public string WarehouseCode { get; set; }
        public decimal? QtySupply { get; set; }
        public decimal? QtyBO { get; set; }
        public string LookupValueName { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? NetAmt { get; set; }
        public string ReferenceNo { get; set; }
        public decimal? DiscPct { get; set; }
        public decimal? CostPrice { get; set; }
    }

    public class UtlPORDDdtlModel
    {
        public long? No { get; set; }
        public string DealerCode { get; set; }
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public decimal? QtyOrder { get; set; }
        public decimal QtySupply { get; set; }
        public decimal QtyBO { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? SalesAmt { get; set; }
        public decimal DiscPct { get; set; }
        public decimal NetSalesAmt { get; set; }
    }

    public class GetModification
    {
        public string ID { get; set; }
        public string InterChangeCode { get; set; }
    }

    public class ParNoSupplied
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string ProductType { get; set; }
        public long ServiceNo { get; set; }
        public DateTime? SupplySlipDate { get; set; }
        public string PartNo { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? CostPrice { get; set; }
        public string TypeOfGoods { get; set; }
        public string BillType { get; set; }
        public decimal? QtyOrder { get; set; }
        public decimal QtySupply { get; set; }
        public decimal? DiscPct { get; set; }
    }
}
