using SimDms.Common;
using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Sales.BLL
{
    public class OmTrSalesDraftSOModelColourBLL : BaseBLL
    {
        #region -- Initiate --

        public OmTrSalesDraftSOModelColourBLL(DataContext _ctx, string _username)
        {
            ctx = _ctx;
            //if (string.IsNullOrEmpty(username))
            //{
                username = _username;
            //}
        }
        #endregion

        #region -- Public Method --
        public List<OmTrSalesDraftSOModelColour> Select4Table(string draftSONo, string salesModelCode, decimal salesModelYear)
        {
            var records = ctx.OmTrSalesDraftSOModelColours.Where(p => p.CompanyCode == CompanyCode
                && p.BranchCode == BranchCode && p.DraftSONo == draftSONo && p.SalesModelCode == salesModelCode
                && p.SalesModelYear == salesModelYear).ToList();
            
            return records;
        }

        public OmTrSalesDraftSOModelColour GetRecord(string draftSONo, string salesModelCode, Decimal salesModelYear, string colourCode)
        {
            var record = ctx.OmTrSalesDraftSOModelColours.Find(CompanyCode, BranchCode, draftSONo, salesModelCode, salesModelYear, colourCode);
            
            return record;
        }

        public bool SaveDetailModelColour(OmTrSalesDraftSOModelBLL omTrSalesDraftSOModelBLL, OmTrSalesDraftSOModelColour recordModelColour, bool isNew)
        {
            bool result = false;
            try
            {
                if (isNew)
                {
                    ctx.OmTrSalesDraftSOModelColours.Add(recordModelColour);
                    Helpers.ReplaceNullable(recordModelColour);
                    result = ctx.SaveChanges() >= 0;
                }
                else
                {
                    Helpers.ReplaceNullable(recordModelColour);
                    result = ctx.SaveChanges() >= 0;
                }

                if (result)
                {
                    var salesModelYear = recordModelColour.SalesModelYear ?? 0; 
                    if (!p_UpdateDraftSOModel(omTrSalesDraftSOModelBLL, recordModelColour.DraftSONo,
                        recordModelColour.SalesModelCode, salesModelYear))
                        result = false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
            return result;
        }
        #endregion

        #region -- Private Method --
        private bool p_UpdateDraftSOModel(OmTrSalesDraftSOModelBLL omTrSalesDraftSOModelBLL, string draftSONo, string salesModelCode, decimal salesModelYear)
        {
            bool result = false;
            OmTrSalesDraftSOModel model = omTrSalesDraftSOModelBLL.GetRecord(draftSONo, salesModelCode, salesModelYear);

            model.QuantityDraftSO = p_Select4TotQty(draftSONo, salesModelCode, salesModelYear);
            model.LastUpdateBy = username;
            model.LastUpdateDate = DateTime.Now;

            Helpers.ReplaceNullable(model);
            if (ctx.SaveChanges() >= 0)
                result = true;
            return result;
        }

        private decimal p_Select4TotQty(string draftSONo, string salesModelCode, decimal salesModelYear)
        {
            var records = ctx.OmTrSalesDraftSOModelColours.Where(p => p.CompanyCode == CompanyCode
                && p.BranchCode == BranchCode && p.DraftSONo == draftSONo && p.SalesModelCode == salesModelCode
                && p.SalesModelYear == salesModelYear);
            
            decimal qty = 0;
            records.ToList();
            if(records != null) {
                qty = records.Sum(p => p.Quantity) ?? 0;
            }

            return qty;
        }

        #endregion

    }
}