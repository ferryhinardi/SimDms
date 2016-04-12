using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.PreSales.Models
{
    [Table("pmMstTeam")]
    public class Team
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string TeamID { get; set; }
        public string TeamName { get; set; }
        public DateTime? StartDateActive { get; set; }
        public DateTime? EndDateActive { get; set; }
        public bool? IsLock { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }

    [Table("PmMstTeamMembers")]
    public class TeamMember
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string EmployeeID { get; set; }
        [Key]
        [Column(Order = 4)]
        public string TeamID { get; set; }
        public int MemberID { get; set; }
        public bool? IsSupervisor { get; set; }
        public bool? IsLock { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public bool? IsSalesMan { get; set; }
    }

    public class MstTeamMember
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string FullName { get; set; }
        public string PositionId { get; set; }
        public string Position { get; set; }
        public string OutletID { get; set; }
        public string OutletName { get; set; }
        public string UserId { get; set; }
    }

    public class TeamMemberLookup
    {
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string TeamID { get; set; }
        public string TeamName { get; set; }
    }

    public class TeamSave
    {
        public string BranchCode { get; set; }
        public string TeamID { get; set; }
        public string TeamName { get; set; }
    }

    public class TeamMemberSave
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public bool IsSupervisor { get; set; }
    }
}