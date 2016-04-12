﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    public class GenerateITSIndentModel
    {
        public string DealerAbbreviation { get; set; }
        public string OutletAbbreviation { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int? InquiryNumber { get; set; }
        public DateTime? InquiryDate { get; set; }
        public string OutletID { get; set; }
        public string BranchHead { get; set; }
        public string SalesHead { get; set; }
        public string SalesCoordinator { get; set; }
        public string Wiraniaga { get; set; }
        public string StatusProspek { get; set; }
        public string PerolehanData { get; set; }
        public string NamaProspek { get; set; }
        public string AlamatProspek { get; set; }
        public string TelpRumah { get; set; }
        public string City { get; set; }
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
        public string ColourDescription { get; set; }
        public string CaraPembayaran { get; set; }
        public string TestDrive { get; set; }
        public int? QuantityInquiry { get; set; }
        public string LastProgress { get; set; }
        public DateTime? LastUpdateStatus { get; set; }
        public DateTime? ProspectDate { get; set; }
        public DateTime? HotDate { get; set; }
        public DateTime? SPKDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string Leasing { get; set; }
        public string DownPayment { get; set; }
        public string Tenor { get; set; }
        public DateTime? LostCaseDate { get; set; }
        public string LostCaseCategory { get; set; }
        public string LostCaseReasonID { get; set; }
        public string LostCaseOtherReason { get; set; }
        public string LostCaseVoiceOfCustomer { get; set; }
        public string MerkLain { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}