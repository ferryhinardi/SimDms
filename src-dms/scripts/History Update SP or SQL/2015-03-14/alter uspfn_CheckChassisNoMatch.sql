if object_id('uspfn_CheckChassisNoMatch') is not null
	drop procedure uspfn_CheckChassisNoMatch
GO
create procedure [dbo].[uspfn_CheckChassisNoMatch]
	@CompanyCode varchar(25),
	@BranchCode varchar(25),
	@SONumber varchar(50)
as

begin
	declare @CountSOVin int;
	declare @CountSOModel int;

	set @CountSOVin = (
		select count(a.SONo)
		  from omTrSalesSOVin a
		 where a.CompanyCode = @CompanyCode
		   and a.BranchCode = @BranchCode
		   and a.SONo = @SONumber
		   and a.ChassisNo != 0
	);

	set @CountSOModel = (
		select SUM(QuantitySO) 
		  from omTrSalesSOModel a
		 where a.CompanyCode = @CompanyCode
		   and a.BranchCode = @BranchCode
		   and a.SONo = @SONumber
	);

	if @CountSOModel = 0
		select convert(bit, 0) as Status;
	if @CountSOModel = @CountSOVin
		select convert(bit, 1) as Status;
	else
		select convert(bit, 0) as Status
end
GO
