using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("msModelMappingHdr")]
    public class MsModelMappingHdr
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string GroupCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string TypeCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string TransmissionType { get; set; }
        public decimal? GroupCodeSeq { get; set; }
        public decimal? TypeCodeSeq { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public bool? IsSelected { get; set; }
    }

    [Table("msModelMappingDtl")]
    public class MsModelMappingDtl
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string GroupCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string TypeCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string TransmissionType { get; set; }
        [Key]
        [Column(Order = 5)]
        public string SalesModelCode { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}