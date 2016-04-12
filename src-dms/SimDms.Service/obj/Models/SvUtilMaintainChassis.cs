using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    public class SvUtilMaintainChassis
    {
        public string PoliceRegNo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerDesc { get; set; }
        public string CustomerAddr { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string DealerDesc { get; set; }
        public string ChassisCode { get; set; }
        public string ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public string EngineNo { get; set; }
        public string ServiceBookNo { get; set; }
        public string ClubCode { get; set; }
        public string ColourCode { get; set; }
        public DateTime? FakturPolisiDate { get; set; }
        public string ClubNo { get; set; }
        public DateTime? ClubDateStart { get; set; }
        public DateTime? ClubDateFinish { get; set; }
        public DateTime? ClubSince { get; set; }
        public string IsClubStatusDesc { get; set; }
        public bool IsClubStatus { get; set; }
        public string IsContractStatusDesc { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveDesc { get; set; }
        public string BasicModel { get; set; }
        public string TransmissionType { get; set; }
        public DateTime? LastServiceDate { get; set; }
        public string LastJobType { get; set; }
        public string ContractNo { get; set; }
        public string ContactName { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
    }

    public class SvUtilMaintainChassisLinkData
    {
        public int SeqNo { get; set; }
        public string LinkData { get; set; }
        public string StatusDesc { get; set; }
    }
}