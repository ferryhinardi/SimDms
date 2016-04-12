using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.SUtility.Models.Dcs
{
    [Table("DealerInfo")]
    public class DealerInfo
    {
        [Key]
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string ProductType { get; set; }
        public string ShortName { get; set; }
    }
}