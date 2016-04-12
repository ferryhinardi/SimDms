USE [BITPKU]
GO
/****** Object:  StoredProcedure [dbo].[uspfn_GetCustomerByProfitCenterCodeId]    Script Date: 10/28/2014 10:45:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[uspfn_GetCustomerByProfitCenterCodeId]  
@CompanyCode varchar(15),  
@BranchCode varchar(15),  
@ProfitCenterCode varchar(15)  
as  
SELECT a.CustomerCode, a.CustomerName  
     , a.Address1 + ' ' + a.Address2 + ' ' + a.Address3 +' ' + a.Address4 as Address, 
     a.Address1, a.Address2, a.Address3, a.Address4  
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
   AND a.CompanyCode = @CompanyCode  
   AND b.BranchCode = @BranchCode 
   AND b.ProfitCenterCode = @ProfitCenterCode  
   AND b.isBlackList = 0  
   AND c.LookupValue= b.ProfitCenterCode   
 AND a.Status = 1 order by a.CustomerCode