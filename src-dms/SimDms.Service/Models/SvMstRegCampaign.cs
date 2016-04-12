using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("svMstFscCampaign")]
    public class svMstFscCampaign
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public decimal ChassisNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string CampaignFlag { get; set; }
        public string SalesModel { get; set; }
        public string FacturNo { get; set; }
        public DateTime? FacturDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }



    }
    [Table("SvRegCampaign")]
    public class SvRegCampaign
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string PoliceRegNo { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public decimal ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CampaignFlag { get; set; }
        public string LookupValueName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ServiceBookNo { get; set; }
        public string Address { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }


    }
    public class RegCampaign
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Column(Order = 3)]
        public string PoliceRegNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CampaignFlag { get; set; }
        public string LookupValueName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ServiceBookNo { get; set; }
        public string Address { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public string SalesModel { get; set; }
    }

    public class MstCustomerVehicleForMstRegCompaign
    {
        public string PoliceRegNo { get; set; }
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string ServiceBookNo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string CityCode { get; set; }
        public string SalesModel { get; set; }
        public string Address { get; set; }
        public string CityName { get; set; }
    }
}