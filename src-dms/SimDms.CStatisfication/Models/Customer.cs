using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.CStatisfication.Models
{
    [Table("gnMstCustomer")]
    public class Customer
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CustomerCode { get; set; }
        public string StandardCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAbbrName { get; set; }
        public string CustomerGovName { get; set; }
        public string CustomerType { get; set; }
        public string CategoryCode { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string PhoneNo { get; set; }
        public string HPNo { get; set; }
        public string FaxNo { get; set; }
        public bool isPKP { get; set; }
        public string NPWPNo { get; set; }
        public DateTime? NPWPDate { get; set; }
        public string SKPNo { get; set; }
        public DateTime? SKPDate { get; set; }
        public string ProvinceCode { get; set; }
        public string AreaCode { get; set; }
        public string CityCode { get; set; }
        public string ZipNo { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Spare01 { get; set; }
        public string Spare02 { get; set; }
        public string Spare03 { get; set; }
        public string Spare04 { get; set; }
        public string Spare05 { get; set; }
        public string Gender { get; set; }
        public string OfficePhoneNo { get; set; }
        public string KelurahanDesa { get; set; }
        public string KecamatanDistrik { get; set; }
        public string KotaKabupaten { get; set; }
        public string IbuKota { get; set; }
        public string CustomerStatus { get; set; }
    }

    [Table("CsCustomerView")]
    public class CustomerView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        // public string CompanyName { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public DateTime? BirthDate { get; set; }

        public string AddPhone1 { get; set; }
        public string AddPhone2 { get; set; }
        public string ReligionCode { get; set; }
    }

    [Table("CsCustomerBuyView")]
    public class CustomerBuyView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CustomerCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string Chassis { get; set; }
        public string Engine { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public bool? IsDeliveredA { get; set; }
        public bool? IsDeliveredB { get; set; }
        public bool? IsDeliveredC { get; set; }
        public bool? IsDeliveredD { get; set; }
        public bool? IsDeliveredE { get; set; }
        public string Comment { get; set; }
        public string Additional { get; set; }
        public int Status { get; set; }

        public string CarType { get; set; }
        public string Color { get; set; }
        public string PoliceRegNo { get; set; }
        public string SalesmanCode { get; set; }
        public string SalesmanName { get; set; }
        public string BranchCode { get; set; }

        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public string StatusInfo { get; set; }

        public DateTime? BirthDate { get; set; }
        public string AddPhone1 { get; set; }
        public string AddPhone2 { get; set; }
        public string ReligionCode { get; set; }
    }
}