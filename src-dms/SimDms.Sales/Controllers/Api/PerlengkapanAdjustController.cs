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
    public class PerlengkapanAdjustController : BaseController
    {
        private const string PAD = "PAD"; // Perlengkapan Adjustment

        public ActionResult Index()
        {
            return View();
        }

        private string getStringStatus(string status)
        {
            var Status = status == "0" ? "OPEN" : status == "1" ? "PRINTED"
                           : status == "2" ? "APPROVED"
                           : status == "3" ? "CANCELED"
                           : status == "9" ? "FINISHED" : "";
            return Status;
        }

        public JsonResult DetailAdjustment(string AdjustmentNo)
        {
            var gridDetail = ctx.omTrPurchasePerlengkapanAdjustmentDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.AdjustmentNo == AdjustmentNo).ToList()
                .Select(m => new PerlengkapanAdjustmentDetailView
                {
                    PerlengkapanCode = m.PerlengkapanCode,
                    PerlengkapanName = ctx.MstPerlengkapan.FirstOrDefault(e => e.CompanyCode == CompanyCode && e.PerlengkapanCode == m.PerlengkapanCode).PerlengkapanName,
                    Quantity = m.Quantity,
                    Remark = m.Remark
                });
            return Json(new { success = true, grid = gridDetail });
        }

        public JsonResult prePrint(omTrPurchasePerlengkapanAdjustment model)
        {
            var msg = "";
            var Hdr = ctx.omTrPurchasePerlengkapanAdjustment.Find(CompanyCode, BranchCode, model.AdjustmentNo);
            var Dtl1 = ctx.omTrPurchasePerlengkapanAdjustmentDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.AdjustmentNo == model.AdjustmentNo);
            if (Dtl1.Count() < 1 )
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

        public JsonResult Approve(omTrPurchasePerlengkapanAdjustment model)
        {
            var msg = "";
            var record = ctx.omTrPurchasePerlengkapanAdjustment.Find(CompanyCode, BranchCode, model.AdjustmentNo);

            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;
            record.Status = "2";

            //bool success = ctx.SaveChanges() >= 0;
            var success = InsertToTrInventQtyperlengkapan(model);
            if (success)
            {
                msg = string.Format(ctx.SysMsgs.Find("5038").MessageCaption, "Approve Perlengkapan Adjustment", "");
                return Json(new { success = success, message = msg, status = getStringStatus(record.Status), Result = record.Status });
            }
            else
            {
                msg = string.Format(ctx.SysMsgs.Find("5039").MessageCaption, "Approve Perlengkapan Adjustment", "");
                return Json(new { success = success, message = msg });
            }
        }

        public bool InsertToTrInventQtyperlengkapan(omTrPurchasePerlengkapanAdjustment model)
        {
            bool result = false;
            var Year = model.AdjustmentDate.Value.Year;
            var Month = model.AdjustmentDate.Value.Month;

            var record = ctx.omTrPurchasePerlengkapanAdjustmentDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.AdjustmentNo == model.AdjustmentNo).ToList();
            if (record != null)
            {
                foreach (omTrPurchasePerlengkapanAdjustmentDetail recDtl in record)
                {
                    var perlengkapan = ctx.OMTrInventQtyPerlengkapan.Find(CompanyCode, BranchCode, Year, Month, recDtl.PerlengkapanCode);
                    if(perlengkapan == null)
                    {
                        perlengkapan = new OMTrInventQtyPerlengkapan
                        {
                            CompanyCode = CompanyCode,
                            BranchCode = BranchCode,
                            Year = Year,
                            Month = Month,
                            PerlengkapanCode = recDtl.PerlengkapanCode,
                            QuantityBeginning  = 0,
                            QuantityIn = recDtl.Quantity,
                            QuantityOut = 0,
                            QuantityEnding = QtyEndingPerlengkapan(recDtl.PerlengkapanCode, model.AdjustmentDate) + recDtl.Quantity,
                            Remark = recDtl.Remark,
                            CreatedBy = CurrentUser.UserId,
                            CreatedDate = DateTime.Now
                        };
                        ctx.OMTrInventQtyPerlengkapan.Add(perlengkapan);
                    }
                    else
                    {
                        var PerlengkapanAdjut = QtyPerlengkapanAdjut(recDtl.PerlengkapanCode, Month, Year);
                        var PerlengkapanIn = QtyPerlengkapanIn(recDtl.PerlengkapanCode, Month, Year);

                        perlengkapan.QuantityIn = PerlengkapanAdjut + PerlengkapanIn;
                        perlengkapan.QuantityEnding = perlengkapan.QuantityBeginning + perlengkapan.QuantityIn - perlengkapan.QuantityOut;
                        perlengkapan.LastUpdateBy = CurrentUser.UserId;
                        perlengkapan.LastUpdateDate = DateTime.Now;

                        //ctx.OMTrInventQtyPerlengkapan.Attach(perlengkapan);
                    }
                }
                try
                {
                    ctx.SaveChanges();
                    return result = true;
                }
                catch (Exception ex)
                {
                    return result = false;
                }
            }
            return result;
        }

        public decimal QtyPerlengkapanIn(string PerlengkapanCode, decimal month, decimal year)
        {
            var query = string.Format(@"SELECT isnull(SUM(a.Quantity),0) as Quantity
            FROM omTrPurchasePerlengkapanInDetail a
            LEFT JOIN omTrPurchasePerlengkapanIn b
            ON a.CompanyCode = b.CompanyCode
            AND a.BranchCode = b.BranchCode
            AND a.PerlengkapanNo = b.PerlengkapanNo
            WHERE a.CompanyCode = '{0}'
            AND a.BranchCode = '{1}'
            AND a.PerlengkapanCode = '{2}'
            AND MONTH(b.PerlengkapanDate) = '{3}'
            AND YEAR(b.PerlengkapanDate) = '{4}'
            ", CompanyCode, BranchCode, PerlengkapanCode, month, year);

            var sqlstr = ctx.Database.SqlQuery<decimal>(query).FirstOrDefault();
            return sqlstr;
        }

        public decimal QtyPerlengkapanAdjut(string PerlengkapanCode, decimal month, decimal year)
        {
            var query = string.Format(@"SELECT isnull(SUM(a.Quantity),0) as Quantity
            FROM omTrPurchasePerlengkapanAdjustmentDetail a
            LEFT JOIN omTrPurchasePerlengkapanAdjustment b
            ON a.CompanyCode = b.CompanyCode
            AND a.BranchCode = b.BranchCode
            AND a.AdjustmentNo = b.AdjustmentNo           
            WHERE a.CompanyCode = '{0}'
            AND a.BranchCode = '{1}'          
            AND a.PerlengkapanCode = '{2}'
            AND MONTH(b.AdjustmentDate) = '{3}'
            AND YEAR(b.AdjustmentDate) = '{4}'
            ", CompanyCode, BranchCode, PerlengkapanCode, month, year);

            var sqlstr = ctx.Database.SqlQuery<decimal>(query).FirstOrDefault();
            return sqlstr;
        }

        public decimal QtyEndingPerlengkapan(string PerlengkapanCode, DateTime? AdjustmentDate)
        {
            var query = string.Format(@"SELECT a.QuantityEnding
            FROM omTrInventQtyPerlengkapan a
            WHERE a.CompanyCode = '{0}'
            AND a.BranchCode = '{1}'
            AND a.PerlengkapanCode = '{2}'
            AND convert(datetime,'01-' + Convert(varchar,a.Month) +'-'+Convert(varchar,a.Year)) < '{3}'
            AND convert(datetime,'01-' + Convert(varchar,a.Month) +'-'+Convert(varchar,a.Year)) = 
            (select max(convert(datetime,'01-' + Convert(varchar,Month) +'-'+Convert(varchar,Year))) 
            from omTrInventQtyPerlengkapan
            WHERE CompanyCode = '{0}'
            AND BranchCode = '{1}'
            AND PerlengkapanCode = '{2}')
            ", CompanyCode, BranchCode, PerlengkapanCode, AdjustmentDate);

            var sqlstr = ctx.Database.SqlQuery<decimal>(query).FirstOrDefault();
            return sqlstr;
        }

        public JsonResult Save(omTrPurchasePerlengkapanAdjustment model)
        {
            string msg = "";
            var record = ctx.omTrPurchasePerlengkapanAdjustment.Find(CompanyCode, BranchCode, model.AdjustmentNo);

            if (record == null)
            {
                record = new omTrPurchasePerlengkapanAdjustment
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    AdjustmentNo = GetNewDocumentNo(PAD, model.AdjustmentDate.Value),
                    AdjustmentDate = model.AdjustmentDate,
                    RefferenceNo = model.RefferenceNo,
                    RefferenceDate = model.RefferenceDate,
                    Remark = model.Remark == null ? "" : model.Remark,
                    Status = "0",
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = ctx.CurrentTime,
                    isLocked = false,
                    LockingBy = "",
                    LockingDate = Convert.ToDateTime("1900-01-01")
                };

                ctx.omTrPurchasePerlengkapanAdjustment.Add(record);
            }
            else
            {
                ctx.omTrPurchasePerlengkapanAdjustment.Attach(record);
            }

            record.RefferenceDate = model.RefferenceDate;
            record.Remark = model.Remark == null ? "" : model.Remark;
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;
            record.isLocked = false;
            record.LockingBy = "";
            record.LockingDate = Convert.ToDateTime("1900-01-01");

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

        public JsonResult SaveDetail(omTrPurchasePerlengkapanAdjustment model, omTrPurchasePerlengkapanAdjustmentDetail detailModel)
        {
            string msg = "";
            var record = ctx.omTrPurchasePerlengkapanAdjustmentDetail.Find(CompanyCode, BranchCode, model.AdjustmentNo, detailModel.PerlengkapanCode);

            if (record == null)
            {
                record = new omTrPurchasePerlengkapanAdjustmentDetail
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    AdjustmentNo = model.AdjustmentNo,
                    PerlengkapanCode = detailModel.PerlengkapanCode,
                    Quantity = detailModel.Quantity == null ? 0 : detailModel.Quantity,
                    Remark = detailModel.Remark == null ? "" : detailModel.Remark,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = ctx.CurrentTime,
                };

                ctx.omTrPurchasePerlengkapanAdjustmentDetail.Add(record);
            }
            else
            {
                record.Quantity = detailModel.Quantity;
                record.Remark = detailModel.Remark == null ? "" : detailModel.Remark;
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = DateTime.Now;
                ctx.omTrPurchasePerlengkapanAdjustmentDetail.Attach(record);
            }


            try
            {
                ctx.SaveChanges();
                var records = ctx.omTrPurchasePerlengkapanAdjustmentDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.AdjustmentNo == model.AdjustmentNo).ToList()
               .Select(m => new PerlengkapanAdjustmentDetailView
               {
                   PerlengkapanCode = m.PerlengkapanCode,
                   PerlengkapanName = ctx.MstPerlengkapan.FirstOrDefault(e => e.CompanyCode == CompanyCode && e.PerlengkapanCode == m.PerlengkapanCode).PerlengkapanName,
                   Quantity = m.Quantity,
                   Remark = m.Remark
               });
                return Json(new { success = true, message = msg, data = record, result = records });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Delete(omTrPurchasePerlengkapanAdjustment model)
        {
            var record = ctx.omTrPurchasePerlengkapanAdjustment.Find(CompanyCode, BranchCode, model.AdjustmentNo);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.Database.ExecuteSqlCommand(@"UPDATE omTrPurchasePerlengkapanAdjustment SET Status = 3
                                                WHERE CompanyCode='" + CompanyCode + "' and BranchCode='" + BranchCode +
                                                "' and AdjustmentNo='" + model.AdjustmentNo + "'");
                ctx.Database.ExecuteSqlCommand(@"DELETE omTrPurchasePerlengkapanAdjustmentDetail 
                                                WHERE CompanyCode='" + CompanyCode + "' and BranchCode='" + BranchCode +
                                                "' and AdjustmentNo='" + model.AdjustmentNo + "'");
            }
            try
            {
                ctx.SaveChanges();
                var data = ctx.omTrPurchasePerlengkapanAdjustment.Find(CompanyCode, BranchCode, model.AdjustmentNo);
                return Json(new { success = true, data = record, Status = getStringStatus(data.Status) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult DeleteDetail(omTrPurchasePerlengkapanAdjustment model, omTrPurchasePerlengkapanAdjustmentDetail detailModel)
        {
            var record = ctx.omTrPurchasePerlengkapanAdjustmentDetail.Find(CompanyCode, BranchCode, model.AdjustmentNo, detailModel.PerlengkapanCode);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.omTrPurchasePerlengkapanAdjustmentDetail.Remove(record);
            }

            try
            {
                ctx.SaveChanges();
                var records = ctx.omTrPurchasePerlengkapanAdjustmentDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.AdjustmentNo == model.AdjustmentNo).ToList()
               .Select(m => new PerlengkapanAdjustmentDetailView
               {
                   PerlengkapanCode = m.PerlengkapanCode,
                   PerlengkapanName = ctx.MstPerlengkapan.FirstOrDefault(e => e.CompanyCode == CompanyCode && e.PerlengkapanCode == m.PerlengkapanCode).PerlengkapanName,
                   Quantity = m.Quantity,
                   Remark = m.Remark
               });
                return Json(new { success = true, data = record, result = records });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
