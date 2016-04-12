using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("OmTrInventQtyVehicle")]
    public class OmTrInventQtyVehicle
    {
        public OmTrInventQtyVehicle()
        {
            this.QtyIn = 0;
            this.Alocation = 0;
            this.QtyOut = 0;
            this.BeginningOH = 0;
            this.EndingOH = 0;
            this.BeginningAV = 0;
            this.EndingAV = 0;
        }

        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public decimal Year { get; set; }
        [Key]
        [Column(Order = 4)]
        public decimal Month { get; set; }
        [Key]
        [Column(Order = 5)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 6)]
        public decimal SalesModelYear { get; set; }
        [Key]
        [Column(Order = 7)]
        public string ColourCode { get; set; }
        [Key]
        [Column(Order = 8)]
        public string WarehouseCode { get; set; }
        public decimal? QtyIn { get; set; }
        public decimal? Alocation { get; set; }
        public decimal? QtyOut { get; set; }
        public decimal? BeginningOH { get; set; }
        public decimal? EndingOH { get; set; }
        public decimal? BeginningAV { get; set; }
        public decimal? EndingAV { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? IsLocked { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockedDate { get; set; }

    }

    public class InquiryInventQtyVehicleView
    {
        public string Year { get; set; }
        public string Month { get; set; }
        public string SalesModelCode { get; set; }
        public string SalesModelDesc { get; set; }
        public string ModelYear { get; set; }
        public string ColourCode { get; set; }
        public string ColourName { get; set; }
        public string WarehouseCode { get; set; }
        public string WareHouseName { get; set; }
        public decimal? QtyIn { get; set; }
        public decimal? Alocation { get; set; }
        public decimal? QtyOut { get; set; }
        public decimal? BeginningAV { get; set; }
        public decimal? EndingAV { get; set; }
    }
}