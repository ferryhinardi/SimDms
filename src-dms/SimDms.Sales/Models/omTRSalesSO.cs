using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("omTRSalesSO")]
    public class OmTRSalesSO
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string SONo { get; set; }
        public DateTime? SODate { get; set; }
        public string SalesType { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public string CustomerCode { get; set; }
        public string TOPCode { get; set; }
        public decimal? TOPDays { get; set; }
        public string BillTo { get; set; }
        public string ShipTo { get; set; }
        public string ProspectNo { get; set; }
        public string SKPKNo { get; set; }
        public string Salesman { get; set; }
        public string WareHouseCode { get; set; }
        public bool? isLeasing { get; set; }
        public string LeasingCo { get; set; }
        public string GroupPriceCode { get; set; }
        public string Insurance { get; set; }
        public string PaymentType { get; set; }
        public decimal? PrePaymentAmt { get; set; }
        public DateTime? PrePaymentDate { get; set; }
        public string PrePaymentBy { get; set; }
        public string CommissionBy { get; set; }
        public decimal? CommissionAmt { get; set; }
        public string PONo { get; set; }
        public string ContractNo { get; set; }
        public DateTime? RequestDate { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string ApproveBy { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string RejectBy { get; set; }
        public DateTime? RejectDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string SalesCode { get; set; }
        public decimal? Installment { get; set; }
        public DateTime? FinalPaymentDate { get; set; }
        public string SalesCoordinator { get; set; }
        public string SalesHead { get; set; }
        public string BranchManager { get; set; }
        [NotMapped]
        public bool? isC1 { get; set; }
        [NotMapped]
        public bool? isC2 { get; set; }
        [NotMapped]
        public bool? isC3 { get; set; }
        [NotMapped]
        public bool? isC4 { get; set; } 
        [NotMapped]
        public string ITSNo { get; set; }
        [NotMapped]
        public string SalesmanName { get; set; }
    }

    [Table("PmMstTeamMembers")]
    public class PmMstTeamMembers
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string EmployeeID { get; set; }
        [Key]
        [Column(Order = 4)]
        public string TeamID { get; set; }
        public int MemberID { get; set; }
        public bool? IsSupervisor { get; set; }
        public bool? IsLock { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public bool? IsSalesMan { get; set; }
    }

    public class InquiryTrSalesSOView
    {
        public string SONo { get; set; }
        public string SODate { get; set; }
        public string SalesType { get; set; }
        public string RefferenceNo { get; set; }
        public string RefferenceDate { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string SalesmanName { get; set; } 
        public string WareHouseName { get; set; }
        public string BillTo { get; set; }
        public string ShipTo { get; set; }
        public string GroupPriceName { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
    }

    public class SOView
    {
        public string SONo { get; set; }
        public DateTime? SODate { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Salesman { get; set; }
        public string SalesmanName { get; set; }
        public string Status { get; set; }
        public string GroupPriceCode { get; set; }
    }
}