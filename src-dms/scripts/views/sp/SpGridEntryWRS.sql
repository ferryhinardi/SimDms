
create view [dbo].[SpGridEntryWRS]  as    

SELECT A.CompanyCode, A.BranchCode, A.DocNo, A.PartNo, A.PurchasePrice, A.WRSNo,
A.DiscPct,  A.ReceivedQty, A.BoxNo, (select PartName from spMstItemInfo C
where C.CompanyCode=A.CompanyCode and C.PartNo=A.PartNo) as NmPart 
  FROM spTrnPRcvDtl A
 INNER JOIN spTrnPRcvHdr B ON 
 B.CompanyCode = A.CompanyCode
 AND B.BranchCode = A.BranchCode 
 AND B.WRSNo = A.WRSNo 



GO


