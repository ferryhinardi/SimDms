using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.CStatisfication.Models
{
    [Table("csTDayCall")]
    public class TDayCall
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CustomerCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string Chassis { get; set; }
        public string Engine { get; set; }
        public string CarTypeCode { get; set; }
        public string ColorCode { get; set; }
        public string PoliceRegNo { get; set; }
        public string SalesmanCode { get; set; }
        public string SalesmanName { get; set; }
        public DateTime BPKDate { get; set; }
        public DateTime STNKDate { get; set; }
        public bool? IsDeliveredA { get; set; }
        public bool? IsDeliveredB { get; set; }
        public bool? IsDeliveredC { get; set; }
        public bool? IsDeliveredD { get; set; }
        public bool? IsDeliveredE { get; set; }
        public string Comment { get; set; }
        public string Additional { get; set; }
        public int Status { get; set; }
    }
}