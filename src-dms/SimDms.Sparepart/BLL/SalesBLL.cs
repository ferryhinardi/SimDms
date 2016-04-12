using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using SimDms.Sparepart.Models;
using SimDms.Common;

namespace SimDms.Sparepart.BLL
{
    partial class SalesBLL : BaseBLL
    {
        #region "Initiate"
        private static SalesBLL _SalesBLL;
        public static SalesBLL Instance(string _username)
            {
                if (_SalesBLL == null)
                {
                    _SalesBLL = new SalesBLL();
                }
                //if (string.IsNullOrEmpty(username))
                //{
                    username = _username;
                //}
                return _SalesBLL;
            }
        #endregion

        

        public bool GenFPJ(SpTrnSFPJHdr recordHeader, SpTrnSFPJInfo recordFPJInfo, bool isNew)
        {
            bool result = false;
            SprProviderMgr ProviderMgrSpr = SprProviderMgr.Instance(username);
            try
            {
                string errorMsg;
                string msgCode = errorMsg = "";

                if (isNew)
                {
                    #region Ambil Inisial No Faktur Penjualan
                    recordHeader.FPJNo = recordHeader.IsPKP ? GetNewFPJStd(recordHeader.FPJDate) : GetNewFPJStd(recordHeader.FPJDate);

                    if (recordHeader.FPJNo.EndsWith("X"))
                    {
                        var msg = string.Format(ctx.SysMsgs.Find(SysMessages.MSG_5046).MessageCaption, "Faktur Pajak");
                        throw new Exception(msg);
                    }
                    recordHeader.DeliveryNo = recordHeader.FPJNo.Substring(7);

                    #region "Goverment Tax"
                    var CommonBLL = (CommonBLL)ProviderMgrSpr.GetInstance(SprProviderMgr.SprProviderMgrBLL.CommonBLL);
                    if ((recordHeader.FPJNo.ToString().Contains("SDH")))
                        recordHeader.FPJGovNo = CommonBLL.GetNoSeriFakturPajak(recordHeader.FPJNo);
                    #endregion
                    #endregion


                    #region "Ambil Inisial No Faktur Penjualan"
                    SpTrnSInvoiceHdr oInvoiceHdr = ctx.SpTrnSInvoiceHdrs.Find(CompanyCode, BranchCode, recordHeader.InvoiceNo);
                    if (oInvoiceHdr.Status == "2")
                    {
                        result = false;
                        errorMsg += "Picking List ini sudah dibuatkan faktur pajak" + "\n";
                        return result;
                    }
                    else if (oInvoiceHdr.Status == "0")
                    {
                        result = (UpdateStatus(recordHeader.InvoiceNo, "2", recordHeader.FPJNo, recordHeader.FPJDate) > 0 ? true : false);
                        if (result)
                        {
                            if (!isNew)
                            {
                                if (string.IsNullOrEmpty(recordHeader.TOPCode))
                                {
                                    errorMsg += "Proses Update Faktur Pajak Header Gagal, Term Of Payment belum ter-set !" + "\n";
                                    return result = false;
                                }

                                using (TransactionScope dbTrans = new TransactionScope())
                                {
                                    int pos;
                                    try
                                    {
                                        pos = 1;
                                        var oSpTrnSFPJHdr = ctx.SpTrnSFPJHdrs.Find(CompanyCode, BranchCode, recordHeader.FPJNo);
                                        oSpTrnSFPJHdr = recordHeader;
                                        ctx.SaveChanges();

                                        pos = 2;
                                        var oSpTrnSFPJInfo = ctx.SpTrnSFPJInfos.Find(CompanyCode, BranchCode, recordHeader.FPJNo);
                                        oSpTrnSFPJInfo = recordFPJInfo;
                                        ctx.SaveChanges();
                                        
                                        result = true;
                                        dbTrans.Complete();
                                    }
                                    catch (Exception ex)
                                    {
                                        dbTrans.Dispose();
                                        result = false;
                                        errorMsg += "Proses Update Faktur Pajak Header & Faktur Pajak Info Gagal" + "\n";
                                    }
                                }
                            }
                            else
                            {
                                #region Simpan data ke table SpTRNSFPJHdr
                                if (string.IsNullOrEmpty(recordHeader.TOPCode))
                                {
                                    errorMsg += "Proses Insert Faktur Pajak Header Gagal, Term Of Payment belum ter-set !" + "\n";
                                    result = false;
                                }
                                try
                                {
                                    ctx.SpTrnSFPJHdrs.Add(recordHeader);

                                }
                                catch (Exception ex)
                                {

                                    throw;
                                }
                                #endregion
                            }
                        }
                    }
                    #endregion

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured in SimDms.Sparepart.BLL, Message="+ex.Message, ex.InnerException);
            }
            return result;
        }

        public string GetNewFPJStd(DateTime TglFpjStd)
        {
            return GetNewDocumentNo("FPJ", TglFpjStd);
        }

        public string GetNewFPJSdh(DateTime TglFPJSdh)
        {
            return GetNewDocumentNo("SDH", TglFPJSdh);
        }

        public string GetNewDocumentNo(string doctype, DateTime transdate)
        {
            try
            {
                var sql = "uspfn_GnDocumentGetNew {0}, {1}, {2}, {3}, {4}";
                var result = ctx.Database.SqlQuery<string>(sql, CompanyCode, BranchCode, doctype, CurrentUser.UserId, transdate);
                return result.First();
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured in SimDms.Sparepart.BLL, Message="+ex.Message, ex.InnerException);
            }
        }

        public int UpdateStatus(string docNo, string status, string FPJNo, DateTime FPJDate)
        {
            int result = -1;
            try
            {
                string query = string.Format("UPDATE spTrnSInvoiceHdr With(ReadPast)" +
            " SET Status = '{6}', " +
            " FPJNo = '{2}', " +
            " FPJDate = '{3}', " +
            " LastUpdateBy = '{4}', " +
            " LastUpdateDate = '{5}' " +
            " WHERE CompanyCode = '{0}' AND " +
            " BranchCode = '{1}' AND InvoiceNo = '{7}'", CompanyCode, BranchCode, FPJNo, FPJDate, username, DateTime.Now, status, docNo);
                ctx.Database.ExecuteSqlCommand(query);
                result = 1;
            }
            catch
            {
                result = -1;
            }
            return result;
        }

    }
}
