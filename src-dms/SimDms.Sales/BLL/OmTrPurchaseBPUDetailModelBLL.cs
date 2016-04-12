using SimDms.Common;
using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Sales.BLL
{
    public class OmTrPurchaseBPUDetailModelBLL : BaseBLL
    {
        #region -- Initiate --
        public OmTrPurchaseBPUDetailModelBLL(DataContext _ctx, string _username)
        {
            this.ctx = _ctx;

            //if (string.IsNullOrEmpty(username))
            //{
                username = _username;
            //}
        }
        #endregion

        #region -- Public Method --
        public OmTrPurchaseBPUDetailModel GetRecord(string pONo, string bPUNo, string salesModelCode, Decimal salesModelYear)
        {
            return ctx.OmTrPurchaseBPUDetailModels.Find(CompanyCode, BranchCode, pONo, bPUNo, salesModelCode, salesModelYear);
        }

        public int Insert(OmTrPurchaseBPUDetailModel record)
        {
            ctx.OmTrPurchaseBPUDetailModels.Add(record);

            Helpers.ReplaceNullable(record);
            return ctx.SaveChanges();
        }

        public int Update()
        {
            return ctx.SaveChanges();
        }
        #endregion
    }
}