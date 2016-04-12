﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    public class HrInqEmployee
    {
        public string CompanyCode { get; set; }
        public string DealerName { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string Grade { get; set; }
        public string PersonnelStatus { get; set; }
        public DateTime? JoinDate { get; set; }
        public string Status { get; set; }
        public int SubOrdinates { get; set; }
        public int MutationTimes { get; set; }
        public int AchieveTimes { get; set; }
        public string LastBranch { get; set; }
        public string LastBranchName { get; set; }
        public string LastPosition { get; set; }
        public string IsValid { get; set; }
        public string IsValidAchieve { get; set; }
        public string TeamLeader { get; set; }
        public string TeamLeaderName { get; set; }
        public DateTime? ResignDate { get; set; }
        public string ResignDescription { get; set; }
        public string MaritalStatus { get; set; }
        public string Religion { get; set; }
        public string Gender { get; set; }
        public string Education { get; set; }
        public string BirthPlace { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string SubDistrict { get; set; }
        public string Village { get; set; }
        public string ZipCode { get; set; }
        public string IdentityNo { get; set; }
        public string NPWPNo { get; set; }
        public DateTime? NPWPDate { get; set; }
        public string Email { get; set; }
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
        public string BloodCode { get; set; }
        public decimal? UniformSize { get; set; }
        public string UniformSizeAlt { get; set; }
        public decimal? ShoesSize { get; set; }
    }
}