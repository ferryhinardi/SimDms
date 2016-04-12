using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("qa2MstRefferenceChar")]
    public class qa2MstRefferenceCharModel
    {
        [Key]
        [Column(Order = 1)]
        public string Event { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductSourceCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string RefferenceType { get; set; }
        [Key]
        [Column(Order = 4)]
        public string RefferenceCode { get; set; }

        public string RefferenceDescI { get; set; }
        public string RefferenceDescE { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}