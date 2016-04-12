
create view [dbo].[SpEntryCS]  as    
  SELECT DISTINCT CompanyCode, BranchCode, a.ClaimNo, ClaimDate, Status
  FROM spTrnPClaimHdr a
 WHERE a.Status IN ('0', '1')




GO


