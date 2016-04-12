using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimDms.Sparepart.Models
{
    [Table("SpTrnStockTakingTemp")]
    public class SpTrnStockTakingTemp
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
        public string STNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal? SeqNo { get; set; }
        public string PartNo { get; set; }
        public string LocationCode { get; set; }
        public bool isMainLocation { get; set; }
        public string MovingCode { get; set; }
        public decimal? OnHandQty { get; set; }
        public DateTime? EntryDate { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }

        [NotMapped]
        public string Flag { get; set; }
      

    }


}

