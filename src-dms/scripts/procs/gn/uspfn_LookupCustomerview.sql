/****** Object:  StoredProcedure [dbo].[uspfn_LookupCustomerview]    Script Date: 10/20/2014 10:44:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[uspfn_LookupCustomerview] (  @CompanyCode varchar(10), @BranchCode varchar(10) )
 as
SELECT distinct a.CustomerCode, a.CustomerName
     , isnull(a.Address1,'') + ' ' + isnull(a.Address2,'') + ' ' + isnull(a.Address3,'') +' ' + isnull(a.Address4,'') as Address
	 , '' LookupValue, '' as ProfitCenter
  FROM gnMstCustomer a with(nolock, nowait)
 left JOIN gnMstCustomerProfitCenter b with(nolock, nowait)
	ON b.CompanyCode = a.CompanyCode
   AND b.CustomerCode = a.CustomerCode
   AND b.BranchCode = @BranchCode
   AND b.isBlackList = 0
 left JOIN gnMstLookUpDtl c
	ON c.CompanyCode = a.CompanyCode
   AND c.CodeID = 'PFCN'
   AND c.LookupValue = b.ProfitCenterCode
 WHERE 1 = 1
   AND a.CompanyCode = @CompanyCode

  