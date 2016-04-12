using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("SvTaxInvoiceView")]
    public class TaxInvoiceView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string FPJNo { get; set; }
        public DateTime FPJDate { get; set; }
        public string InvoiceStart { get; set; }
        public string InvoiceEnd { get; set; }
        public string Invoice { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCodeBill { get; set; }
        public string Customer { get; set; }
        public string CustomerBill { get; set; }
    }

    [Table("SvTaxInvoiceHQView")]
    public class TaxInvoiceHQView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string FPJNo { get; set; }
        public DateTime FPJDate { get; set; }
        public string Invoice { get; set; }
        public string BranchStart { get; set; }
        public string BranchEnd { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCodeBill { get; set; }
        public string Customer { get; set; }
        public string CustomerBill { get; set; }
    }

    [Table("SvTaxInvoiceStdView")]
    public class TaxInvoiceStdView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string FPJNo { get; set; }
        public DateTime FPJDate { get; set; }
        public string InvoiceNo { get; set; }
        public string Customer { get; set; }
        public string CustomerBill { get; set; }
        public string CustomerCode { get; set; }
        public string InvoiceStart { get; set; }
        public string InvoiceEnd { get; set; }
    }
    

    [Table("SvTaxInvoiceLookUpView")]
    public class TaxInvoiceLookUpView
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
        public DateTime InvoiceDate { get; set; }
        public string JobOrderNo { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public string PoliceRegNo { get; set; }
        public string JobType { get; set; }
        public string Customer { get; set; }
        public string CustomerBill { get; set; }
        public string CustomerCode { get; set; }
    }

    [Table("SvTrnFakturPajak")]
    public class TaxInvoice
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string FPJNo { get; set; }
        public DateTime FPJDate { get; set; }
        public string FPJGovNo { get; set; }
        public string FPJCentralNo { get; set; }
        public DateTime? FPJCentralDate { get; set; }
        public decimal NoOfInvoice { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerCodeBill { get; set; }
        public string CustomerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string PhoneNo { get; set; }
        public string HPNo { get; set; }
        public bool IsPKP { get; set; }
        public string SKPNo { get; set; }
        public DateTime? SKPDate { get; set; }
        public string NPWPNo { get; set; }
        public DateTime? NPWPDate { get; set; }
        public string TOPCode { get; set; }
        public decimal? TOPDays { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime SignedDate { get; set; }
        public decimal? PrintSeq { get; set; }
        public string GenerateStatus { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
    }
    
    public class TaxInvoiceSave
    {
        public string BranchCode { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string TOPCode { get; set; }
        public decimal? TOPDays { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPKP { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerCodeBill { get; set; }
    }

    public class FPJGCustLookup
    {
        public string CompanyCode { get; set; }
        public string CustomerCode { get; set; }
        public string BranchCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string ProfitCenter { get; set; }

    }

    public class GetInqFpjList
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public string InvoiceStart { get; set; }
        public string InvoiceEnd { get; set; }
        public string Invoice { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerCodeBill { get; set; }
        public string Customer { get; set; }
        public string CustomerBill { get; set; }
        public string BranchStart { get; set; }
        public string BranchEnd { get; set; }
    }



    public class InqFPJData
    {
        public Int64? RowNum { get; set; }
        public Boolean? IsSelected { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string JobOrderNo { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public Decimal? TotalDPPAmt { get; set; }
        public Decimal? TotalPpnAmt { get; set; }
        public Decimal? TotalSrvAmt { get; set; }
        public string JobType { get; set; }
        public string PoliceRegNo { get; set; }
        public string BasicModel { get; set; }
        public string ChassisCode { get; set; }
        public Decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public Decimal? EngineNo { get; set; }
        public string TOPCode { get; set; }
        public Decimal? TOPDays { get; set; }
        public DateTime? DueDate { get; set; }
        public string FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public string CustomerCodeBill { get; set; }
        public Decimal? Odometer { get; set; }
        public Boolean? IsPkp { get; set; }
        public string CustomerCode { get; set; }
        public string Pelanggan { get; set; }
        public string Pembayar { get; set; }
        public DateTime? DueDate1 { get; set; }
        public Boolean? IsSparepartClaim { get; set; }
        public decimal CampaignFlag { get; set; }
    }

    public class InqGenFPJData
    {
        public Int64?  RowNum  { get; set; }
	    public Boolean?  IsSelected  { get; set; }
	    public String  CompanyCode  { get; set; }
	    public String  BranchCode  { get; set; }
	    public String  InvoiceNo  { get; set; }
	    public DateTime?  InvoiceDate  { get; set; }
	    public String  JobOrderNo  { get; set; }
	    public DateTime?  JobOrderDate  { get; set; }
	    public Decimal?  TotalDPPAmt  { get; set; }
	    public Decimal?  TotalPpnAmt  { get; set; }
	    public Decimal?  TotalSrvAmt  { get; set; }
	    public String  JobType  { get; set; }
	    public String  PoliceRegNo  { get; set; }
	    public String  BasicModel  { get; set; }
	    public String  ChassisCode  { get; set; }
	    public Decimal?  ChassisNo  { get; set; }
	    public String  EngineCode  { get; set; }
	    public Decimal?  EngineNo  { get; set; }
	    public String  TOPCode  { get; set; }
	    public Decimal?  TOPDays  { get; set; }
	    public DateTime?  DueDate  { get; set; }
	    public String  FPJNo  { get; set; }
	    public DateTime?  FPJDate  { get; set; }
	    public String  CustomerCodeBill  { get; set; }
	    public Decimal?  Odometer  { get; set; }
	    public Boolean?  IsPkp  { get; set; }
	    public String  CustomerCode  { get; set; }
	    public String  Pelanggan  { get; set; }
	    public String  Pembayar  { get; set; }
	    public DateTime?  DueDate1  { get; set; }
	    public Boolean?  IsSparepartClaim  { get; set; }
	    public String  CampaignFlag  { get; set; }
    }

    public class InqFPJGet
    {
        public Int64? RowNum { get; set; }
        public Boolean? IsSelected { get; set; }
        public string BranchCode { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string JobOrderNo { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public Decimal? TotalDPPAmt { get; set; }
        public Decimal? TotalPpnAmt { get; set; }
        public Decimal? TotalSrvAmt { get; set; }
        public string JobType { get; set; }
        public string PoliceRegNo { get; set; }
        public string BasicModel { get; set; }
        public string ChassisCode { get; set; }
        public Decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public Decimal? EngineNo { get; set; }
        public string TOPCode { get; set; }
        public Decimal? TOPDays { get; set; }
        public DateTime? DueDate { get; set; }
        public string FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public string CustomerCodeBill { get; set; }
        public Decimal? Odometer { get; set; }
        public Boolean? IsPkp { get; set; }
        public string CustomerCode { get; set; }
        public string Pelanggan { get; set; }
        public string Pembayar { get; set; }
    }

    public class ModelName
    {
        public long? RowNum { get; set; }
        public bool? IsSelected { get; set; }
        public string BranchCode { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string JobOrderNo { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public decimal TotalDPPAmt { get; set; }
        public decimal? TotalPpnAmt { get; set; }
        public decimal TotalSrvAmt { get; set; }
        public string JobType { get; set; }
        public string PoliceRegNo { get; set; }
        public string BasicModel { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string TOPCode { get; set; }
        public decimal? TOPDays { get; set; }
        public DateTime? DueDate { get; set; }
        public string FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public string CustomerCodeBill { get; set; }
        public decimal? Odometer { get; set; }
        public bool? IsPkp { get; set; }
        public string CustomerCode { get; set; }
        public string Pelanggan { get; set; }
        public string Pembayar { get; set; }
    }


    public class TaxInvoicePreSave
    {
        public string BranchCode { get; set; }
        public string InvoiceNo { get; set; }
    }
}