using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Breeze.WebApi;
using Newtonsoft.Json.Linq;
using SimDms.Common.Models;
using SimDms.Sparepart.Models;
using Breeze.WebApi.EF;
using SimDms.Common;

namespace SimDms.Web.Controllers.Api
{
    [BreezeController]
    public class SpRptRegisterController : ApiController
    {
        readonly EFContextProvider<DataContext> ctx = new EFContextProvider<DataContext>();

        [HttpGet]
        public String Metadata()
        {
            return ctx.Metadata();
        }

        [HttpGet]
        public SysUser CurrentUser()
        {
            return ctx.Context.SysUsers.Find(User.Identity.Name);
        }

        public string GetLookupValueName(string CompanyCode, string VarGroup, string varCode)
        {
            string s = "";
            var x = ctx.Context.LookUpDtls.Find(CompanyCode, VarGroup, varCode);
            if (x != null) s = x.LookUpValueName;
            return s;
        }

        [HttpGet]
        public MyUserInfo CurrentUserInfo()
        {
            string s = "";
            var f = ctx.Context.SysUserProfitCenters.Find(User.Identity.Name);
            if (f != null) s = f.ProfitCenter;
            var u = ctx.Context.SysUsers.Find(User.Identity.Name);
            var g = ctx.Context.CoProfiles.Find(u.CompanyCode, u.BranchCode);

            var info = new MyUserInfo
            {
                UserId = u.UserId,
                FullName = u.FullName,
                CompanyCode = u.CompanyCode,
                CompanyGovName = g.CompanyGovName,
                BranchCode = u.BranchCode,
                CompanyName = g.CompanyName,
                TypeOfGoods = u.TypeOfGoods,
                ProductType = g.ProductType,
                ProfitCenter = s,
                TypeOfGoodsName = GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.TypeOfGoods, u.TypeOfGoods),
                ProductTypeName = GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.ProductType, g.ProductType),
                ProfitCenterName = GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.ProfitCenter, s),
                IsActive = u.IsActive,
                RequiredChange = u.RequiredChange,
                SimDmsVersion = GetType().Assembly.GetName().Version.ToString()
            };

            return info;
        }

        [HttpGet]
        public IQueryable<CoProfile> BrowseBranch()
        {
            var uid = CurrentUser();
            return ctx.Context.CoProfiles.Where(a => a.CompanyCode == uid.CompanyCode);
        }

        [HttpGet]
        public IQueryable<GnMstCustomer> BrowseCustomer()
        {
            var uid = CurrentUser();
            var query = string.Format(@"select distinct a.CustomerCode, b.CustomerName 
                from spTrnSLmpHdr a with(nolock, nowait)
                inner join gnMstCustomer b on 
                a.CompanyCode = b.CompanyCode 
                and a.CustomerCode = b.CustomerCode
                inner join spTrnSLmpDtl c on 
                a.CompanyCode = c.CompanyCode 
                and a.BranchCode = c.BranchCode 
                and a.LmpNo = c.LmpNo 
                where a.CompanyCode = {0}
                and a.TransType like '1%'
                order by a.CustomerCode", uid.CompanyCode);

            var queryable = ctx.Context.Database.SqlQuery<GnMstCustomer>(query).AsQueryable();
            return queryable;
        }

        [HttpGet]
        public IQueryable<GnMstCustomer> BrowseCustomerRptRgs(string dateFrom, string dateTo, string paymentCode,string partType )
        {
            var uid = CurrentUser();
            object[] parameters = { uid.CompanyCode, uid.BranchCode, dateFrom, dateTo, partType, paymentCode };
            var query = "uspfn_spCustRptRgs @p0,@p1,@p2,@p3,@p4,@p5";

            var queryable = ctx.Context.Database.SqlQuery<GnMstCustomer>(query, parameters).AsQueryable();
            return queryable;
        }

        [HttpGet]
        public IQueryable<ClaimNoRptRgs020> BrowseClaimNo(string part)
        {
            var uid = CurrentUser();
            var query = string.Format(@"
                    SELECT DISTINCT a.ClaimNo, CONVERT(varchar(12), ClaimDate, 106) AS ClaimDate 
                      FROM spTrnPClaimHdr a
                     WHERE a.CompanyCode = '{0}'
                       AND a.BranchCode = '{1}' 
                       AND a.TypeOfGoods LIKE '{2}' 
                       AND a.Status IN ('2')
                     ORDER BY a.ClaimNo ASC", uid.CompanyCode, uid.BranchCode, part);

            var queryable = ctx.Context.Database.SqlQuery<ClaimNoRptRgs020>(query).AsQueryable();
            return queryable;
        }

        [HttpGet]
        public IQueryable<SupplierRptRgs> BrowseSupplier()
        {
            var uid = CurrentUserInfo();
            object[] parameters = { uid.CompanyCode, uid.ProfitCenter};
            string query = @"
                            SELECT Distinct a.SupplierCode, a.SupplierName
                            FROM gnMstSupplier a
                            LEFT JOIN  GnMstSupplierProfitCenter b
                            ON b.CompanyCode = a.CompanyCode
                            AND b.SupplierCode = a.SupplierCode
                            WHERE a.CompanyCode = @p0";
            if (uid.ProfitCenter != "") query += " AND b.ProfitCenterCode = @p1";

            var queryable = ctx.Context.Database.SqlQuery<SupplierRptRgs>(query, parameters).AsQueryable();
            return queryable;
        }
    }
}
