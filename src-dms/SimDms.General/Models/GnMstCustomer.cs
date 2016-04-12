using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.General.Models
{
    //[Table("GnMstCustomer")]
    //public class GnMstCustomer
    //{
    //    [Key]
    //    [Column(Order = 1)]
    //    public string CompanyCode { get; set; }
    //    [Key]
    //    [Column(Order = 2)]
    //    public string CustomerCode { get; set; }
    //    public string StandardCode { get; set; }
    //    public string CustomerName { get; set; }
    //    public string CustomerAbbrName { get; set; }
    //    public string CustomerGovName { get; set; }
    //    public string CustomerType { get; set; }
    //    public string CategoryCode { get; set; }
    //    public string Address1 { get; set; }
    //    public string Address2 { get; set; }
    //    public string Address3 { get; set; }
    //    public string Address4 { get; set; }
    //    public string PhoneNo { get; set; }
    //    public string HPNo { get; set; }
    //    public string FaxNo { get; set; }
    //    public bool? isPKP { get; set; }
    //    public string NPWPNo { get; set; }
    //    public DateTime? NPWPDate { get; set; }
    //    public string SKPNo { get; set; }
    //    public DateTime? SKPDate { get; set; }
    //    public string ProvinceCode { get; set; }
    //    public string AreaCode { get; set; }
    //    public string CityCode { get; set; }
    //    public string ZipNo { get; set; }
    //    public string Status { get; set; }
    //    public string CreatedBy { get; set; }
    //    public DateTime? CreatedDate { get; set; }
    //    public string LastUpdateBy { get; set; }
    //    public DateTime? LastUpdateDate { get; set; }
    //    public bool? isLocked { get; set; }
    //    public string LockingBy { get; set; }
    //    public DateTime? LockingDate { get; set; }
    //    public string Email { get; set; }
    //    public DateTime? BirthDate { get; set; }
    //    public string Spare01 { get; set; }
    //    public string Spare02 { get; set; }
    //    public string Spare03 { get; set; }
    //    public string Spare04 { get; set; }
    //    public string Spare05 { get; set; }
    //    public string Gender { get; set; }
    //    public string OfficePhoneNo { get; set; }
    //    public string KelurahanDesa { get; set; }
    //    public string KecamatanDistrik { get; set; }
    //    public string KotaKabupaten { get; set; }
    //    public string IbuKota { get; set; }
    //    public string CustomerStatus { get; set; }
    //}

    public class myProfitCenterInfo
    {
        public string ProfitCenterName { get; set; }
        public string CustomerClassName { get; set; }
        public string TaxDesc { get; set; }
        public string CollectorName { get; set; }
        public string TaxTransDesc { get; set; }
        public string SalesmanName { get; set; }
        public string KelARDesc { get; set; }
        public string CustomerGradeName { get; set; }
        public string GroupPriceDesc { get; set; }
 
    }
}