using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.PreSales.Models;

namespace SimDms.PreSales.Controllers.Api
{
    public class UtilityKdpController : BaseController
    {
        public JsonResult GetBranchManagerInfo(string branchCode)
        {
            var query = @"
SELECT a.BranchCode, a.EmployeeID, b.EmployeeName, b.Position
FROM hrEmployeeMutation a
JOIN (
	SELECT c.EmployeeId, c.EmployeeName, c.Position, MAX(d.MutationDate) AS MutationDate
	FROM hrEmployee c
	JOIN hrEmployeeMutation d
	ON c.EmployeeId = d.EmployeeId
	WHERE c.Department = 'SALES' and c.Position = 'BM' and c.PersonnelStatus = '1' 
    AND c.IsDeleted = 0 AND d.IsDeleted = 0
	GROUP BY c.EmployeeId, c.EmployeeName, c.Position
) b
ON a.EmployeeId = b.EmployeeId AND a.MutationDate = b.MutationDate
WHERE a.CompanyCode = @p0 AND a.BranchCode = @p1
ORDER BY b.MutationDate DESC
";
            var manager = ctx.Database.SqlQuery<PositionItem>(query, CompanyCode, branchCode).FirstOrDefault();

            var query1 = @"
SELECT a.BranchCode, a.EmployeeID, b.EmployeeName, b.Position
FROM hrEmployeeMutation a
JOIN (
	SELECT c.EmployeeId, c.EmployeeName, c.Position, MAX(d.MutationDate) AS MutationDate
	FROM hrEmployee c
	JOIN hrEmployeeMutation d
	ON c.EmployeeId = d.EmployeeId
	WHERE c.Department = 'SALES' and c.PersonnelStatus = '1' and c.TeamLeader = @p2
    AND c.IsDeleted = 0 AND d.IsDeleted = 0
	GROUP BY c.EmployeeId, c.EmployeeName, c.Position
) b
ON a.EmployeeId = b.EmployeeId AND a.MutationDate = b.MutationDate
WHERE a.CompanyCode = @p0 AND a.BranchCode = @p1
ORDER BY b.MutationDate DESC
";
            var team = ctx.Database.SqlQuery<PositionItem>(query1, CompanyCode, branchCode, manager.EmployeeID);

            return Json(new { manager = manager, team = team });
        }
    }
}