using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SimDms.Common.Models
{
    [Table("GnDcsUploadFile")]
    public class GnDcsUploadFile
    {
        [Key]
        [Column(Order = 1)]
        public decimal ID { get; set; }
        public string DataID { get; set; }
        public string CustomerCode { get; set; }
        public string ProductType { get; set; }
        public string Contents { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Header { get; set; }
    }
}
