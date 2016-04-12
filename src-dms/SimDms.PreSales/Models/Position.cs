using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.PreSales.Models
{
    [Table("pmPosition")]
    public class Position
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string EmployeeID { get; set; }
        public string UserId { get; set; }
        public string PositionId { get; set; }
        public string OutletID { get; set; }
        public string PositionName { get; set; }
        public string CAttribute1 { get; set; }
        public string CAttribute2 { get; set; }
        public decimal? NAttribute1 { get; set; }
        public decimal? NAttribute2 { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUpdateBy { get; set; }

    }

    public class pmPosisitionView
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string UserId { get; set; }
    }

    public class PositionItem
    {
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Position { get; set; }
        public string PositionName { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Grade { get; set; }
        public string TeamLeader { get; set; }
    }
}