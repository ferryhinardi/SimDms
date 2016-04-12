﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.General.Models
{
    [Table("GnMstOrganizationDtl")]
    public class GnMstOrganizationDtl
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        public string BranchAccNo { get; set; }
        public decimal? SeqNo { get; set; }
        public string BranchName { get; set; }
        public bool? IsBranch { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }

    }
}