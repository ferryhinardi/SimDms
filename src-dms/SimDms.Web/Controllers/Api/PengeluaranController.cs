using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Breeze.WebApi;
using Newtonsoft.Json.Linq;
using SimDms.Common.Models;
using SimDms.Sparepart.Models;
using Breeze.WebApi.EF;
using SimDms.Common;

namespace SimDms.Web.Controllers.Api
{
    [BreezeController]
    public class PengeluaranController : ApiController
    {
        readonly EFContextProvider<DataContext> ctx = new EFContextProvider<DataContext>();
        readonly EFContextProvider<MDContext> ctxMD = new EFContextProvider<MDContext>();

        [HttpGet]
        public String Metadata()
        {
            return ctx.Metadata();
        }

        [HttpGet]
        public SysUser CurrentUser()
        {
            return ctx.Context.SysUsers.Find(User.Identity.Name);
        }

        [HttpGet]
        public MyUserInfo CurrentUserInfo()
        {
            string s = "";
            var f = ctx.Context.SysUserProfitCenters.Find(User.Identity.Name);
            if (f != null) s = f.ProfitCenter;
            var u = ctx.Context.SysUsers.Find(User.Identity.Name);
            var g = ctx.Context.CoProfiles.Find(u.CompanyCode, u.BranchCode);

            var info = new MyUserInfo
            {
                UserId = u.UserId,
                FullName = u.FullName,
                CompanyCode = u.CompanyCode,
                CompanyGovName = g.CompanyGovName,
                BranchCode = u.BranchCode,
                CompanyName = g.CompanyName,
                TypeOfGoods = u.TypeOfGoods,
                ProductType = g.ProductType,
                ProfitCenter = s,
                TypeOfGoodsName = GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.TypeOfGoods, u.TypeOfGoods),
                ProductTypeName = GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.ProductType, g.ProductType),
                ProfitCenterName = GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.ProfitCenter, s),
                IsActive = u.IsActive,
                RequiredChange = u.RequiredChange,
                SimDmsVersion = GetType().Assembly.GetName().Version.ToString()
            };

            return info;
        }

        protected SysUser user
        {
            get
            {
                return CurrentUser();
            }
        }

        public string GetLookupValueName(string CompanyCode, string VarGroup, string varCode)
        {
            string s = "";
            var x = ctx.Context.LookUpDtls.Find(CompanyCode, VarGroup, varCode);
            if (x != null) s = x.LookUpValueName;
            return s;
        }
                
        protected string DealerCode()
        {
            var result = ctx.Context.CompanyMappings.FirstOrDefault(x => x.CompanyCode == user.CompanyCode && x.BranchCode == user.BranchCode);
            if (result != null)
            {
                if (result.CompanyMD == user.CompanyCode && result.BranchMD == user.BranchCode) { return "MD"; }
                else { return "SD"; }
            }
            else return "MD";
        }

        protected string CompanyMD
        {
            get
            {
                return ctx.Context.CompanyMappings.FirstOrDefault(x => x.CompanyCode == user.CompanyCode && x.BranchCode == user.BranchCode).CompanyMD;
            }
        }

        protected string BranchMD
        {
            get
            {
                return ctx.Context.CompanyMappings.FirstOrDefault(x => x.CompanyCode == user.CompanyCode && x.BranchCode == user.BranchCode).BranchMD;
            }
        }

        protected string WarehouseMD
        {
            get
            {
                return ctx.Context.CompanyMappings.FirstOrDefault(x => x.CompanyCode == user.CompanyCode && x.BranchCode == user.BranchCode).WarehouseMD;
            }
        }

        protected string DateTransValidation(DateTime date)
        {
            var user = CurrentUser().UserId;
            var currDate = DateTime.Now.Date;
            var errMsg1 = string.Format(ctx.Context.SysMsgs.Find("5006").MessageCaption, "Tanggal Transaksi", "Periode Transaksi");//string.Format("{0} tidak sesuai dengan {1}", "Tanggal transaksi", "periode transaksi");
            var errMsg2 = string.Format(ctx.Context.SysMsgs.Find("5006").MessageCaption, "Tanggal Transaksi", "Tanggal Server");//string.Format("{0} tidak sesuai dengan {1}", "Tanggal Transaksi", "Tanggal Server");
            var errMsg3 = "Periode sedang di locked";//string.Format("Periode sedang di locked");
            var errMsg4 = "Tanggal Transaksi lebih kecil dari tanggal [TransDate]";//string.Format("Tanggal Transaksi lebih kecil dari tanggal [TransDate]");
            var msg = "";
            try
            {
                var oProfCenter = ctx.Context.SysUserProfitCenters.Find(user);
                //300 For Sparepart
                if (oProfCenter.ProfitCenter.Equals("300"))
                {
                    var oSpare = ctx.Context.GnMstCoProfileSpares.Find(CurrentUser().CompanyCode, CurrentUser().BranchCode);
                    if (oSpare != null)
                    {
                        if (oSpare.TransDate.Equals(DBNull.Value) || oSpare.TransDate < new DateTime(1900, 1, 2)) oSpare.TransDate = DateTime.Now;
                        if (date >= oSpare.PeriodBeg.Date && date <= oSpare.PeriodEnd.Date)
                        {
                            if (date <= currDate)
                            {
                                if (date >= oSpare.TransDate.Date)
                                {
                                    if (oSpare.isLocked == true)
                                    {
                                        msg = errMsg3;
                                    }
                                }
                                else
                                {
                                    errMsg4 = errMsg4.Replace("[TransDate]", oSpare.TransDate.ToString("dd-MMM-yyyy"));
                                    msg = errMsg4;
                                }
                            }
                            else
                            {
                                msg = errMsg2;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return msg;
        }

        [HttpGet]
        public IEnumerable<DocumentNoView> DocumentNoBrowse(string SalesType, bool IsPORDD)
        {
            var query = string.Format(@" SELECT 
                                        	a.DocNo
                                            , a.DocDate
                                            , a.CustomerCode
                                            , CASE a.isLocked WHEN '1' THEN 
                                                (SELECT TOP 1 BranchCodeToDesc FROM spMstCompanyAccount WHERE CompanyCode=a.CompanyCode AND BranchCodeTo=a.CustomerCode)
                                        	  ELSE
                                        	    (SELECT CustomerName FROM gnMstCustomer WHERE CustomerCode = a.CustomerCode AND CompanyCode = a.CompanyCode) END CustomerName
                                            , (SELECT SupplierName FROM gnMstSupplier WHERE SupplierCode = a.CustomerCode AND CompanyCode = a.CompanyCode) AS SupplierName
                                            , a.Status 
                                        FROM 
                                        	spTrnSORDHdr a
                                        WHERE 
                                        	a.CompanyCode = {0}
                                            AND a.BranchCode = {1}
                                            AND a.SalesType = {2}
                                            AND a.Status IN (0,1) 
                                            AND a.isPORDD = {3}
                                            AND a.TypeOfGoods = {4}
                                        ORDER BY 
                                        	a.DocNo DESC
                                        ", CurrentUserInfo().CompanyCode, CurrentUserInfo().BranchCode, SalesType, IsPORDD ? 1 : 0, CurrentUserInfo().TypeOfGoods);

            return ctx.Context.Database.SqlQuery<DocumentNoView>(query);
        }

        [HttpGet]
        public IQueryable<DocumentNoView> DocumentNoBrowseNew(string SalesType, bool IsPORDD)
        {
            var query = string.Format(@" SELECT 
                                        	a.DocNo
                                            , a.DocDate
                                            , a.CustomerCode
                                            , CASE a.isLocked WHEN '1' THEN 
                                                (SELECT TOP 1 BranchCodeToDesc FROM spMstCompanyAccount WHERE CompanyCode=a.CompanyCode AND BranchCodeTo=a.CustomerCode)
                                        	  ELSE
                                        	    (SELECT CustomerName FROM gnMstCustomer WHERE CustomerCode = a.CustomerCode AND CompanyCode = a.CompanyCode) END CustomerName
                                            , (SELECT SupplierName FROM gnMstSupplier WHERE SupplierCode = a.CustomerCode AND CompanyCode = a.CompanyCode) AS SupplierName
                                            , a.Status 
                                        FROM 
                                        	spTrnSORDHdr a
                                        WHERE 
                                        	a.CompanyCode = {0}
                                            AND a.BranchCode = {1}
                                            AND a.SalesType = {2}
                                            AND a.Status IN (0,1) 
                                            AND a.isPORDD = {3}
                                            AND a.TypeOfGoods = {4}
                                        ORDER BY 
                                        	a.DocNo DESC
                                        ", CurrentUserInfo().CompanyCode, CurrentUserInfo().BranchCode, SalesType, IsPORDD ? 1 : 0, CurrentUserInfo().TypeOfGoods);

            return ctx.Context.Database.SqlQuery<DocumentNoView>(query).AsQueryable();
        }

        [HttpGet]
        public IQueryable<CustomerViewLookUp> CustomerCodeLookup(string status)
        {
            var query = string.Format(@"
                                        SELECT a.CustomerCode, a.CustomerName
                                             , a.Address1 + ' ' + a.Address2 + ' ' + a.Address3 +' ' + a.Address4 as Address
                                        	 , c.LookupValue, c.LookUpValueName as ProfitCenter
                                             , b.Salesman
                                          FROM gnMstCustomer a with(nolock, nowait)
                                         INNER JOIN gnMstCustomerProfitCenter b with(nolock, nowait)
                                        	ON b.CompanyCode = a.CompanyCode
                                            AND b.CustomerCode = a.CustomerCode
                                         INNER JOIN gnMstLookUpDtl c
                                        	ON c.CompanyCode = a.CompanyCode
                                           AND c.CodeID = 'PFCN'
                                           AND c.LookupValue = b.ProfitCenterCode
                                         WHERE 1 = 1
                                           AND a.CompanyCode = '{0}'
                                           AND b.BranchCode = '{1}'
                                           AND b.ProfitCenterCode = '{2}'
                                           AND b.isBlackList = 0
                                           AND c.LookupValue= b.ProfitCenterCode
                                           AND a.Status = {3}"
                , CurrentUserInfo().CompanyCode, CurrentUserInfo().BranchCode, "300", status);

            return ctx.Context.Database.SqlQuery<CustomerViewLookUp>(query).AsQueryable();
        }

        [HttpGet]
        public IQueryable<CustomerViewLookUp> CustomerCodeTransStockLookup(string status)
        {
            string query = string.Format(@"
                            SELECT a.CustomerCode, a.CustomerName,
                                   a.Address1+' '+a.Address2+' '+a.Address3+' '+a.Address4 as Address,
                                   c.LookUpValueName as ProfitCenter
                              FROM gnMstCustomer a with(nolock, nowait)
                                INNER JOIN gnMstOrganizationDtl d ON a.CompanyCode = d.CompanyCode AND a.CustomerCode = d.BranchCode
                                INNER JOIN gnMstCustomerProfitCenter b with(nolock, nowait) ON 
                                    a.CompanyCode = b.CompanyCode AND b.CustomerCode=a.CustomerCode
                                INNER JOIN gnMstLookUpDtl c ON c.CompanyCode= a.CompanyCode
                             WHERE  a.CompanyCode='{0}'
                                AND b.BranchCode='{1}'
                                AND b.ProfitCenterCode= '{2}'
                                AND b.isBlackList=0
                                AND a.status = 1
                                AND c.LookupValue = b.ProfitCenterCode
                                AND c.CodeID= '{3}' ", CurrentUserInfo().CompanyCode, CurrentUserInfo().BranchCode, "300", "PFCN");

            return ctx.Context.Database.SqlQuery<CustomerViewLookUp>(query).AsQueryable();
        }

        [HttpGet]
        public IQueryable<CustomerViewLookUp> OpenData(string status)
        {
            var data = ctx.Context.spMstCompanyAccounts.Where(a => a.CompanyCode == user.CompanyCode)
                .Select(a=> new CustomerViewLookUp {
                    CompanyName = a.CompanyCodeToDesc,
                    BranchCode = a.BranchCodeTo,
                    BranchName = a.BranchCodeToDesc
                });

            return data;
        }

        [HttpGet]
        public IQueryable<PaymentLookUp> PaymentLookup()
        {
            var curCompanyCode = CurrentUserInfo().CompanyCode;

            var queryable = ctx.Context.LookUpDtls.Where(a => a.CompanyCode == curCompanyCode && a.CodeID == "PYBY")
                .Select(c => new PaymentLookUp { PaymentCode = c.LookUpValue, PaymentDesc = c.LookUpValueName });

            return queryable;
        }

        [HttpGet]
        public IQueryable<SalesPartLookUp> SalesPartLookup()
        {
            var user = CurrentUserInfo();
            var ids = new[] { "7", "58" };
            var queryable = ctx.Context.Employees.Where(a => a.CompanyCode == user.CompanyCode && a.BranchCode == user.BranchCode
                && ids.Contains(a.TitleCode)).Select(c => new SalesPartLookUp { EmployeeID = c.EmployeeID, EmployeeName = c.EmployeeName });

            return queryable;
        }

        [HttpGet]
        public IQueryable<PartNoLookUp> PartNoLookupUnit(string SONo, string DocNo)
        {
            var user = CurrentUserInfo();
            var compCode = "";
            var branchCode = "";
            var query = String.Empty;

            bool md = DealerCode() == "MD";

            compCode = md ? user.CompanyCode : CompanyMD;
            branchCode = md ? user.BranchCode : BranchMD;

            if (SONo != null)
            {
                string dbSD = ctx.Context.Database.SqlQuery<string>("SELECT TOP 1 DbName FROM gnMstCompanyMapping WHERE CompanyCode='" + user.CompanyCode + "' AND BranchCode='" + user.BranchCode + "'").FirstOrDefault();
                if (dbSD != null || dbSD != "")
                {
                     query = String.Format(@"SELECT
                                        ItemInfo.PartNo
                                        ,Items.ABCClass
                                        ,ItemLoc.OnHand-itemLoc.ReservedSP - itemLoc.ReservedSR - itemLoc.ReservedSL - itemLoc.AllocationSP - itemLoc.AllocationSL - itemLoc.AllocationSR AS AvailQty
                                        ,Items.OnOrder
                                        ,Items.ReservedSP
                                        ,Items.ReservedSR
                                        ,Items.ReservedSL
                                        ,Items.MovingCode
                                        ,ItemInfo.SupplierCode
                                        ,ItemInfo.PartName
                                        ,x.PurchasePrice
                                        ,x.RetailPrice
                                        ,x.RetailPriceInclTax
                                        FROM SpMstItems Items
                                        INNER JOIN SpMstItemInfo ItemInfo ON Items.CompanyCode  = ItemInfo.CompanyCode                          
                                            AND Items.PartNo = ItemInfo.PartNo
                                        INNER JOIN spMstItemLoc ItemLoc ON Items.CompanyCode = ItemLoc.CompanyCode AND Items.BranchCode = ItemLoc.BranchCode
                                            AND Items.PartNo = ItemLoc.PartNo
                                        INNER JOIN {4}..spMstItemPrice x ON x.PartNO = ItemInfo.PartNo AND x.CompanyCode = '{5}' AND x.BranchCode='{6}' 
                                        WHERE Items.CompanyCode = '{0}'
                                          AND Items.BranchCode  = '{1}'    
                                          AND Items.ProductType = '{2}'
                                          AND Items.Status > 0
                                          AND ItemLoc.WarehouseCode = '00'
										  AND ItemInfo.PartNo in (select PartNo from {4}..omTrSalesSOAccsSeq WHERE Sono='{3}' and companycode='{5}' and branchcode='{6}')
                                          AND ItemInfo.PartNo not in (select c.PartNo from {4}..spTrnSORDHdr b inner join {4}..SpTrnSORDDtl c on 
                                          (b.companycode = c.companycode and b.branchcode = c.branchcode and b.DocNo = c.DocNo) where b.UsageDocNo='{3}' and b.companycode='{5}' and b.branchcode='{6}')",
                                              compCode, branchCode, user.ProductType, SONo, dbSD, user.CompanyCode, user.BranchCode, DocNo);
                                       // AND ItemInfo.PartNO not in (select PartNo from {4}..SpTrnSORDDtl WHERE companycode='{5}' and branchcode='{6}' and docno='{7}'
                }                                                                                                                                              
            }

            var datItemFromMD = md ?
                ctx.Context.Database.SqlQuery<PartNoLookUp>(query) :
                ctxMD.Context.Database.SqlQuery<PartNoLookUp>(query);

            //var itemPrice = (from p in ctx.Context.spMstItemPrices 
            //            where p.CompanyCode == user.CompanyCode
            //            && p.BranchCode == user.BranchCode
            //            select p).AsEnumerable();

            var rslt = from a in datItemFromMD
                       select new PartNoLookUp
                       {
                           ABCClass = a.ABCClass,
                           AvailQty = a.AvailQty,
                           MovingCode = a.MovingCode,
                           OnOrder = a.OnOrder,
                           PartName = a.PartName,
                           PartNo = a.PartNo,
                           PurchasePrice = a.PurchasePrice,
                           ReservedSL = a.ReservedSL,
                           ReservedSP = a.ReservedSP,
                           ReservedSR = a.ReservedSR,
                           RetailPrice = a.RetailPrice,
                           RetailPriceInclTax = a.RetailPriceInclTax,
                           SupplierCode = a.SupplierCode
                       };

            return rslt.AsQueryable();
        }

        [HttpGet]
        public IQueryable<PartNoLookUp> PartNoLookup()
        {
            var user = CurrentUserInfo();
            var compCode = "";
            var branchCode = "";

            bool md = DealerCode() == "MD";

            compCode = md ? user.CompanyCode : CompanyMD;
            branchCode = md ? user.BranchCode : BranchMD;

            var query = String.Format(@"
                                        SELECT
                                        ItemInfo.PartNo
                                        ,Items.ABCClass
                                        ,ItemLoc.OnHand - itemLoc.ReservedSP - itemLoc.ReservedSR - itemLoc.ReservedSL - itemLoc.AllocationSP - itemLoc.AllocationSL - itemLoc.AllocationSR AS AvailQty
                                        ,Items.OnOrder
                                        ,Items.ReservedSP
                                        ,Items.ReservedSR
                                        ,Items.ReservedSL
                                        ,Items.MovingCode
                                        ,ItemInfo.SupplierCode
                                        ,ItemInfo.PartName
                                        --,ItemPrice.RetailPrice
                                        --,ItemPrice.RetailPriceInclTax
                                        --,ItemPrice.PurchasePrice
                                        FROM SpMstItems Items
                                        INNER JOIN SpMstItemInfo ItemInfo ON Items.CompanyCode  = ItemInfo.CompanyCode                          
                                            AND Items.PartNo = ItemInfo.PartNo
                                        --INNER JOIN spMstItemPrice ItemPrice ON Items.CompanyCode = ItemPrice.CompanyCode
                                        --AND Items.BranchCode = ItemPrice.BranchCode AND Items.PartNo = ItemPrice.PartNo
                                        INNER JOIN spMstItemLoc ItemLoc ON Items.CompanyCode = ItemLoc.CompanyCode AND Items.BranchCode = ItemLoc.BranchCode
                                            AND Items.PartNo = ItemLoc.PartNo
                                        WHERE Items.CompanyCode = '{0}'
                                          AND Items.BranchCode  = '{1}'    
                                          AND Items.TypeOfGoods = '{2}'
                                          AND Items.ProductType = '{3}'
                                          AND Items.Status > 0
                                          AND ItemLoc.WarehouseCode = '00'", compCode, branchCode, user.TypeOfGoods, user.ProductType);

            var datItemFromMD = md ?
                ctx.Context.Database.SqlQuery<PartNoLookUp>(query) :
                ctxMD.Context.Database.SqlQuery<PartNoLookUp>(query);

            var itemPrice = (from p in ctx.Context.spMstItemPrices
                             where p.CompanyCode == user.CompanyCode
                             && p.BranchCode == user.BranchCode
                             select p).AsEnumerable();

            var rslt = from b in itemPrice
                       join a in datItemFromMD on b.PartNo
                       equals a.PartNo
                       select new PartNoLookUp
                       {
                           ABCClass = a.ABCClass,
                           AvailQty = a.AvailQty,
                           MovingCode = a.MovingCode,
                           OnOrder = a.OnOrder,
                           PartName = a.PartName,
                           PartNo = a.PartNo,
                           PurchasePrice = b.PurchasePrice,
                           ReservedSL = a.ReservedSL,
                           ReservedSP = a.ReservedSP,
                           ReservedSR = a.ReservedSR,
                           RetailPrice = b.RetailPrice,
                           RetailPriceInclTax = b.RetailPriceInclTax,
                           SupplierCode = a.SupplierCode
                       };

            return rslt.AsQueryable();
        }

        [HttpGet]
        public IQueryable<PartNoLookUp> PartNoLookupReq(string ReqNo, string DocNo, string Customer) 
        {
            var user = CurrentUserInfo();
            var compCode = "";
            var branchCode = "";

            bool md = DealerCode() == "MD";

            compCode = md ? CompanyMD : user.CompanyCode;
            branchCode = md ? BranchMD : user.BranchCode;

            var query = String.Format(@"
                                        select 
                                        a.partno
                                        , c.OnHand - c.ReservedSP - c.ReservedSR - c.ReservedSL - c.AllocationSP - c.AllocationSL - c.AllocationSR AS AvailQty
                                        , orderqty OnOrder
                                        , MovingCode
                                        , ABCClass
                                        , partname
                                        from spTrnPREQdtl a
                                        INNER JOIN SpMstItemInfo b ON a.CompanyCode  = b.CompanyCode                          
                                            AND a.PartNo = b.PartNo
                                        INNER JOIN spMstItemLoc c ON a.CompanyCode = b.CompanyCode AND a.BranchCode = c.BranchCode
                                            AND b.PartNo = c.PartNo
                                        INNER JOIN spTrnPREQHdr d ON a.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode
                                            AND a.REQNo = d.REQNo
                                        WHERE a.CompanyCode = '{0}'
                                          AND a.BranchCode  = '{1}'    
                                          AND a.ProductType = '{2}'
                                          AND a.REQNo = '{3}'
                                          AND d.Status = 2
                                          AND d.ProcessFlag = 0
                                          AND not exists(select * 
														from spTrnSORDDtl 
														where CompanyCode = a.CompanyCode
														AND BranchCode  = '{4}' 
														and referenceno = d.ReqNo
														and PartNo = a.PartNo
														)", compCode, Customer, user.ProductType, ReqNo, user.BranchCode);

            var datItemFromMD = ctx.Context.Database.SqlQuery<PartNoLookUp>(query);

            var itemPrice = (from p in ctx.Context.spMstItemPrices
                             where p.CompanyCode == user.CompanyCode
                             && p.BranchCode == user.BranchCode
                             select p).AsEnumerable();

            var rslt = from b in itemPrice
                       join a in datItemFromMD on b.PartNo
                       equals a.PartNo
                       select new PartNoLookUp
                       {
                           ABCClass = a.ABCClass,
                           AvailQty = a.AvailQty,
                           MovingCode = a.MovingCode,
                           OnOrder = a.OnOrder,
                           PartName = a.PartName,
                           PartNo = a.PartNo,
                           PurchasePrice = b.PurchasePrice,
                           ReservedSL = a.ReservedSL,
                           ReservedSP = a.ReservedSP,
                           ReservedSR = a.ReservedSR,
                           RetailPrice = b.RetailPrice,
                           RetailPriceInclTax = b.RetailPriceInclTax,
                           SupplierCode = a.SupplierCode
                       };

            return rslt.AsQueryable();
        }

        [HttpGet]
        public IQueryable<JobOrderLookUp> JobOrderLookup()
        {
            var query =string.Format(@" SELECT
                                	DISTINCT(JobOrderNo)
                                	, JobOrderDate
                                	, CustomerCode
                                	, CustomerCodeBill
                                FROM
                                	svTrnService a WITH(NOLOCK, NOWAIT)
                                	LEFT JOIN svTrnSrvItem b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
                                		AND a.BranchCode = b.BranchCode
                                		AND a.ProductType = b.ProductType
                                        AND a.ServiceNo = b.ServiceNo
                                	LEFT JOIN (SELECT CompanyCode, BranchCode, ProductType, ServiceNo, PartNo,
                                				SUM(ISNULL(DemandQty, 0) - (ISNULL(SupplyQty, 0) - ISNULL(ReturnQty, 0))) QtyOrder
                                				FROM svTrnSrvItem 
                                				GROUP BY CompanyCode, BranchCode, ProductType, ServiceNo, PartNo) Item ON
                                		Item.CompanyCode = a.CompanyCode AND Item.BranchCode = a.BranchCode AND Item.ProductType =
                                			a.ProductType AND Item.ServiceNo = a.ServiceNo AND Item.PartNo = b.PartNo
                                WHERE
                                	a.CompanyCode = {0}
                                	AND a.BranchCode = {1}
                                	AND a.ProductType = '{2}'
                                	AND ServiceStatus < 5
                                	AND ServiceType = '2'
                                	AND TypeOfGoods = {3}	
                                	AND Item.QtyOrder > 0
                                ORDER BY
                                	JobOrderNo DESC
                                ",user.CompanyCode,user.BranchCode,CurrentUserInfo().ProductType,user.TypeOfGoods);

            return ctx.Context.Database.SqlQuery<JobOrderLookUp>(query).AsQueryable();
        }

        [HttpGet]
        public IQueryable<SONoLookUp> SONoLookup()
        {
            var query = string.Format(@"SELECT a.SONo
                                       , b.SODate
                                	    , b.CustomerCode
                                	    , CASE WHEN LEN(b.BillTo) = 0 THEN b.CustomerCode ELSE b.BillTo END AS BillTo
                                	    , CASE WHEN LEN(b.ShipTo) = 0 THEN b.CustomerCode ELSE b.ShipTo END AS ShipTo
                                   FROM omTrSalesSOAccsSeq a
                                        INNER JOIN omTrSalesSO b ON b.CompanyCode=a.CompanyCode AND b.BranchCode=a.BranchCode AND b.SONo=a.SONo
                                   WHERE a.CompanyCode={0}
                                       AND a.BranchCode={1}
                                       AND b.Status='2'
                                       AND a.TypeOfGoods={2}
                                   GROUP BY a.SONo
                                        , b.SODate
                                	    , b.CustomerCode
                                	    , CASE WHEN LEN(b.BillTo) = 0 THEN b.CustomerCode ELSE b.BillTo END 
                                	    , CASE WHEN LEN(b.ShipTo) = 0 THEN b.CustomerCode ELSE b.ShipTo END 
                                        , a.CompanyCode
									    , a.BranchCode
									    , a.TypeOfGoods
                                        --, a.PartNo 
                                   -- HAVING SUM(DemandQty) > SUM(SupplyQty) AND SUM(a.Qty) > SUM(d.QtyOrder)
                                   HAVING Count(a.SONo) > (select count(c.DocNo) from spTrnSORDHdr c 
											INNER JOIN spTrnSORDDtl d ON d.CompanyCode=c.CompanyCode AND d.BranchCode=c.BranchCode AND d.DocNo=c.DocNo
											where c.CompanyCode=a.CompanyCode and c.BranchCode=a.BranchCode and c.UsageDocNo=a.SONo and c.TypeOfGoods=a.TypeOfGoods)
                                   ORDER BY
                                       a.SONo DESC ", user.CompanyCode, user.BranchCode, user.TypeOfGoods);

            return ctx.Context.Database.SqlQuery<SONoLookUp>(query).AsQueryable();
        }

        [HttpGet]
        public IQueryable<OrderNoLookUp> OrderNoLookup(string customerCode)
        {
            var query = string.Format(@"
            SELECT 
                Distinct b.OrderNo, 
                b.OrderDate
            FROM
                SpUtlPORDDDtl a with(nolock, nowait)  
            INNER JOIN 
                SpUtlPORDDHdr b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.OrderNo = b.OrderNo
            WHERE 
                a.CompanyCode = {0}  AND 
                a.BranchCode = {1} AND 
                b.DealerCode = {2} AND
                (Select TypeOfGoods FROM SpMstItems where CompanyCode = {0} and BranchCode = {1} and 
                    PartNo = a.PartNo) = {3} AND
                status= 0
            ORDER BY OrderNo DESC
            ", user.CompanyCode, user.BranchCode, customerCode, user.TypeOfGoods);

            return ctx.Context.Database.SqlQuery<OrderNoLookUp>(query).AsQueryable();
        }

        public IQueryable<PickingListBrowse> PickingListBrowse()
        {
            string isBORelease = "";
            var uid = CurrentUserInfo();
            string sql = string.Format("exec uspfn_GetTrnPickingHdr '{0}','{1}','{2}',{3}", uid.CompanyCode, uid.BranchCode, uid.TypeOfGoods, isBORelease);
            var queryable = ctx.Context.Database.SqlQuery<PickingListBrowse>(sql).AsQueryable();
            return (queryable);
        }

        public IQueryable<CustSOPickingList> CustPickingListLookup()
        {
            var uid = CurrentUserInfo();
            string sql = string.Format("exec uspfn_CustSOPickList '{0}','{1}','{2}','{3}'", uid.CompanyCode, uid.BranchCode, "300", uid.TypeOfGoods);
            var queryable = ctx.Context.Database.SqlQuery<CustSOPickingList>(sql).AsQueryable();
            return (queryable);
        }

        public IQueryable<Employee> EmployeePickedByLookup()
        {
            var uid = CurrentUserInfo();
            var queryable = ctx.Context.Employees.Where(m=>m.CompanyCode == uid.CompanyCode && m.BranchCode == m.BranchCode && m.PersonnelStatus == "1").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<InvoiceCancelLookup> InvoiceCancelOpenLookUp()
        {
            var queryable = ctx.Context.SpTrnSRturSrvHdrs.Where(a => a.CompanyCode == user.CompanyCode && a.BranchCode == user.BranchCode && a.Status == "2").ToList()
                .Select(a => new InvoiceCancelLookup
                {
                    InvoiceNo = a.InvoiceNo,
                    InvoiceDate = a.InvoiceDate,
                    CustomerCode = a.CustomerCode,
                    CustomerName = ctx.Context.GnMstCustomers.FirstOrDefault(c => c.CompanyCode == user.CompanyCode && c.CustomerCode == a.CustomerCode).CustomerName,
                    ReturnNo = a.ReturnNo,
                    ReturnDate = a.ReturnDate
                }).AsQueryable();

            return (queryable);
        }

        [HttpGet]
        public IQueryable<InvoiceCancelLookup> InvoiceCancelLookUp()
        {
            string query = string.Format(@"SELECT a.InvoiceNo
                            	, a.InvoiceDate
                            	, a.CustomerCode 
                            	, (SELECT CustomerName 
                            		FROM GnMstCustomer 
                            		WHERE CompanyCode = a.CompanyCode AND CustomerCode = a.CustomerCode
                            	   ) CustomerName
                                , b.ReturnNo
                                , b.ReturnDate
                                 FROM svTrnInvoice a
                                 LEFT JOIN svTrnReturn b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.InvoiceNo = b.InvoiceNo
                                 WHERE a.CompanyCode = '{0}'
                            	AND a.BranchCode = '{1}'
                            	AND a.ProductType = '{2}'
                            	AND a.InvoiceStatus = 4
                                AND b.ReturnNo is not null
                            	AND a.InvoiceNo in
                            	(
                            		select p.InvoiceNo from (
												select z.InvoiceNo from
													(select a.InvoiceNo, COUNT (b.PartNo) as TotPartInv
														FROM svTrnInvoice a
														inner join svTrnInvItem b
															ON a.CompanyCode = b.CompanyCode 
															AND a.BranchCode = b.BranchCode 
															AND a.InvoiceNo = b.InvoiceNo 
														WHERE a.CompanyCode = '{0}'
														AND a.BranchCode = '{1}'
														AND a.ProductType = '{2}'
														AND a.InvoiceStatus = 4  
														group by a.InvoiceNo) z
													left join (select a.InvoiceNo, COUNT (b.PartNo) as TotPartRet
																FROM SpTrnSRturSrvHdr a
																inner join spTrnSRturSrvDtl b
																	ON a.CompanyCode = b.CompanyCode 
																	AND a.BranchCode = b.BranchCode 
																	AND a.ReturnNo = b.ReturnNo 
																WHERE a.CompanyCode = '{0}'
																AND a.BranchCode = '{1}'  
																group by a.InvoiceNo) y
													on z.InvoiceNo = y.InvoiceNo
													where isnull(z.TotPartInv,0) <> isnull(y.TotPartRet,0))p)
                            	", user.CompanyCode, user.BranchCode, CurrentUserInfo().ProductType);

            return ctx.Context.Database.SqlQuery<InvoiceCancelLookup>(query).AsQueryable();
        }
    }
}
