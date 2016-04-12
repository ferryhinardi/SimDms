using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{

    public class MyComboList
    {
        public string value { get; set; }
        public string text { get; set; }
    }

    [Table("svTrnSrvVOR")]
    public class svTrnSrvVOR
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
        public string JobOrderNo { get; set; }
        public string JobDelayCode { get; set; }
        public string JobReasonDesc { get; set; }
        public DateTime? ClosedDate { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public int StatusVOR { get; set; }
    }

    public class VORPart
    {
        public string TicketNo { get; set; }
        public string DealerCode { get; set; }
        public string GNDealerCode { get; set; }
        public string GNOutletCode { get; set; }
        public string DealerName { get; set; }
        public string SPKNo { get; set; }
        public DateTime? SPKDate { get; set; }
        public Int64 ServiceNo { get; set; }
        public int WIP { get; set; } //Work in Progress
        public string PoliceRegNo { get; set; }
        public string BasicModel { get; set; }
        public string Customer { get; set; }
        public string SA { get; set; }
        public string FM { get; set; }
        public decimal SharingTask { get; set; }
        public string EmployeeID { get; set; }
        public string Mech { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Job { get; set; }
        public string ServiceRequestDesc { get; set; }
        public int countPartDtl { get; set; }
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public string JobDelayCode { get; set; }
        public string JobDelayDesc { get; set; }
        public string JobReasonDesc { get; set; }
        public DateTime? ClosedDate { get; set; }
        public string StatusVOR { get; set; }
        public string CountMechAbs { get; set; }
        public string CountMechAlpha { get; set; }
        public string CountMechOvrTime { get; set; }
        public string CountMechDelay { get; set; }
        public string CountMechNextDelay { get; set; }
    }

    public class VORSummary : VORPart
    {
        public string OutletName { get; set; }
        public string OutletArea { get; set; }
        public string AreaDealer { get; set; }
        public string KalasBeres { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? Process { get; set; }
    }

    public class RefferenceService
    {
        public string RefferenceCode { get; set; }
        public string Description { get; set; }
    }

    public class VORConsistencyReport
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string Area { get; set; }
        public string CompanyName { get; set; }
        public string OutletName { get; set; }
        public int DayOfVOR { get; set; }
        public int DayOfSPK { get; set; }
        public DateTime? DateVOR { get; set; }
        public DateTime? DateSPK { get; set; }
        public int TotalPengiriman { get; set; }
    }

    public class VORConsistencyReportV2 : VORConsistencyReport
    {
        public int TransactionDay { get; set; }
        public int NoVOR { get; set; }
    }

    public class HistJobDelayVOR
    {
        public Int64 ServiceNo { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string GNDealerCode { get; set; }
        public string GNOutletCode { get; set; }
        public string OutletName { get; set; }
        public string OutletArea { get; set; }
        public string AreaDealer { get; set; }
        public string PoliceRegNo { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public string JobDelayDesc1 { get; set; }
        public string JobDelayDesc2 { get; set; }
        public string JobDelayDesc3 { get; set; }
        public string JobDelayDesc4 { get; set; }
        public string JobDelayDesc5 { get; set; }
        public DateTime? LastUpdateDate1 { get; set; }
        public DateTime? LastUpdateDate2 { get; set; }
        public DateTime? LastUpdateDate3 { get; set; }
        public DateTime? LastUpdateDate4 { get; set; }
        public DateTime? LastUpdateDate5 { get; set; }
        public string Status { get; set; }
    }
}