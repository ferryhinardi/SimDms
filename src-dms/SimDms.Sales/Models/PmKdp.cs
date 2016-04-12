﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("pmKDP")]
    public class PmKdp
    {
        [Key]
        [Column(Order = 1)]
        public int InquiryNumber { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string CompanyCode { get; set; }
        public string EmployeeID { get; set; }
        public string SpvEmployeeID { get; set; }
        public DateTime? InquiryDate { get; set; }
        public string OutletID { get; set; }
        public string StatusProspek { get; set; }
        public string PerolehanData { get; set; }
        public string NamaProspek { get; set; }
        public string AlamatProspek { get; set; }
        public string TelpRumah { get; set; }
        public string CityID { get; set; }
        public string NamaPerusahaan { get; set; }
        public string AlamatPerusahaan { get; set; }
        public string Jabatan { get; set; }
        public string Handphone { get; set; }
        public string Faximile { get; set; }
        public string Email { get; set; }
        public string TipeKendaraan { get; set; }
        public string Variant { get; set; }
        public string Transmisi { get; set; }
        public string ColourCode { get; set; }
        public string CaraPembayaran { get; set; }
        public string TestDrive { get; set; }
        public int? QuantityInquiry { get; set; }
        public string LastProgress { get; set; }
        public DateTime? LastUpdateStatus { get; set; }
        public DateTime? SPKDate { get; set; }
        public DateTime? LostCaseDate { get; set; }
        public string LostCaseCategory { get; set; }
        public string LostCaseReasonID { get; set; }
        public string LostCaseOtherReason { get; set; }
        public string LostCaseVoiceOfCustomer { get; set; }
        public DateTime? CreationDate { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string Leasing { get; set; }
        public string DownPayment { get; set; }
        public string Tenor { get; set; }
        public string MerkLain { get; set; }

        [NotMapped]
        public string StatusVehicle { get; set; }
        [NotMapped]
        public string BrandCode { get; set; }
        [NotMapped]
        public string ModelName { get; set; }
        [NotMapped]
        public string NikSales { get; set; }
        [NotMapped]
        public string NikSC { get; set; }
        [NotMapped]
        public string EmployeeName { get; set; }
    }

    [Table("PmKdpAdditional")]
    public class PmKdpAdditional
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public int InquiryNumber { get; set; }
        public string StatusVehicle { get; set; }
        public string OthersBrand { get; set; }
        public string OthersType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    [Table("PmKdpClnUpView")]
    public class PmKdpClnUpView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public int InquiryNumber { get; set; }
        public DateTime InquiryDate { get; set; }
        public string InquiryDateStr { get; set; }
        public string NamaProspek { get; set; }
        public string TipeKendaraan { get; set; }
        public string Variant { get; set; }
        public string Transmisi { get; set; }
        public string ColourCode { get; set; }
        public string PerolehanData { get; set; }
        public string EmployeeID { get; set; }
        public string Wiraniaga { get; set; }
        public string SpvEmployeeID { get; set; }
        public string Coordinator { get; set; }
        public string LastProgress { get; set; }
        public DateTime NextFollowUpDate { get; set; }
    }
}