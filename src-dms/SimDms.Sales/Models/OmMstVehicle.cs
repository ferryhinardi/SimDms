using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("OmMstVehicle")]
    public class OmMstVehicle
    {
        public OmMstVehicle()
        {
            this.EngineNo = 0;
            this.COGSUnit = 0;
            this.COGSOthers = 0;
            this.COGSKaroseri = 0;
            this.PpnBmBuyPaid = 0;
            this.PpnBmBuy = 0;
            this.SalesNetAmt = 0;
            this.PpnBmSellPaid = 0;
            this.PpnBmSell = 0;
        }

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

    [Table("OmMstVehicleTemp")]
    public class OmMstVehicleTemp
    {
        public OmMstVehicleTemp()
        {
            this.ChassisNo = 0;
            this.EngineNo = 0;
            this.CreatedDate = DateTime.Now;
            this.LastUpdateDate = DateTime.Now;
            this.COGSUnit = 0;
            this.COGSOthers = 0;
            this.PpnBmBuyPaid = 0;
            this.PpnBmBuy = 0;
        }

        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string RefDONo { get; set; }
        public Decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public Decimal? EngineNo { get; set; }
        public string SalesModelCode { get; set; }
        public Decimal? SalesModelYear { get; set; }
        public string ColourCode { get; set; }
        public string ServiceBookNo { get; set; }
        public string KeyNo { get; set; }
        [DefaultValue(0)]
        public Decimal? COGSUnit { get; set; }
        [DefaultValue(0)]
        public Decimal? COGSOthers { get; set; }
        [DefaultValue(0)]
        public Decimal? PpnBmBuyPaid { get; set; }
        [DefaultValue(0)]
        public Decimal? PpnBmBuy { get; set; }
        public string PONo { get; set; }
        public string POReturnNo { get; set; }
        public string BPUNo { get; set; }
        public DateTime? BPUDate { get; set; }
        public string HPPNo { get; set; }
        public string WarehouseCode { get; set; }
        public string Status { get; set; }
        public bool IsProfitCenterSales { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    [Table("omMstVehicleHistory")]
    public class OmMstVehicleHistory
    {
        public OmMstVehicleHistory(){
            this.COGSUnit = 0;
            this.COGSOthers = 0;
            this.COGSKaroseri = 0;
            this.PpnBmBuyPaid = 0;
            this.PpnBmBuy = 0;
            this.SalesNetAmt = 0;
            this.PpnBmSellPaid = 0;
            this.PpnBmSell = 0;
            this.LastUpdateDate = DateTime.Now;
            this.EngineNo = 0;
        }

        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public Decimal ChassisNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public Decimal SeqNo { get; set; }
        public string EngineCode { get; set; }
        public Decimal EngineNo { get; set; }
        public string SalesModelCode { get; set; }
        public Decimal SalesModelYear { get; set; }
        public string ColourCode { get; set; }
        public string ServiceBookNo { get; set; }
        public string KeyNo { get; set; }
        public Decimal COGSUnit { get; set; }
        public Decimal COGSOthers { get; set; }
        public Decimal COGSKaroseri { get; set; }
        public Decimal PpnBmBuyPaid { get; set; }
        public Decimal PpnBmBuy { get; set; }
        public Decimal SalesNetAmt { get; set; }
        public Decimal PpnBmSellPaid { get; set; }
        public Decimal PpnBmSell { get; set; }
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
        public DateTime BPKDate { get; set; }
        public string FakturPolisiNo { get; set; }
        public DateTime FakturPolisiDate { get; set; }
        public string PoliceRegistrationNo { get; set; }
        public DateTime PoliceRegistrationDate { get; set; }
        public bool IsProfitCenterSales { get; set; }
        public bool IsProfitCenterService { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public bool IsLocked { get; set; }
        public string LockedBy { get; set; }
        public DateTime LockedDate { get; set; }
        public bool IsNonRegister { get; set; }
        public DateTime BPUDate { get; set; }
        public string SuzukiDONo { get; set; }
        public DateTime SuzukiDODate { get; set; }
        public string SuzukiSJNo { get; set; }
        public DateTime SuzukiSJDate { get; set; }
    }

    public class MstVehicleLookup
    {
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string SalesModelCode { get; set; }
        public string SalesModelDesc { get; set; }
        public decimal? COGSUnit { get; set; }
        public decimal? COGSOthers { get; set; } 
        public decimal? COGSKaroseri { get; set; }
        public string InvoiceNo { get; set; }
        public decimal? SalesNetAmt { get; set; }
        public decimal? PpnBmSellPaid { get; set; }
        public decimal? PpnBmSell { get; set; }
        public string Status { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class OmMstVehicleTempLookUp
    {
        public string CompanyCode { get; set; }
        public string ChassisCode { get; set; }
        public string RefDONo { get; set; }
        public Decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public Decimal? EngineNo { get; set; }
        public string SalesModelCode { get; set; }
        public Decimal? SalesModelYear { get; set; }
        public string ColourCode { get; set; }
        public string ServiceBookNo { get; set; }
        public string KeyNo { get; set; }
        public Decimal? COGSUnit { get; set; }
        public Decimal? COGSOthers { get; set; }
        public Decimal? PpnBmBuyPaid { get; set; }
        public Decimal? PpnBmBuy { get; set; }
        public string PONo { get; set; }
        public string POReturnNo { get; set; }
        public string BPUNo { get; set; }
        public DateTime? BPUDate { get; set; }
        public int BPUSeq { get; set; }
        public string HPPNo { get; set; }
        public string WarehouseCode { get; set; }
        public string Status { get; set; }
        public bool IsProfitCenterSales { get; set; }
        public bool IsActive { get; set; }
    }

    public class InquiryMstVehicleView
    {
        public string CompanyCode { get; set; }
        public string ChassisCode { get; set; }
        public string ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public string EngineNo { get; set; }
        public string SalesModelCode { get; set; }
        public string SalesModelDesc { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string Status { get; set; }
        public string IsActive { get; set; }
        public string ColourOld { get; set; }
        public string ColourCodeOld { get; set; }
        public string WareHouseCode { get; set; }
    }

    public class InquiryDetailDataKendaraan
    {
        public string warehouseName { get; set; }
	    public string ColourName { get; set; }
	    public string servicebookno { get; set; }
	    public string keyno { get; set; }
        public decimal? cogsunit { get; set; }
        public decimal? cogsOthers { get; set; }
        public decimal? cogsKaroseri { get; set; }
        public decimal? dpp { get; set; }
        public decimal? ppn { get; set; }
        public decimal? bbn { get; set; }
	    public string pono { get; set; }
        public string podate { get; set; }
	    public string bpuno { get; set; }
        public string bpudate { get; set; }
	    public string sono { get; set; }
        public string sodate { get; set; }
	    public string dono { get; set; }
        public string dodate { get; set; }
        public string SKPKNo { get; set; }
        public string SKPKDate { get; set; }
        public string bpkno { get; set; }
        public string bpkdate { get; set; }
	    public string invoiceNo { get; set; }
        public string invoicedate { get; set; }
        public string RefferenceSJNo { get; set; }
	    public string RefferenceSJDate { get; set; }
        public string hppno { get; set; }
        public string hppdate { get; set; }
	    public string reqOutNo { get; set; }
        public string reqdate { get; set; }
        public string reffinv { get; set; }
        public string reffinvdate { get; set; }
        public string refffp { get; set; }
        public string refffpdate { get; set; }
	    public string policeregno { get; set; }
        public string policeregdate { get; set; }
	    public string Customer { get; set; }
	    public string Address { get; set; }
	    public string Salesman { get; set; }
	    public string Leasing { get; set; }
	    public string KelAR { get; set; }
        public string BPKBNo { get; set; }
        public string SPKNo { get; set; }
        public string ChassisCode { get; set; }
        public string SalesModelCode { get; set; }
        public decimal ChassisNo { get; set; }
        public decimal EngineNo { get; set; }
    }
}