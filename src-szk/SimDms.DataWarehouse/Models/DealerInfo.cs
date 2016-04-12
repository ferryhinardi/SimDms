using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("DealerInfo")]
    public class DealerInfo
    {
        [Key]
        [Column(Order = 1)]
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string ShortName { get; set; }
        public string ProductType { get; set; }
    }

    public class GenPartSales
    {
        public DateTime? RecordDate { get; set; }
        public String CompanyCode { get; set; }
        public String BranchCode { get; set; }
        public String InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public String FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public String CustomerCode { get; set; }
        public String CustomerName { get; set; }
        public String CustomerClass { get; set; }
        public String PartNo { get; set; }
        public String PartName { get; set; }
        public String TypeOfGoods { get; set; }
        public String TypeOfGoodsDesc { get; set; }
        public Decimal? QtyBill { get; set; }
        public Decimal? CostPrice { get; set; }
        public Decimal? RetailPrice { get; set; }
        public Decimal? DiscPct { get; set; }
        public Decimal? DiscAmt { get; set; }
        public Decimal? NetSalesAmt { get; set; }
        public String Area { get; set; }
        public String DealerCode { get; set; }
        public String DealerAbbreviation { get; set; }
        public String BranchName { get; set; }
        public String DealerName { get; set; }
    }

}