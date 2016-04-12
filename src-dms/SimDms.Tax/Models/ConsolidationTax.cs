using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Tax.Models
{
    public class TaxBrowse
    {
        public decimal PeriodYear { get; set; }
        public decimal PeriodMonth { get; set; }
    }

    public class CompanyBrowse
    {
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
    }

    public class BranchBrowse
    {
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
    }

    public class ConsolidationLookup
    {
        public string LookUpValue { get; set; }
        public string LookUpValueName { get; set; }
    }

    public class SupplierBrowse
    {
        public string SupplierCode { get; set; }
        public string SupplierGovName { get; set; }
        public string Address { get; set; }
        public decimal? Discount { get; set; }
        public string Status { get; set; }
        public string ProfitCenter { get; set; }
        public string NPWPNo { get; set; }
        public bool? IsPKP { get; set; }
    }

    public class CustomerBrowse
    {
        public string CustomerCode { get; set; }
        public string CustomerGovName { get; set; }
        public string Address { get; set; }
        public string ProfitCenter { get; set; }
        public string NPWPNo { get; set; }
        public bool? IsPKP { get; set; }
    }

    public class GetTaxIn
    {
        public long? RowNumber { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string ProductType { get; set; }
        public string ProductDesc { get; set; }
        public decimal PeriodYear { get; set; }
        public decimal PeriodMonth { get; set; }
        public decimal SeqNo { get; set; }
        public string ProfitCenter { get; set; }
        public string TypeOfGoods { get; set; }
        public string TypeOfGoodsDesc { get; set; }
        public string TaxCode { get; set; }
        public string TaxDesc { get; set; }
        public string TransactionCode { get; set; }
        public string TransactionDesc { get; set; }
        public string StatusCode { get; set; }
        public string StatusDesc { get; set; }
        public string DocumentCode { get; set; }
        public string DocumentDesc { get; set; }
        public string DocumentType { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public bool? IsPKP { get; set; }
        public string NPWP { get; set; }
        public string FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public string TaxNo { get; set; }
        public DateTime? TaxDate { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public decimal? DPPAmt { get; set; }
        public decimal? PPNAmt { get; set; }
        public decimal? PPNBmAmt { get; set; }
        public string Description { get; set; }
        public decimal? Quantity { get; set; }
        public bool? RePosting { get; set; }
    }

    public class GetTaxOut
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string ProductType { get; set; }
        public decimal PeriodYear { get; set; }
        public decimal PeriodMonth { get; set; }
        public decimal SeqNo { get; set; }
        public string ProfitCenter { get; set; }
        public string TypeOfGoods { get; set; }
        public string TaxCode { get; set; }
        public string TransactionCode { get; set; }
        public string StatusCode { get; set; }
        public string DocumentCode { get; set; }
        public string DocumentType { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public bool? IsPKP { get; set; }
        public string NPWP { get; set; }
        public string FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public string TaxNo { get; set; }
        public DateTime? TaxDate { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public decimal? DPPAmt { get; set; }
        public decimal? PPNAmt { get; set; }
        public decimal? PPNBmAmt { get; set; }
        public string Description { get; set; }
        public decimal? Quantity { get; set; }
        public string SKPNo { get; set; }
        public bool? RePosting { get; set; }
    }

    public class GetGrandTotal
    {
        public decimal DPPAmt { get; set; }
        public decimal PPNAmt { get; set; }
        public decimal? SumDPPStd { get; set; }
        public decimal? SumDPPSdh { get; set; }
        public decimal? SumPPNStd { get; set; }
        public decimal? SumPPNSdh { get; set; }
        public decimal SumPPnBMAmt { get; set; }
    }

    public class ConsolidationTaxNewRecord
    {
        public string BranchCode { get; set; }
        public string ProfitCenter { get; set; }
        public string DocumentType { get; set; }
    }
}