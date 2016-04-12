﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Models
{
    [Table("GnMstSupplierClass")]
    public class GnMstSupplierClass
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string SupplierClass { get; set; }
        [Key]
        [Column(Order = 4)]
        public string TypeOfGoods { get; set; }
        public string ProfitCenterCode { get; set; }
        public string SupplierClassName { get; set; }
        public string PayableAccNo { get; set; }
        public string DownPaymentAccNo { get; set; }
        public string InterestAccNo { get; set; }
        public string OtherAccNo { get; set; }
        public string TaxInAccNo { get; set; }
        public string WitholdingTaxAccNo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }

    [Table("GnMstSupplier")]
    public class GnMstSupplier
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string SupplierCode { get; set; }
        public string StandardCode { get; set; }
        public string SupplierName { get; set; }
        public string SupplierGovName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string PhoneNo { get; set; }
        public string HPNo { get; set; }
        public string FaxNo { get; set; }
        public string ProvinceCode { get; set; }
        public string AreaCode { get; set; }
        public string CityCode { get; set; }
        public string ZipNo { get; set; }
        public bool? isPKP { get; set; }
        public string NPWPNo { get; set; }
        public DateTime? NPWPDate { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }
    public class GnMstSupplierView
    {
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
    }

    public class SupplierDetails
    {
        public string CompanyCode { get; set; }
        public string SupplierCode { get; set; }
        public string StandardCode { get; set; }
        public string SupplierName { get; set; }
        public string SupplierGovName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string PhoneNo { get; set; }
        public string HPNo { get; set; }
        public string FaxNo { get; set; }
        public string ProvinceCode { get; set; }
        public string AreaCode { get; set; }
        public string CityCode { get; set; }
        public string ZipNo { get; set; }
        public bool? isPKP { get; set; }
        public string NPWPNo { get; set; }
        public DateTime? NPWPDate { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string ProvinceName { get; set; }
        public string AreaName { get; set; }
        public string CityName { get; set; }
    }

    public class SupplierClassView
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string SupplierClass { get; set; }
        public string TypeOfGoods { get; set; }
        public string ProfitCenterCode { get; set; }
        public string SupplierClassName { get; set; }
        public string PayableAccNo { get; set; }
        public string DownPaymentAccNo { get; set; }
        public string InterestAccNo { get; set; }
        public string OtherAccNo { get; set; }
        public string TaxInAccNo { get; set; }
        public string WitholdingTaxAccNo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }
}