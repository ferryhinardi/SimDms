if object_id('uspfn_getSelectLookupCustomer') is not null
       drop procedure uspfn_getSelectLookupCustomer
go

-- =============================================
-- Author:		fhy
-- Create date: 23112015
-- Description:	get lookup customer
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_getSelectLookupCustomer] 	
	@companyCode varchar(25),
	@branchCode varchar(25),
	@profitCenterCode varchar(15),
	@dynamicfilters varchar(max)=''
AS
BEGIN
declare
	@query varchar(max);

set @query= 'SELECT a.CustomerCode, a.CustomerName,a.Address1, a.TopCode, a.TOPDesc, a.TOPCD, a.GroupPriceCode, a.RefferenceDesc1 as GroupPriceDesc, a.SalesCode
                  FROM    (SELECT x.CustomerCode, x.CustomerName, 
                                  x.Address1 + '' '' + x.Address2 + '' '' + x.Address3 + '' '' + x.Address4 as Address1,
                                  b.TOPCode + ''||''
                                  + (SELECT c.LookUpValueName
                                  FROM gnMstLookUpDtl c
                                  WHERE c.CodeID = ''TOPC''
                                  AND c.LookUpValue = b.TOPCode)  AS TOPDesc, 
                                  (SELECT c.ParaValue
                                  FROM gnMstLookUpDtl c
                                  WHERE c.CodeID = ''TOPC''
                                  AND c.LookUpValue = b.TOPCode)  AS TOPCode,
                                  b.TOPCode  AS TOPCD, b.CreditLimit, x.CompanyCode, b.BranchCode, 
                                  b.ProfitCenterCode, b.GroupPriceCode, c.RefferenceDesc1, b.SalesCode
                             FROM gnMstCustomer x
                            LEFT JOIN gnMstCustomerProfitCenter b ON b.CompanyCode = x.CompanyCode AND b.CustomerCode = x.CustomerCode
                            LEFT JOIN omMstRefference c ON x.CompanyCode = c.CompanyCode AND c.RefferenceType = ''GRPR'' AND c.RefferenceCode = b.GroupPriceCode
                            WHERE x.CompanyCode = b.CompanyCode
                                  AND x.CompanyCode = b.CompanyCode
                                  AND b.CompanyCode = '''+@companyCode+'''
                                  AND b.BranchCode = '''+@branchCode+'''
                                  AND b.ProfitCenterCode = '''+@profitCenterCode+''' 
			                      AND x.Status = ''1''
                                  AND b.SalesType = ''0''
			                      AND b.isBlackList = 0
                                  AND b.CreditLimit > 0) a
                       LEFT JOIN
                          gnTrnBankBook c
                       ON a.CompanyCode = c.CompanyCode
                          AND a.BranchCode = c.BranchCode
                          AND a.ProfitCenterCode = c.ProfitCenterCode
                          AND a.CustomerCode = c.CustomerCode
                 WHERE a.CreditLimit >
                          (ISNULL (c.SalesAmt, 0) - ISNULL (c.ReceivedAmt, 0)) '+@dynamicfilters+'
                ORDER BY a.CustomerCode ASC'

print(@query)
exec (@query)

END



GO


