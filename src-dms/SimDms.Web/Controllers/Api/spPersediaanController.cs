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
    public class spPersediaanController : ApiController
    {
        readonly EFContextProvider<DataContext> ctx =  new EFContextProvider<DataContext>();
        private readonly string[] status = { "0", "1" };

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
            var g = ctx.Context.CoProfiles.Find(u.CompanyCode,u.BranchCode);

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
                TypeOfGoodsName = GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.TypeOfGoods  , u.TypeOfGoods),
                ProductTypeName = GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.ProductType, g.ProductType),
                ProfitCenterName = GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.ProfitCenter, s),
                IsActive = u.IsActive,
                RequiredChange = u.RequiredChange,
                SimDmsVersion = GetType().Assembly.GetName().Version.ToString()
            };

            return info;
        }

        [HttpPost]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return ctx.SaveChanges(saveBundle);
        }

        [HttpGet]
        public IQueryable<spMstAccount> MasterAccount()
        {
            return ctx.Context.spMstAccounts;
        }

        [HttpGet]
        public IQueryable<spMasterPartLookup> PartLookup()
        {
            return ctx.Context.spMasterPartLookups;
        }

        [HttpGet]
        public IQueryable<spMstMovingCode> MovingCode()
        {
            return ctx.Context.spMstMovingCodes;
        }

        [HttpGet]
        public IQueryable<spMstOrderParamView> ParamCode()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<spMstOrderParamView>(" sp_spMstOrderParamView '" + Uid.CompanyCode + "','" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);

        }


        [HttpGet]
        public IQueryable<SpMstItemLocItemLookupView> TransPartNo()
        {
            var Uid = CurrentUserInfo();

            var sql = string.Format("exec uspfn_spMasterPartView '{0}','{1}','{2}'", Uid.CompanyCode, Uid.BranchCode,"");
            var queryable = ctx.Context.Database.SqlQuery<SpMstItemLocItemLookupView>(sql).Where(a=>a.TypeOfGoods == Uid.TypeOfGoods && a.ProductType == Uid.ProductType).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SpWarehouseCodeView> WarehouseCodeMPWH()
        {
            var Uid = CurrentUserInfo();
            var queryable = from p in ctx.Context.LookUpDtls
                            where p.CodeID == "MPWH" 
                            select new SpWarehouseCodeView()
                            {
                                warehousecode = p.LookUpValue,
                                lookupvaluename = p.LookUpValueName
                            };
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SpWarehouseCodeView> WarehouseCode(string Id, string param)
        {
            var Uid = CurrentUser();
            //var queryable = ctx.Context.Database.SqlQuery<SpWarehouseCodeView>(" SELECT distinct	a.warehousecode,  b.lookupvaluename  FROM	 spMstItemLoc a inner join  gnMstLookUpDtl b on a.CompanyCode=b.CompanyCode  and  a.warehousecode=b.LookUpValue where   b.LookUpValue NOT LIKE 'X%' and b.CodeID='WRCD' and a.CompanyCode='" + Uid.CompanyCode + "' and a.BranchCode='" + Uid.BranchCode + "'").AsQueryable();

            var query = Id == "0" ? string.Format(@"SELECT
                    spMstItemLoc.WarehouseCode,
                    gnMstLookUpDtl.LookUpValueName
                    FROM spMstItemLoc
                    INNER JOIN gnMstLookUpDtl ON gnMstLookUpDtl.LookUpValue = spMstItemLoc.WarehouseCode 
                            and gnMstLookUpDtl.CompanyCode = spMstItemLoc.CompanyCode
                    WHERE
                    spMstItemLoc.PartNo = '{0}'
                    AND spMstItemLoc.CompanyCode = '{1}' 
                    AND spMstItemLoc.BranchCode = '{2}' 
                    AND gnMstLookUpDtl.CodeID='WRCD'
                    AND gnMstLookUpDtl.LookUpValue NOT LIKE 'X%'", param, Uid.CompanyCode, Uid.BranchCode) :
                    string.Format(@"SELECT a.LookUpValue as WarehouseCode
                          ,a.LookUpValueName
                    FROM gnMstLookUpDtl a
                    WHERE a.CodeID='WRCD' 
                     AND a.LookUpValue <> '{0}'
                     AND a.CompanyCode = '{1}'
                     AND a.LookUpValue NOT LIKE 'X%'", param, Uid.CompanyCode);

            var queryable = ctx.Context.Database.SqlQuery<SpWarehouseCodeView>(query).AsQueryable();
            
            return (queryable);
        }

        [HttpGet]
        public IQueryable<spTrnIAdjusthdrView> lnk5001Browse()
        {
            var Uid = CurrentUser();
            var query = string.Format(@"SELECT c.AdjustmentNo, c.AdjustmentDate, c.ReferenceNo, 
                        c.ReferenceDate, c.TypeOfGoods
                        FROM SpTrnIAdjustHdr c with(nolock,nowait)
                        WHERE c.CompanyCode = {0}
                        AND c.BranchCode = {1}
                        AND c.TypeOfGoods= {2}
                        AND c.Status in ('0','1')
                        ORDER BY c.AdjustmentNo DESC", Uid.CompanyCode, Uid.BranchCode, Uid.TypeOfGoods);

            //var queryable = ctx.Context.SpTrnIAdjustHdrs.Where(a => a.CompanyCode == Uid.CompanyCode && a.BranchCode == Uid.BranchCode
            //    && a.TypeOfGoods == Uid.TypeOfGoods && status.Contains(a.Status));


            var queryable = ctx.Context.Database.SqlQuery<spTrnIAdjusthdrView>(query).AsQueryable();

            //var queryable = ctx.Context.Database.SqlQuery<spTrnIAdjusthdrView>("select AdjustmentNo,AdjustmentDate,ReferenceNo,ReferenceDate from spTrnIAdjustHdr where Status=0 and CompanyCode='" + Uid.CompanyCode + "' and  BranchCode='" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<spTrnIWHTrfhdrView> lnk5002Browse()
        {
            var Uid = CurrentUserInfo();

            var query = string.Format(@"
                SELECT c.WHTrfNo, c.WHTrfDate, c.ReferenceNo, c.ReferenceDate,
                c.TypeOfGoods 
                FROM spTrnIWHTrfHdr c WITH(NOLOCK, NOWAIT)
                WHERE c.CompanyCode = {0}
                AND c.BranchCode = {1}
                AND c.TypeOfGoods = {2}
                AND c.Status in ('0','1')
                ORDER BY  c.WHTrfNo DESC", Uid.CompanyCode, Uid.BranchCode, Uid.TypeOfGoods);

            //var queryable = ctx.Context.spTrnIWHTrfHdrs.Where(a => a.CompanyCode == Uid.CompanyCode && a.BranchCode == Uid.BranchCode
            //    && a.TypeOfGoods == Uid.TypeOfGoods && status.Contains(a.Status));

            var queryable = ctx.Context.Database.SqlQuery<spTrnIWHTrfhdrView>(query).AsQueryable();

            //var queryable = ctx.Context.Database.SqlQuery<spTrnIWHTrfhdrView>("select WHTrfNo,WHTrfDate,ReferenceNo,ReferenceDate from spTrnIWHTrfHdr where Status=0 and CompanyCode='" + Uid.CompanyCode + "' and  BranchCode='" + Uid.BranchCode + "'").AsQueryable();
            
            return (queryable);
        }

        [HttpGet]
        public IQueryable<spTrnIReservedHdrView> lnk5003Browse()
        {
            var Uid = CurrentUser();
            var query = string.Format(@"
                SELECT c.ReservedNo, c.ReservedDate, c.ReferenceNo, 
                c.ReferenceDate, c.TypeOfGoods, c.OprCode
                FROM spTrnIReservedHdr c WITH (NOLOCK, NOWAIT)
                WHERE c.CompanyCode = '{0}'
                   AND c.BranchCode = '{1}'
                	 AND c.TypeOfGoods= '{2}'
                   AND c.Status in ('0','1')
                ORDER BY c.ReservedNo DESC", Uid.CompanyCode, Uid.BranchCode, Uid.TypeOfGoods);

            var queryable = ctx.Context.Database.SqlQuery<spTrnIReservedHdrView>(query).AsQueryable();

            //var queryable = ctx.Context.Database.SqlQuery<spTrnIReservedHdrView>("select ReservedNo,ReservedDate,ReferenceNo,ReferenceDate,PartNo,TypeOfGoods,OprCode,Status from spTrnIReservedHdr where Status=0 and  CompanyCode='" + Uid.CompanyCode + "' and  BranchCode='" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);
        }
    }
}
