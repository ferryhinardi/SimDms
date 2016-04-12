using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sparepart.Models
{
    [Table("SpUtlPINVDDtl")]
    public class SpUtlPINVDDtl
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DealerCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string DeliveryNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public string OrderNo { get; set; }
        [Key]
        [Column(Order = 6)]
        public string PartNo { get; set; }
        public string SupplierCode { get; set; }
        public string SalesNo { get; set; }
        public DateTime? SalesDate { get; set; }
        public string CaseNumber { get; set; }
        public string PartNoShip { get; set; }
        public decimal? QtyShipped { get; set; }
        public decimal? SalesUnit { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? DiscPct { get; set; }
        public decimal? DiscAmt { get; set; }
        public decimal? TotInvoiceAmt { get; set; }
        public DateTime? ProcessDate { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public char? Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string TypeOfGoods { get; set; }
    }

    [Table("spUtlPINVDtlView")]
    public class spUtlPINVDtlView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DeliveryNo { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public Int32 TotItem { get; set; }
        public decimal? TotQty { get; set; }
        public decimal? TotAmount { get; set; }
        
    }
}
