using SimDms.Sparepart.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.BLL
{
    public class SpTrnPOrderBalanceBLL : BaseBLL
    {
        #region "Initiate"
        /// <summary>
        /// 
        /// </summary>
        private static SpTrnPOrderBalanceBLL _SpTrnPOrderBalanceBLL;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_username"></param>
        /// <returns></returns>
        public static SpTrnPOrderBalanceBLL Instance(string _username)
        {
            if (_SpTrnPOrderBalanceBLL == null)
            {
                _SpTrnPOrderBalanceBLL = new SpTrnPOrderBalanceBLL();
            }
            if (string.IsNullOrEmpty(username))
            {
                username = _username;
            }
            return _SpTrnPOrderBalanceBLL;
        }
        #endregion
        
        /// <summary>
        /// Get spTrnPOrderBalance data record
        /// </summary>
        /// <param name="posNo"></param>
        /// <param name="supplierCode"></param>
        /// <param name="partNo"></param>
        /// <param name="seqNo"></param>
        /// <returns>spTrnPOrderBalance record</returns>
        public  spTrnPOrderBalance GetRecord(string posNo, string supplierCode, string partNo, Decimal seqNo)
        {
            var record = ctx.spTrnPOrderBalances.Find(CompanyCode, BranchCode,
                                                posNo, supplierCode,
                                                partNo, seqNo);

            return record;
        }

        /// <summary>
        /// Get new Sequence Number spTrnPOrderBalance
        /// </summary>
        /// <param name="posNo"></param>
        /// <returns>decimal new Sequence Number</returns>
        public decimal GetNewSeqNo(string posNo)
        {
            var lastPO = ctx.spTrnPOrderBalances.Where(p => p.CompanyCode == CompanyCode &&
                p.BranchCode == BranchCode && p.POSNo == posNo).OrderByDescending(p => p.SeqNo).FirstOrDefault();

            return (lastPO == null) ? 1 : lastPO.SeqNo + 1;
        }
        
        /// <summary>
        /// Get distinct POSNo from spTrnPOrderBalance.
        /// </summary>
        /// <param name="PosNo"></param>
        /// <returns>DataTable</returns>
        public DataTable Select4Lookup(string posNo)
        {
            DataTable dt = new DataTable();
            string cmdText = @"
                SELECT 
                    DISTINCT a.POSNo
                FROM 
                    spTrnPOrderBalance a 
                INNER JOIN gnMstSupplier b 
                   ON b.SupplierCode = a.SupplierCode 
                  AND b.CompanyCode  = a.CompanyCode 
                WHERE a.OrderQty > a.Received
                  AND a.CompanyCode = @CompanyCode
                  AND a.BranchCode  = @BranchCode
                  AND a.TypeOfGoods = @TypeOfGoods
                  AND a.POSNo       = @POSNo
                ORDER BY POSNo DESC";

            SqlConnection sqlCon = (SqlConnection)ctx.Database.Connection;
            SqlCommand sqlCmd = sqlCon.CreateCommand();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = cmdText;
            sqlCmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            sqlCmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            sqlCmd.Parameters.AddWithValue("@TypeOfGoods", CurrentUser.TypeOfGoods);
            sqlCmd.Parameters.AddWithValue("@POSNo", posNo);
            SqlDataAdapter sqlDA = new SqlDataAdapter(sqlCmd);
            sqlDA.Fill(dt);
            
            return dt;
        }

        /// <summary>
        /// Get spTrnPOrderBalance data record(s)
        /// </summary>
        /// <param name="posNo"></param>
        /// <returns>DataTable spTrnPOrderBalance</returns>
        public DataTable Select4NoPart(string posNo)
        {
            DataTable dt = new DataTable();
            string cmdText = @"
               SELECT 
                    a.POSNo, a.PartNo, b.PartName, CAST(a.OrderQty as decimal(18,2)) as OrderQty, 
                    a.OnOrder, a.Intransit, a.Received,a.DiscPct, a.PurchasePrice, 
                    Convert(varchar(10),a.SeqNo) SeqNo, a.SupplierCode, a.OnOrder, a.PartNoOriginal, 
                    a.TypeOfGoods 
                FROM 
                    spTrnPOrderBalance a 
                INNER JOIN spMstItemInfo b
                   ON b.PartNo      = a.PartNo
                  AND b.CompanyCode = a.CompanyCode
                WHERE a.CompanyCode = @CompanyCode
                  AND a.BranchCode  = @BranchCode
                  AND a.PosNo    like @PosNo
                ORDER BY a.POSNo DESC, a.SeqNo";

            SqlConnection sqlCon = (SqlConnection)ctx.Database.Connection;
            SqlCommand sqlCmd = sqlCon.CreateCommand();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = cmdText;
            sqlCmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            sqlCmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            sqlCmd.Parameters.AddWithValue("@POSNo", posNo);
            SqlDataAdapter sqlDA = new SqlDataAdapter(sqlCmd);
            sqlDA.Fill(dt);

            return dt;
        }


        /// <summary>
        /// Get distinct POSNo and PartNo from spTrnPOrderBalance.
        /// </summary>
        /// <param name="posNo"></param>
        /// <param name="partNo"></param>
        /// <returns></returns>
        public DataTable Select4NoPart(string posNo, string partNo)
        {
            DataTable dt = new DataTable();
            string cmdText = @"
                SELECT 
                    a.POSNo, a.PartNo
                FROM 
                    spTrnPOrderBalance a 
                INNER JOIN spMstItemInfo b
                   ON b.PartNo      = a.PartNo
                  AND b.CompanyCode = a.CompanyCode
                WHERE a.CompanyCode = @CompanyCode
                  AND a.BranchCode  = @BranchCode
                  AND a.PosNo       = @PosNo
                  AND a.PartNo      = @PartNo
                ORDER BY a.POSNo DESC, a.SeqNo";

            SqlConnection sqlCon = (SqlConnection)ctx.Database.Connection;
            SqlCommand sqlCmd = sqlCon.CreateCommand();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = cmdText;
            sqlCmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            sqlCmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            sqlCmd.Parameters.AddWithValue("@POSNo", posNo);
            sqlCmd.Parameters.AddWithValue("@PartNo", partNo);
            SqlDataAdapter sqlDA = new SqlDataAdapter(sqlCmd);
            sqlDA.Fill(dt);

            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recSpTrnPOrderBalance"></param>
        /// <param name="newPartNo"></param>
        /// <param name="newQty"></param>
        /// <param name="infoForm"></param>
        /// <returns></returns>
        public bool Save(spTrnPOrderBalance recSpTrnPOrderBalance, string newPartNo, decimal newQty, ref XLogger infoForm)
        {
            bool result = false;
            spMstItem recordItemsNew = new spMstItem();
            spMstItem recordItemsSub = new spMstItem();
            spMstItemPrice recordPriceNew = new spMstItemPrice();

            SpMstItemsBLL spMstItemsBLL = SpMstItemsBLL.Instance(CurrentUser.UserId);
            SpMstItemPriceBLL spMstItemPriceBLL = SpMstItemPriceBLL.Instance(CurrentUser.UserId);
            try
            {
                this.GetRecord(recSpTrnPOrderBalance.POSNo, recSpTrnPOrderBalance.SupplierCode, recSpTrnPOrderBalance.PartNo, recSpTrnPOrderBalance.SeqNo);
                recSpTrnPOrderBalance.OnOrder = recSpTrnPOrderBalance.OnOrder - newQty;
                recordItemsNew = spMstItemsBLL.GetRecord(newPartNo);
                recordItemsSub = spMstItemsBLL.GetRecord(recSpTrnPOrderBalance.PartNo);
                recordPriceNew = spMstItemPriceBLL.GetRecord(newPartNo);
                spMstItemPriceBLL = null;

                result = this.Save(recSpTrnPOrderBalance, ref infoForm);
                if (!result)
                {
                    return result;
                }
                else
                {
                    spTrnPOrderBalance newRecord = new spTrnPOrderBalance();
                    newRecord.CompanyCode = recSpTrnPOrderBalance.CompanyCode;
                    newRecord.BranchCode = recSpTrnPOrderBalance.BranchCode;
                    newRecord.POSNo = recSpTrnPOrderBalance.POSNo;
                    newRecord.SupplierCode = recSpTrnPOrderBalance.SupplierCode;
                    newRecord.PartNo = newPartNo;
                    newRecord.SeqNo = GetNewSeqNo(recSpTrnPOrderBalance.POSNo);
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

                    result = this.Save(newRecord, ref infoForm);
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
                            var intItemsNew = spMstItemsBLL.Update();
                            recordItemsNew = null;

                            // Update onOrder for partSub
                            recordItemsSub.OnOrder -= newQty;
                            recordItemsSub.LastUpdateBy = CurrentUser.UserId;
                            recordItemsSub.LastUpdateDate = DateTime.Now;
                            var intItemsSub = spMstItemsBLL.Update();
                            recordItemsSub = null;
                            spMstItemsBLL = null;
                            
                            result = intItemsNew > 0 &&
                              intItemsSub > 0;

                        }
                        else
                        {
                            infoForm.AddMessage(LoggerType.Info, "Part No Tidak ditemukan, Proses update On Order di Item Master Gagal");
                            result = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                infoForm.AddMessage(LoggerType.Error, e.Message);
            }
            return result;
        }

        /// <summary>
        /// Insert or Update spTrnPOrderBalance data
        /// </summary>
        /// <param name="recSpTrnPOrderBalance"></param>
        /// <param name="inForm"></param>
        /// <returns>Int savechange data</returns>
        public bool Save(spTrnPOrderBalance recSpTrnPOrderBalance, ref XLogger inForm)
        {
            bool result = false;
            SpHstPOrderBalanceBLL spHstPOrderBalanceBLL = SpHstPOrderBalanceBLL.Instance(CurrentUser.UserId);
            spTrnPOrderBalance temp = this.GetRecord(recSpTrnPOrderBalance.POSNo, recSpTrnPOrderBalance.SupplierCode,
                                      recSpTrnPOrderBalance.PartNo, recSpTrnPOrderBalance.SeqNo);
            if (temp == null)
            {
                result = this.Insert(recSpTrnPOrderBalance) > 0;
                if (!result)
                {
                    inForm.AddMessage(LoggerType.Info,"Insert Data Order Pembelian Gagal");
                    return result;
                }
            }
            else
            {
                result = this.Update() > 0;
                if (!result)
                {
                    inForm.AddMessage(LoggerType.Info, "Update Data Order Pembelian Gagal");
                    return result;
                }
            }

            result = spHstPOrderBalanceBLL.Save(recSpTrnPOrderBalance, ref inForm);
            spHstPOrderBalanceBLL = null;
            if (!result)
            {
                inForm.AddMessage(LoggerType.Info, "Update Data Histori Order Pembelian Gagal");
                return result;
            }

            return result;
        }

        /// <summary>
        /// Insert spTrnPOrderBalance data
        /// </summary>
        /// <param name="record"></param>
        /// <returns>Int savechange data</returns>
        public int Insert(spTrnPOrderBalance record)
        {
            ctx.spTrnPOrderBalances.Add(record);
            return ctx.SaveChanges();
        }

        /// <summary>
        /// Update spTrnPOrderBalance Data
        /// </summary>
        /// <returns>Int savechange data</returns>
        public int Update()
        {
            return ctx.SaveChanges();
        }
    }
}
