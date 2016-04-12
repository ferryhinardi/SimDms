using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    public class TaskPart
    {
        public string PartNo { get; set; }
        public decimal? Available { get; set; }
        public decimal? RetailPriceInclTax { get; set; }
        public string PartName { get; set; }
        public string TypeOfGoods { get; set; }
        public string GroupTypeOfGoods { get; set; }
        public string Status { get; set; }
        public decimal? NilaiPart { get; set; }
    }
}