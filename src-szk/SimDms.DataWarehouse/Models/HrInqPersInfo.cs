using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    public class HrInqPersInfo
    {
        public string CompanyCode { get; set; }
        public string EmployeeID { get; set; }
        public string SalesID { get; set; }
        public string EmployeeName { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string DeptCode { get; set; }
        public string PosCode { get; set; }
        public string Position { get; set; }
        public string PosName { get; set; }
        public string GradeCode { get; set; }
        public string Grade { get; set; }
        public string Status { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Address { get; set; }
        public DateTime? BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string BloodCode { get; set; }
        public int SubOrdinates { get; set; }
        public int MutationTimes { get; set; }
        public int AchieveTimes { get; set; }
        public string TeamLeader { get; set; }
        public DateTime? JoinDate { get; set; }
        public DateTime? ResignDate { get; set; }
        public string ResignDescription { get; set; }
        public string MaritalStatus { get; set; }
        public string Religion { get; set; }
        public string Education { get; set; }
        public string Gender { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string SubDistrict { get; set; }
        public string Village { get; set; }
        public string ZipCode { get; set; }
        public string IdentityNo { get; set; }
        public string NPWPNo { get; set; }
        public DateTime? NPWPDate { get; set; }
        public string Email { get; set; }
        public string FaxNo { get; set; }
        public string Telephone1 { get; set; }
        public string Telephone2 { get; set; }
        public string Handphone1 { get; set; }
        public string Handphone2 { get; set; }
        public string Handphone3 { get; set; }
        public string Handphone4 { get; set; }
        public string DrivingLicense1 { get; set; }
        public string DrivingLicense2 { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public decimal? UniformSize { get; set; }
        public string UniformSizeAlt { get; set; }
        public string JobOther1 { get; set; }
        public string JobOther2 { get; set; }
    }

    public class HrInqPersInfoDetail: HrInqPersInfo
    {
        public string DealerName { get; set; }
        public string AdditionalJob { get; set; }
        public string City { get; set; }
        public DateTime? PreTraining { get; set; }
        public int PreTrainingPostTest { get; set; }
        public DateTime? Pembekalan { get; set; }
        public int PembekalanPostTest { get; set; }
        public DateTime? Salesmanship { get; set; }
        public int SalesmanshipPostTest { get; set; }
        public DateTime? OJT { get; set; }
        public int OjtPostTest { get; set; }
        public DateTime? FinalReview { get; set; }
        public int FinalReviewPostTest { get; set; }
        public DateTime? SpsSlv { get; set; }
        public int SpsSlvPostTest { get; set; }
        public DateTime? SpsGld { get; set; }
        public int SpsGldPostTest { get; set; }
        public DateTime? SpsPlt { get; set; }
        public int SpsPltPostTest { get; set; }
        public DateTime? SCBsc { get; set; }
        public DateTime? SCAdv { get; set; }
        public DateTime? SHBsc { get; set; }
        public DateTime? SHInt { get; set; }
        public DateTime? BMBsc { get; set; }
        public DateTime? BMInt { get; set; }
    }
}