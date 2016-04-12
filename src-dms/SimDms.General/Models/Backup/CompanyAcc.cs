using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SimDms.Common;


namespace SimDms.Sparepart.Models
{
    [Table("spMstCompanyAccount")]
    public class spMstCompanyAccount : BaseTable
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CompanyCodeTo { get; set; }
        public string CompanyCodeToDesc { get; set; }
        public string BranchCodeTo { get; set; }
        public string BranchCodeToDesc { get; set; }
        public string WarehouseCodeTo { get; set; }
        public string WarehouseCodeToDesc { get; set; }
        public string UrlAddress { get; set; }
        public bool isActive { get; set; }
 
       }


    [Table("spMstCompanyAccountdtl")]
    public class spMstCompanyAccountdtl 
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CompanyCodeTo { get; set; }
        [Key]
        [Column(Order = 3)]
        public string TPGO { get; set; }
        public string InterCompanyAccNoTo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }

    }


    public class spMstCompanyAccountdtl2
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CompanyCodeTo { get; set; }
        [Key]
        [Column(Order = 3)]
        public string TPGO { get; set; }
        public string AccountNo { get; set; }
        //public string CreatedBy { get; set; }
        //public DateTime CreatedDate { get; set; }
        //public string LastUpdateBy { get; set; }
        //public DateTime LastUpdateDate { get; set; }

    }

    [Table("sp_spMstCompanyAccountDtl")]
    public class spMstCompanyAccountdtlview
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CompanyCodeTo { get; set; }
        public string TPGO { get; set; }
        public string TPGOName { get; set; }
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
 
   
    }
    public class sp_spMstCompanyAccount
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        public string CompanyCodeTo { get; set; }
        public string CompanyCodeToDesc { get; set; }
        public string BranchCodeTo { get; set; }
        public string BranchCodeToDesc { get; set; }
        public string WarehouseCodeTo { get; set; }
        public string WarehouseCodeToDesc { get; set; }
        public string UrlAddress { get; set; }
        public bool isActive { get; set; }
 
    }

   
    public class spgnMstAccountViewAccX
    {

   
        public string AccountNo { get; set; }
        public string Description { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
 
    }

    public class LookUpDtlView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CodeID { get; set; }
        [Key]
        [Column(Order = 3)]
        public string LookUpValue { get; set; }
        public string LookUpValueName { get; set; }

    }

    public class LookUpBranchView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }

        public string CompanyName { get; set; }

    }

    public class LookUpCompanyView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }

    }
}