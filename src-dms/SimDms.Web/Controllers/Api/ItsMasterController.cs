using Breeze.WebApi;
using Breeze.WebApi.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

using SimDms.Common.Models;
using SimDms.Common;
using SimDms.PreSales.Models;

namespace SimDms.Web.Controllers.Api
{
    [BreezeController]
    public class ItsMasterController : ApiController
    {
        #region ~Basic Features~
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

        #region ~Deprecated Functions~
        /// <summary>
        /// Deprecated
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<TeamMemberLookup> TeamMemberLookup()
        {
            var data = from a in ctx.Context.Teams
                       join b in ctx.Context.OrganizationDtls
                       on new { a.CompanyCode, a.BranchCode }
                       equals new { b.CompanyCode, b.BranchCode } into _b
                       from b in _b.DefaultIfEmpty()
                       where a.CompanyCode == CompanyCode
                       orderby a.BranchCode, a.TeamID
                       select new TeamMemberLookup
                       {
                           BranchCode = a.BranchCode,
                           BranchName = b.BranchName,
                           TeamID = a.TeamID,
                           TeamName = a.TeamName
                       };

            return data;
        }

        /// <summary>
        /// Deprecated
        /// </summary>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<MstTeamMember> TeamMembers(string branchCode)
        {
            branchCode = branchCode == null ? "" : branchCode;
            var query = string.Format(@"SELECT
                        a.CompanyCode, a.BranchCode,a.EmployeeID, b.EmployeeName, c.FullName, a.PositionId , d.LookupValueName Position, a.OutletId, e.OutletName,
                        a.UserId
                    FROM
                        pmPosition a
                    LEFT JOIN gnMstEmployee b
                        ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.EmployeeId = a.EmployeeId
                    LEFT JOIN sysUser c
                        ON c.UserId=a.UserId
                    LEFT JOIN gnMstLookupDtl d
                        ON d.CompanyCode = a.CompanyCode AND d.LookUpValue = a.PositionID AND d.CodeId='PGRD'
                    LEFT JOIN PmBranchOutlets e
                        ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode AND e.OutletID = a.OutletID 
                    WHERE
                        a.CompanyCode = {0} AND 
                        ((CASE WHEN {1} = '' THEN a.BranchCode END) = a.BranchCode 
                        OR (CASE WHEN {1} <> '' THEN a.BranchCode END) = {1})
                    ORDER BY 
                        a.BranchCode , b.EmployeeName , c.FullName , Position ", CompanyCode, branchCode);

            var data = ctx.Context.Database.SqlQuery<MstTeamMember>(query).AsQueryable();
            return data;
        } 
        #endregion

        [HttpGet]
        public IQueryable<OrganizationDtl> BranchLookup()
        {
            var data = ctx.Context.OrganizationDtls.Where(a => a.CompanyCode == CompanyCode);
            return data;
        }

        [HttpGet]
        public IQueryable<PositionItem> EmployeeLookup(string branchCode)
        {
            var query = @"
SELECT a.BranchCode, d.BranchName, a.EmployeeID, b.EmployeeName, b.Position, b.RelatedUser AS UserID, ISNULL(c.FullName, '') AS UserName
FROM hrEmployeeMutation a
JOIN (
	SELECT c.EmployeeId, c.EmployeeName, c.Position, c.RelatedUser, MAX(d.MutationDate) AS MutationDate
	FROM hrEmployee c
	JOIN hrEmployeeMutation d
	ON c.EmployeeId = d.EmployeeId
	WHERE c.Department = 'SALES'  AND c.PersonnelStatus = 1 AND c.IsDeleted = 0 AND d.IsDeleted = 0
	GROUP BY c.EmployeeId, c.EmployeeName, c.Position, c.RelatedUser
) b
ON a.EmployeeId = b.EmployeeId AND a.MutationDate = b.MutationDate
LEFT JOIN sysUser c
ON b.RelatedUser = c.UserId
JOIN gnMstOrganizationDtl d
ON a.BranchCode = d.BranchCode
WHERE a.CompanyCode = @p0 AND a.BranchCode = @p1
";
            var result = ctx.Context.Database.SqlQuery<PositionItem>(query, CompanyCode, BranchCode).AsQueryable();
            return result;
        }

        [HttpGet]
        public IQueryable<PositionItem> GnMstEmployeeLookup(string branchCode)
        {
            var query = @"
SELECT g.EmployeeID, g.EmployeeName
FROM gnMstEmployee g WITH (NOLOCK, NOWAIT)
WHERE g.CompanyCode = @p0
    AND ((CASE WHEN @p1 = '' THEN g.BranchCode END) = g.BranchCode OR 
    (CASE WHEN @p1 <> '' THEN g.BranchCode END) = @p1)
ORDER BY g.EmployeeID
";
            var result = ctx.Context.Database.SqlQuery<PositionItem>(query, CompanyCode, branchCode).AsQueryable();
            return result;
        }


        [HttpGet]
        public IQueryable<PositionItem> PositionLookup()
        {
            var query = @"
SELECT a.BranchCode, d.BranchName, a.EmployeeID, b.EmployeeName, b.Position, b.RelatedUser AS UserID, ISNULL(c.FullName, '') AS UserName
FROM hrEmployeeMutation a
JOIN (
	SELECT c.EmployeeId, c.EmployeeName, c.Position, c.RelatedUser, MAX(d.MutationDate) AS MutationDate
	FROM hrEmployee c
	JOIN hrEmployeeMutation d
	ON c.EmployeeId = d.EmployeeId
	WHERE c.Department = 'SALES' AND c.PersonnelStatus = 1 AND c.IsDeleted = 0 AND d.IsDeleted = 0
	GROUP BY c.EmployeeId, c.EmployeeName, c.Position, c.RelatedUser
) b
ON a.EmployeeId = b.EmployeeId AND a.MutationDate = b.MutationDate
LEFT JOIN sysUser c
ON b.RelatedUser = c.UserId
JOIN gnMstOrganizationDtl d
ON a.BranchCode = d.BranchCode
WHERE a.CompanyCode = @p0
ORDER BY a.BranchCode
";
            var result = ctx.Context.Database.SqlQuery<PositionItem>(query, CompanyCode).AsQueryable();
            return result;
        }

        [HttpGet]
        public IQueryable<PositionItem> UserLookup(string branchCode)
        {
            var query = @"
SELECT 
    UserID, FullName AS UserName
FROM
    SysUser
WHERE 
    CompanyCode = @p0 AND BranchCode = @p1 
    AND UserId NOT IN (SELECT RelatedUser AS UserId FROM HrEmployee where Department = 'SALES' AND Position <> 'BM' AND RelatedUser IS NOT NULL) 
";
            var result = ctx.Context.Database.SqlQuery<PositionItem>(query, CompanyCode, BranchCode).AsQueryable();
            return result;
        }

        [HttpGet]
        public IQueryable<PositionItem> GradeEmployeeAllBranchLookup()
        {
            var query = @"
SELECT a.BranchCode, d.BranchName, a.EmployeeID, b.EmployeeName, ISNULL(b.Grade, '') AS Grade
FROM hrEmployeeMutation a
JOIN (
	SELECT c.EmployeeId, c.EmployeeName, c.Grade, MAX(d.MutationDate) AS MutationDate
	FROM hrEmployee c
	JOIN hrEmployeeMutation d
	ON c.EmployeeId = d.EmployeeId
	WHERE c.Department = 'SALES' AND c.PersonnelStatus = 1 AND c.IsDeleted = 0 AND d.IsDeleted = 0
	GROUP BY c.EmployeeId, c.EmployeeName, c.Grade
) b
ON a.EmployeeId = b.EmployeeId AND a.MutationDate = b.MutationDate
JOIN gnMstOrganizationDtl d
ON a.BranchCode = d.BranchCode
WHERE a.CompanyCode = @p0
";
            
            var result = ctx.Context.Database.SqlQuery<PositionItem>(query, CompanyCode).AsQueryable();
            return result;
        }

        [HttpGet]
        public IQueryable<GroupItem> TypeLookup()
        {
            var query = @"
SELECT DISTINCT GroupCode FROM OmMstModel WHERE CompanyCode=@p0 AND GroupCode <> ''
";
            var result = ctx.Context.Database.SqlQuery<GroupItem>(query, CompanyCode).AsQueryable();
            return result;
        }

        [HttpGet]
        public IQueryable<GroupItem> VariantLookup(string groupCode)
        {
            var query = @"
SELECT DISTINCT TypeCode FROM OmMstModel WHERE CompanyCode=@p0 AND GroupCode = @p1
";
            var result = ctx.Context.Database.SqlQuery<GroupItem>(query, CompanyCode, groupCode).AsQueryable();
            return result;
        }

        [HttpGet]
        public IQueryable<PositionItem> LeaderLookup(string branchCode)
        {
            var query = @"
SELECT a.BranchCode, d.BranchName, a.EmployeeID, b.EmployeeName, b.Position
FROM hrEmployeeMutation a
JOIN (
	SELECT c.EmployeeId, c.EmployeeName, c.Position, MAX(d.MutationDate) AS MutationDate
	FROM hrEmployee c
	JOIN hrEmployeeMutation d
	ON c.EmployeeId = d.EmployeeId
	WHERE c.Department = 'SALES' AND c.PersonnelStatus = 1 AND c.IsDeleted = 0 AND d.IsDeleted = 0
	AND c.Position = (CASE @p2 WHEN '4W' THEN 'SH' WHEN '2W' THEN 'SC' END)
	GROUP BY c.EmployeeId, c.EmployeeName, c.Position
) b
ON a.EmployeeId = b.EmployeeId AND a.MutationDate = b.MutationDate
JOIN gnMstOrganizationDtl d
ON a.BranchCode = d.BranchCode
WHERE a.CompanyCode = @p0 AND a.BranchCode = @p1
";

            var result = ctx.Context.Database.SqlQuery<PositionItem>(query, CompanyCode, branchCode, ProductType).AsQueryable();
            return result;
        }

        [HttpGet]
        public IQueryable<PositionItem> SalesmanLookup(string branchCode)
        {
            #region query by Bent
            //            var query = @"
//                SELECT a.BranchCode, d.BranchName, a.EmployeeID, b.EmployeeName, b.Position
//                FROM hrEmployeeMutation a
//                JOIN (
//	                SELECT c.EmployeeId, c.EmployeeName, c.Position, MAX(d.MutationDate) AS MutationDate
//	                FROM hrEmployee c
//	                JOIN hrEmployeeMutation d
//	                ON c.EmployeeId = d.EmployeeId
//	                WHERE c.Department = 'SALES' AND c.PersonnelStatus = 1 AND c.IsDeleted = 0 AND d.IsDeleted = 0
//	                AND (c.TeamLeader = '' OR c.TeamLeader IS NULL)
//	                GROUP BY c.EmployeeId, c.EmployeeName, c.Position, c.TeamLeader
//                ) b
//                ON a.EmployeeId = b.EmployeeId AND a.MutationDate = b.MutationDate
//                JOIN gnMstOrganizationDtl d
//                ON a.BranchCode = d.BranchCode
//                WHERE a.CompanyCode = @p0 AND a.BranchCode = @p1
            //";
            #endregion
            var queryable = from p in ctx.Context.HrEmployees
                            where p.CompanyCode == CompanyCode && p.Department == "SALES" && p.Position == "S" 
                                  && (p.TeamLeader == null || p.TeamLeader == "")
                                  && (p.Position != "SH" || p.Position != "SC")
                                  && p.PersonnelStatus == "1"
                            select new PositionItem()
                            {
                                EmployeeID = p.EmployeeID,
                                EmployeeName = p.EmployeeName
                            }; 
            return (queryable);
        }

        [HttpGet]
        public IQueryable<MemberReplacementItem> ReplacementLookup(string branchCode, string employeeID)
        {
            var emp = ctx.Context.HrEmployees.FirstOrDefault(x => x.CompanyCode == CompanyCode &&
                x.EmployeeID == employeeID);
            var pos = emp.Position;

            var query = @"
SELECT a.BranchCode, a.EmployeeID, b.EmployeeName, b.Position
	, d.PosName AS PositionName
	, b.TeamLeader AS TeamLeaderID
	, e.EmployeeName AS TeamLeaderName
    , g.PosName AS TeamLeaderPosition
	, (SELECT COUNT(f.EmployeeID) FROM HrEmployee f 
		WHERE f.TeamLeader = a.EmployeeID)AS MemberCount
FROM hrEmployeeMutation a
JOIN (
	SELECT c.EmployeeId, c.EmployeeName, c.Position, c.TeamLeader, MAX(d.MutationDate) AS MutationDate
	FROM hrEmployee c
	JOIN hrEmployeeMutation d
	ON c.EmployeeId = d.EmployeeId
	WHERE c.Department = 'SALES' AND c.PersonnelStatus = 1 AND c.IsDeleted = 0 AND d.IsDeleted = 0
	AND c.EmployeeID <> @p2 AND c.Position = @p3
	GROUP BY c.EmployeeId, c.EmployeeName, c.Position, c.TeamLeader
) b
ON a.EmployeeId = b.EmployeeId AND a.MutationDate = b.MutationDate
JOIN gnMstPosition d
ON d.CompanyCode = a.CompanyCode AND d.DeptCode = 'SALES' AND d.PosCode = b.Position
JOIN HrEmployee e
ON e.CompanyCode = a.CompanyCode AND e.EmployeeID = b.TeamLeader
JOIN gnMstPosition g
ON g.CompanyCode = a.CompanyCode AND g.DeptCode = 'SALES' AND g.PosCode = e.Position
WHERE a.CompanyCode = @p0 AND a.BranchCode = @p1
";
            var result = ctx.Context.Database.SqlQuery<MemberReplacementItem>(query, CompanyCode, branchCode, employeeID, pos).AsQueryable();
            return result;
        }

        [HttpGet]
        public IQueryable<GroupModelsView> SelectLookUpGroupModel()
        {
            //gnMstCompanyMapping
            string DBMD = "";
            DBMD = ctx.Context.Database.SqlQuery<string>("SELECT dbmd FROM gnMstCompanyMapping WHERE companycode='" + CompanyCode + "' AND branchcode='" + BranchCode + "'").FirstOrDefault(); 
            var query = string.Format("Select distinct 1 seqNo, GroupModel "+
                                      " from " + DBMD + "..MsMstGroupModel " +
                                      " UNION  "+
                                      " Select 0 SeqNO,'<----Select All---->' GroupModel "+
                                      " order by SeqNo Asc, GroupModel Asc" );
            var queryable = ctx.Context.Database.SqlQuery<GroupModelsView>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<TipeKendaraanView> SelectLookUpTipeKendaraan(string tipeKendaraan, string variant)
        {
            //var query = string.Format("uspfn_InquiryITSStatusBtn '{0}','{1}'", tipeKendaraan, variant);
            //var queryable = ctx.Context.Database.SqlQuery<TipeKendaraanView>(query).AsQueryable();
            //return (queryable);

            var carType = tipeKendaraan;
            var vrnt = variant;
            if (carType == null || carType == string.Empty)
            {
                carType = "";
            }
            if (vrnt == null || vrnt == string.Empty)
            {
                vrnt = "";
            }

            string DBMD = "";
            DBMD = ctx.Context.Database.SqlQuery<string>("SELECT dbmd FROM gnMstCompanyMapping WHERE companycode='" + CompanyCode + "' AND branchcode='" + BranchCode + "'").FirstOrDefault();
            var query = string.Format("select distinct 1 SeqNO, a.GroupCode TipeKendaraan , a.TypeCode Variant " +
                                       " from omMstModel a " +
                                       " left join " + DBMD + "..msMstGroupModel b on a.GroupCode = b.ModelType " +
                                       " where b.GroupModel like case when '" + carType + "' = '' then '%%' else '" + carType + "' end " +
                                       "   and TypeCode like case when '" + vrnt + "' = '' then '%%' else '" + vrnt + "' end " +
                                        "  and GroupCode <> '' " +
                                       "  UNION " +
                                       " Select 0 SeqNO,'<----Select All---->' TipeKendaraan, '<----Select All---->' Variant " +
                                       " order by SeqNo Asc,TipeKendaraan Asc, Variant Asc");
            var queryable = ctx.Context.Database.SqlQuery<TipeKendaraanView>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<PmKdpBrowse> kdplist(string NikSH, string NikSales)
        {

            var employeeID = ctx.Context.HrEmployees.Where(x => x.RelatedUser == CurrentUser.UserId).Select(x => x.EmployeeID).FirstOrDefault();

            var subordinates = ctx.Context.HrEmployees.Where(x => x.TeamLeader == employeeID).Select(x => x.EmployeeID).ToList();
            var salesman = ctx.Context.HrEmployees.Where(a => subordinates.Contains(a.TeamLeader) == true).Select(a => a.EmployeeID).ToList();

            var qry = from p in ctx.Context.PmKdps
                      join q in ctx.Context.HrEmployees on new { p.CompanyCode, p.EmployeeID } equals new { q.CompanyCode, q.EmployeeID }
                      join j1 in ctx.Context.MstRefferences on new { p.CompanyCode, p.ColourCode, RefferenceType = "COLO" } equals new { j1.CompanyCode, ColourCode = j1.RefferenceCode, j1.RefferenceType } into join1
                      from r in join1.DefaultIfEmpty()
                      join j2 in ctx.Context.LookUpDtls on new { p.CompanyCode, p.TestDrive, CodeID = "PMOP" } equals new { j2.CompanyCode, TestDrive = j2.LookUpValue, j2.CodeID } into join2
                      from s in join2.DefaultIfEmpty()
                      join j3 in ctx.Context.LookUpDtls on new { p.CompanyCode, p.CityID, CodeID = "CITY" } equals new { j3.CompanyCode, CityID = j3.LookUpValue, j3.CodeID } into join3
                      from t in join3.DefaultIfEmpty()
                      where p.CompanyCode == CompanyCode
                      && p.BranchCode == BranchCode
                      select new PmKdpBrowse
                      {
                         InquiryNumber = p.InquiryNumber,
                         BranchCode = p.BranchCode,
                         TipeKendaraan = p.TipeKendaraan,
                         Variant = p.Variant,
                         Transmisi = p.Transmisi,
                         ColourCode = p.ColourCode,
                         ColourName = r.RefferenceDesc1,
                         NamaProspek = p.NamaProspek,
                         EmployeeID = p.EmployeeID,
                         SpvEmployeeID = p.SpvEmployeeID,
                         //q.EmployeeName,
                         PerolehanData = p.PerolehanData,
                         LastProgress = p.LastProgress,
                         QuantityInquiry = p.QuantityInquiry,
                         TestDrive = s.LookUpValue,
                         TelpRumah = p.TelpRumah,
                         NamaPerusahaan = p.NamaPerusahaan,
                         AlamatProspek = p.AlamatProspek,
                         AlamatPerusahaan = p.AlamatPerusahaan,
                         Handphone = p.Handphone,
                         InquiryDate = p.InquiryDate,
                         CityID = p.CityID,
                         CityName = t.LookUpValueName,
                         Jabatan = p.Jabatan,
                         CaraPembayaran = p.CaraPembayaran,
                         DownPayment = p.DownPayment,
                         Faximile = p.Faximile,
                         LastUpdateStatus = p.LastUpdateStatus,
                         Leasing = p.Leasing,
                         StatusProspek = p.StatusProspek,
                         Tenor = p.Tenor,
                         Email = p.Email,
                         LostCaseDate = p.LostCaseDate,
                         LostCaseCategory = p.LostCaseCategory,
                         LostCaseOtherReason = p.LostCaseOtherReason,
                         LostCaseReasonID = p.LostCaseReasonID,
                         LostCaseVoiceOfCustomer = p.LostCaseVoiceOfCustomer,
                         MerkLain = p.MerkLain
                      };

            if (string.IsNullOrEmpty(NikSales))
            {
                //qry = qry.Where(x => subordinates.Contains(x.EmployeeID) == true);
                qry = qry.Where(x => salesman.Contains(x.EmployeeID) == true);
            }
            else
            {
                qry = qry.Where(p => p.EmployeeID.Equals(NikSales));
            }


            return(qry);
        }
    }
}