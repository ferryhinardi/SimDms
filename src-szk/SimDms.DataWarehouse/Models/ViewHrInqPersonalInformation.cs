using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("ViewHrInqPersonalInformation")]
    public class ViewHrInqPersonalInformation
    {
        [Key]
        [Column(Order=1)]
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        [Key]
        [Column(Order = 2)]
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentCode { get; set; }
        public string Department { get; set; }
        public string PositionCode { get; set; }
        public string Position { get; set; }
        public string Grade { get; set; }
        public string AdditionalJob1 { get; set; }
        public string AdditionalJob2 { get; set; }
        public string PersonnelStatusCode { get; set; }
        public string PersonnelStatus { get; set; }
        public DateTime? JoinDate { get; set; }
        public string TeamLeader { get; set; }
        public DateTime? ResignDate { get; set; }
        public string ResignDescription { get; set; }
        public string MaritalStatus { get; set; }
        public string Religion { get; set; }
        public string Gender { get; set; }
        public string FormalEducation { get; set; }
        public string BirthPlace { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Telephone1 { get; set; }
        public string Telephone2 { get; set; }
        public string Handphone1 { get; set; }
        public string Handphone2 { get; set; }
        public string Handphone3 { get; set; }
        public string PinBB { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public decimal? UniformSize { get; set; }
        public string UniformSizeAlt { get; set; }
        public DateTime? PreTrainingDate { get; set; }
        public string PreTrainingScore { get; set; }
        public DateTime? PembekalanDate { get; set; }
        public string PembekalanScore { get; set; }
        public DateTime? SalesmanshipDate { get; set; }
        public string SalesmanshipScore { get; set; }
        public DateTime? OJTDate { get; set; }
        public string OJTScore { get; set; }
        public DateTime? FinalReviewDate { get; set; }
        public string FinalReviewScore { get; set; }
        public DateTime? SPSSilverDate { get; set; }
        public DateTime? SPSGoldDate { get; set; }
        public DateTime? SPSPlatinumDate { get; set; }
        public DateTime? SCBasicDate { get; set; }
        public DateTime? SCAdvanceDate { get; set; }
        public DateTime? SHBasicDate { get; set; }
        public DateTime? SHIntermediateDate { get; set; }
        public DateTime? BMBasicDate { get; set; }
        public DateTime? BMIntermediateDate { get; set; }
    }
}