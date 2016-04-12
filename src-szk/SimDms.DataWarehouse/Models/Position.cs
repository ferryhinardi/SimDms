﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("gnMstPosition")]
    public class Position
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string DeptCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PosCode { get; set; }
        public string PosName { get; set; }
        public string PosHeader { get; set; }
        public int? PosLevel { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string TitleCode { get; set; }
    }

    [Table("HrMstPosition")]
    public class HrPosition
    {
        [Key]
        [Column(Order = 1)]
        public string DeptCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string PosCode { get; set; }
        public string PosName { get; set; }
        public string PosHeader { get; set; }
        public int? PosLevel { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string TitleCode { get; set; }
    }

    public class CbPosition
    {
        public string DeptCode { get; set; }
        public string PosCode { get; set; }
        public string PosName { get; set; }
        public int PosLevel { get; set; }
    }
}