using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.PreSales.Models
{
    public class OrganizationTreeItem
    {
        public string id { get; set; }
        public int lvl { get; set; }
        public string BranchCode { get; set; }
        public string BranchAbv { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Position { get; set; }
        public int PositionID { get; set; }
        public string PositionName { get; set; }
        public string Employee { get; set; }
        public string TeamLeader { get; set; }
        public int CountKDP { get; set; }
        public string RelatedUser { get; set; }
        public List<OrganizationTreeItem> data { get; set; }
    }

    public class MemberDistributionItem
    {
        public String BranchCode { get; set; }
        public String KeyID { get; set; }
        public String NamaProspek { get; set; }
        public String PerolehanData { get; set; }
        public String Member { get; set; }
    }

    public class MemberPromotionItem
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string PositionName { get; set; }
        public string PositionID { get; set; }
        public string NewPositionName { get; set; }
        public string NewPositionID { get; set; }
        public string TeamLeaderID { get; set; }
        public string TeamLeaderName { get; set; }
        public string LeaderPosID { get; set; }
        public string LeaderPosName { get; set; }
        public string NewLeaderID { get; set; }
        public string NewLeaderName { get; set; }
        public string NewLeaderPosID { get; set; }
        public string NewLeaderPosName { get; set; }
        public bool NoNeedReplacement { get; set; }
    }

    public class MemberReplacementItem
    {
        public string BranchCode { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Position { get; set; }
        public string PositionName { get; set; }
        public string TeamLeaderID { get; set; }
        public string TeamLeaderName { get; set; }
        public string TeamLeaderPosition { get; set; }
        public int MemberCount { get; set; }
    }

    public class ComboItem
    {
        public string id { get; set; }
        public string value { get; set; }
    }
}