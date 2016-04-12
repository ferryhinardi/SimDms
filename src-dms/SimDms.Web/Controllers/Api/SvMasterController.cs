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
    public class SvMasterController : ApiController
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
        public IQueryable<svMstStallView> StallBrowse()
        {
            var queryable = ctx.Context.svMstStallViews.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.ProductType == ProductType).OrderBy(x => x.StallCode);
            return (queryable);
        }

        [HttpGet]
        public IQueryable<CustomerVehicleBrowse> CustVehBrowse()
        {
            var queryable = from p in ctx.Context.CustomerVehicles
                            join q in ctx.Context.Customers
                                on new { p.CompanyCode, p.CustomerCode } equals new { q.CompanyCode, q.CustomerCode }
                            join r in ctx.Context.svGnMstCustomerProfitCenters
                                on new { q.CompanyCode, q.CustomerCode } equals new { r.CompanyCode, r.CustomerCode }
                            where p.CompanyCode == CompanyCode && r.BranchCode == BranchCode && r.ProfitCenterCode == "200"
                            select new CustomerVehicleBrowse()
                             {
                                 PoliceRegNo = p.PoliceRegNo,
                                 ServiceBookNo = p.ServiceBookNo,
                                 FakturPolisiDate = p.FakturPolisiDate,
                                 ChassisCode = p.ChassisCode,
                                 ChassisNo = p.ChassisNo,
                                 BasicModel = p.BasicModel,
                                 EngineCode = p.EngineCode,
                                 EngineNo = p.EngineNo,
                                 ColourCode = p.ColourCode,
                                 TransmissionType = p.TransmissionType,
                                 ProductionYear = p.ProductionYear,
                                 CustomerCode = p.CustomerCode,
                                 CustomerName = q.CustomerName,
                                 DealerCode = p.DealerCode,
                                 ContactName = q.CustomerName,
                                 ContactAddress = q.Address1 + " " + q.Address2 + " " + q.Address3 + " " +q.Address4,
                                 ContactPhone = q.HPNo,
                                 IsActive = p.IsActive
                             };
            //var queryable = ctx.Context.SvKendaraanPels.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ProfitCenterCode == "200");

            return (queryable);
        }

        [HttpGet]
        public IQueryable<SvChassicView> ChassicCodeOpen()
        {
            var queryable = ctx.Context.SvChassicViews.Where(a => a.CompanyCode == CompanyCode);
            return (queryable);
        }

        [HttpGet]
        public IQueryable<svMstJobView> JobBrowse() 
        {
            var Product = ctx.Context.CoProfiles.Find(CompanyCode, BranchCode).ProductType;
            var queryable = from p in ctx.Context.svMstJobs
                            join p1 in ctx.Context.svMstRefferenceServices on new { p.CompanyCode, p.ProductType, p.JobType } equals new { p1.CompanyCode, p1.ProductType, JobType = p1.RefferenceCode }
                            join p2 in ctx.Context.svMstRefferenceServices on new { p.CompanyCode, p.ProductType, p.GroupJobType } equals new { p2.CompanyCode, p2.ProductType, GroupJobType = p2.RefferenceCode }
                            where p.CompanyCode == CompanyCode && p.ProductType == ProductType && p1.RefferenceType == "JOBSTYPE" && p2.RefferenceType == "GRPJOBTY"
                            select new svMstJobView
                            {
                                BasicModel = p.BasicModel,
                                JobType = p.JobType,
                                JobDescription = p1.Description,
                                GroupJobType = p.GroupJobType,
                                GroupJobDescription = p2.Description,
                                WarrantyOdometer = p.WarrantyOdometer,
                                WarrantyTimePeriod = p.WarrantyTimePeriod,
                                WarrantyTimeDim = p.WarrantyTimeDim == "D" ? "Hari" : (p.WarrantyTimeDim == "M" ? "Bulan" : "Tahun"),
                                IsPdiFscStr = p.IsPdiFsc.Value == true ? "Ya" : "Tidak",
                                PdiFscSeq = p.PdiFscSeq,
                                Status = p.IsActive.Value == true ? "Ya" : "Tidak"
                            };
            
            
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SvBasicModelPekerjaan> BasmodPekerjaan()
        {
            var Product = ctx.Context.CoProfiles.Find(CompanyCode, BranchCode).ProductType;
            var queryable = ctx.Context.SvBasicModelPekerjaans.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType).AsQueryable();
            return (queryable);
        }


        [HttpGet]
        public IQueryable<SvJobView> JobView()
        {
            var Product = ctx.Context.CoProfiles.Find(CompanyCode, BranchCode).ProductType;
            var queryable = ctx.Context.SvJobViews.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SvGroupJobView> GroupJobView()
        {
            var Product = ctx.Context.CoProfiles.Find(CompanyCode, BranchCode).ProductType;
            var queryable = ctx.Context.SvGroupJobViews.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SvNomorAccView> NomorAccView()
        {
            var queryable = ctx.Context.SvNomorAccViews.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SvRincianBrowser> RincianBrowser(string refcode, string refcode1)
        {
            var Product = ctx.Context.CoProfiles.Find(CompanyCode, BranchCode).ProductType;
            var queryable = ctx.Context.SvRincianBrowsers.Where(a => a.CompanyCode == CompanyCode && a.ProductType == ProductType && a.BasicModel == refcode && a.JobType == refcode1).AsQueryable();
            return (queryable); 
        }

        [HttpGet]
        public IQueryable<svMstTaskview> lookupforJob(string productType, string basicModel, string jobType) 
        {
            var queryable = ctx.Context.svMstTasks.Where(a => a.CompanyCode == CompanyCode && a.ProductType == productType && a.BasicModel == basicModel && a.JobType == jobType)
                .Select(a => new svMstTaskview
                {
                    OperationNo = a.OperationNo,
                    Description = a.Description,
                    TechnicalModelCode = a.TechnicalModelCode,
                    IsSubCon = a.IsSubCon.Value ? "Ya" : "Tidak",
                    IsCampaign = a.IsCampaign ? "Ya" : "Tidak",
                    IsActive = a.IsActive ? "Aktif" : "Tidak Aktif",
                    OperationHour = a.OperationHour,
                    ClaimHour = a.ClaimHour
                });

            return (queryable);
        }

        [HttpGet]
        public IQueryable<ModelCodeOpen> BasicModelOpen()
        {
            var query = string.Format(@"Select a.RefferenceCode AS BasicModel, a.DescriptionEng AS TechnicalModelCode,
            a.Description AS ModelDescription, CASE a.IsActive WHEN '1' THEN 'Aktif' ELSE 'Tidak Aktif' END AS Status from svMstrefferenceService a
            WHERE a.CompanyCode = '{0}'AND a.ProductType = '{1}' AND a.RefferenceType = 'BASMODEL'
            ", CompanyCode, ProductType);

            var sqlstr = ctx.Context.Database.SqlQuery<ModelCodeOpen>(query).AsQueryable();
            return (sqlstr);
        }

        [HttpGet]
        public IQueryable<ColourCodeOpen> ColourCodeOpen()
        {
            var query = string.Format(@"select RefferenceCode, RefferenceDesc1, RefferenceDesc2 
                from omMstRefference
                where CompanyCode = '{0}' and RefferenceType = 'COLO' 
                order by RefferenceDesc1
            ", CompanyCode);

            var sqlstr = ctx.Context.Database.SqlQuery<ColourCodeOpen>(query).AsQueryable();
            return (sqlstr);
        }

        [HttpGet]
        public IQueryable<CustomerCodeOpen> CustomerCodeOpen()
        {
            var query = string.Format(@"SELECT a.CustomerCode, a.CustomerName, a.Address1, a.Address2, a.Address3, a.Address4, 
                a.Address1 + ' ' + a.Address2 + ' ' + a.Address3 + ' ' + a.Address4 AS Address, 
                CASE a.Status WHEN 1 THEN 'Aktif' ELSE 'TIdak Aktif' END AS Status,
                a.HPNo AS ContactPhone, a.Address1 + ' ' + a.Address2 + ' ' + a.Address3 + ' ' + a.Address4 AS ContactAddress, a.CustomerName ContactName
                  FROM gnMstCustomer a with(nolock, nowait)
                  INNER JOIN gnMstCustomerProfitCenter b
                    ON a.CustomerCode = b.CustomerCode
                WHERE  a.CompanyCode = '{0}' AND b.ProfitCenterCode = '200'
				GROUP BY a.CustomerCode, a.CustomerName, a.Address1, a.Address2, a.Address3, a.Address4, a.Status, a.HPNo
                ORDER BY a.CustomerName ASC
            ", CompanyCode);

            var sqlstr = ctx.Context.Database.SqlQuery<CustomerCodeOpen>(query).AsQueryable();
            return (sqlstr);
        }

        #region PRint Customer Vehicle

        [HttpGet]
        public IQueryable<MstVehicleLookup> ChassisCode4Lookup()
        {
            var query = string.Format(@"select		
	            b.PoliceRegNo , b.CustomerCode, c.CustomerName, a.ChassisCode, 
                convert(varchar, a.ChassisNo) ChassisNo, a.EngineCode, convert(varchar ,a.EngineNo) EngineNo, b.ServiceBookNo, b.FakturPolisiDate, 
                b.ClubCode
            from		
	            omMstVehicle a, svMstCustomerVehicle b, gnMstCustomer c
            where		
	            a.ChassisCode = b.ChassisCode
                and a.ChassisNo = b.ChassisNo
	            and b.CustomerCode = c.CustomerCode
                and a.CompanyCode = '{0}'    
            order by	
	            b.PoliceRegNo asc
            ", CompanyCode);

            var sqlstr = ctx.Context.Database.SqlQuery<MstVehicleLookup>(query).AsQueryable();
            return (sqlstr);
        }

        [HttpGet]
        public IQueryable<MstVehicleLookup> EngineCode4Lookup()
        {
            var query = string.Format(@"
                SELECT PoliceRegNo,svMstCustomerVehicle.CustomerCode,
	            gnMstCustomer.CustomerName, gnMstCustomer.Address1, ChassisCode, convert(varchar, ChassisNo) ChassisNo, EngineCode,
                convert(varchar ,EngineNo) EngineNo,
	            ServiceBookNo, ClubCode, ClubNo, ClubDateStart,  ClubDateFinish, ClubSince,
	            (CASE IsClubStatus WHEN 1 then 'Aktif' ELSE 'Tidak Aktif' end) AS IsClubStatus,
	            (CASE IsContractStatus WHEN 1 then 'Aktif' ELSE 'Tidak Aktif' end) AS IsContractStatus,
	            (CASE IsActive WHEN 1 then 'Aktif' ELSE 'Tidak Aktif' end) AS IsActive,
                BasicModel, TransmissionType, 
                case LastServiceDate when ('19000101') then null else LastServiceDate end as LastServiceDate, 
                LastServiceOdometer, 
                (gnMstCustomer.CustomerCode + ' ' + gnMstCustomer.CustomerName) AS Customer,
                LastJobType,
                ContactName
            FROM svMstCustomerVehicle
            LEFT JOIN gnMstCustomer ON gnMstCustomer.CompanyCode = svMstCustomerVehicle.CompanyCode AND
                gnMstCustomer.CustomerCode = svMstCustomerVehicle.CustomerCode
            WHERE svMstCustomerVehicle.CompanyCode = '{0}'
            ORDER BY PoliceRegNo ASC
            ", CompanyCode);

            var sqlstr = ctx.Context.Database.SqlQuery<MstVehicleLookup>(query).AsQueryable();
            return (sqlstr);
        }

        [HttpGet]
        public IQueryable<MstVehicleLookup> BasicModel4Lookup()
        {
            var query = string.Format(@"
                select x.BasicModel ,x.TechnicalModelCode
                ,case x.Status when 1 then 'Aktif' else 'Tidak Aktif' end [Status]
                ,[ModelDescription] = isnull((
                       select top 1 SalesModelDesc
                       from omMstModel
                       where CompanyCode = x.CompanyCode
                         and BasicModel = x.BasicModel
                         and TechnicalModelCode = x.TechnicalModelCode
                       order by LastUpdateDate desc),'')
                from (
                select distinct
                 a.CompanyCode ,a.BasicModel ,a.TechnicalModelCode ,a.Status
                from omMstModel a
                where a.CompanyCode = '{0}'
                ) x
                order by x.BasicModel
            ", CompanyCode);

            var sqlstr = ctx.Context.Database.SqlQuery<MstVehicleLookup>(query).AsQueryable();
            return (sqlstr);
        }

        #endregion

        [HttpGet]
        public IQueryable<svMstRefferenceServiceView> ReffService()
        {
            var queryable = ctx.Context.svMstRefferenceServiceViews.Where(x => x.CompanyCode == CompanyCode
                 && x.ProductType == ProductType).OrderBy(x => x.CompanyCode);

            return queryable;
        }

        [HttpGet]
        public IQueryable<svMstBillTypeView> BillTypeBrowse()
        {
            var queryable = ctx.Context.svMstBillTypeViews.Where(x => x.CompanyCode == CompanyCode).OrderBy(y => y.BillType);

            return queryable;
        }

        [HttpGet]
        public IQueryable<PartInfoView> PartInfo4Sv() 
        {
            var query = string.Format(@"
                select TOP 2500 itemInfo.PartNo
                     , (item.OnHand - (item.AllocationSP + item.AllocationSR + item.AllocationSL) 
                     - (item.ReservedSP + item.ReservedSR + item.ReservedSL)) as Available
                     , itemPrice.RetailPriceInclTax, itemInfo.PartName, item.TypeOfGoods
                     , case gtgo.ParaValue when 'SPAREPART' then 'SPAREPART' else 'MATERIAL' end GroupTypeOfGoods
                     , (case itemInfo.Status when 1 then 'Aktif' else 'Tidak Aktif' end) as Status
                     , itemPrice.RetailPrice as NilaiPart
                  from spMstItemInfo itemInfo 
                  left join spMstItems item on item.CompanyCode = itemInfo.CompanyCode 
                   and item.BranchCode = '{1}'
                   and item.ProductType = '{2}'
                   and item.PartNo = itemInfo.PartNo
                 inner join spMstItemPrice itemPrice on itemPrice.CompanyCode = itemInfo.CompanyCode 
                   and itemPrice.BranchCode = '{1}'
                   and itemPrice.PartNo = item.PartNo
                  left join spMstItemLoc ItemLoc on itemInfo.CompanyCode = ItemLoc.CompanyCode 
                   and ItemLoc.BranchCode = '{1}'
                   and itemInfo.PartNo = ItemLoc.PartNo
                   and ItemLoc.WarehouseCode = '00'
                  left join gnMstLookupDtl gtgo
                    on gtgo.CompanyCode = item.CompanyCode
                   and gtgo.CodeID = 'GTGO'
                   and gtgo.LookupValue = item.TypeOfGoods
                where itemInfo.CompanyCode = '{0}'
                   and itemInfo.ProductType = '{2}'
                   and itemLoc.partno is not null
            ", CompanyCode, BranchCode, ProductType);

            var sqlstr = ctx.Context.Database.SqlQuery<PartInfoView>(query).AsQueryable();
            return (sqlstr);
        }

        [HttpGet]
        public IQueryable<PackageLookUp> PackageLookUp()
        {
            var query = string.Format(@"
                SELECT 
                PackageCode
                , PackageCode + ' - ' + PackageName Package
                , PackageName
                , BasicModel
                , BasicModel + ' - ' + svMstRefferenceService.Description BasicMod
                , BillTo + ' - ' + gnMstCustomer.CustomerName CustomerBill
                , BillTo
                , gnMstCustomer.CustomerName
                , IntervalYear
                , IntervalKM   
                , svMstRefferenceService.Description BasicModelDesc
                , PackageDesc
            FROM 
                svMstPackage
            LEFT JOIN gnMstCustomer ON
                svMstPackage.CompanyCode = gnMstCustomer.CompanyCode
                AND svMstPackage.BillTo = gnMstCustomer.CustomerCode
            LEFT JOIN svMstRefferenceService ON
                svMstPackage.CompanyCode = svMstRefferenceService.CompanyCode
                AND svMstPackage.BasicModel = svMstRefferenceService.RefferenceCode
                AND svMstRefferenceService.ProductType = '{0}'
                AND svMstRefferenceService.RefferenceType = 'BASMODEL'
            WHERE svMstPackage.CompanyCode = '{1}'
            ORDER BY PackageCode ASC
            ", ProductType, CompanyCode);

            var sqlstr = ctx.Context.Database.SqlQuery<PackageLookUp>(query).AsQueryable();
            return (sqlstr);
        }

        [HttpGet]
        public IQueryable<SvMstMecAvbView> MecAvbOpen()
        {
            var queryable = ctx.Context.SvMstMecAvbViews.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode).OrderBy(p => p.EmployeeName);
            return queryable;
        }

        [HttpGet]
        public IQueryable<SvMstGaransiView> GaransiOpen()
        {
            var queryable = ctx.Context.SvMstGaransiViews.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType).OrderBy(p => new { BasicModel = p.BasicModel, OperationNo = p.OperationNo } );
            return queryable;
        }

        [HttpGet]
        public IQueryable<SvCampaignView> CampaignOpen()
        {
            var queryable = ctx.Context.SvCampaignViews.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType);
            return queryable.OrderBy(p => new { p.ComplainCode, p.DefectCode, p.ChassisCode, p.ChassisStartNo, p.ChassisEndNo });
        }

        [HttpGet]
        public IQueryable<svMstPdiFscView> KSGOpen()
        {
            var queryable = ctx.Context.svMstPdiFscViews.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType);
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SvBasicKsgView> BasicKsgOpen()
        {
            var queryable = ctx.Context.SvBasicKsgViews.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType);
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SvRegCampaign> RegCampaignBrowse()
        {
            var queryable = ctx.Context.SvRegCampaigns.Where(a => a.CompanyCode == CompanyCode);
            return (queryable);
        }

        [HttpGet]
        public IQueryable<MstCustomerVehicleForMstRegCompaign> btnCampaign()
        {
            var query = string.Format(@"
                select	vehicle.PoliceRegNo 
		                , vehicle.ChassisCode
		                , vehicle.ChassisNo
		                , vehicle.EngineCode
		                , vehicle.EngineNo
                        , vehicle.ServiceBookNo		               
		                , vehicle.CustomerCode
		                , cust.CustomerName
                        , cust.Address1
                        , cust.Address2
                        , cust.Address3
                        , cust.CityCode 
                        , isnull(cust.IbuKota,'') CityName		                
                from	svMstCustomerVehicle vehicle with(nolock, nowait)
		                inner join gnMstCustomer cust with(nolock, nowait) on vehicle.CompanyCode = cust.CompanyCode
			                and vehicle.CustomerCode = cust.CustomerCode		                                                          
                where vehicle.CompanyCode = '{0}' and PoliceRegNo != ''
                order by  PoliceRegNo, CustomerName
            ", CompanyCode, BranchCode);

            var sqlstr = ctx.Context.Database.SqlQuery<MstCustomerVehicleForMstRegCompaign>(query).AsQueryable();
            return (sqlstr);
        }

        [HttpGet]
        public IQueryable<SvTargetView> TargetBrowse()
        {
            var queryable = ctx.Context.SvTargetViews.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ProductType == ProductType);
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SvRegPackageView> RegPackageBrowser()
        {
            var queryable = ctx.Context.SvRegPackageViews.Where(a => a.CompanyCode == CompanyCode);
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SvKendaraanPel> KendaraanPel()
        {
            var queryable = ctx.Context.SvKendaraanPels.Where(a => a.CompanyCode == CompanyCode && a.ProfitCenterCode == ProfitCenter);
            return (queryable);
        }

        [HttpGet]
        public IQueryable<svMstPackage> RegPackageOpen(string ModelCode)
        {
            var parameters = ModelCode.Split('?');
            var BasicModel = parameters[0];
            var queryable = ctx.Context.svMstPackages.Where(a => a.CompanyCode == CompanyCode && a.BasicModel == BasicModel);
            return (queryable);
        }

        [HttpGet]
        public IQueryable<svMstPackageBrowse> PackageBrowse()
        {
            var queryable = (from p in ctx.Context.svMstPackages
                            join q in ctx.Context.SvCBasmodViews
                                on p.BasicModel equals q.BasicModel
                            join r in ctx.Context.SvJobViews
                                on p.JobType equals r.JobType
                            join s in ctx.Context.SvPaymentPackages
                                on p.BillTo equals s.CustomerCode
                            select new svMstPackageBrowse()
                            {
                                PackageCode = p.PackageCode,
                                PackageName = p.PackageName,
                                PackageDesc = p.PackageDesc,
                                BasicModel = p.BasicModel,
                                ModelDescription = q.ModelDescription,
                                JobType = p.JobType,
                                JobDescription = r.JobDescription,
                                BillTo = p.BillTo,
                                CustomerName = s.CustomerName,
                                IntervalYear = p.IntervalYear,
                                IntervalKM = p.IntervalKM,
                                PackageSrvSeq = p.PackageSrvSeq
                            }).Distinct();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SvCBasmodView> CBasmodOpen()
        {
            var queryable = ctx.Context.SvCBasmodViews.Where(a => a.CompanyCode == CompanyCode && a.ProductType == ProductType);
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SvPaymentPackage> PaymentOpen()
        {
            var queryable = ctx.Context.SvPaymentPackages.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ProfitCenterCode == ProfitCenter);
            return (queryable);
        }

        [HttpGet]
        public IQueryable<OpNumberPackage> OperationPackage(string BasicModel, string JobType) 
        {
            #region ==QUERY==
            var sql = string.Format(@"
                select a.OperationNo
	             , case '{4}' when 'CLAIM' then isnull(b.ClaimHour, a.ClaimHour) else isnull(b.OperationHour, a.OperationHour) end as OperationHour
	             , case '{4}' when 'CLAIM' then a.LaborCost else isnull(b.LaborPrice, a.LaborPrice) end as LaborPrice
	             , a.Description as OperationName
	             , case a.IsActive when 1 then 'Aktif' else 'Tidak Aktif' end as IsActive
              from svMstTask a
              left join svMstTaskPrice b
                on b.CompanyCode = a.CompanyCode
               and b.BranchCode  = '{1}'
               and b.ProductType = a.ProductType
               and b.BasicModel  = a.BasicModel
               and b.JobType     = a.JobType
               and b.OperationNo = a.OperationNo
             where 1 = 1
               and a.CompanyCode = '{0}'
               and a.ProductType = '{2}'
               and a.BasicModel  = '{3}'
               and a.IsActive    = 1
            ORDER BY a.OperationNo ASC
            ", CompanyCode, BranchCode, ProductType, BasicModel, JobType);
            #endregion
            var data = ctx.Context.Database.SqlQuery<OpNumberPackage>(sql).AsQueryable();
            return (data);
        }

        [HttpGet]
        public IQueryable<NoPart> NoPartPack()
        {
            #region ==QUERY==
            var sql = string.Format(@"
                select itemInfo.PartNo
                 , (item.OnHand - (item.AllocationSP + item.AllocationSR + item.AllocationSL) 
                 - (item.ReservedSP + item.ReservedSR + item.ReservedSL)) as Available
                 , itemPrice.RetailPriceInclTax, itemInfo.PartName, item.TypeOfGoods
                 , case gtgo.ParaValue when 'SPAREPART' then 'SPAREPART' else 'MATERIAL' end GroupTypeOfGoods
                 , (case itemInfo.Status when 1 then 'Aktif' else 'Tidak Aktif' end) as Status
                 , itemPrice.RetailPrice as NilaiPart
              from spMstItemInfo itemInfo 
              left join spMstItems item on item.CompanyCode = itemInfo.CompanyCode 
               and item.BranchCode = '{1}'
               and item.ProductType = '{2}'
               and item.PartNo = itemInfo.PartNo
             inner join spMstItemPrice itemPrice on itemPrice.CompanyCode = itemInfo.CompanyCode 
               and itemPrice.BranchCode = '{1}'
               and itemPrice.PartNo = item.PartNo
              left join spMstItemLoc ItemLoc on itemInfo.CompanyCode = ItemLoc.CompanyCode 
               and ItemLoc.BranchCode = '{1}'
               and itemInfo.PartNo = ItemLoc.PartNo
               and ItemLoc.WarehouseCode = '00'
              left join gnMstLookupDtl gtgo
                on gtgo.CompanyCode = item.CompanyCode
               and gtgo.CodeID = 'GTGO'
               and gtgo.LookupValue = item.TypeOfGoods
            where itemInfo.CompanyCode = '{0}'
               and itemInfo.ProductType = '{2}'
               and itemLoc.partno is not null
            ", CompanyCode, BranchCode, ProductType);
            #endregion
            var data = ctx.Context.Database.SqlQuery<NoPart>(sql).AsQueryable();
            return (data);
        }

        [HttpGet]
        public IQueryable<SvKontrakServiceView> KontrakServiceOpen()
        {
            var queryable = ctx.Context.SvKontrakServiceViews.Where(a => a.CompanyCode == CompanyCode);
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SvCustomerDetailView> CutomerDetailOpen()
        {
            var queryable = ctx.Context.SvCustomerDetailViews.Where(a => a.CompanyCode == CompanyCode && a.ProfitCenterCode == ProfitCenter);
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SvVehicleDetailView> VehicleDetailOpen(string CustomerCode)
        {
            var parameters = CustomerCode.Split('?');
            var Customer = parameters[0];
            var queryable = ctx.Context.SvVehicleDetailViews.Where(a => a.CompanyCode == CompanyCode && a.CustomerCode == Customer);
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SvEventView> EventBrowse()
        {
            var queryable = ctx.Context.SvEventViews.Where(a => a.CompanyCode == CompanyCode);
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SvTarifJasaView> TafjaOpen()
        {
            var queryable = ctx.Context.SvTarifJasaViews.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType && x.BranchCode == BranchCode);
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SvClubView> ClubOpen()
        {
            var queryable = ctx.Context.SvClubViews.Where(x => x.CompanyCode == CompanyCode);
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SvNoPolisi> NoPolisiOpen()
        {
            var queryable = ctx.Context.SvNoPolisis.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode);
            return (queryable);
        }

    }
}