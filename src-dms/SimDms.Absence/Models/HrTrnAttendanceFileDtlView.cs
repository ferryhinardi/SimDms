using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Absence.Models
{
    [Table("HrTrnAttendanceFileDtlView")]
    public class HrTrnAttendanceFileDtlView
    {
        [Key]
        [Column(Order = 0)]
        public string FileID { get; set; }
        [Key]
        [Column(Order=1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        [Key]
        [Column(Order = 3)]
        public string MachineCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public DateTime? AttendanceTime { get; set; }
        [Key]
        [Column(Order = 5)]
        public string IdentityCode { get; set; }
        public string AttendanceStatus { get; set; }
        public string AttendanceDate { get; set; }
        public string Shift { get; set; }
        public string Status { get; set; }
        public string ClockTime { get; set; }
    }
}