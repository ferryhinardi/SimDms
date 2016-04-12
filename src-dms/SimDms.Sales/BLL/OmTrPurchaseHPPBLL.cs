using SimDms.Common;
using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SimDms.Sales.BLL
{
    public class OmTrPurchaseHPPBLL : BaseBLL
    {
        
        #region -- Initiate --
        private static OmTrPurchaseHPPBLL _OmTrPurchaseHPPBLL;

        public static OmTrPurchaseHPPBLL Instance(string _username)
        {
            _OmTrPurchaseHPPBLL = new OmTrPurchaseHPPBLL();
            username = _username;
            return _OmTrPurchaseHPPBLL;
        }
        #endregion

        #region -- Public Method --
 
        public bool validSubDetail(string HPPNo)
        {
            bool result = false;
            var qry = String.Format(@"
            SELECT sum (a.Quantity)
            FROM dbo.omTrPurchaseHPPDetailModel a
            WHERE a.CompanyCode = '{0}'
                AND a.BranchCode = '{1}'
                AND a.HPPNo = '{2}'
            ", CompanyCode, BranchCode, HPPNo);

            decimal totalModelDetail = 0;
            totalModelDetail = ctx.Database.SqlQuery<decimal>(qry).FirstOrDefault();
            
            qry = String.Format(@"
            SELECT count ( * )
            FROM dbo.omTrPurchaseHPPSubDetail a
            WHERE a.CompanyCode = '{0}'
                AND a.BranchCode = '{1}'
                AND a.HPPNo = '{2}'
            ", CompanyCode, BranchCode, HPPNo);
            
            Int32 totalSubDetail = 0;
            totalSubDetail = ctx.Database.SqlQuery<Int32>(qry).FirstOrDefault();

            if (totalModelDetail == totalSubDetail)
                result = true;

            return result;
        }

        public bool ApproveHPP(string HPPNo)
        {
            bool result = false;
            result = ctx.Database.ExecuteSqlCommand("exec uspfn_OmApprovePurchaseHPP '" + CompanyCode + "','" + BranchCode + "','" + HPPNo + "','" + CurrentUser.UserId + "'") > 0;
 
            return result;
        }

        #endregion
    }
}