using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Absence.Models
{
    [Table("HrTrnAttendanceFileHdr")]
    public class HrTrnAttendanceFileHdr
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string GenerateId { get; set; }
        public string FileID { get; set; }
        public int IsTransfered { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    [Table("HrTrnAttendanceFileHdrView")]
    public class HrTrnAttendanceFileHdrView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string FileID { get; set; }
        public string FileName { get; set; }
        public int FileSize { get; set; }
        public string Size { get; set; }
        public string FileType { get; set; }
        public string IsTransfered { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int Processed { get; set; }
        public int Unprocessed { get; set; }
    }

    public class AttendanceRecord
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string AttendanceTime { get; set; }
        public string MachineCode { get; set; }
        public string AttendanceCode { get; set; }
    }
}