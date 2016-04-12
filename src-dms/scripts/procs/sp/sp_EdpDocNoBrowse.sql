CREATE procedure [dbo].[sp_EdpDocNoBrowse] (  

@CompanyCode varchar(10) ,
@BranchCode varchar(10),
@TypeOfGoods varchar(10),
@ProductType varchar(10),
@SupplierCode varchar(30))


as

SELECT 
 a.POSNo
,a.PosDate
,a.SupplierCode
FROM SpTrnPOrderBalance a with(nolock, nowait)
INNER JOIN SpTrnPPOSHdr b ON b.POSNo = a.POSNo AND b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode
WHERE a.OnOrder>0
  AND a.CompanyCode=@CompanyCode
  AND a.BranchCode=@BranchCode
  AND a.TypeOfGoods=@TypeOfGoods
  AND b.ProductType=@ProductType
  AND a.SupplierCode=@SupplierCode
GROUP BY  a.POSNo, a.PosDate, a.SupplierCode 
ORDER BY  a.POSNo DESC
GO


