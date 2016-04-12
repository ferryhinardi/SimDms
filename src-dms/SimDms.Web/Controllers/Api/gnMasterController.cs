using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Reflection;
using Breeze.WebApi;
using Newtonsoft.Json.Linq;
using SimDms.Common.Models;
//using SimDms.General.Models;
using SimDms.Sparepart.Models;
using Breeze.WebApi.EF;
using SimDms.Common;

namespace SimDms.Web.Controllers.Api
{
    [BreezeController]
    public class gnMasterController : ApiController
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

        [HttpGet]
        public IQueryable<SysUser> AllUser()
        {
            var Uid = CurrentUser();
            return ctx.Context.SysUsers.Where(p => p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode).AsQueryable();
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

        [HttpPost]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return ctx.SaveChanges(saveBundle);
        }



        [HttpGet]
        public IQueryable<LookUpDtlview> CustomerCategories()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.LookUpDtls
                            where p.CompanyCode == Uid.CompanyCode && p.CodeID == "CSCT"
                            select new LookUpDtlview()
                            {
                                LookUpValue = p.LookUpValue,
                                LookUpValueName = p.LookUpValueName
                            };
            return (queryable);
        }


        [HttpGet]
        public IQueryable<GnMstZipCodeView> ZipCodes()
        {
            var Uid = CurrentUser();
            //var queryable = (from a in ctx.Context.GnMstZipCodes
            //                 join b in ctx.Context.LookUpDtls
            //                 on new { a.CompanyCode, CodeID = "City", a.KotaKabupaten } equals new { b.CompanyCode, b.CodeID, KotaKabupaten = b.LookUpValueName } into _b
            //                 from b in _b.DefaultIfEmpty()
            //                 join c in ctx.Context.LookUpDtls
            //                 on new { b.CompanyCode, CodeID = "AREA", b.SeqNo } equals new { c.CompanyCode, c.CodeID, c.SeqNo } into _c
            //                 from c in _c.DefaultIfEmpty()
            //                 where a.CompanyCode == Uid.CompanyCode && b.LookUpValue != null
            //                 select new GnMstZipCodeView()
            //                 {
            //                     ZipCode = a.ZipCode,
            //                     KelurahanDesa = a.KelurahanDesa,
            //                     KecamatanDistrik = a.KecamatanDistrik,
            //                     KotaKabupaten = a.KotaKabupaten,
            //                     IbuKota = a.IbuKota,
            //                     isCity = a.isCity,
            //                     CityCode = b.LookUpValue,
            //                     AreaCode = c.LookUpValue
            //                 }).Distinct();


            //            var query = string.Format(@"
            //                           select a.ZipCode, a.IbuKota, a.isCity, a.KecamatanDistrik, a.KelurahanDesa,a.KotaKabupaten, 
            //		                            b.LookUpValue CityCode, c.LookUpValue AreaCode
            //                            from gnMstZipCode a
            //                            left join gnMstLookUpDtl b
            //                            on a.CompanyCode = b.CompanyCode and b.CodeID ='City' and a.KotaKabupaten = b.LookUpValueName
            //                            left join gnMstLookUpDtl c
            //                            on b.CompanyCode = c.CompanyCode and c.CodeID ='AREA' and b.SeqNo = c.seqno");
            //            var queryable = ctx.Context.Database.SqlQuery<GnMstZipCodeView>(query).AsQueryable();

            var query = string.Format(@"select distinct a.ZipCode, a.KelurahanDesa,a.KecamatanDistrik, a.KotaKabupaten, a.IbuKota,  a.isCity
	                , CityCode = isnull((select top 1 LookUpValue from  gnMstLookUpDtl where CompanyCode = a.CompanyCode and CodeID = 'City' and LookUpValueName = a.KotaKabupaten and LookUpValue is not null order by LastUpdateDate desc),'')
	                , AreaCode = isnull((select top 1 LookUpValue from  gnMstLookUpDtl where CompanyCode = a.CompanyCode and CodeID = 'AREA' and LookUpValue is not null  order by LastUpdateDate desc),'') 
                    from GnMstZipCode a
                    where a.CompanyCode = '{0}'", Uid.CompanyCode);
            var queryable = ctx.Context.Database.SqlQuery<GnMstZipCodeView>(query).AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<LookUpDtlview> ProfitCenters()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.LookUpDtls
                            where p.CompanyCode == Uid.CompanyCode && p.CodeID == "PFCN"
                            select new LookUpDtlview()
                            {
                                LookUpValue = p.LookUpValue,
                                LookUpValueName = p.LookUpValueName
                            };
            return (queryable);
        }

        [HttpGet]
        public IQueryable<LookUpDtlview> SupplierGrade()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.LookUpDtls
                            where p.CompanyCode == Uid.CompanyCode && p.CodeID == "SPGR"
                            select new LookUpDtlview()
                            {
                                LookUpValue = p.LookUpValue,
                                LookUpValueName = p.LookUpValueName
                            };
            return (queryable);
        }

        [HttpGet]
        public IQueryable<LookUpDtlview> LookUpDtlAll(string param)
        {

            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.LookUpDtls
                            where p.CompanyCode == Uid.CompanyCode && p.CodeID == param
                            select new LookUpDtlview()
                            {
                                LookUpValue = p.LookUpValue,
                                ParaValue = p.ParaValue,
                                LookUpValueName = p.LookUpValueName
                            };
            return (queryable);
        }

        [HttpGet]
        public IQueryable<GnMstCustomerClassView> CustomerClasses(string profitCenterCode)
        {
            var Uid = CurrentUser();
            var queryable = (from p in ctx.Context.GnMstCustomerClasses
                             where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode && p.ProfitCenterCode == profitCenterCode
                             select new GnMstCustomerClassView()
                             {
                                 CustomerClass = p.CustomerClass,
                                 CustomerClassName = p.CustomerClassName
                             }).Distinct();

            return (queryable);
        }

        [HttpGet]
        public IQueryable<GnMstTaxView> Taxes()
        {

            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.Taxes
                            where p.CompanyCode == Uid.CompanyCode
                            select new GnMstTaxView()
                            {
                                TaxCode = p.TaxCode,
                                TaxPct = p.TaxPct,
                                Description = p.Description
                            };
            //ctx.Context.Database.SqlQuery<GnMstTaxView>("select TaxCode, TaxPct, Description  from gnMstTax  where CompanyCode ='" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<GnMstCollector> Collectors(string profitCenterCode)
        {
            var Uid = CurrentUser();
            var queryable = (from p in ctx.Context.GnMstCollectors
                             where p.CompanyCode == Uid.CompanyCode && p.ProfitCenterCode == profitCenterCode
                             select new GnMstCollector()
                             {
                                 CollectorCode = p.CollectorCode,
                                 CollectorName = p.CollectorName
                             }).Distinct();
            //ctx.Context.Database.SqlQuery<GnMstCollector>("select DISTINCT CollectorCode,CollectorName  from gnMstCollector  where CompanyCode ='" + Uid.CompanyCode + "' and ProfitCenterCode='" + profitCenterCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<GnMstCollector> CollectorsBrowse()
        {
            var Uid = CurrentUser();
            var queryable = (from p in ctx.Context.GnMstCollectors
                             join p1 in ctx.Context.LookUpDtls on new { p.ProfitCenterCode } equals new { ProfitCenterCode = p1.LookUpValue }
                             where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode && p1.CodeID == "PFCN"
                             select new GnMstCollector()
                             {
                                 CollectorCode = p.CollectorCode,
                                 CollectorName = p.CollectorName,
                                 ProfitCenterCode = p.ProfitCenterCode,
                                 ProfitCenterNameDisc = p1.LookUpValueName
                             }).OrderBy(p => new { p.CollectorCode, p.ProfitCenterCode });
            //ctx.Context.Database.SqlQuery<GnMstCollector>("select CollectorCode, ProfitCenterCode ,CollectorName  from gnMstCollector  where CompanyCode ='" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<LookUpDtlview> KodeTransaksiPajak()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.LookUpDtls
                            where p.CompanyCode == Uid.CompanyCode && p.CodeID == "TRPJ"
                            select new LookUpDtlview()
                            {
                                LookUpValue = p.LookUpValue,
                                LookUpValueName = p.LookUpValueName
                            };
            //ctx.Context.Database.SqlQuery<LookUpDtlview>("select LookUpValue,LookUpValueName  from gnMstLookUpDtl where CompanyCode='" + Uid.CompanyCode + "' and CodeID='TRPJ'").AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<LookUpDtlview> KodeProvinsi()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.LookUpDtls
                            where p.CompanyCode == Uid.CompanyCode && p.CodeID == "PROV"
                            orderby p.SeqNo
                            select new LookUpDtlview()
                            {
                                LookUpValue = p.LookUpValue,
                                LookUpValueName = p.LookUpValueName
                            };
            //ctx.Context.Database.SqlQuery<LookUpDtlview>("select LookUpValue,LookUpValueName  from gnMstLookUpDtl where CompanyCode='" + Uid.CompanyCode + "' and CodeID='PROV' order by SeqNo").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<LookUpDtlview> KodeCity(string ProvinceCode)
        {
            var parameters = ProvinceCode.Split('?');
            var Province = parameters[0];
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.LookUpDtls
                            where p.CompanyCode == Uid.CompanyCode && p.CodeID == "CITY" && p.LookUpValue.Substring(0, 2) == ProvinceCode
                            select new LookUpDtlview()
                            {
                                LookUpValue = p.LookUpValue,
                                LookUpValueName = p.LookUpValueName
                            };

            //ctx.Context.Database.SqlQuery<LookUpDtlview>("select LookUpValue,LookUpValueName  from gnMstLookUpDtl where CompanyCode='" + Uid.CompanyCode + "' and CodeID='CITY' and LookupValue like '"+Province+"%'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<LookUpDtlview> KodeArea(string CityCode)
        {
            var parameters = CityCode.Split('?');
            var city = parameters[0];
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.LookUpDtls
                            where p.CompanyCode == Uid.CompanyCode && p.CodeID == "AREA" && p.LookUpValue.Contains(city)
                            select new LookUpDtlview()
                            {
                                LookUpValue = p.LookUpValue,
                                LookUpValueName = p.LookUpValueName
                            };
            //ctx.Context.Database.SqlQuery<LookUpDtlview>("select LookUpValue,LookUpValueName  from gnMstLookUpDtl where CompanyCode='" + Uid.CompanyCode + "' and CodeID='AREA' and LookupValue like '" + city + "%'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<Employee> Salesmans()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Employees.Where(p => p.CompanyCode == Uid.CompanyCode).AsQueryable();
            //ctx.Context.Database.SqlQuery<EmployeeHRView>("select  * from HrEmployeeView where CompanyCode='" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<LookUpDtlview> KelompokAR()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.LookUpDtls
                            where p.CompanyCode == Uid.CompanyCode && p.CodeID == "GPAR"
                            select new LookUpDtlview()
                            {
                                LookUpValue = p.LookUpValue,
                                LookUpValueName = p.LookUpValueName
                            };
            //ctx.Context.Database.SqlQuery<LookUpDtlview>("select LookUpValue,LookUpValueName  from gnMstLookUpDtl where CompanyCode='" + Uid.CompanyCode + "' and CodeID='GPAR'").AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<LookUpDtlview> CustomerGrades()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.LookUpDtls
                            where p.CompanyCode == Uid.CompanyCode && p.CodeID == "CSGR"
                            select new LookUpDtlview()
                            {
                                LookUpValue = p.LookUpValue,
                                LookUpValueName = p.LookUpValueName
                            };
            //ctx.Context.Database.SqlQuery<LookUpDtlview>("select LookUpValue,LookUpValueName  from gnMstLookUpDtl where CompanyCode='" + Uid.CompanyCode + "' and CodeID='CSGR'").AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<OmMstRefference> GroupPrices()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.OmMstRefferences.Where(p => p.CompanyCode == Uid.CompanyCode && p.RefferenceType == "GRPR").AsQueryable();

            //ctx.Context.Database.SqlQuery<OmMstRefferenceView>("select *  from omMstRefference where CompanyCode='" + Uid.CompanyCode + "' and RefferenceType='GRPR'").AsQueryable();
            return (queryable);

        }
        [HttpGet]
        public IQueryable<LookUpDtlview> TypeOfGoods()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.LookUpDtls
                            where p.CompanyCode == Uid.CompanyCode && p.CodeID == "TPGO"
                            select new LookUpDtlview()
                            {
                                LookUpValue = p.LookUpValue,
                                LookUpValueName = p.LookUpValueName
                            };
            //ctx.Context.Database.SqlQuery<LookUpDtlview>("select LookUpValue,LookUpValueName  from gnMstLookUpDtl where CompanyCode='" + Uid.CompanyCode + "' and CodeID='TPGO'").AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<LookUpDtlview> Banks()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.LookUpDtls
                            where p.CompanyCode == Uid.CompanyCode && p.CodeID == "BANK"
                            select new LookUpDtlview()
                            {
                                LookUpValue = p.LookUpValue,
                                LookUpValueName = p.LookUpValueName
                            };
            //ctx.Context.Database.SqlQuery<LookUpDtlview>("select LookUpValue,LookUpValueName  from gnMstLookUpDtl where CompanyCode='" + Uid.CompanyCode + "' and CodeID='BANK'").AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<LookupCustomerview> Customers()
        {
            var Uid = CurrentUserInfo();
            //var queryable = ctx.Context.Database.SqlQuery<LookupCustomerview>("exec uspfn_LookupCustomerview '" + Uid.CompanyCode + "','"+ Uid.BranchCode+"'" ).AsQueryable();
            var queryable = (from a in ctx.Context.GnMstCustomers
                             join b in ctx.Context.MstCustomerProfitCenters
                                 on new { a.CompanyCode, a.CustomerCode, BranchCode = Uid.BranchCode, IsBlackList = true }
                                 equals new { b.CompanyCode, b.CustomerCode, b.BranchCode, b.IsBlackList } into _b
                             from b in _b.DefaultIfEmpty()
                             join c in ctx.Context.LookUpDtls
                                 on new { a.CompanyCode, CodeID = "PFCN", LookUpValue = b.ProfitCenterCode }
                                 equals new { c.CompanyCode, c.CodeID, c.LookUpValue } into _c
                             from c in _c.DefaultIfEmpty()
                             where a.CompanyCode == Uid.CompanyCode
                             select new LookupCustomerview()
                             {
                                 CustomerCode = a.CustomerCode,
                                 CustomerName = a.CustomerName,
                                 Address = a.Address1 ?? "" + " " + a.Address2 ?? "" + " " + a.Address3 ?? "" + " " + a.Address4 ?? "",
                                 LookupValue = "",
                                 ProfitCenter = ""
                             }).Distinct();

            return (queryable);
        }

        [HttpGet]
        public IQueryable<GnMstSupplierView> SuppliersBrowse()
        {
            var Uid = CurrentUser();
            //var query = string.Format("exec uspfn_gnBrowseSupplier '{0}'",Uid.CompanyCode);
            //var queryable = ctx.Context.Database.SqlQuery<GnMstSupplierView>(query).ToList();

            var supplierProfitCenter = from a in ctx.Context.MstSupplierProfitCenters
                                       where a.CompanyCode == Uid.CompanyCode && a.isBlackList == false
                                       select new
                                       {
                                           a.CompanyCode,
                                           a.SupplierCode
                                       };

            var queryable = (from a in ctx.Context.GnMstSuppliers
                             join b in supplierProfitCenter on new { a.CompanyCode, a.SupplierCode }
                                 equals new { b.CompanyCode, b.SupplierCode } into _a
                             from b in _a.DefaultIfEmpty()
                             where a.CompanyCode == Uid.CompanyCode && a.Status == "1"
                             select new GnMstSupplierView()
                             {
                                 SupplierCode = a.SupplierCode,
                                 SupplierName = a.SupplierName
                             }).Distinct();

            return (queryable);
        }

        [HttpGet]
        public IQueryable<SupplierClassView> SupplierClass4Lookup(string ProfitCenterCode)
        {
            var Uid = CurrentUserInfo();
            var queryable = from p in
                                (
                                    from c in ctx.Context.GnMstSupplierClasses
                                    where c.CompanyCode == Uid.CompanyCode && c.BranchCode == Uid.BranchCode && c.ProfitCenterCode == ProfitCenterCode
                                    group c by c.SupplierClass into view
                                    select view.FirstOrDefault()
                                    )
                            select new SupplierClassView()
                            {
                                SupplierClass = p.SupplierClass,
                                SupplierClassName = p.SupplierClassName,
                                ProfitCenterCode = p.ProfitCenterCode
                            };
            //            var query = string.Format(@" 
            //                SELECT distinct(SupplierClass), SupplierClassName, ProfitCenterCode FROM gnMstSupplierClass
            //                                WHERE CompanyCode = '{0}' 
            //                                    AND BranchCode = '{1}'
            //                                    AND ProfitCenterCode = '{2}'"
            //                , Uid.CompanyCode, Uid.BranchCode, ProfitCenterCode);

            //            var queryable = ctx.Context.Database.SqlQuery<SupplierClassView>(query).AsQueryable();
            return (queryable);
        }

        //[HttpGet]
        //public IQueryable<GnMstSupplierClass> SupplierClass()
        //{

        //    return ();
        //}

        [HttpGet]
        public Object execScalar(string SQL)
        {

            var queryable = ctx.Context.Database.SqlQuery<Object>(SQL).FirstOrDefault();
            return (new { data = queryable });

        }
        [HttpGet]
        public IQueryable<GnMstCustomer> CustomersAll()
        {
            var Uid = CurrentUserInfo();
            var queryable = ctx.Context.GnMstCustomers.AsQueryable();
            return (queryable);
        }

        //Master Organisasi GetSegmentAcc
        [HttpGet]
        public IQueryable<GnMstSegmentAccView> GetSegmentAcc()
        {
            var Uid = CurrentUserInfo();
            var sql = string.Format("exec uspfn_gnGetSegmentAcc '{0}', '{1}','{2}'", Uid.CompanyCode, Uid.BranchCode, "100");
            var queryable = ctx.Context.Database.SqlQuery<GnMstSegmentAccView>(sql).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<GnMstSegmentAccView> GetSegmentAcc(string TipeSegAcc)
        {
            var Uid = CurrentUserInfo();
            var sql = string.Format("exec uspfn_gnGetSegmentAcc '{0}', '{1}','{2}'", Uid.CompanyCode, Uid.BranchCode, TipeSegAcc);
            var queryable = ctx.Context.Database.SqlQuery<GnMstSegmentAccView>(sql).AsQueryable();
            return (queryable);
        }

        //Master Organisasi OrgBrowse
        public IQueryable<OrganizationHdrView> GetOrganization()
        {
            var Uid = CurrentUserInfo();
            //var sql = "select CompanyCode, CompanyName, CompanyAccNo, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate from gnMstOrganizationHdr";
            var queryable = from p in ctx.Context.OrganizationHdrs
                            select new OrganizationHdrView()
                            {
                                CompanyCode = p.CompanyCode,
                                CompanyName = p.CompanyName,
                                CompanyAccNo = p.CompanyAccNo,
                                CreatedBy = p.CreatedBy,
                                CreatedDate = p.CreatedDate,
                                LastUpdateBy = p.LastUpdateBy,
                                LastUpdateDate = p.LastUpdateDate
                            };
            //ctx.Context.Database.SqlQuery<OrganizationHdr>(sql).AsQueryable();
            return (queryable);
        }

        public IQueryable<OrganizationDtlView> GetOrganizationDtl()
        {
            var Uid = CurrentUserInfo();
            //var sql = "select CompanyCode, CompanyName, CompanyAccNo, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate from gnMstOrganizationHdr";
            var queryable = from p in ctx.Context.OrganizationDtls
                            select new OrganizationDtlView()
                            {
                                CompanyCode = p.CompanyCode,
                                BranchCode = p.BranchCode,
                                BranchName = p.BranchName
                            };
            //ctx.Context.Database.SqlQuery<OrganizationHdr>(sql).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<gnMstAccountView> Account()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.gnMstAccounts
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            select new gnMstAccountView()
                            {
                                CompanyCode = p.CompanyCode,
                                AccountNo = p.AccountNo,
                                Description = p.Description,
                                AccountType = p.AccountType,
                                Company = p.Company,
                                Branch = p.Branch,
                                CostCtrCode = p.CostCtrCode,
                                ProductType = p.ProductType,
                                NaturalAccount = p.NaturalAccount,
                                InterCompany = p.InterCompany,
                                Futureuse = p.Futureuse,
                                Consol = p.Consol,
                                FromDate = p.FromDate,
                                EndDate = p.EndDate,
                                Balance = p.Balance,
                                CreatedDate = p.CreatedDate
                            };
            //ctx.Context.Database.SqlQuery<gnMSTAccount>("select AccountNo, Description  from gnMstAccount where CompanyCode='" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<gnMstCustomerClass> CustomerClass()
        {
            var Uid = CurrentUserInfo();
            var queryable = from p in ctx.Context.GnMstCustomerClasses
                            join q in ctx.Context.LookUpDtls
                            on new { p.ProfitCenterCode } equals new { ProfitCenterCode = q.LookUpValue }
                            join p1 in ctx.Context.gnMstAccounts
                            on new { p.CompanyCode, p.BranchCode, p.ReceivableAccNo } equals new { p1.CompanyCode, p1.BranchCode, ReceivableAccNo = p1.AccountNo }
                            join p2 in ctx.Context.gnMstAccounts
                            on new { p.CompanyCode, p.BranchCode, p.DownPaymentAccNo } equals new { p2.CompanyCode, p2.BranchCode, DownPaymentAccNo = p2.AccountNo }
                            join p3 in ctx.Context.gnMstAccounts
                            on new { p.CompanyCode, p.BranchCode, p.InterestAccNo } equals new { p3.CompanyCode, p3.BranchCode, InterestAccNo = p3.AccountNo }
                            join p4 in ctx.Context.gnMstAccounts
                            on new { p.CompanyCode, p.BranchCode, p.TaxOutAccNo } equals new { p4.CompanyCode, p4.BranchCode, TaxOutAccNo = p4.AccountNo }
                            join p5 in ctx.Context.gnMstAccounts
                            on new { p.CompanyCode, p.BranchCode, p.LuxuryTaxAccNo } equals new { p5.CompanyCode, p5.BranchCode, LuxuryTaxAccNo = p5.AccountNo }
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode && q.CodeID == "PFCN"
                            select new gnMstCustomerClass()
                            {
                                CustomerClass = p.CustomerClass,
                                CustomerClassName = p.CustomerClassName,
                                TypeOfGoods = p.TypeOfGoods,
                                ProfitCenterCode = p.ProfitCenterCode,
                                ReceivableAccNo = p.ReceivableAccNo,
                                DownPaymentAccNo = p.DownPaymentAccNo,
                                InterestAccNo = p.InterestAccNo,
                                TaxOutAccNo = p.TaxOutAccNo,
                                LuxuryTaxAccNo = p.LuxuryTaxAccNo,
                                ProfitCenterCodeDesc = q.LookUpValueName,
                                ReceivableAccNoDesc = p1.Description,
                                DownPaymentAccNoDesc = p2.Description,
                                InterestAccNoDesc = p3.Description,
                                TaxOutAccNoDesc = p4.Description,
                                LuxuryTaxAccNoDesc = p5.Description
                            };
            //ctx.Context.Database.SqlQuery<gnMstCustomerClass>("select  CustomerClass, CustomerClassName, TypeOfGoods, ProfitCenterCode, ReceivableAccNo, DownPaymentAccNo, InterestAccNo, TaxOutAccNo, LuxuryTaxAccNo from gnMstCustomerClass where companycode= '" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<gnMstSupplierClass> SupplierClass()
        {
            var Uid = CurrentUserInfo();
            var queryable = from p in ctx.Context.GnMstSupplierClasses
                            join q in ctx.Context.LookUpDtls
                            on new { p.ProfitCenterCode } equals new { ProfitCenterCode = q.LookUpValue }
                            join p1 in ctx.Context.gnMstAccounts
                            on new { p.CompanyCode, p.BranchCode, p.PayableAccNo } equals new { p1.CompanyCode, p1.BranchCode, PayableAccNo = p1.AccountNo }
                            join p2 in ctx.Context.gnMstAccounts
                            on new { p.CompanyCode, p.BranchCode, p.DownPaymentAccNo } equals new { p2.CompanyCode, p2.BranchCode, DownPaymentAccNo = p2.AccountNo }
                            join p3 in ctx.Context.gnMstAccounts
                            on new { p.CompanyCode, p.BranchCode, p.InterestAccNo } equals new { p3.CompanyCode, p3.BranchCode, InterestAccNo = p3.AccountNo }
                            join p4 in ctx.Context.gnMstAccounts
                            on new { p.CompanyCode, p.BranchCode, p.TaxInAccNo } equals new { p4.CompanyCode, p4.BranchCode, TaxInAccNo = p4.AccountNo }
                            join p5 in ctx.Context.gnMstAccounts
                            on new { p.CompanyCode, p.BranchCode, p.OtherAccNo } equals new { p5.CompanyCode, p5.BranchCode, OtherAccNo = p5.AccountNo }
                            join p6 in ctx.Context.gnMstAccounts
                            on new { p.CompanyCode, p.BranchCode, p.WitholdingTaxAccNo } equals new { p6.CompanyCode, p6.BranchCode, WitholdingTaxAccNo = p6.AccountNo }
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode && q.CodeID == "PFCN"
                            select new gnMstSupplierClass()
                            {
                                SupplierClass = p.SupplierClass,
                                SupplierClassName = p.SupplierClassName,
                                TypeOfGoods = p.TypeOfGoods,
                                ProfitCenterCode = p.ProfitCenterCode,
                                PayableAccNo = p.PayableAccNo,
                                DownPaymentAccNo = p.DownPaymentAccNo,
                                InterestAccNo = p.InterestAccNo,
                                OtherAccNo = p.OtherAccNo,
                                TaxInAccNo = p.TaxInAccNo,
                                WitholdingTaxAccNo = p.WitholdingTaxAccNo,
                                ProfitCenterCodeDesc = q.LookUpValueName,
                                PayableAccNoDesc = p1.Description,
                                DownPaymentAccNoDesc = p2.Description,
                                InterestAccNoDesc = p3.Description,
                                TaxInAccNoDesc = p4.Description,
                                OtherAccNoDesc = p5.Description,
                                WitholdingTaxAccNoDesc = p6.Description
                            };
            //ctx.Context.Database.SqlQuery<gnMstSupplierClass>("select SupplierClass, SupplierClassName, TypeOfGoods, ProfitCenterCode, PayableAccNo, DownPaymentAccNo, InterestAccNo, OtherAccNo, TaxInAccNo, WitholdingTaxAccNo from gnMstSupplierClass where companycode= '" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<GnMstDocumentView> Documents()
        {
            var Uid = CurrentUserInfo();
            var queryable = from p in ctx.Context.GnMstDocuments
                            join q in ctx.Context.LookUpDtls
                            on new { p.ProfitCenterCode } equals new { ProfitCenterCode = q.LookUpValue }
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            && q.CodeID == "PFCN"
                            select new GnMstDocumentView()
                            {
                                DocumentType = p.DocumentType,
                                DocumentName = p.DocumentName,
                                DocumentPrefix = p.DocumentPrefix,
                                ProfitCenterCode = p.ProfitCenterCode,
                                DocumentYear = p.DocumentYear.ToString(),
                                DocumentSequence = p.DocumentSequence,
                                ProfitCenterNameDisc = q.LookUpValueName
                            };

            //ctx.Context.Database.SqlQuery<GnMstDocumentView>("select DocumentType, DocumentName, DocumentPrefix, ProfitCenterCode, DocumentYear, DocumentSequence from GnMstDocument where companycode= '" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<EmployeeView> Employees()
        {
            var Uid = CurrentUserInfo();
            var queryable = from p in ctx.Context.Employees
                            join p1 in ctx.Context.LookUpDtls on new { p.ProvinceCode } equals new { ProvinceCode = p1.LookUpValue }
                            join p2 in ctx.Context.LookUpDtls on new { p.CityCode } equals new { CityCode = p2.LookUpValue }
                            join p3 in ctx.Context.LookUpDtls on new { p.AreaCode } equals new { AreaCode = p3.LookUpValue }
                            join p4 in ctx.Context.LookUpDtls on new { p.TitleCode } equals new { TitleCode = p4.LookUpValue }
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            && p1.CodeID == "PROV"
                            && p2.CodeID == "CITY"
                            && p3.CodeID == "AREA"
                            && p4.CodeID == "TITL"
                            select new EmployeeView()
                            {
                                BranchCode = p.BranchCode,
                                EmployeeID = p.EmployeeID,
                                EmployeeName = p.EmployeeName,
                                Address1 = p.Address1,
                                Address2 = p.Address2,
                                Address3 = p.Address3,
                                Address4 = p.Address4,
                                PhoneNo = p.PhoneNo,
                                HpNo = p.HpNo,
                                FaxNo = p.FaxNo,
                                ProvinceCode = p.ProvinceCode,
                                AreaCode = p.AreaCode,
                                CityCode = p.CityCode,
                                ZipNo = p.ZipNo,
                                TitleCode = p.TitleCode,
                                JoinDate = p.JoinDate,
                                ResignDate = p.ResignDate,
                                GenderCode = p.GenderCode,
                                BirthPlace = p.BirthPlace,
                                BirthDate = p.BirthDate,
                                MaritalStatusCode = p.MaritalStatusCode,
                                ReligionCode = p.ReligionCode,
                                BloodCode = p.BloodCode,
                                IdentityNo = p.IdentityNo,
                                Height = p.Height,
                                Weight = p.Weight,
                                UniformSize = p.UniformSize,
                                ShoesSize = p.ShoesSize,
                                FormalEducation = p.FormalEducation,
                                PersonnelStatus = p.PersonnelStatus,
                                Nik = p.Nik,
                                //EmpPhotoID = p.EmpPhotoID,
                                EmpIdentityCardID = p.EmpIdentityCardID,
                                EmpImageID = p.EmpImageID,
                                EmpIdentityCardImageID = p.EmpIdentityCardImageID,
                                ProvinceName = p1.LookUpValueName,
                                CityName = p2.LookUpValueName,
                                AreaName = p3.LookUpValueName,
                                TitleName = p4.LookUpValueName
                            };

            //ctx.Context.Database.SqlQuery<EmployeeView>("select BranchCode, EmployeeID, EmployeeName, Address1, Address2, Address3, Address4, PhoneNo, HpNo, FaxNo, ProvinceCode, AreaCode, CityCode, ZipNo, TitleCode, JoinDate, ResignDate, GenderCode, BirthPlace, BirthDate, MaritalStatusCode, ReligionCode, BloodCode, IdentityNo, Height, Weight, UniformSize, ShoesSize, FormalEducation, PersonnelStatus, Nik, EmpPhotoID, EmpIdentityCardID, EmpImageID, EmpIdentityCardImageID from gnMstEmployee where companycode= '" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<GnMstSignatureView> DocumentSignature()
        {
            var Uid = CurrentUserInfo();
            var queryable = from p in ctx.Context.GnMstSignatures
                            join p1 in ctx.Context.LookUpDtls on p.ProfitCenterCode equals p1.LookUpValue
                            join p2 in ctx.Context.GnMstDocuments on new { p.CompanyCode, p.BranchCode, p.DocumentType } equals new { p2.CompanyCode, p2.BranchCode, p2.DocumentType }
                            where p.CompanyCode == Uid.CompanyCode
                            && p1.CodeID == "PFCN"
                            select new GnMstSignatureView()
                            {
                                CompanyCode = p.CompanyCode,
                                BranchCode = p.BranchCode,
                                ProfitCenterCode = p.ProfitCenterCode,
                                DocumentType = p.DocumentType,
                                SeqNo = p.SeqNo,
                                SignName = p.SignName,
                                TitleSign = p.TitleSign,
                                ProfitCenterName = p1.LookUpValueName,
                                DocumentName = p2.DocumentName
                            };

            //ctx.Context.Database.SqlQuery<GnMstSignatureView>("select a.CompanyCode, a.BranchCode, a.ProfitCenterCode, a.DocumentType, a.SeqNo, SignName, TitleSign, LookUpValueName ProfitCenterName from GnMstSignature a inner join gnMstLookUpDtl b on a.ProfitCenterCode=b.LookUpValue and CodeID='PFCN' where a.companycode= '" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<SysMessageBoardsView> Messages()
        {
            var Uid = CurrentUser();
            var sql = string.Format("uspsys_SelectMsgBoards '{0}'", Uid.UserId);
            //var queryable = ctx.Context.SysMessageBoardss.Cast<SysMessageBoardsView>();
            var queryable = ctx.Context.Database.SqlQuery<SysMessageBoardsView>(sql).AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<LookUpHdrs> lookupBrowse()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.LookUpHdrs
                            select new LookUpHdrs()
                            {
                                CodeID = p.CodeID,
                                CodeName = p.CodeName,
                                FieldLength = p.FieldLength,
                                isNumber = p.isNumber
                            };

            //ctx.Context.LookUpHdrs.Cast<LookUpHdrs>();
            //ctx.Context.Database.SqlQuery<LookUpHdrs>("select *  from GnMstLookUpHdr").AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public ParameterView Parameters()
        {
            var Uid = CurrentUser();
            var u = ctx.Context.SysUsers.Find(User.Identity.Name);

            var sql = string.Format("exec uspfn_BrowseParameter '" + u.CompanyCode + "'");
            var check = ctx.Context.Database.SqlQuery<ParameterView2>(sql);
            var v = ctx.Context.sysParams.Find(u.CompanyCode);
            var queryable = new ParameterView
            {
                DbName = v.DBName,
                Extensions = v.Extensions,
                Prefix = v.Prefix,
                FolderPath = v.FolderPath,
                DcsUrl = v.DCSURL,
                TaxUrl = v.TAXURL,
            };
            return queryable;
        }

        [HttpGet]
        public IQueryable<FPJSignatureView> FPJSignature()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.GnMstFPJSignDates
                            join p1 in ctx.Context.LookUpDtls on new { p.CompanyCode, p.ProfitCenterCode } equals new { p1.CompanyCode, ProfitCenterCode = p1.LookUpValue }
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode && p1.CodeID == "PFCN"
                            select new FPJSignatureView()
                            {
                                ProfitCenterCode = p.ProfitCenterCode,
                                LookUpValueName = p1.LookUpValueName,
                                FPJOption = p.FPJOption,
                                FPJOptionDescription = p.FPJOptionDescription
                            };
            //ctx.Context.Database.SqlQuery<FPJSignatureView>("select a.ProfitCenterCode, LookUpValueName, FPJOption, FPJOptionDescription  from GnMstFPJSignDate a inner join gnMstLookUpDtl b on a.CompanyCode = b.CompanyCode and CodeID ='PFCN' and a.ProfitCenterCode = b.LookUpValue where a.companycode= '" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<CoProfileView> AllBranch()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.GnMstCoProfiles
                            join p1 in ctx.Context.LookUpDtls
                            on p.CityCode equals p1.LookUpValue
                            where p1.CodeID == "CITY"
                            select new CoProfileView()
                            {
                                CompanyCode = p.CompanyCode,
                                BranchCode = p.BranchCode,
                                CompanyName = p.CompanyName,
                                CompanyGovName = p.CompanyGovName,
                                Address1 = p.Address1,
                                Address2 = p.Address2,
                                Address3 = p.Address3,
                                Address4 = p.Address4,
                                ZipCode = p.ZipCode,
                                IsPKP = p.isPKP,
                                SKPNo = p.SKPNo,
                                SKPDate = p.SKPDate,
                                NPWPNo = p.NPWPNo,
                                NPWPDate = p.NPWPDate,
                                CityCode = p.CityCode,
                                AreaCode = p.AreaCode,
                                PhoneNo = p.PhoneNo,
                                FaxNo = p.FaxNo,
                                OwnershipName = p.OwnershipName,
                                TaxTransCode = p.TaxTransCode,
                                TaxCabCode = p.TaxCabCode,
                                IsFPJCentralized = p.isFPJCentralized,
                                ProductType = p.ProductType,
                                IsLinkToService = p.isLinkToService,
                                IsLinkToSpare = p.isLinkToSpare,
                                IsLinkToSales = p.isLinkToSales,
                                IsLinkToFinance = p.isLinkToFinance,
                                CityName = p1.LookUpValueName
                            };
            //ctx.Context.Database.SqlQuery<MyUserInfo>("select BranchCode, CompanyName from GnMstCoProfile").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<FPJseqNoview> FPJseqNo()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.GnMstFPJSeqNos
                            join p1 in ctx.Context.GnMstCoProfiles on new { p.CompanyCode, p.BranchCode } equals new { p1.CompanyCode, p1.BranchCode }
                            select new FPJseqNoview()
                            {
                                CompanyCode = p.CompanyCode,
                                Year = p.Year,
                                CompanyGovName = p1.CompanyGovName,
                                BranchCode = p.BranchCode,
                                CompanyName = p1.CompanyName,
                                FPJSeqNo = p.FPJSeqNo,
                                SeqNo = p.SeqNo,
                                BeginTaxNo = p.BeginTaxNo,
                                EndTaxNo = p.EndTaxNo,
                                EffectiveDate = p.EffectiveDate
                            };
            //ctx.Context.Database.SqlQuery<FPJseqNoview>("select a.CompanyCode, Year,CompanyGovName, a.BranchCode, CompanyName,  FPJSeqNo, SeqNo, BeginTaxNo,	EndTaxNo, EffectiveDate  from gnMstFPJSeqNo a inner join gnMstCoProfile b on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<segmentAccView> SegmentAcc(string param)
        {
            var Uid = CurrentUser();
            if (param == "400")
            {
                var queryable = from p in ctx.Context.GnMstSegmentAccs
                                join p1 in ctx.Context.LookUpDtls on new { p.CompanyCode, p.Parent } equals new { p1.CompanyCode, Parent = p1.LookUpValue }
                                join p2 in ctx.Context.LookUpDtls on new { p.CompanyCode, p.TipeSegAcc } equals new { p2.CompanyCode, TipeSegAcc = p2.LookUpValue }
                                where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode && p1.CodeID == "ACCT" && p.TipeSegAcc == param && p2.CodeID == "SEGM"
                                select new segmentAccView()
                                {
                                    CompanyCode = p.CompanyCode,
                                    TipeSegAcc = p.TipeSegAcc,
                                    SegAccNo = p.SegAccNo,
                                    Description = p.Description,
                                    Parent = p.Parent,
                                    FromDate = p.FromDate,
                                    EndDate = p.EndDate,
                                    AccountType = p1.LookUpValueName,
                                    Type = p2.LookUpValueName
                                };
                return (queryable);
            }
            else
            {
                var queryable = from p in ctx.Context.GnMstSegmentAccs
                                join p2 in ctx.Context.LookUpDtls on new { p.CompanyCode, p.TipeSegAcc } equals new { p2.CompanyCode, TipeSegAcc = p2.LookUpValue }
                                where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode && p.TipeSegAcc == param && p2.CodeID == "SEGM"
                                select new segmentAccView()
                                {
                                    CompanyCode = p.CompanyCode,
                                    TipeSegAcc = p.TipeSegAcc,
                                    SegAccNo = p.SegAccNo,
                                    Description = p.Description,
                                    Parent = p.Parent,
                                    FromDate = p.FromDate,
                                    EndDate = p.EndDate,
                                    Type = p2.LookUpValueName
                                };
                return (queryable);
            }

        }

        [HttpGet]
        public IQueryable<periodeView> periodes()
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.Periodes
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            orderby p.FiscalYear, p.FiscalMonth, p.PeriodeNum
                            select new periodeView()
                            {
                                FiscalYear = p.FiscalYear,
                                FiscalMonth = p.FiscalMonth,
                                PeriodeNum = p.PeriodeNum,
                                PeriodeName = p.PeriodeName,
                                FromDate = p.FromDate,
                                EndDate = p.EndDate,
                                StatusSparepart = (p.StatusSparepart == 0 ? "Future Entry" :
                                                    p.StatusSparepart == 1 ? "Open" :
                                                     "Close"),
                                StatusSales = (p.StatusSales == 0 ? "Future Entry" :
                                                    p.StatusSales == 1 ? "Open" :
                                                     "Close"),
                                StatusService = (p.StatusService == 0 ? "Future Entry" :
                                                    p.StatusService == 1 ? "Open" :
                                                     "Close"),
                                StatusFinanceAP = (p.StatusFinanceAP == 0 ? "Future Entry" :
                                                    p.StatusFinanceAP == 1 ? "Open" :
                                                     "Close"),
                                StatusFinanceAR = (p.StatusFinanceAR == 0 ? "Future Entry" :
                                                    p.StatusFinanceAR == 1 ? "Open" :
                                                     "Close"),
                                StatusFinanceGL = (p.StatusFinanceGL == 0 ? "Future Entry" :
                                                    p.StatusFinanceGL == 1 ? "Open" :
                                                     "Close"),
                                FiscalStatus = (p.FiscalStatus == false ? "Non Active" : "Active")
                            };
            return (queryable);
        }

        [HttpGet]
        public IQueryable<LookupCustomerview> profitcenterCustomer(string profitcentercode)
        {
            var Uid = CurrentUser();
            var queryable = from p in ctx.Context.GnMstCustomers
                            join p1 in ctx.Context.ProfitCenters on new { p.CustomerCode } equals new { p1.CustomerCode }
                            join p2 in ctx.Context.LookUpDtls on new { p.CompanyCode } equals new { p2.CompanyCode }
                            where p.CompanyCode == Uid.CompanyCode &&
                            p1.BranchCode == Uid.BranchCode &&
                            p1.ProfitCenterCode == profitcentercode &&
                            p2.CodeID == "PFCN" &&
                            p2.LookUpValue == p1.ProfitCenterCode
                            //&& p.Status = "1" && p1.isBlackList == ??false 
                            select new LookupCustomerview()
                            {
                                CustomerCode = p.CustomerCode,
                                CustomerName = p.CustomerName,
                                Address = p.Address1 + p.Address2 + p.Address3 + p.Address4
                            };
            return (queryable);
        }

        [HttpGet]
        public IQueryable<CreditLimitView> CreditLimitViews(string profitcentercode, string customercode)
        {
            var Uid = CurrentUser();
            var sql = string.Format("exec uspfn_CreditLimitView '" + Uid.CompanyCode + "','" + Uid.BranchCode + "','" + profitcentercode + "','" + customercode + "'");
            var queryable = ctx.Context.Database.SqlQuery<CreditLimitView>(sql).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<UtilityView> CustomerUtility()
        {
            var Uid = CurrentUser();
            var sql = string.Format("exec uspfn_CustomerUtilityView  1");
            var queryable = ctx.Context.Database.SqlQuery<UtilityView>(sql).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<UtilityView> SupplierUtility()
        {
            var Uid = CurrentUser();
            var sql = string.Format("exec uspfn_CustomerUtilityView  2");
            var queryable = ctx.Context.Database.SqlQuery<UtilityView>(sql).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<GnMstReminder> ReminderBrowse()
        {

            var Uid = CurrentUser();
            var queryable = ctx.Context.GnMstReminders.Where(p => p.CompanyCode == Uid.CompanyCode);
            //ctx.Context.Database.SqlQuery<GnMstTaxView>("select TaxCode, TaxPct, Description  from gnMstTax  where CompanyCode ='" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<GnMstApproval> ApprovalBrowse()
        {

            var Uid = CurrentUser();
            var queryable = ctx.Context.GnMstApprovals.Where(p => p.CompanyCode == Uid.CompanyCode);
            //ctx.Context.Database.SqlQuery<GnMstTaxView>("select TaxCode, TaxPct, Description  from gnMstTax  where CompanyCode ='" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SysReport> GetReport()
        {
            var Uid = CurrentUser();
            var query = string.Format(@"
               select * from sysReport");

            var queryable = ctx.Context.Database.SqlQuery<SysReport>(query).AsQueryable();
            return (queryable);

        }
        [HttpGet]
        public IQueryable<SysMenu> ListModule()
        {
            var Uid = CurrentUser();
            var query = string.Format(@"
               select MenuId , MenuIndex, MenuLevel
                      ,MenuCaption, menuheader , MenuID MenuUrl
                  from SysMenu
                 where 1 = 1
                   and MenuLevel = 0
                   and IsVisible = 1
                 order by MenuIndex");
            var queryable = ctx.Context.Database.SqlQuery<SysMenu>(query).AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<SysMenu> ListMenu(string ModuleID)
        {
            var Uid = CurrentUser();
            var query = string.Format(@"
                           select a.MenuId, a.MenuCaption,  
                            a.MenuIndex, a.MenuLevel, a.menuheader , a.MenuID MenuUrl
                            from SysMenu a
                            inner join 
                            (select IsHeader, MenuId  from SysMenu 
                             where 1 = 1
                               and IsVisible = 1
                               and MenuHeader = '{0}') b
                            on b.IsHeader = 1 and a.MenuHeader = b.MenuId
                            where a.IsVisible = 1
                            order by a.MenuIndex", ModuleID);
            var queryable = ctx.Context.Database.SqlQuery<SysMenu>(query).AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<Customerbrowse> CustBrowse()
        {
            string dynamicFilter = "";
            string filters = "";
            string filter = "";

            string url = (Request.RequestUri).ToString();
            string prmString = (url.Substring(url.IndexOf('$')).Split('&')[0]).ToString();
            prmString = prmString.Replace("((", "(");
            prmString = prmString.Replace("))", ")");
            filter = (prmString).Substring(1, 6);
            if (filter == "filter")
            {
                prmString = prmString.Replace('(', '[');
                prmString = prmString.Replace(')', ']');

                dynamicFilter = Helpers.GetDFilter(prmString);
            }

            if (dynamicFilter.Length >0)
            {
                filters = "'" + dynamicFilter + "'";
            }

            var query = string.Format(@"exec uspfn_getCustomerBrowse {0}", filters);
            var queryable = ctx.Context.Database.SqlQuery<Customerbrowse>(query).AsQueryable();
            return (queryable);
        }


        [HttpGet]
        public IQueryable<Customerbrowse> CustomerBrowse()
        {
            var field = "";
            var value = "";
            string dynamicFilter = "";
            string filter = "";
            string[] str;
            
            var nameFilter = string.Empty;
            string url = (Request.RequestUri).ToString();
            string prmString = (url.Substring(url.IndexOf('$')).Split('&')[0]).ToString();
            filter = (prmString).Substring(1, 6);
            if (filter=="filter")
            {
                prmString = prmString.Replace('(', '[');
                prmString = prmString.Replace(')', ']');

                string[] tokens = (prmString.Substring(prmString.IndexOf("["), prmString.Length - prmString.IndexOf("["))).Split(new[] { " and " }, StringSplitOptions.None);

                if (tokens.Length >1)
                {
                    for (int i = 0; i < tokens.Length; i++)
                    {
                        int pFrom = (tokens[i].Substring(1, tokens[i].Length - 1)).IndexOf("[") + "[".Length;
                        int pTo = (tokens[i].Substring(1, tokens[i].Length - 9)).LastIndexOf("]");

                        str = ((tokens[i].Substring(1, tokens[i].Length - 1)).Substring(pFrom, pTo - pFrom)).Split(',');

                        field = str[1].ToString();
                        value = str[0].ToString();

                        dynamicFilter += value != "" ? "' AND " + field + " LIKE ''%" + value + "%'" : "";
                    }
                }
                else
                {
                    int pFrom = prmString.IndexOf("[") + "[".Length;
                    int pTo = prmString.LastIndexOf("]");

                    str = (prmString.Substring(pFrom, pTo - pFrom)).Split(',');
                    field = str[1].ToString();
                    value = str[0].ToString();

                    dynamicFilter += value != "" ? "' AND " + field + " LIKE ''%" + value + "%'" : "";
                }                
            }            

            var Uid = CurrentUser();
            //            var query = string.Format(@"
            //                           select a.*,c.LookUpValueName as CategoryName, b.KelurahanDesa as PosName,
            //                            a.Address1 + ' ' + a.Address2 + ' ' + a.Address3 + ' ' + a.Address4 as AddressGab
            //                            from gnMstCustomer a
            //                            left join gnMstLookUpDtl c
            //                            on a.CompanyCode = c.CompanyCode and a.CategoryCode = c.LookUpValue
            //                            left join gnMstZipCode b
            //                            on a.zipno =b.zipcode and a.KelurahanDesa = b.KelurahanDesa 
            //	                            and a.KecamatanDistrik =b.KecamatanDistrik and a.KotaKabupaten = b.KotaKabupaten 
            //	                            and a.IbuKota =b.IbuKota
            //                            where c.CodeID ='CSCT'");
            //            var queryable = ctx.Context.Database.SqlQuery<Customerbrowse>(query).AsQueryable();

            var queryable = (from p in ctx.Context.GnMstCustomers
                             join q in ctx.Context.LookUpDtls
                             on p.CategoryCode equals q.LookUpValue into _q
                             from q in _q.DefaultIfEmpty()
                             join r in ctx.Context.GnMstZipCodes
                             on new { p.KecamatanDistrik, p.KotaKabupaten, p.IbuKota, p.ZipNo, p.KelurahanDesa } equals new { r.KecamatanDistrik, r.KotaKabupaten, r.IbuKota, ZipNo = r.ZipCode, r.KelurahanDesa } into _r
                             from r in _r.DefaultIfEmpty()
                             where q.CodeID == "CSCT"
                             select new Customerbrowse()
                             {
                                 AddressGab = p.Address1 + " " + p.Address2 + " " + p.Address3 + " " + p.Address4,
                                 CategoryName = q.LookUpValueName,
                                 PosName = r.KelurahanDesa,
                                 CustomerCode = p.CustomerCode,
                                 StandardCode = p.StandardCode,
                                 CustomerName = p.CustomerName,
                                 CustomerAbbrName = p.CustomerAbbrName,
                                 CustomerGovName = p.CustomerGovName,
                                 CustomerType = p.CustomerType,
                                 CategoryCode = p.CategoryCode,
                                 Address1 = p.Address1,
                                 Address2 = p.Address2,
                                 Address3 = p.Address3,
                                 Address4 = p.Address4,
                                 PhoneNo = p.PhoneNo,
                                 HPNo = p.HPNo,
                                 FaxNo = p.FaxNo,
                                 isPKP = p.isPKP,
                                 NPWPNo = p.NPWPNo,
                                 NPWPDate = p.NPWPDate,
                                 SKPNo = p.SKPNo,
                                 SKPDate = p.SKPDate,
                                 ProvinceCode = p.IbuKota,
                                 AreaCode = p.AreaCode,
                                 CityCode = p.CityCode,
                                 ZipNo = p.ZipNo,
                                 Status = p.Status,
                                 Email = p.Email,
                                 BirthDate = p.BirthDate,
                                 Gender = p.Gender,
                                 OfficePhoneNo = p.OfficePhoneNo,
                                 KelurahanDesa = p.KelurahanDesa,
                                 KecamatanDistrik = p.KecamatanDistrik,
                                 KotaKabupaten = p.KotaKabupaten,
                                 IbuKota = p.IbuKota,
                                 CustomerStatus = p.CustomerStatus
                             });
            return (queryable.Distinct());

        }
    }

    public class LookupScalar
    {
        public string valueScalar { get; set; }

    }


}
