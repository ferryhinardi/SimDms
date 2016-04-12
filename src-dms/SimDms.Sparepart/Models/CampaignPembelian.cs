using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimDms.Sparepart.Models
{
    [Table("spMstPurchCampaign")]
    public class spMstPurchCampaign
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
        public string PartNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public DateTime BegDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? DiscPct { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }

    }

    [Table("spMstPurchCampaignView")]
    public class spMstPurchCampaignView
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
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public decimal? DiscPct { get; set; }
        public DateTime BegDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SupplierName { get; set; }
 
    }


}