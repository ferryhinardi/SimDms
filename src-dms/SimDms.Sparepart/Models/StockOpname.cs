using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sparepart.Models
{
    enum logStatus { Open = 0, Print = 1, Closed = 2, Deleted = 3 };
    public class SpSOSelectforInsert
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PartNo { get; set; }
      
        [Key]
        [Column(Order = 4)]
        public string WarehouseCode { get; set; }        
        public string LocationCode { get; set; }
        public string PartCategory { get; set; }
        public string ProductType { get; set; }
        public string TypeOfGoods { get; set; }
        public decimal? OnHand { get; set; }
    }

    public class sp_SpSOSelectforLookup
    {
        [Key]
        [Column(Order = 1)]
        public string STNo { get; set; }
        [Key]
        [Column(Order = 2)]
        public string WarehouseCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string STHdrNo { get; set; }

        [Key]
        [Column(Order = 4)]
        public string PartNO { get; set; }
        
    }


    public class SpTempRec
    {
        public string PartNo { get; set; }
        public string LocationCode { get; set; }
        public bool isMainLocation { get; set; }
        public decimal? OnHand { get; set; }
        public string MovingCode { get; set; }
        public int Flag { get; set; }

    }

    public class SpSOSelectforEntry
    {
        [Key]
        public string PartNo { get; set; }
        public string PartName { get; set; }
    }


}
