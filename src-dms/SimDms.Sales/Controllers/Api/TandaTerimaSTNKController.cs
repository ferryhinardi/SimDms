using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;
using SimDms.Common.Models;

namespace SimDms.Sales.Controllers.Api
{
    public class TandaTerimaSTNKController : BaseController
    {
        private const string TTS = "TTS"; // Permohonan Faktur Polis

        private string getStringStatus(string status)
        {
            var Status = status == "0" ? "OPEN"
                           : status == "1" ? "PRINTED"
                           : status == "2" ? "APPROVED"
                           : status == "3" ? "CANCELED"
                           : status == "9" ? "FINISHED" : "";
            return Status;
        }

        public JsonResult DetailSalesSTNK(string DocNo)
        {
            var gridDetail = ctx.omTrSalesSTNKDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.DocNo == DocNo).ToList()
                .Select(m => new SalesSTNKDetailView
                {
                    ChassisCode = m.ChassisCode,
                    ChassisNo = m.ChassisNo,
                    CustomerCode = m.CustomerCode,
                    CustomerName = ctx.GnMstCustomer.FirstOrDefault(e => e.CompanyCode == CompanyCode && e.CustomerCode == m.CustomerCode).CustomerName,
                    EngineCode = m.EngineCode,
                    EngineNo = m.EngineNo,
                    SalesModelCode = m.SalesModelCode,
                    ColourCode = m.ColourCode,
                    FakturPolisiNo = m.FakturPolisiNo,
                    BPKBNo = m.BPKBNo,
                    PoliceRegistrationNo = m.PoliceRegistrationNo,
                    PoliceRegistrationDate = m.PoliceRegistrationDate,
                    Remark = m.Remark
                });
            return Json(new { success = true, grid = gridDetail });
        }

        public JsonResult prePrint(omTrSalesSTNK model)
        {
            var msg = "";
            var Hdr = ctx.omTrSalesSTNK.Find(CompanyCode, BranchCode, model.DocNo);
            var Dtl1 = ctx.omTrSalesSTNKDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.DocNo == model.DocNo);
            if (Dtl1.Count() < 1)
            {
                msg = "Dokumen tidak dapat dicetak karena tidak memiliki data detail";
                return Json(new { success = false, message = msg });
            }

            if (Hdr.Status == "0" || Hdr.Status.Trim() == "")
            {
                Hdr.Status = "1";
            }

            Hdr.LastUpdateBy = CurrentUser.UserId;
            Hdr.LastUpdateDate = DateTime.Now;

            var result = ctx.SaveChanges() >= 0;

            return Json(new { success = result, Status = getStringStatus(Hdr.Status), stat = Hdr.Status });
        }

        public JsonResult Approve(omTrSalesSTNK model)
        {
            var msg = "";
            var record = ctx.omTrSalesSTNK.Find(CompanyCode, BranchCode, model.DocNo);

            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;
            record.Status = "2";

            var result = ctx.SaveChanges() >= 0;

            return Json(new { success = result, message = msg, status = getStringStatus(record.Status), Result = record.Status });

        }

        public JsonResult Save(omTrSalesSTNK model)
        {
            string msg = "";
            var record = ctx.omTrSalesSTNK.Find(CompanyCode, BranchCode, model.DocNo);

            if (record == null)
            {
                record = new omTrSalesSTNK
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    DocNo = GetNewDocumentNo(TTS, model.DocDate.Value),
                    DocDate = model.DocDate,
                    BPKBOutType = model.BPKBOutType,
                    BPKBOutBy = model.BPKBOutBy,
                    Remark = model.Remark == null ? "" : model.Remark,
                    Status = "0",
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = ctx.CurrentTime,
                    isLocked = false,
                    LockingBy = "",
                    LockingDate = Convert.ToDateTime("1900-01-01"),
                };

                ctx.omTrSalesSTNK.Add(record);
            }
            else
            {
                record.Remark = model.Remark == null ? "" : model.Remark;
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = DateTime.Now;
            }


            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, message = msg, data = record, status = getStringStatus(record.Status) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult SaveDetail(omTrSalesSTNK model, omTrSalesSTNKDetail detailModel)
        {
            var record = ctx.omTrSalesSTNKDetail.Find(CompanyCode, BranchCode, model.DocNo, detailModel.ChassisCode, detailModel.ChassisNo);

            if (record == null)
            {
                record = new omTrSalesSTNKDetail
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    DocNo = model.DocNo,
                    ChassisCode = detailModel.ChassisCode,
                    ChassisNo = detailModel.ChassisNo,
                    EngineCode = detailModel.EngineCode,
                    EngineNo = detailModel.EngineNo,
                    SalesModelCode = detailModel.SalesModelCode,
                    ColourCode = detailModel.ColourCode,
                    PoliceRegistrationNo = detailModel.PoliceRegistrationNo,
                    PoliceRegistrationDate = detailModel.PoliceRegistrationDate,
                    FakturPolisiNo = detailModel.FakturPolisiNo,
                    BPKBNo = detailModel.BPKBNo,
                    CustomerCode = detailModel.CustomerCode,
                    Remark = detailModel.Remark == null ? "" : detailModel.Remark,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = ctx.CurrentTime,
                };

                ctx.omTrSalesSTNKDetail.Add(record);
            }
            else
            {
                record.Remark = model.Remark;
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = DateTime.Now;

                //ctx.omTrSalesReqDetail.Attach(record);
            }

            try
            {
                ctx.SaveChanges();
                var gridDetail = ctx.omTrSalesSTNKDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.DocNo == model.DocNo).ToList()
                .Select(m => new SalesSTNKDetailView
                {
                    ChassisCode = m.ChassisCode,
                    ChassisNo = m.ChassisNo,
                    CustomerCode = m.CustomerCode,
                    CustomerName = ctx.GnMstCustomer.FirstOrDefault(e => e.CompanyCode == CompanyCode && e.CustomerCode == m.CustomerCode).CustomerName,
                    EngineCode = m.EngineCode,
                    EngineNo = m.EngineNo,
                    SalesModelCode = m.SalesModelCode,
                    ColourCode = m.ColourCode,
                    FakturPolisiNo = m.FakturPolisiNo,
                    BPKBNo = m.BPKBNo,
                    PoliceRegistrationNo = m.PoliceRegistrationNo,
                    PoliceRegistrationDate = m.PoliceRegistrationDate,
                    Remark = m.Remark
                });
                return Json(new { success = true, grid = gridDetail });

                //return Json(new { success = true, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Delete(omTrSalesSTNK model)
        {
            var record = ctx.omTrSalesSTNK.Find(CompanyCode, BranchCode, model.DocNo);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.Database.ExecuteSqlCommand(@"UPDATE omTrSalesSTNK SET Status = 3
                                                WHERE CompanyCode='" + CompanyCode + "' and BranchCode='" + BranchCode +
                                                "' and DocNo='" + model.DocNo + "'");
                ctx.Database.ExecuteSqlCommand(@"DELETE omTrSalesSTNKDetail 
                                                WHERE CompanyCode='" + CompanyCode + "' and BranchCode='" + BranchCode +
                                                "' and DocNo='" + model.DocNo + "'");
            }

            try
            {
                ctx.SaveChanges();

                return Json(new { success = true, data = record, Status = "Canceled" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult DeleteDetail(omTrSalesSTNK model, omTrSalesSTNKDetail detailModel)
        {
            var record = ctx.omTrSalesSTNKDetail.Find(CompanyCode, BranchCode, model.DocNo, detailModel.ChassisCode, detailModel.ChassisNo);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.omTrSalesSTNKDetail.Remove(record);
            }

            try
            {
                ctx.SaveChanges();

                return Json(new { success = true, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
