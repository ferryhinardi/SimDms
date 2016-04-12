using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("omTrInventTransferInDetail")]
    public class omTrInventTransferInDetail
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string TransferInNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public decimal TransferInSeq { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string ColourCode { get; set; }
        public string Remark { get; set; }
        public string StatusTransferOut { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class TransferOutDetailView
    {
        public string TransferInNo { get; set; }
        public string TransferInSeq { get; set; }
        public string SalesModelCode { get; set; }
        public string SalesModelYear { get; set; }
        public string SalesModelDesc { get; set; }
        public string ChassisCode { get; set; }
        public string ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public string EngineNo { get; set; }
        public string ColourCode { get; set; }
        public string ColourName { get; set; }
        public string Remark { get; set; }
    }
}