using System;

namespace SimDms.DataWarehouse.Models
{
    public class SalesTeam
    {
        public string Area { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string PositionName { get; set; }
        public string GradeName { get; set; }
        public string LeaderName { get; set; }
    }

    public class SalesTeamHeader
    {
        public string Area { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string GroupArea { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
    }
}