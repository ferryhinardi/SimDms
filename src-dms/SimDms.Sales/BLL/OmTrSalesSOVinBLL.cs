using SimDms.Sales.Models;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Sales.BLL 
{
    public class OmTrSalesSOVinBLL : BaseBLL 
    {
        #region -- Initiate --
        public OmTrSalesSOVinBLL (DataContext _ctx, string _username)
        {
            this.ctx = _ctx;

            //if (string.IsNullOrEmpty(_username))
            //{
                username = _username;
            //}
        }
        #endregion

        #region -- Public Method --
        public List<omTrSalesSOVin> Select4Table(string SONo, string salesModelCode, decimal salesModelYear, string colourCode) 
        {
            var records = ctx.omTrSalesSOVins.Where(p => p.CompanyCode == CompanyCode
                && p.BranchCode == BranchCode && p.SONo == SONo && p.SalesModelCode == salesModelCode 
                && p.SalesModelYear == salesModelYear && p.ColourCode == colourCode).ToList();
            
            return records;
        }

        public omTrSalesSOVin GetRecord(string SONo, string salesModelCode, Decimal salesModelYear, string colourCode, int sOSeq)
        {
            var record = ctx.omTrSalesSOVins.Find(CompanyCode, BranchCode, SONo, salesModelCode, salesModelYear, colourCode, sOSeq);

            return record;
        }

        public omTrSalesSOVin GetRecordSO(string SONo)
        {
            var record = ctx.omTrSalesSOVins.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.SONo == SONo && p.ChassisNo != 0).FirstOrDefault();
            return record;
        }

        
        private int p_GetSeqNo(string companyCode, string branchCode, string SONo, string salesModelCode
            , decimal salesModelYear, string colourCode)
        {
            int seqNo = 0;
            var record = ctx.omTrSalesSOVins.Where(p => p.CompanyCode == companyCode
                && p.BranchCode == branchCode && p.SONo == SONo && p.SalesModelCode == salesModelCode
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