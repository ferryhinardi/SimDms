using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.PreSales.Models;

namespace SimDms.PreSales.Controllers.Api
{
    public class TransferKdpController : BaseController
    {
        public JsonResult GetUserEmployeeID()
        {
            var message = "";
            var employeeID = "";
            try
            {
                var employee = ctx.HrEmployees.FirstOrDefault(x => x.CompanyCode == CompanyCode &&
                    x.RelatedUser == CurrentUser.UserId);
                if (employee == null) throw new Exception("User belum terhubung dengan ID Karyawan !");
                if (employee.Position != "SH" && employee.Position != "SC")
                    throw new Exception("Modul Transfer KDP hanya untuk digunakan oleh Sales Head/Coordinator !");
                employeeID = employee.EmployeeID;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return Json(new { message = message, employeeID = employeeID });
        }

        public JsonResult TransferKdp(string employeeID, IEnumerable<KdpDtl> details)
        {
            var message = "";
            var query = "";

            try
            {
                // manual distribution
                if (details != null)
                {
                    foreach (var detail in details)
                    {
                        query += string.Format(@"
update PmKDP set EmployeeID = '{0}', LastUpdateBy = '{5}', LastUpdateDate = getDate()
where CompanyCode = '{1}' and BranchCode = '{2}' and EmployeeID = '{3}'
	and InquiryNumber in (
		select TOP {4} inquiryNumber from PmKDP 
		where CompanyCode = '{1}' 
		and BranchCode = '{2}'
		and EmployeeID = '{3}'
        order by inquiryNumber
	)
", detail.EmployeeID, CompanyCode, BranchCode, employeeID, detail.KDPQty, CurrentUser.UserId);
                    }
                    var result = ctx.Database.ExecuteSqlCommand(query);

                }
                //random distribution
                else
                {
                    query = string.Format(@"
select EmployeeID from PmMstTeamMembers 
where CompanyCode = '{0}' 
	and BranchCode = '{1}' 
	and TeamID = (
		select TeamID from PmMstTeamMembers 
		where CompanyCode = '{0}' 
			and BranchCode = '{1}' 
			and employeeID = '{2}' 
			and IsSupervisor = 0
	) and IsSupervisor = 1                
", CompanyCode, BranchCode, employeeID);
                    var spvEmployeeID = ctx.Database.SqlQuery<string>(query).FirstOrDefault();
                    // Get employees for KDP Distribution
                    query = GetTeamMembersBySupervisor(spvEmployeeID, employeeID);
                    var members = ctx.Database.SqlQuery<TransferKdp>(query, CompanyCode, BranchCode, spvEmployeeID).ToList();

                    // Get KDP to distribute
                    var kdps = ctx.PmKdps.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode
                        && x.EmployeeID == employeeID);

                    query = "";
                    var incMember = 0;
                    var incExec = 1;
                    foreach (var kdp in kdps)
                    {
                        query += string.Format(@"
update PmKDP set EmployeeID = '{0}', LastUpdateBy = '{1}' , LastUpdateDate = getDate()
where CompanyCode = '{2}' and BranchCode = '{3}' and InquiryNumber = '{4}' and employeeID = '{5}'
", members[incMember].EmployeeID, CurrentUser.UserId, CompanyCode, BranchCode, kdp.InquiryNumber, employeeID);
                        if (incMember == members.Count - 1) incMember = 0;
                        else incMember++;

                        if ((incExec % 50) == 0)
                        {
                            var result = ctx.Database.ExecuteSqlCommand(query) > 0;

                            incExec = 0;
                            query = "";
                            if (!result) throw new Exception("Update PmKDP gagal"); ;
                        }
                        else incExec++;
                    }

                    if (query != "")
                    {
                        var result = ctx.Database.ExecuteSqlCommand(query) > 0;
                        if (!result) throw new Exception("Update PmKDP gagal"); ;
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(new { message = message });
        }

        private string GetTeamMembersBySupervisor(string spvID, string ignoredEmployeeID)
        {
            var query = @"
SELECT a.CompanyCode, a.BranchCode, d.BranchName, a.EmployeeID, b.EmployeeName, b.Position, b.TeamLeader,
(
		select 
			count(inquiryNumber) 
		from 
			PmKDP 
		where
			CompanyCode = a.CompanyCode 
			and BranchCode = a.BranchCode 
			and EmployeeID = a.EmployeeID
	) inquiryCount
FROM hrEmployeeMutation a
JOIN (
	SELECT c.EmployeeId, c.EmployeeName, c.Position, c.TeamLeader, MAX(d.MutationDate) AS MutationDate
	FROM hrEmployee c
	JOIN hrEmployeeMutation d
	ON c.EmployeeId = d.EmployeeId
	WHERE c.Department = 'SALES' AND c.PersonnelStatus = 1 AND c.IsDeleted = 0 AND d.IsDeleted = 0
	AND c.TeamLeader = @p2
	GROUP BY c.EmployeeId, c.EmployeeName, c.Position, c.TeamLeader
) b
ON a.EmployeeId = b.EmployeeId AND a.MutationDate = b.MutationDate
JOIN gnMstOrganizationDtl d
ON a.BranchCode = d.BranchCode
WHERE a.CompanyCode = @p0 AND a.BranchCode = @p1
";
            if (ignoredEmployeeID != "") query += @" AND a.EmployeeID != @p2";
            return query;
        }
    }
}