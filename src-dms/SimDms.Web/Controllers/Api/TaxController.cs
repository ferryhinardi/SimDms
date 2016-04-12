using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Breeze.WebApi;
using Newtonsoft.Json.Linq;
using Breeze.WebApi.EF;
using SimDms.Tax.Controllers;
using SimDms.Tax.Models;
using SimDms.Tax;
using SimDms.Common;
using SimDms.Common.Models;

namespace SimDms.Web.Controllers.Api
{
    [BreezeController]
    public class TaxController : ApiController
    {
        readonly EFContextProvider<DataContext> ctx = new EFContextProvider<DataContext>();

        [HttpGet]
        public String Metadata()
        {
            return ctx.Metadata();
        }

        [HttpGet]
        public GnMstSignature SignName(string ProfitCentre) 
        {
            var documentType = "";
            var myPro  ="";
            if (ProfitCentre == "" || ProfitCentre == null)
                myPro = ctx.Context.SysUserProfitCenters.Find(User.Identity.Name).ProfitCenter;
            else 
                myPro = ProfitCentre;

            var user = ctx.Context.SysUsers.Find(User.Identity.Name); 
            if (myPro == "300")
            {
                documentType = "FPJ";
            }
            else if (myPro == "200")
            {
                documentType = "FPS";
            }
            else if (myPro == "000")
            {
                documentType = "PJK";
            }
            else if (myPro == "100") 
            {
                documentType = "PJU";
            }
            var signName = ctx.Context.gnMstSignatures.Where( p => p.CompanyCode == user.CompanyCode 
                && p.BranchCode == user.BranchCode 
                && p.DocumentType == documentType).
                OrderBy(p => p.SeqNo).FirstOrDefault();
            return signName;
        }

        [HttpGet]
        public TxFpjConfig FPJSales(string FPJID)
        {
           // var myPro = "";
            var myPro = ctx.Context.TxFpjConfigs.Find(FPJID);
            return myPro;
        }

        [HttpGet]
        public SysUserProfitCenter ProfitCenter()
        {
            var myPro = ctx.Context.SysUserProfitCenters.Find(User.Identity.Name);
            return myPro;
        }

        [HttpGet]
        public SysUser CurrentUser()
        {
            var user = ctx.Context.SysUsers.Find(User.Identity.Name);
            user.CoProfile = ctx.Context.CoProfiles.Find(user.CompanyCode, user.BranchCode);
            return user;
        }

        [HttpGet]
        public MyUserInfo CurrentUserInfo()
        {
            var u = ctx.Context.SysUsers.Find(User.Identity.Name);
            var g = ctx.Context.CoProfiles.Find(u.CompanyCode, u.BranchCode);
            u.CoProfile = ctx.Context.CoProfiles.Find(u.CompanyCode, u.BranchCode);
            var f = ctx.Context.SysUserProfitCenters.Find(User.Identity.Name);

            var info = new MyUserInfo
            {
                UserId = u.UserId,
                FullName = u.FullName,
                CompanyCode = u.CompanyCode,
                BranchCode = u.BranchCode,
                CompanyGovName = g.CompanyGovName, // company // dealer
                CompanyName = g.CompanyName, //branch/ outlet
                TypeOfGoods = u.TypeOfGoods,
                ProductType = g.ProductType,
                IsActive = u.IsActive,
                RequiredChange = u.RequiredChange,
                ProfitCenter = f.ProfitCenter,
                SimDmsVersion = GetType().Assembly.GetName().Version.ToString()
            };

            return info;
        }

        [HttpGet]
        public IQueryable<gnTaxInView> Supplier4Tax(string Year, string Month, string UnitUsaha)
        {
            var Uid = CurrentUser();
            var ProductType = ctx.Context.CoProfiles.Where(p => p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode).FirstOrDefault().ProductType;
            var query = string.Format(@"
                select 
	                    distinct(SupplierCode)
	                    , SupplierName
                    from 
	                    gnTaxIn
                    where
	                    CompanyCode= '{0}'
	                    and BranchCode= '{1}'
	                    and ProductType= '{2}'
	                    and PeriodYear= '{3}'
	                    and PeriodMonth= '{4}'
                        and ProfitCenter like '{5}'
                       ", Uid.CompanyCode, Uid.BranchCode, ProductType, Year, Month, UnitUsaha);

            var queryable = ctx.Context.Database.SqlQuery<gnTaxInView>(query).AsQueryable();
            return (queryable);
        }

        #region ConsolidationTax

        [HttpGet]
        public IQueryable<TaxBrowse> TaxInBrowse()
        {
            var Uid = CurrentUser();
            var data = ctx.Context.gnTaxIn.Where(a => a.CompanyCode == Uid.CompanyCode && a.BranchCode == Uid.BranchCode && a.ProductType == Uid.CoProfile.ProductType)
                .Select(a => new TaxBrowse
                {
                    PeriodMonth = a.PeriodMonth,
                    PeriodYear = a.PeriodYear
                })
                    .Distinct()
                    .OrderByDescending(a => new { a.PeriodYear, a.PeriodMonth });

            return (data);
        }

        [HttpGet]
        public IQueryable<TaxBrowse> TaxOutBrowse()
        {
            var Uid = CurrentUser();
            var data = ctx.Context.GnTaxOuts.Where(a => a.CompanyCode == Uid.CompanyCode && a.BranchCode == Uid.BranchCode && a.ProductType == Uid.CoProfile.ProductType)
                .Select(a => new TaxBrowse
                {
                    PeriodMonth = a.PeriodMonth,
                    PeriodYear = a.PeriodYear
                })
                    .Distinct()
                    .OrderByDescending(a => new { a.PeriodYear, a.PeriodMonth });

            return (data);
        }

        [HttpGet]
        public IQueryable<CompanyBrowse> CompanyBrowse()
        {
            var Uid = CurrentUser();
            var data = ctx.Context.OrganizationHdrs.Select(a => new CompanyBrowse { CompanyCode = a.CompanyCode, CompanyName = a.CompanyName });
            return (data);
        }

        [HttpGet]
        public IQueryable<BranchBrowse> BranchBrowse()
        {
            var Uid = CurrentUser();
            var data = ctx.Context.OrganizationDtls.Where(a => a.CompanyCode == Uid.CompanyCode).Select(a => new BranchBrowse { BranchCode = a.BranchCode, BranchName = a.BranchName });
            return (data);
        }

        [HttpGet]
        public IQueryable<ConsolidationLookup> ConsolidationLookup(string type)
        {
            var codeId = "";
            switch (type)
            {
                case "typeofgoods":
                    codeId = GnMstLookUpHdr.TypeOfGoods;
                    break;
                case "taxin":
                    codeId = GnMstLookUpHdr.TaxCodeIN;
                    break;
                case "taxout":
                    codeId = GnMstLookUpHdr.TaxCodeOUT;
                    break;
                case "transin":
                    codeId = GnMstLookUpHdr.TaxTransactionCodeIN;
                    break;
                case "transout":
                    codeId = GnMstLookUpHdr.TaxTransactionCodeOUT;
                    break;
                case "statusin":
                    codeId = GnMstLookUpHdr.TaxStatusCodeIN;
                    break;
                case "statusout":
                    codeId = GnMstLookUpHdr.TaxStatusCodeOUT;
                    break;
                case "document":
                    codeId = GnMstLookUpHdr.TaxDocumentCode;
                    break;
                case "doctype":
                    codeId = GnMstLookUpHdr.TaxDocumentType;
                    break;
            }

            var Uid = CurrentUser();
            var data = ctx.Context.LookUpDtls.Where(a => a.CompanyCode == Uid.CompanyCode && a.CodeID.Equals(codeId)).OrderBy(a => a.SeqNo).Select(a => new ConsolidationLookup { LookUpValue = a.LookUpValue, LookUpValueName = a.LookUpValueName });
            return data;
        }

        [HttpGet]
        public IQueryable<SupplierBrowse> SupplierBrowse(string ProfitCenter)
        {
            var Uid = CurrentUser();
            var Uif = CurrentUserInfo();
            var profitCenterName = ctx.Context.LookUpDtls.Find(Uid.CompanyCode, "PFCN", Uif.ProfitCenter).LookUpValueName;

            var data = from a in ctx.Context.Suppliers
                       join d in ctx.Context.SupplierProfitCenters
                       on new { a.CompanyCode, a.SupplierCode }
                       equals new { d.CompanyCode, d.SupplierCode }
                       where a.CompanyCode == Uid.CompanyCode
                       && d.BranchCode == Uid.BranchCode
                       && d.ProfitCenterCode == (string.IsNullOrEmpty(ProfitCenter) == true ?  Uif.ProfitCenter : ProfitCenter)
                       && d.isBlackList == false
                       && a.Status == "1"
                       select new SupplierBrowse
                       {
                           SupplierCode = a.SupplierCode,
                           SupplierGovName = a.SupplierGovName,
                           Address = (a.Address1 + " " + a.Address2 + " " + a.Address3 + " " + a.Address4),
                           Discount = d.DiscPct,
                           Status = a.Status == "0" ? "Tidak Aktif" : "Aktif",
                           ProfitCenter = profitCenterName,
                           NPWPNo = a.NPWPNo,
                           IsPKP = a.isPKP
                       };

            return data;
        }

        [HttpGet]
        public IQueryable<CustomerBrowse> CustomerBrowse()
        {

            var Uid = CurrentUser();
            var Uif = CurrentUserInfo();

            var query = string.Format(@"
                        SELECT a.CustomerCode, a.CustomerGovName,
                   a.Address1+' '+a.Address2+' '+a.Address3+' '+a.Address4 as Address,
                   c.LookUpValueName as ProfitCenter, a.NPWPNo, a.IsPKP
              FROM gnMstCustomer a with(nolock, nowait)
                INNER JOIN gnMstCustomerProfitCenter b with(nolock, nowait) ON 
                    b.CustomerCode= b.CustomerCode AND b.CustomerCode=a.CustomerCode
                INNER JOIN gnMstLookUpDtl c ON c.CompanyCode= a.CompanyCode
             WHERE  a.CompanyCode={0}
                AND b.BranchCode={1}
                AND b.ProfitCenterCode= '{2}'
                AND b.isBlackList=0
                AND a.status = 1
                AND c.LookupValue= b.ProfitCenterCode" +
                " AND c.CodeID= '" + GnMstLookUpHdr.ProfitCenter + "'" +
                " ORDER BY a.CustomerCode", Uid.CompanyCode, Uid.BranchCode, Uif.ProfitCenter);

            var data = ctx.Context.Database.SqlQuery<CustomerBrowse>(query).AsQueryable();
            return data;
        }
    
        #endregion
        
        #region Perbaikan Faktur Pajak

        [HttpGet]
        public IQueryable<svTrnFakturPajakView> svFakturPajak4Tax(string Branch)
        {
            var Uid = CurrentUser();

            if (Branch != null)
            {
            var query = string.Format(@"
                SELECT *, FPJNo, (SELECT c.CompanyName from gnMstCoProfile c where c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode) as BranchName
		             , FPJDate
		             , CustomerName as CustomerNameBill
	              FROM svTrnFakturPajak a 
	             WHERE CompanyCode    = '{0}'
	               AND BranchCode     = '{1}' 
	               AND LEFT(FPJNo, 3) = 'FPS'
                       ", Uid.CompanyCode, Branch);

            var queryable = ctx.Context.Database.SqlQuery<svTrnFakturPajakView>(query).AsQueryable();
            return (queryable);
            }
            else
            {
                var query = string.Format(@"
                SELECT *, FPJNo, (SELECT c.CompanyName from gnMstCoProfile c where c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode) as BranchName
		             , FPJDate
		             , CustomerName as CustomerNameBill
	              FROM svTrnFakturPajak a 
	             WHERE CompanyCode    = '{0}'
	               AND LEFT(FPJNo, 3) = 'FPS'
                       ", Uid.CompanyCode);

                var queryable = ctx.Context.Database.SqlQuery<svTrnFakturPajakView>(query).AsQueryable();
                return (queryable);
            }
        }

        [HttpGet]
        public IQueryable<spTrnSFPJHdrView> spFakturPajak4Tax(string Branch)
        {
            var Uid = CurrentUser();

            if (Branch != null)
            {
                var query = string.Format(@"
                SELECT a.BranchCode, (SELECT c.CompanyName from gnMstCoProfile c where c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode) as BranchName
                     , a.FPJGovNo, a.FPJNo , a.FPJDate , a.PickingSlipNo , a.PickingSlipDate , a.InvoiceNo , a.InvoiceDate, a.CustomerCode as CustomerCodeBill
                     , b.CustomerName as CustomerNameBill
                     , b.Address1, b.Address2, b.Address3, b.Address4, b.NPWPNo, b.NPWPDate, b.SKPNo, b.SKPDate
	              FROM spTrnSFPJHdr a WITH(NOLOCK, NOWAIT)
                  INNER JOIN SpTrnSFPJInfo b
                    ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.FPJNo = b.FPJNo
	             WHERE a.CompanyCode = '{0}'
	               AND a.BranchCode  = '{1}'
	               AND a.IsPKP = 1
                       ", Uid.CompanyCode, Branch);

                var queryable = ctx.Context.Database.SqlQuery<spTrnSFPJHdrView>(query).AsQueryable();
                return (queryable);
            }
            else
            {
                var query = string.Format(@"
                 SELECT a.BranchCode, (SELECT c.CompanyName from gnMstCoProfile c where c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode) as BranchName
                     , a.FPJGovNo, a.FPJNo , a.FPJDate , a.PickingSlipNo , a.PickingSlipDate , a.InvoiceNo , a.InvoiceDate, a.CustomerCode as CustomerCodeBill
                     , b.CustomerName as CustomerNameBill
                     , b.Address1, b.Address2, b.Address3, b.Address4, b.NPWPNo, b.NPWPDate, b.SKPNo, b.SKPDate
	              FROM spTrnSFPJHdr a WITH(NOLOCK, NOWAIT)
                  INNER JOIN SpTrnSFPJInfo b
                    ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.FPJNo = b.FPJNo
	             WHERE a.CompanyCode = '{0}'
	               AND a.IsPKP = 1
                       ", Uid.CompanyCode);

                var queryable = ctx.Context.Database.SqlQuery<spTrnSFPJHdrView>(query).AsQueryable();
                return (queryable);
            }
        }

        [HttpGet]
        public IQueryable<MstCustomerView> CustomerCodeBill(string Branch, string OptionsTrans, string CustomerCode, string FPJNo)
        {
            var Uid = CurrentUser();
            if (OptionsTrans == "Service")
            {
                var data = ctx.Context.GnMstCustomer.Where(a => a.CompanyCode == Uid.CompanyCode && a.CustomerCode == CustomerCode)
                    .Select(a => new MstCustomerView
                    {
                        CustomerCode = a.CustomerCode,
                        CustomerName = a.CustomerName,
                    });
                return (data);
            }
            else
            {
                var query = string.Format(@"
                SELECT
	                a.CustomerCode,a.CustomerCodeBill,b.CustomerName,b.CustomerGovName
                FROM
	                spTrnSFPJHdr a WITH(NOLOCK, NOWAIT)
                    left join gnMstCustomer b on a.CompanyCode=b.CompanyCode and a.CustomerCode=b.CustomerCode
                WHERE
	                a.CompanyCode = '{0}'
	                AND a.BranchCode = '{1}'
	                AND a.FPJNo = '{2}'
                       ", Uid.CompanyCode, Branch, FPJNo);

                var queryable = ctx.Context.Database.SqlQuery<MstCustomerView>(query).AsQueryable();
                return (queryable);
            }
        }

        #endregion

        [HttpGet]
        public IQueryable<GenerateTaxView> FakturNo(string profitCenterCode, string stardate, string enddate)
        {
            var Uid = CurrentUser();
            var sqlstr = "";
            if (profitCenterCode == "000")
            {
                sqlstr = @"
                        SELECT 
                            a.ProfitCenterCode,a.FPJGovNo,a.FPJGovDate,a.DocNo,a.DocDate,b.DueDate,c.CustomerName
	                        ,isnull(c.Address1,'')+' '+isnull(c.Address2,'')+' '+isnull(c.Address3,'')+' '+isnull(c.Address4,'') Address
	                        ,b.DPPAmt,b.PPNAmt,(b.DPPAmt+b.PPNAmt) Total
                        FROM 
                            GnGenerateTax a 
                            inner join arFakturPajakHdr b on
                                a.CompanyCode= b.CompanyCode and a.BranchCode= b.BranchCode and a.DocNo= b.InvoiceNo
                            left join gnMstCustomer c on
                                a.CompanyCode= c.CompanyCode and b.CustomerCode= c.CustomerCode
                        WHERE 
                            a.CompanyCode = '{0}' AND a.BranchCode = '{1}' 
                            AND a.ProfitCenterCode = '{2}'
                            AND CONVERT(VARCHAR, a.FPJGovDate, 112) BETWEEN '{3}' AND '{4}'
                                        ";
            }
            else if (profitCenterCode == "100")
            {
                sqlstr = @"
                SELECT 
                    a.ProfitCenterCode,a.FPJGovNo,a.FPJGovDate,a.DocNo,a.DocDate,b.DueDate,c.CustomerName
	                ,isnull(c.Address1,'')+' '+isnull(c.Address2,'')+' '+isnull(c.Address3,'')+' '+isnull(c.Address4,'') Address
	                ,b.DPPAmt,b.PPNAmt,(b.DPPAmt+b.PPNAmt) Total
                FROM 
                    GnGenerateTax a 
                    inner join omFakturPajakHdr b on
                        a.CompanyCode= b.CompanyCode and a.BranchCode= b.BranchCode and a.DocNo= b.InvoiceNo
                    left join gnMstCustomer c on
                        a.CompanyCode= c.CompanyCode and b.CustomerCode= c.CustomerCode
                WHERE 
                    a.CompanyCode = '{0}' AND a.BranchCode = '{1}' 
                    AND a.ProfitCenterCode = '{2}'
                    AND CONVERT(VARCHAR, a.FPJGovDate, 112) BETWEEN '{3}' AND '{4}'
                                ";
            }
            else if (profitCenterCode == "200")
            {
                sqlstr = @"
                SELECT 
                    a.ProfitCenterCode,a.FPJGovNo,a.FPJGovDate,a.DocNo,a.DocDate,b.DueDate,b.CustomerName
	                ,isnull(b.Address1,'')+' '+isnull(b.Address2,'')+' '+isnull(b.Address3,'')+' '+isnull(b.Address4,'') Address
	                ,c.TotalDppAmt DPPAmt,c.TotalPpnAmt PPNAmt,(c.TotalDppAmt+c.TotalPpnAmt) Total 
                FROM 
                    GnGenerateTax a 
                    inner join svTrnFakturPajak b on
                        a.CompanyCode= b.CompanyCode and a.BranchCode= b.BranchCode and a.DocNo= b.FPJNo
                    inner join svTrnInvoice c on
                        a.CompanyCode= c.CompanyCode and a.BranchCode= c.BranchCode and b.FPJNo= c.FPJNo	
                WHERE 
                    a.CompanyCode = '{0}' AND a.BranchCode = '{1}' 
                    AND a.ProfitCenterCode = '{2}'
                    AND CONVERT(VARCHAR, a.FPJGovDate, 112) BETWEEN '{3}' AND '{4}'
                    AND b.isPKP = 1 AND b.NoOfInvoice = 1
                order by a.FpjGovdate, right(a.FpjGovNo,len(a.FpjGovNo)-8)
                                ";
            }
            else
            {
                sqlstr = @"
                SELECT 
                    a.CompanyCode,a.BranchCode,a.ProfitCenterCode,a.FPJGovNo,a.FPJGovDate,a.DocNo,a.DocDate,b.DueDate
	                ,b.CustomerName,b.Address,b.DPPAmt,b.PPNAmt,b.Total, b.FpjNo, b.FPJDate
                FROM 
                    GnGenerateTax a 
                    inner join (
		                select 
			                a.CompanyCode,a.BranchCode,a.FpjNo, a.FpjDate,a.InvoiceNo,a.DueDate,a.TotDPPAmt DPPAmt,a.TotPPNAmt PPNAmt,(a.TotDPPAmt+a.TotPPNAmt) Total 
			                ,b.CustomerName,isnull(b.Address1,'')+' '+isnull(b.Address2,'')+' '+isnull(b.Address3,'')+' '+isnull(b.Address4,'') Address
		                from 
			                spTrnSFPJHdr a
			                left join spTrnSFPJInfo b on
				                a.CompanyCode= b.CompanyCode and a.BranchCode= b.BranchCode and a.FPJNo= b.FPJNo
	                ) b on
                        a.CompanyCode= b.CompanyCode and a.BranchCode= b.BranchCode and a.DocNo= b.InvoiceNo
                WHERE 
                    a.CompanyCode = '{0}' AND a.BranchCode = '{1}' 
                    AND a.ProfitCenterCode = '{2}'
                    AND CONVERT(VARCHAR, a.FPJGovDate, 112) BETWEEN '{3}' AND '{4}'
                                ";
            }
            var query = string.Format(sqlstr, Uid.CompanyCode, Uid.BranchCode, profitCenterCode, stardate, enddate);
            var queryable = ctx.Context.Database.SqlQuery<GenerateTaxView>(query).AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<GenerateTaxView> FakturNoGab (string profitCenterCode, string stardate, string enddate)
        {
            var Uid = CurrentUser();
            int isCentral = 0;
            var dt = ctx.Context.LookUpDtls.Where(p => p.CompanyCode == Uid.CompanyCode && p.CodeID == GnMstLookUpHdr.FPGabungan);
            var isbranch = ctx.Context.OrganizationDtls.FirstOrDefault(p => p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode).IsBranch;
            if (dt.Count() > 0 &&  dt.FirstOrDefault().LookUpValue.Equals("1") && isbranch == false) isCentral = 1;

                var query = string.Format(@"
                SELECT 
                    a.BranchCode,a.ProfitCenterCode,a.FPJGovNo,a.FPJGovDate,a.DocNo,a.DocDate,b.DueDate,b.CustomerName
                    ,isnull(b.Address1,'')+' '+isnull(b.Address2,'')+' '+isnull(b.Address3,'')+' '+isnull(b.Address4,'') Address
                    ,sum(c.TotalDppAmt) DPPAmt, sum(c.TotalPpnAmt) PPNAmt,(sum(c.TotalDppAmt)+ sum(c.TotalPpnAmt)) Total
                FROM 
                    GnGenerateTax a 
                    inner join svTrnFakturPajak b on
                        a.CompanyCode= b.CompanyCode and a.BranchCode= b.BranchCode and a.DocNo= b.FPJNo
                    inner join svTrnInvoice c on
                        a.CompanyCode= c.CompanyCode 
		                and ((case when '{5}' = 1 then c.BranchCode end) <> ''
			                or (case when '{5}' = 0 then c.BranchCode end) = '{1}')
		                and b.FPJNo= c.FPJNo	
                WHERE 
                    a.CompanyCode = '{0}'
	                AND ((case when '{5}' = 1 then a.BranchCode end) <> ''
		                or (case when '{5}' = 0 then a.BranchCode end) = '{1}')
                    AND a.ProfitCenterCode ='{2}'
                    AND CONVERT(VARCHAR, a.FPJGovDate, 112) BETWEEN '{3}' AND '{4}'
                    AND b.isPKP = 1 AND b.NoOfInvoice > 1
	                AND c.IsLocked = '{5}'
                GROUP BY
                    a.BranchCode,a.ProfitCenterCode,a.FPJGovNo,a.FPJGovDate,a.DocNo,a.DocDate,b.DueDate,b.CustomerName
                    ,isnull(b.Address1,'')+' '+isnull(b.Address2,'')+' '+isnull(b.Address3,'')+' '+isnull(b.Address4,'')
                                       ", Uid.CompanyCode, Uid.BranchCode, profitCenterCode, stardate, enddate, isCentral);

            var queryable = ctx.Context.Database.SqlQuery<GenerateTaxView>(query).AsQueryable();
                return (queryable);
        }

        [HttpGet]
        public IQueryable<GenerateTaxView> GenerateTax4Lookup(DateTime DateFrom, DateTime DateTo)
        {
            var Uid = CurrentUser();
            var query = string.Format(@"uspfn_GnListTax '{0}', '{1}', '{2}'
                       ", Uid.CompanyCode, DateFrom, DateTo);

            var queryable = ctx.Context.Database.SqlQuery<GenerateTaxView>(query).AsQueryable();
            return (queryable);
        }
    }
}
