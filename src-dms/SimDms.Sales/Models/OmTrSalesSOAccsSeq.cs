using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("OmTrSalesSOAccsSeq")]
    public class OmTrSalesSOAccsSeq
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string SONo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string PartNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal PartSeq { get; set; }
        public decimal? DemandQty { get; set; }
        public decimal? Qty { get; set; }
        public decimal? SupplyQty { get; set; }
        public decimal? ReturnQty { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? DiscExcludePPn { get; set; }
        public decimal? AfterDiscDPP { get; set; }
        public decimal? AfterDiscPPn { get; set; }
        public decimal? AfterDiscTotal { get; set; }
        public string TypeOfGoods { get; set; }
        public string BillType { get; set; }
        public string SupplySlipNo { get; set; }
        public DateTime? SupplySlipDate { get; set; }
        public string SSReturnNo { get; set; }
        public DateTime? SSReturnDate { get; set; }
        public bool? isSubstitution { get; set; }
        public decimal? CancelQty { get; set; }
        public decimal? InvoiceQty { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        [NotMapped]
        public string PartName { get; set; }
        [NotMapped]
        public string ProductType { get; set; }  
    }

    public class SalesSOAccsSeq
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string SONo { get; set; }
        public string PartNo { get; set; }
        public decimal PartSeq { get; set; }
        public decimal? DemandQty { get; set; }
        public decimal? Qty { get; set; }
        public decimal? SupplyQty { get; set; }
        public decimal? ReturnQty { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? DiscExcludePPn { get; set; }
        public decimal? AfterDiscDPP { get; set; }
        public decimal? AfterDiscPPn { get; set; }
        public decimal? AfterDiscTotal { get; set; }
        public string TypeOfGoods { get; set; }
        public string BillType { get; set; }
        public string SupplySlipNo { get; set; }
        public DateTime? SupplySlipDate { get; set; }
        public string SSReturnNo { get; set; }
        public DateTime? SSReturnDate { get; set; }
        public bool? isSubstitution { get; set; }
        public decimal? CancelQty { get; set; }
        public decimal? InvoiceQty { get; set; }
        public string PartName { get; set; }
        public string ProductType { get; set; }
        public decimal? Total { get; set; } 
    }
}