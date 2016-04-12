using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Absence.Models
{
    [Table("GnMstZipCode")]
    public class GnMstZipCode
    {
        [Key]
        [Column(Order=1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ZipCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string KelurahanDesa { get; set; }
        [Key]
        [Column(Order = 4)]
        public string KecamatanDistrik { get; set; }
        [Key]
        [Column(Order = 5)]
        public string KotaKabupaten { get; set; }
        [Key]
        [Column(Order = 6)]
        public string Ibukota { get; set; }
        public bool isCity { get; set; }
        public string Notes { get; set; }
    }
}