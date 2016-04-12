using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("omMstModelAccount")]
    public class MstModelAccount
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string SalesModelCode { get; set; }
        public string SalesAccNo { get; set; }
        public string DiscountAccNo { get; set; }
        public string ReturnAccNo { get; set; }
        public string COGsAccNo { get; set; }
        public string InventoryAccNo { get; set; }
        public string SalesAccNoAks { get; set; }
        public string ReturnAccNoAks { get; set; }
        public string COGsAccNoAks { get; set; }
        public string InventoryAccNoAks { get; set; }
        public string BBNAccNo { get; set; }
        public string KIRAccNo { get; set; }
        public string Remark { get; set; }
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string HReturnAccNo { get; set; }
        public string PReturnAccNo { get; set; }
        public string DiscountAccNoAks { get; set; }
        public string ShipAccNo { get; set; }
        public string DepositAccNo { get; set; }
        public string OthersAccNo { get; set; }
        public string InTransitTransferStockAccNo { get; set; }
    }

    public class ModelAccountBrowse
    {
        public string SalesModelCode { get; set; }
        public string SalesAccNo { get; set; }
        public string DiscountAccNo { get; set; }
        public string ReturnAccNo { get; set; }
        public string COGsAccNo { get; set; }
        public string InventoryAccNo { get; set; }
        public string SalesAccNoAks { get; set; }
        public string ReturnAccNoAks { get; set; }
        public string COGsAccNoAks { get; set; }
        public string InventoryAccNoAks { get; set; }
        public string BBNAccNo { get; set; }
        public string KIRAccNo { get; set; }
        public string Remark { get; set; }
        public bool? IsActive { get; set; }
        public string HReturnAccNo { get; set; }
        public string PReturnAccNo { get; set; }
        public string DiscountAccNoAks { get; set; }
        public string ShipAccNo { get; set; }
        public string DepositAccNo { get; set; }
        public string OthersAccNo { get; set; }
        public string InTransitTransferStockAccNo { get; set; }
        public string SalesModelDesc { get; set; }
        public string SalesAccDesc { get; set; }
        public string DiscountAccDesc { get; set; }
        public string ReturnAccDesc { get; set; }
        public string COGsAccDesc { get; set; }
        public string HReturnAccDesc { get; set; }
        public string SalesAccDescAks { get; set; }
        public string ReturnAccDescAks { get; set; }
        public string DiscountAccDescAks { get; set; }
        public string ShipAccDesc { get; set; }
        public string DepositAccDesc { get; set; }
        public string OthersAccDesc { get; set; }
        public string BBNAccDesc { get; set; }
        public string KIRAccDesc { get; set; }
        public string PReturnAccDesc { get; set; }
        public string IntransitAccDesc { get; set; }
        public string InventoryAccDesc { get; set; }
    }
}