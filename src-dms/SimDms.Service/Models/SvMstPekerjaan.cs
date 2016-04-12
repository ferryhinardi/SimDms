using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    //table
    [Table("svMstJob")]
    public class svMstJob
    {

        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BasicModel { get; set; }
        [Key]
        [Column(Order = 4)]
        public string JobType { get; set; }
        public string GroupJobType { get; set; }
        public bool? IsPdiFsc { get; set; }
        public decimal? PdiFscSeq { get; set; }
        public decimal? WarrantyOdometer { get; set; }
        public decimal? WarrantyTimePeriod { get; set; }
        public string WarrantyTimeDim { get; set; }
        public int? CounterOperationNo { get; set; }
        public string ReceivableAccountNo { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
    }
    /*[Table("svMstTask")]
    public class svMstTask
    {

        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BasicModel { get; set; }
        [Key]
        [Column(Order = 4)]
        public string JobType { get; set; }
        [Key]
        [Column(Order = 5)]
        public string OperationNo { get; set; }
        public string Description { get; set; }
        public decimal? OperationHour { get; set; }
        public decimal? ClaimHour { get; set; }
        public decimal? LaborCost { get; set; }
        public decimal? LaborPrice { get; set; }
        public string TechnicalModelCode { get; set; }
        public bool? IsSubCon { get; set; }
        public bool IsCampaign { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string BillType { get; set; }


    }*/
    [Table("svMstTaskPart")]
    public class svMstTaskPart
    {

        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BasicModel { get; set; }
        [Key]
        [Column(Order = 4)]
        public string JobType { get; set; }
        [Key]
        [Column(Order = 5)]
        public string OperationNo { get; set; }
        [Key]
        [Column(Order = 6)]
        public string PartNo { get; set; }
        public decimal? Quantity { get; set; }
        public decimal RetailPrice { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string BillType { get; set; }



    }

    [Table("svMstTaskPrice")]
    public class svMstTaskPrice
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 4)]
        public string BasicModel { get; set; }
        [Key]
        [Column(Order = 5)]
        public string JobType { get; set; }
        [Key]
        [Column(Order = 6)]
        public string OperationNo { get; set; }
        public decimal? OperationHour { get; set; }
        public decimal? ClaimHour { get; set; }
        public decimal? LaborCost { get; set; }
        public decimal? LaborPrice { get; set; }
    }

    //view
    [Table("SvPekerjaanView")]
    public class SvMstPekerjaanBrowse
    {

        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BasicModel { get; set; }
        [Key]
        [Column(Order = 4)]
        public string JobType { get; set; }
        public string JobDescription { get; set; }
        public string GroupJobType { get; set; }
        public string GroupJobDescription { get; set; }
        public decimal? WarrantyOdometer { get; set; }
        public decimal? WarrantyTimePeriod { get; set; }
        public string WarrantyTimeDimStr { get; set; }
        public string WarrantyTimeDim { get; set; }
        public string IsPdiFscStr { get; set; }
        public bool? IsPdiFsc { get; set; }
        public string PdiFscSeq { get; set; }
        public string Status { get; set; }
        public bool? IsActive { get; set; } 

    }

    [Table("SvBasicModelPekerjaan")]
    public class SvBasicModelPekerjaan
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BasicModel { get; set; }
        public string TechnicalModelCode { get; set; }
        public string ModelDescription { get; set; }
        public string Status { get; set; }


    }
    [Table("SvJobView")]
    public class SvJobView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string RefferenceType { get; set; }
        [Key]
        [Column(Order = 4)]
        public string JobType { get; set; }
        public string JobDescription { get; set; }
        public string DescriptionEng { get; set; }
        public string Status { get; set; }




    }
    [Table("SvGroupJobView")]
    public class SvGroupJobView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string RefferenceType { get; set; }
        [Key]
        [Column(Order = 4)]
        public string GroupJobType { get; set; }
        public string GroupJobDescription { get; set; }
        public string DescriptionEng { get; set; }
        public string IsActive { get; set; }
    }

    [Table("SvNomorAccView")]
    public class SvNomorAccView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string AccountNo { get; set; }
        public string AccDescription { get; set; }

    }
    [Table("SvRincinJob")]
    public class SvRincianJob
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BasicModel { get; set; }
        [Key]
        [Column(Order = 4)]
        public string JobType { get; set; }
        [Key]
        [Column(Order = 5)]
        public string OperationNo { get; set; }
        public string Description { get; set; }
        public string TechnicalModelCode { get; set; }
        public bool IsActive { get; set; } 
        public bool IsCampaign { get; set; }
        public bool? IsSubCon { get; set; }
        public decimal? OperationHour { get; set; }
        public decimal? ClaimHour { get; set; }
        public string BillType { get; set; }
        public decimal? LaborCost { get; set; }
        public decimal? LaborPrice { get; set; }
      //  public string GroupJobType { get; set; }

    }
    [Table("SvRincianBrowser")]
    public class SvRincianBrowser
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BasicModel { get; set; }
        [Key]
        [Column(Order = 4)]
        public string JobType { get; set; }
        [Key]
        [Column(Order = 5)]
        public string OperationNo { get; set; }
        public string Description { get; set; }
        public string TechnicalModelCode { get; set; }
        public string IsSubConStr { get; set; }
        public string IsCampaignStr { get; set; }
        public string Status { get; set; }
        public bool IsActiveR { get; set; }
        public bool IsCampaign { get; set; }
        public bool? IsSubCon { get; set; }
        public decimal? OperationHour { get; set; }
        public decimal? ClaimHour { get; set; }
        public string BillType { get; set; }
        public decimal? LaborCost { get; set; }
        public decimal? LaborPrice { get; set; }


    }
    [Table("SvRincianPart")]
    public class SvRincianPart
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string BasicModel { get; set; }
        [Key]
        [Column(Order = 5)]
        public string OperationNo { get; set; }
        [Key]
        [Column(Order = 6)]
        public string JobType { get; set; }
        [Key]
        [Column(Order = 7)]
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public decimal? Quantity { get; set; }
        public decimal RetailPrice { get; set; }
        public string GroupJobType { get; set; }
        public string BillTypePart { get; set; }  
        public string BillTypeDesc { get; set; }
        public decimal? PdiFscSeq { get; set; }



    }
    [Table("SvPartNoView")]
    public class SvPartNoView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 4)]
        public string PartNo { get; set; }
        public decimal? Available { get; set; }
        public decimal? RetailPriceInclTax { get; set; }
        public string PartName { get; set; }
        public string TypeOfGoods { get; set; }
        public string GroupTypeOfGoods { get; set; }
        public string Status { get; set; }
        public decimal? NilaiPart { get; set; }

    }
    [Table("SvBasicCopy")]
    public class SvBasicCopy
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BasicModel { get; set; }
        public string TechnicalModelCode { get; set; }
        public string ModelDescription { get; set; }
        public string Status { get; set; }


    }
    public class NoPart
    {
        [Key]
        [Column(Order = 1)]
        public string PartNo { get; set; }
        public decimal? Available { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? RetailPriceInclTax { get; set; }
        public string PartName { get; set; }
        public string TypeOfGoods { get; set; }
        public string GroupTypeOfGoods { get; set; }
        public string Status { get; set; }
        public decimal? Price { get; set; }
    }

    [Table("svMstTask")]
    public class  svMstTask
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BasicModel { get; set; }
        [Key]
        [Column(Order = 4)]
        public string JobType { get; set; }
        [Key]
        [Column(Order = 5)]
        public string OperationNo { get; set; }
        public string Description { get; set; }
        public decimal? OperationHour { get; set; }
        public decimal? ClaimHour { get; set; }
        public decimal? LaborCost { get; set; }
        public decimal? LaborPrice { get; set; }
        public string TechnicalModelCode { get; set; }
        public bool? IsSubCon { get; set; }
        public bool IsCampaign { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string BillType { get; set; }
    }

    public class svMstTaskview 
    {
        public string ProductType { get; set; }
        public string BasicModel { get; set; }
        public string JobType { get; set; }
        public string OperationNo { get; set; }
        public string Description { get; set; }
        public decimal? OperationHour { get; set; }
        public decimal? ClaimHour { get; set; }
        public decimal? LaborCost { get; set; }
        public decimal? LaborPrice { get; set; }
        public string TechnicalModelCode { get; set; }
        public string IsSubCon { get; set; }
        public string IsCampaign { get; set; }
        public string IsActive { get; set; }
        public string BillType { get; set; }
    }

    public class svMstJobView 
    {
        public string CompanyCode { get; set; }
        public string ProductType { get; set; }
        public string BasicModel { get; set; }
        public string JobType { get; set; }
        public string JobDescription { get; set; } 
        public string GroupJobType { get; set; }
        public string GroupJobDescription { get; set; }
        public bool? IsPdiFsc { get; set; }
        public string IsPdiFscStr { get; set; }
        public decimal? PdiFscSeq { get; set; }
        public decimal? WarrantyOdometer { get; set; }
        public decimal? WarrantyTimePeriod { get; set; }
        public string WarrantyTimeDim { get; set; }
        public int? CounterOperationNo { get; set; }
        public string ReceivableAccountNo { get; set; }
        public string AccDescription { get; set; } 
        public bool? IsActive { get; set; }
        public string Status { get; set; }
        public bool? IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }

    }

    public class NomorAccount
    {
        public string SalesAccNo { get; set; }
        public string DiscAccNo { get; set; }
        public string ReturnAccNo { get; set; }
        public string COGSAccNo { get; set; }
        public string ReturnPybAccNo { get; set; }
    }

    public class OutstandingSubCon
    {
        public Int64? ServiceNo { get; set; }
        public String JobOrderNo { get; set; }
        public Decimal? OldCost { get; set; }
        public Decimal? NewCost { get; set; }
        public Boolean? IsSelected { get; set; }
    }
}