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
    public class SpTrnStockTakingTempBLL : BaseBLL
    {
        #region "Initiate"
        /// <summary>
        /// 
        /// </summary>
        private static SpTrnStockTakingTempBLL _SpTrnStockTakingTempBLL;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_username"></param>
        /// <returns></returns>
        public static SpTrnStockTakingTempBLL Instance(string _username)
        {
            if (_SpTrnStockTakingTempBLL == null)
            {
                _SpTrnStockTakingTempBLL = new SpTrnStockTakingTempBLL();
            }
            if (string.IsNullOrEmpty(username))
            {
                username = _username;
            }
            return _SpTrnStockTakingTempBLL;
        }
        #endregion

        /// <summary>
        /// Get spTrnStockTakingHdr Data Record
        /// </summary>
        /// <param name="newPartNo"></param>
        /// <returns>spMstItem record</returns>
        public SpTrnStockTakingTemp GetRecord(string STHDrNo, string STNo, decimal seqNo){
            var record = ctx.SpTrnStockTakingTemps.Find(CompanyCode, BranchCode, STHDrNo, STNo, seqNo);

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
