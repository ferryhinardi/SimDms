using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.SUtility.Models
{
    [Table("gnMstCustDealer")]
    public class GnMstCustDealer
    {
        [Key]
        [Column(Order= 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order= 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string SelectCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public int Year { get; set; }
        [Key]
        [Column(Order = 5)]
        public int Month { get; set; }
        public int NoOfUnitService { get; set; }
        public int NoOfUnit { get; set; }
        public int NoOfService { get; set; }
        public int NoOfSparePart { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}