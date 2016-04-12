using SimDms.Common.Models;
using SimDms.Sales.Models.Result;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using SimDms.Sales.Models;
using System.Text;

namespace SimDms.Sales.Controllers.Api
{
    public class PostingJurnalController : BaseController
    {
        public GetPeriod GetPeriod()
        {
            var query = string.Format(@"select gb.* from (
            select 'AP' as Code, CompanyCode, BranchCode, PeriodBeg, PeriodEnd from gnMstCoProfileFinance UNION ALL
            select 'AR' as Code, CompanyCode, BranchCode, PeriodBegAR, PeriodEndAR from gnMstCoProfileFinance UNION ALL
            select 'GL' as Code, CompanyCode, BranchCode, PeriodBegGL, PeriodEndGL from gnMstCoProfileFinance UNION ALL
            select 'SALES' as Code, CompanyCode, BranchCode, PeriodBeg, PeriodEnd from gnMstCoProfileSales)gb
            where gb.CompanyCode = '{0}'
            and gb.BranchCode = '{1}'
            and gb.Code = '{2}'", CompanyCode, BranchCode, "SALES");

            var data = ctx.Database.SqlQuery<GetPeriod>(query).FirstOrDefault();

            return data;
        }

        public JsonResult Periode()
        {
            return Json(new
            {
                DateFrom = GetPeriod().PeriodBeg,
                DateTo = GetPeriod().PeriodEnd
            });
        }

        public JsonResult TransaksiDataID(string Trans)
        {
            List<object> listOfDataID = new List<object>();
            if (Trans == "0")
            {
                listOfDataID.Add(new { value = "ALL", text = "ALL" });
                listOfDataID.Add(new { value = "HPP", text = "HPP" });
                listOfDataID.Add(new { value = "PR", text = "PURCHASE RETURN" });
                listOfDataID.Add(new { value = "KT", text = "KAROSERI TERIMA" });
            }
            else if (Trans == "1")
            {
                listOfDataID.Add(new { value = "ALL", text = "ALL" });
                listOfDataID.Add(new { value = "INV", text = "INVOICE" });
                listOfDataID.Add(new { value = "DN", text = "DEBET NOTE" });
                listOfDataID.Add(new { value = "SR", text = "SALES RETURN" });
            }
            else
            {
                listOfDataID.Add(new { value = "ALL", text = "ALL" });
                listOfDataID.Add(new { value = "VTO", text = "Transfer Out" });
                listOfDataID.Add(new { value = "VTI", text = "Transfer In" });
                listOfDataID.Add(new { value = "VTOMB", text = "Transfer Out Multi Branch" });
                listOfDataID.Add(new { value = "VTIMB", text = "Transfer In Multi Branch" });
            }
            return Json(listOfDataID);
        }

        public JsonResult DefaultPeriod()
        {
            var query = string.Format(@"
               SELECT FiscalYear, FiscalMonth, FiscalPeriod, PeriodBeg, PeriodEnd 
               FROM GnMstCoProfileSales
               WHERE CompanyCode = '{0}' AND BranchCode = '{1}'
                ", CompanyCode, BranchCode);

            var queryable = ctx.Database.SqlQuery<CoProfileSalesView>(query).AsQueryable();
            return Json(new
            {
                data = queryable,
                fiscalPeriode = queryable.FirstOrDefault().FiscalPeriod.Value.ToString().PadLeft(2, '0')
            });
        }

        public JsonResult GetJurnal(string TypeJournal, string DocNo)
        {
            string qSql = "";
            string qSqlDtl = "";
            StringBuilder sb = new StringBuilder();

            if (TypeJournal.ToLower().StartsWith("transfer") || TypeJournal.ToLower() == "karoseri" || TypeJournal.ToLower() == "purchase" ||
                TypeJournal.ToLower() == "invoice")
            {
                qSql = string.Format(@"uspfn_OmGetJournal '{0}','{1}','{2}','{3}'
                ", CompanyCode, BranchCode, TypeJournal, DocNo);
                qSqlDtl = string.Format(@"uspfn_OmGetJournalDebetCredit '{0}','{1}','{2}','{3}'
                ", CompanyCode, BranchCode, TypeJournal, DocNo);

            }

            #region PURCHASE RETURN

            else if (TypeJournal.ToLower() == "purchasereturn")
            {
                qSql = "SELECT * INTO #f1 FROM (";
                qSql += " SELECT CompanyCode, BranchCode, ReturnNo, SalesModelCode, SalesModelYear, ISNULL(Quantity, 0) * (ISNULL(AfterDiscDPP, 0) + ISNULL(OthersDPP, 0) + ISNULL(AfterDiscPPnBM, 0)) AS DPP";
                qSql += ",  ISNULL(Quantity, 0) * (ISNULL(AfterDiscPPn, 0) + ISNULL(OthersPPn, 0)) AS PPn";
                qSql += " FROM omTrPurchaseReturnDetailModel";
                qSql += " WHERE CompanyCode = '"+ CompanyCode +"' AND BranchCode = '"+ BranchCode +"' AND ReturnNo = '"+ DocNo +"'";
                qSql += " ) #f1";

                qSql += " SELECT * INTO #f2 FROM (";
                qSql += " SELECT CompanyCode, BranchCode, ReturnNo, SalesModelCode, SUM(DPP) AS DPP, SUM(PPn) AS PPn FROM #f1";
                qSql += " GROUP BY CompanyCode, BranchCode, ReturnNo, SalesModelCode";
                qSql += " ) #f2";

                qSql += " SELECT * INTO #f3 FROM (";
                qSql += " SELECT a.CompanyCode, a.BranchCode, a.ReturnNo AS DocNo, c.ReturnDate AS DocDate, a.SalesModelCode, a.DPP, a.PPn, b.InventoryAccNo AS AccInvent";
                qSql += ", e.TaxInAccNo AS AccPPn, b.PReturnAccNo AS AccAP";
                qSql += " FROM #f2 a";
                qSql += " LEFT JOIN OmMstModelAccount b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.SalesModelCode = a.SalesModelCode";
                qSql += " LEFT JOIN OmTrPurchaseReturn c ON c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode AND c.ReturnNo = a.ReturnNo";
                qSql += " LEFT JOIN OmTrPurchaseHPP hpp ON hpp.CompanyCode = a.CompanyCode AND hpp.BranchCode = a.BranchCode AND hpp.HPPNo = c.HPPNo";
                qSql += " LEFT JOIN GnMstSupplierProfitCenter d ON d.CompanyCode = a.CompanyCode AND d.BranchCode = a.BranchCode AND d.SupplierCode = hpp.SupplierCode AND d.ProfitCenterCode = '100'";
                qSql += " LEFT JOIN GnMstSupplierClass e ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode AND e.SupplierClass = d.SupplierClass AND e.ProfitCenterCode = d.ProfitCenterCode";
                //qSql += " WHERE b.isActive = 1";
                qSql += " ) #f3";

                qSql += " SELECT * INTO #f4 FROM (";
                qSql += " SELECT 1 AS Seq, CompanyCode, BranchCode, DocNo, DocDate, DocDate AS AccDate, '100' AS ProfitCenterCode";
                qSql += ", AccAP AS AccountNo, 'UNIT' AS JournalCode, 'PURCHASERETURN' AS TypeJournal, DocNo AS ApplyTo, (ISNULL(DPP, 0) + ISNULL(PPn, 0)) AS AmountDb, 0 AS AmountCr";
                qSql += ", 'PSEMENTARA' AS TypeTrans";
                qSql += " FROM #f3";
                qSql += " WHERE (ISNULL(DPP, 0) + ISNULL(PPn, 0)) > 0";
                qSql += " UNION";
                qSql += " SELECT 2 AS Seq, CompanyCode, BranchCode, DocNo, DocDate, DocDate AS AccDate, '100' AS ProfitCenterCode";
                qSql += ", AccInvent AS AccountNo, 'UNIT' AS JournalCode, 'PURCHASERETURN' AS TypeJournal, DocNo AS ApplyTo, 0 AS AmountDb, ISNULL(DPP, 0) AS AmountCr";
                qSql += ", 'INVENTORY' AS TypeTrans";
                qSql += " FROM #f3";
                qSql += " WHERE ISNULL(DPP, 0) > 0";
                qSql += " UNION";
                qSql += " SELECT 3 AS Seq, CompanyCode, BranchCode, DocNo, DocDate, DocDate AS AccDate, '100' AS ProfitCenterCode";
                qSql += ", AccPPn AS AccountNo, 'UNIT' AS JournalCode, 'PURCHASERETURN' AS TypeJournal, DocNo AS ApplyTo, 0 AS AmountDb,  ISNULL(PPn, 0) AS AmountCr";
                qSql += ", 'PPN' AS TypeTrans";
                qSql += " FROM #f3";
                qSql += " WHERE ISNULL(PPn, 0) > 0";
                qSql += " ) #f4";
                qSqlDtl = "SELECT SUM(a.AmountCr) as AmountCr, sum(a.AmountDb) as AmountDb FROM #f4 a";
                qSqlDtl = qSql + " " + qSqlDtl;
                qSql += " SELECT a.*, b.Description AS AccDescription FROM #f4 a";
                qSql += " LEFT JOIN GnMstAccount b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.AccountNo = a.AccountNo";
                qSql += " ORDER BY a.DocNo, a.Seq ASC";
                qSql += " DROP TABLE #F1";
                qSql += " DROP TABLE #F2";
                qSql += " DROP TABLE #F3";
                qSql += " DROP TABLE #F4";
            }

            #endregion

            #region DEBET NOTE

            else if (TypeJournal.ToLower() == "debetnote")
            {

                qSql = "SELECT * INTO #f1 FROM (";
                qSql += " SELECT a.CompanyCode, a.BranchCode, a.DNNo, c.SalesModelCode, c.SalesModelYear";
                qSql += ", ISNULL(SUM(b.BBN) , 0) AS BBN, ISNULL(SUM(b.KIR) , 0) AS KIR";
                qSql += ", 0 AS ShipAmt, 0 AS DepositAmt, 0 AS OthersAmt";
                qSql += " FROM omTrSalesDN a";
                qSql += " LEFT JOIN omTrSalesDNVin b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.DNNo = a.DNNo";
                qSql += " LEFT JOIN omTrSalesInvoiceVIN c ON c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode AND c.InvoiceNo = a.InvoiceNo AND c.ChassisCode = b.ChassisCode AND c.ChassisNo = b.ChassisNo";
                qSql += " WHERE a.CompanyCode = '" + CompanyCode + "' AND a.BranchCode = '" + BranchCode + "' AND a.DNNo = '" + DocNo + "'";
                qSql += " GROUP BY a.CompanyCode, a.BranchCode, a.DNNo, c.SalesModelCode, c.SalesModelYear";
                qSql += " UNION";
                qSql += " SELECT a.CompanyCode, a.BranchCode, a.DNNo, a.SalesModelCode, a.SalesModelYear";
                qSql += ", 0 AS BBN, 0 AS KIR";
                qSql += ", ISNULL(SUM(a.Quantity), 0) * ISNULL(SUM(a.ShipAmt) , 0) AS ShipAmt";
                qSql += ", ISNULL(SUM(a.Quantity), 0) * ISNULL(SUM(a.DepositAmt) , 0) AS DepositAmt";
                qSql += ", ISNULL(SUM(a.Quantity), 0) * ISNULL(SUM(a.OthersAmt) , 0) AS OthersAmt";
                qSql += " FROM omTrSalesDNModel a";
                qSql += " WHERE a.CompanyCode = '" + CompanyCode + "' AND a.BranchCode = '" + BranchCode + "' AND a.DNNo = '" + DocNo + "'";
                qSql += " GROUP BY a.CompanyCode, a.BranchCode, a.DNNo, a.SalesModelCode, a.SalesModelYear";
                qSql += " ) #f1";

                qSql += " SELECT * INTO #f2 FROM (";
                qSql += " SELECT CompanyCode, BranchCode, DNNo, SalesModelCode, SUM(BBN) AS BBN, SUM(KIR) AS KIR";
                qSql += ", SUM(ShipAmt) AS ShipAmt, SUM(DepositAmt) AS DepositAmt, SUM(OthersAmt) AS OthersAmt";
                qSql += "  FROM #f1";
                qSql += " GROUP BY CompanyCode, BranchCode, DNNo, SalesModelCode";
                qSql += " ) #f2";

                qSql += " SELECT * INTO #f3 FROM (";
                qSql += " SELECT a.CompanyCode, a.BranchCode, a.DNNo AS DocNo, c.DNDate AS DocDate, a.SalesModelCode, a.BBN, a.KIR";
                qSql += ", b.BBNAccNo AS AccBBN, b.KIRAccNo AS AccKIR, e.ReceivableAccNo AS AccAR";
                qSql += ", a.ShipAmt, a.DepositAmt, a.OthersAmt, b.ShipAccNo AS AccShip, b.DepositAccNo AS AccDeposit, b.OthersAccNo AS AccOthers";
                qSql += " FROM #f2 a";
                qSql += " LEFT JOIN OmMstModelAccount b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.SalesModelCode = a.SalesModelCode";
                qSql += " LEFT JOIN omTrSalesDN c ON c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode AND c.DNNo = a.DNNo";
                qSql += " LEFT JOIN GnMstCustomerProfitCenter d ON d.CompanyCode = a.CompanyCode AND d.BranchCode = a.BranchCode AND d.CustomerCode = c.CustomerCode AND d.ProfitCenterCode = '100'";
                qSql += " LEFT JOIN GnMstCustomerClass e ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode AND e.CustomerClass = d.CustomerClass AND e.ProfitCenterCode = d.ProfitCenterCode";
                qSql += " ) #f3";

                qSql += " SELECT * INTO #f4 FROM (";
                qSql += " SELECT 1 AS Seq, a.CompanyCode, a.BranchCode, a.DocNo, a.DocDate, a.DocDate AS AccDate, '100' AS ProfitCenterCode";
                qSql += ", a.AccAR AS AccountNo, 'UNIT' AS JournalCode, 'DEBETNOTE' AS TypeJournal, a.DocNo AS ApplyTo";
                qSql += ", (ISNULL(a.BBN, 0) + ISNULL(a.KIR, 0) + ISNULL(a.ShipAmt, 0) + ISNULL(a.DepositAmt, 0) + ISNULL(a.OthersAmt, 0)) AS AmountDb";
                qSql += ", 0 AS AmountCr";
                qSql += ", 'AR' AS TypeTrans";
                qSql += " FROM #f3 a";
                qSql += " WHERE  (ISNULL(a.BBN, 0) + ISNULL(a.KIR, 0) + ISNULL(a.ShipAmt, 0) + ISNULL(a.DepositAmt, 0) + ISNULL(a.OthersAmt, 0)) > 0";
                qSql += " UNION";
                qSql += " SELECT 2 AS Seq, CompanyCode, BranchCode, DocNo, DocDate, DocDate AS AccDate, '100' AS ProfitCenterCode";
                qSql += ", AccBBN AS AccountNo, 'UNIT' AS JournalCode, 'DEBETNOTE' AS TypeJournal, DocNo AS ApplyTo, 0 AS AmountDb, ISNULL(BBN, 0)  AS AmountCr";
                qSql += ", 'BBN' AS TypeTrans";
                qSql += " FROM #f3";
                qSql += " WHERE  ISNULL(BBN, 0) > 0";
                qSql += " UNION";
                qSql += " SELECT 3 AS Seq, CompanyCode, BranchCode, DocNo, DocDate, DocDate AS AccDate, '100' AS ProfitCenterCode";
                qSql += ", AccKIR AS AccountNo, 'UNIT' AS JournalCode, 'DEBETNOTE' AS TypeJournal, DocNo AS ApplyTo, 0 AS AmountDb, ISNULL(KIR, 0) AS AmountCr";
                qSql += ", 'KIR' AS TypeTrans";
                qSql += " FROM #f3";
                qSql += " WHERE  ISNULL(KIR, 0)  > 0";
                qSql += " UNION";
                qSql += " SELECT 4 AS Seq, CompanyCode, BranchCode, DocNo, DocDate, DocDate AS AccDate, '100' AS ProfitCenterCode";
                qSql += ", AccShip AS AccountNo, 'UNIT' AS JournalCode, 'DEBETNOTE' AS TypeJournal, DocNo AS ApplyTo, 0 AS AmountDb, ISNULL(ShipAmt, 0) AS AmountCr";
                qSql += ", 'ONGKOS KIRIM' AS TypeTrans";
                qSql += " FROM #f3";
                qSql += " WHERE  ISNULL(ShipAmt, 0)  > 0";
                qSql += " UNION";
                qSql += " SELECT 5 AS Seq, CompanyCode, BranchCode, DocNo, DocDate, DocDate AS AccDate, '100' AS ProfitCenterCode";
                qSql += ", AccDeposit AS AccountNo, 'UNIT' AS JournalCode, 'DEBETNOTE' AS TypeJournal, DocNo AS ApplyTo, 0 AS AmountDb, ISNULL(DepositAmt, 0) AS AmountCr";
                qSql += ", 'UNIT DEPOSIT' AS TypeTrans";
                qSql += " FROM #f3";
                qSql += " WHERE  ISNULL(DepositAmt, 0)  > 0";
                qSql += " UNION";
                qSql += " SELECT 6 AS Seq, CompanyCode, BranchCode, DocNo, DocDate, DocDate AS AccDate, '100' AS ProfitCenterCode";
                qSql += ", AccOthers AS AccountNo, 'UNIT' AS JournalCode, 'DEBETNOTE' AS TypeJournal, DocNo AS ApplyTo, 0 AS AmountDb, ISNULL(OthersAmt, 0) AS AmountCr";
                qSql += ", 'LAIN - LAIN' AS TypeTrans";
                qSql += " FROM #f3";
                qSql += " WHERE  ISNULL(OthersAmt, 0)  > 0";
                qSql += " ) #f4";
                qSqlDtl = "SELECT SUM(a.AmountCr) as AmountCr, sum(a.AmountDb) as AmountDb FROM #f4 a";
                qSqlDtl = qSql + " " + qSqlDtl;
                qSql += " SELECT a.*, b.Description AS AccDescription FROM #f4 a";
                qSql += " LEFT JOIN GnMstAccount b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.AccountNo = a.AccountNo";
                qSql += " ORDER BY a.DocNo, a.Seq ASC";
                qSql += " DROP TABLE #F1";
                qSql += " DROP TABLE #F2";
                qSql += " DROP TABLE #F3";
                qSql += " DROP TABLE #F4";
            }

            #endregion

            #region SALES RETURN

            else if (TypeJournal.ToLower() == "return")
            {

                qSql = "SELECT * INTO #f1 FROM (";
                qSql += " SELECT a.CompanyCode, a.BranchCode, a.ReturnNo, a.SalesModelCode, a.SalesModelYear, ISNULL(a.Quantity, 0) * (ISNULL(a.AfterDiscDPP, 0)) AS DPP, 0 AS COGs";
                qSql += ", ISNULL(a.Quantity, 0) * (ISNULL(a.AfterDiscPPn, 0)) AS PPn, ISNULL(a.Quantity, 0) * ISNULL(a.DiscExcludePPn, 0) AS Discount, ISNULL(a.Quantity, 0) * ISNULL(a.AfterDiscPPnBm, 0) AS PPnBM";
                qSql += " FROM omTrSalesReturnDetailModel a";
                qSql += " LEFT JOIN omTrSalesReturn b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.ReturnNo = a.ReturnNo";
                qSql += " WHERE a.CompanyCode = '" + CompanyCode + "' AND a.BranchCode = '" + BranchCode + "' AND a.ReturnNo = '" + DocNo + "'";
                qSql += " GROUP BY a.CompanyCode, a.BranchCode, a.ReturnNo, a.SalesModelCode, a.SalesModelYear, ISNULL(a.Quantity, 0) * (ISNULL(a.AfterDiscDPP, 0))";
                qSql += " ,  ISNULL(a.Quantity, 0) * (ISNULL(a.AfterDiscPPn, 0)), ISNULL(a.Quantity, 0) * ISNULL(a.DiscExcludePPn, 0), ISNULL(a.Quantity, 0) * ISNULL(a.AfterDiscPPnBm, 0)";
                qSql += " UNION";
                qSql += " SELECT  a.CompanyCode, a.BranchCode, a.ReturnNo, a.SalesModelCode, a.SalesModelYear, 0 AS DPP, ISNULL(SUM(b.COGs), 0) AS COGs ";
                qSql += ", 0 AS PPn, 0 AS Discount, 0 AS PPnBM";
                qSql += " FROM OmTrSalesReturnVin a";
                qSql += " INNER JOIN OmTrSalesInvoiceVin b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode";
                qSql += " AND a.SalesModelCode = b.SalesModelCode AND a.SalesModelYear = b.SalesModelYear";
                qSql += " AND a.ChassisCode = b.ChassisCode AND a.ChassisNo = b.ChassisNo";
                qSql += " INNER JOIN OmTrSalesReturn c ON a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode";
                qSql += " AND a.ReturnNo = c.ReturnNo AND c.InvoiceNo = b.InvoiceNo ";
                qSql += " WHERE a.CompanyCode = '" + CompanyCode + "' AND a.BranchCode = '" + BranchCode + "' AND a.ReturnNo = '" + DocNo + "'";
                qSql += " GROUP BY a.CompanyCode, a.BranchCode, a.ReturnNo, a.SalesModelCode, a.SalesModelYear";
                qSql += " ) #f1";

                qSql += " SELECT * INTO #f2 FROM (";
                qSql += " SELECT CompanyCode, BranchCode, ReturnNo, SalesModelCode, SUM(DPP) AS DPP, SUM(COGs) AS COGs, SUM(PPn) AS PPn, SUM(Discount) AS Discount, SUM(PPnBM) AS PPnBM";
                qSql += "  FROM #f1";
                qSql += " GROUP BY CompanyCode, BranchCode, ReturnNo, SalesModelCode";
                qSql += " ) #f2";

                qSql += " SELECT * INTO #f3 FROM (";
                qSql += " SELECT a.CompanyCode, a.BranchCode, a.ReturnNo AS DocNo, c.ReturnDate AS DocDate, a.SalesModelCode, a.DPP, a.COGs, a.PPn, a.Discount, a.PPnBM";
                qSql += ", b.ReturnAccNo AS AccSalesReturn, b.DiscountAccNo AS AccDiscount, e.TaxOutAccNo AS AccPPn, b.HReturnAccNo AS AccAR";
                qSql += ", e.LuxuryTaxAccNo AS AccPPnBM, b.COGsAccNo AS AccCOGsUnit, b.InventoryAccNo AS AccInventUnit";
                qSql += " FROM #f2 a";
                qSql += " LEFT JOIN OmMstModelAccount b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.SalesModelCode = a.SalesModelCode";
                qSql += " LEFT JOIN omTrSalesReturn c ON c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode AND c.ReturnNo = a.ReturnNo";
                qSql += " LEFT JOIN GnMstCustomerProfitCenter d ON d.CompanyCode = a.CompanyCode AND d.BranchCode = a.BranchCode AND d.CustomerCode = c.CustomerCode AND d.ProfitCenterCode = '100'";
                qSql += " LEFT JOIN GnMstCustomerClass e ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode AND e.CustomerClass = d.CustomerClass AND e.ProfitCenterCode = d.ProfitCenterCode";
                //qSql += " WHERE b.isActive = 1";
                qSql += " ) #f3";

                qSql += " SELECT * INTO #ff1 FROM (";
                qSql += " SELECT a.CompanyCode, a.BranchCode, a.ReturnNo, a.OtherCode, a.SalesModelCode, a.SalesModelYear, ISNULL(b.Quantity, 0) * ISNULL(a.AfterDiscDPP, 0)  AS DPP";
                qSql += " ,  ISNULL(b.Quantity, 0) * ISNULL(a.AfterDiscPPn, 0)  AS PPn, ISNULL(b.Quantity, 0) * ISNULL(a.DiscExcludePPn, 0)  AS Discount";
                qSql += " FROM omTrSalesReturnOther a";
                qSql += " LEFT JOIN omTrSalesReturnDetailModel b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.ReturnNo = a.ReturnNo";
                qSql += " AND b.BPKNo = a.BPKNo AND b.SalesModelCode = a.SalesModelCode AND b.SalesModelYear = a.SalesModelYear";
                qSql += " WHERE a.CompanyCode = '" + CompanyCode + "' AND a.BranchCode = '" + BranchCode + "' AND a.ReturnNo = '" + DocNo + "'";
                qSql += " ) #ff1";

                qSql += " SELECT * INTO #ff2 FROM (";
                qSql += " SELECT CompanyCode, BranchCode, ReturnNo, SalesModelCode, SUM(DPP) AS DPP, SUM(PPn) AS PPn, SUM(Discount) AS Discount";
                qSql += "  FROM #ff1";
                qSql += " GROUP BY CompanyCode, BranchCode, ReturnNo, SalesModelCode";//, OtherCode
                qSql += " ) #ff2";

                qSql += " SELECT * INTO #ff3 FROM (";
                qSql += " SELECT a.CompanyCode, a.BranchCode, a.ReturnNo AS DocNo, c.ReturnDate AS DocDate, a.DPP";//, OtherCode
                qSql += ", b.ReturnAccNoAks AS AccReturnAks, (ISNULL(e.TaxPct, 0) / 100) * a.DPP AS PPn, b.COGSAccNoAks AS AccCOGSAks, b.InventoryAccNoAks AS AccInventAks";
                qSql += ", a.Discount, b.DiscountAccNoAks AS AccDiscountAks";
                qSql += " FROM #ff2 a";
                qSql += " LEFT JOIN OmMstModelAccount b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.SalesModelCode = a.SalesModelCode";
                qSql += " LEFT JOIN omTrSalesReturn c ON c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode AND c.ReturnNo = a.ReturnNo";
                qSql += " LEFT JOIN GnMstCustomerProfitCenter d ON d.CompanyCode = a.CompanyCode AND d.BranchCode = a.BranchCode AND d.CustomerCode = c.CustomerCode AND d.ProfitCenterCode = '100'";
                qSql += " LEFT JOIN GnMstTax e ON e.CompanyCode = a.CompanyCode AND e.TaxCode = d.TaxCode";
                qSql += " ) #ff3";

                qSql += " SELECT * INTO #ff3a FROM(";
                qSql += " SELECT CompanyCode, BranchCode, DocNo, DocDate, SUM(DPP) AS DPP, SUM(PPn) AS PPn, SUM(Discount) AS Discount";
                qSql += " FROM #ff3";
                qSql += " GROUP BY CompanyCode, BranchCode, DocNo, DocDate";
                qSql += " ) #ff3a";

                qSql += " SELECT * INTO #f4 FROM (";
                qSql += " SELECT 1 AS Seq, a.CompanyCode, a.BranchCode, a.DocNo, a.DocDate, a.DocDate AS AccDate, '100' AS ProfitCenterCode";
                qSql += ", a.AccSalesReturn AS AccountNo, 'UNIT' AS JournalCode, 'RETURN' AS TypeJournal, a.DocNo AS ApplyTo, (ISNULL(a.DPP, 0) + ISNULL(a.Discount, 0)) AS AmountDb, 0 AS AmountCr";
                qSql += ", 'RETUR SALES UNIT' AS TypeTrans";
                qSql += " FROM #f3 a";
                qSql += " WHERE   (ISNULL(a.DPP, 0) + ISNULL(a.Discount, 0)) > 0";
                qSql += " UNION";
                qSql += " SELECT 2 AS Seq, CompanyCode, BranchCode, DocNo, DocDate, DocDate AS AccDate, '100' AS ProfitCenterCode";
                qSql += ", AccReturnAks AS AccountNo, 'UNIT' AS JournalCode, 'RETURN' AS TypeJournal, DocNo AS ApplyTo, (ISNULL(DPP, 0)  + ISNULL(Discount, 0)) AS AmountDb, 0 AS AmountCr";
                qSql += ", 'RETURN SALES AKSESORIS' AS TypeTrans";
                qSql += " FROM #ff3";
                qSql += " WHERE  (ISNULL(DPP, 0) + ISNULL(Discount, 0)) > 0";
                qSql += " UNION";
                qSql += " SELECT 3 AS Seq, a.CompanyCode, a.BranchCode, a.DocNo, a.DocDate, a.DocDate AS AccDate, '100' AS ProfitCenterCode";
                qSql += ", a.AccPPn AS AccountNo, 'UNIT' AS JournalCode, 'RETURN' AS TypeJournal, a.DocNo AS ApplyTo, (ISNULL(a.PPn, 0) + ISNULL(b.PPn, 0)) AS AmountDb, 0 AS AmountCr";//+ c.PPn
                qSql += ", 'PPN' AS TypeTrans";
                qSql += " FROM #f3 a";
                qSql += " LEFT JOIN #ff3a b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.DocNo = a.DocNo";
                qSql += " WHERE  (ISNULL(a.PPn, 0) + ISNULL(b.PPn, 0)) > 0";

                qSql += " UNION";
                qSql += " SELECT 4 AS Seq, CompanyCode, BranchCode, DocNo, DocDate, DocDate AS AccDate, '100' AS ProfitCenterCode";
                qSql += ", AccPPnBM AS AccountNo, 'UNIT' AS JournalCode, 'RETURN' AS TypeJournal, DocNo AS ApplyTo, ISNULL(PPnBM, 0) AS AmountDb, 0 AS AmountCr";
                qSql += ", 'PPnBM' AS TypeTrans";
                qSql += " FROM #f3";
                qSql += " WHERE  ISNULL(PPnBM, 0) > 0";

                qSql += " UNION";
                qSql += " SELECT 5 AS Seq, a.CompanyCode, a.BranchCode, a.DocNo, a.DocDate, a.DocDate AS AccDate, '100' AS ProfitCenterCode";
                qSql += ", a.AccAR AS AccountNo, 'UNIT' AS JournalCode, 'RETURN' AS TypeJournal, a.DocNo AS ApplyTo, 0 AS AmountDb, (ISNULL(a.DPP, 0) + ISNULL(a.PPn, 0) + ISNULL(a.PPnBM, 0) + ISNULL(b.DPP, 0) + ISNULL(b.PPn, 0)) AS AmountCr";// + c.DPP + c.PPn
                qSql += ", 'HRETURN' AS TypeTrans";
                qSql += " FROM #f3 a";
                qSql += " LEFT JOIN #ff3a b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.DocNo = a.DocNo";
                qSql += " WHERE  (ISNULL(a.DPP, 0) + ISNULL(a.PPn, 0) + ISNULL(a.PPnBM, 0) + ISNULL(b.DPP, 0) + ISNULL(b.PPn, 0)) > 0";
                qSql += " UNION";
                qSql += " SELECT 6 AS Seq, CompanyCode, BranchCode, DocNo, DocDate, DocDate AS AccDate, '100' AS ProfitCenterCode";
                qSql += ", AccDiscount AS AccountNo, 'UNIT' AS JournalCode, 'RETURN' AS TypeJournal, DocNo AS ApplyTo, 0 AS AmountDb, ISNULL(Discount, 0) AS AmountCr";
                qSql += ", 'DISCOUNT UNIT' AS TypeTrans";
                qSql += " FROM #f3";
                qSql += " WHERE  ISNULL(Discount, 0) > 0";

                qSql += " UNION";
                qSql += " SELECT 7 AS Seq, CompanyCode, BranchCode, DocNo, DocDate, DocDate AS AccDate, '100' AS ProfitCenterCode";
                qSql += ", AccDiscountAks AS AccountNo, 'UNIT' AS JournalCode, 'RETURN' AS TypeJournal, DocNo AS ApplyTo, 0 AS AmountDb, ISNULL(Discount, 0) AS AmountCr";
                qSql += ", 'DISCOUNT AKSESORIS' AS TypeTrans";
                qSql += " FROM #ff3";
                qSql += " WHERE  ISNULL(Discount, 0) > 0";

                qSql += " UNION";
                qSql += " SELECT 8 AS Seq, CompanyCode, BranchCode, DocNo, DocDate, DocDate AS AccDate, '100' AS ProfitCenterCode";
                qSql += ", AccInventUnit AS AccountNo, 'UNIT' AS JournalCode, 'RETURN' AS TypeJournal, DocNo AS ApplyTo, ISNULL(COGs, 0) AS AmountDb, 0 AS AmountCr";
                qSql += ", 'INVENTORY UNIT' AS TypeTrans";
                qSql += " FROM #f3";
                qSql += " WHERE  ISNULL(COGs, 0) > 0";
                qSql += " UNION";
                qSql += " SELECT 9 AS Seq, CompanyCode, BranchCode, DocNo, DocDate, DocDate AS AccDate, '100' AS ProfitCenterCode";
                qSql += ", AccCOGsUnit AS AccountNo, 'UNIT' AS JournalCode, 'RETURN' AS TypeJournal, DocNo AS ApplyTo, 0 AS AmountDb, ISNULL(COGs, 0) AS AmountCr";
                qSql += ", 'HPP UNIT' AS TypeTrans";
                qSql += " FROM #f3";
                qSql += " WHERE  ISNULL(COGs, 0) > 0";
                qSql += " ) #f4";
                qSqlDtl = "SELECT SUM(a.AmountCr) as AmountCr, sum(a.AmountDb) as AmountDb FROM #f4 a";
                qSqlDtl = qSql + " " + qSqlDtl;
                qSql += " SELECT a.*, b.Description AS AccDescription FROM #f4 a";
                qSql += " LEFT JOIN GnMstAccount b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.AccountNo = a.AccountNo";
                qSql += " ORDER BY a.DocNo, a.Seq ASC";
                qSql += " DROP TABLE #F1";
                qSql += " DROP TABLE #F2";
                qSql += " DROP TABLE #F3";
                qSql += " DROP TABLE #F4";
            }

            #endregion

            var queryable = ctx.Database.SqlQuery<GetJurnal>(qSql).AsQueryable();
            var queryableDtl = ctx.Database.SqlQuery<GetJurnal>(qSqlDtl).AsQueryable();
            return Json(new { success = true, data = queryable, dataDtl = queryableDtl });

            
        }

        public JsonResult SelectByPeriode(string fiscalYear, int periodeNum)
        {
            var query = string.Format(@"SELECT 
CONVERT(varchar, FiscalYear) + RIGHT('00' + CONVERT(varchar, PeriodeNum), 2) AS Periode
, CONVERT(VARCHAR(4), FiscalYear) AS FiscalYear, PeriodeNum, FiscalMonth, PeriodeName, FromDate, EndDate
, CASE ISNULL(StatusSparepart, 0 ) WHEN 0 THEN 'Future Entry' WHEN 1 THEN 'Open' WHEN 2 THEN 'Close' END AS StatusSP
, CASE ISNULL(StatusSales, 0 ) WHEN 0 THEN 'Future Entry' WHEN 1 THEN 'Open' WHEN 2 THEN 'Close' END AS StatusSL
, CASE ISNULL(StatusService, 0 ) WHEN 0 THEN 'Future Entry' WHEN 1 THEN 'Open' WHEN 2 THEN 'Close' END AS StatusSV
, CASE ISNULL(StatusFinanceAP, 0 ) WHEN 0 THEN 'Future Entry' WHEN 1 THEN 'Open' WHEN 2 THEN 'Close' END AS StatusAP
, CASE ISNULL(StatusFinanceAR, 0 ) WHEN 0 THEN 'Future Entry' WHEN 1 THEN 'Open' WHEN 2 THEN 'Close' END AS StatusAR
, CASE ISNULL(StatusFinanceGL, 0 ) WHEN 0 THEN 'Future Entry' WHEN 1 THEN 'Open' WHEN 2 THEN 'Close' END AS StatusGL
, CASE ISNULL(FiscalStatus, 0 ) WHEN 0 THEN 'Not Active' WHEN 1 THEN 'Active' END AS StatusFiscal 
FROM gnMstPeriode WHERE CompanyCode ='{0}' AND BranchCode = '{1}' AND FiscalYear = '{2}' AND PeriodeNum = {3}
ORDER BY FiscalYear DESC, FiscalMonth DESC, PeriodeNum DESC", CompanyCode, BranchCode, fiscalYear, periodeNum);
            var data = ctx.Database.SqlQuery<PeriodePostingJurnal>(query).AsQueryable();

            return Json(new { success = true, result = data});
        }

        #region SelectPurchase

        private JsonResult SelectPurchase4ALL(DateTime? DateFrom, DateTime? DateTo, string Status)
        {
            string qSql = "";
            qSql = "SELECT";
            if (Status == "5")
            {
                qSql += " CONVERT(bit, 1) AS isSelected, ";
            }
            qSql += " a.CompanyCode, a.BranchCode, 'PURCHASE' AS TypeJournal, a.HPPNo AS DocNo, a.HPPDate AS DocDate, a.PONo AS RefNo";
            qSql += " , a.SupplierCode AS RefCode, b.SupplierName AS RefName, CASE ISNULL(a.Status, 0) WHEN '5' THEN 'Posted' ELSE 'UnPosted' END AS Status";
            qSql += " FROM OmTrPurchaseHPP a";
            qSql += " LEFT JOIN GnMstSupplier b ON b.CompanyCode = a.CompanyCode AND b.SupplierCode = a.SupplierCode";
            qSql += " WHERE a.CompanyCode ='" + CompanyCode + "' AND a.BranchCode ='" + BranchCode + "' AND a.Status ='" + Status + "'";
            qSql += " AND a.HPPDate BETWEEN'" + DateFrom + "' AND '" + DateTo + "'";
            qSql += " UNION ";
            qSql += " SELECT ";
            if (Status == "5")
            {
                qSql += " CONVERT(bit, 1) AS isSelected, ";
            }
            qSql += " b.CompanyCode, b.BranchCode, 'PURCHASERETURN' AS TypeJournal, b.ReturnNo AS DocNo, b.ReturnDate AS DocDate, b.HPPNo AS RefNo";
            qSql += " , '' AS RefCode, '' AS RefName, CASE ISNULL(b.Status, 0) WHEN '5' THEN 'Posted' ELSE 'UnPosted' END  AS Status";
            qSql += " FROM OmTrPurchaseReturn b";
            qSql += " WHERE b.CompanyCode ='" + CompanyCode + "' AND b.BranchCode ='" + BranchCode + "' AND b.Status ='" + Status + "'";
            qSql += " AND b.ReturnDate BETWEEN'" + DateFrom + "' AND '" + DateTo + "'";
            qSql += " UNION ";
            qSql += " SELECT ";
            if (Status == "5")
            {
                qSql += " CONVERT(bit, 1) AS isSelected, ";
            }
            qSql += " c.CompanyCode, c.BranchCode, 'KAROSERI' AS TypeJournal, c.KaroseriTerimaNo AS DocNo, c.KaroseriTerimaDate AS DocDate, c.KaroseriSPKNo AS RefNo";
            qSql += " , c.SupplierCode AS RefCode, d.SupplierName AS RefName, CASE ISNULL(c.Status, 0) WHEN '5' THEN 'Posted' ELSE 'UnPosted' END  AS Status";
            qSql += " FROM OmTrPurchaseKaroseriTerima c";
            qSql += " LEFT JOIN GnMstSupplier d ON c.CompanyCode = d.CompanyCode AND c.SupplierCode = d.SupplierCode";
            qSql += " WHERE c.CompanyCode ='" + CompanyCode + "' AND c.BranchCode ='" + BranchCode + "' AND c.Status ='" + Status + "'";
            qSql += " AND c.KaroseriTerimaDate BETWEEN'" + DateFrom + "' AND '" + DateTo + "'";

            var queryable = ctx.Database.SqlQuery<JurnalPurchase>(qSql).AsQueryable();
            return Json(queryable);
        }

        private JsonResult SelectPurchase4HPP(DateTime? DateFrom, DateTime? DateTo, string Status)
        {
            string qSql = "";
            qSql = "SELECT";
            if (Status == "5")
            {
                qSql += " CONVERT(bit, 1) AS isSelected, ";
            }
            qSql += " a.CompanyCode, a.BranchCode, 'PURCHASE' AS TypeJournal, a.HPPNo AS DocNo, a.HPPDate AS DocDate, a.PONo AS RefNo";
            qSql += " , a.SupplierCode AS RefCode, b.SupplierName AS RefName, CASE ISNULL(a.Status, 0) WHEN '5' THEN 'Posted' ELSE 'UnPosted' END AS Status";
            qSql += " FROM OmTrPurchaseHPP a";
            qSql += " LEFT JOIN GnMstSupplier b ON b.CompanyCode = a.CompanyCode AND b.SupplierCode = a.SupplierCode";
            qSql += " WHERE a.CompanyCode ='" + CompanyCode + "' AND a.BranchCode ='" + BranchCode + "' AND a.Status ='" + Status + "'";
            qSql += " AND a.HPPDate BETWEEN'" + DateFrom + "' AND '" + DateTo +"'";

            var queryable = ctx.Database.SqlQuery<JurnalPurchase>(qSql).AsQueryable();
            return Json(queryable);
        }

        private JsonResult SelectPurchase4PR(DateTime? DateFrom, DateTime? DateTo, string Status)
        {
            string qSql = "";
            qSql += " SELECT ";
            if (Status == "5")
            {
                qSql += " CONVERT(bit, 1) AS isSelected, ";
            }
            qSql += " a.CompanyCode, a.BranchCode, 'PURCHASERETURN' AS TypeJournal, a.ReturnNo AS DocNo, a.ReturnDate AS DocDate, a.HPPNo AS RefNo";
            qSql += " , '' AS RefCode, '' AS RefName, CASE ISNULL(a.Status, 0) WHEN '5' THEN 'Posted' ELSE 'UnPosted' END  AS Status";
            qSql += " FROM OmTrPurchaseReturn a";
            qSql += " WHERE a.CompanyCode ='" + CompanyCode + "' AND a.BranchCode ='" + BranchCode + "' AND a.Status ='" + Status + "'";
            qSql += " AND a.HPPDate BETWEEN'" + DateFrom + "' AND '" + DateTo + "'";

            var queryable = ctx.Database.SqlQuery<JurnalPurchase>(qSql).AsQueryable();
            return Json(queryable);
        }

        private JsonResult SelectPurchase4KT(DateTime? DateFrom, DateTime? DateTo, string Status)
        {
            string qSql = "";
            qSql += " SELECT ";
            if (Status == "5")
            {
                qSql += " CONVERT(bit, 1) AS isSelected, ";
            }
            qSql += " a.CompanyCode, a.BranchCode, 'KAROSERI' AS TypeJournal, a.KaroseriTerimaNo AS DocNo, a.KaroseriTerimaDate AS DocDate, a.KaroseriSPKNo AS RefNo";
            qSql += " , a.SupplierCode AS RefCode, b.SupplierName AS RefName, CASE ISNULL(a.Status, 0) WHEN '5' THEN 'Posted' ELSE 'UnPosted' END  AS Status";
            qSql += " FROM OmTrPurchaseKaroseriTerima a";
            qSql += " LEFT JOIN GnMstSupplier b ON b.CompanyCode = a.CompanyCode AND b.SupplierCode = a.SupplierCode";
            qSql += " WHERE a.CompanyCode ='" + CompanyCode + "' AND a.BranchCode ='" + BranchCode + "' AND a.Status ='" + Status + "'";
            qSql += " AND a.HPPDate BETWEEN'" + DateFrom + "' AND '" + DateTo + "'";

            var queryable = ctx.Database.SqlQuery<JurnalPurchase>(qSql).AsQueryable();
            return Json(queryable);
        }

        #endregion

        #region SelectSales

        private JsonResult SelectSales4ALL(DateTime? DateFrom, DateTime? DateTo, string Status)
        {
            string qSql = "";
            qSql = "SELECT ";
            if (Status == "5")
            {
                qSql += " CONVERT(bit, 1) AS isSelected, ";
            }
            qSql += " a.CompanyCode, a.BranchCode, 'INVOICE' AS TypeJournal, a.InvoiceNo AS DocNo, a.InvoiceDate AS DocDate, a.SONo AS RefNo";
            qSql += ", a.CustomerCode AS RefCode, b.CustomerName AS RefName, CASE ISNULL(a.Status, 0) WHEN '5' THEN 'Posted' ELSE 'UnPosted' END AS Status";
            qSql += " FROM OmTrSalesInvoice a";
            qSql += " LEFT JOIN GnMstCustomer b ON b.CompanyCode = a.CompanyCode AND b.CustomerCode = a.CustomerCode";
            qSql += " WHERE a.CompanyCode = '" + CompanyCode + "' AND a.BranchCode ='" + BranchCode + "' AND a.Status ='" + Status + "'";
            qSql += " AND a.InvoiceDate BETWEEN '" + DateFrom + "' AND '" + DateTo + "'";
            qSql += " UNION ";
            qSql += " SELECT ";
            if (Status == "5")
            {
                qSql += " CONVERT(bit, 1) AS isSelected, ";
            }
            qSql += " c.CompanyCode, c.BranchCode, 'DEBETNOTE' AS TypeJournal, c.DNNo AS DocNo, c.DNDate AS DocDate, c.InvoiceNo AS RefNo";
            qSql += ", c.CustomerCode AS RefCode, d.CustomerName AS RefName, CASE ISNULL(c.Status, 0) WHEN '5' THEN 'Posted' ELSE 'UnPosted' END AS Status";
            qSql += " FROM OmTrSalesDN c";
            qSql += " LEFT JOIN GnMstCustomer d ON d.CompanyCode = c.CompanyCode AND d.CustomerCode = c.CustomerCode";
            qSql += " WHERE c.CompanyCode ='" + CompanyCode + "' AND c.BranchCode ='" + BranchCode + "' AND c.Status ='" + Status + "'";
            qSql += " AND c.DNDate BETWEEN '" + DateFrom + "' AND '" + DateTo + "'";
            qSql += " UNION ";
            qSql += " SELECT ";
            if (Status == "5")
            {
                qSql += " CONVERT(bit, 1) AS isSelected, ";
            }
            qSql += " e.CompanyCode, e.BranchCode, 'RETURN' AS TypeJournal, e.ReturnNo AS DocNo, e.ReturnDate AS DocDate, e.InvoiceNo AS RefNo";
            qSql += ", e.CustomerCode AS RefCode, f.CustomerName AS RefName, CASE ISNULL(e.Status, 0) WHEN '5' THEN 'Posted' ELSE 'UnPosted' END AS Status";
            qSql += " FROM OmTrSalesReturn e";
            qSql += " LEFT JOIN GnMstCustomer f ON f.CompanyCode = e.CompanyCode AND f.CustomerCode = e.CustomerCode";
            qSql += " WHERE e.CompanyCode ='" + CompanyCode + "' AND e.BranchCode ='" + BranchCode + "' AND e.Status ='" + Status + "'";
            qSql += " AND e.ReturnDate BETWEEN'" + DateFrom + "' AND '" + DateTo + "'";

            var queryable = ctx.Database.SqlQuery<JurnalPurchase>(qSql).AsQueryable();
            return Json(queryable);
        }

        private JsonResult SelectSales4INV(DateTime? DateFrom, DateTime? DateTo, string Status)
        {
            string qSql = "";
            qSql = "SELECT ";
            if (Status == "5")
            {
                qSql += " CONVERT(bit, 1) AS isSelected, ";
            }
            qSql += " a.CompanyCode, a.BranchCode, 'INVOICE' AS TypeJournal, a.InvoiceNo AS DocNo, a.InvoiceDate AS DocDate, a.SONo AS RefNo";
            qSql += ", a.CustomerCode AS RefCode, b.CustomerName AS RefName, CASE ISNULL(a.Status, 0) WHEN '5' THEN 'Posted' ELSE 'UnPosted' END AS Status";
            qSql += " FROM OmTrSalesInvoice a";
            qSql += " LEFT JOIN GnMstCustomer b ON b.CompanyCode = a.CompanyCode AND b.CustomerCode = a.CustomerCode";
            qSql += " WHERE a.CompanyCode = '" + CompanyCode + "' AND a.BranchCode ='" + BranchCode + "' AND a.Status ='" + Status + "'";
            qSql += " AND convert(varchar, a.InvoiceDate, 112) BETWEEN '" + Convert.ToDateTime(DateFrom).Date.ToString("yyyyMMdd") + "' AND '" + Convert.ToDateTime(DateTo).Date.ToString("yyyyMMdd") + "'";

            var queryable = ctx.Database.SqlQuery<JurnalPurchase>(qSql).AsQueryable();
            return Json(queryable);
        }

        private JsonResult SelectSales4DN(DateTime? DateFrom, DateTime? DateTo, string Status)
        {
            string qSql = "";
            qSql += " SELECT ";
            if (Status == "5")
            {
                qSql += " CONVERT(bit, 1) AS isSelected, ";
            }
            qSql += " a.CompanyCode, a.BranchCode, 'DEBETNOTE' AS TypeJournal, a.DNNo AS DocNo, a.DNDate AS DocDate, a.InvoiceNo AS RefNo";
            qSql += ", a.CustomerCode AS RefCode, b.CustomerName AS RefName, CASE ISNULL(a.Status, 0) WHEN '5' THEN 'Posted' ELSE 'UnPosted' END AS Status";
            qSql += " FROM OmTrSalesDN a";
            qSql += " LEFT JOIN GnMstCustomer b ON b.CompanyCode = a.CompanyCode AND b.CustomerCode = a.CustomerCode";
            qSql += " WHERE a.CompanyCode ='" + CompanyCode + "' AND a.BranchCode ='" + BranchCode + "' AND a.Status ='" + Status + "'";
            qSql += " AND a.DNDate BETWEEN '" + DateFrom + "' AND '" + DateTo + "'";

            var queryable = ctx.Database.SqlQuery<JurnalPurchase>(qSql).AsQueryable();
            return Json(queryable);
        }

        private JsonResult SelectSales4SR(DateTime? DateFrom, DateTime? DateTo, string Status)
        {
            string qSql = "";
            qSql += " SELECT ";
            if (Status == "5")
            {
                qSql += " CONVERT(bit, 1) AS isSelected, ";
            }
            qSql += " a.CompanyCode, a.BranchCode, 'RETURN' AS TypeJournal, a.ReturnNo AS DocNo, a.ReturnDate AS DocDate, a.InvoiceNo AS RefNo";
            qSql += ", a.CustomerCode AS RefCode, b.CustomerName AS RefName, CASE ISNULL(a.Status, 0) WHEN '5' THEN 'Posted' ELSE 'UnPosted' END AS Status";
            qSql += " FROM OmTrSalesReturn a";
            qSql += " LEFT JOIN GnMstCustomer b ON b.CompanyCode = a.CompanyCode AND b.CustomerCode = a.CustomerCode";
            qSql += " WHERE a.CompanyCode ='" + CompanyCode + "' AND a.BranchCode ='" + BranchCode + "' AND a.Status ='" + Status + "'";
            qSql += " AND a.ReturnDate BETWEEN'" + DateFrom + "' AND '" + DateTo + "'";

            var queryable = ctx.Database.SqlQuery<JurnalPurchase>(qSql).AsQueryable();
            return Json(queryable);
        }

        #endregion

        #region SelectInventory

        private JsonResult SelectInventory4ALL(DateTime? DateFrom, DateTime? DateTo, string Status)
        {
            var query = string.Format(@"uspfn_SelectInventory4All '{0}','{1}','{2}','{3}','{4}'
                ", CompanyCode, BranchCode, Status, DateFrom, DateTo);

            var queryable = ctx.Database.SqlQuery<JurnalInventory>(query).AsQueryable();
            return Json(queryable);
        }

        private JsonResult SelectInventory4VTO(DateTime? DateFrom, DateTime? DateTo, string Status)
        {
            var query = string.Format(@"
               select convert(bit, (case Status when '5' then '1' else '0' end)) as IsSelected
                 , case isnull(Status, 0) when '5' then 'Posted' else 'UnPosted' end as Status
	             , CompanyCode
	             , BranchCode
	             , 'TRANSFEROUT' as TypeJournal
	             , TransferOutNo as DocNo
	             , TransferOutDate as DocDate
	             , ReferenceNo as ReffNo
	             , BranchCodeFrom + '-' + WareHouseCodeFrom as BranchCodeFrom
	             , BranchCodeTo + '-' + WareHouseCodeTo as BranchCodeTo
              from omTrInventTransferOut
             where 1 = 1
               and CompanyCode = '{0}'
               and BranchCode  = '{1}'
               and Status      = '{2}'
               and BranchCodeFrom <> BranchCodeTo
               and TransferOutDate between '{3}' and '{4}'
                ", CompanyCode, BranchCode, Status, DateFrom, DateTo);

            var queryable = ctx.Database.SqlQuery<JurnalInventory>(query).AsQueryable();
            return Json(queryable);
        }

        private JsonResult SelectInventory4VTI(DateTime? DateFrom, DateTime? DateTo, string Status)
        {
            var query = string.Format(@"
               select convert(bit, (case Status when '5' then '1' else '0' end)) as IsSelected
                 , case isnull(Status, 0) when '5' then 'Posted' else 'UnPosted' end as Status
	             , CompanyCode
	             , BranchCode
	             , 'TRANSFERIN' as TypeJournal
	             , TransferInNo as DocNo
	             , TransferInDate as DocDate
	             , ReferenceNo as ReffNo
	             , BranchCodeFrom + '-' + WareHouseCodeFrom as BranchCodeFrom
	             , BranchCodeTo + '-' + WareHouseCodeTo as BranchCodeTo
              from omTrInventTransferIn
             where 1 = 1
               and CompanyCode = '{0}'
               and BranchCode  = '{1}'
               and Status      = '{2}'
               and BranchCodeFrom <> BranchCodeTo
               and TransferInDate between '{3}' and '{4}'
                ", CompanyCode, BranchCode, Status, DateFrom, DateTo);

            var queryable = ctx.Database.SqlQuery<JurnalInventory>(query).AsQueryable();
            return Json(queryable);
        }

        private JsonResult SelectInventory4VTOMB(DateTime? DateFrom, DateTime? DateTo, string Status)
        {
            var query = string.Format(@"
               select convert(bit, (case Status when '5' then '1' else '0' end)) as IsSelected
                 , case isnull(Status, 0) when '5' then 'Posted' else 'UnPosted' end as Status
	             , CompanyCode
	             , BranchCode
	             , 'TRANSFEROUTMULTIBRANCH' as TypeJournal
	             , TransferOutNo as DocNo
	             , TransferOutDate as DocDate
	             , ReferenceNo as ReffNo
	             , BranchCodeFrom + '-' + WareHouseCodeFrom as BranchCodeFrom
	             , BranchCodeTo + '-' + WareHouseCodeTo as BranchCodeTo
              from omTrInventTransferOutMultiBranch
             where 1 = 1
               and CompanyCode = '{0}'
               and BranchCode  = '{1}'
               and Status      = '{2}'
               and BranchCodeFrom <> BranchCodeTo
               and TransferOutDate between '{3}' and '{4}'
                ", CompanyCode, BranchCode, Status, DateFrom, DateTo);

            var queryable = ctx.Database.SqlQuery<JurnalInventory>(query).AsQueryable();
            return Json(queryable);
        }

        private JsonResult SelectInventory4VTIMB(DateTime? DateFrom, DateTime? DateTo, string Status)
        {
            var query = string.Format(@"
               select convert(bit, (case Status when '5' then '1' else '0' end)) as IsSelected
                 , case isnull(Status, 0) when '5' then 'Posted' else 'UnPosted' end as Status
	             , CompanyCode
	             , BranchCode
	             , 'TRANSFERINMULTIBRANCH' as TypeJournal
	             , TransferInNo as DocNo
	             , TransferInDate as DocDate
	             , ReferenceNo as ReffNo
	             , BranchCodeFrom + '-' + WareHouseCodeFrom as BranchCodeFrom
	             , BranchCodeTo + '-' + WareHouseCodeTo as BranchCodeTo
               from omTrInventTransferInMultiBranch
               where 1 = 1
               and CompanyCode = '{0}'
               and BranchCode  = '{1}'
               and Status      = '{2}'
               and BranchCodeFrom <> BranchCodeTo
               and TransferInDate between '{3}' and '{4}'
                ", CompanyCode, BranchCode, Status, DateFrom, DateTo);

            var queryable = ctx.Database.SqlQuery<JurnalInventory>(query).AsQueryable();
            return Json(queryable);
        }

        #endregion

        public JsonResult ComboTransaksi(Jurnal model)
        {
            var datas = new JsonResult(); 
            var msg = "";
            var success = true;
            if ((model.DateFrom < model.DateFromHide) || (model.DateFrom > model.DateToHide))
            {
                datas = Json(new { success = false, message = "Tanggal harus dalam range periode!" });
            }
            if ((model.DateTo < model.DateFromHide) || (model.DateTo > model.DateToHide))
            {
                datas = Json(new { success = false, message = "Tanggal harus dalam range periode!" });
            }
            if (model.DateFrom > model.DateTo)
            {
                datas = Json(new { success = false, message = "Tanggal dari harus lebih kecil atau sama dengan tanggal sampai" });
            }

            var datefrom = model.DateFrom;
            var dateto = model.DateTo.Value.AddDays(1);
            var status = "";

            if (success == true)
            {
                //Purchase
                if (model.Trans == "0")
                {
                    if (model.Status == "0")
                    {
                        if (model.Transaksi == "ALL")
                        {
                            datas = Json(new { success = true, data = SelectPurchase4ALL(datefrom, dateto, status = "5") });
                        }
                        else if (model.Transaksi == "HPP")
                        {
                            datas = Json(new { success = true, data = SelectPurchase4HPP(datefrom, dateto, status = "5") });
                        }
                        else if (model.Transaksi == "PR")
                        {
                            datas = Json(new { success = true, data = SelectPurchase4PR(datefrom, dateto, status = "5") });
                        }
                        else if (model.Transaksi == "KT")
                        {
                            datas = Json(new { success = true, data = SelectPurchase4KT(datefrom, dateto, status = "5") });
                        }
                    }
                    else
                    {
                        if (model.Transaksi == "ALL")
                        {
                            datas = Json(new { success = true, data = SelectPurchase4ALL(datefrom, dateto, status = "2") });
                        }
                        else if (model.Transaksi == "HPP")
                        {
                            status = model.Status;
                            datas = Json(new { success = true, data = SelectPurchase4HPP(datefrom, dateto, status = "2") });

                        }
                        else if (model.Transaksi == "PR")
                        {
                            datas = Json(new { success = true, data = SelectPurchase4PR(datefrom, dateto, status = "2") });
                        }
                        else if (model.Transaksi == "KT")
                        {
                            datas = Json(new { success = true, data = SelectPurchase4KT(datefrom, dateto, status = "2") });
                        }
                    }
                }
                //Sales
                else if (model.Trans == "1")
                {
                    if (model.Status == "0")
                    {
                        if (model.Transaksi == "ALL")
                        {
                            datas = Json(new { success = true, data = SelectSales4ALL(datefrom, dateto, status = "5") });
                        }
                        else if (model.Transaksi == "INV")
                        {
                            datas = Json(new { success = true, data = SelectSales4INV(datefrom, dateto, status = "5") });
                        }
                        else if (model.Transaksi == "DN")
                        {
                            datas = Json(new { success = true, data = SelectSales4DN(datefrom, dateto, status = "5") });
                        }
                        else if (model.Transaksi == "SR")
                        {
                            datas = Json(new { success = true, data = SelectSales4SR(datefrom, dateto, status = "5") });
                        }
                    }
                    else
                    {
                        if (model.Transaksi == "ALL")
                        {
                            datas = Json(new { success = true, data = SelectSales4ALL(datefrom, dateto, status = "2") });
                        }
                        else if (model.Transaksi == "INV")
                        {
                            datas = Json(new { success = true, data = SelectSales4INV(datefrom, dateto, status = "2") });
                        }
                        else if (model.Transaksi == "DN")
                        {
                            datas = Json(new { success = true, data = SelectSales4DN(datefrom, dateto, status = "2") });
                        }
                        else if (model.Transaksi == "SR")
                        {
                            datas = Json(new { success = true, data = SelectSales4SR(datefrom, dateto, status = "2") });
                        }
                    }
                }
                //Stock Inventory
                else
                {
                    if (model.Status == "0")
                    {
                        if (model.Transaksi == "ALL")
                        {
                            datas = Json(new { success = true, data = SelectInventory4ALL(datefrom, dateto, status = "5") });
                        }
                        else if (model.Transaksi == "VTO")
                        {
                            datas = Json(new { success = true, data = SelectInventory4VTO(datefrom, dateto, status = "5") });
                        }
                        else if (model.Transaksi == "VTI")
                        {
                            datas = Json(new { success = true, data = SelectInventory4VTI(datefrom, dateto, status = "5") });
                        }
                        else if (model.Transaksi == "VTOMB")
                        {
                            datas = Json(new { success = true, data = SelectInventory4VTOMB(datefrom, dateto, status = "5") });
                        }
                        else if (model.Transaksi == "VTIMB")
                        {
                            datas = Json(new { success = true, data = SelectInventory4VTIMB(datefrom, dateto, status = "5") });
                        }
                    }
                    else
                    {
                        if (model.Transaksi == "ALL")
                        {
                            datas = Json(new { success = true, data = SelectInventory4ALL(datefrom, dateto, status = "2") });
                        }
                        else if (model.Transaksi == "VTO")
                        {
                            datas = Json(new { success = true, data = SelectInventory4VTO(datefrom, dateto, status = "2") });
                        }
                        else if (model.Transaksi == "VTI")
                        {
                            datas = Json(new { success = true, data = SelectInventory4VTI(datefrom, dateto, status = "2") });
                        }
                        else if (model.Transaksi == "VTOMB")
                        {
                            datas = Json(new { success = true, data = SelectInventory4VTOMB(datefrom, dateto, status = "2") });
                        }
                        else if (model.Transaksi == "VTIMB")
                        {
                            datas = Json(new { success = true, data = SelectInventory4VTIMB(datefrom, dateto, status = "2") });
                        }
                    }
                }
            }
            return datas;
        }

        public JsonResult PostingJurnal(List<JurnalPurchase> model)
        {
            var query = "";
            int queryable = 0;
            try
            {
                if (model.Count() > 0)
                    {
                        foreach (JurnalPurchase recDtl in model)
                        {
                            if (recDtl.isSelected == true)
                            {
                                switch (recDtl.TypeJournal.ToLower())
                                {
                                    case "purchase":
                                        query = string.Format(@"uspfn_OmPostingPurchase '{0}','{1}','{2}','{3}'
                                    ", CompanyCode, BranchCode, recDtl.DocNo, CurrentUser.UserId);

                                        queryable = ctx.Database.ExecuteSqlCommand(query);
                                        break;
                                    case "purchasereturn":
                                        query = string.Format(@"uspfn_OmPostingPurchaseReturn '{0}','{1}','{2}','{3}'
                                    ", CompanyCode, BranchCode, recDtl.DocNo, CurrentUser.UserId);

                                        queryable = ctx.Database.ExecuteSqlCommand(query);
                                        break;
                                    case "karoseri":
                                        query = string.Format(@"uspfn_OmPostingKaroseri '{0}','{1}','{2}','{3}'
                                    ", CompanyCode, BranchCode, recDtl.DocNo, CurrentUser.UserId);

                                        queryable = ctx.Database.ExecuteSqlCommand(query);
                                        break;
                                    case "invoice":
                                        query = string.Format(@"uspfn_OmPostingSalesInvoice '{0}','{1}','{2}','{3}'
                                    ", CompanyCode, BranchCode, recDtl.DocNo, CurrentUser.UserId);

                                        queryable = ctx.Database.ExecuteSqlCommand(query);
                                        break;
                                    case "debetnote":
                                        // Cek LookUp
                                        var lookUp = ctx.LookUpDtls.Find(CompanyCode, "DNUFL", "STATUS");
                                        if (lookUp != null && lookUp.ParaValue == "1")
                                        {
                                            query = string.Format(@"uspfn_OmPostingSalesDNNew '{0}','{1}','{2}','{3}'
                                        ", CompanyCode, BranchCode, recDtl.DocNo, CurrentUser.UserId);

                                            queryable = ctx.Database.ExecuteSqlCommand(query);
                                        }
                                        else
                                        {
                                            query = string.Format(@"uspfn_OmPostingSalesDN '{0}','{1}','{2}','{3}'
                                        ", CompanyCode, BranchCode, recDtl.DocNo, CurrentUser.UserId);

                                            queryable = ctx.Database.ExecuteSqlCommand(query);
                                        }
                                        break;
                                    case "return":
                                        query = string.Format(@"uspfn_OmPostingReturn '{0}','{1}','{2}','{3}'
                                    ", CompanyCode, BranchCode, recDtl.DocNo, CurrentUser.UserId);

                                        queryable = ctx.Database.ExecuteSqlCommand(query);
                                        break;
                                    case "transferout":
                                        query = string.Format(@"uspfn_OmPostingTransOut '{0}','{1}','{2}','{3}'
                                    ", CompanyCode, BranchCode, recDtl.DocNo, CurrentUser.UserId);

                                        queryable = ctx.Database.ExecuteSqlCommand(query);
                                        break;
                                    case "transferin":
                                        query = string.Format(@"uspfn_OmPostingTransIn '{0}','{1}','{2}','{3}'
                                    ", CompanyCode, BranchCode, recDtl.DocNo, CurrentUser.UserId);

                                        queryable = ctx.Database.ExecuteSqlCommand(query);
                                        break;
                                    case "transferoutmultibranch":
                                        query = string.Format(@"uspfn_OmPostingTransOutMultiBranch '{0}','{1}','{2}','{3}'
                                    ", CompanyCode, BranchCode, recDtl.DocNo, CurrentUser.UserId);

                                        queryable = ctx.Database.ExecuteSqlCommand(query);
                                        break;
                                    case "transferinmultibranch":
                                        query = string.Format(@"uspfn_OmPostingTransInMultiBranch '{0}','{1}','{2}','{3}'
                                    ", CompanyCode, BranchCode, recDtl.DocNo, CurrentUser.UserId);

                                        queryable = ctx.Database.ExecuteSqlCommand(query);
                                        break;
                                }
                            }
                        }
                    }

                return Json(new { success = true, message = "Posting Data Berhasil" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message, error_log = ex.Message });
            }
        }
    }
}
