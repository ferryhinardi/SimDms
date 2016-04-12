using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace SimDms.Absence.Models
{
    [Table("HrEmployeeAdditionalJob")]
    public class HrEmployeeAdditionalJob
    {
        [Key]
        [Column(Order=0)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order=1)]
        public string EmployeeID { get; set; }
        [Key]
        [Column(Order = 2)]
        public int SeqNo { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string Grade { get; set; }
        public DateTime? AssignDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
    }
}