

create procedure [dbo].[uspfn_spTrnPSUGGORHdr] (  @CompanyCode varchar(15) ,  @BranchCode varchar(15) )
 as

SELECT 
 a.SuggorNo
--,a.SuggorDate
,CONVERT(varchar(15), a.SuggorDate, 103) as SuggorDate
,a.SupplierCode
,b.SupplierName
FROM spTrnPSUGGORHdr a
LEFT JOIN gnMstSupplier b on b.CompanyCode=a.CompanyCode AND b.SupplierCode=a.SupplierCode                               
WHERE a.CompanyCode=@CompanyCode AND a.BranchCode=@BranchCode

AND a.status < 2
ORDER BY a.SuggorNo DESC