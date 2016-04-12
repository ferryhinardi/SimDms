using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("SvTrnReturn")]
    public class Return
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
        public string ReturnNo { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public string FPJGovNo { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerCodeBill { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockedDate { get; set; }
    }

    [Table("SvReturnServiceView")]
    public class ReturnView
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
        public string ReturnNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceStatus { get; set; }
        public string FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public string JobOrderNo { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public string JobType { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string PoliceRegNo { get; set; }
        public string BasicModel { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerCodeBill { get; set; }
        public string Remarks { get; set; }
        public string Customer { get; set; }
        public string CustomerBill { get; set; }
        public string ServiceBookNo { get; set; }
        public decimal? Odometer { get; set; }
        public string TransmissionType { get; set; }
        public string ColourCode { get; set; }
    }

    public class ReturnInvoice
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
        public string JobOrderNo { get; set; }
        public string PoliceRegNo { get; set; }
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; }
        public string ColorCode { get; set; }
        public string ColorCodeDesc { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustAddr1 { get; set; }
        public string CustAddr2 { get; set; }
        public string CustAddr3 { get; set; }
        public string CustAddr4 { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public string CustomerCodeBill { get; set; }
        public string CustomerNameBill { get; set; }
        public string CustAddr1Bill { get; set; }
        public string CustAddr2Bill { get; set; }
        public string CustAddr3Bill { get; set; }
        public string CustAddr4Bill { get; set; }
        public string CityCodeBill { get; set; }
        public string CityNameBill { get; set; }
        public string PhoneNo { get; set; }
        public string FaxNo { get; set; }
        public string HPNo { get; set; }
        public decimal LaborDiscPct { get; set; }
        public decimal PartDiscPct { get; set; }
        public decimal MaterialDiscPct { get; set; }
        public string JobType { get; set; }
        public string ForemanID { get; set; }
        public string MechanicID { get; set; }
        public string ServiceStatus { get; set; }
        public string FPJNo { get; set; }
        public bool IsContract { get; set; }
        public string ContractNo { get; set; }
        public string ContractEndPeriod { get; set; }
        public bool? ContractStatus { get; set; }
        public string ContractStatusDesc { get; set; }
        public bool IsClub { get; set; }
        public string ClubCode { get; set; }
        public string ClubEndPeriod { get; set; }
        public bool? ClubStatus { get; set; }
        public string ClubStatusDesc { get; set; }
        public string JobTypeDesc { get; set; }
        public string ServiceStatusDesc { get; set; }
        public string ForemanName { get; set; }
        public string MechanicName { get; set; }
        public string TaxCode { get; set; }
        public decimal TaxPct { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
    }

    public class InvoiceDetail
    {
        [Key]
        [Column(Order = 1)]
        public long SeqNo { get; set; }
        public string BillType { get; set; }
        public string BillTypeDesc { get; set; }
        public string TypeOfGoods { get; set; }
        public string TypeOfGoodsDesc { get; set; }
        public string TaskPartNo { get; set; }
        public decimal SupplyQty { get; set; }
        public decimal OprRetailPriceTotal { get; set; }
        public string SupplySlipNo { get; set; }
        public string TaskPartDesc { get; set; }
        public decimal Price { get; set; }
        public decimal DiscPct { get; set; }
        public decimal PriceNet { get; set; }
    }
}