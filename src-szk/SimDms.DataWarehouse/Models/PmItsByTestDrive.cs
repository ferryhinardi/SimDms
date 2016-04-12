using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("PmItsByTestDrive")]
    public class PmItsByTestDrive
    {
        [Key]
        [Column(Order = 1)]
        public string Area { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public long InquiryNumber { get; set; }
        public DateTime? InquiryDate { get; set; }
        public string DealerAbbreviation { get; set; }
        public string OutletAbbreviation { get; set; }

        public DateTime? SPKDate { get; set; }
        public string TipeKendaraan { get; set; }
        public string Variant { get; set; }
        public string ColourCode { get; set; }
        public string Transmisi { get; set; }
        public string LastProgress { get; set; }
        public int Inq { get; set; }
        public int InqTestDrive { get; set; }
        public int OutsSPK { get; set; }
        public int OutsSPKTestDrive { get; set; }
        public int NewSPK { get; set; }
        public int NewSPKTestDrive { get; set; }
        public int TotalSPK { get; set; }
        public int TotalSPKTestDrive { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUpdateBy { get; set; }

    }

    public class PmItsByTestDrive2
    {
        public string Area { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public long InquiryNumber { get; set; }
        public DateTime? InquiryDate { get; set; }
        public string DealerAbbreviation { get; set; }
        public string OutletAbbreviation { get; set; }
        public DateTime? SPKDate { get; set; }
        public string TipeKendaraan { get; set; }
        public string Variant { get; set; }
        public string ColourCode { get; set; }
        public string Transmisi { get; set; }
        public int Inq { get; set; }
        public int InqTestDrive { get; set; }
        public int OutsSPK { get; set; }
        public int OutsSPKTestDrive { get; set; }
        public int NewSPK { get; set; }
        public int NewSPKTestDrive { get; set; }
        public int TotalSPK { get; set; }
        public int TotalSPKTestDrive { get; set; }
    }

    public class PmItsByTestDrive3
    {
        public string Area { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public long InquiryNumber { get; set; }
        public DateTime? InquiryDate { get; set; }
        public string DealerAbbreviation { get; set; }
        public string OutletAbbreviation { get; set; }
        public DateTime? SPKDate { get; set; }
        public string TipeKendaraan { get; set; }
        public string Variant { get; set; }
        public string ColourCode { get; set; }
        public string Transmisi { get; set; }
        public string LastProgress { get; set; }
        public int Inq { get; set; }
        public int InqTestDrive { get; set; }
        public int OutsSPK { get; set; }
        public int OutsSPKTestDrive { get; set; }
        public int NewSPK { get; set; }
        public int NewSPKTestDrive { get; set; }
        public int TotalSPK { get; set; }
        public int TotalSPKTestDrive { get; set; }
    }
}