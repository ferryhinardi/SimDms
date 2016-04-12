CREATE procedure [dbo].[uspfn_spMstItemsInsertFromMD]
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@PartNo varchar(20),
	@UserID varchar(15)
AS	

BEGIN

--begin tran
--declare @CompanyCode varchar(15)
--declare @BranchCode varchar(15)
--declare @PartNo varchar(20)
--declare @MovingCode varchar(15)
--declare @UserID varchar(15)

--set @CompanyCode = '6159401000'
--set @BranchCode = '6159401001'
--set @PartNo = '990H0-990AX-009'
--set @MovingCode = '0'
--set @UserID = 'ga'
declare @sql NVARCHAR(max)

--==================================================================================================================================
-- Chek PartNo di table spMstItems MD
--==================================================================================================================================
declare @xPartNo varchar(15)		
set @sql = 'select @xPartNo = PartNo from ' 
	+ dbo.GetDbMD(@CompanyCode, @BranchCode) + '..spMstItems '
	+ ' where CompanyCode = ''' + dbo.GetCompanyMD(@CompanyCode, @BranchCode) + ''' 
		and BranchCode = ''' + dbo.GetBranchMD(@CompanyCode, @BranchCode) + '''
		and PartNo = ''' + @PartNo + '''
		and MovingCode = ''0'''
execute sp_executesql @sql, N'@xPartNo varchar(15) OUTPUT', @xPartNo = @xPartNo OUTPUT 
--select @xPartNo

if (select @xPartNo) is not null begin
	--==================================================================================================================================
	-- INSERT spMstItems
	--==================================================================================================================================
	if not exists (select PartNo from spMstItems  
		where CompanyCode = @CompanyCode 
		and BranchCode = @BranchCode
		and PartNo = @PartNo
		and MovingCode = 0) begin
		
		set @sql = 'select * into #temp from ' 
		+ dbo.GetDbMD(@CompanyCode, @BranchCode) + '..spMstItems '
		+ ' where CompanyCode = ''' + dbo.GetCompanyMD(@CompanyCode, @BranchCode) + ''' 
			and BranchCode = ''' + dbo.GetBranchMD(@CompanyCode, @BranchCode) + '''
			and PartNo = ''' + @PartNo + '''
			and MovingCode = ''0'''
		+ CHAR(13) + CHAR(13) +		
		
		'insert into spMstItems(
			CompanyCode,BranchCode,PartNo,MovingCode,DemandAverage,BornDate,ABCClass,LastDemandDate,LastPurchaseDate,LastSalesDate
			,BOMInvAmt,BOMInvQty,BOMInvCostPrice,OnOrder,InTransit,OnHand,AllocationSP,AllocationSR,AllocationSL,BackOrderSP
			,BackOrderSR,BackOrderSL,ReservedSP,ReservedSR,ReservedSL,BorrowQty,BorrowedQty,SalesUnit,OrderUnit,OrderPointQty
			,SafetyStockQty,LeadTime,OrderCycle,SafetyStock,Utility1,Utility2,Utility3,Utility4,TypeOfGoods,Status
			,ProductType,PartCategory,CreatedBy,CreatedDate,LastUpdateBy,LastUpdateDate,isLocked,LockingBy,LockingDate,PurcDiscPct
		)
		values(
			''' + @CompanyCode + ''',''' + @BranchCode + ''',''' + @PartNo + ''',''0'',0,GetDate(),(select ABCClass from #temp),
			GetDate(),GetDate(),GetDate()
			,0,0,0,0,0,0,0,0,0,0
			,0,0,0,0,0,0,0,(select isnull(SalesUnit,0) from #temp),(select isnull(OrderUnit,0) from #temp),0
			,(select isnull(SafetyStockQty,0) from #temp),(select isnull(LeadTime,0) from #temp),(select isnull(OrderCycle,0) from #temp),(select isnull(SafetyStock,0) from #temp)
			,(select isnull(Utility1,'''') from #temp),(select isnull(Utility2,'''') from #temp),(select isnull(Utility3,'''') from #temp),(select isnull(Utility4,'''') from #temp)
			,(select isnull(TypeOfGoods,'''') from #temp),1
			,(select isnull(ProductType,'''') from #temp),(select isnull(PartCategory,'''') from #temp),''' + @UserID + ''',GetDate(),''' + @UserID + ''',GetDate(),0,'''',GetDate()
			,(select isnull(PurcDiscPct,0) from #temp)
		)'
		
		+ CHAR(13) + 
		'drop table #temp;'
		
		--print (@sql)
		exec (@sql)
	end	

	--==================================================================================================================================
	-- INSERT spMstItemLoc
	--==================================================================================================================================
	if not exists (select PartNo from spMstItemLoc
		where CompanyCode = @CompanyCode 
		and BranchCode = @BranchCode
		and PartNo = @PartNo
		and WarehouseCode = '00') begin
		
		insert into spMstItemLoc(
			CompanyCode,BranchCode,PartNo,WarehouseCode,LocationCode,LocationSub1,LocationSub2,LocationSub3,LocationSub4,LocationSub5
			,LocationSub6,BOMInvAmount,BOMInvQty,BOMInvCostPrice,OnHand,AllocationSP,AllocationSR,AllocationSL,BackOrderSP,BackOrderSR
			,BackOrderSL,ReservedSP,ReservedSR,ReservedSL,Status,CreatedBy,CreatedDate,LastUpdateBy,LastUpdateDate,isLocked
			,LockingBy,LockingDate
		)
		values(
			@CompanyCode,@BranchCode,@PartNo,'00',' - ',' - ',' - ',' - ',' - ',' - '
			,' - ',0,0,0,0,0,0,0,0,0
			,0,0,0,0,'1',@UserID,GetDate(),@UserID,GetDate(),0
			,@UserID,GetDate()
		)
	end

	--==================================================================================================================================
	-- INSERT spMstItemPrice
	--==================================================================================================================================
	if not exists (select PartNo from spMstItemPrice
		where CompanyCode = @CompanyCode 
		and BranchCode = @BranchCode
		and PartNo = @PartNo) begin

		set @sql = 'select * into #temp from ' 
			+ dbo.GetDbMD(@CompanyCode, @BranchCode) + '..spMstItemPrice '
			+ ' where CompanyCode = ''' + dbo.GetCompanyMD(@CompanyCode, @BranchCode) + ''' 
				and BranchCode = ''' + dbo.GetBranchMD(@CompanyCode, @BranchCode) + '''
				and PartNo = ''' + @PartNo + ''''
			+ CHAR(13) + CHAR(13) +		
			
		'insert into spMstItemPrice(
			CompanyCode,BranchCode,PartNo,RetailPrice,RetailPriceInclTax,PurchasePrice,CostPrice,OldRetailPrice,OldPurchasePrice,OldCostPrice
			,LastPurchaseUpdate,LastRetailPriceUpdate,CreatedBy,CreatedDate,LastUpdateBy,LastUpdateDate,isLocked,LockingBy,LockingDate
		)
		values(
			''' + @CompanyCode + ''',''' + @BranchCode + ''',''' + @PartNo + ''',(select isnull(RetailPrice,0) from #temp),(select isnull(RetailPriceInclTax,0) from #temp)
			,(select isnull(PurchasePrice,0) from #temp),(select isnull(CostPrice,0) from #temp),0,0,0
			,GetDate(),GetDate(),''' + @UserID + ''',GetDate(),''' + @UserID + ''',GetDate(),0,'''',GetDate() 
		)'
		
		+ CHAR(13) + CHAR(13) +	
		'drop table #temp';
		
		--print (@sql)
		exec (@sql);
	end
end

--rollback tran

END