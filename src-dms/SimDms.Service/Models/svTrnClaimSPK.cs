using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("svTrnClaimSPK")]
    public class SvTrnClaimSPK
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
        public decimal? TotalNoOfItem { get; set; }
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
        public DateTime? CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
    }
}