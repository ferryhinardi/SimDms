using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Absence.Models
{
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
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
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

}