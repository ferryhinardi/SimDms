using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.DataWarehouse.Models
{
    [Table("GroupArea")]
    public class GroupArea
    {
        [Key]
        public string GroupNo { get; set; }
        public string AreaDealer { get; set; }
    }

    [Table("GroupArea2W")]
    public class GroupArea2W
    {
        [Key]
        public string GroupNo { get; set; }
        public string AreaDealer { get; set; }
    }

    [Table("SrvGroupArea")]
    public class SrvGroupArea
    {
        [Key]
        public string GroupNo { get; set; }
        public string AreaDealer { get; set; }
        public string GNGroupNo { get; set; }
    }

    public class ParamValue
    {
        public string value { get; set; }
        public string text { get; set; }
    }

    public class GroupAreaView
    {
        public int groupNo { get; set; }
        public string area { get; set; }
        public int orders { get; set; }
    }

    public class DealerV2View
    {
        public int groupNo { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
    }

    public class DealerV3View
    {
        public string groupNo { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
    }

    public class ListOutlet
    {
        public string OutletCode { get; set; }
        public string OutletName { get; set; }
    }

    public class BranchV2View
    {
        public string DealerCode { get; set; }
        public string OutletCode { get; set; }
        public string CompanyName { get; set; }
    }

    public class ParamValueTipeKendaraan
    {
        public decimal GroupCodeSeq { get; set; }
        public string GroupCode { get; set; }
        public string GroupModel { get; set; }
    }

}