using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using SimDms.Service.Models;
using SimDms.Common;
using SimDms.Common.Models;

namespace SimDms.Service.Controllers.Api
{
    public class SubConController : BaseController
    {
        #region -- SubCon Ordering --
        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                BranchCode = BranchCode,
                ProductType = ProductType,
                PODate = DateTime.Now
            });
        }

        [HttpPost]
        public JsonResult GetJobOrder(string JobOrderNo)
        {
            try
            {
                var record = ctx.JobOrderViews.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && x.JobOrderNo == JobOrderNo);
                if (record == null) throw new Exception("Job Order not found");
                return Json(new { Message = string.Empty, Record = record });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetSupplier(string supplierCode)
        {
            try
            {
                var cityName = "";
                var phoneNo = "";
                var topDays = "0";
                var podDisc = "0";
                var supplierRec = new Supplier();
                Supplier oGnMstSupplier = ctx.Suppliers.Find(CompanyCode, supplierCode);
                if (oGnMstSupplier != null){
                    GnMstLookUpDtl oGnMstLookUpDtl = ctx.GnMstLookUpDtls.Find(CompanyCode, "CITY",  oGnMstSupplier.CityCode);
                    cityName = (oGnMstLookUpDtl != null) ? oGnMstLookUpDtl.LookUpValueName : "";
                    phoneNo = (oGnMstSupplier.PhoneNo != "" && oGnMstSupplier.HPNo != "") ? oGnMstSupplier.PhoneNo + " / " + oGnMstSupplier.HPNo : ((oGnMstSupplier.PhoneNo != "" && oGnMstSupplier.HPNo == "") ? oGnMstSupplier.PhoneNo : oGnMstSupplier.HPNo);
                    SupplierProfitCenter oGnMstSupplierProfitCenter = ctx.SupplierProfitCenters.Find(CompanyCode, BranchCode, oGnMstSupplier.SupplierCode, ProfitCenter);
                    if (oGnMstSupplierProfitCenter != null)
                    {
                        oGnMstLookUpDtl =  ctx.GnMstLookUpDtls.Find(CompanyCode, "TOPC",  oGnMstSupplierProfitCenter.TOPCode);
                        topDays = (oGnMstLookUpDtl != null) ? oGnMstLookUpDtl.ParaValue : "0";
                        podDisc = oGnMstSupplierProfitCenter.DiscPct.ToString();
                    }

                    supplierRec = oGnMstSupplier;
                }

                return Json(new { Record = new {
                    SupplierCode = supplierRec.SupplierCode,
                    SupplierName = supplierRec.SupplierName,
                    Address1 = supplierRec.Address1,
                    Address2 = supplierRec.Address2,
                    Address3 = supplierRec.Address3,
                    Address4 = supplierRec.Address4,
                    CityCode = supplierRec.CityCode,
                    CityName = cityName,
                    Phone = phoneNo,
                    DiscPct = podDisc,
                    TOPDays = podDisc
                }});
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetPODetails(string poNo)
        {
            var sql = string.Format(@"
                        select (row_number() over (order by a.OperationNo)) No
                        ,a.PONo
                        ,a.OperationNo
                        ,a.OperationHour
                        ,a.POPrice
                        ,isnull(c.Description,'') Description
                        from svTrnPoSubConTask a
                        left join svTrnPoSubCon b on 1 = 1
                         and b.CompanyCode = a.CompanyCode
                         and b.BranchCode = a.BranchCode
                         and b.ProductType = a.ProductType
                         and b.PONo = a.PONo
                        left join svMstTask c on 1 = 1
                         and c.CompanyCode = a.CompanyCode
                         and c.ProductType = a.ProductType
                         and c.BasicModel = b.BasicModel
                         and c.JobType in (b.JobType, 'CLAIM', 'OTHER')
                         and c.OperationNo = a.OperationNo
                        where 1 = 1
                         and a.CompanyCode = '{0}'
                         and a.BranchCode = '{1}'
                         and a.ProductType = '{2}'
                         and a.PONo = '{3}'
                        ", CompanyCode, BranchCode, ProductType, poNo);
            var data = ctx.Database.SqlQuery<SubConTaskView>(sql);
            //if (data.Count() == 0) return Json(null);
            return Json(data);
        }

        [HttpPost]
        public JsonResult Save(SubCon model)
        {
            var message = string.Empty;
            try
            {
                var isNew = false;
                var record = ctx.SubCons.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType && x.PONo == model.PONo);
                if (record == null)
                {
                    isNew = true;
                    record = new SubCon
                    {
                        CompanyCode = CompanyCode,
                        BranchCode = BranchCode,
                        ProductType = ProductType,
                        PODate = model.PODate,
                        RecNo = "",
                        RecDate = new DateTime(1900, 1, 1),
                        FPJNo = "",
                        FPJDate = new DateTime(1900, 1, 1),
                        FPJGovNo = "",
                        LockingBy = "",
                        LockingDate = new DateTime(1900, 1, 1),
                        Remarks = "",
                        PONo = GetNewDocumentNo("POT", model.PODate.Value),
                    };
                }

                record.JobOrderNo = model.JobOrderNo;
                record.SupplierCode = model.SupplierCode;
                record.Remarks = model.Remarks;

                var sql = string.Format(@"
SELECT * FROM SvTrnService 
WHERE CompanyCode = '{0}' AND BranchCode = '{1}' AND ProductType = '{2}' AND ServiceType = '{3}'
AND JobOrderNo = '{4}'"
                    , CompanyCode, BranchCode, ProductType, '2', model.JobOrderNo);

                var service = ctx.Database.SqlQuery<TrnService>(sql).FirstOrDefault();
                if (service == null) throw new Exception("Job Order is not found.");

                var supplier = GetRowSupplierPC(record.SupplierCode);

                if (isNew) message = InsertPO(record, supplier, service);
                else UpdatePO(record, supplier);

                if (message != "") throw new Exception(message);

                var subConView = ctx.SubConViews.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType && x.PONo == record.PONo);

                return Json(new { Message = "", Record = subConView });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult CreatePO(SubCon model)
        {
            var message = string.Empty;
            var subcon = new SubConView();
            try
            {
                if (model.ServiceAmt == 0) throw new Exception("Tidak dapat Buat PO karena nilai harga 0");

                var record = ctx.SubCons.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType && x.PONo == model.PONo);
                record.POStatus = "2";
                var result = ctx.SaveChanges();

                message = UpdatePO(record, GetRowSupplierPC(record.SupplierCode));
                if (message != "") throw new Exception(message);

                subcon = ctx.SubConViews.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && x.PONo == record.PONo);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
            return Json(new { Message = message, Record = subcon });
        }

        [HttpPost]
        public JsonResult CancelPO(SubCon model)
        {
            var message = string.Empty;
            var subcon = new SubConView();
            try
            {
                var record = ctx.SubCons.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType && x.PONo == model.PONo);

                if (model.POStatus == "2") record.POStatus = "0";
                else if (model.POStatus == "0") record.POStatus = "1";
                var result = ctx.SaveChanges();
                if (result > 0) message = UpdatePO(record, GetRowSupplierPC(record.SupplierCode));
                if (message != "") throw new Exception(message);

                subcon = ctx.SubConViews.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && x.PONo == record.PONo);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
            return Json(new { Message = message, Record = subcon });
        }

        [HttpPost]
        public JsonResult SaveDtl(SubConTaskView model)
        {
            var message = string.Empty;
            var subcon = new SubConView();
            try
            {
                if (model.POPrice <= 0) throw new Exception("Harga tidak boleh kurang atau sama dengan 0");
                var result = UpdatePOTask(model);

                var record = ctx.SubCons.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType && x.PONo == model.PONo);

                if (result > 0)
                {
                    result += UpdateSubCon(record);
                    result += UpdateSrvTask(record);
                }

                subcon = ctx.SubConViews.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && x.PONo == record.PONo);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
            return Json(new { Message = message, Record = subcon });
        }

        [HttpPost]
        public JsonResult RemoveDtl(SubConTask model)
        {
            var message = string.Empty;
            try
            {
                var result = 0;
                var detail = ctx.SubConTasks.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType && x.PONo == model.PONo
                    && x.OperationNo == model.OperationNo);
                ctx.SubConTasks.Remove(detail);
                result = ctx.SaveChanges();

                var record = ctx.SubCons.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType && x.PONo == model.PONo);
                result += UpdateSubCon(record);
                result += UpdateSrvTask(record);

                //add by fhi 04/02/2015  : penambahan update IsSubCon== false. saat remove detail
                var rcd = ctx.ServiceTasks.Find(CompanyCode, BranchCode, ProductType, model.PONo, model.OperationNo);
                rcd.IsSubCon = false;
                ctx.SaveChanges();

                //end
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(new { Message = message });
        }

        public JsonResult LookupJobOrder()
        {
            var records = (from PoSubCon in ctx.SubCons
                join supplier in ctx.Suppliers on new { PoSubCon.CompanyCode, PoSubCon.SupplierCode } equals new { supplier.CompanyCode, supplier.SupplierCode } into _supplier
                from supplier in _supplier.DefaultIfEmpty()
                join RefferenceService in ctx.svMstRefferenceServices on new { PoSubCon.CompanyCode, PoSubCon.ProductType, PoSubCon.POStatus, RefferenceType = "PORRSTAT" }
                equals new { RefferenceService.CompanyCode, RefferenceService.ProductType, POStatus = RefferenceService.RefferenceCode, RefferenceService.RefferenceType } into _RefferenceService
                from RefferenceService in _RefferenceService.DefaultIfEmpty()
                where PoSubCon.CompanyCode ==  CompanyCode 
                && PoSubCon.BranchCode ==  BranchCode
                && PoSubCon.ProductType == ProductType
                orderby PoSubCon.PONo ascending
                select new {
                    PoSubCon.PONo,
                    PoSubCon.PODate,
                    PoSubCon.JobOrderNo,
                    PoSubCon.JobOrderDate, 
                    PoSubCon.SupplierCode,
                    supplier.SupplierName,
                    PoSubCon.PODisc,
                    PoSubCon.ServiceAmt, 
                    RefferenceService.Description
                }).AsQueryable();

            return Json(records.toKG());
        }

        #endregion

        #region -- SubCon Receiving --
        public JsonResult RcvDefault()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                BranchCode = BranchCode,
                ProductType = ProductType,
                RecDate = DateTime.Now,
                FPJDate = DateTime.Now,
                Year = DateTime.Now.Year
            });
        }

        [HttpPost]
        public JsonResult RcvSave(SubCon model)
        {
            try
            {
                var record = ctx.SubCons.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && x.PONo == model.PONo);
                if (record == null) throw new Exception("PO not found. Error.");
                record.FPJNo = model.FPJNo;
                record.FPJDate = model.FPJDate;
                record.FPJGovNo = model.FPJGovNo;
                record.POStatus = "3";

                if (record.RecNo.Trim() == string.Empty || !record.RecNo.StartsWith("RRO")
                        || record.RecNo.Trim() == "RRO/XX/YYYYYY")
                {
                    if (record.RecDate.Value.Date == new DateTime(1900, 1, 1)) record.RecDate = DateTime.Now;
                    record.RecNo = GetNewDocumentNo("RRO", record.RecDate.Value);
                }
                
                var result = UpdatePO(record, GetRowSupplierPC(record.SupplierCode));
                if (result != "") throw new Exception(result); 
                
                //var subConView = ctx.SubConViews.FirstOrDefault(x => x.CompanyCode == CompanyCode
                //     && x.BranchCode == BranchCode && x.ProductType == ProductType && x.PONo == record.PONo);
                
                //return Json(new { Message = "", Record = subConView });

                var subConRcvView = ctx.SubConRcvViews.FirstOrDefault(x => x.CompanyCode == CompanyCode
                     && x.BranchCode == BranchCode && x.ProductType == ProductType && x.PONo == record.PONo);

                return Json(new { Message = "", Record = subConRcvView });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult RcvProcess(SubCon model)
        {
            try
            {
                if (model.ServiceAmt == 0) throw new Exception(
                    "Tidak dapat melakukan Proses Penerimaan karena nilai harga 0");

                var record = ctx.SubCons.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && x.PONo == model.PONo);
                if (record == null) throw new Exception("PO not found. Error.");

                record.POStatus = "5";
                var result = UpdatePO(record, GetRowSupplierPC(record.SupplierCode));
                if (result != "") throw new Exception(result);

                var subConView = ctx.SubConViews.FirstOrDefault(x => x.CompanyCode == CompanyCode
                     && x.BranchCode == BranchCode && x.ProductType == ProductType && x.PONo == record.PONo);

                return Json(new { Message = "", Record = subConView });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult RcvCancel(SubCon model)
        {
            try
            {
                var record = ctx.SubCons.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && x.PONo == model.PONo);
                if (record == null) throw new Exception("PO not found. Error.");

                record.POStatus = "4";
                var result = UpdatePO(record, GetRowSupplierPC(record.SupplierCode));
                if (result != "") throw new Exception(result);

                var subConView = ctx.SubConViews.FirstOrDefault(x => x.CompanyCode == CompanyCode
                     && x.BranchCode == BranchCode && x.ProductType == ProductType && x.PONo == record.PONo);

                return Json(new { Message = "", Record = subConView });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        public JsonResult LookUpSPKPesananPekerjaanLuar()
        {
            var ShowAll = Request["ShowAll"] ?? "1";
            var records = ctx.Database.SqlQuery<LookUpTrnServiceEstimation>("exec uspfn_SvTrnServiceSelectPesananPekerjaanLuar @p0, @p1, @p2, @p3",
                CompanyCode, BranchCode, ProductType, ShowAll == "0" ? true : false).AsQueryable();

            return Json(records.toKG());
        }

        public JsonResult SPKPesananPekerjaanLuar(string JobOrderNo)
        {
            var ShowAll = Request["ShowAll"] ?? "1";
            var records = ctx.Database.SqlQuery<LookUpTrnServiceEstimation>("exec uspfn_SvTrnServiceSelectPesananPekerjaanLuar @p0, @p1, @p2, @p3",
                CompanyCode, BranchCode, ProductType, ShowAll == "0" ? true : false).AsQueryable();
            var data = records.Where(a => a.JobOrderNo == JobOrderNo).FirstOrDefault();

            return Json(new { Success = data != null, data = data});
        }

        public JsonResult PopupateDescription(string ColorCode, string ForemanID)
        {
            var colorName = "";
            var foremanName = "";
            var recordColor = ctx.OmMstRefferences.Find(CompanyCode, OmMstRefferenceConstant.CLCD, ColorCode);
            if (recordColor != null)
                colorName = recordColor.RefferenceDesc1;

            recordColor = null;

            var recordEmploye = ctx.Employees.Find(CompanyCode, BranchCode, ForemanID);
            if (recordEmploye != null)
                foremanName = recordEmploye.EmployeeName;

            recordEmploye = null;

            return Json(new { ColorName = colorName, ForemanName = foremanName });
        }



        #endregion

        #region -- SQL Commands --
        private string InsertPO(SubCon record, SupplierProfitCenterView supplier, TrnService service)
        {
            var message = string.Empty;
            try
            {
                var result = 0;
                record.POStatus = "0";
                record.JobOrderNo = service.JobOrderNo;
                record.JobOrderDate = service.JobOrderDate.Value;
                record.JobType = service.JobType;
                record.BasicModel = service.BasicModel;
                record.TOPCode = supplier.TOPCode;
                record.TOPDays = supplier.TOPDays;
                record.DueDate = record.PODate.Value.AddDays(Convert.ToDouble(record.TOPDays));
                record.SupplierCode = supplier.SupplierCode;
                record.PODisc = supplier.DiscPct ?? 0;
                record.PPhPct = supplier.TaxPct;
                record.PPnPct = supplier.TaxPct;
                record.GrossAmt = 0;
                record.DiscAmt = record.GrossAmt * record.PODisc * Convert.ToDecimal(0.01);
                record.DppAmt = record.GrossAmt - record.DiscAmt;
                record.PphAmt = 0;
                record.PpnAmt = record.DppAmt * record.PPnPct * Convert.ToDecimal(0.01);
                record.ServiceAmt = record.DppAmt + record.PpnAmt;
                record.PrintSeq = 0;
                record.CreatedBy = CurrentUser.UserId;
                record.CreatedDate = DateTime.Now;
                record.LastupdateBy = CurrentUser.UserId;
                record.LastupdateDate = DateTime.Now;
                ctx.SubCons.Add(record);
                result = ctx.SaveChanges();

                if (result > 0)
                {
                    result += InsertPOTask(record);
                    result += UpdateSrvTask(record);
                    result += UpdateSubCon(record);
                }

                service = ctx.Services.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && x.JobOrderNo == record.JobOrderNo && x.ServiceType == "2");
                if (service.ServiceStatus == "0")
                    result = UpdateStatusSPK(record);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        private string UpdatePO(SubCon record, SupplierProfitCenterView supplier)
        {
            var message = string.Empty;
            try
            {
                var result = 0;
                record.TOPCode = supplier.TOPCode;
                record.TOPDays = supplier.TOPDays;
                record.DueDate = record.PODate.Value.AddDays(Convert.ToDouble(record.TOPDays));
                record.SupplierCode = supplier.SupplierCode;
                record.PODisc = supplier.DiscPct ?? 0;
                record.PPhPct = supplier.TaxPct;
                record.PPnPct = supplier.TaxPct;
                record.DiscAmt = record.GrossAmt * record.PODisc * Convert.ToDecimal(0.01);
                record.DppAmt = record.GrossAmt - record.DiscAmt;
                record.PpnAmt = record.DppAmt * record.PPnPct * Convert.ToDecimal(0.01);
                record.ServiceAmt = record.DppAmt + record.PpnAmt;
                record.LastupdateBy = CurrentUser.UserId;
                record.LastupdateDate = DateTime.Now;

                Helpers.ReplaceNullable(record);
                result = ctx.SaveChanges();

                // Action yg dilakukan Batal PO, status menjadi Cancel Draft Receiving maka update PONo di svTrnSrvTask menjadi empty.
                if ((record.POStatus == "1" || record.POStatus == "4") && result > 0)
                    result = POCancel(record);

                // Posting PO
                if (record.POStatus == "5" && result > 0)
                    result = PostingPOSubCon(record);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        private SupplierProfitCenterView GetRowSupplierPC(string supplierCode)
        {
            var suppliers = (from a in ctx.Suppliers
                             join b in ctx.SupplierProfitCenters
                                 on new { a.CompanyCode, BranchCode, ProfitCenterCode = ProfitCenter, a.SupplierCode }
                                 equals new { b.CompanyCode, b.BranchCode, b.ProfitCenterCode, b.SupplierCode } into _b
                             from b in _b.DefaultIfEmpty()
                             join c in ctx.LookUpDtls
                                 on new { a.CompanyCode, CodeID = "TOPC", LookUpValue = b.TOPCode }
                                 equals new { c.CompanyCode, c.CodeID, c.LookUpValue } into _c
                             from c in _c.DefaultIfEmpty()
                             join d in ctx.Taxes
                                 on new { a.CompanyCode, b.TaxCode }
                                 equals new { d.CompanyCode, d.TaxCode } into _d
                             from d in _d.DefaultIfEmpty()
                             where a.CompanyCode == CompanyCode && a.SupplierCode == supplierCode
                             select new SupplierProfitCenterView
                             {
                                 SupplierCode = a.SupplierCode,
                                 ProfitCenterCode = b.ProfitCenterCode,
                                 TOPCode = b.TOPCode,
                                 TOPDaysString = c.ParaValue,
                                 DiscPct = b.DiscPct,
                                 TaxPct = d.TaxPct
                             }).ToList();

            if (suppliers.Count == 0) throw new Exception("Error. Supplier tidak ditemukan.");

            for (int i = 0; i < suppliers.Count; i++)
            {
                suppliers[i].SeqNo = i + 1;
                suppliers[i].TOPDays = Convert.ToInt32(suppliers[i].TOPDaysString);
            }
            var supplier = suppliers.FirstOrDefault();
            return supplier;
        }

        private int InsertPOTask(SubCon subcon)
        {
            #region -- Query --
            var sql = string.Format(@"
declare @ServiceNo bigint

set @ServiceNo = (
select top 1 ServiceNo from svTrnService where 1 = 1 
 and CompanyCode = '{0}'
 and BranchCode = '{1}'
 and ProductType = '{2}'
 and JobOrderNo = '{3}'
)

select * into #t1 from(
select
 a.CompanyCode
,a.BranchCode
,a.ProductType
,'{4}' PONo
,a.OperationNo
,a.OperationHour
--,0 POPrice
,a.SubConPrice POPrice
,isnull(a.SubConPrice,0) SubConPrice
,1 status
,'{5}' CreatedBy
,'{6}' CreatedDate
,'{5}' LastUpdateBy
,'{6}' LastUpdateDate
from svTrnSrvTask a
where 1 = 1
 and a.CompanyCode = '{0}'
 and a.BranchCode = '{1}'
 and a.ProductType = '{2}'
 and a.ServiceNo = @ServiceNo
 and IsSubCon = 1
 and PONo = ''
)#t1

insert into svTrnPoSubConTask
select * from #t1
where OperationNo not in(
select OperationNo from svTrnPoSubConTask
where 1 = 1
 and CompanyCode = '{0}'
 and BranchCode = '{1}'
 and ProductType = '{2}'
 and PONo = '{4}'
)

drop table #t1",
               CompanyCode, BranchCode, ProductType, subcon.JobOrderNo, subcon.PONo, CurrentUser.UserId, DateTime.Now);
            #endregion

            return ctx.Database.ExecuteSqlCommand(sql);
        }

        private int UpdateSrvTask(SubCon subcon)
        {
            #region -- Query --
            var sql = string.Format(@"
declare @JobOrderNo varchar(20)
set @JobOrderNo = (
select top 1 JobOrderNo from svTrnPoSubCon
where 1 = 1
 and CompanyCode = '{0}'
 and BranchCode = '{1}'
 and ProductType = '{2}'
 and PONo = '{3}'
)

declare @ServiceNo bigint
set @ServiceNo = (
select top 1 ServiceNo from svTrnService
where 1 = 1
 and CompanyCode = '{0}'
 and BranchCode = '{1}'
 and ProductType = '{2}'
 and JobOrderNo = @JobOrderNo
)

select * into #t1 from(
select (row_number() over (order by OperationNo)) RecNo
,OperationNo
,SubConPrice
,LastUpdateBy
,LastUpdateDate
from svTrnPoSubConTask
where 1 = 1
 and CompanyCode = '{0}'
 and BranchCode = '{1}'
 and ProductType = '{2}'
 and PONo = '{3}'
)#t1

declare @nRow int, @maxRow int
set @nRow = 0
set @maxRow = (select max(RecNo) from #t1)

while @nRow < @maxRow
begin
 set @nRow = @nRow + 1
 update svTrnSrvTask set
  SubConPrice = (select SubConPrice from #t1 where RecNo = @nRow)
 ,PONo = '{3}'
 ,LastUpdateBy = (select LastUpdateBy from #t1 where RecNo = @nRow)
 ,LastUpdateDate = (select LastUpdateDate from #t1 where RecNo = @nRow)
 where 1 = 1
  and CompanyCode = '{0}'
  and BranchCode = '{1}'
  and ProductType = '{2}'
  and ServiceNo = @ServiceNo
  and OperationNo = (select OperationNo from #t1 where RecNo = @nRow)
end

drop table #t1
",
                CompanyCode, BranchCode, ProductType, subcon.PONo);
            #endregion

            return ctx.Database.ExecuteSqlCommand(sql);
        }

        private int UpdateSubCon(SubCon subcon)
        {
            #region -- Query --
            var sql = string.Format(@"
declare @Price numeric(18,0)
set @Price = (
select SUM(POPrice) from svTrnPoSubConTask
where 1 = 1
 and CompanyCode = '{0}'
 and BranchCode = '{1}'
 and ProductType = '{2}'
 and PONo = '{3}'
)

update svTrnPoSubCon
set GrossAmt = isnull(@Price,0),
    DiscAmt = isnull(@Price,0) * PODisc * 0.01,
    DppAmt = isnull(@Price,0) - (isnull(@Price,0) * PODisc * 0.01),
    PpnAmt = (isnull(@Price,0) - (isnull(@Price,0) * PODisc * 0.01)) * PphPct * 0.01,
    ServiceAmt = (isnull(@Price,0) - (isnull(@Price,0) * PODisc * 0.01)) + ((isnull(@Price,0) - (isnull(@Price,0) * PODisc * 0.01)) * PphPct * 0.01)
where 1 = 1
 and CompanyCode = '{0}'
 and BranchCode = '{1}'
 and ProductType = '{2}'
 and PONo = '{3}'",
                CompanyCode, BranchCode, ProductType, subcon.PONo);
            #endregion

            return ctx.Database.ExecuteSqlCommand(sql);
        }

        private int UpdateStatusSPK(SubCon subcon)
        {
            #region -- Query --
            var sql = string.Format(@"
update svTrnService
set 
    ServiceStatus = 1,
    LastUpdateBy = '{4}',
    LastUpdateDate = '{5}'
where
    CompanyCode = '{0}' and
    BranchCode = '{1}' and
    ProductType = '{2}' and
    JobOrderNo = '{3}'
",
                CompanyCode, BranchCode, ProductType, subcon.JobOrderNo, CurrentUser.UserId, DateTime.Now);
            #endregion

            return ctx.Database.ExecuteSqlCommand(sql);
        }

        private int POCancel(SubCon subcon)
        {
            #region -- Query --
            var sql = string.Format(@"
update svTrnSrvTask
set 
    PONo = '',
    LastUpdateBy = '{4}',
    LastUpdateDate = '{5}'
where 
    PONo = '{3}' and
    CompanyCode = '{0}' and
    BranchCode = '{1}' and
    ProductType = '{2}'
            ",
                CompanyCode, BranchCode, ProductType, subcon.PONo, CurrentUser.UserId, DateTime.Now);
            #endregion

            return ctx.Database.ExecuteSqlCommand(sql);
        }

        private int PostingPOSubCon(SubCon subcon)
        {
            return ctx.Database.ExecuteSqlCommand("uspfn_SvTrnPurchaseSubConPosting @p0, @p1, @p2, @p3, @p4",
                CompanyCode, BranchCode, ProductType, subcon.RecNo, CurrentUser.UserId);
        }

        private int UpdatePOTask(SubConTaskView task)
        {
            var sql = string.Format(@"
                        update svTrnPoSubConTask
                        set POPrice = '{5}', SubConPrice = '{5}'
                        where 1 = 1
                         and CompanyCode = '{0}'
                         and BranchCode = '{1}'
                         and ProductType = '{2}'
                         and PONo = '{3}'
                         and OperationNo = '{4}'
                        ", CompanyCode, BranchCode, ProductType, task.PONo,
                task.OperationNo, task.POPrice);
            return ctx.Database.ExecuteSqlCommand(sql);
        }
        #endregion
    }
}
