if object_id('uspfn_SvInsertDefaultTaskMovement') is not null
	drop procedure uspfn_SvInsertDefaultTaskMovement
GO
CREATE procedure [uspfn_SvInsertDefaultTaskMovement]
--declare 
@CompanyCode as varchar(15),
@BranchCode as varchar(15),
@ProductType as varchar(15),
@ServiceNo as int,
@UserID as varchar(100)
as

--set @CompanyCode = '6006400001'
--set	@BranchCode = '6006400101'
--set	@ProductType = '4W'
--set	@ServiceNo = 1
--set	@UserID = 'yo'

declare @Sql as varchar(max)

set @Sql = 
'insert into '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..svSDMovement
select a.CompanyCode, a.BranchCode, b.JobOrderNo, b.JobOrderDate, a.PartNo, a.PartSeq, dbo.GetWarehouseMD(a.CompanyCode,a.BranchCode) WarehouseCode, 
a.DemandQty, a.DemandQty, a.DiscPct, a.CostPrice, a.RetailPrice, a.TypeOfGoods, dbo.GetCompanyMD(a.CompanyCode,a.BranchCode) CompanyMD, 
dbo.GetBranchMD(a.CompanyCode,a.BranchCode) BranchMD, dbo.GetWarehouseMD(a.CompanyCode,a.BranchCode) WarehouseMD, c.RetailPriceInclTax, c.RetailPrice,
c.CostPrice, ''x'', '''+@ProductType+''', ''300'', ''0'', ''0'', ''1900/01/01'', '''+@UserID+''', GETDATE(), '''+@UserID+''', GETDATE()  
from 
svTrnSrvItem a 
inner join SvTrnService b on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.ServiceNo = b.ServiceNo
left join '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..spMstItemPrice c ON c.CompanyCode = dbo.GetCompanyMD(a.CompanyCode,a.BranchCode) and c.BranchCode = dbo.GetBranchMD(a.CompanyCode,a.BranchCode)
and c.PartNo = a.PartNo
where a.CompanyCode = '''+@CompanyCode+''' and a.BranchCode = '''+@BranchCode+''' and a.ServiceNo = ' + convert(varchar,@ServiceNo,1)
print(@Sql)
exec(@Sql)
go
