using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimDms.Sales.Models;

namespace SimDms.Sales.BLL
{
    public class OmTrSalesReturnBLL : BaseBLL
    {
        private static OmTrSalesReturnBLL _OmTrSalesReturnBLL;
        
        public static OmTrSalesReturnBLL Instance (string _username)
        {
            _OmTrSalesReturnBLL = new OmTrSalesReturnBLL();
            username = _username;

            return _OmTrSalesReturnBLL;
        }

    }

    //public static bool SaveReturn(OmTrSalesReturn record, bool isNew)
    //    {
    //        bool result = false;
    //        IDbContext ctx = DbFactory.Configure(true);
    //        OmTrSalesReturnDao oOmTrSalesReturnDao = new OmTrSalesReturnDao(ctx);
    //        try
    //        {
    //            if (isNew)
    //            {
    //                record.ReturnNo = GetNewReturnNo(ctx, record.ReturnDate);
    //                if (record.ReturnNo.EndsWith("X")) throw new ApplicationException
    //                    (string.Format(SysMessageBLL.GetMessage(SysMessage.MSG_5046), GnMstDocument.RTS));

    //                result = oOmTrSalesReturnDao.Insert(record) > 0;
    //            }
    //            else
    //            {
    //                result = oOmTrSalesReturnDao.Update(record) > 0;
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            ctx.RollBackTransaction();
    //            XLogger.Log(ex);
    //        }
    //        finally
    //        {
    //            if (result)
    //                ctx.CommitTransaction();
    //            else
    //                ctx.RollBackTransaction();
    //        }
    //        return result;
    //    }

    //public static string GetNewReturnNo(IDbContext ctx, DateTime transDate)
    //    {
    //        return GnMstDocumentBLL.GetNewDocumentNo(ctx, GnMstDocument.RTS, transDate);
    //    }
    

}