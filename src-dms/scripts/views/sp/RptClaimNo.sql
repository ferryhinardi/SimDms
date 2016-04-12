
create view [dbo].[RptClaimReceivedNo]  as    

  SELECT CompanyCode, BranchCode, ClaimReceivedNo
    ,ClaimNo    
    ,ClaimReceivedDate, TypeOfGoods   
  FROM spTrnPRcvClaimHdr 
 WHERE (Status = '2' OR Status = '8')


GO


