using SimDms.Common;
using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Sales.BLL
{
    public class OmTrSalesDraftSOModelBLL : BaseBLL
    {
        #region -- Initiate --
        public OmTrSalesDraftSOModelBLL (DataContext _ctx, string _username)
        {
            this.ctx = _ctx;

            //if (string.IsNullOrEmpty(username))
            //{
                username = _username;
            //}
        }
        #endregion

        #region -- Public Method --
        public List<OmTrSalesDraftSOModel> Select4Table(string draftSONo)
        {
            var records = ctx.OmTrSalesDraftSOModels.Where(p =>
                p.CompanyCode == CompanyCode && p.BranchCode 
                == BranchCode & p.DraftSONo == draftSONo).ToList();
            
            return records;
        }

        public bool CheckingQtySO(string draftSONo)
        {
            bool result = false;
            var records = (from a in ctx.OmTrSalesSOModels
                          join b in ctx.OmTRSalesSOs on new { a.CompanyCode, a.BranchCode, a.SONo }
                          equals new { b.CompanyCode, b.BranchCode, b.SONo }
                          where a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && b.SKPKNo == draftSONo
                          select new { a, b } ).ToList();

            if (records != null)
            {
                if(records.Count == 0)
                result = true;
            }

            return result;
        }

        public OmTrSalesDraftSOModel GetRecord(string draftSONo, string salesModelCode, decimal salesModelYear)
        {
            var record = ctx.OmTrSalesDraftSOModels.Find(CompanyCode, BranchCode, draftSONo,
                salesModelCode, salesModelYear);


            return record;
        }

        public bool SaveDetailDraftSO(OmTrSalesDraftSOModel recordModel, OmTrSalesDraftSO record, bool isNew, bool isTypeSales)
        {
            bool result = false;
            try
            {
                if (isNew)
                {
                    if (isTypeSales)
                    {
                        //UpdateInquiryStatus(ctx, record, "DEAL") && 
                        ctx.OmTrSalesDraftSOModels.Add(recordModel);
                        Helpers.ReplaceNullable(recordModel);
                        if (ctx.SaveChanges() > 0) result = true;
                    }
                    else
                    {
                        ctx.OmTrSalesDraftSOModels.Add(recordModel);
                        Helpers.ReplaceNullable(recordModel);
                        if (ctx.SaveChanges() > 0) result = true;
                    }
                }
                else
                {
                    Helpers.ReplaceNullable(record);
                    Helpers.ReplaceNullable(recordModel);
                    if (ctx.SaveChanges() >= 0) result = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
            return result;
        }
        #endregion
    }
}