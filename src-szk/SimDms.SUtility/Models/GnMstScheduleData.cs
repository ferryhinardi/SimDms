using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.SUtility.Models
{
    [Table("GnMstScheduleData")]
    public class GnMstScheduleData
    {
        [Key]
        [Column(Order=1)]
        public string UniqueID { get; set; }
        public string CompanyCode { get; set; }
        public string DataType { get; set; }
        public int Segment { get; set; }
        public string Data { get; set; }
        public DateTime LastSendDate { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}