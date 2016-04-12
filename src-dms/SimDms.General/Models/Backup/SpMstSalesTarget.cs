using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sparepart.Models
{
    [Table("SpMstSalesTarget")]
    public class SpMstSalesTarget
    {
        [Key]
        [Column(Order=1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public Decimal Year { get; set; }
        [Key]
        [Column(Order = 4)]
        public Decimal Month { get; set; }
        public Decimal QtyTarget { get; set; }
        public Decimal AmountTarget { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public Decimal TotalJaringan { get; set; }
    }
}
