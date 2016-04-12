using SimDms.Common;
using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace SimDms.Sales.BLL
{
    public class OmTrSalesReqBLL : BaseBLL
    {
        #region -- Initiate --
        public OmTrSalesReqBLL(DataContext _ctx, string _username)
        {
            this.ctx = _ctx;

             //if (string.IsNullOrEmpty(username))
            //{
                username = _username;
            //}
        }
        #endregion

        #region -- Public Method --        
        public IQueryable<omTrSalesReq> Select4Send(DateTime startDate, DateTime endDate, bool isCBU)
        {
            var records = ctx.omTrSalesReq.Where(p => p.CompanyCode == CompanyCode
                && p.BranchCode == BranchCode).ToList().Where(x => 
                    ((DateTime)x.ReqDate).Date >= startDate.Date && ((DateTime)x.ReqDate).Date <= endDate.Date && x.Status == "2" && x.isCBU == isCBU ).AsQueryable();

            //var records = ctx.omTrSalesReq.Where(p => p.CompanyCode == CompanyCode
            //    && p.BranchCode == BranchCode && p.ReqDate.ToString("dd.MM.yy") == startDate.ToString("dd.MM.yy"));

            return records;
        }

        public IQueryable<omTrSalesReq> Select4Send(DateTime startDate, DateTime endDate)
        {
            var records = ctx.omTrSalesFPolRevision.Where(p => p.CompanyCode == CompanyCode
                && p.BranchCode == BranchCode).ToList().Where(x =>
                    ((DateTime)x.RevisionDate).Date >= startDate.Date && ((DateTime)x.RevisionDate).Date <= endDate.Date).AsQueryable()
                    .Select(x => new omTrSalesReq() { ReqNo = x.RevisionNo, ReqDate = x.RevisionDate });

            //var records = ctx.omTrSalesReq.Where(p => p.CompanyCode == CompanyCode
            //    && p.BranchCode == BranchCode && p.ReqDate.ToString("dd.MM.yy") == startDate.ToString("dd.MM.yy"));

            return records;
        }
        #endregion
    }
}