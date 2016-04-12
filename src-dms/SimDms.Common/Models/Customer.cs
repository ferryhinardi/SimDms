using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Common.Models
{
    [Table("GnMstCustomer")]
    public class GnMstCustomer
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
    } 

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

    //[Table("SvCustomerView")]
    //public class CustomerView
    //{
    //    [Key]
    //    [Column(Order = 1)]
    //    public string CompanyCode { get; set; }
    //    [Key]
    //    [Column(Order = 2)]
    //    public string CustomerCode { get; set; }
    //    public string CustomerName { get; set; }
    //    public string CompanyName { get; set; }
    //    public string Address { get; set; }
    //    public string Address1 { get; set; }
    //    public string Address2 { get; set; }
    //    public string Address3 { get; set; }
    //    public string Address4 { get; set; }
    //    public string PhoneNo { get; set; }
    //    public string HPNo { get; set; }
    //    public string NPWPNo { get; set; }
    //    public DateTime NPWPDate { get; set; }
    //    public string SKPNo { get; set; }
    //    public DateTime SKPDate { get; set; }
    //    public DateTime? BirthDate { get; set; }
    //}

    //[Table("SvTrnSenderDealerView")]
    //public class SenderDealerView
    //{
    //    [Key]
    //    [Column(Order = 1)]
    //    public string CompanyCode { get; set; }
    //    [Key]
    //    [Column(Order = 2)]
    //    public string BranchCode { get; set; }
    //    [Key]
    //    [Column(Order = 3)]
    //    public string CustomerCode { get; set; }
    //    public string CustomerName { get; set; }
    //    public string CustomerAbbrName { get; set; }
    //    public string Address { get; set; }
    //    public string Address1 { get; set; }
    //    public string Address2 { get; set; }
    //    public string Address3 { get; set; }
    //    public string Address4 { get; set; }
    //    public string CityCode { get; set; }
    //    public string CityCodeDesc { get; set; }
    //    public string PhoneNo { get; set; }
    //    public string HPNo { get; set; }
    //    public string FaxNo { get; set; }
    //    public string ProfitCenterCode { get; set; }
    //    public string ProfitCenter { get; set; }
    //    public string Status { get; set; }

    //}

    public class GnMstZipCodeView
    {
        //public string CompanyCode { get; set; }
        public string ZipCode { get; set; }
        public string KelurahanDesa { get; set; }
        public string KecamatanDistrik { get; set; }
        public bool isCity { get; set; }
        public string KotaKabupaten { get; set; }
        public string IbuKota { get; set; }
        public string Notes { get; set; }
        public string CityCode { get; set; }
        public string AreaCode { get; set; } 
    }

    [Table("GnMstZipCode")]
    public class GnMstZipCode
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ZipCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string KelurahanDesa { get; set; }
        [Key]
        [Column(Order = 4)]
        public string KecamatanDistrik { get; set; }
        public bool isCity { get; set; }
        [Key]
        [Column(Order = 6)]
        public string KotaKabupaten { get; set; }
        [Key]
        [Column(Order = 7)]
        public string IbuKota { get; set; }
        public string Notes { get; set; }
        public bool isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class LookUpDtlview
    {
        public string LookUpValue { get; set; }
        public string ParaValue { get; set; }
        public string LookUpValueName { get; set; }
        public decimal? seqno { get; set; } 
    }

    public class AccountNoLookup
    {
        public string AccountNo { get; set; }
        public string Description { get; set; }
    }

    public class gnMstCustomerClass
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
        public string ProfitCenterCodeDesc { get; set; }
        public string ReceivableAccNoDesc { get; set; }
        public string DownPaymentAccNoDesc { get; set; }
        public string InterestAccNoDesc { get; set; }
        public string TaxOutAccNoDesc { get; set; }
        public string LuxuryTaxAccNoDesc { get; set; } 
    }
    //public class LookUpDtlview2
    //{
    //    public string LookupValue { get; set; }
    //    public string LookUpValueName { get; set; }
    //}

    public class GnMstCustomerClassView
    {
        [Key]
        [Column(Order = 1)]
        public string CustomerClass { get; set; }
        public string CustomerClassName { get; set; }
        

    }
    
    public class GnMstTaxView
    {
        public string TaxCode { get; set; }
        public decimal? TaxPct { get; set; }
        public string Description { get; set; }
    }


  
    public class GnMstCollector
    {
 
        [Key]
        [Column(Order = 1)]
        public string CollectorCode { get; set; }
        public string CollectorName { get; set; }
        public string ProfitCenterCode { get; set; }
        public string ProfitCenterNameDisc { get; set; }  
    }

    [Table("gnMstCollector")]
    public class gnMstCollector
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string CollectorCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string ProfitCenterCode { get; set; }
        public string CollectorName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }
 
    public class  EmployeeHRView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string EmployeeID { get; set; }
        public string SalesID { get; set; }
        public string ServiceID { get; set; }
        public string EmployeeName { get; set; }
        public string Email { get; set; }
        public string FaxNo { get; set; }
        public string Handphone1 { get; set; }
        public string Handphone2 { get; set; }
        public string Handphone3 { get; set; }
        public string Handphone4 { get; set; }
        public string Telephone1 { get; set; }
        public string Telephone2 { get; set; }
        public string OfficeLocation { get; set; }
        public bool? IsLinkedUser { get; set; }
        public string FullName { get; set; }
        public string RelatedUser { get; set; }
        public DateTime? JoinDate { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string Grade { get; set; }
        public string Rank { get; set; }
        public string TeamLeader { get; set; }
        public string PersonnelStatus { get; set; }
        public DateTime? ResignDate { get; set; }
        public string ResignDescription { get; set; }
        public string ResignCategory { get; set; }
        public string IdentityNo { get; set; }
        public string NPWPNo { get; set; }
        public DateTime? NPWPDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string SubDistrict { get; set; }
        public string Village { get; set; }
        public string ZipCode { get; set; }
        public string Gender { get; set; }
        public string Religion { get; set; }
        public string DrivingLicense1 { get; set; }
        public string DrivingLicense2 { get; set; }
        public string MaritalStatus { get; set; }
        public string MaritalStatusCode { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public decimal? UniformSize { get; set; }
        public string UniformSizeAlt { get; set; }
        public decimal? ShoesSize { get; set; }
        public string FormalEducation { get; set; }
        public string BloodCode { get; set; }
        public string OtherInformation { get; set; }
        public string Address { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Status { get; set; }
        public string AdditionalJob1 { get; set; }
        public string AdditionalJob2 { get; set; }
        public string DepartmentName { get; set; }
        public string PositionName { get; set; }
        public string GradeName { get; set; }
        public string AdditionalJob1Name { get; set; }
        public string AdditionalJob2Name { get; set; }
        public string RankName { get; set; }
        public string SelfPhoto { get; set; }
        public string IdentityCardPhoto { get; set; }
    }
    public class EmployeeView {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string PhoneNo { get; set; }
        public string HpNo { get; set; }
        public string FaxNo { get; set; }
        public string ProvinceCode { get; set; }
        public string AreaCode { get; set; }
        public string CityCode { get; set; }
        public string ZipNo { get; set; }
        public string TitleCode { get; set; }
        public DateTime? JoinDate { get; set; }
        public DateTime? ResignDate { get; set; }
        public string GenderCode { get; set; }
        public string BirthPlace { get; set; }
        public DateTime? BirthDate { get; set; }
        public string MaritalStatusCode { get; set; }
        public string ReligionCode { get; set; }
        public string BloodCode { get; set; }
        public string IdentityNo { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public decimal? UniformSize { get; set; }
        public decimal? ShoesSize { get; set; }
        public string FormalEducation { get; set; }
        public string PersonnelStatus { get; set; }
        public bool? IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public string Nik { get; set; }
        public string EmpPhotoID { get; set; }
        public string EmpIdentityCardID { get; set; }
        public string EmpImageID { get; set; }
        public string EmpIdentityCardImageID { get; set; }
        public string ProvinceName { get; set; }
        public string AreaName { get; set; }
        public string CityName { get; set; }
        public string TitleName { get; set; } 
    }
   
    public class OmMstRefferenceView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string RefferenceType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string RefferenceCode { get; set; }
        public string RefferenceDesc1 { get; set; }
        public string RefferenceDesc2 { get; set; }
        public string Remark { get; set; }
        public char? Status { get; set; }
        public bool? IsLocked { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }

    
    }


    public class LookupCustomerview
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string LookupValue { get; set; }
        public string ProfitCenter { get; set; }
    }

    public class GnMstDocumentView {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string DocumentType { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPrefix { get; set; }
        public string ProfitCenterCode { get; set; }
        public string DocumentYear { get; set; }
        public decimal DocumentSequence { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string ProfitCenterNameDisc { get; set; } 
    }

    public class GnMstSignatureView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProfitCenterCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string DocumentType { get; set; }
        [Key]
        [Column(Order = 5)]
        public int SeqNo { get; set; }
        public string SignName { get; set; }
        public string TitleSign { get; set; }
        public string ProfitCenterName { get; set; } 
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string DocumentName { get; set; }  
    }

    public class SysMessageBoardsView {
        public int MessageID { get; set; }
        public string MessageHeader { get; set; }
        public string MessageText { get; set; }
        public string MessageTo { get; set; }
        public string MessageTarget { get; set; }
        public string MessageParams { get; set; }
    }
     
    public class ParameterView
    {
        public string DbName { get; set; }
        public string Extensions { get; set; }
        public string Prefix { get; set; }
        public string FolderPath { get; set; }
        public string DcsUrl { get; set; }
        public string TaxUrl { get; set; } 
    }

    public class ParameterView2
    {
        public string DbName { get; set; }
        public string Extensions { get; set; }
        public string Prefix { get; set; }
        public string FolderPath { get; set; }
        public string DcsUrl { get; set; }
        public string TaxUrl { get; set; }
    }

    public class LookUpHdrs  
    {
        public string CompanyCode { get; set; }
        public string CodeID { get; set; }
        public string CodeName { get; set; }
        public decimal? FieldLength { get; set; }
        public bool? isNumber { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }


    public class FPJSignatureView
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string ProfitCenterCode { get; set; }
        public string FPJOption { get; set; }
        public string FPJOptionDescription { get; set; }
        public bool? IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LookUpValueName { get; set; } 
    }

    public class FPJseqNoview {
        public string CompanyCode { get; set; }
        public string CompanyGovName { get; set; }
        public string BranchCode { get; set; }
        public string CompanyName { get; set; }
        public int Year { get; set; }
        public long? FPJSeqNo { get; set; } 
        public int SeqNo { get; set; }
        public long? BeginTaxNo { get; set; }
        public long? EndTaxNo { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }
    public class PeriodeView
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public decimal FiscalYear { get; set; }
        public decimal FiscalMonth { get; set; }
        public decimal PeriodeNum { get; set; }
        public string PeriodeName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? StatusSparepart { get; set; }
        public int? StatusSales { get; set; }
        public int? StatusService { get; set; }
        public int? StatusFinanceAP { get; set; }
        public int? StatusFinanceAR { get; set; }
        public int? StatusFinanceGL { get; set; }
        public bool? FiscalStatus { get; set; }
        public string CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class segmentAccView
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string TipeSegAcc { get; set; }
        public string SegAccNo { get; set; }
        public string Description { get; set; }
        public string Parent { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string AccountType { get; set; }
        public string Type { get; set; } 
    }

    public class periodeView
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public decimal FiscalYear { get; set; }
        public decimal FiscalMonth { get; set; }
        public decimal PeriodeNum { get; set; }
        public string PeriodeName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string StatusSparepart { get; set; }
        public string StatusSales { get; set; }
        public string StatusService { get; set; }
        public string StatusFinanceAP { get; set; }
        public string StatusFinanceAR { get; set; }
        public string StatusFinanceGL { get; set; }
        public string FiscalStatus { get; set; }
        public string CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class CreditLimitView
    {
        public string Nomor { get; set; }
        public string ProfitCenterCode { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string PaymentBy { get; set; }
        public string CreditLimit { get; set; }
        public string SalesAmt { get; set; }
        public string ReceivedAmt { get; set; }
        public string Credit { get; set; }
        public string BalanceAmt { get; set; }
    }

    public class gnMstAccountView 
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string AccountNo { get; set; }
        public string Description { get; set; }
        public string AccountType { get; set; }
        public string Company { get; set; }
        public string Branch { get; set; }
        public string CostCtrCode { get; set; }
        public string ProductType { get; set; }
        public string NaturalAccount { get; set; }
        public string InterCompany { get; set; }
        public string Futureuse { get; set; }
        public string Consol { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Balance { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class OrganizationHdrView
    {
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAccNo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class OrganizationDtlView
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string BranchAccNo { get; set; }
        public decimal? SeqNo { get; set; }
        public string BranchName { get; set; }
        public bool? IsBranch { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class GnMstSegmentAccView 
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string TipeSegAcc { get; set; }
        public string SegAccNo { get; set; }
        public string Description { get; set; }
        public string Parent { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class UtilityView 
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public bool? IsAutoGenerate { get; set; }
        public decimal? Sequence { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string GenarateNo { get; set; } 
    }

    public class MstCustomerView
    {
        public string CustomerCode { get; set; }
        public string CustomerCodeBill { get; set; }
        public string CustomerName { get; set; }
        public string CustomerGovName { get; set; }
        public string Address { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string ProfitCenter { get; set; }
        public string PhoneNo { get; set; }
        public string HPNo { get; set; }
        public string FaxNo { get; set; }
        public bool? isPKP { get; set; }
        public string NPWPNo { get; set; }
        public DateTime? NPWPDate { get; set; }
        public string SKPNo { get; set; }
        public DateTime? SKPDate { get; set; }
        public string TopCode { get; set; }
        public string TOPCD { get; set; }
        public string TOPDesc { get; set; } 
        public string GroupPriceCode { get; set; }
        public string GroupPriceDesc { get; set; }
        public string SalesCode { get; set; } 
    }

    public class Customerbrowse
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
        public string CategoryName { get; set; }
        public string PosName { get; set; }
        public string AddressGab { get; set; }
    } 
}