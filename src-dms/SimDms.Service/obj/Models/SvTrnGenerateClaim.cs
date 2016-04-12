﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("GnMstLookUpDtl")]
    public class GnMstLookUpDtl
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CodeID { get; set; }
        [Key]
        [Column(Order = 3)]
        public string LookUpValue { get; set; }
        public decimal? SeqNo { get; set; }
        public string ParaValue { get; set; }
        public string LookUpValueName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }

    }

    public class GenerateClm
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 4)]
        public string BatchNo { get; set; }
        public DateTime BatchDate { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public string FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public string FPJGovNo { get; set; }
        public decimal? LotNo { get; set; }
        public decimal? ProcessSeq { get; set; }
        public decimal TotalNoOfItem { get; set; }
        public decimal? TotalClaimAmt { get; set; }
        public decimal? OtherCompensationAmt { get; set; }
        public string LockingBy { get; set; }
    }
}