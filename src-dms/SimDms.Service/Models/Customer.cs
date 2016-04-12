using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    //[Table("gnMstCustomer")]
    //public class Customer
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
    //    public bool isPKP { get; set; }
    //    public string NPWPNo { get; set; }
    //    public DateTime NPWPDate { get; set; }
    //    public string SKPNo { get; set; }
    //    public DateTime SKPDate { get; set; }
    //    public DateTime? BirthDate { get; set; }
    //}

    [Table("SvCustomerView")]
    public class CustomerView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }
        [NotMapped]
        public string Address { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public string FaxNo { get; set; }
        public string PhoneNo { get; set; }
        public string HPNo { get; set; }
        public string NPWPNo { get; set; }
        public DateTime NPWPDate { get; set; }
        public string SKPNo { get; set; }
        public DateTime SKPDate { get; set; }
        public DateTime? BirthDate { get; set; }
    }

    [Table("SvTrnSenderDealerView")]
    public class SenderDealerView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAbbrName { get; set; }
        public string Address { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string CityCode { get; set; }
        public string CityCodeDesc { get; set; }
        public string PhoneNo { get; set; }
        public string HPNo { get; set; }
        public string FaxNo { get; set; }
        public string ProfitCenterCode { get; set; }
        public string ProfitCenter { get; set; }
        public string Status { get; set; }

    }

    public class CustomerCodeBillItem
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Address1 { get; set; }
        public string CustomerAbbrName { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string CityCode { get; set; }
        public string CityDesc { get; set; }
        public string PhoneNo { get; set; }
        public string HPNo { get; set; }
        public string FaxNo { get; set; }
        public string ProfitCenter { get; set; }
        public string Status { get; set; }
        public string TOPDesc { get; set; }
    }

    public class CustomerExtraInfoItem
    {
        public string CustomerCode { get; set; }
        public string NPWPNo { get; set; }
        public DateTime? NPWPDate { get; set; }
        public decimal LaborDiscPct { get; set; }
        public decimal PartsDiscPct { get; set; }
        public decimal MaterialDiscPct { get; set; }
    }

    public class ListDiscountItem
    {
        public int SeqNo { get; set; }
        public string DiscountType { get; set; }
        public decimal LaborDiscPct { get; set; }
        public decimal PartDiscPct { get; set; }
        public decimal MaterialDiscPct { get; set; }
    }

    public class ListSupplier
    {
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
    }
}