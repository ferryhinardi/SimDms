using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Service.Models;

namespace SimDms.Service.Controllers.Api
{
    public class ReturnController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                BranchCode = BranchCode,
                ProductType = ProductType,
                RefferenceDate = DateTime.Now
            });
        }

        public JsonResult Returns()
        {
            #region -- Query --
            var sql = string.Format(@"
SELECT TOP 1500
 Invoice.CompanyCode, Invoice.BranchCode,
 Invoice.ProductType, Invoice.InvoiceNo, 
 case Invoice.InvoiceDate when ('19000101') then null else Invoice.InvoiceDate end as InvoiceDate
, retur.ReturnNo
, reffServ.DescriptionEng InvoiceStatus
, Invoice.FPJNo
,case Invoice.FPJDate when ('19000101') then null else Invoice.FPJDate end as FPJDate
,Invoice.JobOrderNo
,case Invoice.JobOrderDate when ('19000101') then null else Invoice.JobOrderDate end as JobOrderDate
,Invoice.JobType, Invoice.ChassisCode, Invoice.ChassisNo, Invoice.EngineCode
,Invoice.EngineNo, Invoice.PoliceRegNo, Invoice.BasicModel, Invoice.CustomerCode, Invoice.CustomerCodeBill
,Invoice.Remarks, (Invoice.CustomerCode + ' - ' + Cust.CustomerName) as Customer
,(Invoice.CustomerCodeBill + ' - ' + CustBill.CustomerName) as CustomerBill
, vehicle.ServiceBookNo, Invoice.Odometer
, vehicle.TransmissionType, vehicle.ColourCode
FROM svTrnInvoice Invoice
LEFT JOIN gnMstCustomer Cust
    ON Cust.CompanyCode = Invoice.CompanyCode AND Cust.CustomerCode = Invoice.CustomerCode
LEFT JOIN gnMstCustomer CustBill
    ON CustBill.CompanyCode = Invoice.CompanyCode AND CustBill.CustomerCode = Invoice.CustomerCodeBill
LEFT JOIN svMstcustomerVehicle vehicle 
	ON Invoice.CompanyCode = vehicle.CompanyCode and Invoice.ChassisCode = vehicle.ChassisCode and 
	Invoice.ChassisNo = vehicle.ChassisNo and Invoice.EngineCode = vehicle.EngineCode and 
	Invoice.EngineNo = vehicle.EngineNo and Invoice.BasicModel = vehicle.BasicModel	
LEFT JOIN svMstRefferenceService reffServ 
	ON Invoice.CompanyCode = reffServ.CompanyCode
	AND Invoice.ProductType = reffServ.ProductType
	AND reffServ.RefferenceType = 'INVSTATS'
	AND Invoice.InvoiceStatus = reffServ.RefferenceCode
LEFT JOIN SvTrnReturn retur 
	ON Invoice.CompanyCode = retur.CompanyCode
	AND Invoice.BranchCode = retur.BranchCode
	AND Invoice.ProductType = retur.ProductType
	AND Invoice.InvoiceNo = retur.InvoiceNo
WHERE Invoice.CompanyCode = '{0}' AND Invoice.BranchCode = '{1}' 
    AND Invoice.ProductType = '{2}'
    AND convert(varchar, Invoice.InvoiceDate, 112) >= isnull((
            select top 1 convert(varchar, FromDate, 112) from gnMstPeriode
             where 1 = 1
               and CompanyCode = '{0}'
               and BranchCode = '{1}'
               and FiscalYear = (select FiscalYear from gnMstCoProfileService where CompanyCode = '{0}' and BranchCode = '{1}')
             order by FromDate
            ), '')
", CompanyCode, BranchCode, ProductType); 
            #endregion
            var queryable = ctx.Database.SqlQuery<ReturnView>(sql);
            return Json(GeLang.DataTables<ReturnView>.Parse(queryable.AsQueryable(), Request));
        }

        [HttpPost]
        public JsonResult GetInvoiceComplete(string InvoiceNo)
        {
            #region -- Query --
            var sql = string.Format(@"
select * into #t1 from (
    select
	     a.CompanyCode
	    ,a.BranchCode
	    ,a.ProductType
	    ,a.InvoiceNo
        ,h.POSNo as RefferenceNo
        ,h.POSDate as RefferenceDate
	    ,a.JobOrderNo
	    ,a.PoliceRegNo
	    ,a.ChassisCode
	    ,a.ChassisNo
	    ,b.ColorCode
	    ,rtrim(rtrim(b.ColorCode) +
	     case isnull(c.RefferenceDesc2,'') when '' then '' else ' - ' end +
	     isnull(c.RefferenceDesc2,'')) as ColorCodeDesc
	    ,a.CustomerCode
	    ,d.CustomerName
	    ,d.Address1 CustAddr1
	    ,d.Address2 CustAddr2
	    ,d.Address3 CustAddr3
	    ,d.Address4 CustAddr4
	    ,d.CityCode CityCode
	    ,e.LookupValueName CityName
	    ,a.CustomerCodeBill
	    ,f.CustomerName CustomerNameBill
	    ,f.Address1 CustAddr1Bill
	    ,f.Address2 CustAddr2Bill
	    ,f.Address3 CustAddr3Bill
	    ,f.Address4 CustAddr4Bill
	    ,f.CityCode CityCodeBill
	    ,g.LookupValueName CityNameBill
	    ,f.PhoneNo
	    ,f.FaxNo
	    ,f.HPNo
	    ,a.LaborDiscPct
	    ,a.PartsDiscPct PartDiscPct
	    ,a.MaterialDiscPct
	    ,a.JobType
	    ,b.ForemanID
	    ,b.MechanicID
	    ,b.ServiceStatus
        ,a.FPJNo
        ,a.InvoiceStatus
	from svTrnInvoice a with (nowait,nolock)
	left join svTrnService b with (nowait,nolock)
	  on b.CompanyCode = a.CompanyCode
	 and b.BranchCode  = a.BranchCode
	 and b.ProductType = a.ProductType
	 and b.JobOrderNo  = a.JobOrderNo
	left join omMstRefference c with (nowait,nolock)
	  on c.CompanyCode = b.CompanyCode
	 and c.RefferenceCode = 'COLO'
	 and RefferenceCode = b.ColorCode
	left join gnMstCustomer d with (nowait,nolock)
	  on d.CompanyCode = a.CompanyCode
	 and d.CustomerCode = a.CustomerCode
	left join gnMstLookupDtl e with (nowait,nolock)
	  on e.CompanyCode = a.CompanyCode
	 and e.CodeID = 'CITY'
	 and e.LookUpValue = d.CityCode
	left join gnMstCustomer f with (nowait,nolock)
	  on f.CompanyCode = a.CompanyCode
	 and f.CustomerCode = a.CustomerCodeBill
	left join gnMstLookupDtl g with (nowait,nolock)
	  on g.CompanyCode = a.CompanyCode
	 and g.CodeID = 'CITY'
	 and g.LookUpValue = f.CityCode
    left join spTrnPPOSHdr h
	  on a.CompanyCode = h.CompanyCode
	 and a.BranchCode = h.BranchCode
	 and a.InvoiceNo = h.Remark
   where 1 = 1
	 and a.CompanyCode = '{0}'
	 and a.BranchCode  = '{1}'
	 and a.ProductType = '{2}'
	 and a.InvoiceNo   = '{3}'
) #t1

select
 a.* 
,b.IsContractStatus IsContract
,b.ContractNo
,convert(varchar, c.EndPeriod, 106) ContractEndPeriod
,c.IsActive ContractStatus
,case b.IsContractStatus 
	when 1 then (case c.IsActive when 1 then 'Aktif' else 'Tidak Aktif' end)
    else ''
 end ContractStatusDesc
,b.IsClubStatus IsClub
,b.ClubCode
,convert(varchar, b.ClubDateFinish, 106) ClubEndPeriod
,d.IsActive ClubStatus
,case b.IsClubStatus
	when 1 then (case d.IsActive when 1 then 'Aktif' else 'Tidak Aktif' end)
	else ''
 end ClubStatusDesc
,e.Description JobTypeDesc
,f.Description ServiceStatusDesc
,g.EmployeeName ForemanName
,h.EmployeeName MechanicName
,isnull(i.TaxCode,'') TaxCode
,isnull(j.TaxPct,0) TaxPct
from #t1 a
left join svMstCustomerVehicle b with (nowait,nolock)
  on b.CompanyCode = a.CompanyCode
 and b.ChassisCode = a.ChassisCode
 and b.ChassisNo = a.ChassisNo
left join svMstContract c with (nowait,nolock)
  on c.CompanyCode = a.CompanyCode
 and c.ContractNo = b.ContractNo
 and b.IsContractStatus = 1
left join svMstClub d with (nowait,nolock)
  on d.CompanyCode = a.CompanyCode
 and d.ClubCode = b.ClubCode
left join SvMstRefferenceService e with (nowait,nolock)
  on e.CompanyCode = a.CompanyCode
 and e.ProductType = a.ProductType
 and e.RefferenceCode = a.JobType
 and e.RefferenceType = 'JOBSTYPE'
left join SvMstRefferenceService f with (nowait,nolock)
  on f.CompanyCode = a.CompanyCode
 and f.ProductType = a.ProductType
 and f.RefferenceCode = a.ServiceStatus
 and f.RefferenceType = 'SERVSTAS'
left join gnMstEmployee g with (nowait,nolock)
  on g.CompanyCode = a.CompanyCode
 and g.BranchCode = a.BranchCode
 and g.EmployeeID = a.ForemanID
left join gnMstEmployee h with (nowait,nolock)
  on h.CompanyCode = a.CompanyCode
 and h.BranchCode = a.BranchCode
 and h.EmployeeID = a.MechanicID
left join gnMstCustomerProfitCenter i with (nowait,nolock)
  on i.CompanyCode = a.CompanyCode
 and i.BranchCode = a.BranchCode
 and i.CustomerCode = a.CustomerCode
 and i.ProfitCenterCode = '200'
left join gnMstTax j with (nowait,nolock)
  on j.CompanyCode = a.CompanyCode
 and j.TaxCode = i.TaxCode
order by a.InvoiceNo

drop table #t1
", CompanyCode, BranchCode, ProductType, InvoiceNo); 
            #endregion

            try 
            { 
                var result = ctx.Database.SqlQuery<ReturnInvoice>(sql);
                return Json(new { Message = "", Record = result.FirstOrDefault() });
            }
            catch (Exception ex) 
            { 
                return Json(new { Message = ex.Message }); 
            }
        }

        [HttpPost]
        public JsonResult GetTrnService(string jobOrderNo)
        {
            try
            {
                var record = ctx.Services.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && (jobOrderNo.StartsWith("EST") ? x.EstimationNo == jobOrderNo :
                    jobOrderNo.StartsWith("BOK") ? x.BookingNo == jobOrderNo && x.ServiceType == "1" :
                    jobOrderNo.StartsWith("SPK") ? x.JobOrderNo == jobOrderNo && x.ServiceType == "2" :
                    true));
                
                return Json(new { Message = "", Record = record });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetReturnService(string InvoiceNo)
        {
            try
            {
                var record = ctx.Returns.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && x.InvoiceNo == InvoiceNo);

                return Json(new { Message = "", Record = record });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetInvoiceDetails(string invoiceNo, string jobType, long serviceNo, string jobOrderNo)
        {
            var billType = !string.IsNullOrEmpty(invoiceNo) ? invoiceNo.Substring(2, 1) :
                (jobType.StartsWith("FSC01") || jobType.StartsWith("PDI")) ? "F" :
                jobType.StartsWith("CLAIM") ? "W" :
                GetFirstBillType(serviceNo);
            var data = ctx.Database.SqlQuery<InvoiceDetail>(
                "exec uspfn_SvTrnInvoiceSelectDtl @p0, @p1, @p2, @p3, @p4",
                CompanyCode, BranchCode, ProductType, jobOrderNo, billType);
            if (data.Count() == 0) return Json(null);
            return Json(data);
        }

        [HttpPost]
        public JsonResult GetInvoiceHdr(string invoiceNo)
        {
            try
            {
                var invoice = ctx.Invoices.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && x.InvoiceNo == invoiceNo);
                return Json(new { Message = "", Record = invoice });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ProcessInvoice(Return data)
        {
            try
            {
                var invoice = ctx.Invoices.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == "4W" && x.InvoiceNo == data.InvoiceNo);
                TaxInvoice taxInvoice = null;
                if (invoice != null) taxInvoice = ctx.TaxInvoices.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.FPJNo == invoice.FPJNo);
                if (taxInvoice == null) throw new Exception(
                    "Proses Gagal karena Invoice yang akan di retur belum ada nomor pajaknya");

                var spk = ctx.Documents.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.DocumentType == "SPK");
                if (spk == null) throw new Exception("Master Document tidak ditemukan");

                if (spk.DocumentYear != DateTime.Now.Year) throw new Exception(string.Format(
                    "{0} tidak sesuai dengan {1}", @"Apakah sudah dilakukan Tutup Tahun / Bulan? Tahun Fiscal",
                    "Tahun Transaksi"));

                //var rtn = ctx.Documents.FirstOrDefault(x => x.CompanyCode == CompanyCode
                //    && x.BranchCode == BranchCode && x.DocumentType == "RTN");
                //var currReturnNo = Convert.ToInt32(rtn.DocumentSequence.ToString()) + 1;
                //var noRetur = "RTN/" + DateTime.Now.Year.ToString().Substring(2, 2) + "/" + currReturnNo.ToString().PadLeft(6, '0');
                string RTN = "RTN";
                var noRetur = GetNewDocumentNo(RTN, DateTime.Now);

                var record = new Return
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    ProductType = ProductType,
                    ReturnNo = noRetur,
                    ReturnDate = DateTime.Now,
                    InvoiceNo = invoice.InvoiceNo,
                    InvoiceDate = invoice.InvoiceDate,
                    FPJNo = invoice.FPJNo,
                    FPJDate = invoice.FPJDate,
                    FPJGovNo = taxInvoice.FPJGovNo,
                    CreatedBy = User.Identity.Name,
                    CreatedDate = DateTime.Now,
                    UpdateBy = User.Identity.Name,
                    UpdateDate = DateTime.Now,
                    RefferenceNo = data.RefferenceNo,
                    RefferenceDate = data.RefferenceDate,
                    CustomerCode = invoice.CustomerCode,
                    CustomerCodeBill = invoice.CustomerCodeBill,
                    LockedBy = null,
                    LockedDate = new DateTime(1900, 1, 1),
                };

                var isValid = true;

                isValid = CancelFaktur(noRetur, invoice.InvoiceNo, invoice.JobOrderNo);
                if (isValid)
                {
                    ctx.Returns.Add(record);
                    //rtn.DocumentSequence = currReturnNo;
                    ctx.SaveChanges();
                    return Json(new { Message = "", ReturnNo = noRetur });
                }
                else return Json(new { Message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult PostSave(string jobOrderNo, string invoiceNo)
        {
            try
            {
                var svc = ctx.Services.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && x.JobOrderNo == jobOrderNo);
                if (svc != null)
                {
                    svc.ServiceStatus = "8";
                    ctx.SaveChanges();
                }
                var invoice = ctx.Invoices.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && x.InvoiceNo == invoiceNo);
                if (invoice != null)
                {
                    invoice.InvoiceStatus = "4";
                    if (string.IsNullOrEmpty(svc.InvoiceNo)) svc.InvoiceNo = invoice.InvoiceNo;
                    ctx.SaveChanges();
                }

                return Json(new { Message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        private string GetFirstBillType(long serviceNo)
        {
            var billType = ctx.Database.SqlQuery<string>("exec uspfn_SvGetFirstBillType @p0, @p1, @p2, @p3", 
                CompanyCode, BranchCode, ProductType, serviceNo).FirstOrDefault();
            return billType != null ? billType : "";
        }

        private bool CancelFaktur(string returnNo, string invoiceNo, string jobOrderNo)
        {
            var sql = "exec uspfn_SvTrnReturInvoicePosting @p0, @p1, @p2, @p3, @p4, @p5, @p6";
            var result = ctx.Database.ExecuteSqlCommand(sql,
                CompanyCode, BranchCode, ProductType, invoiceNo, User.Identity.Name,
                returnNo, jobOrderNo);
            return result > 0;
        }
    }
}
