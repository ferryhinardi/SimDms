using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sparepart.Models
{
    [Table("spHstSparepartWeekly")]
    public class SparepartWeekly
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public decimal PeriodYear { get; set; }
        [Key]
        [Column(Order = 4)]
        public decimal PeriodMonth { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal PeriodWeek { get; set; }
        [Key]
        [Column(Order = 6)]
        public string TypeOfGoods { get; set; }
        public decimal Netto_WS { get; set; }
        public decimal HPP_WS { get; set; }
        public decimal Netto_PS { get; set; }
        public decimal HPP_PS { get; set; }
        public decimal Netto_C { get; set; }
        public decimal HPP_C { get; set; }
        public decimal NilaiStock { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class SparepartWeeklyGrid
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public decimal PeriodYear { get; set; }
        public decimal PeriodMonth { get; set; }
        public decimal PeriodWeek { get; set; }
        public string TypeOfGoods { get; set; }
        public decimal Netto_WS { get; set; }
        public decimal HPP_WS { get; set; }
        public decimal Netto_PS { get; set; }
        public decimal HPP_PS { get; set; }
        public decimal Netto_C { get; set; }
        public decimal HPP_C { get; set; }
        public decimal NilaiStock { get; set; }
    }
}
