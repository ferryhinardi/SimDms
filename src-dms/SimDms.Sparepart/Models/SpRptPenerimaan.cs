using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Models
{
    [Table("RptBinningBrowse")]
    public class RptBinningBrowse
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BinningNo { get; set; }
        public DateTime? BinningDate { get; set; }
        public string Status { get; set; }
        public string ReferenceNo { get; set; }
        public string DNSupplierNo { get; set; }
        public string TypeOfGoods { get; set; }
    }

    [Table("RptWRSBrowse")]
    public class RptWRSBrowse
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string WRSNo { get; set; }
        public DateTime? WRSDate { get; set; }
        public string BinningNo { get; set; }
        public DateTime? BinningDate { get; set; }
        public string TypeOfGoods { get; set; }
        public string TransType { get; set; }
    }

    [Table("gnMstSignature")]
    public class gnMstSignature
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProfitCenterCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string DocumentType { get; set; }
        [Key]
        [Column(Order = 5)]
        public int SeqNo { get; set; }
        public string SignName { get; set; }
        public string TitleSign { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }

    [Table("RptHPPBrowse")]
    public class RptHPPBrowse
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string HPPNo { get; set; }
        public DateTime? HPPDate { get; set; }
        public string WRSNo { get; set; }
        public DateTime? WRSDate { get; set; }
        public string Status { get; set; }
        public string TypeOfGoods { get; set; }
    }

    [Table("RptClaimNo")]
    public class RptClaimNo
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ClaimNo { get; set; }
        public DateTime? ClaimDate { get; set; }
        public string TypeOfGoods { get; set; }
    }

    [Table("RptClaimReceivedNo")]
    public class RptClaimReceivedNo
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ClaimReceivedNo { get; set; }
        public string ClaimNo { get; set; }
        public DateTime? ClaimReceivedDate { get; set; }
        public string TypeOfGoods { get; set; }
    }

}
