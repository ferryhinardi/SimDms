using SimDms.Common;
using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SimDms.Sales.BLL
{
    public class SendBLL : BaseBLL
    {
        #region -- Initiate --
        public SendBLL(DataContext _ctx, string _username)
        {
            this.ctx = _ctx;

             //if (string.IsNullOrEmpty(username))
            //{
                username = _username;
            //}
        }
        #endregion

        #region -- Public Method --        
        public IEnumerable<OmSelectFakturPolisi> GetFakturPolisiDataTableNoReq(DateTime dateBegin, DateTime dateEnd, string isCheck, string regNoFrom, string regNoTo, bool isCBU)
        {
            IEnumerable<OmSelectFakturPolisi> records = ctx.Database.SqlQuery<OmSelectFakturPolisi>(@"exec uspfn_OmSelectFakturPolisi 
                @CompanyCode=@p0, @BranchCode=@p1, @DateBegin=@p2, @DateEnd=@p3, @Check=@p4, @RegNoFrom=@p5, @RegNoTo=@p6, @isCBu=@p7", 
                CompanyCode, BranchCode, dateBegin.Date.ToString("yyyyMMdd"), dateEnd.Date.ToString("yyyyMMdd"),
                isCheck, regNoFrom, regNoTo, isCBU == true ? "1" : "0");
            
            return records;
        }

        public string GetFakturPolisiBatchNo()
        {
            string result = "";
            try
            {
                string n = GetNewDocumentNo(GnMstDocumentConstant.BFP, ctx.CurrentTime, "OM");
                if (n.EndsWith("X")) throw new ApplicationException
                    (string.Format(GetMessage(SysMessages.MSG_5046), GnMstDocumentConstant.BFP));
                result = n.Substring(7);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return result;
        }

        public IEnumerable<OmGetStockDataTable> GetStockDataTable(DateTime dateBegin, DateTime dateEnd)
        {
            IEnumerable<OmGetStockDataTable> records = ctx.Database.SqlQuery<OmGetStockDataTable>(@"exec uspfn_OmGetStockDataTable 
                @CompanyCode=@p0, @DateBegin=@p1, @DateEnd=@p2",
                CompanyCode, dateBegin.Date.ToString("yyyyMMdd"), dateEnd.Date.ToString("yyyyMMdd"));

            return records;
        }

        public string GetStockBatchNo()
        {
            string result = "";
            try
            {
                string n = GetNewDocumentNo(GnMstDocumentConstant.BVC, ctx.CurrentTime, "OM");
                if (n.EndsWith("X")) throw new ApplicationException
                    (string.Format(GetMessage(SysMessages.MSG_5046), GnMstDocumentConstant.BVC));
                result = n.Substring(7);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return result;
        }

        public IEnumerable<OmSelectRevFakturPolisi> GetRevFakturPolisiDataTableNoRev(DateTime dateBegin, DateTime dateEnd, string isCheck, string regNoFrom, string regNoTo)
        {
            IEnumerable<OmSelectRevFakturPolisi> records = ctx.Database.SqlQuery<OmSelectRevFakturPolisi>(@"select * from omtrsalesfpolrevision
                where CompanyCode=@p0 and BranchCode=@p1
		          AND CONVERT(VARCHAR, RevisionDate, 112) BETWEEN @p2 AND @p3
		          AND (case when @p4 <> '0' then RevisionNo else @p5 end) BETWEEN @p5 AND @p6",
                CompanyCode, BranchCode, dateBegin.Date.ToString("yyyyMMdd"), dateEnd.Date.ToString("yyyyMMdd"),
                isCheck, regNoFrom, regNoTo);

            return records;
        }

        public string GetRevFakturPolisiBatchNo()
        {
            string result = "";
            try
            {
                string n = GetNewDocumentNo(GnMstDocumentConstant.BRFP, ctx.CurrentTime, "OM");
                if (n.EndsWith("X")) throw new ApplicationException
                    (string.Format(GetMessage(SysMessages.MSG_5046), GnMstDocumentConstant.BFP));
                result = n.Substring(7);
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