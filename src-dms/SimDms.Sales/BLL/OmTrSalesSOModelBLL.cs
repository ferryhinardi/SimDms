using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Sales.BLL
{
    public class OmTrSalesSOModelBLL : BaseBLL
    {
        #region -- Initiate --
        public OmTrSalesSOModelBLL (DataContext _ctx, string _username)
        {
            this.ctx = _ctx;

            //if (string.IsNullOrEmpty(username))
            //{
                username = _username;
            //}
        }
        #endregion

        #region -- Public Method --
        public List<OmTrSalesSOModel> Select4Table(string SONo)
        {
            var records = ctx.OmTrSalesSOModels.Where(p => p.CompanyCode == CompanyCode
                && p.BranchCode == BranchCode && p.SONo == SONo).ToList();

            return records;
        }

        public List<OmTrSalesSOModel> Select4Table()
        {
            var records = ctx.OmTrSalesSOModels.Where(p => p.CompanyCode == CompanyCode
                            && p.BranchCode == BranchCode).ToList();

            return records;
        }

        #endregion
    }
}