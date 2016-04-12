using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    public class ProsesStockTaking
    {
        public DateTime STDate { get; set; }
        public string STHdrNo { get; set; }
        public string STHdrNoTo { get; set; }
        public string WHCode { get; set; }
        public string WHDesc { get; set; }
        public string WHCodeTo { get; set; }
        public string WHDescTo { get; set; }
        public string SalesModelCode { get; set; }
        public string SalesModelDesc { get; set; }
        public string SalesModelCodeTo { get; set; }
        public string SalesModelDescTo { get; set; }
        public string SalesModelYear { get; set; }
        public string SalesModelYearTo { get; set; }
        public string ColorCode { get; set; }
        public string ColorDesc { get; set; }
        public string ColorCodeTo { get; set; }
        public string ColorDescTo { get; set; }
        public bool isColor { get; set; }
        public bool isModel { get; set; }
        public bool isWH { get; set; }
        public bool isYear { get; set; }
    }

    [Table("OmTrStockTakingHdr")]
    public class omTrStockTakingHdr
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string STHdrNo { get; set; }
        public DateTime? STDate { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    [Table("OmTrStockTakingDtl")]
    public class omTrStockTakingDtl
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string STHdrNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public int STNo { get; set; }
        public string WareHouseCode { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string ColourCode { get; set; }
        public DateTime? EntryDate { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string Status2 { get; set; }
        public int PrintSeq { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class omTrStockTakingDtlProcess
    {
        public string STHdrNo { get; set; }
        public int STNo { get; set; }
        public int Status { get; set; }
    }

    public class EntryInvTag
    {
        [Key]
        public string STHdrNo { get; set; }
        public DateTime? STDate { get; set; }
        public string Status { get; set; }
    }

    public class PostingStokTaking
    {
        [Key]
        public string STHdrNo { get; set; }
        public string STHdrNoTo { get; set; }
    }

    public class gnMstPeriode
    {
        public string Periode { get; set; }
        public decimal? FiscalMonth { get; set; }
        public decimal? PeriodeNum { get; set; }
        public string PeriodeName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
    }
}