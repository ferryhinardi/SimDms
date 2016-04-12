if object_id('uspfn_omSlsBPKLkpMdlCode') is not null
	drop procedure uspfn_omSlsBPKLkpMdlCode
GO
create procedure [dbo].[uspfn_omSlsBPKLkpMdlCode]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),  
 @DONo varchar(15) 
)  
AS  
BEGIN  
--exec uspfn_omSlsDoLkpMdlCode 6006410,600641001,''
SELECT DISTINCT  
	a.CompanyCode
	,a.SalesModelCode
	,a.SalesModelDesc
	,a.FakturPolisiDesc
	,a.EngineCode
	,a.PpnBmCodeBuy
	,a.PpnBmPctBuy
	,a.PpnBmCodeSell
	,a.PpnBmPctSell
	,a.PpnBmCodePrincipal
	,a.PpnBmPctPrincipal
	,a.Remark
	,a.BasicModel
	,a.TechnicalModelCode
	,a.ProductType
	,a.TransmissionType
	,a.IsChassis
	,a.IsCbu
	,a.SMCModelCode
	,a.GroupCode
	,a.TypeCode
	,a.CylinderCapacity
	,a.fuel
	,a.ModelPrincipal
	,a.Specification
	,a.ModelLine
	,a.Status
	,a.CreatedBy
	,a.CreatedDate
	,a.LastUpdateBy
	,a.LastUpdateDate
	,a.IsLocked
	,a.LockedBy
	,a.LockedDate
	,a.MarketModelCode
	,a.GroupMarketModel
	,a.ColumnMarketModel
FROM omMstModel a
INNER JOIN OmTrSalesDODetail b
ON b.CompanyCode = a.CompanyCode
AND b.SalesModelCode = a.SalesModelCode
WHERE a.CompanyCode = @CompanyCode
AND b.BranchCode = @BranchCode
AND b.DONo = @DONo
AND b.StatusBPK = '0'
ORDER BY a.SalesModelCode ASC
end    
GO
