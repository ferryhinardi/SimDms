using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SimDms.General.Models
{
    [Table("SysMessageBoards")]
    public class SysMessageBoard 
    {
        [Key]
        public int MessageID { get; set; }
        public string MessageHeader { get; set; }
        public string MessageText { get; set; }
        public string MessageTo { get; set; }
        public string MessageTarget { get; set; }
        public string MessageParams { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}