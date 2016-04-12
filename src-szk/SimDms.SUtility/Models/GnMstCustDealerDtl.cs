using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.SUtility.Models
{
    [Table("gnMstCustDealerDtl")]
    public class GnMstCustDealerDtl
    {
        [Key]
        [Column(Order=1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order=2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order=3)]
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerGovName { get; set; }
        public string CustomerStatus { get; set; }
        public string Address { get; set; }
        public string ProvinceName { get; set; }
        public string CityName{ get; set; }
        public string ZipNo { get; set; }
        public string KelurahanDesa { get; set; }
        public string KecamatanDistrik { get; set; }
        public string KotaKabupaten { get; set; }
        public string IbuKota { get; set; }
        public string PhoneNo { get; set; }
        public string HpNo { get; set; }
        public string FaxNo { get; set; }
        public string OfficePhoneNo { get; set; }
        public string Email { get; set; }
        public DateTime?  BirthDate { get; set; }
        public bool? isPKP { get; set; }
        public string NPWPNo { get; set; }
        public DateTime? NPWPDate { get; set; }
        public string SKPNo { get; set; }
        public DateTime? SKPDate { get; set; }
        public string CustomerType { get; set; }
        public string Gender { get; set; }
        public string KTP { get; set; }
        [Key]
        [Column(Order=4)]
        public DateTime LastTransactionDate { get; set; }
        [Key]
        [Column(Order=5)]
        public string TransType { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class GnMstCustDealerDtlModel
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerGovName { get; set; }
        public string CustpmerStatus { get; set; }
        public string Address { get; set; }
        public string ProvinceName { get; set; }
        public string CityName { get; set; }
        public string ZipNo { get; set; }
        public string KelurahanDesa { get; set; }
        public string KecamatanDistrik { get; set; }
        public string KotaKabupaten { get; set; }
        public string IbuKota { get; set; }
        public string PhoneNo { get; set; }
        public string HpNo { get; set; }
        public string FaxNo { get; set; }
        public string OfficePhoneNo { get; set; }
        public string Email { get; set; }
        public string BirthDate { get; set; }
        public bool? isPKP { get; set; }
        public string NPWPNo { get; set; }
        public string NPWPDate { get; set; }
        public string SKPNo { get; set; }
        public string SKPDate { get; set; }
        public string CustomerType { get; set; }
        public char Gender { get; set; }
        public string KTP { get; set; }
        public DateTime LastTransactionDate { get; set; }
        public string TransType { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateDate { get; set; }
    }
}