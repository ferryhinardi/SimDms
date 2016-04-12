using SimDms.PreSales.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.PreSales.Controllers.Api
{
    public class InquiryLostCaseController : BaseController
    {
        public JsonResult Default()
        {
            string UserId = CurrentUser.UserId;
            var recPosition = ctx.Positions.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.UserId == UserId).FirstOrDefault();
            var result = Json(new{});
            if (recPosition != null)
            {
                if (recPosition.PositionName == "COO")
                {
                    var sql = string.Format(@"select 
                        tm.CompanyCode, tm.BranchCode, tm.EmployeeID, emp.EmployeeName
                        from 
                        PmMstTeamMembers tm 
                        left join GnMstEmployee emp on tm.CompanyCode = emp.CompanyCode 
                        and tm.BranchCode = emp.BranchCode and tm.EmployeeID = emp.EmployeeID
                        where tm.EmployeeID in (select EmployeeID from PmPosition where PositionID = '40')
                        and tm.CompanyCode = '{0}'
                        ", CompanyCode);
                    var dtBM = ctx.Database.SqlQuery<MstTeamMember>(sql).Select(x => new { value = x.EmployeeID, text = x.EmployeeName }).ToList();

                    result = Json(new { success = true, dtBM = dtBM, JsonRequestBehavior.AllowGet });
                }

                else if (recPosition.PositionName == "Branch Manager")
                {
                    var sql = string.Format(@"select  
                        a.EmployeeID, b.EmployeeName, a.UserID
                        FROM pmPosition a
                        LEFT JOIN gnMstEmployee b
                        ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.EmployeeID = a.EmployeeID
                        WHERE
                        a.CompanyCode = '{0}' AND ((CASE WHEN '{1}'='' THEN a.BranchCode END)<>'' 
                        OR (CASE WHEN '{1}'<>'' THEN a.BranchCode END)='{1}') 
                        AND a.PositionId = '{2}'
                        ORDER BY b.EmployeeName
                        ", CompanyCode, BranchCode, recPosition.PositionId);
                    var dtBM = ctx.Database.SqlQuery<pmPosisitionView>(sql).Select(x => new { value = x.EmployeeID, text = x.EmployeeName }).ToList();
                    var NikBM = ctx.Database.SqlQuery<pmPosisitionView>(sql).Where(a => a.UserId == UserId).FirstOrDefault();

                    var sql2 = string.Format(@"select
                        a.employeeID, b.EmployeeName, c.OutletID, d.OutletName
                        FROM
                        PmMstTeamMembers a
                        LEFT JOIN GnMstEmployee b
                        ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode 
                        AND b.EmployeeId = a.EmployeeID
                        LEFT JOIN PmPosition c
                        ON c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode 
                        AND c.UserID = '{2}'
                        LEFT JOIN PmBranchOutlets d
	                    ON d.CompanyCode = a.CompanyCode AND d.BranchCode = a.BranchCode 
                        AND d.OutletID = c.OutletID
                        WHERE
                        a.CompanyCode = '{0}' 
                        AND ((CASE WHEN '{1}'='' THEN a.BranchCode END)<>'' OR (CASE WHEN '{1}'<>'' THEN a.BranchCode END)='{1}') 
                        AND a.IsSupervisor = 0 
                        AND TeamID = (SELECT TeamID FROM PmMStteamMembers WHERE CompanyCode = '{0}' 
                            AND ((CASE WHEN '{1}'='' THEN BranchCode END)<>'' OR (CASE WHEN '{1}'<>'' THEN BranchCode END)='{1}') AND EmployeeID = c.EmployeeID AND IsSupervisor = 1)

                        ", CompanyCode, BranchCode, UserId);
                    var NikSH = ctx.Database.SqlQuery<MstTeamMember>(sql2).FirstOrDefault();

                    if (NikSH != null)
                    {
                        var dtSH = ctx.Database.SqlQuery<pmPosisitionView>(sql2).Select(x => new { value = x.EmployeeID, text = x.EmployeeName }).ToList();

                        result = Json(new { success = true, NikBM = NikBM, dtBM = dtBM, dtSH = dtSH, JsonRequestBehavior.AllowGet });
                    }
                    else
                    {
                        result = Json(new { success = false, NikBM = NikBM, dtBM = dtBM, JsonRequestBehavior.AllowGet, message = "User belum memiliki Sales Head di Master Team Members !" });
                    }
                }
            }
            else
            {
                result = Json(new { success = false, message = "User belum di-setting di Master Position !" });
            }

            return result;
        }

    }
}
