using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sparepart.Models
{
    [Table("glInterface")]
    public class GLInterface
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
        public decimal SeqNo { get; set; }
        public DateTime? DocDate { get; set; }
        public string ProfitCenterCode { get; set; }
        public DateTime? AccDate { get; set; }
        public string AccountNo { get; set; }
        public string JournalCode { get; set; }
        public string TypeJournal { get; set; }
        public string ApplyTo { get; set; }
        public decimal? AmountDb { get; set; }
        public decimal? AmountCr { get; set; }
        public string TypeTrans { get; set; }
        public string BatchNo { get; set; }
        public DateTime? BatchDate { get; set; }
        public string StatusFlag { get; set; }
        public string CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }

    }

    [Table("glJournal")]
    public class glJournal
    {
    [Key]
    [Column(Order = 1)]
    public string CompanyCode { get; set; }
    [Key]
    [Column(Order = 2)]
    public string BranchCode { get; set; }
    [Key]
    [Column(Order = 3)]
    public decimal FiscalYear { get; set; }
    public string ProfitCenterCode { get; set; }
    [Key]
    [Column(Order = 5)]
    public string JournalNo { get; set; }
    public string JournalType { get; set; }
    public DateTime? JournalDate { get; set; }
    public string DocSource { get; set; }
    public string ReffNo { get; set; }
    public DateTime? ReffDate { get; set; }
    public decimal? FiscalMonth { get; set; }
    public decimal? PeriodeNum { get; set; }
    public string Periode { get; set; }
    public string PeriodeName { get; set; }
    public DateTime? GLDate { get; set; }
    public string BalanceType { get; set; }
    public decimal? amountDb {get; set;}
    public decimal? amountCr {get; set;}
    public string Status { get; set; }
    public string StatusRecon { get; set; }
    public string BatchNo { get; set; }
    public DateTime? PostingDate { get; set; }
    public string StatusReverse { get; set; }
    public DateTime? ReverseDate { get; set; }
    public decimal? PrintSeq { get; set; }
    public bool? FSend { get; set; }
    public string SendBy { get; set; }
    public DateTime? SendDate { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string LastUpdateBy { get; set; }
    public DateTime? LastUpdateDate { get; set; }
    }

    [Table("GlJournalDtl")]
    public class GlJournalDtl
    {
    [Key]
    [Column(Order = 1)]
    public string CompanyCode { get; set; }
    [Key]
    [Column(Order = 2)]
    public string BranchCode { get; set; }
    [Key]
    [Column(Order = 3)]
    public decimal FiscalYear { get; set; }
    [Key]
    [Column(Order = 4)]
    public string JournalNo { get; set; }
    [Key]
    [Column(Order = 5)]
    public decimal SeqNo { get; set; }
    public string AccountNo { get; set; }
    public string Description { get; set; }
    public string JournalType { get; set; }
    public decimal? amountDb { get; set; }
    public decimal? amountCr { get; set; }
    public string TypeTrans { get; set; }
    public string AccountType { get; set; }
    public string DocNo { get; set; }
    public string StatusReverse { get; set; }
    public DateTime? ReverseDate { get; set; }
    public bool? FSend { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    }
}
