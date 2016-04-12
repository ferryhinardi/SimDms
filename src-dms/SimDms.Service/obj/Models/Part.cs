using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    public class InquiryPart
    {
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public string ProductType { get; set; }
        public string Partcategory { get; set; }
        public string TypeOfGoods { get; set; }
        public string TypeOfGoodsDesc { get; set; }
        public string Status { get; set; }
        public decimal? Available { get; set; }
        public decimal? RetailPriceInclTax { get; set; }
        public string StatusDesc { get; set; }
    }

    public class ItemMod
    {
        public string ID { get; set; }
        public string InterChangeCode { get; set; }
    }

}