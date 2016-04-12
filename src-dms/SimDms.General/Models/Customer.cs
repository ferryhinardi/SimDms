using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.General.Models
{
    public class LookupCustomer
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string LookupValue { get; set; }
    }

    public class CustomerModel
    {
        public string CompanyCode { get; set; }
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
        public bool? isPKP { get; set; }
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
        public string PosCode { get; set; }
    }

    public class CustomerProfitCenterModel
    {
        public string CustomerCode { get; set; }
        public string ProfitCenterCode { get; set; }
        public string ProfitCenterName { get; set; }
        public string CustomerClass { get; set; }
        public string CustomerClassName { get; set; }
        public string CustomerGovName { get; set; }
        public string PaymentCode { get; set; }
        public string PaymentDesc { get; set; }
        public string TaxCode { get; set; }
        public string TaxDesc { get; set; }
        public string CollectorCode { get; set; }
        public string CollectorName { get; set; }
        public string TaxTransCode { get; set; }
        public string TaxTransDesc { get; set; }
        public string Salesman { get; set; }
        public string SalesmanName { get; set; }
        public string KelAR { get; set; }
        public string KelARDesc { get; set; }
        public decimal? CreditLimit { get; set; }
        public bool? isOverDueAllowed { get; set; }
        public bool? isBlackList { get; set; } 
        public string CustomerGrade { get; set; }
        public string CustomerGradeName { get; set; }
        public string ContactPerson { get; set; }
        public string TOPCode { get; set; }
        public string TOPDesc { get; set; }
        public string GroupPrice { get; set; }
        public string GroupPriceCode { get; set; }
        public string GroupPriceDesc { get; set; }
        public decimal? DiscPct { get; set; }
        public string SalesType { get; set; }
        public decimal? PartDiscPct { get; set; }
        public decimal? MaterialDiscPct { get; set; }
        public decimal? LaborDiscPct { get; set; }
    }

    public class CustomerDiscountModel
    {
        public string CustomerCode { get; set; }
        public string ProfitCenterCode { get; set; }
        public string ProfitCenterName { get; set; }
        public string TypeOfGoods { get; set; }
        public string TypeOfGoodsName{ get; set; }
        public decimal? DiscPct { get; set; } 
    }

    public class CustomerBankModel
    {
        public string CustomerCode { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountBank { get; set; }
    }

    public class CustomerClassModel
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string CustomerClass { get; set; }
        public string TypeOfGoods { get; set; }
        public string ProfitCenterCode { get; set; }
        public string CustomerClassName { get; set; }
        public string ReceivableAccNo { get; set; }
        public string DownPaymentAccNo { get; set; }
        public string InterestAccNo { get; set; }
        public string TaxOutAccNo { get; set; }
        public string LuxuryTaxAccNo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public bool isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime LockingDate { get; set; }
    }

    public class LookupCoProfile  
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string LookupValue { get; set; }
    }
}