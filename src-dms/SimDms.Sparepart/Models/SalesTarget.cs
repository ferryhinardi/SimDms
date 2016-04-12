using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimDms.Sparepart.Models
{
    [Table("spMstSalesTarget")]
    public class spMstSalesTarget
    {
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
        public decimal? QtyTarget { get; set; }
        public decimal? AmountTarget { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public decimal? TotalJaringan { get; set; }
       }


    public class spMstSalesTargetview
    {
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
        public decimal? QtyTarget { get; set; }
        public decimal? AmountTarget { get; set; }
        public decimal? TotalJaringan { get; set; }
 

    }


    [Table("spMstSalesTargetDtl")]
    public class spMstSalesTargetDtl
    {
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
        public string CategoryCode { get; set; }
        public decimal? QtyTarget { get; set; }
        public decimal? AmountTarget { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public decimal? TotalJaringan { get; set; }
    }

    public class sp_spCategoryCodeview
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
            
    }

    public class sp_spMstSalesTargetDtlview
    {
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
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public decimal? QtyTarget { get; set; }
        public decimal? AmountTarget { get; set; }
        public decimal? TotalJaringan { get; set; }

    }

    public class sp_spMstSalesTargetDtlviewSum
    {
        public decimal? sumQtyTarget { get; set; }
        public decimal? sumAmountTarget { get; set; }
        public decimal? sumTotalJaringan { get; set; }
    }

    
}