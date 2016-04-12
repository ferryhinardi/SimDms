using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Breeze.WebApi;
using Newtonsoft.Json.Linq;
using SimDms.Common.Models;
using SimDms.Service.Models;

using Breeze.WebApi.EF;
using SimDms.Common;

namespace SimDms.Web.Controllers.Api
{
    [BreezeController]
    public class SvTransaksiController : ApiController
    {

        readonly EFContextProvider<DataContext> ctx = new EFContextProvider<DataContext>();
        readonly EFContextProvider<MDContext> ctxMD = new EFContextProvider<MDContext>();

        [HttpGet]
        public String Metadata()
        {
            return ctx.Metadata();
        }

        protected string DealerCode()
        {
            var uid = CurrentUser();
            var result = ctx.Context.CompanyMappings.FirstOrDefault(x => x.CompanyCode == uid.CompanyCode && x.BranchCode == uid.BranchCode);
            if (result != null)
            {
                if (result.CompanyMD == uid.CompanyCode && result.BranchMD == uid.BranchCode) { return "MD"; }
                else { return "SD"; }
            }
            else return "MD";
        }


        [HttpGet]
        public SysUser CurrentUser()
        {
            return ctx.Context.SysUsers.Find(User.Identity.Name);
        }

        [HttpGet]
        public SysUserProfitCenter SysUserProfitCenters()
        {
            return ctx.Context.SysUserProfitCenters.Find(CurrentUser().UserId);
        }

        protected string CompanyCode
        {
            get
            {
                return CurrentUser().CompanyCode;
            }
        }

        protected string BranchCode
        {
            get
            {
                return CurrentUser().BranchCode;

            }
        }

        protected string ProductType
        {
            get
            {
                return ctx.Context.CoProfiles.Find(CurrentUser().CompanyCode, CurrentUser().BranchCode).ProductType;
            }
        }

        protected string ProfitCenter
        {
            get
            {
                var IsAdmin = ctx.Context.Database.SqlQuery<bool>(string.Format(@"select top 1 b.IsAdmin from sysusergroup a 
                    left join SysGroup b on b.GroupId = a.GroupId where 1 = 1 and a.UserId = '{0}' and b.GroupId = a.GroupId",
                    CurrentUser().UserId)).SingleOrDefault();
                string profitCenter = "200";
                if (!IsAdmin)
                {
                    string s = "000";
                    var x = ctx.Context.SysUserProfitCenters.Find(CurrentUser().UserId);
                    if (x != null) s = x.ProfitCenter;
                    return s;
                }
                else
                {
                    return profitCenter;
                }
            }
        }

        [HttpGet]
        public MyUserInfo CurrentUserInfo()
        {
            var u = ctx.Context.SysUsers.Find(User.Identity.Name);
            var g = ctx.Context.CoProfiles.Find(u.CompanyCode, u.BranchCode);
            u.CoProfile = ctx.Context.CoProfiles.Find(u.CompanyCode, u.BranchCode);
            var f = ctx.Context.SysUserProfitCenters.Find(User.Identity.Name);

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
                ProfitCenter = f.ProfitCenter,
                SimDmsVersion = GetType().Assembly.GetName().Version.ToString()
            };

            return info;
        }

        [HttpGet]
        public IQueryable<JobOrderView> Estimations()
        {
            var uid = CurrentUser();
            var queryable = ctx.Context.JobOrderViews
                .Where(p => p.CompanyCode == uid.CompanyCode && p.BranchCode == uid.BranchCode && p.ServiceType == "0"
                    && new string[] { "0", "1", "2", "3", "4", "5" }.Contains(p.ServiceStatus));
            return (queryable);
        }

        [HttpGet]
        public IQueryable<JobOrderView> Bookings()
        {
            var uid = CurrentUser();
            var queryable = ctx.Context.JobOrderViews.Where(p => p.CompanyCode == uid.CompanyCode && p.BranchCode == uid.BranchCode && p.ServiceType == "1" && new string[] { "0", "1", "2", "3", "4", "5" }.Contains(p.ServiceStatus));
            return (queryable);
        }

        [HttpGet]
        public IQueryable<JobOrderView> JobOrders()
        {
            var uid = CurrentUser();
            var queryable = ctx.Context.JobOrderViews.Where(p => p.CompanyCode == uid.CompanyCode
                && p.BranchCode == uid.BranchCode && p.ServiceType == "2"
                && new string[] { "0", "1", "2", "3", "4", "5" }
                .Contains(p.ServiceStatus));
            return (queryable);
        }

        [HttpGet]
        public IQueryable<CustomerVehicleView> CustomerVehicles()
        {
            var uid = CurrentUser();
            var queryable = ctx.Context.CustomerVehicleViews.Where(p => p.CompanyCode == uid.CompanyCode && p.BranchCode == uid.BranchCode);
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SimDms.Service.Models.CustomerView> customers()
        {
            var queryable = ctx.Context.CustomerViews.Where(p => p.CustomerName != "");
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SimDms.Service.Models.JobTypeView> JobTypes(string BasicModel)
        {
            var basicModel = BasicModel ?? "";
            var uid = CurrentUser();
            //var queryable = ctx.Context.JobTypeViews
            //                .Where(p => p.CompanyCode == uid.CompanyCode && p.BasicModel == basicModel)
            //                .OrderBy(p => p.JobType);

            //return (queryable);

            var qry = string.Format(@"select 
	                                     CompanyCode,
	                                     BasicModel,
	                                     JobType,
	                                     Description
                                       from (select a.CompanyCode
                                         , a.BasicModel
                                         , a.JobType
                                         , b.Description
                                         , case when a.JobType='OTHER' then '1'
		                                    when a.JobType='BODYREPAIR' then '3'
		                                    when a.JobType='CLAIM' then '4'
		                                    else '2' end as orderNo		
                                      from SvMstJob a
                                      left join svMstRefferenceService b
                                        on b.CompanyCode = a.CompanyCode
                                       and b.ProductType = a.ProductType
                                       and b.RefferenceCode = a.JobType
                                       and b.RefferenceType = 'JOBSTYPE'
  
                                      where 
                                    a.isActive=1 and
                                    a.CompanyCode = '{0}' and
                                    a.BasicModel='{1}'
                                       ) a
                                       order by a.orderNo,JobType", uid.CompanyCode, basicModel);

            var query = ctx.Context.Database.SqlQuery<JobTypeView>(qry).AsQueryable();
            return (query);
        }

        [HttpGet]
        public IQueryable<SaView> ServiceAdvisors()
        {
            var uid = CurrentUser();
            var queryable = ctx.Context.SaViews.Where(p => p.CompanyCode == uid.CompanyCode && p.BranchCode == uid.BranchCode).OrderBy(p => p.EmployeeName);
            return (queryable);
        }

        [HttpGet]
        public IQueryable<FmView> Foremans()
        {
            var uid = CurrentUser();
            var queryable = ctx.Context.FmViews.Where(p => p.CompanyCode == uid.CompanyCode && p.BranchCode == uid.BranchCode).OrderBy(p => p.EmployeeName);
            return (queryable);
        }

        [HttpGet]
        public IQueryable<TaskNoView> PartNo(string basicModel, string jobType)
        {

            var uid = CurrentUser();
            var prodtyp = ctx.Context.CoProfiles.Find(uid.CompanyCode, uid.BranchCode).ProductType;

            #region -- Query --
            var sql = @"select a.OperationNo, a.Description as DescriptionTask
			 , case @p3 when 'CLAIM' then isnull(b.ClaimHour, a.ClaimHour) else isnull(b.OperationHour, a.OperationHour) end as Qty
			 , case @p3 when 'CLAIM' then a.LaborCost else isnull(b.LaborPrice, a.LaborPrice) end as Price
		  from svMstTask a
         left join svMstTaskPrice b
            on b.CompanyCode = a.CompanyCode
           and b.BranchCode  = @p4
           and b.ProductType = a.ProductType
           and b.BasicModel  = a.BasicModel
           and b.JobType     = a.JobType
           and b.OperationNo = a.OperationNo
		 where a.CompanyCode = @p0
		   and a.ProductType = @p1
		   and a.BasicModel = @p2
           and a.IsActive    = 1
           and a.JobType in (@p3, 'CLAIM','OTHER')
        order by a.OperationNo asc
";
            #endregion

            var queryable = ctx.Context.Database.SqlQuery<TaskNoView>(sql, uid.CompanyCode, prodtyp, basicModel, jobType, uid.BranchCode).AsQueryable();
            //var queryable = ctx.TaskNoViews.Where(e => e.CompanyCode == CompanyCode && e.ProductType == ProductType && e.BasicModel == basicModel);
            return (queryable);
        }


        [HttpGet]
        public IQueryable<NoPart> NoPartOpen()
        {
            var uid = CurrentUser();
            var prodtyp = ctx.Context.CoProfiles.Find(uid.CompanyCode, uid.BranchCode).ProductType;

            if (DealerCode() == "MD")
            {
                var sql = string.Format(@"                      
                select itemInfo.PartNo
     , (item.OnHand - (item.AllocationSP + item.AllocationSR + item.AllocationSL) 
     - (item.ReservedSP + item.ReservedSR + item.ReservedSL)) as Available
     , itemPrice.RetailPriceInclTax, itemInfo.PartName, item.TypeOfGoods
     , case gtgo.ParaValue when 'SPAREPART' then 'SPAREPART' else 'MATERIAL' end GroupTypeOfGoods
     , (case itemInfo.Status when 1 then 'Aktif' else 'Tidak Aktif' end) as Status
     , itemPrice.RetailPrice as Price
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
            ", uid.CompanyCode, uid.BranchCode, prodtyp);

                var queryable = ctx.Context.Database.SqlQuery<NoPart>(sql).AsQueryable();
                return (queryable);

            }
            else
            {
                var wrhid = ctx.Context.CompanyMappings.FirstOrDefault(x => x.CompanyCode == uid.CompanyCode && x.BranchCode == uid.BranchCode).WarehouseMD;
                var cmpnymd = ctx.Context.CompanyMappings.FirstOrDefault(x => x.CompanyCode == uid.CompanyCode && x.BranchCode == uid.BranchCode).CompanyMD;
                var brchmd = ctx.Context.CompanyMappings.FirstOrDefault(x => x.CompanyCode == uid.CompanyCode && x.BranchCode == uid.BranchCode).BranchMD;

                var sql = string.Format(@"
                select itemInfo.PartNo
     , (item.OnHand - (item.AllocationSP + item.AllocationSR + item.AllocationSL) 
     - (item.ReservedSP + item.ReservedSR + item.ReservedSL)) as Available
     , itemPrice.RetailPriceInclTax, itemInfo.PartName, item.TypeOfGoods
     , case gtgo.ParaValue when 'SPAREPART' then 'SPAREPART' else 'MATERIAL' end GroupTypeOfGoods
     , (case itemInfo.Status when 1 then 'Aktif' else 'Tidak Aktif' end) as Status
     , itemPrice.RetailPrice as Price
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
   and ItemLoc.WarehouseCode = '{3}'
  left join gnMstLookupDtl gtgo
    on gtgo.CompanyCode = item.CompanyCode
   and gtgo.CodeID = 'GTGO'
   and gtgo.LookupValue = item.TypeOfGoods
where itemInfo.CompanyCode = '{0}'
   and itemInfo.ProductType = '{2}'
   and itemLoc.partno is not null
            ", cmpnymd, brchmd, prodtyp, wrhid);

                //using (var ctxMD = new MDContext())
                //{
                    var queryable = ctxMD.Context.Database.SqlQuery<NoPart>(sql).AsQueryable();
                    return queryable;
                //}
            }
        }

        [HttpGet]
        public IQueryable<DiscountLookup> ListDiscountServiceLookup(string customerCode, string chassisCode, decimal chassisNo, string jobtype)
        {
            var uid = CurrentUser();

            string qry = string.Format("exec uspfn_SvListDicount '{0}', '{1}', '{2}', '{3}', {4}, '{5}', '{6}'",
                uid.CompanyCode, uid.BranchCode, customerCode, chassisCode, chassisNo
                , DateTime.Now, jobtype);
            var queryable = ctx.Context.Database.SqlQuery<DiscountLookup>(qry)
                .AsQueryable<DiscountLookup>().OrderBy(p => p.SeqNo);
            return (queryable);


        }


        [HttpGet]
        public IQueryable<InvoiceFP> InvoiceList(string ChassisCode, string ChassisNo, string JobOrderNo)
        {
            var uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<InvoiceFP>(
               string.Format("exec uspfn_SvInqInvList '{0}', '{1}', '{2}', {3}, '{4}'", uid.CompanyCode, uid.BranchCode,
               ChassisCode, ChassisNo, JobOrderNo)).AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<InvoiceFP> SelectLookupInvoice()
        {
            var Uid = CurrentUserInfo();
            var queryable = ctx.Context.Database.SqlQuery<InvoiceFP>("select  trnInvoice.InvoiceNo," +
                    " trnInvoice.InvoiceDate, trnInvoice.JobOrderNo,trnInvoice.JobOrderDate" +
                    " from svTrnInvoice trnInvoice " +
                    " left join svTrnFakturPajak trnFakturPajak on trnInvoice.CompanyCode = trnFakturPajak.CompanyCode and" +
                    " trnInvoice.BranchCode = trnFakturPajak.BranchCode and trnInvoice.FPJNo = trnFakturPajak.FPJNo " +
                    " left join gnMstCustomer mstCust on mstCust.CompanyCode = trnInvoice.CompanyCode and mstCust.CustomerCode = trnInvoice.CustomerCode " +
                    " where trnInvoice.CompanyCode = '" + Uid.CompanyCode + "' and trnInvoice.BranchCode = '" + Uid.BranchCode + "' and " +
                    " trnInvoice.ProductType = '" + Uid.ProductType + "' ").AsQueryable();
            return (queryable);
        }

        #region INPUT VOR

        [HttpGet]
        public IQueryable<VORBrowse> VORBrowse()
        {
            var data = from a in ctx.Context.VORs
                       join b in ctx.Context.Services
                       on new { a.CompanyCode, a.BranchCode, a.ServiceNo }
                       equals new { b.CompanyCode, b.BranchCode, b.ServiceNo } into _b
                       from b in _b.DefaultIfEmpty()
                       join c in ctx.Context.Customers
                       on new { b.CompanyCode, b.CustomerCode }
                       equals new { c.CompanyCode, c.CustomerCode } into _c
                       from c in _c.DefaultIfEmpty()
                       join d in ctx.Context.Employees
                       on new { b.CompanyCode, b.BranchCode, EmployeeID = b.ForemanID }
                       equals new { d.CompanyCode, d.BranchCode, d.EmployeeID } into _d
                       from d in _d.DefaultIfEmpty()
                       join e in ctx.Context.Employees
                       on new { b.CompanyCode, b.BranchCode, EmployeeID = b.MechanicID }
                       equals new { e.CompanyCode, e.BranchCode, e.EmployeeID } into _e
                       from e in _e.DefaultIfEmpty()
                       join f in ctx.Context.svMstRefferenceServices
                       on new { b.CompanyCode, b.ProductType, RefferenceCode = b.JobType, RefferenceType = "JOBSTYPE" }
                       equals new { f.CompanyCode, f.ProductType, f.RefferenceCode, f.RefferenceType } into _f
                       from f in _f.DefaultIfEmpty()
                       join g in ctx.Context.svMstRefferenceServices
                       on new { b.CompanyCode, b.ProductType, RefferenceCode = a.JobDelayCode, RefferenceType = "DELAY_CODE" }
                       equals new { g.CompanyCode, g.ProductType, g.RefferenceCode, g.RefferenceType } into _g
                       from g in _g.DefaultIfEmpty()
                       select new VORBrowse
                       {
                           ServiceNo = b.ServiceNo,
                           JobOrderNo = a.JobOrderNo,
                           JobOrderDate = b.JobOrderDate,
                           JobDelayCode = a.JobDelayCode,
                           JobDelayDesc = g.Description,
                           JobReasonDesc = a.JobReasonDesc,
                           ClosedDate = a.ClosedDate == null ? new DateTime(1900, 1, 1) : a.ClosedDate,
                           CreatedDate = a.CreatedDate,
                           IsSparepart = a.IsSparepart,
                           IsActive = a.IsActive,
                           PoliceRegNo = b.PoliceRegNo,
                           BasicModel = b.BasicModel,
                           TransmissionType = b.TransmissionType,
                           Odometer = b.Odometer,
                           CustomerCode = b.CustomerCode,
                           CustomerName = c.CustomerName,
                           JobType = b.JobType,
                           JobTypeDesc = f.Description,
                           ForemanID = b.ForemanID,
                           ForemanName = d.EmployeeName,
                           MechanicID = b.MechanicID,
                           MechanicName = e.EmployeeName,
                           Customer = b.CustomerCode + " - " + c.CustomerName,
                           SA = b.MechanicID + " - " + e.EmployeeName,
                           FM = b.ForemanID + " - " + d.EmployeeName,
                           JobTypes = b.JobType + " - " + f.Description
                       };

            return data;
        }

        [HttpGet]
        public IQueryable<JobOrderBrowse> JobOrderBrowse()
        {
            var srvStat = ProductType == "4W";
            string[] stat = { "0", "1", "2", "3", "4", "5" };

            var data = from a in ctx.Context.Services
                       join cust in ctx.Context.Customers
                       on new { a.CompanyCode, a.CustomerCode }
                       equals new { cust.CompanyCode, cust.CustomerCode } into _cust
                       from cust in _cust.DefaultIfEmpty()
                       join custBill in ctx.Context.Customers
                       on new { a.CompanyCode, CustomerCode = a.CustomerCodeBill }
                       equals new { custBill.CompanyCode, custBill.CustomerCode } into _custBill
                       from custBill in _custBill.DefaultIfEmpty()
                       join d in ctx.Context.Employees
                       on new { a.CompanyCode, a.BranchCode, EmployeeID = a.ForemanID }
                       equals new { d.CompanyCode, d.BranchCode, d.EmployeeID } into _d
                       from d in _d.DefaultIfEmpty()
                       join e in ctx.Context.Employees
                       on new { a.CompanyCode, a.BranchCode, EmployeeID = a.MechanicID }
                       equals new { e.CompanyCode, e.BranchCode, e.EmployeeID } into _e
                       from e in _e.DefaultIfEmpty()
                       join f in ctx.Context.svMstRefferenceServices
                       on new { a.CompanyCode, a.ProductType, RefferenceCode = a.JobType, RefferenceType = "JOBSTYPE" }
                       equals new { f.CompanyCode, f.ProductType, f.RefferenceCode, f.RefferenceType } into _f
                       from f in _f.DefaultIfEmpty()
                       join g in ctx.Context.svMstRefferenceServices
                       on new { a.CompanyCode, a.ProductType, RefferenceCode = a.ServiceStatus, RefferenceType = "SERVSTAS" }
                       equals new { g.CompanyCode, g.ProductType, g.RefferenceCode, g.RefferenceType } into _g
                       from g in _g.DefaultIfEmpty()
                       join vor in ctx.Context.VORs
                       on new { a.CompanyCode, a.BranchCode, a.ServiceNo }
                       equals new { vor.CompanyCode, vor.BranchCode, vor.ServiceNo } into _vor
                       from vor in _vor.DefaultIfEmpty()
                       join inv in ctx.Context.Invoices
                       on new { a.CompanyCode, a.BranchCode, a.ProductType, a.JobOrderNo }
                       equals new { inv.CompanyCode, inv.BranchCode, inv.ProductType, inv.JobOrderNo } into _inv
                       from inv in _inv.DefaultIfEmpty()
                       where a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ServiceType == "2" &&
                       new string[] { "0", "1", "2", "3", "4", "5" }.Contains(a.ServiceStatus) && (vor.ServiceNo == null || inv.InvoiceDate != a.JobOrderDate.Value)
                       select new JobOrderBrowse
                       {
                           ServiceNo = a.ServiceNo,
                           JobOrderNo = a.JobOrderNo,
                           JobOrderDate = a.JobOrderDate,
                           ClosedDate = a.EstimateFinishDate == null ? new DateTime(1900, 1, 1) : a.EstimateFinishDate,
                           PoliceRegNo = a.PoliceRegNo,
                           ServiceBookNo = a.ServiceBookNo,
                           BasicModel = a.BasicModel,
                           TransmissionType = a.TransmissionType,
                           ChassisCode = a.ChassisCode + " ",
                           ChassisNo = a.ChassisNo ?? 0,
                           EngineCode = a.EngineCode + " ",
                           EngineNo = a.EngineNo ?? 0,
                           ColorCode = a.ColorCode,
                           CustomerCode = a.CustomerCode,
                           CustomerName = cust.CustomerName,
                           CustomerCodeBill = a.CustomerCodeBill,
                           CustomerCodeBillName = custBill.CustomerName,
                           Pelanggan = a.CustomerCode + " - " + cust.CustomerName,
                           Pembayar = a.CustomerCodeBill + " - " + custBill.CustomerName,
                           Odometer = a.Odometer,
                           JobType = a.JobType,
                           JobTypeDesc = f.Description,
                           ForemanID = a.ForemanID,
                           ForemanName = d.EmployeeName,
                           MechanicID = a.MechanicID,
                           MechanicName = e.EmployeeName,
                           ServiceStatus = a.ServiceStatus == "4" && srvStat ? g.Description : a.ServiceStatus == "4" && !srvStat ? g.LockingBy : g.Description,
                       };

            return data;
        }

        [HttpGet]
        public IQueryable<JobDelayBrowse> JobDelayBrowse(string JobOrderNo)
        {
            var service = ctx.Context.Services.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ProductType == ProductType && a.JobOrderNo == JobOrderNo);

            var data = service != null ? ctx.Context.svMstRefferenceServices.Where(a => a.CompanyCode == CompanyCode && a.ProductType == ProductType
                 && a.RefferenceType == "DELAY_CODE" && a.RefferenceCode != "DELAY1")
                 .Select(c => new JobDelayBrowse
                 {
                     JobDelayCode = c.RefferenceCode,
                     JobDelayDesc = c.Description
                 }) : null;

            return data;
        }

        [HttpGet]
        public IQueryable<VORPart> POSNoBrowse(long ServiceNo)
        {
            var service = ctx.Context.Services.Find(CompanyCode, BranchCode, ProductType, ServiceNo);

            var data = (from a in ctx.Context.spTrnPPOSDtls
                        join b in ctx.Context.spTrnPPOSHdrs
                        on new { a.CompanyCode, a.BranchCode, a.POSNo }
                        equals new { b.CompanyCode, b.BranchCode, b.POSNo } into _b
                        from b in _b.DefaultIfEmpty()
                        where a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ProductType == ProductType
                        && !b.isDeleted.Value && b.POSDate.Value.Year == service.JobOrderDate.Value.Year && b.POSDate.Value.Month >= service.JobOrderDate.Value.Month
                        select new VORPart
                        {
                            POSNo = a.POSNo
                        }).Distinct();
            return data;
        }

        [HttpGet]
        public IQueryable<VORPart> PartNoBrowse(string POSNo)
        {
            var data = ctx.Context.spTrnPPOSDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.POSNo == POSNo).ToList()
                .Select(c => new VORPart
                {
                    PartNo = c.PartNo,
                    PartName = ctx.Context.ItemInfos.Find(CompanyCode, c.PartNo).PartName ?? "",
                    PartQty = c.OrderQty
                }).AsQueryable();

            return data;
        }

        #endregion

        [HttpGet]
        public IQueryable<BrowseVehicleHistory> InqVehicleHistory4Lookup()
        {
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
                    where a.CompanyCode='{0}' and a.IsActive = 1 and e.BranchCode = '{1}' and e.ProfitCenterCode = '{2}'
				", CompanyCode, BranchCode, ProfitCenter);

            var queryable = ctx.Context.Database.SqlQuery<BrowseVehicleHistory>(query).AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<MstCustomerVehicleView> PoliceBrowse()
        {
            var data = from c in ctx.Context.CustomerVehicles
                       join d in ctx.Context.Customers
                       on new { c.CompanyCode, c.CustomerCode }
                       equals new { d.CompanyCode, d.CustomerCode } into _d
                       from d in _d.DefaultIfEmpty()
                       select new MstCustomerVehicleView
                       {
                           PoliceRegNo = c.PoliceRegNo,
                           ServiceBookNo = c.ServiceBookNo,
                           BasicModel = c.BasicModel,
                           TransmissionType = c.TransmissionType,
                           ChassisCode = c.ChassisCode,
                           ChassisNo = c.ChassisNo,
                           EngineCode = c.EngineCode,
                           EngineNo = c.EngineNo,
                           Customer = c.CustomerCode + " - " + d.CustomerName,
                           LastServiceDate = c.LastServiceDate,
                           LastServiceOdometer = c.LastServiceOdometer,
                           LastJobType = c.LastJobType,
                           ContactName = c.ContactName,
                       };
            return data;
        }

        [HttpGet]
        public IQueryable<FPJGCustLookup> FPJGCustLookup()
        {
            var data = ctx.Context.Database.SqlQuery<FPJGCustLookup>(string.Format("exec uspfn_SvInqFpjCust {0},{1}", CompanyCode, BranchCode)).AsQueryable();

            return data;
        }

        [HttpGet]
        public IQueryable<FPJGCustLookup> FPJGHQCustLookup()
        {
            var data = ctx.Context.Database.SqlQuery<FPJGCustLookup>(string.Format("exec uspfn_SvInqFpjCust {0},{1}", CompanyCode, BranchCode)).AsQueryable();

            return data;
        }

        [HttpGet]
        public IQueryable<FPJGCustLookup> FPJHQCustLookup()
        {
            var data = ctx.Context.Database.SqlQuery<FPJGCustLookup>(string.Format("exec uspfn_SvInqFpjCustomer {0},{1},{2}", CompanyCode, BranchCode, ProfitCenter)).AsQueryable();

            return data;
        }

        [HttpGet]
        public IQueryable<GetInqFpjList> TaxInvoice(string docPrefix)
        {
            var data = ctx.Context.Database.SqlQuery<GetInqFpjList>(string.Format("exec uspfn_SvInqFpjList {0},{1},'{2}'", CompanyCode, BranchCode, docPrefix)).AsQueryable();

            return data;
        }

        [HttpGet]
        public IQueryable<GetInqFpjList> TaxInvoiceHQ()
        {
            var data = ctx.Context.Database.SqlQuery<GetInqFpjList>(string.Format("exec uspfn_SvInqFpjHQList {0},{1}", CompanyCode, BranchCode)).AsQueryable();

            return data;
        }

        [HttpGet]
        public IQueryable<KSGSPKLookUp> GenKSGSPKLookup()
        {
            var query = string.Format(@"
                                    select 
	                                    a.BatchNo
	                                    ,a.BatchDate
	                                    ,(
		                                    select top 1 GenerateNo+' ('+substring(BranchCode,len(BranchCode)-1,2)+')'
		                                    from SvTrnPdiFsc
		                                    where 1 = 1
			                                    and CompanyCode = a.CompanyCode
			                                    and ProductType = a.ProductType
			                                    and BatchNo = a.BatchNo
		                                    order by BranchCode,GenerateNo asc
	                                    ) GenerateNoStart
	                                    ,(
		                                    select top 1 GenerateNo+' ('+substring(BranchCode,len(BranchCode)-1,2)+')'
		                                    from SvTrnPdiFsc
		                                    where 1 = 1
			                                    and CompanyCode = a.CompanyCode
			                                    and ProductType = a.ProductType
			                                    and BatchNo = a.BatchNo
		                                    order by GenerateNo,BranchCode desc
	                                    ) GenerateNoEnd
	                                    ,isnull((
		                                    select sum(b.TotalNoOfItem) 
		                                    from SvTrnPdiFsc b 
		                                    where 1 = 1
			                                    and b.CompanyCode = a.CompanyCode
			                                    and b.ProductType = a.ProductType
			                                    and b.BatchNo = a.BatchNo
	                                    ),0) TotalNoOfItem
	                                    ,isnull((
		                                    select sum(b.TotalAmt) 
		                                    from SvTrnPdiFsc b 
		                                    where 1 = 1
			                                    and b.CompanyCode = a.CompanyCode
			                                    and b.ProductType = a.ProductType
			                                    and b.BatchNo = a.BatchNo
	                                    ),0) TotalAmt
                                    from 
	                                    svTrnPdiFscBatch a 
                                    where 
	                                    1 = 1
	                                    and a.CompanyCode = {0}
	                                    and a.BranchCode = {1}
	                                    and a.ProductType = '{2}'", CompanyCode, BranchCode, ProductType);

            var data = ctx.Context.Database.SqlQuery<KSGSPKLookUp>(query).AsQueryable();
            return data;
        }
    }
}