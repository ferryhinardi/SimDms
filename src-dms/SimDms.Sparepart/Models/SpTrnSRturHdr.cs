﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sparepart.Models
{
    [Table("SpTrnSRturHdr")]
    public class SpTrnSRturHdr
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ReturnNo { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string CustomerCode { get; set; }
        public string FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public string FPJCentralNo { get; set; }
        public DateTime? FPJCentralDate { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public decimal? TotReturQty { get; set; }
        public decimal? TotReturAmt { get; set; }
        public decimal? TotDiscAmt { get; set; }
        public decimal? TotDPPAmt { get; set; }
        public decimal? TotPPNAmt { get; set; }
        public decimal? TotFinalReturAmt { get; set; }
        public decimal? TotCostAmt { get; set; }
        public bool? isPKP { get; set; }
        public string NPWPNo { get; set; }
        public decimal? PrintSeq { get; set; }
        public string Status { get; set; }
        public string TypeOfGoods { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }

    public class SpTrnSRturHdrView
    {
        public string ReturnNo {get;set;}
        public DateTime ReferenceDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public string CustomerCode { get; set; }
        public string ReferenceNo { get; set; }
    }
}
