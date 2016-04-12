using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using System.Web.Mvc;
using SimDms.Sparepart.Models;
using System.Transactions;
using System.Collections;
using SimDms.Common;
using System.Globalization;
using System.Threading;
using TracerX;

namespace SimDms.Sparepart.Controllers.Api
{
    public class EntryPickedListController : BaseController
    {
        private const string ProfitCenter = "300";
        private const string ProfitCenterName = "SPARE PART";

        string[] unitems = { "-", "unidentified" };

        public JsonResult Default()
        {
            return Json(new { PickingSlipDate = ctx.CurrentTime });
        }

        public JsonResult DefaultNonSales()
        {
            var lookUpTREX = ctx.LookUpDtls.Find(CompanyCode, "TREX", "STATUS");
            bool isTrex = false;
            if (lookUpTREX != null)
            {
                if (lookUpTREX.ParaValue == "1") isTrex = true;
            }
            return Json(new { isTrex = isTrex });
        }

        [HttpPost]
        public JsonResult GetCustomerOrderDtl(SpTrnSPickingHdr model)
        {
            List<CustOrderDetails> ListCustOrderDtl = null;
            try
            {
                if (model.IsBORelease)
                {
                    var CustProfCtr = ctx.ProfitCenters.Find(CompanyCode, BranchCode, model.CustomerCode, ProfitCenter);
                    //check record for overdue order
                    if (CustProfCtr != null)
                    {
                        if (!CustProfCtr.isOverDueAllowed.Value)
                        {
                            if (IsOverdueOrder(model.CustomerCode))
                            {
                                var sysMsg = ctx.SysMsgs.Find("5023");
                                return Json(new { success = false, message = sysMsg.MessageCaption });
                            }
                        }
                    }
                }

                //get data for New Order
                if (!model.IsBORelease)
                {
                    //GetSO4NonPLNewOrderWindow
                    var sql = string.Format("exec uspfn_spGetSO4NonPLNewOrderWindow '{0}','{1}','{2}','{3}','{4}','{5}','{6}'", CompanyCode, BranchCode, model.CustomerCode, model.SalesType, model.TransType, TypeOfGoods, ProductType);
                    ListCustOrderDtl = ctx.Database.SqlQuery<CustOrderDetails>(sql).ToList();
                }
                else
                {
                    //get data for Back Order
                    if (int.Parse(model.SalesType) == 0)
                    {
                        //get data for salestype SP Penjualan GetSO4PLBackOrderWindow
                        var sql = string.Format("exec uspfn_spGetSO4PLBackOrderWindow '{0}','{1}','{2}','{3}','{4}','{5}','{6}'", CompanyCode, BranchCode, model.CustomerCode, model.SalesType, model.TransType, TypeOfGoods, ProductType);
                        ListCustOrderDtl = ctx.Database.SqlQuery<CustOrderDetails>(sql).ToList();
                    }
                    else
                    {
                        //GetSO4NonPLBackOrderWindow
                        var sql = string.Format("exec uspfn_spGetSO4NonPLBackOrderWindow '{0}','{1}','{2}','{3}','{4}','{5}','{6}'", CompanyCode, BranchCode, model.CustomerCode, model.SalesType, model.TransType, TypeOfGoods, ProductType);
                        ListCustOrderDtl = ctx.Database.SqlQuery<CustOrderDetails>(sql).ToList();
                    }
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error pada function Sparepart.EntryPickedListController.GetCustomerOrderDtl, Message=" + ex.Message.ToString() });
            }
            if (ListCustOrderDtl.Count > 0)
            {
                var listCustDtl = ListCustOrderDtl
                .Select(p => new
                {
                    chkSelect = (p.chkSelect) ? 1 : 0,
                    p.CustomerCode,
                    p.DocDate,
                    p.DocNo,
                    p.OrderDate,
                    p.OrderNo,
                    p.PaymentName,
                    p.ReferenceDate,
                    p.ReferenceNo,
                    p.Remark
                });

                return Json(new { success = true, data = listCustDtl });
            }
            else
            {
                var sysMsg = ctx.SysMsgs.Find("5015").MessageCaption;
                return Json(new { success = false, message = sysMsg });
            }
        }

        public JsonResult GetPartOrderDetail(SpTrnSPickingHdr model, string[] DocNoList)
        {
            List<PickingPartOrderDetail> listOrderDtl = null;
            string sql = "";
            if (model.IsBORelease)
            {
                listOrderDtl = Select4TablePLFromBO(DocNoList);
            }
            else
            {
                listOrderDtl = Select4TablePLFromNewOrder(DocNoList);
            }
            return Json(new { success = true, data = listOrderDtl, message = "" });
        }

        public JsonResult GetPickingHdr(SpTrnSPickingHdr model)
        {
            List<CustOrderDetails> lstCustOrderDtl = null;
            string sql = "";
            if (model.SalesType == "0")
            {
                sql = string.Format("exec uspfn_spPickingHdrSales '{0}','{1}','{2}','{3}'", CompanyCode, BranchCode, model.PickingSlipNo, "PYBY");
                lstCustOrderDtl = ctx.Database.SqlQuery<CustOrderDetails>(sql).ToList();
                lstCustOrderDtl = (from m in lstCustOrderDtl
                                  select new CustOrderDetails
                                  {
                                      DocNo = m.DocNo,
                                      DocDate = m.DocDate,
                                      CustomerCode = m.CustomerCode,
                                      PaymentName = m.PaymentName,
                                      ReferenceNo = m.OrderNo,
                                      ReferenceDate = m.OrderDate 
                                  }).ToList();
            }
            else
            {
                sql = string.Format("exec uspfn_spPickingHdrNonSales '{0}','{1}','{2}'", CompanyCode, BranchCode, model.PickingSlipNo);
                lstCustOrderDtl = ctx.Database.SqlQuery<CustOrderDetails>(sql).ToList();
            }
            
            return Json(new { success = true, data = lstCustOrderDtl, message = "" });

        }

        public JsonResult GetPickingDtl(SpTrnSPickingHdr model)
        {
            List<PickingPartOrderDetail> listPickingDtl = null;
            var sql = string.Format("exec uspfn_spGetPickingDtls '{0}','{1}','{2}'", CompanyCode, BranchCode, model.PickingSlipNo);
            listPickingDtl = ctx.Database.SqlQuery<PickingPartOrderDetail>(sql).ToList();
            return Json(new { success = true, data = listPickingDtl, message = "" });
        }

        protected List<PickingPartOrderDetail> Select4TablePLFromNewOrder(string[] DocNoList)
        {
            var sql = @"SELECT DISTINCT 
                     row_number () OVER (ORDER BY spTrnSOSupply.DocNo, spTrnSOSupply.CreatedDate ASC) AS NoUrut
                    ,spTrnSOSupply.DocNo
                    ,spTrnSOSupply.PartNo 
                    ,spTrnSOSupply.PartNoOriginal
                    ,spTrnSOSupply.SupSeq
                    ,spTrnSOSupply.PTSeq
                    ,isnull(spTrnSORDHdr.ExPickingSlipNo,' ') ExPickingSlipNo                                                           
	                ,CONVERT(decimal(10,5), CONVERT(varbinary(20), spTrnSOSupply.QtySupply)) AS QtyPick
	                ,CONVERT(decimal(10,5), CONVERT(varbinary(20), spTrnSOSupply.QtySupply)) AS QtyPicked                                                           
	                ,CONVERT(decimal(10,5), CONVERT(varbinary(20), spTrnSOSupply.QtyBill)) AS QtyBill                                
                    ,CONVERT(varchar, spTrnSORDHdr.DocDate, 106)
                FROM spTrnSOSupply WITH (nolock, nowait)                               
                    INNER JOIN spTrnSORDHdr ON spTrnSORDHdr.DocNo = spTrnSOSupply.DocNo AND
                        spTrnSORDHdr.CompanyCode = spTrnSOSupply.CompanyCode AND
                        spTrnSORDHdr.BranchCode = spTrnSOSupply.BranchCode
                WHERE spTrnSOSupply.CompanyCode = '{0}'
                AND spTrnSOSupply.BranchCode = '{1}'
                AND spTrnSOSupply.SupSeq = 0 ";

            string DocNoParam = "";
            for (int i = 0; i < DocNoList.Length; i++)
            {

                if (DocNoList.Length == 1)
                {
                    DocNoParam = DocNoParam + " (spTrnSOSupply.DocNo = '" + DocNoList[i] + "' ) ";
                    break;
                }
                else if (i == DocNoList.Length - 1)
                {
                    DocNoParam = DocNoParam + " spTrnSOSupply.DocNo = '" + DocNoList[i] + "'";
                    DocNoParam = " (" + DocNoParam + ") ";
                    break;
                }
                else
                    DocNoParam = DocNoParam + " spTrnSOSupply.DocNo = '" + DocNoList[i] + "' OR ";
            }
            if (DocNoList.Length > 0)
                sql = sql + " AND " + DocNoParam;
            sql = string.Format(sql, CompanyCode, BranchCode);
            var ListPartDetail = ctx.Database.SqlQuery<PickingPartOrderDetail>(sql).ToList();
            return ListPartDetail;
        }

        protected List<PickingPartOrderDetail> Select4TablePLFromBO(string[] DocNoList)
        {
            string sql = @"
            SELECT DISTINCT 
                 row_number () OVER (ORDER BY spTrnSORDDtl.DocNo, spTrnSORDDtl.CreatedDate ASC) AS NoUrut
                ,spTrnSORDDtl.DocNo
                ,spTrnSORDDtl.PartNo 
                ,PartNoOriginal
                ,isnull(spTrnSORDHdr.ExPickingSlipNo,' ') ExPickingSlipNo
                ,(CONVERT(decimal(10,5), CONVERT(varbinary(20), ISNULL(QtyBO, 0))) -
		        CONVERT(decimal(10,5), CONVERT(varbinary(20), ISNULL(QtyBoSupply, 0))) - 
		        CONVERT(decimal(10,5), CONVERT(varbinary(20), ISNULL(QtyBOCancel, 0)))) AS QtyBOOutstd
            FROM spTrnSORDDtl                           
                LEFT JOIN spTrnSORDHdr ON spTrnSORDHdr.DocNo = spTrnSORDDtl.DocNo AND
                    spTrnSORDHdr.CompanyCode = spTrnSORDDtl.CompanyCode AND
                    spTrnSORDHdr.BranchCode = spTrnSORDDtl.BranchCode
            WHERE spTrnSORDDtl.CompanyCode = '{0}'
            AND spTrnSORDDtl.BranchCode = '{1}'
            AND QtyBO - QtyBOSupply - QtyBOCancel > 0
            ";
            string DocNoParam = "";
            for (int i = 0; i < DocNoList.Length; i++)
            {

                if (DocNoList.Length == 1)
                {
                    DocNoParam = DocNoParam + "(spTrnSORDDtl.DocNo = '" + DocNoList[i] + "' )";
                    break;
                }
                else if (i == DocNoList.Length - 1)
                {
                    DocNoParam = DocNoParam + "spTrnSORDDtl.DocNo = '" + DocNoList[i] + "'";
                    DocNoParam = "(" + DocNoParam + ")";
                    break;
                }
                else
                    DocNoParam = DocNoParam + "spTrnSORDDtl.DocNo = '" + DocNoList[i] + "' OR ";
            }

            if (DocNoList.Length > 0)
                sql = sql + " AND " + DocNoParam;


            sql = string.Format(sql, CompanyCode, BranchCode);
            var ListPartDetail = ctx.Database.SqlQuery<PickingPartOrderDetail>(sql, CompanyCode, BranchCode).ToList();
            return ListPartDetail;
        }

        public JsonResult GeneratePL(SpTrnSPickingHdr model, string[] DocNoList, bool isExternal)
        {
            using (TransactionScope TranScope = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout }))
            {
                Object returnObj = null;
                try
                {
                    var errorMgr = IsValidStatus(model.PickingSlipNo);
                    if (!string.IsNullOrEmpty(errorMgr))
                    {
                        return Json(new { success = false, message = errorMgr });
                    }
                    var oPickHdr = ctx.SpTrnSPickingHdrs.Find(CompanyCode, BranchCode, model.PickingSlipNo);
                    if (oPickHdr == null)
                    {
                        oPickHdr = new SpTrnSPickingHdr();
                        oPickHdr.CreatedBy = CurrentUser.UserId;
                        oPickHdr.CreatedDate = DateTime.Now;
                        oPickHdr.CompanyCode = CompanyCode;
                        oPickHdr.BranchCode = BranchCode;
                        oPickHdr.CustomerCode = model.CustomerCode;

                        var oSOHdr = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, DocNoList[0]);
                        oPickHdr.CustomerCodeBill = (oSOHdr == null ? model.CustomerCode : oSOHdr.CustomerCodeBill);
                        oPickHdr.CustomerCodeShip = model.CustomerCode;
                        oPickHdr.PickedBy = model.PickedBy;
                        oPickHdr.IsBORelease = model.IsBORelease;
                        oPickHdr.IsSubstitution = true;
                        oPickHdr.PickingSlipDate = model.PickingSlipDate;

                        var oGnCust = ctx.GnMstCustomers.Find(CompanyCode, model.CustomerCode);
                        if (oGnCust != null)
                        {
                            oPickHdr.IsIncludePPN = (oGnCust.isPKP == null ? false : oGnCust.isPKP.Value);
                        }

                        oPickHdr.TransType = model.TransType;
                        oPickHdr.SalesType = model.SalesType;

                        oPickHdr.Remark = model.Remark;
                        oPickHdr.TotSalesQty = 0;
                        oPickHdr.TotSalesAmt = 0;
                        oPickHdr.TotDiscAmt = 0;
                        oPickHdr.TotDPPAmt = 0;
                        oPickHdr.TotPPNAmt = 0;
                        oPickHdr.TotFinalSalesAmt = 0;
                        oPickHdr.Status = "0";
                        oPickHdr.PrintSeq = 0;
                        oPickHdr.TypeOfGoods = CurrentUser.TypeOfGoods;
                        oPickHdr.LastUpdateBy = CurrentUser.UserId;
                        oPickHdr.LastUpdateDate = DateTime.Now;
                        oPickHdr.IsLocked = false;

                        if (isExternal)
                            oPickHdr.IsLocked = true;
                        else
                            oPickHdr.IsLocked = false;

                        errorMgr = DateTransValidation(model.PickingSlipDate.Date);
                        if (!string.IsNullOrEmpty(errorMgr))
                        {
                            return Json(new { success = false, message = errorMgr });
                        }

                        if (model.IsBORelease)
                        {
                            //generate Back Order
                            BORelease(oPickHdr, DocNoList);
                        }
                        else
                        {
                            GeneratePL(oPickHdr, DocNoList.ToList(), false);
                        }

                        TranScope.Complete();
                        returnObj = new { success = true, message = "Proses Generate PL berhasil", PickingSlipNo = oPickHdr.PickingSlipNo };
                    }
                }
                catch (Exception ex)
                {
                    TranScope.Dispose();
                    returnObj = new { sucess = false, message = "Error pada proses GeneratePL, Message=" + ex.Message.ToString() };
                }


                return Json(returnObj);
            }
        }

        public JsonResult GetDataPickingHdr(string PickingSlipNo)
        {
            var oPickHdr = ctx.SpTrnSPickingHdrs.Find(CompanyCode, BranchCode, PickingSlipNo);
            return Json(new { success = true, data = oPickHdr });
        }

        public decimal GetPPNPct(string CustCode)
        {
            var sql = "exec uspfn_GetPPNPct {0}, {1}, {2}, {3}";
            var PPNPct = ctx.Database.SqlQuery<decimal>(sql, CompanyCode, BranchCode, CustCode, ProfitCenter).FirstOrDefault();
            return PPNPct;
        }

        public int GetSupSeq(string DocNo)
        {
            var sql = "exec uspfn_GetSupSeq {0}, {1}, {2}";
            var PPNPct = ctx.Database.SqlQuery<int?>(sql, CompanyCode, BranchCode, DocNo).FirstOrDefault();
            return PPNPct == null?0: PPNPct.Value;
        }

        public void BORelease(SpTrnSPickingHdr PickHdr, string[] ListDocNo)
        {
            decimal decBO = 0;
            int iSupSeq = 0;
            Dictionary<string, int> aSupSeq = new Dictionary<string, int>();
            //using (TransactionScope transScope = new TransactionScope())
            //{
            //    try
            //    {
                    decimal amtSales = 0;
                    decimal PpnAmt = 0;
                    decimal PpnPct = GetPPNPct(PickHdr.CustomerCode);

                    foreach (var DocNo in ListDocNo)
                    {
                        if (!unitems.Contains(DocNo) && !string.IsNullOrEmpty(DocNo))
                        {
                            SpTrnSORDHdr recsHdrSORLock = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, DocNo);

                            if (recsHdrSORLock == null)
                            {
                                throw new Exception("Proses BO Release Gagal\nData sedang di Locking, Tunggu beberapa saat lagi");
                            }
                            else
                            {
                                recsHdrSORLock.LastUpdateDate = DateTime.Now;
                                recsHdrSORLock.LastUpdateBy = CurrentUser.UserId;
                                ctx.SaveChanges();
                            }

                            iSupSeq = GetSupSeq(DocNo);
                            aSupSeq.Add(DocNo, iSupSeq);
                        }
                    }

                    List<PickingPartOrderDetail> PartPickList = Select4TablePLFromBO(ListDocNo);
                    foreach (var PartPick in PartPickList)
                    {
                        var oSORDHdr = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, PartPick.DocNo);
                        var oSORDtl = ctx.SpTrnSORDDtls.Find(CompanyCode, BranchCode, PartPick.DocNo, PartPick.PartNo, "00", PartPick.PartNoOriginal);

                        //var recPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, oSORDtl.PartNo);
                        var sqlItemPrice = string.Format("SELECT * FROM {0}..spMstItemPrice WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                                GetDbMD(), CompanyMD, BranchMD, oSORDtl.PartNo);
                        var recPrice = ctx.Database.SqlQuery<spMstItemPrice>(sqlItemPrice).FirstOrDefault();

                        oSORDtl.CostPrice = oSORDtl.CostPrice; //recPrice != null ? recPrice.CostPrice : oSORDtl.CostPrice;

                        bool resultSubs = false;

                        decBO = 0;
                        iSupSeq = aSupSeq[PartPick.DocNo];
                        decBO = SaveStockAllocation(oSORDHdr, oSORDtl, PartPick, false, out resultSubs, out amtSales, iSupSeq);

                        if (!resultSubs)
                        {
                            throw new Exception("Proses Insert Sales Order Supply Gagal");
                        }

                        if (PartPick.QtyBOOutstd - decBO > 0)
                        {
                            oSORDtl.QtyBOSupply = oSORDtl.QtyBOSupply + (PartPick.QtyBOOutstd - decBO);
                            oSORDtl.LastUpdateBy = CurrentUser.UserId;
                            oSORDtl.LastUpdateDate = DateTime.Now;
                            ctx.SaveChanges();

                            //UpdateStock(PartPick.PartNo, "00", 0, 0, (PartPick.QtyBOOutstd - decBO) * -1, 0, PickHdr.SalesType);
                        }

                    }
                    var listBOAfterStockAllocation = Select4TablePLFromBOAfterStockAllication(ListDocNo, aSupSeq);

                    if (listBOAfterStockAllocation.Count == 0)
                    {
                        throw new Exception("Available Part tidak mencukupi");
                    }
                    else
                    {
                        GeneratePL(PickHdr, ListDocNo.ToList(), false);
                        PpnAmt = PpnPct > 0 ? Math.Truncate((amtSales * (PpnPct / 100))) : 0;
                        amtSales = amtSales + PpnAmt;
                        if (amtSales > 0)
                        {
                            UpdateHeaderAndBankBook(PickHdr.CustomerCode, PickHdr.SalesType, amtSales);
                        }
                    }
                //    transScope.Complete();
                //}
                //catch (Exception ex)
                //{
                //    transScope.Dispose();
                //    throw new Exception("Error pada function BORelease, Message" + ex.Message.ToString());
                //}
            //}
        }

        private void UpdateHeaderAndBankBook(string CustCode, string salesType, decimal totFinalSalesAmt)
        {
            switch (salesType)
            {
                case "0":
                    BankBook oGnTrnBankBook = ctx.BankBooks.Find(CompanyCode, BranchCode, CustCode, ProfitCenter);
                    var CustPC = ctx.MstCustomerProfitCenters.Find(CompanyCode, BranchCode, CustCode, ProfitCenter);
                    decimal decCreditAlreadyUsed = oGnTrnBankBook == null ? 0 : oGnTrnBankBook.SalesAmt.Value - oGnTrnBankBook.ReceivedAmt.Value;

                    var Lookup = ctx.LookUpDtls.Find(CompanyCode, "TOPC", CustPC.TOPCode);
                    if (Lookup != null)
                    {
                        if (Lookup.ParaValue != "0")
                        {
                            if (CustPC.CreditLimit < decCreditAlreadyUsed + totFinalSalesAmt && CurrentUser.CoProfile.IsLinkToFinance.Value)
                            {
                                throw new Exception(ctx.SysMsgs.Find("5024").MessageCaption);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Kode TOP belum disetting untuk pelanggan ini");
                    }
                    break;
                default:
                    break;
            }

            if (salesType == "0")
            {
                UpdateBankBook(CustCode, totFinalSalesAmt, true);
            }
        }

        private void UpdateBankBook(string CustCode, decimal amount, bool isSales)
        {
            BankBook oBankBook = ctx.BankBooks.Find(CompanyCode, BranchCode, CustCode, ProfitCenter);
            try
            {
                if (oBankBook == null)
                {
                    oBankBook = new BankBook();
                    oBankBook.CompanyCode = CompanyCode;
                    oBankBook.BranchCode = BranchCode;
                    oBankBook.CustomerCode = CustCode;
                    oBankBook.ProfitCenterCode = ProfitCenter;
                }
                if (isSales)
                    oBankBook.SalesAmt += amount;
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error pada function UpdateBankBook, Message");
            }
        }

        private List<PickingPartOrderDetailAS> Select4TablePLFromBOAfterStockAllication(string[] ListDocNo, Dictionary<string, int> aSupSeq)
        {
            var sql = @"SELECT DISTINCT spTrnSOSupply.DocNo
                ,spTrnSOSupply.PartNo 
                ,spTrnSOSupply.PartNoOriginal
                ,spTrnSORDHdr.ExPickingSlipNo                                                            
                ,0 AS QtyBoSupply
                ,spTrnSOSupply.QtyPicked AS QtyPick                                
                ,spTrnSOSupply.QtyPicked
                ,0 AS QtyBill
                ,spTrnSORDHdr.DocDate
                ,spTrnSOSupply.SupSeq
                ,spTrnSOSupply.PTSeq
                FROM spTrnSOSupply WITH (nolock, nowait)                               
                    LEFT JOIN spTrnSORDDtl WITH (nolock, nowait) ON spTrnSORDDtl.DocNo = spTrnSOSupply.DocNo AND
                        spTrnSORDDtl.CompanyCode = spTrnSOSupply.CompanyCode AND
                        spTrnSORDDtl.BranchCode = spTrnSOSupply.BranchCode
                    LEFT JOIN spTrnSORDHdr WITH (nolock, nowait) ON spTrnSORDHdr.DocNo = spTrnSORDDtl.DocNo AND
                        spTrnSORDHdr.CompanyCode = spTrnSORDDtl.CompanyCode AND
                        spTrnSORDHdr.BranchCode = spTrnSORDDtl.BranchCode
                WHERE spTrnSOSupply.CompanyCode = {0}
                AND spTrnSOSupply.BranchCode = {1} ";
            var idx = 0;
            var commandSql = "";
            var unitem = ListDocNo.ToList().IndexOf("-");
            List<string> DocList = ListDocNo.ToList();
            if (unitem > -1)
                DocList.RemoveAt(unitem);
            foreach (var DocNo in DocList)
            {
                if (!unitems.Contains(DocNo) && !string.IsNullOrEmpty(DocNo))
                {
                    var DocumentNo = DocNo;
                    var SupSeq = aSupSeq[DocNo];
                    if (DocList.Count == 1 || idx == DocList.Count - 1)
                    {
                        commandSql += " (spTrnSOSupply.DocNo = '" + DocNo + "' AND SupSeq = '" + SupSeq + "') ";
                        break;
                    }
                    else
                    {
                        commandSql += " (spTrnSOSupply.DocNo = '" + DocNo + "' AND SupSeq = '" + SupSeq + "') OR ";
                    }
                }
            }
            if (DocList.Count > 0)
            {
                sql = sql + " AND " + commandSql;
            }
            sql = sql + " ORDER BY spTrnSOSupply.DocNo ";
            var listPart = ctx.Database.SqlQuery<PickingPartOrderDetailAS>(sql, CompanyCode, BranchCode).ToList();
            return listPart;
        }

        public decimal SaveStockAllocation(SpTrnSORDHdr oSpTrnSORDHdr, SpTrnSORDDtl oSpTrnSORDDtl, PickingPartOrderDetail PartPick, bool IsAlloc, out bool resultSubs, out decimal amtSales, decimal supSeq)
        {
            decimal decQtyOrder = PartPick.QtyBOOutstd;
            try
            {
                amtSales = 0;
                decimal availItem = 0;
                decimal qtyNewSupply = 0;

                bool md = DealerCode() == "MD";

                SpMstItemLoc oSpMstItemLoc = null;
                spMstItem oSpMstItems = null;
                SpTrnSOSupply oSpTrnSOSupply = null;

                spMstItemMod oSpMstItemMod = null;
                List<spMstItemMod> lstSpMstItemMod = new List<spMstItemMod>();

                bool isGetSusbtitution = oSpTrnSORDHdr.isSubstitution.Value;

                // update
                if (md)
                {
                    oSpMstItemLoc = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, PartPick.PartNo, "00");
                    oSpMstItems = ctx.spMstItems.Find(CompanyCode, BranchCode, PartPick.PartNo);
                }
                else
                {
                    var ItemLoc = @"select * from " + GetDbMD() + @"..SpMstItemLoc where CompanyCode='" + CompanyMD + "' AND BranchCode='" + BranchMD + "' and PartNo ='" + PartPick.PartNo + "' and WarehouseCode ='00'";
                    oSpMstItemLoc = ctx.Database.SqlQuery<SpMstItemLoc>(ItemLoc).FirstOrDefault();

                    var Item = @"select * from " + GetDbMD() + @"..SpMstItems where CompanyCode='" + CompanyMD + "' AND BranchCode='" + BranchMD + "' and PartNo ='" + PartPick.PartNo + "'";
                    oSpMstItems = ctx.Database.SqlQuery<spMstItem>(Item).FirstOrDefault();
                }

                //SpMstItemLoc oSpMstItemLoc = md ? ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, PartPick.PartNo, "00") : ctxMD.SpMstItemLocs.Find(CompanyMD, BranchMD, PartPick.PartNo, "00");
                //spMstItem oSpMstItems = md ? ctx.spMstItems.Find(CompanyCode, BranchCode, PartPick.PartNo) : ctxMD.spMstItems.Find(CompanyMD, BranchMD, PartPick.PartNo);
                
                availItem = oSpMstItemLoc.OnHand.Value -
                            (oSpMstItemLoc.AllocationSP.Value +
                                oSpMstItemLoc.AllocationSR.Value +
                                oSpMstItemLoc.AllocationSL.Value) -
                            (oSpMstItemLoc.ReservedSP.Value +
                                oSpMstItemLoc.ReservedSR.Value +
                                oSpMstItemLoc.ReservedSL.Value);
                
                if (availItem > 0 && PartPick.QtyBOOutstd <= availItem)
                    isGetSusbtitution = false;

                if (isGetSusbtitution)
                {
                    lstSpMstItemMod = fc_SelectModifikasi(oSpTrnSORDDtl.PartNo);
                }

                if (!isGetSusbtitution || lstSpMstItemMod.Count <= 0)
                {
                    oSpMstItemMod = new spMstItemMod();
                    oSpMstItemMod.PartNo = oSpTrnSORDDtl.PartNo;
                    lstSpMstItemMod.Add(oSpMstItemMod);
                }

                resultSubs = false;
                decimal SalesAmt = 0
                        , DiscAmt = 0
                        , DppAmt = 0;
                foreach (var oSpMstItemModTemp in lstSpMstItemMod)
                {
                    oSpMstItemLoc = null; oSpMstItems = null;

                    // update
                    if (md)
                    {
                        oSpMstItemLoc = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, PartPick.PartNo, "00");
                        oSpMstItems = ctx.spMstItems.Find(CompanyCode, BranchCode, PartPick.PartNo);
                    }
                    else
                    {
                        var ItemLoc = @"select * from " + GetDbMD() + @"..SpMstItemLoc where CompanyCode='" + CompanyMD + "' AND BranchCode='" + BranchMD + "' and PartNo ='" + PartPick.PartNo + "' and WarehouseCode ='00'";
                        oSpMstItemLoc = ctx.Database.SqlQuery<SpMstItemLoc>(ItemLoc).FirstOrDefault();

                        var Item = @"select * from " + GetDbMD() + @"..SpMstItems where CompanyCode='" + CompanyMD + "' AND BranchCode='" + BranchMD + "' and PartNo ='" + PartPick.PartNo + "'";
                        oSpMstItems = ctx.Database.SqlQuery<spMstItem>(Item).FirstOrDefault();
                    }
                    //oSpMstItemLoc = md ? ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, PartPick.PartNo, "00") : ctxMD.SpMstItemLocs.Find(CompanyMD, BranchMD, PartPick.PartNo, "00");
                    //oSpMstItems = md ? ctx.spMstItems.Find(CompanyCode, BranchCode, PartPick.PartNo) : ctxMD.spMstItems.Find(CompanyMD, BranchMD, PartPick.PartNo);

                    if (oSpMstItemLoc == null || oSpMstItems == null)
                        availItem = 0;
                    else if (!oSpMstItems.TypeOfGoods.Equals(oSpTrnSORDHdr.TypeOfGoods) ||
                             !oSpMstItems.ProductType.Equals(oSpTrnSORDDtl.ProductType))
                        availItem = 0;
                    else
                        availItem = oSpMstItemLoc.OnHand.Value -
                                   (oSpMstItemLoc.AllocationSP.Value +
                                    oSpMstItemLoc.AllocationSR.Value +
                                    oSpMstItemLoc.AllocationSL.Value) -
                                   (oSpMstItemLoc.ReservedSP.Value +
                                    oSpMstItemLoc.ReservedSR.Value +
                                    oSpMstItemLoc.ReservedSL.Value);

                    if (availItem > 0)
                    {
                        qtyNewSupply = availItem > decQtyOrder ? decQtyOrder : availItem;

                        // prepare data supply 
                        oSpTrnSOSupply = prepareSupplyData(oSpMstItems, oSpTrnSORDHdr, oSpMstItemModTemp.PartNo, PartPick.PartNo, oSpMstItemLoc.LocationCode,
                                        qtyNewSupply, supSeq, oSpTrnSORDDtl.RetailPrice.Value, oSpTrnSORDDtl.RetailPriceInclTax.Value, oSpTrnSORDDtl.CostPrice.Value, oSpTrnSORDDtl.DiscPct.Value);

                        // insert spTrnSOSupply, update item dan itemloc
                        resultSubs = UpdateStockAndSupply(oSpTrnSOSupply, oSpTrnSORDHdr, qtyNewSupply, IsAlloc);

                        if (!resultSubs)
                            break;

                        // hitung sales amount untuk part no yang dialokasi
                        SalesAmt = oSpTrnSOSupply.RetailPrice.Value * qtyNewSupply;
                        DiscAmt = Math.Round(SalesAmt * oSpTrnSOSupply.DiscPct.Value / 100, 0, MidpointRounding.AwayFromZero);
                        DppAmt = SalesAmt - DiscAmt;
                        amtSales += DppAmt;
                    }
                    else
                    {
                        qtyNewSupply = 0;
                        resultSubs = true;
                    }

                    decQtyOrder = decQtyOrder - qtyNewSupply;

                    if (decQtyOrder <= 0)
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error pada function SaveStockAllocation, Message=" + ex.Message.ToString());
            }
            return decQtyOrder;
        }

        private bool UpdateStockAndSupply(SpTrnSOSupply oSpTrnSOSupply,
                                   SpTrnSORDHdr recordHeader, decimal itemOnHand, bool isAlloc)
        {
            bool retVal = false;
            try
            {
                // prepare dao object for current transaction
                ctx.SpTrnSOSupplys.Add(oSpTrnSOSupply);
                ctx.SaveChanges();

                // update stock 
                //UpdateStock(oSpTrnSOSupply.PartNo, "00", 0, itemOnHand, 0, 0, recordHeader.SalesType);


                // insert movement log
                MovementLog(recordHeader.DocNo, recordHeader.DocDate.Value, oSpTrnSOSupply.PartNo,
                            "00", "OUT", isAlloc ? (recordHeader.SalesType.Equals("0") ? "SA-PJUAL" :
                            "SA-NPJUAL") : (recordHeader.SalesType.Equals("0") ? "BR-PJUAL" : "BR-NPJUAL"), itemOnHand);
                retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
            }
            return retVal;
        }

        private SpTrnSOSupply prepareSupplyData(spMstItem oSpMstItemsDao,
            SpTrnSORDHdr oSpTrnSORDHdr, string partNo, string partNoOriginal,
            string locationCode, decimal qtySupply, decimal supSeq,
            decimal retailPrice, decimal retailPriceIncTax,
            decimal costPrice, decimal discPct)
        {
            SpTrnSOSupply oSpTrnSOSupply = new SpTrnSOSupply();
            try
            {
                if (oSpTrnSORDHdr == null)
                {
                    throw new Exception(string.Format(ctx.SysMsgs.Find("5034").MessageCaption, "header data"));
                }

                oSpTrnSOSupply.CompanyCode = oSpTrnSORDHdr.CompanyCode;
                oSpTrnSOSupply.BranchCode = oSpTrnSORDHdr.BranchCode;
                oSpTrnSOSupply.DocNo = oSpTrnSORDHdr.DocNo;
                oSpTrnSOSupply.SupSeq = int.Parse(supSeq.ToString());
                oSpTrnSOSupply.PartNo = partNo;
                oSpTrnSOSupply.PartNoOriginal = partNoOriginal;
                oSpTrnSOSupply.PTSeq = GetPTSeq(oSpTrnSORDHdr.DocNo, supSeq);
                oSpTrnSOSupply.ReferenceNo = oSpTrnSORDHdr.OrderNo;
                oSpTrnSOSupply.ReferenceDate = oSpTrnSORDHdr.OrderDate;
                oSpTrnSOSupply.WarehouseCode = "00";
                oSpTrnSOSupply.LocationCode = locationCode;
                oSpTrnSOSupply.QtySupply = qtySupply;
                oSpTrnSOSupply.QtyPicked = 0;
                oSpTrnSOSupply.QtyBill = 0;

                if (partNoOriginal != partNo)
                {
                    spMstItemPrice recItemPrice = ctx.spMstItemPrices.Find(oSpTrnSORDHdr.CompanyCode, oSpTrnSORDHdr.BranchCode, partNo);
                    if (recItemPrice != null)
                    {
                        oSpTrnSOSupply.RetailPrice = recItemPrice.RetailPrice;
                        oSpTrnSOSupply.RetailPriceInclTax = recItemPrice.RetailPriceInclTax;
                        oSpTrnSOSupply.CostPrice = costPrice; //recItemPrice.CostPrice;
                    }
                }
                else
                {
                    oSpTrnSOSupply.RetailPrice = retailPrice;
                    oSpTrnSOSupply.RetailPriceInclTax = retailPriceIncTax;
                    oSpTrnSOSupply.CostPrice = costPrice;
                }

                spMstItem oSpMstItems = ctx.spMstItems.Find(CompanyCode, BranchCode, partNo);
                if (oSpMstItems != null)
                {
                    oSpTrnSOSupply.MovingCode = oSpMstItems.MovingCode;
                    oSpTrnSOSupply.ABCClass = oSpMstItems.ABCClass;
                    oSpTrnSOSupply.ProductType = oSpMstItems.ProductType;
                    oSpTrnSOSupply.PartCategory = oSpMstItems.PartCategory;
                }

                SpMstItemInfo oSpMstItemInfo = ctx.SpMstItemInfos.Find(CurrentUser.CompanyCode, partNo);
                string suppCode = oSpMstItemInfo == null ? string.Empty : oSpMstItemInfo.SupplierCode;

                oSpTrnSOSupply.DiscPct = discPct;
                oSpTrnSOSupply.Status = "0";
                oSpTrnSOSupply.CreatedBy = CurrentUser.UserId;
                oSpTrnSOSupply.CreatedDate = DateTime.Now;
                oSpTrnSOSupply.LastUpdateBy = CurrentUser.UserId;
                oSpTrnSOSupply.LastUpdateDate = DateTime.Now;
            }
            catch (Exception ex)
            {
                throw new Exception("Error pada function prepareSupplyData, Message=" + ex.Message.ToString());
            }
            return oSpTrnSOSupply;
        }

        public List<spMstItemMod> fc_SelectModifikasi(string PartNo)
        {
            List<spMstItemMod> ItemModList = new List<spMstItemMod>();
            var sql = string.Format("SELECT * FROM GetModifikasi( '{0}')", PartNo);
            var listModif = ctx.Database.SqlQuery<Modifikasi>(sql);
            foreach (var Modif in listModif)
            {
                var itemMod = new spMstItemMod();
                itemMod.PartNo = Modif.ID;
                itemMod.InterChangeCode = Modif.InterChangeCode;
                ItemModList.Add(itemMod);
            }
            return ItemModList;
        }

        private int GetPTSeq(string DocNo, decimal supSeq)
        {
            var sql = string.Format("exec uspfn_GetPTSeq '{0}','{1}','{2}',{3}", CompanyCode, BranchCode, DocNo, supSeq);
            var PTSeq = ctx.Database.SqlQuery<int>(sql).FirstOrDefault();
            return PTSeq;
        }

        protected void InsertSpTrnSPickingHdrDtl(SpTrnSPickingHdr oPickHdr, List<string> listDocNo, bool bGenerated)
        {
            try
            {
                if (oPickHdr.PickingSlipNo.EndsWith("X"))
                {
                    throw new Exception(string.Format(ctx.SysMsgs.Find("5046").MessageCaption, "Picking List"));
                }
                ctx.SpTrnSPickingHdrs.Add(oPickHdr);
                ctx.SaveChanges();

                var qrDb = "SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode='" + CompanyCode + "' AND BranchCode='" + BranchCode + "'";
                string DBMD = ctx.Database.SqlQuery<string>(qrDb).FirstOrDefault();

                int iSeq = 1;
                //Insert Picking Dtl
                foreach (var DocNo in listDocNo)
                {
                    //get spTrnSORDDtl
                    var oTrnSOSupplyList = ctx.SpTrnSOSupplys.Where(m => m.CompanyCode == CompanyCode && m.BranchCode == BranchCode && m.DocNo == DocNo).ToList();
                    foreach (var oTrnSOSupply in oTrnSOSupplyList)
                    {
                        var recordSOSupply = ctx.SpTrnSOSupplys.Find(CompanyCode, BranchCode, oTrnSOSupply.DocNo,
                            oTrnSOSupply.SupSeq, oTrnSOSupply.PartNo, oTrnSOSupply.PartNoOriginal, oTrnSOSupply.PTSeq);

                        SpTrnSPickingDtl oPickingDtl = new SpTrnSPickingDtl();
                        if (recordSOSupply != null)
                        {
                            oPickingDtl.WarehouseCode = recordSOSupply.WarehouseCode;
                            oPickingDtl.RetailPriceInclTax = recordSOSupply.RetailPriceInclTax;
                            oPickingDtl.RetailPrice = recordSOSupply.RetailPrice;
                            oPickingDtl.CostPrice = recordSOSupply.CostPrice;
                            oPickingDtl.DiscPct = recordSOSupply.DiscPct;
                            oPickingDtl.ReferenceNo = recordSOSupply.ReferenceNo;
                            oPickingDtl.ReferenceDate = recordSOSupply.ReferenceDate;
                            oPickingDtl.LocationCode = recordSOSupply.LocationCode;

                            // [Add by Beny on Feb-23-2009]
                            oPickingDtl.SalesAmt = recordSOSupply.QtySupply * recordSOSupply.RetailPrice;
                            oPickingDtl.DiscAmt = Math.Round(decimal.Parse((oPickingDtl.SalesAmt.Value * (recordSOSupply.DiscPct.Value / 100)).ToString()), 0, MidpointRounding.AwayFromZero);
                            oPickingDtl.NetSalesAmt = oPickingDtl.SalesAmt - oPickingDtl.DiscAmt;
                            oPickingDtl.TotSalesAmt = oPickingDtl.SalesAmt - oPickingDtl.DiscAmt;
                        }
                        oPickingDtl.CompanyCode = CompanyCode;
                        oPickingDtl.BranchCode = BranchCode;
                        oPickingDtl.PickingSlipNo = oPickHdr.PickingSlipNo;
                        oPickingDtl.DocNo = oTrnSOSupply.DocNo;
                        oPickingDtl.PartNo = oTrnSOSupply.PartNo;
                        oPickingDtl.PartNoOriginal = oTrnSOSupply.PartNoOriginal;
                        oPickingDtl.QtyBill = 0;
                        oPickingDtl.isClosed = false;

                        var oTrnSorHdr = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, DocNo);
                        if (oTrnSorHdr != null) oPickingDtl.DocDate = oTrnSorHdr.DocDate;

                        if (bGenerated)
                        {
                            oPickingDtl.ExPickingSlipNo = oTrnSorHdr.ExPickingSlipNo;
                            oPickingDtl.ExPickingSlipDate = oTrnSorHdr.ExPickingSlipDate;
                        }

                        oPickingDtl.QtySupply = recordSOSupply.QtySupply;
                        oPickingDtl.QtyOrder = oPickingDtl.QtySupply;
                        oPickingDtl.QtyPicked = oPickingDtl.QtySupply;

                        var queryItems = string.Format("SELECT * FROM " + DBMD + "..spMstItems WHERE CompanyCode='{0}' and BranchCode ='{1}' and PartNo='{2}'", CompanyMD, BranchMD, oPickingDtl.PartNo);
                        var oMstItem = ctx.Database.SqlQuery<spMstItem>(queryItems).FirstOrDefault();
                        if (oMstItem != null)
                        {
                            oPickingDtl.ABCClass = oMstItem.ABCClass;
                            oPickingDtl.MovingCode = oMstItem.MovingCode;
                            oPickingDtl.PartCategory = oMstItem.PartCategory;
                            oPickingDtl.ProductType = oMstItem.ProductType;
                        }

                        oPickingDtl.CreatedBy = CurrentUser.UserId;
                        oPickingDtl.CreatedDate = DateTime.Now;
                        oPickingDtl.LastUpdateBy = CurrentUser.UserId;
                        oPickingDtl.LastUpdateDate = DateTime.Now;

                        Helpers.ReplaceNullable(oPickingDtl);
                        ctx.SpTrnSPickingDtls.Add(oPickingDtl);

                        //Insert svSDMovement
                        if (IsMD == false)
                        {
                            var sqlItemPrice = string.Format("SELECT * FROM {0}..spMstItemPrice WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                                            GetDbMD(), CompanyMD, BranchMD, oTrnSOSupply.PartNo);
                            spMstItemPrice oItemPrice = ctx.Database.SqlQuery<spMstItemPrice>(sqlItemPrice).FirstOrDefault();
                        
                       
                            var iQry = @"insert into " + DBMD + @"..svSDMovement (CompanyCode, BranchCode, DocNo, DocDate, PartNo, PartSeq, WarehouseCode, QtyOrder, Qty, DiscPct, CostPrice, RetailPrice,   
	                                TypeOfGoods, CompanyMD, BranchMD, WarehouseMD, RetailPriceInclTaxMD, RetailPriceMD, CostPriceMD, QtyFlag, ProductType, ProfitCenterCode, 
	                                Status, ProcessStatus, ProcessDate, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate) 
                                    VALUES('" + CompanyCode + "','" + BranchCode + "','" + oPickHdr.PickingSlipNo + "','" + ctx.CurrentTime + "','" + oTrnSOSupply.PartNo +
                                        "','" + iSeq + "','" + recordSOSupply.WarehouseCode + "'," + Convert.ToDecimal(oPickingDtl.QtyPicked.Value) + "," + Convert.ToDecimal(oPickingDtl.QtyPicked.Value) + "," + Convert.ToDecimal(recordSOSupply.DiscPct.Value) + "," + Math.Floor(recordSOSupply.CostPrice.Value) +
                                        ",'" + recordSOSupply.RetailPrice + "','" + CurrentUser.TypeOfGoods + "','" + CompanyMD + "','" + BranchMD + "','" + recordSOSupply.WarehouseCode + "','" + oItemPrice.RetailPriceInclTax + "','" + oItemPrice.RetailPrice +
                                        "','" + oItemPrice.CostPrice + "','x','" + oMstItem.ProductType + "','300','0','0','" + ctx.CurrentTime + "','" + CurrentUser.UserId + "','" + ctx.CurrentTime + "','" + CurrentUser.UserId + "','" + ctx.CurrentTime + "')"+
                                         "UPDATE "+ DBMD + @"..svSDMovement 
                                        SET QtyOrder = "+ Convert.ToDecimal(oPickingDtl.QtyPicked.Value) +", Qty = "+ Convert.ToDecimal(oPickingDtl.QtyPicked.Value) +",LastUpdateBy = '"+ CurrentUser.UserId +"',"+
                                            "LastUpdateDate = '"+ctx.CurrentTime+"' WHERE CompanyCode = '"+ CompanyCode +"' AND BranchCode = '"+ BranchCode +"' AND DocNo = '"+ oPickingDtl.DocNo +"' AND PartNo = '"+  oTrnSOSupply.PartNo +"'";
                            
                            var sqlCount = string.Format("select count(*) as count from " + DBMD + @"..svHstSDMovement
                                    where CompanyCode = '{0}' 
                                    and BranchCode = '{1}'
                                    and DocNo = '{2}'
                                    and PartNo = '{3}'", CompanyCode, BranchCode, oPickHdr.PickingSlipNo, oTrnSOSupply.PartNo);
                            var count = (int)ctx.Database.SqlQuery<int>(sqlCount).FirstOrDefault();
                            
                            if(count > 0){
                                var qty = Convert.ToDecimal(oPickingDtl.QtyPicked.Value);
                                iQry = string.Format("update " + DBMD + @"..svSDMovement 
                                    set QtyOrder += {0}, Qty += {1}
                                    where CompanyCode = '{2}' 
                                    and BranchCode = '{3}'
                                    and DocNo = '{4}'
                                    and PartNo = '{5}'",
                                    qty, qty, CompanyCode, BranchCode, oPickHdr.PickingSlipNo, oTrnSOSupply.PartNo);
                            }
                            else
                            {
                                iSeq = iSeq + 1;
                            }

                            ctx.Database.ExecuteSqlCommand(iQry);
                        }
                    }
                }
                
                ctx.SaveChanges();

                if (IsMD == false)
                {
                    var sql = string.Format("exec uspfn_UpdateSvSDMovement '{0}','{1}','{2}'", CompanyCode, BranchCode, oPickHdr.PickingSlipNo);
                    ctx.Database.ExecuteSqlCommand(sql);
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error pada function InsertSpTrnSPickingHdrDtl, Message=" + ex.Message.ToString());
            }
        }

        protected void UpdateSORDHdr(List<string> listDocNo, bool bGenerated, string PickingSlipNo)
        {
            try
            {
                var sql = "";
                foreach (var DocNo in listDocNo)
                {
                    if (!bGenerated)
                    {
                        sql = @"UPDATE spTrnSORDHdr
                                SET Status = 4,
                                    LastUpdateBy = '{0}',
                                    LastUpdateDate = '{1}',
                                    ExPickingSlipNo = '{2}',
                                    ExPickingSlipDate = '{1}'    
                                WHERE spTrnSORDHdr.DocNo in (select distinct docno from sptrnspickingdtl where 
                                    companycode = '{3}' and branchcode = '{4}' and pickingslipno = '{2}') AND
                                    CompanyCode = '{3}' AND
                                    BranchCode = '{4}' ";
                    }
                    else
                    {
                        sql = @"UPDATE spTrnSORDHdr
                                SET Status = 4,
                                    LastUpdateBy = {0},
                                    LastUpdateDate = {1}
                                WHERE spTrnSORDHdr.DocNo in (select distinct docno from sptrnspickingdtl where 
                                    companycode = '{3}' and branchcode = '{4}' and pickingslipno = '{2}') AND
                                    CompanyCode = '{3}' AND
                                    BranchCode = '{4}'";
                    }

                    sql = string.Format(sql, CurrentUser.UserId, DateTime.Now, PickingSlipNo, CompanyCode, BranchCode);
                    ctx.Database.ExecuteSqlCommand(sql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Pada function UpdateSORDHdr, Message=" + ex.Message.ToString());
            }
        }

        protected void UpdateAmountPickingHdr(string PickingSlipNo)
        {
            try
            {
                var oPickingHdr = ctx.SpTrnSPickingHdrs.Find(CompanyCode, BranchCode, PickingSlipNo);
                oPickingHdr.TotSalesQty = GetTotSupplyQty(PickingSlipNo);
                oPickingHdr.TotSalesAmt = GetTotSalesAmt(PickingSlipNo);
                oPickingHdr.TotDiscAmt = GetTotDiscAmt(PickingSlipNo);
                oPickingHdr.TotDPPAmt = GetTotDPPAmt(PickingSlipNo);

                if (oPickingHdr.SalesType == "1")
                {
                    oPickingHdr.TotPPNAmt = 0;
                }
                else
                {
                    var CustPC = ctx.MstCustomerProfitCenters.Find(CompanyCode, BranchCode, oPickingHdr.CustomerCode, ProfitCenter);
                    var Tax = (string.IsNullOrEmpty(CustPC.TaxCode) ? null : ctx.Taxes.Find(CompanyCode, CustPC.TaxCode));

                    oPickingHdr.TotPPNAmt = (Tax != null) ? decimal.Parse(Math.Truncate(decimal.Parse((oPickingHdr.TotDPPAmt * Tax.TaxPct / 100).ToString())).ToString()) : 0;
                }

                oPickingHdr.TotFinalSalesAmt = oPickingHdr.TotPPNAmt + oPickingHdr.TotDPPAmt;
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error pada function UpdateAmountPickingHdr, Message=" + ex.Message.ToString());
            }
        }

        private void GeneratePL(SpTrnSPickingHdr oPickHdr, List<string> listDocNo, bool bGenerated)
        {

            //using (TransactionScope trans = new TransactionScope())
            //{
                try
                {
                    //Generate New picking slip No
                    var sql = string.Format("exec uspfn_GnDocumentGetNew '{0}','{1}','{2}','{3}','{4}'", CompanyCode, BranchCode, "PLS", CurrentUser.UserId, oPickHdr.PickingSlipDate.Date.ToString("yyyy/MM/dd"));
                    oPickHdr.PickingSlipNo = ctx.Database.SqlQuery<string>(sql).FirstOrDefault();

                    //insert SpTrnSPickingHdr and SpTrnSPickingDtl
                    InsertSpTrnSPickingHdrDtl(oPickHdr, listDocNo, bGenerated);

                    // Update SORDHdr, set status = 4 (PL Generated), ExPickingSlipNo = No PL, ExPickingSlipDate = Date PL
                    UpdateSORDHdr(listDocNo, bGenerated, oPickHdr.PickingSlipNo);

                    // Update amount in spTrnSPickingHdr
                    UpdateAmountPickingHdr(oPickHdr.PickingSlipNo);

                    // Update spTrnSOSupply, set PickingSlipNo and update status = 1.
                    // Update spTrnSORDDtl, set status = 4 (PL Generated).
                    GeneratePL(listDocNo, oPickHdr.PickingSlipNo, bGenerated);
                    //trans.Complete();
                }
                catch (Exception ex)
                {
                    //trans.Dispose();
                    throw new Exception("Error pada function GeneratePL, Message=" + ex.Message.ToString());
                }
            //}
        }

        protected void GeneratePL(List<string> listDocNo, string pickingSlipNo, bool bGenerated)
        {
            try
            {
                foreach (var docNo in listDocNo)
                {
                    var sql = @"UPDATE spTrnSOSupply 
                SET PickingSlipNo = '{0}',
                    Status = 1,
                    LastUpdateBy = '{1}',
                    LastUpdateDate = '{2}'
                WHERE CompanyCode = '{3}' AND
                    BranchCode = '{4}' AND
                    DocNo = '{5}'
                    
                    UPDATE a
                    SET a.Status = 4,
                    LastUpdateBy = '{1}',
                    LastUpdateDate = '{2}'
                    FROM spTrnSORDDtl a
                    INNER JOIN spTrnSOSupply b on 
                    a.CompanyCode = b.CompanyCode
                    AND a.BranchCode = b.BranchCode
                    AND a.DocNo = b.DocNo
                    AND a.PartNo = b.PartNoOriginal
                    WHERE a.CompanyCode = '{3}' AND
                        a.BranchCode = '{4}' AND
                        a.DocNo = '{5}' AND
                        a.WarehouseCode = '00'
                    ";

                    sql = string.Format(sql, pickingSlipNo, CurrentUser.UserId, DateTime.Now.ToShortDateString(), CompanyCode, BranchCode, docNo);
                    ctx.Database.ExecuteSqlCommand(sql);

                    //var oTrnSOSupply = ctx.SpTrnSOSupplys.Where(m => m.CompanyCode == CompanyCode && m.BranchCode == BranchCode && m.DocNo == docNo);

                    //foreach (var soSupply in oTrnSOSupply)
                    //{
                        //soSupply = ctx.SpTrnSOSupplys.Find(CompanyCode, BranchCode, soSupply.DocNo, soSupply.SupSeq, soSupply.PartNo, soSupply.PartNoOriginal, soSupply.PTSeq);

                        //if (string.IsNullOrEmpty(soSupply.PickingSlipNo))
                        //{
                        //    soSupply.PickingSlipNo = pickingSlipNo;
                        //    soSupply.Status = "1";
                        //    soSupply.LastUpdateBy = CurrentUser.UserId;
                        //    soSupply.LastUpdateDate = DateTime.Now;
                        //    ctx.SaveChanges();
                        //}
                        //else
                        //{
                        //    throw new Exception(string.Format(ctx.SysMsgs.Find("5045").MessageCaption, docNo, "generate"));
                        //}

                        ////soSupply = ctx.SpTrnSOSupplys.Find(CompanyCode, BranchCode, soSupply.DocNo, soSupply.SupSeq, soSupply.PartNo, soSupply.PartNoOriginal, soSupply.PTSeq);

                        //if (soSupply.PickingSlipNo == "")
                        //{
                        //    throw new Exception("Terjadi kesalahan dalam pencatatan nomor picking slip di sales order, tekan toolbar New untuk me-refresh screen dan lakukan kembali proses picking !");
                        //}

                        //ctx.SaveChanges();

                        // Update status=4 di table spTRNSORDDtl pada saat GeneratePL/BO Release
                    //    UpdateStatusspTRNSORDDtl(docNo, soSupply.PartNoOriginal);
                    //}
                    
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Proses Update Sales Supply Gagal, Message=" + ex.Message.ToString());
            }
        }

        public JsonResult UpdatePrintStatus(SpTrnSPickingHdr model)
        {
            Object returnObj = null;
            var oPickHdr = ctx.SpTrnSPickingHdrs.Find(CompanyCode, BranchCode, model.PickingSlipNo);
            if (oPickHdr != null)
            {
                var errorMsg = IsValidStatus(model.PickingSlipNo);
                if (string.IsNullOrEmpty(errorMsg))
                {
                    oPickHdr.PrintSeq += 1;
                    if (Convert.ToInt32(oPickHdr.Status) < 2)
                        oPickHdr.Status = "1";
                    oPickHdr.LastUpdateBy = CurrentUser.UserId;
                    oPickHdr.LastUpdateDate = DateTime.Now;
                    ctx.SaveChanges();
                    returnObj = new { success = true, message = "" };
                }
                else
                {
                    returnObj = new { success = false, message = errorMsg };
                }
            }
            else
            {
                returnObj = new { success = false, message = "Maaf data yang ingin diupdate tidak ada." };
            }
            return Json(returnObj);
        }

        public JsonResult UpdateRemarkPickingHdr(string PickingSlipNo, string Remark)
        {
            Object returnObj = null;
            var oPickHdr = ctx.SpTrnSPickingHdrs.Find(CompanyCode, BranchCode, PickingSlipNo);
            if (oPickHdr != null)
            {
                var errorMsg = IsValidStatus(PickingSlipNo);
                if (string.IsNullOrEmpty(errorMsg))
                {
                    oPickHdr.Remark = Remark;
                    oPickHdr.LastUpdateBy = CurrentUser.UserId;
                    oPickHdr.LastUpdateDate = DateTime.Now;
                    ctx.SaveChanges();
                    returnObj = new { success = true, message = "" };
                }
                else
                {
                    returnObj = new { success = false, message = errorMsg };
                }
            }
            else
            {
                returnObj = new { success = false, message = "Maaf data yang ingin diupdate tidak ada." };
            }
            return Json(returnObj);
        }

        public JsonResult UpdatePickPartDetail(PickingPartOrderDetail model, string PickingSlipNo)
        {
            Object returnObj = null;
            string errorMsg = IsValidStatus(PickingSlipNo);
            if (!string.IsNullOrEmpty(errorMsg))
            {
                returnObj = new { success = false, message = errorMsg };
                return Json(returnObj);
            }

            //if (model.QtyPicked < 1)
            //{
            //    errorMsg = "Minimum quantity pengambilan adalah 1";
            //    returnObj = new { success = false, message = errorMsg };
            //    return Json(returnObj);
            //}

            if (model.QtyPicked > model.QtyPick)
            {
                errorMsg = ctx.SysMsgs.Find("5016").MessageCaption;
                returnObj = new { success = false, message = errorMsg };
                return Json(returnObj);
            }



            var oPickDtl = ctx.SpTrnSPickingDtls.Find(CompanyCode, BranchCode, PickingSlipNo, "00", model.PartNo, model.PartNoOriginal, model.DocNo);
            var qrDb = "SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode='" + CompanyCode + "' AND BranchCode='" + BranchCode + "'";
            string DBMD = ctx.Database.SqlQuery<string>(qrDb).FirstOrDefault();

            if (!IsMD)
            {
                var sql = @"UPDATE "+ DBMD + @"..svSDMovement 
                SET QtyOrder = {6}, Qty = {6},
                    LastUpdateBy = '{0}',
                    LastUpdateDate = '{1}'
                WHERE CompanyCode = '{2}' AND
                      BranchCode = '{3}' AND
                      DocNo = '{4}' AND
                      PartNo = '{5}'
                           UPDATE "+ DBMD + @"..svSDMovement 
                SET QtyOrder = {6}, Qty = {6},
                    LastUpdateBy = '{0}',
                    LastUpdateDate = '{1}'
                WHERE CompanyCode = '{2}' AND
                      BranchCode = '{3}' AND
                      DocNo = '{7}' AND
                      PartNo = '{5}'"                     
                    ;
                sql = string.Format(sql, CurrentUser.UserId, DateTime.Now.ToShortDateString(), CompanyCode, BranchCode, model.DocNo, model.PartNo, model.QtyPicked, PickingSlipNo);
                ctx.Database.ExecuteSqlCommand(sql);
            }

            if (oPickDtl != null)
            {
                oPickDtl.SalesAmt = model.QtyPicked * oPickDtl.RetailPrice;
                oPickDtl.DiscAmt = Math.Round(decimal.Parse((oPickDtl.SalesAmt.Value * (oPickDtl.DiscPct.Value / 100)).ToString()), 0, MidpointRounding.AwayFromZero);
                oPickDtl.NetSalesAmt = oPickDtl.SalesAmt - oPickDtl.DiscAmt;
                oPickDtl.QtyPicked = model.QtyPicked;
                oPickDtl.LastUpdateDate = DateTime.Now;
                oPickDtl.LastUpdateBy = CurrentUser.UserId;
                ctx.SaveChanges();
            }


            else
            {
                returnObj = new { success = false, message = "Data yang anda ingin update tidak di temukan." };
            }

            try
            {
                RecalculateAmount(oPickDtl, PickingSlipNo);
                returnObj = new { success = true, message = "" };
            }
            catch (Exception ex)
            {
                returnObj = new { success = false, message = ex.Message.ToString() };
            }
            return Json(returnObj);
        }

        protected void RecalculateAmount(SpTrnSPickingDtl oPickDtl, string PickSlipNo)
        {
            try
            {
                var oPickHdr = ctx.SpTrnSPickingHdrs.Find(CompanyCode, BranchCode, PickSlipNo);
                oPickHdr.TotSalesQty = GetTotPickedQty(PickSlipNo);
                oPickHdr.TotSalesAmt = GetTotSalesAmt(PickSlipNo);
                oPickHdr.TotDiscAmt = GetTotDiscAmt(PickSlipNo);
                oPickHdr.TotDPPAmt = GetTotDPPAmt(PickSlipNo);

                if (oPickHdr.SalesType == "1")
                {
                    oPickHdr.TotPPNAmt = 0;
                }
                else
                {
                    var MstCustPc = ctx.ProfitCenters.Find(CompanyCode, BranchCode, oPickHdr.CustomerCode, ProfitCenter);
                    var oTax = (string.IsNullOrEmpty(MstCustPc.TaxCode)) ? null : ctx.Taxes.Find(
                            CompanyCode, MstCustPc.TaxCode);

                    oPickHdr.TotPPNAmt = (oTax != null) ? Math.Truncate(decimal.Parse((oPickHdr.TotDPPAmt * oTax.TaxPct / 100).ToString())) : 0;

                    oPickHdr.TotFinalSalesAmt = oPickHdr.TotDPPAmt + oPickHdr.TotPPNAmt;
                    oPickHdr.LastUpdateBy = CurrentUser.UserId;
                    oPickHdr.LastUpdateDate = DateTime.Now;
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error pada function RecalculateAmount, Message=" + ex.Message.ToString());
            }
        }

        protected void UpdateStatusspTRNSORDDtl(string DocNo, string PartNoOri)
        {
            try
            {
                var sql = @"UPDATE spTrnSORDDtl 
                SET Status = 4,
                    LastUpdateBy = '{0}',
                    LastUpdateDate = '{1}'
                WHERE spTrnSORDDtl.CompanyCode = '{2}' AND
                    spTrnSORDDtl.BranchCode = '{3}' AND
                    spTrnSORDDtl.DocNo = '{4}' AND
                    spTrnSORDDtl.PartNo = '{5}' AND
                    spTrnSORDDtl.WarehouseCode = '00' AND
                    spTrnSORDDtl.PartNoOriginal = '{6}'";
                sql = string.Format(sql, CurrentUser.UserId, DateTime.Now.ToShortDateString(), CompanyCode, BranchCode, DocNo, PartNoOri, PartNoOri);
                ctx.Database.ExecuteSqlCommand(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("Error pada function UpdateStatusspTRNSORDDtl, Message=" + ex.Message.ToString());
            }
        }

        protected decimal GetTotPickedQty(string PickingSlipNo)
        {
            var sql = "exec uspfn_spGetTotPickedQty {0}, {1}, {2}";
            return ctx.Database.SqlQuery<decimal>(sql, CompanyCode, BranchCode, PickingSlipNo).FirstOrDefault();
        }

        protected decimal GetTotSupplyQty(string PickingSlipNo)
        {
            var sql = "exec uspfn_spGetTotSupplyQty {0}, {1}, {2}";
            return ctx.Database.SqlQuery<decimal>(sql, CompanyCode, BranchCode, PickingSlipNo).FirstOrDefault();
        }

        protected decimal GetTotSalesAmt(string PickingSlipNo)
        {
            var sql = "exec uspfn_spGetTotSalesAmt {0}, {1}, {2}";
            return ctx.Database.SqlQuery<decimal>(sql, CompanyCode, BranchCode, PickingSlipNo).FirstOrDefault();
        }

        protected decimal GetTotDiscAmt(string PickingSlipNo)
        {
            var sql = "exec uspfn_spGetTotDiscAmt {0}, {1}, {2}";
            return ctx.Database.SqlQuery<decimal>(sql, CompanyCode, BranchCode, PickingSlipNo).FirstOrDefault();
        }

        protected decimal GetTotDPPAmt(string PickingSlipNo)
        {
            var sql = "exec uspfn_spGetTotDPPAmt {0}, {1}, {2}";
            return ctx.Database.SqlQuery<decimal>(sql, CompanyCode, BranchCode, PickingSlipNo).FirstOrDefault();
        }

        protected decimal GetTotSalesQty(string PickingSlipNo)
        {
            var sql = "exec uspfn_spGetTotSalesQty {0}, {1}, {2}";
            return ctx.Database.SqlQuery<decimal>(sql, CompanyCode, BranchCode, PickingSlipNo).FirstOrDefault();
        }

        protected decimal GetSumQtyQtyPicked(string PickingSlipNo)
        {
            var sql = "exec uspfn_spGetSumQtyQtyPicked {0}, {1}, {2}";
            return ctx.Database.SqlQuery<decimal>(sql, CompanyCode, BranchCode, PickingSlipNo).FirstOrDefault();
        }

        protected string IsValidStatus(string PickingSlipNo)
        {
            var errorMsg = "";
            var oPickHdr = ctx.SpTrnSPickingHdrs.Find(CompanyCode, BranchCode, PickingSlipNo);
            if (oPickHdr != null)
            {
                if (int.Parse(oPickHdr.Status) > 1)
                {
                    errorMsg = "Nomor dokumen ini sudah ter-posting !!";
                }
            }
            return errorMsg;
        }

        protected bool IsOverdueOrder(string CustomerCode)
        {
            bool returnVal = false;
            var sql = string.Format("exec uspfn_CheckIsOverdueOrder '{0}','{1}','{2}','{3}','{4}'", CompanyCode, BranchCode, CustomerCode, DateTime.Now.Date, ProfitCenter);
            var OverdueData = ctx.Database.SqlQuery<ArInterface>(sql).ToList();
            if (OverdueData.Count > 0)
            {
                returnVal = true;
            }
            else
            {
                returnVal = false;
            }
            return returnVal;
        }

        public JsonResult BtnGenerateBill(SpTrnSPickingHdr model, string[] DocNoList)
        {
            object returnObj = null;
            SpTrnSPickingHdr oPickHdr = null;
            bool IsSuccess = false;
            using (var trx = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var errorMsg = IsValidStatus(model.PickingSlipNo);
                    oPickHdr = ctx.SpTrnSPickingHdrs.Find(CompanyCode, BranchCode, model.PickingSlipNo);
                    if (!string.IsNullOrEmpty(errorMsg))
                    {
                        throw new Exception(errorMsg);
                    }

                    //check updateIsExternal
                    var sqlIsExt = string.Format("exec uspfn_spUpdateIsExternal '{0}','{1}','{2}','{3}'", CompanyCode, BranchCode, model.PickingSlipNo, CurrentUser.UserId);
                    ctx.Database.ExecuteSqlCommand(sqlIsExt);

                    if (oPickHdr != null)
                    {
                        decimal TotalFinalSlsAmt = GetTotFinalSalesAmtSO(oPickHdr, model.IsBORelease);

                        //BackOrder
                        //CallGenerateBill
                        IsSuccess = GenerateBill(oPickHdr, model.IsBORelease, TotalFinalSlsAmt, DocNoList);

                        //IsSuccess = true;
                    }

                    if (IsSuccess)
                    {
                        try
                        {
                            List<SpTrnSOSupply> ListspTrnSOSupply = new List<SpTrnSOSupply>();
                            foreach (var DocNo in DocNoList.Distinct())
                            {
                                var tmpListSOSupply = ctx.SpTrnSOSupplys.Where(m => m.CompanyCode == CompanyCode && m.BranchCode == BranchCode && m.DocNo == DocNo).ToList();
                                foreach (var SOSupply in tmpListSOSupply)
                                {
                                    ListspTrnSOSupply.Add(SOSupply);
                                }
                            }
                            InsertInvoice(oPickHdr, ListspTrnSOSupply);

                            // Commit transaction
                            trx.Commit();
                            returnObj = new { success = true, message = "Proses Generate Bill berhasil" };
                        }
                        catch (Exception ex)
                        {
                            // Rollback transaction
                            trx.Rollback();
                            returnObj = new { success = false, message = "Error pada function BtnGenerateBill, Message=" + ex.Message.ToString() };
                            return Json(returnObj);
                        }
                    }
                    return Json(returnObj);
                }
                catch (Exception ex)
                {
                    // Rollback transaction
                    trx.Rollback();
                    returnObj = new { success = false, message = "Error pada function BtnGenerateBill, Message=" + ex.Message.ToString() };
                    return Json(returnObj);
                }
            }
        }

        protected bool GenerateBillNew(SpTrnSPickingHdr PickHdr, bool IsBO, decimal TotalFinalSlsAmt, string[] ListDocNo)
        {
            bool success = true;

            var sql = @"
-- update status pickingHdr, set status = 2
UPDATE SpTrnSPickingHdr 
SET Status = 2,
    LastUpdateBy = '{0}',
    LastUpdateDate = '{1}'
WHERE CompanyCode = '{2}' AND
    BranchCode = '{3}' AND
    PickingSlipNo = '{4}'

-- Update spTrnSPickingDtl, QtyBill = QtyPicked. Calculate Sales base on Qty Bill.
UPDATE a
SET a.QtyBill = a.QtyPicked,
    a.SalesAmt = a.QtyBill * a.RetailPrice,
    a.DiscAmt = ROUND(a.SalesAmt * (a.DiscPct / 100), 0),
    a.NetSalesAmt = a.SalesAmt -  a.DiscAmt,
    a.TotSalesAmt = a.NetSalesAmt,
    a.LastUpdateBy = '{0}',
    a.LastUpdateDate = '{1}'
FROM spTrnSPickingDtl a
INNER JOIN spTrnSOSupply b ON a.CompanyCode = b.CompanyCode
and a.BranchCode = b.BranchCode
and a.DocNo = b.DocNo
and a.PickingSlipNo = b.PickingSlipNo
and a.WarehouseCode = '00'
and a.PartNo = b.PartNo
and a.PartNoOriginal = b.PartNoOriginal
WHERE a.CompanyCode = '{2}'
and a.BranchCode = '{3}'
and a.PickingSlipNo = '{4}'
            ";

            sql = string.Format(sql, CurrentUser.UserId, DateTime.Now.ToShortDateString(), CompanyCode, BranchCode, PickHdr.PickingSlipNo);
            ctx.Database.ExecuteSqlCommand(sql);

          
            return success;
        }

        protected bool GenerateBill(SpTrnSPickingHdr PickHdr, bool IsBO, decimal TotalFinalSlsAmt, string[] ListDocNo)
        {
            bool success = true;
            try
            {
                var oPickHdr = ctx.SpTrnSPickingHdrs.Find(CompanyCode, BranchCode, PickHdr.PickingSlipNo);
                //update status pickingHdr
                if (oPickHdr.Status == "2")
                {
                    success = false;
                    throw new Exception(string.Format(ctx.SysMsgs.Find("5045").MessageCaption, PickHdr.PickingSlipNo, "generate"));
                }
                oPickHdr.Status = "2";
                oPickHdr.LastUpdateBy = CurrentUser.UserId;
                oPickHdr.LastUpdateDate = DateTime.Now;
                ctx.SaveChanges();

                //Get spTrnSOSupply
                List<SpTrnSOSupply> ListspTrnSOSupply = new List<SpTrnSOSupply>();
                foreach (var DocNo in ListDocNo.Distinct())
                {
                    var tmpListSOSupply = ctx.SpTrnSOSupplys.Where(m => m.CompanyCode == CompanyCode && m.BranchCode == BranchCode && m.DocNo == DocNo).ToList();
                    foreach (var SOSupply in tmpListSOSupply)
                    {
                        ListspTrnSOSupply.Add(SOSupply);
                    }
                }

                // Update spTrnSPickingDtl, QtyBill = QtyPicked. Calculate Sales base on Qty Bill.
                success = GenerateBill(PickHdr, ListspTrnSOSupply);

                // Update spTrnSOSupply, QtyBil and QtyPicked
                if (success) success = GenerateBill(PickHdr, IsBO, ListspTrnSOSupply);

                //call RecalculateGeneratePL
                if (success) success = RecalculateGeneratePL(PickHdr, ListspTrnSOSupply, TotalFinalSlsAmt);

                // Update spTrnSORDHDR, Status = 5 (Bill Generated) 
                if (success) success = UpdateSpTrnSORDHdr(PickHdr, ListspTrnSOSupply);

                /* Update spTrnSORDDtl  & Inventory (SpMstItems+ SpMstItemLoc)                      */
                /* - increase QtyBO if Generate PL from New Order and QtySupply >= QtyPicked        */
                /* - increase QtyBOSupply if Generate PL from Back Order and QtySupply >= QtyPicked */
                /*   BO Manual (QtySupply - QtyBill)                                                */
                if (success) success = UpdateSpTrnSORDDtl(PickHdr, ListspTrnSOSupply);
            }
            catch (Exception ex)
            {
                success = false;
                throw new Exception(ex.Message.ToString());
            }

            return success;
        }

        protected void InsertInvoice(SpTrnSPickingHdr PickHdr, List<SpTrnSOSupply> ListspTrnSOSupply)
        {
            bool bIncludePPN = true;

            decimal decQtyQtyPicked = GetSumQtyQtyPicked(PickHdr.PickingSlipNo);

            if (decQtyQtyPicked > 0)
            {
                if (PickHdr.SalesType == "0")
                {
                    try
                    {
                        SpTrnSInvoiceHdr recordInvHdr = new SpTrnSInvoiceHdr();

                        recordInvHdr.CompanyCode = CompanyCode;
                        recordInvHdr.BranchCode = BranchCode;
                        recordInvHdr.CreatedBy = CurrentUser.UserId;
                        recordInvHdr.CreatedDate = DateTime.Now;
                        recordInvHdr.CustomerCode = PickHdr.CustomerCode;
                        recordInvHdr.CustomerCodeBill = PickHdr.CustomerCodeBill;
                        recordInvHdr.CustomerCodeShip = PickHdr.CustomerCodeShip;
                        recordInvHdr.LastUpdateBy = CurrentUser.UserId;
                        recordInvHdr.LastUpdateDate = DateTime.Now;
                        recordInvHdr.PickingSlipDate = PickHdr.PickingSlipDate;
                        recordInvHdr.PickingSlipNo = PickHdr.PickingSlipNo;
                        recordInvHdr.TotDiscAmt = PickHdr.TotDiscAmt;
                        recordInvHdr.TotDPPAmt = PickHdr.TotDPPAmt;
                        recordInvHdr.TotFinalSalesAmt = PickHdr.TotFinalSalesAmt;
                        recordInvHdr.TotPPNAmt = PickHdr.TotPPNAmt;
                        recordInvHdr.TotSalesAmt = PickHdr.TotSalesAmt;
                        recordInvHdr.TotSalesQty = PickHdr.TotSalesQty;
                        recordInvHdr.TransType = PickHdr.TransType;
                        recordInvHdr.TypeOfGoods = PickHdr.TypeOfGoods;
                        recordInvHdr.Status = "0";
                        recordInvHdr.SalesType = PickHdr.SalesType;
                        
                        var recSpare = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode);
                        var currDate = DateTime.Now;
                        var TransDate = recSpare.TransDate;
                        TransDate = TransDate.ChangeTime(currDate.Hour, currDate.Minute, currDate.Second, currDate.Millisecond, currDate.Ticks);

                        recordInvHdr.InvoiceDate = recSpare != null ? TransDate : DateTime.Now;
                        var sql = string.Format("exec uspfn_GnDocumentGetNew '{0}','{1}','{2}','{3}','{4}'", CompanyCode, BranchCode, "INV", CurrentUser.UserId, recordInvHdr.InvoiceDate.ToShortDateString());
                        recordInvHdr.InvoiceNo = ctx.Database.SqlQuery<string>(sql).FirstOrDefault();
                        recordInvHdr.FPJDate = Convert.ToDateTime("01/01/1900");
                        recordInvHdr.LockingDate = Convert.ToDateTime("01/01/1900");
                        recordInvHdr.FPJNo = string.Empty;
                        recordInvHdr.LockingBy = MyLogger.GetCRC32(CompanyCode + BranchCode + recordInvHdr.InvoiceNo);
                        ctx.SpTrnSInvoiceHdrs.Add(recordInvHdr);
                        ctx.SaveChanges();

                        if (recordInvHdr.TotPPNAmt == 0)
                            bIncludePPN = false;

                        //Insert SpTrnSInvoiceDtl
                        InsertSpTrnSInvoiceDtl(recordInvHdr, PickHdr, ListspTrnSOSupply, bIncludePPN);

                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error pada saat Insert SpTrnSInvoiceHdr, Message" + ex.Message.ToString());
                    }
                }
                else if (PickHdr.SalesType == "1" || PickHdr.SalesType == "2" || PickHdr.SalesType == "3")
                {
                    try
                    {
                        SpTrnSBPSFHdr recordBPSFHdr = new SpTrnSBPSFHdr();

                        recordBPSFHdr.LockingDate = Convert.ToDateTime("01/01/1900");
                        recordBPSFHdr.BranchCode = BranchCode;
                        recordBPSFHdr.CompanyCode = CompanyCode;
                        recordBPSFHdr.CreatedBy = CurrentUser.UserId;
                        recordBPSFHdr.CreatedDate = DateTime.Now;
                        recordBPSFHdr.CustomerCode = PickHdr.CustomerCode;
                        recordBPSFHdr.CustomerCodeBill = PickHdr.CustomerCodeBill;
                        recordBPSFHdr.CustomerCodeShip = PickHdr.CustomerCodeShip;
                        recordBPSFHdr.LastUpdateBy = CurrentUser.UserId;
                        recordBPSFHdr.LastUpdateDate = DateTime.Now;
                        recordBPSFHdr.PickingSlipDate = PickHdr.PickingSlipDate;
                        recordBPSFHdr.PickingSlipNo = PickHdr.PickingSlipNo;
                        recordBPSFHdr.TransType = PickHdr.TransType;
                        recordBPSFHdr.SalesType = PickHdr.SalesType;
                        recordBPSFHdr.Status = "0";
                        recordBPSFHdr.TotDiscAmt = PickHdr.TotDiscAmt;
                        recordBPSFHdr.TotDPPAmt = PickHdr.TotDPPAmt;
                        recordBPSFHdr.TotFinalSalesAmt = PickHdr.TotFinalSalesAmt;
                        recordBPSFHdr.TotPPNAmt = PickHdr.TotPPNAmt;
                        recordBPSFHdr.TotSalesAmt = PickHdr.TotSalesAmt;
                        recordBPSFHdr.TotSalesQty = PickHdr.TotSalesQty;
                        recordBPSFHdr.TypeOfGoods = PickHdr.TypeOfGoods;

                        GnMstCoProfileSpare recSpare = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode);
                        recordBPSFHdr.BPSFDate = recSpare != null ? recSpare.TransDate : DateTime.Now;

                        var sql = string.Format("exec uspfn_GnDocumentGetNew '{0}','{1}','{2}','{3}','{4}'", CompanyCode, BranchCode, "BPF", CurrentUser.UserId, recordBPSFHdr.BPSFDate.Value.ToShortDateString());
                        recordBPSFHdr.BPSFNo = ctx.Database.SqlQuery<string>(sql).FirstOrDefault();
                        ctx.SpTrnSBPSFHdrs.Add(recordBPSFHdr);

                        ctx.SaveChanges();

                        if (recordBPSFHdr.TotPPNAmt == 0)
                            bIncludePPN = false;

                        //Insert SpTrnSBPSFDtl
                        InsertSpTrnSBPSFDtl(recordBPSFHdr, PickHdr, ListspTrnSOSupply, bIncludePPN);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error pada saat Insert SpTrnSBPSFHdr, Message=" + ex.Message.ToString());
                    }
                }
            }
        }

        protected void InsertSpTrnSBPSFDtl(SpTrnSBPSFHdr recordHdr, SpTrnSPickingHdr PickHdr, List<SpTrnSOSupply> ListSpTrnSOSupply, bool isIncludePPN)
        {
            foreach (var SoSupply in ListSpTrnSOSupply)
            {
                var pickingDtl = ctx.SpTrnSPickingDtls.Find(CompanyCode, BranchCode, PickHdr.PickingSlipNo, "00", SoSupply.PartNo,
                                    SoSupply.PartNoOriginal, SoSupply.DocNo);

                if (pickingDtl.QtyPicked > 0)
                {
                    SpTrnSBPSFDtl recordDtl = new SpTrnSBPSFDtl();
                    recordDtl.LastUpdateDate = Convert.ToDateTime("01/01/1900");
                    recordDtl.ExPickingListDate = Convert.ToDateTime("01/01/1900");

                    recordDtl.CompanyCode = CompanyCode;
                    recordDtl.BranchCode = BranchCode;
                    recordDtl.BPSFNo = recordHdr.BPSFNo;
                    recordDtl.PartNo = pickingDtl.PartNo;
                    recordDtl.DocNo = pickingDtl.DocNo;
                    recordDtl.DocDate = pickingDtl.DocDate;
                    recordDtl.PartNoOriginal = pickingDtl.PartNoOriginal;
                    recordDtl.CreatedBy = CurrentUser.UserId;
                    recordDtl.CreatedDate = DateTime.Now;

                    SpTrnSORDHdr recordSORDHdr = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, recordHdr.BPSFNo);
                    if (recordSORDHdr != null)
                    {
                        recordDtl.ExPickingListNo = recordSORDHdr.ExPickingSlipNo;
                        recordDtl.ExPickingListDate = recordSORDHdr.ExPickingSlipDate;
                    }
                    var recordSOSupply = ctx.SpTrnSOSupplys.Find(CompanyCode, BranchCode, SoSupply.DocNo,
                                            SoSupply.SupSeq, SoSupply.PartNo,
                                            SoSupply.PartNoOriginal, SoSupply.PTSeq);

                    if (recordSOSupply != null)
                    {
                        recordDtl.ABCClass = SoSupply.ABCClass;
                        recordDtl.LocationCode = SoSupply.LocationCode;
                        recordDtl.MovingCode = SoSupply.MovingCode;
                        recordDtl.PartCategory = SoSupply.PartCategory;
                        recordDtl.PartNoOriginal = SoSupply.PartNoOriginal;
                        recordDtl.ProductType = SoSupply.ProductType;
                        recordDtl.ReferenceDate = SoSupply.ReferenceDate;
                        recordDtl.ReferenceNo = SoSupply.ReferenceNo;
                        recordDtl.WarehouseCode = SoSupply.WarehouseCode;
                    }

                    if (PickHdr.IsBORelease)
                    {
                        recordDtl.ExPickingListNo = PickHdr.PickingSlipNo;
                        recordDtl.ExPickingListDate = PickHdr.PickingSlipDate;
                    }

                    recordDtl.CostPrice = pickingDtl.CostPrice;
                    recordDtl.RetailPrice = pickingDtl.RetailPrice;
                    recordDtl.RetailPriceInclTax = pickingDtl.RetailPriceInclTax;
                    recordDtl.SalesAmt = pickingDtl.SalesAmt;
                    recordDtl.TotSalesAmt = pickingDtl.TotSalesAmt;
                    recordDtl.DiscAmt = pickingDtl.DiscAmt;
                    recordDtl.NetSalesAmt = pickingDtl.NetSalesAmt;
                    recordDtl.DiscPct = pickingDtl.DiscPct;
                    recordDtl.QtyBill = pickingDtl.QtyBill;
                    recordDtl.PPNAmt = 0;

                    ctx.SpTrnSBPSFDtls.Add(recordDtl);
                    Helpers.ReplaceNullable(recordDtl);
                }
            }
            
            ctx.SaveChanges();
        }

        protected void InsertSpTrnSInvoiceDtl(SpTrnSInvoiceHdr InvoiceHdr, SpTrnSPickingHdr PickHdr, List<SpTrnSOSupply> ListSpTrnSOSupply, bool isIncludePPN)
        {
                try
                {
                    decimal PpnPct = 0;
                    int iSeq = 1;

                    foreach (var SoSupply in ListSpTrnSOSupply)
                    {
                        var pickingDtl = ctx.SpTrnSPickingDtls.Find(CompanyCode, BranchCode, PickHdr.PickingSlipNo, "00", SoSupply.PartNo,
                                            SoSupply.PartNoOriginal, SoSupply.DocNo);

                        if (pickingDtl.QtyPicked != pickingDtl.QtyBill)
                        {
                            throw new Exception("Error karena QtyPicked != QtyBill");
                        }

                        if (pickingDtl.QtyPicked > 0)
                        {
                            SpTrnSInvoiceDtl recordDtl = new SpTrnSInvoiceDtl();
                            recordDtl.ExPickingListDate = Convert.ToDateTime("01/01/1900");
                            recordDtl.LastUpdateDate = Convert.ToDateTime("01/01/1900");
                            recordDtl.CompanyCode = CompanyCode;
                            recordDtl.BranchCode = BranchCode;
                            recordDtl.InvoiceNo = InvoiceHdr.InvoiceNo;
                            recordDtl.PartNo = pickingDtl.PartNo;
                            recordDtl.PartNoOriginal = pickingDtl.PartNoOriginal;
                            recordDtl.DocNo = pickingDtl.DocNo;
                            recordDtl.DocDate = pickingDtl.DocDate.Value;
                            recordDtl.CreatedBy = CurrentUser.UserId;
                            recordDtl.CreatedDate = DateTime.Now;

                            var recordSOSupply = ctx.SpTrnSOSupplys.Find(CompanyCode, BranchCode, SoSupply.DocNo,
                                                SoSupply.SupSeq, SoSupply.PartNo,
                                                SoSupply.PartNoOriginal, SoSupply.PTSeq);

                            if (recordSOSupply != null)
                            {
                                recordDtl.ABCClass = recordSOSupply.ABCClass;
                                recordDtl.LocationCode = recordSOSupply.LocationCode;
                                recordDtl.MovingCode = recordSOSupply.MovingCode;
                                recordDtl.PartCategory = recordSOSupply.PartCategory;
                                recordDtl.PartNoOriginal = recordSOSupply.PartNoOriginal;
                                recordDtl.ProductType = recordSOSupply.ProductType;
                                recordDtl.ReferenceDate = recordSOSupply.ReferenceDate.Value;
                                recordDtl.ReferenceNo = recordSOSupply.ReferenceNo;
                                recordDtl.WarehouseCode = recordSOSupply.WarehouseCode;
                            }

                            // Pembetulan COGS
                            recordDtl.CostPrice = GetCostPrice(pickingDtl.PartNo); //pickingDtl.CostPrice.Value;
                            recordDtl.RetailPrice = pickingDtl.RetailPrice.Value;
                            recordDtl.RetailPriceInclTax = pickingDtl.RetailPriceInclTax.Value;
                            recordDtl.SalesAmt = pickingDtl.SalesAmt.Value;
                            recordDtl.TotSalesAmt = pickingDtl.TotSalesAmt.Value;
                            recordDtl.DiscAmt = pickingDtl.DiscAmt.Value;
                            recordDtl.NetSalesAmt = pickingDtl.NetSalesAmt.Value;
                            recordDtl.DiscPct = pickingDtl.DiscPct.Value;
                            recordDtl.QtyBill = pickingDtl.QtyBill.Value;
                            recordDtl.PPNAmt = 0;

                            ctx.SpTrnSInvoiceDtls.Add(recordDtl);

                            //Insert svSDMovement
                            if (IsMD == false)
                            {
                                var sqlItemPrice = string.Format("SELECT * FROM {0}..spMstItemPrice WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                                    GetDbMD(), CompanyMD, BranchMD, pickingDtl.PartNo);
                                spMstItemPrice oItemPrice = ctx.Database.SqlQuery<spMstItemPrice>(sqlItemPrice).FirstOrDefault();

                                ///Insert SvSDMovement here!!!
                                var qrDb = "SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode='" + CompanyCode + "' AND BranchCode='" + BranchCode + "'";
                                string DBMD = ctx.Database.SqlQuery<string>(qrDb).FirstOrDefault();

                                var iQry = @"insert into " + DBMD + @"..svSDMovement (CompanyCode, BranchCode, DocNo, DocDate, PartNo, PartSeq, WarehouseCode, QtyOrder, Qty, DiscPct, CostPrice, RetailPrice,   
	                        TypeOfGoods, CompanyMD, BranchMD, WarehouseMD, RetailPriceInclTaxMD, RetailPriceMD, CostPriceMD, QtyFlag, ProductType, ProfitCenterCode, 
	                        Status, ProcessStatus, ProcessDate, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate) 
                            VALUES('" + CompanyCode + "','" + BranchCode + "','" + InvoiceHdr.InvoiceNo + "','" + ctx.CurrentTime + "','" + pickingDtl.PartNo +
                                "','" + iSeq + "','" + recordSOSupply.WarehouseCode + "','" + recordSOSupply.QtyBill + "','" + recordSOSupply.QtyBill + "','" + recordSOSupply.DiscPct + "','" + Math.Floor(recordSOSupply.CostPrice.Value) +
                                "','" + recordSOSupply.RetailPrice + "','" + CurrentUser.TypeOfGoods + "','" + CompanyMD + "','" + BranchMD + "','" + recordSOSupply.WarehouseCode + "','" + oItemPrice.RetailPriceInclTax + "','" + oItemPrice.RetailPrice +
                                "','" + oItemPrice.CostPrice + "','x','" + pickingDtl.ProductType + "','300','0','0','" + ctx.CurrentTime + "','" + CurrentUser.UserId + "','" + ctx.CurrentTime + "','" + CurrentUser.UserId + "','" + ctx.CurrentTime + "')";

                                var sqlCount = string.Format("select count(*) as count from " + DBMD + @"..svHstSDMovement
                                    where CompanyCode = '{0}' 
                                    and BranchCode = '{1}'
                                    and DocNo = '{2}'
                                    and PartNo = '{3}'", CompanyCode, BranchCode, InvoiceHdr.PickingSlipNo, pickingDtl.PartNo);
                                var count = (int)ctx.Database.SqlQuery<int>(sqlCount).FirstOrDefault();

                                if (count > 0)
                                {
                                    var qty = Convert.ToDecimal(recordSOSupply.QtySupply.Value);
                                    iQry = iQry + " " + string.Format("update " + DBMD + @"..svSDMovement 
                                    set QtyOrder += {0}, Qty += {1}
                                    where CompanyCode = '{2}' 
                                    and BranchCode = '{3}'
                                    and DocNo = '{4}'
                                    and PartNo = '{5}'",
                                        qty, qty, CompanyCode, BranchCode, InvoiceHdr.PickingSlipNo, pickingDtl.PartNo);
                                }
                                else
                                {
                                    iSeq = iSeq + 1;
                                }
                                ctx.Database.ExecuteSqlCommand(iQry);
                            }

                            ctx.SaveChanges();
                        }
                    }

                    if (IsMD == false)
                    {
                        var sql = string.Format("exec uspfn_UpdateSvSDMovement '{0}','{1}','{2}'", CompanyCode, BranchCode, InvoiceHdr.InvoiceNo);
                        ctx.Database.ExecuteSqlCommand(sql);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error pada function InsertSpTrnSInvoiceDtl, Message=" + ex.Message.ToString());
                }
        }

        protected bool UpdateSpTrnSORDDtl(SpTrnSPickingHdr PickHdr, List<SpTrnSOSupply> ListSpTrnSOSupply)
        {
            bool success = true;
            try
            {
                decimal decManualBO;
                //bool result = true;
                foreach (var SoSupply in ListSpTrnSOSupply)
                {
                    var oPickDtl = ctx.SpTrnSPickingDtls.Find(CompanyCode, BranchCode, PickHdr.PickingSlipNo, "00", SoSupply.PartNo,
                                    SoSupply.PartNoOriginal, SoSupply.DocNo);
                    var oSORDHdr = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, SoSupply.DocNo);
                    var oSORDDtl = ctx.SpTrnSORDDtls.Find(CompanyCode, BranchCode, SoSupply.DocNo, SoSupply.PartNoOriginal, "00", SoSupply.PartNoOriginal);
                    if (oSORDDtl.Status == "5")
                    {
                    }
                    else
                    {
                        decManualBO = 0;
                        decManualBO = oPickDtl.QtySupply.Value - oPickDtl.QtyPicked.Value;

                        if (decManualBO > 0)
                        {
                            if (PickHdr.IsBORelease)
                            {
                                oSORDDtl.QtyBill += SoSupply.QtyBill;
                                oSORDDtl.QtyBOSupply -= decManualBO;
                            }
                            else
                            {
                                if (oSORDHdr.isBO.Value)
                                {
                                    oSORDDtl.QtyBill = SoSupply.QtyBill;
                                    oSORDDtl.QtyBO += decManualBO;
                                }
                                else
                                {
                                    oSORDDtl.QtyBill = SoSupply.QtyBill;
                                    oSORDDtl.QtyBO += decManualBO;
                                    oSORDDtl.QtyBOCancel += decManualBO;
                                }
                            }
                            oSORDDtl.Status = "5";
                            oSORDDtl.LastUpdateBy = CurrentUser.UserId;
                            oSORDDtl.LastUpdateDate = DateTime.Now;
                            ctx.SaveChanges();
                            UpdateStock(SoSupply.PartNo, SoSupply.WarehouseCode, 0, (decManualBO * -1), 0, 0, oSORDHdr.SalesType);

                            decManualBO = (oSORDHdr.SalesType == "0" || PickHdr.SalesType == "1") ? decManualBO : 0;
                            if (oSORDHdr.isBO.Value)
                            {
                                UpdateStock(SoSupply.PartNo, SoSupply.WarehouseCode, 0, 0, decManualBO, 0, oSORDHdr.SalesType);
                            }
                        }
                        else if (decManualBO == 0)
                        {
                            oSORDDtl.Status = "5";
                            oSORDDtl.QtyBill += SoSupply.QtyBill;
                            oSORDDtl.LastUpdateBy = CurrentUser.UserId;
                            oSORDDtl.LastUpdateDate = DateTime.Now;
                            ctx.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                success = false;
                throw new Exception("Error pada function UpdateSpTrnSORDDtl, Message=" + ex.Message.ToString());
            }

            return success;
        }

        protected bool UpdateSpTrnSORDHdr(SpTrnSPickingHdr PickHdr, List<SpTrnSOSupply> ListSpTrnSOSupply)
        {
            bool success = true;
            try
            {
                foreach (var SoSupply in ListSpTrnSOSupply)
                {
                    var oSOHdr = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, SoSupply.DocNo);
                    oSOHdr.Status = "5";
                    oSOHdr.LastUpdateBy = CurrentUser.UserId;
                    oSOHdr.LastUpdateDate = DateTime.Now;
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                success = false;
                throw new Exception("Error pada function UpdateSPTrnsSORDhdr, Message=" + ex.Message.ToString());
            }

            return success;
        }

        protected bool RecalculateGeneratePL(SpTrnSPickingHdr PickHdr, List<SpTrnSOSupply> ListspTrnSOSupply, decimal TotalFinalSlsAmt)
        {
            bool success = true;
            try
            {
                var oPickHdr = ctx.SpTrnSPickingHdrs.Find(CompanyCode, BranchCode, PickHdr.PickingSlipNo);
                //decimal var = record.TotFinalSalesAmt;
                oPickHdr.TotSalesQty = GetTotSalesQty(oPickHdr.PickingSlipNo);
                oPickHdr.TotSalesAmt = GetTotSalesAmt(oPickHdr.PickingSlipNo);
                oPickHdr.TotDiscAmt = GetTotDiscAmt(oPickHdr.PickingSlipNo);
                oPickHdr.TotDPPAmt = GetTotDPPAmt(oPickHdr.PickingSlipNo);

                if (oPickHdr.SalesType == "1")
                {
                    oPickHdr.TotPPNAmt = 0;
                }
                else
                {
                    var MstCustPc = ctx.ProfitCenters.Find(CompanyCode, BranchCode, oPickHdr.CustomerCode, ProfitCenter);
                    var oTax = (string.IsNullOrEmpty(MstCustPc.TaxCode)) ? null : ctx.Taxes.Find(
                            CompanyCode, MstCustPc.TaxCode);

                    oPickHdr.TotPPNAmt = (oTax != null) ? Math.Truncate(decimal.Parse((oPickHdr.TotDPPAmt * oTax.TaxPct / 100).ToString())) : 0;
                }


                oPickHdr.TotFinalSalesAmt = oPickHdr.TotDPPAmt + oPickHdr.TotPPNAmt;
                oPickHdr.LastUpdateBy = CurrentUser.UserId;
                oPickHdr.LastUpdateDate = DateTime.Now;
                ctx.SaveChanges();

                if (oPickHdr.SalesType == "0")
                {
                    if (!((TotalFinalSlsAmt - oPickHdr.TotFinalSalesAmt) == 0))
                    {
                        var recBank = ctx.BankBooks.Find(CompanyCode, BranchCode, oPickHdr.CustomerCode, ProfitCenter);
                        if (recBank != null)
                        {
                            recBank.SalesAmt = recBank.SalesAmt - (TotalFinalSlsAmt - oPickHdr.TotFinalSalesAmt);
                        }
                    }
                }
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                success = false;
                throw new Exception("Error pada function RecalculateGeneratePL, Message=" + ex.Message.ToString());
            }

            return success;
        }

        protected bool GenerateBill(SpTrnSPickingHdr PickingHdr, bool IsBackOrder, List<SpTrnSOSupply> ListSpSOSupply)
        {
            bool success = true;
            try
            {
                SpTrnSOSupply oSOSupply = null;
                SpTrnSPickingDtl oSPickingDtl = null;
                //get part details
                foreach (var SoSupply in ListSpSOSupply)
                {
                    oSOSupply = ctx.SpTrnSOSupplys.Find(CompanyCode, BranchCode, SoSupply.DocNo, SoSupply.SupSeq, SoSupply.PartNo, SoSupply.PartNoOriginal, SoSupply.PTSeq);
                    oSPickingDtl = ctx.SpTrnSPickingDtls.Find(CompanyCode, BranchCode, PickingHdr.PickingSlipNo, "00", SoSupply.PartNo, SoSupply.PartNoOriginal, SoSupply.DocNo);

                    if (oSPickingDtl != null)
                    {
                        oSOSupply.QtyBill = oSPickingDtl.QtyBill;
                        oSOSupply.Status = "2";
                        oSOSupply.QtyPicked = oSPickingDtl.QtyPicked;
                        oSOSupply.LastUpdateBy = CurrentUser.UserId;
                        oSOSupply.LastUpdateDate = DateTime.Now;
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                success = false;
                throw new Exception("Error pada function GenerateBill, Message=" + ex.Message.ToString());
            }

            return success;
        }

        protected bool GenerateBill(SpTrnSPickingHdr PickingHdr, List<SpTrnSOSupply> listSpTrnSOSupply)
        {
            bool success = true;
            try
            {
                foreach (var PickingDtl in listSpTrnSOSupply)
                {
                    var oPickDtl = ctx.SpTrnSPickingDtls.Find(CompanyCode, BranchCode, PickingHdr.PickingSlipNo, "00", PickingDtl.PartNo, PickingDtl.PartNoOriginal, PickingDtl.DocNo);
                    oPickDtl.QtyBill = oPickDtl.QtyPicked;
                    oPickDtl.SalesAmt = oPickDtl.QtyBill * oPickDtl.RetailPrice;
                    oPickDtl.DiscAmt = Math.Round((decimal)(oPickDtl.SalesAmt * (PickingDtl.DiscPct / 100)), 0, MidpointRounding.AwayFromZero);
                    oPickDtl.NetSalesAmt = oPickDtl.SalesAmt - oPickDtl.DiscAmt;
                    oPickDtl.TotSalesAmt = oPickDtl.NetSalesAmt;
                    oPickDtl.LastUpdateBy = CurrentUser.UserId;
                    oPickDtl.LastUpdateDate = DateTime.Now;
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                success = false;
                throw new Exception("Error pada function GenerateBill, Message=" + ex.Message.ToString());
            }

            return success;
        }

        protected decimal GetTotFinalSalesAmtSO(SpTrnSPickingHdr PickHdr, bool IsBO)
        {
            decimal total = 0;
            decimal salesAmt = 0;
            decimal disc = 0;
            decimal netSalesAmt = 0;
            try
            {
                //get SONo by PickingList
                List<string> listSONo = ctx.Database.SqlQuery<string>(@"Select distinct DocNo from sptrnspickingdtl 
                                        where companycode= {0} and branchcode={1} and pickingslipno = {2} ", CompanyCode, BranchCode, PickHdr.PickingSlipNo).ToList();

                foreach (var SONo in listSONo)
                {
                    var ListSORDtl = ctx.SpTrnSORDDtls.Where(m => m.CompanyCode == CompanyCode && m.BranchCode == BranchCode && m.DocNo == SONo).ToList();
                    foreach (var SORDtl in ListSORDtl)
                    {
                        if (IsBO)
                        {
                            salesAmt = (decimal)SORDtl.QtyBOSupply * (decimal)SORDtl.RetailPrice;
                        }
                        else
                        {
                            salesAmt = (decimal)SORDtl.QtySupply * (decimal)SORDtl.RetailPrice;
                        }
                        disc = Math.Round((decimal)(salesAmt * (Convert.ToDecimal(SORDtl.DiscPct) / 100)), 0, MidpointRounding.AwayFromZero);
                        netSalesAmt = salesAmt - disc;

                        string taxCode = "";
                        decimal taxPct = 0;


                        var oPickHdr = ctx.SpTrnSPickingHdrs.Find(CompanyCode, BranchCode, PickHdr.PickingSlipNo);
                        bool isLocked = (oPickHdr.IsLocked == null ? false : oPickHdr.IsLocked.Value);
                        if (isLocked)
                        {
                            taxCode = ctx.ProfitCenters.Find(CompanyCode, BranchCode, PickHdr.CustomerCode, ProfitCenter).TaxCode;
                            taxPct = (string.IsNullOrEmpty(taxCode)) ? 0 : ctx.Taxes.Find(CompanyCode, taxCode).TaxPct.Value;
                        }
                        if (taxPct == 0)
                        {
                            total += netSalesAmt;
                        }
                        else { total += netSalesAmt + Math.Truncate(netSalesAmt * taxPct / 100); }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error pada function GetTotFinalSalesAmtSO, Message=" + ex.Message.ToString());
            }
            return total;
        }

        // Dikarenakan insert invoce gagal mulu diciptakanlah fungsi bokong ini
        // mba yo ini fungsinya
        public JsonResult HelpInsertInvoice()
        {
            var PLS = "PLS/15/006542";
            var SOC = "SOC/15/000608";

            var oPickHdr = ctx.SpTrnSPickingHdrs.Find(CompanyCode, BranchCode, PLS);
            using (TransactionScope transInv = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout }))
            {
                try
                {
                    //Ini Biar aman coba cek di pickingnya jumlahnya ada berapa?
                    //Klo sudah sesuai bisa insert langsung
                    
                    List<SpTrnSOSupply> ListspTrnSOSupply = new List<SpTrnSOSupply>();
                    var tmpListSOSupply = ctx.SpTrnSOSupplys.Where(m => m.CompanyCode == CompanyCode && m.BranchCode == BranchCode && m.DocNo == SOC).ToList();
                    foreach (var SOSupply in tmpListSOSupply)
                    {
                        ListspTrnSOSupply.Add(SOSupply);
                    }

                    //check updateIsExternal
                    //var sqlIsExt = string.Format("exec uspfn_spUpdateIsExternal '{0}','{1}','{2}','{3}'", CompanyCode, BranchCode, PLS, CurrentUser.UserId);
                    //ctx.Database.ExecuteSqlCommand(sqlIsExt);

                    //string[] docno = new string[] { SOC };
                    //if (oPickHdr != null)
                    //{
                    //    decimal TotalFinalSlsAmt = GetTotFinalSalesAmtSO(oPickHdr, false);

                    //    //BackOrder
                    //    //CallGenerateBill
                    //    bool isTrue = true;
                    //    isTrue = GenerateBill(oPickHdr, false, TotalFinalSlsAmt, docno);

                    //    //IsSuccess = true;
                    //}
                    UpdateSpTrnSORDDtl(oPickHdr, ListspTrnSOSupply);

                    InsertInvoice(oPickHdr, ListspTrnSOSupply);
                    // sehabis beres insert bisa cek yang terinput sesuai yg di sosupplynya apa kaga
                    // cek juga sdmovementnya dapetnya sama ga jumlahnya

                    transInv.Complete();
                    return Json(true);
                }
                catch (Exception ex)
                {
                    transInv.Dispose();
                    return Json(false);
                }
            }
        }
    }
}
