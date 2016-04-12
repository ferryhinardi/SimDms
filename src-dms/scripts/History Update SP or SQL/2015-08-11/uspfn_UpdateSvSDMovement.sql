IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[uspfn_UpdateSvSDMovement]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[uspfn_UpdateSvSDMovement]
GO

create procedure uspfn_UpdateSvSDMovement
--declare 
@CompanyCode as varchar(15),
@BranchCode as varchar(15),
@DocNo  as varchar(15)
 		
--select @CompanyCode = '6006400001', @BranchCode = '6006400102', @DocNo = 'PLS/14/102948'
as
begin

declare @SQL as varchar(max)
set @SQL = 
'select * into #t1 from(
select CompanyCode, BranchCode, DocNo, 
(select top 1 DocDate from '+ dbo.GetDbMD(@CompanyCode, @BranchCode) + '..svSDMovement where CompanyCode = '+''''+ @CompanyCode +''''+' and BranchCode = '+''''+ @BranchCode+''''+' and DocNo = '+''''+@DocNo+''''+') DocDate, 
PartNo, 1 PartSeq, WarehouseCode, SUM(QtyOrder) QtyOrder, SUM(Qty) Qty, DiscPct, CostPrice, RetailPrice, TypeOfGoods, CompanyMD, BranchMD, 
WarehouseMD, RetailPriceInclTaxMD, RetailPriceMD, CostPriceMD, QtyFlag, ProductType, ProfitCenterCode, Status, ProcessStatus, 
(select top 1 ProcessDate from '+ dbo.GetDbMD(@CompanyCode, @BranchCode) + '..svSDMovement where CompanyCode = '+''''+ @CompanyCode +''''+' and BranchCode = '+''''+ @BranchCode+''''+' and DocNo = '+''''+@DocNo+''''+') ProcessDate, 
(select top 1 CreatedBy from '+ dbo.GetDbMD(@CompanyCode, @BranchCode) + '..svSDMovement where CompanyCode = '+''''+ @CompanyCode +''''+' and BranchCode = '+''''+ @BranchCode+''''+' and DocNo = '+''''+@DocNo+''''+') CreatedBy, 
(select top 1 CreatedDate from '+ dbo.GetDbMD(@CompanyCode, @BranchCode) + '..svSDMovement where CompanyCode = '+''''+ @CompanyCode +''''+' and BranchCode = '+''''+ @BranchCode+''''+' and DocNo = '+''''+@DocNo+''''+') CreatedDate, 
(select top 1 LastUpdateBy from '+ dbo.GetDbMD(@CompanyCode, @BranchCode) + '..svSDMovement where CompanyCode = '+''''+ @CompanyCode +''''+' and BranchCode = '+''''+ @BranchCode+''''+' and DocNo = '+''''+@DocNo+''''+') LastUpdateBy, 
(select top 1 LastUpdateDate from '+ dbo.GetDbMD(@CompanyCode, @BranchCode) + '..svSDMovement where CompanyCode = '+''''+ @CompanyCode +''''+' and BranchCode = '+''''+ @BranchCode+''''+' and DocNo = '+''''+@DocNo+''''+') LastUpdateDate
from '+ dbo.GetDbMD(@CompanyCode, @BranchCode) + '..svSDMovement 
where CompanyCode = '+''''+ @CompanyCode +''''+' and BranchCode = '+''''+ @BranchCode+''''+' and DocNo = '+''''+@DocNo+''''+'
group by CompanyCode, BranchCode, DocNo, PartNo, WarehouseCode, DiscPct, CostPrice, RetailPrice, TypeOfGoods, CompanyMD, BranchMD, 
WarehouseMD, RetailPriceInclTaxMD, RetailPriceMD, CostPriceMD, QtyFlag, ProductType, ProfitCenterCode, Status, ProcessStatus
) #t1

declare @CompanyTemp as varchar(20)
declare @BranchTemp as varchar(20)
declare @DocTemp as varchar(20)
declare @PartTemp as varchar(20)
declare @PartSeq as int
set @PartSeq = 0
if (select COUNT(*) from #t1) > 0
begin
declare myCursor cursor for
select CompanyCode, BranchCode, DocNo, PartNo from #t1

open myCursor
fetch next from myCursor into @CompanyTemp, @BranchTemp, @DocTemp, @PartTemp
while @@FETCH_STATUS = 0
begin
delete from '+ dbo.GetDbMD(@CompanyCode, @BranchCode) + '..svSDMovement where CompanyCode = @CompanyTemp and BranchCode = @BranchTemp and DocNo = @DocTemp and PartNo = @PartTemp
set @PartSeq = @PartSeq+1
insert into '+ dbo.GetDbMD(@CompanyCode, @BranchCode) + '..svSDMovement
select CompanyCode, BranchCode, DocNo, DocDate, PartNo, @PartSeq, WarehouseCode, QtyOrder, Qty, DiscPct, CostPrice, RetailPrice, TypeOfGoods, CompanyMD, 
BranchMD, WarehouseMD, RetailPriceInclTaxMD, RetailPriceMD, CostPriceMD, QtyFlag, ProductType, ProfitCenterCode, Status, ProcessStatus, ProcessDate, 
CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate
from #t1 where CompanyCode = @CompanyTemp and BranchCode = @BranchTemp and DocNo = @DocTemp and PartNo = @PartTemp
fetch next from myCursor into @CompanyTemp, @BranchTemp, @DocTemp, @PartTemp
end

close myCursor
deallocate myCursor
end

drop table #t1'

--print(@SQL)
exec(@SQL)

end
GO
