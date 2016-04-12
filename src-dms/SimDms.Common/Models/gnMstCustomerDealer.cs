using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Common.Models
{
    [Table("gnMstCustomerDealer")]
    public class gnMstCustomerDealer
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CustomerCode { get; set; }
        public string SuzukiCode { get; set; }
        public string Suzuki2Code { get; set; }
        public string CustomerName { get; set; }
        public string CustomerGovName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string ProvinceCode { get; set; }
        public string ProvinceName { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public string ZipNo { get; set; }
        public string KelurahanDesa { get; set; }
        public string KecamatanDistrik { get; set; }
        public string KotaKabupaten { get; set; }
        public string IbuKota { get; set; }
        public string PhoneNo { get; set; }
        public string HPNo { get; set; }
        public string FaxNo { get; set; }
        public string OfficePhoneNo { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool? isPKP { get; set; }
        public string NPWPNo { get; set; }
        public DateTime? NPWPDate { get; set; }
        public string SKPNo { get; set; }
        public DateTime? SKPDate { get; set; }
        public string CustomerType { get; set; }
        public string CustomerTypeDesc { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryDesc { get; set; }
        public string Status { get; set; }
        public string CustomerStatus { get; set; }
        public string DCSFlag { get; set; }
        public DateTime? DCSDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}
