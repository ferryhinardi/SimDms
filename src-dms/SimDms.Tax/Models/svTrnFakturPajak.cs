using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Tax.Models
{
    [Table("svTrnFakturPajak")]
    public class svTrnFakturPajak
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string FPJNo { get; set; }
        public DateTime FPJDate { get; set; }
        public string FPJGovNo { get; set; }
        public string FPJCentralNo { get; set; }
        public DateTime? FPJCentralDate { get; set; }
        public decimal NoOfInvoice { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerCodeBill { get; set; }
        public string CustomerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string PhoneNo { get; set; }
        public string HPNo { get; set; }
        public bool IsPKP { get; set; }
        public string SKPNo { get; set; }
        public DateTime? SKPDate { get; set; }
        public string NPWPNo { get; set; }
        public DateTime? NPWPDate { get; set; }
        public string TOPCode { get; set; }
        public decimal? TOPDays { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime SignedDate { get; set; }
        public decimal? PrintSeq { get; set; }
        public string GenerateStatus { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
    }

    public class svTrnFakturPajakView
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string FPJNo { get; set; }
        public DateTime FPJDate { get; set; }
        public string FPJGovNo { get; set; }
        public string FPJCentralNo { get; set; }
        public DateTime? FPJCentralDate { get; set; }
        public decimal NoOfInvoice { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCodeBill { get; set; }
        public string CustomerNameBill { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string PhoneNo { get; set; }
        public string HPNo { get; set; }
        public bool IsPKP { get; set; }
        public string SKPNo { get; set; }
        public DateTime? SKPDate { get; set; }
        public string NPWPNo { get; set; }
        public DateTime? NPWPDate { get; set; }
    }
}