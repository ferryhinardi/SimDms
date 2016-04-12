using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.PreSales.Models
{
    [Table("MsMstGroupModel")]
    public class GroupModels
    {
        [Key]
        [Column(Order = 1)]
        public string GroupModel { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ModelType { get; set; }
        public bool Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }

    public class GroupModelsView
    {
        public int SeqNO { get; set; }
        public string GroupModel { get; set; }
    }

    public class TipeKendaraanView
    {
        public int SeqNO { get; set; }
        public string TipeKendaraan { get; set; }
        public string Variant { get; set; }
    }
}