
create view [dbo].[SpVendorClaimView]  as 
   
SELECT CompanyCode, BranchCode ,ClaimReceivedNo
    ,ClaimNo    
    ,ClaimReceivedDate, ClaimDate,
    TypeOfGoods, Status   
  FROM spTrnPRcvClaimHdr 
 WHERE --CompanyCode = '6006406'
   --AND BranchCode = '6006401'
   --AND TypeOfGoods = '0'
   (Status = '0' OR Status = '1')
 --ORDER BY ClaimReceivedNo DESC




GO


