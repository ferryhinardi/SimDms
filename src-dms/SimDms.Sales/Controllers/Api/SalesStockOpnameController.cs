using SimDms.Common;
using SimDms.Sales.BLL;
using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Sales.Controllers.Api
{
    public class SalesStockOpnameController : BaseController
    {
        public string getNewDoc(DateTime transdate)
        {
            try
            {
                return GetNewDocumentNoHpp("STC", transdate.ToString("yyyyMMdd"));
            }
            catch (System.Data.SqlClient.SqlException e) {
                return e.Message;
            }
        }

        public JsonResult ProsesStockTaking(ProsesStockTaking model) 
        {
            var pesan = "";
            if (!CompareString(model.WHCode, model.WHCodeTo))
            {
                pesan = "Gudang Awal tidak boleh melebihi Gudang Akhir";
                return Json(new {success = false, message = pesan });
            }
            if (!CompareString(model.SalesModelCode, model.SalesModelCodeTo))
            {
                pesan = "Model Awal tidak boleh melebihi Model Akhir";
                return Json(new { success = false, message = pesan });
            }
            if (!CompareString(model.SalesModelYear, model.SalesModelYearTo))
            {
                pesan = "Tahun Awal tidak boleh melebihi Tahun Akhir";
                return Json(new { success = false, message = pesan });
            }
            if (!CompareString(model.ColorCode, model.ColorCodeTo))
            {
                pesan = "Warna Awal tidak boleh melebihi Warna Akhir";
                return Json(new { success = false, message = pesan });
            }

            string qFilter = "";

            if (model.isWH) {
                qFilter += " AND WarehouseCode BETWEEN '" + model.WHCode + "'  AND '" + model.WHCodeTo + "'";
            }
            if (model.isModel) {
                qFilter += " AND SalesModelCode BETWEEN '" + model.SalesModelCode + "'  AND '" + model.SalesModelCodeTo + "'";
            }

            if (model.isYear)
            {
                qFilter += " AND SalesModelYear BETWEEN '" + model.SalesModelYear + "'  AND '" + model.SalesModelYearTo + "'";
            }

            if (model.isColor)
            {
                qFilter += " AND ColourCode BETWEEN '" + model.ColorCode + "'  AND '" + model.ColorCodeTo + "'";
            }

            
            
            bool result = false;
            var doc = getNewDoc(DateTime.Now);
            if (doc.Length > 20) {
                return Json(new { success = false, message = doc });
            }
            using (var transScope = new TransactionScope(TransactionScopeOption.RequiresNew,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                var record = ctx.omTrStockTakingHdrs.Find(CompanyCode, BranchCode, doc);
                if (record == null) {
                    record = new omTrStockTakingHdr { 
                        CompanyCode = CompanyCode,
                        BranchCode = BranchCode,
                        STHdrNo = doc,
                        CreatedBy = CurrentUser.UserId,
                        CreatedDate = DateTime.Now
                    
                    };
                    ctx.omTrStockTakingHdrs.Add(record);
                }
                record.STDate = model.STDate;
                record.Status = "0";
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = DateTime.Now;


                var query = string.Format(@"
                INSERT INTO OmTrStockTakingDtl(CompanyCode, BranchCode, STHdrNo, STNo, WareHouseCode, ChassisCode, ChassisNo 
                , EngineCode, EngineNo, SalesModelCode, SalesModelYear, ColourCode, Remark, Status, PrintSeq
                , CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
                SELECT '{0}' AS CompanyCode,'{1}' AS BranchCode, '{2}' AS STHdrNo, (ROW_NUMBER() OVER (PARTITION BY '{2}' ORDER BY ChassisCode, ChassisNo)) AS STNo
                , WarehouseCode, ChassisCode, ChassisNo
                , EngineCode, EngineNo, SalesModelCode, SalesModelYear, ColourCode, '' AS Remark, '0' AS Status, 0 AS PrintSeq
                , '{3}', GETDATE(), '{3}', GETDATE() FROM OmMstVehicle
                WHERE CompanyCode = '{0}' AND Status IN( '0', '3', '4') AND isActive = 1
                AND ChassisNo NOT IN(SELECT ChassisNo FROM OmTrStockTakingDtl WHERE CompanyCode = OmMstVehicle.CompanyCode
                AND BranchCode = '{1}' AND Status = '0') " + qFilter+ " ", CompanyCode, BranchCode, doc, CurrentUser.UserId);
                try
                {
                    ctx.SaveChanges();
                    ctx.Database.ExecuteSqlCommand(query);
                    ctx.SaveChanges();
                    transScope.Complete();

                    return Json(new { success = true, message = "Proses Stock Taking berhasil" });
                }
                catch (Exception ex) {
                    return Json(new { success = false, message = "Proses Stock Taking gagal atau sudah proses stock taking", error_log = ex.Message });            
                }
            }
        }

        public bool CompareString(string a, string b)
        {
            int comparison = string.Compare(a, b, false);
            if (comparison > 0)
            {
                return false;
            }
            else {
                return true;
            }
        }

        public JsonResult PostingStokTaking(PostingStokTaking model) 
        {
            string msg = "Proses posting stok taking tidak berhasi!!!";
            if (!CompareString(model.STHdrNo, model.STHdrNoTo)) {
                return Json(new { success = false, message = "No Awal tidak boleh lebih dari No. Akhir!!!" });
            }
            try
            {
                var omTrStockTakingHdrBLL = new OmTrStockTakingHdrBLL(ctx, CurrentUser.UserId);
                var result = omTrStockTakingHdrBLL.Posting(model.STHdrNo, model.STHdrNoTo);
                omTrStockTakingHdrBLL = null;
                if (result)
                {
                    msg = "Proses posting stok taking berhasil";
                }

                return Json(new { success = result, message = msg });
            }
            catch (Exception ex) {
                return Json(new { success = false, message = msg, error_log = ex.Message });
            }
        }

        public JsonResult InvTagPrint(ProsesStockTaking model) 
        {
            var pesan = "";
            if (!CompareString(model.WHCode, model.WHCodeTo))
            {
                pesan = "Gudang Awal tidak boleh melebihi Gudang Akhir";
                return Json(new { success = false, message = pesan });
            }
            if (!CompareString(model.SalesModelCode, model.SalesModelCodeTo))
            {
                pesan = "Model Awal tidak boleh melebihi Model Akhir";
                return Json(new { success = false, message = pesan });
            }
            if (!CompareString(model.SalesModelYear, model.SalesModelYearTo))
            {
                pesan = "Tahun Awal tidak boleh melebihi Tahun Akhir";
                return Json(new { success = false, message = pesan });
            }
            if (!CompareString(model.ColorCode, model.ColorCodeTo))
            {
                pesan = "Warna Awal tidak boleh melebihi Warna Akhir";
                return Json(new { success = false, message = pesan });
            }
            if (!CompareString(model.STHdrNo, model.STHdrNoTo)) {
                pesan = "No. Stock Opname Awal tidak boleh melebihi No. Stock Opname Akhir";
                return Json(new { success = false, message = pesan });
            }

            return Json(new { success = true });
        }

        public JsonResult Select4View(string STHdrNo)
        {
            try
            {
                var omTrStockTakingDtlBLL = new OmTrStockTakingDtlBLL(ctx, CurrentUser.UserId);
                var records = omTrStockTakingDtlBLL.Select4View(STHdrNo);
                omTrStockTakingDtlBLL = null;

                return Json(new { success = true, data = records });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Preview data inventory tidak berhasil.", error_log = ex.Message });
            }
        }

        public JsonResult ProsesEntryInventoryTag(string STHdrNo, List<omTrStockTakingDtlProcess> model)
        {
            string msg = "Proses Entry Inventory Tag tidak berhasil";
            try
            {
                bool result = false; int status = 0;
                var omTrStockTakingDtlBLL = new OmTrStockTakingDtlBLL(ctx, CurrentUser.UserId);
                using (var transScope = new TransactionScope(TransactionScopeOption.RequiresNew,
               new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    if (model.Count() > 0)
                    {
                        foreach (omTrStockTakingDtlProcess recDtl in model)
                        {
                            status = recDtl.Status == 0 ? 2 : 1;
                            result = omTrStockTakingDtlBLL.UpdateStatus(STHdrNo, recDtl.STNo, status);
                        }
                        omTrStockTakingHdr recHdr = ctx.omTrStockTakingHdrs.Find(CompanyCode, BranchCode, STHdrNo);
                        if (recHdr != null)
                        {
                            recHdr.Status = "1";
                            recHdr.LastUpdateBy = CurrentUser.UserId;
                            recHdr.LastUpdateDate = DateTime.Now;
                            result = ctx.SaveChanges() >= 0;
                        }
                        transScope.Complete();
                        msg = "Proses Entry Inventory Tag berhasil";
                    }
                    omTrStockTakingDtlBLL = null;

                    return Json(new { success = result, message = msg });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = msg, error_log = ex.Message });
            }
        }

        #region -- Private Method --
        
        #endregion
    }
}
