

create view [dbo].[SpPartNoView]  as    
 SELECT a.CompanyCode, BranchCode, a.PartNo, x.PartName, a.DocNo, WRSNo, ReceivedQty, a.PurchasePrice FROM spTrnPRcvDtl a
INNER JOIN spMstItemInfo x with(nolock, nowait) on a.CompanyCode = x.CompanyCode AND a.PartNo = x.PartNo
WHERE NOT EXISTS
(
SELECT 1 FROM spTrnPClaimDtl b
INNER JOIN spTrnPClaimHdr c ON b.CompanyCode = c.CompanyCode AND b.BranchCode = c.BranchCode AND b.ClaimNo = c.ClaimNo
WHERE a.CompanyCode = b.CompanyCode 
AND a.BranchCode = b.BranchCode
AND c.WRSNo = a.WRSNO
AND a.Partno = b.PartNo
AND a.DocNo = b.DocNo
)
--AND a.CompanyCode = '6006406'
--AND a.BranchCode = '6006401'
--AND a.WRSNO = 'WRN/13/000780'


GO


