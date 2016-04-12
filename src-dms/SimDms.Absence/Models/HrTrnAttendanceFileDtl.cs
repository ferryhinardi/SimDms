using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Absence.Models
{
    [Table("HrTrnAttendanceFileDtl")]
	public class HrTrnAttendanceFileDtl
	{
		[Key]
		[Column(Order=1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string GenerateId { get; set; }
        [Key]
        [Column(Order = 3)]
        public int SequenceNo { get; set; }
        public string FileID { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? AttendanceTime { get; set; }
        public string MachineCode { get; set; }
        public string IdentityCode { get; set; }
        public bool IsTransfered { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
	}
}