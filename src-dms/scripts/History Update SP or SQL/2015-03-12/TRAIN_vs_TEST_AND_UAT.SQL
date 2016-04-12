if object_id('uspfn_SelectAccOther') is not null
	drop procedure uspfn_SelectAccOther
GO
CREATE procedure [dbo].[uspfn_SelectAccOther]
	@CompanyCode varchar(25),
	@Reff varchar(25)
as
begin
	SELECT a.RefferenceCode
	     , a.RefferenceDesc1 RefferenceDesc1
      FROM dbo.omMstRefference a
     WHERE a.CompanyCode = @CompanyCode
       AND a.RefferenceType = @Reff
       AND a.Status != '0'
end
GO

if object_id('uspfn_OmTrSalesSoVinSend') is not null
	drop procedure uspfn_OmTrSalesSoVinSend
GO
CREATE procedure [dbo].[uspfn_OmTrSalesSoVinSend]   
 @LastUpdateDate datetime,  
 @Segment int  
as  
  
select * into #t1 from (  
select top (@Segment) *
  from omTrSalesSOVin  
 where LastUpdateDate is not null  
   and LastUpdateDate > @LastUpdateDate  
 order by LastUpdateDate asc )#t1  
  
select * from #t1  
  
  drop table #t1  
GO

if object_id('sp_updateOmTrInventQtyVehicleSO') is not null
	drop procedure sp_updateOmTrInventQtyVehicleSO
GO
create procedure [dbo].[sp_updateOmTrInventQtyVehicleSO]
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
		declare @Month int;
		declare @Alocation int, @EndingOH int, @EndingAV int;
		
		set @Month = (select FiscalPeriod from '+@dbMD+'.dbo.gnMstCoProfileSales where companycode= '''+@companyMD+''' and BranchCode='''+@branchMD+''')
		
		if exists (select * from '+@dbMD+'.dbo.OmTrInventQtyVehicle 
		where companycode='''+@companyMD+''' and BranchCode='''+@branchMD+''' and SalesModelCode = '''+@SalesModelCode+''' and SalesModelYear='+@SalesModelYear+' and ColourCode = '''+@ColourCode+''' and WarehouseCode = '''+@WarehouseCode+''' )
	begin
		set @Alocation = (select Alocation + '+@Quantity+' from '+@dbMD+'.dbo.OmTrInventQtyVehicle
			where companycode='''+@companyMD+''' and BranchCode='''+@branchMD+''' and SalesModelCode = '''+@SalesModelCode+''' and SalesModelYear='+@SalesModelYear+' and ColourCode = '''+@ColourCode+''' and WarehouseCode = '''+@WarehouseCode+''' and Month =@Month )
        set @EndingOH = (select BeginningOH + QtyIn - QtyOut from '+@dbMD+'.dbo.OmTrInventQtyVehicle
			where companycode='''+@companyMD+''' and BranchCode='''+@branchMD+''' and SalesModelCode = '''+@SalesModelCode+''' and SalesModelYear='+@SalesModelYear+' and ColourCode = '''+@ColourCode+''' and WarehouseCode = '''+@WarehouseCode+''' and Month =@Month )
        set @EndingAV = (select BeginningAV + QtyIn - Alocation - QtyOut from '+@dbMD+'.dbo.OmTrInventQtyVehicle
			where companycode='''+@companyMD+''' and BranchCode='''+@branchMD+''' and SalesModelCode = '''+@SalesModelCode+''' and SalesModelYear='+@SalesModelYear+' and ColourCode = '''+@ColourCode+''' and WarehouseCode = '''+@WarehouseCode+''' and Month =@Month )
		
		if (@EndingAV < 0) select 1 as Status
		else begin
			update '+@dbMD+'.dbo.OmTrInventQtyVehicle
			set LastUpdateBy = '''+@userId+''', LastUpdateDate =getdate(), Alocation = @Alocation, EndingAV =@EndingAV, EndingOH = @EndingOH
			where companycode='''+@companyMD+''' and BranchCode='''+@branchMD+''' and SalesModelCode = '''+@SalesModelCode+''' and SalesModelYear='+@SalesModelYear+' and ColourCode = '''+@ColourCode+''' and WarehouseCode = '''+@WarehouseCode+''' and Month =@Month

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



if object_id('sp_updateOmMstVehicleSO') is not null
	drop procedure sp_updateOmMstVehicleSO
GO
create procedure [dbo].[sp_updateOmMstVehicleSO]
	@companyCode varchar(25),
	@BranchCode varchar(25),
	@ChassisCode varchar(25),
	@ChassisNo varchar(25),
	@SONO varchar(25),
	@userId varchar(25)
as

begin
	declare @CountSOVin int;
	declare @CountSOModel int;
	declare @dbMD varchar(25), @sqlStr varchar(max);
	
	--set @Month = (select FiscalPeriod from gnMstCoProfileSales where companycode=@companyCode and BranchCode=@BranchCode)
	set @dbMD =(select dbMD from gnMstCompanyMapping where CompanyCode=@companyCode and BranchCode=@BranchCode)
	
	set @sqlStr = '
		if exists (select * from '+ @dbMD +'.dbo.omMstVehicle where ChassisCode = '''+@ChassisCode+''' and ChassisNo = '''+@ChassisNo+''')
		begin
			update '+ @dbMD +'.dbo.omMstVehicle
			set status=3, SONo = '''+@SONO+''', LastUpdateBy = '''+@userId+''', LastUpdateDate =getdate()
			where ChassisCode = '''+@ChassisCode+''' and ChassisNo= '''+@ChassisNo+'''

			select convert(bit, 1) as Status
		end
		else select convert(bit, 0) as Status
	'
	
	--if exists (select * from BAT_UAT.dbo.omMstVehicle where ChassisCode = @ChassisCode and ChassisNo =@ChassisNo)
	--begin
		--update BAT_UAT.dbo.omMstVehicle
		--set status=3, SONo = @SONO, LastUpdateBy = @userId, LastUpdateDate =getdate()
		--where ChassisCode = @ChassisCode and ChassisNo=@ChassisNo

		--select convert(bit, 1) as Status
	--end
	--else select convert(bit, 0) as Status
	--select	@sqlStr
	exec(@sqlStr)
end

GO
