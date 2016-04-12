using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("svMstAccount")]
    public class svMstAccount
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
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }


    }
}