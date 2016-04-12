CREATE FUNCTION [dbo].[IsIndependentDealer] () 
RETURNS BIT
AS 
BEGIN
	declare @isIndependent bit
	declare @cekMapping int
	declare @DbName varchar(30)
	declare @DbMD varchar(30)

	set @isIndependent = 1
	set @cekMapping = isnull((select count(*) from gnMstCompanyMapping), 0) 

	if(@cekMapping > 0) begin
	    select top 1 @DbName=DbName, @DbMD=DbMD from gnMstCompanyMapping

		--set @DbName = (select top 1 DbName from gnMstCompanyMapping)
		--set @DbMD = (select top 1 DbMD from gnMstCompanyMapping)

		--if(@DbMD IS NULL OR @DbMD = '') begin
		--	set @DbMD = @DbName
		--end

		if(isnull(@DbName,'') <> isnull(@DbMD,'')) begin
			set @isIndependent = 0
		end
	end

	return @isIndependent
END