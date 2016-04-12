
ALTER PROCEDURE [dbo].[uspfn_GetCostPrice] (@CompanyCode varchar(15), @BranchCode varchar(15), 
	@PartNo varchar(20), @CostPrice numeric(18,2) output)
AS
BEGIN
	DECLARE @Discount NUMERIC(18,2)
	DECLARE @PurcDiscPct numeric(18,2)
	DECLARE @DiscPct numeric(18,2)

	set @Discount = 0;
    set @PurcDiscPct = (SELECT PurcDiscPct FROM spMstItems WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PartNo = @PartNo)

	if(@PurcDiscPct is not null) begin
		--discount from master items
		SET @Discount = @PurcDiscPct
	end
	else begin
        --discount from master supplier
        set @DiscPct = (select DiscPct from gnMstSupplierProfitCenter where CompanyCode = @CompanyCode and BranchCode = @BranchCode
			and SupplierCode = (select dbo.GetBranchMD(@CompanyCode, @BranchCode)) and ProfitCenterCode = '300')
		
		if(@DiscPct is not null)begin
			if(@DiscPct >= 0) begin
				SET @Discount = @Discount + @DiscPct;
			end
		end 
	end
	
	--declare @xCostPrice numeric(18,2)
	declare @sql nvarchar(512);
	set @sql = 'select @CostPrice = (b.RetailPrice - (b.RetailPrice * (' + convert(varchar, @Discount) + ' *  0.01)))
		from ' + dbo.GetDbMD(@CompanyCode, @BranchCode) + '..spMstItemPrice b 
		where b.CompanyCode =''' + dbo.GetCompanyMD(@CompanyCode, @BranchCode) + '''
		and b.BranchCode =''' + dbo.GetBranchMD(@CompanyCode, @BranchCode) + '''
		and b.PartNo =''' + @PartNo + ''''


		print @sql

	execute sp_executesql @sql, N'@CostPrice numeric(18,2) OUTPUT', @CostPrice = @CostPrice OUTPUT
END
