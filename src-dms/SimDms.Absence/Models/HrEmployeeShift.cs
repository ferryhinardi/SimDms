using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Absence.Models
{
    [Table("HrEmployeeShift")]
    public class HrEmployeeShift
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string EmployeeID { get; set; }
        [Key]
        [Column(Order = 3)]
        public string AttdDate { get; set; }
        public string ShiftCode { get; set; }
        public string ClockInTime { get; set; }
        public string ClockOutTime { get; set; }
        public string OnDutyTime { get; set; }
        public string OffDutyTime { get; set; }
        public string OnRestTime { get; set; }
        public string OffRestTime { get; set; }
        public decimal? CalcOvertime { get; set; }
        public decimal? ApprOvertime { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }

    [Table("HrEmployeeShiftView")]
    public class HrEmployeeShiftView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string EmployeeID { get; set; }
        [Key]
        [Column(Order = 3)]
        public string AttdDate { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string DepartmentName { get; set; }
        public string Position { get; set; }
        public string PositionName { get; set; }
        public string Grade { get; set; }
        public string Rank { get; set; }
        public string ShiftCode { get; set; }
        public string Shift { get; set; }
        public string ClockInTime { get; set; }
        public string ClockOutTime { get; set; }
        public string OnDutyTime { get; set; }
        public string OffDutyTime { get; set; }
        public string OnRestTime { get; set; }
        public string OffRestTime { get; set; }
        public decimal? CalcOvertime { get; set; }
        public decimal? ApprOvertime { get; set; }
    }

    public class HrEmployeeShiftModel
    {
        public string CompanyCode { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string IsHaveShift { get; set; }
        public string Shift { get; set; }
        public string OnDutyTime { get; set; }
        public string OnRestTime { get; set; }
        public string OffDutyTime { get; set; }
        public string OffRestTime { get; set; }
    }
}