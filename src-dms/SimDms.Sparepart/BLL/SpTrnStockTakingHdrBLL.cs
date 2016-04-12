using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimDms.Sparepart.Models;

namespace SimDms.Sparepart.BLL
{
    /// <summary>
    /// 
    /// </summary>
    public class SpTrnStockTakingHdrBLL : BaseBLL
    {
        #region "Initiate"
        /// <summary>
        /// 
        /// </summary>
        private static SpTrnStockTakingHdrBLL _SpTrnStockTakingHdrBLL;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_username"></param>
        /// <returns></returns>
        public static SpTrnStockTakingHdrBLL Instance(string _username)
        {
            if (_SpTrnStockTakingHdrBLL == null)
            {
                _SpTrnStockTakingHdrBLL = new SpTrnStockTakingHdrBLL();
            }
            if (string.IsNullOrEmpty(username))
            {
                username = _username;
            }
            return _SpTrnStockTakingHdrBLL;
        }
        #endregion

        /// <summary>
        /// Get spTrnStockTakingHdr Data Record
        /// </summary>
        /// <param name="newPartNo"></param>
        /// <returns>spMstItem record</returns>
        public SpTrnStockTakingHdr GetRecord(string STHDrNo){
            var record = ctx.SpTrnStockTakingHdrs.Find(CompanyCode, BranchCode, STHDrNo);

            return record;
        }

        /// <summary>
        /// Update spTrnStockTakingHdr data
        /// </summary>
        /// <returns>Int savechange</returns>
        public int Update()
        {
            return ctx.SaveChanges();
        }
    }
}
