GO
/****** Object:  StoredProcedure [dbo].[sp_Select4LookupCustomer]    Script Date: 01/16/2015 17:50:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].sp_Select4LookupCustomer (@CompanyCode varchar(10) , @BranchCode varchar(10),
	@ProfitCenterCode varchar(10), @TOPC varchar(10))
AS
 SELECT TableA.CustomerCode, TableA.CustomerName, TableA.TopCode, TableA.TOPCD, TableA.GroupPriceCode, TableA.RefferenceDesc1 as GroupPriceDesc
  FROM    (SELECT a.CustomerCode, a.CustomerName, 
                  a.Address1 + ' ' + a.Address2 + ' ' + a.Address3 + ' ' + a.Address4 as Address,
                  b.TOPCode + '||'
                  + (SELECT c.LookUpValueName
                  FROM gnMstLookUpDtl c
                  WHERE c.CodeID = @TOPC
                  AND c.LookUpValue = b.TOPCode)  AS TopCode, b.TOPCode  AS
                  TOPCD, b.CreditLimit, a.CompanyCode, b.BranchCode, b.
                  ProfitCenterCode, b.GroupPriceCode, c.RefferenceDesc1
             FROM gnMstCustomer a
            LEFT JOIN gnMstCustomerProfitCenter b ON b.CompanyCode = a.CompanyCode AND b.CustomerCode = a.CustomerCode
            LEFT JOIN omMstRefference c ON a.CompanyCode = c.CompanyCode AND c.RefferenceType = 'GRPR' AND c.RefferenceCode = b.GroupPriceCode
            WHERE a.CompanyCode = b.CompanyCode
                  AND a.CompanyCode = b.CompanyCode
                  AND b.CompanyCode = @CompanyCode
                  AND b.BranchCode = @BranchCode
                  AND b.ProfitCenterCode = @ProfitCenterCode
                  AND a.Status = '1'
                  AND b.SalesType = '0'
                  AND b.isBlackList = 0
                  AND b.CreditLimit > 0) TableA
       LEFT JOIN
          gnTrnBankBook c
       ON TableA.CompanyCode = c.CompanyCode
          AND TableA.BranchCode = c.BranchCode
          AND TableA.ProfitCenterCode = c.ProfitCenterCode
          AND TableA.CustomerCode = c.CustomerCode
 WHERE TableA.CreditLimit >
          (ISNULL (c.SalesAmt, 0) - ISNULL (c.ReceivedAmt, 0))
ORDER BY TableA.CustomerCode ASC