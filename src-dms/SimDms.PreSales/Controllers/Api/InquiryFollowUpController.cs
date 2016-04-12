using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using SimDms.PreSales.Models;

namespace SimDms.PreSales.Controllers.Api
{
    public class InquiryFollowUpController : BaseController
    {
        #region -- Private Classes --
        private class ComboItem
        {
            public string value { get; set; }
            public string text { get; set; }
        }
        private class EmployeeItem
        {
            public string EmployeeID { get; set; }
            public string EmployeeName { get; set; }
        }
        private class TeamMemberItem
        {
            public string EmployeeID { get; set; }
            public string EmployeeName { get; set; }
            public string OutletID { get; set; }
            public string OutletName { get; set; }
            public string UserID { get; set; }
        }
        private class OutletItem
        {
            public string OutletID { get; set; }
            public string OutletName { get; set; }
        }
        #endregion

        private const string SALES = "S";
        private const string SALES_CO = "SC";
        private const string SALES_HEAD = "SH";
        private const string BRANCH_MANAGER = "BM";
        private const string COO = "COO";
        private const string SALES_ADMIN = "ADM";

        #region -- Private Methods --
        private IQueryable<ComboItem> SelectBranchOutlets(bool useBranch)
        {
//            var query = @"SELECT OutletID AS value, OutletName AS text FROM PmBranchOutlets
//                WHERE CompanyCode = @p0 AND ((CASE WHEN @p1='' THEN BranchCode END)<>'' 
//                OR (CASE WHEN @p1<>'' THEN BranchCode END)=@p1)";
//            return ctx.Database.SqlQuery<ComboItem>(query, CompanyCode, useBranch ? BranchCode : "").OrderBy(x => x.value);

            var branches = useBranch ? ctx.CoProfiles.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode) :
                ctx.CoProfiles.Where(x => x.CompanyCode == CompanyCode);
            var items = branches.Select(x => new ComboItem
            {
                value = x.BranchCode,
                text = x.CompanyName
            });
            return items;
        }

        private IEnumerable<ComboItem> SelectEmployeesByPosition(string position, bool useBranch)
        {
            var query = @"
SELECT a.BranchCode, a.EmployeeId as value, b.EmployeeName as text, b.Position, b.RelatedUser AS UserId
FROM hrEmployeeMutation a
JOIN (
	SELECT c.EmployeeId, c.EmployeeName, c.Position, c.RelatedUser, MAX(d.MutationDate) AS MutationDate
	FROM hrEmployee c
	JOIN hrEmployeeMutation d
	ON c.EmployeeId = d.EmployeeId
	WHERE c.Department = 'SALES' AND d.IsJoinDate = 1 AND c.Position = @p2
	GROUP BY c.EmployeeId, c.EmployeeName, c.Position, c.RelatedUser
) b
ON a.EmployeeId = b.EmployeeId AND a.MutationDate = b.MutationDate
WHERE a.CompanyCode = @p0 AND 
((CASE WHEN @p1='' THEN a.BranchCode END)<>'' OR (CASE WHEN @p1<>'' THEN a.BranchCode END)=@p1)
";
            return ctx.Database.SqlQuery<ComboItem>(query, CompanyCode, useBranch ? BranchCode : "", position);
        }

        private EmployeeItem GetEmployeeBasedUserID(string userId)
        {
            var query = @"
SELECT a.BranchCode, a.EmployeeId, b.EmployeeName, b.Position, b.RelatedUser AS UserId
FROM hrEmployeeMutation a
JOIN (
	SELECT c.EmployeeId, c.EmployeeName, c.Position, c.RelatedUser, MAX(d.MutationDate) AS MutationDate
	FROM hrEmployee c
	JOIN hrEmployeeMutation d
	ON c.EmployeeId = d.EmployeeId
	WHERE c.Department = 'SALES' AND d.IsJoinDate = 1
	GROUP BY c.EmployeeId, c.EmployeeName, c.Position, c.RelatedUser
) b
ON a.EmployeeId = b.EmployeeId AND a.MutationDate = b.MutationDate
WHERE a.CompanyCode = @p0 AND 
((CASE WHEN @p1='' THEN a.BranchCode END)<>'' OR (CASE WHEN @p1<>'' THEN a.BranchCode END)=@p1)
AND b.RelatedUser = @p2
ORDER BY b.EmployeeName
";
            return ctx.Database.SqlQuery<EmployeeItem>(query, CompanyCode, BranchCode, userId).FirstOrDefault();
        }

        private Position GetBranchManagerData(string position)
        {
            return ctx.Positions.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode
                && x.PositionId == position);
        }

        private IEnumerable<TeamMemberItem> GetParentPosition(string child)
        {
            var query = @"SELECT 
                        a.employeeID, b.EmployeeName, c.OutletID, d.OutletName, e.UserID
                    FROM
                        PmMstTeamMembers a
                    LEFT JOIN GnMstEmployee b
                        ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode 
                        AND b.EmployeeId = a.EmployeeID
                    LEFT JOIN PmPosition c
                        ON c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode 
                        AND c.UserID = @p2
                    LEFT JOIN PmBranchOutlets d
	                    ON d.CompanyCode = a.CompanyCode AND d.BranchCode = a.BranchCode 
                        AND d.OutletID = c.OutletID
                    LEFT JOIN PmPosition e
                        ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode 
                        AND e.EmployeeID = a.EmployeeID
                    WHERE
                        a.CompanyCode = @p0 AND a.BranchCode = @p1 AND a.IsSupervisor = 1
                        AND TeamID = (SELECT TeamID FROM PmMStteamMembers WHERE CompanyCode = @p0 
                            AND BranchCode = @p1 AND EmployeeID = c.EmployeeID AND IsSupervisor = 0)";
            return ctx.Database.SqlQuery<TeamMemberItem>(query, CompanyCode, BranchCode, child);
        }

        private IEnumerable<ComboItem> GetChildPosition(string child, bool useBranch)
        {
            var query = @"
SELECT a.BranchCode, a.EmployeeId as value, b.EmployeeName as text, b.Position, b.RelatedUser AS UserId
FROM hrEmployeeMutation a
JOIN (
	SELECT c.EmployeeId, c.EmployeeName, c.Position, c.RelatedUser, MAX(d.MutationDate) AS MutationDate
	FROM hrEmployee c
	JOIN hrEmployeeMutation d
	ON c.EmployeeId = d.EmployeeId
	WHERE c.Department = 'SALES' AND d.IsJoinDate = 1 
	AND c.TeamLeader = (SELECT e.EmployeeId FROM hrEmployee e WHERE e.CompanyCode = @p0 AND e.RelatedUser = @p2)
	GROUP BY c.EmployeeId, c.EmployeeName, c.Position, c.RelatedUser
) b
ON a.EmployeeId = b.EmployeeId AND a.MutationDate = b.MutationDate
WHERE a.CompanyCode = @p0 AND 
((CASE WHEN @p1='' THEN a.BranchCode END)<>'' OR (CASE WHEN @p1<>'' THEN a.BranchCode END)=@p1)
ORDER BY b.EmployeeName
";
            return ctx.Database.SqlQuery<ComboItem>(query, CompanyCode, useBranch ? BranchCode : "", child);
        }
        

        private IEnumerable<TeamMemberItem> GetMembers(string employeeId)
        {
            var query = @"
select * from PmMstTeamMembers 
where CompanyCode = @p0 and BranchCode = @p1
and TeamID in (
select TeamID from PmMstTeamMembers 
where CompanyCode = @p0 and BranchCode = @p1 and EmployeeID = @p2 and IsSupervisor = 1 )
and IsSupervisor = 0
";
            return ctx.Database.SqlQuery<TeamMemberItem>(query, CompanyCode, BranchCode, employeeId);
        }

        private OutletItem GetOutletByEmployeeID(string employeeId, bool useBranch)
        {
            var query = @" 
                        SELECT 
                            b.OutletID, b.OutletName 
                        FROM 
                            PmPosition a 
                        LEFT JOIN PmBranchOutlets b 
                            ON b.CompanyCode=a.CompanyCode AND b.BranchCode=a.BranchCode 
                            AND b.OutletID=a.OutletID 
                        WHERE 
                            a.CompanyCode=@p0 
                            AND ((CASE WHEN @p1='' THEN a.BranchCode END)<>'' OR (CASE WHEN @p1<>'' THEN a.BranchCode END)=@p1) 
                            AND a.EmployeeID=@p2 ";
            return ctx.Database.SqlQuery<OutletItem>(query, CompanyCode, useBranch ? BranchCode : "", employeeId).FirstOrDefault();
        }
        #endregion

        #region -- Public Json Methods --
        public JsonResult GetPositionId()
        {
            var posStr = ctx.Database.SqlQuery<string>(@"SELECT PositionId FROM PmPosition
                    WHERE CompanyCode = @p0 AND BranchCode = @p1 AND UserId = @p2",
                    CompanyCode, BranchCode, CurrentUser.UserId).FirstOrDefault();
            int positionID = 0;
            return Json(int.TryParse(posStr, out positionID) ? positionID : 0);
        }

        public JsonResult ComboSourceBranchOutlets(bool useBranch)
        {
            var list = SelectBranchOutlets(useBranch);
            return Json(new { list = list });
        }

        public JsonResult ComboSourceEmployeeByPosition(string position, bool useBranch)
        {
            var list = SelectEmployeesByPosition(position, useBranch).ToList();
            return Json(new { list = list });
        }

        public JsonResult GetUserEmployee()
        {
            var employee = GetEmployeeBasedUserID(CurrentUser.UserId);
            return Json(new { data = employee });
        }

        public JsonResult ComboSourceChildPosition(string position)
        {
            var message = "";
            var list = GetChildPosition(CurrentUser.UserId, true).ToList();
            if (list.Count() <= 0) message = 
                position == SALES_HEAD ? "User belum memiliki Sales Coordinator di Master Team Members !" :
                position == SALES_CO ? "User belum memiliki Salesman di Master Team Members !" : "";
            return Json(new { message = message, list = list });
        }

        public JsonResult ComboSourceParentPosition(string position, string userID)
        {
            var message = "";
            var result = GetParentPosition(userID == "" ? CurrentUser.UserId : userID);
            if (result.Count() <= 0) message = 
                position == SALES_CO ? "User belum memiliki Sales Head di Master Team Members !" :
                position == SALES ? "User belum memiliki Sales Coordinator di Master Team Members !" : "";
            return Json(new { message = message, result = result });
        }

        public JsonResult ComboOnChange(string position, string employeeID, bool isCOO)
        {
            var list = new List<ComboItem>();
            //var position = ctx.Positions.FirstOrDefault(x => x.CompanyCode == CompanyCode
            //        && x.BranchCode == (isCOO ? "" : BranchCode) && x.EmployeeID == employeeID);
            var result = GetChildPosition(CurrentUser.UserId, !isCOO);
            if (result.Count() > 0) list.AddRange(result);
            
            if (position == SALES_CO)
            {
                var outlet = GetOutletByEmployeeID(employeeID, !isCOO);
                return Json(new { list = list, outlet = outlet });
            }
            else return Json(new { list = list });
        }

        public JsonResult GetPartType()
        {
            var message = ""; var id = ""; var pos = "";
            var pt = ctx.CoProfiles.FirstOrDefault(x => 
                x.CompanyCode == CompanyCode && x.BranchCode == BranchCode).ProductType;
            try
            {
                var emp = ctx.HrEmployees.FirstOrDefault(x =>
                x.CompanyCode == CompanyCode && x.RelatedUser == CurrentUser.UserId);
                if (emp == null) throw new Exception("User ID ini belum terhubung dengan ID karyawan. Silakan cek modul Man Power Management.");
                id = emp.EmployeeID;
                pos = emp.Position;
                if (pos == "") throw new Exception("Posisi User ID belum di-set! Silakan cek modul Man Power Management.");
            }
            catch (Exception ex)
            {
                message = ex.Message;    
            }
            return Json(new { message = message, partType = pt, employeeId = id, position = pos });
        }
        #endregion
    }
}
