using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("OutletInfo")]
    public class OutletInfo
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string ShortBranchName { get; set; }
    }

    [Table("svMstDealerOutletMapping")]
    public class svMstDealerOutletMapping
    {
        [Key]
        [Column(Order = 1)]
        public string DealerCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string OutletCode { get; set; }
        public string OutletName { get; set; }
    }

    [Table("svMasterDealerOutletMapping")]
    public class svMasterDealerOutletMapping
    {
        [Key]
        [Column(Order = 1)]
        public string GNDealerCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string GNOutletCode { get; set; }
        public string DealerCode { get; set; }
        public string OutletCode { get; set; }
        public string OutletName { get; set; }
    }

    [Table("svMasterDealerMapping")]
    public class svMasterDealerMapping
    {
        [Key]
        [Column(Order = 1)]
        public long SeqNo { get; set; }
        public int? GroupNo { get; set; }
        public string Area { get; set; }
        public string DealerName { get; set; }
        public string DealerAbbreviation { get; set; }
        public string DealerCode { get; set; }
        public string GNDealerCode { get; set; }
        public bool? isActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? IsDealerSrv { get; set; }
    }
}