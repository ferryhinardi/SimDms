

create view [dbo].[SpEdpDetail]  as  
  
SELECT row_number () OVER (ORDER BY a.CreatedDate) AS NoUrut, a.CompanyCode, a.BranchCode, a.BinningNo,
 a.DocNo,a.PartNo, a.PurchasePrice, a.DiscPct, 
a.ReceivedQty, a.BoxNo, (select PartName from spMstItemInfo c WITH(NOLOCK, NOWAIT)
WHERE c.CompanyCode=a.CompanyCode and c.PartNo=a.PartNo) as NmPart
FROM spTrnPBinnDtl a WITH(NOLOCK, NOWAIT)
INNER JOIN spTrnPBinnHdr b WITH(NOLOCK, NOWAIT) ON b.BinningNo = a.BinningNo 
AND b.CompanyCode = a.CompanyCode
AND b.BranchCode = a.BranchCode


GO


