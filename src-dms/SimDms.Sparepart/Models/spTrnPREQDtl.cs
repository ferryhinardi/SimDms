using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace SimDms.Sparepart.Models
{
    [Table("spTrnPREQDtl")]
    public class spTrnPREQDtl
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string REQNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string PartNo { get; set; }
        public decimal SeqNo { get; set; }
        public decimal? OrderQty { get; set; }
        public string ABCClass { get; set; }
        public string MovingCode { get; set; }
        public string ProductType { get; set; }
        public string TypeOfGoods { get; set; }
        public string PartCategory { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string Note { get; set; }
    }

    public class SpTrnPREQDtlView
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string REQNo { get; set; }
        public string PartNo { get; set; }
        public decimal SeqNo { get; set; }
        public decimal? OrderQty { get; set; }
        public string ABCClass { get; set; }
        public string MovingCode { get; set; }
        public string ProductType { get; set; }
        public string TypeOfGoods { get; set; }
        public string PartCategory { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string Note { get; set; }
        public string PartName { get; set; }
    }
}
