using SimDms.Service.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.Common.Models;
using System.Reflection;
using System.Linq.Expressions;
using SimDms.Common;
using System.IO;
using System.Data.Entity;
using System.Collections.Generic;
using SimDms.Common;

namespace SimDms.Service.Controllers.Api
{
    public class GridController : BaseController
    {
        public JsonResult Estimations()
        {
            var queryable = ctx.JobOrderViews.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.ServiceType == "0" && new string[] { "0", "1", "2", "3", "4", "5" }.Contains(p.ServiceStatus));
            return Json(GeLang.DataTables<JobOrderView>.Parse(queryable, Request));
        }

        public JsonResult Bookings()
        {
            var queryable = ctx.JobOrderViews.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.ServiceType == "1" && new string[] { "0", "1", "2", "3", "4", "5" }.Contains(p.ServiceStatus));
            return Json(GeLang.DataTables<JobOrderView>.Parse(queryable, Request));
        }

        public JsonResult JobOrders()
        {
            var queryable = ctx.JobOrderViews.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.ServiceType == "2" && new string[] { "0", "1", "2", "3", "4", "5" }.Contains(p.ServiceStatus));
            return Json(GeLang.DataTables<JobOrderView>.Parse(queryable, Request));
        }

        public JsonResult JobOrdersByStatus(string serviceStatus)
        {
            if (serviceStatus == null)
            {
                var queryable = ctx.JobOrderViews.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode);
                return Json(GeLang.DataTables<JobOrderView>.Parse(queryable, Request));
            }
            else
            {
                var queryable = ctx.JobOrderViews.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.ServiceStatus == serviceStatus);
                return Json(GeLang.DataTables<JobOrderView>.Parse(queryable, Request));
            }
        }

        public JsonResult JobOrdersByStatusForInvoice(string serviceStatus)
        {
            var ShowAll = Request["ShowAll"] ?? "1";

            var uid = CurrentUser;
            var records = ctx.JobOrderViews
                .Where(p => p.CompanyCode == uid.CompanyCode
                && p.BranchCode == uid.BranchCode && p.ServiceType == "2"
                && new string[] { serviceStatus }
                .Contains(p.ServiceStatus)).OrderByDescending(p => p.ServiceNo);

            if (ShowAll == "0")
            {
                records = ctx.JobOrderViews
                .Where(p => p.CompanyCode == uid.CompanyCode
                && p.BranchCode == uid.BranchCode && p.ServiceType == "2")
                .OrderByDescending(p => p.ServiceNo);
            }

            return Json(records.toKG());

            //if (serviceStatus == null)
            //{
            //    var queryable = ctx.JobOrderViews.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode);
            //    return Json(queryable.toKG());
            //}
            //else
            //{
            //    var queryable = ctx.JobOrderViews.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.ServiceStatus == serviceStatus);
            //    return Json(queryable.toKG());
            //}
        }

        public JsonResult TrnServiceMechanics(string JobOrderNo)
        {
            if (JobOrderNo != null)
            {
                string sql = string.Format(@"select * 
                    from 
                    SvTrnServiceMechanicByJobOrderNoView 
                    where CompanyCode = '{0}'
                    and BranchCode  = '{1}'
                    and ProductType = '{2}'
                    and JobOrderNo = '{3}' 
                    and ServiceType = 2",
                    CompanyCode,
                    BranchCode,
                    ProductType,
                    JobOrderNo);
                var queryable = ctx.Database.SqlQuery<TrnServiceMechanicView>(sql);

                return Json(new { data = queryable.FirstOrDefault() });
            }
            else
            {
                var queryable = ctx.TrnServiceMechanicViews.Where(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType);

                return Json(queryable.toKG());
            }
        }

        public JsonResult CustomerVehicles()
        {
            var queryable = ctx.CustomerVehicleViews.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode);
            return Json(GeLang.DataTables<CustomerVehicleView>.Parse(queryable, Request));
        }

        public JsonResult JobTypes()
        {
            var basicModel = Request["BasicModel"] ?? "";
            var queryable = ctx.JobTypeViews.Where(p => p.CompanyCode == CompanyCode && p.BasicModel == basicModel);
            return Json(GeLang.DataTables<JobTypeView>.Parse(queryable, Request));
        }

        public JsonResult ServiceAdvisors()
        {
            var queryable = ctx.SaViews.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode).OrderBy(p => p.EmployeeName);
            return Json(GeLang.DataTables<SaView>.Parse(queryable, Request));
        }

        public JsonResult Foremans()
        {
            var queryable = ctx.FmViews.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode).OrderBy(p => p.EmployeeName);
            return Json(GeLang.DataTables<FmView>.Parse(queryable, Request));
        }

        public JsonResult InvoicesMaintainFPJ()
        {
            var fiscalYear = ctx.CoProfileServices.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode).FirstOrDefault().FiscalYear;
            var periodeDate = ctx.Periodes.Where(p => p.CompanyCode == p.CompanyCode && p.BranchCode == BranchCode && p.FiscalYear == fiscalYear).OrderBy(p => p.FromDate).FirstOrDefault().FromDate;
            var queryable = ctx.InvoiceViews.Where(p => p.CompanyCode == p.CompanyCode && p.BranchCode == BranchCode && p.ProductType == ProductType &&
                p.InvoiceDate >= periodeDate && p.FPJNo != string.Empty && p.FPJNo.Contains("FPS"));
            return Json(GeLang.DataTables<InvoiceView>.Parse(queryable, Request));
        }

        public JsonResult InvoicesMaintainInv()
        {
            var fiscalYear = ctx.CoProfileServices.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode).FirstOrDefault().FiscalYear;
            var periodeDate = ctx.Periodes.Where(p => p.CompanyCode == p.CompanyCode && p.BranchCode == BranchCode && p.FiscalYear == fiscalYear).OrderBy(p => p.FromDate).FirstOrDefault().FromDate;
            //var invoice1 = (from p in ctx.GLInterfaces
            //                where p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.ProfitCenterCode == ProfitCenter && p.StatusFlag == "0"
            //                select p.DocNo).ToList();
            //var invoice2 = (from p in ctx.ARInterfaces
            //                where p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.ProfitCenterCode == ProfitCenter && p.StatusFlag == "0"
            //                select p.DocNo).ToList();

            //var queryable = ctx.data InvoiceViews.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.ProductType == ProductType &&
            //   p.InvoiceDate >= periodeDate && invoice1.Contains(p.InvoiceNo) && invoice2.Contains(p.InvoiceNo));

            var queryable = ctx.Database.SqlQuery<InvoiceViewSP>(string.Format("exec uspfn_SvTrnMaintainInv {0},{1},'{2}',{3},'{4}'", CompanyCode, BranchCode, ProductType, ProfitCenter, periodeDate.Value.ToString("yyyyMMdd"))).AsQueryable();
            return Json(GeLang.DataTables<InvoiceViewSP>.Parse(queryable, Request));
        }

        public JsonResult InvoicesMaintainCustCodeBill()
        {
            var query = @"
select 
 a.CustomerCode
,a.CustomerName
,a.Address1 + ' ' + a.Address2 + ' ' + a.Address3 + ' ' + a.Address4 as Address
,a.Address1
,a.CustomerAbbrName
,a.Address2, a.Address3, a.Address4, a.CityCode
,ISNULL(d.LookUpValueName, '') as CityDesc
,a.PhoneNo, a.HPNo, a.FaxNo
,e.LookupValueName as ProfitCenter
,case Status when 1 then 'Aktif' else 'Tidak Aktif' end as Status
,f.LookupValueName as TOPDesc
from gnMstCustomer a with(nolock, nowait)
inner join gnMstCustomerProfitCenter b with(nolock, nowait) 
   on b.CustomerCode = a.CustomerCode 
  and b.CompanyCode = a.CompanyCode
left join gnMstCustomerClass c with(nolock, nowait) 
   on c.ProfitCenterCode = b.ProfitCenterCode 
  and c.CustomerClass = b.CustomerClass 
  and c.CompanyCode = b.CompanyCode
  and c.BranchCode = b.BranchCode
left join gnMstLookUpDtl d with(nolock, nowait)
   on d.CompanyCode = a.CompanyCode
  and d.CodeID = 'CITY'
  and d.LookUpValue = a.CityCode
left join gnMstLookUpDtl e with(nolock, nowait)
   on e.CompanyCode = a.CompanyCode
  and e.CodeID = 'PFCN'
  and e.LookUpValue = b.ProfitCenterCode
left join gnMstLookUpDtl f with(nolock, nowait)
   on f.CompanyCode = a.CompanyCode
  and f.CodeID = 'TOPC'
  and f.LookUpValue = b.TOPCode
where b.IsBlackList = 0 
  and b.ProfitCenterCode = @p2
  and a.CompanyCode = @p0
  and b.BranchCode = @p1
  and a.Status = 1
  and a.CategoryCode not in ('00', '01')
";
            var result = ctx.Database.SqlQuery<CustomerCodeBillItem>(query, CompanyCode, BranchCode, ProfitCenter).AsQueryable();

            return Json(GeLang.DataTables<CustomerCodeBillItem>.Parse(result, Request));
        }

        public JsonResult SubCons()
        {

            var ShowAll = (Request["ShowAll"] ?? "0") == "1";

            var queryable = ctx.SubConViews.Where(x => x.CompanyCode == CompanyCode
                && x.BranchCode == BranchCode && x.ProductType == ProductType);

            if (!ShowAll)
                queryable = queryable.Where(x => !new string[] { "1", "5" }.Contains(x.RefferenceCode));

            return Json(queryable.toKG());
        }

        public JsonResult SubCons2()
        {
            var queryable = ctx.SubConViews.Where(x => x.CompanyCode == CompanyCode
                && x.BranchCode == BranchCode && x.ProductType == ProductType
                && x.RefferenceCode == "2");

            return Json(queryable.toKG());
            //return Json(GeLang.DataTables<SubConView>.Parse(queryable, Request));
        }

        public JsonResult SubCons3(string PONo)
        {
            var queryable = ctx.SubConViews.Where(x => x.CompanyCode == CompanyCode
                && x.BranchCode == BranchCode && x.ProductType == ProductType
                && x.RefferenceCode == "2");
            var data = queryable.Where(a => a.PONo == PONo).FirstOrDefault();
            return Json(new { Success = data != null, data = data });
        }

        public JsonResult SubConRcvs()
        {
            var queryable = ctx.SubConRcvViews.Where(x => x.CompanyCode == CompanyCode
                && x.BranchCode == BranchCode && x.ProductType == ProductType
                && new string[] { "3", "5" }.Contains(x.POStatus)).OrderBy(x => x.RecNo)
                .AsQueryable();

            return Json(queryable.toKG());
        }

        public JsonResult SignForSubCon()
        {
            var queryable = ctx.Signatures.Where(x => x.CompanyCode == CompanyCode
                && x.BranchCode == BranchCode && x.ProfitCenterCode == ProfitCenter
                && x.DocumentType == "POT").AsQueryable();

            return Json(queryable.toKG());
        }

        public JsonResult SignForSubConRcv()
        {
            var queryable = ctx.Signatures.Where(x => x.CompanyCode == CompanyCode
                && x.BranchCode == BranchCode && x.ProfitCenterCode == ProfitCenter
                && x.DocumentType == "RRO").AsQueryable();

            return Json(queryable.toKG());
        }

        public JsonResult Suppliers()
        {
            var queryable = ctx.SupplierViews.Where(x => x.CompanyCode == CompanyCode
                && x.BranchCode == BranchCode && x.ProfitCenterCode == ProfitCenter);
            return Json(GeLang.DataTables<SupplierView>.Parse(queryable, Request));
        }

        public JsonResult SuppliersForSubCon()
        {
            var queryable = ctx.SupplierViews.Where(x => x.CompanyCode == CompanyCode
                && x.BranchCode == BranchCode && x.ProfitCenterCode == ProfitCenter);
            return Json(queryable.toKG());
        }

        #region tidak dipakai
        public JsonResult Customers()
        {
            var queryable = ctx.CustomerViews.Where(p => p.CustomerName != "");
            return Json(GeLang.DataTables<CustomerView>.Parse(queryable, Request));
        }

        public JsonResult TaxInvoice()
        {
            var docPrefix = Request["docPrefix"];

            if (IsBranch)
            {
                var queryable = ctx.TaxInvoiceViews.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.Invoice.StartsWith(docPrefix));
                return Json(GeLang.DataTables<TaxInvoiceView>.Parse(queryable, Request));
            }
            else
            {
                var queryable = ctx.TaxInvoiceHQViews.Where(p => p.CompanyCode.Equals(CompanyCode) && p.Invoice.StartsWith(docPrefix)).OrderByDescending(c => c.FPJDate);
                return Json(GeLang.DataTables<TaxInvoiceHQView>.Parse(queryable, Request));
            }
        }
        #endregion

        public JsonResult TaxInvoiceStdLookUp()
        {
            var queryable = ctx.TaxInvoiceLookUpViews.Where(e => e.CompanyCode == CompanyCode && e.BranchCode == BranchCode && e.ProductType == ProductType).OrderBy(e => e.InvoiceDate);
            return Json(GeLang.DataTables<TaxInvoiceLookUpView>.Parse(queryable, Request));
        }

        public JsonResult TaxInvoiceStd()
        {
            var queryable = ctx.TaxInvoiceStdViews.Where(e => e.CompanyCode == CompanyCode && e.BranchCode == BranchCode);
            return Json(GeLang.DataTables<TaxInvoiceStdView>.Parse(queryable, Request));
        }

        public JsonResult ReturnServices()
        {
            var fiscalYear = ctx.CoProfileServices.FirstOrDefault(x =>
                x.CompanyCode == CompanyCode && x.BranchCode == BranchCode).FiscalYear;
            var period = ctx.Periodes.OrderBy(x => x.FromDate).FirstOrDefault(x =>
                x.CompanyCode == CompanyCode && x.BranchCode == BranchCode
                && x.FiscalYear == fiscalYear).FromDate;

            var result = from a in ctx.ReturnViews
                         where a.CompanyCode == CompanyCode && a.BranchCode == BranchCode
                         && a.ProductType == ProductType
                         && a.InvoiceDate.Value >= period
                         && (from b in ctx.ARInterfaces
                             where b.CompanyCode == CompanyCode && b.BranchCode == BranchCode
                             && a.InvoiceNo == b.DocNo
                             && b.ReceiveAmt == 0 && b.BlockAmt == 0 && b.DebetAmt == 0 && b.CreditAmt == 0
                             select b.DocNo).Any()
                         select a;

            return Json(GeLang.DataTables<ReturnView>.Parse(result, Request));
        }

        public JsonResult Signatures()
        {
            var data = ctx.Signatures.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ProfitCenterCode == "200" && a.DocumentType.StartsWith(@"IN"))
                .Select(a => new SignatureViewLookup { SignName = a.SignName, TitleSign = a.TitleSign }).Distinct();

            return Json(GeLang.DataTables<SignatureViewLookup>.Parse(data.AsQueryable(), Request));
        }

        public JsonResult SignaturesForInvoice()
        {
            var data = ctx.Signatures.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ProfitCenterCode == "200" && a.DocumentType.StartsWith(@"IN"))
                .Select(a => new SignatureViewLookup { SignName = a.SignName, TitleSign = a.TitleSign }).Distinct();

            return Json(data.AsQueryable().toKG());
        }

        public JsonResult MaintainSPK()
        {
            var queryable = ctx.MaintainSPKViews.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode);
            return Json(GeLang.DataTables<MaintainSPKView>.Parse(queryable, Request));
        }

        public JsonResult BasicModel()
        {
            var queryable = ctx.BasicModelViews.Where(e => e.CompanyCode == CompanyCode && e.ProductType == ProductType).OrderBy(e => e.RefferenceCode);
            return Json(GeLang.DataTables<BasicModelView>.Parse(queryable, Request));
        }

        public JsonResult TaskType(string basicModel)
        {
            var queryable = ctx.TaskTypeViews.Where(e => e.CompanyCode == CompanyCode && e.ProductType == ProductType && e.BasicModel == basicModel);
            return Json(GeLang.DataTables<TaskTypeView>.Parse(queryable, Request));
        }

        //        public JsonResult PartNo(string basicModel, string jobType)
        //        {

        //            #region -- Query --
        //            var sql = string.Format(@"select a.OperationNo, a.Description as DescriptionTask
        //			 , case '{3}' when 'CLAIM' then isnull(b.ClaimHour, a.ClaimHour) else isnull(b.OperationHour, a.OperationHour) end as Qty
        //			 , case '{3}' when 'CLAIM' then a.LaborCost else isnull(b.LaborPrice, a.LaborPrice) end as Price
        //		  from svMstTask a
        //         left join svMstTaskPrice b
        //            on b.CompanyCode = a.CompanyCode
        //           and b.BranchCode  = '{4}'
        //           and b.ProductType = a.ProductType
        //           and b.BasicModel  = a.BasicModel
        //           and b.JobType     = a.JobType
        //           and b.OperationNo = a.OperationNo
        //		 where a.CompanyCode = '{0}'
        //		   and a.ProductType = '{1}'
        //		   and a.BasicModel = '{2}'
        //           and a.IsActive    = 1
        //           and a.JobType in ('{3}', 'CLAIM','OTHER')
        //", CompanyCode, ProductType, basicModel, jobType, BranchCode);
        //            #endregion

        //            var data = ctx.Database.SqlQuery<TaskNoView>(sql);

        //            //var queryable = ctx.TaskNoViews.Where(e => e.CompanyCode == CompanyCode && e.ProductType == ProductType && e.BasicModel == basicModel);
        //            return Json(GeLang.DataTables<TaskNoView>.Parse(data.AsQueryable(), Request));
        //        }

        public JsonResult InvoiceView()
        {
            var queryable = ctx.InvoiceViews.Where(e => e.CompanyCode == CompanyCode && e.BranchCode == BranchCode && e.ProductType == ProductType);
            return Json(queryable.toKG());

        }

        public JsonResult InvoiceCancelView()
        {
            // Tambah validasi jika SD-MD maka cek data invoice di svSDMovement -- Rudiana 2016-08-01
            var queryable = ctx.InvoiceCancelViews.Where(e => e.CompanyCode == CompanyCode && e.BranchCode == BranchCode && e.ProductType == ProductType);
            if (!IsIndependentDealer)
            {
                var invoceDat = queryable;
                var checkDatInSvSdMov = (from a in invoceDat.ToList()
                                        join b in ctxMD.SvSDMovements 
                                            on new { a.CompanyCode, a.BranchCode, a.InvoiceNo } 
                                            equals new { b.CompanyCode, b.BranchCode, InvoiceNo = b.DocNo }                                                                                                             
                                        where b.ProcessStatus == "0"
                                        select a ).ToList();
                
                queryable = checkDatInSvSdMov.AsQueryable();
            }
            
            return Json(GeLang.DataTables<InvoiceCancelView>.Parse(queryable, Request));
        }

        public JsonResult InvoiceLUView(int jobType)
        {
            #region query
            string query = string.Format(@"
SELECT TOP 1500
 Invoice.CompanyCode, Invoice.BranchCode,Invoice.ProductType, Invoice.InvoiceNo, 
 case Invoice.InvoiceDate when ('19000101') then null else Invoice.InvoiceDate end as InvoiceDate
,Invoice.InvoiceStatus, Invoice.FPJNo
,case Invoice.FPJDate when ('19000101') then null else Invoice.FPJDate end as FPJDate
,Invoice.JobOrderNo
,case Invoice.JobOrderDate when ('19000101') then null else Invoice.JobOrderDate end as JobOrderDate
,Invoice.JobType, Invoice.ChassisCode, Invoice.ChassisNo, Invoice.EngineCode
,Invoice.EngineNo, Invoice.PoliceRegNo, Invoice.BasicModel, Invoice.CustomerCode, Invoice.CustomerCodeBill
,Invoice.Remarks, (Invoice.CustomerCode + ' - ' + Cust.CustomerName) as Customer
,(Invoice.CustomerCodeBill + ' - ' + CustBill.CustomerName) as CustomerBill
, vehicle.ServiceBookNo, Invoice.Odometer
FROM svTrnInvoice Invoice
LEFT JOIN gnMstCustomer Cust
    ON Cust.CompanyCode = Invoice.CompanyCode AND Cust.CustomerCode = Invoice.CustomerCode
LEFT JOIN gnMstCustomer CustBill
    ON CustBill.CompanyCode = Invoice.CompanyCode AND CustBill.CustomerCode = Invoice.CustomerCodeBill
LEFT JOIN svMstcustomerVehicle vehicle 
	ON Invoice.CompanyCode = vehicle.CompanyCode and Invoice.ChassisCode = vehicle.ChassisCode and 
	Invoice.ChassisNo = vehicle.ChassisNo and Invoice.EngineCode = vehicle.EngineCode and 
	Invoice.EngineNo = vehicle.EngineNo and Invoice.BasicModel = vehicle.BasicModel	
LEFT JOIN svMstFSCCampaign fsc 
    ON fsc.CompanyCode = Invoice.CompanyCode
    AND fsc.ChassisCode = Invoice.ChassisCode
    AND fsc.ChassisNo = Invoice.ChassisNo
WHERE Invoice.CompanyCode = '{0}' AND Invoice.BranchCode = '{1}' AND Invoice.ProductType = '{2}'  AND Invoice.InvoiceNo like 'INF%' AND (PostingFlag = 0 or PostingFlag = null)", CompanyCode, BranchCode, ProductType);

            #endregion

            var queryable = ctx.Database.SqlQuery<InvoiceLUView>(query).AsQueryable();

            if (jobType == 0)
                queryable = queryable.Where(c => c.JobType.Equals("PDI"));
            else if (jobType == 1)
                queryable = queryable.Where(c => c.JobType != "PDI");

            return Json(GeLang.DataTables<InvoiceLUView>.Parse(queryable.AsQueryable(), Request));
        }

        public JsonResult InvoiceList(string ChassisCode, string ChassisNo, string JobOrderNo)
        {
            var enumerable = ctx.Database.SqlQuery<InvoiceFP>(
               string.Format("exec uspfn_SvInqInvList '{0}', '{1}', '{2}', {3}, '{4}'", CompanyCode, BranchCode, ChassisCode, ChassisNo, JobOrderNo));
            return Json(GeLang.DataTables<InvoiceFP>.Parse(enumerable.AsQueryable(), Request));
        }

        public JsonResult Mechanics()
        {
            #region ==QUERY==
            var sql = string.Format(@"
                select	EmployeeId, EmployeeName
                from	gnMstEmployee with(nolock, nowait)
                where	CompanyCode = '{0}'
		                and BranchCode = '{1}'
		                and TitleCode in ('9')
                        and isnull(PersonnelStatus, 0) = 1
            ", CompanyCode, BranchCode);
            #endregion
            var data = ctx.Database.SqlQuery<EmployeeLookup>(sql);

            return Json(data.AsQueryable().toKG());
        }

        public JsonResult Branch()
        {
            var queryable = ctx.CoProfiles.Where(a => a.CompanyCode == CompanyCode)
                .Select(e => new BranchLookup { CompanyCode = e.CompanyCode, BranchCode = e.BranchCode, CompanyName = e.CompanyName });

            return Json(GeLang.DataTables<BranchLookup>.Parse(queryable, Request));

        }

        public JsonResult AllBranchFromSPK()
        {
            var queryable = ctx.AllBranchFromSPKs.Where(e => e.CompanyCode == CompanyCode && e.BranchCode == BranchCode && e.ProductType == ProductType);

            return Json(GeLang.DataTables<AllBranchFromSPK>.Parse(queryable, Request));
        }

        public JsonResult AllBranchFromSPKKG()
        {
            var queryable = ctx.AllBranchFromSPKs.Where(e => e.CompanyCode == CompanyCode && e.BranchCode == BranchCode && e.ProductType == ProductType);

            return Json(queryable.toKG());
        }

        public JsonResult AllBranchFromSPKNew()
        {
            var queryable = ctx.AllBranchFromSPKs;

            return Json(GeLang.DataTables<AllBranchFromSPK>.Parse(queryable, Request));
        }

        public JsonResult PdiFscs(string source)
        {
            var queryable = ctx.PdiFscViews.Where(x => x.CompanyCode == CompanyCode
                && x.BranchCode == BranchCode && x.ProductType == ProductType
                && new string[] { "0", "1", "2" }.Contains(x.PostingFlag)
                && !x.FromInvoiceNo.StartsWith("SPK") && !x.ToInvoiceNo.StartsWith("SPK")
                && x.SourceData == source);
            return Json(GeLang.DataTables<PdiFscView>.Parse(queryable, Request));
        }

        public JsonResult PdiFscAll()
        {
            var queryable = ctx.PdiFscViews.Where(x => x.CompanyCode == CompanyCode
                && x.BranchCode == BranchCode && x.ProductType == ProductType
                && new string[] { "0", "1", "2" }.Contains(x.PostingFlag)
                && !x.FromInvoiceNo.StartsWith("SPK") && !x.ToInvoiceNo.StartsWith("SPK"));
            return Json(GeLang.DataTables<PdiFscView>.Parse(queryable, Request));
        }

        public JsonResult SenderDealers()
        {
            var queryable = ctx.SenderDealerViews.Where(x => x.CompanyCode == CompanyCode
                && x.BranchCode == BranchCode && x.ProfitCenterCode == ProfitCenter);

            return Json(queryable.KGrid());
        }

        public JsonResult ReffTypeOpen(string refcode)
        {
            //var queryable = ctx.svMstRefferenceServices.Order;
            //return Json(GeLang.DataTables<svMstRefferenceService>.Parse(queryable.AsQueryable(), Request));
            var queryable = ctx.svMstRefferenceServiceViews.Where(x => x.CompanyCode == CompanyCode
                && x.ProductType == ProductType).OrderBy(x => x.CompanyCode);
            return Json(GeLang.DataTables<svMstRefferenceServiceView>.Parse(queryable, Request));
        }

        public JsonResult BatchPackages()
        {
            var enumerable = ctx.Database.SqlQuery<BatchPackageSP>(
                string.Format("exec uspfn_SvTrnPackageBatchList '{0}', '{1}'", CompanyCode, BranchCode));
            return Json(GeLang.DataTables<BatchPackageSP>.Parse(enumerable.AsQueryable(), Request));
        }

        public JsonResult reffcode(string refcode)
        {
            var queryable = ctx.svMstRefferenceServiceViews.Where(x => x.CompanyCode == CompanyCode
                && x.ProductType == ProductType && x.RefferenceType == refcode);

            //return Json(GeLang.DataTables<svMstRefferenceServiceView>.Parse(queryable, Request));
            return Json(queryable.toKG());
        }

        public JsonResult BillTypeOpen(string refcode)
        {
            var queryable = ctx.svMstBillTypeViews.Where(x => x.CompanyCode == CompanyCode);
            return Json(GeLang.DataTables<svMstBillTypeView>.Parse(queryable, Request));
        }

        public JsonResult getCustomer(string refcode)
        {
            var queryable = ctx.svMstGetCusDets.Where(x => x.CompanyCode == CompanyCode);
            return Json(GeLang.DataTables<svMstGetCusDet>.Parse(queryable, Request));
        }

        public JsonResult StallOpen(string refcode)
        {
            var queryable = ctx.svMstStallViews.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType);
            return Json(GeLang.DataTables<svMstStallView>.Parse(queryable, Request));
        }

        public JsonResult WaktuOpen(string refcode)
        {
            var queryable = ctx.SvMstWaktuKerjaViews.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode);
            return Json(GeLang.DataTables<SvMstWaktuKerjaView>.Parse(queryable, Request));
        }

        public JsonResult MecAvbOpen(string refcode)
        {
            var queryable = ctx.SvMstMecAvbViews.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode);
            return Json(GeLang.DataTables<SvMstMecAvbView>.Parse(queryable, Request));
        }

        public JsonResult GaransiOpen(string refcode)
        {
            var queryable = ctx.SvMstGaransiViews.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType);
            return Json(GeLang.DataTables<SvMstGaransiView>.Parse(queryable, Request));
        }

        public JsonResult SvBMViewOpen(string refcode)
        {
            var queryable = ctx.SvBMViews.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType);
            return Json(GeLang.DataTables<SvBMView>.Parse(queryable, Request));
        }

        public JsonResult SvJTViewOpen(string refcode)
        {
            var queryable = ctx.SvJTViews.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType && x.BasicModel == refcode);
            return Json(GeLang.DataTables<SvJTView>.Parse(queryable, Request));
        }

        public JsonResult CampaignOpen(string refcode)
        {
            var queryable = ctx.SvCampaignViews.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType);
            return Json(queryable.AsQueryable());
        }

        public JsonResult ComplainOpen(string refcode)
        {
            var queryable = ctx.SvComplainViews.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType);
            return Json(GeLang.DataTables<SvComplainView>.Parse(queryable, Request));
        }

        public JsonResult DefectOpen(string refcode)
        {
            var queryable = ctx.SvDefectViews.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType);
            return Json(GeLang.DataTables<SvDefectView>.Parse(queryable, Request));
        }

        public JsonResult OperationOpen(string refcode)
        {
            var queryable = ctx.SvOperationViews.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType);
            return Json(GeLang.DataTables<SvOperationView>.Parse(queryable, Request));
        }

        public JsonResult PekerjaanBrowse(string refcode)
        {
            var queryable = ctx.SvMstPekerjaanBrowses.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType);
            return Json(GeLang.DataTables<SvMstPekerjaanBrowse>.Parse(queryable, Request));
        }

        public JsonResult BasmodPekerjaan(string refcode)
        {
            var queryable = ctx.SvBasicModelPekerjaans.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType);
            return Json(GeLang.DataTables<SvBasicModelPekerjaan>.Parse(queryable, Request));
        }

        public JsonResult JobView(string refcode)
        {
            var queryable = ctx.SvJobViews.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType);
            return Json(GeLang.DataTables<SvJobView>.Parse(queryable, Request));
        }

        public JsonResult GroupJobView(string refcode)
        {
            var queryable = ctx.SvGroupJobViews.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType);
            return Json(GeLang.DataTables<SvGroupJobView>.Parse(queryable, Request));
        }

        public JsonResult NomorAccView(string refcode)
        {
            var queryable = ctx.SvNomorAccViews.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode);
            return Json(GeLang.DataTables<SvNomorAccView>.Parse(queryable, Request));
        }

        //        public JsonResult NoPartOpen()
        //        {
        //            if (DealerCode() == "MD")
        //            {
        //                var sql = string.Format(@"                      
        //                select itemInfo.PartNo
        //     , (item.OnHand - (item.AllocationSP + item.AllocationSR + item.AllocationSL) 
        //     - (item.ReservedSP + item.ReservedSR + item.ReservedSL)) as Available
        //     , itemPrice.RetailPriceInclTax, itemInfo.PartName, item.TypeOfGoods
        //     , case gtgo.ParaValue when 'SPAREPART' then 'SPAREPART' else 'MATERIAL' end GroupTypeOfGoods
        //     , (case itemInfo.Status when 1 then 'Aktif' else 'Tidak Aktif' end) as Status
        //     , itemPrice.RetailPrice as Price
        //  from spMstItemInfo itemInfo 
        //  left join spMstItems item on item.CompanyCode = itemInfo.CompanyCode 
        //   and item.BranchCode = '{1}'
        //   and item.ProductType = '{2}'
        //   and item.PartNo = itemInfo.PartNo
        // inner join spMstItemPrice itemPrice on itemPrice.CompanyCode = itemInfo.CompanyCode 
        //   and itemPrice.BranchCode = '{1}'
        //   and itemPrice.PartNo = item.PartNo
        //  left join spMstItemLoc ItemLoc on itemInfo.CompanyCode = ItemLoc.CompanyCode 
        //   and ItemLoc.BranchCode = '{1}'
        //   and itemInfo.PartNo = ItemLoc.PartNo
        //   and ItemLoc.WarehouseCode = '00'
        //  left join gnMstLookupDtl gtgo
        //    on gtgo.CompanyCode = item.CompanyCode
        //   and gtgo.CodeID = 'GTGO'
        //   and gtgo.LookupValue = item.TypeOfGoods
        //where itemInfo.CompanyCode = '{0}'
        //   and itemInfo.ProductType = '{2}'
        //   and itemLoc.partno is not null
        //            ", CompanyCode, BranchCode, ProductType);
        //                var data = ctx.Database.SqlQuery<NoPart>(sql);

        //                return Json(GeLang.DataTables<NoPart>.Parse(data.AsQueryable(), Request));
        //            }
        //            else
        //            {
        //                var sql = string.Format(@"
        //                select itemInfo.PartNo
        //     , (item.OnHand - (item.AllocationSP + item.AllocationSR + item.AllocationSL) 
        //     - (item.ReservedSP + item.ReservedSR + item.ReservedSL)) as Available
        //     , itemPrice.RetailPriceInclTax, itemInfo.PartName, item.TypeOfGoods
        //     , case gtgo.ParaValue when 'SPAREPART' then 'SPAREPART' else 'MATERIAL' end GroupTypeOfGoods
        //     , (case itemInfo.Status when 1 then 'Aktif' else 'Tidak Aktif' end) as Status
        //     , itemPrice.RetailPrice as Price
        //  from spMstItemInfo itemInfo 
        //  left join spMstItems item on item.CompanyCode = itemInfo.CompanyCode 
        //   and item.BranchCode = '{1}'
        //   and item.ProductType = '{2}'
        //   and item.PartNo = itemInfo.PartNo
        // inner join spMstItemPrice itemPrice on itemPrice.CompanyCode = itemInfo.CompanyCode 
        //   and itemPrice.BranchCode = '{1}'
        //   and itemPrice.PartNo = item.PartNo
        //  left join spMstItemLoc ItemLoc on itemInfo.CompanyCode = ItemLoc.CompanyCode 
        //   and ItemLoc.BranchCode = '{1}'
        //   and itemInfo.PartNo = ItemLoc.PartNo
        //   and ItemLoc.WarehouseCode = '{3}'
        //  left join gnMstLookupDtl gtgo
        //    on gtgo.CompanyCode = item.CompanyCode
        //   and gtgo.CodeID = 'GTGO'
        //   and gtgo.LookupValue = item.TypeOfGoods
        //where itemInfo.CompanyCode = '{0}'
        //   and itemInfo.ProductType = '{2}'
        //   and itemLoc.partno is not null
        //            ", CompanyMD, BranchMD, ProductType, WarehouseMD);
        //                var data = ctxMD.Database.SqlQuery<NoPart>(sql);

        //                return Json(GeLang.DataTables<NoPart>.Parse(data.AsQueryable(), Request));
        //            }                    
        //        }

        public JsonResult KSGOpen(string refcode)
        {
            var queryable = ctx.svMstPdiFscViews.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType);
            return Json(GeLang.DataTables<svMstPdiFscView>.Parse(queryable, Request));
        }

        public JsonResult BasicKsgOpen(string refcode)
        {
            var queryable = ctx.SvBasicKsgViews.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType);
            return Json(GeLang.DataTables<SvBasicKsgView>.Parse(queryable, Request));
        }

        public JsonResult TafjaOpen(string refcode)
        {
            var queryable = ctx.SvTarifJasaViews.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType && x.BranchCode == BranchCode);
            return Json(GeLang.DataTables<SvTarifJasaView>.Parse(queryable, Request));
        }

        public JsonResult ClubOpen(string refcode)
        {
            var queryable = ctx.SvClubViews.Where(x => x.CompanyCode == CompanyCode);
            return Json(GeLang.DataTables<SvClubView>.Parse(queryable, Request));
        }

        public JsonResult NoPolisiOpen(string refcode)
        {
            var queryable = ctx.SvNoPolisis.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode);
            return Json(GeLang.DataTables<SvNoPolisi>.Parse(queryable, Request));
        }

        public JsonResult InquiryClaimLku(string branchFrom, string branchTo, int ClaimType)
        {
            bool isSprClaim = ClaimType == 1 ? true : false;
            var query = "exec uspfn_SvInqGetClaimLku {0}, {1}, {2}, {3}";
            object[] parameters = { CompanyCode, branchFrom, branchTo, isSprClaim };
            var data = ctx.Database.SqlQuery<InquiryClaim>(query, parameters);

            return Json(data.AsQueryable().KGrid());
        }

        public JsonResult Claim(string SourceData, int ClaimType = 0, bool report = false)
        {
            IQueryable<ClaimView> queryable;

            if (!report)
            {
                if (ClaimType != null)
                {
                    bool isSprClaim = ClaimType == 1 ? true : false;
                    queryable = ctx.ClaimViews.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ProductType == ProductType
                        && a.IsSparepartClaim == isSprClaim && a.SourceData == SourceData).OrderByDescending(a => a.GenerateNo);

                    if (Request["fltStatus"] == null)
                    {
                        string fltStatusDefault = Request["fltStatusDefault"];
                        if (fltStatusDefault == null)
                        {
                            queryable = queryable.Where(a => a.PostingFlag == "2");
                        }
                        else
                        {
                            queryable = queryable.Where(a => a.PostingFlag == fltStatusDefault);
                        }
                    }
                    else
                    {
                        string fltStatus = Request["fltStatus"];
                        if (!string.IsNullOrWhiteSpace(fltStatus)) queryable = queryable.Where(a => a.PostingFlag == fltStatus);
                    }
                }
                else
                {
                    queryable = ctx.ClaimViews.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ProductType == ProductType
                        && a.SourceData == SourceData).OrderByDescending(a => a.GenerateNo);
                }
            }
            else
            {
                queryable = ctx.ClaimViews.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ProductType == ProductType
                    && a.SourceData == SourceData).OrderByDescending(a => a.GenerateNo);
            }

            return Json(queryable.toKG());
        }

        public JsonResult KJobOrders()
        {
            var queryable = ctx.JobOrderViews.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.ServiceType == "2" && new string[] { "0", "1", "2", "3", "4", "5" }.Contains(p.ServiceStatus));

            string fltSPKNo = Request["fltSPKNo"];
            string fltPoliceNo = Request["fltPoliceNo"];
            string fltServiceBookNo = Request["fltServiceBookNo"];
            string fltEngineNo = Request["fltEngineNo"];
            string fltChassisNo = Request["fltChassisNo"];
            string fltCustomer = Request["fltCustomer"];

            if (!string.IsNullOrWhiteSpace(fltSPKNo)) queryable = queryable.Where(a => a.JobOrderNo == fltSPKNo);
            if (!string.IsNullOrWhiteSpace(fltPoliceNo)) queryable = queryable.Where(a => a.PoliceRegNo == fltPoliceNo);
            if (!string.IsNullOrWhiteSpace(fltServiceBookNo)) queryable = queryable.Where(a => a.ServiceBookNo == fltServiceBookNo);
            if (!string.IsNullOrWhiteSpace(fltEngineNo)) queryable = queryable.Where(a => a.EngineCode == fltEngineNo);
            if (!string.IsNullOrWhiteSpace(fltChassisNo)) queryable = queryable.Where(a => a.EngineCode == fltEngineNo);
            if (!string.IsNullOrWhiteSpace(fltCustomer)) queryable = queryable.Where(a => a.Customer.Contains(fltCustomer));

            return Json(queryable.KGrid());
        }

        public JsonResult Category()
        {
            var queryable = ctx.RefferenceServiceViews.Where(a => a.CompanyCode == CompanyCode && a.ProductType == ProductType && a.RefferenceType == "CLAIMCAT");
            return Json(GeLang.DataTables<RefferenceServiceView>.Parse(queryable, Request));
        }

        public JsonResult BasicModelClaim()
        {
            var queryable = ctx.Models.Where(a => a.CompanyCode == CompanyCode).Select(a => new BasicModelClaim { BasicModel = a.BasicModel }).Distinct().OrderBy(a => a.BasicModel);
            return Json(GeLang.DataTables<BasicModelClaim>.Parse(queryable, Request));
        }

        public JsonResult Complain()
        {
            var queryable = ctx.RefferenceServiceViews.Where(a => a.CompanyCode == CompanyCode && a.ProductType == ProductType && a.RefferenceType == "COMPLNCD");
            return Json(GeLang.DataTables<RefferenceServiceView>.Parse(queryable, Request));
        }

        public JsonResult Defect()
        {
            var queryable = ctx.RefferenceServiceViews.Where(a => a.CompanyCode == CompanyCode && a.ProductType == ProductType && a.RefferenceType == "DEFECTCD");
            return Json(GeLang.DataTables<RefferenceServiceView>.Parse(queryable, Request));
        }

        public JsonResult BasicCode(string basicmodel)
        {
            var queryable = ctx.BasicCodeViews.Where(a => a.CompanyCode == CompanyCode && a.ProductType == ProductType && a.BasicModel == basicmodel);
            return Json(GeLang.DataTables<BasicCodeView>.Parse(queryable, Request));
        }

        public JsonResult PartNoClaim()
        {
            #region -- Query --
            var query = string.Format(@"
declare @ClaimDisc numeric(5,2)
	set @ClaimDisc = 0

if exists (
	select * from gnMstLookupDtl a
	 where a.CompanyCode = '{0}'
	   and a.CodeID      = 'SRV_FLAG'
	   and a.LookUpValue = 'CLM_DISC'
   )
begin
	set @ClaimDisc = isnull((
		select convert(numeric(5, 2), ParaValue) from gnMstLookupDtl a
		 where a.CompanyCode = '{0}'
		   and a.CodeID      = 'SRV_FLAG'
		   and a.LookUpValue = 'CLM_DISC'
	   ), 0)
end   

select itemInfo.PartNo
     , (item.OnHand - (item.AllocationSP + item.AllocationSR + item.AllocationSL) 
     - (item.ReservedSP + item.ReservedSR + item.ReservedSL)) as Available
     , itemPrice.RetailPriceInclTax
     , itemPrice.RetailPrice
     , ceiling(itemPrice.RetailPrice * ((100.0 - @ClaimDisc)/100.0)) ClaimPrice
     , itemInfo.PartName
  from spMstItemInfo itemInfo 
  left join spMstItems item
    on item.CompanyCode      = itemInfo.CompanyCode
   and item.BranchCode       = '{1}'
   and item.ProductType      = '{2}'
   and item.PartNo           = itemInfo.PartNo
 inner join spMstItemPrice itemPrice
    on itemPrice.CompanyCode = itemInfo.CompanyCode
   and itemPrice.BranchCode  = '{1}'
   and itemPrice.PartNo      = item.PartNo
  left join spMstItemLoc ItemLoc
    on itemInfo.CompanyCode  = ItemLoc.CompanyCode 
   and ItemLoc.BranchCode    = '{1}'
   and itemInfo.PartNo       = ItemLoc.PartNo
   and ItemLoc.WarehouseCode = '00'
 where itemInfo.CompanyCode  = '{0}'
   and itemInfo.ProductType  = '{2}'
   and itemInfo.Status       = 1
   and itemLoc.partno is not null", CompanyCode, BranchCode, ProductType);

            #endregion

            var data = ctx.Database.SqlQuery<PartNoClaim>(query);

            //var queryable = ctx.TaskNoViews.Where(e => e.CompanyCode == CompanyCode && e.ProductType == ProductType && e.BasicModel == basicModel);
            return Json(GeLang.DataTables<PartNoClaim>.Parse(data.AsQueryable(), Request));
        }

        public JsonResult KontrakServiceOpen(string basicmodel)
        {
            var queryable = ctx.SvKontrakServiceViews.Where(a => a.CompanyCode == CompanyCode);
            return Json(GeLang.DataTables<SvKontrakServiceView>.Parse(queryable, Request));
        }

        public JsonResult CutomerDetailOpen(string basicmodel)
        {
            var queryable = ctx.SvCustomerDetailViews.Where(a => a.CompanyCode == CompanyCode && a.ProfitCenterCode == ProfitCenter);
            return Json(GeLang.DataTables<SvCustomerDetailView>.Parse(queryable, Request));
        }

        public JsonResult VehicleDetailOpen(string cuscode)
        {
            var queryable = ctx.SvVehicleDetailViews.Where(a => a.CompanyCode == CompanyCode && a.CustomerCode == cuscode);
            return Json(GeLang.DataTables<SvVehicleDetailView>.Parse(queryable, Request));
        }

        public JsonResult getMekanik()
        {
            var queryable = ctx.SvGetMekaniks.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode);
            return Json(GeLang.DataTables<SvGetMekanik>.Parse(queryable, Request));
        }

        public JsonResult RincianBrowser(string refcode, string refcode1)
        {
            var queryable = ctx.SvRincianBrowsers.Where(a => a.CompanyCode == CompanyCode && a.ProductType == ProductType && a.BasicModel == refcode && a.JobType == refcode1);
            return Json(GeLang.DataTables<SvRincianBrowser>.Parse(queryable, Request));
        }

        public JsonResult KendaraanPel()
        {
            //IQueryable<SvKendaraanPel> queryable;
            //var queryable = ctx.SvKendaraanPels.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ProfitCenterCode == ProfitCenter).Select(a => new {
            var queryable = ctx.SvKendaraanPels.Where(a => a.CompanyCode == CompanyCode && a.ProfitCenterCode == ProfitCenter).Select(a => new
            {
                PoliceRegNo = a.PoliceRegNo,
                ServiceBookNo = a.ServiceBookNo,
                CustomerDesc = a.CustomerDesc,
                ChassisCode = a.ChassisCode,
                ChassisNo = a.ChassisNo,
                BasicModel = a.BasicModel,
                EngineCode = a.EngineCode,
                EngineNo = a.EngineNo
            });
            string PoliceRegNo = Request["PoliceRegNo"];
            string ServiceBookNo = Request["ServiceBookNo"];
            string CustomerDesc = Request["CustomerDesc"];
            string ChassisCode = Request["ChassisCode"];
            string ChassisNo = Request["ChassisNo"];
            string BasicModel = Request["BasicModel"];
            string EngineCode = Request["EngineCode"];
            string EngineNo = Request["EngineNo"];

            if (!string.IsNullOrWhiteSpace(PoliceRegNo)) queryable = queryable.Where(a => a.PoliceRegNo == PoliceRegNo);
            if (!string.IsNullOrWhiteSpace(ServiceBookNo)) queryable = queryable.Where(a => a.ServiceBookNo == ServiceBookNo);
            if (!string.IsNullOrWhiteSpace(CustomerDesc)) queryable = queryable.Where(a => a.CustomerDesc == CustomerDesc);
            if (!string.IsNullOrWhiteSpace(ChassisCode)) queryable = queryable.Where(a => a.ChassisCode == ChassisCode);
            if (!string.IsNullOrWhiteSpace(ChassisNo)) queryable = queryable.Where(a => a.ChassisNo == ChassisNo);
            if (!string.IsNullOrWhiteSpace(BasicModel)) queryable = queryable.Where(a => a.BasicModel == BasicModel);
            if (!string.IsNullOrWhiteSpace(EngineCode)) queryable = queryable.Where(a => a.EngineCode == EngineCode);
            if (!string.IsNullOrWhiteSpace(EngineNo)) queryable = queryable.Where(a => a.EngineNo == EngineNo);

            return Json(queryable.KGrid());
            //return Json(GeLang.DataTables<SvKendaraanPel>.Parse(queryable, Request));
        }

        public JsonResult ChassicCodeOpen()
        {
            var queryable = ctx.SvChassicViews.Where(a => a.CompanyCode == CompanyCode);
            return Json(GeLang.DataTables<SvChassicView>.Parse(queryable, Request));
        }

        public JsonResult CBasmodOpen()
        {
            var queryable = ctx.SvCBasmodViews.Where(a => a.CompanyCode == CompanyCode && a.ProductType == ProductType);
            return Json(GeLang.DataTables<SvCBasmodView>.Parse(queryable, Request));
        }

        public JsonResult ColorOpen()
        {
            var queryable = ctx.SvColorViews.Where(a => a.CompanyCode == CompanyCode);
            return Json(GeLang.DataTables<SvColorView>.Parse(queryable, Request));
        }

        public JsonResult DealerOpen()
        {
            var queryable = ctx.SvMstDealerViews.Where(a => a.CompanyCode == CompanyCode);
            return Json(GeLang.DataTables<SvMstDealerView>.Parse(queryable, Request));
        }

        public JsonResult EventBrowse()
        {
            var queryable = ctx.SvEventViews.Where(a => a.CompanyCode == CompanyCode);
            return Json(GeLang.DataTables<SvEventView>.Parse(queryable, Request));
        }

        public JsonResult InquiryPart()
        {
            string fltDealerPart = Request["fltDealerPart"];

            #region ==QUERY==
            string query = string.Format(@"
select
 a.PartNo
,a.PartName
,a.ProductType
,a.Partcategory
,isnull(b.TypeOfGoods,'') TypeOfGoods
,isnull(c.LookupValueName,'') TypeOfGoodsDesc
,b.Status
,(b.OnHand - (b.AllocationSP + b.AllocationSR + b.AllocationSL)-(b.ReservedSP + b.ReservedSR + b.ReservedSL)) as Available
,d.RetailPriceInclTax
,case b.Status
  when '0' then 'Tidak Aktif'
  when '1' then 'Aktif'
  when '2' then 'Diskontinu'
 end as StatusDesc
from spMstItemInfo a
{0} spMstItems b on b.CompanyCode = a.CompanyCode and b.BranchCode = '{2}' and b.PartNo=a.PartNo
{0} gnMstLookupDtl c on c.CodeID='TPGO' and c.CompanyCode = a.CompanyCode and c.LookUpValue=b.TypeOfGoods
left join spMstItemPrice d on d.CompanyCode= a.CompanyCode and d.BranchCode = '{2}' and d.PartNo=a.PartNo
where a.CompanyCode='{1}'
  and a.ProductType='{3}'
  and b.Status in (0,1,2)
", fltDealerPart == "1" ? "inner join" : "left join", CompanyMD, BranchMD, ProductType);

            #endregion

            var data = ctxMD.Database.SqlQuery<InquiryPart>(query).AsQueryable();

            string ftlPartNo = Request["ftlPartNo"];
            string ftlPartType = Request["ftlPartType"];
            string ftlPartName = Request["ftlPartName"];
            string ftlStatus = Request["ftlStatus"];

            if (!string.IsNullOrWhiteSpace(ftlPartNo)) data = data.Where(a => a.PartNo.Contains(ftlPartNo));
            if (!string.IsNullOrWhiteSpace(ftlPartType)) data = data.Where(a => a.TypeOfGoodsDesc.ToUpper().Contains(ftlPartType.ToUpper()));
            if (!string.IsNullOrWhiteSpace(ftlPartName)) data = data.Where(a => a.PartName.ToUpper().Contains(ftlPartName.ToUpper()));
            if (!string.IsNullOrWhiteSpace(ftlStatus)) data = data.Where(a => a.StatusDesc.ToUpper().Equals(ftlStatus.ToUpper()));

            return Json(data.AsQueryable().KGrid());
        }

        public JsonResult BasicCopy()
        {
            var queryable = ctx.SvBasicModelPekerjaans.Where(a => a.CompanyCode == CompanyCode && a.ProductType == ProductType);
            return Json(GeLang.DataTables<SvBasicModelPekerjaan>.Parse(queryable, Request));
        }

        public JsonResult PaymentOpen()
        {
            var queryable = ctx.SvPaymentPackages.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ProfitCenterCode == ProfitCenter);
            return Json(GeLang.DataTables<SvPaymentPackage>.Parse(queryable, Request));
        }

        public JsonResult PackageBrowse()
        {
            var queryable = ctx.svMstPackages.Where(a => a.CompanyCode == CompanyCode);
            return Json(GeLang.DataTables<svMstPackage>.Parse(queryable, Request));
        }

        public JsonResult RegPackageOpen(string basmod)
        {
            var queryable = ctx.svMstPackages.Where(a => a.CompanyCode == CompanyCode && a.BasicModel == basmod);
            return Json(GeLang.DataTables<svMstPackage>.Parse(queryable, Request));
        }

        public JsonResult NoPartPack()
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
            var data = ctx.Database.SqlQuery<NoPart>(sql).AsQueryable();

            string PartNo = Request["PartNo"];
            string PartName = Request["PartName"];

            if (!string.IsNullOrWhiteSpace(PartNo)) data = data.Where(a => a.PartNo == PartNo);
            if (!string.IsNullOrWhiteSpace(PartName)) data = data.Where(a => a.PartName == PartName);

            return Json(data.AsQueryable().KGrid());
        }

        public JsonResult OperationPackage(string basmod, string jobtype)
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
            ", CompanyCode, BranchCode, ProductType, basmod, jobtype);
            #endregion
            var data = ctx.Database.SqlQuery<OpNumberPackage>(sql).AsQueryable();

            string OperationNo = Request["OperationNo"];
            string OperationName = Request["OperationName"];

            if (!string.IsNullOrWhiteSpace(OperationNo)) data = data.Where(a => a.OperationNo == OperationNo);
            if (!string.IsNullOrWhiteSpace(OperationName)) data = data.Where(a => a.OperationName == OperationName);

            return Json(data.AsQueryable().KGrid());
        }

        public JsonResult RegPackageBrowser()
        {
            var queryable = ctx.SvRegPackageViews.Where(a => a.CompanyCode == CompanyCode);
            return Json(GeLang.DataTables<SvRegPackageView>.Parse(queryable, Request));
        }

        public JsonResult TargetBrowse()
        {
            var queryable = ctx.SvTargetViews.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ProductType == ProductType);
            return Json(GeLang.DataTables<SvTargetView>.Parse(queryable, Request));
        }

        public JsonResult AccountBrowse()
        {
            var queryable = ctx.svMstAccounts.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode);
            return Json(GeLang.DataTables<svMstAccount>.Parse(queryable, Request));
        }
        public JsonResult RegCampaignBrowse()
        {
            var queryable = ctx.SvRegCampaigns.Where(a => a.CompanyCode == CompanyCode);
            return Json(GeLang.DataTables<SvRegCampaign>.Parse(queryable, Request));
        }
        public JsonResult btnCampaign()
        {
            #region -- Query --
            var query = string.Format(@"
select * from(
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
, cust.CityCode, vehicle.BasicModel as SalesModel
, rtrim(rtrim(cust.Address1) + ' ' + rtrim(cust.Address2) + ' ' + rtrim(cust.Address3) + ' ' + rtrim(cust.Address4)) as Address
, isnull(cust.IbuKota,'') CityName		                
from	svMstCustomerVehicle vehicle with(nolock, nowait)
inner join gnMstCustomer cust with(nolock, nowait) on vehicle.CompanyCode = cust.CompanyCode
and vehicle.CustomerCode = cust.CustomerCode		                                                          
where vehicle.CompanyCode = '6006406' 
union
select distinct
PoliceRegistrationNo PoliceRegNo
, a.ChassisCode
, a.ChassisNo
, a.EngineCode
, a.EngineNo
, a.ServiceBookNo	                
, c.CustomerCode
, d.CustomerName
, d.Address1
, d.Address2
, d.Address3
, d.CityCode , b.BasicModel as SalesModel
, rtrim(rtrim(d.Address1) + ' ' + rtrim(d.Address2) + ' ' + rtrim(d.Address3) + ' ' + rtrim(d.Address4)) as Address
, isnull(d.IbuKota,'') CityName		 
from omMstVehicle a with(nolock, nowait)
left join omMstModel b on a.CompanyCode = b.CompanyCode
and a.EngineCode = b.EngineCode	
left join omTrSalesSO c  with(nolock, nowait) on a.CompanyCode = b.CompanyCode
and c.BranchCode = '6006401'
and c.SONo = a.SONo
inner join gnMstCustomer d with(nolock, nowait) on d.CompanyCode = a.CompanyCode
and d.CustomerCode = c.CustomerCode
) temp
where PoliceRegNo != ''
", CompanyCode, BranchCode);

            #endregion

            var data = ctx.Database.SqlQuery<RegCampaign>(query).AsQueryable();

            string PoliceRegNo = Request["PoliceRegNo"];
            string ChassisCode = Request["ChassisCode"];
            string EngineCode = Request["EngineCode"];
            string CustomerCode = Request["CustomerCode"];

            if (!string.IsNullOrWhiteSpace(PoliceRegNo)) data = data.Where(a => a.PoliceRegNo == PoliceRegNo);
            if (!string.IsNullOrWhiteSpace(ChassisCode)) data = data.Where(a => a.ChassisCode == ChassisCode);
            if (!string.IsNullOrWhiteSpace(EngineCode)) data = data.Where(a => a.EngineCode == EngineCode);
            if (!string.IsNullOrWhiteSpace(CustomerCode)) data = data.Where(a => a.CustomerCode == CustomerCode);

            return Json(data.AsQueryable().KGrid());
        }

        //public JsonResult TaskPart(string BasicModel, string JobType, string ChassisCode, string ChassisNo, 
        //    string TransType, string ItemType, string BillType)
        //{
        //    var sql = string.Format(
        //        "exec uspfn_SvLkuTaskPart '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}'",
        //        CompanyCode, BranchCode, BasicModel, JobType, ChassisCode, ChassisNo, 
        //        TransType, ItemType, BillType);
        //    var queryable = ctx.Database.SqlQuery<TaskPart>(sql).AsQueryable();
        //    return Json(GeLang.DataTables<TaskPart>.Parse(queryable, Request));
        //}
        public JsonResult MaintainClaim(string KsgClaim)
        {

            #region -- Query --
            var query = string.Format(@"
            declare @KsgClaim    varchar(15)
            set @KsgClaim = '{2}'
            if @KsgClaim = 'KSG'
            begin
	            select top 500 BranchCode, BatchNo, BatchDate, ReceiptNo, ReceiptDate, FPJNo, FPJDate, FPJGovNo
	              from svTrnPdiFscBatch
	             where 1 = 1
	               and CompanyCode = '{0}'
	               and BranchCode = '{1}'
	             order by BatchNo desc
            end
            else
            begin
	            select top 500 BranchCode, BatchNo, BatchDate, ReceiptNo, ReceiptDate, FPJNo, FPJDate, FPJGovNo
	              from svTrnClaimBatch
	             where 1 = 1
	               and CompanyCode = '{0}'
	               and BranchCode = '{1}'
	             order by BatchNo desc
            end", CompanyCode, BranchCode, KsgClaim);

            #endregion

            var data = ctx.Database.SqlQuery<MaintainClaim>(query);
            return Json(data.AsQueryable().KGrid());
        }

        public JsonResult VehicleService()
        {
            var data = ctx.VehicleServiceViews.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode).OrderBy(a => a.PoliceRegNo);
            return Json(data.KGrid());
        }

        public JsonResult TrnNoSpk(string ClaimType)
        {
            var query = "exec uspfn_SvTrnInputInfoSpk {0}, {1}, {2}, {3}";
            object[] parameters = { CompanyCode, BranchCode, ClaimType, CurrentUser.UserId };

            var dt = ctx.Database.SqlQuery<InputInfoSpk>(query, parameters).AsQueryable();
            string JobOrderNo = Request["JobOrderNo"];
            string PoliceRegNo = Request["PoliceRegNo"];
            string ServiceBookNo = Request["ServiceBookNo"];
            string ChassisCode = Request["ChassisCode"];
            string BasicModel = Request["BasicModel"];
            string EngineCode = Request["EngineCode"];

            if (!string.IsNullOrWhiteSpace(JobOrderNo)) dt = dt.Where(a => a.JobOrderNo == JobOrderNo);
            if (!string.IsNullOrWhiteSpace(PoliceRegNo)) dt = dt.Where(a => a.PoliceRegNo == PoliceRegNo);
            if (!string.IsNullOrWhiteSpace(ServiceBookNo)) dt = dt.Where(a => a.ServiceBookNo == ServiceBookNo);
            if (!string.IsNullOrWhiteSpace(ChassisCode)) dt = dt.Where(a => a.ChassisCode == ChassisCode);
            if (!string.IsNullOrWhiteSpace(BasicModel)) dt = dt.Where(a => a.BasicModel == BasicModel);
            if (!string.IsNullOrWhiteSpace(EngineCode)) dt = dt.Where(a => a.EngineCode == EngineCode);

            return Json(dt.AsQueryable().KGrid());
            //return Json(GeLang.DataTables<InputInfoSpk>.Parse(dt.AsQueryable(), Request));
        }

        public JsonResult SvTrnCatCode()
        {
            var queryable = ctx.SvTrnCatCodes.Where(a => a.CompanyCode == CompanyCode && a.ProductType == ProductType);
            return Json(GeLang.DataTables<SvTrnCatCode>.Parse(queryable, Request));
        }

        public JsonResult SvTrnComCode()
        {
            var queryable = ctx.SvTrnComCodes.Where(a => a.CompanyCode == CompanyCode && a.ProductType == ProductType);
            return Json(GeLang.DataTables<SvTrnComCode>.Parse(queryable, Request));
        }

        public JsonResult SvTrnDefCode()
        {
            var queryable = ctx.SvTrnDefCodes.Where(a => a.CompanyCode == CompanyCode && a.ProductType == ProductType);
            return Json(GeLang.DataTables<SvTrnDefCode>.Parse(queryable, Request));
        }

        public JsonResult SvTrnOpNo(long ServiceNo)
        {
            var queryable = ctx.SvTrnOpNos.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ServiceNo == ServiceNo);
            return Json(GeLang.DataTables<SvTrnOpNo>.Parse(queryable, Request));
        }

        public JsonResult CausalPart(long ServiceNo)
        {
            var items = ctx.ServiceItems.Where(a => a.ServiceNo == ServiceNo).Select(c => c.PartNo);

            var ShowAll = Request["ShowAll"] ?? "1";
            string PartNo = Request["PartNo"];
            string PartName = Request["PartName"];

            #region -- Query --
            var query = @"
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
";
            //if (items != null && items.Count() <= 1) query += "and item.PartNo like '%" + items.FirstOrDefault() + "%'";
            var x = string.Join("','", items);

            if (!(ShowAll == "0"))
            {
                if (items != null) query += "and item.PartNo in ('" + x + "')";
            }


            if (!string.IsNullOrWhiteSpace(PartNo)) query += "and item.PartNo like '%" + PartNo + "%'";
            if (!string.IsNullOrWhiteSpace(PartName)) query += "and item.PartName like '%" + PartName + "%'";
            //, CompanyMD, BranchMD, ProductType);

            #endregion

            var data = ctxMD.Database.SqlQuery<CausalPart>(string.Format(query, CompanyMD, BranchMD, ProductType)).ToList();

            return Json(data.AsQueryable().toKG());
        }

        private Expression<Func<GenerateClm, bool>> IndividualPropertySearch
        {
            get
            {

                PropertyInfo[] _properties = new GenerateClm().GetType().GetProperties();
                var paramExpr = Expression.Parameter(typeof(GenerateClm), "val");
                Expression whereExpr = Expression.Constant(true); // default is val => True
                foreach (string key in Request.Params.AllKeys.Where(x => x.StartsWith("filter[filters]") && x.Contains("][field]")))
                {
                    int property = -1;

                    int c = 0;
                    string query = Request[key];//.ToLower();
                    _properties.ToList().ForEach(x =>
                    {

                        if (x.Name == query)
                        {
                            property = c;
                        }
                        c++;
                    });
                    if (property == -1) break;

                    query = key.Replace("filter[filters][", "").Replace("][field]", "");
                    //Where(x => x.Name == "ReceiptNo").SingleOrDefault().GetIndexParameters();
                    query = Request["filter[filters][" + query + "][value]"];
                    var toStringCall = Expression.Call(
                                        Expression.Call(
                                            Expression.Property(paramExpr, _properties[property]), "ToString", new Type[0]),
                                        typeof(string).GetMethod("ToLower", new Type[0]));

                    whereExpr = Expression.And(whereExpr,
                                               Expression.Call(toStringCall,
                                                               typeof(string).GetMethod("Contains"),
                                                               Expression.Constant(query)));

                }
                return Expression.Lambda<Func<GenerateClm, bool>>(whereExpr, paramExpr);
            }
        }

        public JsonResult GenerateClm(int sentralize)
        {
            #region -- Query --
            var query = string.Format(@"
                select a.CompanyCode,
                a.BatchNo, a.BatchDate, a.ReceiptNo, 
                case convert(varchar, a.ReceiptDate, 112) when '19000101' Then getdate() else a.ReceiptDate end as ReceiptDate, 
                case a.FPJNo WHEN '' THEN '-' ELSE a.FPJNo END AS FPJNo, 
                case convert(varchar, a.FPJDate, 112) when '19000101' Then getdate() else a.FPJDate end as FPJDate, 
                case a.FPJGovNo WHEN '' THEN '-' ELSE a.FPJGovNo END AS FPJGovNo, a.LotNo, a.ProcessSeq,
                sum(b.TotalNoOfItem) TotalNoOfItem, 
                sum(b.TotalClaimAmt) TotalClaimAmt, 
                sum(b.OtherCompensationAmt) OtherCompensationAmt,a.LockingBy
                from 
                SvTrnClaim b
                left join SvTrnClaimBatch a on a.CompanyCode = b.CompanyCode and 
                a.BranchCode = b.BranchCode and a.ProductType = b.ProductType and
                a.BatchNo = b.BatchNo
                where 
                a.CompanyCode = @p0
                and a.BranchCode LIKE case when {0} = 1 then '%%' else @p1 end
                and a.ProductType = @p2
                GROUP BY a.BatchNo, BatchDate, ReceiptNo, ReceiptDate,a.FPJNo, a.FPJDate, a.FPJGovNo, a.LotNo, ProcessSeq, a.LockingBy, a.CompanyCode 
                ORDER BY a.BatchNo Desc", sentralize);
            #endregion

            var data = ctx.Database.SqlQuery<GenerateClm>(query, CompanyCode, BranchCode, ProductType).AsQueryable();

            string BatchNo = Request["BatchNo"];
            string ReceiptNo = Request["ReceiptNo"];
            string FPJNo = Request["FPJNo"];
            string FPJGovNo = Request["FPJGovNo"];

            if (!string.IsNullOrWhiteSpace(BatchNo)) data = data.Where(a => a.BatchNo == BatchNo);
            if (!string.IsNullOrWhiteSpace(ReceiptNo)) data = data.Where(a => a.ReceiptNo == ReceiptNo);
            if (!string.IsNullOrWhiteSpace(FPJNo)) data = data.Where(a => a.FPJNo == FPJNo);
            if (!string.IsNullOrWhiteSpace(FPJGovNo)) data = data.Where(a => a.FPJGovNo == FPJGovNo);

            return Json(data.AsQueryable().toKG());
        }

        public JsonResult WarrantyClaimLookup()
        {
            var BranchCode = (string.IsNullOrWhiteSpace(HttpContext.Request["BranchCode"]) ? CurrentUser.BranchCode : HttpContext.Request["BranchCode"]);
            var CompanyCode = CurrentUser.CompanyCode;
            var ProductType = ctx.CoProfiles.Find(CompanyCode, BranchCode).ProductType;
            string qry = string.Format("exec GetWarrantyClaimLookup '{0}', '{1}', '{2}'", CompanyCode, BranchCode, ProductType);
            var data = ctx.Database.SqlQuery<WarrantyClaimLookup>(qry).AsQueryable<WarrantyClaimLookup>();
            return Json(GeLang.DataTables<WarrantyClaimLookup>.Parse(data, Request));
        }

        public JsonResult ListDiscountServiceLookup(string customerCode, string chassisCode, decimal chassisNo, string jobtype)
        {
            string qry = string.Format("exec uspfn_SvListDicount '{0}', '{1}', '{2}', '{3}', {4}, '{5}', '{6}'", CompanyCode, BranchCode, customerCode, chassisCode, chassisNo
                , DateTime.Now, jobtype);
            var data = ctx.Database.SqlQuery<DiscountLookup>(qry).AsQueryable<DiscountLookup>().OrderBy(p => p.SeqNo);
            return Json(GeLang.DataTables<DiscountLookup>.Parse(data, Request));
        }

        public JsonResult ServicePackages()
        {
            string qry = string.Format("exec uspfn_SvTrnPackageList '{0}', '{1}'", CompanyCode, BranchCode);
            var data = ctx.Database.SqlQuery<PackageSP>(qry).AsQueryable<PackageSP>().OrderBy(p => p.GenerateNo);

            return Json(GeLang.DataTables<PackageSP>.Parse(data, Request));
        }

        public JsonResult JobOrderLookup(string ServiceType)
        {
            var data = new List<SvTrnSPKGeneralView>().AsQueryable();
            if (string.IsNullOrWhiteSpace(ServiceType) == false)
            {
                data = ctx.SvTrnSPKGeneralViews.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode);
                if (ServiceType == "0") // Estimasi
                    data = data.Where(p => new string[] { "0", "2" }.Contains(p.ServiceType) && p.EstimationNo != "" && new string[] { "0", "1", "2", "2", "3", "4", "5" }.Contains(p.ServiceStatus))
                        .OrderByDescending(p => p.EstimationNo);

                if (ServiceType == "1") //Booking
                    data = data.Where(p => p.ServiceType == "1").OrderByDescending(p => p.BookingNo);

                if (ServiceType == "2") //SPK
                    data = data.Where(p => p.ServiceType == "2" && new string[] { "0", "1", "2", "2", "3", "4", "5" }.Contains(p.ServiceStatus))
                        .OrderByDescending(p => p.JobOrderNo);
            }

            return Json(data.KGrid());
        }

        public JsonResult CustomerLookup(string categoryCodes)
        {
            var categoryCode = categoryCodes.Split(new string[] { "," }, StringSplitOptions.None);
            var data = from a in ctx.Customers
                       join b in ctx.svGnMstCustomerProfitCenters
                       on new { a.CustomerCode, a.CompanyCode } equals new { b.CustomerCode, b.CompanyCode }
                       where a.CompanyCode == CompanyCode && a.Status == "1" && b.ProfitCenterCode == ProfitCenter
                       && categoryCode.Contains(a.CategoryCode)
                       select new { a.CustomerCode, a.CustomerName };

            return Json(data.AsQueryable().toKG());
        }

        public JsonResult ReceiveWClaimLookup()
        {
            var data = from a in ctx.SvTrnClaimJudgements
                       join b in ctx.Claims
                       on new { a.CompanyCode, a.BranchCode, a.ProductType, a.GenerateNo }
                       equals new { b.CompanyCode, b.BranchCode, b.ProductType, b.GenerateNo }
                       where a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ProductType == ProductType
                       select new
                       {
                           a.SuzukiRefferenceNo,
                           a.SuzukiRefferenceDate,
                           a.ReceivedDate,
                           b.SenderDealerCode,
                           b.SenderDealerName,
                           b.ReceiveDealerCode,
                           b.LotNo,
                           b.BatchNo
                       };

            return Json(data.toKG());
        }

        public JsonResult PartNo(string basicModel, string jobType)
        {
            var prodtyp = ctx.CoProfiles.Find(CompanyCode, BranchCode).ProductType;

            var sql = string.Format(@"select a.OperationNo, isnull(a.Description,'') as DescriptionTask
			 , case '{3}' when 'CLAIM' then isnull(b.ClaimHour, a.ClaimHour) else isnull(b.OperationHour, a.OperationHour) end as Qty
			 , case '{3}' when 'CLAIM' then a.LaborCost else isnull(b.LaborPrice, a.LaborPrice) end as Price
		  from svMstTask a
         left join svMstTaskPrice b
            on b.CompanyCode = a.CompanyCode
           and b.BranchCode  = '{4}'
           and b.ProductType = a.ProductType
           and b.BasicModel  = a.BasicModel
           and b.JobType     = a.JobType
           and b.OperationNo = a.OperationNo
		 where a.CompanyCode = '{0}'
		   and a.ProductType = '{1}'
		   and a.BasicModel = '{2}'
           and a.IsActive    = 1
           and a.JobType in ('{5}', 'CLAIM','OTHER')
        order by a.OperationNo asc
        ", CompanyCode, prodtyp, basicModel, jobType, BranchCode, jobType);

            var rslt = ctx.Database.SqlQuery<TaskNoView>(sql);
            return Json(rslt.AsQueryable().toKG());

            //            if (DealerCode() == "MD")
            //            {
            //                var sql = string.Format(@"select a.OperationNo, a.Description as DescriptionTask
            //                			 , case '{3}' when 'CLAIM' then isnull(b.ClaimHour, a.ClaimHour) else isnull(b.OperationHour, a.OperationHour) end as Qty
            //                			 , case '{3}' when 'CLAIM' then a.LaborCost else isnull(b.LaborPrice, a.LaborPrice) end as Price
            //                		  from svMstTask a
            //                         left join svMstTaskPrice b
            //                            on b.CompanyCode = a.CompanyCode
            //                           and b.BranchCode  = '{4}'
            //                           and b.ProductType = a.ProductType
            //                           and b.BasicModel  = a.BasicModel
            //                           and b.JobType     = a.JobType
            //                           and b.OperationNo = a.OperationNo
            //                		 where a.CompanyCode = '{0}'
            //                		   and a.ProductType = '{1}'
            //                		   and a.BasicModel = '{2}'
            //                           and a.IsActive    = 1
            //                           and a.JobType in ('{5}', 'CLAIM','OTHER')
            //                        order by a.OperationNo asc
            //                        ", CompanyCode, prodtyp, basicModel, jobType, BranchCode, jobType);

            //                var rslt = ctx.Database.SqlQuery<TaskNoView>(sql);

            //                return Json(rslt.AsQueryable().toKG());
            //            }
            //            else
            //            {
            //                var cmpnymd = ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode).CompanyMD;
            //                var brchmd = ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode).BranchMD;

            //                var sql1 = string.Format(@"select a.OperationNo, a.Description as DescriptionTask
            //                			 , case '{3}' when 'CLAIM' then isnull(b.ClaimHour, a.ClaimHour) else isnull(b.OperationHour, a.OperationHour) end as Qty
            //                			 , case '{3}' when 'CLAIM' then a.LaborCost else isnull(b.LaborPrice, a.LaborPrice) end as Price
            //                		  from svMstTask a
            //                         left join svMstTaskPrice b
            //                            on b.CompanyCode = a.CompanyCode
            //                           and b.BranchCode  = '{4}'
            //                           and b.ProductType = a.ProductType
            //                           and b.BasicModel  = a.BasicModel
            //                           and b.JobType     = a.JobType
            //                           and b.OperationNo = a.OperationNo
            //                		 where a.CompanyCode = '{0}'
            //                		   and a.ProductType = '{1}'
            //                		   and a.BasicModel = '{2}'
            //                           and a.IsActive    = 1
            //                           and a.JobType in ('{5}', 'CLAIM','OTHER')
            //                        order by a.OperationNo asc
            //                        ", cmpnymd, prodtyp, basicModel, jobType, brchmd, jobType);

            //                var rslt1 = ctxMD.Database.SqlQuery<TaskNoView>(sql1);

            //                var sql2 = string.Format(@"select a.OperationNo, a.Description as DescriptionTask
            //                			 , case '{3}' when 'CLAIM' then isnull(b.ClaimHour, a.ClaimHour) else isnull(b.OperationHour, a.OperationHour) end as Qty
            //                			 , case '{3}' when 'CLAIM' then a.LaborCost else isnull(b.LaborPrice, a.LaborPrice) end as Price
            //                		  from svMstTask a
            //                         left join svMstTaskPrice b
            //                            on b.CompanyCode = a.CompanyCode
            //                           and b.BranchCode  = '{4}'
            //                           and b.ProductType = a.ProductType
            //                           and b.BasicModel  = a.BasicModel
            //                           and b.JobType     = a.JobType
            //                           and b.OperationNo = a.OperationNo
            //                		 where a.CompanyCode = '{0}'
            //                		   and a.ProductType = '{1}'
            //                		   and a.BasicModel = '{2}'
            //                           and a.IsActive    = 1
            //                           and a.JobType in ('{5}', 'CLAIM','OTHER')
            //                        order by a.OperationNo asc
            //                        ", CompanyCode, prodtyp, basicModel, jobType, BranchCode, jobType);

            //                var rslt2 = ctx.Database.SqlQuery<TaskNoView>(sql2);

            //                var rslt = from b in rslt2
            //                           join a in rslt1 on b.OperationNo equals a.OperationNo
            //                           select new TaskNoView
            //                           {
            //                               OperationNo = a.OperationNo,
            //                               DescriptionTask = a.DescriptionTask,
            //                               Qty = b.Qty,
            //                               Price = b.Price
            //                           };

            //                return Json(rslt.AsQueryable().toKG());
            //            }
        }

        public JsonResult NoPartOpen()
        {
            var prodtyp = ctx.CoProfiles.Find(CompanyCode, BranchCode).ProductType;

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
            ", CompanyCode, BranchCode, prodtyp);

                var takListMD = ctx.Database.SqlQuery<NoPart>(sql);

                var rslt = ctx.Database.SqlQuery<NoPart>(sql);

                return Json(rslt.AsQueryable().toKG());
            }
            else
            {
                var wrhid = ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode).WarehouseMD;
                var cmpnymd = ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode).CompanyMD;
                var brchmd = ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode).BranchMD;

                var sql = string.Format(@"
                select itemInfo.PartNo
                 , (item.OnHand - (item.AllocationSP + item.AllocationSR + item.AllocationSL) 
                 - (item.ReservedSP + item.ReservedSR + item.ReservedSL)) as Available
                 --, itemPrice.RetailPriceInclTax
                 , itemInfo.PartName, item.TypeOfGoods
                 , case gtgo.ParaValue when 'SPAREPART' then 'SPAREPART' else 'MATERIAL' end GroupTypeOfGoods
                 , (case itemInfo.Status when 1 then 'Aktif' else 'Tidak Aktif' end) as Status
                 --, itemPrice.RetailPrice as Price
                 , itemInfo.CompanyCode    
              from spMstItemInfo itemInfo 
              left join spMstItems item on item.CompanyCode = itemInfo.CompanyCode 
               and item.BranchCode = '{1}'
               and item.ProductType = '{2}'
               and item.PartNo = itemInfo.PartNo
             --inner join spMstItemPrice itemPrice on itemPrice.CompanyCode = itemInfo.CompanyCode 
             --and itemPrice.BranchCode = '{1}'
             --and itemPrice.PartNo = item.PartNo
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

                var datItemFromMD = ctxMD.Database.SqlQuery<NoPart>(sql);

                var itemPrice = ctxMD.Database.SqlQuery<ItemPrice>(
                    string.Format("select  * from spMstItemPrice with (nolock,nowait) where companycode= {0} and branchcode={1}", cmpnymd, brchmd)
                    ).AsEnumerable();
                    
                    //(from p in ctxMD.ItemPrices
                    //             where p.CompanyCode == cmpnymd
                    //        && p.BranchCode == brchmd
                    //             select p).AsEnumerable();

                var rslt = from b in itemPrice
                           join a in datItemFromMD on b.PartNo
                           equals a.PartNo
                           select new NoPart
                           {
                               Available = a.Available,
                               GroupTypeOfGoods = a.GroupTypeOfGoods,
                               PartName = a.PartName,
                               PartNo = a.PartNo,
                               Quantity = a.Quantity,
                               RetailPriceInclTax = b.RetailPriceInclTax,
                               Status = a.Status,
                               TypeOfGoods = a.TypeOfGoods,
                               Price = b.RetailPrice
                           };

                return Json(rslt.AsQueryable().toKG());
            }
        }

        public JsonResult LookUpCusVeh()
        {
            string sql = string.Format(@"EXEC uspfn_svCusVehLookUp '{0}', '{1}', '{2}'",
                CompanyCode, BranchCode, Helpers.GetDynamicFilter(Request));

            var records = ctx.Database.SqlQuery<LookupMstCustomerVehilce>(sql).AsQueryable();

            return Json(records.toKG(ApplyFilterKendoGrid.False));
        }

        public JsonResult NoPartOpenblur(string partno)
        {
            var prodtyp = ctx.CoProfiles.Find(CompanyCode, BranchCode).ProductType;

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
                and itemInfo.partno = '{3}'
            ", CompanyCode, BranchCode, prodtyp, partno);

                var rslt = ctx.Database.SqlQuery<NoPart>(sql);

                return Json(rslt.AsQueryable().toKG());

            }
            else
            {
                var wrhid = ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode).WarehouseMD;
                var cmpnymd = ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode).CompanyMD;
                var brchmd = ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode).BranchMD;

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
               and itemInfo.partno = '{4}'
            ", cmpnymd, brchmd, prodtyp, wrhid, partno);

                var rslt = ctxMD.Database.SqlQuery<NoPart>(sql).FirstOrDefault();
                if (rslt == null)
                {
                    return Json(new { success = false });
                }
                else
                {
                    return Json(new { success = true, data = rslt });
                }

            }

        }



        public JsonResult NoPolisiOpenGrid()
        {


            var field = "";
            var value = "";
            string dynamicFilter = "";

            for (int i = 0; i < 7; i++)
            {
                field = Request["filter[filters][" + i + "][field]"] ?? "";
                value = Request["filter[filters][" + i + "][value]"] ?? "";

                if (dynamicFilter == "")
                {
                    dynamicFilter += value != "" ? " AND " + field + " LIKE '%" + value + "%'" : "";
                }
                else
                {
                    dynamicFilter += value != "" ? " AND " + field + " LIKE '%" + value + "%'" : "";
                }
            }

            //dynamicFilter = dynamicFilter != "" ? dynamicFilter += "'" : "";

            string sql = string.Format(@"select top 500 * from SvNoPolisi where CompanyCode='{0}' and BranchCode='{1}' {2}",
                CompanyCode, BranchCode, dynamicFilter);

            var records = ctx.Database.SqlQuery<SvNoPolisi>(sql);


            return Json(records.AsQueryable().toKG());


        }


        public JsonResult KendaraanPelGrid()
        {


            var field = "";
            var value = "";
            string dynamicFilter = "";

            for (int i = 0; i < 7; i++)
            {
                field = Request["filter[filters][" + i + "][field]"] ?? "";
                value = Request["filter[filters][" + i + "][value]"] ?? "";

                if (dynamicFilter == "")
                {
                    dynamicFilter += value != "" ? " AND " + field + " LIKE '%" + value + "%'" : "";
                }
                else
                {
                    dynamicFilter += value != "" ? " AND " + field + " LIKE '%" + value + "%'" : "";
                }
            }

            //dynamicFilter = dynamicFilter != "" ? dynamicFilter += "'" : "";

            string sql = string.Format(@"select top 500 * from SvKendaraanPel where CompanyCode='{0}' and ProfitCenterCode='{1}' {2}",
                CompanyCode, ProfitCenter, dynamicFilter);

            var records = ctx.Database.SqlQuery<SvKendaraanPel>(sql);


            return Json(records.AsQueryable().toKG());


        }


        public JsonResult btnCampaignGrid()
        {

            var field = "";
            var value = "";
            string dynamicFilter = "";

            for (int i = 0; i < 7; i++)
            {
                field = Request["filter[filters][" + i + "][field]"] ?? "";
                value = Request["filter[filters][" + i + "][value]"] ?? "";

                if (dynamicFilter == "")
                {
                    dynamicFilter += value != "" ? " AND " + field + " LIKE '%" + value + "%'" : "";
                }
                else
                {
                    dynamicFilter += value != "" ? " AND " + field + " LIKE '%" + value + "%'" : "";
                }
            }


            var sql = string.Format(@"
                 select	top 500 vehicle.PoliceRegNo 
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
                where vehicle.CompanyCode = '{0}' and PoliceRegNo != '' {1}
                
            
            ", CompanyCode, dynamicFilter);

            var records = ctx.Database.SqlQuery<MstCustomerVehicleForMstRegCompaign>(sql);


            return Json(records.AsQueryable().toKG());
        }

        public JsonResult SelectPart4MstJob(string basicModel, string jobType)
        {
            string sql = string.Format(@"EXEC uspfn_SelectPart4MstJob '{0}', '{1}', '{2}', '{3}', '{4}'",
                CompanyCode, BranchCode, ProductType, basicModel, jobType, Helpers.GetDynamicFilter(Request), 500);

            var records = ctx.Database.SqlQuery<PartInfoView>(sql).AsQueryable();
            return Json(records.toKG(ApplyFilterKendoGrid.False));
        }
    }
}