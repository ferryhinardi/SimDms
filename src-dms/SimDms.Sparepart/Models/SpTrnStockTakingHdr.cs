using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimDms.Sparepart.Models
{
    [Table("SpTrnStockTakingHdr")]
    public class SpTrnStockTakingHdr
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
        public string WarehouseCode { get; set; }
        public string TypeCode { get; set; }
        public string Condition { get; set; }
        public string Status { get; set; }
        public string TypeOfGoods { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
    }
}
