if object_id('uspfn_omSlsBPKLkpMdlYear') is not null
	drop procedure uspfn_omSlsBPKLkpMdlYear
GO
create procedure [dbo].[uspfn_omSlsBPKLkpMdlYear]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),  
 @DONo varchar(15),
 @SalesModelCode varchar(20)
)  
AS  
begin
	SELECT DISTINCT a.*
FROM omMstModelYear a
INNER JOIN OmTrSalesDODetail b
ON b.CompanyCode = a.CompanyCode
AND b.SalesModelCode = a.SalesModelCode
AND b.salesModelYear = a.SalesModelYear
WHERE a.CompanyCode = @CompanyCode
AND b.BranchCode = @BranchCode
AND a.Status = '1'
AND b.StatusBPK = '0'
AND b.DONo = @DONo
AND b.SalesModelCode = @SalesModelCode					 
end			
GO


