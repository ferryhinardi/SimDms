create procedure uspfn_sp_partinquiry
@CompanyCode varchar(15), @TypeOfGoods varchar(2), @ProductType varchar(2)
AS
begin
	SELECT 
	 Items.PartNo,ItemInfo.PartName,ItemLoc.WarehouseCode
	,ItemLoc.LocationCode
	,(ItemLoc.OnHand - (ItemLoc.AllocationSP + ItemLoc.AllocationSR + ItemLoc.AllocationSL + ItemLoc.ReservedSP + ItemLoc.ReservedSR + ItemLoc.ReservedSL)) AS QtyAvail
	,Items.OnOrder,ItemPrice.RetailPriceInclTax
	,CASE ItemInfo.IsGenuinePart WHEN 1 THEN 'Ya' ELSE 'Tidak' END AS IsGenuinePart
	,ItemInfo.SupplierCode,ItemPrice.RetailPrice
	,Items.ProductType,Items.PartCategory
	,(SELECT LookupValueName 
		FROM gnMstLookupDtl 
	   WHERE CodeID = 'PRCT' AND 
			 LookUpValue = Items.PartCategory AND 
			 CompanyCode = @CompanyCode) AS CategoryName
	,CASE Items.Status WHEN 1 THEN 'Aktif' ELSE 'Tidak' END AS IsActive
	,ItemInfo.OrderUnit
	,Supplier.SupplierName
	,(SELECT LookupValueName 
		FROM gnMstLookupDtl 
	  WHERE CodeID = 'TPGO' AND 
			LookUpValue = Items.TypeOfGoods AND 
			CompanyCode = @CompanyCode) AS TypeOfGoods
	FROM SpMstItems Items
	INNER JOIN SpMstItemInfo ItemInfo   ON Items.CompanyCode  = ItemInfo.CompanyCode                          
							 AND Items.PartNo = ItemInfo.PartNo
	INNER JOIN SpMstItemLoc ItemLoc ON Items.CompanyCode  = ItemLoc.CompanyCode
		AND Items.BranchCode = ItemLoc.BranchCode	
		AND Items.PartNo = ItemLoc.PartNo
	INNER JOIN SpMstItemPrice ItemPrice ON Items.CompanyCode  = ItemPrice.CompanyCode
		AND Items.BranchCode = ItemPrice.BranchCode	
		AND Items.PartNo = ItemPrice.PartNo		 
	LEFT JOIN GnMstSupplier Supplier ON Supplier.CompanyCode  = Items.CompanyCode 
							 AND Supplier.SupplierCode = ItemInfo.SupplierCode
	WHERE Items.CompanyCode = @CompanyCode
	  AND Items.BranchCode  = (select top 1 BranchCode from gnMstOrganizationDtl where CompanyCode = items.CompanyCode and isBranch = '0')    
	  AND Items.TypeOfGoods = @TypeOfGoods
	  AND Items.ProductType = @ProductType
	  AND ItemLoc.WarehouseCode = '00'
	end
GO

create procedure uspfn_sp_partinquiry_onorder
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@PartNo varchar(32)
AS
SELECT 
	ob.POSNo, ob.POSDate, ob.OnOrder, 
	ob.InTransit, ob.Received, ob.WRSNo,
	sp.SupplierName
FROM 
	spTrnPOrderBalance ob
	INNER JOIN gnMstSupplier sp ON sp.SupplierCode = ob.SupplierCode 
	AND sp.CompanyCode = ob.CompanyCode
WHERE 
    ob.CompanyCode    = @CompanyCode
    AND ob.BranchCode = @BranchCode
    AND ob.PartNo     = @PartNo
GO

create procedure uspfn_sp_partinquiry_demandandsales
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@PartNo varchar(32)
AS
SELECT 
	a.Year
	, Month = 
	case a.Month 
	  when 1 then 'Jan'
	  when 2 then 'Feb'
	  when 3 then 'Mar'
	  when 4 then 'Apr'
	  when 5 then 'Mei'
	  when 6 then 'Jun'
	  when 7 then 'Jul'
	  when 8 then 'Ags'
	  when 9 then 'Sep'
	  when 10 then 'Okt'
	  when 11 then 'Nov'
	  else 'Des'
	end
	,a.DemandFreq
	,a.DemandQty
	,a.SalesFreq
	,a.SalesQty
FROM spHstDemandItem a
WHERE a.CompanyCode = @CompanyCode
  AND a.BranchCode = @BranchCode
  AND a.PartNo = @PartNo
Order By a.Year DESC, a.Month DESC
GO

create procedure uspfn_sp_partinquiry_subsitusi
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@PartNo varchar(32),
	@TypeOfGoods varchar(2)
AS

CREATE TABLE #TmpSubsitusi(
	[No] int,
	PartNo varchar(32),
	PartName varchar(122),
	InterchangeCode char(2),
	UnitConversion int,
	OnHand numeric(12,2),
	AllocationSP numeric(12,2),
	OnOrder numeric(12,2),
	InTransit numeric(12,2),
	Received numeric(12,2),
	isRegister varchar(22)
);

declare @interchangecode char(2), @newPartNo varchar(32), @partName varchar(200)
declare @onhand numeric(18,2), @alokasi numeric(18,2), @register varchar(22), @nomor int
declare @onorder numeric(18,2), @intransit numeric(18,2), @received numeric(18,2)

set @nomor = 1

declare c_subsitusi_cursor CURSOR LOCAL 
FOR
	SELECT ID,interchangecode FROM GetSUGGORModifikasi(@PartNo)
	where ID <> @PartNo

OPEN c_subsitusi_cursor

FETCH NEXT FROM c_subsitusi_cursor
INTO @newPartNo, @interchangecode  	

WHILE @@FETCH_STATUS = 0
BEGIN

	-- initialize default value
	select @onhand=0, @alokasi=0, @register = 'Not Register', @partName='', @onorder=0, @intransit=0, @received=0

	-- check registered part
	IF EXISTS(select onhand from spMstItems where partno=@newPartNo and CompanyCode=@CompanyCode and BranchCode=@BranchCode)
	begin
		select 
			@onhand= isnull(onhand,0), 
			@alokasi= isnull((AllocationSP + AllocationSL + AllocationSR),0),
			@register = 'Register'  
		from 
			spMstItems 
		where 
			partno=@newPartNo and 
			CompanyCode=@CompanyCode and 
			BranchCode=@BranchCode
	end

	-- get partname
	select @partName=partname from spMstItemInfo  
	where partno=@newPartNo and CompanyCode=@CompanyCode

	SELECT 
		@onorder = sum(a.OnOrder), 
		@intransit=sum(a.InTransit), 
		@received=sum(a.Received)
	FROM spTrnPOrderBalance a 
	WHERE a.CompanyCode=@CompanyCode
		AND a.BranchCode=@BranchCode
		AND a.PartNo=@newPartNo
		AND a.TypeOfGoods=@TypeOfGoods
	GROUP BY a.PartNo

	INSERT INTO  #TmpSubsitusi values (@nomor, @newPartNo, @partName, @interchangecode,1,@onhand,@alokasi,@onorder, @intransit, @received, @register);
	SET @nomor = @nomor + 1

	FETCH NEXT FROM c_subsitusi_cursor INTO @newPartNo, @interchangecode  

END

CLOSE c_subsitusi_cursor
DEALLOCATE c_subsitusi_cursor

select * from #TmpSubsitusi

GO

create procedure uspfn_sp_partinquiry_location
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@PartNo varchar(32)
AS
SELECT 
	spMstItemLoc.WarehouseCode,
	gnMstLookUpDtl.LookUpValueName,
	spMstItemLoc.LocationCode,
	spMstItemLoc.OnHand,
	spMstItemLoc.AllocationSP,
	spMstItemLoc.AllocationSL,
	spMstItemLoc.AllocationSR,
	CASE spMstItemLoc.WarehouseCode WHEN '00' THEN spMstItems.OnOrder ELSE 0 END AS OnOrder,
	CASE spMstItemLoc.WarehouseCode WHEN '00' THEN spMstItems.InTransit ELSE 0 END AS InTransit,
	CASE spMstItemLoc.WarehouseCode WHEN '00' THEN spMstItems.BorrowQty ELSE 0 END AS BorrowQty,
	CASE spMstItemLoc.WarehouseCode WHEN '00' THEN spMstItems.BorrowedQty ELSE 0 END AS BorrowedQty,
	spMstItemLoc.BackOrderSP,
	spMstItemLoc.BackOrderSL,
	spMstItemLoc.BackOrderSR,
	spMstItemLoc.ReservedSP,
	spMstItemLoc.ReservedSL,
	spMstItemLoc.ReservedSR
FROM spMstItemLoc
INNER JOIN spMstItems ON spMstItems.PartNo = spMstItemLoc.PartNo AND
    spMstItems.CompanyCode = spMstItemLoc.CompanyCode AND
    spMstItems.BranchCode = spMstItemLoc.BranchCode
INNER JOIN gnMstLookUpDtl ON gnMstLookUpDtl.LookUpValue = spMstItemLoc.WarehouseCode AND
    gnMstLookUpDtl.CompanyCode = spMstItemLoc.CompanyCode    
WHERE spMstItemLoc.CompanyCode=@CompanyCode
AND spMstItemLoc.BranchCode= @BranchCode
AND gnMstLookUpDtl.CodeID ='WRCD'
AND spMstItems.PartNo = @PartNo
AND gnMstLookUpDtl.LookUpValue NOT LIKE 'X%'
GO

create procedure uspfn_sp_partinquiry_demandandsaleshq
	@CompanyCode varchar(15),
	@PartNo varchar(32)
AS
SELECT a.BranchCode,
 a.Year
,Month = 
case a.Month 
  when 1 then 'Jan'
  when 2 then 'Feb'
  when 3 then 'Mar'
  when 4 then 'Apr'
  when 5 then 'Mei'
  when 6 then 'Jun'
  when 7 then 'Jul'
  when 8 then 'Ags'
  when 9 then 'Sep'
  when 10 then 'Okt'
  when 11 then 'Nov'
  else 'Des'
end
,a.DemandFreq
,a.DemandQty
,a.SalesFreq
,a.SalesQty
FROM spHstDemandItem a
WHERE a.CompanyCode = @CompanyCode  AND a.PartNo = @PartNo
GO

create procedure uspfn_sp_partinquiry_locationhq
	@CompanyCode varchar(15),
	@PartNo varchar(32)
AS
SELECT 
	spMstItemLoc.BranchCode,
	spMstItemLoc.WarehouseCode,
	gnMstLookUpDtl.LookUpValueName,
	spMstItemLoc.LocationCode,
	spMstItemLoc.OnHand,
	spMstItemLoc.AllocationSP,
	spMstItemLoc.AllocationSL,
	spMstItemLoc.AllocationSR,
	CASE spMstItemLoc.WarehouseCode WHEN '00' THEN spMstItems.OnOrder ELSE 0 END AS OnOrder,
	CASE spMstItemLoc.WarehouseCode WHEN '00' THEN spMstItems.InTransit ELSE 0 END AS InTransit,
	CASE spMstItemLoc.WarehouseCode WHEN '00' THEN spMstItems.BorrowQty ELSE 0 END AS BorrowQty,
	CASE spMstItemLoc.WarehouseCode WHEN '00' THEN spMstItems.BorrowedQty ELSE 0 END AS BorrowedQty,
	spMstItemLoc.BackOrderSP,
	spMstItemLoc.BackOrderSL,
	spMstItemLoc.BackOrderSR,
	spMstItemLoc.ReservedSP,
	spMstItemLoc.ReservedSL,
	spMstItemLoc.ReservedSR
FROM spMstItemLoc
INNER JOIN spMstItems ON spMstItems.PartNo = spMstItemLoc.PartNo AND
    spMstItems.CompanyCode = spMstItemLoc.CompanyCode AND
    spMstItems.BranchCode = spMstItemLoc.BranchCode
INNER JOIN gnMstLookUpDtl ON gnMstLookUpDtl.LookUpValue = spMstItemLoc.WarehouseCode AND
    gnMstLookUpDtl.CompanyCode = spMstItemLoc.CompanyCode    
WHERE spMstItemLoc.CompanyCode=@CompanyCode
AND gnMstLookUpDtl.CodeID ='WRCD'
AND spMstItems.PartNo = @PartNo
AND gnMstLookUpDtl.LookUpValue NOT LIKE 'X%'
GO
