if object_id('sp_updateOmTrInventQtyVehicleSO') is not null
	drop procedure sp_updateOmTrInventQtyVehicleSO
GO
CREATE procedure [dbo].[sp_updateOmTrInventQtyVehicleSO]
	@companyCode varchar(25),
	@BranchCode varchar(25),
	@SalesModelCode varchar(25),
	@SalesModelYear varchar(25),
	@ColourCode varchar(25),
	@WarehouseCode varchar(25),
	@Quantity varchar,
	@UserId varchar(25)
as

begin
	declare @CountSOVin int;
	declare @CountSOModel int;
	--declare @Month int;
	--declare @Alocation int, @EndingOH int, @EndingAV int
	declare @dbMD varchar(25), @sqlStr varchar(max), @companyMD varchar(25),@branchMD varchar(25);
	
	set @dbMD =(select dbMD from gnMstCompanyMapping where CompanyCode= @companyCode and BranchCode=@BranchCode)
	set @companyMD = (select companyMD from gnMstCompanyMapping where CompanyCode= @companyCode and BranchCode=@BranchCode)
	set @branchMD = (select unitBranchMD from gnMstCompanyMapping where CompanyCode= @companyCode and BranchCode=@BranchCode)
	
	--set @Month = (select FiscalPeriod from BAT_UAT.dbo.gnMstCoProfileSales where companycode=@companyCode and BranchCode=@BranchCode)
	
	set @sqlStr = '
		declare @Month int, @Year int;
		declare @Alocation int, @EndingOH int, @EndingAV int;
		
		set @Month = (select Month(PeriodeBeg) from '+@dbMD+'.dbo.gnMstCoProfileSales where companycode= '''+@companyMD+''' and BranchCode='''+@branchMD+''')
		set @Year = (select Year(PeriodeBeg) from '+@dbMD+'.dbo.gnMstCoProfileSales where companycode= '''+@companyMD+''' and BranchCode='''+@branchMD+''')

		if exists (select * from '+@dbMD+'.dbo.OmTrInventQtyVehicle 
		where companycode='''+@companyMD+''' and BranchCode='''+@branchMD+''' and SalesModelCode = '''+@SalesModelCode+''' and SalesModelYear='+@SalesModelYear+' and ColourCode = '''+@ColourCode+''' and WarehouseCode = '''+@WarehouseCode+''' )
	begin
		set @Alocation = (select Alocation + '+@Quantity+' from '+@dbMD+'.dbo.OmTrInventQtyVehicle
			where companycode='''+@companyMD+''' and BranchCode='''+@branchMD+''' and SalesModelCode = '''+@SalesModelCode+''' and SalesModelYear='+@SalesModelYear+' and ColourCode = '''+@ColourCode+''' and WarehouseCode = '''+@WarehouseCode+''' and Month =@Month and Year = @Year )
        set @EndingOH = (select BeginningOH + QtyIn - QtyOut from '+@dbMD+'.dbo.OmTrInventQtyVehicle
			where companycode='''+@companyMD+''' and BranchCode='''+@branchMD+''' and SalesModelCode = '''+@SalesModelCode+''' and SalesModelYear='+@SalesModelYear+' and ColourCode = '''+@ColourCode+''' and WarehouseCode = '''+@WarehouseCode+''' and Month =@Month and Year = @Year)
        set @EndingAV = (select BeginningAV + QtyIn - Alocation - QtyOut from '+@dbMD+'.dbo.OmTrInventQtyVehicle
			where companycode='''+@companyMD+''' and BranchCode='''+@branchMD+''' and SalesModelCode = '''+@SalesModelCode+''' and SalesModelYear='+@SalesModelYear+' and ColourCode = '''+@ColourCode+''' and WarehouseCode = '''+@WarehouseCode+''' and Month =@Month and Year = @Year)
		
		if (@EndingAV < 0) select 1 as Status
		else begin
			update '+@dbMD+'.dbo.OmTrInventQtyVehicle
			set LastUpdateBy = '''+@userId+''', LastUpdateDate =getdate(), Alocation = @Alocation, EndingAV =@EndingAV, EndingOH = @EndingOH
			where companycode='''+@companyMD+''' and BranchCode='''+@branchMD+''' and SalesModelCode = '''+@SalesModelCode+''' and SalesModelYear='+@SalesModelYear+' and ColourCode = '''+@ColourCode+''' and WarehouseCode = '''+@WarehouseCode+''' and Month = @Month and Year = @Year

			select 2 as Status
		end
	end
	else select 0 as Status
	'
	--if exists (select * from BAT_UAT.dbo.OmTrInventQtyVehicle 
		--where companycode=@companyCode and BranchCode=@BranchCode and SalesModelCode = @SalesModelCode and SalesModelYear=@SalesModelYear and ColourCode =@ColourCode and WarehouseCode =@WarehouseCode )
	--begin
		--set @Alocation = (select Alocation + @Quantity from BAT_UAT.dbo.OmTrInventQtyVehicle
			--where companycode=@companyCode and BranchCode=@BranchCode and SalesModelCode = @SalesModelCode 
			--and SalesModelYear=@SalesModelYear and ColourCode =@ColourCode and WarehouseCode =@WarehouseCode and Month =@Month )
        --set @EndingOH = (select BeginningOH + QtyIn - QtyOut from BAT_UAT.dbo.OmTrInventQtyVehicle
			--where companycode=@companyCode and BranchCode=@BranchCode and SalesModelCode = @SalesModelCode 
			--and SalesModelYear=@SalesModelYear and ColourCode =@ColourCode and WarehouseCode =@WarehouseCode and Month =@Month )
        --set @EndingAV = (select BeginningAV + QtyIn - Alocation - QtyOut from BAT_UAT.dbo.OmTrInventQtyVehicle
			--where companycode=@companyCode and BranchCode=@BranchCode and SalesModelCode = @SalesModelCode 
			--and SalesModelYear=@SalesModelYear and ColourCode =@ColourCode and WarehouseCode =@WarehouseCode and Month =@Month )
		
		--if (@EndingAV < 0) select 1 as Status
		--else begin
			--update BAT_UAT.dbo.OmTrInventQtyVehicle
			--set LastUpdateBy = @userId, LastUpdateDate =getdate(), Alocation = @Alocation, EndingAV =@EndingAV, EndingOH = @EndingOH
			--where companycode=@companyCode and BranchCode=@BranchCode and SalesModelCode = @SalesModelCode 
			--and SalesModelYear=@SalesModelYear and ColourCode =@ColourCode and WarehouseCode =@WarehouseCode and Month =@Month

			--select 2 as Status
		--end
	--end
	--else select 0 as Status
--select @sqlStr
exec(@sqlStr)
end


GO


