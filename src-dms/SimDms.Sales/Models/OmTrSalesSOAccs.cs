using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("OmTrSalesSOAccs")]
    public class OmTrSalesSOAccs
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
        public decimal? DemandQty { get; set; }
        public decimal? SupplyQty { get; set; }
        public decimal? ReturnQty { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public char? TypeOfGoods { get; set; }
        public char? BillType { get; set; }
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
    }
}