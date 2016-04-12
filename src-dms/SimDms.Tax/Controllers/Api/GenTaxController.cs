using SimDms.Common;
using SimDms.Common.Models;
using SimDms.Tax.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Tax.Controllers.Api
{
    public class GenTaxController : BaseController
    {
        public JsonResult Init()
        {
            bool isLinkFin = CurrentUser.CoProfile.IsLinkToFinance.Value;
            bool isLinkSP = CurrentUser.CoProfile.IsLinkToSpare.Value;
            bool isLinkSV = CurrentUser.CoProfile.IsLinkToService.Value;
            bool isLinkSL = CurrentUser.CoProfile.IsLinkToSales.Value;

            bool result = false;

            var isHolding = (from p in ctx.CoProfiles
                             join p1 in ctx.OrganizationDtls on new { p.CompanyCode, p.BranchCode } equals new { p1.CompanyCode, p1.BranchCode }
                             where p.CompanyCode == CompanyCode && p1.IsBranch == false
                             select new
                             {
                                 IsFPJCentralized = p.IsFPJCentralized
                             }).FirstOrDefault();

            if (isHolding.IsFPJCentralized.Value && ctx.OrganizationDtls.Find(CompanyCode, BranchCode).IsBranch)
                result = true;
            
            var coProfileFin = ctx.CoProfileFinances.Find(CompanyCode, BranchCode);
            var periodeBegAR = coProfileFin.PeriodBegAR.Value.ToString("dd-MMM-yyyy");
            var periodeEndAR = coProfileFin.PeriodEndAR.Value.ToString("dd-MMM-yyyy");
           
            return Json(new
            {
                isLinkFin = isLinkFin,
                isLinkSP = isLinkSP,
                isLinkSV = isLinkSV,
                isLinkSL = isLinkSL,
                isHolding = result,
                ProfitCenter = ProfitCenter,
                periodeBegAR = periodeBegAR,
                periodeEndAR = periodeEndAR, 
                fpjDate = DateTime.Now.ToString("dd-MMM-yyyy"),
                bFPJCentralized = isHolding.IsFPJCentralized.Value
            });
        }

        public JsonResult CheckSettingTXOL()
        {
            var dt = ctx.LookUpDtls.Where(p => p.CompanyCode == CompanyCode && p.CodeID == GnMstLookUpHdr.TaxOnline).ToList();

            if (dt.Count == 0) return Json(new { success = false, message = "Mohon setting parameter destination untuk Kode Company dan Branch Company di Master Lookup (TXOL)" });
            else return Json(new { success = true, message = "" });
        }

        public JsonResult ValidateQueryFpj(DateTime hiddenFromDate, DateTime hiddenEndDate, DateTime fpjDate, bool bFPJCentralized, string profitCenter, bool bFPJG)
        {
            string message = string.Empty;
            var lookupStsCpjk = ctx.LookUpDtls.Find(CompanyCode, "CPJK", "STATUS");
            bool bAllowOverPeriod = false;
            if (lookupStsCpjk != null)
            {
                if (lookupStsCpjk.ParaValue == "1") bAllowOverPeriod = true;
            }
            DateTime endDate = new DateTime();
            var coProfileFin = ctx.CoProfileFinances.Find(CompanyCode, BranchCode);

            if (bAllowOverPeriod)
            {
                message = Check3SClosingStatus(fpjDate, bFPJCentralized);
                if (message != string.Empty) return Json(new { message = message });
            }
            else
                endDate = hiddenEndDate.Date;

             if (fpjDate < hiddenFromDate || fpjDate > hiddenEndDate) {
                message = "Tanggal harus dalam periode " + hiddenFromDate.Date.ToString("dd-MMM-yyyy") + " s/d " + hiddenEndDate.Date.ToString("dd-MMM-yyyy");
                return Json(new
                {
                    message = message

                });
            }

             string sql1 = string.Format(
@"SELECT LookUpValue , ParaValue 
FROM gnMstLookUpDtl 
WHERE CompanyCode= '{0}'
    AND CodeID = 'BRANCH' 
	AND ParaValue = 
	( 
		SELECT ParaValue FROM gnMstLookUpDtl 
		WHERE CodeID = 'BRANCH' 
		AND LookUpValue = '{1}'
	)
", CompanyCode, BranchCode);

             DataTable dtBranch = MyHelpers.GetTable(ctx, sql1);
             string branchCode = string.Empty;
             if (dtBranch.Rows.Count > 0)
             {
                 foreach (DataRow row in dtBranch.Rows)
                     branchCode += string.Format(",'{0}'", row["LookUpValue"]);
                 if (branchCode.Length > 0) branchCode = branchCode.Substring(1);
             }
             else
                 branchCode = "'" + BranchCode + "'";

             string sql2 = @"select 
    row_number() over(order by y.BranchCode,y.DocDate,y.CustomerCodeBill, y.ProfitCenter, convert(varchar, convert(datetime,y.DocDate), 106) asc) as No
    ,y.chkSelect,y.CompanyCode,y.BranchCode,y.ProfitCenter,isnull(y.FPJGovNo,'') FPJGovNo
    ,isnull(y.FPJGovDate,'')FPJGovDate,y.DocNo, convert(varchar, convert(datetime,y.DocDate), 106) DocDate, y.CustomerCodeBill, y.CustName, y.InvNo
from(
    select 
        Convert(bit, 0) chkSelect, x.CompanyCode, x.BranchCode
        , a.LookUpValueName ProfitCenter, x.FPJGovNo, x.FPJGovDate
        , x.DocNo, x.DocDate, x.CustomerCodeBill, x.CustName, x.InvNo
    from (
	    SELECT	CompanyCode, BranchCode,'300' AS ProfitCenter,FPJGovNo
			    ,NULL AS FPJGovDate,InvoiceNo as DocNo,convert(varchar,InvoiceDate,112) AS DocDate, CustomerCodeBill
                ,(SELECT CustomerName FROM gnMstCustomer WHERE CustomerCode = CustomerCodeBill) CustName
			    , FPJNo InvNo  
	    FROM	SpTrnSFPJHdr
	    WHERE	CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
			        or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
		        )
                AND isPKP = 1 
                AND ISNULL(FPJGovNo, '') = ''
			    AND CONVERT(VARCHAR, InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
	    UNION
	    SELECT	a.CompanyCode, a.BranchCode,'200' AS ProfitCenter,FPJGovNo
	            ,NULL AS FPJGovDate,a.FPJNo as DocNo,convert(varchar,a.FPJDate,112) AS DocDate, a.CustomerCodeBill
	            ,(SELECT CustomerName FROM gnMstCustomer WHERE CustomerCode = a.CustomerCodeBill) CustName
	            , b.InvoiceNo InvNo  
        FROM	SvTrnFakturPajak a
        LEFT JOIN svTrnInvoice b ON a.CompanyCode = b.CompanyCode AND
	        a.BranchCode = b.BranchCode AND
	        a.FPJNo = b.FPJNo
	    WHERE	a.CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = 'True' then a.BranchCode end) <> ''
			        or (case when @IsFPJCentral = 'False' then a.BranchCode end) in (@BranchCode)
		        )
                AND a.isPKP = 1 AND ISNULL(a.FPJGovNo, '') = ''
			    AND CONVERT(VARCHAR, a.FPJDate, 112) BETWEEN @StartDate AND @FPJDate
	    UNION
	    SELECT	a.CompanyCode, a.BranchCode,'100' AS ProfitCenter,a.FakturPajakNo FPJGovNo
			    ,NULL AS FPJGovDate,a.InvoiceNo as DocNo,convert(varchar,a.InvoiceDate,112) AS DocDate, a.CustomerCode
			    , b.CustomerName CustName			    
			    , c.InvoiceNo InvNo
	    FROM	OmFakturPajakHdr a
	    LEFT JOIN gnMstCustomer b ON a.CompanyCode = b.CompanyCode
			AND a.CustomerCode = b.CustomerCode
		LEFT JOIN OmTrSalesInvoice c ON a.CompanyCode = c.CompanyCode
			AND a.BranchCode = c.BranchCode
			AND a.InvoiceNo = c.InvoiceNo
	    WHERE	a.CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = 'True' then a.BranchCode end) <> ''
			        or (case when @IsFPJCentral = 'False' then a.BranchCode end) in (@BranchCode)
		        ) 
                AND a.TaxType = 'Standard' AND ISNULL(a.FakturPajakNo, '') = ''
			    AND CONVERT(VARCHAR, a.InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
	    UNION
	    SELECT	a.CompanyCode, a.BranchCode,'000' AS ProfitCenter,FakturPajakNo FPJGovNo
			    ,NULL AS FPJGovDate,a.InvoiceNo as DocNo,convert(varchar,a.InvoiceDate,112) AS DocDate, a.CustomerCode
			    , b.CustomerName CustName			    
			    , c.InvoiceNo InvNo
	    FROM	ArFakturPajakHdr a
	    LEFT JOIN gnMstCustomer b ON a.CompanyCode = b.CompanyCode
			AND a.CustomerCode = b.CustomerCode
		LEFT JOIN arTrnInvoiceHdr c ON a.CompanyCode = c.CompanyCode	
			AND a.BranchCode = c.BranchCode
			AND a.InvoiceNo = c.InvoiceNo
	    WHERE	a.CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = 'True' then a.BranchCode end) <> ''
			        or (case when @IsFPJCentral = 'False' then a.BranchCode end) in (@BranchCode)
		        )
                AND a.TaxType = 'Standard' AND ISNULL(a.FakturPajakNo, '') = ''
			    AND CONVERT(VARCHAR, a.InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
                AND a.DPPAMt > 0
    ) x 
    left join gnMstLookUpDtl a on a.CompanyCode= x.CompanyCode and a.CodeID='PFCN' and a.LookUpValue= x.ProfitCenter
    where x.ProfitCenter like ''+@ProfitCenter+''
) y";

             if (bFPJG)
                 sql2 = sql2 + " inner join gnMstLookupDtl x on x.CompanyCode = y.CompanyCode and x.CodeID = 'FPJG' and x.LookupValue = y.CustomerCodeBill";
             else
                 sql2 = sql2 + " left join gnMstLookupDtl x on x.CompanyCode = y.CompanyCode and x.CodeID = 'FPJG' and x.LookupValue = y.CustomerCodeBill where x.LookupValue is null";

             sql2 = sql2 + " order by convert(varchar, convert(datetime,y.DocDate), 106)";

             sql2 = sql2.Replace("@CompanyCode", string.Format("'{0}'", CompanyCode));
             sql2 = sql2.Replace("@BranchCode", string.Format("{0}", branchCode));
             sql2 = sql2.Replace("@StartDate", string.Format("'{0}'", hiddenFromDate.ToString("yyyyMMdd")));
             sql2 = sql2.Replace("@FPJDate", string.Format("'{0}'", fpjDate.ToString("yyyyMMdd")));
             sql2 = sql2.Replace("@ProfitCenter", string.Format("'{0}'", profitCenter));
             sql2 = sql2.Replace("@IsFPJCentral", string.Format("'{0}'", bFPJCentralized));
               
             var fpjList = ctx.Database.SqlQuery<GenTax>(sql2).ToList();
             if (fpjList.Count == 0) message = "Tidak ada data yang ditampilkan";

            return Json(new
            {
                message = message, data = fpjList
            });
        }

        private string Check3SClosingStatus(DateTime dtFPJ, bool bFPJCentralized)
        {
            string msg = "Ada cabang: \n", branch = "";

            string sql = string.Format(@"
select * from gnMstPeriode
where CompanyCode= '{0}'
    and BranchCode = (case when '{1}'='' then BranchCode else '{1}' end)
	and Year(EndDate)=Year('{2}') and Month(EndDate)=Month('{2}')
	and (StatusSparepart <> 1  or StatusSales <> 1 or StatusService <> 1)
", CompanyCode, (bFPJCentralized == true) ? "" : BranchCode, dtFPJ);
            var dt3SClosingStatus = ctx.Database.SqlQuery<Period>(sql).ToArray();

            if (dt3SClosingStatus.Count() > 0)
            {
                foreach (Period row in dt3SClosingStatus)
                    branch += ("\n - " + row.BranchCode);
            }
            if (!string.IsNullOrEmpty(branch))
            {
                branch = branch.Substring(1);
                msg += (branch + "\nSparepart,Service, atau Salesnya yang belum tutup.");
            }
            else
            {
                msg = "";
            }
                  

            return msg;
        }
         
        public DataTable CheckValidTransaction(DateTime startDate, DateTime fpjDate, bool isFPJCentral)
        {
            string sql = @"
SELECT TR.* FROM 
( 
    SELECT 
		CompanyCode , BranchCode , '300' AS ProfitCenterCode , InvoiceNo AS DocNo , 
		InvoiceDate AS DocDate , DueDate , FPJNo AS RefNo , FPJDate AS RefDate 
    FROM 
		SPTrnSFPJHdr 
    WHERE 
		CompanyCode = @CompanyCode
		AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
				or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
			)
		AND isPKP = 1 
		AND isnull ( FPJGovNo , '' ) = '' 
		AND convert ( varchar , InvoiceDate , 112 ) BETWEEN @StartDate AND @FPJDate
    UNION 

    SELECT 
		CompanyCode , BranchCode , '200' AS ProfitCenterCode , FPJNo AS DocNo , 
		FPJDate AS DocDate , DueDate , '' AS RefNo , NULL AS RefDate 
    FROM 
		SVTrnFakturPajak 
    WHERE 
		CompanyCode = @CompanyCode
		AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
				or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
			)
		AND isPKP = 1 
		AND isnull ( FPJGovNo , '' ) = '' 
		AND convert ( varchar , FPJDate , 112 ) BETWEEN @StartDate AND @FPJDate
    UNION 

    SELECT 
		CompanyCode , BranchCode , '100' AS ProfitCenterCode , InvoiceNo AS DocNo , 
		InvoiceDate AS DocDate , DueDate , '' AS RefNo , NULL AS RefDate 
    FROM 
		OmFakturPajakHdr 
    WHERE 
		CompanyCode = @CompanyCode
		AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
				or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
			)
		AND TaxType = 'Standard' 
		AND isnull ( FakturPajakNo , '' ) = '' 
		AND convert ( varchar , InvoiceDate , 112 ) BETWEEN @StartDate AND @FPJDate
    UNION 

    SELECT 
		CompanyCode , BranchCode , '000' AS ProfitCenterCode , InvoiceNo AS DocNo , 
		InvoiceDate AS DocDate , DueDate , FPJNo AS RefNo , FPJDate AS RefDate 
    FROM 
		ARFakturPajakHdr 
    WHERE 
		CompanyCode = @CompanyCode
		AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
				or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
			)
		AND TaxType = 'Standard' 
		AND isnull ( FakturPajakNo , '' ) = '' 
		AND convert ( varchar , InvoiceDate , 112 ) BETWEEN @StartDate AND @FPJDate 
) 
AS TR
";
            sql = sql.Replace("@CompanyCode", string.Format("'{0}'", CompanyCode));
            sql = sql.Replace("@BranchCode", string.Format("{0}", BranchCode));
            sql = sql.Replace("@StartDate", string.Format("'{0}'", startDate.Date.ToString("yyyyMMdd")));
            sql = sql.Replace("@FPJDate", string.Format("'{0}'", fpjDate.Date.ToString("yyyyMMdd")));
            sql = sql.Replace("@IsFPJCentral", string.Format("'{0}'", isFPJCentral));

            DataTable dt = MyHelpers.GetTable(ctx, sql);

            return dt;
        }

        public bool CheckPendingDocumentFPJ(DateTime startDate, DateTime fpjDate, string docNo, bool isFPJCentral, bool isFPJGU)
        {
            bool result = false;
            
            string sql = @"
select x.*
from(
    SELECT	BranchCode,InvoiceNo, convert(varchar,InvoiceDate,112) InvoiceDate
    FROM	OmFakturPajakHdr
    LEFT JOIN gnMstLookupDtl on gnMstLookupDtl.CompanyCode = OmFakturPajakHdr.CompanyCode
			AND gnMstLookupDtl.LookupValue = CustomerCode
			AND gnMstLookupDtl.CodeID = 'FPJG'
    WHERE	OmFakturPajakHdr.CompanyCode = @CompanyCode 
			AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
				or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
			)
			AND TaxType = 'Standard'
		    AND CONVERT(VARCHAR, InvoiceDate, 112) >= @StartDate AND CONVERT(VARCHAR, InvoiceDate, 112) < @FPJDate  
            AND (CASE WHEN @isFPJGU = 'False' THEN gnMstLookupDtl.LookupValue ELSE isnull(gnMstLookupDtl.LookupValue, '')  END) = (CASE WHEN @isFPJGU = 'False' THEN gnMstLookupDtl.LookupValue ELSE '' END)
	    UNION
    SELECT	BranchCode,InvoiceNo, convert(varchar,InvoiceDate,112) InvoiceDate
    FROM	ARFakturPajakHdr
    LEFT JOIN gnMstLookupDtl on gnMstLookupDtl.CompanyCode = ARFakturPajakHdr.CompanyCode
			AND gnMstLookupDtl.LookupValue = CustomerCode
			AND gnMstLookupDtl.CodeID = 'FPJG'
    WHERE	ARFakturPajakHdr.CompanyCode = @CompanyCode 
			AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
				or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
			)
			AND TaxType = 'Standard'
		    AND CONVERT(VARCHAR, InvoiceDate, 112) >= @StartDate AND CONVERT(VARCHAR, InvoiceDate, 112) < @FPJDate
            AND (CASE WHEN @isFPJGU = 'False' THEN gnMstLookupDtl.LookupValue ELSE isnull(gnMstLookupDtl.LookupValue, '')  END) = (CASE WHEN @isFPJGU = 'False' THEN gnMstLookupDtl.LookupValue ELSE '' END)
	    UNION
    SELECT	BranchCode,InvoiceNo, convert(varchar,InvoiceDate,112) InvoiceDate
    FROM	SPTrnSFPJHdr
     LEFT JOIN gnMstLookupDtl on gnMstLookupDtl.CompanyCode = SPTrnSFPJHdr.CompanyCode
			AND gnMstLookupDtl.LookupValue = CustomerCode
			AND gnMstLookupDtl.CodeID = 'FPJG'
    WHERE	SPTrnSFPJHdr.CompanyCode = @CompanyCode 
			AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
				or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
			)
			AND isPKP = 1
		    AND CONVERT(VARCHAR, InvoiceDate, 112) >= @StartDate AND CONVERT(VARCHAR, InvoiceDate, 112) < @FPJDate
            AND (CASE WHEN @isFPJGU = 'False' THEN gnMstLookupDtl.LookupValue ELSE isnull(gnMstLookupDtl.LookupValue, '')  END) = (CASE WHEN @isFPJGU = 'False' THEN gnMstLookupDtl.LookupValue ELSE '' END)
	    UNION
    SELECT	BranchCode,FPJNO AS  InvoiceNo, convert(varchar,FPJDate,112) AS InvoiceDate 
    FROM	SVTrnFakturPajak
    WHERE	CompanyCode = @CompanyCode 
			AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
				or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
			)
			AND isPKP = 1
		    AND CONVERT(VARCHAR, FPJDate, 112) >= @StartDate AND CONVERT(VARCHAR, FPJDate, 112) < @FPJDate
		    AND (select TOP 1 TotalSrvAmt 
				   from svTrnInvoice 
				  where CompanyCode = @CompanyCode 
				    AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
							or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
							) 
					and FPJNo = SVTrnFakturPajak.FPJNo) <> 0
            AND EXISTS (SELECT ParaValue FROM gnMstLookupDtl WHERE CodeID = 'FPJG' AND LookupValue = (CASE WHEN @isFPJGU = 'False' THEN '' ELSE CustomerCode END))
    EXCEPT
    SELECT	BranchCode,DocNo AS InvoiceNo, convert(varchar,DocDate,112) AS InvoiceDate 
    FROM	GnGenerateTax
    WHERE	CompanyCode = @CompanyCode 
			AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
				or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
			)
		    AND CONVERT(VARCHAR, DocDate, 112) >= @StartDate AND CONVERT(VARCHAR, DocDate, 112) < @FPJDate
) x
                ";
            if (!string.IsNullOrEmpty(docNo))
                sql += " where @DocNo not like '%|'+x.BranchCode+' '+x.InvoiceNo+'|%'";

            sql = sql.Replace("@CompanyCode", string.Format("'{0}'", CompanyCode));
            sql = sql.Replace("@BranchCode", string.Format("{0}", BranchCode));
            sql = sql.Replace("@StartDate", string.Format("'{0}'", startDate.Date.ToString("yyyyMMdd")));
            sql = sql.Replace("@FPJDate", string.Format("'{0}'", fpjDate.Date.ToString("yyyyMMdd")));
            sql = sql.Replace("@IsFPJCentral", string.Format("'{0}'", isFPJCentral));
            sql = sql.Replace("@DocNo", string.Format("'{0}'", docNo));
            sql = sql.Replace("@isFPJGU", string.Format("'{0}'", isFPJGU));

            DataTable dt = MyHelpers.GetTable(ctx, sql);
           
            if (dt.Rows.Count > 0)
            {
                result = true;
            }
            return result;
        }

        public DataTable CheckClosingData(DateTime startDate)
        {
            string qSql = @"
SELECT BranchCode, StatusSparepart, StatusService, StatusSales
FROM GnMstPeriode
WHERE CompanyCode = @CompanyCode 
    AND BranchCode in (@BranchCode)
    AND FiscalStatus = 1
    AND CONVERT(VARCHAR, FromDate, 112) = DATEADD(mm, -1, @StartDate)";

            qSql = qSql.Replace("@CompanyCode", string.Format("'{0}'", CompanyCode));
            qSql = qSql.Replace("@BranchCode", string.Format("{0}", BranchCode));
            qSql = qSql.Replace("@StartDate", string.Format("'{0}'", startDate.Date.ToString("yyyyMMdd")));

            DataTable dt = MyHelpers.GetTable(ctx, qSql);
            
            return dt;
        }

        public DataTable GetMinTransDate(DateTime fpjDate)
        {
            string sql = @"
SELECT TR.BranchCode, TR.ProfitCenter , TR.TransDate 
FROM 
( 
    SELECT BranchCode,'Sparepart' AS ProfitCenter , isnull ( TransDate , '19000101' ) AS TransDate 
    FROM GnMstCoProfileSpare 
    WHERE CompanyCode = @CompanyCode 
        AND BranchCode in (@BranchCode)
    UNION 

    SELECT BranchCode,'Service' AS ProfitCenter , isnull ( TransDate , '19000101' ) AS TransDate 
    FROM GnMstCoProfileService 
    WHERE CompanyCode = @CompanyCode
        AND BranchCode in (@BranchCode)
    UNION 

    SELECT BranchCode,'Unit' AS ProfitCenter , isnull ( TransDate , '19000101' ) AS TransDate 
    FROM GnMstCoProfileSales 
    WHERE CompanyCode = @CompanyCode
        AND BranchCode in (@BranchCode)
    UNION 

    SELECT BranchCode,'Finance' AS ProfitCenter , isnull ( TransDateAR , '19000101' ) AS TransDate 
    FROM GnMstCoProfileFinance 
    WHERE CompanyCode = @CompanyCode 
        AND BranchCode in (@BranchCode)
) 
AS TR WHERE convert ( varchar , TR.TransDate , 112 ) < @FPJDate
";

            sql = sql.Replace("@CompanyCode", string.Format("'{0}'", CompanyCode));
            sql = sql.Replace("@BranchCode", string.Format("{0}", BranchCode));
            sql = sql.Replace("@FPJDate", string.Format("'{0}'", fpjDate.ToString("yyyyMMdd")));

            DataTable dt = MyHelpers.GetTable(ctx, sql);
            return dt;
        }

        public bool CheckPendingDocument(DateTime startDate, DateTime fpjDate, bool isFPJCentral, bool isFPJGU)
        {
            bool result = false;

            string sql = @"
SELECT InvoiceNo , InvoiceDate 
FROM OmTrSalesInvoice 
WHERE CompanyCode = @CompanyCode 
	AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
			or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
		)
	AND isStandard = '1' 
	AND isnull ( FakturPajakNo , '' ) = '' 
	AND Status <> '3' 
	AND convert ( varchar , InvoiceDate , 112 ) BETWEEN @StartDate AND @FPJDate 
    AND EXISTS (SELECT ParaValue FROM gnMstLookupDtl WHERE CodeID = 'FPJG' AND LookupValue = (CASE WHEN @isFPJGU = 'False' THEN '' ELSE CustomerCode END))
EXCEPT 
SELECT InvoiceNo , InvoiceDate 
FROM OmFakturPajakHdr 
WHERE CompanyCode = @CompanyCode 
	AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
			or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
		)
	AND TaxType = 'Standard' 
	AND convert ( varchar , InvoiceDate , 112 ) BETWEEN @StartDate AND @FPJDate 

UNION 

SELECT InvoiceNo , InvoiceDate 
FROM ARTrnInvoiceHdr 
WHERE CompanyCode = @CompanyCode 
	AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
			or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
		) 
	AND StatusTax = '1' 
	AND isnull ( FPJNo , '' ) = '' 
	AND Status <> '1' 
	AND convert ( varchar , InvoiceDate , 112 ) BETWEEN @StartDate AND @FPJDate 
    AND EXISTS (SELECT ParaValue FROM gnMstLookupDtl WHERE CodeID = 'FPJG' AND LookupValue = (CASE WHEN @isFPJGU = 'False' THEN '' ELSE CustomerCode END))
EXCEPT 
SELECT InvoiceNo , InvoiceDate 
FROM ARFakturPajakHdr 
WHERE CompanyCode = @CompanyCode 
	AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
			or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
		)
	AND TaxType = 'Standard' 
	AND convert ( varchar , InvoiceDate , 112 ) BETWEEN @StartDate AND @FPJDate
";

            sql = sql.Replace("@CompanyCode", string.Format("'{0}'", CompanyCode));
            sql = sql.Replace("@BranchCode", string.Format("{0}", BranchCode));
            sql = sql.Replace("@StartDate", string.Format("'{0}'", startDate.Date.ToString("yyyyMMdd")));
            sql = sql.Replace("@FPJDate", string.Format("'{0}'", fpjDate.Date.ToString("yyyyMMdd")));
            sql = sql.Replace("@IsFPJCentral", string.Format("'{0}'", isFPJCentral));
            sql = sql.Replace("@isFPJGU", string.Format("'{0}'", isFPJGU));

            DataTable dt = MyHelpers.GetTable(ctx, sql);
            if (dt.Rows.Count > 0)
            {
                result = true;
            }
            return result;
        }

        public DataTable SelectPendingDocumentGab(DateTime startDate, DateTime fpjDate, string docNo, bool isFPJCentral, bool isCheckAll, bool isFPJGU)
        {
            DataTable dt = MyHelpers.GetTable(ctx, string.Format("exec usprpt_GnTaxDocPendingNew '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', {7}", CompanyCode, BranchCode, 
                startDate.ToString("yyyyMMdd"), fpjDate.ToString("yyyyMMdd"), (isFPJCentral) ? "True" : "False", (isCheckAll) ? "True" : "False", docNo, isFPJGU));
            
            return dt;
        }

        public JsonResult IsValidTransDate(DateTime startDate, DateTime fpjDate, string docNo, string profitCenter, bool isFPJCentral,
            bool isLinkFin, bool isLinkSP, bool isLinkSV, bool isLinkSL, bool bCheckAll, bool isFPJGU)
        {
            string errMsg = ""; bool bProcess = true;

            DataTable dtCheck = CheckValidTransaction(startDate.Date, fpjDate.Date, isFPJCentral);
            if (dtCheck.Rows.Count == 0)
            {
                if (!CheckPendingDocumentFPJ(startDate.Date, fpjDate.Date, docNo, isFPJCentral, isFPJGU))
                {
                    if (CheckPendingDocument(startDate.Date, fpjDate.Date, isFPJCentral, isFPJGU))
                    {
                        errMsg += "#INFO# Ada Transaksi yang pending";
                        return Json(new { success = false, message = errMsg });
                    }
                    errMsg += "#INFO# Tidak ada Transaksi yang belum generate No.Faktur Pajak";
                    return Json(new { success = false, message = errMsg });
                }
            }

            if (!isLinkFin)
            {
                DataTable dtClosing = CheckClosingData(startDate.Date);
                errMsg += "Generate Faktur Pajak Tidak Bisa Dilakukan karena : \r\n";
                foreach (DataRow drClosing in dtClosing.Rows)
                {
                    if (drClosing != null && drClosing.ItemArray.Length > 0)
                    {
                        if (isLinkSP)
                            if (drClosing["StatusSparepart"].ToString() != "2")
                                errMsg += "\r\n - Sparepart [" + drClosing["BranchCode"].ToString() + "] untuk periode tersebut belum melakukan tutup bulan";
                        if (isLinkSV)
                            if (drClosing["StatusService"].ToString() != "2")
                                errMsg += "\r\n - Service [" + drClosing["BranchCode"].ToString() + "] untuk periode tersebut belum melakukan tutup bulan";
                        if (isLinkSL)
                            if (drClosing["StatusSales"].ToString() != "2")
                                errMsg += "\r\n - Sales/Unit [" + drClosing["BranchCode"].ToString() + "] untuk periode tersebut belum melakukan tutup bulan";

                        if (errMsg != string.Empty) return Json(new { success = false, message = errMsg, bProcess = bProcess });
                    }
                }
            }
            bool bPending = false; errMsg = "";

            DataTable dtTransDate = GetMinTransDate(fpjDate.Date);
            if (dtTransDate != null && dtTransDate.Rows.Count > 0)
            {
                foreach (DataRow row in dtTransDate.Rows)
                {
                    if (isLinkSP && row["ProfitCenter"].ToString() == "Sparepart")
                        errMsg += "\r\n - [" + row["BranchCode"].ToString() + "] Profit Center " + row["ProfitCenter"].ToString() + ", Transaksi terakhir tanggal " + Convert.ToDateTime(row["TransDate"]).Date.ToString("dd-MMM-yyyy");
                    if (isLinkSV && row["ProfitCenter"].ToString() == "Service")
                        errMsg += "\r\n - [" + row["BranchCode"].ToString() + "] Profit Center " + row["ProfitCenter"].ToString() + ", Transaksi terakhir tanggal " + Convert.ToDateTime(row["TransDate"]).Date.ToString("dd-MMM-yyyy");
                    if (isLinkSL && row["ProfitCenter"].ToString() == "Unit")
                        errMsg += "\r\n - [" + row["BranchCode"].ToString() + "] Profit Center " + row["ProfitCenter"].ToString() + ", Transaksi terakhir tanggal " + Convert.ToDateTime(row["TransDate"]).Date.ToString("dd-MMM-yyyy");
                    if (isLinkFin && row["ProfitCenter"].ToString() == "Finance")
                        errMsg += "\r\n - [" + row["BranchCode"].ToString() + "] Profit Center " + row["ProfitCenter"].ToString() + ", Transaksi terakhir tanggal " + Convert.ToDateTime(row["TransDate"]).Date.ToString("dd-MMM-yyyy");
                }
            }
            if (CheckPendingDocument(startDate.Date, fpjDate.Date, isFPJCentral, isFPJGU))
                bPending = true;

            if (CheckPendingDocumentFPJ(startDate.Date, fpjDate.Date, docNo, isFPJCentral, isFPJGU))
                bPending = true;

            DataTable dtPending = new DataTable();
            if (bPending)
            {
                if (bCheckAll)
                {
                    string sql1 = string.Format(
@"SELECT LookUpValue , ParaValue 
FROM gnMstLookUpDtl 
WHERE CompanyCode= '{0}'
    AND CodeID = 'BRANCH' 
	AND ParaValue = 
	( 
		SELECT ParaValue FROM gnMstLookUpDtl 
		WHERE CodeID = 'BRANCH' 
		AND LookUpValue = '{1}'
	)
", CompanyCode, BranchCode);

                    DataTable dtBranch = MyHelpers.GetTable(ctx, sql1);
                    string branchCode = string.Empty;
                    if (dtBranch.Rows.Count > 0)
                    {
                        foreach (DataRow row in dtBranch.Rows)
                            branchCode += string.Format(",'{0}'", row["LookUpValue"]);
                        if (branchCode.Length > 0) branchCode = branchCode.Substring(1);
                    }
                    else
                        branchCode = "'" + BranchCode + "'";

                    string sql2 = @"select 
    row_number() over(order by y.BranchCode,y.DocDate,y.CustomerCodeBill, y.ProfitCenter asc) as No
    ,y.chkSelect,y.CompanyCode,y.BranchCode,y.ProfitCenter,isnull(y.FPJGovNo,'') FPJGovNo
    ,isnull(y.FPJGovDate,'')FPJGovDate,y.DocNo, convert(varchar, convert(datetime,y.DocDate), 106) DocDate, y.CustomerCodeBill
from(
    select 
        Convert(bit, 0) chkSelect, x.CompanyCode, x.BranchCode
        , a.LookUpValueName ProfitCenter, x.FPJGovNo, x.FPJGovDate
        , x.DocNo, x.DocDate, x.CustomerCodeBill
    from (
	    SELECT	CompanyCode, BranchCode,'300' AS ProfitCenter,FPJGovNo
			    ,NULL AS FPJGovDate,InvoiceNo as DocNo,convert(varchar,InvoiceDate,112) AS DocDate, CustomerCodeBill
	    FROM	SpTrnSFPJHdr
	    WHERE	CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
			        or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
		        )
                AND isPKP = 1 
                AND ISNULL(FPJGovNo, '') = ''
			    AND CONVERT(VARCHAR, InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
	    UNION
	    SELECT	CompanyCode, BranchCode,'200' AS ProfitCenter,FPJGovNo
			    ,NULL AS FPJGovDate,FPJNo as DocNo,convert(varchar,FPJDate,112) AS DocDate, CustomerCodeBill
	    FROM	SvTrnFakturPajak
	    WHERE	CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
			        or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
		        )
                AND isPKP = 1 AND ISNULL(FPJGovNo, '') = ''
			    AND CONVERT(VARCHAR, FPJDate, 112) BETWEEN @StartDate AND @FPJDate
	    UNION
	    SELECT	CompanyCode, BranchCode,'100' AS ProfitCenter,FakturPajakNo FPJGovNo
			    ,NULL AS FPJGovDate,InvoiceNo as DocNo,convert(varchar,InvoiceDate,112) AS DocDate, CustomerCode
	    FROM	OmFakturPajakHdr
	    WHERE	CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
			        or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
		        ) 
                AND TaxType = 'Standard' AND ISNULL(FakturPajakNo, '') = ''
			    AND CONVERT(VARCHAR, InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
	    UNION
	    SELECT	CompanyCode, BranchCode,'000' AS ProfitCenter,FakturPajakNo FPJGovNo
			    ,NULL AS FPJGovDate,InvoiceNo as DocNo,convert(varchar,InvoiceDate,112) AS DocDate, CustomerCode
	    FROM	ArFakturPajakHdr
	    WHERE	CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
			        or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
		        )
                AND TaxType = 'Standard' AND ISNULL(FakturPajakNo, '') = ''
			    AND CONVERT(VARCHAR, InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
                AND DPPAMt > 0
    ) x 
    left join gnMstLookUpDtl a on a.CompanyCode= x.CompanyCode and a.CodeID='PFCN' and a.LookUpValue= x.ProfitCenter
    where x.ProfitCenter like ''+@ProfitCenter+''
) y";

                    if (isFPJGU)
                        sql2 = sql2 + " inner join gnMstLookupDtl x on x.CompanyCode = y.CompanyCode and x.CodeID = 'FPJG' and x.LookupValue = y.CustomerCodeBill";
                    else
                        sql2 = sql2 + " left join gnMstLookupDtl x on x.CompanyCode = y.CompanyCode and x.CodeID = 'FPJG' and x.LookupValue = y.CustomerCodeBill where x.LookupValue is null";

                    sql2 = sql2.Replace("@CompanyCode", string.Format("'{0}'", CompanyCode));
                    sql2 = sql2.Replace("@BranchCode", string.Format("{0}", branchCode));
                    sql2 = sql2.Replace("@StartDate", string.Format("'{0}'", startDate.ToString("yyyyMMdd")));
                    sql2 = sql2.Replace("@FPJDate", string.Format("'{0}'", fpjDate.ToString("yyyyMMdd")));
                    sql2 = sql2.Replace("@ProfitCenter", string.Format("'{0}'", profitCenter));
                    sql2 = sql2.Replace("@IsFPJCentral", string.Format("'{0}'", isFPJCentral));

                    var fpjList = ctx.Database.SqlQuery<GenTax>(sql2).ToList();

                    docNo = "|";
                    foreach (var item in fpjList)
                    {
                        docNo += item.BranchCode + " " + item.DocNo + "|";
                    }
                }

                dtPending = SelectPendingDocumentGab(startDate.Date, fpjDate, docNo, isFPJCentral, bCheckAll, isFPJGU);

                DataRow[] rowPending = dtPending.Select(string.Format("InvoiceDate < '{0}' ", fpjDate));
                if (rowPending.Length > 0)
                {
                    bProcess = false;             
                }
                if (errMsg != string.Empty) errMsg += "\r\n - Ada Document pending/ belum generate pajak, Lihat Daftar Dokumen Pending";
                else { errMsg += "Transaksi Tanggal : " + fpjDate.Date.ToString("dd-MMM-yyyy") + "\r\n\r\n - Ada Document pending/belum generate pajak, Lihat Daftar Dokumen Pending"; }
            }

            if (errMsg != string.Empty) return Json(new { success = false, message = errMsg, bPending = bPending, data = dtPending, bProcess = bProcess });
            else return Json(new { success = true, message = "", bProcess = bProcess });
        }

        public JsonResult GenerateTax(DateTime startDate, DateTime fpjDate, string docNo, string profitCenter, bool isFPJCentral, bool isFPJGU, bool bcheckAll)
        {
            long value = -1;
            bool result = false;
            string msgError = "";
            SqlConnection con = (SqlConnection)ctx.Database.Connection;
            DataTable dt = new DataTable();
            

            try
            {
                long lastSeqNo = 0;
                //if (isWebService)
                //    lastSeqNo = TaxHelper.GetLastTaxNo(user.CompanyCode, user.BranchCode, fpjDate.Year);

                if (lastSeqNo > -1)
                {                    
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandTimeout = 1000000;

                    if (bcheckAll)
                    {
                        string sql1 = string.Format(
@"SELECT LookUpValue , ParaValue 
FROM gnMstLookUpDtl 
WHERE CompanyCode= '{0}'
    AND CodeID = 'BRANCH' 
	AND ParaValue = 
	( 
		SELECT ParaValue FROM gnMstLookUpDtl 
		WHERE CodeID = 'BRANCH' 
		AND LookUpValue = '{1}'
	)
", CompanyCode, BranchCode);

                        DataTable dtBranch = MyHelpers.GetTable(ctx, sql1);
                        string branchCode = string.Empty;
                        if (dtBranch.Rows.Count > 0)
                        {
                            foreach (DataRow row in dtBranch.Rows)
                                branchCode += string.Format(",'{0}'", row["LookUpValue"]);
                            if (branchCode.Length > 0) branchCode = branchCode.Substring(1);
                        }
                        else
                            branchCode = "'" + BranchCode + "'";

                        string sql2 = @"select 
    row_number() over(order by y.BranchCode,y.DocDate,y.CustomerCodeBill, y.ProfitCenter asc) as No
    ,y.chkSelect,y.CompanyCode,y.BranchCode,y.ProfitCenter,isnull(y.FPJGovNo,'') FPJGovNo
    ,isnull(y.FPJGovDate,'')FPJGovDate,y.DocNo, convert(varchar, convert(datetime,y.DocDate), 106) DocDate, y.CustomerCodeBill
from(
    select 
        Convert(bit, 0) chkSelect, x.CompanyCode, x.BranchCode
        , a.LookUpValueName ProfitCenter, x.FPJGovNo, x.FPJGovDate
        , x.DocNo, x.DocDate, x.CustomerCodeBill
    from (
	    SELECT	CompanyCode, BranchCode,'300' AS ProfitCenter,FPJGovNo
			    ,NULL AS FPJGovDate,InvoiceNo as DocNo,convert(varchar,InvoiceDate,112) AS DocDate, CustomerCodeBill
	    FROM	SpTrnSFPJHdr
	    WHERE	CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
			        or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
		        )
                AND isPKP = 1 
                AND ISNULL(FPJGovNo, '') = ''
			    AND CONVERT(VARCHAR, InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
	    UNION
	    SELECT	CompanyCode, BranchCode,'200' AS ProfitCenter,FPJGovNo
			    ,NULL AS FPJGovDate,FPJNo as DocNo,convert(varchar,FPJDate,112) AS DocDate, CustomerCodeBill
	    FROM	SvTrnFakturPajak
	    WHERE	CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
			        or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
		        )
                AND isPKP = 1 AND ISNULL(FPJGovNo, '') = ''
			    AND CONVERT(VARCHAR, FPJDate, 112) BETWEEN @StartDate AND @FPJDate
	    UNION
	    SELECT	CompanyCode, BranchCode,'100' AS ProfitCenter,FakturPajakNo FPJGovNo
			    ,NULL AS FPJGovDate,InvoiceNo as DocNo,convert(varchar,InvoiceDate,112) AS DocDate, CustomerCode
	    FROM	OmFakturPajakHdr
	    WHERE	CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
			        or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
		        ) 
                AND TaxType = 'Standard' AND ISNULL(FakturPajakNo, '') = ''
			    AND CONVERT(VARCHAR, InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
	    UNION
	    SELECT	CompanyCode, BranchCode,'000' AS ProfitCenter,FakturPajakNo FPJGovNo
			    ,NULL AS FPJGovDate,InvoiceNo as DocNo,convert(varchar,InvoiceDate,112) AS DocDate, CustomerCode
	    FROM	ArFakturPajakHdr
	    WHERE	CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
			        or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
		        )
                AND TaxType = 'Standard' AND ISNULL(FakturPajakNo, '') = ''
			    AND CONVERT(VARCHAR, InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
                AND DPPAMt > 0
    ) x 
    left join gnMstLookUpDtl a on a.CompanyCode= x.CompanyCode and a.CodeID='PFCN' and a.LookUpValue= x.ProfitCenter
    where x.ProfitCenter like ''+@ProfitCenter+''
) y";

                        if (isFPJGU)
                            sql2 = sql2 + " inner join gnMstLookupDtl x on x.CompanyCode = y.CompanyCode and x.CodeID = 'FPJG' and x.LookupValue = y.CustomerCodeBill";
                        else
                            sql2 = sql2 + " left join gnMstLookupDtl x on x.CompanyCode = y.CompanyCode and x.CodeID = 'FPJG' and x.LookupValue = y.CustomerCodeBill where x.LookupValue is null";

                        sql2 = sql2.Replace("@CompanyCode", string.Format("'{0}'", CompanyCode));
                        sql2 = sql2.Replace("@BranchCode", string.Format("{0}", branchCode));
                        sql2 = sql2.Replace("@StartDate", string.Format("'{0}'", startDate.ToString("yyyyMMdd")));
                        sql2 = sql2.Replace("@FPJDate", string.Format("'{0}'", fpjDate.ToString("yyyyMMdd")));
                        sql2 = sql2.Replace("@ProfitCenter", string.Format("'{0}'", profitCenter));
                        sql2 = sql2.Replace("@IsFPJCentral", string.Format("'{0}'", isFPJCentral));

                        var fpjList = ctx.Database.SqlQuery<GenTax>(sql2).ToList();

                        docNo = "|";
                        foreach (var item in fpjList)
                        {
                            docNo += item.BranchCode + " " + item.DocNo + "|";
                        }
                    }

                    if (isFPJCentral)
                    {
                        if (isFPJGU)
                            cmd.CommandText = "usprpt_GnGenerateSeqTaxWoBranchUnion";
                        else
                            cmd.CommandText = "usprpt_GnGenerateSeqTaxWoBranch";
                    }
                    else
                    {
                        if (isFPJGU)
                            cmd.CommandText = "usprpt_GnGenerateSeqTaxUnion";
                        else
                            cmd.CommandText = "usprpt_GnGenerateSeqTax";
                    }
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 1000000;
                    cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                    cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                    cmd.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyyMMdd"));
                    cmd.Parameters.AddWithValue("@FPJDate", fpjDate.ToString("yyyyMMdd"));
                    cmd.Parameters.AddWithValue("@ProfitCenterCode", profitCenter);
                    cmd.Parameters.AddWithValue("@UserId", CurrentUser.UserId);
                    cmd.Parameters.AddWithValue("@DocNo", docNo);

                    con.Open();
                    object o = cmd.ExecuteScalar();
                    value = (o != null) ? Convert.ToInt64(o) : -1;
                    if (value >= -1)
                    {
                        result = true;
                        con.Close();
                    }

                    //string qSql = "";
                    
//                    if (profitCenter == "")
//                    {
//                        qSql = @"
//SELECT 
//    ( row_number ( ) OVER ( ORDER BY FPJGovNo ASC ) ) AS NO ,a.BranchCode,b.LookupValueName AS ProfitCenter , 
//    FPJGovNo , FPJGovDate , DocNo , DocDate 
//FROM GnGenerateTax a 
//LEFT JOIN GnMstLookupDtl b ON a.CompanyCode = b.CompanyCode 
//    AND b.CodeID = @CodeID 
//    AND b.LookupValue = a.ProfitCenterCode 
//WHERE a.CompanyCode = @CompanyCode
//    AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
//        or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
//    )
//    AND convert ( varchar , a.FPJGovDate , 112 ) BETWEEN @StartDate AND @EndDate
//";
//                        if (profitCenter != "") qSql += " AND ProfitCenterCode = @ProfitCenter";

//                        qSql = qSql.Replace("@CompanyCode", string.Format("'{0}'", CompanyCode));
//                        qSql = qSql.Replace("@BranchCode", string.Format("{0}", BranchCode));
//                        qSql = qSql.Replace("@CodeID", string.Format("'{0}'", GnMstLookUpHdr.ProfitCenter));
//                        qSql = qSql.Replace("@StartDate", string.Format("'{0}'", startDate.Date.ToString("yyyyMMdd")));
//                        qSql = qSql.Replace("@EndDate", string.Format("'{0}'", fpjDate.Date.ToString("yyyyMMdd")));
//                        qSql = qSql.Replace("@ProfitCenter", string.Format("'{0}'", profitCenter));
//                        qSql = qSql.Replace("@IsFPJCentral", string.Format("'{0}'", isFPJCentral));
//                    }
//                    else
//                    {
                      string qSql = @"
SELECT  (row_number () OVER (ORDER BY FPJGovNo ASC)) AS No,  Convert(bit, 0) chkSelect, CompanyCode, BranchCode, ProfitCenter, FPJGovNo, FPJGovDate, DocNo, DocDate, CustName, InvNo FROM(
SELECT a.CompanyCode, a.BranchCode, b.LookupValueName AS ProfitCenter, a.FPJGovNo, convert(varchar, convert(datetime,FPJGovDate), 106) FPJGovDate, DocNo, 
	convert(varchar, convert(datetime,DocDate), 106) DocDate, d.CustomerName CustName, c.FPJNo InvNo
FROM GnGenerateTax a 
LEFT JOIN GnMstLookupDtl b ON a.CompanyCode = b.CompanyCode 
	AND b.CodeID = 'PFCN'
	AND b.LookupValue = a.ProfitCenterCode 
INNER JOIN SpTrnSFPJHdr c on a.CompanyCode = c.CompanyCode 
	AND a.BranchCode = c.BranchCode
	AND a.FPJGovNo = c.FPJGovNo
	AND a.DocNo = c.InvoiceNo
LEFT JOIN gnMstCustomer d on a.CompanyCode = d.CompanyCode
	AND c.CustomerCodeBill = d.CustomerCode			
UNION
SELECT a.CompanyCode, a.BranchCode, b.LookupValueName AS ProfitCenter, a.FPJGovNo, convert(varchar, convert(datetime,FPJGovDate), 106) FPJGovDate, DocNo, 
	convert(varchar, convert(datetime,DocDate), 106) DocDate, d.CustomerName CustName, e.InvoiceNo InvNo
FROM GnGenerateTax a 
LEFT JOIN GnMstLookupDtl b ON a.CompanyCode = b.CompanyCode 
	AND b.CodeID = 'PFCN'
	AND b.LookupValue = a.ProfitCenterCode 
INNER JOIN SvTrnFakturPajak c on a.CompanyCode = c.CompanyCode 
	AND a.BranchCode = c.BranchCode
	AND a.FPJGovNo = c.FPJGovNo
	AND a.DocNo = c.FPJNo
LEFT JOIN gnMstCustomer d on a.CompanyCode = d.CompanyCode
	AND c.CustomerCodeBill = d.CustomerCode			
INNER JOIN svTrnInvoice e ON a.CompanyCode = e.CompanyCode AND
        a.BranchCode = e.BranchCode AND
        c.FPJNo = e.FPJNo	
UNION
SELECT a.CompanyCode, a.BranchCode, b.LookupValueName AS ProfitCenter, a.FPJGovNo, convert(varchar, convert(datetime,FPJGovDate), 106) FPJGovDate, DocNo, 
	convert(varchar, convert(datetime,DocDate), 106) DocDate, d.CustomerName CustName, e.InvoiceNo InvNo
FROM GnGenerateTax a 
LEFT JOIN GnMstLookupDtl b ON a.CompanyCode = b.CompanyCode 
	AND b.CodeID = 'PFCN'
	AND b.LookupValue = a.ProfitCenterCode 
INNER JOIN OmFakturPajakHdr c on a.CompanyCode = c.CompanyCode 
	AND a.BranchCode = c.BranchCode
	AND a.FPJGovNo = c.FakturPajakNo
	AND a.DocNo = c.InvoiceNo
LEFT JOIN gnMstCustomer d on a.CompanyCode = d.CompanyCode
	AND c.CustomerCode = d.CustomerCode			
INNER JOIN OmTrSalesInvoice e ON a.CompanyCode = e.CompanyCode AND
        a.BranchCode = e.BranchCode AND
        c.InvoiceNo = e.InvoiceNo
UNION
SELECT a.CompanyCode, a.BranchCode, b.LookupValueName AS ProfitCenter, a.FPJGovNo, convert(varchar, convert(datetime,FPJGovDate), 106) FPJGovDate, DocNo, 
	convert(varchar, convert(datetime,DocDate), 106) DocDate, d.CustomerName CustName, e.InvoiceNo InvNo
FROM GnGenerateTax a 
LEFT JOIN GnMstLookupDtl b ON a.CompanyCode = b.CompanyCode 
	AND b.CodeID = 'PFCN'
	AND b.LookupValue = a.ProfitCenterCode 
INNER JOIN ArFakturPajakHdr c on a.CompanyCode = c.CompanyCode 
	AND a.BranchCode = c.BranchCode
	AND a.FPJGovNo = c.FakturPajakNo
	AND a.DocNo = c.InvoiceNo
LEFT JOIN gnMstCustomer d on a.CompanyCode = d.CompanyCode
	AND c.CustomerCode = d.CustomerCode			
INNER JOIN arTrnInvoiceHdr e ON a.CompanyCode = e.CompanyCode	
			AND a.BranchCode = e.BranchCode
			AND e.InvoiceNo = c.InvoiceNo) #t1
WHERE CompanyCode = @CompanyCode
AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
    or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode))
AND convert(varchar, convert(datetime,FPJGovDate), 112) BETWEEN  @StartDate AND @EndDate
AND (case when @ProfitCenter = '' then BranchCode+' '+DocNo end)=BranchCode+' '+DocNo
	or (case when @ProfitCenter <> '' then @DocNo end) like '%|' + BranchCode+' '+DocNo + '|%'";

                        qSql = qSql.Replace("@CompanyCode", string.Format("'{0}'", CompanyCode));
                        qSql = qSql.Replace("@BranchCode", string.Format("{0}", BranchCode));
                        qSql = qSql.Replace("@CodeID", string.Format("'{0}'", GnMstLookUpHdr.ProfitCenter));
                        qSql = qSql.Replace("@StartDate", string.Format("'{0}'", startDate.Date.ToString("yyyyMMdd")));
                        qSql = qSql.Replace("@EndDate", string.Format("'{0}'", fpjDate.Date.ToString("yyyyMMdd")));
                        qSql = qSql.Replace("@ProfitCenter", string.Format("'{0}'", profitCenter));
                        qSql = qSql.Replace("@IsFPJCentral", string.Format("'{0}'", isFPJCentral));
                        qSql = qSql.Replace("@ProfitCenter", string.Format("'{0}'", profitCenter));
                        qSql = qSql.Replace("@DocNo", string.Format("'{0}'", docNo));
                    //}

                    dt = MyHelpers.GetTable(ctx, qSql);
                }
            }
            catch (Exception ex)
            {
                result = false;
                msgError = ex.Message;
                con.Close();
                    
            }           

            //if (isWebService)
            //    if (!TaxHelper.UnLockTaxNo(user.CompanyCode, user.BranchCode, fpjDate.Year, value)) XLogger.Log("Unlock tabel pajak gagal");
            return Json(new { success = result, message = msgError, data = dt });
        }

        public JsonResult GenerateTaxOnline(DateTime startDate, DateTime fpjDate, string docNo, string profitCenter, bool isFPJCentral, bool isFPJGU, bool bcheckAll)
        {
            bool result = true;
            string msgError = string.Empty;
            long value = -1;
            string companyCode = ctx.LookUpDtls.Where(p => p.CompanyCode == CompanyCode && p.CodeID == GnMstLookUpHdr.TaxOnline && p.LookUpValue == "COMPANYCODE").FirstOrDefault().ParaValue;
            string branchCode = ctx.LookUpDtls.Where(p => p.CompanyCode == CompanyCode && p.CodeID == GnMstLookUpHdr.TaxOnline && p.LookUpValue == "BRANCHCODE").FirstOrDefault().ParaValue;
            SqlConnection con = (SqlConnection)ctx.Database.Connection;
            SqlCommand cmd = con.CreateCommand();
            DataTable dt = new DataTable();

            try
            {
                if (bcheckAll)
                {
                    string sql1 = string.Format(
@"SELECT LookUpValue , ParaValue 
FROM gnMstLookUpDtl 
WHERE CompanyCode= '{0}'
    AND CodeID = 'BRANCH' 
	AND ParaValue = 
	( 
		SELECT ParaValue FROM gnMstLookUpDtl 
		WHERE CodeID = 'BRANCH' 
		AND LookUpValue = '{1}'
	)
", CompanyCode, BranchCode);

                    DataTable dtBranch = MyHelpers.GetTable(ctx, sql1);
                    string branchCode1 = string.Empty;
                    if (dtBranch.Rows.Count > 0)
                    {
                        foreach (DataRow row in dtBranch.Rows)
                            branchCode1 += string.Format(",'{0}'", row["LookUpValue"]);
                        if (branchCode1.Length > 0) branchCode1 = branchCode1.Substring(1);
                    }
                    else
                        branchCode1 = "'" + BranchCode + "'";

                    string sql2 = @"select 
    row_number() over(order by y.BranchCode,y.DocDate,y.CustomerCodeBill, y.ProfitCenter asc) as No
    ,y.chkSelect,y.CompanyCode,y.BranchCode,y.ProfitCenter,isnull(y.FPJGovNo,'') FPJGovNo
    ,isnull(y.FPJGovDate,'')FPJGovDate,y.DocNo, convert(varchar, convert(datetime,y.DocDate), 106) DocDate, y.CustomerCodeBill
from(
    select 
        Convert(bit, 0) chkSelect, x.CompanyCode, x.BranchCode
        , a.LookUpValueName ProfitCenter, x.FPJGovNo, x.FPJGovDate
        , x.DocNo, x.DocDate, x.CustomerCodeBill
    from (
	    SELECT	CompanyCode, BranchCode,'300' AS ProfitCenter,FPJGovNo
			    ,NULL AS FPJGovDate,InvoiceNo as DocNo,convert(varchar,InvoiceDate,112) AS DocDate, CustomerCodeBill
	    FROM	SpTrnSFPJHdr
	    WHERE	CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
			        or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
		        )
                AND isPKP = 1 
                AND ISNULL(FPJGovNo, '') = ''
			    AND CONVERT(VARCHAR, InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
	    UNION
	    SELECT	CompanyCode, BranchCode,'200' AS ProfitCenter,FPJGovNo
			    ,NULL AS FPJGovDate,FPJNo as DocNo,convert(varchar,FPJDate,112) AS DocDate, CustomerCodeBill
	    FROM	SvTrnFakturPajak
	    WHERE	CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
			        or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
		        )
                AND isPKP = 1 AND ISNULL(FPJGovNo, '') = ''
			    AND CONVERT(VARCHAR, FPJDate, 112) BETWEEN @StartDate AND @FPJDate
	    UNION
	    SELECT	CompanyCode, BranchCode,'100' AS ProfitCenter,FakturPajakNo FPJGovNo
			    ,NULL AS FPJGovDate,InvoiceNo as DocNo,convert(varchar,InvoiceDate,112) AS DocDate, CustomerCode
	    FROM	OmFakturPajakHdr
	    WHERE	CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
			        or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
		        ) 
                AND TaxType = 'Standard' AND ISNULL(FakturPajakNo, '') = ''
			    AND CONVERT(VARCHAR, InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
	    UNION
	    SELECT	CompanyCode, BranchCode,'000' AS ProfitCenter,FakturPajakNo FPJGovNo
			    ,NULL AS FPJGovDate,InvoiceNo as DocNo,convert(varchar,InvoiceDate,112) AS DocDate, CustomerCode
	    FROM	ArFakturPajakHdr
	    WHERE	CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
			        or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
		        )
                AND TaxType = 'Standard' AND ISNULL(FakturPajakNo, '') = ''
			    AND CONVERT(VARCHAR, InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
                AND DPPAMt > 0
    ) x 
    left join gnMstLookUpDtl a on a.CompanyCode= x.CompanyCode and a.CodeID='PFCN' and a.LookUpValue= x.ProfitCenter
    where x.ProfitCenter like ''+@ProfitCenter+''
) y";

                    if (isFPJGU)
                        sql2 = sql2 + " inner join gnMstLookupDtl x on x.CompanyCode = y.CompanyCode and x.CodeID = 'FPJG' and x.LookupValue = y.CustomerCodeBill";
                    else
                        sql2 = sql2 + " left join gnMstLookupDtl x on x.CompanyCode = y.CompanyCode and x.CodeID = 'FPJG' and x.LookupValue = y.CustomerCodeBill where x.LookupValue is null";

                    sql2 = sql2.Replace("@CompanyCode", string.Format("'{0}'", CompanyCode));
                    sql2 = sql2.Replace("@BranchCode", string.Format("{0}", branchCode));
                    sql2 = sql2.Replace("@StartDate", string.Format("'{0}'", startDate.ToString("yyyyMMdd")));
                    sql2 = sql2.Replace("@FPJDate", string.Format("'{0}'", fpjDate.ToString("yyyyMMdd")));
                    sql2 = sql2.Replace("@ProfitCenter", string.Format("'{0}'", profitCenter));
                    sql2 = sql2.Replace("@IsFPJCentral", string.Format("'{0}'", isFPJCentral));

                    var fpjList = ctx.Database.SqlQuery<GenTax>(sql2).ToList();

                    docNo = "|";
                    foreach (var item in fpjList)
                    {
                        docNo += item.BranchCode + " " + item.DocNo + "|";
                    }
                }

                long lastSeqNo = GetLastTaxNo(companyCode, branchCode, fpjDate.Year, cmd, con);
                if (lastSeqNo > -1)
                {
                    // TaxCabCode could be hard code like this, i dont know what the reason (Yo, 8 Mei 2014)
                    string taxCabCode = "000";

                    cmd.CommandText = "usprpt_GnGenerateSeqTaxOnline";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 120;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                    cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                    cmd.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyyMMdd"));
                    cmd.Parameters.AddWithValue("@FPJDate", fpjDate.ToString("yyyyMMdd"));
                    cmd.Parameters.AddWithValue("@ProfitCenterCode", profitCenter);
                    cmd.Parameters.AddWithValue("@UserId", CurrentUser.UserId);
                    cmd.Parameters.AddWithValue("@DocNo", docNo);
                    cmd.Parameters.AddWithValue("@LastSeqNo", lastSeqNo);
                    cmd.Parameters.AddWithValue("@TaxCabCode", taxCabCode);

                    con.Open();
                    object o = cmd.ExecuteScalar();
                    if (o == DBNull.Value)
                    {
                        return Json(new { success = false, message = "Profit center user anda berubah, please cek" });
                    }
                    value = (o != null) ? Convert.ToInt64(o) : -1;                    
                    if (value >= -1)
                        result = UnLockTaxNo(companyCode, branchCode, fpjDate.Year, value, cmd, con);
                }
                else
                    UnLockTaxNo(companyCode, branchCode, fpjDate.Year, value, cmd, con);

                string qSql = @"
SELECT  (row_number () OVER (ORDER BY FPJGovNo ASC)) AS No,  Convert(bit, 0) chkSelect, CompanyCode, BranchCode, ProfitCenter, FPJGovNo, FPJGovDate, DocNo, DocDate, CustName, InvNo FROM(
SELECT a.CompanyCode, a.BranchCode, b.LookupValueName AS ProfitCenter, a.FPJGovNo, convert(varchar, convert(datetime,FPJGovDate), 106) FPJGovDate, DocNo, 
	convert(varchar, convert(datetime,DocDate), 106) DocDate, d.CustomerName CustName, c.FPJNo InvNo
FROM GnGenerateTax a 
LEFT JOIN GnMstLookupDtl b ON a.CompanyCode = b.CompanyCode 
	AND b.CodeID = 'PFCN'
	AND b.LookupValue = a.ProfitCenterCode 
INNER JOIN SpTrnSFPJHdr c on a.CompanyCode = c.CompanyCode 
	AND a.BranchCode = c.BranchCode
	AND a.FPJGovNo = c.FPJGovNo
	AND a.DocNo = c.InvoiceNo
LEFT JOIN gnMstCustomer d on a.CompanyCode = d.CompanyCode
	AND c.CustomerCodeBill = d.CustomerCode			
UNION
SELECT a.CompanyCode, a.BranchCode, b.LookupValueName AS ProfitCenter, a.FPJGovNo, convert(varchar, convert(datetime,FPJGovDate), 106) FPJGovDate, DocNo, 
	convert(varchar, convert(datetime,DocDate), 106) DocDate, d.CustomerName CustName, e.InvoiceNo InvNo
FROM GnGenerateTax a 
LEFT JOIN GnMstLookupDtl b ON a.CompanyCode = b.CompanyCode 
	AND b.CodeID = 'PFCN'
	AND b.LookupValue = a.ProfitCenterCode 
INNER JOIN SvTrnFakturPajak c on a.CompanyCode = c.CompanyCode 
	AND a.BranchCode = c.BranchCode
	AND a.FPJGovNo = c.FPJGovNo
	AND a.DocNo = c.FPJNo
LEFT JOIN gnMstCustomer d on a.CompanyCode = d.CompanyCode
	AND c.CustomerCodeBill = d.CustomerCode			
INNER JOIN svTrnInvoice e ON a.CompanyCode = e.CompanyCode AND
        a.BranchCode = e.BranchCode AND
        c.FPJNo = e.FPJNo	
UNION
SELECT a.CompanyCode, a.BranchCode, b.LookupValueName AS ProfitCenter, a.FPJGovNo, convert(varchar, convert(datetime,FPJGovDate), 106) FPJGovDate, DocNo, 
	convert(varchar, convert(datetime,DocDate), 106) DocDate, d.CustomerName CustName, e.InvoiceNo InvNo
FROM GnGenerateTax a 
LEFT JOIN GnMstLookupDtl b ON a.CompanyCode = b.CompanyCode 
	AND b.CodeID = 'PFCN'
	AND b.LookupValue = a.ProfitCenterCode 
INNER JOIN OmFakturPajakHdr c on a.CompanyCode = c.CompanyCode 
	AND a.BranchCode = c.BranchCode
	AND a.FPJGovNo = c.FakturPajakNo
	AND a.DocNo = c.InvoiceNo
LEFT JOIN gnMstCustomer d on a.CompanyCode = d.CompanyCode
	AND c.CustomerCode = d.CustomerCode			
INNER JOIN OmTrSalesInvoice e ON a.CompanyCode = e.CompanyCode AND
        a.BranchCode = e.BranchCode AND
        c.InvoiceNo = e.InvoiceNo
UNION
SELECT a.CompanyCode, a.BranchCode, b.LookupValueName AS ProfitCenter, a.FPJGovNo, convert(varchar, convert(datetime,FPJGovDate), 106) FPJGovDate, DocNo, 
	convert(varchar, convert(datetime,DocDate), 106) DocDate, d.CustomerName CustName, e.InvoiceNo InvNo
FROM GnGenerateTax a 
LEFT JOIN GnMstLookupDtl b ON a.CompanyCode = b.CompanyCode 
	AND b.CodeID = 'PFCN'
	AND b.LookupValue = a.ProfitCenterCode 
INNER JOIN ArFakturPajakHdr c on a.CompanyCode = c.CompanyCode 
	AND a.BranchCode = c.BranchCode
	AND a.FPJGovNo = c.FakturPajakNo
	AND a.DocNo = c.InvoiceNo
LEFT JOIN gnMstCustomer d on a.CompanyCode = d.CompanyCode
	AND c.CustomerCode = d.CustomerCode			
INNER JOIN arTrnInvoiceHdr e ON a.CompanyCode = e.CompanyCode	
			AND a.BranchCode = e.BranchCode
			AND e.InvoiceNo = c.InvoiceNo) #t1
WHERE CompanyCode = @CompanyCode
AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
    or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode))
AND convert (varchar, FPJGovDate , 112 ) BETWEEN @StartDate AND @EndDate
AND (case when @ProfitCenter = '' then BranchCode+' '+DocNo end)=BranchCode+' '+DocNo
	or (case when @ProfitCenter <> '' then @DocNo end) like '%|' + BranchCode+' '+DocNo + '|%'";

                qSql = qSql.Replace("@CompanyCode", string.Format("'{0}'", CompanyCode));
                qSql = qSql.Replace("@BranchCode", string.Format("{0}", BranchCode));
                qSql = qSql.Replace("@CodeID", string.Format("'{0}'", GnMstLookUpHdr.ProfitCenter));
                qSql = qSql.Replace("@StartDate", string.Format("'{0}'", startDate.Date.ToString("yyyyMMdd")));
                qSql = qSql.Replace("@EndDate", string.Format("'{0}'", fpjDate.Date.ToString("yyyyMMdd")));
                qSql = qSql.Replace("@ProfitCenter", string.Format("'{0}'", profitCenter));
                qSql = qSql.Replace("@IsFPJCentral", string.Format("'{0}'", isFPJCentral));
                qSql = qSql.Replace("@ProfitCenter", string.Format("'{0}'", profitCenter));
                qSql = qSql.Replace("@DocNo", string.Format("'{0}'", docNo));

                dt = MyHelpers.GetTable(ctx, qSql);
            }
            catch (Exception ex)
            {
                UnLockTaxNo(companyCode, branchCode, fpjDate.Year, value, cmd, con);
                result = false;
                msgError = ex.Message;
                con.Close();
            }

            return Json(new { success = result, message = msgError, data = dt });
        }

        private long GetLastTaxNo(string companyCode, string branchCode, int Year, SqlCommand cmd, SqlConnection conn)
        {
            long result = -1;

            cmd.CommandText = "uspfn_GnLockTaxNo";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", companyCode);
            cmd.Parameters.AddWithValue("@BranchCode", branchCode);
            cmd.Parameters.AddWithValue("@Year", Year);
            cmd.Parameters.AddWithValue("@IPNo", ((SqlConnection)(cmd.Connection)).WorkstationId);
            cmd.Parameters.AddWithValue("@UserID", CurrentUser.UserId);

            conn.Open();
            object o = cmd.ExecuteScalar();
            result = (o != null) ? Convert.ToInt64(o) : -1;
            conn.Close();

            return result;
        }

        private bool UnLockTaxNo(string companyCode, string branchCode, int Year, long SeqNoNew, SqlCommand cmd, SqlConnection con)
        {
            bool result = true;

            cmd.CommandText = "uspfn_GnUnLockTaxNo";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", companyCode);
            cmd.Parameters.AddWithValue("@BranchCode", branchCode);
            cmd.Parameters.AddWithValue("@Year", Year);
            cmd.Parameters.AddWithValue("@FPJSeqNoNew", SeqNoNew);
            cmd.Parameters.AddWithValue("@UserID", CurrentUser.UserId);
            
            result = cmd.ExecuteNonQuery() > 0;
           
            return result;
        }
    }
}
