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


