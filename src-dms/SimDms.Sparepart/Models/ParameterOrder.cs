using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SimDms.Common;


namespace SimDms.Sparepart.Models
{
    public class spMstItemInfoExport
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string PartNo { get; set; }
        public string SupplierCode { get; set; }
        public string PartName { get; set; }
         public string PartCategory { get; set; }
     }




    [Table("spMstOrderParamView")]
    public class spMstOrderParamView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string MovingCode { get; set; }
        public string MovingCodeName { get; set; }
        public decimal? LeadTime { get; set; }
        public decimal? OrderCycle { get; set; }
        public decimal? SafetyStock { get; set; }
 
     

    }

        [Table("sp_spMstOrderParamLookup")]
    public class spMstOrderParamLookup
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string MovingCode { get; set; }
        public string MovingCodeName { get; set; }
        public decimal? LeadTime { get; set; }
        public decimal? OrderCycle { get; set; }
        public decimal? SafetyStock { get; set; }
 
     

    }
    
    [Table("spMstOrderParam")]
    public class spMstOrderParam : BaseTable
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string SupplierCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string MovingCode { get; set; }
        public decimal? LeadTime { get; set; }
        public decimal? OrderCycle { get; set; }
        public decimal? SafetyStock { get; set; }

    }

    [Table("GnMstSupplier")]
    public class gnMstSupplierView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string PhoneNo { get; set; }
        public string FaxNo { get; set; }
        public string ProvinceCode { get; set; }
        public string AreaCode { get; set; }
        public string CityCode { get; set; }
        public string ZipNo { get; set; }
    }

}