﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SimDms.Common.Models
{
    [Table("gnMstPeriode")]
    public class Period
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public decimal FiscalYear { get; set; }
        [Key]
        [Column(Order = 4)]
        public decimal FiscalMonth { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal PeriodeNum { get; set; }
        public string PeriodeName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? StatusSparepart { get; set; }
        public int? StatusSales { get; set; }
        public int? StatusService { get; set; }
        public int? StatusFinanceAP { get; set; }
        public int? StatusFinanceAR { get; set; }
        public int? StatusFinanceGL { get; set; }
        public bool? FiscalStatus { get; set; }
        public string CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }


    public class PeriodView
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string FiscalYear { get; set; }
        public decimal FiscalMonth { get; set; }
        public string Periode { get; set; } 
        public decimal PeriodeNum { get; set; }
        public string PeriodeName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? StatusSparepart { get; set; }
        public int? StatusSales { get; set; }
        public int? StatusService { get; set; }
        public int? StatusFinanceAP { get; set; }
        public int? StatusFinanceAR { get; set; }
        public int? StatusFinanceGL { get; set; }
        public bool? FiscalStatus { get; set; }
    }

    public class PeriodViewModel
    {
        public string CompanyCodeAlias { get; set; }
        public decimal BranchCode { get; set; }
        public decimal FiscalYear { get; set; }
        public decimal FiscalMonth { get; set; }
        public string Periode { get; set; }
        public decimal PeriodeNum { get; set; }
        public string PeriodeName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? StatusSparepart { get; set; }
        public int? StatusSales { get; set; }
        public int? StatusService { get; set; }
        public int? StatusFinanceAP { get; set; }
        public int? StatusFinanceAR { get; set; }
        public int? StatusFinanceGL { get; set; }
        public bool? FiscalStatus { get; set; }
    }
}