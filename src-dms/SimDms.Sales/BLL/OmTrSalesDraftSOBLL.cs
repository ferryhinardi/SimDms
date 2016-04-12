using SimDms.Common;
using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SimDms.Sales.BLL
{
    public class OmTrSalesDraftSOBLL : BaseBLL
    {
        #region -- Initiate --

        public OmTrSalesDraftSOBLL(DataContext _ctx, string _username)
        {
            //if (_OmTrSalesDraftSOBLL == null)
            //{
            ctx = _ctx;
            //}
            //if (string.IsNullOrEmpty(username))
            //{
            username = _username;
            //}
        }
        #endregion

        #region -- Public Method --
        public OmTrSalesDraftSO GetRecord(string draftSONo)
        {
            var record = ctx.OmTrSalesDraftSOs.Find(CompanyCode, BranchCode, draftSONo);
            return record;
        }

        public IQueryable<Select4LookupCustomer> Select4LookupCustomer(string profitCenterCode)
        {
            var records = ctx.Database.SqlQuery<Select4LookupCustomer>("exec sp_Select4LookupCustomer @p0, @p1, @p2, @p3",
                CompanyCode, BranchCode, profitCenterCode, GnMstLookUpHdr.TermOfPayment).AsQueryable();

            return records;
        }

        public IQueryable<Select4LookupCustomer> Select4LookupCustomer2(string profitCenterCode)
        {
            var records = ctx.Database.SqlQuery<Select4LookupCustomer>("exec sp_Select4LookupCustomer2 @p0, @p1, @p2, @p3",
                CompanyCode, BranchCode, profitCenterCode, GnMstLookUpHdr.TermOfPayment).AsQueryable();

            return records;
        }

        public bool SaveDraftSO(OmTrSalesDraftSO record, bool isNew)
        {
            bool result = false;
            if (isNew)
            {
                try
                {
                    var SODate = (DateTime)record.DraftSODate;
                    record.DraftSONo = p_GetNewDraftSONo(SODate);
                    if (record.DraftSONo.EndsWith("X")) throw new ApplicationException(string.Format(GetMessage(SysMessages.MSG_5046), GnMstDocumentConstant.SPU));

                    ctx.OmTrSalesDraftSOs.Add(record);
                    Helpers.ReplaceNullable(record);
                    result = ctx.SaveChanges() > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            else
            {
                Helpers.ReplaceNullable(record);
                result = ctx.SaveChanges() >= 0;
            }
            return result;
        }

        public bool DeleteDraftSO(string draftSONo)
        {
            return p_DeleteDraftSO(draftSONo, "", 0, "", "", 0, 1);
        }

        public bool DeleteDraftSO(string draftSONo, string salesModelCode, decimal salesModelYear)
        {
            return p_DeleteDraftSO(draftSONo, salesModelCode, salesModelYear, "", "", 0, 2);
        }

        public bool DeleteDraftSO(string draftSONo, string salesModelCode, decimal salesModelYear, string colourCode)
        {
            return p_DeleteDraftSO(draftSONo, salesModelCode, salesModelYear, colourCode, "", 0, 3);
        }

        public bool DeleteDraftSO(string draftSONo, string salesModelCode, decimal salesModelYear, string colourCode, int soSeq)
        {
            return p_DeleteDraftSO(draftSONo, salesModelCode, salesModelYear, colourCode, "", soSeq, 4);
        }

        public bool DeleteDraftSoOthers(string draftSONo, string salesModelCode, decimal salesModelYear, string otherCode)
        {
            return p_DeleteDraftSO(draftSONo, salesModelCode, salesModelYear, "", otherCode, 0, 5);
        }

        public bool ApproveDraftSO(OmTrSalesDraftSO record, bool isLInkITS)
        {
            bool result = false;
            try
            {
                Helpers.ReplaceNullable(record);
                result = ctx.SaveChanges() > 0;
                if (result)
                {
                    if (isLInkITS)
                    {
                        if (record.ProspectNo != "")
                        {
                            if (!p_Update4ITS(Convert.ToInt32(record.ProspectNo), username, "SPK"))
                                throw new Exception("Update ITS Gagal");
                        }
                    }
                }
                else
                    throw new Exception("Gagal update Draft SO");
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception(ex.Message);
            }

            return result;
        }

        public bool UnApprovedDraftSO(OmTrSalesDraftSO record, bool isLinkITS)
        {
            bool result = false;

            try
            {
                Helpers.ReplaceNullable(record);
                result = ctx.SaveChanges() > 0;
                if (result)
                {
                    if (isLinkITS)
                    {
                        if (record.ProspectNo != "")
                        {
                            if (!p_Update4ITS(Convert.ToInt32(record.ProspectNo), username, "HP"))
                                throw new Exception("Update ITS Gagal");
                        }
                    }
                }
                else
                    throw new Exception("Gagal update Draft SO");
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception(ex.Message);
            }

            return result;
        }

        public bool QuantityCheck(string draftSONo, string salesModelCode, decimal salesModelYear, string colourCode)
        {
            bool result = false;
            int countVin = p_GetCountVin(draftSONo, salesModelCode, salesModelYear, colourCode);
            int sumQuantity = p_GetSumQuantity(draftSONo, salesModelCode, salesModelYear, colourCode);
            if (countVin >= sumQuantity)
                result = true;
            return result;
        }

        #endregion

        #region -- Private Mothod --
        private string p_GetNewDraftSONo(DateTime transDate)
        {
            return GetNewDocumentNo(GnMstDocumentConstant.SPU, transDate);
        }

        private bool p_DeleteDraftSO(string draftSONo, string salesModelCode, decimal salesModelYear, string colourCode
            , string otherCode, int soSeq, int deleteType)
        {
            int salesYear = Convert.ToInt32(salesModelYear);
            var result = ctx.Database.ExecuteSqlCommand(@"exec uspfn_OmDeleteSalesDraftSO 
                    @CompanyCode=@p0, @BranchCode=@p1, @DraftSONo=@p2, @SalesModelCode=@p3, 
                    @SalesModelYear=@p4, @ColourCode=@p5, @OtherCode=@p6, @SOSeq=@p7, @UserId=@p8, @DeleteType=@p9",
                    CompanyCode, BranchCode, draftSONo, salesModelCode, salesYear, colourCode, otherCode,
                    soSeq, CurrentUser.UserId, deleteType) > 0;

            return result;
        }

        private int p_GetCountVin(string draftSONo, string salesModelCode, decimal salesModelYear, string colourCode)
        {
            int countVin = ctx.OmTrSalesDraftSOVins.Where(p => p.CompanyCode == CompanyCode
                && p.BranchCode == BranchCode && p.DraftSONo == draftSONo && p.SalesModelCode == salesModelCode
                && p.SalesModelYear == salesModelYear && p.ColourCode == colourCode).Count();

            return countVin;
        }

        private int p_GetSumQuantity(string draftSONo, string salesModelCode, decimal salesModelYear, string colourCode)
        {
            int sumQty = 0;
            var records = ctx.OmTrSalesDraftSOModelColours.Where(p => p.CompanyCode == CompanyCode
                && p.BranchCode == BranchCode && p.DraftSONo == draftSONo && p.SalesModelCode == salesModelCode
                && p.SalesModelYear == salesModelYear && p.ColourCode == colourCode);
            ;
            if (records != null)
            {
                decimal qty = records.Sum(p => p.Quantity) ?? 0;
                sumQty = (int)qty;
            }

            return sumQty;
        }

        private bool p_Update4ITS(int prospectNo, string userID, string progress)
        {
            bool result = false;
            PmKdp pmKDP = ctx.PmKdps.Find(prospectNo, BranchCode, CompanyCode);
            if (pmKDP != null)
            {
                pmKDP.LastProgress = progress;
                pmKDP.LastUpdateStatus = ctx.CurrentTime;
                pmKDP.LastUpdateBy = userID;
                pmKDP.LastUpdateDate = ctx.CurrentTime;

                Helpers.ReplaceNullable(pmKDP);
                result = ctx.SaveChanges() > 0;

                if (result)
                {
                    int seqHist = 0;
                    var recPmHist = ctx.PmStatusHistories.Where(p => p.CompanyCode == CompanyCode &&
                        p.BranchCode == BranchCode && p.InquiryNumber == pmKDP.InquiryNumber);

                    if (recPmHist != null)
                    {
                        seqHist = recPmHist.FirstOrDefault().SequenceNo;
                    }

                    PmStatusHistory oPmStatusHistory = new PmStatusHistory();
                    oPmStatusHistory.CompanyCode = CompanyCode;
                    oPmStatusHistory.BranchCode = BranchCode;
                    oPmStatusHistory.InquiryNumber = pmKDP.InquiryNumber;
                    oPmStatusHistory.SequenceNo = seqHist + 1;
                    oPmStatusHistory.LastProgress = progress;
                    oPmStatusHistory.UpdateDate = ctx.CurrentTime;
                    oPmStatusHistory.UpdateUser = userID;
                    ctx.PmStatusHistories.Add(oPmStatusHistory);

                    Helpers.ReplaceNullable(oPmStatusHistory);
                    result = ctx.SaveChanges() > 0;
                }
            }

            return result;
        }

        #endregion
    }
}