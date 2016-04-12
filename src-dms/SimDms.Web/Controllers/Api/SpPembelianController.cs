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
    public class spPembelianController : ApiController
    {
        readonly EFContextProvider<DataContext> ctx =  new EFContextProvider<DataContext>();

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



        [HttpGet]
        public IQueryable<spPembelianView> PembelianBrowse()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<spPembelianView>(" Select * from SpPembelian where CompanyCode =  '" + Uid.CompanyCode + "' and BranchCode = '" + Uid.BranchCode + "' and TypeOfGoods='"+Uid.TypeOfGoods+"'").AsQueryable();
            return (queryable);
        }


        [HttpGet]
        public IQueryable<SpMstItemLocItemLookupView> TransPartNo()
        {
            var Uid = CurrentUser();

            var sql = string.Format("exec uspfn_spMasterPartView '{0}','{1}'", Uid.CompanyCode, Uid.BranchCode);
            var queryable = ctx.Context.Database.SqlQuery<SpMstItemLocItemLookupView>(sql).AsQueryable();
            return (queryable);
        }


        [HttpGet]
        public IQueryable<spTrnIAdjusthdrView> InventoryAdjustmentBrowse()
        {
            
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<spTrnIAdjusthdrView>("select AdjustmentNo,AdjustmentDate,ReferenceNo,ReferenceDate from spTrnIAdjustHdr where CompanyCode='" + Uid.CompanyCode + "' and  BranchCode='" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);
        }


        [HttpGet]
        public IQueryable<spTrnPPOSHdrView> spTrnPPOSHdrBrowse()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<spTrnPPOSHdrView>("  exec uspfn_spTrnPPOSHdr '" + Uid.CompanyCode + "','" + Uid.BranchCode + "','" + Uid.TypeOfGoods + "','2'").AsQueryable();
            return (queryable);
        }


    }
}
