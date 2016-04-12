
create view [dbo].[SpEdpPartNo]  as    
SELECT a.CompanyCode, a.BranchCode, 
 a.PartNo
,b.PartName
,c.CostPrice
,a.QtyBill,
CustomerCode, a.BPSFNo
FROM spTrnSBPSFDtl a
INNER JOIN spMstItemInfo b
   ON b.CompanyCode = a.CompanyCode
  AND b.PartNo      = a.PartNo
INNER JOIN spMstItemPrice c
   ON c.CompanyCode = a.CompanyCode
  AND c.BranchCode  = a.BranchCode
  AND c.PartNo      = a.PartNo
INNER JOIN spTrnSBPSFHdr d
   ON d.CompanyCode = a.CompanyCode
  AND d.BranchCode  = a.BranchCode
  AND d.BPSFNo      = a.BPSFNo
/*WHERE a.CompanyCode=@CompanyCode
  AND a.BranchCode=@BranchCode
  AND d.CustomerCode=@CustomerCode
  AND a.BPSFNo = @BPSFNo*/
GROUP BY a.PartNo, b.PartName, c.CostPrice, a.QtyBill, a.CompanyCode, a.BranchCode, CustomerCode, a.BPSFNo

GO


