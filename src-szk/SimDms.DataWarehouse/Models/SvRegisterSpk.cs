using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("SvRegisterSpk")]
    public class SvRegisterSpk
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
        public long ServiceNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public string TaskPartNo { get; set; }
        [Key]
        [Column(Order = 6)]
        public decimal TaskPartSeq { get; set; }
        public string JobOrderNo { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public string BasicModel { get; set; }
        public string PoliceRegNo { get; set; }
        public decimal? Odometer { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string GroupJobType { get; set; }
        public string GroupJobTypeDesc { get; set; }
        public string JobType { get; set; }
        public string JobTypeDesc { get; set; }
        public string OperationNo { get; set; }
        public string OperationName { get; set; }
        public decimal? OperationHour { get; set; }
        public string FmID { get; set; }
        public string FmName { get; set; }
        public string SaID { get; set; }
        public string SaName { get; set; }
        public string MechanicID { get; set; }
        public string MechanicName { get; set; }
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public decimal? DemandQty { get; set; }
        public decimal? SupplyQty { get; set; }
        public decimal? ReturnQty { get; set; }
        public string SupplySlipNo { get; set; }
        public string SSReturnNo { get; set; }
        public string TaskPartType { get; set; }
        public string ServiceRequestDesc { get; set; }
        public string ServiceStatus { get; set; }
        public string ServiceStatusDesc { get; set; }
        public decimal? TotalSrvAmount { get; set; }
        public string InvoiceNo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}