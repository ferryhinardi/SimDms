﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.CStatisfication.Models
{
    [Table("CsStnkExt")]
    public class CsStnkExt
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
        public bool IsStnkExtend { get; set; }
        [Key]
        [Column(Order = 4)]
        public DateTime? StnkExpiredDate { get; set; }
        public bool ReqKtp { get; set; }
        public bool ReqStnk { get; set; }
        public bool ReqBpkb { get; set; }
        public bool ReqSuratKuasa { get; set; }
        public string Comment { get; set; }
        public string Additional { get; set; }
        public int Status { get; set; }
        public bool? Ownership { get; set; }
        public DateTime? FinishDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public DateTime? StnkDate { get; set; }
        public DateTime? BpkbDate { get; set; }
        public string Reason { get; set; }
    }

    [Table("CsLkuStnkExtensionView")]
    public class CsLkuStnkExtensionView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Chassis { get; set; }
        public string Engine { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string ColourCode { get; set; }
        public DateTime? BpkbDate { get; set; }
        public DateTime? StnkDate { get; set; }
        public DateTime? BPKDate { get; set; }
        public bool? IsLeasing { get; set; }
        public string LeasingCo { get; set; }
        public string Category { get; set; }
        public string LeasingName { get; set; }
        public decimal? Installment { get; set; }
        public string Tenor { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public string Salesman { get; set; }
        public string SalesmanName { get; set; }
        public bool? IsStnkExtend { get; set; }
        public DateTime? StnkExpiredDate { get; set; }
        public bool? ReqKtp { get; set; }
        public bool? ReqStnk { get; set; }
        public bool? ReqBpkb { get; set; }
        public bool? ReqSuratKuasa { get; set; }
        public string Comment { get; set; }
        public string Additional { get; set; }
        public int? Status { get; set; }
        public bool? Ownership { get; set; }
        public string StatusInfo { get; set; }
        public string PoliceRegNo { get; set; }
        public string Outstanding { get; set; }
    }

    
}