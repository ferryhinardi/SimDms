

create view [dbo].[SpPartOrderView]  as    
SELECT DISTINCT a.CompanyCode, BranchCode,
a.PartNo, 
b.PartName,
a.DocNo, ClaimNo
FROM spTrnPCLaimDtl a
LEFT JOIN spMstItemInfo b ON b.CompanyCode = a.CompanyCode 
    AND b.PartNo = a.PartNo
--WHERE -- a.CompanyCode = '6006406'
    --AND a.BranchCode = '6006401'
    --AND ClaimNo = 'CLM/12/000001'
--ORDER BY a.PartNo


GO


