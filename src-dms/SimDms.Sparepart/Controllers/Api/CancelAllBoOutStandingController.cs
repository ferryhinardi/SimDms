using SimDms.Common.Models;
using SimDms.Sparepart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Sparepart.Controllers.Api
{
    public class CancelAllBoOutStandingController : BaseController
    {
        public JsonResult CheckCancelAllBoOuts(string User, string CompanyCode, string BranchCode, List<spCancelAllBoOuts> model)
        {
            var msg = "";

            try
            {
                bool result = false;
                spCancelAllBoOuts tempData1 = new spCancelAllBoOuts();
                List<spCancelAllBoOuts> tempData2 = new List<spCancelAllBoOuts>();
                using (var transScope = new TransactionScope(TransactionScopeOption.RequiresNew,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    if (model.Count() > 0)
                    {
                        foreach (spCancelAllBoOuts records in model)
                        {
                            tempData1.chkSelect = records.chkSelect == 0 ? 2 : 1;
                            tempData1.DocNo = records.DocNo;
                            tempData1.PartNo = records.PartNo;
                            tempData1.PartNoOriginal = records.PartNoOriginal;
                            tempData1.QtyBOOts = records.QtyBOOts == 0 ? 0 : 0;
                            tempData1.QtyBOCancel = records.QtyBOCancel == 0 ? 0 : 0;
                            tempData2.Add(tempData1);
                        }

                        result = CheckDataChecked(tempData2);
                        if (result)
                        {
                            msg = "Proses berhasil";

                        }
                        else
                        {
                            msg = "Tidak ada BO yang dipilih untuk diproses";
                        }
                        transScope.Complete();

                    }
                    return Json(new { success = result, message = msg });
                }
            }
            catch (Exception ex)
            {
                msg = "Proses gagal";
                return Json(new { success = false, message = msg, error_log = ex.Message });
            }
        }

        public bool CheckDataChecked(List<spCancelAllBoOuts> tempData)
        {
            spCancelAllBoOuts tempList = new spCancelAllBoOuts();
            bool result = false;
            int chkCount = tempData.Count;

            var val = tempData.Find(p => p.chkSelect == 1);
            if (val == null)
            {
                result = false;
            }
            else
            {
                result = true;
            }

            return result;
        }

        public JsonResult ProcessCancelAllBoOuts(string User, string CompanyCode, string BranchCode, string SalesType,string Note, List<spCancelAllBoOuts> model)
        {
            var msg = "";
            bool result = false;

            try
            {
                
                spCancelAllBoOuts tempData1 = new spCancelAllBoOuts();
                List<spCancelAllBoOuts> tempData2 = new List<spCancelAllBoOuts>();
                using (var transScope = new TransactionScope(TransactionScopeOption.RequiresNew,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    if (model.Count() > 0)
                    {
                        foreach (spCancelAllBoOuts records in model)
                        {
                            //tempData1.chkSelect = records.chkSelect == 0 ? 2 : 1;
                            //tempData1.DocNo = records.DocNo;
                            //tempData1.PartNo = records.PartNo;
                            //tempData1.PartNoOriginal = records.PartNoOriginal;
                            //tempData1.QtyBOOts = records.QtyBOOts == 0 ? 0 : 0;
                            //tempData1.QtyBOCancel = records.QtyBOCancel == 0 ? 0 : 0;
                            tempData2.Add(records);
                        }

                        result = ProcessDetailData(User, CompanyCode, BranchCode,SalesType, Note, tempData2);
                        transScope.Complete();

                    }
                    result = true;                    
                }
            }
            catch (Exception ex)
            {
                msg = "Proses gagal";
                return Json(new { success = false, message = msg, error_log = ex.Message });
            }

            return Json(new { success = result, message = msg });
        }

        public bool ProcessDetailData(string User, string CompanyCode, string BranchCode, string SalesType,string Note, List<spCancelAllBoOuts> tempData)
        {
            bool result = false;
            decimal? boQty = 0;
            spCancelAllBoOuts tempData1 = new spCancelAllBoOuts();
            List<spCancelAllBoOuts> tempData2 = new List<spCancelAllBoOuts>();
            SpTrnSORDDtl recordDtl = new SpTrnSORDDtl();

            if (tempData.Count() > 0)
            {
                foreach (spCancelAllBoOuts records in tempData)
                {
                    tempData1.chkSelect = records.chkSelect == 0 ? 2 : 1;
                    tempData1.DocNo = records.DocNo;
                    tempData1.PartNo = records.PartNo;
                    tempData1.PartNoOriginal = records.PartNoOriginal;
                    tempData1.QtyBOOts = records.QtyBOOts == 0 ? 0 : 0;
                    tempData1.QtyBOCancel = records.QtyBOCancel == 0 ? 0 : 0;

                    recordDtl = GetRecordDetailData(User, CompanyCode, BranchCode, tempData1.DocNo, tempData1.PartNo, "00", tempData1.PartNoOriginal);

                    if (recordDtl != null)
                    {
                        boQty = recordDtl.QtyBO - recordDtl.QtyBOSupply - recordDtl.QtyBOCancel;
                        recordDtl.QtyBOCancel += boQty;

                        result = UpdateSpTrnSORDDtlQtyBoCancel(User, CompanyCode, BranchCode, tempData1.DocNo, tempData1.PartNo, "00", tempData1.PartNoOriginal, recordDtl.QtyBOCancel);
                        if (result)
                        {
                            result = UpdateStock(tempData1.PartNoOriginal, "00", 0, 0, (boQty * -1), 0, SalesType,User);
                            if (result)
                            {
                                result = logTransactionBOCancel(CompanyCode, BranchCode, tempData1.DocNo, tempData1.PartNo, tempData1.PartNoOriginal, "00", tempData1.QtyBOOts, tempData1.QtyBOCancel, Note, User);
                            }
                        }
                        else
                        {
                            return false;
                        }

                    }
                }

            }

            return result;
        }

        public SpTrnSORDDtl GetRecordDetailData(string User, string Company, string BranchCode, string DocNo, string PartNo, string Warehouse, string PartNoOriginal)
        {
            SpTrnSORDDtl recordDtl = new SpTrnSORDDtl();

            recordDtl = ctx.SpTrnSORDDtls.Find(CompanyCode,BranchCode,DocNo,PartNo,Warehouse,PartNoOriginal);
            return recordDtl;

        }

        public bool UpdateSpTrnSORDDtlQtyBoCancel(string User, string Company, string BranchCode, string DocNo, string PartNo, string Warehouse, string PartNoOriginal, decimal? QtyBOCancel)
        {
            bool result = false;
            var trnSORDDtl = ctx.SpTrnSORDDtls.Find(CompanyCode, BranchCode, DocNo, PartNo, Warehouse, PartNoOriginal);
            if (trnSORDDtl != null)
            {
                trnSORDDtl.QtyBOCancel = QtyBOCancel;
                trnSORDDtl.LastUpdateBy = User;
                trnSORDDtl.LastUpdateDate = DateTime.Now;
                try
                {
                    ctx.SaveChanges();
                    result = true;
                }
                catch
                {
                    result = false;
                } 
            }
            return result; 
        }

        public bool UpdateStock(string partno, string whcode, decimal? onhand, decimal? allocation, decimal? backorder, decimal? reserved, string salesType, string User)
        {
            bool result = false;
            string msg = "";
            decimal onh = Convert.ToDecimal(Convert.ToString(onhand));
            decimal allc = Convert.ToDecimal(Convert.ToString(allocation));
            decimal backOrder = Convert.ToDecimal(Convert.ToString(backorder));
            decimal rsrvd = Convert.ToDecimal(Convert.ToString(reserved));
            spMstItem spMstItems = new spMstItem();
            SpMstItemLoc SpMstItemLocs = new SpMstItemLoc();
            var oItem = ctx.spMstItems.Find(CompanyCode, BranchCode, partno);
            if (oItem != null)
            {
                var oItemLoc = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, partno, whcode);
                if (oItemLoc != null)
                {
                    if (Math.Abs(onh) > 0)
                    {
                        oItemLoc.OnHand += onhand;
                        oItem.OnHand += onhand;

                        if (oItemLoc.OnHand < 0)
                        {
                            msg = "OnHand untuk Part = '" + partno + "', ItemLoc = '" + oItemLoc.OnHand + "', transaksi tidak bisa lanjut";
                            result = false;
                        }
                        if (oItem.OnHand < 0)
                        {
                            msg = "OnHand untuk Part = '" + partno + "', ItemLoc = '" + oItem.OnHand + "', transaksi tidak bisa lanjut";
                            result = false;
                        }
                    }
                    if (Math.Abs(allc) > 0)
                    {
                        if (!string.IsNullOrEmpty(salesType) && (salesType == "0" || salesType == "1"))
                        {
                            oItemLoc.AllocationSP += allocation;
                            oItem.AllocationSP += allocation;

                            if (oItemLoc.AllocationSP < 0)
                            {
                                msg = "AllocationSP untuk Part = '" + partno + "', ItemLoc = '" + oItemLoc.AllocationSP + "', transaksi tidak bisa lanjut";
                                result = false;
                            }
                            if (oItem.AllocationSP < 0)
                            {
                                msg = "AllocationSP untuk Part = '" + partno + "', Item = '" + oItem.AllocationSP + "', transaksi tidak bisa lanjut";
                                result = false;
                            }
                        }
                        if (!string.IsNullOrEmpty(salesType) && salesType == "2")
                        {
                            oItemLoc.AllocationSR += allocation;
                            oItem.AllocationSR += allocation;

                            if (oItemLoc.AllocationSR < 0)
                            {
                                msg = "AllocationSR untuk Part = '" + partno + "', ItemLoc = '" + oItemLoc.AllocationSR + "', transaksi tidak bisa lanjut";
                                result = false;
                            }
                            if (oItem.AllocationSR < 0)
                            {
                                msg = "AllocationSR untuk Part = '" + partno + "', Item = '" + oItem.AllocationSR + "', transaksi tidak bisa lanjut";
                                result = false;
                            }
                        }
                        if (!string.IsNullOrEmpty(salesType) && salesType == "3")
                        {
                            oItemLoc.AllocationSL += allocation;
                            oItem.AllocationSL += allocation;

                            if (oItemLoc.AllocationSL < 0)
                            {
                                msg = "AllocationSL untuk Part = '" + partno + "', ItemLoc = '" + oItemLoc.AllocationSL + "', transaksi tidak bisa lanjut";
                                result = false;
                            }
                            if (oItem.AllocationSL < 0)
                            {
                                msg = "AllocationSL untuk Part = '" + partno + "', Item = '" + oItem.AllocationSL + "', transaksi tidak bisa lanjut";
                                result = false;
                            }
                        }
                    }
                    if (Math.Abs(backOrder) > 0)
                    {
                        if (!string.IsNullOrEmpty(salesType) && (salesType == "0" || salesType == "1"))
                        {
                            oItemLoc.BackOrderSP += backorder;
                            oItem.BackOrderSP += backorder;

                            if (oItemLoc.BackOrderSP < 0)
                            {
                                msg = "BackOrderSP untuk Part = '" + partno + "', ItemLoc = '" + oItemLoc.BackOrderSP + "', transaksi tidak bisa lanjut";
                                result = false;
                            }
                            if (oItem.BackOrderSP < 0)
                            {
                                msg = "BackOrderSP untuk Part = '" + partno + "', Item = '" + oItem.BackOrderSP + "', transaksi tidak bisa lanjut";
                                result = false;
                            }
                        }

                        if (!string.IsNullOrEmpty(salesType) && salesType == "2")
                        {
                            oItemLoc.BackOrderSR += backorder;
                            oItem.BackOrderSR += backorder;

                            if (oItemLoc.BackOrderSR < 0)
                            {
                                msg = "BackOrderSR untuk Part = '" + partno + "', ItemLoc = '" + oItemLoc.BackOrderSR + "', transaksi tidak bisa lanjut";
                                result = false;
                            }
                            if (oItem.BackOrderSR < 0)
                            {
                                msg = "BackOrderSR untuk Part = '" + partno + "', Item = '" + oItem.BackOrderSR + "', transaksi tidak bisa lanjut";
                                result = false;
                            }
                        }

                        if (!string.IsNullOrEmpty(salesType) && salesType == "3")
                        {
                            oItemLoc.BackOrderSL += backorder;
                            oItem.BackOrderSL += backorder;

                            if (oItemLoc.BackOrderSL < 0)
                            {
                                msg = "BackOrderSL untuk Part = '" + partno + "', ItemLoc = '" + oItemLoc.BackOrderSL + "', transaksi tidak bisa lanjut";
                                result = false;
                            }
                            if (oItem.BackOrderSL < 0)
                            {
                                msg = "BackOrderSL untuk Part = '" + partno + "', Item = '" + oItem.BackOrderSL + "', transaksi tidak bisa lanjut";
                                result = false;
                            }
                        }
                    }
                    if (Math.Abs(rsrvd) > 0)
                    {
                        oItemLoc.ReservedSP += reserved;
                        oItem.ReservedSP += reserved;

                        if (oItemLoc.ReservedSP < 0)
                        {
                            msg = "ReservedSP untuk Part = '" + partno + "', ItemLoc = '" + oItemLoc.ReservedSP + "', transaksi tidak bisa lanjut";
                            result = false;
                        }
                        if (oItem.ReservedSP < 0)
                        {
                            msg = "ReservedSP untuk Part = '" + partno + "', ItemLoc = '" + oItem.ReservedSP + "', transaksi tidak bisa lanjut";
                            result = false;
                        }
                    }

                    oItemLoc.LastUpdateDate = DateTime.Now;
                    oItemLoc.LastUpdateBy = User;
                    oItem.LastUpdateDate = DateTime.Now;
                    oItem.LastUpdateBy = User;

                    result = UpdateItemLocs(oItemLoc);
                    if (result)
                    {
                        UpdateItem(oItem);
                    }

                }
            }


            return result;
        }

        public bool UpdateItemLocs(SpMstItemLoc oItemLoc)
        {
            var result = false;
            var mstItemLoc = ctx.SpMstItemLocs.Find(oItemLoc.CompanyCode, oItemLoc.BranchCode, oItemLoc.PartNo, oItemLoc.WarehouseCode);
            if (mstItemLoc != null)
            {
                mstItemLoc.OnHand = oItemLoc.OnHand;
                mstItemLoc.AllocationSP = oItemLoc.AllocationSP;
                mstItemLoc.AllocationSR = oItemLoc.AllocationSR;
                mstItemLoc.AllocationSL = oItemLoc.AllocationSL;
                mstItemLoc.BackOrderSP = oItemLoc.BackOrderSP;
                mstItemLoc.BackOrderSR = oItemLoc.BackOrderSR;
                mstItemLoc.BackOrderSL = oItemLoc.BackOrderSL;
                mstItemLoc.ReservedSP = oItemLoc.ReservedSP;
                mstItemLoc.LastUpdateDate = oItemLoc.LastUpdateDate;
                mstItemLoc.LastUpdateBy = oItemLoc.LastUpdateBy;

                try
                {
                    ctx.SaveChanges();
                    result = true;
                }
                catch
                {
                    result = false;
                }

            }
            return result;
        }

        public bool UpdateItem(spMstItem oItem)
        {
            var result = false;
            var mstItem = ctx.spMstItems.Find(oItem.CompanyCode, oItem.BranchCode, oItem.PartNo);
            if (mstItem != null)
            {
                mstItem.OnHand = oItem.OnHand;
                mstItem.AllocationSP = oItem.AllocationSP;
                mstItem.AllocationSR = oItem.AllocationSR;
                mstItem.AllocationSL = oItem.AllocationSL;
                mstItem.BackOrderSP = oItem.BackOrderSP;
                mstItem.BackOrderSR = oItem.BackOrderSR;
                mstItem.BackOrderSL = oItem.BackOrderSL;
                mstItem.ReservedSP = oItem.ReservedSP;
                mstItem.LastUpdateDate = oItem.LastUpdateDate;
                mstItem.LastUpdateBy = oItem.LastUpdateBy;

                try
                {
                    ctx.SaveChanges();
                    result = true;
                }
                catch
                {
                    result = false;
                }

            }
            return result;
        }

        public bool logTransactionBOCancel(string CompanyCode, string BranchCode, string DocNo, string PartNo, string PartNoOri, string warehouseCode, decimal? QtyOut, decimal? QtyBo, string Note, string User)
        {
            bool result = false;
            var trnSORDHdr = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, DocNo);
            var trnSORDDtl = ctx.SpTrnSORDDtls.Find(CompanyCode, BranchCode, DocNo, PartNo, warehouseCode, PartNoOri);
            var docType = "BOC";
            //string year = Convert.ToString(DateTime.Now.Year);
            //string month = Convert.ToString(DateTime.Now.Month);
            //string day = Convert.ToString(DateTime.Now.Day);
            //string docNumber = GetNewDocumentNo(docType, Convert.ToDateTime(year + "-" + (Convert.ToString(Convert.ToUInt32(month) - 1)) + " - " + day));

            string docNumber = GetNewDocumentNo(docType, Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")));

            if (trnSORDDtl != null && trnSORDHdr != null)
            {
                spHstBOCancels recordLog = new spHstBOCancels();
                recordLog.BOCancelNo = docNumber;
                recordLog.CompanyCode = CompanyCode;
                recordLog.BranchCode = BranchCode;
                recordLog.Month = DateTime.Now.Month;
                recordLog.Year = DateTime.Now.Year;
                recordLog.DocNo = trnSORDHdr.DocNo;
                recordLog.DocDate = trnSORDHdr.DocDate;
                recordLog.PartNo = trnSORDDtl.PartNo;
                recordLog.WarehouseCode = trnSORDDtl.WarehouseCode;
                recordLog.ProductType = trnSORDDtl.ProductType;
                recordLog.PartCategory = trnSORDDtl.PartCategory;
                recordLog.MovingCode = trnSORDDtl.MovingCode;
                recordLog.ABCClass = trnSORDDtl.ABCClass;
                recordLog.BOOutstanding = QtyOut;
                recordLog.BOCancel = QtyBo;
                recordLog.CreatedBy = User;
                recordLog.CreatedDate = DateTime.Now;
                recordLog.Note = Note;

                ctx.spHstBOCancels.Add(recordLog);

                try
                {
                    ctx.SaveChanges();
                    result = true;
                }
                catch
                {
                    result = false;
                }
            }
            return result;

        }
    }
}
