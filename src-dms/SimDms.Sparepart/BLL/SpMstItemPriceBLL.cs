using SimDms.Sparepart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.BLL
{
    public class SpMstItemPriceBLL : BaseBLL
    {
        #region "Initiate"
        /// <summary>
        /// 
        /// </summary>
        private static SpMstItemPriceBLL _SpMstItemPriceBLL;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_username"></param>
        /// <returns></returns>
        public static SpMstItemPriceBLL Instance(string _username)
        {
            if (_SpMstItemPriceBLL == null)
            {
                _SpMstItemPriceBLL = new SpMstItemPriceBLL();
            }
            //if (string.IsNullOrEmpty(username))
            //{
                username = _username;
            //}
            return _SpMstItemPriceBLL;
        }
        #endregion

        /// <summary>
        /// Get spMstItemPrice Data Record
        /// </summary>
        /// <param name="partNo"></param>
        /// <returns>spMstItemPrice record</returns>
        public spMstItemPrice GetRecord(string partNo)
        {
            var record = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, partNo);

            return record;
        }
    }
}
