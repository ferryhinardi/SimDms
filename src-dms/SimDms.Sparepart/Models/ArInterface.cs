using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sparepart.Models
{
    [Table("ArInterface")]
    public class ArInterface
    {
        [Key]
        [Column(Order=1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order=2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DocNo { get; set; }
        public DateTime DocDate { get; set; }
        public string ProfitCenterCode { get; set; }
        public decimal? NettAmt { get; set; }
        public decimal? ReceiveAmt { get; set; }
        public string CustomerCode { get; set; }
        public string TOPCode { get; set; }
        public DateTime DueDate { get; set; }
        public string TypeTrans { get; set; }
        public decimal? BlockAmt { get; set; }
        public decimal? DebetAmt { get; set; }
        public decimal? CreditAmt { get; set; }
        public string SalesCode { get; set; }
        public string LeasingCode { get; set; }
        public string StatusFlag { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string AccountNo { get; set; }
        public string FakturPajakNo { get; set; }
        public DateTime FakturPajakDate { get; set; }
    }
}
