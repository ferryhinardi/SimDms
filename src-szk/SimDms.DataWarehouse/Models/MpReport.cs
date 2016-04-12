using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    public class SfmPersInfo
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
        public int? SubOrdinates { get; set; }
        public int? MutationTimes { get; set; }
        public int? AchieveTimes { get; set; }
        public string TeamLeader { get; set; }
        public DateTime? JoinDate { get; set; }
        public DateTime? ResignDate { get; set; }
        public string ResignDescription { get; set; }
        public string MaritalStatus { get; set; }
        public string Religion { get; set; }
        public string Education { get; set; }
        public string Gender { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
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
    }

    public class MpPersInfo
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Position { get; set; }
        public DateTime? JoinDate { get; set; }
        public string Handphone1 { get; set; }
        public string MaritalStatus { get; set; }
        public string PersonnelStatus { get; set; }
        public string PositionCheck { get; set; }
        public DateTime? JoinDateCheck { get; set; }
        public int? HistAchievement { get; set; }
        public int? HistMutation { get; set; }
    }

    public class MpDashboard
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public int? BranchManager { get; set; }
        public int? SalesHead { get; set; }
        public int? SalesCoordinator { get; set; }
        public int? Salesman { get; set; }
        public int? SalesmanPlatinum { get; set; }
        public int? SalesmanGold { get; set; }
        public int? SalesmanSilver { get; set; }
        public int? SalesmanTrainee { get; set; }
        public int? TotalSalesForce { get; set; }
    }

    public class ManPowerDashboardModel
    {
        public string AreaCode { get; set; }
        public string DealerCode { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class SfMpDashboard
    {
        public string Outlet { get; set; }
        public Int32? BranchManager { get; set; }
        public Int32? SalesHead { get; set; }
        public Int32? Platinum { get; set; }
        public Decimal? PlatinumPct { get; set; }
        public Int32? Gold { get; set; }
        public Decimal? GoldPct { get; set; }
        public Int32? Silver { get; set; }
        public Decimal? SilverPct { get; set; }
        public Int32? Trainee { get; set; }
        public Decimal? TraineePct { get; set; }
        public Int32? TotalSalesPerson { get; set; }
        public Int32? TotalSalesForce { get; set; }
    }

    public class MpTrainingSummary
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public int? BM { get; set; }
        public int? BMT { get; set; }
        public int? BMN { get; set; }
        public int? SH { get; set; }
        public int? SHT { get; set; }
        public int? SHN { get; set; }
        public int? SC { get; set; }
        public int? SCT { get; set; }
        public int? SCN { get; set; }
        public int? S { get; set; }
        public int? ST { get; set; }
        public int? SN { get; set; }
        public int? T { get; set; }
        public int? TT { get; set; }
        public int? TN { get; set; }
    }

    public class MpDataTrend
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public int? January { get; set; }
        public int? February { get; set; }
        public int? March { get; set; }
        public int? April { get; set; }
        public int? May { get; set; }
        public int? June { get; set; }
        public int? July { get; set; }
        public int? August { get; set; }
        public int? September { get; set; }
        public int? October { get; set; }
        public int? November { get; set; }
        public int? December { get; set; }
    }

    public class MpRotation
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public int? InitialAmount { get; set; }
        public int? JoinAmount { get; set; }
        public int? RotationInAmount { get; set; }
        public int? ResignAmount { get; set; }
        public int? RotationOutAmount { get; set; }
        public int? TotalManPower { get; set; }
    }

    public class SfmTrainingDetail
    {
        public string OutletCode { get; set; }
        public string OutletAbbr { get; set; }
        public int? Jml { get; set; }
        public int? T { get; set; }
        public int? NT { get; set; }
    }

    public class TrainingDetailData
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string OutletAbbr { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Position { get; set; }
        public string PosName { get; set; }
        public string Grade { get; set; }
        public string GradeName { get; set; }
        public DateTime? JoinDate { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
    }
}