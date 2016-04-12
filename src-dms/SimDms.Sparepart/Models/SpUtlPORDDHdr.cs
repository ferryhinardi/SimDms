using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Models
{
    [Table("SpUtlPORDDHdr")]
    public class SpUtlPORDDHdr
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DealerCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string OrderNo { get; set; }
        public string RcvDealerCode { get; set; }
        public string ShipToDealerCode { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderType { get; set; }
        public string DealerName { get; set; }
        public decimal? TotNoItem { get; set; }
        public string ProductType { get; set; }
        public string BackOrderStatus { get; set; }
        public string Overseas { get; set; }
        public string SpecialInstruction { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
