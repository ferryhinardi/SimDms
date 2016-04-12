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
    public class SpStockOpnameController : ApiController
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
        public IQueryable<sp_SpSOSelectforLookup> BrowseFormTagNo()
        {
            var uid = CurrentUser();
            var sql = string.Format("exec sp_SpSOSelectforLookup '{0}','{1}'", uid.CompanyCode, uid.BranchCode);
            var queryable = ctx.Context.Database.SqlQuery<sp_SpSOSelectforLookup>(sql).AsQueryable();
            return (queryable);
        }


        [HttpGet]
        public IQueryable<SpTrnStockTakingTemp> BrowseSeqNo(string stno)
        {
            var uid = CurrentUser();            
            var notin=ctx.Context.SpTrnStockTakingDtls
                       .Where(x=>x.CompanyCode==uid.CompanyCode
                                 && x.BranchCode==uid.BranchCode
                                 && x.STNo==stno)
                        .Select(x=>x.SeqNo)
                        .ToList();

            var tmp = ctx.Context.SpTrnStockTakingTemps
                    .Where(x => x.BranchCode == uid.BranchCode
                                && x.CompanyCode == uid.CompanyCode
                                && x.STNo == stno
                                && !notin.Contains(x.SeqNo))
                    .AsQueryable();                             
            return (tmp);
        }


        [HttpGet]
        public IQueryable<SpSOSelectforEntry> SelectEntryStockTaking(string warehouseCode)
        {
           // var uid = CurrentUserInfo(); 
            
            
            //var prt = ctx.Context.SpMstItemlocViews
            //            .Where(x => x.CompanyCode == uid.CompanyCode
            //                     && x.BranchCode == uid.BranchCode
            //                     && x.WarehouseCode == warehouseCode
            //                     && x.ProductType == uid.ProductType)
            //            .AsQueryable();

            var Uid = CurrentUserInfo();
            var queryable = ctx.Context.Database.SqlQuery<SpSOSelectforEntry>("[sp_SpSOSelectforEntry] '" + Uid.CompanyCode + "','" + Uid.BranchCode + "','" + Uid.ProductType + "','" + warehouseCode + "'").AsQueryable();
            return (queryable); 

            //var uid = CurrentUser();
            //var sql = string.Format("exec sp_SpSOSelectforLookup '{0}','{1}'", uid.CompanyCode, uid.BranchCode);
            //var queryable = ctx.Context.Database.SqlQuery<sp_SpSOSelectforLookup>(sql).AsQueryable();
            //return (queryable);


            //return prt;
        }
        
    }
}
