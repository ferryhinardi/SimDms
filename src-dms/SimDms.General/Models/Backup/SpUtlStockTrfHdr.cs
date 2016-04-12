using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sparepart.Models
{
    [Table("spUtlStockTrfHdr")]
    public class SpUtlStockTrfHdr
    {
        [Key]
        [Column(Order=1)]
        public string CompanyCode {get; set; }
        [Key]
        [Column(Order =2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DealerCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string LampiranNo { get; set; }
        public string RcvDealerCode{ get; set; }
        public string InvoiceNo { get; set; }
        public string BinningNo { get; set; }
        public DateTime? BinningDate { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy{ get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string TypeOfGoods { get; set; }
    }
}
