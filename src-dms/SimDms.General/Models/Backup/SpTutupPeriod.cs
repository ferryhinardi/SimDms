using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Models
{
    [Table("SpClosePeriodPending")]
    public class SpClosePeriodPending
    {
        [Key]
        [Column(Order = 1)]
        public string DocumentNo { get; set; }
        public int FiscalYear { get; set; }
        public int FiscalMonth { get; set; }
        public string Status { get; set; }
        public string ProfitCenter { get; set; }
        public string TableName { get; set; }
        public string TipePart { get; set; }
    }

    public class QueryTutupPeriod1
    {
        public string PartNo { get; set; }
        public string NewPartNo { get; set; }
        public decimal? DemandAverage { get; set; }
    }

    public class QueryTutupPeriod2
    {
        public decimal FiscalYear { get; set; }
        public decimal FiscalMonth { get; set; }
        public decimal PeriodeNum { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
