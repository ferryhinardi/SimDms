using SimDms.Common;
using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Sales.BLL
{
    public class OmTrStockTakingHdrBLL : BaseBLL
    {
        #region -- Initiate --
        public OmTrStockTakingHdrBLL(DataContext _ctx, string _username)
        {
            this.ctx = _ctx;

            //if (string.IsNullOrEmpty(username))
            //{
                username = _username;
            //}
        }
        #endregion

        #region -- Public Method --
        public bool Posting(string stNoFrom, string stNoTo)
        {
            bool result = false;
            var records = ctx.omTrStockTakingHdrs.ToList()
                .Where(p => p.CompanyCode == CompanyCode
                && p.BranchCode == BranchCode && 
                (p.STHdrNo.CompareTo(stNoFrom) > -1 && p.STHdrNo.CompareTo(stNoTo) < 1));

            foreach (omTrStockTakingHdr rec in records)
            {
                rec.Status = "2";
                rec.LastUpdateBy = username;
                rec.LastUpdateDate = ctx.CurrentTime;

                Helpers.ReplaceNullable(rec);
            }
            records = null;
            return result = ctx.SaveChanges() >= 0;
        }
        #endregion
    }
}