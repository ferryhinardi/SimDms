using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Models
{
    [Table("svTrnService")]
    public class SvTrnService
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
        public string ServiceType { get; set; }
        public string ServiceStatus { get; set; }
        public string JobOrderNo { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public string EstimationNo { get; set; }
        public DateTime? EstimationDate { get; set; }
        public string BookingNo { get; set; }
        public DateTime? BookingDate { get; set; }
        public string InvoiceNo { get; set; }
        public string ForemanID { get; set; }
        public string MechanicID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerCodeBill { get; set; }
        public string PoliceRegNo { get; set; }
        public string ServiceBookNo { get; set; }
        public string BasicModel { get; set; }
        public string TransmissionType { get; set; }
        public string VIN { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string ColorCode { get; set; }
        public decimal? Odometer { get; set; }
        public string JobType { get; set; }
        public string ServiceRequestDesc { get; set; }
        public bool? ConfirmChangingPart { get; set; }
        public DateTime? EstimateFinishDate { get; set; }
        public DateTime? EstimateFinishDateSys { get; set; }
        public decimal? LaborDiscPct { get; set; }
        public decimal? PartDiscPct { get; set; }
        public decimal? MaterialDiscAmt { get; set; }
        public bool InsurancePayFlag { get; set; }
        public decimal? InsuranceOwnRisk { get; set; }
        public string InsuranceNo { get; set; }
        public string InsuranceJobOrderNo { get; set; }
        public decimal? PPNPct { get; set; }
        public decimal? PPHPct { get; set; }
        public decimal? LaborGrossAmt { get; set; }
        public decimal? PartsGrossAmt { get; set; }
        public decimal? MaterialGrossAmt { get; set; }
        public decimal? LaborDiscAmt { get; set; }
        public decimal? PartsDiscAmt { get; set; }
        public decimal? MaterialDiscPct { get; set; }
        public decimal? LaborDppAmt { get; set; }
        public decimal? PartsDppAmt { get; set; }
        public decimal? MaterialDppAmt { get; set; }
        public decimal? TotalDPPAmount { get; set; }
        public decimal? TotalPphAmount { get; set; }
        public decimal? TotalPpnAmount { get; set; }
        public decimal? TotalSrvAmount { get; set; }
        public decimal PrintSeq { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? IsSparepartClaim { get; set; }
        public DateTime? JobOrderClosed { get; set; }
    }
}
