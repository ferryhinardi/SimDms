using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Models
{
    [Table("spHstBOCancel")]
    public class spHstBOCancels
    {
        [Key]
        [Column(Order = 1)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public decimal Month { get; set; }
        [Key]
        [Column(Order = 4)]
        public decimal Year { get; set; }
        [Key]
        [Column(Order = 5)]
        public string BOCancelNo { get; set; }
        [Key]
        [Column(Order = 6)]
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        [Key]
        [Column(Order = 8)]
        public string PartNo { get; set; }
        public string WarehouseCode { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public string MovingCode { get; set; }
        public string ABCClass { get; set; }
        public decimal? BOOutstanding { get; set; }
        public decimal? BOCancel { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Note { get; set; }
    }   

    public class spHstBOCancel
    {
        [Key]
        [Column(Order = 1)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public decimal Month { get; set; }
        [Key]
        [Column(Order = 4)]
        public decimal Year { get; set; }
        [Key]
        [Column(Order = 5)]
        public string BOCancelNo { get; set; }
        [Key]
        [Column(Order = 6)]
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        [Key]
        [Column(Order = 8)]
        public string PartNo { get; set; }
        public string WarehouseCode { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public string MovingCode { get; set; }
        public string ABCClass { get; set; }
        public decimal? BOOutstanding { get; set; }
        public decimal? BOCancel { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Note { get; set; }
    }   
}
