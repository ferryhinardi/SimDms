using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("OmTrInventTransferOutMultiBranch")]
    public class OmTrInventTransferOutMultiBranch  
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string TransferOutNo { get; set; }
        public DateTime? TransferOutDate { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public string CompanyCodeFrom { get; set; }
        public string BranchCodeFrom { get; set; }
        public string WareHouseCodeFrom { get; set; }
        public string CompanyCodeTo { get; set; }
        public string BranchCodeTo { get; set; }
        public string WareHouseCodeTo { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockedDate { get; set; }
    }

    [Table("OmTrInventTransferInMultiBranch")]
    public class OmTrInventTransferInMultiBranch
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string TransferInNo { get; set; }
        public DateTime? TransferInDate { get; set; }
        public string TransferOutNo { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public string CompanyCodeFrom { get; set; }
        public string BranchCodeFrom { get; set; }
        public string WareHouseCodeFrom { get; set; }
        public string CompanyCodeTo { get; set; }
        public string BranchCodeTo { get; set; }
        public string WareHouseCodeTo { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockedDate { get; set; }
    }

     [Table("omTrInventTransferOutDetailMultiBranch")]
    public class omTrInventTransferOutDetailMultiBranch
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string TransferOutNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public decimal TransferOutSeq { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string ColourCode { get; set; }
        public decimal? COGSUnit { get; set; }
        public decimal? COGSOthers { get; set; }
        public decimal? COGSKaroseri { get; set; }
        public string Remark { get; set; }
        public string StatusTransferIn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

     [Table("omTrInventTransferInDetailMultiBranch")]
     public class omTrInventTransferInDetailMultiBranch
     {
         [Key]
         [Column(Order = 1)]
         public string CompanyCode { get; set; }
         [Key]
         [Column(Order = 2)]
         public string BranchCode { get; set; }
         [Key]
         [Column(Order = 3)]
         public string TransferInNo { get; set; }
         [Key]
         [Column(Order = 4)]
         public decimal TransferInSeq { get; set; }
         public string SalesModelCode { get; set; }
         public decimal? SalesModelYear { get; set; }
         public string ChassisCode { get; set; }
         public decimal? ChassisNo { get; set; }
         public string EngineCode { get; set; }
         public decimal? EngineNo { get; set; }
         public string ColourCode { get; set; }
         public decimal? COGSUnit { get; set; }
         public decimal? COGSOthers { get; set; }
         public decimal? COGSKaroseri { get; set; }
         public string Remark { get; set; }
         public string StatusTransferOut { get; set; }
         public string CreatedBy { get; set; }
         public DateTime? CreatedDate { get; set; }
         public string LastUpdateBy { get; set; }
         public DateTime? LastUpdateDate { get; set; }
     }
}