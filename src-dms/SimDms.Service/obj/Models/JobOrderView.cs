using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("SvBasicModelView")]
    public class BasicModelView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string RefferenceType { get; set; }
        [Key]
        [Column(Order = 4)]
        public string RefferenceCode { get; set; }
        public string Description { get; set; }
        public string DescriptionEng { get; set; }
        public string IsActive { get; set; }
    }

    [Table("SvTaskTypeView")]
    public class TaskTypeView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BasicModel { get; set; }
        [Key]
        [Column(Order = 4)]
        public string TaskType { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string IsActive { get; set; }

    }

    [Table("SvTaskNoView")]
    public class TaskNoView
    {
        //[Key]
        //[Column(Order = 1)]
        //public string CompanyCode { get; set; }
        //[Key]
        //[Column(Order = 2)]
        //public string ProductType { get; set; }
        //[Key]
        //[Column(Order = 3)]
        //public string BasicModel { get; set; }
        //[Key]
        //[Column(Order = 4)]
        //public string JobType { get; set; }
        [Key]
        [Column(Order = 1)]
        public string OperationNo { get; set; }
        public string DescriptionTask { get; set; }
        public decimal? Qty { get; set; }
        public decimal? Price { get; set; }

        //public decimal? ClaimHour { get; set; }
        //public decimal? OperationHour { get; set; }
        //public decimal? LaborCost { get; set; }
        //public decimal? LaborPrice { get; set; }
    }
}