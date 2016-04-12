using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("SvTrnClaimView")]
    public class ClaimView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 4)]
        public string GenerateNo { get; set; }
        public DateTime? GenerateDate { get; set; }
        public string FromInvoiceNo { get; set; }
        public string ToInvoiceNo { get; set; }
        public string Invoice { get; set; }
        public string FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public string FPJGovNo { get; set; }
        public string SourceData { get; set; }
        public string SourceDataDesc { get; set; }
        public decimal TotalNoOfItem { get; set; }
        public decimal? TotalClaimAmt { get; set; }
        public string SenderDealerName { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public string PostingFlag { get; set; }
        public string PostingFlagDesc { get; set; }
        public string SenderDealerCode { get; set; }
        public string CustomerName { get; set; }
        public bool? IsSparepartClaim { get; set; }
    }

    [Table("SvTrnClaim")]
    public class Claim
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 4)]
        public string GenerateNo { get; set; }
        public DateTime? GenerateDate { get; set; }
        public string SourceData { get; set; }
        public string FromInvoiceNo { get; set; }
        public string ToInvoiceNo { get; set; }
        public string FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public string FPJGovNo { get; set; }
        public string SenderDealerCode { get; set; }
        public string ReceiveDealerCode { get; set; }
        public string SenderDealerName { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public decimal TotalNoOfItem { get; set; }
        public decimal? LotNo { get; set; }
        public string PostingFlag { get; set; }
        public decimal? TotalOperationHour { get; set; }
        public decimal? TotalSubletHour { get; set; }
        public decimal? TotalOperationAmt { get; set; }
        public decimal? TotalSubletAmt { get; set; }
        public decimal? TotalPartAmt { get; set; }
        public decimal? TotalClaimAmt { get; set; }
        public decimal? OtherCompensationAmt { get; set; }
        public decimal? TotalOperationPayHour { get; set; }
        public decimal? TotalSubletPayHour { get; set; }
        public decimal? TotalOperationPaymentAmt { get; set; }
        public decimal? TotalSubletPaymentAmt { get; set; }
        public decimal? TotalPartPaymentAmt { get; set; }
        public decimal? TotalClaimPaymentAmt { get; set; }
        public decimal? OtherCompensationPaymentAmt { get; set; }
        public string BatchNo { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public bool? IsSparepartClaim { get; set; }
    }

    [Table("svTrnClaimApplication")]
    public class ClaimApplication
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 4)]
        public string GenerateNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal GenerateSeq { get; set; }
        public string InvoiceNo { get; set; }
        public string IssueNo { get; set; }
        public DateTime? IssueDate { get; set; }
        public string ClaimStatus { get; set; }
        public string PaymentNo { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string ServiceBookNo { get; set; }
        public string BasicModel { get; set; }
        public string TechnicalModel { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal EngineNo { get; set; }
        public DateTime? RegisteredDate { get; set; }
        public DateTime RepairedDate { get; set; }
        public decimal Odometer { get; set; }
        public bool IsCbu { get; set; }
        public string ComplainCode { get; set; }
        public string DefectCode { get; set; }
        public string CategoryCode { get; set; }
        public string OperationNo { get; set; }
        public decimal? OperationHour { get; set; }
        public decimal? SubletHour { get; set; }
        public decimal? OperationAmt { get; set; }
        public decimal? SubletAmt { get; set; }
        public decimal? PartAmt { get; set; }
        public decimal? ClaimAmt { get; set; }
        public string TroubleDescription { get; set; }
        public string ProblemExplanation { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
    }

    [Table("svTrnClaimSPKApp")]
    public class SvTrnClaimSPKApp
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 4)]
        public string GenerateNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal GenerateSeq { get; set; }
        public string InvoiceNo { get; set; }
        public string IssueNo { get; set; }
        public DateTime? IssueDate { get; set; }
        public char ClaimStatus { get; set; }
        public string PaymentNo { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string ServiceBookNo { get; set; }
        public string BasicModel { get; set; }
        public string TechnicalModel { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal EngineNo { get; set; }
        public DateTime? RegisteredDate { get; set; }
        public DateTime RepairedDate { get; set; }
        public decimal Odometer { get; set; }
        public bool IsCbu { get; set; }
        public string ComplainCode { get; set; }
        public string DefectCode { get; set; }
        public char CategoryCode { get; set; }
        public string OperationNo { get; set; }
        public decimal? OperationHour { get; set; }
        public decimal? SubletHour { get; set; }
        public decimal? OperationAmt { get; set; }
        public decimal? SubletAmt { get; set; }
        public decimal? PartAmt { get; set; }
        public decimal? ClaimAmt { get; set; }
        public string TroubleDescription { get; set; }
        public string ProblemExplanation { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
    }

    [Table("SvTrnClaimPart")]
    public class ClaimPart
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 4)]
        public string GenerateNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal GenerateSeq { get; set; }
        [Key]
        [Column(Order = 6)]
        public decimal? PartSeq { get; set; }
        public bool IsCausal { get; set; }
        public string PartNo { get; set; }
        public string ProcessedPartNo { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? PaymentQuantity { get; set; }
        public decimal? PaymentTotalPrice { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
    }

    [Table("SvBasicCodeView")]
    public class BasicCodeView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BasicModel { get; set; }
        public string BasicCode { get; set; }
        public string VarCom { get; set; }
        [Key]
        [Column(Order = 4)]
        public string OperationNo { get; set; }
        public decimal? OperationHour { get; set; }
        public decimal? LaborPrice { get; set; }
        public string Description { get; set; }
        public string IsActive { get; set; }
        [Key]
        [Column(Order = 5)]
        public string JobType { get; set; }
    }

    [Table("SvTrnClaimBatch")]
    public class ClaimBatch
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 4)]
        public string BatchNo { get; set; }
        public DateTime BatchDate { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public string FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public string FPJGovNo { get; set; }
        public decimal? LotNo { get; set; }
        public decimal? ProcessSeq { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class InquiryClaim
    {
        [Key]
        [Column(Order = 1)]
        public int SeqNo { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public bool IsCbu { get; set; }
        public string CategoryCode { get; set; }
        public string ComplainCode { get; set; }
        public string DefectCode { get; set; }
        public decimal? SubletHour { get; set; }
        public decimal? SubletAmt { get; set; }
        public string CausalPartNo { get; set; }
        public string TroubleDescription { get; set; }
        public string ProblemExplanation { get; set; }
        public string OperationNo { get; set; }
        public decimal? OperationHour { get; set; }
        public decimal? OperationAmt { get; set; }
    }

    public class BasicModelClaim
    {
        public string BasicModel { get; set; }
    }

    public class PartNoClaim
    {
        [Key]
        [Column(Order = 1)]
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public decimal? Available { get; set; }
        public decimal? RetailPriceInclTax { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? ClaimPrice { get; set; }
    }

    public class ClaimAppData
    {
        public long? No { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string IssueNo { get; set; }
        public DateTime? IssueDate { get; set; }
        public string ServiceBookNo { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal EngineNo { get; set; }
        public bool IsCBU { get; set; }
        public string BasicModel { get; set; }
        public DateTime? RegisteredDate { get; set; }
        public DateTime RepairedDate { get; set; }
        public decimal Odometer { get; set; }
        public string ComplainCode { get; set; }
        public string ComplainDesc { get; set; }
        public string DefectCode { get; set; }
        public string DefectDesc { get; set; }
        public decimal? SubletHour { get; set; }
        public string BasicCode { get; set; }
        public string Description { get; set; }
        public string VarCom { get; set; }
        public string OperationNo { get; set; }
        public decimal? Hours { get; set; }
        public decimal GenerateSeq { get; set; }
        public string TroubleDescription { get; set; }
        public string ProblemExplanation { get; set; }
    }

    public class PartData
    {
        public long? No { get; set; }
        public bool IsCausal { get; set; }
        public string PartNo { get; set; }
        public decimal? Quantity { get; set; }
        public string PartName { get; set; }
        public decimal PartSeq { get; set; }
        public decimal? UnitPrice { get; set; }
    }

     public class PartInfoView  
    {
        public decimal? Available { get; set; }
        public decimal? RetailPriceInclTax { get; set; }
        public string PartNo { get; set; }
        public string TypeOfGoods { get; set; }
        public string PartName { get; set; }
        public string GroupTypeOfGoods { get; set; }
        public string Status { get; set; }
        public decimal? NilaiPart { get; set; } 
    }

    public class InfoCost
    {
        public long? No { get; set; }
        public string BasicModel { get; set; }
        public Decimal? Total { get; set; }
    }
}