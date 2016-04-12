using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimDms.Sales.Models;

namespace SimDms.Sales.BLL
{
    public class OmFakturPajakHdrBLL : BaseBLL
    {
        #region -- Initiate --
        /// <summary>
        /// 
        /// </summary>
        private static OmFakturPajakHdrBLL _OmFakturPajakHdrBLL;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_username"></param>
        /// <returns></returns>
        public static OmFakturPajakHdrBLL Instance(string _username)
        {
            //if (_OmTrPurchaseBPUBLL == null)
            //{
            _OmFakturPajakHdrBLL = new OmFakturPajakHdrBLL();
            //}
            //if (string.IsNullOrEmpty(username))
            //{
                username = _username;
            //}
            return _OmFakturPajakHdrBLL;
        }
        #endregion

        public  bool Save(DataContext ctxx, omFakturPajakHdr record, DateTime signDate)
        {
            bool result = false;
            var user = CurrentUser;

            //IDbContext ctxx = DbFactory.Configure(true);
            //OmFakturPajakHdrDao oOmFakturPajakHdrDao = new OmFakturPajakHdrDao(ctx);
         
                if (record.TaxType == "Standard")
                    record.FakturPajakNo = string.Empty;
                else record.FakturPajakNo = GetNoFPSederhanaUnit(ctxx,signDate.Year);

                if (record.TaxType != "Standard")
                    if (record.FakturPajakNo == string.Empty)
                        throw new ApplicationException("No. Faktur Pajak tidak valid");

                decimal dppAmt = 0; decimal discAmt = 0; decimal totalAmt = 0; decimal ppnAmt = 0; decimal totQty = 0; decimal ppnBMPaid = 0;
                decimal dppAmtAccs = 0; decimal totalAmtAccs = 0; decimal ppnAmtAccs = 0; decimal totQtyAccs = 0;
                decimal dppAmtOther = 0; decimal totalAmtOther = 0; decimal ppnAmtOther = 0; decimal totQtyOther = 0;

                var rowAccs = ctxx.Database.SqlQuery<omSlsInvGetTotal4Tax>
                    (string.Format("exec uspfn_omSlsInvGetAccsSeqTotal4Tax {0},{1},'{2}'", CompanyCode, BranchCode, record.InvoiceNo))
                    .FirstOrDefault();
                //OmTrSalesInvoiceAccsSeqBLL.GetTotal4Tax(ctx, user.CompanyCode, user.BranchCode, record.InvoiceNo);

                if (rowAccs != null)
                {
                    dppAmtAccs = rowAccs.DPPAmt;
                    totalAmtAccs=rowAccs.TotalAmt;
                    ppnAmtAccs = rowAccs.PPnAmt;
                    totQtyAccs = rowAccs.TotQuantity;
                }
                var rowOther = ctxx.Database.SqlQuery<omSlsInvGetTotal4Tax>
                    (string.Format("exec uspfn_omSlsInvOtherGetTotal4Tax {0},{1},'{2}'", CompanyCode, BranchCode, record.InvoiceNo))
                    .FirstOrDefault();                    

                if (rowOther != null)
                {
                    dppAmtOther = rowOther.DPPAmt;
                    totalAmtOther=rowOther.TotalAmt;
                    ppnAmtOther = rowOther.PPnAmt;
                    totQtyOther = rowOther.TotQuantity;
                }
                var row = ctxx.Database.SqlQuery<omSlsInvGetTotal4TaxModel>
                    (string.Format("exec uspfn_omSlsInvModelGetTotal4Tax {0},{1},'{2}'", CompanyCode, BranchCode, record.InvoiceNo))
                    .FirstOrDefault();
                if (row != null)
                {
                    dppAmt = row.DPPAmt;
                    discAmt =  row.DiscAmt;
                    totalAmt = row.TotalAmt;
                    ppnAmt =   row.PPnAmt;
                    totQty =   row.TotQuantity;
                    ppnBMPaid =row.PPnBMPaid;
                }

                record.DPPAmt = dppAmt + dppAmtAccs + dppAmtOther;
                record.DiscAmt = discAmt;
                record.TotalAmt = totalAmt + totalAmtAccs + totalAmtOther;
                record.PPnAmt = ppnAmt + ppnAmtAccs + ppnAmtOther;
                record.TotQuantity = totQty + totQtyAccs + totQtyOther;
                record.PPnBMPaid = ppnBMPaid;

                //result = oOmFakturPajakHdrDao.Insert(record) > 0;

  //              ctxx.omfak
                ctxx.omFakturPajakHdrs.Add(record);

                result = ctxx.SaveChanges() >= 0;

                if (result == true)
                {
                    if (record.TaxType != "Standard")
                    {                        
                          var recInv = ctxx.OmTrSalesInvoices.Find(CompanyCode, BranchCode, record.InvoiceNo);
                        
                        if (recInv != null)
                        {
                            recInv.FakturPajakNo = record.FakturPajakNo;
                            recInv.FakturPajakDate = record.FakturPajakDate;
                            //recInv.Status = "9";
                            recInv.LastUpdateBy = user.UserId;
                            recInv.LastUpdateDate = ctxx.CurrentTime;                            
                            result = ctxx.SaveChanges() >= 0;
                        }
                    }
                    if (result == true)
                    {
                        //result = OmFakturPajakDetailDOBLL.SaveDO(ctxx, user.CompanyCode, user.BranchCode, record.InvoiceNo);
                        //result = ctxx.Database.ExecuteSqlCommand("exec uspfn_OmFakturPajakDtlDOSaveDO {0},{1},'{2}'", CompanyCode, BranchCode, record.InvoiceNo) >= 0;
                        result = ctxx.Database.ExecuteSqlCommand("exec uspfn_OmFakturPajakDtlDOSaveDO " + CompanyCode + ", " + BranchCode + ", '" + record.InvoiceNo + "'") >= 0;

                        //result=OmFakturPajakDetailBLL.SaveDetail(ctx, user.CompanyCode, user.BranchCode, record.InvoiceNo);
                        result = ctxx.Database.ExecuteSqlCommand("exec uspfn_OmFakturPajakDtlSaveDetail " + CompanyCode + ", " + BranchCode + ", '" + record.InvoiceNo + "'") >= 0;

                        //result = OmFakturPajakDetailAccsSeqBLL.SaveDetail(ctx, user.CompanyCode, user.BranchCode, record.InvoiceNo);
                        result = ctxx.Database.ExecuteSqlCommand("exec uspfn_OmFakturPajakDtlAccsSeqSaveDetail " + CompanyCode + ", " + BranchCode + ", '" + record.InvoiceNo + "'") >= 0;
                        
                        //result = OmFakturPajakDetailOthersBLL.SaveDetail(ctx, user.CompanyCode, user.BranchCode, record.InvoiceNo);
                        result = ctxx.Database.ExecuteSqlCommand("exec uspfn_OmFakturPajakDtlOthersSaveDetail " + CompanyCode + ", " + BranchCode + ", '" + record.InvoiceNo + "', 'OTHS'") >= 0;
                        
                    }
                }
         
            return result;
        }

        private  string GetNoFPSederhanaUnit(DataContext ctxx, int year)
        {
            bool result = false;
            var user = CurrentUser;
            var oFPSNo = ctxx.GnMstDocuments.Find(CompanyCode, BranchCode, "FPO");
            if (oFPSNo != null)
            {
                string tahun = year.ToString().Substring(2, 2);
                string suffix = oFPSNo.DocumentPrefix;
                decimal iseq = oFPSNo.DocumentSequence + 1;
                string seq = iseq.ToString().PadLeft(7, '0');

                oFPSNo.CompanyCode = user.CompanyCode;
                oFPSNo.BranchCode = user.BranchCode;
                oFPSNo.DocumentYear = year;
                oFPSNo.DocumentSequence = iseq;                
                
                result = ctxx.SaveChanges() > -1;
                
                if (result)
                    return string.Format("{0}-{1}{2}", suffix, tahun, seq);
                else
                    return string.Empty;
            }
            return string.Empty;
        }


    }
}