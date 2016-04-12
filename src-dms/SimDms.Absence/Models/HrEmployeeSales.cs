using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Absence.Models
{
    [Table("HrEmployeeSales")]
    public class HrEmployeeSales
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string EmployeeID { get; set; }
        public string SalesID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class ATPMResultModel
    {
        public bool status { get; set; }
        public string atpmID { get; set; }
        public string message { get; set; }
    }

    public class SalesIDModel
    {
        public bool success { get; set; }
        public SalesIDDetailsModel data { get; set; }
    }


    public class SalesIDModel2
    {
        public int Nomor { get; set; } 
    }

    public class SalesIDDetailsModel
    {
        public string DocID { get; set; }
        public string DocCode { get; set; }
        public string DocSeq { get; set; }
        public string CompanyCode { get; set; }
        public string ClientIP { get; set; }
        public string HostName { get; set; }
        public string HostAddress { get; set; }
        public string LogonName { get; set; }
        public string TokenNo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}