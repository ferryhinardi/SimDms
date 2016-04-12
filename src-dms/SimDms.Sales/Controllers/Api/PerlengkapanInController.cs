using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Sales.Models;
using SimDms.Common;

namespace SimDms.Sales.Controllers.Api
{
    public class PerlengkapanInController : BaseController
    {
        //
        // GET: /PerlengkapanIn/
        private string msg = "";

        public JsonResult getDetailGrid(string perlengkapanNo)
        {
            var data = ctx.omTrPurchasePerlengkapanIn.Find(CompanyCode, BranchCode, perlengkapanNo);

            var grid = ctx.omTrPurchasePerlengkapanInDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.PerlengkapanNo == perlengkapanNo)
               .Join(ctx.MstPerlengkapan, c => new { c.CompanyCode, c.BranchCode, c.PerlengkapanCode }, d => new { d.CompanyCode, d.BranchCode, d.PerlengkapanCode }, (c, d) => new { c, d })
                  .Select(x => new
                  {
                      PerlengkapanCode = x.c.PerlengkapanCode,
                      PerlengkapanName = x.d.PerlengkapanName,
                      Quantity = x.c.Quantity,
                      Remark = x.c.Remark
                  }).OrderBy(z => z.PerlengkapanCode);

            return Json(new { data = data, grid = grid });
        }

        public JsonResult ValidateSave(omTrPurchasePerlengkapanIn model, int options, string batchNo)
        {
            var valDate = DateTransValidation(model.PerlengkapanDate.Value,ref msg);
            if (!valDate) return Json(new { success = valDate, message = msg });

            switch (options)
            {
                case 0:
                    break;
                case 1:
                    var IsExistReffNo = ctx.omTrPurchasePerlengkapanIn.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.RefferenceNo == model.RefferenceNo);
                    if (IsExistReffNo != null)
                    {
                        msg = "No. Ref. sudah ada di dalam tabel Perlengkapan";
                        return Json(new { success = false, message = msg });
                    }
                    break;
            }

            bool isNew = false;
            bool result = false;
            var record = ctx.omTrPurchasePerlengkapanIn.Find(CompanyCode, BranchCode, model.PerlengkapanNo);

            if (record == null)
            {
                isNew = true;
                record = new omTrPurchasePerlengkapanIn()
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    PerlengkapanDate = model.PerlengkapanDate,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };
            }
            record.RefferenceNo = model.RefferenceNo;
            if (model.RefferenceNo != "")
                record.RefferenceDate = model.RefferenceDate;
            else
                record.RefferenceDate = Convert.ToDateTime("1900/01/01");

            record.PerlengkapanType = model.PerlengkapanType;
            record.SourceDoc = model.SourceDoc;
            record.Remark = model.Remark ?? string.Empty;
            record.Status = "0";
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;

            if (isNew == true)
            {
                record.PerlengkapanNo = GetNewDocumentNo("PIN", model.PerlengkapanDate.Value);
                ctx.omTrPurchasePerlengkapanIn.Add(record);
                result = ctx.SaveChanges() > 0;
                if (result == true && options == 1)
                {
                    result = SavePOUpload(record.PerlengkapanNo, record.RefferenceNo, batchNo);
                }
                msg = "Data Berhasil Disimpan";
            }
            else
            {
                result = ctx.SaveChanges() >= 0;
                msg = "Data Berhasil Disimpan";
            }

            return Json(new { success = result, message = msg, perlengkapanNo = record.PerlengkapanNo });
        }

        private bool SavePOUpload(string perlengkapanNo, string BPPNo, string batchNo)
        {
            bool result = false;

            //try
            //{
            OmUtlSACCSDtl1 utlDtl1 = ctx.OmUtlSACCSDtl1s.Find(CompanyCode, BranchCode, batchNo, BPPNo);
            if (utlDtl1 != null)
            {
                utlDtl1.Status = "1";
                utlDtl1.LastUpdateBy = CurrentUser.UserId;
                utlDtl1.LastUpdateDate = DateTime.Now;

                if (ctx.SaveChanges() > 0)
                {
                    var query = string.Format(@"SELECT a.PerlengkapanCode
                                FROM OmUtlSACCSDtl2 a
                                where CompanyCode = {0}
                                    and BranchCode = {1}
                                    and BPPNo = '{2}'  
                                    and not exists (
                                        select * 
                                        from omMstPerlengkapan
                                        where CompanyCode=a.CompanyCode and BranchCode=a.BranchCode
                                            and PerlengkapanCode=a.PerlengkapanCode", CompanyCode, BranchCode, BPPNo);

                    string o = ctx.Database.SqlQuery<string>(query).FirstOrDefault();

                    if (!string.IsNullOrEmpty(o))
                    {
                        msg ="Silahkan input dahulu kode perlengkapan: " + o + " di Master Perlengkapan";
                        result = false;
                    }

                    var dtOmUtlSACCSDtl2 = ctx.OmUtlSACCSDtl2s.Where(a=>a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.BPPNo == BPPNo);

                    if (dtOmUtlSACCSDtl2.Count() > 0)
                    {
                        foreach (var row in dtOmUtlSACCSDtl2)
                        {
                            bool isNew = false;
                            var oOmTrPurchasePerlengkapanInDetail = ctx.omTrPurchasePerlengkapanInDetail.Find(CompanyCode, BranchCode, perlengkapanNo, row.PerlengkapanCode);
                            if (oOmTrPurchasePerlengkapanInDetail == null)
                            {
                                isNew = true;
                                oOmTrPurchasePerlengkapanInDetail = new omTrPurchasePerlengkapanInDetail();
                                oOmTrPurchasePerlengkapanInDetail.CompanyCode = CompanyCode;
                                oOmTrPurchasePerlengkapanInDetail.BranchCode = BranchCode;
                                oOmTrPurchasePerlengkapanInDetail.PerlengkapanNo = perlengkapanNo;
                                oOmTrPurchasePerlengkapanInDetail.PerlengkapanCode = row.PerlengkapanCode;
                                oOmTrPurchasePerlengkapanInDetail.CreatedBy = CurrentUser.UserId;
                                oOmTrPurchasePerlengkapanInDetail.CreatedDate = DateTime.Now;
                            }
                            oOmTrPurchasePerlengkapanInDetail.Remark = "";
                            oOmTrPurchasePerlengkapanInDetail.Quantity = row.Quantity;
                            oOmTrPurchasePerlengkapanInDetail.LastUpdateBy = CurrentUser.UserId;
                            oOmTrPurchasePerlengkapanInDetail.LastUpdateDate = DateTime.Now;
                            if (isNew)
                            {
                                ctx.omTrPurchasePerlengkapanInDetail.Add(oOmTrPurchasePerlengkapanInDetail);
                                result = ctx.SaveChanges() > 0;
                            }
                            else
                            {
                                result = ctx.SaveChanges() >= 0;
                            }
                        }
                    }
                }
            }
            return result;
        }

        public JsonResult SaveDetail(omTrPurchasePerlengkapanInDetail model, string perlengkapanNo)
        {
            var Dtl = ctx.omTrPurchasePerlengkapanInDetail.Find(CompanyCode, BranchCode, perlengkapanNo, model.PerlengkapanCode);
            if (Dtl == null)
            {
                Dtl = new omTrPurchasePerlengkapanInDetail()
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    PerlengkapanNo = perlengkapanNo,
                    PerlengkapanCode = model.PerlengkapanCode,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };
                ctx.omTrPurchasePerlengkapanInDetail.Add(Dtl);
            }
            Dtl.Quantity = model.Quantity;
            Dtl.Remark = model.Remark ?? string.Empty;
            Dtl.LastUpdateBy = CurrentUser.UserId;
            Dtl.LastUpdateDate = DateTime.Now;

            var result = false;
            try
            {
                Helpers.ReplaceNullable(Dtl);
                ctx.SaveChanges();
                resetStatus(Dtl.PerlengkapanNo);
                result = true;
                msg = "Data detail berhasil disimpan!!!";
            }
            catch (Exception e)
            {
                msg = "Data detail gagal disimpan!!!";
            }

            return Json(new { success = result, message = msg});
        }

        public JsonResult Delete(omTrPurchasePerlengkapanIn model)
        {
            var preDelDtl = (from a in ctx.omTrPurchasePerlengkapanInDetail
                        join d in ctx.omTrPurchasePerlengkapanIn
                        on new { a.CompanyCode, a.BranchCode }
                        equals new { d.CompanyCode, d.BranchCode }
                        into e
                        from d in e.DefaultIfEmpty()
                        where a.CompanyCode == CompanyCode
                        && a.BranchCode == BranchCode
                        && a.PerlengkapanNo == model.PerlengkapanNo
                        select new
                        {
                            PerlengkapanCode = a.PerlengkapanCode,
                            Quantity = a.Quantity,
                            Year = d.PerlengkapanDate.Value.Year,
                            Month = d.PerlengkapanDate.Value.Month
                        }).Distinct().ToList();

            foreach(var dtl in preDelDtl)
            {
                var perlengkapan = ctx.OMTrInventQtyPerlengkapan.Find(CompanyCode, BranchCode, dtl.Year, dtl.Month, dtl.PerlengkapanCode);
                if (perlengkapan != null)
                {
                    perlengkapan.QuantityIn = perlengkapan.QuantityIn - dtl.Quantity;
                    perlengkapan.QuantityEnding = perlengkapan.QuantityBeginning + perlengkapan.QuantityIn - perlengkapan.QuantityOut;
                    perlengkapan.LastUpdateBy = CurrentUser.UserId;
                    perlengkapan.LastUpdateDate = DateTime.Now;
                    ctx.SaveChanges();
                }
            }

            var query = string.Format(@"DELETE FROM omTrPurchasePerlengkapanInDetail
                                    WHERE CompanyCode = {0}
                                    AND BranchCode = {1}
                                    AND PerlengkapanNo = '{2}'",CompanyCode,BranchCode,model.PerlengkapanNo);

            var result = ctx.Database.ExecuteSqlCommand(query) > -1;

            if(result)
            {
                var Hdr = ctx.omTrPurchasePerlengkapanIn.Find(CompanyCode, BranchCode, model.PerlengkapanNo);
                Hdr.Status = "3";
                ctx.SaveChanges();

                return Json(new { success = true, message = "Data Berhasil Dihapus!!!", status = getStringStatus(Hdr.Status) }); 
            }
            else { return Json(new { success = false, message = "Data Gagal Dihapus!!!"}); }
            
        }

        public JsonResult DeleteDetail(omTrPurchasePerlengkapanInDetail model, string perlengkapanNo)
        {
            var Dtl = ctx.omTrPurchasePerlengkapanInDetail.Find(CompanyCode,BranchCode, perlengkapanNo, model.PerlengkapanCode);
            if(Dtl != null)
            {
                ctx.omTrPurchasePerlengkapanInDetail.Remove(Dtl);
                resetStatus(perlengkapanNo);
                return Json(new { success = true, message = "Data Detail Berhasil Dihapus!!!"});
            }
            return Json(new { success = false, message = "Data Detail Galal Dihapus!!!"});
        }

        public JsonResult Approve(omTrPurchasePerlengkapanIn model)
        {
            var record = ctx.omTrPurchasePerlengkapanIn.Find(CompanyCode, BranchCode, model.PerlengkapanNo);

            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;
            record.Status = "2";

            var PerlengkapanDetail = ctx.omTrPurchasePerlengkapanInDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.PerlengkapanNo == model.PerlengkapanNo).ToList();

            bool success = ctx.SaveChanges() >= 0;
            if (success)
            {

                foreach (var perlDet in PerlengkapanDetail)
                {
                    if (ctx.OMTrInventQtyPerlengkapan.Find(record.CompanyCode, record.BranchCode, record.PerlengkapanDate.Value.Year, record.PerlengkapanDate.Value.Month, perlDet.PerlengkapanCode) != null)
                    {
                        var perlengkapanOne = ctx.OMTrInventQtyPerlengkapan.Find(record.CompanyCode, record.BranchCode, record.PerlengkapanDate.Value.Year, record.PerlengkapanDate.Value.Month, perlDet.PerlengkapanCode);
                        perlengkapanOne.QuantityIn =
                            QtyPerlengkapan(true, record.CompanyCode, record.BranchCode, perlDet.PerlengkapanCode, record.PerlengkapanDate.Value.Month, record.PerlengkapanDate.Value.Year)
                            + QtyPerlengkapan(false, record.CompanyCode, record.BranchCode, perlDet.PerlengkapanCode, record.PerlengkapanDate.Value.Month, record.PerlengkapanDate.Value.Year);
                        perlengkapanOne.QuantityEnding = perlengkapanOne.QuantityBeginning + perlengkapanOne.QuantityIn - perlengkapanOne.QuantityOut;
                        perlengkapanOne.LastUpdateBy = CurrentUser.UserId;
                        perlengkapanOne.LastUpdateDate = DateTime.Now;
                        
                    }
                    else
                    {
                        var perlengkapanTwo = ctx.OMTrInventQtyPerlengkapan.Find(record.CompanyCode, record.BranchCode, record.PerlengkapanDate.Value.Year, record.PerlengkapanDate.Value.Month, perlDet.PerlengkapanCode);
                        if (perlengkapanTwo == null)
                        {
                            perlengkapanTwo = new OMTrInventQtyPerlengkapan();
                            perlengkapanTwo.CompanyCode = record.CompanyCode;
                            perlengkapanTwo.BranchCode = record.BranchCode;
                            perlengkapanTwo.Year = record.PerlengkapanDate.Value.Year;
                            perlengkapanTwo.Month = record.PerlengkapanDate.Value.Month;
                            perlengkapanTwo.PerlengkapanCode = perlDet.PerlengkapanCode;
                            perlengkapanTwo.QuantityBeginning = 0;
                            perlengkapanTwo.QuantityIn = perlDet.Quantity;
                            perlengkapanTwo.QuantityOut = 0;
                            perlengkapanTwo.QuantityEnding = perlDet.Quantity;//OmTrInventQtyPerlengkapanBLL.QtyEndingPerlengkapan(record.CompanyCode, record.BranchCode, perlDet.PerlengkapanCode, record.PerlengkapanDate) + perlDet.Quantity;
                            perlengkapanTwo.Remark = perlDet.Remark ?? string.Empty;
                            perlengkapanTwo.CreatedBy = CurrentUser.UserId;
                            perlengkapanTwo.CreatedDate = DateTime.Now;
                        }
                        Helpers.ReplaceNullable(perlengkapanTwo);
                        ctx.OMTrInventQtyPerlengkapan.Add(perlengkapanTwo);
                    }
                    if (ctx.SaveChanges() < 0)
                    {
                        return Json(new { success = false });
                    }
                    else
                    {
                        success = true;
                    }
                }
            }

            if (success)
            {
                msg = string.Format(ctx.SysMsgs.Find("5038").MessageCaption, "Approve Perlengkapan In", "");
                return Json(new { success = success, message = msg, status = getStringStatus(record.Status) });
            }
            else
            {
                msg = string.Format(ctx.SysMsgs.Find("5039").MessageCaption, "Approve Perlengkapan In", "");
                return Json(new { success = success, message = msg });
            }
        }

        public JsonResult prePrint(omTrPurchasePerlengkapanIn model)
        {
            var result = false;
            var Hdr = ctx.omTrPurchasePerlengkapanIn.Find(CompanyCode, BranchCode, model.PerlengkapanNo);
            var Dtl = ctx.omTrPurchasePerlengkapanInDetail.Where(a=>a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.PerlengkapanNo == model.PerlengkapanNo);
            if (Dtl.Count() > 0)
            {

                if (Hdr.Status == "0" || Hdr.Status.Trim() == "")
                {
                    Hdr.Status = "1";
                }

                Hdr.LastUpdateBy = CurrentUser.UserId;
                Hdr.LastUpdateDate = DateTime.Now;

                result = ctx.SaveChanges() >= 0;

                var Status = getStringStatus(Hdr.Status);

                return Json(new { success = result, Status = Status, stat = Hdr.Status });
            }
            else
            {
                msg = ctx.SysMsgs.Find("5047").MessageCaption;
                return Json(new { success = result, message = msg  });
            }

        }

        private void resetStatus(string PerlengkapanNo)
        {
            var record = ctx.omTrPurchasePerlengkapanIn.Find(CompanyCode, BranchCode, PerlengkapanNo);

            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;
            record.Status = "0";

            ctx.SaveChanges();
        }

        private decimal QtyPerlengkapan(bool IsIn, string companyCode, string branchCode, string perlengkapanCode, int month, int year)
        {
            decimal data;
            if(IsIn)
            {
            data = (from a in ctx.omTrPurchasePerlengkapanInDetail 
                       join b in ctx.omTrPurchasePerlengkapanIn
                       on new {a.CompanyCode, a.BranchCode, a.PerlengkapanNo} 
                       equals new {b.CompanyCode,b.BranchCode, b.PerlengkapanNo} into _b
                       from b in _b.DefaultIfEmpty()
                       where a.CompanyCode == companyCode
                       && a.BranchCode == branchCode
                       && a.PerlengkapanCode == perlengkapanCode
                       && b.PerlengkapanDate.Value.Month == month
                       && b.PerlengkapanDate.Value.Year == year
                       select new {
                       Quantity = a.Quantity
                       }).Sum(a => (decimal)a.Quantity);
            }
            else
            {
                var query = string.Format(@"
                    SELECT isnull(SUM(a.Quantity),0) as Quantity
                    FROM omTrPurchasePerlengkapanAdjustmentDetail a
                    LEFT JOIN omTrPurchasePerlengkapanAdjustment b
                    ON a.CompanyCode = b.CompanyCode
                    AND a.BranchCode = b.BranchCode
                    AND a.AdjustmentNo = b.AdjustmentNo           
                    WHERE a.CompanyCode = {0}
                    AND a.BranchCode = {1}           
                    AND a.PerlengkapanCode = '{2}'
                    AND MONTH(b.AdjustmentDate) = {3}
                    AND YEAR(b.AdjustmentDate) = {4}",companyCode,branchCode,perlengkapanCode,month,year);

                data = ctx.Database.SqlQuery<decimal>(query).FirstOrDefault();

                //data = (from a in ctx.omTrPurchasePerlengkapanAdjustmentDetail
                //        join b in ctx.omTrPurchasePerlengkapanAdjustment
                //        on new { a.CompanyCode, a.BranchCode, a.AdjustmentNo }
                //        equals new { b.CompanyCode, b.BranchCode, b.AdjustmentNo } into _b
                //        from b in _b.DefaultIfEmpty()
                //        where a.CompanyCode == companyCode
                //        && a.BranchCode == branchCode
                //        && a.PerlengkapanCode == perlengkapanCode
                //        && b.AdjustmentDate.Value.Month == month
                //        && b.AdjustmentDate.Value.Year == year
                //        select new
                //        {
                //            Quantity = a.Quantity
                //        }).Sum(a => (decimal)a.Quantity);
            }

            return data;
        }
    }
}
