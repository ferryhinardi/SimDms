using SimDms.Common;
using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Sales.BLL
{
    public class OmTrSalesDraftSOModelOthersBLL : BaseBLL
    {
        #region -- Initiate --

        public OmTrSalesDraftSOModelOthersBLL(DataContext _ctx, string _username)
        {
            ctx = _ctx;
            //if (string.IsNullOrEmpty(username))
            //{
                username = _username;
            //}
        }
        #endregion

        #region -- Public Method --
        public List<OmTrSalesDraftSOModelOthersSelect4Table> Select4Table(string draftSONo, string salesModelCode, decimal salesModelYear)
        {
            var records = ctx.OmTrSalesDraftSOModelOthers.Where(p => p.CompanyCode == CompanyCode
                && p.BranchCode == BranchCode && p.DraftSONo == draftSONo && p.SalesModelCode == salesModelCode
                && p.SalesModelYear == salesModelYear).Select(
                p => new OmTrSalesDraftSOModelOthersSelect4Table()
                {
                    OtherCode = p.OtherCode,
                    AccsName = ctx.MstRefferences.Where(q => p.CompanyCode == q.CompanyCode && p.OtherCode == q.RefferenceCode
                        && q.RefferenceType == "OTHS").FirstOrDefault().RefferenceDesc1,
                    DPP = p.DPP,
                    PPn = p.PPn,
                    Remark = p.Remark,
                    BeforeDiscTotal = p.BeforeDiscTotal,
                    AfterDiscTotal = p.AfterDiscTotal,
                    AfterDiscDPP = p.AfterDiscDPP,
                    AfterDiscPPn = p.AfterDiscPPn
                }).ToList();

            return records;
        }

        public OmTrSalesDraftSOModelOthers GetRecord (string draftSONo, string salesModelCode, Decimal salesModelYear, string otherCode){
            var record = ctx.OmTrSalesDraftSOModelOthers.Find(CompanyCode, BranchCode, draftSONo, salesModelCode, salesModelYear, otherCode);

            return record;
        }

        public bool SaveAccOthers(OmTrSalesDraftSO record, OmTrSalesDraftSOModelOthers recordOthers, bool isNew)
        {
            bool result = false;
            try
            {
                if (isNew)
                {
                    ctx.OmTrSalesDraftSOModelOthers.Add(recordOthers);
                    Helpers.ReplaceNullable(recordOthers);
                    result =  ctx.SaveChanges() > 0;
                }
                else
                {
                    Helpers.ReplaceNullable(record);
                    Helpers.ReplaceNullable(recordOthers);
                    result = ctx.SaveChanges() >= 0;
                }
                if (result)
                {
                    decimal salesModelYear = recordOthers.SalesModelYear ?? 0;
                    if (!p_UpdateSOModel(record.CompanyCode, record.BranchCode, record.DraftSONo, recordOthers.SalesModelCode, salesModelYear))
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
            return result;
        }

        #endregion

        #region -- Private Methode -- 
        private bool p_UpdateSOModel(string companyCode, string branchCode, string draftSONo
            , string salesModelCode, decimal salesModelYear)
        {
            bool result = false;
            var omTrSalesDraftSOModelBLL = new OmTrSalesDraftSOModelBLL(ctx, username);
            OmTrSalesDraftSOModel model = omTrSalesDraftSOModelBLL.GetRecord(draftSONo, salesModelCode, salesModelYear);
            if (model != null)
            {
                var records = p_Select4DPPorPPn(companyCode, branchCode, draftSONo, salesModelCode, salesModelYear);
                if (records.Count > 0)
                {
                    var rec = records.FirstOrDefault();
                    model.OthersDPP = rec.AfterDiscDPP ?? 0;
                    model.OthersPPn = rec.AfterDiscPPn ?? 0;
                    
                    Helpers.ReplaceNullable(model);
                    if (ctx.SaveChanges() >= 0)
                        result = true;
                }
            }
            omTrSalesDraftSOModelBLL = null;
            return result;
        }

        private List<OmTrSalesDraftSOModelOthers> p_Select4DPPorPPn(string companyCode, string branchCode, string draftSONo, string salesModelCode
            , decimal salesModelYear)
        {
            var records = ctx.OmTrSalesDraftSOModelOthers.Where(p => p.CompanyCode == companyCode 
                && p.BranchCode == branchCode && p.DraftSONo == draftSONo && p.SalesModelCode == salesModelCode 
                && p.SalesModelYear == salesModelYear).ToList();
            
            return records;
        }


        #endregion
    }
}