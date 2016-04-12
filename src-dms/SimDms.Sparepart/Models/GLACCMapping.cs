using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimDms.Sparepart.Models
{
    [Table("spMstAccount")]
    public class spMstAccount
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string TypeOfGoods { get; set; }
        public string SalesAccNo { get; set; }
        public string COGSAccNo { get; set; }
        public string InventoryAccNo { get; set; }
        public string DiscAccNo { get; set; }
        public string ReturnAccNo { get; set; }
        public string ReturnPybAccNo { get; set; }
        public string OtherIncomeAccNo { get; set; }
        public string OtherReceivableAccNo { get; set; }
        public string InTransitAccNo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }


    }

    [Table("spMstAccountView")]
    public class spMstAccountView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string TypeOfGoods { get; set; }
        public string NameOfGoods { get; set; }
        public string SalesAccNo { get; set; }
        public string COGSAccNo { get; set; }
        public string InventoryAccNo { get; set; }
        public string DiscAccNo { get; set; }
        public string ReturnAccNo { get; set; }
        public string ReturnPybAccNo { get; set; }
        public string OtherIncomeAccNo { get; set; }
        public string OtherReceivableAccNo { get; set; }
        public string InTransitAccNo { get; set; }
 
 
    }




    [Table("spgnMstAccountView")]
    public class spgnMstAccountView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }

        public string AccountNo { get; set; }
        public string Description { get; set; }
        //public string AccountType { get; set; }
        //public string Company { get; set; }
        //public string Branch { get; set; }
        //public string CostCtrCode { get; set; }
        //public string ProductType { get; set; }
        //public string NaturalAccount { get; set; }
        //public string InterCompany { get; set; }
        //public string Futureuse { get; set; }
        //public string Consol { get; set; }
        //public DateTime? FromDate { get; set; }
        //public DateTime? EndDate { get; set; }
        //public string Balance { get; set; }
        //public string CreatedBy { get; set; }
        //public DateTime? CreatedDate { get; set; }
        //public string LastUpdateBy { get; set; }
        //public DateTime? LastUpdateDate { get; set; }
    }

}