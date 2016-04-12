using SimDms.Sparepart.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.BLL
{
    public class SpHstPOrderBalanceBLL : BaseBLL
    {
        #region "Initiate"
        /// <summary>
        /// 
        /// </summary>
        private static SpHstPOrderBalanceBLL _SpHstPOrderBalanceBLL;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_username"></param>
        /// <returns></returns>
        public static SpHstPOrderBalanceBLL Instance(string _username)
        {
            if (_SpHstPOrderBalanceBLL == null)
            {
                _SpHstPOrderBalanceBLL = new SpHstPOrderBalanceBLL();
            }
            //if (string.IsNullOrEmpty(username))
            //{
                username = _username;
            //}
            return _SpHstPOrderBalanceBLL;
        }
        #endregion

        /// <summary>
        /// Get SpHstPOrderBalance Data Record
        /// </summary>
        /// <param name="pONo"></param>
        /// <param name="lastUpdateDate"></param>
        /// <returns>SpHstPOrderBalance record</returns>
        public SpHstPOrderBalance GetRecord(string pONo, DateTime lastUpdateDate)
        {
            var record = ctx.SpHstPOrderBalances.Find(CompanyCode, BranchCode, pONo, lastUpdateDate);

            return (record == null) ? null : record;
        }

        /// <summary>
        /// Insert or Update SpHstPOrderBalance Data 
        /// </summary>
        /// <param name="oTrnPOrderBalance"></param>
        /// <param name="infoForm"></param>
        /// <returns>If success then true else false</returns>
        public bool Save(spTrnPOrderBalance oTrnPOrderBalance, ref XLogger infoForm)
        {
            //SpHstPOrderBalance record = new SpHstPOrderBalance();
            //record.CompanyCode = oTrnPOrderBalance.CompanyCode;
            //record.BranchCode = oTrnPOrderBalance.BranchCode;
            //record.PONo = oTrnPOrderBalance.POSNo;
            //record.LastUpdateDate = DmsTime.Now;
            //record.OldPartNo = oTrnPOrderBalance.PartNoOriginal;
            //record.OldOrderQty = 
            bool result = false;
            var lastUpdateDate = Convert.ToDateTime(oTrnPOrderBalance.LastUpdateDate).AddSeconds(10);
            SpHstPOrderBalance temp = this.GetRecord(oTrnPOrderBalance.POSNo, lastUpdateDate);
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
                result = this.Insert(temp) > 0;
                if (!result)
                {
                    infoForm.AddMessage(LoggerType.Info, "Insert Data Order Pembelian Gagal");
                    return result;
                }
            }
            else
            {
                result = this.Update() >= 0;
                if (!result)
                {
                    infoForm.AddMessage(LoggerType.Info, "Update Data Order Pembelian Gagal");
                    return result;
                }
            }
            return result;
        }

        /// <summary>
        /// Insert SpHstPOrderBalance Data
        /// </summary>
        /// <param name="record"></param>
        /// <returns>Int savechange data</returns>
        public int Insert(SpHstPOrderBalance record)
        {
            ctx.SpHstPOrderBalances.Add(record);
            
            return ctx.SaveChanges();
        }

        /// <summary>
        /// Insert SpHstPOrderBalance Data
        /// </summary>
        /// <returns>Int savechange</returns>
        public int Update()
        {
            return ctx.SaveChanges();
        }
    }
}
