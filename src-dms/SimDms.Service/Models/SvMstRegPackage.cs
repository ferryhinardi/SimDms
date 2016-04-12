using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{

    [Table("svMstPackageContract")]
    public class svMstPackageContract
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string PackageCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public long ChassisNo { get; set; }
        public string PoliceRegNo { get; set; }
        public string CustomerCode { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string VirtualAccount { get; set; }
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }




    }

    [Table("SvRegPackageView")]
    public class SvRegPackageView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string PoliceRegNo { get; set; }
        public string Chassis { get; set; }
        public string ChassisCode { get; set; }
        public long ChassisNo { get; set; }
        public string PackageCode { get; set; }
        public string Package { get; set; }
        public string Customer { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public string AccountNo { get; set; }



    }
}