using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("omTrInventTransferIn")]
    public class omTrInventTransferIn
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

    public class InquiryTrTransferInView
    {
        public string TransferInNo { get; set; }
        public string TransferInDate { get; set; }
        public string TransferOutNo { get; set; }
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

    public class TransferInView
    {
        public string TransferInNo { get; set; }
        public DateTime? TransferInDate { get; set; }
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
        public string Status { get; set; }
        public string Remark { get; set; }
        public string CompanyCodeTo { get; set; }
        public string CompanyCodeFrom { get; set; }
    }
}