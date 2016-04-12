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
    public class SpRptPenerimaanController : ApiController
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


        /*  Binning List Pembelian */
        [HttpGet]
        public IQueryable<RptBinningBrowse> BinningListBrowse()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<RptBinningBrowse>(" Select * from RptBinningBrowse where CompanyCode =  '" + Uid.CompanyCode + "' and BranchCode = '" + Uid.BranchCode + "' and TypeOfGoods = '" + Uid.TypeOfGoods + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<RptWRSBrowse> WRSBrowse(string TransType)
        {
            var qr = "";
            if (TransType == "5")
            {
                qr = "TransType <> '4'";
            }
            else
            {
                qr = "TransType = " + TransType;
            }


            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<RptWRSBrowse>(" Select * from RptWRSBrowse where CompanyCode =  '" + Uid.CompanyCode + "' and BranchCode = '" + Uid.BranchCode + "' and TypeOfGoods = '" + Uid.TypeOfGoods + "'and " + qr).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<RptHPPBrowse> EntryHPPBrwose()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<RptHPPBrowse>(" Select * from RptHPPBrowse where CompanyCode =  '" + Uid.CompanyCode + "' and BranchCode = '" + Uid.BranchCode + "' and TypeOfGoods = '" + Uid.TypeOfGoods + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<RptClaimNo> ClaimNoBrwose()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<RptClaimNo>(" Select * from RptClaimNo where CompanyCode =  '" + Uid.CompanyCode + "' and BranchCode = '" + Uid.BranchCode + "' and TypeOfGoods = '" + Uid.TypeOfGoods + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<RptClaimReceivedNo> ReceivedClaimNoBrwose()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<RptClaimReceivedNo>(" Select * from RptClaimReceivedNo where CompanyCode =  '" + Uid.CompanyCode + "' and BranchCode = '" + Uid.BranchCode + "' and TypeOfGoods = '" + Uid.TypeOfGoods + "'").AsQueryable();
            return (queryable);
        } 

    }
}
