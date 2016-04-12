using Breeze.WebApi;
using Breeze.WebApi.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

using SimDms.Common.Models;
using SimDms.Service.Models;
using SimDms.Common;

namespace SimDms.Web.Controllers.Api
{
    [BreezeController]
    public class SvReportController : ApiController
    {
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
            }
        }

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

        [HttpGet]
        public IQueryable<CoProfile> BrowseBranch()
        {
            return ctx.Context.CoProfiles.Where(a => a.CompanyCode == CompanyCode);
        }

        [HttpGet]
        public IQueryable<BatchBrowse> BatchBrowse()
        {
            int sentralize = 0;
            var lookUpDtl = ctx.Context.LookUpDtls.Find(CompanyCode, "SRV_FLAG", "CLM_HOLDING");
            if (lookUpDtl == null)
                sentralize = 1;
            else if (lookUpDtl.ParaValue == "1")
                sentralize = 1;

            var query = string.Format(@"                
            select 
	            a.BatchNo, a.BatchDate, a.ReceiptNo, 
                case convert(varchar, a.ReceiptDate, 112) when '19000101' Then '' else replace(convert(varchar, a.ReceiptDate, 106), ' ', '-') end as ReceiptDate, 
                a.FPJNo, 
                case convert(varchar, a.FPJDate, 112) when '19000101' Then '' else replace(convert(varchar, a.FPJDate, 106), ' ', '-') end as FPJDate, 
                a.FPJGovNo, a.LotNo, a.ProcessSeq,
	            sum(b.TotalNoOfItem) TotalNoOfItem, 
                sum(b.TotalClaimAmt) TotalClaimAmt, 
                sum(b.OtherCompensationAmt) OtherCompensationAmt
            from 
	            SvTrnClaim b
	            left join SvTrnClaimBatch a on a.CompanyCode = b.CompanyCode and 
		            a.BranchCode = b.BranchCode and a.ProductType = b.ProductType and
		            a.BatchNo = b.BatchNo
            where 
	            a.CompanyCode = {0}
	            and a.BranchCode LIKE case when {1} = 1 then '%%' else {2} end
	            and a.ProductType = '{3}'
            group By a.BatchNo, a.BatchDate, a.ReceiptNo, a.ReceiptDate, 
	            a.FPJNo, a.FPJDate, a.FPJGovNo, a.LotNo, a.ProcessSeq
            ", CompanyCode, sentralize, BranchCode, ProductType);

            var data = ctx.Context.Database.SqlQuery<BatchBrowse>(query).AsQueryable();

            return data;
        }
    }
}

