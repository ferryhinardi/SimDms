using SimDms.Sparepart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.BLL
{
    public class SpMstItemLocBLL : BaseBLL
    {
        #region "Initiate"
        /// <summary>
        /// 
        /// </summary>
        private static SpMstItemLocBLL _SpMstItemLocBLL;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_username"></param>
        /// <returns></returns>
        public static SpMstItemLocBLL Instance(string _username)
        {
            if (_SpMstItemLocBLL == null)
            {
                _SpMstItemLocBLL = new SpMstItemLocBLL();
            }
            //if (string.IsNullOrEmpty(username))
            //{
                username = _username;
            //}
            return _SpMstItemLocBLL;
        }
        #endregion

        /// <summary>
        /// Get SpMstItemLoc data record 
        /// </summary>
        /// <param name="partNo"></param>
        /// <param name="warehouseCode"></param>
        /// <returns>SpMstItemLoc record</returns>
        public SpMstItemLoc GetRecord(DataContext _ctx, string partNo, string warehouseCode)
        {
            var query = string.Format(@"select * from {0}..SpMstItemLoc where CompanyCode ='{1}' and BranchCode ='{2}' and PartNo = '{3}' and WarehouseCode = '{4}'", GetDbMD(), CompanyMD, BranchMD, partNo, warehouseCode);
            var record = _ctx.Database.SqlQuery<SpMstItemLoc>(query).FirstOrDefault();

            return record;
        }

        public bool UpdateStock(DataContext _ctx, string partno, string whcode, decimal? onhand, int allocation, int backorder, int reserved, string salesType)
        {
            if (onhand == null)
            {
                onhand = 0;
            }
            bool result = false;
            
            var query = string.Format(@"select * from {0}..spMstItems where CompanyCode = '{1}' and BranchCode = '{2}' and PartNo = '{3}'", GetDbMD(), CompanyMD, BranchMD, partno);
            var oItem = _ctx.Database.SqlQuery<spMstItem>(query).FirstOrDefault();

            if (oItem != null)
            {
                //TODO : Tambahkan check result untuk yang hasilnya negatif
                var oItemLoc = GetRecord(_ctx, partno,whcode);
                    
                    //_ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, partno, whcode);
             

                if (oItemLoc != null)
                {
                    if (Math.Abs((decimal)onhand) > 0)
                    {
                        oItemLoc.OnHand += onhand;
                        oItem.OnHand += onhand;

                        // Tambahkan check result untuk yang ItemLoc negatif
                        if (oItemLoc.OnHand < 0)
                        {
                            throw new Exception(string.Format("OnHand untuk Part = {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.OnHand));
                        }
                        // Tambahkan check result untuk yang Item negatif
                        if (oItem.OnHand < 0)
                        {

                            throw new Exception(string.Format("OnHand untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.OnHand));

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
                                throw new Exception(string.Format("AllocationSP untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.AllocationSP));
                            }

                            // Tambahkan check result untuk yang Item negatif
                            if (oItem.AllocationSP < 0)
                            {
                                throw new Exception(string.Format("AllocationSP untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItem.AllocationSP));
                            }
                        }

                        if (!string.IsNullOrEmpty(salesType) && salesType == "2")
                        {
                            oItemLoc.AllocationSR += allocation;
                            oItem.AllocationSR += allocation;

                            // Tambahkan check result untuk yang ItemLoc negatif
                            if (oItemLoc.AllocationSR < 0)
                            {
                                throw new Exception(string.Format("AllocationSR untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.AllocationSR));
                            }

                            // Tambahkan check result untuk yang Item negatif
                            if (oItem.AllocationSR < 0)
                            {
                                throw new Exception(string.Format("AllocationSR untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItem.AllocationSR));

                            }
                        }

                        if (!string.IsNullOrEmpty(salesType) && salesType == "3")
                        {
                            oItemLoc.AllocationSL += allocation;
                            oItem.AllocationSL += allocation;

                            // Tambahkan check result untuk yang ItemLoc negatif
                            if (oItemLoc.AllocationSL < 0)
                            {
                                throw new Exception(string.Format("AllocationSL untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.AllocationSL));
                            }

                            // Tambahkan check result untuk yang Item negatif
                            if (oItem.AllocationSL < 0)
                            {
                                throw new Exception(string.Format("AllocationSL untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItem.AllocationSL));
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
                                throw new Exception(string.Format("BackOrderSP untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.BackOrderSP));
                            }

                            // Tambahkan check result untuk yang Item negatif
                            if (oItem.BackOrderSP < 0)
                            {
                                throw new Exception(string.Format("BackOrderSP untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.BackOrderSP));
                            }
                        }

                        if (!string.IsNullOrEmpty(salesType) && salesType == "2")
                        {
                            oItemLoc.BackOrderSR += backorder;
                            oItem.BackOrderSR += backorder;

                            // Tambahkan check result untuk yang ItemLoc negatif
                            if (oItemLoc.BackOrderSR < 0)
                            {
                                throw new Exception(string.Format("BackOrderSR untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.BackOrderSR));
                            }

                            // Tambahkan check result untuk yang Item negatif
                            if (oItem.BackOrderSR < 0)
                            {
                                throw new Exception(string.Format("BackOrderSR untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.BackOrderSR));
                            }
                        }

                        if (!string.IsNullOrEmpty(salesType) && salesType == "3")
                        {
                            oItemLoc.BackOrderSL += backorder;
                            oItem.BackOrderSL += backorder;

                            // Tambahkan check result untuk yang ItemLoc negatif
                            if (oItemLoc.BackOrderSL < 0)
                            {
                                throw new Exception(string.Format("BackOrderSL untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.BackOrderSL));
                            }

                            // Tambahkan check result untuk yang Item negatif
                            if (oItem.BackOrderSL < 0)
                            {
                                throw new Exception(string.Format("BackOrderSL untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.BackOrderSL));
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
                            throw new Exception(string.Format("ReservedSP untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.ReservedSP));
                        }

                        // Tambahkan check result untuk yang Item negatif
                        if (oItem.ReservedSP < 0)
                        {
                            throw new Exception(string.Format("ReservedSP untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.ReservedSP));
                        }
                    }
                   
                    oItemLoc.LastUpdateDate = DateTime.Now;
                    oItemLoc.LastUpdateBy = CurrentUser.UserId;
                    oItem.LastUpdateDate = DateTime.Now;
                    oItem.LastUpdateBy = CurrentUser.UserId;

                    var setUpdateItemLoc = string.Format(@"UPDATE {0}..SpMstItemLoc
                    SET OnHand = {1}, AllocationSP = {2}, AllocationSR = {3}, AllocationSL = {4},
                    BackOrderSP = {5}, BackOrderSR = {6}, BackOrderSL = {7}, ReservedSP = {8}, LastUpdateDate = '{9}', LastUpdateBy = '{10}'
                    WHERE CompanyCode ='{11}' and BranchCode ='{12}' and PartNo = '{13}' and WarehouseCode = '{14}'",
                    GetDbMD(), oItemLoc.OnHand, oItemLoc.AllocationSP, oItemLoc.AllocationSR, oItemLoc.AllocationSL, oItemLoc.BackOrderSP, oItemLoc.BackOrderSR, oItemLoc.BackOrderSL, oItemLoc.ReservedSP,
                    DateTime.Now, CurrentUser.UserId, CompanyMD, BranchMD, partno, WarehouseMD);
                    
                    var setUpdateItems = string.Format(@"update {0}..spMstItems
                    SET OnHand = {1}, AllocationSP = {2}, AllocationSR = {3}, AllocationSL = {4},
                    BackOrderSP = {5}, BackOrderSR = {6}, BackOrderSL = {7},  ReservedSP = {8}, LastUpdateDate = '{9}', LastUpdateBy = '{10}'
                    WHERE CompanyCode ='{11}' and BranchCode ='{12}' and PartNo = '{13}'",
                    GetDbMD(), oItem.OnHand, oItem.AllocationSP, oItem.AllocationSR, oItem.AllocationSL, oItem.BackOrderSP, oItem.BackOrderSR, oItem.BackOrderSL, oItem.ReservedSP,
                    DateTime.Now, CurrentUser.UserId, CompanyMD, BranchMD, partno);

                    try
                    {
                        _ctx.Database.ExecuteSqlCommand(setUpdateItemLoc);
                        _ctx.Database.ExecuteSqlCommand(setUpdateItems);
                        //_ctx.SaveChanges();
                        //_ctx.spMstItems.Add(oItem);
                        //_ctx.SpMstItemLocs.Add(oItemLoc);
                        result = true;
                    }
                    catch
                    {
                        result = false;
                    }
                }
            }
            return result;
        }

        public bool UpdateStock(DataContext _ctx, string partno, int borrowed, decimal? borrow)
        {
            if (borrow == null)
            {
                borrow = 0;
            }
            bool result = false;
            
            //var oItem = ctx.spMstItems.Find(CompanyCode, BranchCode, partno);

            var query = string.Format(@"select * from {0}..spMstItems where CompanyCode = '{1}' and BranchCode = '{2}' and PartNo = '{3}'", GetDbMD(), CompanyMD, BranchMD, partno);
            var oItem = _ctx.Database.SqlQuery<spMstItem>(query).FirstOrDefault();

            if (oItem != null)
            {
                if (Math.Abs(borrowed) > 0)
                {
                    oItem.BorrowedQty += borrowed;
                    // Tambahkan check result untuk yang ItemLoc negatif
                    if (oItem.BorrowedQty < 0)
                    {
                        throw new Exception(string.Format("BorrowedQty Item = {0}, transaksi tidak bisa lanjut", oItem.BorrowedQty));
                    }
                }

                if (Math.Abs((decimal)borrow) > 0)
                {
                    oItem.BorrowQty += borrow;

                    // Tambahkan check result untuk yang Item negatif
                    if (oItem.BorrowQty < 0)
                    {
                        throw new Exception(string.Format("BorrowQty Item = {0}, transaksi tidak bisa lanjut", oItem.BorrowQty));
                    }
                }

                oItem.LastUpdateDate = DateTime.Now;
                oItem.LastUpdateBy = CurrentUser.UserId;

                var setUpdate = string.Format(@"UPDATE {0}..spMstItems 
                SET BorrowedQty = {1}, BorrowQty = {2}, LastUpdateDate = '{3}', LastUpdateBy = '{4}'
                WHERE CompanyCode = '{5}' and BranchCode = '{6}' and PartNo = '{7}'"
                , GetDbMD(), oItem.BorrowedQty, oItem.BorrowQty, DateTime.Now, CurrentUser.UserId, CompanyMD, BranchMD, partno);

                try
                {
                    _ctx.Database.ExecuteSqlCommand(setUpdate);
                    //ctx.SaveChanges();
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
