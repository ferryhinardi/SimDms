﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("OmTrSalesPerlengkapanOutDetail")]
    public class OmTrSalesPerlengkapanOutDetail
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PerlengkapanNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public string PerlengkapanCode { get; set; }
        public decimal? QuantityStd { get; set; }
        public decimal? Quantity { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }        
    }
}