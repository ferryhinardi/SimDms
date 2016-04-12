using Breeze.WebApi;
using Breeze.WebApi.EF;
using SimDms.Common;
using SimDms.Common.Models;
using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace SimDms.Web.Controllers.Api
{
    [BreezeController]
    public class SvUtilityController : ApiController
    {
        readonly EFContextProvider<DataContext> ctx = new EFContextProvider<DataContext>();

        [HttpGet]
        public String Metadata()
        {
            return ctx.Metadata();
        }

        //[HttpGet]
        //public SysUser CurrentUser()
        //{
        //    return ctx.Context.SysUsers.Find(User.Identity.Name);
        //}

        //public string GetLookupValueName(string CompanyCode, string VarGroup, string varCode)
        //{
        //    string s = "";
        //    var x = ctx.Context.LookUpDtls.Find(CompanyCode, VarGroup, varCode);
        //    if (x != null) s = x.LookUpValueName;
        //    return s;
        //}

        //[HttpGet]
        //public MyUserInfo CurrentUserInfo()
        //{
        //    string s = "";
        //    var f = ctx.Context.SysUserProfitCenters.Find(User.Identity.Name);
        //    if (f != null) s = f.ProfitCenter;
        //    var u = ctx.Context.SysUsers.Find(User.Identity.Name);
        //    var g = ctx.Context.CoProfiles.Find(u.CompanyCode, u.BranchCode);

        //    var info = new MyUserInfo
        //    {
        //        UserId = u.UserId,
        //        FullName = u.FullName,
        //        CompanyCode = u.CompanyCode,
        //        CompanyGovName = g.CompanyGovName,
        //        BranchCode = u.BranchCode,
        //        CompanyName = g.CompanyName,
        //        TypeOfGoods = u.TypeOfGoods,
        //        ProductType = g.ProductType,
        //        ProfitCenter = s,
        //        TypeOfGoodsName = GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.TypeOfGoods, u.TypeOfGoods),
        //        ProductTypeName = GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.ProductType, g.ProductType),
        //        ProfitCenterName = GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.ProfitCenter, s),
        //        IsActive = u.IsActive,
        //        RequiredChange = u.RequiredChange,
        //        SimDmsVersion = GetType().Assembly.GetName().Version.ToString()
        //    };

        //    return info;
        //}

        protected SysUserView CurrentUser
        {
            get
            {
                return ctx.Context.SysUserViews.FirstOrDefault(a => a.UserId == User.Identity.Name);

                //return ctx.SysUserViews.Find(User.Identity.Name);
                //return ctx.SysUserViews.Where(x => x.UserId==User.Identity.Name).FirstOrDefault();
            }
        }

        protected string ProfitCenter
        {
            get
            {
                var IsAdmin = ctx.Context.Database.SqlQuery<bool>(string.Format(@"select top 1 b.IsAdmin from sysusergroup a 
                    left join SysGroup b on b.GroupId = a.GroupId where 1 = 1 and a.UserId = '{0}' and b.GroupId = a.GroupId",
                    CurrentUser.UserId)).SingleOrDefault();
                string profitCenter = "200";
                if (!IsAdmin)
                {
                    string s = "000";
                    var x = ctx.Context.SysUserProfitCenters.Find(CurrentUser.UserId);
                    if (x != null) s = x.ProfitCenter;
                    return s;
                }
                else
                {
                    return profitCenter;
                }
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

        [HttpGet]
        public IQueryable<SvUtilMaintainChassis> MaintainChassisLookup()
        {
            #region Query
            var query = string.Format(@"
select distinct
 a.PoliceRegNo
,a.CustomerCode
,b.CustomerName
,  rtrim(rtrim(a.CustomerCode) + ' - ' + rtrim(b.CustomerName)) as CustomerDesc
,  rtrim(rtrim(b.Address1) + ' ' + rtrim(b.Address2) + ' ' + rtrim(b.Address3) + ' ' + rtrim(b.Address4)) as CustomerAddr
, b.Address1, b.Address2, b.Address3 + ' ' + b.Address4 Address3
,a.DealerCode
,c.CustomerName DealerName
, (a.CustomerCode + ' - ' + c.CustomerName) as DealerDesc
,a.ChassisCode
,  cast(a.ChassisNo as varchar) ChassisNo
,a.EngineCode
,  cast(a.EngineNo as varchar) EngineNo
,a.ServiceBookNo
,a.ClubCode
,a.ColourCode
,case a.FakturPolisiDate when ('19000101') then null else a.FakturPolisiDate end FakturPolisiDate
,a.ClubNo
,case a.ClubDateStart when ('19000101') then null else a.ClubDateStart end ClubDateStart
,case a.ClubDateFinish when ('19000101') then null else a.ClubDateFinish end ClubDateFinish
,case a.ClubSince when ('19000101') then null else a.ClubSince end ClubSince
, (case a.IsClubStatus when 1 then 'Aktif' else 'Tidak Aktif' end) as IsClubStatusDesc
,a.IsClubStatus
, (case a.IsContractStatus when 1 then 'Aktif' else 'Tidak Aktif' end) as IsContractStatusDesc
,a.IsActive
, (case a.IsActive when 1 then 'Aktif' else 'Tidak Aktif' end) as IsActiveDesc
,a.BasicModel
,a.TransmissionType
,case a.LastServiceDate when ('19000101') then null else a.LastServiceDate end LastServiceDate
,a.LastJobType
,a.ChassisNo
,a.ContractNo
,a.ContactName
,b.CityCode
,f.LookUpValueName CityName
from svMstCustomerVehicle a
left join gnMstCustomer b on b.CompanyCode = a.CompanyCode 
	and b.CustomerCode = a.CustomerCode
left join gnMstCustomer c on c.CompanyCode = a.CompanyCode 
	and c.CustomerCode = a.DealerCode
left join svMstJob d on 
	d.CompanyCode = a.CompanyCode and
	d.BasicModel = a.BasicModel 	
inner join gnMstCustomerProfitCenter e on 
    e.CompanyCode = a.CompanyCode and
    e.CustomerCode = a.CustomerCode
left join gnMstLookupDtl f on 
    f.CompanyCode = a.CompanyCode and
    f.CodeID = 'CITY' and
    f.LookUpValue = b.CityCode
where a.CompanyCode='{0}' and a.IsActive = 1 and e.ProfitCenterCode = '{1}' and e.BranchCode = '{2}'", CompanyCode, ProfitCenter, BranchCode);

            #endregion

            return ctx.Context.Database.SqlQuery<SvUtilMaintainChassis>(query).AsQueryable();
        }

        [HttpGet]
        public IQueryable<UploadFileType> FileTypeLookup()
        {
            var data = ctx.Context.svMstRefferenceServices.Where(a => a.CompanyCode == CompanyCode && a.ProductType == ProductType && a.RefferenceType.Equals("DOWNLOAD"))
                .Select(a => new UploadFileType
                {
                    RefferenceType = a.RefferenceType,
                    RefferenceCode = a.RefferenceCode,
                    Description = a.Description,
                    DescriptionEng = a.DescriptionEng,
                    IsActive = a.IsActive,
                    Status = a.IsActive ? "Aktif" : "Tidak Aktif"
                });

            return data;
        }

        [HttpGet]
        public IQueryable<SvUtilInvoiceCancelLookup> InvoiceCancelLookup()
        {
            var query = "exec uspfn_svSelectInvoiceForCancellation @p0, @p1, @p2, @p3";
            var result = ctx.Context.Database.SqlQuery<SvUtilInvoiceCancelLookup>(query,
                CompanyCode, BranchCode, ProductType, "").AsQueryable();

            return result;
        }
    }
}