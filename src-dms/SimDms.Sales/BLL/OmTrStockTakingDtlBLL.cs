using SimDms.Common;
using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Sales.BLL
{
    public class OmTrStockTakingDtlBLL : BaseBLL
    {
        #region -- Initiate --
        public OmTrStockTakingDtlBLL(DataContext _ctx, string _username)
        {
            this.ctx = _ctx;

            //if (string.IsNullOrEmpty(username))
            //{
                username = _username;
            //}
        }
        #endregion

        #region -- Public Method --
        public IEnumerable<dynamic> Select4View(string STHdrNo)
        {
            var records = ctx.omTrStockTakingDtls.Where(p => p.CompanyCode == CompanyCode
                && p.BranchCode == BranchCode && p.STHdrNo == STHdrNo)
                .ToList().Select(p => new
                {
                    Status = ((p.Status == null) ? 0 : Convert.ToInt32(p.Status)) == 1 ? 1 : 0,
                    p.STNo,
                    p.WareHouseCode,
                    p.SalesModelCode,
                    p.SalesModelYear,
                    p.ColourCode,
                    p.ChassisCode,
                    p.ChassisNo,
                    p.EngineCode,
                    p.EngineNo
                });

            return records;
        }

        public bool UpdateStatus(string stHdrNo, int stNo, int status)
        {
            var record = ctx.omTrStockTakingDtls.Find(CompanyCode, BranchCode, stHdrNo, stNo);
            if (record != null)
            {
                record.Status = status.ToString();
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = DateTime.Now;
            }
            Helpers.ReplaceNullable(record);

            return ctx.SaveChanges() >= 0;
        }
        #endregion
    }
}