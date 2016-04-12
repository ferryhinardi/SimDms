//using System;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace SimDms.General.Models
//{
//    [Table("GnMstCustomerClass")]
//    public class GnMstCustomerClass
//    {
//        [Key]
//        [Column(Order = 1)]
//        public string CompanyCode { get; set; }
//        [Key]
//        [Column(Order = 2)]
//        public string BranchCode { get; set; }
//        [Key]
//        [Column(Order = 3)]
//        public string CustomerClass { get; set; }
//        [Key]
//        [Column(Order = 4)]
//        public char TypeOfGoods { get; set; }
//        public string ProfitCenterCode { get; set; }
//        public string CustomerClassName { get; set; }
//        public string ReceivableAccNo { get; set; }
//        public string DownPaymentAccNo { get; set; }
//        public string InterestAccNo { get; set; }
//        public string TaxOutAccNo { get; set; }
//        public string LuxuryTaxAccNo { get; set; }
//        public string CreatedBy { get; set; }
//        public DateTime? CreatedDate { get; set; }
//        public string LastUpdateBy { get; set; }
//        public DateTime? LastUpdateDate { get; set; }
//        public bool? isLocked { get; set; }
//        public string LockingBy { get; set; }
//        public DateTime? LockingDate { get; set; }

//    }
//}