using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    public class WarrantyClaimLookup
    {
        public string BatchNo { get; set; }
        public DateTime? BatchDate { get; set; }
        public string ReceipNo { get; set; }
        public string ReceiptDate { get; set; }
        public string FPJNo { get; set; }
        public string FPJDate { get; set; }
        public string FPJGovNo { get; set; }
        public decimal LotNo { get; set; }
        public decimal ProcessSeq { get; set; }
        public decimal TotalNoOfItem { get; set; }
        public decimal TotalClaimAmt { get; set; }
        public decimal OtherCompensationAmt { get; set; }
    }

    public class WarrantyClaimUploadHdr
    {
        public string GenerateNo { get; set; }
        public DateTime? GenerateDate { get; set; }
        public string SenderDealerCode { get; set; }
        public string SenderDealerName { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public decimal? LotNo { get; set; }
    }

    //public class WarrantyClaimUploadDtls
    //{
    //    public int GenerateSeq { get; set; }
    //    public string CategoryCode { get; set; }
    //    public string IssueNo { get; set; }
    //    public DateTime IssueDate { get; set; }
    //    public string InvoiceNo { get; set; }
    //    public string ServiceBookNo { get; set; }
    //    public string ChassisCode { get; set; }
    //    public string ChassisNo { get; set; }
    //    public string EngineCode { get; set; }
    //    public string EngineNo { get; set; }
    //    public string BasicModel { get; set; }
    //    public DateTime RegisteredDate { get; set; }
    //    public DateTime RepairedDate { get; set; }
    //    public decimal Odometer { get; set; }
    //    public string ComplainCode { get; set; }
    //    public string DefectCode { get; set; }
    //    public decimal SubletHour { get; set; }
    //    public string BasicCode { get; set; }
    //    public string VarCom { get; set; }
    //    public decimal OperationHour { get; set; }
    //    public decimal ClaimAmt { get; set; }
    //    public string TroubleDescription { get; set; }
    //    public string ProblemExplanation { get; set; }
    //}

    public class WarrantyClaimUploadDtls
    {
        public int GenerateSeq { get; set; }
        public string CategoryCode { get; set; }
        public string IssueNo { get; set; }
        public DateTime IssueDate { get; set; }
        public string InvoiceNo { get; set; }
        public string ServiceBookNo { get; set; }
        public string ChassisCodeChassisNo { get; set; }
        public string EngineCodeEngineNo { get; set; }
        public string BasicModel { get; set; }
        public DateTime RegisteredDate { get; set; }
        public DateTime RepairedDate { get; set; }
        public decimal Odometer { get; set; }
        public string ComplainCode { get; set; }
        public string DefectCode { get; set; }
        public decimal SubletHour { get; set; }
        public string BasicCode { get; set; }
        public string VarCom { get; set; }
        public decimal OperationHour { get; set; }
        public decimal ClaimAmt { get; set; }
        public string TroubleDescription { get; set; }
        public string ProblemExplanation { get; set; }
    }

    public class WarrantyClaimUploadPart 
    {
        public int GenerateSeq { get; set; }
        public int PartSeq { get; set; }
        public bool IsCausal { get; set; }
        public string PartNo { get; set; }
        public decimal Quantity { get; set; }
        public string PartName { get; set; }
    }

    public class WarrantyClaimUploadCost
    {
        public int RecNo { get; set; }
        public string BasicModel { get; set; }
        public decimal TotalClaimAmt { get; set; }
    }

    public class WarrantyClaimReceiveHdr 
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string PaymentNo { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal TotalTicket { get; set; }
    }
}