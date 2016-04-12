using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    public class JobType
    {
    }

    [Table("SvJobTypeView")]
    public class JobTypeView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BasicModel { get; set; }
        [Key]
        [Column(Order = 3)]
        public string JobType { get; set; }
        public string Description { get; set; }
    }
}