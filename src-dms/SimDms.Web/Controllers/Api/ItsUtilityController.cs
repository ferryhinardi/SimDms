using Breeze.WebApi;
using Breeze.WebApi.EF;
using SimDms.Common;
using SimDms.PreSales.Models;
using SimDms.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace SimDms.Web.Controllers.Api
{
    [BreezeController]
    public class ItsUtilityController : ApiController
    {
        #region ~~ Basic Features ~~
        readonly EFContextProvider<DataContext> ctx = new EFContextProvider<DataContext>();

        [HttpGet]
        public String Metadata()
        {
            return ctx.Metadata();
        }

        protected SysUserView CurrentUser
        {
            get
            {
                return ctx.Context.SysUserViews.FirstOrDefault(a => a.UserId == User.Identity.Name);

                //return ctx.SysUserViews.Find(User.Identity.Name);
                //return ctx.SysUserViews.Where(x => x.UserId==User.Identity.Name).FirstOrDefault();
            }
        }

        //        protected string ProfitCenter
        //        {
        //            get
        //            {
        //                var IsAdmin = ctx.Context.Database.SqlQuery<bool>(string.Format(@"select top 1 b.IsAdmin from sysusergroup a 
        //                    left join SysGroup b on b.GroupId = a.GroupId where 1 = 1 and a.UserId = '{0}' and b.GroupId = a.GroupId",
        //                    CurrentUser.UserId)).SingleOrDefault();
        //                string profitCenter = "200";
        //                if (!IsAdmin)
        //                {
        //                    string s = "000";
        //                    var x = ctx.Context.SysUserProfitCenters.Find(CurrentUser.UserId);
        //                    if (x != null) s = x.ProfitCenter;
        //                    return s;
        //                }
        //                else
        //                {
        //                    return profitCenter;
        //                }
        //            }
        //        }

        protected string CompanyCode
        {
            get
            {
                return CurrentUser.CompanyCode;
            }
        }

        protected string BranchCode
        {
            get
            {
                return CurrentUser.BranchCode;
            }
        }

        protected string BranchName
        {
            get
            {
                return CurrentUser.BranchName;
            }
        }

        protected string ProductType
        {
            get
            {
                return ctx.Context.CoProfiles.Find(CompanyCode, BranchCode).ProductType;
            }
        }

        protected string TypeOfGoods
        {
            get
            {

                return CurrentUser.TypeOfGoods;
            }
        }

        protected string TypeOfGoodsName
        {
            get
            {
                string s = "";
                var x = ctx.Context.LookUpDtls.Find(CompanyCode, GnMstLookUpHdr.TypeOfGoods, CurrentUser.TypeOfGoods);
                if (x != null) s = x.LookUpValueName;
                return s;
            }
        }
        #endregion

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
	WHERE c.Department = 'SALES'
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

        [HttpGet]
        public IQueryable<TransferKdp> EmployeeLookup (string spvEmployeeID)
        {
            var query = GetTeamMembersBySupervisor(spvEmployeeID, "");
            var result = ctx.Context.Database.SqlQuery<TransferKdp>(query, 
                CompanyCode, BranchCode, spvEmployeeID).AsQueryable();
            return result;
        }

        [HttpGet]
        public IQueryable<TransferKdp> KDPEmployeeLookup(string spvEmployeeID, string employeeID)
        {
            var query = GetTeamMembersBySupervisor(spvEmployeeID, employeeID);
            var result = ctx.Context.Database.SqlQuery<TransferKdp>(query,
                CompanyCode, BranchCode, spvEmployeeID).AsQueryable();
            return result;
        }
    }
}