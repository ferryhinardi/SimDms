using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("svTrnPdiFsc")]
    public class PdiFsc
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
        public string ReceiverDealerCode { get; set; }
        public string SenderDealerName { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public decimal TotalNoOfItem { get; set; }
        public bool IsCampaign { get; set; }
        public string PostingFlag { get; set; }
        public decimal? TotalLaborAmt { get; set; }
        public decimal? TotalMaterialAmt { get; set; }
        public decimal? TotalAmt { get; set; }
        public decimal? TotalLaborPaymentAmt { get; set; }
        public decimal? TotalMaterialPaymentAmt { get; set; }
        public decimal? TotalPaymentAmt { get; set; }
        public string BatchNo { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
    }

    [Table("SvTrnPdiFscView")]
    public class PdiFscView
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
        public decimal? TotalAmt { get; set; }
        public string SenderDealerName { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public string PostingFlag { get; set; }
        public string PostingFlagDesc { get; set; }
        public string SenderDealerCode { get; set; }
        public string CustomerName { get; set; }
    }

    [Table("svTrnPdiFscApplication")]
    public class PdiFscApplication
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
        public string PdiFscStatus { get; set; }
        public string InvoiceNo { get; set; }
        public string PaymentNo { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string SuzukiRefferenceNo { get; set; }
        public string JudgementFlag { get; set; }
        public string ServiceBookNo { get; set; }
        public string BasicModel { get; set; }
        public string TransmissionType { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public decimal? PdiFsc { get; set; }
        public DateTime? ServiceDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? RegisteredDate { get; set; }
        public decimal? Odometer { get; set; }
        public decimal? LaborAmount { get; set; }
        public decimal? MaterialAmount { get; set; }
        public decimal? PdiFscAmount { get; set; }
        public decimal? PaymentAmount { get; set; }
        public decimal? LaborPaymentAmount { get; set; }
        public decimal? MaterialPaymentAmount { get; set; }
        public decimal? PdiFscPaymentAmount { get; set; }
        public string Remarks { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public string BranchCodeInv { get; set; }
    }

    [Table("svMstPdiFscRate")]
    public class PdiFscRate
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
        [Key]
        [Column(Order = 4)]
        public bool IsCampaign { get; set; }
        [Key]
        [Column(Order = 5)]
        public string TransmissionType { get; set; }
        [Key]
        [Column(Order = 6)]
        public decimal PdiFscSeq { get; set; }
        [Key]
        [Column(Order = 7)]
        public DateTime EffectiveDate { get; set; }
        public string Description { get; set; }
        public decimal RegularLaborAmount { get; set; }
        public decimal RegularMaterialAmount { get; set; }
        public decimal RegularTotalAmount { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public decimal? LaborRate { get; set; }
    }

    
}