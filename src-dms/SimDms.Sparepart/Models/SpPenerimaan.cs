using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Models
{
    /* Entry Claim Supplier  */
    public class SpEntryCS
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ClaimNo { get; set; }
        public DateTime? ClaimDate { get; set; }
        public string Status { get; set; }
    }

    [Table("spTrnPClaimHdr")]
    public class spTrnPClaimHdr
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ClaimNo { get; set; }
        public DateTime? ClaimDate { get; set; }
        public string WRSNo { get; set; }
        public DateTime? WRSDate { get; set; }
        public string TransType { get; set; }
        public string SupplierCode { get; set; }
        public string BinningNo { get; set; }
        public DateTime? BinningDate { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public string Status { get; set; }
        public decimal? PrintSeq { get; set; }
        public string TypeOfGoods { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }

    }

    [Table("SpTrnPRcvHdr")]
    public class SpTrnPRcvHdr
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string WRSNo { get; set; }
        public DateTime? WRSDate { get; set; }
        public string BinningNo { get; set; }
        public DateTime? BinningDate { get; set; }
        public string ReceivingType { get; set; }
        public string DNSupplierNo { get; set; }
        public DateTime? DNSupplierDate { get; set; }
        public string TransType { get; set; }
        public string SupplierCode { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public decimal? TotItem { get; set; }
        public decimal? TotWRSAmt { get; set; }
        public string Status { get; set; }
        public decimal? PrintSeq { get; set; }
        public string TypeOfGoods { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }


    }

    public class WRSNoBrowse
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string WRSNo { get; set; }
        public string TypeOfGoods { get; set; }
        public DateTime? WRSDate { get; set; }
        public int? CountPartNo { get; set; }
        public int CountClaim { get; set; }


    }

    public class SpNoPartView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public string DocNo { get; set; }
        public string WRSNo { get; set; }
        public decimal? ReceivedQty { get; set; }
        public decimal? PurchasePrice { get; set; }


    }

    public class SpWrongNoPartView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PartNo { get; set; }
        public string PartNoWrong { get; set; }
        public string PartName { get; set; }
        public string IsGenuinePart { get; set; }
        public string CategoryName { get; set; }
        public string PartCategory { get; set; }
        public decimal? OrderUnit { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string TypeOfGoods { get; set; }
        public string Status { get; set; }
        


    }

    [Table("SpGridPartNo")]
    public class SpGridPartNo
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ClaimNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string PartNo { get; set; }
        public string PartNoWrong { get; set; }
        public string ClaimType { get; set; }
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public decimal? SeqNo { get; set; }
        public decimal? OvertageQty { get; set; }
        public decimal? ReceivedQty { get; set; }
        public decimal? ShortageQty { get; set; }
        public decimal? DemageQty { get; set; }
        public decimal? WrongQty { get; set; }
        public decimal? TotClaimQty { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? CostPrice { get; set; }
        public string ReasonCode { get; set; }
        public string ReasonCodeStr { get; set; }
        public string Status { get; set; }
        public string ABCClass { get; set; }
        public string MovingCode { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }




    }

    [Table("SpTrnPClaimDtl")]
    public class SpTrnPClaimDtl
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ClaimNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string PartNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public decimal? SeqNo { get; set; }
        public string PartNoWrong { get; set; }
        public string ClaimType { get; set; }
        public decimal? OvertageQty { get; set; }
        public decimal? ShortageQty { get; set; }
        public decimal? DemageQty { get; set; }
        public decimal? WrongQty { get; set; }
        public decimal? TotClaimQty { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? CostPrice { get; set; }
        public string ReasonCode { get; set; }
        public decimal? ReceivedQty { get; set; }
        public string Status { get; set; }
        public string ABCClass { get; set; }
        public string MovingCode { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class SpTrnPClaimDtlViewModel
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string ClaimNo { get; set; }
        public string PartNo { get; set; }
        public decimal? OvertageQty { get; set; }
        public decimal? ShortageQty { get; set; }
        public decimal? DemageQty { get; set; }
        public decimal? WrongQty { get; set; }
        public string MovingCode { get; set; }
        public decimal? CostPrice { get; set; }
    }

    [Table("SpTrnPRcvDtl")]
    public class SpTrnPRcvDtl
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string WRSNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string PartNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public string WarehouseCode { get; set; }
        public string LocationCode { get; set; }
        public string BoxNo { get; set; }
        public decimal? ReceivedQty { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? DiscPct { get; set; }
        public string ABCClass { get; set; }
        public string MovingCode { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    /* Receiving Claim Vendor  */
    public class SpVendorClaimView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ClaimReceivedNo { get; set; }
        public string ClaimNo { get; set; }
        public DateTime? ClaimReceivedDate { get; set; }
        public DateTime? ClaimDate { get; set; }
        public string TypeOfGoods { get; set; }
        public string Status { get; set; }

    }

    public class SpRecClaimNo
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ClaimNo { get; set; }
        public DateTime? ClaimDate { get; set; }
        public string TypeOfGoods { get; set; }
        
    }

    [Table("SpTrnPRcvClaimDtl")]
    public class SpTrnPRcvClaimDtl
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ClaimReceivedNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string ClaimNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public string PartNo { get; set; }
        [Key]
        [Column(Order = 6)]
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public string PartNoWrong { get; set; }
        public string ClaimType { get; set; }
        public decimal? RcvOvertageQty { get; set; }
        public decimal? RcvShortageQty { get; set; }
        public decimal? RcvDamageQty { get; set; }
        public decimal? RcvWrongQty { get; set; }
        public decimal? TotRcvClaimQty { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? CostPrice { get; set; }
        public string Status { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public string ABCClass { get; set; }
        public string MovingCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }

    }

    public class SpPartOrderView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public string DocNo { get; set; }
        public string ClaimNo { get; set; }

    }

    [Table("SpTrnPRcvClaimHdr")]
    public class SpTrnPRcvClaimHdr
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ClaimReceivedNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string ClaimNo { get; set; }
        public DateTime? ClaimReceivedDate { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public string SupplierCode { get; set; }
        public DateTime? ClaimDate { get; set; }
        public string TypeOfGoods { get; set; }
        public string Status { get; set; }
        public int? PrintSeq { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }


    }

    [Table("SpLoadDetail_TranStock")]
    public class SpLoadDetail_TranStock
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string LampiranNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string DealerCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public long? NoUrut { get; set; }
        public string DocNo { get; set; }
        public string PartNo { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? DiscPct { get; set; }
        public decimal? ReceivedQty { get; set; }
        public string BoxNo { get; set; }
        public string NmPart { get; set; }
    }

    /* Receiving Claim Vendor  */
    public class EntryHPPBrowse
    {
        public String CompanyCode { get; set; }
        public String BranchCode { get; set; }
        public String HPPNo { get; set; }
        public DateTime? HPPDate { get; set; }
        public String WRSNo { get; set; }
        public DateTime? WRSDate { get; set; }
        public String ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public Decimal? TotPurchAmt { get; set; }
        public Decimal? TotNetPurchAmt { get; set; }
        public Decimal? TotTaxAmt { get; set; }
        public String TaxNo { get; set; }
        public DateTime? TaxDate { get; set; }
        public Decimal? MonthTax { get; set; }
        public Decimal? YearTax { get; set; }
        public DateTime? DueDate { get; set; }
        public Decimal? DiffNetPurchAmt { get; set; }
        public Decimal? DiffTaxAmt { get; set; }
        public Decimal? TotHPPAmt { get; set; }
        public Decimal? CostPrice { get; set; }
        public Decimal? PrintSeq { get; set; }
        public String TypeOfGoods { get; set; }
        public String Status { get; set; }
        public String CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public String DNSupplierNo { get; set; }
        public String SupplierName { get; set; }
        public String BinningNo { get; set; }
        public DateTime? BinningDate { get; set; }
        public String SupplierCode { get; set; }
        public String StatusStr { get; set; }
    }

    public class SpWRSHpp
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string WRSNo { get; set; }
        public DateTime? WRSDate { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public string DNSupplierNo { get; set; }
        public string SupplierName { get; set; }
        public string TypeOfGoods { get; set; }
    }

    [Table("SpTrnPHPP")]
    public class SpTrnPHPP
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string HPPNo { get; set; }
        public DateTime? HPPDate { get; set; }
        public string WRSNo { get; set; }
        public DateTime? WRSDate { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public decimal? TotPurchAmt { get; set; }
        public decimal? TotNetPurchAmt { get; set; }
        public decimal? TotTaxAmt { get; set; }
        public string TaxNo { get; set; }
        public DateTime? TaxDate { get; set; }
        public decimal? MonthTax { get; set; }
        public decimal? YearTax { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? DiffNetPurchAmt { get; set; }
        public decimal? DiffTaxAmt { get; set; }
        public decimal? TotHPPAmt { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? PrintSeq { get; set; }
        public string TypeOfGoods { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    [Table("SpLoadEntryHPP")]
    public class SpLoadEntryHPP
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string HPPNo { get; set; }
        public DateTime? HPPDate { get; set; }
        public string WRSNo { get; set; }
        public DateTime? WRSDate { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public decimal? TotPurchAmt { get; set; }
        public decimal? TotNetPurchAmt { get; set; }
        public decimal? TotTaxAmt { get; set; }
        public string TaxNo { get; set; }
        public DateTime? TaxDate { get; set; }
        public decimal? MonthTax { get; set; }
        public decimal? YearTax { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? DiffNetPurchAmt { get; set; }
        public decimal? DiffTaxAmt { get; set; }
        public decimal? TotHPPAmt { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? PrintSeq { get; set; }
        public string TypeOfGoods { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string DNSupplierNo { get; set; }
        public string SupplierName { get; set; }
        public string BinningNo { get; set; }
        public DateTime? BinningDate { get; set; }
        public string SupplierCode { get; set; }
        public string StatusStr { get; set; }
    }

    //[Table("gnMstSupplierClass")]
    //public class gnMstSupplierC
    //{
    //    [Key]
    //    [Column(Order = 1)]
    //    public string CompanyCode { get; set; }
    //    [Key]
    //    [Column(Order = 2)]
    //    public string BranchCode { get; set; }
    //    [Key]
    //    [Column(Order = 3)]
    //    public string SupplierClass { get; set; }
    //    [Key]
    //    [Column(Order = 4)]
    //    public string TypeOfGoods { get; set; }
    //    public string ProfitCenterCode { get; set; }
    //    public string SupplierClassName { get; set; }
    //    public string PayableAccNo { get; set; }
    //    public string DownPaymentAccNo { get; set; }
    //    public string InterestAccNo { get; set; }
    //    public string OtherAccNo { get; set; }
    //    public string TaxInAccNo { get; set; }
    //    public string WitholdingTaxAccNo { get; set; }
    //    public string CreatedBy { get; set; }
    //    public DateTime? CreatedDate { get; set; }
    //    public string LastUpdateBy { get; set; }
    //    public DateTime? LastUpdateDate { get; set; }
    //    public bool? isLocked { get; set; }
    //    public string LockingBy { get; set; }
    //    public DateTime? LockingDate { get; set; }
    //}

    [Table("ApInterface")]
    public class ApInterface
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DocNo { get; set; }
        public string ProfitCenterCode { get; set; }
        public DateTime? DocDate { get; set; }
        public string Reference { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public decimal? NetAmt { get; set; }
        public decimal? PPHAmt { get; set; }
        public string SupplierCode { get; set; }
        public decimal? PPNAmt { get; set; }
        public decimal? PPnBM { get; set; }
        public string AccountNo { get; set; }
        public DateTime? TermsDate { get; set; }
        public string TermsName { get; set; }
        public decimal? TotalAmt { get; set; }
        public string StatusFlag { get; set; }
        public string CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public decimal? ReceiveAmt { get; set; }
        public string FakturPajakNo { get; set; }
        public DateTime? FakturPajakDate { get; set; }
        public string RefNo { get; set; }
    }

    

    public class SaveHPP {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string HPPNo { get; set; }
        public DateTime? HPPDate { get; set; }
        public string WRSNo { get; set; }
        public DateTime? WRSDate { get; set; }
        public string ReferenceNo { get; set; }
        public string SupplierCode { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public decimal? TotPurchAmt { get; set; }
        public decimal? TotNetPurchAmt { get; set; }
        public decimal? TotTaxAmt { get; set; }
        public string TaxNo { get; set; }
        public DateTime? TaxDate { get; set; }
        public decimal? MonthTax { get; set; }
        public decimal? YearTax { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? DiffNetPurchAmt { get; set; }
        public decimal? DiffTaxAmt { get; set; }
        public decimal? TotHPPAmt { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? PrintSeq { get; set; }
        public string TypeOfGoods { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    /* Entry Penerimaan Persediaan(WRS) */
    [Table("SpGridEntryWRS")]
    public class GridEntryWRS
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DocNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string PartNo { get; set; }
        public decimal? PurchasePrice { get; set; }
        public string WRSNo { get; set; }
        public decimal? DiscPct { get; set; }
        public decimal? ReceivedQty { get; set; }
        public string BoxNo { get; set; }
        public string NmPart { get; set; }
    }
    
    public class TotalQty
    {
        public decimal ReceivedQty { get; set; }
        public string LookUpValueName { get; set; }
    }

    [Table("SpTrnPBinnDtl")]
    public class SpTrnPBinnDtl
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BinningNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string PartNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public string WarehouseCode { get; set; }
        public string LocationCode { get; set; }
        [Key]
        [Column(Order = 6)]
        public string BoxNo { get; set; }
        public decimal? ReceivedQty { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? DiscPct { get; set; }
        public string ABCClass { get; set; }
        public string MovingCode { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class SaveSpTrnPBinnDtl
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string BinningNo { get; set; }
        public string PartNo { get; set; }
        public string DocNo { get; set; }
        public decimal TotItem { get; set; }
        public decimal TotBinningAmt { get; set; }
        public DateTime? DocDate { get; set; }
        public string WarehouseCode { get; set; }
        public string LocationCode { get; set; }
        public string BoxNo { get; set; }
        public decimal? ReceivedQty { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? DiscPct { get; set; }
        public string ABCClass { get; set; }
        public string MovingCode { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    /* Entry Draft Penerimaan (Binning) */
    [Table("SpEdpDetail")]
    public class SpEdpDetail
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BinningNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public long? NoUrut { get; set; }
        public string DocNo { get; set; }
        public string PartNo { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? DiscPct { get; set; }
        public decimal? ReceivedQty { get; set; }
        public string BoxNo { get; set; }
        public string NmPart { get; set; }
    }

    public class TotalItem
    {
        public int TotItem { get; set; }
        public decimal TotBinningAmt { get; set; }
    }

    public class SpEdpSupplier
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string Alamat { get; set; }
        public decimal? Diskon { get; set; }
        public string StatusStr { get; set; }
        public string Status { get; set; }
        public string ProfitCenterCode { get; set; }
        public string ProfitCenterCodeStr { get; set; }
        public bool? isBlackList { get; set; }
    }

    public class EdpPelangganBrowse
    {
        public String CustomerCode { get; set; }
        public String CustomerName { get; set; }
        public String Address { get; set; }
        public String ProfitCenter { get; set; }
    }

    public class SpEdpDnNo
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string DeliveryNo { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
    }

    public class EdpTransNo
    {
        public String LampiranNo { get; set; }
        public String SupplierCode { get; set; }
        public String SupplierName { get; set; }
        public String TypeofGoods { get; set; }
    }

    public class EdpDocNo
    {
        public String POSNo { get; set; }
        public DateTime? PosDate { get; set; }
        public String SupplierCode { get; set; }
    }

    public class EdpBpsNo
    {
        public String BPSFNo { get; set; }
        public DateTime? BPSFDate { get; set; }
        public String PickingSlipNo { get; set; }
        public DateTime? PickingSlipDate { get; set; }
    }

    public class PINVDData
    {
        public String CompanyCode { get; set; }
        public String BranchCode { get; set; }
        public String DocNo { get; set; }
        public String DeliveryNo { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public String InvoiceNo { get; set; }
        public String PartNo { get; set; }
        public String NmPart { get; set; }
        public Decimal? PurchasePrice { get; set; }
        public Decimal? DiscPct { get; set; }
        public Decimal? ReceivedQty { get; set; }
        public String BoxNo { get; set; }
        public String SupplierCode { get; set; }
        public Decimal? CostPrice { get; set; }
        public String ProductType { get; set; }
    }

    public class SpEdpPartNo
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? QtyBill { get; set; }
        public string CustomerCode { get; set; }
        public string BPSFNo { get; set; }
    }

    public class SpEdpPartNo_Pembelian
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal? DiscPct { get; set; }
        public decimal? MaxReceived { get; set; }
        public decimal? ReminQty { get; set; }
        public string POSNo { get; set; }
    }

    public class SpEdpPartNo_Internal
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string ProductType { get; set; }
        public string TypeOfGoods { get; set; }
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public decimal? Available { get; set; }
        public string MovingCode { get; set; }
    }

    public class SpEdpPartNo_Others
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string SupplierCode { get; set; }
        public string ProductType { get; set; }
        public string TypeOfGoods { get; set; }
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public decimal? Available { get; set; }
        public string MovingCode { get; set; }
    }

    public class QueryCloseWrs
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string WRSNo { get; set; }
        public string PartNo { get; set; }
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public string WarehouseCode { get; set; }
        public string LocationCode { get; set; }
        public string BoxNo { get; set; }
        public decimal? ReceivedQty { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? DiscPct { get; set; }
        public string ABCClass { get; set; }
        public string MovingCode { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string NmPart { get; set; }
    }

    public class QueryCloseWrs1
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string PartNo { get; set; }
        public string MovingCode { get; set; }
        public decimal? DemandAverage { get; set; }
        public DateTime? BornDate { get; set; }
        public string ABCClass { get; set; }
        public DateTime? LastDemandDate { get; set; }
        public DateTime? LastPurchaseDate { get; set; }
        public DateTime? LastSalesDate { get; set; }
        public decimal? BOMInvAmt { get; set; }
        public decimal? BOMInvQty { get; set; }
        public decimal? BOMInvCostPrice { get; set; }
        public decimal? OnOrder { get; set; }
        public decimal? InTransit { get; set; }
        public decimal? OnHand { get; set; }
        public decimal? AllocationSP { get; set; }
        public decimal? AllocationSR { get; set; }
        public decimal? AllocationSL { get; set; }
        public decimal? BackOrderSP { get; set; }
        public decimal? BackOrderSR { get; set; }
        public decimal? BackOrderSL { get; set; }
        public decimal? ReservedSP { get; set; }
        public decimal? ReservedSR { get; set; }
        public decimal? ReservedSL { get; set; }
        public decimal? BorrowQty { get; set; }
        public decimal? BorrowedQty { get; set; }
        public decimal? SalesUnit { get; set; }
        public decimal? OrderUnit { get; set; }
        public decimal? OrderPointQty { get; set; }
        public decimal? SafetyStockQty { get; set; }
        public decimal? LeadTime { get; set; }
        public decimal? OrderCycle { get; set; }
        public decimal? SafetyStock { get; set; }
        public string Utility1 { get; set; }
        public string Utility2 { get; set; }
        public string Utility3 { get; set; }
        public string Utility4 { get; set; }
        public string TypeOfGoods { get; set; }
        public string Status { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public decimal PurcDiscPct { get; set; }
        public String SupplierCode { get; set; }
        public decimal CostPrice { get; set; }
    }

    public class QueryCloseWrs2
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string PartNo { get; set; }
        public DateTime UpdateDate { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? RetailPriceInclTax { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal? OldRetailPrice { get; set; }
        public decimal? OldPurchasePrice { get; set; }
        public decimal? OldCostPirce { get; set; }
        public decimal? OldDiscount { get; set; }
        public DateTime? LastPurchaseUpdate { get; set; }
        public DateTime? LastRetailPriceUpdate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string StatusMaintain { get; set; }
    }

    public class COGSLev 
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string WRSNo { get; set; }
        public decimal COGS { get; set; }
        public string DocNo { get; set; }
        public string journalNo { get; set; }
        public decimal OnOrder { get; set; }
    }

    public class FISTCAL
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public decimal FiscalYear { get; set; }
        public decimal FiscalMonth { get; set; }
        public string Periode { get; set; }
        public decimal PeriodeNum { get; set; }
        public string PeriodeName { get; set; }
    }

    [Table("gnMstAccount")]
    public class gnMstAccount
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string AccountNo { get; set; }
        public string Description { get; set; }
        public string AccountType { get; set; }
        public string Company { get; set; }
        public string Branch { get; set; }
        public string CostCtrCode { get; set; }
        public string ProductType { get; set; }
        public string NaturalAccount { get; set; }
        public string InterCompany { get; set; }
        public string Futureuse { get; set; }
        public string Consol { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Balance { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    [Table("SpMstCompanyAccountDtl")]
    public class SpMstCompanyAccountDtl
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CompanyCodeTo { get; set; }
        [Key]
        [Column(Order = 3)]
        public string TPGO { get; set; }
        public string InterCompanyAccNoTo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    [Table("SpSelectByNoBinning")]
    public class SpSelectByNoBinning
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BinningNo { get; set; }
        public long? NoUrut { get; set; }
        public string DocNo { get; set; }
        public string PartNo { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? DiscPct { get; set; }
        public decimal? ReceivedQty { get; set; }
        public string BoxNo { get; set; }
        public string NmPart { get; set; }
    }

    [Table("SpSelectByNoWRS")]
    public class SpSelectByNoWRS
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string WRSNo { get; set; }
        public string PartNo { get; set; }
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public string WarehouseCode { get; set; }
        public string LocationCode { get; set; }
        public string BoxNo { get; set; }
        public decimal? ReceivedQty { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? DiscPct { get; set; }
        public string ABCClass { get; set; }
        public string MovingCode { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    
}
