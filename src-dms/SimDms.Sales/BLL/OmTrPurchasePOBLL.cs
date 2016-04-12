using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
//using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SimDms.Sales.BLL
{
    public class OmTrPurchasePOBLL : BaseBLL
    {
        #region -- Initiate --
        public OmTrPurchasePOBLL(DataContext _ctx, string _username)
        {
            this.ctx = _ctx;

            //if (string.IsNullOrEmpty(username))
            //{
                username = _username;
            //}
        }
        #endregion
        
        #region -- Public Method --
        public IEnumerable<OmTrPurchasePOSelect4BPUView> Select4BPU(string status)
        {
            string sql = string.Format(@"SELECT * FROM OmTrPurchasePOSelect4BPUView
                WHERE CompanyCode = '{0}' AND BranchCode = '{1}'
                AND Status = '{2}'", CompanyCode, BranchCode, status);
            var records = ctx.Database.SqlQuery<OmTrPurchasePOSelect4BPUView>(sql.Trim());

            return records;
        }
        #endregion
    }
}