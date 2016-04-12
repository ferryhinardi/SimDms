

/****** Object:  StoredProcedure [dbo].[sp_EdpPelangganBrowse]    Script Date: 07/03/2014 14:16:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO






CREATE procedure [dbo].[sp_EdpPelangganBrowse] (  

@CompanyCode varchar(10),
@BranchCode varchar(10),
@ProfitCenter varchar(30))


as

SELECT a.CustomerCode, a.CustomerName,
       a.Address1+' '+a.Address2+' '+a.Address3+' '+a.Address4 as Address,
       c.LookUpValueName as ProfitCenter
  FROM gnMstCustomer a with(nolock, nowait)
    INNER JOIN gnMstCustomerProfitCenter b with(nolock, nowait) ON 
        a.CompanyCode = b.CompanyCode 
        AND b.BranchCode = @BranchCode
        AND b.ProfitCenterCode = @ProfitCenter
        AND b.CustomerCode = a.CustomerCode
        AND b.isBlackList=0
    INNER JOIN gnMstLookUpDtl c ON c.CompanyCode= a.CompanyCode
         AND c.LookupValue= b.ProfitCenterCode
         AND c.CodeID= 'PFCN'
 WHERE  a.CompanyCode=@CompanyCode
    AND a.status = 1   
    ORDER BY a.CustomerCode
    






GO


