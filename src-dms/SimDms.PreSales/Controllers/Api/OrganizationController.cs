using SimDms.Common.Models;
using SimDms.PreSales.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace SimDms.PreSales.Controllers.Api
{
    public class OrganizationController : BaseController
    {
        public JsonResult LoadLocalBranch()
        {
            var result = ctx.OrganizationDtls.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode);
            return Json(result);
        }

        public JsonResult LoadEmployees(string branchCode, string position)
        {
            var query = @"
;WITH _1 AS (
	SELECT a.CompanyCode, a.EmployeeId, a.EmployeeName, a.Position, a.RelatedUser
	FROM hrEmployee a
	WHERE a.CompanyCode = @p0
	AND a.Department = 'SALES' AND a.PersonnelStatus = '1' AND a.IsDeleted = 0 
    AND (a.IsAssigned = 0 OR a.IsAssigned IS NULL)
	AND a.Position = CASE @p2 WHEN '' THEN a.Position ELSE @p2 END
	AND (a.TeamLeader IS NULL OR a.TeamLeader = '')
	AND a.EmployeeID NOT IN (
			SELECT DISTINCT g.TeamLeader FROM HrEmployee g
			WHERE g.CompanyCode = a.CompanyCode AND g.TeamLeader = a.EmployeeID AND g.PersonnelStatus = '1')
), _2 AS (
	SELECT a.CompanyCode, a.EmployeeID, MAX(a.MutationDate) AS MutationDate
	FROM HrEmployeeMutation a
	WHERE a.CompanyCode = @p0 AND a.IsDeleted = 0
	GROUP BY a.CompanyCode, a.EmployeeId
), _3 AS (
	SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.MutationDate
	FROM _2 a
	INNER JOIN HrEmployeeMutation b
	ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
), _4 AS (	
	SELECT a.CompanyCode, b.BranchCode, a.EmployeeId, a.EmployeeName, a.Position
	, c.PosLevel AS PositionID, c.PosName AS PositionName
	, (rtrim(a.EmployeeID) + ' - ' + rtrim(a.EmployeeName)) Employee
	, ISNULL(d.OutletAbbreviation, ISNULL(b.BranchCode, '-')) AS BranchAbv
	, a.RelatedUser, isnull((
			select count(*) from PmKDP
			 where CompanyCode  = a.CompanyCode
			   and BranchCode   = b.BranchCode
			   and EmployeeID   = a.EmployeeID
			), 0) CountKDP
	, b.MutationDate
	FROM _1 a
	LEFT JOIN _3 b
	ON a.CompanyCode = b.CompanyCode AND a.EmployeeId = b.EmployeeId
	LEFT JOIN gnMstPosition c
	ON a.CompanyCode = c.CompanyCode AND c.DeptCode = 'SALES' AND a.Position = c.PosCode
	LEFT JOIN gnMstDealerOutletMapping d
	ON a.CompanyCode = d.DealerCode AND b.BranchCode = d.OutletCode
	WHERE b.BranchCode = CASE @p1 WHEN '' THEN b.BranchCode ELSE @p1 END
	OR b.BranchCode IS NULL
)
SELECT * FROM _4 a
ORDER BY a.PositionID DESC, a.BranchCode, a.EmployeeID
";
            var result = ctx.Database.SqlQuery<OrganizationTreeItem>(query, CompanyCode, branchCode, position);
            return Json(result);
        }

        public JsonResult LoadOrganization(string branchCode)
        {
            var query = @"exec uspfn_pmSelectOrganizationTree @p0, @p1";
            var rawdata = ctx.Database.SqlQuery<OrganizationTreeItem>(query, CompanyCode, branchCode).ToList();
            if (rawdata.Count() == 0) return Json(new { result = rawdata });
            var maxLvl = rawdata.Max(x => x.lvl);

            var structured = new List<OrganizationTreeItem>();

            for (int i = 0; i <= maxLvl; i++)
            {
                var records = rawdata.Where(x => x.lvl == i).ToList();
                records.ForEach(x => { x.data = new List<OrganizationTreeItem>(); });
                if (i == 0) structured.AddRange(records);
                else
                {
                    foreach (var record in records)
                    {
                        var head = structured.Flatten(x => x.data).FirstOrDefault(x => x.EmployeeID == record.TeamLeader);
                        if (head != null) head.data.Add(record);
                    }                    
                }
            }

            return Json(new { result = structured });
        }

        public JsonResult LeaveMember(OrganizationTreeItem source)
        {
            var message = "";

            try
            {
                var query = @"
SELECT d.BranchCode, c.EmployeeId, c.EmployeeName, c.Position, c.TeamLeader
FROM hrEmployee c
JOIN hrEmployeeMutation d
ON c.CompanyCode = d.CompanyCode 
AND c.EmployeeId = d.EmployeeId
WHERE 1=1
AND c.CompanyCode = @p0
AND d.BranchCode = @p1
AND c.Department = 'SALES' 
AND c.TeamLeader = @p2
AND c.PersonnelStatus = 1 
AND c.IsDeleted = 0 
AND d.IsDeleted = 0
";

                var members = ctx.Database.SqlQuery<PositionItem>(query, CompanyCode, source.BranchCode, source.EmployeeID);
                if (members.Count() > 0) throw new Exception(
                    "Employee " + source.EmployeeName +
                    " tidak bisa dihapus karena masih memiliki Team Member aktif sebanyak " +
                    members.Count().ToString());

                var inactives = ctx.HrEmployees.Where(x => x.CompanyCode == CompanyCode
                    && x.TeamLeader == source.EmployeeID && x.PersonnelStatus != "1").ToList();
                foreach (var inactive in inactives)
                {
                    var person = ctx.HrEmployees.FirstOrDefault(x => x.CompanyCode == CompanyCode &&
                        x.EmployeeID == inactive.EmployeeID);
                    person.TeamLeader = "";
                    person.UpdatedBy = CurrentUser.UserId;
                    person.UpdatedDate = DateTime.Now;
                    ctx.SaveChanges();
                }

                var employee = ctx.HrEmployees.FirstOrDefault(x => x.CompanyCode == CompanyCode &&
                    x.EmployeeID == source.EmployeeID);
                employee.TeamLeader = "";
                employee.IsAssigned = false;
                employee.UpdatedBy = CurrentUser.UserId;
                employee.UpdatedDate = DateTime.Now;

                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(new { message = message });
        }

        public JsonResult SelectMembers(string branchCode, string employee1, string employee2)
        {
            try
            {
                var query = @"exec uspfn_pmSelectMembers @p0, @p1, @p2";

                var result1 = ctx.Database.SqlQuery<MemberDistributionItem>(query, CompanyCode, branchCode, employee1);
                var result2 = ctx.Database.SqlQuery<MemberDistributionItem>(query, CompanyCode, branchCode, employee2);

                return Json(new { message = "", result1 = result1, result2 = result2 });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }            
        }

        public JsonResult TransferMember(string branchCode, string id1, string id2, string data1, string data2)
        {
            var message = "";

            try
            {                
                var list1 = data1.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var list2 = data2.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                var query = "exec uspfn_pmApplyMemberTransfer @p0, @p1, @p2, @p3, @p4";

                foreach (var item in list1)
                    ctx.Database.ExecuteSqlCommand(query, CompanyCode, branchCode, item, id1, CurrentUser.UserId);

                foreach (var item in list2)
                    ctx.Database.ExecuteSqlCommand(query, CompanyCode, branchCode, item, id2, CurrentUser.UserId);

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(new { message = message });
        }

        public JsonResult GetPromotionData(string employeeID)
        {
            var message = "";
            MemberPromotionItem member = null;
            try
            {
                var emp = ctx.HrEmployees.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.EmployeeID == employeeID);
                var spv = ctx.HrEmployees.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.EmployeeID == emp.TeamLeader);
                var lookup = ctx.LookUpDtls.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.CodeID == "SPOS" &&
                        x.LookUpValue == emp.Position);
                var spvlookup = ctx.LookUpDtls.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.CodeID == "SPOS" &&
                        x.LookUpValue == spv.Position);
                var newpos = ctx.LookUpDtls.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.CodeID == "SPOS" &&
                        x.SeqNo == spvlookup.SeqNo);
                var newspv = ctx.HrEmployees.FirstOrDefault(x => x.CompanyCode == CompanyCode &&
                    x.EmployeeID == spv.TeamLeader);
                if (newspv == null) throw new Exception("Leader is already at highest position");
                var newspvlookup = ctx.LookUpDtls.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.CodeID == "SPOS" &&
                        x.LookUpValue == newspv.Position);
                var noNeedReplacement = !(ctx.HrEmployees.Count(x => x.CompanyCode == CompanyCode && x.TeamLeader == employeeID) > 0);

                member = new MemberPromotionItem
                {
                    EmployeeID = emp.EmployeeID,
                    EmployeeName = emp.EmployeeName,
                    PositionID = lookup.SeqNo.ToString(),
                    PositionName = lookup.LookUpValueName,
                    NewPositionID = newpos.SeqNo.ToString(),
                    NewPositionName = newpos.LookUpValueName,
                    TeamLeaderID = spv.EmployeeID,
                    TeamLeaderName = spv.EmployeeName,
                    LeaderPosID = spvlookup.SeqNo.ToString(),
                    LeaderPosName = spvlookup.LookUpValueName,
                    NewLeaderID = newspv.EmployeeID,
                    NewLeaderName = newspv.EmployeeName,
                    NewLeaderPosID = newspvlookup.SeqNo.ToString(),
                    NewLeaderPosName = newspvlookup.LookUpValueName,
                    NoNeedReplacement = noNeedReplacement
                };
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return Json(new { message = message, member = member });
        }

        public JsonResult ConfirmPromotion(string employeeID, string newLeaderID, string replacementID)
        {
            var message = "";
            var info = "";

            try
            {
                var emp = ctx.HrEmployees.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.EmployeeID == employeeID);
                var spv = ctx.HrEmployees.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.EmployeeID == emp.TeamLeader);
                var members = ctx.HrEmployees.Where(x => x.CompanyCode == CompanyCode
                    && x.TeamLeader == employeeID);
                var lookup = ctx.LookUpDtls.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.CodeID == "SPOS"
                    && x.LookUpValue == spv.Position);
                var replacement = ctx.HrEmployees.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.EmployeeID == replacementID);

                emp.TeamLeader = newLeaderID;
                emp.Position = spv.Position;
                emp.UpdatedBy = CurrentUser.UserId;
                emp.UpdatedDate = DateTime.Now;

                if (members.Count() > 0)
                {
                    foreach (var member in members)
                    {
                        member.TeamLeader = replacementID;
                        member.UpdatedBy = CurrentUser.UserId;
                        member.UpdatedDate = DateTime.Now;
                    }
                }

                ctx.SaveChanges();

                var achievement = new HrEmployeeAchievement
                {
                    CompanyCode = CompanyCode,
                    EmployeeID = employeeID,
                    Grade = "",
                    AssignDate = DateTime.Now,
                    Department = emp.Department,
                    Position = emp.Position,
                    IsDeleted = false,
                    IsJoinDate = false,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                    UpdatedBy = CurrentUser.UserId,
                    UpdatedDate = DateTime.Now
                };
                ctx.HrEmployeeAchievements.Add(achievement);
                ctx.SaveChanges();

                info = "Promosi karyawan sukses. " + Environment.NewLine +
                    emp.EmployeeName + " diangkat menjadi " + lookup.LookUpValueName + ". " +
                    (replacement == null ? "" : Environment.NewLine +
                    "Sebanyak " + members.Count().ToString() + " karyawan pindah ke Team " + replacement.EmployeeName);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return Json(new { message = message, info = info });
        }

        public JsonResult JoinMember(string branchCode, string employeeID, string leaderID)
        {
            var message = "";

            var query = "exec uspfn_pmEmployeeJoinBranch @p0, @p1, @p2, @p3, @p4";
            try
            {
                ctx.Database.CommandTimeout = 3600;
                var result = ctx.Database.ExecuteSqlCommand(query, CompanyCode, branchCode, employeeID, leaderID, CurrentUser.UserId);
                
            }
            catch(Exception ex)
            {
                message = ex.Message;
            }           
            
            return Json(new { message = message });
        }

        public JsonResult ComboBranches()
        {
            var query = @"SELECT OutletCode AS id, OutletCode + ' - ' + OutletAbbreviation AS value
FROM gnMstDealerOutletMapping 
WHERE DealerCode = @p0
";
            var items = ctx.Database.SqlQuery<ComboItem>(query, CompanyCode);
            return Json(new { items = items });
        }

        public JsonResult EmployeeMutation(string branchCode, string employeeID)
        {
            var message = "";
            try
            {
                var employee = ctx.HrEmployees.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.EmployeeID == employeeID);
                employee.IsAssigned = true;
                ctx.SaveChanges();

                var record = new HrEmployeeMutation
                {
                    CompanyCode = CompanyCode,
                    BranchCode = branchCode,
                    EmployeeID = employeeID,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false,
                    IsJoinDate = false,
                    MutationDate = DateTime.Now,
                    UpdatedBy = CurrentUser.UserId,
                    UpdatedDate = DateTime.Now
                };
                ctx.HrEmployeeMutations.Add(record);
                ctx.SaveChanges();

                if (employee.RelatedUser != null | employee.RelatedUser != "")
                {
                    var UserId = ctx.SysUsers.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.UserId == employee.RelatedUser);
                    UserId.BranchCode = branchCode;
                    ctx.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(new { message = message });
        }

        public JsonResult RandomizeKDP(string employeeID)
        {
            try
            {
                var query = "exec uspfn_pmRandomizeKDP @CompanyCode, @EmployeeID";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@CompanyCode", CompanyCode),
                    new SqlParameter("@EmployeeID", employeeID)
                };

                var result = ctx.Database.ExecuteSqlCommand(query, parameters);

                return Json(new { message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        /*
        public JsonResult RandomizeKDP(string employeeID)
        {
            using (var scope = new TransactionScope())
            {
                var msg = "";
                try
                {
                    var emp = GetEmployee(employeeID);
                    if (emp == null) throw new Exception("Error. Employee " + employeeID + " tidak ditemukan");
                    if (string.IsNullOrWhiteSpace(emp.TeamLeader))
                        throw new Exception("Error. Employee " + employeeID + " tidak memiliki Team Leader");
                    var spv = GetEmployee(emp.TeamLeader);
                    if (spv == null) throw new Exception("Error. Team Leader dari " + employeeID + " tidak ditemukan");
                    var kdps = GetKDPs(employeeID);
                    kdps.Shuffle();
                    var queue = new Queue<PmKdpItem>(kdps);
                    ctx.Database.CommandTimeout = 3600;
                    var members = GetTeamMembers(spv.BranchCode, spv.EmployeeID);
                    var self = members.FirstOrDefault(x => x.EmployeeID == emp.EmployeeID);
                    members.Remove(self);
                    members.Shuffle();

                    var m = 0;
                    while (true)
                    {
                        if (queue.Count == 0) break;
                        var item = queue.Dequeue();

                        var kdp = ctx.PmKdps.FirstOrDefault(x => x.InquiryNumber == item.InquiryNumber
                            && x.BranchCode == item.BranchCode && x.CompanyCode == item.CompanyCode);
                        kdp.EmployeeID = members[m].EmployeeID;
                        kdp.LastUpdateBy = CurrentUser.UserId;
                        kdp.LastUpdateDate = DateTime.Now;
                        ctx.SaveChanges();

                        m = m == members.Count - 1 ? 0 : m += 1;
                    }
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                }
                finally
                {
                    scope.Dispose();                    
                }
                return Json(new { message = msg });
            }
        }
        */

        private PositionItem GetEmployee(string employeeID)
        {
            var query = @"
;WITH _1 AS
(
    SELECT a.CompanyCode, a.EmployeeID, MAX(a.MutationDate) AS MutationDate FROM HrEmployeeMutation a
    WHERE a.CompanyCode = @p0 AND a.EmployeeID = @p1
    GROUP BY a.CompanyCode, a.EmployeeID
), _2 AS
(
    SELECT a.CompanyCode, a.EmployeeID, b.BranchCode, a.MutationDate FROM _1 a
    INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
), _3 AS
(
    SELECT a.CompanyCode, a.EmployeeID, a.Position, b.BranchCode, a.TeamLeader FROM HrEmployee a
    LEFT JOIN _2 b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
    WHERE a.CompanyCode = @p0 AND a.Department = 'SALES'
    AND a.EmployeeID = @p1
)
SELECT * FROM _3
";
            return ctx.Database.SqlQuery<PositionItem>(query, CompanyCode, employeeID).FirstOrDefault();            
        }

        private List<PositionItem> GetTeamMembers(string branchCode, string teamLeader)
        {
            var leader = GetEmployee(teamLeader);
            if(leader == null) return null;

            var query = @"
;WITH _1 AS
(
    SELECT a.CompanyCode, a.EmployeeID, MAX(a.MutationDate) AS MutationDate FROM HrEmployeeMutation a
    WHERE a.CompanyCode = @p0 AND a.BranchCode = @p1 AND a.IsDeleted = 0 
    GROUP BY a.CompanyCode, a.EmployeeID
), _2 AS
(
    SELECT a.CompanyCode, a.EmployeeID, b.BranchCode, a.MutationDate FROM _1 a
    INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
), _3 AS
(
    SELECT a.CompanyCode, a.EmployeeID, a.Position, b.BranchCode, a.TeamLeader FROM HrEmployee a
    LEFT JOIN _2 b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
    WHERE a.CompanyCode = @p0 AND a.Department = 'SALES'
    AND a.TeamLeader = @p2 AND a.PersonnelStatus = '1' AND a.IsDeleted = 0 AND a.IsAssigned = 1
)
SELECT * FROM _3
";
            return ctx.Database.SqlQuery<PositionItem>(query, CompanyCode, leader.BranchCode, teamLeader).ToList();
        }

        private List<PmKdpItem> GetKDPs(string employeeID)
        {
            var emp = GetEmployee(employeeID);
            var query = @"
SELECT * FROM pmKDP a
WHERE a.CompanyCode = @p0 AND a.BranchCode = @p1 AND a.EmployeeID = @p2
";
            return ctx.Database.SqlQuery<PmKdpItem>(query, CompanyCode, emp.BranchCode, employeeID).ToList();
        }
    }

    public static class Linq
    {
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> e, Func<T, IEnumerable<T>> f)
        {
            return e.SelectMany(c => f(c).Flatten(f)).Concat(e);
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}