using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Service.BLL
{
    public class SvTrnClaimJudgementBLL : BaseBLL
    {
        #region "Initiate"
        /// <summary>
        /// 
        /// </summary>
        private static SvTrnClaimJudgementBLL _SvTrnClaimJudgementBLL;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_username"></param>
        /// <returns></returns>
        public static SvTrnClaimJudgementBLL Instance(string _username)
        {
            //if (_SvTrnClaimJudgementBLL == null)
            //{
                _SvTrnClaimJudgementBLL = new SvTrnClaimJudgementBLL();
            //}
            if (string.IsNullOrEmpty(username))
            {
                username = _username;
            }
            return _SvTrnClaimJudgementBLL;
        }
        #endregion

        #region Public Method
        public IQueryable<dynamic> ClaimJudgement4Lookup()
        {
            var dycData = from a in ctx.SvTrnClaimJudgements
                          join b in ctx.Claims
                          on new { a.CompanyCode, a.BranchCode, a.ProductType, a.GenerateNo }
                          equals new { b.CompanyCode, b.BranchCode, b.ProductType, b.GenerateNo }
                          where a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ProductType == ProductType                        
                          select new
                          {
                              a,
                              b.SenderDealerCode,
                              b.SenderDealerName,
                              b.ReceiveDealerCode,
                              b.LotNo,
                              b.BatchNo
                          };

            return dycData;
        }
        #endregion
    }
}