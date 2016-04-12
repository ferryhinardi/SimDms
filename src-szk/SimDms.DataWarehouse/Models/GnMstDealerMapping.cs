﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("GnMstDealerMapping")]
    public class GnMstDealerMapping
    {
        [Key]
        public System.Int64 SeqNo { get; set; }
        public int GroupNo { get; set; }
        public string Area { get; set; }
        public string DealerName { get; set; }
        public string DealerAbbreviation { get; set; }
        public string DealerCode { get; set; }
        public bool isActive { get; set; }
    }

    [Table("GnMstDealerMappingNew")]
    public class GnMstDealerMappingNew
    {
        [Key]
        [Column(Order = 1)]
        public System.Int64 SeqNo { get; set; }
        public int? GroupNo { get; set; }
        public string Area { get; set; }
        public bool? isJV { get; set; }
        public string DealerName { get; set; }
        public string DealerAbbreviation { get; set; }
        public string DealerCode { get; set; }
        public bool? isActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}