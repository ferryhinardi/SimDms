using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Absence.Models
{
    [Table("HrEmployeeEducation")]
    public class HrEmployeeEducation
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string EmployeeID { get; set; }
        public int EduSeq { get; set; }
        [Key]
        [Column(Order = 3)]
        public string College { get; set; }
        public string Education { get; set; }
        public string YearBegin { get; set; }
        public string YearFinish { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
    }

    public class HrEmployeeEducationModel
    {
        public string EmployeeID { get; set; }
        public string HistoryEducationCollege { get; set; }
        public string HistoryEducationEducation { get; set; }
        public string HistoryEducationYearBegin { get; set; }
        public string HistoryEducationYearFinish { get; set; }
    }
}