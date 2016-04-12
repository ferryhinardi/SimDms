using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("svTrnInvClaim")]
    public class svTrnInvClaim
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
        public string InvoiceNo { get; set; }
        public bool IsCbu { get; set; }
        public string CategoryCode { get; set; }
        public string ComplainCode { get; set; }
        public string DefectCode { get; set; }
        public decimal? SubletHour { get; set; }
        public decimal? SubletAmt { get; set; }
        public string CausalPartNo { get; set; }
        public string TroubleDescription { get; set; }
        public string ProblemExplanation { get; set; }
        public bool? IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string OperationNo { get; set; }
        public decimal? OperationHour { get; set; }
        public decimal? OperationAmt { get; set; }

    }

    [Table("SvTrnCatCode")]
    public class SvTrnCatCode
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string RefferenceCode { get; set; }
        public string Description { get; set; }
        public string DescriptionEng { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }


    }

    [Table("SvTrnComCode")]
    public class SvTrnComCode
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ComCode { get; set; }
        public string Description { get; set; }
        public string DescriptionEng { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }



    }

    [Table("SvTrnDefCode")]
    public class SvTrnDefCode
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DefCod { get; set; }
        public string Description { get; set; }
        public string DescriptionEng { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }



    }

    [Table("SvTrnOpNo")]
    public class SvTrnOpNo
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public long ServiceNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string OperationNo { get; set; }
        public decimal? OperationHour { get; set; }
        public string Description { get; set; }
    }

    public class CausalPart
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
        public string PartNo { get; set; }
        public string PartName { get; set; }

    }

    public class HelpSave
    {
        public string RefferenceCode { get; set; }
        public string ComCode { get; set; }
        public string DefCod { get; set; }
        public string OperationNoDtl { get; set; }
        public string PartNo { get; set; }
        public string TroubleDesc { get; set; }
        public string ProblemExp { get; set; }

    }

    public class InputInfoSpk
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
        public bool? IsSparepartClaim { get; set; }
        public string Customer { get; set; }
        public string CustomerBill { get; set; }

    }

    [Table("SvTrnSPKGeneralView")]
    public class SvTrnSPKGeneralView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public Int64 ServiceNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string JobOrderNo { get; set; }
        public string ServiceType { get; set; }
        public string ForemanID { get; set; }
        public string EstimationNo { get; set; }
        public DateTime? EstimationDate  { get; set; }
        public string BookingNo { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public string PoliceRegNo { get; set; }
        public string ServiceBookNo { get; set; }
        public string BasicModel { get; set; }
        public string TransmissionType { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string KodeRangka { get; set; }
        public string KodeMesin { get; set; }
        public string ColorCode { get; set; }
        public string Customer { get; set; }
        public string CustomerBill { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerCodeBill { get; set; }
        public decimal? Odometer { get; set; }
        public string JobType { get; set; }
        public string ServiceStatus { get; set; }
        public bool InsurancePayFlag { get; set; }
        public decimal? InsuranceOwnRisk { get; set; }
        public string InsuranceNo { get; set; }
        public string InsuranceJobOrderNo { get; set; }
        public decimal? LaborDiscPct { get; set; }
        public decimal? PartDiscPct { get; set; }
        public decimal? MaterialDiscPct { get; set; }
        public decimal? PPNPct { get; set; }
        public string ServiceRequestDesc { get; set; }
        public bool ConfirmChangingPart { get; set; }
        public string MechanicID { get; set; }
        public DateTime? EstimateFinishDate { get; set; }
        public decimal? LaborDPPAmt { get; set; }
        public decimal? PartsDPPAmt { get; set; }
        public decimal? MaterialDPPAmt { get; set; }
        public decimal? TotalDPPAmount { get; set; }
        public decimal? TotalPpnAmount { get; set; }
        public decimal? TotalPphAmount { get; set; }
        public decimal? TotalSrvAmount { get; set; }
        public string EmployeeName { get; set; }
        public string AddressBill { get; set; }
        public string NPWPNo { get; set; }
        public string PhoneNo { get; set; }
        public string HPNo { get; set; }
    }

}