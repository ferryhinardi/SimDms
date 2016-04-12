using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.Text;
using System.Data;

using SimDms.Sparepart.Models;
using SimDms.Common;
using SimDms.Common.Models;

namespace SimDms.Sparepart.Controllers.Api
{
    public class MaintenanceOrderPoController : BaseController
    {
        #region -- CRUD JsonResult Public method --
        public JsonResult Save(spTrnPOrderBalance record)
        {
            var rcdOrderBalance = ctx.spTrnPOrderBalances.Find(CompanyCode, BranchCode, record.POSNo, record.SupplierCode, record.PartNo, record.SeqNo);
            using (var tranScope = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {

                    bool bolSuccsess = true;
                    string strMessage = string.Empty;

                    if (rcdOrderBalance == null)
                    {
                        strMessage = "Data PO tidak ditemukan.";
                        bolSuccsess = false;
                    }

                    if (bolSuccsess)
                    {
                        bolSuccsess = p_Update(rcdOrderBalance, record.OnOrder ?? 0);
                        if (bolSuccsess)
                        {
                            strMessage = ctx.SysMsgs.Find(SysMessages.MSG_1100).MessageCaption;
                            tranScope.Commit();
                        }
                        else
                        {
                            tranScope.Rollback();
                            strMessage = string.Format(ctx.SysMsgs.Find(SysMessages.MSG_5039).MessageCaption, "ubah jumlah order", "");
                        }
                    }

                    return Json(new { success = bolSuccsess, message = strMessage });
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
        private bool p_Update(spTrnPOrderBalance record, decimal newQtyOrder)
        {
            var oldOrderQty = record.OnOrder ?? 0;
            int intTrn = 0;
            int intHst = 0;
            bool result = false;
            SpHstPOrderBalance historyRecord = new SpHstPOrderBalance();
            record.OnOrder = newQtyOrder;
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;
            intTrn = ctx.SaveChanges();

            historyRecord.BranchCode = record.BranchCode;
            historyRecord.CompanyCode = record.CompanyCode;
            historyRecord.PONo = record.POSNo;
            historyRecord.OldPartNo = record.PartNo;
            historyRecord.OldOrderQty = oldOrderQty;
            historyRecord.NewPartNo = record.PartNo;
            historyRecord.NewOrderQty = newQtyOrder;
            historyRecord.PurchasePrice = record.PurchasePrice ?? 0;
            historyRecord.Discount = record.DiscPct ?? 0;
            historyRecord.CostPrice = record.CostPrice ?? 0;
            historyRecord.LastUpdateBy = record.LastUpdateBy;
            historyRecord.LastUpdateDate = DateTime.Now;
            ctx.SpHstPOrderBalances.Add(historyRecord);
            
            Helpers.ReplaceNullable(historyRecord);

            intHst = ctx.SaveChanges();

            result = intTrn > 0 && intHst > 0;
            if (result)
            {
                decimal oldQtyOrder = historyRecord.OldOrderQty;
                result = p_UpdateSpMstItems(record, oldQtyOrder, newQtyOrder);
            }

            return result;
        }

        private bool p_UpdateSpMstItems(spTrnPOrderBalance record, decimal oldQtyOrder, decimal newQtyOrder)
        {
            var Item = @"select * from " + GetDbMD() + @"..spMstItems where CompanyCode='" + CompanyMD + "' AND BranchCode='" + BranchMD + "' and PartNo ='" + record.PartNo + "'";
            var recordItem = ctx.Database.SqlQuery<spMstItem>(Item).FirstOrDefault();

            recordItem.OnOrder = recordItem.OnOrder - (oldQtyOrder - newQtyOrder);
            recordItem.LastUpdateBy = record.LastUpdateBy;
            recordItem.LastUpdateDate = DateTime.Now;

            // Update Items
            var query = string.Format(@"update {0}..spMstItems set OnOrder = {4} , LastUpdateBy ='{5}', LastUpdateDate ='{6}' where CompanyCode = '{1}' and BranchCode = '{2}' and PartNo = '{3}'",
                GetDbMD(), CompanyMD, BranchMD, record.PartNo, recordItem.OnOrder, recordItem.LastUpdateBy, recordItem.LastUpdateDate);
            var intItemsUpdate = ctx.Database.ExecuteSqlCommand(query);
            recordItem = null;

            return intItemsUpdate > 0;
        }
    #endregion
    }
}
