using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("GLInterface")]
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
}