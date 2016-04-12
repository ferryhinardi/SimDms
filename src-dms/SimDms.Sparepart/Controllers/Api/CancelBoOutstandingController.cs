using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using SimDms.Sparepart.Models;
using System.Transactions;
using System.Data.SqlClient;
using System.Data;

namespace SimDms.Sparepart.Controllers.Api
{
    public class CancelBoOutstandingController : BaseController
    {
        public JsonResult ProsesBatalBO(string CompanyCode, string BranchCode, string PartNo, string SalesType,
            decimal QtyBo, string warehouseCode, string NoSO, string User, string PartNoOri, string Tax, decimal QtyOut,
            string CustomerCode, string TransType, string Note)
        {
            bool result = false;
            string msg = "";

            try
            {
                bool flagCancel = false;
                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = "EXEC uspfn_spCheckOutsQty '" + CompanyCode + "','" + BranchCode + "','" + CustomerCode + "','" + TransType + "','" + SalesType + "','" + PartNoOri + "'";
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    decimal boOut = Convert.ToDecimal(dr["QtyBOOts"].ToString());
                    if (QtyBo > boOut)
                        flagCancel = true;
                }
                if (dt.Rows.Count == 0)
                    flagCancel = true;
                if (flagCancel)
                {
                    msg = "Process gagal";
                    result = false;
                }
                else
                {
                    // Decrease back order on spMstItems and spMstitemLoc.
                    result = UpdateStock(PartNoOri, warehouseCode, 0, 0, (QtyBo * -1), 0, SalesType,User);
                    if (result)
                    {
                        // Increase Qty BO Cancel on spTrnSORDDtl.
                        result = UpdateQtyBOCancel(CompanyCode, BranchCode, QtyBo, User, NoSO, PartNo, warehouseCode, PartNoOri);

                        if (result)
                        {
                            result = logTransactionBOCancel(CompanyCode, BranchCode, NoSO,PartNo, PartNoOri,warehouseCode,QtyOut,QtyBo,Note,User);
                        }
                        msg="Proses cancel BO outstanding berhasil";
                        result = true;
                    }
                }
                //result = true;
            }
            catch
            {
                msg = "Process gagal";
                result = false;
            }
            return Json(new { success = result, message = msg });
        }

        public bool UpdateStock(string partno, string whcode, decimal onhand, decimal allocation, decimal backorder, decimal reserved, string salesType, string User)
        {
            bool result = false;
            string msg = "";
            spMstItem spMstItems = new spMstItem();
            SpMstItemLoc SpMstItemLocs = new SpMstItemLoc();
            var oItem = ctx.spMstItems.Find(CompanyCode, BranchCode, partno);           

            if (oItem != null)
            {
                var oItemLoc = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, partno, whcode);
                if (oItemLoc != null)
                {
                    if (Math.Abs(onhand) > 0)
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
                    if (Math.Abs(allocation) > 0)
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
                    if (Math.Abs(backorder) > 0)
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
                    if (Math.Abs(reserved) > 0)
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

                    //result = SpMstItemLocs.Update(oItemLoc) > 0;
                    //if (result) result = oItemDao.Update(oItem) > 0;

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
            var result=false;
            var mstItemLoc = ctx.SpMstItemLocs.Find(oItemLoc.CompanyCode, oItemLoc.BranchCode, oItemLoc.PartNo, oItemLoc.WarehouseCode);
            if (mstItemLoc !=null)
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

        public bool UpdateQtyBOCancel(string CompanyCode, string BranchCode,decimal QtyBo, string User, string NoSO, string PartNo, string warehouseCode, string PartNoOri)
        {
            var result = false;
            var trnSORDDtl = ctx.SpTrnSORDDtls.Find(CompanyCode,BranchCode,NoSO,PartNo,warehouseCode,PartNoOri);
            if (trnSORDDtl !=null)
            {
                trnSORDDtl.QtyBOCancel = trnSORDDtl.QtyBOCancel + QtyBo;
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

        public bool logTransactionBOCancel(string CompanyCode, string BranchCode, string DocNo, string PartNo, string PartNoOri, string warehouseCode, decimal QtyOut, decimal QtyBo, string Note, string User)
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
                    result=false;
                }
            }
            return result;

        }

    }
}
