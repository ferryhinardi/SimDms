using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.SUtility.Models.Dcs
{
    [Table("gnDcsDownloadFile")]
    public class DcsDownloadClone
    {
        [Key]
        [Column("ID")]
        public decimal ID { get; set; }
        [Column("DataID")]
        public string DataID { get; set; }
        [Column("CustomerCode")]
        public string CustomerCode { get; set; }
        [Column("CreatedDate")]
        public DateTime? CreatedDate { get; set; }
        [Column("Status")]
        public string Status { get; set; }
        [Column("ProductType")]
        public string ProductType { get; set; }
        [Column("Contents")]
        public string Contents { get; set; }
        [Column("UpdatedDate")]
        public DateTime? UpdateDate{ get; set; }
        [Column("Header")]
        public string Header { get; set; }
    }
}