using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    public class HistVehicle
    {
        public bool? Cetak { get; set; }
        public string BranchCode { get; set; }
        public string PoliceRegNo { get; set; }
        public string BasicModel { get; set; }
        public string TransmissionType { get; set; }
        public string Chassis { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string Engine { get; set; }
        public string ServiceBookNo { get; set; }
        public string ColourCode { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Customer { get; set; }
        public DateTime? FakturPolisiDate { get; set; }
        public DateTime? LastServiceDate { get; set; }
        public decimal? LastServiceOdometer { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string Dealer { get; set; }
        public string Remarks { get; set; }

        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public bool IsCheckDate { get; set; }
        public bool IsAllBranch { get; set; }

    }

    public class HistVehicleDetails
    {
        public Int32? TaskPartSeq { get; set; }
        public String BranchCode { get; set; }
        public String JobOrderNo { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public String InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public String FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public String JobType { get; set; }
        public Decimal? Odometer { get; set; }
        public String MechanicId { get; set; }
        public String ForemanId { get; set; }
        public String OperationNo { get; set; }
        public Decimal? OperationQty { get; set; }
        public Decimal? OperationAmt { get; set; }
        public Decimal? TotalSrvAmount { get; set; }
        public Decimal? SharingTask { get; set; }
        public String Description { get; set; }
        public String NameSA { get; set; }
        public String NameForeman { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    [Table("SvHistVehicleServiceView")]
    public class VehicleServiceView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PoliceRegNo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerDesc { get; set; }
        public string CustomerAddr { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string DealerDesc { get; set; }
        public string ChassisCode { get; set; }
        public string ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public string EngineNo { get; set; }
        public string ServiceBookNo { get; set; }
        public string ClubCode { get; set; }
        public string ColourCode { get; set; }
        public DateTime? FakturPolisiDate { get; set; }
        public string ClubNo { get; set; }
        public DateTime? ClubDateStart { get; set; }
        public DateTime? ClubDateFinish { get; set; }
        public DateTime? ClubSince { get; set; }
        public string IsClubStatusDesc { get; set; }
        public bool IsClubStatus { get; set; }
        public string IsContractStatusDesc { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveDesc { get; set; }
        public string BasicModel { get; set; }
        public string TransmissionType { get; set; }
        public DateTime? LastServiceDate { get; set; }
        public string LastJobType { get; set; }
        public string ContractNo { get; set; }
        public string ContactName { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }

    }

    public class HistoryIdNasModel
    {
        public bool success { get; set; }
        public HistoryDataNasModel data { get; set; }
    }

    public class HistoryDataNasModel
    {
        public string ServiceNo { get; set; }
        public string JobOrderNo { get; set; }
        public string PoliceRegNo { get; set; }
        public string Vin { get; set; }
        public decimal Odometer { get; set; }
        public string JobsDesc { get; set; }
        public string PartName { get; set; }
        public decimal? Qty { get; set; }
    }

}