using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("omTrSalesSPKDetail")]
    public class omTrSalesSPKDetail
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string SPKNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal ChassisNo { get; set; }
        public string ReqInNo { get; set; }
        public string FakturPolisiNo { get; set; }
        public string PoliceRegistrationNo { get; set; }
        public DateTime? PoliceRegistrationDate { get; set; }
        public DateTime? STNKInDate { get; set; }
        public string STNKInBy { get; set; }
        public DateTime? STNKOutDate { get; set; }
        public string STNKOutBy { get; set; }
        public DateTime? BPKBInDate { get; set; }
        public string BPKBInBy { get; set; }
        public DateTime? BPKBOutDate { get; set; }
        public string BPKBOutBy { get; set; }
        public DateTime? KIRInDate { get; set; }
        public string KIRInBy { get; set; }
        public DateTime? KIROutDate { get; set; }
        public string KIROutBy { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string BPKBNo { get; set; }
    }

    [Table("omTrSalesSPKSubDetail")]
    public class omTrSalesSPKSubDetail
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string SPKNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal ChassisNo { get; set; }
        public DateTime BPKBOutDate { get; set; }
        [Key]
        [Column(Order = 7)]
        public string BPKBOutType { get; set; }
        [Key]
        [Column(Order = 8)]
        public string BPKBOutBy { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class SPKDetailView 
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string SPKNo { get; set; }
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; }
        public string ReqInNo { get; set; }
        public string FakturPolisiNo { get; set; }
        public string PoliceRegistrationNo { get; set; }
        public string PoliceRegistrationDate { get; set; }
        public string STNKInDate { get; set; }
        public string STNKInBy { get; set; }
        public string STNKOutDate { get; set; }
        public string STNKOutBy { get; set; }
        public string BPKBInDate { get; set; }
        public string BPKBInBy { get; set; }
        public string BPKBOutDate { get; set; }
        public string BPKBOutBy { get; set; }
        public string KIRInDate { get; set; }
        public string KIRInBy { get; set; }
        public string KIROutDate { get; set; }
        public string KIROutBy { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateDate { get; set; }
        public string BPKBNo { get; set; }
        public string Address { get; set; }
        public string CustomerName { get; set; } 
        public string CustomerCode { get; set; }
        public string Leasing { get; set; }  
    }

    public class SPKSubDetailView
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string SPKNo { get; set; }
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; }
        public string BPKBOutDate { get; set; }
        public string BPKBOutType { get; set; }
        public string BPKBOutBy { get; set; }
        public string tipe { get; set; }
        public string Nama { get; set; }
        public string BPKBOutDateDesc { get; set; }
    }
}