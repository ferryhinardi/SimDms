
go
if object_id('uspfn_SelectWHDO') is not null
	drop procedure uspfn_SelectWHDO

go
create procedure uspfn_SelectWHDO
	@CompanyCode varchar(13),
	@BranchCode varchar(13),
	@SONumber varchar(25)
as
begin
	SELECT Count(*) QtyDO
	     , a.WarehouseCode
		 , b.SalesModelCode
		 , b.SalesModelYear
		 , b.ColourCode
     FROM omTrSalesDO a
     INNER JOIN omTrSalesDODetail b
	    ON a.CompanyCode = b.CompanyCode
	   AND a.BranchCode = b.BranchCode
	   AND a.DONo = b.DONo
     WHERE a.CompanyCode = @CompanyCode
	   AND a.BranchCode = @BranchCode
	   AND a.SONo = @SONumber
	   AND a.Status = '2'
     Group By a.WarehouseCode, b.SalesModelCode, b.SalesModelYear, b.ColourCode
end