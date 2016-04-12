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
    public class spPenerimaanController : ApiController
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


        /*  Entry Claim Supplier */
        [HttpGet]
        public IQueryable<SpEntryCS> EntryCSBrowse()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<SpEntryCS>(" Select * from SpEntryCS where CompanyCode =  '" + Uid.CompanyCode + "' and BranchCode = '" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<WRSNoBrowse> WRSNoBrowse()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<WRSNoBrowse>(" Select * from SpWRSNo where CompanyCode =  '" + Uid.CompanyCode + "' and BranchCode = '" + Uid.BranchCode + "' and TypeOfGoods = '"+Uid.TypeOfGoods+"'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SpNoPartView> PartNoBrowse(string WRS)
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<SpNoPartView>(" Select * from SpPartNoView where CompanyCode =  '" + Uid.CompanyCode + "' and BranchCode = '" + Uid.BranchCode + "' and WRSNo = '" + WRS + "'").AsQueryable();
            return (queryable);
        }


        [HttpGet]
        public IQueryable<SpWrongNoPartView> WrongPartNo()
        {
            var Uid = CurrentUserInfo();

            var sql = string.Format("exec uspfn_SpWrongPart '{0}','{1}'", Uid.CompanyCode, Uid.ProductType);
            var queryable = ctx.Context.Database.SqlQuery<SpWrongNoPartView>(sql).AsQueryable();
            return (queryable);
        }

        /*  Receiving Claim Vendor  */
        [HttpGet]
        public IQueryable<SpVendorClaimView> RecClaimVendorBrowse()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<SpVendorClaimView>(" Select * from SpVendorClaimView where CompanyCode =  '" + Uid.CompanyCode + "' and BranchCode = '" + Uid.BranchCode + "' and TypeOfGoods = '"+Uid.TypeOfGoods+"'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SpRecClaimNo> RecClaimNo()
        {
            var Uid = CurrentUserInfo();

            var sql = string.Format("exec sp_SpReceiveClaimNo '{0}','{1}', {2}", Uid.CompanyCode, Uid.BranchCode, Uid.TypeOfGoods);
            var queryable = ctx.Context.Database.SqlQuery<SpRecClaimNo>(sql).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SpPartOrderView> SpPartOrderBrowse(string CLM)
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<SpPartOrderView>(" Select * from SpPartOrderView where CompanyCode =  '" + Uid.CompanyCode + "' and BranchCode = '" + Uid.BranchCode + "' and ClaimNo = '" + CLM + "'").AsQueryable();
            return (queryable);
        }

        /*  Entry HPP  */
        
        [HttpGet]
        public IQueryable<EntryHPPBrowse> EntryHPPBrowse()
        {
            var Uid = CurrentUserInfo();
            var record = ctx.Context.Database.SqlQuery<GnMstCoProfileSpare>(" Select * from GnMstCoProfileSpare WITH(NoLock,NoWait) where CompanyCode =  '" + Uid.CompanyCode + "' and BranchCode = '" + Uid.BranchCode + "'").FirstOrDefault();
            var sql = string.Format("exec sp_EntryHPPBrowse '{0}','{1}', {2}, {3}, {4}", Uid.CompanyCode, Uid.BranchCode, Uid.TypeOfGoods, record.PeriodBeg.ToString("yyyyMMdd"), record.PeriodEnd.ToString("yyyyMMdd"));
            var queryable = ctx.Context.Database.SqlQuery<EntryHPPBrowse>(sql).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SpWRSHpp> WRSHppBrowse()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<SpWRSHpp>(" Select * from SpWRSHpp where CompanyCode =  '" + Uid.CompanyCode + "' and BranchCode = '" + Uid.BranchCode + "' and TypeOfGoods = '" + Uid.TypeOfGoods + "'").AsQueryable();
            return (queryable);
        }

        /*  Entry Penerimaan Persediaan(WRS)  */
        
        [HttpGet]
        public IQueryable<SpTrnPRcvHdr> EntryWRSBrowse(string RecType)
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<SpTrnPRcvHdr>(" Select * from SpTrnPRcvHdr where CompanyCode =  '" + Uid.CompanyCode + "' and BranchCode = '" + Uid.BranchCode + "' and (Status = '0' OR Status = '1') and ReceivingType = '" + RecType + "' and TypeOfGoods = '" + Uid.TypeOfGoods + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<spTrnPBinnHdr> BinningNoBrowse(string RecType)
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<spTrnPBinnHdr>(" Select * from spTrnPBinnHdr where CompanyCode =  '" + Uid.CompanyCode + "' and BranchCode = '" + Uid.BranchCode + "' AND Status IN('2') and ReceivingType = '" + RecType + "' and TypeOfGoods = '" + Uid.TypeOfGoods + "'").AsQueryable();
            return (queryable);
        }


        /* Entry Draft Penerimaan (Binning) */

        [HttpGet]
        public IQueryable<spTrnPBinnHdr> EdpBrowse(string RecType)
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<spTrnPBinnHdr>(" Select * from spTrnPBinnHdr where CompanyCode =  '" + Uid.CompanyCode + "' and BranchCode = '" + Uid.BranchCode + "' and (Status = '0' OR Status = '1') and ReceivingType = '" + RecType + "' and TypeOfGoods = '" + Uid.TypeOfGoods + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SpEdpSupplier> EdpSupplierBrowse()
        {
            var Uid = CurrentUserInfo();
            var queryable = ctx.Context.Database.SqlQuery<SpEdpSupplier>(" Select * from SpEdpSupplier where CompanyCode =  '" + Uid.CompanyCode + "' and BranchCode = '" + Uid.BranchCode + "' and isBlackList = '0' and Status = '1' and ProfitCenterCode = '" + Uid.ProfitCenter + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<EdpPelangganBrowse> EdpPelangganBrowse()
        {
            var Uid = CurrentUserInfo();
            var sql = string.Format("exec sp_EdpPelangganBrowse '{0}','{1}', {2}", Uid.CompanyCode, Uid.BranchCode, Uid.ProfitCenter);
            var queryable = ctx.Context.Database.SqlQuery<EdpPelangganBrowse>(sql).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SpEdpDnNo> EdpDnNoBrowse()
        {
            var Uid = CurrentUserInfo();
            var queryable = ctx.Context.Database.SqlQuery<SpEdpDnNo>(" Select * from SpEdpDnNo where CompanyCode =  '" + Uid.CompanyCode + "' and BranchCode = '" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SpEdpDnNo> EdpDnNoBrowseNew()
        {
            var Uid = CurrentUserInfo();
            var queryable = ctx.Context.Database.SqlQuery<SpEdpDnNo>("exec uspfn_LookupNoDnPINVS  '{0}','{1}', '{2}", Uid.CompanyCode, Uid.BranchCode, Uid.TypeOfGoods).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<EdpTransNo> EdpTransNoBrowse(bool isTrex)
        {
            var Uid = CurrentUserInfo();
            var sql = string.Format("exec sp_EdpTransNo '{0}','{1}', '{2}', {3}", Uid.CompanyCode, Uid.BranchCode, Uid.TypeOfGoods, isTrex);
            var queryable = ctx.Context.Database.SqlQuery<EdpTransNo>(sql).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<EdpDocNo> EdpDocNoBrowse(string SupplierCode)
        {
            var Uid = CurrentUserInfo();
            var sql = string.Format("exec sp_EdpDocNoBrowse '{0}','{1}', '{2}', '{3}', '{4}'", Uid.CompanyCode, Uid.BranchCode, Uid.TypeOfGoods, Uid.ProductType, SupplierCode);
            var queryable = ctx.Context.Database.SqlQuery<EdpDocNo>(sql).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<EdpBpsNo> EdpBpsNoBrowse(string CustomerCode)
        {
            var Uid = CurrentUserInfo();
            var sql = string.Format("exec sp_EdpBpsNoBrowse '{0}','{1}', '{2}', '{3}', '{4}'", Uid.CompanyCode, Uid.BranchCode, Uid.TypeOfGoods, Uid.ProductType, CustomerCode);
            var queryable = ctx.Context.Database.SqlQuery<EdpBpsNo>(sql).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SpEdpPartNo> EdpPartNoBrowse(string CustomerCode, string BPSFNo)
        {
            var Uid = CurrentUserInfo();
            var queryable = ctx.Context.Database.SqlQuery<SpEdpPartNo>(" Select * from SpEdpPartNo where CompanyCode =  '" + Uid.CompanyCode + "' and BranchCode = '" + Uid.BranchCode + "' and CustomerCode='" + CustomerCode + "' and BPSFNo='" + BPSFNo + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SpEdpPartNo_Pembelian> EdpPartNo_PembelianBrowse(string DocNo, string bPPN)
        {
            var Uid = CurrentUserInfo();
            

                var sql = string.Format("exec sp_EdpPartNo_Pembelian '{0}','{1}', '{2}', {3}", Uid.CompanyCode, Uid.BranchCode, DocNo, bPPN);
                var queryable = ctx.Context.Database.SqlQuery<SpEdpPartNo_Pembelian>(sql).AsQueryable();
                return (queryable);

        }

        [HttpGet]
        public IQueryable<SpEdpPartNo_Internal> EdpPartNo_InternalBrowse()
        {
            var Uid = CurrentUserInfo();
            var queryable = ctx.Context.Database.SqlQuery<SpEdpPartNo_Internal>(" Select * from SpEdpPartNo_Internal where CompanyCode =  '" + Uid.CompanyCode + "' and BranchCode = '" + Uid.BranchCode + "' and ProductType='" + Uid.ProductType + "' and TypeOfGoods = '"+Uid.TypeOfGoods+"'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SpEdpPartNo_Others> EdpPartNo_OthersBrowse(string SupplierCode)
        {
            var Uid = CurrentUserInfo();
            var queryable = ctx.Context.Database.SqlQuery<SpEdpPartNo_Others>(" Select * from SpEdpPartNo_Others where CompanyCode =  '" + Uid.CompanyCode + "' and BranchCode = '" + Uid.BranchCode + "' and ProductType='" + Uid.ProductType + "' and TypeOfGoods = '" + Uid.TypeOfGoods + "' and SupplierCode='" + SupplierCode + "'").AsQueryable();
            return (queryable);
        }
 
    }
}
