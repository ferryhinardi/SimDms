using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.PreSales.Models
{
    [Table("OmMstVehicle")]
    public class OmMstVehicle
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public decimal ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string ColourCode { get; set; }
        public string ServiceBookNo { get; set; }
        public string KeyNo { get; set; }
        public decimal? COGSUnit { get; set; }
        public decimal? COGSOthers { get; set; }
        public decimal? COGSKaroseri { get; set; }
        public decimal? PpnBmBuyPaid { get; set; }
        public decimal? PpnBmBuy { get; set; }
        public decimal? SalesNetAmt { get; set; }
        public decimal? PpnBmSellPaid { get; set; }
        public decimal? PpnBmSell { get; set; }
        public string PONo { get; set; }
        public string POReturnNo { get; set; }
        public string BPUNo { get; set; }
        public string HPPNo { get; set; }
        public string KaroseriSPKNo { get; set; }
        public string KaroseriTerimaNo { get; set; }
        public string SONo { get; set; }
        public string SOReturnNo { get; set; }
        public string DONo { get; set; }
        public string BPKNo { get; set; }
        public string InvoiceNo { get; set; }
        public string ReqOutNo { get; set; }
        public string TransferOutNo { get; set; }
        public string TransferInNo { get; set; }
        public string WarehouseCode { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public bool IsAlreadyPDI { get; set; }
        public DateTime? BPKDate { get; set; }
        public string FakturPolisiNo { get; set; }
        public DateTime? FakturPolisiDate { get; set; }
        public string PoliceRegistrationNo { get; set; }
        public DateTime? PoliceRegistrationDate { get; set; }
        public bool IsProfitCenterSales { get; set; }
        public bool IsProfitCenterService { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool IsLocked { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockedDate { get; set; }
        public bool IsNonRegister { get; set; }
        public DateTime? BPUDate { get; set; }
        public string SuzukiDONo { get; set; }
        public DateTime SuzukiDODate { get; set; }
        public string SuzukiSJNo { get; set; }
        public DateTime SuzukiSJDate { get; set; }
        public string TransferOutMultiBranchNo { get; set; }
        public string TransferInMultiBranchNo { get; set; }

    }
}