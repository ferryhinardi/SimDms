using SimDms.Common;
using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Sales.BLL
{
    public class OmTrSalesDraftSOVinBLL : BaseBLL
    {
                #region -- Initiate --
        public OmTrSalesDraftSOVinBLL (DataContext _ctx, string _username)
        {
            this.ctx = _ctx;

            //if (string.IsNullOrEmpty(username))
            //{
                username = _username;
            //}
        }
        #endregion

        #region -- Public Method --
        public List<OmTrSalesDraftSOVin> Select4Table(string draftSONo, string salesModelCode, decimal salesModelYear, string colourCode)
        {
            var records = ctx.OmTrSalesDraftSOVins.Where(p => p.CompanyCode == CompanyCode
                && p.BranchCode == BranchCode && p.DraftSONo == draftSONo && p.SalesModelCode == salesModelCode 
                && p.SalesModelYear == salesModelYear && p.ColourCode == colourCode).ToList();
            
            return records;
        }

        public OmTrSalesDraftSOVin GetRecord(string draftSONo, string salesModelCode, Decimal salesModelYear, string colourCode, int sOSeq)
        {
            var record = ctx.OmTrSalesDraftSOVins.Find(CompanyCode, BranchCode, draftSONo, salesModelCode, salesModelYear, colourCode, sOSeq);

            return record;
        }

        public bool SaveVin(OmTrSalesDraftSO record, OmTrSalesDraftSOVin recordVin, bool isNew)
        {
            bool result = false;
            try
            {
                if (isNew)
                {
                    var salesModelYear = recordVin.SalesModelYear ?? 0;
                    recordVin.SOSeq = p_GetSeqNo(recordVin.CompanyCode, recordVin.BranchCode, recordVin.DraftSONo,
                        recordVin.SalesModelCode, salesModelYear, recordVin.ColourCode) + 1;

                    ctx.OmTrSalesDraftSOVins.Add(recordVin);
                    Helpers.ReplaceNullable(recordVin);
                    result = ctx.SaveChanges() > 0;
                }
                else
                {
                    Helpers.ReplaceNullable(recordVin);
                    result = ctx.SaveChanges() >= 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return result;
        }

        private int p_GetSeqNo(string companyCode, string branchCode, string draftSONo, string salesModelCode
            , decimal salesModelYear, string colourCode)
        {
            int seqNo = 0;
            var record = ctx.OmTrSalesDraftSOVins.Where(p => p.CompanyCode == companyCode
                && p.BranchCode == branchCode && p.DraftSONo == draftSONo && p.SalesModelCode == salesModelCode
                && p.SalesModelYear == salesModelYear && p.ColourCode == colourCode)
                .Select( p => new
                {
                    SOSeq = (p.SOSeq == null) ? 0 : p.SOSeq
                }).ToList();

            if (record.Count > 0)
            {

                seqNo = record.Max(p => p.SOSeq);
            }

            return seqNo;
        }

        #endregion
    }
}