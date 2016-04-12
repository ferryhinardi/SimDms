CREATE procedure [dbo].[uspfn_SvInqFpjCustomer]  
 @CompanyCode nvarchar(20),  
 @BranchCode nvarchar(20),
 @ProfitCenterCode nvarchar(20) 
  as  
   SELECT distinct a.CustomerCode, a.CustomerName,
       a.Address1+' '+a.Address2+' '+a.Address3+' '+a.Address4 as Address,
       c.LookUpValueName as ProfitCenter
  FROM gnMstCustomer a with(nolock, nowait)
    INNER JOIN gnMstCustomerProfitCenter b with(nolock, nowait) ON 
        b.CustomerCode= b.CustomerCode AND b.CustomerCode=a.CustomerCode
    INNER JOIN gnMstLookUpDtl c ON c.CompanyCode= a.CompanyCode
    LEFT JOIN svTrnInvoice d on d.CompanyCode = a.CompanyCode
        and d.CustomerCode = a.CustomerCode 
 WHERE  a.CompanyCode=@CompanyCode
    AND b.BranchCode=@BranchCode
    AND b.ProfitCenterCode= @ProfitCenterCode
    AND b.isBlackList=0
    AND a.status = 1
    AND d.InvoiceStatus = 0
    AND c.LookupValue= b.ProfitCenterCode 
    AND c.CodeID = 'PFCN'
    ORDER BY a.CustomerCode