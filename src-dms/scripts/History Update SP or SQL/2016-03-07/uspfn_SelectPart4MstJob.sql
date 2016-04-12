create procedure uspfn_SelectPart4MstJob
@CompanyCode as varchar(15),
@BranchCode as varchar(15),
@ProductType as varchar(2),
@BasicModel as varchar(20),
@DynamicFilter varchar(4000) = '',
@top int = 100
		
--declare @CompanyCode as varchar(15) = '6006408',
--		@BranchCode as varchar(15) = '6006431',
--		@ProductType as varchar(2) = '4W',
--		@BasicModel as varchar(20) = '',
--		@DynamicFilter varchar(4000) = '',
--		@top int = 100

as
begin
	
DECLARE @Query varchar(max);
set @Query = 
'select TOP ' + CONVERT(VARCHAR, @top) + ' itemInfo.PartNo
 , (item.OnHand - (item.AllocationSP + item.AllocationSR + item.AllocationSL) 
 - (item.ReservedSP + item.ReservedSR + item.ReservedSL)) as Available
 , itemPrice.RetailPriceInclTax, itemInfo.PartName, item.TypeOfGoods
 , case gtgo.ParaValue when ''SPAREPART'' then ''SPAREPART'' else ''MATERIAL'' end GroupTypeOfGoods
 , (case itemInfo.Status when 1 then ''Aktif'' else ''Tidak Aktif'' end) as Status
 , itemPrice.RetailPrice as NilaiPart
from spMstItemInfo itemInfo 
left join spMstItems item on item.CompanyCode = itemInfo.CompanyCode 
and item.BranchCode = ''' + @BranchCode + '''
and item.ProductType = ''' + @ProductType + '''
and item.PartNo = itemInfo.PartNo
inner join spMstItemPrice itemPrice on itemPrice.CompanyCode = itemInfo.CompanyCode 
and itemPrice.BranchCode = ''' + @BranchCode + '''
and itemPrice.PartNo = item.PartNo
left join spMstItemLoc ItemLoc on itemInfo.CompanyCode = ItemLoc.CompanyCode 
and ItemLoc.BranchCode = ''' + @BranchCode + '''
and itemInfo.PartNo = ItemLoc.PartNo
and ItemLoc.WarehouseCode = ''00''
left join gnMstLookupDtl gtgo
on gtgo.CompanyCode = item.CompanyCode
and gtgo.CodeID = ''GTGO''
and gtgo.LookupValue = item.TypeOfGoods
where itemInfo.CompanyCode = ''' + @CompanyCode + '''
and itemInfo.ProductType = ''' + @ProductType + '''
and itemLoc.partno is not null '

if exists (select * from svMstRefferenceService where RefferenceType = 'LOCK_BASMODEL' and RefferenceCode = @BasicModel)
set @Query = @Query +
'and itemInfo.PartNo in
(select * from dbo.Split((select description from svMstRefferenceService where RefferenceType = ''LOCK_BASMODEL'' and RefferenceCode = ''' + @BasicModel + '''),'',''))'
 + @DynamicFilter
else set @Query = @Query + @DynamicFilter
 
 exec (@Query)
 
 end
