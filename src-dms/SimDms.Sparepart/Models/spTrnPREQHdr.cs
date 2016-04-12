using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Models
{
    [Table("spTrnPREQHdr")]
    public class spTrnPREQHdr
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string REQNo { get; set; }
        public DateTime? REQDate { get; set; }
        public string SupplierCode { get; set; }
        public string Remark { get; set; }
        public decimal? PrintSeq { get; set; }
        public string Status { get; set; }
        public string ProcessFlag { get; set; }
        public bool? isDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class spTrnPREQHdrView
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string REQNo { get; set; }
        public DateTime? REQDate { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string Remark { get; set; }
        public decimal? PrintSeq { get; set; }
        public string Status { get; set; }
        public string StatusDesc { get; set; }
        public string ProcessFlag { get; set; }
        public bool? isDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}
