using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    public class BranchLookup
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        public string CompanyName { get; set; }
    }

    [Table("SvGetAllBranchFromSPKView")]
    public class AllBranchFromSPK
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BranchCode { get; set; }
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

    public class FscFromSPKModel
    {
        public string CompanyCode { get; set; }
        public string GenerateNo { get; set; }
        public string GenerateDate { get; set; }
        public string BranchFrom { get; set; }
        public string BranchTo { get; set; }
        public string PeriodeDateFrom { get; set; }
        public string PeriodeDateTo { get; set; }
        public bool IsPDI { get; set; }
        public bool IsFSC { get; set; }

        public string Branch { get; set; }
        public string IsSPKOutstanding { get; set; }
        public string ManyBranch { get; set; }
    }

    public class SPKOutstandingView
    {
        [Key]
        [Column(Order = 1)]
        public string BranchCode { get; set; }
        public string JobOrderNo { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public string PoliceRegNo { get; set; }
        public string BasicModel { get; set; }
        public string JobType { get; set; }
        public string EmployeeName { get; set; }
        public string Status { get; set; }
    }

    public class PdiFscSave
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string JobOrderNo { get; set; }
        public string JobOrderDate { get; set; }
        public string BasicModel { get; set; }
        public string ServiceBookNo { get; set; }
        public string PdiFscSeq { get; set; }
        public string Odometer { get; set; }
        public string LaborGrossAmt { get; set; }
        public string MaterialGrossAmt { get; set; }
        public string PdiFscAmount { get; set; }
        public string FakturPolisiDate { get; set; }
        public string BPKDate { get; set; }
        public string ChassisCode { get; set; }
        public string ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public string EngineNo { get; set; }
        public string InvoiceNo { get; set; }
        public string FPJNo { get; set; }
        public string FPJDate { get; set; }
        public string FPJGovNo { get; set; }
        public string TransmissionType { get; set; }
        public string ServiceStatus { get; set; }
        public string ProductType { get; set; }
    }

    public class PdiFscSave2
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string JobOrderNo { get; set; }
        public string JobOrderDate { get; set; }
        public string BasicModel { get; set; }
        public string ServiceBookNo { get; set; }
        public decimal PdiFscSeq { get; set; }
        public decimal Odometer { get; set; }
        public decimal LaborGrossAmt { get; set; }
        public decimal MaterialGrossAmt { get; set; }
        public decimal PdiFscAmount { get; set; }
        public string FakturPolisiDate { get; set; }
        public string BPKDate { get; set; }
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal EngineNo { get; set; }
        public string InvoiceNo { get; set; }
        public string FPJNo { get; set; }
        public string FPJDate { get; set; }
        public string FPJGovNo { get; set; }
        public string TransmissionType { get; set; }
        public string ServiceStatus { get; set; }
        public string ProductType { get; set; }
    }

    public class KSGSPKLookUp
    {
        public string BatchNo { get; set; }
        public DateTime? BatchDate { get; set; }
        public string GenerateNoStart { get; set; }
        public string GenerateNoEnd { get; set; }
        public decimal TotalNoOfItem { get; set; }
        public decimal TotalAmt { get; set; }
    }

    public class GetKSGSPK
    {
        public int IsSelected { get; set; }
        public string BranchData { get; set; }
        public string BranchCode { get; set; }
        public string GenerateNo { get; set; }
        public DateTime? GenerateDate { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public string SenderDealerCode { get; set; }
        public string SenderDealerName { get; set; }
        public decimal TotalNoOfItem { get; set; }
        public decimal? TotalLaborAmt { get; set; }
        public decimal? TotalMaterialAmt { get; set; }
        public decimal? TotalAmt { get; set; }
    }

    public class GenKSGSPKSave
    {
        public string BranchCode { get; set; }
        public string GenerateNo { get; set; }
    }

    public class PdiFscHdrFileModel
    {
        public string BatchNo { get; set; }
        public DateTime? BatchDate { get; set; }
        public string GenerateNoStart { get; set; }
        public string GenerateNoEnd { get; set; }
        public decimal TotalNoOfItem { get; set; }
        public decimal TotalAmt { get; set; }
        public string RecordID { get; set; }
        public string DataID { get; set; }
        public string DealerCode { get; set; }
        public string RcvDealerCode { get; set; }
        public string DealerName { get; set; }
        public decimal TotalItem { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public string ProductType { get; set; }
        public bool IsCampaign { get; set; }
    }

    public class PdiFscDtlFileModel
    {
        public string BranchData { get; set; }
        public string BranchCode { get; set; }
        public string GenerateNo { get; set; }
        public DateTime? GenerateDate { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public string SenderDealerCode { get; set; }
        public string SenderDealerName { get; set; }
        public decimal TotalNoOfItem { get; set; }
        public decimal? TotalLaborAmt { get; set; }
        public decimal? TotalMaterialAmt { get; set; }
        public decimal? TotalAmt { get; set; }
        public string BatchNo { get; set; }
        public decimal? GenerateSeq { get; set; }
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
    }

    //getksg from spk
    public class SvTrnPDIFSC
    {
        public string BranchCode { get; set; }
        public string GenerateNo { get; set; }
        public DateTime? GenerateDate { get; set; }
        public Int32 GenerateSeq { get; set; }
        public string SenderDealerCode { get; set; }
        public string SenderDealerName { get; set; }
        public string FpjNo { get; set; }
        public DateTime? FpjDate { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public string SuzukiRefferenceNo { get; set; }
        public string PaymentNo { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string SPKNo { get; set; }
        public string IsCampaign { get; set; }
        public string ServiceBookNo { get; set; }
        public Int32 PdiFsc { get; set; }
        public decimal? Odometer { get; set; }
        public DateTime? ServiceDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? RegisteredDate { get; set; }
        public string BasicModel { get; set; }
        public string TransmissionType { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public decimal? LaborAmount { get; set; }
        public decimal? MaterialAmount { get; set; }
        public decimal? PdiFscAmount { get; set; }
        public decimal? LaborPaymentAmount { get; set; }
        public decimal? MaterialPaymentAmount { get; set; }
        public decimal? PdiFscPaymentAmount { get; set; }
        public string SourceData { get; set; }
        public string Remarks { get; set; }

    }
}