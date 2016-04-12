using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("SvTrnMaintainSPKView")]
    public class MaintainSPKView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public long ServiceNo { get; set; }
        public string JobOrderNo { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public string PoliceRegNo { get; set; }
        public string ServiceBookNo { get; set; }
        public string BasicModel { get; set; }
        public string TransmissionType { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string ColorCode { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCodeBill { get; set; }
        public string CustomerNameBill { get; set; }
        public decimal? Odometer { get; set; }
        public string JobType { get; set; }
        public string ServiceStatus { get; set; }
        public string ServiceName { get; set; }
        public string ForemanID { get; set; }
        public string ForemanName { get; set; }
    }
}