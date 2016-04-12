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
    public class TandaTerimaBPKBController : BaseController
    {
        private const string TTB = "TTB"; // Permohonan Faktur Polis

        private string getStringStatus(string status)
        {
            var Status = status == "0" ? "OPEN"
                           : status == "1" ? "PRINTED"
                           : status == "2" ? "APPROVED"
                           : status == "3" ? "CANCELED"
                           : status == "9" ? "FINISHED" : "";
            return Status;
        }

        public JsonResult DetailSalesBPKB(string DocNo)
        {
            var gridDetail = ctx.omTrSalesBPKBDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.DocNo == DocNo).ToList()
                .Select(m => new SalesBPKBDetailView
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

        public JsonResult prePrint(omTrSalesBPKB model)
        {
            var msg = "";
            var Hdr = ctx.omTrSalesBPKB.Find(CompanyCode, BranchCode, model.DocNo);
            var Dtl1 = ctx.omTrSalesBPKBDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.DocNo == model.DocNo);
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

        public JsonResult Approve(omTrSalesBPKB model)
        {
            var msg = "";
            var record = ctx.omTrSalesBPKB.Find(CompanyCode, BranchCode, model.DocNo);

            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;
            record.Status = "2";

            var result = ctx.SaveChanges() >= 0;

            return Json(new { success = result, message = msg, status = getStringStatus(record.Status), Result = record.Status });

        }

        public JsonResult Save(omTrSalesBPKB model)
        {
            string msg = "";
            var record = ctx.omTrSalesBPKB.Find(CompanyCode, BranchCode, model.DocNo);

            if (record == null)
            {
                record = new omTrSalesBPKB
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    DocNo = GetNewDocumentNo(TTB, model.DocDate.Value),
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

                ctx.omTrSalesBPKB.Add(record);
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

        public JsonResult SaveDetail(omTrSalesBPKB model, omTrSalesBPKBDetail detailModel)
        {
            var record = ctx.omTrSalesBPKBDetail.Find(CompanyCode, BranchCode, model.DocNo, detailModel.ChassisCode, detailModel.ChassisNo);

            if (record == null)
            {
                record = new omTrSalesBPKBDetail
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

                ctx.omTrSalesBPKBDetail.Add(record);
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
                var gridDetail = ctx.omTrSalesBPKBDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.DocNo == model.DocNo).ToList()
                .Select(m => new SalesBPKBDetailView
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

        public JsonResult Delete(omTrSalesBPKB model)
        {
            var record = ctx.omTrSalesBPKB.Find(CompanyCode, BranchCode, model.DocNo);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.Database.ExecuteSqlCommand(@"UPDATE omTrSalesBPKB SET Status = 3
                                                WHERE CompanyCode='" + CompanyCode + "' and BranchCode='" + BranchCode +
                                                "' and DocNo='" + model.DocNo + "'");
                ctx.Database.ExecuteSqlCommand(@"DELETE omTrSalesBPKBDetail 
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

        public JsonResult DeleteDetail(omTrSalesBPKB model, omTrSalesBPKBDetail detailModel)
        {
            var record = ctx.omTrSalesBPKBDetail.Find(CompanyCode, BranchCode, model.DocNo, detailModel.ChassisCode, detailModel.ChassisNo);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.omTrSalesBPKBDetail.Remove(record);
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
