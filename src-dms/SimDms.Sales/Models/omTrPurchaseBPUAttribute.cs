using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimDms.Sales.Models
{
        [Table("omTrPurchaseBPUAttribute")]
        public class OmTrPurchaseBPUAttribute  
        {
            public OmTrPurchaseBPUAttribute(){
                this.Subsidi1 = 0;
                this.Subsidi2 = 0;
                this.Subsidi3 = 0;
                this.Subsidi4 = 0;
                this.PotSKP = 0;
                this.Kompensasi = 0;
                this.HargaAccessories = 0;
                this.AttributeNum1 = 0;
                this.AttributeNum2 = 0;
                this.AttributeNum3 = 0;
            }

            [Key]
            [Column(Order = 1)]
            public string CompanyCode { get; set; }
            [Key]
            [Column(Order = 2)]
            public string BranchCode { get; set; }
            [Key]
            [Column(Order = 3)]
            public string BPUNo { get; set; }
            [Key]
            [Column(Order = 4)]
            public string DONo { get; set; }
            public string Status { get; set; }
            public string FakturPolisiNo { get; set; }
            public DateTime? FakturPolisiDate { get; set; }
            public string WareHouseCode { get; set; }
            public decimal? Subsidi1 { get; set; }
            public decimal? Subsidi2 { get; set; }
            public decimal? Subsidi3 { get; set; }
            public decimal? Subsidi4 { get; set; }
            public decimal? PotSKP { get; set; }
            public decimal? Kompensasi { get; set; }
            public decimal? HargaAccessories { get; set; }
            public string AttributeChar1 { get; set; }
            public decimal? AttributeNum1 { get; set; }
            public DateTime? AttributeDate1 { get; set; }
            public string AttributeChar2 { get; set; }
            public decimal? AttributeNum2 { get; set; }
            public DateTime? AttributeDate2 { get; set; }
            public string AttributeChar3 { get; set; }
            public decimal? AttributeNum3 { get; set; }
            public DateTime? AttributeDate3 { get; set; }
            public string CreateBy { get; set; }
            public DateTime? CreateDate { get; set; }
            public string LastUpdateBy { get; set; }
            public DateTime? LastUpdateDate { get; set; }
        }

        public class BPUAttrView
        {
            public string CompanyCode { get; set; }
            public string BranchCode { get; set; }
            public string BPUNo { get; set; }
            public string DONo { get; set; }
            public string Status { get; set; }
            public string FakturPolisiNo { get; set; }
            public DateTime? FakturPolisiDate { get; set; }
            public string WareHouseCode { get; set; }
            public decimal? Subsidi1 { get; set; }
            public decimal? Subsidi2 { get; set; }
            public decimal? Subsidi3 { get; set; }
            public decimal? Subsidi4 { get; set; }
            public decimal? PotSKP { get; set; }
            public decimal? Kompensasi { get; set; }
            public decimal? HargaAccessories { get; set; }
            public string AttributeChar1 { get; set; }
            public decimal? AttributeNum1 { get; set; }
            public DateTime? AttributeDate1 { get; set; }
            public string AttributeChar2 { get; set; }
            public decimal? AttributeNum2 { get; set; }
            public DateTime? AttributeDate2 { get; set; }
            public string AttributeChar3 { get; set; }
            public decimal? AttributeNum3 { get; set; }
            public DateTime? AttributeDate3 { get; set; }
            public string CreateBy { get; set; }
            public DateTime? CreateDate { get; set; }
            public string LastUpdateBy { get; set; }
            public DateTime? LastUpdateDate { get; set; }
            public string WarehouseName { get; set; }
        }
}
