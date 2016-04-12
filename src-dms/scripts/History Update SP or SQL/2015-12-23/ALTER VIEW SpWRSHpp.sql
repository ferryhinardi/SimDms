ALTER view [dbo].[SpWRSHpp]  as 

SELECT a.CompanyCode, a.BranchCode,
WRSNo,WRSDate, b.ReferenceNo, b.ReferenceDate, b.DNSupplierNo, c.SupplierName, a.TypeOfGoods  
FROM 
spTrnPRcvHdr a
LEFT JOIN SpTrnPBinnHdr b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode
AND b.BinningNo = a.BinningNo 
LEFT JOIN gnMstSupplier c ON c.CompanyCode = a.CompanyCode AND c.SupplierCode = b.SupplierCode
WHERE --a.CompanyCode = '6006406'
--AND a.BranchCode = '6006401'
a.Status = '2'
--AND a.TypeOfGoods ='0'
AND a.ReceivingType in (1,2)
AND a.TransType = 4
--ORDER BY a.CreatedDate DESC
GO