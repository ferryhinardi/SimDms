using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimDms.Sparepart.Models;
using SimDms.Common;
using SimDms.Common.Models;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace SimDms.Sparepart.Controllers.Api
{
    public class MaintenanceOrderSubtitusiController : BaseController
    {
        #region -- JsonResult public method --
        public JsonResult ValidateInput(string posNo, string partNo)
        {
            try
            {
                // TO DO : Check if change value by user
                //1.No Pos
                var distinctTrnPOrderBalance = 
                    (from a in ctx.spTrnPOrderBalances
                    join b in ctx.GnMstSuppliers on new {a.SupplierCode, a.CompanyCode} equals new {b.SupplierCode, b.CompanyCode}
                    where a.OrderQty > a.Received
                        && a.CompanyCode == CompanyCode
                        && a.BranchCode == BranchCode
                        && a.TypeOfGoods == TypeOfGoods
                        && a.POSNo == posNo
                     orderby a.POSNo descending
                    select a.POSNo
                    ).Distinct().ToList();

                if (distinctTrnPOrderBalance.Count() == 0)
                {
                    return Json(new { success = false, message = "No POS tidak diketahui." });
                }
                //2.No part
                var listTrnPOrderBalance = ctx.spTrnPOrderBalances.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.POSNo == posNo && x.PartNo == partNo).ToList();
                if (listTrnPOrderBalance.Count() == 0)
                {
                    return Json(new { success = false, message = "No Part tidak diketahui." });
                }

                var recSpTrnPOrderBalance = new spTrnPOrderBalance();
                if (listTrnPOrderBalance.Count() > 0)
                {
                    var rec = listTrnPOrderBalance.FirstOrDefault();
                    recSpTrnPOrderBalance = ctx.spTrnPOrderBalances.Find(CompanyCode, BranchCode, posNo, rec.SupplierCode, partNo, rec.SeqNo);
                }

                if (recSpTrnPOrderBalance == null)
                    return Json(new { success = "undefined", message = "" });
                else
                    return Json(new { success = true, message = "Are you sure ?" });
            }
            catch (Exception ex)
            {
                string innerEx = (ex.InnerException == null) ? ex.Message :
                    (ex.InnerException.InnerException == null) ? ex.Message : ex.InnerException.InnerException.Message;
                {
                    return Json(new
                    {
                        success = false,
                        message = (ex.InnerException == null) ?
                        " Periksa kembali inputan Anda!" + " Error Message:" + ex.Message :
                        " Periksa kembali inputan Anda!" + " Error Message:" + innerEx
                    });
                }
            }
        }
        #endregion

        #region -- CRUD public method --
        public JsonResult Save(spTrnPOrderBalance record, string newPartNo, decimal newQty)
        {
            using (var tranScope = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                bool bolSuccsess = false;
                string strMessage = string.Empty;
                try
                {
                    string WAREHOUSECODE = "00";

                    var listTrnPOrderBalance = ctx.spTrnPOrderBalances.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.POSNo == record.POSNo && x.PartNo == record.PartNo).ToList();

                    var recSpTrnPOrderBalance = new spTrnPOrderBalance();
                    if (listTrnPOrderBalance.Count() > 0)
                    {
                        var rec = listTrnPOrderBalance.FirstOrDefault();
                        recSpTrnPOrderBalance = ctx.spTrnPOrderBalances.Find(CompanyCode, BranchCode, record.POSNo, rec.SupplierCode, record.PartNo, rec.SeqNo);
                    }

                    if (recSpTrnPOrderBalance != null)
                    {
                        var ItemLoc = @"select * from " + GetDbMD() + @"..SpMstItemLoc where CompanyCode='" + CompanyMD + "' AND BranchCode='" + BranchMD + "' and PartNo ='" + newPartNo + "' and WarehouseCode ='" + WAREHOUSECODE + "'";
                        var recItemLoc = ctx.Database.SqlQuery<SpMstItemLoc>(ItemLoc).FirstOrDefault();
                        if (recItemLoc == null)
                        {
                            strMessage = "Data tidak dapat disimpan karena item lokasi untuk no. part subtitusi belum disetting !";
                            return Json(new { success = false, message = strMessage });
                        }

                        var Item = @"select * from " + GetDbMD() + @"..spMstItems where CompanyCode='" + CompanyMD + "' AND BranchCode='" + BranchMD + "' and PartNo ='" + newPartNo + "'";
                        var recItem = ctx.Database.SqlQuery<spMstItem>(Item).FirstOrDefault();

                        if (recItem.Status != "1")
                        {
                            strMessage = "Data tidak dapat disimpan karena status no. part subtitusi belum aktif !";
                            return Json(new { success = bolSuccsess, message = strMessage });
                        }

                        // Check if subtitution part have "0" purchase price
                        var sqlItemPrice = string.Format("SELECT * FROM {0}..spMstItemPrice WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                                    GetDbMD(), CompanyMD, BranchMD, newPartNo);
                        var itmPrice = ctx.Database.SqlQuery<spMstItemPrice>(sqlItemPrice).FirstOrDefault();

                        if (itmPrice != null)
                        {
                            if (itmPrice.PurchasePrice == 0)
                                return Json(new { success = bolSuccsess, message = ctx.SysMsgs.Find(SysMessages.MSG_5029).MessageCaption });

                        }
                        else
                            return Json(new { success = bolSuccsess, message = ctx.SysMsgs.Find(SysMessages.MSG_5029).MessageCaption });


                        bolSuccsess = p_Save(recSpTrnPOrderBalance, newPartNo, newQty);
                        if (bolSuccsess)
                        {
                            tranScope.Commit();
                            strMessage = ctx.SysMsgs.Find(SysMessages.MSG_1100).MessageCaption;
                        }
                        else
                        {
                            tranScope.Rollback();
                            throw new Exception("Proses Simpan Maintenace Order Substitusi Gagal");
                        }

                        return Json(new { success = bolSuccsess, message = strMessage });
                    }
                    else
                    {
                        strMessage = "Data tidak dapat disimpan karena No. POS tidak ditemukan.";

                        tranScope.Rollback();
                        return Json(new { success = false, message = strMessage });
                    }
                }
                catch (Exception ex)
                {
                    tranScope.Rollback();
                    string innerEx = (ex.InnerException == null) ? ex.Message :
                        (ex.InnerException.InnerException == null) ? ex.Message : ex.InnerException.InnerException.Message;
                    {
                        return Json(new
                        {
                            success = false,
                            message = (ex.InnerException == null) ?
                            " Periksa kembali inputan Anda!" + " Error Message:" + ex.Message :
                            " Periksa kembali inputan Anda!" + " Error Message:" + innerEx
                        });
                    }
                }
            }
        }
        #endregion

        #region -- private method --
        public bool p_Save(spTrnPOrderBalance recSpTrnPOrderBalance, string newPartNo, decimal newQty)
        {
            bool result = false;
            spMstItem recordItemsNew = new spMstItem();
            spMstItem recordItemsSub = new spMstItem();
            spMstItemPrice recordPriceNew = new spMstItemPrice();

            try
            {
                recSpTrnPOrderBalance.OnOrder = recSpTrnPOrderBalance.OnOrder - newQty;

                // New Item
                var Item = @"select * from " + GetDbMD() + @"..spMstItems where CompanyCode='" + CompanyMD + "' AND BranchCode='" + BranchMD + "' and PartNo ='" + newPartNo + "'";
                recordItemsNew = ctx.Database.SqlQuery<spMstItem>(Item).FirstOrDefault();

                // Item Substitut
                Item = @"select * from " + GetDbMD() + @"..spMstItems where CompanyCode='" + CompanyMD + "' AND BranchCode='" + BranchMD + "' and PartNo ='" + recSpTrnPOrderBalance.PartNo + "'";
                recordItemsSub = ctx.Database.SqlQuery<spMstItem>(Item).FirstOrDefault();

                // Item Price
                var sqlItemPrice = string.Format("SELECT * FROM {0}..spMstItemPrice WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                            GetDbMD(), CompanyMD, BranchMD, newPartNo);
                recordPriceNew = ctx.Database.SqlQuery<spMstItemPrice>(sqlItemPrice).FirstOrDefault();

                spTrnPOrderBalance temp = ctx.spTrnPOrderBalances.Find(CompanyCode, BranchCode, recSpTrnPOrderBalance.POSNo, recSpTrnPOrderBalance.SupplierCode,
                                          recSpTrnPOrderBalance.PartNo, recSpTrnPOrderBalance.SeqNo);
                if (temp == null)
                {
                    Helpers.ReplaceNullable(recSpTrnPOrderBalance);
                    ctx.spTrnPOrderBalances.Add(recSpTrnPOrderBalance);
                    result = ctx.SaveChanges() > 0;

                    if (!result)
                    {
                        throw new Exception("Insert Data Order Pembelian Gagal");
                    }
                }
                else
                {
                    //var z = ctx.SaveChanges();
                    result = ctx.SaveChanges() >= 0;
                    if (!result)
                    {
                        throw new Exception("Update Data Order Pembelian Gagal");
                    }
                }

                result = p_SaveHistoryBalance(recSpTrnPOrderBalance);
                if (!result)
                {
                    throw new Exception("Update Data Histori Order Pembelian Gagal");
                }

                if (!result)
                {
                    return result;
                }
                else
                {
                    // New spTrnPOrderBalance Data
                    spTrnPOrderBalance newRecord = new spTrnPOrderBalance();
                    newRecord.CompanyCode = recSpTrnPOrderBalance.CompanyCode;
                    newRecord.BranchCode = recSpTrnPOrderBalance.BranchCode;
                    newRecord.POSNo = recSpTrnPOrderBalance.POSNo;
                    newRecord.SupplierCode = recSpTrnPOrderBalance.SupplierCode;
                    newRecord.PartNo = newPartNo;
                    newRecord.SeqNo = p_GetNewSeqNo(recSpTrnPOrderBalance.POSNo);
                    newRecord.PartNoOriginal = recSpTrnPOrderBalance.PartNoOriginal;
                    newRecord.POSDate = recSpTrnPOrderBalance.POSDate;
                    newRecord.OrderQty = recSpTrnPOrderBalance.OrderQty;
                    newRecord.OnOrder = newQty;
                    newRecord.DiscPct = recSpTrnPOrderBalance.DiscPct;

                    if (recordPriceNew != null)
                    {
                        newRecord.PurchasePrice = recordPriceNew.PurchasePrice;
                        newRecord.CostPrice = recordPriceNew.CostPrice;
                        recordPriceNew = null;
                    }
                    else
                        return result = false;

                    if (!string.IsNullOrEmpty(recordItemsNew.ABCClass))
                        newRecord.ABCClass = recordItemsNew.ABCClass;
                    else
                        newRecord.ABCClass = string.Empty;
                    if (!string.IsNullOrEmpty(recordItemsNew.MovingCode))
                        newRecord.MovingCode = recordItemsNew.MovingCode;
                    else
                        newRecord.MovingCode = string.Empty;

                    newRecord.WRSNo = recSpTrnPOrderBalance.WRSNo;
                    newRecord.WRSDate = recSpTrnPOrderBalance.WRSDate;
                    newRecord.TypeOfGoods = recSpTrnPOrderBalance.TypeOfGoods;
                    newRecord.CreatedBy = CurrentUser.UserId;
                    newRecord.CreatedDate = DateTime.Now;
                    newRecord.LastUpdateBy = CurrentUser.UserId;
                    newRecord.LastUpdateDate = DateTime.Now;

                    temp = ctx.spTrnPOrderBalances.Find(CompanyCode, BranchCode, newRecord.POSNo, newRecord.SupplierCode,
                                          newRecord.PartNo, newRecord.SeqNo);
                    if (temp == null)
                    {
                        Helpers.ReplaceNullable(newRecord);
                        ctx.spTrnPOrderBalances.Add(newRecord);
                        result = ctx.SaveChanges() > 0;

                        if (!result)
                        {
                            throw new Exception("Insert Data Order Pembelian Gagal");
                        }
                    }
                    else
                    {
                        result = ctx.SaveChanges() >= 0;
                        if (!result)
                        {
                            throw new Exception("Update Data Order Pembelian Gagal");
                        }
                    }

                    result = p_SaveHistoryBalance(newRecord);
                    if (!result)
                    {
                        throw new Exception("Update Data Histori Order Pembelian Gagal");
                    }

                    newRecord = null;

                    if (!result)
                    {
                        return result;
                    }
                    else
                    {
                        if (recordItemsNew != null && recordItemsSub != null)
                        {
                            // Update onOrder for partNew
                            recordItemsNew.OnOrder += newQty;
                            recordItemsNew.LastUpdateBy = CurrentUser.UserId;
                            recordItemsNew.LastUpdateDate = DateTime.Now;
                            var query = string.Format(@"update {0}..spMstItems set OnOrder = {4} , LastUpdateBy ='{5}', LastUpdateDate ='{6}' where CompanyCode = '{1}' and BranchCode = '{2}' and PartNo = '{3}'", GetDbMD(), CompanyMD, BranchMD, recordItemsNew.PartNo, recordItemsNew.OnOrder, recordItemsNew.LastUpdateBy, recordItemsNew.LastUpdateDate);
                            var intItemsNew = ctx.Database.ExecuteSqlCommand(query);
                            recordItemsNew = null;

                            // Update onOrder for partSub
                            recordItemsSub.OnOrder -= newQty;
                            recordItemsSub.LastUpdateBy = CurrentUser.UserId;
                            recordItemsSub.LastUpdateDate = DateTime.Now;
                            query = string.Format(@"update {0}..spMstItems set OnOrder = {4} , LastUpdateBy ='{5}', LastUpdateDate ='{6}' where CompanyCode = '{1}' and BranchCode = '{2}' and PartNo = '{3}'", GetDbMD(), CompanyMD, BranchMD, recordItemsSub.PartNo, recordItemsSub.OnOrder, recordItemsSub.LastUpdateBy, recordItemsSub.LastUpdateDate);
                            var intItemsSub = ctx.Database.ExecuteSqlCommand(query);
                            recordItemsSub = null;

                            result = intItemsNew > 0 &&
                              intItemsSub > 0;

                        }
                        else
                        {
                            throw new Exception("Part No Tidak ditemukan, Proses update On Order di Item Master Gagal");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Proses Simpan Maintenace Order Substitusi Gagal. Error: " + ex.Message);
            }
            return result;
        }

        public bool p_SaveHistoryBalance(spTrnPOrderBalance oTrnPOrderBalance)
        {
            bool result = false;
            var lastUpdateDate = Convert.ToDateTime(oTrnPOrderBalance.LastUpdateDate).AddSeconds(10);
            SpHstPOrderBalance temp = ctx.SpHstPOrderBalances.Find(CompanyCode, BranchCode, oTrnPOrderBalance.POSNo, lastUpdateDate); ;
            bool isNew = false;
            if (temp == null)
            {
                isNew = true;
                temp = new SpHstPOrderBalance();
                temp.CompanyCode = oTrnPOrderBalance.CompanyCode;
                temp.BranchCode = oTrnPOrderBalance.BranchCode;
                temp.PONo = oTrnPOrderBalance.POSNo;
                temp.NewOrderQty = decimal.Zero;
            }
            temp.OldPartNo = oTrnPOrderBalance.PartNoOriginal;
            temp.OldOrderQty = Convert.ToDecimal(temp.NewOrderQty);
            temp.NewPartNo = oTrnPOrderBalance.PartNo;
            temp.NewOrderQty = oTrnPOrderBalance.OrderQty ?? 0;
            temp.PurchasePrice = oTrnPOrderBalance.PurchasePrice ?? 0;
            temp.Discount = oTrnPOrderBalance.DiscPct ?? 0;
            temp.CostPrice = oTrnPOrderBalance.CostPrice ?? 0;
            temp.LastUpdateBy = oTrnPOrderBalance.LastUpdateBy;
            temp.LastUpdateDate = DateTime.Now.AddSeconds(10);

            if (isNew)
            {
                Helpers.ReplaceNullable(temp);
                ctx.SpHstPOrderBalances.Add(temp);
                result = ctx.SaveChanges() > 0;
                if (!result)
                {
                    throw new Exception("Insert Data Order Pembelian Gagal");
                }
            }
            else
            {
                result = ctx.SaveChanges() == 0;
                if (!result)
                {
                    throw new Exception("Update Data Order Pembelian Gagal");
                }
            }

            return result;
        }

        public decimal p_GetNewSeqNo(string posNo)
        {
            var lastPO = ctx.spTrnPOrderBalances.Where(p => p.CompanyCode == CompanyCode &&
                p.BranchCode == BranchCode && p.POSNo == posNo).OrderByDescending(p => p.SeqNo).FirstOrDefault();

            return (lastPO == null) ? 1 : lastPO.SeqNo + 1;
        }
        #endregion
    }
}