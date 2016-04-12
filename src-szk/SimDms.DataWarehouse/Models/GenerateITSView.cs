using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.DataWarehouse.Models
{
    [Table("GenerateITSView")]
    public class GenerateITSView 
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public long InquiryNumber { get; set; }
        public DateTime InquiryDate { get; set; }
        public string Area { get; set; }
        public string DealerName { get; set; }
        public string OutletAbbreviation { get; set; }
        public string TipeKendaraan { get; set; }
        public string Variant { get; set; }
        public string Transmisi { get; set; }
        public string ColourCode { get; set; }
        public string Wiraniaga { get; set; }
        public string SalesCoordinator { get; set; }
        public string SalesHead { get; set; }
        public string LastProgress { get; set; }
        public string LostCaseCategory { get; set; }
        public string LostCaseReasonID { get; set; }
        public string SPKDate { get; set; }
        public int QuantityInquiry { get; set; }
        public string WiraniagaFlag { get; set; }
        public string Grading { get; set; }
        public string BranchHead { get; set; }
        public string PerolehanData { get; set; }
        public string TestDrive { get; set; }
        public string CaraPembayaran { get; set; }
        public string Leasing { get; set; }
        public string DownPayment { get; set; }
        public string Tenor { get; set; }
        public string NamaProspek { get; set; }
        public string AlamatProspek { get; set; }
        public string TelpRumah { get; set; }
        public DateTime NextFollowUpdate { get; set; }
        public string AlamatPerusahaan { get; set; }
        public string Handphone { get; set; }
        public string MerkLain { get; set; }       
    }
}
