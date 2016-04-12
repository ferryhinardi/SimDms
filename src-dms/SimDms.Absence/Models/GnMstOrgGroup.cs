﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Absence.Models
{
    [Table("GnMstOrgGroup")]
    public class GnMstOrgGroup
    {
        [Key]
        [Column(Order=1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string OrgGroupCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string OrgCode { get; set; }
        public string OrgName { get; set; }
        public string OrgHeader { get; set; }
        public int OrgSeq { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public string Createdby { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}