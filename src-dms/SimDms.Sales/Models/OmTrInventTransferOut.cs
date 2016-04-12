using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("OmTrInventTransferOut")]
    public class OmTrInventTransferOut
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
        public string BranchCodeFrom { get; set; }
        public string WareHouseCodeFrom { get; set; }
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

    public class InquiryTrTransferOutView
    {
        public string CompanyCode { get; set; }
        public string TransferOutNo { get; set; }
        public string TransferOutDate { get; set; }
        public string ReferenceNo { get; set; }
        public string ReferenceDate { get; set; }
        public string BranchCodeFrom { get; set; }
        public string BranchNameFrom { get; set; }
        public string WareHouseCodeFrom { get; set; }
        public string WareHouseNameFrom { get; set; }
        public string BranchCodeTo { get; set; }
        public string BranchNameTo { get; set; }
        public string WareHouseCodeTo { get; set; }
        public string WareHouseNameTo { get; set; }
        public string ReturnDate { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
    }

    public class TransferOutBrowse
    {
        public string CompanyCodeFrom { get; set; }
        public string CompanyCodeTo { get; set; } 
        public string TransferOutNo { get; set; }
        public DateTime? TransferOutDate { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public string BranchCodeFrom { get; set; }
        public string BranchFrom { get; set; }
        public string WareHouseCodeFrom { get; set; }
        public string WareHouseFrom { get; set; } 
        public string BranchCodeTo { get; set; }
        public string BranchTo { get; set; }
        public string WareHouseCodeTo { get; set; }
        public string WareHouseTo { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
    }

    public class TransferOutBrowseMulti
    {
        public string CompanyCodeFrom { get; set; }
        public string CompanyCodeTo { get; set; }
        public string CompanyCodeToDesc { get; set; }
        public string TransferOutNo { get; set; }
        public DateTime? TransferOutDate { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public string BranchCodeFrom { get; set; }
        public string BranchFrom { get; set; }
        public string WareHouseCodeFrom { get; set; }
        public string WareHouseFrom { get; set; }
        public string BranchCodeTo { get; set; }
        public string BranchCodeToDesc { get; set; }
        public string BranchTo { get; set; }
        public string WareHouseCodeTo { get; set; }
        public string WareHouseTo { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
    } 

    public class TransferOutView
    {
        public string TransferOutNo { get; set; }
        public DateTime? TransferOutDate { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
    }

    public class WHview
    {
        public string LookUpValue { get; set; }
        public string LookUpValueName { get; set; }
        public decimal? SeqNo { get; set; }
    }

    public class select4model
    {
        public string SalesModelCode { get; set; }
        public decimal SalesModelYear { get; set; }
        public string SalesModelDesc { get; set; } 
        public string EngineCode { get; set; }
        public string ChassisCode { get; set; }
        public string ChassisNo { get; set; } //
        public string EngineNo { get; set; }
        public string ColourCode { get; set; }
        public string ColourName { get; set; }
        public string WarehouseCode { get; set; } 
        public string WarehouseName { get; set; } 
        public int? Qty { get; set; } 
    }

}