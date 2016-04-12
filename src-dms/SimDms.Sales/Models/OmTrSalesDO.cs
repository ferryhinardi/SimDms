using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("OmTrSalesDO")]
    public class OmTrSalesDO
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DONo { get; set; }
        public DateTime? DODate { get; set; }
        public string SONo { get; set; }
        public string CustomerCode { get; set; }
        public string ShipTo { get; set; }
        public string WareHouseCode { get; set; }
        public string Expedition { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }

    }

    public class InquiryTrSalesDOView
    {
        public string DONo { get; set; }
        public string DODate { get; set; }
        public string SalesType { get; set; }
        public string SONo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string ShipTo { get; set; }
        public string WareHouseCode { get; set; }
        public string WareHouseName { get; set; }
        public string Expedition { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
    }

    public class DOView
    {
        public string DONo { get; set; }
        public DateTime? DODate { get; set; }
        public string SONo { get; set; }
        public DateTime? SODate { get; set; }
        public string Status { get; set; }
        public string GroupPriceCode { get; set; }
    }
}