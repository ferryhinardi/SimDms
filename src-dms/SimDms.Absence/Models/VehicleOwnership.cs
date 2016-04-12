using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Absence.Models
{
    [Table("HrEmployeeVehicle")]
    public class HrEmployeeVehicle
    {
        [Key]
        [Column(Order=0)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 1)]
        public string EmployeeID { get; set; }
        [Key]
        [Column(Order = 2)]
        public int VehSeq { get; set; }
        public string Type { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string PoliceRegNo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}