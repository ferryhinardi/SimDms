using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.Sparepart.Models;
using TracerX;
using SimDms.Common;
using SimDms.Common.Models;

namespace SimDms.Sparepart.Controllers.Api
{
    public class PersediaanController : BaseController
    {
        private string msg = "";

        public string getDataNO(string Code)
        {
           // var transdate = ctx.CoProfileServices.Find(CompanyCode, BranchCode).TransDate;
            return GetNewDocumentNo(Code,DateTime.Now);
        }

        /*---------------------  Inventory Adjustment  --------------------*/

        public JsonResult getdatatablelnk5001(SpTrnIAdjustHdr model)
        {
            var Hdr = ctx.SpTrnIAdjustHdrs.Find(CompanyCode, BranchCode, model.AdjustmentNo);
            var queryable = ctx.Database.SqlQuery<spTrnIAdjustDtlView>("uspfn_spTrnIAdjustDtlview '" + CompanyCode + "','" + BranchCode + "','" + model.AdjustmentNo + "'").AsQueryable();
            return Json(new { table = queryable, lblstatus = SetStatusLabel(Hdr.Status) });
        }

        public JsonResult Savelnk5001(SpTrnIAdjustHdr model)
        {
            var Hdr = ctx.SpTrnIAdjustHdrs.Find(CompanyCode, BranchCode, model.AdjustmentNo);

            string msg = "";
            if (CurrentUser.TypeOfGoods == "" || CurrentUser.TypeOfGoods.Equals(null))
                return Json(new { success = false, message = "Type Of Goods User Belum Di Setting !" });
            if (checkStatusIA(Hdr))
                return Json(new { success = false, message = "Nomor dokumen ini sudah ter-posting !!" });
            var dtv = DateTransValidation(model.AdjustmentDate.Value);
            if (dtv != "")
                return Json(new { success = false, message = dtv });

            var record = Hdr;

            if (record == null)
            {
                record = new SpTrnIAdjustHdr
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    AdjustmentNo = getDataNO("ADJ"),
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };

                ctx.SpTrnIAdjustHdrs.Add(record);
                msg = "New Inventory Adjustment added...";
            }
            else
            {
                ctx.SpTrnIAdjustHdrs.Attach(record);
                msg = "Inventory Adjustment updated";
            }

            record.AdjustmentDate = model.AdjustmentDate;
            record.ReferenceNo = model.ReferenceNo;
            record.ReferenceDate = model.ReferenceDate;
            record.TypeOfGoods = CurrentUser.TypeOfGoods;
            record.Status = "0";
 
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;

            try
            {
                ctx.SaveChanges();

                return Json(new { success = true, message = msg, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Deletelnk5001(SpTrnIAdjustHdr model)
        {
            var msg = "";
            var record = ctx.SpTrnIAdjustHdrs.Find(CompanyCode, BranchCode, model.AdjustmentNo);

            if (checkStatusIA(record))
                return Json(new { success = false, message = "Nomor dokumen ini sudah ter-posting !!" });


            if (record != null)
            {
                record.Status = "3";
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = DateTime.Now;
            }

            try
            {
                ctx.SaveChanges();
                var lblStatus = SetStatusLabel(record.Status);
                return Json(new { success = true, lblstatus = lblStatus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
     
        public JsonResult SaveDetailslnk5001(SpTrnIAdjustDtl model)
        {
            var msg = "";
            var Hdr = ctx.SpTrnIAdjustHdrs.Find(CompanyCode, BranchCode, model.AdjustmentNo);

            if (checkStatusIA(Hdr))
                return Json(new { success = false, message = "Nomor dokumen ini sudah ter-posting !!" });

            if (model.QtyAdjustment <= 0)
                return Json(new { success = false, message = "Jumlah part yang akan di adjust harus lebih besar dari 0" });

            spMstItem recItems = ctx.spMstItems.Find(CompanyCode, BranchCode, model.PartNo);
            SpMstItemLoc recLoc = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, model.PartNo, model.WarehouseCode);
            spMstItemPrice recPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, model.PartNo);

            // Check if part no is available in Master Item, Master Item Loc or Master Item Price
            if (recItems == null || recLoc == null || recPrice == null)
            {
                msg = string.Format("Proses simpan detail adjustment gagal \nNo. part {0} belum terdaftar di Master Item / Master Item Lokasi / Master Item Price", model.PartNo);
                return Json(new { success = false, message = msg });
            }

            // Check if AdjustmentCode is "-" and not enough available part
            if (model.AdjustmentCode == "-")
            {
                decimal availableQty = recLoc.OnHand.Value - (recLoc.AllocationSL.Value + recLoc.AllocationSP.Value + recLoc.AllocationSR.Value)
                    - (recLoc.ReservedSL.Value + recLoc.ReservedSP.Value + recLoc.ReservedSR.Value);

                if (availableQty < model.QtyAdjustment)
                {
                    msg = string.Format("Proses simpan detail adjustment gagal \nAvailable qty untuk no. part {0} = {1}", model.PartNo, availableQty.ToString("n2"));
                    return Json(new { success = false, message = msg });
                }
            }

            var record = ctx.SpTrnIAdjustDtls.Find(CompanyCode, BranchCode, model.AdjustmentNo, model.WarehouseCode, model.PartNo);

            if (record == null)
            {
                record = new SpTrnIAdjustDtl
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    AdjustmentNo = model.AdjustmentNo,
                    WarehouseCode = model.WarehouseCode,
                    PartNo = model.PartNo,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };

                ctx.SpTrnIAdjustDtls.Add(record);
                msg = "New Inventory Adjusmnet Details added...";
            }
            else
            {
                ctx.SpTrnIAdjustDtls.Attach(record);
                msg = "Inventory Adjusmnet Details updated";
            }

            record.LocationCode = model.LocationCode;
            record.AdjustmentCode = model.AdjustmentCode;
            record.QtyAdjustment = model.QtyAdjustment;
            record.RetailPriceInclTax = recPrice.RetailPriceInclTax;
            record.RetailPrice = recPrice.RetailPrice;
            record.CostPrice = recPrice.CostPrice;
            record.ReasonCode = model.ReasonCode;
            record.MovingCode = model.MovingCode;
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;

            try
            {
                var result = ctx.SaveChanges() > 0;
                Hdr.Status = "0";
                Hdr.LastUpdateBy = CurrentUser.UserId;
                Hdr.LastUpdateDate = DateTime.Now;
                if (result)
                {
                    var queryable = ctx.Database.SqlQuery<spTrnIAdjustDtlView>("uspfn_spTrnIAdjustDtlview '" + CompanyCode + "','" + BranchCode + "','" + model.AdjustmentNo + "'").AsQueryable();

                    if (queryable != null)
                    {
                        return Json(new { success = true, data = queryable, count = queryable.Count(), status = SetStatusLabel(Hdr.Status) });
                    }
                    else
                    {
                        return Json(new { success = true, count = 0, status = SetStatusLabel(Hdr.Status) });
                    }
                }
                else
                {
                    msg = "Proses simpan detail adjustment gagal";
                    return Json(new { success = false, message = msg});
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult DeleteDetailslnk5001(SpTrnIAdjustDtl model)
        {
            var msg = "";
            var Hdr = ctx.SpTrnIAdjustHdrs.Find(CompanyCode, BranchCode, model.AdjustmentNo);

            if (checkStatusIA(Hdr))
                return Json(new { success = false, message = "Nomor dokumen ini sudah ter-posting !!" });

            var record = ctx.SpTrnIAdjustDtls.Find(CompanyCode, BranchCode, model.AdjustmentNo, model.WarehouseCode, model.PartNo);
 
            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.SpTrnIAdjustDtls.Remove(record);
                //get adjustment hdr
                //var adjHdr = ctx.SpTrnIAdjustHdrs.Find(CompanyCode, BranchCode, model.AdjustmentNo);
                Hdr.Status = "0";
                Hdr.LastUpdateBy = CurrentUser.UserId;
                Hdr.LastUpdateDate = DateTime.Now;
            }

            try
            {
                var result = ctx.SaveChanges() > 0;
                if(result)
                {
                    var queryable = ctx.Database.SqlQuery<spTrnIAdjustDtlView>("uspfn_spTrnIAdjustDtlview '" + CompanyCode + "','" + BranchCode + "','" + model.AdjustmentNo + "'").AsQueryable();

                    if (queryable != null)
                    {
                        return Json(new { success = true, data = queryable, count = queryable.Count(), status = SetStatusLabel(Hdr.Status) });
                    }
                    else
                    {
                        return Json(new { success = true, count = 0 });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Proses hapus detail adjustment gagal" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Printlnk5001(string adjustmentNo)
        {
            var Sign = ctx.GnMstSignatures.OrderBy(a => a.SeqNo)
                .FirstOrDefault(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ProfitCenterCode == ProfitCenter && a.DocumentType == "ADJ");

            var City = ctx.LookUpDtls.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.CodeID == "CITY" && a.LookUpValue == CurrentUser.CoProfile.CityCode).LookUpValueName.ToUpper();

            string signName = Sign == null ? string.Empty : Sign.SignName.ToUpper();
            string titleSign = Sign == null ? string.Empty : Sign.TitleSign.ToUpper();

            var Hdr = ctx.SpTrnIAdjustHdrs.Find(CompanyCode, BranchCode, adjustmentNo);
            if (Hdr != null)
            {
                Hdr.Status = "1";
                Hdr.PrintSeq += 1;
                Hdr.LastUpdateBy = CurrentUser.UserId;
                Hdr.LastUpdateDate = DateTime.Now;

                bool result = ctx.SaveChanges() > 0;
                if (!result)
                {
                    return Json(new { success = false, message = "Gagal print data" });
                }
            }
            return Json(new { success = true, lblstatus = SetStatusLabel(Hdr.Status) });
        }

        public JsonResult Postinglnk5001(SpTrnIAdjustHdr model)
        {
            var dtv = DateTransValidation(model.AdjustmentDate.Value);
            if (dtv != "")
                return Json(new { success = false, message = dtv });


            var dtl = ctx.SpTrnIAdjustDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.AdjustmentNo == model.AdjustmentNo).ToList();

            foreach (var dtls in dtl)
            {
                SpMstItemLoc recordLoc = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, dtls.PartNo, dtls.WarehouseCode);

                if (recordLoc == null)
                {
                    msg = string.Format("Proses posting adjustment Gagal, No. part {0} belum terdaftar di Master Item Lokasi", dtls.PartNo);
                    return Json(new { success = false, message = msg });
                }

                decimal availableQty = recordLoc.OnHand.Value - (recordLoc.AllocationSL.Value + recordLoc.AllocationSP.Value +
                    recordLoc.AllocationSR.Value) - (recordLoc.ReservedSL.Value + recordLoc.ReservedSP.Value + recordLoc.ReservedSR.Value);

                if (availableQty < dtls.QtyAdjustment && dtls.AdjustmentCode == "-")
                {
                    msg = string.Format("Proses posting adjustment gagal, \nAvailable qty untuk No. Part {0} = {1}", dtls.PartNo, availableQty.ToString("n2"));
                    return Json(new { success = false, message = msg });
                }
            }

            if (PostingAdjustment(model.AdjustmentNo))
            {
                var recUpdate = ctx.SpTrnIAdjustHdrs.Find(CompanyCode, BranchCode, model.AdjustmentNo);
                return Json(new { success = true, message = msg, data = recUpdate, lblstatus = SetStatusLabel(recUpdate.Status) });
            }
            else
            {
                msg = "Proses posting adjustment gagal";
                return Json(new { success = false, message = msg });
            }
        }

        public bool PostingAdjustment(string adjustmentNo)
        {
            bool result = false;

            try
            {
                SpTrnIAdjustHdr recordAdjustHdr = ctx.SpTrnIAdjustHdrs.Find(CompanyCode, BranchCode, adjustmentNo);

                if (recordAdjustHdr == null)
                    return false;
                else if (recordAdjustHdr.Status == "2")
                {
                    msg = "Proses Posting gagal \nNo. Nomor Adjustment = " + adjustmentNo + " telah di posting";
                    return false;
                }

                if (recordAdjustHdr.Status == "1")
                {
                    recordAdjustHdr.Status = "2";
                    result = ctx.SaveChanges() > 0;
                    if (result)
                    {
                        result = SavePosting(recordAdjustHdr, true);
                        if (result) result = UpdateStockAdjustment(recordAdjustHdr);
                       
                    }
                    else
                        msg = "Data sedang di Locking, Tunggu beberapa saat lagi";
                }
                else
                {
                    msg = "Status Adjustment tidak benar, tolong di Periksa";
                    result = false;
                }

            }
            catch (Exception ex)
            {
                result = false;
                msg = ex.Message;
            }
            
            return result;
        }

        public bool SavePosting(SpTrnIAdjustHdr oRecord, bool adjustment)
        {
            bool result = false;
           
            spMstAccount mstAccount = ctx.spMstAccounts.Find(oRecord.CompanyCode, oRecord.BranchCode, oRecord.TypeOfGoods);
          
            try
            {
                var query = string.Format(@"
            SELECT 
                A.AdjustmentCode AS AdjustmentCode,
                (SUM(A.QtyAdjustment * B.CostPrice) ) AS AmountPrice 
            FROM 
                SpTrnIAdjustDtl AS A, SpMstItemPrice AS B
            WHERE 
                A.CompanyCode = B.CompanyCode AND
                A.BranchCode = B.BranchCode AND
                A.PartNo = B.PartNo AND
                A.CompanyCode = '{0}' AND
                A.BranchCode = '{1}' AND 
                A.AdjustmentNo = '{2}'
               GROUP BY A.AdjustmentCode", CompanyCode, BranchCode, oRecord.AdjustmentNo);

                var tableGlAdj = ctx.Database.SqlQuery<GlAdjustContainer>(query).ToList();


                //var tableGlAdj = (from a in ctx.SpTrnIAdjustDtls
                //       from d in ctx.spMstItemPrices
                //       where a.CompanyCode == d.CompanyCode
                //       && a.BranchCode == d.BranchCode
                //       && a.PartNo == d.PartNo
                //       && a.CompanyCode == CompanyCode
                //       && a.BranchCode == BranchCode
                //       &&a.AdjustmentNo == oRecord.AdjustmentNo
                //       select new {
                //           AdjustmentCode = a.AdjustmentCode,
                //           AmountPrice = a.QtyAdjustment * d.CostPrice
                //       });

                if (tableGlAdj.Count() > 0)
                {
                    int seq = 1;
                    foreach (var row in tableGlAdj)
                    {
                        GLInterface glInterface = new GLInterface();
                        /* Create Journal for Debit */
                        glInterface.CompanyCode = CompanyCode;
                        glInterface.BranchCode = BranchCode;
                        glInterface.DocNo = oRecord.AdjustmentNo;
                        glInterface.SeqNo = seq;
                        seq++;

                        glInterface.DocDate = oRecord.AdjustmentDate;
                        glInterface.ProfitCenterCode = ProfitCenter;
                        glInterface.AccDate = oRecord.AdjustmentDate;

                        if (row.AdjustmentCode.Equals("+"))
                        {
                            glInterface.AccountNo = mstAccount.InventoryAccNo;
                            glInterface.TypeTrans = "INVENTORY";
                        }

                        if (row.AdjustmentCode.Equals("-"))
                        {
                            if (adjustment)
                            {
                                glInterface.AccountNo = mstAccount.COGSAccNo;
                                glInterface.TypeTrans = "COGS";
                            }
                            else
                            {
                                glInterface.AccountNo = mstAccount.OtherReceivableAccNo;
                                glInterface.TypeTrans = "AR OTHERS";
                            }
                        }

                        glInterface.JournalCode = "SPAREPART";
                        glInterface.TypeJournal = "ADJUSTMENT";
                        glInterface.ApplyTo = oRecord.AdjustmentNo;

                        if (row.AmountPrice == 0)
                        {
                            result = false;
                            msg = "Data tidak dapat disimpan karena item price belum disetting";
                            break;
                        }
                        glInterface.AmountDb = row.AmountPrice;
                        glInterface.AmountCr = 0;
                        glInterface.StatusFlag = "0";
                        glInterface.CreateBy = CurrentUser.UserId;
                        glInterface.CreateDate = DateTime.Now;
                        glInterface.LastUpdateBy = CurrentUser.UserId;
                        glInterface.LastUpdateDate = DateTime.Now;
                        ctx.GLInterfaces.Add(glInterface);

                        if (ctx.SaveChanges() < 0)
                        {
                            result = false;
                            msg = "Gagal simpan data";
                            break;
                        }
                        else
                            result = true;

                        glInterface = new GLInterface();
                        /* Create Journal for Kredit */
                        
                        glInterface.CompanyCode = CompanyCode;
                        glInterface.BranchCode = BranchCode;
                        glInterface.DocNo = oRecord.AdjustmentNo;
                        glInterface.SeqNo = seq;
                        seq++;

                        glInterface.DocDate = oRecord.AdjustmentDate;
                        glInterface.ProfitCenterCode = ProfitCenter;
                        glInterface.AccDate = oRecord.AdjustmentDate;

                        glInterface.JournalCode = "SPAREPART";
                        glInterface.TypeJournal = "ADJUSTMENT";
                        glInterface.ApplyTo = oRecord.AdjustmentNo;

                        if (row.AdjustmentCode.Equals("+"))
                        {
                            if (adjustment)
                            {
                                glInterface.AccountNo = mstAccount.COGSAccNo;
                                glInterface.TypeTrans = "COGS";
                            }
                            else
                            {
                                glInterface.AccountNo = mstAccount.OtherIncomeAccNo;
                                glInterface.TypeTrans = "OTHERINC";
                            }
                        }

                        if (row.AdjustmentCode.Equals("-"))
                        {
                            glInterface.AccountNo = mstAccount.InventoryAccNo;
                            glInterface.TypeTrans = "INVENTORY";
                        }

                        glInterface.AmountDb = 0;
                        glInterface.AmountCr = row.AmountPrice;
                        glInterface.StatusFlag = "0";
                        glInterface.CreateBy = CurrentUser.UserId;
                        glInterface.CreateDate = DateTime.Now;
                        glInterface.LastUpdateBy = CurrentUser.UserId;
                        glInterface.LastUpdateDate = DateTime.Now;
                        ctx.GLInterfaces.Add(glInterface);
                        if (ctx.SaveChanges() < 0)
                        {
                            result = false;
                            msg = "Gagal simpan data";
                            break;
                        }
                        else
                            result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public bool UpdateStockAdjustment(SpTrnIAdjustHdr oRecord)
        {
            bool result = false;
            decimal available = 0;
            SpMstItemLoc recordLoc;

            try
            {
                var query = string.Format(@"
                        SELECT 
                            Partno, WarehouseCode, AdjustmentCode, QtyAdjustment,
                            SUM( CASE AdjustmentCode WHEN '+' THEN (QtyAdjustment * 1) WHEN '-' THEN (QtyAdjustment * -1) END) AS Qty
                        FROM 
                            spTrnIAdjustDtl 
                        WHERE AdjustmentNo='{2}' AND 
                            CompanyCode = '{0}' AND
                            BranchCode = '{1}'
                        GROUP BY partNo, WarehouseCode, AdjustmentCode, QtyAdjustment;
                        ", CompanyCode, BranchCode, oRecord.AdjustmentNo);

                var tableItems = ctx.Database.SqlQuery<spTrnIAdjustDtlContainer>(query).ToList();

                foreach (var items in tableItems)
                {
                    // TO DO : This line of code below will validated the warehouseCode
                    recordLoc = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, items.PartNo, items.WarehouseCode);
                    available = recordLoc.OnHand.Value + items.Qty.Value - (recordLoc.AllocationSL.Value + recordLoc.AllocationSP.Value + recordLoc.AllocationSR.Value)
                        - (recordLoc.ReservedSP.Value + recordLoc.ReservedSL.Value + recordLoc.ReservedSR.Value);

                    if (available >= 0)
                    {
                        if (Convert.ToInt32(items.WarehouseCode) < 97)
                        {
                            result = UpdateStock(items.PartNo, items.WarehouseCode, items.Qty.Value, 0, 0, 0, string.Empty);
                        }
                        else
                        {
                            result = UpdateStockWarehouse(items.PartNo, items.WarehouseCode, items.Qty.Value, 0, 0, 0);
                        }

                        if (result)
                            result = MovementLog(oRecord.AdjustmentNo, oRecord.AdjustmentDate.Value,
                                items.PartNo, "", items.WarehouseCode, true, items.AdjustmentCode, items.QtyAdjustment.Value);
                    }
                    else
                    {
                        result = false;
                        msg = "Posting tidak dapat dilakukan karena available part kurang atau sama dengan nol";
                    }
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /*-------------------------Warehouse Transfer ------------------------*/

        public JsonResult getdatatablelnk5002(spTrnIWHTrfDtl model)
        {
            var Hdr = ctx.spTrnIWHTrfHdrs.Find(CompanyCode, BranchCode, model.WHTrfNo);
            var queryable = ctx.Database.SqlQuery<spTrnIWHTrfDtlView>("uspfn_spTrnIWHTrfDtlview '" + CompanyCode + "','" + BranchCode + "','" + model.WHTrfNo + "'").AsQueryable();

            return Json(new { table = queryable, lblstatus = SetStatusLabel(Hdr.Status) });
        }

        public JsonResult Savelnk5002(spTrnIWHTrfHdr model)
        {
            var Hdr = ctx.spTrnIWHTrfHdrs.Find(CompanyCode, BranchCode, model.WHTrfNo);

            if (checkStatusWH(Hdr))
            {
                return Json(new { success = false, message = "Nomor dokumen ini sudah ter-posting !!" });
            }

            if (TypeOfGoods == "" || TypeOfGoods.Equals(null))
            {
                return Json(new { success = false, message = "Type Of Goods User Belum Di Setting !" });
            }

            var dtv = DateTransValidation(model.WHTrfDate.Value);
            if (dtv != "") { return Json(new { success = false, message = dtv }); }

            var record = Hdr;

            if (record == null)
            {
                record = new spTrnIWHTrfHdr 
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    WHTrfNo = getDataNO("WTR"),
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };

                ctx.spTrnIWHTrfHdrs.Add(record);
                msg = "New Warehouse Transfer added...";
            }
            else
            {
                ctx.spTrnIWHTrfHdrs.Attach(record);
                msg = "Warehouse Transfer updated";
            }

            record.WHTrfDate = model.WHTrfDate;
            record.ReferenceNo = model.ReferenceNo;
            record.ReferenceDate = model.ReferenceDate;
            record.TypeOfGoods = TypeOfGoods;
            record.Status = "0";

            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, message = msg, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Deletelnk5002(spTrnIWHTrfHdr model)
        {
            var record = ctx.spTrnIWHTrfHdrs.Find(CompanyCode, BranchCode, model.WHTrfNo);

            if (checkStatusWH(record))
            {
                return Json(new { success = false, message = "Nomor dokumen ini sudah ter-posting , Record tidak bisa dihapus!!!" });
            }

            if (record != null)
            {
                record.Status = "3";
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = DateTime.Now;
            }

            try
            {
                ctx.SaveChanges();
                var lblStatus = SetStatusLabel(record.Status);
                return Json(new { success = true, lblstatus = lblStatus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult ValidateDtllnk5002(spTrnIWHTrfDtl model)
        {
            var msg_1101 = "Gagal simpan data";
            var recordItem = ctx.spMstItems.Find(CompanyCode,BranchCode, model.PartNo);
            var recordLocFrom = new SpMstItemLoc();

            if (recordItem == null)
            {
                return Json(new { success = false, message = msg_1101 });
            }
            else
            {
                recordLocFrom = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, model.PartNo, model.FromWarehouseCode);
                if (recordLocFrom == null)
                {
                    return Json(new { success = false, message = msg_1101 });
                }
                else
                {
                    LookUpDtl oWarehouse = ctx.LookUpDtls.Find(CompanyCode, "WRCD", model.ToWarehouseCode);
                    if (oWarehouse == null)
                    {
                        return Json(new { success = false, message = msg_1101 });
                    }
                }
            }

            if (recordLocFrom != null)
            {
                decimal avaQty = recordLocFrom.OnHand.Value - (recordLocFrom.AllocationSP.Value + recordLocFrom.AllocationSR.Value
                     + recordLocFrom.AllocationSL.Value + recordLocFrom.ReservedSL.Value + recordLocFrom.ReservedSP.Value
                     + recordLocFrom.ReservedSR.Value);
                if ((avaQty - model.Qty) < 0)
                {
                    var msg = "Data tidak dapat disimpan karena available part tidak mencukupi" + "\n" + "Available item : " + avaQty.ToString("n2");
                    return Json(new { success = false, message = msg});
                }
            }

            var data = ctx.spMstItems.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.PartNo == model.PartNo)
                .Join(ctx.spMstItemPrices
                , a => new { a.CompanyCode, a.BranchCode, a.PartNo }
                , b => new { b.CompanyCode, b.BranchCode, b.PartNo }
                , (a, b) => new { a, b })
                .Select(m => new WHDataDetail
                {
                    PartNo = m.a.PartNo,
                    MovingCode = m.a.MovingCode,
                    RetailPrice = m.b.RetailPrice,
                    RetailPriceInclTax = m.b.RetailPriceInclTax,
                    CostPrice = m.b.CostPrice,
                    LocationCode = ctx.SpMstItemLocs.FirstOrDefault(c => c.CompanyCode == CompanyCode && c.BranchCode == BranchCode && c.PartNo == model.PartNo).LocationCode
                }).FirstOrDefault();
                
            if (string.IsNullOrEmpty(data.PartNo))
            {
                return Json(new { success = false, message = "Transaksi tidak dapat diproses karena tidak ada data detail" });
            }
            if ((decimal)data.CostPrice == 0 || (decimal)data.RetailPrice == 0|| (decimal)data.RetailPriceInclTax == 0)
            {
                return Json(new { success = false, message = "Data tidak dapat disimpan karena item price belum disetting" });
            }

            return Json(new { success = true });
        }

        public JsonResult SaveDetailslnk5002(spTrnIWHTrfDtl model)
        {
            var recordItem = ctx.spMstItems.Find(CompanyCode, BranchCode, model.PartNo);
            var recordPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, model.PartNo);
            var recordLokasi = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, model.PartNo, model.FromWarehouseCode);
            var recordLoc = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, model.PartNo, model.ToWarehouseCode);

            var record = ctx.spTrnIWHTrfDtls.Find(CompanyCode, BranchCode, model.WHTrfNo, model.PartNo, model.FromWarehouseCode, model.ToWarehouseCode);

            if (record == null)
            {
                record = new spTrnIWHTrfDtl
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    WHTrfNo = model.WHTrfNo,
                    FromWarehouseCode = model.FromWarehouseCode,
                    ToWarehouseCode = model.ToWarehouseCode,
                    PartNo = model.PartNo,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };

                ctx.spTrnIWHTrfDtls.Add(record);
                msg = "New Warehouse Transfer Details added...";
            }
            else
            {
                ctx.spTrnIWHTrfDtls.Attach(record);
                msg = "Warehouse Transfer Details updated";
            }

            record.Qty = model.Qty;
            record.RetailPriceInclTax = recordPrice.RetailPriceInclTax;
            record.RetailPrice = recordPrice.RetailPrice;
            record.CostPrice = recordPrice.CostPrice;
            if (recordLokasi != null)
                record.FromLocationCode = recordLokasi.LocationCode;
            else
                record.FromLocationCode = "-";

            if (recordLoc != null)
                record.ToLocationCode = recordLoc.LocationCode;
            else
                record.ToLocationCode = "-";

            record.ReasonCode = model.ReasonCode;
            record.MovingCode = recordItem.MovingCode;
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;

            try
            {
                
                ctx.SaveChanges();

                var queryable = ctx.Database.SqlQuery<spTrnIWHTrfDtlView>("uspfn_spTrnIWHTrfDtlview '" + CompanyCode + "','" + BranchCode + "','" + model.WHTrfNo + "'").AsQueryable();

                if (queryable != null)
                {
                    var records = queryable.Select(x => new
                    {
                        PartNo = x.PartNo,
                        PartName = x.PartName,
                        FromWarehouseCode = x.FromWarehouseCode,
                        FromWarehouseName = x.FromWarehouseName,
                        ToWarehouseCode = x.ToWarehouseCode,
                        ToWarehouseName = x.ToWarehouseName,
                        ReasonCode = x.ReasonCode,
                        Qty = x.Qty 
                    }).ToList();

                    return Json(new { success = true, data = records, count = records.Count });
                }
                else
                {
                    return Json(new { success = true, count = 0 });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult DeleteDetailslnk5002(spTrnIWHTrfDtl model)
        {
            var record = ctx.spTrnIWHTrfDtls.Find(CompanyCode, BranchCode, model.WHTrfNo, model.PartNo, model.FromWarehouseCode, model.ToWarehouseCode);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.spTrnIWHTrfDtls.Remove(record);
            }

            try
            {
                ctx.SaveChanges();

                var queryable = ctx.Database.SqlQuery<spTrnIAdjustDtlView>("uspfn_spTrnIWHTrfDtlview '" + CompanyCode + "','" + BranchCode + "','" + model.WHTrfNo + "'").AsQueryable();

                if (queryable != null)
                {
                    return Json(new { success = true, data = queryable, count = queryable.Count() });
                }
                else
                {
                    return Json(new { success = true, count = 0 });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Postinglnk5002(spTrnIWHTrfHdr model)
        {
            var record = ctx.spTrnIWHTrfHdrs.Find(CompanyCode, BranchCode, model.WHTrfNo);

            if (checkStatusWH(record))
            {
                msg = "Nomor dokumen ini sudah ter-posting !!";
                return Json(new { success = false, message = msg });
            }

            var dtv = DateTransValidation(model.WHTrfDate.Value);
            if (dtv != "") { return Json(new { success = false, message = dtv }); }

            var whDetail = ctx.spTrnIWHTrfDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.WHTrfNo == model.WHTrfNo).ToList();

            bool negativeTranserQty = false;

            foreach (var detail in whDetail)
            {
                var recordLocFrom = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, detail.PartNo, detail.FromWarehouseCode);
                decimal availableQty = recordLocFrom.OnHand.Value - (recordLocFrom.AllocationSL.Value + recordLocFrom.AllocationSP.Value + recordLocFrom.AllocationSR.Value) -
                    (recordLocFrom.ReservedSL.Value + recordLocFrom.ReservedSP.Value + recordLocFrom.ReservedSR.Value);

                if (availableQty < detail.Qty.Value)
                {
                    negativeTranserQty = true;
                    msg = string.Format("Posting tidak dapat dilakukan karena available part tidak mencukupi, Available qty untuk part no {0} = {1}", detail.PartNo, availableQty.ToString("n2"));
                }
            }
            if (negativeTranserQty)
            {
                return Json(new { success = false, message = msg });
            }

            if (record.Status != "1")
            {
                if (record.Status == "0")
                {
                    msg = "Proses Proses Posting Warehouse Transfer gagal. Harap print dokumen terlebih dahulu";
                    return Json(new { success = false, message = msg });
                }
                else if (record.Status == "2")
                {
                    msg = string.Format("Dokumen No {0} sudah di {1}", record.WHTrfNo, "Posting");
                    return Json(new { success = false, message = msg });
                }
            }

            if (whDetail.Count() != 0)
            {
                bool stat = PostingTransfer(model.WHTrfNo);
                if (!stat)
                {
                    if (record.Status == "2" || record.Status == "3")
                    {
                        msg = string.Format("Dokumen No {0} sudah di {1}", record.WHTrfNo, "Posting");
                    }

                    msg = string.Format("Proses {0} gagal. {1}", "Posting Transfer", string.Empty);

                    return Json(new { success = false, message = msg });
                }
            }
            else
            {
                msg = "Transaksi tidak dapat diproses karena tidak ada data detail";
                return Json(new { success = false, message = msg});
            }

            var recUpdate = ctx.spTrnIWHTrfHdrs.Find(CompanyCode, BranchCode, model.WHTrfNo);
            return Json(new { success = true, message = msg, data = recUpdate, lblstatus = SetStatusLabel(recUpdate.Status) });
        }

        public JsonResult Printlnk5002(string whTrfNo)
        {
            var Hdr = ctx.spTrnIWHTrfHdrs.Find(CompanyCode, BranchCode, whTrfNo);
            if (checkStatusWH(Hdr))
            {
                return Json(new { success = false, message = "Nomor dokumen ini sudah ter-posting !!" });
            }

            var query = string.Format(@"SELECT  
                                row_number () OVER (ORDER BY spTrnIWHTrfDtl.CreatedDate) AS NoUrut,
                                spTrnIWHTrfDtl.PartNo, FromWarehouseCode,ToWarehouseCode,Qty,GnMstLookUpDtl.LookUpValueName,
                                SpMstItemInfo.PartName
                                FROM spTrnIWHTrfDtl WITH(NOLOCK, NOWAIT)
                                INNER JOIN spTrnIWHTrfHdr WITH(NOLOCK, NOWAIT) ON spTrnIWHTrfHdr.WHTrfNo = spTrnIWHTrfDtl.WHTrfNo
                                    AND spTrnIWHTrfHdr.CompanyCode = spTrnIWHTrfDtl.CompanyCode
                                    AND spTrnIWHTrfHdr.BranchCode = spTrnIWHTrfDtl.BranchCode
                                INNER JOIN gnMstLookUpDtl WITH(NOLOCK, NOWAIT) ON GnMstLookUpDtl.LookUpValue = spTrnIWHTrfDtl.ReasonCode
                                    AND gnMstLookUpDtl.CompanyCode = spTrnIWHTrfDtl.CompanyCode
                                INNER JOIN spMstItemInfo WITH(NOLOCK, NOWAIT) ON spMstItemInfo.PartNo = spTrnIWHTrfDtl.PartNo
                                    AND spMstItemInfo.CompanyCode = spTrnIWHTrfDtl.CompanyCode
                                where spTrnIWHTrfDtl.CompanyCode ={0}
                                AND spTrnIWHTrfDtl.BranchCode = {1} 
                                AND spTrnIWHTrfDtl.WHTrfNo = '{2}'
                                AND gnMstLookUpDtl.CodeId='RSWT'", CompanyCode, BranchCode, whTrfNo);

            var dt = ctx.Database.SqlQuery<WHLookupDetail>(query);

            if (dt.Count() == 0)
            {
                return Json(new { success = false, message = "Gagal print data" });

            }

            if (Hdr.Status.Equals("2"))
            {
                return Json(new { success = false, message = "Proses Posting Warehouse Transfer gagal. Dokumen sudah ter-posting" });
            }

            if (Hdr.Status == "0")
               
            Hdr.Status = "1";
            Hdr.PrintSeq += 1;
            Hdr.LastUpdateBy = CurrentUser.UserId;
            Hdr.LastUpdateDate = DateTime.Now;

            bool result = ctx.SaveChanges() > 0;
            if (!result)
            {
                return Json(new { success = false, message = "Gagal print data" });
            }

            return Json(new { success = true, lblstatus = SetStatusLabel(Hdr.Status) });
        }

        private bool PostingTransfer(string WHTrfNo)
        {
            bool result = false;

            try
            {
                var query = string.Format(@"SET Lock_Timeout 500; SELECT TOP 1 * FROM SpTrnIWHTrfHdr  WITH (updLock, ReadPast)
                                WHERE CompanyCode = '{0}' AND BranchCode = '{1}' AND WHTrfNo = '{2}'",CompanyCode,BranchCode, WHTrfNo);
                var recordWHTrfHdr = ctx.Database.SqlQuery<spTrnIWHTrfHdr>(query).FirstOrDefault();

                if (recordWHTrfHdr == null)
                    return false;

                if (recordWHTrfHdr.Status == "2")
                {
                    msg =string.Format("Proses {0} gagal. {1}Posting", "\nNo. Nomor Warehouse Transfer = " + WHTrfNo + " telah di posting");
                    return false;
                }
                string epesan = "";
                result = updateQuantity(WHTrfNo, ref epesan);
                if (!result)
                {
                    msg = epesan;
                    return result;
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message.ToString();
            }
         
            return result;
        }

        private bool updateQuantity(string whTrfNo, ref string ePesan)
        {
            bool stat = false;
            int caseStock = 0;
            bool result = false;
            //decimal QtyTujuan = 0;
            decimal QtyReal = 0;
            string[] warehouseRest = { "97", "98", "99", "X1", "X2", "X3" };

            var whDetail = ctx.spTrnIWHTrfDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.WHTrfNo == whTrfNo).ToList();
            if (whDetail.Count() > 0)
            {
                foreach (var detail in whDetail)
                {
                   stat = true;

                    // NOTES : This line will insert new item locations based on the the "events" value
                    var recordLocTo = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode,
                                  detail.PartNo,
                                  detail.ToWarehouseCode);

                    var recordLocFrom = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode,
                                    detail.PartNo,
                                    detail.FromWarehouseCode);

                    // TO DO : This line of code below will prepare the data
                    var recordItem = ctx.spMstItems.Find(CompanyCode, BranchCode,
                                    detail.PartNo);

                    var recordDetail = ctx.spTrnIWHTrfDtls.Find(CompanyCode, BranchCode, 
                                   detail.WHTrfNo,
                                   detail.PartNo, 
                                   detail.FromWarehouseCode, 
                                   detail.ToWarehouseCode);

                    var Hdr = ctx.spTrnIWHTrfHdrs.Find(CompanyCode, BranchCode, detail.WHTrfNo);

                    if (recordLocTo == null)
                    {
                        recordLocTo = new SpMstItemLoc();
                        recordLocTo.CompanyCode = CompanyCode;
                        recordLocTo.BranchCode = BranchCode;
                        recordLocTo.WarehouseCode = detail.ToWarehouseCode;
                        recordLocTo.LocationCode = "-";
                        recordLocTo.PartNo = detail.PartNo;
                        recordLocTo.CreatedBy = CurrentUser.UserId;
                        recordLocTo.CreatedDate = DateTime.Now;

                        ctx.SpMstItemLocs.Add(recordLocTo);
                    }

                    ctx.SaveChanges();

                    // TO DO : This line of code will update the items
                    if (recordDetail != null)
                    {
                        // TO DO : This line of code will check again the Qty
                        decimal avaQty = recordLocFrom.OnHand.Value - (recordLocFrom.AllocationSP.Value
                            + recordLocFrom.AllocationSR.Value + recordLocFrom.AllocationSL.Value
                            + recordLocFrom.ReservedSP.Value + recordLocFrom.ReservedSL.Value
                            + recordLocFrom.ReservedSR.Value);

                        QtyReal = detail.Qty.Value;

                        if (avaQty < QtyReal)
                        {
                            // TO DO : This line of code will handle the error that caused not by exception
                            ePesan = "Qty yang tersedia tidak mencukupi utk Claim";
                            return false;
                        }

                        // TO DO : This line of code below will update the spMstItems
                        // ==========================================================

                        #region Validation Update Stock
                        caseStock = SetCaseUpdateStock(detail.FromWarehouseCode,
                                    detail.ToWarehouseCode, warehouseRest);

                        switch (caseStock)
                        {   /*  Claim Part [Shortage, Damage, Wrong Part]*/
                            case 1:
                                result = UpdateStock(detail.PartNo,
                                         detail.FromWarehouseCode, (QtyReal * -1), 0, 0, 0, string.Empty)
                                         &&
                                         UpdateStockWarehouse(detail.PartNo,
                                         detail.ToWarehouseCode, QtyReal, 0, 0, 0);
                                break;
                            /*  Receiving Claim Part [Shortage, Damage, Wrong Part]*/
                            case 2:
                                result = UpdateStock( detail.PartNo,
                                         detail.ToWarehouseCode, QtyReal, 0, 0, 0, string.Empty)
                                         &&
                                         UpdateStockWarehouse(detail.PartNo,
                                         detail.FromWarehouseCode, (QtyReal * -1), 0, 0, 0);
                                break;
                            /*  Warehouse Transfer */
                            case 3:
                                result = UpdateStockWarehouse( detail.PartNo,
                                         detail.FromWarehouseCode, (QtyReal * -1), 0, 0, 0)
                                         &&
                                         UpdateStockWarehouse(detail.PartNo,
                                         detail.ToWarehouseCode, QtyReal, 0, 0, 0);
                                break;
                        }

                        #endregion

                        if (!result)
                        {
                            ePesan = "Proses Update Persediaan Gagal";
                            result = false;
                            return result;
                        }

                        if (result)
                            result = MovementLog(Hdr.WHTrfNo, Hdr.WHTrfDate.Value, detail.PartNo, detail.ToWarehouseCode,
                                detail.FromWarehouseCode, false, "",detail.Qty.Value);

                        if (!result)
                        {
                            ePesan = "Update Movement Log Transaksi Gagal";
                            result = false;
                            return result;
                        }

                        if (stat)
                        {
                            Hdr.Status = "2";
                            Hdr.LastUpdateBy = CurrentUser.UserId;
                            Hdr.LastUpdateDate = DateTime.Now;
                            result = ctx.SaveChanges() > 0;

                            if (!result)
                            {
                                ePesan = "Update Status Warehouse TRF Header Gagal";
                                result = false;
                                return result;
                            }
                        }
                    }
                    else
                    {
                        ePesan = "Detail Data transaksi tidak ada";
                        result = false;
                        return result;
                    }
                }
            }
            return result;
        }

        private int SetCaseUpdateStock(string fromWH, string toWH, string[] whValid)
        {
            if (fromWH.StartsWith("X"))
            {
                return 2;
            }
            else if (toWH.StartsWith("X"))
            {
                return 1;
            }
            else
            {
                try
                {
                    if (Convert.ToInt32(fromWH) < 97)
                    {
                        for (int x = 0; x < whValid.Length; x++)
                        {
                            if (toWH == whValid[x].ToString())
                                return 1;
                        }
                        return 3;
                    }
                    else
                    {
                        for (int x = 0; x < whValid.Length; x++)
                        {
                            if (toWH == whValid[x].ToString())
                                return 3;
                        }
                        return 2;
                    }
                }
                catch (Exception)
                {

                    return 3;
                }
            }
        }

        /*-------------------------Reserved Sparepart ------------------------*/

        public JsonResult getdatatablelnk5003(spTrnIReservedDtl model)
        {
            var Hdr = ctx.spTrnIReservedHdrs.Find(CompanyCode, BranchCode, model.ReservedNo);
            var queryable = ctx.Database.SqlQuery<spTrnIReservedDtlView>("uspfn_spTrnIReservedDtlview '" + CompanyCode + "','" + BranchCode + "','" + model.ReservedNo + "'").AsQueryable();
            return Json(new { table = queryable, lblstatus = SetStatusLabel(Hdr.Status) });
        }

        public JsonResult Savelnk5003(spTrnIReservedHdr model)
        {
            if (TypeOfGoods == "" || TypeOfGoods == null)
                return Json(new { success = false, message = "Type Of Goods User Belum Di Setting !" });

            var dtv = DateTransValidation(model.ReservedDate.Value);
            if (dtv != "")
                return Json(new { success = false, message = dtv });

            string RSVNo = "";

            if (string.IsNullOrEmpty(model.ReservedNo))
            {
                RSVNo = getDataNO("RSV");

                if (RSVNo.EndsWith("X"))
                msg = "Dokumen No Reserved belum ada di Master Dokumen";
                return Json(new { success = false, message = msg });
            }
            else
                RSVNo = model.ReservedNo;

            var record = ctx.spTrnIReservedHdrs.Find(CompanyCode, BranchCode, RSVNo);

            if (record == null)
            {
                record = new spTrnIReservedHdr
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    ReservedNo = RSVNo,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };

                ctx.spTrnIReservedHdrs.Add(record);
                msg = "New Reserved Sparepart added...";
            }
            else
            {
                ctx.spTrnIReservedHdrs.Attach(record);
                msg = "Reserved Sparepart updated";
            }

            record.ReferenceNo = model.ReferenceNo;
            record.ReservedDate = model.ReservedDate;
            record.ReferenceDate = model.ReferenceDate;
            record.TypeOfGoods = TypeOfGoods;
            record.PrintSeq = 0;
            record.Status = "0";
            record.OprCode = model.OprCode;
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;

            try
            {
                ctx.SaveChanges();

                return Json(new { success = true, message = msg, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Deletelnk5003(spTrnIReservedHdr model)
        {
            var record = ctx.spTrnIReservedHdrs.Find(CompanyCode, BranchCode, model.ReservedNo);

            if (!record.Status.Equals("0"))
            {
                msg = "Data ini tidak boleh dihapus";
                return Json(new { success = false, message = msg });
            }

            record.Status = "3";
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;

            if (record != null)
            {
                record.Status = "3";
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = DateTime.Now;
            }

            try
            {
                ctx.SaveChanges();
                var lblStatus = SetStatusLabel(record.Status);
                return Json(new { success = true, lblstatus = lblStatus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
              
        public JsonResult SaveDetailslnk5003(spTrnIReservedDtl model)
        {
            var record = ctx.spTrnIReservedDtls.Find(CompanyCode, BranchCode, model.ReservedNo, model.WarehouseCode, model.PartNo);

            if (record == null)
            {
                record = new spTrnIReservedDtl
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    ReservedNo = model.ReservedNo,
                    WarehouseCode = model.WarehouseCode,
                    PartNo = model.PartNo,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };

                ctx.spTrnIReservedDtls.Add(record);
                msg = "Reserved Sparepart Details added...";
            }
            else
            {
                ctx.spTrnIReservedDtls.Attach(record);
                msg = "Reserved Sparepart Details updated";
            }

            if (!string.IsNullOrEmpty(model.LocationCode))
                record.LocationCode = model.LocationCode;
            else
                record.LocationCode = "-";

            
            SpMstItemLoc oSpMstItemLoc = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, model.PartNo, model.WarehouseCode);

            decimal availableQty = 0;

            if (oSpMstItemLoc != null)
            {
                availableQty = oSpMstItemLoc.OnHand.Value - oSpMstItemLoc.AllocationSP.Value - oSpMstItemLoc.AllocationSL.Value -
                    oSpMstItemLoc.AllocationSR.Value - oSpMstItemLoc.ReservedSP.Value - oSpMstItemLoc.ReservedSL.Value - oSpMstItemLoc.ReservedSR.Value;
            }

            record.AvailableQty = availableQty;
            record.ReservedQty = model.ReservedQty;
 
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;

            try
            {
                ctx.SaveChanges();

                var queryable = ctx.Database.SqlQuery<spTrnIReservedDtlView>("uspfn_spTrnIReservedDtlview '" + CompanyCode + "','" + BranchCode + "','" + model.ReservedNo + "'").AsQueryable();

                if (queryable != null)
                {
                    var records = queryable.Select(x => new
                    {
                        PartNo = x.PartNo,
                        PartName = x.PartName,
                        AvailableQty = x.AvailableQty,
                        ReservedQty = x.ReservedQty
                    }).ToList();



                    return Json(new { success = true, data = records, count = records.Count });
                }
                else
                {
                    return Json(new { success = true, count = 0 });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
         
        public JsonResult DeleteDetailslnk5003(spTrnIReservedDtl model)
        {

            var record = ctx.spTrnIReservedDtls.Find(CompanyCode, BranchCode, model.ReservedNo, "00", model.PartNo);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.spTrnIReservedDtls.Remove(record);
            }

            try
            {
                ctx.SaveChanges();

                var queryable = ctx.Database.SqlQuery<spTrnIReservedDtlView>("uspfn_spTrnIReservedDtlview '" + CompanyCode + "','" + BranchCode + "','" + model.ReservedNo + "'").AsQueryable();

                if (queryable != null)
                {
                    var records = queryable.Select(x => new
                    {
                        PartNo = x.PartNo,
                        PartName = x.PartName,
                        AvailableQty = x.AvailableQty,
                        ReservedQty = x.ReservedQty
                    }).ToList();


                    return Json(new { success = true, data = records, count = records.Count });
                }
                else
                {
                    return Json(new { success = true, count = 0 });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public Object PartInfo(string PartNo)
        {
            string partNo = Request["PartNo"] ?? "";
            if (!string.IsNullOrEmpty(PartNo))
            {
                return Json(new
                {
                    data1 = MyHelpers.GetTable(ctx, "select OnHand,AllocationSP,AllocationSR,AllocationSL,BackOrderSP,BackOrderSR,BackOrderSL,ReservedSP,ReservedSL,ReservedSR  from spMstItemLoc where CompanyCode='" + CompanyCode + "' and BranchCode='" + BranchCode + "' and WarehouseCode='00' and  PartNo='" + partNo + "'"),
                    data2 = MyHelpers.GetTable(ctx, "select OnOrder,InTransit,MovingCode,ABCClass from spMstItems where CompanyCode='" + CompanyCode + "' and BranchCode='" + BranchCode + "' and  PartNo='" + partNo + "'"),
                    data3 = MyHelpers.GetTable(ctx, "select a.SupplierCode,a.SupplierName from gnMstSupplier a inner join spMstItemInfo b on a.SupplierCode=b.SupplierCode where a.CompanyCode='" + CompanyCode + "' and   b.PartNo='" + partNo + "'"),
                    dataQty = MyHelpers.GetTable(ctx, "exec uspfn_spMasterPartView '" + CompanyCode + "','" + BranchCode + "','" + partNo + "'")
                });
            }
            return null;
        }

        public JsonResult Postinglnk5003(spTrnIReservedHdr model)
        {
            var dtv = DateTransValidation(model.ReservedDate.Value);
            if (dtv != "") { return Json(new { success = false, message = dtv }); }
            
            var Hdr = ctx.spTrnIReservedHdrs.Find(CompanyCode, BranchCode, model.ReservedNo);

            if (Hdr != null)
            {
                if (Hdr.Status != "1")
                {
                    if (Hdr.Status == "2")
                        msg = "Proses Posting Reserved gagal. Dokumen sudah ter-Posting";
                    else
                        msg = "Proses Posting Reserved gagal.Harap print dokumen terlebih dahulu";

                    return Json(new { success = false, message = msg });
                }
            }

            var Dtls = ctx.spTrnIReservedDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ReservedNo == Hdr.ReservedNo).ToList();

            if (Dtls == null)
            {
                msg = "Transaksi tidak dapat diproses karena tidak ada data detail";
                return Json(new { success = false, message = msg });
            }

            string message = "";
            string message1 = "";
            bool minusAvailable = false;
            bool minusReserved = false;

            foreach (var recordReserved in Dtls)
            {
                SpMstItemLoc recordLoc = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, recordReserved.PartNo, recordReserved.WarehouseCode);

                // Calculate the reserved
                decimal allocation = recordLoc.AllocationSL.Value + recordLoc.AllocationSP.Value + recordLoc.AllocationSR.Value;
                decimal reservedWOSp = recordLoc.ReservedSL.Value + recordLoc.ReservedSR.Value + recordLoc.ReservedSP.Value;
                decimal available = 0;
                if (Hdr.OprCode == "-")
                    available = recordLoc.OnHand.Value - allocation - reservedWOSp + (recordReserved.ReservedQty.Value);
                else
                    available = recordLoc.OnHand.Value - allocation - reservedWOSp - (recordReserved.ReservedQty.Value);

                if (available < 0)
                {
                    minusAvailable = true;
                    decimal maxAvailable = recordLoc.OnHand.Value - (recordLoc.AllocationSL.Value + recordLoc.AllocationSP.Value + recordLoc.AllocationSR.Value) -
                        (recordLoc.ReservedSR.Value + recordLoc.ReservedSL.Value + recordLoc.ReservedSP.Value);
                    message = string.Format("\nQty available untuk no. part {0} = {1}", recordReserved.PartNo, maxAvailable.ToString("n2"));
                }

                // Check if current release (-) reserved > qty reserved that store in spMstItemLoc table
                if ((Hdr.OprCode == "-") && (recordReserved.ReservedQty > recordLoc.ReservedSP))
                {
                    minusReserved = true;
                    message1 = string.Format("\nQty reserved maksimal untuk no. part {0} = {1}", recordReserved.PartNo, recordLoc.ReservedSP.Value.ToString("n2"));
                }
            }

            if (minusAvailable)
            {
                msg = "Posting tidak dapat dilakukan karena available part tidak mencukupi" + message.ToString();
                return Json(new { success = false, message = msg });
            }

            if (minusReserved)
            {
                msg = "Proses posting reserved gagal." + message1.ToString();
                return Json(new { success = false, message = msg });
            }

            #region PostingReversed
            bool isPosted = false;

            try
            {
                // update SpTrnIReservedHdr
                Hdr.Status = "2";    // POSTED
                Hdr.LastUpdateBy = CurrentUser.UserId;
                Hdr.LastUpdateDate = DateTime.Now;
                isPosted = ctx.SaveChanges() > 0;

                // update SpMstItemLocBLL table
                foreach (var recDetail in Dtls)
                {
                    // get SpMstItemLoc
                    var oSpMstItemLoc = ctx.SpMstItemLocs.Find(recDetail.CompanyCode, recDetail.BranchCode,
                        recDetail.PartNo, recDetail.WarehouseCode);
                    if (oSpMstItemLoc == null)
                        isPosted = false;

                    decimal allocation = oSpMstItemLoc.AllocationSL.Value + oSpMstItemLoc.AllocationSP.Value + oSpMstItemLoc.AllocationSR.Value;
                    decimal reserved = oSpMstItemLoc.ReservedSL.Value + oSpMstItemLoc.ReservedSP.Value + oSpMstItemLoc.ReservedSR.Value;
                    decimal available = 0;

                    if (Hdr.OprCode.Equals("+"))
                        available = oSpMstItemLoc.OnHand.Value - allocation - reserved - recDetail.ReservedQty.Value;
                    else
                        available = oSpMstItemLoc.OnHand.Value - allocation - reserved + recDetail.ReservedQty.Value;

                    if (available >= 0)
                    {
                        decimal reservedQty = Hdr.OprCode.Equals("+") ? recDetail.ReservedQty.Value : recDetail.ReservedQty.Value * -1;
                        // update SpMstItemLoc table
                        isPosted = UpdateStock(recDetail.PartNo, "00", 0, 0, 0, reservedQty, string.Empty);
                    }
                }

                // update SpMstItems table
                foreach (var oReservedDtl in Dtls)
                {
                    var totalReservedSP = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, oReservedDtl.PartNo, oReservedDtl.WarehouseCode).ReservedSP;

                    // get SpMstItem by Part No
                    var oSpMstItem = ctx.spMstItems.Find(CompanyCode, BranchCode, oReservedDtl.PartNo);

                    // update ReservedSP in SpMstItem
                    oSpMstItem.ReservedSP = totalReservedSP;

                    // update last update in SpMstItem
                    oSpMstItem.LastUpdateBy = CurrentUser.UserId;
                    oSpMstItem.LastUpdateDate = DateTime.Now;

                    // update SpMstItems table
                    isPosted = ctx.SaveChanges() > 0;
                }

            }
            catch (Exception ex)
            {
                isPosted = false;
                msg = ex.Message;
            }
            
            #endregion

            if (isPosted)
            {
                var recUpdate = ctx.spTrnIReservedHdrs.Find(CompanyCode, BranchCode, model.ReservedNo);
                return Json(new { success = true, message = msg, data = recUpdate, lblstatus = SetStatusLabel(recUpdate.Status) });
            }
            else
            {
                // show error message
                if (Hdr.Status.Equals("2")) //checking for reserved sparepart that was posted or not
                {
                    msg = string.Format("Dokumen No {0} sudah di {1}", Hdr.ReservedNo, "Posting, gagal simpan data.");
                    return Json(new { success = true, message = msg });
                }
                else
                    msg = "Posting tidak dapat dilakukan karena available part kurang atau sama dengan nol";
                return Json(new { success = true, message = msg });

            }
        }

        public JsonResult Printlnk5003(string reservedNo)
        {
            var Hdr = ctx.spTrnIReservedHdrs.Find(CompanyCode, BranchCode, reservedNo);

            if (Hdr.Status.Equals("2"))
            {
                return Json(new { success = false, message = "Proses Posting Warehouse Transfer gagal. Dokumen sudah ter-posting" });
            }

            if (Hdr.Status == "0")

            Hdr.Status = "1";
            Hdr.PrintSeq += 1;
            Hdr.LastUpdateBy = CurrentUser.UserId;
            Hdr.LastUpdateDate = DateTime.Now;

            bool result = ctx.SaveChanges() > 0;
            if (!result)
            {
                return Json(new { success = false, message = "Gagal print data" });
            }

            return Json(new { success = true, lblstatus = SetStatusLabel(Hdr.Status) });
        }

        #region General

        private bool checkStatusWH(spTrnIWHTrfHdr Hdr)
        {
            if (Hdr != null)
                if (Hdr.Status == "2" || Hdr.Status == "3")
                    return true;

            return false;
        }

        private bool checkStatusIA(SpTrnIAdjustHdr Hdr)
        {
            if (Hdr != null)
                if (Hdr.Status == "2" || Hdr.Status == "3")
                    return true;

            return false;
        }

        private bool UpdateStock(string partno, string whcode, decimal onhand, decimal allocation, decimal backorder, decimal reserved, string salesType)
        {
            bool result = false;
            spMstItem oItem = ctx.spMstItems.Find(CompanyCode, BranchCode, partno);
            if (oItem != null)
            {
                //TODO : Tambahkan check result untuk yang hasilnya negatif
                SpMstItemLoc oItemLoc = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, partno, whcode);

                if (oItemLoc != null)
                {
                    if (Math.Abs(onhand) > 0)
                    {
                        oItemLoc.OnHand += onhand;
                        oItem.OnHand += onhand;

                        // Tambahkan check result untuk yang ItemLoc negatif
                        if (oItemLoc.OnHand < 0)
                        {
                            msg = string.Format("OnHand untuk Part = {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.OnHand);
                            return false;
                        }

                        // Tambahkan check result untuk yang Item negatif
                        if (oItem.OnHand < 0)
                        {
                            msg = string.Format("OnHand untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.OnHand);
                            return false;
                        }
                    }

                    if (Math.Abs(allocation) > 0)
                    {
                        if (!string.IsNullOrEmpty(salesType) && (salesType == "0" || salesType == "1"))
                        {
                            oItemLoc.AllocationSP += allocation;
                            oItem.AllocationSP += allocation;

                            // Tambahkan check result untuk yang ItemLoc negatif
                            if (oItemLoc.AllocationSP < 0)
                            {
                                msg = string.Format("AllocationSP untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.AllocationSP);
                                return false;
                            }

                            // Tambahkan check result untuk yang Item negatif
                            if (oItem.AllocationSP < 0)
                            {
                                msg = string.Format("AllocationSP untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItem.AllocationSP);
                                return false;
                            }
                        }

                        if (!string.IsNullOrEmpty(salesType) && salesType == "2")
                        {
                            oItemLoc.AllocationSR += allocation;
                            oItem.AllocationSR += allocation;

                            // Tambahkan check result untuk yang ItemLoc negatif
                            if (oItemLoc.AllocationSR < 0)
                            {
                                msg = string.Format("AllocationSR untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.AllocationSR);
                                return false;
                            }

                            // Tambahkan check result untuk yang Item negatif
                            if (oItem.AllocationSR < 0)
                            {
                                msg = string.Format("AllocationSR untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItem.AllocationSR);
                                return false;
                            }
                        }

                        if (!string.IsNullOrEmpty(salesType) && salesType == "3")
                        {
                            oItemLoc.AllocationSL += allocation;
                            oItem.AllocationSL += allocation;

                            // Tambahkan check result untuk yang ItemLoc negatif
                            if (oItemLoc.AllocationSL < 0)
                            {
                                msg = string.Format("AllocationSL untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.AllocationSL);
                                return false;
                            }

                            // Tambahkan check result untuk yang Item negatif
                            if (oItem.AllocationSL < 0)
                            {
                                msg = string.Format("AllocationSL untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItem.AllocationSL);
                                return false;
                            }
                        }
                    }

                    if (Math.Abs(backorder) > 0)
                    {
                        if (!string.IsNullOrEmpty(salesType) && (salesType == "0" || salesType == "1"))
                        {
                            oItemLoc.BackOrderSP += backorder;
                            oItem.BackOrderSP += backorder;

                            // Tambahkan check result untuk yang ItemLoc negatif
                            if (oItemLoc.BackOrderSP < 0)
                            {
                                msg = string.Format("BackOrderSP untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.BackOrderSP);
                                return false;
                            }

                            // Tambahkan check result untuk yang Item negatif
                            if (oItem.BackOrderSP < 0)
                            {
                                msg = string.Format("BackOrderSP untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.BackOrderSP);
                                return false;
                            }
                        }

                        if (!string.IsNullOrEmpty(salesType) && salesType == "2")
                        {
                            oItemLoc.BackOrderSR += backorder;
                            oItem.BackOrderSR += backorder;

                            // Tambahkan check result untuk yang ItemLoc negatif
                            if (oItemLoc.BackOrderSR < 0)
                            {
                                msg = string.Format("BackOrderSR untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.BackOrderSR);
                                return false;
                            }

                            // Tambahkan check result untuk yang Item negatif
                            if (oItem.BackOrderSR < 0)
                            {
                                msg = string.Format("BackOrderSR untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.BackOrderSR);
                                return false;
                            }
                        }

                        if (!string.IsNullOrEmpty(salesType) && salesType == "3")
                        {
                            oItemLoc.BackOrderSL += backorder;
                            oItem.BackOrderSL += backorder;

                            // Tambahkan check result untuk yang ItemLoc negatif
                            if (oItemLoc.BackOrderSL < 0)
                            {
                                msg = string.Format("BackOrderSL untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.BackOrderSL);
                                return false;
                            }

                            // Tambahkan check result untuk yang Item negatif
                            if (oItem.BackOrderSL < 0)
                            {
                                msg = string.Format("BackOrderSL untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.BackOrderSL);
                                return false;
                            }
                        }
                    }

                    if (Math.Abs(reserved) > 0)
                    {
                        oItemLoc.ReservedSP += reserved;
                        oItem.ReservedSP += reserved;

                        // Tambahkan check result untuk yang ItemLoc negatif
                        if (oItemLoc.ReservedSP < 0)
                        {
                            msg = string.Format("ReservedSP untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.ReservedSP);
                            return false;
                        }

                        // Tambahkan check result untuk yang Item negatif
                        if (oItem.ReservedSP < 0)
                        {
                            msg = string.Format("ReservedSP untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.ReservedSP);
                            return false;
                        }
                    }
                    oItemLoc.LastUpdateDate = DateTime.Now;
                    oItemLoc.LastUpdateBy = CurrentUser.UserId;

                    result = ctx.SaveChanges() > 0;
                    if (result)
                        oItem.LastUpdateDate = DateTime.Now;
                    oItem.LastUpdateBy = CurrentUser.UserId;
                    result = ctx.SaveChanges() > 0;
                }
            }
            return result;
        }

        private bool UpdateStockWarehouse(string partno, string whcode, decimal onhand, decimal allocation, decimal backorder, decimal reserved)
        {
            bool result = false;
            SpMstItemLoc oItemLoc = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, partno, whcode);

            if (oItemLoc != null)
            {
                //TODO : Tambahkan check result untuk yang hasilnya negatif
                if (Math.Abs(onhand) > 0)
                {
                    oItemLoc.OnHand += onhand;

                    // Tambahkan check result untuk yang ItemLoc negatif
                    if (oItemLoc.OnHand < 0)
                    {
                        msg = string.Format("OnHand untuk Part = {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.OnHand);
                        return false;
                    }
                }

                if (Math.Abs(allocation) > 0)
                {
                    oItemLoc.AllocationSP += allocation;

                    // Tambahkan check result untuk yang ItemLoc negatif
                    if (oItemLoc.AllocationSP < 0)
                    {
                        msg = string.Format("AllocationSP untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.AllocationSP);
                        return false;
                    }
                }

                if (Math.Abs(backorder) > 0)
                {
                    oItemLoc.BackOrderSP += backorder;

                    // Tambahkan check result untuk yang ItemLoc negatif
                    if (oItemLoc.BackOrderSP < 0)
                    {
                        msg = string.Format("BackOrderSP untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.BackOrderSP);
                        return false;
                    }
                }

                if (Math.Abs(reserved) > 0)
                {
                    oItemLoc.ReservedSP += reserved;

                    // Tambahkan check result untuk yang ItemLoc negatif
                    if (oItemLoc.ReservedSP < 0)
                    {
                        msg = string.Format("ReservedSP untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.ReservedSP);
                        return false;
                    }
                }
                oItemLoc.LastUpdateDate = DateTime.Now;
                oItemLoc.LastUpdateBy = CurrentUser.UserId;

                result = ctx.SaveChanges() > 0;
            }
            return result;
        }

        private bool MovementLog(string docno, DateTime docdate, string partno, string whcodeTo, string whcodeFrom, bool caseAdj, string opAdjust, decimal qty)
        {
            string signCode = "OUT";
            string subSignCode = "";
            bool result = false;
            spMstItem oItems = ctx.spMstItems.Find(CompanyCode, BranchCode, partno);
            spMstItemPrice oItemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, partno);
            SpMstItemLoc oItemLocFrom = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, partno, whcodeFrom);
            SpMstItemLoc oItemLocTo = new SpMstItemLoc();

            //TO DO : Nilai dari subsigncode dan signcode perlu dimasukkan dalam commonBLL
            if (caseAdj)
            {
                subSignCode = "ADJ";
                if (opAdjust == "+")
                    signCode = "IN";
            }
            else
                subSignCode = "WTR";

            // TO DO : Insert to Item Movement record
            if (whcodeTo != "")
            {
                SpTrnIMovement oIMovement = new SpTrnIMovement();
                oItemLocTo = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, partno, whcodeTo);
                if (oItemLocTo != null)
                {
                    oIMovement.CompanyCode = CompanyCode;
                    oIMovement.BranchCode = BranchCode;
                    oIMovement.DocNo = docno;
                    oIMovement.DocDate = docdate;
                    oIMovement.CreatedDate = DateTime.Now;

                    oIMovement.WarehouseCode = oItemLocTo.WarehouseCode;
                    oIMovement.LocationCode = oItemLocTo.LocationCode;
                    oIMovement.PartNo = oItemLocTo.PartNo;
                    oIMovement.SignCode = "IN";
                    oIMovement.SubSignCode = subSignCode;
                    oIMovement.Qty = qty;
                    oIMovement.Price = oItemPrice.RetailPrice;
                    oIMovement.CostPrice = oItemPrice.CostPrice;
                    oIMovement.ABCClass = oItems.ABCClass;
                    oIMovement.MovingCode = oItems.MovingCode;
                    oIMovement.ProductType = oItems.ProductType;
                    oIMovement.PartCategory = oItems.PartCategory;
                    oIMovement.CreatedBy = CurrentUser.UserId;

                    ctx.SpTrnIMovements.Add(oIMovement);
                    result = ctx.SaveChanges() > 0;
                }
                else
                    result = false;
            }

            if (oItemLocFrom != null && oItemPrice != null && oItems != null)
            {
                SpTrnIMovement oIMovement = new SpTrnIMovement();

                oIMovement.CompanyCode = CompanyCode;
                oIMovement.BranchCode = BranchCode;
                oIMovement.DocNo = docno;
                oIMovement.DocDate = docdate;
                //oIMovement.CreatedDate = DmsTime.Now.AddMinutes(1);
                oIMovement.CreatedDate = DateTime.Now;

                oIMovement.WarehouseCode = oItemLocFrom.WarehouseCode;
                oIMovement.LocationCode = oItemLocFrom.LocationCode;
                oIMovement.PartNo = oItemLocFrom.PartNo;
                oIMovement.SignCode = signCode;
                oIMovement.SubSignCode = subSignCode;
                oIMovement.Qty = qty;
                oIMovement.Price = oItemPrice.RetailPrice;
                oIMovement.CostPrice = oItemPrice.CostPrice;
                oIMovement.ABCClass = oItems.ABCClass;
                oIMovement.MovingCode = oItems.MovingCode;
                oIMovement.ProductType = oItems.ProductType;
                oIMovement.PartCategory = oItems.PartCategory;
                oIMovement.CreatedBy = CurrentUser.UserId;

                ctx.SpTrnIMovements.Add(oIMovement);
                result = ctx.SaveChanges() > 0;
            }
            return result;
        } 
        
        #endregion
    }
}
