﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.General.Models
{
    [Table("gnMstSupplierBank")]
    public class GnMstSupplierBank
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string SupplierCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountBank { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }

    }
}