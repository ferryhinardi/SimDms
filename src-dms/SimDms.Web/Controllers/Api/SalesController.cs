using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Breeze.WebApi;
using Newtonsoft.Json.Linq;
using Breeze.WebApi.EF;
using SimDms.Sales.Controllers;
using SimDms.Sales;
using SimDms.Sales.Models;
using SimDms.Common;
using SimDms.Common.Models;
using WsTrfOut = SimDms.Common.TransOutService.cs;

namespace SimDms.Web.Controllers.Api
{
    [BreezeController]
    public class SalesController : ApiController
    {
        readonly EFContextProvider<DataContext> ctx = new EFContextProvider<DataContext>();
        readonly EFContextProvider<MDContext> ctxMD = new EFContextProvider<MDContext>();

        [HttpGet]
        public String Metadata()
        {
            return ctx.Metadata();
        }

        [HttpGet]
        public OrganizationDtl isBranch()
        {
            var user = ctx.Context.SysUsers.Find(User.Identity.Name);
            var myPro = ctx.Context.OrganizationDtls.Find(user.CompanyCode, user.BranchCode);
            return myPro;
        }

        [HttpGet]
        public CoProfileSales fiscalMonth()
        {
            var user = ctx.Context.SysUsers.Find(User.Identity.Name);
            var myPro = ctx.Context.CoProfileSaleses.Find(user.CompanyCode, user.BranchCode);
            return myPro;
        }

        [HttpGet]
        public SysUserProfitCenter ProfitCenter()
        {
            var myPro = ctx.Context.SysUserProfitCenters.Find(User.Identity.Name);
            return myPro;
        }

        [HttpGet]
        public SysUser CurrentUser()
        {
            var user = ctx.Context.SysUsers.Find(User.Identity.Name);
            user.CoProfile = ctx.Context.CoProfiles.Find(user.CompanyCode, user.BranchCode);
            return user;
        }

        [HttpGet]
        protected string DealerCode()
        {
            var Uid = CurrentUser();
            var result = ctx.Context.CompanyMappings.FirstOrDefault(x => x.CompanyCode == Uid.CompanyCode && x.BranchCode == Uid.BranchCode);
            if (result != null)
            {
                if (result.CompanyMD == Uid.CompanyCode && result.BranchMD == Uid.BranchCode) { return "MD"; }
                else { return "SD"; }
            }
            else return "MD";
        }

        protected SysUser user
        {
            get
            {
                return CurrentUser();
            }
        }

        protected string CompanyMD
        {
            get
            {
                var Uid = CurrentUser();
                var rd = ctx.Context.CompanyMappings.FirstOrDefault(x => x.CompanyCode == Uid.CompanyCode && x.BranchCode == Uid.BranchCode);
                if (rd == null)
                {
                    return Uid.CompanyCode;
                }
                return rd.CompanyMD;
            }
        }

        protected string BranchMD
        {
            get
            {
                var Uid = CurrentUser();
                var rd = ctx.Context.CompanyMappings.FirstOrDefault(x => x.CompanyCode == Uid.CompanyCode && x.BranchCode == Uid.BranchCode);
                if (rd == null)
                {
                    return Uid.BranchCode;
                }
                return rd.BranchMD;

            }
        }

        protected string UnitBranchMD
        {
            get
            {
                var Uid = CurrentUser();
                var rd = ctx.Context.CompanyMappings.FirstOrDefault(x => x.CompanyCode == Uid.CompanyCode && x.BranchCode == Uid.BranchCode);
                if (rd == null)
                {
                    return Uid.BranchCode;
                }
                return rd.UnitBranchMD;
            }
        }

        protected string WarehouseMD
        {
            get
            {
                var Uid = CurrentUser();
                var rd = ctx.Context.CompanyMappings.FirstOrDefault(x => x.CompanyCode == Uid.CompanyCode && x.BranchCode == Uid.BranchCode);
                if (rd == null)
                {
                    return "";
                }
                return rd.WarehouseMD;
            }
        }

        [HttpGet]
        public MyUserInfo CurrentUserInfo()
        {
            var u = ctx.Context.SysUsers.Find(User.Identity.Name);
            var g = ctx.Context.CoProfiles.Find(u.CompanyCode, u.BranchCode);
            u.CoProfile = ctx.Context.CoProfiles.Find(u.CompanyCode, u.BranchCode);
            var f = ctx.Context.SysUserProfitCenters.Find(User.Identity.Name);

            var IsAdmin = ctx.Context.Database.SqlQuery<bool>(string.Format(@"select top 1 isnull(b.IsAdmin, 0) from sysusergroup a 
                    left join SysGroup b on b.GroupId = a.GroupId where 1 = 1 and a.UserId = '{0}' and b.GroupId = a.GroupId",
                    CurrentUser().UserId)).SingleOrDefault();

            string profitCenter = "100";
            var info = new MyUserInfo
            {
                UserId = u.UserId,
                FullName = u.FullName,
                CompanyCode = u.CompanyCode,
                BranchCode = u.BranchCode,
                CompanyGovName = g.CompanyGovName, // company // dealer
                CompanyName = g.CompanyName, //branch/ outlet
                TypeOfGoods = u.TypeOfGoods,
                ProductType = g.ProductType,
                IsActive = u.IsActive,
                RequiredChange = u.RequiredChange,
                ProfitCenter = IsAdmin ? profitCenter : f.ProfitCenter,
                SimDmsVersion = GetType().Assembly.GetName().Version.ToString()
            };

            return info;
        }


        [HttpGet]
        public IQueryable<MstRefferenceView> RefferenceBrowse()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.MstRefferences
                            where p.CompanyCode == Uid.CompanyCode
                            select new MstRefferenceView()
                            {
                                RefferenceType = p.RefferenceType,
                                RefferenceCode = p.RefferenceCode,
                                RefferenceDesc1 = p.RefferenceDesc1,
                                RefferenceDesc2 = p.RefferenceDesc2,
                                Remark = p.Remark,
                                Status = p.Status
                            };

            //var queryable = ctx.Context.Database.SqlQuery<MstRefferenceView>("SELECT RefferenceType, RefferenceCode, RefferenceDesc1, RefferenceDesc2, Remark, Cast(Status as bit) as Status FROM omMstRefference WHERE CompanyCode = '" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<RefferenceTypeView> RefferenceTypeLookup()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.RefferenceTypeView;
            //var queryable = ctx.Context.Database.SqlQuery<RefferenceTypeLookup>("sp_RefferenceTypeLookup '" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<MtsModelView> ModulBrowse()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.MstModels
                            where p.CompanyCode == Uid.CompanyCode
                            select new MtsModelView()
                            {
                                SalesModelCode = p.SalesModelCode,
                                SalesModelDesc = p.SalesModelDesc,
                                FakturPolisiDesc = p.FakturPolisiDesc,
                                EngineCode = p.EngineCode,
                                PpnBmCodeBuy = p.PpnBmCodeBuy,
                                PpnBmPctBuy = p.PpnBmPctBuy,
                                PpnBmCodeSell = p.PpnBmCodeSell,
                                PpnBmPctSell = p.PpnBmPctSell,
                                PpnBmCodePrincipal = p.PpnBmCodePrincipal,
                                PpnBmPctPrincipal = p.PpnBmPctPrincipal,
                                Remark = p.Remark,
                                BasicModel = p.BasicModel,
                                TechnicalModelCode = p.TechnicalModelCode,
                                ProductType = p.ProductType,
                                TransmissionType = p.TransmissionType,
                                IsChassis = p.IsChassis,
                                IsCbu = p.IsCbu,
                                SMCModelCode = p.SMCModelCode,
                                GroupCode = p.GroupCode,
                                TypeCode = p.TypeCode,
                                CylinderCapacity = p.CylinderCapacity,
                                fuel = p.fuel,
                                ModelPrincipal = p.ModelPrincipal,
                                Specification = p.Specification,
                                ModelLine = p.ModelLine,
                                Status = p.Status
                            };
            //var queryable = ctx.Context.Database.SqlQuery<MtsModelView>("SELECT * FROM omMstModel WHERE CompanyCode = '" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<omMstModelColourBrowse> MstModelColourBrowse()
        {
            var Uid = CurrentUser();
            var queryable = (from p in ctx.Context.MstModels
                             join q in ctx.Context.MstModelColours
                                 on p.SalesModelCode equals q.SalesModelCode
                             where p.CompanyCode == Uid.CompanyCode
                             select new omMstModelColourBrowse()
                            {
                                SalesModelCode = p.SalesModelCode,
                                SalesModelDesc = p.SalesModelDesc,
                            }).Distinct();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<MstModelYearBrowse> MstModelYearBrowse()
        {
            var Uid = CurrentUser();
            var queryable = (from p in ctx.Context.MstModels
                             join q in ctx.Context.MstModelYear
                                 on p.SalesModelCode equals q.SalesModelCode
                             where p.CompanyCode == Uid.CompanyCode
                             select new MstModelYearBrowse()
                             {
                                 SalesModelCode = p.SalesModelCode,
                                 SalesModelDesc = p.SalesModelDesc,
                             }).Distinct();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<GnMstTaxView> TaxCodeLookup()
        {

            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.Taxs
                            where p.CompanyCode == Uid.CompanyCode
                            select new GnMstTaxView()
                            {
                                TaxCode = p.TaxCode,
                                TaxPct = p.TaxPct
                            };
            //var queryable = ctx.Context.Database.SqlQuery<GnMstTaxView>("select TaxCode, TaxPct  from gnMstTax  where CompanyCode ='" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<AccountNoLookup> AccountNoLookup()
        {

            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.GnMstAccounts
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            select new AccountNoLookup()
                            {
                                AccountNo = p.AccountNo,
                                Description = p.Description
                            };
            //var queryable = ctx.Context.Database.SqlQuery<AccountNoLookup>("select AccountNo, Description  from GnMstAccount  where CompanyCode ='" + Uid.CompanyCode + "' and BranchCode ='" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<OthersInventoryBrowse> OthersInventoryBrowse()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.MstOthersInventory
                            join q in ctx.Context.GnMstAccounts
                            on new { AccountNo = p.OthersNonInventoryAccNo } equals new { q.AccountNo }
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            select new OthersInventoryBrowse()
                            {
                                OthersNonInventory = p.OthersNonInventory,
                                OthersNonInventoryDesc = p.OthersNonInventoryDesc,
                                OthersNonInventoryAccNo = p.OthersNonInventoryAccNo,
                                Description = q.Description,
                                Remark = p.Remark,
                                IsActive = p.IsActive
                            };
            //var queryable = ctx.Context.Database.SqlQuery<OthersInventoryBrowse>("sp_MstOthersInventoryBrowse '" + Uid.CompanyCode + "','" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<DealerCodeLookup> DealerCodeLookup()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<DealerCodeLookup>("SELECT TOP 1 CompanyCode, CompanyName FROM GnMstOrganizationHdr WHERE CompanyCode ='" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<OutLetLookup> OutLetLookup()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<OutLetLookup>("SELECT TOP 1 BranchCode, BranchName FROM GnMstOrganizationDtl WHERE CompanyCode ='" + Uid.CompanyCode + "' and BranchCode ='" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SalesmanIDLookup> SalesmanIDLookup()
        {
            var Uid = CurrentUser();
            var Qry = from p in ctx.Context.HrEmployees
                      where p.CompanyCode == Uid.CompanyCode && p.Department == "SALES"
                      select new SalesmanIDLookup()
                      {
                          EmployeeID = p.EmployeeID,
                          EmployeeName = p.EmployeeName,
                          LookUpValueName = p.Department
                      };
            if (Qry.Count() == 0)
            {
                Qry = from p in ctx.Context.Employees
                      join q in ctx.Context.LookUpDtls
                       on p.TitleCode equals q.ParaValue
                      where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode && q.CodeID == "TITL"
                      select new SalesmanIDLookup()
                      {
                          EmployeeID = p.EmployeeID,
                          EmployeeName = p.EmployeeName,
                          LookUpValueName = q.LookUpValueName
                      };
            }
            return (Qry);
        }


        [HttpGet]
        public IQueryable<tKendaraan> LkpTipeKendaraan(string reff)
        {
            var Uid = CurrentUser();
            var qry = String.Format(@"SELECT a.RefferenceCode AS typeID, a.RefferenceDesc1 AS typeName
                                FROM dbo.omMstRefference a
                                WHERE a.CompanyCode = '{0}'
                                AND a.RefferenceType = '{1}'
                                AND a.Status != '0'", Uid.CompanyCode, reff);
            var list = ctx.Context.Database.SqlQuery<tKendaraan>(qry).AsQueryable();
            return (list);
        }

        [HttpGet]
        public IQueryable<MarketModelLookup> MarketModelLookup()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.LookUpDtls
                            where p.CompanyCode == Uid.CompanyCode && p.CodeID == "MKTCD"
                            select new MarketModelLookup()
                            {
                                LookUpValue = p.LookUpValue,
                                LookUpValueName = p.LookUpValueName
                            };
            //var queryable = ctx.Context.Database.SqlQuery<MarketModelLookup>("SELECT LookUpValue, LookUpValueName FROM gnMstLookUpDtl WHERE CodeID='MKTCD' AND CompanyCode ='" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SalesTargetBrowse> SalesTargetBrowse()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.omMstSalesTarget
                            join q in ctx.Context.Employees
                                on p.SalesmanID equals q.EmployeeID
                            where p.DealerCode == Uid.CompanyCode && p.OutletCode == Uid.BranchCode
                            select new SalesTargetBrowse()
                            {
                                DealerCode = p.DealerCode,
                                OutletCode = p.OutletCode,
                                Year = p.Year,
                                Month = p.Month,
                                SalesmanID = p.SalesmanID,
                                EmployeeName = q.EmployeeName,
                                MarketModel = p.MarketModel,
                                TargetUnit = p.TargetUnit,
                                isActive = p.isActive,
                            };
            //var queryable = ctx.Context.Database.SqlQuery<SalesTargetBrowse>("SELECT * FROM omMstSalesTarget WHERE DealerCode ='" + Uid.CompanyCode + "' and OutletCode ='" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<CompanyAccountBrowse> CompanyAccountBrowse()
        {
            var Uid = CurrentUser();
            //var queryable = from p in ctx.Context.MstCompanyAccount
            //                join q in ctx.Context.GnMstAccounts
            //                    on p.InterCompanyAccNoTo equals q.AccountNo
            //                where p.CompanyCode == Uid.CompanyCode && q.BranchCode == Uid.BranchCode
            //                select new CompanyAccountBrowse()
            //                {
            //                    CompanyCode = p.CompanyCode,
            //                    CompanyCodeTo = p.CompanyCodeTo,
            //                    CompanyCodeToDesc = p.CompanyCodeToDesc,
            //                    BranchCodeTo = p.BranchCodeTo,
            //                    BranchCodeToDesc = p.BranchCodeToDesc,
            //                    WarehouseCodeTo = p.WarehouseCodeTo,
            //                    WarehouseCodeToDesc = p.WarehouseCodeToDesc,
            //                    InterCompanyAccNoTo = p.InterCompanyAccNoTo,
            //                    InterCompanyAccNoToDesc = q.Description,
            //                    UrlAddress = p.UrlAddress,
            //                    isActive = p.isActive,
            //                    Status = p.isActive == false ? "Tidak Aktif" : "Aktif"
            //                };
            var queryable = ctx.Context.Database.SqlQuery<CompanyAccountBrowse>(@"select *, case IsActive when '1' then 'Aktif' else 'Tdk Aktif' end Status
                                                                                    from omMstCompanyAccount
                                                                                     where companycode={0} and companycodeto=case '' when '' then companycodeto else {0} end 
                                                                                     and isActive = 1",Uid.CompanyCode).ToList();
            return (queryable.AsQueryable());
        }

        [HttpGet]
        public IQueryable<MtsModelView> SalesModelCodeLookup()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.OmMstModels
                            where p.CompanyCode == Uid.CompanyCode && p.Status == "1"
                            select new MtsModelView()
                            {
                                SalesModelCode = p.SalesModelCode,
                                SalesModelDesc = p.SalesModelDesc
                            };
            //var queryable = ctx.Context.Database.SqlQuery<MtsModelView>("SELECT SalesModelCode, SalesModelDesc FROM omMstModel WHERE CompanyCode = '" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<ModelAccountBrowse> ModelAccountBrowse()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<ModelAccountBrowse>("sp_ModelAccountBrowse '" + Uid.CompanyCode + "','" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SupplierView> SupplierCodeLookup()
        {
            var Uid = CurrentUser();
            var queryable = (from p in ctx.Context.Supplier
                             join q in ctx.Context.SupplierProfitCenter
                                  on new { p.CompanyCode, p.SupplierCode }
                                  equals new { q.CompanyCode, q.SupplierCode }
                             where p.CompanyCode == Uid.CompanyCode && q.BranchCode == Uid.BranchCode
                             && q.isBlackList == false && p.Status == "1"
                             select new SupplierView()
                             {
                                 SupplierCode = p.SupplierCode,
                                 SupplierName = p.SupplierName
                             }).Distinct();
            //var queryable = ctx.Context.Database.SqlQuery<SupplierView>("SELECT SupplierCode, SupplierName FROM gnMstSupplier WHERE CompanyCode = '" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<CityCodeLookup> CityCodeLookup()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.LookUpDtls
                            where p.CompanyCode == Uid.CompanyCode && p.CodeID == "CITY"
                            select new CityCodeLookup()
                            {
                                LookUpValue = p.LookUpValue,
                                LookUpValueName = p.LookUpValueName
                            };
            //var queryable = ctx.Context.Database.SqlQuery<CityCodeLookup>("SELECT LookUpValue, LookUpValueName FROM gnMstLookUpDtl WHERE CodeID='CITY' AND CompanyCode ='" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SalesModelYearLookup> SalesModelYearLookup(string SalesModelCode)
        {
            var parameters = SalesModelCode.Split('?');
            var ModelCode = parameters[0];
            var Uid = CurrentUser();
            var g = ctx.Context.CoProfiles.Find(Uid.CompanyCode, Uid.BranchCode);
            var queryable = ctx.Context.Database.SqlQuery<SalesModelYearLookup>("SELECT SalesModelYear, SalesModelDesc FROM omMstModelYear WHERE CompanyCode = '" + Uid.CompanyCode + "' and SalesModelCode like '" + ModelCode + "%'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<BBNKIRBrowse> BBNKIRBrowse()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.MstBBNKIR
                            join q in ctx.Context.Supplier
                                on p.SupplierCode equals q.SupplierCode
                            join r in ctx.Context.LookUpDtls
                                on p.CityCode equals r.LookUpValue
                            join s in ctx.Context.OmMstModels
                                on p.SalesModelCode equals s.SalesModelCode
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            select new BBNKIRBrowse()
                            {
                                SupplierCode = p.SupplierCode,
                                SupplierName = q.SupplierName,
                                CityCode = p.CityCode,
                                CityName = r.LookUpValueName,
                                SalesModelCode = p.SalesModelCode,
                                SalesModelDesc = s.SalesModelDesc,
                                SalesModelYear = p.SalesModelYear,
                                BBN = p.BBN,
                                KIR = p.KIR,
                                Remark = p.Remark,
                                Status = p.Status
                            };
            //var queryable = ctx.Context.Database.SqlQuery<BBNKIRBrowse>("sp_BBNKIRBrowse '" + Uid.CompanyCode + "','" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<GroupPriceCodeLookup> GroupPriceCodeLookup()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.MstRefferences
                            where p.CompanyCode == Uid.CompanyCode && p.RefferenceType == "GRPR"
                            select new GroupPriceCodeLookup()
                            {
                                RefferenceCode = p.RefferenceCode,
                                RefferenceDesc1 = p.RefferenceDesc1
                            };
            //var queryable = ctx.Context.Database.SqlQuery<GroupPriceCodeLookup>("SELECT RefferenceCode, RefferenceDesc1 FROM OmMstRefference where RefferenceType='GRPR' AND CompanyCode ='" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<KaroseriBrowse> KaroseriBrowse()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.MstKaroseri
                            join q in ctx.Context.Supplier
                                on p.SupplierCode equals q.SupplierCode
                            join r in ctx.Context.OmMstModels
                                on p.SalesModelCode equals r.SalesModelCode
                            join s in ctx.Context.OmMstModels
                                on p.SalesModelCodeNew equals s.SalesModelCode
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            select new KaroseriBrowse()
                            {
                                SupplierCode = p.SupplierCode,
                                SupplierName = q.SupplierName,
                                SalesModelCode = p.SalesModelCode,
                                SalesModelDesc = r.SalesModelDesc,
                                SalesModelCodeNew = p.SalesModelCodeNew,
                                SalesModelDescNew = s.SalesModelDesc,
                                DPPMaterial = p.DPPMaterial,
                                DPPFee = p.DPPFee,
                                DPPOthers = p.DPPOthers,
                                PPn = p.PPn,
                                Total = p.Total,
                                Remark = p.Remark,
                                Status = p.Status
                            };
            //var queryable = ctx.Context.Database.SqlQuery<KaroseriBrowse>("sp_KaroseriBrowse '" + Uid.CompanyCode + "','" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<PerlengkapanBrowse> PerlengkapanBrowse()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.MstPerlengkapan
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            select new PerlengkapanBrowse()
                            {
                                PerlengkapanCode = p.PerlengkapanCode,
                                PerlengkapanName = p.PerlengkapanName,
                                Remark = p.Remark,
                                Status = p.Status,
                                Status2 = p.Status == "0" ? "Not Active" : "Active"
                            };
            //var queryable = ctx.Context.Database.SqlQuery<PerlengkapanBrowse>("SELECT CompanyCode, BranchCode, PerlengkapanCode, PerlengkapanName, Remark, Cast(Status as bit) as [Status], CASE WHEN  Status= 0 THEN 'Not Active' ELSE 'Active' END as [Status2]  FROM omMstPerlengkapan WHERE CompanyCode ='" + Uid.CompanyCode + "' and BranchCode ='" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<ColourCodeLookup> ColourCodeLookup()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.MstRefferences
                            where p.CompanyCode == Uid.CompanyCode && p.RefferenceType == "COLO"
                            select new ColourCodeLookup()
                            {
                                RefferenceCode = p.RefferenceCode,
                                RefferenceDesc1 = p.RefferenceDesc1
                            };
            //var queryable = ctx.Context.Database.SqlQuery<ColourCodeLookup>("SELECT RefferenceCode, RefferenceDesc1 FROM OmMstRefference where RefferenceType='COLO' AND CompanyCode ='" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<PerlengkapanCodeLookup> PerlengkapanCodeLookup()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.MstPerlengkapan
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            select new PerlengkapanCodeLookup()
                            {
                                PerlengkapanCode = p.PerlengkapanCode,
                                PerlengkapanName = p.PerlengkapanName
                            };
            //var queryable = ctx.Context.Database.SqlQuery<PerlengkapanCodeLookup>("SELECT PerlengkapanCode, PerlengkapanName FROM omMstPerlengkapan where  CompanyCode ='" + Uid.CompanyCode + "' and BranchCode ='" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<omMstModelPerlengkapanBrowse> MstModelPerlengkapanBrowse()
        {
            var Uid = CurrentUser();
            var queryable = (from p in ctx.Context.MstModels
                             join q in ctx.Context.MstModelPerlengkapan
                                 on p.SalesModelCode equals q.SalesModelCode
                             where p.CompanyCode == Uid.CompanyCode
                             select new omMstModelPerlengkapanBrowse()
                             {
                                 SalesModelCode = p.SalesModelCode,
                                 SalesModelDesc = p.SalesModelDesc,
                             }).Distinct();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<MstPriceListBeliBrowse> PriceListBeliBrowse()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.MstPriceListBeli
                            join q in ctx.Context.Supplier
                                on p.SupplierCode equals q.SupplierCode
                            join r in ctx.Context.OmMstModels
                                on p.SalesModelCode equals r.SalesModelCode
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            select new MstPriceListBeliBrowse()
                           {
                               SupplierCode = p.SupplierCode,
                               SupplierName = q.SupplierName,
                               SalesModelCode = p.SalesModelCode,
                               SalesModelDesc = r.SalesModelDesc,
                               SalesModelYear = p.SalesModelYear,
                               PPnBMPaid = p.PPnBMPaid,
                               DPP = p.DPP,
                               PPn = p.PPn,
                               PPnBM = p.PPnBM,
                               Total = p.Total,
                               Remark = p.Remark,
                               Status = p.Status == "1" ? true : false
                           };

            //SELECT a.CompanyCode, a.BranchCode, a.SupplierCode, b.SupplierName, a.SalesModelCode, c.SalesModelDesc, a.SalesModelYear
            //,a.PPnBMPaid, a.DPP, a.PPn, a.PPnBM, a.Total, a.Remark, cast(a.Status AS bit) as Status
            //FROM omMstPricelistBuy a
            //LEFT JOIN gnMstSupplier b
            //    ON a.SupplierCode = b.SupplierCode
            //LEFT JOIN omMstModel c
            //    ON a.SalesModelCode = c.SalesModelCode

            //var data = from a in queryable
            //           
            //var queryable = ctx.Context.Database.SqlQuery<MstPriceListBeliBrowse>("sp_MstPriceListBeliBrowse '" + Uid.CompanyCode + "','" + Uid.BranchCode + "'").AsQueryable();
            //return (data.AsQueryable());
            return (queryable);
        }

        [HttpGet]
        public IQueryable<PriceListJualBrowse> PriceListJualBrowse()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.OmMstPricelistSells
                            join q in ctx.Context.MstRefferences
                                on p.GroupPriceCode equals q.RefferenceCode
                            join r in ctx.Context.OmMstModels
                                on p.SalesModelCode equals r.SalesModelCode
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode && q.RefferenceType == "GRPR"
                            select new PriceListJualBrowse()
                          {
                              GroupPriceCode = p.GroupPriceCode,
                              GroupPriceName = q.RefferenceDesc1,
                              SalesModelCode = p.SalesModelCode,
                              SalesModelDesc = r.SalesModelDesc,
                              SalesModelYear = p.SalesModelYear,
                              TotalMinStaff = p.TotalMinStaff,
                              DPP = p.DPP,
                              PPn = p.PPn,
                              PPnBM = p.PPnBM,
                              Total = p.Total,
                              Remark = p.Remark,
                              Status = p.Status == "1" ? true : false,
                              TaxCode = p.TaxCode,
                              TaxPct = p.TaxPct
                          };

            //var data = from a in queryable
            //               
            //var queryable = ctx.Context.Database.SqlQuery<PriceListJualBrowse>("sp_MstPriceListJualBrowse '" + Uid.CompanyCode + "','" + Uid.BranchCode + "'").AsQueryable();
            //return (data.AsQueryable());
            return (queryable.AsQueryable());
        }

        [HttpGet]
        public IQueryable<OmMstRefference> RefferenceCodeLookup(string RefferenceType)
        {
            var parameters = RefferenceType.Split('?');
            var ReffType = parameters[0];
            var Uid = CurrentUser();
            //var queryable = ctx.Context.Database.SqlQuery<OmMstRefference>("SELECT RefferenceCode, RefferenceDesc1 FROM omMstRefference WHERE CompanyCode = '" + Uid.CompanyCode + "' and RefferenceType = '" + ReffType + "'").AsQueryable();
            var queryable = from p in ctx.Context.MstRefferences
                            where p.CompanyCode == Uid.CompanyCode && p.RefferenceType == RefferenceType
                            select new OmMstRefference()
                            {
                                RefferenceType = p.RefferenceCode,
                                RefferenceDesc1 = p.RefferenceDesc1
                            };
            return (queryable);
        }


        /* Purchase Order*/

        [HttpGet]
        public IQueryable<POBrowse> POBrowse()
        {
            var Uid = CurrentUser();

            var data = from e in ctx.Context.OmTrPurchasePOs
                       join d in ctx.Context.Supplier
                       on e.SupplierCode
                       equals d.SupplierCode
                       where e.CompanyCode == Uid.CompanyCode &&
                       e.BranchCode == Uid.BranchCode
                       select new POBrowse
                       {
                           PONo = e.PONo,
                           PODate = e.PODate,
                           RefferenceNo = e.RefferenceNo,
                           RefferenceDate = e.RefferenceDate,
                           SupplierCode = e.SupplierCode,
                           SupplierName = d.SupplierName,
                           Status = e.Status == "0" ? "OPEN" : e.Status == "1" ? "PRINTED"
                               : e.Status == "2" ? "APPROVED"
                               : e.Status == "3" ? "CANCELED"
                               : e.Status == "9" ? "FINISHED" : "",
                           Stat = e.Status
                       };
            return data;
        }

        [HttpGet]
        public IQueryable<ReffNoBrowse> ReffNoBrowse()
        {
            var query = string.Format(@"
                         SELECT a.BatchNo, b.SKPNo as RefferenceNo, b.SKPDate as RefferenceDate, 
                                a.DealerCode, c.SupplierCode,c.SupplierName
                          FROM omUtlSPORDHdr a
                        	inner join omUtlSPORDDtl1 b on a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode
                        		AND a.BatchNo = b.BatchNo
                            join gnMstSupplier c on c.StandardCode = a.DealerCode
                         WHERE a.CompanyCode= {0}
                               and a.BranchCode= {1}
                               AND b.Status = '0'
                        AND not exists(
                        	SELECT 1 FROM OmTrPurchasePO WHERE CompanyCode = a.CompanyCode
                        	AND BranchCode = a.BranchCode and RefferenceNo=b.SKPNo AND status != '3')", CurrentUser().CompanyCode, CurrentUser().BranchCode);
            var data = ctx.Context.Database.SqlQuery<ReffNoBrowse>(query).AsQueryable();
            return data;
        }

        [HttpGet]
        public IQueryable<SalesModelCodeBrowse> SalesModelCodeBrowse(string supplierCode)
        {
            var uid = CurrentUser();
            //            var DB = ctx.Context.CompanyMappings.Where(a => a.CompanyCode == Uid.CompanyCode && a.BranchCode == Uid.BranchCode).FirstOrDefault().DbMD;
            //            var query = string.Format(@"uspfn_SalesModelCode4PO '{0}','{1}','{2}','{3}'
            //                ",DB, Uid.CompanyCode, Uid.BranchCode, supplierCode);

            //            var queryable = ctx.Context.Database.SqlQuery<SalesModelCodeBrowse>(query).AsQueryable();
            //            return queryable;

            var data = from d in ctx.Context.MstModels
                        //from d in ctx.Context.OmMstPricelistBuys
                        //join m in ctx.Context.MstModels
                        //on new { d.CompanyCode, d.SalesModelCode }
                        //equals new { m.CompanyCode, m.SalesModelCode }
                       //join n in ctxMD.Context.MstModels
                       //on d.SalesModelCode equals n.SalesModelCode
                       //on new { d.CompanyCode, d.SalesModelCode }
                       //equals new { m.CompanyCode, m.SalesModelCode }
                       where d.CompanyCode == uid.CompanyCode
                       //&& d.BranchCode == uid.BranchCode
                       //&& d.SupplierCode == supplierCode
                       && d.Status != "0"
                       select new SalesModelCodeBrowse
                       {
                           SalesModelCode = d.SalesModelCode,
                           SalesModelDesc = d.SalesModelDesc
                       };

            return data;
        }

        [HttpGet]
        public IQueryable<OmMstPricelistBuy> SalesModelYearBrowse(string supplierCode, string salesModelCode)
        {
            var uid = CurrentUser();
            var data = ctx.Context.OmMstPricelistBuys.Where(a => a.CompanyCode == uid.CompanyCode && a.BranchCode == uid.BranchCode && a.SupplierCode == supplierCode && a.SalesModelCode == salesModelCode && a.Status == "1");
            return data;
        }

        [HttpGet]
        public IQueryable<ColourCodeBrowse> ColourCodeBrowse(string salesModelCode)
        {
            var uid = CurrentUser();
            //var data = ctx.Context.OmMstModelColours.Where(a => a.CompanyCode == uid.CompanyCode && a.SalesModelCode == salesModelCode
            //    && ctx.Context.MstRefferences.Where(m => m.RefferenceCode == a.ColourCode && m.CompanyCode == a.CompanyCode && m.RefferenceType == "COLO"))
            //    .Select(a => new ColourCodeBrowse {
            //    ColourCode = a.ColourCode,
            //    ColourDesc = 
            //    });

            //            var query = String.Format(@"SELECT a.ColourCode, (SELECT b.RefferenceDesc1
            //                        FROM omMstRefference b
            //                        WHERE b.RefferenceCode = a.ColourCode
            //                        AND b.CompanyCode = a.CompanyCode AND b.RefferenceType = 'COLO')  AS ColourDesc, a.Remark
            //                        FROM omMstModelColour a
            //                        WHERE a.CompanyCode = '{0}' AND a.SalesModelCode = '{1}'", uid.CompanyCode, salesModelCode);

            //            var queryable = ctx.Context.Database.SqlQuery<ColourCodeBrowse>(query).AsQueryable();
            var queryable = from a in ctx.Context.MstModelColours
                            join b in ctx.Context.MstRefferences
                            on new { a.CompanyCode, a.ColourCode } equals new { b.CompanyCode, ColourCode = b.RefferenceCode }
                            where b.RefferenceType == "COLO" && a.CompanyCode == uid.CompanyCode && a.SalesModelCode == salesModelCode
                            select new ColourCodeBrowse()
                            {
                                ColourCode = a.ColourCode,
                                ColourDesc = b.RefferenceDesc1,
                                Remark = a.Remark
                            };
            return (queryable);
        }

        /*------------------------*/


        /* Stock Opname*/

        [HttpGet]
        public IQueryable<OmMstRefference> RefferenceStockTaking(string RefferenceType)
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<OmMstRefference>("SELECT RefferenceCode, RefferenceDesc1 FROM omMstRefference WHERE CompanyCode = '" + Uid.CompanyCode + "' and RefferenceType = '" + RefferenceType + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SalesModelYearLookup> YearStockTaking()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<SalesModelYearLookup>("SELECT DISTINCT SalesModelYear FROM OmMstModelYear WHERE CompanyCode = '" + Uid.CompanyCode + "' and Status = '1'").AsQueryable();
            return (queryable);
        }


        [HttpGet]
        public IQueryable<EntryInvTag> STHdrNoBrowse(string pil)
        {
            var Uid = CurrentUser();
            if (pil == "entry")
            {
                var queryable = ctx.Context.Database.SqlQuery<EntryInvTag>("SELECT STHdrNo, STDate, (CASE ISNULL(Status, '0') WHEN '0' THEN 'BELUM ENTRY' WHEN '1' THEN 'ENTRY' WHEN '2' THEN 'POSTED' END) AS Status FROM OmTrStockTakingHdr" +
            " WHERE CompanyCode = '" + Uid.CompanyCode + "' and BranchCode = '" + Uid.BranchCode + "' and Status <> '2'").AsQueryable();
                return (queryable);
            }
            else
            {
                var queryable = ctx.Context.Database.SqlQuery<EntryInvTag>("SELECT STHdrNo, STDate FROM OmTrStockTakingHdr" +
            " WHERE CompanyCode = '" + Uid.CompanyCode + "' and BranchCode = '" + Uid.BranchCode + "' and Status = '1'").AsQueryable();
                return (queryable);
            }


            // return (queryable);
        }


        [HttpGet]
        public IQueryable<BPUView> BPULookup()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.omTrPurchaseBPU
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            select new BPUView()
                            {
                                BPUNo = p.BPUNo,
                                BPUDate = p.BPUDate,
                                RefferenceDONo = p.RefferenceDONo,
                                RefferenceDODate = p.RefferenceDODate,
                                RefferenceSJNo = p.RefferenceSJNo,
                                RefferenceSJDate = p.RefferenceSJDate,
                                PONo = p.PONo
                            }; return (queryable);
        }

        [HttpGet]
        public IQueryable<ReturnView> ReturnLookup()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.omTrPurchaseReturn
                            join q in ctx.Context.omTrPurchaseHPP
                                on p.HPPNo equals q.HPPNo
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            select new ReturnView()
                            {
                                ReturnNo = p.ReturnNo,
                                ReturnDate = p.ReturnDate,
                                HPPNo = p.HPPNo,
                                HPPDate = q.HPPDate
                            }; return (queryable);
        }


        [HttpGet]
        public IQueryable<PerlengkapanInView> PerlengkapanInLookup()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.omTrPurchasePerlengkapanIn
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            select new PerlengkapanInView()
                            {
                                PerlengkapanNo = p.PerlengkapanNo,
                                PerlengkapanDate = p.PerlengkapanDate,
                                RefferenceNo = p.RefferenceNo,
                                RefferenceDate = p.RefferenceDate
                            }; return (queryable);
        }

        [HttpGet]
        public IQueryable<PerlengkapanAdjustmentView> PerlengkapanAdjustmentLookup()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.omTrPurchasePerlengkapanAdjustment
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            select new PerlengkapanAdjustmentView()
                            {
                                AdjustmentNo = p.AdjustmentNo,
                                AdjustmentDate = p.AdjustmentDate,
                                RefferenceNo = p.RefferenceNo,
                                RefferenceDate = p.RefferenceDate
                            }; return (queryable);
        }

        [HttpGet]
        public IQueryable<gnMstPeriode> RincianVehHilang(string year)
        {
            var Uid = CurrentUser();
            var sql = "SELECT  CONVERT(varchar, FiscalYear) + RIGHT('00' + CONVERT(varchar, PeriodeNum), 2) AS Periode," +
            " PeriodeNum, FiscalMonth, PeriodeName, FromDate, EndDate, CASE ISNULL(StatusSales , 0 ) WHEN 0 THEN 'Future Entry' WHEN 1 THEN 'Open' WHEN 2 THEN 'Close' END AS Status " +
            " FROM gnMstPeriode WHERE FiscalStatus = 1 AND CompanyCode = '" + Uid.CompanyCode + "' AND BranchCode = '" + Uid.BranchCode + "' AND FiscalYear = " + year + " AND StatusSales = 1";
            var queryable = ctx.Context.Database.SqlQuery<gnMstPeriode>(sql).AsQueryable();
            //var queryable = from a in ctx.Context.MstPeriod
            //                where a.FiscalStatus == true
            //                && a.CompanyCode == Uid.CompanyCode
            //                && a.BranchCode == Uid.BranchCode
            //                && a.FiscalYear.ToString() == year.ToString()
            //                && a.StatusSales == 1
            //                select new gnMstPeriode()
            //                {
            //                    PeriodeNum = a.FiscalYear.ToString(),
            //                    FiscalMonth = a.FiscalMonth,
            //                    PeriodeName = a.PeriodeName,
            //                    FromDate = a.FromDate,
            //                    EndDate = a.EndDate,

            //                };
            return (queryable);
        }

        [HttpGet]
        public IQueryable<KaroseriDetailModel> KaroseriLookup()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.omTrPurchaseKaroseri
                            join q in ctx.Context.Supplier
                                on p.SupplierCode equals q.SupplierCode
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            select new KaroseriDetailModel()
                            {
                                KaroseriSPKNo = p.KaroseriSPKNo,
                                KaroseriSPKDate = p.KaroseriSPKDate,
                                SalesModelCodeOld = p.SalesModelCodeOld,
                                SalesModelYear = p.SalesModelYear,
                                SupplierCode = p.SupplierCode,
                                SupplierName = q.SupplierName,
                                Remark = p.Remark
                            }; return (queryable);
        }

        [HttpGet]
        public IQueryable<SOView> SOLookup()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.OmTRSalesSOs
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            select new SOView()
                            {
                                SONo = p.SONo,
                                SODate = p.SODate
                            }; return (queryable);
        }

        [HttpGet]
        public IQueryable<OmTrPurchaseBPULookupView> BPUBrowse()
        {
            var Uid = CurrentUser();
            var records = ctx.Context.OmTrPurchaseBPULookupView.Where(p => p.CompanyCode == Uid.CompanyCode
                && p.BranchCode == Uid.BranchCode)
                .OrderByDescending(p => p.BPUNo);

            return records;
        }

        [HttpGet]
        public IQueryable<OmTrSalesDraftSOLookupView> SPUBrowse()
        {
            var Uid = CurrentUser();
            var records = ctx.Context.OmTrSalesDraftSOLookupViews.Where(p => p.CompanyCode == Uid.CompanyCode
                && p.BranchCode == Uid.BranchCode)
                .OrderByDescending(p => p.DraftSONo);

            return records;
        }

        [HttpGet]
        public IQueryable<SOView> SOSalesmanLookup()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.Employees
                            where p.CompanyCode == Uid.CompanyCode
                            select new SOView()
                            {
                                Salesman = p.EmployeeID,
                                SalesmanName = p.EmployeeName
                            }; return (queryable);
        }

        [HttpGet]
        public IQueryable<DOView> DOLookup()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.OmTrSalesDOs
                            join q in ctx.Context.OmTRSalesSOs
                                on p.SONo equals q.SONo
                            where p.CompanyCode == Uid.CompanyCode
                            select new DOView()
                            {
                                SONo = p.SONo,
                                SODate = q.SODate,
                                DONo = p.DONo,
                                DODate = p.DODate
                            }; return (queryable);
        }

        [HttpGet]
        public IQueryable<BPKView> BPKLookup()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.OmTrSalesBPKs
                            join q in ctx.Context.OmTrSalesDOs
                                on p.DONo equals q.DONo
                            join r in ctx.Context.OmTRSalesSOs
                                on p.SONo equals r.SONo
                            where p.CompanyCode == Uid.CompanyCode
                            select new BPKView()
                            {
                                SONo = p.SONo,
                                SODate = r.SODate,
                                DONo = p.DONo,
                                DODate = q.DODate,
                                BPKNo = p.BPKNo,
                                BPKDate = p.BPKDate
                            }; return (queryable);
        }

        [HttpGet]
        public IQueryable<BPUAttrView> BPUAttr()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.omTrPurchaseBPUAttributes
                            join p1 in ctx.Context.LookUpDtls
                            on p.WareHouseCode equals p1.LookUpValue
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == p.BranchCode && p1.CodeID == "MPWH"
                            select new BPUAttrView()
                            {
                                BPUNo = p.BPUNo,
                                DONo = p.DONo,
                                FakturPolisiDate = p.FakturPolisiDate,
                                FakturPolisiNo = p.FakturPolisiNo,
                                WareHouseCode = p.WareHouseCode,
                                Subsidi1 = p.Subsidi1,
                                Subsidi2 = p.Subsidi2,
                                Subsidi3 = p.Subsidi3,
                                Subsidi4 = p.Subsidi4,
                                Kompensasi = p.Kompensasi,
                                PotSKP = p.PotSKP,
                                HargaAccessories = p.HargaAccessories,
                                Status = p.Status,
                                WarehouseName = p1.LookUpValueName
                            }; return (queryable);
        }


        [HttpGet]
        public IQueryable<HPPView> HPPBrowse()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.omTrPurchaseHPP
                            join p1 in ctx.Context.Supplier
                            on p.SupplierCode equals p1.SupplierCode
                            join p2 in ctx.Context.OmTrPurchasePOs
                            on p.PONo equals p2.PONo
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            select new HPPView()
                            {
                                HPPNo = p.HPPNo,
                                HPPDate = p.HPPDate,
                                PONo = p.PONo,
                                SupplierCode = p.SupplierCode,
                                BillTo = p.BillTo,
                                RefferenceInvoiceNo = p.RefferenceInvoiceNo,
                                RefferenceInvoiceDate = p.RefferenceInvoiceDate,
                                RefferenceFakturPajakNo = p.RefferenceFakturPajakNo,
                                RefferenceFakturPajakDate = p.RefferenceFakturPajakDate,
                                DueDate = p.DueDate,
                                Remark = p.Remark,
                                Status = p.Status,
                                SupplierName = p1.SupplierName,
                                RefferenceNo = p2.RefferenceNo


                            }; return (queryable);
        }

        [HttpGet]
        public IQueryable<HPPView> POLookup()
        {
            var Uid = CurrentUser();
            //            var query = string.Format(@"
            //                SELECT DISTINCT  (a.PONo), a.SupplierCode, (SELECT c.SupplierName
            //                            FROM dbo.gnMstSupplier c
            //                            WHERE c.SupplierCode = a.SupplierCode) as SupplierName, a.BillTo, a.RefferenceNo
            //            FROM dbo.omTrPurchasePO a, dbo.omTrPurchaseBPUDetailModel b
            //            WHERE a.CompanyCode = b.CompanyCode
            //                AND a.BranchCode = b.BranchCode
            //                AND a.PONO = b.PONO
            //                AND b.QuantityHPP < b.QuantityBPU
            //                AND a.CompanyCode = '{0}'
            //                AND a.BranchCode = '{1}'", Uid.CompanyCode, Uid.BranchCode);
            //            var queryable = ctx.Context.Database.SqlQuery<HPPView>(query).AsQueryable();
            //var queryable = ctx.Context.Database.SqlQuery<spTrnIReservedHdrView>("select ReservedNo,ReservedDate,ReferenceNo,ReferenceDate,PartNo,TypeOfGoods,OprCode,Status from spTrnIReservedHdr where Status=0 and  CompanyCode='" + Uid.CompanyCode + "' and  BranchCode='" + Uid.BranchCode + "'").AsQueryable();
            var queryable = from p in
                                (from a in ctx.Context.OmTrPurchasePOs
                                 join b in ctx.Context.OmTrPurchaseBPUDetailModels
                                 on new { a.PONo, a.BranchCode } equals new { b.PONo, b.BranchCode }
                                 join c in ctx.Context.Supplier
                                 on a.SupplierCode equals c.SupplierCode
                                 where a.CompanyCode == Uid.CompanyCode
                                       && a.BranchCode == Uid.BranchCode
                                       && b.QuantityHPP < b.QuantityBPU
                                 group new { a, b, c } by a.PONo into View
                                 select View.FirstOrDefault())
                            select new HPPView()
                            {
                                PONo = p.a.PONo,
                                SupplierCode = p.a.SupplierCode,
                                SupplierName = p.c.SupplierName,
                                BillTo = p.a.BillTo,
                                RefferenceNo = p.a.RefferenceNo
                            };
            return (queryable);
        }

        [HttpGet]
        public IQueryable<ReffInvView> ReffInvLookup()
        {
            var Uid = CurrentUser();
            var qry = string.Format(@"
                SELECT distinct a.BatchNo, b.InvoiceNo, b.InvoiceDate, b.FakturPajakNo, b.
                       FakturPajakDate, c.PONo, c.SupplierCode, c.BillTo, b.DueDate,
                       (select d.supplierName from gnMstSupplier d where d.companyCode = c.companyCode
					   and d.supplierCode = c.supplierCode) as SupplierName, c.RefferenceNo, b.Remark
                  FROM omUtlSHPOKHdr a, omUtlSHPOKDtl1 b, omTrPurchasePO c
					   inner join omTrPurchaseBPU e on c.companyCode = e.companyCode
					   and c.branchCode = e.branchCode and c.PONo = e.PONo
                 WHERE a.CompanyCode = b.CompanyCode
                       AND a.BranchCode = b.BranchCode
                       AND a.BatchNo = b.BatchNo
                       AND a.CompanyCode = c.CompanyCode
                       AND a.BranchCode = c.BranchCode
                       AND c.RefferenceNo = b.SKPNo
                       AND a.CompanyCode = '{0}' 
                       AND a.BranchCode = '{1}'
                       AND b.Status = '0' 
                       AND e.Status = '2' 
                       AND b.InvoiceNo not in (select RefferenceInvoiceNo from OmTrPurchaseHPP
                       where companyCode = a.companyCode and branchCode = b.branchCode and Status <> '3')", Uid.CompanyCode, Uid.BranchCode);
            var queryable = ctx.Context.Database.SqlQuery<ReffInvView>(qry).AsQueryable();


            //            var query = string.Format(@"
            //                SELECT distinct a.BatchNo, b.InvoiceNo, b.InvoiceDate, b.FakturPajakNo, b.
            //                       FakturPajakDate, c.PONo, c.SupplierCode, c.BillTo, b.DueDate,
            //                       (select d.supplierName from gnMstSupplier d where d.companyCode = c.companyCode
            //					   and d.supplierCode = c.supplierCode) as SupplierName, c.RefferenceNo, b.Remark
            //                  FROM omUtlSHPOKHdr a, omUtlSHPOKDtl1 b, omTrPurchasePO c
            //					   inner join omTrPurchaseBPU e on c.companyCode = e.companyCode
            //					   and c.branchCode = e.branchCode and c.PONo = e.PONo
            //                 WHERE a.CompanyCode = b.CompanyCode
            //                       AND a.BranchCode = b.BranchCode
            //                       AND a.BatchNo = b.BatchNo
            //                       AND a.CompanyCode = c.CompanyCode
            //                       AND a.BranchCode = c.BranchCode
            //                       AND c.RefferenceNo = b.SKPNo
            //                       AND a.CompanyCode = convert(varchar,'{0}')
            //                       AND a.BranchCode = convert(varchar,'{1}')
            //                       ", Uid.CompanyCode, Uid.BranchCode);
            //            var queryable = ctx.Context.Database.SqlQuery<ReffInvView>(query).AsQueryable();
            //var queryable = ctx.Context.Database.SqlQuery<spTrnIReservedHdrView>("select ReservedNo,ReservedDate,ReferenceNo,ReferenceDate,PartNo,TypeOfGoods,OprCode,Status from spTrnIReservedHdr where Status=0 and  CompanyCode='" + Uid.CompanyCode + "' and  BranchCode='" + Uid.BranchCode + "'").AsQueryable();
            //var queryable = from p in
            //                    (
            //                        from a in ctx.Context.OmUtlSHPOKHdrs
            //                        join b in ctx.Context.OmUtlSHPOKDtl1s
            //                        on new { a.CompanyCode, a.BranchCode, a.BatchNo } equals new { b.CompanyCode, b.BranchCode, b.BatchNo } where
            //                        !ctx.Context.omTrPurchaseHPP.Any(f => (f.CompanyCode == Uid.CompanyCode) && (f.BranchCode == Uid.BranchCode) && (f.Status != "3") && (f.RefferenceInvoiceNo == b.InvoiceNo))
            //                        join c in ctx.Context.OmTrPurchasePOs
            //                        on new { a.CompanyCode, a.BranchCode, b.SKPNo } equals new { c.CompanyCode, c.BranchCode, SKPNo = c.RefferenceNo }
            //                        join d in ctx.Context.Supplier
            //                        on new { c.CompanyCode, c.SupplierCode } equals new { d.CompanyCode, d.SupplierCode }
            //                        join e in ctx.Context.omTrPurchaseBPU
            //                        on new { c.CompanyCode, c.BranchCode, c.PONo } equals new { e.CompanyCode, e.BranchCode, e.PONo }
            //                        where a.CompanyCode == Uid.CompanyCode && a.BranchCode == Uid.BranchCode && a.Status == "0"
            //                        group new { a, b, c, d, e } by a.BatchNo into View
            //                        select View.FirstOrDefault()
            //                    )
            //                select new ReffInvView()
            //                {
            //                    BatchNo = p.a.BatchNo,
            //                    InvoiceNo = p.b.InvoiceNo,
            //                    InvoiceDate = p.b.InvoiceDate,
            //                    FakturPajakNo = p.b.FakturPajakNo,
            //                    FakturPajakDate = p.b.FakturPajakDate,
            //                    PONo = p.c.PONo,
            //                    SupplierCode = p.c.SupplierCode,
            //                    SupplierName = p.d.SupplierName,
            //                    BillTo = p.c.BillTo,
            //                    DueDate = p.b.DueDate,
            //                    RefferenceNo = p.c.RefferenceNo,
            //                    Remark = p.b.Remark
            //                };
            return (queryable);
        }

        [HttpGet]
        public IQueryable<BPUView> BPUDetailLookup(string PONo)
        {
            var Uid = CurrentUser();
            //            var query = string.Format(@"
            //                SELECT DISTINCT a.BPUNo, a.RefferenceDONo, a.RefferenceSJNo, a.PONo
            //                FROM    dbo.omTrPurchaseBPU a
            //                INNER JOIN
            //                        dbo.omTrPurchaseBPUDetailModel b
            //                        ON a.CompanyCode = b.CompanyCode
            //                        AND a.BranchCode = b.BranchCode
            //                        AND a.PONo = b.PONo
            //                        AND a.BPUNo = b.BPUNo
            //                WHERE   a.CompanyCode = '{0}'
            //                        AND a.BranchCode = '{1}'
            //                        AND a.PONo = '{2}'
            //                        and a.Status = 2 AND b.QuantityHPP < b.QuantityBPU
            //                       ", Uid.CompanyCode, Uid.BranchCode, PONo);

            //            var queryable = ctx.Context.Database.SqlQuery<BPUView>(query).AsQueryable();

            //var queryable = ctx.Context.Database.SqlQuery<spTrnIReservedHdrView>("select ReservedNo,ReservedDate,ReferenceNo,ReferenceDate,PartNo,TypeOfGoods,OprCode,Status from spTrnIReservedHdr where Status=0 and  CompanyCode='" + Uid.CompanyCode + "' and  BranchCode='" + Uid.BranchCode + "'").AsQueryable();
           //SELECT DISTINCT a.BPUNo, a.RefferenceDONo, a.RefferenceSJNo, a.PONo
           //     FROM    dbo.omTrPurchaseBPU a
           //     INNER JOIN
           //             dbo.omTrPurchaseBPUDetailModel b
           //             ON a.CompanyCode = b.CompanyCode
           //             AND a.BranchCode = b.BranchCode
           //             AND a.PONo = b.PONo
           //             AND a.BPUNo = b.BPUNo
           //     WHERE   a.CompanyCode = @CompanyCode
           //             AND a.BranchCode = @BranchCode
           //             AND a.PONo = @PONo
           //             and a.Status = 2 AND b.QuantityHPP < b.QuantityBPU 


            var queryable = (from a in ctx.Context.omTrPurchaseBPU
                        join b in ctx.Context.OmTrPurchaseBPUDetailModels
                            on new { a.CompanyCode, a.BranchCode, a.PONo, a.BPUNo } equals new { b.CompanyCode, b.BranchCode, b.PONo, b.BPUNo }
                        where a.CompanyCode == Uid.CompanyCode
                                && a.BranchCode == Uid.BranchCode
                                && a.PONo == PONo
                                && a.Status == "2"
                                && b.QuantityHPP < b.QuantityBPU
                        select new BPUView()
                        {
                            BPUNo = a.BPUNo,
                            SupplierCode = a.SupplierCode,
                            PONo = a.PONo,
                            RefferenceSJNo = a.RefferenceSJNo,
                            RefferenceDONo = a.RefferenceDONo
                        }).Distinct();

            return (queryable);
        }

        [HttpGet]
        public IQueryable<MtsModelView> SalesModelCode4HPP(string PONo, string BPUNo, string salesModelCode)
        {
            var Uid = CurrentUser();
            var query = string.Format(@"
                SELECT distinct (a.SalesModelCode) as SalesModelCode
                FROM dbo.omTrPurchaseBPUDetail a  
                WHERE a.CompanyCode = '{0}'      
                AND a.BranchCode = '{1}'       
                AND a.PONo = '{2}'       
                AND a.BPUNo = '{3}'       
                AND a.isReturn = 0  
                union  
                SELECT distinct b.SalesModelCode as SalesModelCode
                FROM dbo.omTrPurchaseHPPDetailModel b  
                WHERE b.CompanyCode = '{0}'       
                AND b.BranchCode = '{1}'       
                AND b.BPUNo = '{3}'  
                       ", Uid.CompanyCode, Uid.BranchCode, PONo, BPUNo);
            if (!String.IsNullOrEmpty(salesModelCode))
                query = string.Format(@"
                SELECT distinct a.SalesModelCode as SalesModelCode, a.SalesModelYear as SalesModelYear  
                FROM dbo.omTrPurchaseBPUDetail a  
                WHERE a.CompanyCode = '{0}'      
                AND a.BranchCode = '{1}'      
                AND a.PONo =  '{2}'      
                AND a.BPUNo =  '{3}'    
                AND a.isReturn = 0 
                AND a.SalesModelCode =  '{4}'  
                union  
                SELECT distinct b.SalesModelCode as SalesModelCode, b.SalesModelYear as SalesModelYear  
                FROM dbo.omTrPurchaseHPPDetailModel b  
                WHERE b.CompanyCode = '{0}'     
                AND b.BranchCode = '{1}'     
                AND b.BPUNo = '{3}' 
                AND b.SalesModelCode = '{4}' ", Uid.CompanyCode, Uid.BranchCode, PONo, BPUNo, salesModelCode);

            var queryable = ctx.Context.Database.SqlQuery<MtsModelView>(query).AsQueryable();

            //var queryable = ctx.Context.Database.SqlQuery<spTrnIReservedHdrView>("select ReservedNo,ReservedDate,ReferenceNo,ReferenceDate,PartNo,TypeOfGoods,OprCode,Status from spTrnIReservedHdr where Status=0 and  CompanyCode='" + Uid.CompanyCode + "' and  BranchCode='" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<HPPView> supplier4Karoseri()
        {
            var Uid = CurrentUser();
            var profitCenter = ctx.Context.SysUserProfitCenters.Where(p => p.UserId == Uid.UserId).FirstOrDefault().ProfitCenter;
            var query = string.Format(@"
                            SELECT a.SupplierCode, a.SupplierName
                        FROM gnMstSupplier a 
                        INNER JOIN GnMstSupplierProfitCenter b 
                        ON a.CompanyCode=b.CompanyCode
                        AND a.SupplierCode=b.SupplierCode                                  
                        WHERE a.SupplierCode in (SELECT DISTINCT SupplierCode FROM omMstKaroseri where Status = 1)
                        AND a.CompanyCode='{0}'
                        AND b.BranchCode='{1}'
                        AND b.ProfitCenterCode='{2}'
                        AND b.isBlackList=0
                        AND a.status = 1
                        ORDER BY a.SupplierCode", Uid.CompanyCode, Uid.BranchCode, "100");
            var queryable = ctx.Context.Database.SqlQuery<HPPView>(query).AsQueryable();
            //var queryable = ctx.Context.Database.SqlQuery<spTrnIReservedHdrView>("select ReservedNo,ReservedDate,ReferenceNo,ReferenceDate,PartNo,TypeOfGoods,OprCode,Status from spTrnIReservedHdr where Status=0 and  CompanyCode='" + Uid.CompanyCode + "' and  BranchCode='" + Uid.BranchCode + "'").AsQueryable();
            //var query = from p in
            //                (
            //                    from a in ctx.Context.MstKaroseri
            //                    where a.Status == "1"
            //                    group a by a.SupplierCode into View
            //                    select View.FirstOrDefault().SupplierCode
            //                    )
            //            select p;


            //var queryable = from a in ctx.Context.Supplier
            //                join b in ctx.Context.SupplierProfitCenter
            //                on new { a.CompanyCode, a.SupplierCode } equals new { b.CompanyCode, b.SupplierCode }
            //                where query.Contains(a.SupplierCode)
            //                        && a.CompanyCode == Uid.CompanyCode
            //                        && b.ProfitCenterCode == "100"
            //                        && b.isBlackList == false
            //                        && a.Status == "1"
            //                select new HPPView()
            //                {
            //                    SupplierCode = a.SupplierCode,
            //                    SupplierName = a.SupplierName
            //                };
            return (queryable);
        }


        [HttpGet]
        public IQueryable<PerlengkapanOutView> PerlengkapanoutLookup()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.omTrSalesPerlengkapanOuts
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            select new PerlengkapanOutView()
                            {
                                PerlengkapanNo = p.PerlengkapanNo,
                                PerlengkapanDate = p.PerlengkapanDate,
                                RefferenceNo = p.ReferenceNo,
                                RefferenceDate = p.ReferenceDate
                            }; return (queryable);
        }

        [HttpGet]
        public IQueryable<SOView> SOCustomerLookup()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.GnMstCustomer
                            where p.CompanyCode == Uid.CompanyCode
                            select new SOView()
                            {
                                CustomerCode = p.CustomerCode,
                                CustomerName = p.CustomerName
                            }; return (queryable);
        }

        [HttpGet]
        public IQueryable<SalesReqView> SalesReqLookup()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.omTrSalesReq
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            select new SalesReqView()
                            {
                                ReqNo = p.ReqNo,
                                ReqDate = p.ReqDate
                            }; return (queryable);
        }

        [HttpGet]
        public IQueryable<SPKView> SPKLookup()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.omTrSalesSPK
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            select new SPKView()
                            {
                                SPKNo = p.SPKNo,
                                SPKDate = p.SPKDate
                            }; return (queryable);
        }

        [HttpGet]
        public IQueryable<SPKView> ChassisLookup()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<SPKView>("select distinct convert(varchar, chassisNo) as chassisNo from ommstvehicle WHERE CompanyCode = '" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);
        }



        [HttpGet]
        public IQueryable<TransferOutView> TransferOutLookup()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.OmTrInventTransferOut
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            select new TransferOutView()
                            {
                                TransferOutNo = p.TransferOutNo,
                                TransferOutDate = p.TransferOutDate
                            }; return (queryable);
        }

        [HttpGet]
        public IQueryable<SparePartList>SOPartAcc(string SONo)
        {
            var Uid = CurrentUser();
            var qry = string.Format("uspfn_SelectPartsAcc '{0}','{1}','{2}'", Uid.CompanyCode, Uid.BranchCode, SONo);
            var rslt = ctx.Context.Database.SqlQuery<SparePartList>(qry).OrderBy(a => a.JenisPart).AsQueryable();
            return (rslt);
        } 


        [HttpGet]
        public IQueryable<InvoiceView> InvoiceLookup()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.OmTrSalesInvoices
                            join q in ctx.Context.OmTRSalesSOs
                                on p.SONo equals q.SONo
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            select new InvoiceView()
                            {
                                InvoiceNo = p.InvoiceNo,
                                InvoiceDate = p.InvoiceDate,
                                SONo = p.SONo,
                                SODate = q.SODate
                            }; return (queryable);
        }

        [HttpGet]
        public IQueryable<SalesReturnView> SalesReturnLookup()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.omTrSalesReturn
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            select new SalesReturnView()
                            {
                                InvoiceNo = p.InvoiceNo,
                                InvoiceDate = p.InvoiceDate,
                                ReturnNo = p.ReturnNo,
                                ReturnDate = p.ReturnDate
                            }; return (queryable);
        }

        [HttpGet]
        public IQueryable<WhareHouseLookup> WhareHouseLookup()
        {
            bool isMainDealer = DealerCode() == "MD";
            var Uid = CurrentUser();
            var CompanyCode = isMainDealer ? Uid.CompanyCode : CompanyMD;
            var BranchCode = isMainDealer ? Uid.BranchCode : BranchMD;
            if (isMainDealer)
            {
                var queryable = from p in ctx.Context.MstRefferences
                                where p.CompanyCode == CompanyCode && p.RefferenceType == "WARE"
                                select new WhareHouseLookup()
                                {
                                    RefferenceCode = p.RefferenceCode,
                                    RefferenceDesc1 = p.RefferenceDesc1
                                }; return (queryable);

            }
            else
            {
                var queryable = from p in ctxMD.Context.MstRefferences
                                where p.CompanyCode == CompanyCode && p.RefferenceType == "WARE"
                                select new WhareHouseLookup()
                                {
                                    RefferenceCode = p.RefferenceCode,
                                    RefferenceDesc1 = p.RefferenceDesc1
                                }; return (queryable);

            }
        }

        [HttpGet]
        public IQueryable<TransferOutView> BranchLookup()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<TransferOutView>("select distinct a.BranchCode,a.BranchName from  gnMstOrganizationDtl a INNER JOIN OmTrInventTransferOut b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode Where a.CompanyCode ='" + Uid.CompanyCode + "' and a.BranchCode='" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<TransferInView> TransfeInLookup()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.omTrInventTransferIn
                            join q in ctx.Context.OmTrInventTransferOut
                                on p.TransferOutNo equals q.TransferOutNo
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            select new TransferInView()
                            {
                                TransferInNo = p.TransferInNo,
                                TransferInDate = p.TransferInDate,
                                TransferOutNo = p.TransferOutNo,
                                TransferOutDate = q.TransferOutDate,
                            }; return (queryable);
        }

        [HttpGet]
        public IQueryable<InquiryPerlengkapanLookup> InquiryPerlengkapanLookup()
        {
            var Uid = CurrentUser();
            var queryable = (from p in ctx.Context.OMTrInventQtyPerlengkapan
                             join q in ctx.Context.MstPerlengkapan
                                 on p.PerlengkapanCode equals q.PerlengkapanCode
                             where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                             select new InquiryPerlengkapanLookup()
                             {
                                 PerlengkapanCode = p.PerlengkapanCode,
                                 PerlengkapanName = q.PerlengkapanName
                             }).Distinct(); return (queryable);
        }

        [HttpGet]
        public IQueryable<MstVehicleLookup> InquiryDataKendaraanModelLookup()
        {
            bool isMainDealer = DealerCode() == "MD";
            var Uid = CurrentUser();
            var CompanyCode = isMainDealer ? Uid.CompanyCode : CompanyMD;
            var BranchCode = isMainDealer ? Uid.BranchCode : BranchMD;
            if (isMainDealer)
            {
                var queryable = (from p in ctx.Context.OmMstVehicles
                                 join q in ctx.Context.MstModels
                                     on p.SalesModelCode equals q.SalesModelCode
                                 where p.CompanyCode == CompanyCode
                                 select new MstVehicleLookup()
                                 {
                                     SalesModelCode = p.SalesModelCode,
                                     SalesModelDesc = q.SalesModelDesc
                                 }).Distinct(); return (queryable);
            }
            else
            {
                var queryable = (from p in ctxMD.Context.OmMstVehicles
                                 join q in ctxMD.Context.MstModels
                                     on p.SalesModelCode equals q.SalesModelCode
                                 where p.CompanyCode == CompanyCode
                                 select new MstVehicleLookup()
                                 {
                                     SalesModelCode = p.SalesModelCode,
                                     SalesModelDesc = q.SalesModelDesc
                                 }).Distinct(); return (queryable);
            }

        }


        [HttpGet]
        public IQueryable<MstVehicleLookup> InquiryDataKendaraanCHassisCodeLookup()
        {
            bool isMainDealer = DealerCode() == "MD";
            var Uid = CurrentUser();
            var CompanyCode = isMainDealer ? Uid.CompanyCode : CompanyMD;
            var BranchCode = isMainDealer ? Uid.BranchCode : BranchMD;
            if (isMainDealer)
            {
                var queryable = (from p in ctx.Context.OmMstVehicles
                                 where p.CompanyCode == CompanyCode
                                 select new MstVehicleLookup()
                                 {
                                     ChassisCode = p.ChassisCode,
                                 }).Distinct(); return (queryable);

            }
            else
            {
                var queryable = (from p in ctxMD.Context.OmMstVehicles
                                 where p.CompanyCode == CompanyCode
                                 select new MstVehicleLookup()
                                 {
                                     ChassisCode = p.ChassisCode,
                                 }).Distinct(); return (queryable);

            }
        }

        [HttpGet]
        public IQueryable<MstVehicleLookup> InquiryDataKendaraanCHassisNoLookup()
        {
            bool isMainDealer = DealerCode() == "MD";
            var Uid = CurrentUser();
            var CompanyCode = isMainDealer ? Uid.CompanyCode : CompanyMD;
            var BranchCode = isMainDealer ? Uid.BranchCode : BranchMD;
            if (isMainDealer)
            {
                var queryable = (from p in ctx.Context.OmMstVehicles
                                 where p.CompanyCode == CompanyCode
                                 select new MstVehicleLookup()
                                 {
                                     ChassisNo = p.ChassisNo,
                                 }).Distinct(); return (queryable);
            }
            else
            {
                var queryable = (from p in ctxMD.Context.OmMstVehicles
                                 where p.CompanyCode == CompanyCode
                                 select new MstVehicleLookup()
                                 {
                                     ChassisNo = p.ChassisNo,
                                 }).Distinct(); return (queryable);
            }
        }

        /* Purchase Return*/

        [HttpGet]
        public IQueryable<ReturnView> PurchaseReturnBrowse()
        {
            var Uid = CurrentUser();

            var data = from e in ctx.Context.omTrPurchaseReturn
                       where e.CompanyCode == Uid.CompanyCode &&
                       e.BranchCode == Uid.BranchCode
                       select new ReturnView
                       {
                           ReturnNo = e.ReturnNo,
                           ReturnDate = e.ReturnDate,
                           RefferenceNo = e.RefferenceNo,
                           RefferenceDate = e.RefferenceDate,
                           HPPNo = e.HPPNo,
                           RefferenceFakturPajakNo = e.RefferenceFakturPajakNo,
                           Remark = e.Remark,
                           Status = e.Status == "0" ? "Open" : e.Status == "1" ? "Printed"
                           : e.Status == "2" ? "Approved"
                           : e.Status == "3" ? "Canceled"
                           : e.Status == "9" ? "Finished" : "",
                           Stat = e.Status
                       };

            return data;
        }

        [HttpGet]
        public IQueryable<HPPView> HPPNoLookup()
        {
            var Uid = CurrentUser();
            var queryable = (from p in ctx.Context.omTrPurchaseHPP
                             join q in ctx.Context.Supplier
                                 on p.SupplierCode equals q.SupplierCode
                             where p.CompanyCode == Uid.CompanyCode 
                             && p.BranchCode == Uid.BranchCode
                             && (p.Status == "2" || p.Status == "5")
                             select new HPPView()
                             {
                                 HPPNo = p.HPPNo,
                                 HPPDate = p.HPPDate,
                                 PONo = p.PONo,
                                 SupplierName = p.SupplierCode + "-" + q.SupplierName,
                                 RefferenceFakturPajakNo = p.RefferenceFakturPajakNo,
                                 RefferenceFakturPajakDate = p.RefferenceFakturPajakDate
                             }).Distinct(); return (queryable);
        }

        [HttpGet]
        public IQueryable<BPUView> BPUNoLookup(string ReturnNo, string HPPNo)
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<BPUView>("sp_PurchaseReturnBPULookup '" + Uid.CompanyCode + "','" + Uid.BranchCode + "','" + ReturnNo + "','" + HPPNo + "'").AsQueryable();

            return (queryable);
        }

        [HttpGet]
        public IQueryable<MtsModelView> ModelCodeLookup(string HPPNo, string BPUNo)
        {
            var uid = CurrentUser();
            //            var query = String.Format(@"SELECT a.SalesModelCode,a.SalesModelDesc
            //            FROM omMstModel a
            //            WHERE a.SalesModelCode in (select distinct SalesModelCode from omTrPurchaseHPPDetailModel
            //						               where CompanyCode = {0} and BranchCode = '{1}'
            //						               and HPPNo = '{2}' and BPUNo = '{3}')
            //            AND a.CompanyCode = '{0}'", uid.CompanyCode, uid.BranchCode, HPPNo, BPUNo);
            //            var queryable = ctx.Context.Database.SqlQuery<MtsModelView>(query).AsQueryable();
            var query = from p in
                            (
                                from a in ctx.Context.omTrPurchaseHPPDetailModel
                                where a.CompanyCode == uid.CompanyCode && a.BranchCode == uid.BranchCode
                                && a.HPPNo == HPPNo && a.BPUNo == BPUNo
                                group a by a.SalesModelCode into View
                                select View.FirstOrDefault().SalesModelCode
                                )
                        select p;

            var queryable = from a in ctx.Context.MstModels
                            where a.CompanyCode == uid.CompanyCode && query.Contains(a.SalesModelCode)
                            select new MtsModelView()
                            {
                                SalesModelCode = a.SalesModelCode,
                                SalesModelDesc = a.SalesModelDesc
                            };
            //var queryable = ctx.Context.MstModels.Where(p => query.Contains(p.SalesModelCode));
            return (queryable);
        }

        [HttpGet]
        public IQueryable<MstModelYearView> ModelYearLookup(string HPPNo, string BPUNo, string SalesModelCode)
        {
            var uid = CurrentUser();
            //            var query = String.Format(@"SELECT a.SalesModelYear ,a.SalesModelDesc
            //            FROM omMstModelYear a
            //            WHERE a.SalesModelYear in (select distinct SalesModelYear from omTrPurchaseHPPDetailModel
            //                                       where CompanyCode = '{0}' and BranchCode = '{1}'
            //                                       and HPPNo = '{2}' and BPUNo = '{3}' and SalesModelCode = '{4}')
            //            AND a.CompanyCode = '{0}'
            //            AND a.SalesModelCode = '{4}'", uid.CompanyCode, uid.BranchCode, HPPNo, BPUNo, SalesModelCode);
            //            var queryable = ctx.Context.Database.SqlQuery<MstModelYearView>(query).AsQueryable();
            var query = from p in
                            (
                                from a in ctx.Context.omTrPurchaseHPPDetailModel
                                where a.CompanyCode == uid.CompanyCode && a.BranchCode == uid.BranchCode
                                && a.HPPNo == HPPNo && a.BPUNo == BPUNo && a.SalesModelCode == SalesModelCode
                                group a by a.SalesModelYear into View
                                select View.FirstOrDefault().SalesModelYear
                                )
                        select p;

            var queryable = from a in ctx.Context.MstModelYear
                            where a.CompanyCode == uid.CompanyCode && query.Contains(a.SalesModelYear)
                            && a.SalesModelCode == SalesModelCode
                            select new MstModelYearView()
                            {
                                SalesModelYear = a.SalesModelYear,
                                SalesModelDesc = a.SalesModelDesc
                            };
            return (queryable);
        }

        [HttpGet]
        public IQueryable<ReturnSubDetail> ChassisCodeLookup(string HPPNo, string BPUNo, string SalesModelCode, string SalesModelYear)
        {
            var uid = CurrentUser();
            //            var query = String.Format(@"
            //                        select a.ChassisNo,a.EngineCode,a.ChassisCode,a.EngineNo,
            //	                        a.ColourCode as ColourCode, b.RefferenceDesc1 as ColourName, convert(varchar,a.HPPSeq) as HPPSeq
            //                        from omTrPurchaseHPPSubDetail a
            //	                        left join omMstRefference b
            //		                        on a.CompanyCode = b.CompanyCode
            //		                        and b.RefferenceType = 'COLO'
            //		                        and b.RefferenceCode = a.ColourCode
            //	                        left join omMstVehicle c
            //		                        on a.CompanyCode = c.CompanyCode
            //		                        and a.ChassisCode  = c.ChassisCode
            //		                        and a.ChassisNo = c.ChassisNo
            //		                        and c.Status = '0'
            //                        where a.CompanyCode = '{0}'
            //	                        and a.BranchCode = '{1}'
            //	                        and a.HPPNo = '{2}'
            //	                        and a.BPUNo = '{3}'
            //	                        and a.SalesModelCode = '{4}'
            //	                        and a.SalesModelYear = '{5}'
            //	                        and a.isReturn = '0'", uid.CompanyCode, uid.BranchCode, HPPNo, BPUNo, SalesModelCode, SalesModelYear);
            //            var queryable = ctx.Context.Database.SqlQuery<ReturnSubDetail>(query).AsQueryable();
            var queryable = from a in ctx.Context.omTrPurchaseHPPSubDetail
                            join b in ctx.Context.MstRefferences
                            on new { a.CompanyCode, a.ColourCode, RefferenceType = "COLO" } equals new { b.CompanyCode, ColourCode = b.RefferenceCode, b.RefferenceType }
                            join c in ctx.Context.OmMstVehicles
                            on new { a.CompanyCode, a.ChassisCode, a.ChassisNo, Status = "0" } equals new { c.CompanyCode, c.ChassisCode, c.ChassisNo, c.Status }
                            where a.CompanyCode == uid.CompanyCode
                            && a.BranchCode == uid.BranchCode
                            && a.HPPNo == HPPNo
                            && a.BPUNo == BPUNo
                            && a.SalesModelCode == SalesModelCode
                            && a.SalesModelYear.ToString() == SalesModelYear.ToString()
                            && a.isReturn == false
                            select new ReturnSubDetail()
                            {
                                ChassisNo = a.ChassisNo,
                                ChassisCode = a.ChassisCode,
                                EngineNo = a.EngineNo,
                                EngineCode = a.EngineCode,
                                ColourCode = a.ColourCode,
                                ColourName = b.RefferenceDesc1,
                                HPPSeq = a.HPPSeq.ToString()
                            };
            return (queryable);
        }

        /*------------------------*/

        /* Perlengkapan Adjustment*/

        [HttpGet]
        public IQueryable<PerlengkapanAdjustmentView> PerlengkapanAdjustBrowse()
        {
            var Uid = CurrentUser();

            var data = from e in ctx.Context.omTrPurchasePerlengkapanAdjustment
                       where e.CompanyCode == Uid.CompanyCode &&
                       e.BranchCode == Uid.BranchCode
                       select new PerlengkapanAdjustmentView
                       {
                           AdjustmentNo = e.AdjustmentNo,
                           AdjustmentDate = e.AdjustmentDate,
                           RefferenceNo = e.RefferenceNo,
                           RefferenceDate = e.RefferenceDate,
                           Remark = e.Remark,
                           Status = e.Status == "0" ? "OPEN" : e.Status == "1" ? "PRINTED"
                           : e.Status == "2" ? "APPROVED"
                           : e.Status == "3" ? "CANCELED"
                           : e.Status == "9" ? "FINISHED" : "",
                           Stat = e.Status
                       };

            return data;
        }

        /*------------------------*/

        #region Perlengkapan In

        [HttpGet]
        public IQueryable<ReffEqInNoBrowse> ReffEqInNoBrowse()
        {
            var query = string.Format(@"SELECT a.BPPNo,a.BatchNo,a.BPPDate,b.BPUNo, b.RefferenceSJNo FROM OmUtlSACCSDtl1 a
            INNER JOIN OmTrPurchaseBPU b ON 
            b.CompanyCode = a.CompanyCode AND
            b.RefferenceSJNo = a.SJNo AND
            b.Status != '0'			
            INNER JOIN OmUtlSACCSHdr c ON 
            c.CompanyCode = a.CompanyCode AND
            c.BatchNo = a.BatchNo AND
            a.Status = '0'
            and a.BPPNo not in (select RefferenceNo from omTrPurchasePerlengkapanIn where
            companyCode = b.companyCode and branchCode = b.branchCode and Status != 3)
            WHERE a.CompanyCode = {0}
            AND a.BranchCode = {1}
            ORDER BY a.BPPNo ASC", CurrentUser().CompanyCode, CurrentUser().BranchCode);

            var data = ctx.Context.Database.SqlQuery<ReffEqInNoBrowse>(query).AsQueryable();
            return data;
        }

        [HttpGet]
        public IQueryable<PerlengkapanInView> EquipmentInBrowse()
        {
            var Uid = CurrentUser();
            var data = ctx.Context.omTrPurchasePerlengkapanIn.Where(a => a.CompanyCode == Uid.CompanyCode && a.BranchCode == Uid.BranchCode)
                .Select(e => new PerlengkapanInView
                {
                    PerlengkapanNo = e.PerlengkapanNo,
                    PerlengkapanDate = e.PerlengkapanDate,
                    RefferenceNo = e.RefferenceNo,
                    RefferenceDate = e.RefferenceDate,
                    PerlengkapanType = e.PerlengkapanType,
                    PerlengkapanTypeName = e.PerlengkapanType == "1" ? "BPU" : e.PerlengkapanType == "2" ? "TRANSFER" : "RETURN",
                    SourceDoc = e.SourceDoc,
                    Remark = e.Remark,
                    Status = e.Status == "0" ? "OPEN" : e.Status == "1" ? "PRINTED"
                               : e.Status == "2" ? "APPROVED"
                               : e.Status == "3" ? "CANCELED"
                               : e.Status == "9" ? "FINISHED" : "",
                    Stat = e.Status
                });
            return data;
        }

        [HttpGet]
        public IQueryable<PerlengkapanInDetailView> EquipmentBrowse()
        {
            var Uid = CurrentUser();
            var data = ctx.Context.MstPerlengkapan.Where(a => a.CompanyCode == Uid.CompanyCode && a.BranchCode == Uid.BranchCode && a.Status == "1")
                .Select(m => new PerlengkapanInDetailView
                {
                    PerlengkapanCode = m.PerlengkapanCode,
                    PerlengkapanName = m.PerlengkapanName,
                    Remark = m.Remark
                });

            return data;
        }

        [HttpGet]
        public IQueryable<SourceDocBrowse> SourceDocBrowse(string type)
        {
            var Uid = CurrentUser();
            var data = new List<SourceDocBrowse>();

            switch (type)
            {
                case "1":
                    data = ctx.Context.omTrPurchaseBPU.Where(a => a.CompanyCode == Uid.CompanyCode && a.BranchCode == Uid.BranchCode && a.Status != "0")
                            .Select(a => new SourceDocBrowse
                            {
                                BPUNo = a.BPUNo,
                                BPUDate = a.BPUDate,
                                PONo = a.PONo,
                                ReffereneSJNo = a.RefferenceSJNo
                            }).OrderBy(a => a.BPUNo).ToList();
                    break;
                case "2":
                    data = ctx.Context.omTrInventTransferIn.Where(a => a.CompanyCode == Uid.CompanyCode && a.BranchCode == Uid.BranchCode)
                        .Select(a => new SourceDocBrowse
                        {
                            TransferInNo = a.TransferInNo,
                            TransferInDate = a.TransferInDate
                        }).OrderByDescending(a => a.TransferInNo).ToList();
                    break;
                case "3":
                    data = ctx.Context.omTrSalesReturn.Where(a => a.CompanyCode == Uid.CompanyCode && a.BranchCode == Uid.BranchCode)
                        .Select(a => new SourceDocBrowse
                        {
                            ReturnNo = a.ReturnNo,
                            ReturnDate = a.ReturnDate,
                            RefferenceNo = a.ReferenceNo
                        }).OrderBy(a => a.ReturnNo).ToList();
                    break;
            }
            return data.AsQueryable();
        }

        #endregion

        [HttpGet]
        public IQueryable<inquiryTrPurchaseKaroseriView> SalesModelCodeOld(string supplierCode)
        {
            var Uid = CurrentUser();
            //            var query = string.Format(@"
            //                SELECT a.SalesModelCode as SalesModelCodeOld, b.SalesModelDesc, 
            //                PPn, DPPOthers, DPPFee, DPPMaterial, Total
            //                FROM omMstKaroseri a
            //                LEFT JOIN omMstModel b
            //                ON a.CompanyCode =b.COmpanyCode
            //                AND a.SalesModelCode = b.SalesModelCode
            //                WHERE a.Status = 1
            //                AND a.CompanyCode = '{0}'
            //                AND a.BranchCode ='{1}'
            //                AND a.SupplierCode = '{2}'
            //                ORDER BY a.SalesModelCode", Uid.CompanyCode, Uid.BranchCode, supplierCode);
            //            var queryable = ctx.Context.Database.SqlQuery<inquiryTrPurchaseKaroseriView>(query).AsQueryable();
            //var queryable = ctx.Context.Database.SqlQuery<spTrnIReservedHdrView>("select ReservedNo,ReservedDate,ReferenceNo,ReferenceDate,PartNo,TypeOfGoods,OprCode,Status from spTrnIReservedHdr where Status=0 and  CompanyCode='" + Uid.CompanyCode + "' and  BranchCode='" + Uid.BranchCode + "'").AsQueryable();
            var queryable = from a in ctx.Context.MstKaroseri
                            join b in ctx.Context.MstModels
                            on new { a.CompanyCode, a.SalesModelCode } equals new { b.CompanyCode, b.SalesModelCode }
                            where a.Status == "1"
                            && a.CompanyCode == Uid.CompanyCode
                            && a.BranchCode == Uid.BranchCode
                            && a.SupplierCode == supplierCode
                            orderby a.SalesModelCode
                            select new inquiryTrPurchaseKaroseriView()
                            {
                                SalesModelCodeOld = a.SalesModelCode,
                                SalesModelCodeNew = a.SalesModelCodeNew,
                                SalesModelDesc = b.SalesModelDesc,
                                PPn = a.PPn,
                                DPPOthers = a.DPPOthers,
                                DPPFee = a.DPPFee,
                                DPPMaterial = a.DPPMaterial,
                                Total = a.Total
                            };
            return (queryable);
        }

        [HttpGet]
        public IQueryable<MstModelYearView> SalesModelYear(string SalesModelCode)
        {
            var Uid = CurrentUser();
            //            var query = string.Format(@"
            //                select Convert(varchar(4),a.SalesModelYear) AS SalesModelYear, a.SalesModelDesc, a.ChassisCode, a.Remark
            //            from OmMstModelYear a
            //            WHERE a.CompanyCode = '{0}'
            //            AND a.SalesModelCode = '{1}'
            //            AND a.Status = '1'
            //            ORDER BY a.SalesModelYear,a.SalesModelDesc ASC 
            //                       ", Uid.CompanyCode, SalesModelCode);
            //           var queryable = ctx.Context.Database.SqlQuery<MstModelYearView>(query).AsQueryable();
            //var queryable = ctx.Context.Database.SqlQuery<spTrnIReservedHdrView>("select ReservedNo,ReservedDate,ReferenceNo,ReferenceDate,PartNo,TypeOfGoods,OprCode,Status from spTrnIReservedHdr where Status=0 and  CompanyCode='" + Uid.CompanyCode + "' and  BranchCode='" + Uid.BranchCode + "'").AsQueryable();
            var queryable = from a in ctx.Context.MstModelYear
                            where a.CompanyCode == Uid.CompanyCode
                            && a.SalesModelCode == SalesModelCode
                            && a.Status == "1"
                            orderby a.SalesModelCode, a.SalesModelYear ascending
                            select new MstModelYearView()
                            {
                                SalesModelYear = a.SalesModelYear,
                                SalesModelDesc = a.SalesModelDesc,
                                ChassisCode = a.ChassisCode,
                                Remark = a.Remark
                            };
            return (queryable);
        }

        [HttpGet]
        public IQueryable<InquiryMstVehicleView> ChssisNoLoad(string SalesModelCode, string SalesModelYear, string ChassisCode)
        {
            var Uid = CurrentUser();
            //            var query = string.Format(@"
            //                SELECT Convert(varchar,a.ChassisNo) AS ChassisNo,
            //            (SELECT b.RefferenceDesc1
            //                FROM omMstRefference b
            //                WHERE b.RefferenceCode = a.ColourCode
            //                AND b.CompanyCode = a.CompanyCode AND b.RefferenceType = 'COLO') as ColourOld,
            //            EngineCode, Convert(varchar,EngineNo) as EngineNo, ColourCode as ColourCodeOld
            //            FROM omMstVehicle a
            //            WHERE a.Status = 0
            //            AND a.isActive = 1
            //            AND a.CompanyCode = '{0}'
            //            AND a.ChassisCode = '{1}'
            //            AND a.SalesModelCode = '{2}'
            //            AND a.SalesModelYear = '{3}'
            //            order by a.ChassisNo asc 
            //                       ", Uid.CompanyCode, ChassisCode, SalesModelCode, SalesModelYear);

            //            var queryable = ctx.Context.Database.SqlQuery<InquiryMstVehicleView>(query).AsQueryable();

            //var queryable = ctx.Context.Database.SqlQuery<spTrnIReservedHdrView>("select ReservedNo,ReservedDate,ReferenceNo,ReferenceDate,PartNo,TypeOfGoods,OprCode,Status from spTrnIReservedHdr where Status=0 and  CompanyCode='" + Uid.CompanyCode + "' and  BranchCode='" + Uid.BranchCode + "'").AsQueryable();
            var queryable = from a in ctx.Context.OmMstVehicles
                            join b in ctx.Context.MstRefferences
                            on new { a.CompanyCode, a.ColourCode } equals new { b.CompanyCode, ColourCode = b.RefferenceCode }
                            where a.CompanyCode == Uid.CompanyCode
                            && a.SalesModelCode == SalesModelCode
                            && a.SalesModelYear.ToString() == SalesModelYear.ToString()
                            && a.IsActive == true
                            && b.RefferenceType == "COLO"
                            && a.Status == "0"
                            && a.ChassisCode == ChassisCode
                            orderby a.ChassisNo ascending
                            select new InquiryMstVehicleView()
                            {
                                ChassisNo = a.ChassisNo.ToString(),
                                ChassisCode = a.ChassisCode,
                                EngineNo = a.EngineNo.ToString(),
                                EngineCode = a.EngineCode,
                                ColourCodeOld = a.ColourCode,
                                ColourOld = b.RefferenceDesc1
                            };
            return (queryable);
        }

        [HttpGet]
        public IQueryable<inquiryTrPurchaseKaroseriView> KaroseriBrowses()
        {
            var Uid = CurrentUser();
            //            var query = string.Format(@"
            //                SELECT a.KaroseriSPKNo, a.KaroseriSPKDate, a.RefferenceNo, a.SupplierCode, b.SupplierName,
            //                a.SalesModelCodeOld, c.SalesModelDesc, a.Remark, SalesModelCodeNew, SalesModelYear, RefferenceDate,
            //                a.ChassisCode,Quantity,DPPMaterial,DPPFee,DPPOthers,PPn,Total,DurationDays,
            //                CASE a.Status WHEN '0' THEN 'OPEN' WHEN '1' THEN 'PRINT' WHEN '2' THEN 'CLOSE' WHEN '3' THEN 'CANCEL' WHEN '9' THEN 'FINISH' END as Status
            //                FROM omTrPurchaseKaroseri a
            //                LEFT JOIN gnMstSupplier b
            //                ON a.CompanyCode = b.CompanyCode
            //                AND a.SupplierCode = b.SupplierCode
            //                LEFT JOIN omMstModel c
            //                ON a.CompanyCode = c.CompanyCode
            //                AND a.SalesModelCodeOld = c.SalesModelCode
            //                WHERE a.CompanyCode = '{0}'
            //                AND a.BranchCode = '{1}'
            //                ORDER BY a.KaroseriSPKNo DESC", Uid.CompanyCode, Uid.BranchCode);
            //            var queryable = ctx.Context.Database.SqlQuery<inquiryTrPurchaseKaroseriView>(query).AsQueryable();
            //            //var queryable = ctx.Context.Database.SqlQuery<spTrnIReservedHdrView>("select ReservedNo,ReservedDate,ReferenceNo,ReferenceDate,PartNo,TypeOfGoods,OprCode,Status from spTrnIReservedHdr where Status=0 and  CompanyCode='" + Uid.CompanyCode + "' and  BranchCode='" + Uid.BranchCode + "'").AsQueryable();
            var queryable = from a in ctx.Context.omTrPurchaseKaroseri
                            join b in ctx.Context.Supplier
                            on new { a.CompanyCode, a.SupplierCode } equals new { b.CompanyCode, b.SupplierCode } into _b
                            from b in _b.DefaultIfEmpty()
                            join c in ctx.Context.MstModels
                            on new { a.CompanyCode, a.SalesModelCodeOld } equals new { c.CompanyCode, SalesModelCodeOld = c.SalesModelCode } into _c
                            from c in _c.DefaultIfEmpty()
                            join d in ctx.Context.LookUpDtls
                            on new { a.CompanyCode, a.BranchCode, a.WarehouseCode, CodeID = "MPWH" } equals new { d.CompanyCode, BranchCode = d.ParaValue, WarehouseCode = d.LookUpValue, d.CodeID } into _d
                            from d in _d.DefaultIfEmpty()
                            where a.CompanyCode == Uid.CompanyCode
                                 && a.BranchCode == Uid.BranchCode
                            orderby a.KaroseriSPKNo descending
                            select new inquiryTrPurchaseKaroseriView()
                            {
                                KaroseriSPKNo = a.KaroseriSPKNo,
                                KaroseriSPKDate = a.KaroseriSPKDate,
                                RefferenceNo = a.RefferenceNo,
                                SupplierCode = a.SupplierCode,
                                SupplierName = b.SupplierName,
                                SalesModelCodeOld = a.SalesModelCodeOld,
                                SalesModelDesc = c.SalesModelDesc,
                                Remark = a.Remark,
                                SalesModelCodeNew = a.SalesModelCodeNew,
                                SalesModelYear = a.SalesModelYear,
                                RefferenceDate = a.RefferenceDate,
                                ChassisCode = a.ChassisCode,
                                Quantity = a.Quantity,
                                DPPFee = a.DPPFee,
                                DPPMaterial = a.DPPMaterial,
                                DPPOthers = a.DPPOthers,
                                PPn = a.PPn,
                                Total = a.Total,
                                DurationDays = a.DurationDays,
                                Status = a.Status == "0" ? "OPEN" : a.Status == "1" ? "PRINT" : a.Status == "2" ? "CLOSE" : a.Status == "3" ? "CANCEL" : a.Status == "9" ? "FINISH" : "",
                                WareHouseCode = a.WarehouseCode,
                                WareHouseName = d.LookUpValueName
                            };
            return (queryable);
        }

        [HttpGet]
        public IQueryable<ColourLookup> ColourLookup(string SalesModelCode)
        {
            var Uid = CurrentUser();
            var query = string.Format(@"
                SELECT a.ColourCode as ColourCodeNew, (SELECT b.RefferenceDesc1
                FROM omMstRefference b
                WHERE b.RefferenceCode = a.ColourCode
                AND b.CompanyCode = a.CompanyCode AND b.RefferenceType = 'COLO')  AS ColourNew, a.Remark
                FROM omMstModelColour a
                WHERE a.CompanyCode = '{0}' 
                AND a.SalesModelCode = '{1}'
                ", Uid.CompanyCode, SalesModelCode);

            var queryable = ctx.Context.Database.SqlQuery<ColourLookup>(query).AsQueryable();

            //var queryable = ctx.Context.Database.SqlQuery<spTrnIReservedHdrView>("select ReservedNo,ReservedDate,ReferenceNo,ReferenceDate,PartNo,TypeOfGoods,OprCode,Status from spTrnIReservedHdr where Status=0 and  CompanyCode='" + Uid.CompanyCode + "' and  BranchCode='" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);
        }

        #region  Karoseri Terima

        [HttpGet]
        public IQueryable<KaroseriTerimaView> KaroseriTerimaBrowse()
        {
            var Uid = CurrentUser();

            var data = from e in ctx.Context.omTrPurchaseKaroseriTerima
                       where e.CompanyCode == Uid.CompanyCode &&
                       e.BranchCode == Uid.BranchCode
                       select new KaroseriTerimaView
                       {
                           KaroseriTerimaNo = e.KaroseriTerimaNo,
                           KaroseriTerimaDate = e.KaroseriTerimaDate,
                           KaroseriSPKNo = e.KaroseriSPKNo,
                           SupplierCode = e.SupplierCode,
                           RefferenceInvoiceNo = e.RefferenceInvoiceNo,
                           RefferenceInvoiceDate = e.RefferenceInvoiceDate,
                           RefferenceFakturPajakNo = e.RefferenceFakturPajakNo,
                           RefferenceFakturPajakDate = e.RefferenceFakturPajakDate,
                           DueDate = e.DueDate,
                           SalesModelCodeOld = e.SalesModelCodeOld,
                           SalesModelYear = e.SalesModelYear,
                           SalesModelCodeNew = e.SalesModelCodeNew,
                           ChassisCode = e.ChassisCode,
                           Quantity = e.Quantity,
                           DPPMaterial = e.DPPMaterial,
                           DPPFee = e.DPPFee,
                           DPPOthers = e.DPPOthers,
                           PPn = e.PPn,
                           Total = e.Total,
                           PPh = e.PPh,
                           Status = e.Status == "0" ? "OPEN" : e.Status == "1" ? "PRINTED"
                           : e.Status == "2" ? "APPROVED"
                           : e.Status == "3" ? "CANCELED"
                           : e.Status == "9" ? "FINISHED" : "",
                           Stat = e.Status
                       };

            return data;
        }

        [HttpGet]
        public IQueryable<KaroseriView> KaroseriSPKLookup()
        {
            var Uid = CurrentUser();

            var data = from e in ctx.Context.omTrPurchaseKaroseri
                       where e.CompanyCode == Uid.CompanyCode &&
                       e.BranchCode == Uid.BranchCode && e.Status == "2"
                       select new KaroseriView
                       {
                           KaroseriSPKNo = e.KaroseriSPKNo,
                           KaroseriSPKDate = e.KaroseriSPKDate,
                           SupplierCode = e.SupplierCode,
                           SalesModelCodeOld = e.SalesModelCodeOld,
                           SalesModelYear = e.SalesModelYear,
                           SalesModelCodeNew = e.SalesModelCodeNew,
                           ChassisCode = e.ChassisCode,
                           Quantity = e.Quantity,
                           DPPMaterial = e.DPPMaterial,
                           DPPFee = e.DPPFee,
                           DPPOthers = e.DPPOthers,
                           PPn = e.PPn,
                           Total = e.Total
                       };

            return data;
        }

        [HttpGet]
        public IQueryable<KaroseriTerimaDetailView> KaroseriTerimaChassisCodeLookup(string KaroseriSPKNo)
        {
            var uid = CurrentUser();
            var query = String.Format(@"
            SELECT a.ChassisNo, a.ChassisCode, a.EngineNo, a.EngineCode
		    , a.ColourCodeOld, b.RefferenceDesc1 as ColourNameOld
		    , a.ColourCodeNew, c.RefferenceDesc1 as ColourNameNew
            FROM omTrPurchaseKaroseriDetail a
			INNER JOIN omMstRefference b
				ON a.ColourCodeOld = b.RefferenceCode AND b.RefferenceType = 'COLO'
			INNER JOIN omMstRefference c
				ON a.ColourCodeNew = c.RefferenceCode AND c.RefferenceType = 'COLO'
            WHERE a.isKaroseriTerima <> 1
            AND a.CompanyCode = '{0}'           
            AND a.KaroseriSPKNo = '{1}'
            order by a.ChassisCode,a.ChassisNo asc", uid.CompanyCode, KaroseriSPKNo);

            var queryable = ctx.Context.Database.SqlQuery<KaroseriTerimaDetailView>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<KaroseriTerimaDetailView> KaroseriTerimaColourCodeLookup(string SalesModelCodeOld)
        {
            var uid = CurrentUser();
            var query = String.Format(@"
            SELECT a.ColourCode as ColourCodeNew, (SELECT b.RefferenceDesc1
            FROM omMstRefference b
            WHERE b.RefferenceCode = a.ColourCode
            AND b.CompanyCode = a.CompanyCode AND b.RefferenceType = 'COLO')  AS ColourNameNew, a.Remark
            FROM omMstModelColour a
            WHERE a.CompanyCode = '{0}' 
            AND a.SalesModelCode = '{1}'
            order by a.ColourCode asc", uid.CompanyCode, SalesModelCodeOld);

            var queryable = ctx.Context.Database.SqlQuery<KaroseriTerimaDetailView>(query).AsQueryable();
            return (queryable);
        }

        #endregion

        #region Permohonan Faktur Polis

        [HttpGet]
        public IQueryable<SalesReqView> MntnPermohonanBrowse()
        {
            var Uid = CurrentUser();

            var data = from e in ctx.Context.omTrSalesReq
                       join f in ctx.Context.GnMstCustomer
                        on e.SubDealerCode equals f.CustomerCode
                       where e.CompanyCode == Uid.CompanyCode &&
                       e.BranchCode == Uid.BranchCode && e.Status == "2"
                       select new SalesReqView
                       {
                           ReqNo = e.ReqNo,
                           ReqDate = e.ReqDate,
                           ReffNo = e.ReffNo,
                           ReffDate = e.ReffDate,
                           StatusFaktur = e.StatusFaktur,
                           Faktur = e.StatusFaktur == "1" ? "Ya" : "No",
                           SubDealerCode = e.SubDealerCode,
                           CustomerName = f.CustomerName,
                           Status = e.Status == "0" ? "OPEN" : e.Status == "1" ? "PRINTED"
                           : e.Status == "2" ? "APPROVED"
                           : e.Status == "3" ? "CANCELED"
                           : e.Status == "9" ? "FINISHED" : "",
                           Stat = e.Status,
                           isCBU = e.isCBU
                       };

            return data;
        }

        [HttpGet]
        public IQueryable<SalesReqView> PermohonanBrowse()
        {
            var Uid = CurrentUser();

            var data = from e in ctx.Context.omTrSalesReq
                       join f in ctx.Context.GnMstCustomer
                        on e.SubDealerCode equals f.CustomerCode
                       where e.CompanyCode == Uid.CompanyCode &&
                       e.BranchCode == Uid.BranchCode
                       select new SalesReqView
                       {
                           ReqNo = e.ReqNo,
                           ReqDate = e.ReqDate,
                           ReffNo = e.ReffNo,
                           ReffDate = e.ReffDate,
                           StatusFaktur = e.StatusFaktur,
                           Faktur = e.StatusFaktur == "1" ? "Ya" : "No",
                           SubDealerCode = e.SubDealerCode,
                           CustomerName = f.CustomerName,
                           Status = e.Status == "0" ? "OPEN" : e.Status == "1" ? "PRINTED"
                           : e.Status == "2" ? "APPROVED"
                           : e.Status == "3" ? "CANCELED"
                           : e.Status == "9" ? "FINISHED" : "",
                           Stat = e.Status,
                           isCBU = e.isCBU,
                           Remark = e.Remark
                       };

            return data;
        }

        [HttpGet]
        public IQueryable<SubDealerLookup> SubDealerLookup()
        {
            var Uid = CurrentUserInfo();
            var queryable = ctx.Context.Database.SqlQuery<SubDealerLookup>("uspfn_OmInquiryPenjualFP '" + Uid.CompanyCode + "','" + Uid.BranchCode + "','" + Uid.ProfitCenter + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<ChassisNoSP> ChassisNoSP(string CustomerCode, string isCBU)
        {
            var Uid = CurrentUserInfo();
            //string DBMD="";
            //bool independent = false;
            bool otom = true;
            string rcd = ctx.Context.Database.SqlQuery<string>("SELECT ParaValue from gnMstLookUpDtl WHERE CodeID='OTOM' AND LookUpValue='UNIT' AND CompanyCode='" + Uid.CompanyCode + "'").FirstOrDefault();
            if (rcd != null && rcd == "0")
            {
                otom = false;
            }

            var DBMD = ctx.Context.Database.SqlQuery<string>("SELECT dbmd FROM gnMstCompanyMapping WHERE companycode='" + Uid.CompanyCode + "' AND branchcode='" + Uid.BranchCode + "'").FirstOrDefault();
            ctx.Context.Database.CommandTimeout = 180;
            var queryable = ctx.Context.Database.SqlQuery<ChassisNoSP>("uspfn_OmInquiryChassisReq '" + Uid.CompanyCode + "','" + Uid.BranchCode + "','" + CustomerCode + "','" + isCBU + "'").AsQueryable();

            //if ((Uid.CompanyCode == CompanyMD) && (Uid.BranchCode == BranchMD))
            //{
            //    independent = true;
            //}


            //if queryable.Where(a => a.SODate == )

            //if (queryable.Count() == 0)
            if (DBMD != null && otom)
            {
                //queryable = ctxMD.Context.Database.SqlQuery<ChassisNoSP>("uspfn_OmInquiryChassisReq '" + CompanyMD + "','" + BranchMD + "','" + CustomerCode + "','" + isCBU + "'").AsQueryable();
                
                queryable = ctx.Context.Database.SqlQuery<ChassisNoSP>("uspfn_OmInquiryChassisReqMD '" + Uid.CompanyCode + "','" + Uid.BranchCode + "','" + CustomerCode + "','" + isCBU + "'").AsQueryable();

                //if (queryable.Count() == 0 || independent)
                //{
                //    queryable = ctx.Context.Database.SqlQuery<ChassisNoSP>("uspfn_OmInquiryChassisReq '" + Uid.CompanyCode + "','" + Uid.BranchCode + "','" + CustomerCode + "','" + isCBU + "'").AsQueryable();
                //}
            }
            return (queryable);
        }

        [HttpGet]
        public IQueryable<NoFakturPolis> NoFakturPolis(string ChassisCode, string ChassisNo)
        {
            var uid = CurrentUser();
            var query = String.Format(@"
                SELECT a.FakturPolisiNo, CASE
                              WHEN a.IsBlanko = 1 THEN 'Blanko'
                              WHEN a.IsBlanko = 0 THEN 'non-Blanko'
                           END  as statusBlanko 
                  FROM OmTrSalesFakturPolisi a
                 WHERE a.CompanyCode = '{0}'
                       AND a.ChassisCode = '{1}'
                       AND a.ChassisNo = '{2}'
            ", uid.CompanyCode, ChassisCode, ChassisNo);

            var queryable = ctx.Context.Database.SqlQuery<NoFakturPolis>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<WHview> Revision() 
        {
            var uid = CurrentUser();
            var query = String.Format(@" 
            SELECT LookUpValue , LookUpValueName , SeqNo
                FROM gnMstLookUpDtl
                WHERE CompanyCode = '{0}'
                    AND CodeID = 'REVI'
                ORDER BY SeqNo", uid.CompanyCode);

            var queryable = ctx.Context.Database.SqlQuery<WHview>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SalesFPolRevisionDetailView> RevisiPermohonanBrowse() 
        {
            var Uid = CurrentUser();

            var data = from e in ctx.Context.omTrSalesFPolRevision
                       join f in ctx.Context.GnMstCustomer
                        on e.SubDealerCode equals f.CustomerCode
                        into a
                       from b in a.DefaultIfEmpty()
                       where e.CompanyCode == Uid.CompanyCode &&
                       e.BranchCode == Uid.BranchCode
                       select new SalesFPolRevisionDetailView
                       {
                           RevisionNo = e.RevisionNo,
                           RevisionDate = e.RevisionDate,
                           SubDealerCode = e.SubDealerCode,
                           CustomerName = b.CustomerName,
                           Status = e.Status == "0" ? "OPEN" : e.Status == "1" ? "PRINTED"
                           : e.Status == "2" ? "APPROVED"
                           : e.Status == "3" ? "CANCELED"
                           : e.Status == "9" ? "FINISHED" : "",
                           Stat = e.Status,
                           ChassisCode = e.ChassisCode,
                           ChassisNo = e.ChassisNo,
                           FakturPolisiName = e.FakturPolisiName,
                           FakturPolisiAddress1 = e.FakturPolisiAddress1,
                           FakturPolisiAddress2 = e.FakturPolisiAddress2,
                           FakturPolisiAddress3 = e.FakturPolisiAddress3,
                           PostalCode = e.PostalCode,
                           PostalCodeDesc = e.PostalCodeDesc,
                           FakturPolisiCity = e.FakturPolisiCity,
                           FakturPolisiCityName = (ctx.Context.LookUpDtls.Where(x => x.CodeID == "CITY" && x.LookUpValue == e.FakturPolisiCity).FirstOrDefault().LookUpValueName),
                           FakturPolisiTelp1 = e.FakturPolisiTelp1,
                           FakturPolisiTelp2 = e.FakturPolisiTelp2,
                           FakturPolisiHP = e.FakturPolisiHP,
                           FakturPolisiBirthday = e.FakturPolisiBirthday,
                           IsCityTransport = e.IsCityTransport,
                           FakturPolisiNo = e.FakturPolisiNo,
                           FakturPolisiDate = e.FakturPolisiDate,
                           FakturPolisiArea = e.FakturPolisiArea,
                           IDNo = e.IDNo,
                           RevisionCode = e.RevisionCode,
                           RevisionName = (ctx.Context.LookUpDtls.Where(x => x.CodeID == "REVI" && x.LookUpValue == e.RevisionCode).FirstOrDefault().LookUpValueName)
                       };

            return data;
        }

        [HttpGet]
        public IQueryable<SalesFPolRevisionDetailView> ChassisCode4Rev()
        {
            var Uid = CurrentUser();
            //var VinCD = ctx.Context.omTrSalesFPolRevision.Select(x => (x.ChassisCode + x.ChassisNo)).ToList();
            var data = from e in ctx.Context.omTrSalesReq
                       join g in ctx.Context.omTrSalesReqDetail
                        on e.ReqNo equals g.ReqNo
                       join f in ctx.Context.GnMstCustomer
                        on e.SubDealerCode equals f.CustomerCode
                       where e.CompanyCode == Uid.CompanyCode &&
                       e.BranchCode == Uid.BranchCode //&& (!VinCD.Contains(g.ChassisCode + g.ChassisNo)) 
                       select new SalesFPolRevisionDetailView
                       {
                           ReqNo = e.ReqNo,
                           ReqDate = e.ReqDate,
                           ReffNo = e.ReffNo,
                           ReffDate = e.ReffDate,
                           StatusFaktur = e.StatusFaktur,
                           Faktur = e.StatusFaktur == "1" ? "Ya" : "No",
                           SubDealerCode = e.SubDealerCode,
                           CustomerName = f.CustomerName,
                           Status = e.Status == "0" ? "OPEN" : e.Status == "1" ? "PRINTED"
                           : e.Status == "2" ? "APPROVED"
                           : e.Status == "3" ? "CANCELED"
                           : e.Status == "9" ? "FINISHED" : "",
                           Stat = e.Status,
                           isCBU = e.isCBU,
                           Remark = e.Remark,
                           ChassisCode = g.ChassisCode,
                           ChassisNo = g.ChassisNo,
                           FakturPolisiName = g.FakturPolisiName,
                           FakturPolisiAddress1 = g.FakturPolisiAddress1,
                           FakturPolisiAddress2 = g.FakturPolisiAddress2,
                           FakturPolisiAddress3 = g.FakturPolisiAddress3,
                           PostalCode = g.PostalCode,
                           PostalCodeDesc = g.PostalCodeDesc,
                           FakturPolisiCity = g.FakturPolisiCity,
                           FakturPolisiCityName = (ctx.Context.LookUpDtls.Where(x => x.CodeID == "CITY" && x.LookUpValue == g.FakturPolisiCity).FirstOrDefault().LookUpValueName),
                           FakturPolisiTelp1 = g.FakturPolisiTelp1,
                           FakturPolisiTelp2 = g.FakturPolisiTelp2,
                           FakturPolisiHP = g.FakturPolisiHP,
                           FakturPolisiBirthday = g.FakturPolisiBirthday,
                           IsCityTransport = g.IsCityTransport,
                           FakturPolisiNo = g.FakturPolisiNo,
                           FakturPolisiDate = g.FakturPolisiDate,
                           FakturPolisiArea = g.FakturPolisiArea,
                           IDNo = g.IDNo
                       };
            return data;
        }
        #endregion

        [HttpGet]
        public IQueryable<GnMstZipCodeView> ZipCodeLookUp()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.GnMstZipCode
                            where p.CompanyCode == Uid.CompanyCode
                            select new GnMstZipCodeView()
                            {
                                ZipCode = p.ZipCode,
                                KelurahanDesa = p.KelurahanDesa,
                                KecamatanDistrik = p.KecamatanDistrik,
                                KotaKabupaten = p.KotaKabupaten,
                                IbuKota = p.IbuKota,
                                isCity = p.isCity

                            };
            return (queryable);
        }

        #region Faktur Polisi

        [HttpGet]
        public IQueryable<FakturPolisiBrowse> FakturPolisiBrowse()
        {
            var Uid = CurrentUser();
            var data = ctx.Context.OmTrSalesFakturPolisi.Where(a => a.CompanyCode == Uid.CompanyCode && a.BranchCode == Uid.BranchCode)
                .Select(e => new FakturPolisiBrowse
                {
                    FakturPolisiNo = e.FakturPolisiNo,
                    FakturPolisiDate = e.FakturPolisiDate,
                    ChassisCode = e.ChassisCode,
                    ChassisNo = e.ChassisNo,
                    ReqNo = e.ReqNo,
                    Status = e.Status == "0" ? "OPEN"
                        : e.Status == "2" ? "APPROVED"
                        : e.Status == "3" ? "CANCELED"
                        : "",
                    Stat = e.Status,
                    IsBlanko = e.IsBlanko
                });

            return data;
        }

        [HttpGet]
        public IQueryable<ChassisBrowse> ChassisBrowse(string ChassisCode, int isChassisCode)
        {
            var Uid = CurrentUser();
            object[] parameters = { Uid.CompanyCode, ChassisCode ?? "", 0, isChassisCode };
            var query = "exec uspfn_OmInquiryChassisFP @p0,@p1,@p2,@p3";

            var data = ctx.Context.Database.SqlQuery<ChassisBrowse>(query, parameters).AsQueryable();

            return data;
        }
        #endregion

        #region Tanda Terima BPKB

        [HttpGet]
        public IQueryable<BPKBBrowse> BPKBBrowse()
        {
            var Uid = CurrentUser();
            var query = String.Format(@"
                SELECT DocNo, DocDate, BPKBOutBy, BPKBOutType, Remark, (CASE ISNULL(BPKBOutType, 0) WHEN '0' THEN (SELECT CustomerName FROM GnMstCustomer WHERE CompanyCode = CompanyCode AND CustomerCode = BPKBOutBy)
                WHEN '1' THEN (SELECT RefferenceDesc1 FROM OmMstRefference WHERE CompanyCode = CompanyCode AND RefferenceType = 'WARE' AND RefferenceCode = BPKBOutBy)
                WHEN '2' THEN (SELECT CustomerName FROM GnMstCustomer WHERE CompanyCode = CompanyCode AND CustomerCode = BPKBOutBy) END) AS BPKBOutByName,
		        CASE BPKBOutType WHEN '0' THEN 'Leasing'
					     WHEN '1' THEN 'Cabang'
						 WHEN '2' THEN 'Pelanggan'
		        END as BPKBOutTypeDes, Status,
		        CASE Status WHEN '0' THEN 'OPEN'
                    WHEN '1' THEN 'PRINTED'
					WHEN '2' THEN 'APPROVED'
					WHEN '3' THEN 'CENCELED'
		        END as Stat
                FROM omTrSalesBPKB
                WHERE 1 = 1
                   AND CompanyCode = '{0}'
                   AND BranchCode = '{1}'
            ", Uid.CompanyCode, Uid.BranchCode);

            var queryable = ctx.Context.Database.SqlQuery<BPKBBrowse>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<BPKBOut> BPKBOutLookup(string Type)
        {
            var uid = CurrentUser();
            var query = String.Format(@"
                SELECT DISTINCT a.BPKBOutBy, 
                (CASE ISNULL(a.BPKBOutType, 0) WHEN '0' THEN (SELECT CustomerName FROM GnMstCustomer WHERE CompanyCode = a.CompanyCode AND CustomerCode = a.BPKBOutBy)
                WHEN '1' THEN (SELECT RefferenceDesc1 FROM OmMstRefference WHERE CompanyCode = a.CompanyCode AND RefferenceType = 'WARE' AND RefferenceCode = a.BPKBOutBy)
                WHEN '2' THEN (SELECT CustomerName FROM GnMstCustomer WHERE CompanyCode = a.CompanyCode AND CustomerCode = a.BPKBOutBy) END) AS BPKBOutByName
                FROM OmTrSalesSPKSubDetail a
                 WHERE 1 = 1
                   AND a.CompanyCode = '{0}'
                   AND a.BranchCode = '{1}'
                   AND a.BPKBOutType = '{2}'
            ", uid.CompanyCode, uid.BranchCode, Type);

            var queryable = ctx.Context.Database.SqlQuery<BPKBOut>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SalesBPKBDetailView> ChassisCode4BPKB(string BPKBOutType, string BPKBOutBy)
        {
            var uid = CurrentUser();
            var query = String.Format(@"
                SELECT a.ChassisCode, a.ChassisNo, d.CustomerCode, e.CustomerName, c.EngineCode, c.EngineNo, a.FakturPolisiNo, c.SalesModelCode, 
                c.ColourCode, a.BPKBNo, a.PoliceRegistrationNo, a.PoliceRegistrationDate, a.Remark
                    FROM OmTrSalesSPKDetail a
					INNER JOIN OmTrSalesSPKSubDetail b
					ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode
					AND a.SPKNo = b.SPKNo AND a.ChassisCode = b.ChassisCode AND a.ChassisNo = b.ChassisNo
                    inner join omTrSalesSOVin c on a.CompanyCode=c.CompanyCode and a.ChassisCode=c.ChassisCode
		                and a.ChassisNo=c.ChassisNo
	                inner join omTrSalesSO d on c.CompanyCode=d.CompanyCode and  c.BranchCode=d.BranchCode
		                and c.SONo=d.SONo
                    inner join gnMstCustomer e on d.CustomerCode=e.CustomerCode and d.CompanyCode=e.CompanyCode
                    WHERE 1 = 1
                    AND a.CompanyCode = '{0}'
                    AND a.BranchCode = '{1}'
                    AND a.BPKBNo <> ''
                    AND a.BPKBNo IS NOT NULL
                    AND a.BPKBInDate <> '19000101'
                    --AND (a.BPKBNo is not null OR a.BPKBNo != '')
                    --AND (a.BPKBInDate is not null OR a.BPKBInDate != '19000101')
                    AND not exists (SELECT 1 
                                        FROM OmTrSalesBPKB x
                                        INNER JOIN OmTrSalesBPKBDetail y
                                            ON x.CompanyCode = y.CompanyCode 
                                                AND x.BranchCode = y.BranchCode
                                                AND x.DocNo = y.DocNo
                                                AND a.ChassisCode = y.ChassisCode
                                                AND a.ChassisNo = y.ChassisNo
                                        WHERE x.BPKBOutType = '{2}'
                                            AND x.BPKBOutBy = '{3}')
					AND b.BPKBOutType = '{2}'
                    AND b.BPKBOutBy = '{3}'
            ", uid.CompanyCode, uid.BranchCode, BPKBOutType, BPKBOutBy);

            var queryable = ctx.Context.Database.SqlQuery<SalesBPKBDetailView>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SalesBPKBDetailView>ChassisNo4BPKB(string ChassisCode, string BPKBOutType, string BPKBOutBy)
        {
            var uid = CurrentUser();
//            var query = String.Format(@"
//                SELECT a.ChassisCode, a.ChassisNo, a.FakturPolisiNo, a.PoliceRegistrationNo, a.PoliceRegistrationDate, a.BPKBNo, 
//		                c.EngineCode, c.EngineNo, c.SalesModelCode, c.ColourCode, d.CustomerCode, e.CustomerGovName as CustomerName
//                FROM OmTrSalesSPKDetail a
//                INNER JOIN OmTrSalesSPKSubDetail b
//					                ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode
//					                AND a.SPKNo = b.SPKNo AND a.ChassisCode = b.ChassisCode AND a.ChassisNo = b.ChassisNo
//                --INNER JOIN omMstVehicle c
//	            --    ON a.CompanyCode = c.CompanyCode 
//	            --    AND a.ChassisCode = c.ChassisCode and a.ChassisNo = c.ChassisNo
//                INNER JOIN omTrSalesSO d
//	                ON c.CompanyCode = d.CompanyCode and c.SONo = d.SONo
//                INNER JOIN gnMstCustomer e
//	                ON e.CompanyCode = d.CompanyCode AND e.CustomerCode = d.CustomerCode
//                    WHERE 1 = 1
//                    AND a.CompanyCode = '{0}'
//                    AND a.BranchCode = '{1}'
//                    AND a.ChassisCode = '{2}'
//                    --AND a.BPKBNo <> ''
//                    --AND a.BPKBNo IS NOT NULL
//                    --AND a.BPKBInDate <> '19000101'
//                    AND (a.BPKBNo is not null OR a.BPKBNo != '')
//                    AND (a.BPKBInDate is not null OR a.BPKBInDate != '19000101')
//                    AND not exists (SELECT 1 
//                                        FROM OmTrSalesBPKB x
//                                        INNER JOIN OmTrSalesBPKBDetail y
//                                            ON x.CompanyCode = y.CompanyCode 
//                                                AND x.BranchCode = y.BranchCode
//                                                AND x.DocNo = y.DocNo
//                                                AND a.ChassisCode = y.ChassisCode
//                                                AND a.ChassisNo = y.ChassisNo
//                                        WHERE x.BPKBOutType = '{3}'
//                                            AND x.BPKBOutBy = '{4}')
//                    AND b.BPKBOutType = '{3}'
//                    AND b.BPKBOutBy = '{4}'
//            ", uid.CompanyCode, uid.BranchCode, ChassisCode, BPKBOutType, BPKBOutBy);
            var query = String.Format(@"
                SELECT a.ChassisCode, a.ChassisNo, d.CustomerCode, e.CustomerName, c.EngineCode, c.EngineNo, a.FakturPolisiNo, c.SalesModelCode, 
                c.ColourCode, a.BPKBNo, a.PoliceRegistrationNo, a.PoliceRegistrationDate, a.Remark
                    FROM OmTrSalesSPKDetail a
					INNER JOIN OmTrSalesSPKSubDetail b
					ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode
					AND a.SPKNo = b.SPKNo AND a.ChassisCode = b.ChassisCode AND a.ChassisNo = b.ChassisNo
                    inner join omTrSalesSOVin c on a.CompanyCode=c.CompanyCode and a.ChassisCode=c.ChassisCode
		                and a.ChassisNo=c.ChassisNo
	                inner join omTrSalesSO d on c.CompanyCode=d.CompanyCode and  c.BranchCode=d.BranchCode
		                and c.SONo=d.SONo
                    inner join gnMstCustomer e on d.CustomerCode=e.CustomerCode and d.CompanyCode=e.CompanyCode                        
                    WHERE 1 = 1
                    AND a.CompanyCode = '{0}'
                    AND a.BranchCode = '{1}'
                    AND a.ChassisCode = '{2}'
                    AND a.BPKBNo <> ''
                    AND a.BPKBNo IS NOT NULL
                    AND a.BPKBInDate <> '19000101'
                    --AND (a.BPKBNo is not null OR a.BPKBNo != '')
                    --AND (a.BPKBInDate is not null OR a.BPKBInDate != '19000101')
                    AND not exists (SELECT 1 
                                        FROM OmTrSalesBPKB x
                                        INNER JOIN OmTrSalesBPKBDetail y
                                            ON x.CompanyCode = y.CompanyCode 
                                                AND x.BranchCode = y.BranchCode
                                                AND x.DocNo = y.DocNo
                                                AND a.ChassisCode = y.ChassisCode
                                                AND a.ChassisNo = y.ChassisNo
                                        WHERE x.BPKBOutType = '{3}'
                                            AND x.BPKBOutBy = '{4}')
                    AND b.BPKBOutType = '{3}'
                    AND b.BPKBOutBy = '{4}'
            ", uid.CompanyCode, uid.BranchCode, ChassisCode, BPKBOutType, BPKBOutBy);
            var queryable = ctx.Context.Database.SqlQuery<SalesBPKBDetailView>(query).AsQueryable();
            return (queryable);
        }

        #endregion

        #region Report Inventory

        [HttpGet]
        public IQueryable<ReportInvetoryLookUp> CodeBrowse(string baseOn)
        {
            var Uid = CurrentUser();
            var data =
                baseOn == "Gudang" ?
                ctx.Context.MstRefferences.Where(a => a.CompanyCode == Uid.CompanyCode && a.RefferenceType.Equals("WARE") && a.Status == "1").Select
                (a => new ReportInvetoryLookUp
                {
                    Code = a.RefferenceCode,
                    Desc = a.RefferenceDesc1
                }) :
                ctx.Context.OmMstModels.Where(a => a.CompanyCode == Uid.CompanyCode && a.Status != "0").Select(a => new ReportInvetoryLookUp
                {
                    Code = a.SalesModelCode,
                    Desc = a.SalesModelDesc
                });

            return data;
        }

        [HttpGet]
        public IQueryable<ReportInvetoryLookUp> ColourBrowse()
        {
            var Uid = CurrentUser();
            var query = string.Format(@"
            SELECT distinct a.ColourCode as Code, (SELECT b.RefferenceDesc1
                   FROM omMstRefference b
                   WHERE b.RefferenceCode = a.ColourCode
                   AND b.CompanyCode = a.CompanyCode AND b.RefferenceType = 'COLO') 'Desc'
              FROM omMstModelColour a  
             WHERE a.CompanyCode = '{0}'", Uid.CompanyCode);

            var data = ctx.Context.Database.SqlQuery<ReportInvetoryLookUp>(query).AsQueryable();

            return data;
        }

        [HttpGet]
        public IQueryable<ReportInvetoryLookUp> BranchBrowse()
        {
            var Uid = CurrentUser();
            var data = ctx.Context.CoProfiles.Select(a => new ReportInvetoryLookUp
            {
                Code = a.BranchCode,
                Desc = a.CompanyName
            });

            return data;

        }

        #endregion

        #region Report Sales

        [HttpGet]
        public IQueryable<SOView> SOLookup4Report(string Status)
        {
            var parameters = Status.Split('?');
            var status = parameters[0];
            var Uid = CurrentUser();
            if (status != "-1")
            {
                var queryable = from p in ctx.Context.OmTRSalesSOs
                                where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode && p.Status == status
                                select new SOView()
                                {
                                    SONo = p.SONo,
                                    GroupPriceCode = p.GroupPriceCode,
                                    SODate = p.SODate,
                                    Status = p.Status == "0" ? "Open"
                                       : p.Status == "1" ? "Printed"
                                       : p.Status == "2" ? "Approved"
                                       : p.Status == "3" ? "Canceled"
                                       : p.Status == "9" ? "Finished" : "",
                                };
                return (queryable);
            }
            else
            {
                var queryable = from p in ctx.Context.OmTRSalesSOs
                                where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                                select new SOView()
                                {
                                    SONo = p.SONo,
                                    GroupPriceCode = p.GroupPriceCode,
                                    SODate = p.SODate,
                                    Status = p.Status == "0" ? "Open"
                                       : p.Status == "1" ? "Printed"
                                       : p.Status == "2" ? "Approved"
                                       : p.Status == "3" ? "Canceled"
                                       : p.Status == "9" ? "Finished" : "",
                                };
                return (queryable);
            }
        }

        [HttpGet]
        public IQueryable<DOView> DOLookup4Report(string Status)
        {
            var parameters = Status.Split('?');
            var status = parameters[0];
            var Uid = CurrentUser();
            if (status != "-1")
            {
                var queryable = from p in ctx.Context.OmTrSalesDOs
                                join q in ctx.Context.OmTRSalesSOs
                                    on p.SONo equals q.SONo
                                where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode && p.Status == status
                                select new DOView()
                                {
                                    DONo = p.DONo,
                                    GroupPriceCode = q.GroupPriceCode,
                                    DODate = p.DODate,
                                    Status = p.Status == "0" ? "Open"
                                       : p.Status == "1" ? "Printed"
                                       : p.Status == "2" ? "Approved"
                                       : p.Status == "3" ? "Canceled"
                                       : p.Status == "9" ? "Finished" : "",
                                };
                return (queryable);
            }
            else
            {
                var queryable = from p in ctx.Context.OmTrSalesDOs
                                join q in ctx.Context.OmTRSalesSOs
                                    on p.SONo equals q.SONo
                                where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                                select new DOView()
                                {
                                    DONo = p.DONo,
                                    GroupPriceCode = q.GroupPriceCode,
                                    DODate = p.DODate,
                                    Status = p.Status == "0" ? "Open"
                                       : p.Status == "1" ? "Printed"
                                       : p.Status == "2" ? "Approved"
                                       : p.Status == "3" ? "Canceled"
                                       : p.Status == "9" ? "Finished" : "",
                                };
                return (queryable);
            }
        }

        [HttpGet]
        public IQueryable<BPKView> BPKLookup4Report(string Status)
        {
            var parameters = Status.Split('?');
            var status = parameters[0];
            var Uid = CurrentUser();
            if (status != "-1")
            {
                var queryable = from p in ctx.Context.OmTrSalesBPKs
                                join q in ctx.Context.OmTRSalesSOs
                                    on p.SONo equals q.SONo
                                where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode && p.Status == status
                                select new BPKView()
                                {
                                    BPKNo = p.BPKNo,
                                    GroupPriceCode = q.GroupPriceCode,
                                    BPKDate = p.BPKDate,
                                    Status = p.Status == "0" ? "Open"
                                       : p.Status == "1" ? "Printed"
                                       : p.Status == "2" ? "Approved"
                                       : p.Status == "3" ? "Canceled"
                                       : p.Status == "9" ? "Finished" : "",
                                };
                return (queryable);
            }
            else
            {
                var queryable = from p in ctx.Context.OmTrSalesBPKs
                                join q in ctx.Context.OmTRSalesSOs
                                    on p.SONo equals q.SONo
                                where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                                select new BPKView()
                                {
                                    BPKNo = p.BPKNo,
                                    GroupPriceCode = q.GroupPriceCode,
                                    BPKDate = p.BPKDate,
                                    Status = p.Status == "0" ? "Open"
                                       : p.Status == "1" ? "Printed"
                                       : p.Status == "2" ? "Approved"
                                       : p.Status == "3" ? "Canceled"
                                       : p.Status == "9" ? "Finished" : "",
                                };
                return (queryable);
            }
        }

        [HttpGet]
        public IQueryable<InvoiceView> SalesInvoiceLookup4Report(string Status)
        {
            var parameters = Status.Split('?');
            var status = parameters[0];
            var Uid = CurrentUser();
            if (status != "-1")
            {
                var queryable = from p in ctx.Context.OmTrSalesInvoices
                                join q in ctx.Context.OmTRSalesSOs
                                    on p.SONo equals q.SONo
                                where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode && p.Status == status
                                select new InvoiceView()
                                {
                                    InvoiceNo = p.InvoiceNo,
                                    GroupPriceCode = q.GroupPriceCode,
                                    InvoiceDate = p.InvoiceDate,
                                    Status = p.Status == "0" ? "Open"
                                       : p.Status == "1" ? "Printed"
                                       : p.Status == "2" ? "Approved"
                                       : p.Status == "3" ? "Canceled"
                                       : p.Status == "5" ? "Posted"
                                       : p.Status == "9" ? "Finished" : "",
                                };
                return (queryable);
            }
            else
            {
                var queryable = from p in ctx.Context.OmTrSalesInvoices
                                join q in ctx.Context.OmTRSalesSOs
                                    on p.SONo equals q.SONo
                                where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                                select new InvoiceView()
                                {
                                    InvoiceNo = p.InvoiceNo,
                                    GroupPriceCode = q.GroupPriceCode,
                                    InvoiceDate = p.InvoiceDate,
                                    Status = p.Status == "0" ? "Open"
                                       : p.Status == "1" ? "Printed"
                                       : p.Status == "2" ? "Approved"
                                       : p.Status == "3" ? "Canceled"
                                       : p.Status == "5" ? "Posted"
                                       : p.Status == "9" ? "Finished" : "",
                                };
                return (queryable);
            }
        }

        [HttpGet]
        public IQueryable<MstLookUpDtlView> GroupARLookup()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.LookUpDtls
                            where p.CompanyCode == Uid.CompanyCode && p.CodeID == "GPAR"
                            select new MstLookUpDtlView()
                            {
                                ParaValue = p.ParaValue,
                                LookUpValue = p.LookUpValue,
                                LookUpValueName = p.LookUpValueName
                            };
            return (queryable);
        }

        //add by fir 01/08/2015
        [HttpGet]
        public IQueryable<gnMstPeriode> PenjualanTerbaikLookup4Report(string OptionByYear, string Year)
        {
            var parameter = OptionByYear.Split('?');
            var optionByYear = parameter[0];
            var Uid = CurrentUser();
            if (optionByYear == "0")
            {
                var queryable = ctx.Context.Database.SqlQuery<gnMstPeriode>("SELECT  CONVERT(varchar, FiscalYear) + RIGHT('00' + CONVERT(varchar, PeriodeNum), 2) AS Periode," +
                    "PeriodeNum, FiscalMonth, PeriodeName, FromDate, EndDate" +
                    " FROM gnMstPeriode WHERE CompanyCode = '" + Uid.CompanyCode + "' and branchCode='" + Uid.BranchCode + "' AND FiscalYear = '" + Year + "' ").AsQueryable();
                return (queryable);
            }
            else
            {
                var queryable = ctx.Context.Database.SqlQuery<gnMstPeriode>("SELECT  CONVERT(varchar, FiscalYear) + RIGHT('00' + CONVERT(varchar, PeriodeNum), 2) AS Periode," +
                    "PeriodeNum, FiscalMonth, PeriodeName, FromDate, EndDate" +
                    " FROM gnMstPeriode WHERE CompanyCode = '" + Uid.CompanyCode + "'  and branchCode='" + Uid.BranchCode + "' AND YEAR(FromDate) =   '" + Year + "' ").AsQueryable();
                return (queryable);
            }
        }

        [HttpGet]
        public IQueryable<CoProfile> BranchPenjualanTerbaikLookup4Report()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<CoProfile>("select * from gnMstCoProfile WHERE CompanyCode = '" + Uid.CompanyCode + "'  and branchCode='" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SalesReqView> FakturReqSudahTergenerate4Report()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.omTrSalesReq
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            select new SalesReqView
                            {
                                ReqNo = p.ReqNo,
                                ReqDate = p.ReqDate
                            }; return (queryable);
        }

        [HttpGet]
        public IQueryable<MtsModelView> SummaryPermohonanFakPolSM4Report()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<MtsModelView>("SELECT a.SalesModelCode, a.SalesModelDesc " +
                    " FROM OmMstModel a " +
                   " WHERE a.SalesModelCode IN  ( SELECT distinct a.SalesModelCode FROM OmMstVehicle a " +
                              " JOIN OmTrSalesReqDetail b  ON b.CompanyCode = a.CompanyCode AND b.ChassisNo= a.Chassisno  AND b.ChassisCode=a.ChassisCode )" +
                      "  AND a.CompanyCode = '" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SOView> SummaryPermohonanFakPolC4Report()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<SOView>("SELECT CustomerCode, CustomerName " +
                    " FROM gnMstCustomer " +
                    " WHERE  CustomerCode IN (SELECT DISTINCT SubDealerCode from OmTrSalesReq) AND CompanyCode = '" + Uid.CompanyCode + "' ").AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<SOView> RekapHarian4Report()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<SOView>("SELECT distinct a.CustomerCode, a.CustomerName " +
                            " FROM gnMstCustomer a with(nolock, nowait) " +
                            " INNER JOIN  GnMstCustomerProfitCenter b on a.CompanyCode = b.CompanyCode AND b.CustomerCode = a.CustomerCode " +
                            " LEFT JOIN GnMstLookupDtl c ON a.CompanyCode = c.CompanyCode AND b.ProfitCenterCode = c.LookUpValue AND CodeID = 'PFCN' " +
                            " WHERE a.CompanyCode='" + Uid.CompanyCode + "' AND b.BranchCode = '" + Uid.BranchCode + "' ").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<InvoiceView> FakturPnjPreprinted4Report()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<InvoiceView>("SELECT a.InvoiceNo, a.InvoiceDate, a.SONo" +
                        " FROM omTrSalesInvoice a " +
                        " LEFT JOIN gnMstCustomer b ON a.CompanyCode = b.CompanyCode AND a.CustomerCode = b.CustomerCode" +
                        " LEFT JOIN gnMstCustomerProfitCenter c ON a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode " +
                        " AND a.CustomerCode = c.CustomerCode AND c.ProfitCenterCode = '100' " +
                        " LEFT JOIN omTrSalesSO d ON a.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode AND a.SONo = d.SONo " +
                        " WHERE a.CompanyCode = '" + Uid.CompanyCode + "'  AND a.BranchCode= '" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<BPKBBrowse> TtdTerimaBpkbStrg4Report(string Storage, string DocNoStart, string DocNoTo)
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<BPKBBrowse>("select * from (SELECT distinct d.BPKBOutBy " +
                    ",case(d.BPKBOutType) when '0' then e.CustomerName when '2' then e.CustomerName else f.RefferenceDesc1 end as BPKBOutByName " +
                    " from OmTrSalesBpkbDetail a " +
                    " Inner join OmTrSalesBPKB d on d.CompanyCode = a.CompanyCode  and d.BranchCode = a.BranchCode  and d.DocNo = a.DocNo " +
                    " left join gnMstCustomer e on e.CompanyCode = a.CompanyCode and e.CustomerCode = d.BPKBOutBy " +
                    " left join omMstRefference f on f.CompanyCode = a.CompanyCode  and f.RefferenceType = 'WARE' and f.RefferenceCode = d.BPKBOutBy " +
                    " where a.CompanyCode = '" + Uid.CompanyCode + "' " +
                    " and a.BranchCode = '" + Uid.BranchCode + "' " +
                    " and d.BPKBOutType = '" + Storage + "' " +
                    " and (case when '" + DocNoStart + "' = '' then '' else d.DocNo end) between '" + DocNoStart + "' and '" + DocNoTo + "')a").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<BPKBBrowse> TtdTerimaBpkbDocNo4Report(string Storage, string BPKBCode)
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<BPKBBrowse>("SELECT DocNo,DocDate,BPKBOutType,BPKBOutBy,Remark " +
                  " FROM omTrSalesBPKB a " +
                 " WHERE a.CompanyCode = '" + Uid.CompanyCode + "' " +
                  " AND a.BranchCode = '" + Uid.BranchCode + "' AND a.BPKBOutType = '" + Storage + "' " +
                  " AND (CASE WHEN '" + BPKBCode + "' = '' THEN '' ELSE a.BPKBOutBy END) = '" + BPKBCode + "' ").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<OutLetLookup> DaftarBpkbPerlokasiBrnc4Report()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<OutLetLookup>("SELECT BranchCode, BranchName " +
                    " FROM gnMstOrganizationDtl " +
                    " WHERE CompanyCode = '" + Uid.CompanyCode + "' ").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SupplierView> DaftarTdTerimaFakturBnnSpl4Report()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<SupplierView>("SELECT *, row_number() over(order by supplierName) seqNo " +
                " FROM (SELECT DISTINCT(a.SupplierCode) , a.SupplierName " +
                " FROM gnMstSupplier a WITH(NOLOCK, NOWAIT) " +
                " LEFT JOIN gnMstSupplierProfitCenter b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.SupplierCode = b.SupplierCode " +
                " WHERE a.CompanyCode ='6114201' AND b.ProfitCenterCode = '100') a").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SupplierView> OutstandingSpkSpl4Report()
        {
            var Uid = CurrentUserInfo();
            var queryable = ctx.Context.Database.SqlQuery<SupplierView>("SELECT a.SupplierCode, a.SupplierName" +
                       " FROM gnMstSupplier a " +
                       " JOIN gnmstSupplierProfitCenter b ON a.CompanyCode= b.CompanyCode AND a.SupplierCode = b.SupplierCode " +
                       " WHERE a.CompanyCode='" + Uid.CompanyCode + "'" +
                       " AND b.BranchCode='" + Uid.BranchCode + "' AND b.isBlackList=0 AND a.status = 1 " +
                       "AND b.ProfitCenterCode='" + Uid.ProfitCenter + "' ").AsQueryable();
            return (queryable);
        }

        //end

        [HttpGet]
        public IQueryable<MstCustomerView> CustomerLookup4Report()
        {
            var Uid = CurrentUserInfo();
            var query = string.Format(@"
                SELECT a.CustomerCode, a.CustomerName, a.Address1, a.Address2, a.Address3, c.LookUpValueName AS ProfitCenter
                FROM gnMstCustomer a
                INNER JOIN  GnMstCustomerProfitCenter b on a.CompanyCode = b.CompanyCode AND b.CustomerCode = a.CustomerCode
                LEFT JOIN GnMstLookupDtl c ON a.CompanyCode = c.CompanyCode AND b.ProfitCenterCode = c.LookUpValue AND CodeID = 'PFCN'
                WHERE a.CompanyCode='{0}' AND b.BranchCode = '{1}' AND b.ProfitCenterCode = '{2}' 
                ", Uid.CompanyCode, Uid.BranchCode, Uid.ProfitCenter);

            var queryable = ctx.Context.Database.SqlQuery<MstCustomerView>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<CoProfileView> BranchLookup4Report()
        {

            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.CoProfiles
                            where p.CompanyCode == Uid.CompanyCode
                            select new CoProfileView()
                            {
                                BranchCode = p.BranchCode,
                                CompanyName = p.CompanyName
                            };
            return (queryable);
        }

        [HttpGet]
        public IQueryable<MstCustomerView> LeasingLookup4Report()
        {
            var Uid = CurrentUserInfo();
            var query = string.Format(@"
                SELECT a.CustomerCode, a.CustomerName
                  FROM gnMstCustomer a INNER JOIN gnMstCustomerProfitCenter b
                          ON a.CompanyCode = b.CompanyCode AND a.CustomerCode = b.CustomerCode
                 WHERE a.CompanyCode = '{0}' 
                    AND b.BranchCode = '{1}'
                    AND b.ProfitCenterCode = '{2}'
                    AND a.CategoryCode = 32 
                ", Uid.CompanyCode, Uid.BranchCode, Uid.ProfitCenter);

            var queryable = ctx.Context.Database.SqlQuery<MstCustomerView>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<PeriodView> PriodeLookup4Report(string FiscalYear)
        {
            var Uid = CurrentUserInfo();
            var query = string.Format(@"
                SELECT  CONVERT(varchar, FiscalYear) + RIGHT('00' + CONVERT(varchar, PeriodeNum), 2) AS Periode, PeriodeNum, FiscalMonth, PeriodeName, FromDate, EndDate
                FROM gnMstPeriode WHERE
                CompanyCode = '{0}'
                AND BranchCode = '{1}' AND FiscalYear = '{2}'
                and convert(int,convert(varchar,YEAR(EndDate),112)+ case when MONTH(EndDate) < 10 then '0' + convert(varchar,MONTH(EndDate),112) else  convert(varchar,MONTH(EndDate),112) end) <= (convert(int,convert(varchar,YEAR(getdate()),112)+ (case when MONTH(getdate()) < 10 then '0' + convert(varchar,MONTH(getdate()),112) else  convert(varchar,MONTH(getdate()),112) end)))
                ORDER BY FiscalYear ASC, FiscalMonth ASC, PeriodeNum ASC 
                ", Uid.CompanyCode, Uid.BranchCode, FiscalYear);

            var queryable = ctx.Context.Database.SqlQuery<PeriodView>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<FakturPolisiBrowse> FakturLookup4Report()
        {
            var Uid = CurrentUserInfo();
            var query = string.Format(@"
                SELECT FakturPolisiNo, FakturPolisiDate, IsBlanko, CASE Status
                  WHEN '0' THEN 'Open'
                  WHEN '1' THEN 'Printed'
                  WHEN '2' THEN 'Approved'
                  WHEN '3' THEN 'Deleted'
                END as Status FROM OmTrSalesFakturPolisi
                WHERE CompanyCode = '{0}'
                AND BranchCode = '{1}' 
                ", Uid.CompanyCode, Uid.BranchCode);

            var queryable = ctx.Context.Database.SqlQuery<FakturPolisiBrowse>(query).AsQueryable();
            return (queryable);
        }


        #endregion

        [HttpGet]
        public IQueryable<OutLetLookup> BranchToLookup()
        {
            var uid = CurrentUser();
            var query = String.Format(@" 
            SELECT BranchCode, BranchName
                FROM gnMstOrganizationDtl
                WHERE CompanyCode = '{0}'
                ORDER BY BranchCode", uid.CompanyCode);

            var queryable = ctx.Context.Database.SqlQuery<OutLetLookup>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<WHview> Select4WH(string BranchCode)
        {
            var gnMstLookUpDtl = ctx.Context.LookUpDtls.Find(user.CompanyCode, "OTOM", "UNIT");
            string codeID = "";
            if (gnMstLookUpDtl != null)
            {
                if (gnMstLookUpDtl.ParaValue == "0")
                {
                    codeID = "MPWH";
                }
                else
                    codeID = "MPWHTO";

            }
            else codeID = "MPWHTO";

            var uid = CurrentUser();
            var query = String.Format(@" 
            SELECT LookUpValue , LookUpValueName , SeqNo
                FROM gnMstLookUpDtl
                WHERE CompanyCode = '{0}'
                    AND CodeID = '{1}'
                    AND ParaValue= '{2}'
                ORDER BY SeqNo", uid.CompanyCode, codeID, BranchCode);

            var queryable = ctx.Context.Database.SqlQuery<WHview>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<WHview> Select4WHTO(string BranchCode)
        {
            var uid = CurrentUser();
            var query = String.Format(@" 
            SELECT LookUpValue , LookUpValueName , SeqNo
                FROM gnMstLookUpDtl
                WHERE CompanyCode = '{0}'
                    AND CodeID = 'MPWHTO'
                    AND ParaValue= '{1}'
                ORDER BY SeqNo", uid.CompanyCode, BranchCode);

            var queryable = ctx.Context.Database.SqlQuery<WHview>(query).AsQueryable();
            return (queryable);
        }
        [HttpGet]
        public IQueryable<TransferOutBrowse> TranferoutLookup()
        {
            var uid = CurrentUser();
            var query = String.Format(@" 
            SELECT 
            a.TransferOutNo, 
            a.TransferOutDate, 
            a.ReferenceNo, 
            a.ReferenceDate, 
            a.BranchCodeFrom, 
            b.BranchName AS BranchFrom, 
            a.BranchCodeTo, 
            c.BranchName AS BranchTo, 
            a.WareHouseCodeFrom, 
            d.RefferenceDesc1 AS WareHouseFrom, 
            a.WareHouseCodeTo, 
            e.RefferenceDesc1 AS WareHouseTo, 
            a.ReturnDate, 
            a.Remark, 
            CASE a.Status 
	            WHEN '0' THEN 'OPEN' 
	            WHEN '1' THEN 'PRINTED' 
	            WHEN '2' THEN 'APPROVED' 
	            WHEN '3' THEN 'DELETED' 
	            WHEN '5' THEN 'TRANSFERED' 
	            WHEN '9' THEN 'FINISHED' 
	            ELSE '' 
            END 
            AS Status 
            FROM  OmTrInventTransferOut a 
            LEFT JOIN gnMstOrganizationDtl b
            ON    a.CompanyCode = b.CompanyCode 
            AND   a.BranchCodeFrom = b.BranchCode 
            LEFT JOIN gnMstOrganizationDtl c
            ON    a.CompanyCode = c.CompanyCode 
            AND   a.BranchCodeTo = c.BranchCode 
            LEFT JOIN omMstRefference d
            ON    a.CompanyCode = d.CompanyCode 
            AND   a.WareHouseCodeFrom = d.RefferenceCode 
            AND   d.RefferenceType = 'WARE' 
            LEFT JOIN omMstRefference e
            ON    e.CompanyCode = a.CompanyCode 
            AND   e.RefferenceCode = a.WareHouseCodeTo 
            AND   e.RefferenceType = 'WARE' 
            WHERE a.CompanyCode = '{0}'
            AND   a.BranchCode = '{1}'
            ORDER BY a.TransferOutNo DESC,a.TransferOutDate DESC", uid.CompanyCode, uid.BranchCode);

            var queryable = ctx.Context.Database.SqlQuery<TransferOutBrowse>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<TransferOutBrowse> Select4Out()
        {
            var uid = CurrentUser();
            var query = String.Format(@" 
           SELECT distinct a.TransferOutNo, 
                a.TransferOutDate, 
                a.BranchCodeFrom,
                b.BranchName, 
                b.BranchName as BranchFrom,
                a.WareHouseCodeFrom,c.RefferenceDesc1 as WareHouseFrom, 
                a.BranchCodeTo, 
                b.BranchName as BranchTo,
                a.WareHouseCodeTo, 
                d.RefferenceDesc1 as WareHouseTo,
                   a.Status
                FROM omTrInventTransferOut a 
                INNER JOIN gnMstOrganizationDtl b ON
                b.CompanyCode = a.CompanyCode AND
                b.BranchCode = a.BranchCodeFrom
                INNER JOIN gnMstOrganizationDtl e ON
                e.CompanyCode = a.CompanyCode AND
                e.BranchCode = a.BranchCodeTo
                INNER JOIN omMstRefference c ON
                a.CompanyCode = c.CompanyCode AND
                a.WareHouseCodeFrom = c.RefferenceCode 
                INNER JOIN omMstRefference d ON
                a.CompanyCode = d.CompanyCode AND
                a.WareHouseCodeTo= d.RefferenceCode 
                INNER JOIN omTrInventTransferOutDetail z
                on a.companyCode = z.companyCode 
                and a.branchCode = z.branchCode 
                and a.TransferOutNo = z.transferoutNo
                and z.statusTransferin = '0'
                WHERE a.CompanyCode = '{0}' AND
                a.BranchCodeTo = '{1}' AND
                a.Status in ('2','5')
                ORDER BY a.TransferOutNo ASC", uid.CompanyCode, uid.BranchCode);

            var queryable = ctx.Context.Database.SqlQuery<TransferOutBrowse>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<WsTrfOut.OmTransferOut> Select4OutTrfIn(string CompanyCodeFrom, string BranchCodeFrom)
        {
            var uid = CurrentUser();

            var ws = new WsTrfOut.TransOutService();
            //ws.Url = "http://localhost:50436/TransOutService.svc?wsdl";
            ws.Url = Helpers.GetURL(uid.CompanyCode, CompanyCodeFrom);

            var datas = ws.GetOmTransOutNo(CompanyCodeFrom, BranchCodeFrom, uid.CompanyCode, uid.BranchCode).AsQueryable();

            return (datas);
        }

       


        [HttpGet]
        public IQueryable<TransferInView> TransferInBrowse()
        {
            var uid = CurrentUser();
            var query = String.Format(@" 
           SELECT 
            a.TransferInNo ,  
            a.TransferInDate , 
            a.TransferOutNo , 
            b.TransferOutDate , 
            a.ReferenceDate,
            a.ReturnDate,
            a.BranchCodeFrom,
            a.BranchCodeTo,
            a.WareHouseCodeTo, a.WareHouseCodeFrom,
             c.BranchName AS BranchFrom, d.BranchName as BranchTo,
             e.RefferenceDesc1 as WareHouseFrom, f.RefferenceDesc1 as WareHouseTo,
            CASE a.Status 
                WHEN '0' THEN 'OPEN' 
                WHEN '1' THEN 'PRINTED' 
                WHEN '2' THEN 'APPROVED' 
                WHEN '3' THEN 'DELETED' 
	            WHEN '5' THEN 'TRANSFERED' 
                WHEN '9' THEN 'FINISHED' 
                ELSE '' 
            END 
            Status, a.Remark 
            FROM OmTrInventTransferIn a 
            LEFT JOIN OmTrInventTransferOut b ON a.CompanyCode = b.CompanyCode 
            AND a.BranchCode = b.BranchCodeTo 
            AND a.TransferOutNo = b.TransferOutNo 
              INNER JOIN gnMstOrganizationDtl c ON
                 c.CompanyCode = a.CompanyCode AND
                c.BranchCode = a.BranchCodeFrom 
                INNER JOIN gnMstOrganizationDtl d ON
                 d.CompanyCode = a.CompanyCode AND
                d.BranchCode = a.BranchCodeFrom
              INNER JOIN omMstRefference e ON
                a.CompanyCode = e.CompanyCode AND
                a.WareHouseCodeFrom = e.RefferenceCode 
                INNER JOIN omMstRefference f ON
                a.CompanyCode = f.CompanyCode AND
                a.WareHouseCodeTo= f.RefferenceCode
            WHERE a.CompanyCode = '{0}' 
            AND a.BranchCode = '{1}' 
            ORDER BY a.TransferInNo DESC", uid.CompanyCode, uid.BranchCode);

            var queryable = ctx.Context.Database.SqlQuery<TransferInView>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<select4model> Select4Model(string WarehouseCode)
        {
            var uid = CurrentUser();
            var query = String.Format(@" 
             SELECT a.SalesModelCode, a.SalesModelDesc, a.EngineCode, count(*) Qty
              FROM OmMstModel a, omMstVehicle b
             WHERE b.CompanyCode = a.CompanyCode
               AND b.SalesModelCode = a.SalesModelCode
               AND b.WarehouseCode = '{1}'
               AND a.Status in (1,2)
               AND b.Status = 0
               AND a.CompanyCode = '{0}'
              and not exists (
                    select 1 
                    from omTrInventTransferOutDetail x 
	                    inner join omTrInventTransferOut y on x.CompanyCode=y.CompanyCode and x.BranchCode=y.BranchCode
		                    and x.TransferOutNo=y.TransferOutNo
                    where y.Status in ('0','1') and x.ChassisCode=b.ChassisCode and x.ChassisNo=b.ChassisNo
                )
             GROUP BY a.SalesModelCode, a.SalesModelDesc, a.EngineCode
             ORDER BY a.SalesModelCode", uid.CompanyCode, WarehouseCode);

            var queryable = ctx.Context.Database.SqlQuery<select4model>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<select4model> Select4ModelTrf(string WarehouseCode)
        {
            var Uid = CurrentUser();
            var query = string.Format(@"
               SELECT a.SalesModelCode, a.SalesModelDesc, a.EngineCode, count(*) Qty
            FROM OmMstModel a, omMstVehicle b
            WHERE b.CompanyCode = a.CompanyCode
               AND b.SalesModelCode = a.SalesModelCode
               AND b.WarehouseCode = {1}
               AND a.Status in (1,2)
               AND b.Status = 0
               AND a.CompanyCode = {0}
              and not exists (
                    select 1 
                    from omTrInventTransferOutDetailMultiBranch x 
	                inner join omTrInventTransferOutMultiBranch y on x.CompanyCode=y.CompanyCode 
                        and x.BranchCode=y.BranchCode
		                and x.TransferOutNo=y.TransferOutNo
                    where 
                        x.CompanyCode = {0}
                        and y.Status in ('0','1') 
                        and x.ChassisCode=b.ChassisCode 
                        and x.ChassisNo=b.ChassisNo
                )
             GROUP BY a.SalesModelCode, a.SalesModelDesc, a.EngineCode
             ORDER BY a.SalesModelCode
				", Uid.CompanyCode, WarehouseCode);

            var queryable = ctx.Context.Database.SqlQuery<select4model>(query).AsQueryable();
            return (queryable);
        }


        [HttpGet]
        public IQueryable<select4model> Select4ModelYear(string SalesModelCode, string WarehouseCode)
        {
            var uid = CurrentUser();
            var query = String.Format(@" 
             SELECT distinct b.SalesModelYear, b.SalesModelCode, a.SalesModelDesc, b.ChassisCode, convert(int,count(b.EngineNo)) Qty
                  FROM OmMstModel a, omMstVehicle b
                 WHERE b.CompanyCode = a.CompanyCode
                   AND b.SalesModelCode = a.SalesModelCode
                   AND b.WarehouseCode = '{2}'
                   AND a.Status in (1,2)
                   AND b.Status = 0
                   AND a.CompanyCode = '{0}'
                   AND a.SalesModelCode = '{1}'
                    and not exists (
                        select 1 
                        from omTrInventTransferOutDetail x 
	                        inner join omTrInventTransferOut y on x.CompanyCode=y.CompanyCode and x.BranchCode=y.BranchCode
		                        and x.TransferOutNo=y.TransferOutNo
                        where y.Status in ('0','1') and x.ChassisCode=b.ChassisCode and x.ChassisNo=b.ChassisNo
                    )
                 GROUP BY b.SalesModelYear, b.SalesModelCode, a.SalesModelDesc, b.ChassisCode", uid.CompanyCode, SalesModelCode, WarehouseCode);

            var queryable = ctx.Context.Database.SqlQuery<select4model>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<select4model> Select4ModelYearTrf(string SalesModelCode, string WarehouseCode)
        {
            var uid = CurrentUser();
            var query = String.Format(@" 
                SELECT 
                    distinct b.SalesModelYear, b.SalesModelCode, a.SalesModelDesc, b.ChassisCode
                    , count(b.EngineNo) Qty
                FROM OmMstModel a, omMstVehicle b
                WHERE b.CompanyCode = a.CompanyCode
                   AND b.SalesModelCode = a.SalesModelCode
                   AND b.WarehouseCode = '{2}'
                   AND a.Status in (1,2)
                   AND b.Status = 0
                   AND a.CompanyCode = '{0}'
                   AND a.SalesModelCode = '{1}'
                   and not exists (
                        select 1 
                        from omTrInventTransferOutDetailMultiBranch x 
	                        inner join omTrInventTransferOutMultiBranch y on x.CompanyCode=y.CompanyCode and x.BranchCode=y.BranchCode
		                        and x.TransferOutNo=y.TransferOutNo
                        where y.Status in ('0','1') and x.ChassisCode=b.ChassisCode and x.ChassisNo=b.ChassisNo
                    )
                 GROUP BY b.SalesModelYear, b.SalesModelCode, a.SalesModelDesc, b.ChassisCode",
                uid.CompanyCode, SalesModelCode, WarehouseCode);

            var queryable = ctx.Context.Database.SqlQuery<select4model>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<select4model> Select4Chassis(string SalesModelCode, string SalesModelYear, string ChassisCode, string WarehouseCode)
        {
            var uid = CurrentUser();
            var query = String.Format(@" 
             SELECT Convert(varchar,a.ChassisNo) as ChassisNo, 
Convert(varchar,a.EngineNo) as EngineNo,a.ColourCode,b.RefferenceDesc1 as ColourName
                FROM omMstVehicle a
                LEFT JOIN omMstRefference b
                ON a.CompanyCode = b.CompanyCode
                AND a.ColourCode = b.RefferenceCode
                WHERE b.RefferenceType = 'COLO'
                AND a.Status = 0
                AND a.CompanYCode = '{0}'
                AND a.SalesMOdelCode = '{1}'
                AND a.SalesModelYear = '{2}'
                AND a.ChassisCode = '{3}'
                AND a.WareHouseCode ='{4}'
                and not exists (
                    select 1 
                    from omTrInventTransferOutDetail x 
	                    inner join omTrInventTransferOut y on x.CompanyCode=y.CompanyCode and x.BranchCode=y.BranchCode
		                    and x.TransferOutNo=y.TransferOutNo
                    where y.Status in ('0','1') and x.ChassisCode=a.ChassisCode and x.ChassisNo=a.ChassisNo
                )
                ORDER BY a.ChassisNo", uid.CompanyCode, SalesModelCode, SalesModelYear, ChassisCode, WarehouseCode);

            var queryable = ctx.Context.Database.SqlQuery<select4model>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<select4model> Select4ChassisTrf(string SalesModelCode, string SalesModelYear, string ChassisCode, string WarehouseCode)
        {
            var uid = CurrentUser();
            var query = String.Format(@"SELECT 
                    Convert(varchar,a.ChassisNo) as ChassisNo, Convert(varchar,a.EngineNo) as EngineNo, 
                    a.ColourCode, b.RefferenceDesc1 as ColourName
                FROM omMstVehicle a
                LEFT JOIN omMstRefference b ON a.CompanyCode = b.CompanyCode
                    AND a.ColourCode = b.RefferenceCode
                WHERE b.RefferenceType = 'COLO'
                    AND a.Status = 0
                    AND a.CompanYCode = '{0}'
                    AND a.SalesMOdelCode = '{1}'
                    AND a.SalesModelYear = '{2}'
                    AND a.ChassisCode = '{3}'
                    AND a.WareHouseCode = '{4}'
                    AND not exists (
                    select 1 
                    from omTrInventTransferOutDetailMultiBranch x 
	                    inner join omTrInventTransferOutMultiBranch y on x.CompanyCode=y.CompanyCode 
                            and x.BranchCode=y.BranchCode
		                    and x.TransferOutNo=y.TransferOutNo
                    where y.Status in ('0','1') and x.ChassisCode=a.ChassisCode and x.ChassisNo=a.ChassisNo
                )
                ORDER BY a.ChassisNo", uid.CompanyCode, SalesModelCode, SalesModelYear, ChassisCode, WarehouseCode);

            var queryable = ctx.Context.Database.SqlQuery<select4model>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<select4model> Select4ModelByTOut(string TransferOutNo)
        {
            var uid = CurrentUser();
            var query = String.Format(@" 
            SELECT DISTINCT  
                    a.SalesModelCode, a.SalesModelDesc, a.EngineCode
                FROM 
                    OmMstModel a 
                JOIN 
                    omTrInventtransferOutDetail b ON b.CompanyCode=a.CompanyCode
                    AND b.SalesModelCode=a.SalesModelCode 
                JOIN 
                    omMstVehicle c ON c.CompanyCode=a.CompanyCode
                    AND c.ChassisCode=b.ChassisCode AND c.ChassisNo=b.ChassisNo AND c.Status=7
                WHERE a.Status in (1,2) 
                    AND a.CompanyCode = '{0}'
                    AND b.TransferOutNo = '{1}'
                    and b.StatusTransferIn= 0
                ORDER BY 
                    SalesModelCode", uid.CompanyCode, TransferOutNo);

            var queryable = ctx.Context.Database.SqlQuery<select4model>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<WsTrfOut.SalesModel> Select4ModelTrfOut(string CompanyCodeFrom, string TransferOutNo)
        {
            var uid = CurrentUser();
            var ws = new WsTrfOut.TransOutService();
            //ws.Url = "http://localhost:50436/TransOutService.svc?wsdl"; 
            ws.Url = Helpers.GetURL(user.CompanyCode, CompanyCodeFrom);
            var datas = ws.GetSalesModelCode(CompanyCodeFrom, TransferOutNo).AsQueryable();
            
            return (datas);
        }
        
        [HttpGet]
        public IQueryable<select4model> Select4ModelYearByTOut(string SalesModelCode, string TransferOutNo)
        {
            var uid = CurrentUser();
            var query = String.Format(@" 
            SELECT 
                    a.SalesModelYear, a.SalesModelDesc, a.ChassisCode
                FROM 
                    OmMstModelYear a
                JOIN 
                    omTrInventtransferOutDetail b ON b.CompanyCode=a.CompanyCode
                    AND b.SalesModelCode=a.SalesModelCode AND  b.SalesModelYear=a.SalesModelYear
                WHERE 
                    a.Status = 1
                    AND a.CompanyCode = '{0}'
                    AND a.SalesModelCode = '{1}'
                    AND b.TransferOutNo = '{2}'
                    and b.StatusTransferIn=0
                ORDER BY 
                    SalesModelYear", uid.CompanyCode, SalesModelCode, TransferOutNo);

            var queryable = ctx.Context.Database.SqlQuery<select4model>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<WsTrfOut.SalesModelYear> Select4ModelYearTrfOut(string CompanyCodeFrom, string TransferOutNo, string SalesModelCode)
        {
            var uid = CurrentUser();
            var ws = new WsTrfOut.TransOutService();
            //ws.Url = "http://localhost:50436/TransOutService.svc?wsdl"; 
            ws.Url = Helpers.GetURL(user.CompanyCode, CompanyCodeFrom);
            var datas = ws.GetSalesModelYear(CompanyCodeFrom, TransferOutNo, SalesModelCode).AsQueryable();

            return (datas);
        }

        [HttpGet]
        public IQueryable<select4model> Select4ChassisByTOut(string SalesModelCode, string SalesModelYear, string ChassisCode, string TransferOutNo)
        {
            var uid = CurrentUser();
            var query = String.Format(@" 
            SELECT 
	                a.ChassisCode, Convert(varchar,a.ChassisNo) as ChassisNo, a.EngineCode, 
	                Convert(varchar,a.EngineNo) as EngineNo,a.ColourCode,
	                c.RefferenceDesc1 as ColourName
                FROM 
	                omMstVehicle a
                JOIN 
	                omTrInventtransferOutDetail b ON b.CompanyCode=a.CompanyCode AND b.ChassisNo=a.ChassisNo
	                AND b.SalesModelCode=a.SalesModelCode AND b.SalesModelYear=a.SalesModelYear	
	                AND b.ChassisCode=a.ChassisCode
                JOIN 
	                omMstRefference c ON a.CompanyCode = c.CompanyCode
	                AND a.ColourCode = c.RefferenceCode
                WHERE 
	                c.RefferenceType = 'COLO'
	                AND a.Status = 7
                    AND a.CompanYCode    = '{0}'               
	                AND b.SalesMOdelCode = '{1}'
	                AND b.SalesModelYear = '{2}'
	                AND b.ChassisCode    = '{3}'
                    AND b.TransferOutNo  = '{4}'
                    and b.StatusTransferIn= 0", uid.CompanyCode, SalesModelCode, SalesModelYear, ChassisCode, TransferOutNo);

            var queryable = ctx.Context.Database.SqlQuery<select4model>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<WsTrfOut.SalesVehicle> Select4ChassisTrfOut(string CompanyCodeFrom, string TransferOutNo, string SalesModelCode, string SalesModelYear, string ChassisCode)
        {
            var uid = CurrentUser();
            var ws = new WsTrfOut.TransOutService();
            //ws.Url = "http://localhost:50436/TransOutService.svc?wsdl"; 
            ws.Url = Helpers.GetURL(user.CompanyCode, CompanyCodeFrom);
            var datas = ws.GetSalesVehicle(CompanyCodeFrom, TransferOutNo, SalesModelCode, Convert.ToDecimal(SalesModelYear), true, ChassisCode).AsQueryable();

            return (datas);
        }

        [HttpGet]
        public IQueryable<ColorChangeView> ColorChanges()
        {
            var uid = CurrentUser();
            //            var query = String.Format(@" 
            //            SELECT a.DocNo, a.DocDate, a.ReferenceNo, a.ReferenceDate, a.Remark,
            //            CASE a.Status WHEN '0' THEN 'OPEN'WHEN '1' THEN 'PRINTED' WHEN '2' THEN 'CLOSED' WHEN '3' THEN 'DELETED' WHEN '9' THEN 'FINISHED' ELSE '' END Status
            //            FROM OmTrInventColorChange a
            //            WHERE a.CompanyCode = '{0}' AND a.BranchCode = '{1}'   
            //            ORDER BY a.DocNo DESC", uid.CompanyCode, uid.BranchCode);
            //            var queryable = ctx.Context.Database.SqlQuery<ColorChangeView>(query).AsQueryable();
            var queryable = from a in ctx.Context.OmTrInventColorChange
                            where a.CompanyCode == uid.CompanyCode && a.BranchCode == uid.BranchCode
                            select new ColorChangeView()
                            {
                                DocNo = a.DocNo,
                                DocDate = a.DocDate,
                                ReferenceNo = a.ReferenceNo,
                                ReferenceDate = a.ReferenceDate,
                                Remark = a.Remark,
                                Status = a.Status == "0" ? "OPEN" : a.Status == "1" ? "PRINTED" : a.Status == "2" ? "CLOSED" : a.Status == "3" ? "DELETED" : a.Status == "9" ? "FINISHED" : ""
                            };
            return (queryable);
        }

        [HttpGet]
        public IQueryable<select4model> Select4ChassisNo(string SalesModelCode, string SalesModelYear, string ChassisCode)
        {
            var uid = CurrentUser();
            var query = String.Format(@" 
             SELECT Convert(varchar,a.ChassisNo) as ChassisNo, 
                a.EngineCode,Convert(varchar,a.EngineNo) as EngineNo, 
                a.WarehouseCode, c.RefferenceDesc1 as WarehouseName,
                a.ColourCode,b.RefferenceDesc1 as ColourName
                FROM omMstVehicle a
                LEFT JOIN omMstRefference b
                ON a.CompanyCode = b.CompanyCode
                AND a.ColourCode = b.RefferenceCode
                AND b.RefferenceType = 'COLO'
                LEFT JOIN omMstRefference c
                ON a.CompanyCode = c.CompanyCode
                AND a.WarehouseCode = c.RefferenceCode
                AND c.RefferenceType = 'WARE'
                WHERE a.Status = 0
                AND a.CompanYCode = '{0}'
                AND a.SalesMOdelCode = '{1}'
                AND a.SalesModelYear = '{2}'
                AND a.ChassisCode = '{3}'
                ORDER BY a.WarehouseCode, a.ColourCode, a.ChassisNo", uid.CompanyCode, SalesModelCode, SalesModelYear, ChassisCode);

            var queryable = ctx.Context.Database.SqlQuery<select4model>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<MstModelYearBrowse> SalesModelYear4ColourChange(string SalesModelCode)
        {
            var Uid = CurrentUser();
            var query = string.Format(@"
                select distinct 
                a.SalesModelYear, 
                a.SalesModelDesc, 
                isnull(b.ChassisCode, '') ChassisCode
                , a.Remark
                , CASE WHEN a.Status = 0 THEN 'Tidak Aktif' ELSE 'Aktif' END AS Status
                , a.SalesModelYear, a.SalesModelDesc
            from OmMstModelYear a
            left join omMstVehicle b on
	            b.CompanyCode = a.CompanyCode
	            and b.SalesModelCode = a.SalesModelCode
	            and b.SalesModelYear = a.SalesModelYear
	            and b.status = 0
            WHERE a.CompanyCode ='{0}'
            AND a.SalesModelCode = '{1}'
            AND b.ChassisCode is not null
            ORDER BY a.SalesModelYear,a.SalesModelDesc ASC 
                       ", Uid.CompanyCode, SalesModelCode);

            var queryable = ctx.Context.Database.SqlQuery<MstModelYearBrowse>(query).AsQueryable();

            //var queryable = ctx.Context.Database.SqlQuery<spTrnIReservedHdrView>("select ReservedNo,ReservedDate,ReferenceNo,ReferenceDate,PartNo,TypeOfGoods,OprCode,Status from spTrnIReservedHdr where Status=0 and  CompanyCode='" + Uid.CompanyCode + "' and  BranchCode='" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SPKView> SPKBBN()
        {
            var Uid = CurrentUser();
            var query = string.Format(@"
                SELECT a.SPKNo, a.SPKDate, a.SupplierCode, (SELECT b.SupplierName
                           FROM gnMstSupplier b
                           WHERE a.SupplierCode = b.SupplierCode)  AS SupplierName, 
                           b.ChassisCode, convert(varchar, b.ChassisNo, 100) ChassisNo,
                           CASE
                              WHEN a.Status = 0 THEN 'OPEN'
                              WHEN a.Status = 1 THEN 'PRINTED'
                              WHEN a.Status = 2 THEN 'APPROVED'
                              WHEN a.Status = 3 THEN 'CANCELED'
                              WHEN a.Status = 9 THEN 'FINISHED'
                           END  AS Status, a.Remark, a.RefferenceNo, a.RefferenceDate 
                  FROM omTrSalesSPK a
                  LEFT JOIN omTrSalesSPKDetail b 
	                ON a.CompanyCode = b.CompanyCode
	                AND a.BranchCode = b.BranchCode
	                AND a.SPKNo = b.SPKNo
                 WHERE a.CompanyCode = '{0}'
                   AND a.BranchCode = '{1}'
                    ORDER BY a.SPKNo DESC
                       ", Uid.CompanyCode, Uid.BranchCode);

            var queryable = ctx.Context.Database.SqlQuery<SPKView>(query).AsQueryable();

            //var queryable = ctx.Context.Database.SqlQuery<spTrnIReservedHdrView>("select ReservedNo,ReservedDate,ReferenceNo,ReferenceDate,PartNo,TypeOfGoods,OprCode,Status from spTrnIReservedHdr where Status=0 and  CompanyCode='" + Uid.CompanyCode + "' and  BranchCode='" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SPKView> ChassisCodeSPKBBN()
        {
            var Uid = CurrentUser();
            var query = string.Format(@"
                SELECT distinct a.ChassisCode
                FROM omTrSalesReqDetail a
	                inner join OmTrSalesFakturPolisi b on a.ChassisCode=b.ChassisCode and a.ChassisNo=b.ChassisNo
                 WHERE a.CompanyCode = '{0}'
                   AND a.BranchCode = '{1}'
	                AND not exists(
			                        SELECT 1
			                        FROM OmTrSalesSPKDetail 
			                        WHERE CompanyCode = a.CompanyCode
			                        AND BranchCode = a.BranchCode
			                        and ChassisCode=a.ChassisCode
			                        and ChassisNo=a.ChassisNo
                            )
                       ", Uid.CompanyCode, Uid.BranchCode);
            var queryable = ctx.Context.Database.SqlQuery<SPKView>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SPKDetailView> ChassisNoSPKBBN(string ChassisCode)
        {
            var Uid = CurrentUser();
            var query = string.Format(@"
                SELECT distinct b.ChassisNo ChassisNo,b.ReqNo as ReqInNo,b.FakturPolisiNo, SKPKName as CustomerName, c.SubDealerCode CustomerCode,
a.SKPKAddress1 + a.SKPKAddress2+' '+ a.SKPKAddress3 as Address, d.leasingCo Leasing
FROM omTrSalesReqDetail a
	inner join OmTrSalesFakturPolisi b on a.ChassisCode=b.ChassisCode and a.ChassisNo=b.ChassisNo
    inner join omTrSalesReq c on a.ReqNo=c.ReqNo and a.BranchCode=c.BranchCode
    inner join omTrSalesSO d on a.SONo=d.SONo and a.BranchCode=d.BranchCode
WHERE a.CompanyCode = '{0}'
   AND a.BranchCode = '{1}'
	AND a.ChassisCode = '{2}'
	AND not exists(
			SELECT 1
			FROM OmTrSalesSPKDetail 
			WHERE CompanyCode = a.CompanyCode
			AND BranchCode = a.BranchCode
			and ChassisCode=a.ChassisCode
			and ChassisNo=a.ChassisNo
	)
                       ", Uid.CompanyCode, Uid.BranchCode, ChassisCode);
            var queryable = ctx.Context.Database.SqlQuery<SPKDetailView>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<POBrowse> PO4Lookup(string Status)
        {
            var Uid = CurrentUser();
            var query = "";
            if (Status != "-1")
            {
                query = string.Format(@"
                SELECT a.PONo, a.PODate, a.SupplierCode,
                                (SELECT b.SupplierName
                                    FROM dbo.gnMstSupplier b
                                    WHERE a.SupplierCode = b.SupplierCode) as SupplierName, 
                                CASE
                                    WHEN a.Status = 0 THEN 'Open'
                                    WHEN a.Status = 1 THEN 'Printed'
                                    WHEN a.Status = 2 THEN 'Approved'
                                    WHEN a.Status = 3 THEN 'Canceled'
                                    WHEN a.Status = 9 THEN 'Finished'
                                END as Status 
                    FROM dbo.omTrPurchasePO a
                    WHERE a.CompanyCode = '{0}' AND a.BranchCode = '{1}' and a.Status = '{2}'
                       ", Uid.CompanyCode, Uid.BranchCode, Status);
            }
            else
            {
                query = string.Format(@"
                SELECT a.PONo, a.PODate, a.SupplierCode,
                                (SELECT b.SupplierName
                                    FROM dbo.gnMstSupplier b
                                    WHERE a.SupplierCode = b.SupplierCode) as SupplierName, 
                                CASE
                                    WHEN a.Status = 0 THEN 'Open'
                                    WHEN a.Status = 1 THEN 'Printed'
                                    WHEN a.Status = 2 THEN 'Approved'
                                    WHEN a.Status = 3 THEN 'Canceled'
                                    WHEN a.Status = 9 THEN 'Finished'
                                END as Status 
                    FROM dbo.omTrPurchasePO a
                    WHERE a.CompanyCode = '{0}' AND a.BranchCode = '{1}'
                       ", Uid.CompanyCode, Uid.BranchCode);
            }

            var queryable = ctx.Context.Database.SqlQuery<POBrowse>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<BPUView> BPU4Lookup(string Status)
        {
            var Uid = CurrentUser();
            var query = "";
            if (Status != "-1")
            {
                query = string.Format(@"
                SELECT a.BPUNo, a.BPUDate, a.PONo, 
                                CASE
                                    WHEN a.Status = 0 THEN 'Open'
                                    WHEN a.Status = 1 THEN 'Printed'
                                    WHEN a.Status = 2 THEN 'Approved'
                                    WHEN a.Status = 3 THEN 'Canceled'
                                    WHEN a.Status = 9 THEN 'Finished'
                                END as Status 
                    FROM OmTrPurchaseBPU a
                    WHERE a.CompanyCode ='{0}'
                    AND a.BranchCode = '{1}' and a.Status = '{2}'
                       ", Uid.CompanyCode, Uid.BranchCode, Status);
            }
            else
            {
                query = string.Format(@"
                SELECT a.BPUNo, a.BPUDate, a.PONo, 
                                CASE
                                    WHEN a.Status = 0 THEN 'Open'
                                    WHEN a.Status = 1 THEN 'Printed'
                                    WHEN a.Status = 2 THEN 'Approved'
                                    WHEN a.Status = 3 THEN 'Canceled'
                                    WHEN a.Status = 9 THEN 'Finished'
                                END as Status 
                    FROM OmTrPurchaseBPU a
                    WHERE a.CompanyCode = '{0}' AND a.BranchCode = '{1}'
                       ", Uid.CompanyCode, Uid.BranchCode);
            }

            var queryable = ctx.Context.Database.SqlQuery<BPUView>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<HPPView> HPP4Lookup(string Status)
        {
            var Uid = CurrentUser();
            var query = "";
            if (Status != "-1")
            {
                query = string.Format(@"
                SELECT a.HPPNo, a.HPPDate, a.PONo, (SELECT b.SupplierName 
                        FROM dbo.gnMstSupplier b
                        WHERE a.SupplierCode = b.SupplierCode)  AS supplierName, 
                                CASE
                                    WHEN a.Status = 0 THEN 'Open'
                                    WHEN a.Status = 1 THEN 'Printed'
                                    WHEN a.Status = 2 THEN 'Approved'
                                    WHEN a.Status = 3 THEN 'Canceled'
                                    WHEN a.Status = 9 THEN 'Finished'
                                END as Status
                FROM dbo.omTrPurchaseHPP a
                    WHERE a.CompanyCode ='{0}'
                    AND a.BranchCode = '{1}' and a.Status = '{2}'
                       ", Uid.CompanyCode, Uid.BranchCode, Status);
            }
            else
            {
                query = string.Format(@"
                SELECT a.HPPNo, a.HPPDate, a.PONo, (SELECT b.SupplierName 
                        FROM dbo.gnMstSupplier b
                        WHERE a.SupplierCode = b.SupplierCode)  AS supplierName, 
                                CASE
                                    WHEN a.Status = 0 THEN 'Open'
                                    WHEN a.Status = 1 THEN 'Printed'
                                    WHEN a.Status = 2 THEN 'Approved'
                                    WHEN a.Status = 3 THEN 'Canceled'
                                    WHEN a.Status = 9 THEN 'Finished'
                                END as Status
                FROM dbo.omTrPurchaseHPP a
                    WHERE a.CompanyCode = '{0}' AND a.BranchCode = '{1}'
                       ", Uid.CompanyCode, Uid.BranchCode);
            }

            var queryable = ctx.Context.Database.SqlQuery<HPPView>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<PerlengkapanInView> Perlengkapan4Lookup(string Status)
        {
            var Uid = CurrentUser();
            var query = "";
            if (Status != "-1")
            {
                query = string.Format(@"
               SELECT a.PerlengkapanNo,a.PerlengkapanDate,a.RefferenceNo,a.SourceDoc, a.Remark,
            CASE a.PerlengkapanType WHEN  '1' THEN 'BPU' WHEN '2' THEN 'TRANSFER' WHEN '3' THEN 'RETURN' END as PerlengkapanType,
                                CASE
                                    WHEN a.Status = 0 THEN 'Open'
                                    WHEN a.Status = 1 THEN 'Printed'
                                    WHEN a.Status = 2 THEN 'Approved'
                                    WHEN a.Status = 3 THEN 'Canceled'
                                    WHEN a.Status = 9 THEN 'Finished'
                                END as Status
            FROM OmTrPurchasePerlengkapanIn a
                    WHERE a.CompanyCode ='{0}'
                    AND a.BranchCode = '{1}' and a.Status = '{2}'
                       ", Uid.CompanyCode, Uid.BranchCode, Status);
            }
            else
            {
                query = string.Format(@"
              SELECT a.PerlengkapanNo,a.PerlengkapanDate,a.RefferenceNo,a.SourceDoc, a.Remark,
            CASE a.PerlengkapanType WHEN  '1' THEN 'BPU' WHEN '2' THEN 'TRANSFER' WHEN '3' THEN 'RETURN' END as PerlengkapanType,
                                CASE
                                    WHEN a.Status = 0 THEN 'Open'
                                    WHEN a.Status = 1 THEN 'Printed'
                                    WHEN a.Status = 2 THEN 'Approved'
                                    WHEN a.Status = 3 THEN 'Canceled'
                                    WHEN a.Status = 9 THEN 'Finished'
                                END as Status
            FROM OmTrPurchasePerlengkapanIn a
                    WHERE a.CompanyCode = '{0}' AND a.BranchCode = '{1}'
                       ", Uid.CompanyCode, Uid.BranchCode);
            }

            var queryable = ctx.Context.Database.SqlQuery<PerlengkapanInView>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<PerlengkapanAdjustmentView> Adjustment4Lookup(string Status)
        {
            var Uid = CurrentUser();
            var query = "";
            if (Status != "-1")
            {
                query = string.Format(@"
              SELECT a.AdjustmentNo,a.AdjustmentDate,a.RefferenceNo, a.Remark,
                                CASE
                                    WHEN a.Status = 0 THEN 'Open'
                                    WHEN a.Status = 1 THEN 'Printed'
                                    WHEN a.Status = 2 THEN 'Approved'
                                    WHEN a.Status = 3 THEN 'Canceled'
                                    WHEN a.Status = 9 THEN 'Finished'
                                END as Status
            FROM OmTrPurchasePerlengkapanAdjustment a
                    WHERE a.CompanyCode ='{0}'
                    AND a.BranchCode = '{1}' and a.Status = '{2}'
                       ", Uid.CompanyCode, Uid.BranchCode, Status);
            }
            else
            {
                query = string.Format(@"
               SELECT a.AdjustmentNo,a.AdjustmentDate,a.RefferenceNo, a.Remark,
                                CASE
                                    WHEN a.Status = 0 THEN 'Open'
                                    WHEN a.Status = 1 THEN 'Printed'
                                    WHEN a.Status = 2 THEN 'Approved'
                                    WHEN a.Status = 3 THEN 'Canceled'
                                    WHEN a.Status = 9 THEN 'Finished'
                                END as Status
            FROM OmTrPurchasePerlengkapanAdjustment a
                    WHERE a.CompanyCode = '{0}' AND a.BranchCode = '{1}'
                       ", Uid.CompanyCode, Uid.BranchCode);
            }

            var queryable = ctx.Context.Database.SqlQuery<PerlengkapanAdjustmentView>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<ReturnView> Return4Lookup(string Supplier)
        {
            var Uid = CurrentUser();
            var query = "";
            if (Supplier != "")
            {
                query = string.Format(@"
             SELECT a.ReturnNo,a.ReturnDate FROM omTrPurchaseReturn a
            LEFT JOIN omTrPurchaseHPP c ON a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode AND a.HPPNo = c.HPPNo
                    WHERE a.CompanyCode ='{0}'
                    AND a.BranchCode = '{1}' and c.SupplierCode = '{2}'
                      ORDER BY a.ReturnNo ASC ", Uid.CompanyCode, Uid.BranchCode, Supplier);
            }
            else
            {
                query = string.Format(@"
               SELECT a.ReturnNo,a.ReturnDate FROM omTrPurchaseReturn a
                    WHERE a.CompanyCode = '{0}' AND a.BranchCode = '{1}'
                    ORDER BY a.ReturnNo ASC
                       ", Uid.CompanyCode, Uid.BranchCode);
            }
            var queryable = ctx.Context.Database.SqlQuery<ReturnView>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<PeriodView> Periode4Lookup(string FiscalYear)
        {
            var Uid = CurrentUser();
            var query = string.Format(@"
               SELECT  CONVERT(varchar, FiscalYear) + RIGHT('00' + CONVERT(varchar, PeriodeNum), 2) AS Periode, CONVERT(VARCHAR(4), FiscalYear) AS FiscalYear, PeriodeNum, FiscalMonth, PeriodeName, FromDate, EndDate
            , CASE ISNULL(StatusSparepart, 0 ) WHEN 0 THEN 'Future Entry' WHEN 1 THEN 'Open' WHEN 2 THEN 'Close' END AS StatusSP
            , CASE ISNULL(StatusSales, 0 ) WHEN 0 THEN 'Future Entry' WHEN 1 THEN 'Open' WHEN 2 THEN 'Close' END AS StatusSL
            , CASE ISNULL(StatusService, 0 ) WHEN 0 THEN 'Future Entry' WHEN 1 THEN 'Open' WHEN 2 THEN 'Close' END AS StatusSV
            , CASE ISNULL(StatusFinanceAP, 0 ) WHEN 0 THEN 'Future Entry' WHEN 1 THEN 'Open' WHEN 2 THEN 'Close' END AS StatusAP
            , CASE ISNULL(StatusFinanceAR, 0 ) WHEN 0 THEN 'Future Entry' WHEN 1 THEN 'Open' WHEN 2 THEN 'Close' END AS StatusAR
            , CASE ISNULL(StatusFinanceGL, 0 ) WHEN 0 THEN 'Future Entry' WHEN 1 THEN 'Open' WHEN 2 THEN 'Close' END AS StatusGL
            , CASE ISNULL(FiscalStatus, 0 ) WHEN 0 THEN 'Not Active' WHEN 1 THEN 'Active' END AS StatusFiscal
             FROM gnMstPeriode WHERE CompanyCode = '{0}'
             AND BranchCode = '{1}'
            and FiscalYear = '{2}'
                  order by fiscalyear, PeriodeNum asc", Uid.CompanyCode, Uid.BranchCode, FiscalYear);

            var queryable = ctx.Context.Database.SqlQuery<PeriodView>(query).AsQueryable();
            return (queryable);
        }

        //        [HttpGet]
        //        public IQueryable<CompanyAccountView> CompanyAccount(string compTo, bool isCekStatus)
        //        {
        //            var Uid = CurrentUser();
        //            var sqlstr = ""; 
        //            if (isCekStatus)
        //            {
        //                sqlstr = @"
        //                select *, case IsActive when '1' then 'Aktif' else 'Tdk Aktif' end Status
        //                from omMstCompanyAccount
        //                where companycode='{0}' and companycodeto=case '{1}' when '' then companycodeto else '{1}' end 
        //                    and isActive = 1  ";
        //            }
        //            else
        //            {
        //                sqlstr = @"
        //                select *, case IsActive when '1' then 'Aktif' else 'Tdk Aktif' end Status
        //                from omMstCompanyAccount
        //                where companycode='{0}' and companycodeto=case '{1}' when '' then companycodeto else '{1}' end ";
        //            }
        //            var query = string.Format(sqlstr, Uid.CompanyCode, compTo);
        //            var queryable = ctx.Context.Database.SqlQuery<CompanyAccountView>(query).AsQueryable();
        //            return (queryable);
        //        }

        #region Tanda Terima STNK

        [HttpGet]
        public IQueryable<STNKBrowse> STNKBrowse()
        {
            var Uid = CurrentUser();
            var query = String.Format(@"
                SELECT a.DocNo, a.DocDate, a.BPKBOutType, a.BPKBOutBY, a.Remark, a.Status, CASE
                              WHEN a.Status = 0 THEN 'OPEN'
                              WHEN a.Status = 1 THEN 'PRINTED'
                              WHEN a.Status = 2 THEN 'APPROVED'
                              WHEN a.Status = 3 THEN 'CANCELED'
                              WHEN a.Status = 9 THEN 'FINISHED'
                           END  AS Stat,
                        (CASE a.BPKBOutType WHEN 0 THEN 'Leasing' WHEN 1 THEN 'Cabang' ELSE 'Pelanggan' END) BPKBOutTypeName,
                        (CASE WHEN a.BPKBOutType != 1 THEN (SELECT CustomerName FROM gnMstCustomer WHERE CompanyCode = a.CompanyCode AND CustomerCode = a.BPKBOutBY)
                        ELSE (SELECT RefferenceDesc1 FROM omMstRefference WHERE RefferenceType = 'WARE' AND RefferenceCode = a.BPKBOutBy) END) BPKBOutByName
                  FROM omTrSalesSTNK a
                 WHERE a.CompanyCode = '{0}'
                   AND a.BranchCode = '{1}'
                    ORDER BY a.DocNo DESC
            ", Uid.CompanyCode, Uid.BranchCode);

            var queryable = ctx.Context.Database.SqlQuery<STNKBrowse>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<STNKOut> STNKOutLookup(string Type)
        {
            var uid = CurrentUser();
            var query = String.Format(@"
                select * into #t1
                from (
	                select distinct
		                a.CompanyCode
		                ,case when '{2}'='0' then isnull(c.LeasingCo,'')
			                when '{2}'='1' then c.BranchCode
			                when '{2}'='2' then c.CustomerCode
		                end Code
		                ,'{2}' [Type]
	                from omTrSalesSPKDetail a
		                inner join omTrSalesSOVin b on a.CompanyCode=b.CompanyCode and a.ChassisCode=b.ChassisCode
			                and a.ChassisNo=b.ChassisNo
		                inner join omTrSalesSO c on b.CompanyCode=c.CompanyCode and  b.BranchCode=c.BranchCode
			                and b.SONo=c.SONo
	                where a.CompanyCode='{0}'
		                and a.BranchCode='{1}'
                ) #t1

                select Code BPKBOutBy
	                ,case when '{2}'='1' then isnull(c.CompanyName,'')
		                else isnull(b.CustomerName,'')
	                end BPKBOutByName
                from #t1 a
	                left join gnMstCustomer b on a.CompanyCode=b.CompanyCode and a.Code=b.CustomerCode
	                left join gnMstCoProfile c on a.CompanyCode=c.CompanyCode and a.Code=c.BranchCode
                where isnull(Code,'') <> ''

                drop table #t1
            ", uid.CompanyCode, uid.BranchCode, Type);

            var queryable = ctx.Context.Database.SqlQuery<STNKOut>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SalesSTNKDetailView> ChassisCode4STNK(string BPKBOutType, string BPKBOutBy)
        {
            var uid = CurrentUser();
            var query = String.Format(@"
                select a.ChassisCode, a.ChassisNo, c.CustomerCode, d.CustomerName, b.EngineCode, b.EngineNo, a.FakturPolisiNo, b.SalesModelCode, 
                b.ColourCode, a.BPKBNo, a.PoliceRegistrationNo, a.PoliceRegistrationDate, a.Remark
                from omTrSalesSPKDetail a
	                inner join omTrSalesSOVin b on a.CompanyCode=b.CompanyCode and a.ChassisCode=b.ChassisCode
		                and a.ChassisNo=b.ChassisNo
	                inner join omTrSalesSO c on b.CompanyCode=c.CompanyCode and  b.BranchCode=c.BranchCode
		                and b.SONo=c.SONo
                    inner join gnMstCustomer d on c.CustomerCode=d.CustomerCode and c.CompanyCode=d.CompanyCode
                where a.CompanyCode='{0}'
	                and a.BranchCode='{1}'
	                and (case when '{2}'='0' then isnull(c.LeasingCo,'')
			                when '{2}'='1' then c.BranchCode
			                when '{2}'='2' then c.CustomerCode
	                end)= '{3}'
                    and not exists (
                        select 1
                        from OmTrSalesSTNKDetail x
	                        inner join OmTrSalesSTNK y on x.CompanyCode=y.CompanyCode and x.BranchCode=y.BranchCode
		                        and x.DocNo=y.DocNo
                        where x.CompanyCode=a.CompanyCode
	                        and x.BranchCode=a.BranchCode
                            and x.ChassisCode=a.ChassisCode and x.ChassisNo=a.ChassisNo
                            and y.Status <> '3'
                    )
            ", uid.CompanyCode, uid.BranchCode, BPKBOutType, BPKBOutBy);

            var queryable = ctx.Context.Database.SqlQuery<SalesSTNKDetailView>(query).AsQueryable();
            return (queryable.Distinct());
        }

        [HttpGet]
        public IQueryable<SalesSTNKDetailView> ChassisNo4STNK(string ChassisCode, string BPKBOutType, string BPKBOutBy)
        {
            var uid = CurrentUser();
            var query = String.Format(@"
                select a.ChassisNo, a.ChassisCode, c.CustomerCode, d.CustomerName, b.EngineCode, b.EngineNo, a.FakturPolisiNo, b.SalesModelCode, 
                b.ColourCode, a.BPKBNo, a.PoliceRegistrationNo, a.PoliceRegistrationDate, a.Remark
                from omTrSalesSPKDetail a
	                inner join omTrSalesSOVin b on a.CompanyCode=b.CompanyCode and a.ChassisCode=b.ChassisCode
		                and a.ChassisNo=b.ChassisNo
	                inner join omTrSalesSO c on b.CompanyCode=c.CompanyCode and  b.BranchCode=c.BranchCode
		                and b.SONo=c.SONo
                    inner join gnMstCustomer d on c.CustomerCode=d.CustomerCode and c.CompanyCode=d.CompanyCode
                where a.CompanyCode='{0}'
	                and a.BranchCode='{1}'
                    and a.ChassisCode= '{2}'
	                and (case when '{3}'='0' then isnull(c.LeasingCo,'')
			                when '{3}'='1' then c.BranchCode
			                when '{3}'='2' then c.CustomerCode
	                end)= '{4}'
                    and not exists (
                        select 1
                        from OmTrSalesSTNKDetail x
	                        inner join OmTrSalesSTNK y on x.CompanyCode=y.CompanyCode and x.BranchCode=y.BranchCode
		                        and x.DocNo=y.DocNo
                        where x.CompanyCode=a.CompanyCode
	                        and x.BranchCode=a.BranchCode
                            and x.ChassisCode=a.ChassisCode and x.ChassisNo=a.ChassisNo
                            and y.Status <> '3'
                    )
            ", uid.CompanyCode, uid.BranchCode, ChassisCode, BPKBOutType, BPKBOutBy);

            var queryable = ctx.Context.Database.SqlQuery<SalesSTNKDetailView>(query).AsQueryable();
            return (queryable);
        }


        #endregion

        [HttpGet]
        public IQueryable<TransferOutBrowseMulti> TranferoutMultiLookup()
        {
            var uid = CurrentUser();
            var query = String.Format(@" 
            SELECT distinct
            a.TransferOutNo, 
            a.TransferOutDate, 
            a.ReferenceNo, 
            a.ReferenceDate, 
            a.BranchCodeFrom, 
            b.BranchName AS BranchFrom, 
            a.BranchCodeTo, 
            c.BranchCodeToDesc AS BranchTo, 
            a.WareHouseCodeFrom, 
            d.RefferenceDesc1 AS WareHouseFrom, 
            a.WareHouseCodeTo, 
            c.WarehouseCodeToDesc AS WareHouseTo, 
            a.ReturnDate, 
            a.Remark, 
            CASE a.Status 
	            WHEN '0' THEN 'OPEN' 
	            WHEN '1' THEN 'PRINTED' 
	            WHEN '2' THEN 'APPROVED' 
	            WHEN '3' THEN 'DELETED' 
	            WHEN '5' THEN 'TRANSFERED' 
	            WHEN '9' THEN 'FINISHED' 
	            ELSE '' 
            END 
            AS Status,
            a.CompanyCodeFrom,
            a.CompanyCodeTo,
            c.CompanyCodeToDesc,
            c.BranchCodeToDesc
        FROM  OmTrInventTransferOutMultiBranch a 
        LEFT JOIN gnMstOrganizationDtl b ON a.CompanyCode = b.CompanyCode 
            AND   a.BranchCodeFrom = b.BranchCode 
        LEFT JOIN omMstCompanyAccount c ON a.CompanyCode = c.CompanyCode 
            AND c.CompanyCodeTo = a.CompanyCodeTo
        LEFT JOIN omMstRefference d ON a.CompanyCode = d.CompanyCode 
            AND   a.WareHouseCodeFrom = d.RefferenceCode 
            AND   d.RefferenceType = 'WARE' 
        WHERE a.CompanyCode = '{0}'
            AND   a.BranchCode = '{1}'
        ORDER BY a.TransferOutNo DESC,a.TransferOutDate DESC", uid.CompanyCode, uid.BranchCode);

            var queryable = ctx.Context.Database.SqlQuery<TransferOutBrowseMulti>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<TransferInView> TransferInMultiBrowse()
        {
            var uid = CurrentUser();
            var query = String.Format(@" 
            SELECT 
            a.TransferInNo ,  
            a.TransferInDate , 
            a.TransferOutNo , 
            b.TransferOutDate , 
            a.ReferenceNo,
            a.ReferenceDate,
            a.ReturnDate,
            a.CompanyCodeFrom,
            a.CompanyCodeTo, 
            a.BranchCodeFrom,
            a.BranchCodeTo,
            a.WareHouseCodeTo, 
            a.WareHouseCodeFrom,
            c.BranchCodeToDesc AS BranchFrom, 
			d.BranchName as BranchTo,
            c.WarehouseCodeToDesc as WareHouseFrom, 
			e.RefferenceDesc1 as WareHouseTo,
            CASE a.Status 
                WHEN '0' THEN 'OPEN' 
                WHEN '1' THEN 'PRINTED' 
                WHEN '2' THEN 'CLOSED' 
                WHEN '3' THEN 'DELETED' 
	            WHEN '5' THEN 'TRANSFERED' 
                WHEN '9' THEN 'FINISHED' 
                ELSE '' 
            END 
            Status, a.Remark 
            FROM omTrInventTransferInMultiBranch a 
            LEFT JOIN OmTrInventTransferOutMultiBranch b ON a.CompanyCode = b.CompanyCode 
            AND a.BranchCode = b.BranchCodeTo 
            AND a.TransferOutNo = b.TransferOutNo 
            Left JOIN omMstCompanyAccount c ON
                 c.CompanyCodeTo = a.CompanyCodeFrom AND
                c.BranchCodeTo = a.BranchCodeFrom 
            left JOIN gnMstOrganizationDtl d ON
                 d.CompanyCode = a.CompanyCode AND
                d.BranchCode = a.BranchCodeTo
              left JOIN omMstRefference e ON
                a.CompanyCode = e.CompanyCode AND
                a.WareHouseCodeTo = e.RefferenceCode  
            WHERE a.CompanyCode = '{0}' 
            AND a.BranchCode = '{1}' 
            ORDER BY a.TransferInNo DESC", uid.CompanyCode, uid.BranchCode);

            var queryable = ctx.Context.Database.SqlQuery<TransferInView>(query).AsQueryable();
            return (queryable);
        }

        #region Kwitansi Unit

        [HttpGet]
        public IQueryable<OmTrSalesReceiptView> KwitansiBrowse()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.OmTrSalesReceipt
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            select new OmTrSalesReceiptView()
                            {
                                ReceiptNo = p.ReceiptNo,
                                ChassisCode = p.ChassisCode,
                                ChassisNo = p.ChassisNo,
                                EngineCode = p.EngineCode,
                                EngineNo = p.EngineNo,
                                CustomerCode = p.CustomerCode,
                                CustomerName = p.CustomerName,
                                Description = p.Description,
                                Amount = p.Amount,
                                FakturPolisiNo = p.FakturPolisiNo,
                                ColourCode = p.ColourCode,
                                ColourDescription = p.ColourDescription,
                                PrintSeq = p.PrintSeq,
                                ReceiptStatus = p.ReceiptStatus
                            };

            return (queryable);

        }

        [HttpGet]
        public IQueryable<FakturPolisiBrowse> LookUpFakturPenjualan()
        {
            var Uid = CurrentUser();
            var query = string.Format(@"
               select a.FakturPolisiNo,
		                a.chassisCode,
		                a.ChassisNo,
		                EngineCode, 
		                EngineNo,
		                SalesModelCode,
		                SalesModelYear, 
		                ColourCode,
						d.RefferenceDesc1 ColourDescription,
						b.FakturPolisiName as CustomerName,
						(Case when ProductType = '2W' then 'Sepeda Motor' else 'Mobil' end) ProductType
                from OmTrSalesFakturPolisi a
				LEFT JOIN omTrSalesReqDetail b
					ON a.FakturPolisiNo = b.FakturPolisiNo
				INNER JOIN gnMstCoProfile c
					ON a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
				INNER JOIN omMstRefference d
					ON a.ColourCode = d.RefferenceCode AND d.RefferenceType = 'COLO'
                where a.CompanyCode = '{0}'
                 AND a.BranchCode = '{1}'
                 AND a.ChassisCode+convert(varchar,a.ChassisNo)+EngineCode+convert(varchar,EngineNo) not in(select ChassisCode+convert(varchar,ChassisNo)+EngineCode+convert(varchar,EngineNo) from OmTrSalesReceipt)
                 AND a.Status <> '2'
				", Uid.CompanyCode, Uid.BranchCode);

            var queryable = ctx.Context.Database.SqlQuery<FakturPolisiBrowse>(query).AsQueryable();
            return (queryable);

        }

        #endregion

        [HttpGet]
        public IQueryable<omUtlFpolReqView> GetFakturPolisiDCSHeader()
        {
            var Uid = CurrentUser();
            var query = string.Format(@"
               select distinct a.DealerCode,b.CustomerName,a.BatchNo , case when a.Status = 1 then 'Posted' else 'Un-Posted' end Status,a.CreatedDate
                                    from omUtlFpolReq a
                                    left join gnMstCustomer b on a.CompanyCode = b.CompanyCode
                                        and (a.DealerCode = b.CustomerCode or a.DealerCode = b.StandardCode)
                                   where a.CompanyCode = '{0}' 
                                        and a.BranchCode = '{1}'
				", Uid.CompanyCode, Uid.BranchCode);

            var queryable = ctx.Context.Database.SqlQuery<omUtlFpolReqView>(query).AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<ITS> ItsList()
        {
            var Uid = CurrentUser();
            var query = string.Format(@"
               exec uspfn_SelectITSNo @CompanyCode = '{0}', @BranchCode = '{1}'
				", Uid.CompanyCode, Uid.BranchCode);

            var queryable = ctx.Context.Database.SqlQuery<ITS>(query).AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<MstCustomerView> Select4LookupCustomer2(string ProfitCenterCode)
        {
            var Uid = CurrentUser();
            var query = string.Format(@"
               SELECT a.CustomerCode
                    , a.CustomerName
                    , isnull(a.Address1,'') + ' ' + isnull(a.Address2,'') + ' ' + isnull(a.Address3,'') + ' ' + isnull(a.Address4,'') as Address
                    , isnull(a.Address1,'') Address1
                    , isnull(a.Address2,'') Address2
                    , isnull(a.Address3,'') Address3
                    , isnull(a.Address4,'') Address4
                    , ISNULL(c.ParaValue,'') AS TopCode
                    , ISNULL(c.ParaValue,'') as TOPDesc
                    , b.TopCode as TOPCD
                    , b.GroupPriceCode
                    , d.RefferenceDesc1 as GroupPriceDesc
                    , b.SalesCode
                FROM gnMstCustomer a
                LEFT JOIN gnMstCustomerProfitCenter b ON b.CompanyCode = a.CompanyCode 
                    AND b.CustomerCode = a.CustomerCode
                LEFT JOIN gnMstLookUpDtl c ON c.CompanyCode=a.CompanyCode
                    AND c.LookUpValue = b.TOPCode    
                    AND c.CodeID = 'TOPC'
                LEFT JOIN omMstRefference d ON a.CompanyCode = d.CompanyCode 
                    AND d.RefferenceType = 'GRPR' 
                    AND d.RefferenceCode = b.GroupPriceCode
                WHERE a.CompanyCode = '{0}'
                    AND b.BranchCode = '{1}'
                    AND b.ProfitCenterCode ='{2}'
	                AND a.Status = '1'
                    AND b.SalesType = '1'
	                AND b.isBlackList = 0
                ORDER BY CustomerCode
				", Uid.CompanyCode, Uid.BranchCode, "100");

            var queryable = ctx.Context.Database.SqlQuery<MstCustomerView>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<MstCustomerView> Select4LookupCustomer(string ProfitCenterCode)
        {
            var Uid = CurrentUser();
            var query = string.Format(@"
               SELECT TableA.CustomerCode, TableA.CustomerName, TableA.TopCode, TableA.TOPDesc, TableA.TOPCD, TableA.GroupPriceCode, TableA.RefferenceDesc1 as GroupPriceDesc, TableA.SalesCode
                  FROM    (SELECT a.CustomerCode, a.CustomerName, 
                                  a.Address1 + ' ' + a.Address2 + ' ' + a.Address3 + ' ' + a.Address4 as Address,
                                  b.TOPCode + '||'
                                  + (SELECT c.LookUpValueName
                                  FROM gnMstLookUpDtl c
                                  WHERE c.CodeID = 'TOPC'
                                  AND c.LookUpValue = b.TOPCode)  AS TOPDesc, 
                                  (SELECT c.ParaValue
                                  FROM gnMstLookUpDtl c
                                  WHERE c.CodeID = 'TOPC'
                                  AND c.LookUpValue = b.TOPCode)  AS TOPCode,
                                  b.TOPCode  AS TOPCD, b.CreditLimit, a.CompanyCode, b.BranchCode, 
                                  b.ProfitCenterCode, b.GroupPriceCode, c.RefferenceDesc1, b.SalesCode
                             FROM gnMstCustomer a
                            LEFT JOIN gnMstCustomerProfitCenter b ON b.CompanyCode = a.CompanyCode AND b.CustomerCode = a.CustomerCode
                            LEFT JOIN omMstRefference c ON a.CompanyCode = c.CompanyCode AND c.RefferenceType = 'GRPR' AND c.RefferenceCode = b.GroupPriceCode
                            WHERE a.CompanyCode = b.CompanyCode
                                  AND a.CompanyCode = b.CompanyCode
                                  AND b.CompanyCode = '{0}'
                                  AND b.BranchCode = '{1}'
                                  AND b.ProfitCenterCode = '{2}'
			                      AND a.Status = '1'
                                  AND b.SalesType = '0'
			                      AND b.isBlackList = 0
                                  AND b.CreditLimit > 0) TableA
                       LEFT JOIN
                          gnTrnBankBook c
                       ON TableA.CompanyCode = c.CompanyCode
                          AND TableA.BranchCode = c.BranchCode
                          AND TableA.ProfitCenterCode = c.ProfitCenterCode
                          AND TableA.CustomerCode = c.CustomerCode
                 WHERE TableA.CreditLimit >
                          (ISNULL (c.SalesAmt, 0) - ISNULL (c.ReceivedAmt, 0))
                ORDER BY TableA.CustomerCode ASC
				", Uid.CompanyCode, Uid.BranchCode, "100");

            var queryable = ctx.Context.Database.SqlQuery<MstCustomerView>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<MstCustomerView> Select4LookupCustomerPO(string ProfitCenterCode)
        {
            var Uid = CurrentUser();
            var query = string.Format(@"
               	    SELECT 
					a.CustomerCode
					, a.CustomerName
					, isnull(a.Address1,'') + ' ' + isnull(a.Address2,'') + ' ' + isnull(a.Address3,'') + ' ' + isnull(a.Address4,'') as Address
                    , isnull(a.Address1,'') Address1
                    , isnull(a.Address2,'') Address2
                    , isnull(a.Address3,'') Address3
                    , isnull(a.Address4,'') Address4
                    , ISNULL(c.ParaValue,'') AS TopCode
                    , ISNULL(c.ParaValue,'') as TOPDesc
                    , b.TopCode as TOPCD
                    , b.GroupPriceCode
                    , d.RefferenceDesc1 as GroupPriceDesc
                    , b.SalesCode
                FROM gnMstCustomer a 
                LEFT JOIN gnMstCustomerProfitCenter b ON b.CompanyCode = a.CompanyCode 
                    AND b.CustomerCode = a.CustomerCode
                LEFT JOIN gnMstLookUpDtl c ON c.CompanyCode=a.CompanyCode
                    AND c.LookUpValue = b.TOPCode    
                    AND c.CodeID = 'TOPC'
                LEFT JOIN omMstRefference d ON a.CompanyCode = d.CompanyCode 
                    AND d.RefferenceType = 'GRPR' 
                    AND d.RefferenceCode = b.GroupPriceCode
                WHERE a.CompanyCode = '{0}'
                    AND b.BranchCode = '{1}'
                    AND b.ProfitCenterCode ='{2}'
                    AND b.SalesType = '0'
	                AND b.isBlackList = 0
                ORDER BY a.CustomerCode ASC"
                , Uid.CompanyCode, Uid.BranchCode, "100");

            var queryable = ctx.Context.Database.SqlQuery<MstCustomerView>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SalesmanIDLookup> Select4LookupSalesman()
        {
            var Uid = CurrentUser();
            //            var query = string.Format(@"
            //               SELECT 
            //                    a.EmployeeID, a.EmployeeName, a.TitleCode
            //                    , (SELECT LookUpValueName FROM gnMstLookupDtl WHERE CompanyCode = a.CompanyCode and CodeID = 'TITL' AND LookUpValue = a.TitleCode) AS TitleName
            //                FROM GnMstEmployee a
            //                WHERE a.CompanyCode = '{0}' 
            //                    AND a.BranchCode  = '{1}'
            //				", Uid.CompanyCode, Uid.BranchCode);

            //            var queryable = ctx.Context.Database.SqlQuery<SalesmanIDLookup>(query).AsQueryable();

            var emp = ctx.Context.HrEmployees.Where(x => x.CompanyCode == Uid.CompanyCode).FirstOrDefault();
            IQueryable<SalesmanIDLookup>queryable = null;

            if (Uid.CoProfile.ProductType == "4W")
            {
                if (emp != null)
                {
                    queryable = from a in ctx.Context.HrEmployeeViews
                                where a.CompanyCode == Uid.CompanyCode && a.Department == "SALES"
                                select new SalesmanIDLookup()
                                {
                                    EmployeeID = a.EmployeeID,
                                    EmployeeName = a.EmployeeName,
                                    TitleName = a.PositionName
                                };
                }
                else
                {
                    queryable = from a in ctx.Context.Employees
                                join b in ctx.Context.LookUpDtls on a.TitleCode equals b.LookUpValue
                                where a.CompanyCode == Uid.CompanyCode && a.BranchCode == Uid.BranchCode && b.CodeID == "TITL"
                                select new SalesmanIDLookup()
                                {
                                    EmployeeID = a.EmployeeID,
                                    EmployeeName = a.EmployeeName,
                                    TitleName = b.LookUpValueName
                                };
                }
            }
            else 
            {
                if (emp != null)
                {
                    queryable = from a in ctx.Context.HrEmployeeViews
                                where a.CompanyCode == Uid.CompanyCode && a.Department == "SALES"
                                select new SalesmanIDLookup()
                                {
                                    EmployeeID = a.EmployeeID,
                                    EmployeeName = a.EmployeeName,
                                    TitleName = a.PositionName
                                };
                }
                else
                {
                    queryable = from a in ctx.Context.Employees
                                join b in ctx.Context.LookUpDtls on a.TitleCode equals b.LookUpValue
                                where a.CompanyCode == Uid.CompanyCode && a.BranchCode == Uid.BranchCode && b.CodeID == "TITL"
                                select new SalesmanIDLookup()
                                {
                                    EmployeeID = a.EmployeeID,
                                    EmployeeName = a.EmployeeName,
                                    TitleName = b.LookUpValueName
                                };
                }
            }
            
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SOMerk> Merk(string SalesModelCode, bool isSuzuki)
        {
            var issuzuki = isSuzuki == true ? "1" : "0";
            var queryable = ctx.Context.Database.SqlQuery<SOMerk>("Exec uspfn_CheckModelVehicle '" + SalesModelCode + "','" + issuzuki + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<TrSalesSOView> SalesSO()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.TrSalesSOViews.Where(p => p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode);
            return (queryable);
        }

        [HttpGet]
        public IQueryable<ColourSO> SelectColour4SO(string SalesModelCode, string InquiryNumber)
        {
            var Uid = CurrentUser();
            bool independent = false;
            bool otom = true;
            string rcd = ctx.Context.Database.SqlQuery<string>("SELECT ParaValue from gnMstLookUpDtl WHERE CodeID='OTOM' AND LookUpValue='UNIT' AND CompanyCode='" + Uid.CompanyCode + "'").FirstOrDefault();
            if (rcd != null && rcd == "0")
            {
                otom = false;
            }

            IQueryable<ColourSO> queryable = null;

            if (Uid.CompanyCode == CompanyMD && Uid.BranchCode == BranchMD)
            {
                independent = true;
            }

            int inumber = Convert.ToInt32(InquiryNumber);
            if (independent || !otom)
            {
                if (InquiryNumber == null)
                {
                    queryable = from a in ctx.Context.OmMstModelColours
                                join b in ctx.Context.MstRefferences
                                on new { a.CompanyCode, a.ColourCode, RefferenceType = "COLO" } equals new { b.CompanyCode, ColourCode = b.RefferenceCode, b.RefferenceType }
                                join c in ctx.Context.OmTrInventQtyVehicles
                                on new { a.CompanyCode, BranchCode = Uid.BranchCode, a.ColourCode, SalesModelCode = SalesModelCode } equals new { c.CompanyCode, c.BranchCode, c.ColourCode, c.SalesModelCode }
                                where a.CompanyCode == Uid.CompanyCode && a.SalesModelCode == SalesModelCode && c.EndingAV > 0
                                select new ColourSO()
                                {
                                    ColourCode = a.ColourCode,
                                    ColourDesc = b.RefferenceDesc1
                                };
                }
                else
                {
                    string sqlstr = ctx.Context.PmKdps.Where(a => a.CompanyCode == Uid.CompanyCode && a.BranchCode == Uid.BranchCode && a.InquiryNumber == inumber).FirstOrDefault().ColourCode;
                    queryable = from a in ctx.Context.OmMstModelColours
                                join b in ctx.Context.MstRefferences
                                on new { a.CompanyCode, a.ColourCode, RefferenceType = "COLO" } equals new { b.CompanyCode, ColourCode = b.RefferenceCode, b.RefferenceType }
                                where a.CompanyCode == Uid.CompanyCode && a.SalesModelCode == SalesModelCode && a.ColourCode == sqlstr
                                select new ColourSO()
                                {
                                    ColourCode = a.ColourCode,
                                    ColourDesc = b.RefferenceDesc1
                                };
                }
            }
            else
            {
                if (InquiryNumber == null)
                {
                    var colo = (from a in ctxMD.Context.OmTrInventQtyVehicles
                               where a.CompanyCode == CompanyMD && a.BranchCode == UnitBranchMD && a.SalesModelCode == SalesModelCode && a.EndingAV > 0
                               select a.ColourCode).Distinct().ToList();

                    queryable = (from a in ctx.Context.OmMstModelColours
                                join b in ctx.Context.MstRefferences
                                on new { a.CompanyCode, a.ColourCode, RefferenceType = "COLO" } equals new { b.CompanyCode, ColourCode = b.RefferenceCode, b.RefferenceType }
                                where a.CompanyCode == Uid.CompanyCode && a.SalesModelCode == SalesModelCode && colo.Contains(a.ColourCode)
                                select new ColourSO()
                                {
                                    ColourCode = a.ColourCode,
                                    ColourDesc = b.RefferenceDesc1
                                }).Distinct();
                }
                else
                {
                    string sqlstr = ctx.Context.PmKdps.Where(a => a.CompanyCode == Uid.CompanyCode && a.BranchCode == Uid.BranchCode && a.InquiryNumber == inumber).FirstOrDefault().ColourCode;
                    queryable = (from a in ctx.Context.OmMstModelColours
                                join b in ctx.Context.MstRefferences
                                on new { a.CompanyCode, a.ColourCode, RefferenceType = "COLO" } equals new { b.CompanyCode, ColourCode = b.RefferenceCode, b.RefferenceType }
                                where a.CompanyCode == Uid.CompanyCode && a.SalesModelCode == SalesModelCode && a.ColourCode == sqlstr
                                select new ColourSO()
                                {
                                    ColourCode = a.ColourCode,
                                    ColourDesc = b.RefferenceDesc1
                                }).Distinct();
                }
            }


            
            //            var query = string.Format(@"
            //               DECLARE @Colour AS VARCHAR(15)
            //                SET     @Colour = (SELECT ColourCode FROM pmKDP WHERE CompanyCode = '{0}' AND BranchCode = '{1}' AND InquiryNumber = '{3}')
            //
            //                SELECT a.ColourCode, (SELECT b.RefferenceDesc1
            //                       FROM omMstRefference b
            //                       WHERE b.RefferenceCode = a.ColourCode
            //                       AND b.CompanyCode = a.CompanyCode AND b.RefferenceType = 'COLO')  AS colourDesc, a.Remark
            //                  FROM omMstModelColour a
            //                 WHERE a.CompanyCode = '{0}' 
            //                    AND a.SalesModelCode = '{2}' 
            //                    AND (CASE WHEN '{3}' = '' THEN '' ELSE a.ColourCode END) = (CASE WHEN '{3}' = '' THEN '' ELSE @Colour END) 
            //				", Uid.CompanyCode, Uid.BranchCode, SalesModelCode, InquiryNumber);

            //            var queryable = ctx.Context.Database.SqlQuery<ColourSO>(query).AsQueryable();
            return (queryable);
        }


        [HttpGet]
        public IQueryable<ColourSO> SelectColour4SOPO(string SalesModelCode, string SalesModelYear, string PONo, string NOPlg)
        {
            var Uid = CurrentUser();
            string CompanySD = ctx.Context.Database.SqlQuery<string>("SELECT CompanyCode FROM gnmstcompanymapping WHERE BranchCode='" + NOPlg + "'").FirstOrDefault();
            string DbSD = ctx.Context.Database.SqlQuery<string>("SELECT DbName FROM gnmstcompanymapping WHERE CompanyCode='" + CompanySD + "' AND BranchCode='" + NOPlg + "'").FirstOrDefault();
//            var Qry = String.Format(@"SELECT a.ColourCode, b.RefferenceDesc1 AS ColourDesc, a.Quantity AS Quantity FROM {4}..omTrPurchasePOModelColour a INNER JOIN {4}..omMstRefference b
//                                    ON b.CompanyCode = a.CompanyCode AND b.RefferenceCode = a.ColourCode AND b.RefferenceType='COLO'
//                                    WHERE a.CompanyCode='{0}' AND a.BranchCode='{1}' AND a.PONo='{2}' AND a.SalesModelCode='{3}'", CompanySD, NOPlg, PONo, SalesModelCode, DbSD);

            var Qry = String.Format(@"SELECT a.ColourCode, b.RefferenceDesc1 AS ColourDesc, (a.Quantity - (SELECT ISNULL(SUM(d.Quantity),0) FROM omTrSalesSO c  
                                    INNER JOIN omTrSalesSOModelColour d ON d.CompanyCode = c.CompanyCode AND d.BranchCode = c.BranchCode AND d.SONo = c.SONo WHERE c.RefferenceNo=a.PONo AND 
                                    c.CompanyCode='{5}' AND c.BranchCode='{6}' AND d.SalesModelCode=a.SalesModelCode AND d.SalesModelYear=a.SalesModelYear AND d.ColourCode=a.ColourCode)) AS Quantity FROM {4}..omTrPurchasePOModelColour a INNER JOIN {4}..omMstRefference b
                                    ON b.CompanyCode = a.CompanyCode AND b.RefferenceCode = a.ColourCode AND b.RefferenceType='COLO'
                                    WHERE a.CompanyCode='{0}' AND a.BranchCode='{1}' AND a.PONo='{2}' AND a.SalesModelCode='{3}' AND a.SalesModelYear='{7}' AND
                                    (a.Quantity - (SELECT ISNULL(SUM(d.Quantity),0) FROM omTrSalesSO c  
                                    INNER JOIN omTrSalesSOModelColour d ON d.CompanyCode = c.CompanyCode AND d.BranchCode = c.BranchCode AND d.SONo = c.SONo WHERE c.RefferenceNo=a.PONo AND 
                                    c.CompanyCode='{5}' AND c.BranchCode='{6}' AND d.SalesModelCode=a.SalesModelCode AND d.SalesModelYear=a.SalesModelYear AND d.ColourCode=a.ColourCode)) > 0"
                                    , CompanySD, NOPlg, PONo, SalesModelCode, DbSD, Uid.CompanyCode, Uid.BranchCode, SalesModelYear);
            var queryable = ctx.Context.Database.SqlQuery<ColourSO>(Qry).AsQueryable();

            return (queryable);
        }


        [HttpGet]
        public IQueryable<ChassisSO2> SelectChassis4SO(string SalesModelCode, decimal? SalesModelYear, string ColourCode, string WareHouseCode, string ChassisCode)
        {
            var Uid = CurrentUser();
            var query = String.Empty;
            var gnmapping = ctx.Context.CompanyMappings.Where(p => p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode).FirstOrDefault();
            bool otom = true;
            string rcd = ctx.Context.Database.SqlQuery<string>("SELECT ParaValue from gnMstLookUpDtl WHERE CodeID='OTOM' AND LookUpValue='UNIT' AND CompanyCode='" + Uid.CompanyCode + "'").FirstOrDefault();
            if (rcd != null && rcd == "0")
            {
                otom = false;
            }

            //if (gnmapping != null)
            //{
            //    //var notExist = from ne1 in ctx.Context.omTrSalesSOVins
                //               join ne2 in ctx.Context.OmTRSalesSOs on new { ne1.CompanyCode, ne1.BranchCode, ne1.SONo, ne1.ChassisCode, ne1.ChassisNo }
                //               equals new { ne2.CompanyCode, ne2.BranchCode, ne2.SONo, t1.ChassisCode, t1.ChassisNo } into ne2
                //               from a in ne2.DefaultIfEmpty()
                //               where new string[] {"0","1"}.Contains (a.Status)
                //queryable = from t1 in ctxMD.Context.OmMstVehicles
                //                join t2 in ctxMD.Context.OmTrInventQtyVehicles.Where(x => x.CompanyCode == gnmapping.CompanyMD && 
                //                    x.SalesModelCode == SalesModelCode && x.SalesModelYear == SalesModelYear && x.ColourCode == ColourCode && x.WarehouseCode == WareHouseCode)
                //                    .OrderByDescending(x => x.Year).OrderByDescending(x => x.Month).Take(1)
                //                    on new { t1.CompanyCode, t1.SalesModelCode, SalesModelYear=t1.SalesModelYear.Value, t1.ColourCode, t1.WarehouseCode }
                //                    equals new { t2.CompanyCode, t2.SalesModelCode, t2.SalesModelYear, t2.ColourCode, t2.WarehouseCode }

                //                where t1.CompanyCode == gnmapping.CompanyMD && t2.BranchCode == gnmapping.UnitBranchMD && t1.Status == "0" &&
                //                    t1.IsActive && t1.SalesModelCode == SalesModelCode && t1.SalesModelYear == SalesModelYear &&
                //                    t1.ColourCode == ColourCode && t1.WarehouseCode == WareHouseCode && t1.ChassisCode == ChassisCode && t2.EndingAV > 0 &&
                //                    !(from ne1 in ctx.Context.omTrSalesSOVins
                //                       join ne2 in ctx.Context.OmTRSalesSOs on new { ne1.CompanyCode, ne1.BranchCode, ne1.SONo, ne1.ChassisCode, ChassisNo = ne1.ChassisNo.Value }
                //                       equals new { ne2.CompanyCode, ne2.BranchCode, ne2.SONo, t1.ChassisCode, t1.ChassisNo } into ne2
                //                       from a in ne2.DefaultIfEmpty()
                //                       where new string[] {"0","1"}.Contains (a.Status)
                //                       select 1
                //                    ).Any()
                //                select new ChassisSO2()
                //                {
                //                    ChassisNo = t1.ChassisNo.ToString(),
                //                    EngineCode = t1.EngineCode,
                //                    EngineNo = t1.EngineNo.ToString(),
                //                    ServiceBookNo = t1.ServiceBookNo,
                //                    KeyNo = t1.KeyNo
                //                };

                if ((Uid.CompanyCode == CompanyMD) && (Uid.BranchCode == BranchMD) || !otom || gnmapping == null)
                {
                    query = string.Format(@"
                            SELECT convert(varchar, a.ChassisNo, 10) ChassisNo, a.EngineCode, convert(varchar, a.EngineNo, 10) EngineNo, a.ServiceBookNo, a.KeyNo
                            FROM omMstVehicle a
	                            INNER JOIN (
		                            SELECT TOP 1 * 
		                             FROM omTrInventQtyVehicle a
		                             WHERE a.CompanyCode = '{0}' 
			                               AND a.BranchCode = '{1}'
			                               AND a.SalesModelCode = '{2}'
			                               AND a.SalesModelYear = '{3}' 
			                               AND a.ColourCode = '{5}'
			                               AND a.WarehouseCode = '{6}'
		                            ORDER BY a.[Year] DESC, a.[Month] DESC
	                            ) b on a.CompanyCode=b.CompanyCode 
	                            AND a.SalesModelCode=b.SalesModelCode AND a.SalesModelYear=b.SalesModelYear 
	                            AND a.ColourCode=b.ColourCode AND a.WarehouseCode=b.WarehouseCode
                            WHERE a.CompanyCode = '{0}' 
	                             AND b.BranchCode='{1}' 
	                             AND a.Status = '0'
	                             AND a.IsActive = '1'
	                             AND a.SalesModelCode = '{2}' 
	                             AND a.SalesModelYear = '{3}'  
	                             AND a.ChassisCode = '{4}' 
	                             AND a.ColourCode = '{5}' 
	                             AND a.WareHouseCode = '{6}' 
	                             AND b.EndingAV > 0
	                             and not exists (
		                            select 1 from omTrSalesSOVin x 
			                            left join omTrSalesSO y on x.CompanyCode=y.CompanyCode and x.BranchCode=y.BranchCode
				                            and x.SONo=y.SONo
		                            where y.Status in ('0','1') and x.ChassisCode=a.ChassisCode and x.ChassisNo=a.ChassisNo
	                            )
                            ORDER BY a.ChassisCode ASC",
                            Uid.CompanyCode, Uid.BranchCode, SalesModelCode, SalesModelYear, ChassisCode, ColourCode, WareHouseCode);
                }
                else
                {
                    query = string.Format(@"
                            SELECT convert(varchar, a.ChassisNo, 10) ChassisNo, a.EngineCode, convert(varchar, a.EngineNo, 10) EngineNo, a.ServiceBookNo, a.KeyNo
                            FROM {7}.dbo.omMstVehicle a
	                            INNER JOIN (
		                            SELECT TOP 1 * 
		                             FROM {7}.dbo.omTrInventQtyVehicle a
		                             WHERE a.CompanyCode = '{0}' 
			                               AND a.BranchCode = '{1}'
			                               AND a.SalesModelCode = '{2}'
			                               AND a.SalesModelYear = '{3}' 
			                               AND a.ColourCode = '{5}'
			                               AND a.WarehouseCode = '{6}'
		                            ORDER BY a.[Year] DESC, a.[Month] DESC
	                            ) b on a.CompanyCode=b.CompanyCode 
	                            AND a.SalesModelCode=b.SalesModelCode AND a.SalesModelYear=b.SalesModelYear 
	                            AND a.ColourCode=b.ColourCode AND a.WarehouseCode=b.WarehouseCode
                            WHERE a.CompanyCode = '{0}' 
	                             AND b.BranchCode='{1}' 
	                             AND a.Status = '0'
	                             AND a.IsActive = '1'
	                             AND a.SalesModelCode = '{2}' 
	                             AND a.SalesModelYear = '{3}'  
	                             AND a.ChassisCode = '{4}' 
	                             AND a.ColourCode = '{5}' 
	                             AND a.WareHouseCode = '{6}' 
	                             AND b.EndingAV > 0
	                             and not exists (
		                            select 1 from omTrSalesSOVin x 
			                            left join omTrSalesSO y on x.CompanyCode=y.CompanyCode and x.BranchCode=y.BranchCode
				                            and x.SONo=y.SONo
		                            where y.Status in ('0','1') and x.ChassisCode=a.ChassisCode and x.ChassisNo=a.ChassisNo
	                            )
                            ORDER BY a.ChassisCode ASC

				", CompanyMD, gnmapping.UnitBranchMD, SalesModelCode, SalesModelYear, ChassisCode, ColourCode, WareHouseCode, gnmapping.DbMD);
                }
            //}
            var result = ctx.Context.Database.SqlQuery<ChassisSO2>(query).AsQueryable();
            return (result);
        }

        [HttpGet]
        public IQueryable<HPPView> SelectPemasok4SO(string ProfitCenterCode, string SalesModelCode, string SalesModelYear)
        {
            var Uid = CurrentUser();
            var query = string.Format(@"
               SELECT DISTINCT a.SupplierCode, a.SupplierName 
                  FROM gnMstSupplier a, gnMstSupplierProfitCenter b, omMstBBNKIR c
                 WHERE a.CompanyCode = b.CompanyCode
                       AND b.CompanyCode = c.CompanyCode
                       AND a.SupplierCode = b.SupplierCode
                       AND b.SupplierCode = c.SupplierCode
                       AND a.CompanyCode = '{0}'
                       AND c.BranchCode = '{1}'
                       AND b.ProfitCenterCode = '{2}'
                       AND c.Status = '1'
                       AND c.SalesModelCode = '{3}'
                       AND c.SalesModelYear = '{4}'
                ORDER BY a.SupplierCode ASC 
				", Uid.CompanyCode, Uid.BranchCode, ProfitCenterCode, SalesModelCode, SalesModelYear);

            var queryable = ctx.Context.Database.SqlQuery<HPPView>(query).AsQueryable();
            return (queryable);
        }


        [HttpGet]
        public IQueryable<CitySO> SelectCity4SO(string SupplierCode, string SalesModelCode, string SalesModelYear)
        {
            var Uid = CurrentUser();
            var query = string.Format(@"
               SELECT DISTINCT a.LookUpValue CityCode, a.LookUpValueName CityDesc, b.BBN, b.KIR
                  FROM gnMstLookUpDtl a, omMstBBNKIR b
                 WHERE a.CompanyCode = b.CompanyCode
                       AND a.CompanyCode = '{0}'
                       AND a.LookUpValue = b.CityCode
                       AND b.BranchCode = '{1}'
                       AND a.CodeID = 'CITY'
                       AND b.Status = '1'
                       AND b.SupplierCode = '{2}'
                       AND b.SalesModelCode = '{3}'
                       AND b.SalesModelYear = '{4}'
                ORDER BY a.LookUpValue 
				", Uid.CompanyCode, Uid.BranchCode, SupplierCode, SalesModelCode, SalesModelYear);

            var queryable = ctx.Context.Database.SqlQuery<CitySO>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<AksLL> Select4GroupPrice()
        {
            var Uid = CurrentUser();
            var query = string.Format(@"SELECT a.RefferenceCode, a.RefferenceDesc1
                                FROM dbo.omMstRefference a
                                WHERE a.CompanyCode = '{0}'
                                AND a.RefferenceType = 'OTHS'
                                AND a.Status != '0'", Uid.CompanyCode);

            var queryable = ctx.Context.Database.SqlQuery<AksLL>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<ItemNewSO> Select4LookupItemNew()
        {
            var Uid = CurrentUser();
            var companyMapping = ctx.Context.CompanyMappings.Find(user.CompanyCode,user.BranchCode);
            string dbName = companyMapping.DbMD.ToString();
            string cmpCode = companyMapping.CompanyMD.ToString();
            string bchCode = companyMapping.BranchMD.ToString();

            var query = string.Format(@"SELECT itemInfo.PartNo
                    , ISNULL((item.OnHand - (item.AllocationSP + item.AllocationSR + item.AllocationSL)- (item.ReservedSP + item.ReservedSR + item.ReservedSL)), 0)  AS Available
                    , ISNULL(itemPrice.RetailPriceInclTax, 0)
                    , itemInfo.PartName
                    , (CASE itemInfo.Status
                        WHEN 1 THEN 'Aktif' ELSE 'Tidak Aktif'
                       END)  AS Status
                    , dtl.LookUpValueName as JenisPart
                    , ISNULL(itemPrice.RetailPrice, 0)  AS NilaiPart
                    , ISNULL(itemPrice.CostPrice, 0)
                FROM  {2}..spMstItemInfo itemInfo                    
                LEFT JOIN {2}..spMstItems item ON item.CompanyCode = itemInfo.CompanyCode
                    AND item.BranchCode = '{1}'
                    AND item.PartNo = itemInfo.PartNo
                LEFT JOIN {2}..spMstItemPrice itemPrice ON itemPrice.CompanyCode = itemInfo.CompanyCode
                    AND itemPrice.BranchCode = '{1}'
                    AND itemPrice.PartNo = item.PartNo
                LEFT JOIN {2}..GnMstLookUpDtl dtl ON item.companyCode = dtl.companyCode 
                    AND dtl.CodeId = 'TPGO'
                    AND dtl.LookUpValue = item.TypeOfGoods                    
                WHERE itemInfo.CompanyCode = '{0}'
                    AND itemInfo.Status = '1'
                    AND (item.OnHand - (item.AllocationSP + item.AllocationSR + item.AllocationSL)
                        - (item.ReservedSP + item.ReservedSR + item.ReservedSL)) > 0", cmpCode, bchCode, dbName);

            var queryable = ctx.Context.Database.SqlQuery<ItemNewSO>(query).AsQueryable();
            return (queryable);
        }
        [HttpGet]
        public IQueryable<SKPKFromDrafSO> SelectDraftSO()
        {
            var Uid = CurrentUser();
            var query = string.Format(@"
               select distinct a.DraftSONo,a.DraftSODate,a.CustomerCode,isnull(c.CustomerName,'') CustomerName,ProspectNo
                ,a.Salesman,a.LeasingCo,a.Installment,a.FinalPaymentDate,a.CommissionAmt,a.RequestDate,a.Remark, 
                a.GroupPriceCode, f.RefferenceDesc1 GroupPriceName
                ,d.EmployeeName as SalesmanName, a.TOPCode, a.TOPDays
                from omTrSalesDraftSO a
                inner join omTrSalesDraftSOModel b 
	                on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode and a.DraftSONo=b.DraftSONo
                left join gnMstCustomer c 
	                on a.CompanyCode=c.CompanyCode and a.CustomerCode=c.CustomerCode
                left join gnmstemployee d
	                on d.employeeID = a.Salesman
                left join ommstrefference f
	                on f.RefferenceCode = a.GroupPriceCode
                where a.CompanyCode='{0}' and a.BranchCode='{1}'
	                and b.QuantitySO < b.QuantityDraftSO
                    and a.Status='2'
                    and a.ProspectNo = a.ProspectNo 
				", Uid.CompanyCode, Uid.BranchCode);

            var queryable = ctx.Context.Database.SqlQuery<SKPKFromDrafSO>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<MstCustomerView> select4LookupTo(string ProfitCenterCode)
        {
            var Uid = CurrentUser();
            //            var query = string.Format(@"
            //               SELECT a.CustomerCode, a.CustomerName
            //                  FROM gnMstCustomer a 
            //                INNER JOIN gnMstCustomerProfitCenter b
            //                  ON a.CompanyCode = b.CompanyCode AND 
            //                     a.CustomerCode = b.CustomerCode AND
            //                     b.BranchCode = '{1}'
            //                WHERE a.CompanyCode = '{0}' AND 
            //                      b.ProfitCenterCode = '{2}' 
            //				", Uid.CompanyCode, Uid.BranchCode, ProfitCenterCode);
            //            var queryable = ctx.Context.Database.SqlQuery<MstCustomerView>(query).AsQueryable();
            var queryable = from a in ctx.Context.GnMstCustomer
                            join b in ctx.Context.CustomerProfitCenters
                            on new { a.CompanyCode, Uid.BranchCode, a.CustomerCode } equals new { b.CompanyCode, b.BranchCode, b.CustomerCode }
                            where a.CompanyCode == Uid.CompanyCode && b.ProfitCenterCode == ProfitCenterCode
                            select new MstCustomerView()
                            {
                                CustomerCode = a.CustomerCode,
                                CustomerName = a.CustomerName,
                            };
            return (queryable);

        }

        [HttpGet]
        public IQueryable<select4model> Select4ModelSOPO(string SONo, string PONo, string NOPlg)
        {
            var Uid = CurrentUser();
            string CompanySD = ctx.Context.Database.SqlQuery<string>("SELECT CompanyCode FROM gnmstcompanymapping WHERE BranchCode='" + NOPlg + "'").FirstOrDefault();
            string DbSD = ctx.Context.Database.SqlQuery<string>("SELECT DbName FROM gnmstcompanymapping WHERE CompanyCode='" + CompanySD + "' AND BranchCode='" + NOPlg + "'").FirstOrDefault();
            var query = string.Format(@"
                SELECT a.SalesModelCode, b.SalesModelDesc FROM {3}..omTrPurchasePOModel a INNER JOIN {3}..omMstModel b ON a.CompanyCode=b.CompanyCode AND a.SalesModelCode=b.SalesModelCode
                WHERE a.CompanyCode = '{0}' AND a.BranchCode='{1}' AND a.PONo='{2}' 
                AND a.SalesModelCode NOT IN (SELECT d.SalesModelCode FROM omTrSalesSO c INNER JOIN omTrSalesSOModel d ON d.CompanyCode=c.CompanyCode AND d.BranchCode=c.BranchCode AND d.SONo=c.SONo 
                AND c.CompanyCode='{4}' AND c.BranchCode='{5}' AND c.RefferenceNo=a.PONo AND (SELECT SUM(x.QuantitySO) FROM omTrSalesSOModel x WHERE x.CompanyCode = c.CompanyCode
				AND x.BranchCode=c.BranchCode AND x.SONo = c.SONo) = a.QuantityPO)
                ORDER BY a.SalesModelCode ASC   
				", CompanySD, NOPlg, PONo, DbSD, Uid.CompanyCode, Uid.BranchCode);

            //Tambahkan validasi untuk so yang parsial (1 PO bisa banyak SO)

            var queryable = ctx.Context.Database.SqlQuery<select4model>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<select4model> Select4ModelYearPO(string PONo, string NOPlg, string SalesModel)
        {
            var Uid = CurrentUser();
            string CompanySD = ctx.Context.Database.SqlQuery<string>("SELECT CompanyCode FROM gnmstcompanymapping WHERE BranchCode='" + NOPlg + "'").FirstOrDefault();
            string DbSD = ctx.Context.Database.SqlQuery<string>("SELECT DbName FROM gnmstcompanymapping WHERE CompanyCode='" + CompanySD + "' AND BranchCode='" + NOPlg + "'").FirstOrDefault();
            var query = String.Format(@"SELECT a.SalesModelYear, a.SalesModelCode, b.SalesModelDesc, c.ChassisCode,  convert(int,count(c.EngineNo)) Qty FROM {3}..omTrPurchasePOModel a 
                                        INNER JOIN {3}..omMstModel b ON a.CompanyCode=b.CompanyCode AND a.SalesModelCode=b.SalesModelCode
                                        INNER JOIN omMstVehicle c ON c.CompanyCode='{4}' AND c.SalesModelCode = a.SalesModelCode 
                                        AND c.SalesModelYear = a.SalesModelYear WHERE a.CompanyCode = '{0}' AND a.BranchCode='{1}' AND a.PONo='{2}' AND a.SalesModelCode='{5}' 
                                        AND NOT EXISTS (
	                                        SELECT 1 FROM omTrInventTransferOutDetail x 
	                                        INNER JOIN omTrInventTransferOut y on x.CompanyCode=y.CompanyCode and x.BranchCode=y.BranchCode
	                                        AND x.TransferOutNo=y.TransferOutNo
                                            WHERE y.Status in ('0','1') and x.ChassisCode=c.ChassisCode and x.ChassisNo=c.ChassisNo)
                                        GROUP BY a.SalesModelYear, a.SalesModelCode, b.SalesModelDesc, c.ChassisCode
                                        ORDER BY a.SalesModelYear, a.SalesModelCode ASC", CompanySD, NOPlg, PONo, DbSD, Uid.CompanyCode, SalesModel);                       

            var queryable = ctx.Context.Database.SqlQuery<select4model>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<select4model> Select4ModelSO(string Number)
        {
            var Uid = CurrentUser();
            var query = string.Format(@"
               select a.SalesModelCode,b.SalesModelDesc
                from omTrSalesDraftSOModel a
                    left join omMstModel b on a.CompanyCode=b.CompanyCode and a.SalesModelCode=b.SalesModelCode
                where a.CompanyCode='{0}' and a.BranchCode='{1}' and a.DraftSONo='{2}'
                    and not exists (
                        select 1
                        from omTrSalesSOModel x
                            inner join omTrSalesSO y on x.CompanyCode=y.CompanyCode and x.BranchCode=y.BranchCode
                                and x.SONo=y.SONo
                        where x.CompanyCode=a.CompanyCode and x.BranchCode=a.BranchCode 
                            and x.SalesModelCode=a.SalesModelCode and y.SKPKNo=a.DraftSONo
                    )
                order by a.SalesModelCode asc 
				", Uid.CompanyCode, Uid.BranchCode, Number);

            var queryable = ctx.Context.Database.SqlQuery<select4model>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<select4model> Select4LookupModel(string Number)
        {
            int number = Convert.ToInt32(Number);
            var Uid = CurrentUser();
            var query = string.Empty;
            var queryable = ctx.Context.Database.SqlQuery<select4model>("Select top 1 SalesModelCode from omMstModelYear").AsQueryable();
            //            query = string.Format(@"select a.SalesModelCode, a.SalesModelDesc 
            //                                from omMstModel a
            //                                inner join pmKDP b on a.CompanyCode = a.CompanyCode
            //	                                and b.BranchCode = '{1}'	                            
            //	                                and a.GroupCode = b.TipeKendaraan
            //	                                and a.Transmissiontype = b.Transmisi
            //	                                and a.TypeCode = b.Variant
            //                                where a.CompanyCode = '{0}'
            //                                    and b.InquiryNumber = '{2}'", Uid.CompanyCode, Uid.BranchCode, Number);
            //                queryable = ctx.Context.Database.SqlQuery<select4model>(query).AsQueryable();
            if (Number != null)
            {
                queryable = from a in ctx.Context.MstModels
                            join b in ctx.Context.PmKdps
                            on new { a.CompanyCode, Uid.BranchCode, a.GroupCode, a.TransmissionType, a.TypeCode } equals new { b.CompanyCode, b.BranchCode, GroupCode = b.TipeKendaraan, TransmissionType = b.Transmisi, TypeCode = b.Variant }
                            where a.CompanyCode == Uid.CompanyCode && b.InquiryNumber == number
                            select new select4model()
                               {
                                   SalesModelCode = a.SalesModelCode,
                                   SalesModelDesc = a.SalesModelDesc
                               };
            }
            else queryable = from p in
                                 (
                                    from a in ctx.Context.MstModelYear
                                    where a.Status == "1" || a.Status == "2"
                                    group a by a.SalesModelCode into View
                                    select View.FirstOrDefault()
                                 )
                             select new select4model()
                             {
                                 SalesModelCode = p.SalesModelCode,
                                 SalesModelDesc = p.SalesModelDesc
                             };
            //                query = string.Format(@"SELECT DISTINCT a.SalesModelCode, a.SalesModelDesc
            //                          FROM omMstModelYear a 
            //                          WHERE a.CompanyCode = '{0}' AND a.Status IN ('1', '2') ", Uid.CompanyCode);


            return (queryable);
        }

        [HttpGet]
        public IQueryable<ITSNoDraftSO_2> BrowseITS2W()
        {
            var Uid = CurrentUser();
            // var query = string.Empty;
            //query = string.Format(@"exec uspfn_SelectITSNo '{0}', '{1}' ", Uid.CompanyCode, Uid.BranchCode);
            //var queryable = ctx.Context.Database.SqlQuery<ITSNoDraftSO>(query).AsQueryable();

            var queryable = from p in ctx.Context.ITSNoDraftSOs
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode //&& p.Createdby == Uid.UserId
                            select (p);
            return (queryable);
        }

        [HttpGet]
        public IQueryable<ITSNoDraftSO_4> BrowseITS4W()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.ITSNoDraftSOs4
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            select (p);
            return (queryable);
        }

        //[HttpGet]
        //public IQueryable<select4model>LookupModelPO(string PONo)
        //{

        //    //return (queryable)
        //}

        [HttpGet]
        public IQueryable<modelPONumber> LookupPONumber(string NOPlg )
        {
            var Uid = CurrentUser();
            //var qry = string.Empty;
            string CompanySD = ctx.Context.Database.SqlQuery<string>("SELECT CompanyCode FROM gnmstcompanymapping WHERE BranchCode='" + NOPlg + "'").FirstOrDefault();
            string dbSD = ctx.Context.Database.SqlQuery<string>("SELECT DbName FROM gnmstcompanymapping WHERE CompanyCode='" + CompanySD + "' AND BranchCode='" + NOPlg + "'").FirstOrDefault();
            if (dbSD != null || dbSD != "")
            {
                var qry = string.Format(@"SELECT a.PONo, a.PODate, a.Remark FROM {2}..omTrpurchasePO a 
                                        WHERE a.CompanyCode='{0}' AND a.BranchCode='{1}' AND a.IsLocked=0 AND a.Status='2'", CompanySD, NOPlg, dbSD);
                var queryable = ctx.Context.Database.SqlQuery<modelPONumber>(qry).AsQueryable();
    
                return (queryable);
            }
            return null;
        }

        [HttpGet]
        public IQueryable<select4model> Select4SalesModelYear(string Number, string SalesModelCode)
        {
            var Uid = CurrentUser();

            var query = string.Format(@"SELECT DISTINCT a.SalesModelYear, a.SalesModelDesc,a.ChassisCode  
                        FROM omTrSalesDraftSOModel b
	                        left join omMstModelYear a on a.CompanyCode=b.CompanyCode and a.SalesModelCode=b.SalesModelCode
		                        and a.SalesModelYear=b.SalesModelYear
                        WHERE b.CompanyCode = '{0}'  
	                        and b.BranchCode= '{1}'
	                        and b.DraftSONo= '{2}'
	                        AND b.SalesModelCode = '{3}'
	                        AND a.Status = '1'  
	                        and not exists (
		                        select 1
                                from omTrSalesSOModel x
                                    inner join omTrSalesSO y on x.CompanyCode=y.CompanyCode and x.BranchCode=y.BranchCode
                                        and x.SONo=y.SONo
                                where x.CompanyCode=b.CompanyCode and x.BranchCode=b.BranchCode 
                                    and y.SKPKNo=b.DraftSONo and x.SalesModelCode=b.SalesModelCode
			                        and x.SalesModelYear=b.SalesModelYear
	                        )
                        ORDER BY a.SalesModelYear ASC", Uid.CompanyCode, Uid.BranchCode, Number, SalesModelCode);
            var queryable = ctx.Context.Database.SqlQuery<select4model>(query).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<select4model> select4LookupModelYear(string Number, string SalesModelCode)
        {
            var Uid = CurrentUser();
            //            var query = string.Format(@"
            //                         SELECT DISTINCT a.SalesModelYear,a.SalesModelDesc, a.ChassisCode
            //                         FROM omMstModelYear a 
            //                         LEFT JOIN omMstPriceListSell b 
            //                         ON a.CompanyCode = b.CompanyCode 
            //                         AND a.SalesModelCode = b.SalesModelCode
            //                         AND a.SalesModelYear = b.SalesModelYear 
            //                         AND b.GroupPriceCode = '{2}'
            //                         WHERE a.CompanyCode = '{0}'
            //                         AND a.SalesModelCode = '{1}'
            //                         AND a.Status = '1' 
            //                         ",
            //                         Uid.CompanyCode, SalesModelCode, Number); //number adalah group price code
            //            var queryable = ctx.Context.Database.SqlQuery<select4model>(query).AsQueryable();

            //var queryable = (from u in ctx.Context.MstModelYear
            //                 join p in ctx.Context.OmMstPricelistSells
            //                on new { u.CompanyCode, u.SalesModelCode, u.SalesModelYear, GroupPriceCode = Number } equals new { p.CompanyCode, p.SalesModelCode, p.SalesModelYear, p.GroupPriceCode } into gj
            //                 from x in gj.DefaultIfEmpty()
            //                 where u.CompanyCode == Uid.CompanyCode && u.Status == "1" && u.SalesModelCode == SalesModelCode
            //                 select new select4model()
            //                 {
            //                     SalesModelYear = u.SalesModelYear.ToString(),
            //                     SalesModelDesc = u.SalesModelDesc,
            //                     ChassisCode = u.ChassisCode
            //                 }).Distinct();


            //var queryable = from p in (
            //                        from a in ctx.Context.MstModelYear
            //                        join b in ctx.Context.OmMstPricelistSells
            //                        on new { a.CompanyCode, a.SalesModelCode, a.SalesModelYear } equals new { b.CompanyCode, b.SalesModelCode, b.SalesModelYear }
            //                        where a.CompanyCode == Uid.CompanyCode && a.Status == "1"  && a.SalesModelCode == SalesModelCode
            //                        group a by a.SalesModelCode into View
            //                        from b in View.DefaultIfEmpty()
            //                        select View.FirstOrDefault()
            //                    )
            //                select new select4model()
            //                {
            //                    SalesModelYear = p.SalesModelYear,
            //                    SalesModelDesc = p.SalesModelDesc,
            //                    ChassisCode = p.ChassisCode
            //                };


            var queryable = (from a in ctx.Context.MstModelYear
                             join b in ctx.Context.OmMstPricelistSells.Where(x => x.GroupPriceCode == Number)
           on new { a.CompanyCode, a.SalesModelCode, a.SalesModelYear } equals new { b.CompanyCode, b.SalesModelCode, b.SalesModelYear }
           into view
                             from b in view.DefaultIfEmpty()
                             where a.CompanyCode == Uid.CompanyCode && a.Status == "1" && a.SalesModelCode == SalesModelCode
                             select new select4model
                             {
                                 SalesModelYear = a.SalesModelYear,
                                 SalesModelDesc = a.SalesModelDesc,
                                 ChassisCode = a.ChassisCode
                             }).Distinct();

            return (queryable);
        }

        [HttpGet]
        public IQueryable<LookUpDtlview> WareHouse()
        {
            var Uid = CurrentUser();
            bool otom = true;
            string rcd = ctx.Context.Database.SqlQuery<string>("SELECT ParaValue from gnMstLookUpDtl WHERE CodeID='OTOM' AND LookUpValue='UNIT' AND CompanyCode='" + Uid.CompanyCode + "'").FirstOrDefault();
            if (rcd != null && rcd == "0")
            {
                otom = false;
            }

            //            var query = string.Format(@"
            //               SELECT LookUpValue, LookUpValueName, seqno
            //                FROM gnMstLookUpDtl
            //                WHERE CompanyCode = '{0}'
            //                    AND CodeID = 'MPWH'
            //                    AND ParaValue= '{1}'
            //                ORDER BY SeqNo 
            //				", Uid.CompanyCode, Uid.BranchCode);
            //            var queryable = ctx.Context.Database.SqlQuery<LookUpDtlview>(query).AsQueryable();
            if (!otom)
            {
                var queryable1 = from a in ctx.Context.LookUpDtls
                                where a.CompanyCode == Uid.CompanyCode && a.CodeID == "MPWH" && a.ParaValue == Uid.BranchCode
                                select new LookUpDtlview()
                                {
                                    LookUpValue = a.LookUpValue,
                                    LookUpValueName = a.LookUpValueName,
                                    seqno = a.SeqNo
                                };
                return (queryable1);
            }
            else
            {
                var queryable = from a in ctxMD.Context.LookUpDtls
                                where a.CompanyCode == CompanyMD && a.CodeID == "MPWH" && a.ParaValue == UnitBranchMD
                                select new LookUpDtlview()
                                {
                                    LookUpValue = a.LookUpValue,
                                    LookUpValueName = a.LookUpValueName,
                                    seqno = a.SeqNo
                                };
                return (queryable);
            }
            
        }

        [HttpGet]
        public IQueryable<LookUpDtlview> WareHouseSD()
        {
            var Uid = CurrentUser();
                var queryable1 = from a in ctx.Context.LookUpDtls
                                 where a.CompanyCode == Uid.CompanyCode && a.CodeID == "MPWH" && a.ParaValue == Uid.BranchCode
                                 select new LookUpDtlview()
                                 {
                                     LookUpValue = a.LookUpValue,
                                     LookUpValueName = a.LookUpValueName,
                                     seqno = a.SeqNo
                                 };
                return (queryable1);
        }

        [HttpGet]
        public IQueryable<MstCustomerView> LeasingCo(string ProfitCenterCode)
        {
            var Uid = CurrentUser();
            //            var query = string.Format(@"
            //               SELECT UPPER(a.CustomerCode) as CustomerCode, UPPER(a.CustomerName) as CustomerName
            //                  FROM gnMstCustomer a INNER JOIN gnMstCustomerProfitCenter b
            //                          ON a.CompanyCode = b.CompanyCode AND a.CustomerCode = b.CustomerCode
            //                 WHERE a.CompanyCode = '{0}' 
            //                    AND b.BranchCode = '{1}'
            //                    AND b.ProfitCenterCode = '{2}'
            //                    AND a.CategoryCode = 32 
            //				", Uid.CompanyCode, Uid.BranchCode, ProfitCenterCode);
            //var queryable = ctx.Context.Database.SqlQuery<MstCustomerView>(query).AsQueryable();
            var queryable = from a in ctx.Context.GnMstCustomer
                            join b in ctx.Context.CustomerProfitCenters
                            on new { a.CompanyCode, Uid.BranchCode, a.CustomerCode } equals new { b.CompanyCode, b.BranchCode, b.CustomerCode }
                            where a.CompanyCode == Uid.CompanyCode && b.ProfitCenterCode == ProfitCenterCode && a.CategoryCode == "32"
                            select new MstCustomerView()
                            {
                                CustomerCode = a.CustomerCode,
                                CustomerName = a.CustomerName,
                            };
            return (queryable);
        }

        [HttpGet]
        public IQueryable<GetInquiryBtn> GetInquiryBtn(string Area, string Dealer, string Outlet, string detail)
        {
            var Uid = CurrentUser();
            var query = string.Format(@"uspfn_gnInquiryBtn '{0}','{1}','{2}','{3}'
            ", Area == null ? "" : Area
             , Dealer == null ? "" : Dealer
             , Outlet == null ? "" : Outlet
             , detail == null ? "" : detail);

            var queryable = ctx.Context.Database.SqlQuery<GetInquiryBtn>(query).AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<GetInquirySalesLookUpBtn> GetInquirySalesLookUpBtn(DateTime startDate, DateTime endDate, string area, string dealer, string outlet, string branchHead, string salesHead, string salesCoordinator, string salesman, string detail, string salesType)
        {
            var Uid = CurrentUser();
            var query = string.Format(@"uspfn_OmInquirySalesLookUpBtnWeb '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}'
            ", startDate
            , endDate
            , area != null ? area : ""
            , dealer != null ? dealer : ""
            , outlet != null ? outlet : ""
            , branchHead != null ? branchHead : ""
            , salesHead != null ? salesHead : ""
            , salesCoordinator != null ? salesCoordinator : ""
            , salesman != null ? salesman : ""
            , detail
            , salesType);

            var queryable = ctx.Context.Database.SqlQuery<GetInquirySalesLookUpBtn>(query).AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<MstGroupModelView> GroupModel4LookUp()
        {
            var Uid = CurrentUser();
            var query = string.Format(@"
                Select distinct 1 seqNo, GroupModel 
                from MsMstGroupModel 
                UNION 
                Select 0 SeqNO,'<----Select All---->' GroupModel
                order by SeqNo Asc, GroupModel Asc");
            var queryable = ctx.Context.Database.SqlQuery<MstGroupModelView>(query).AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<MstGroupModelView> TipeKendaraan4LookUp(string TipeKendaraan, string Variant)
        {
            var Uid = CurrentUser();
            var query = string.Format(@"uspfn_InquiryITSStatusBtn '{0}','{1}'
                ", TipeKendaraan, Variant);

            var queryable = ctx.Context.Database.SqlQuery<MstGroupModelView>(query).AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<HPPView> HPPLookup()
        {
            var Uid = CurrentUser();
            //            var query = string.Format(@"
            //                        SELECT 
            //                            a.HPPNo, 
            //                            a.PONo, 
            //                            (SELECT b.SupplierName 
            //                                FROM dbo.gnMstSupplier b
            //                                WHERE a.SupplierCode = b.SupplierCode)  AS supplierName
            //                            , CASE ISNULL(a.Status, 0) WHEN 0 THEN 'OPEN' WHEN 1 THEN 'PRINTED' WHEN 2 THEN 'APPROVED'
            //                                    WHEN 3 THEN 'CANCELED' WHEN 9 THEN 'FINISHED' END AS Status
            //                                , a.RefferenceInvoiceNo, a.RefferenceFakturPajakNo, a.RefferenceFakturPajakDate,a.DueDate,
            //                                a.Remark,
            //                                a.Status, a.SupplierCode, a.RefferenceInvoiceDate, a.HPPDate, a.BillTo
            //                FROM dbo.omTrPurchaseHPP a
            //                WHERE a.CompanyCode = '{0}' AND a.BranchCode = '{1}'
            //                ORDER BY a.HPPNo DESC", Uid.CompanyCode, Uid.BranchCode);
            //            var queryable = ctx.Context.Database.SqlQuery<HPPView>(query).AsQueryable();
            var queryable = from a in ctx.Context.omTrPurchaseHPP
                            join b in ctx.Context.Supplier
                            on new { a.CompanyCode, a.SupplierCode } equals new { b.CompanyCode, b.SupplierCode }
                            where a.CompanyCode == Uid.CompanyCode && a.BranchCode == Uid.BranchCode
                            select new HPPView()
                            {
                                HPPNo = a.HPPNo,
                                PONo = a.PONo,
                                SupplierName = b.SupplierName,
                                RefferenceInvoiceNo = a.RefferenceInvoiceNo,
                                RefferenceFakturPajakNo = a.RefferenceFakturPajakNo,
                                RefferenceFakturPajakDate = a.RefferenceFakturPajakDate,
                                DueDate = a.DueDate,
                                Remark = a.Remark,
                                Status = a.Status,
                                SupplierCode = a.SupplierCode,
                                RefferenceInvoiceDate = a.RefferenceInvoiceDate,
                                HPPDate = a.HPPDate,
                                BillTo = a.BillTo,
                                LockingDate = a.LockingDate.Value
                            };
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SubSalesModelView> ChassisCodeDtlHPP(string PONo, string BPUNo, string SalesModelCode, decimal SalesModelYear)
        {
            var Uid = CurrentUser();
            //            var query = string.Format(@"
            //                SELECT distinct  a.ChassisCode 
            //                FROM dbo.omTrPurchaseBPUDetail a
            //                WHERE a.CompanyCode = '{0}'
            //                    AND a.PONo = '{1}'
            //                    AND a.BPUNo = '{2}'
            //                    AND a.SalesModelCode = '{3}'
            //                    AND a.SalesModelYear = '{4}'
            //                    AND a.StatusHPP = 0
            //                    AND a.isReturn = 0", Uid.CompanyCode, PONo, BPUNo, SalesModelCode, SalesModelYear);
            //            var queryable = ctx.Context.Database.SqlQuery<SubSalesModelView>(query).AsQueryable();
            var queryable = from p in
                                (
                                   from a in ctx.Context.omTrPurchaseBPUDetail
                                   where a.CompanyCode == Uid.CompanyCode
                                   && a.BranchCode == Uid.BranchCode
                                   && a.PONo == PONo
                                   && a.BPUNo == BPUNo
                                   && a.SalesModelCode == SalesModelCode
                                   && a.SalesModelYear == SalesModelYear
                                   && a.StatusHPP == "0"
                                   && a.isReturn == false
                                   group a by a.ChassisCode into View
                                   select View.FirstOrDefault()
                                )
                            select new SubSalesModelView()
                            {
                                ChassisCode = p.ChassisCode
                            };
            return (queryable);
        }

        [HttpGet]
        public IQueryable<InquiryTrPurchaseBPUDetailView> ChassisNoDtlHPP(string PONo, string BPUNo, string SalesModelCode, decimal SalesModelYear, string ChassisCode)
        {
            var Uid = CurrentUser();
            //            var query = string.Format(@" 
            //                SELECT distinct a.ChassisNo, a.EngineCode, a.EngineNo, a.ColourCode, a.BPUSeq
            //                FROM dbo.omTrPurchaseBPUDetail a
            //                WHERE a.CompanyCode = '{0}'
            //                    AND a.PONo = '{1}'
            //                    AND a.BPUNo = '{2}'
            //                    AND a.SalesModelCode = '{3}'
            //                    AND a.SalesModelYear = '{4}'
            //                    AND a.ChassisCode = '{5}'
            //                    AND a.StatusHPP = 0
            //                    AND a.isReturn = 0
            //                ORDER BY a.BPUSeq", Uid.CompanyCode, PONo, BPUNo, SalesModelCode, SalesModelYear, ChassisCode);
            //            var queryable = ctx.Context.Database.SqlQuery<InquiryTrPurchaseBPUDetailView>(query).AsQueryable();
            var queryable = from p in
                                (
                                   from a in ctx.Context.omTrPurchaseBPUDetail
                                   where a.CompanyCode == Uid.CompanyCode
                                   && a.BranchCode == Uid.BranchCode
                                   && a.PONo == PONo
                                   && a.BPUNo == BPUNo
                                   && a.SalesModelCode == SalesModelCode
                                   && a.SalesModelYear == SalesModelYear
                                   && a.StatusHPP == "0"
                                   && a.isReturn == false
                                   group a by a.ChassisNo into View
                                   select View.FirstOrDefault()
                                )
                            select new InquiryTrPurchaseBPUDetailView()
                            {
                                ChassisCode = p.ChassisCode,
                                ChassisNo = p.ChassisNo,
                                EngineCode = p.EngineCode,
                                EngineNo = p.EngineNo,
                                ColourCode = p.ColourCode
                            };
            return (queryable);
        }

        #region Delivery Information

        [HttpGet]
        public IQueryable<BPKLookUp> BPKNoLookUp()
        {
            var Uid = CurrentUser();
            var rslt = ctx.Context.Database.SqlQuery<BPKLookUp>(string.Format("SELECT BPKNo, BPKDate from omTrSalesBPK where CompanyCode = '{0}' AND BranchCode = '{1}' order by BPKNo ASC ", Uid.CompanyCode, Uid.BranchCode)).AsQueryable();
            return (rslt);

        }

        #endregion

        #region SPK & Tracking BBN

        [HttpGet]
        public IQueryable<codeName> SelectBPKB(string type)
        {
            var userInfo = CurrentUserInfo();
            IQueryable<codeName> data;

            if (type == "0")
            {
                data = (from a in ctx.Context.GnMstCustomer
                        join b in ctx.Context.CustomerProfitCenters
                        on new { a.CompanyCode, a.CustomerCode }
                        equals new { b.CompanyCode, b.CustomerCode }
                        where a.CompanyCode == userInfo.CompanyCode
                        && b.BranchCode == userInfo.BranchCode
                        && b.ProfitCenterCode == userInfo.ProfitCenter
                        && a.CategoryCode == "32"
                        select new codeName
                        {
                            Code = a.CustomerCode,
                            Name = a.CustomerName
                        });
            }
            else if (type == "1")
            {
                data = from a in ctx.Context.MstRefferences
                       where a.CompanyCode == userInfo.CompanyCode
                       && a.RefferenceType == "WARE"
                       && a.Status != "0"
                       select new codeName
                       {
                           Code = a.RefferenceCode,
                           Name = a.RefferenceDesc1
                       };
            }
            else
            {
                data = from a in ctx.Context.GnMstCustomer
                       where a.CompanyCode == userInfo.CompanyCode
                       select new codeName
                       {
                           Code = a.CustomerCode,
                           Name = a.CustomerName
                       };

            }

            return data;
        }

        #endregion
    }

    public class tKendaraan
    {
        public string typeID { get; set; }
        public string typeName { get; set; }
    }

    public class codeName
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
