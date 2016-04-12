/****** Object:  StoredProcedure [dbo].[uspfn_GetCostPrice]    Script Date: 6/26/2015 8:24:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		SDMS.RUDIANA
-- Create date: 2015-06-05
-- Description:	Get CostPrice Value
-- =============================================
ALTER PROCEDURE [dbo].[uspfn_GetCostPrice] (@CompanyCode varchar(15), @BranchCode varchar(15), 
	@PartNo varchar(20), @CostPrice numeric(18,2) output)
AS
BEGIN
	DECLARE @Discount NUMERIC(18,2)
	DECLARE @PurcDiscPct numeric(18,2)
	DECLARE @DiscPct numeric(18,2)

	set @Discount = 0;
    set @PurcDiscPct = (SELECT PurcDiscPct FROM spMstItems WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PartNo = @PartNo)
	
	DECLARE @TPGO VARCHAR(5)
	set @TPGO = (SELECT TypeOfGoods FROM spMstItems WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PartNo = @PartNo)

	declare @sql nvarchar(512);

	IF(@TPGO = 2 or @TPGO = 5)
	BEGIN
	IF(@TPGO = 2)
	BEGIN
	set @sql = 'select @CostPrice = (b.RetailPrice - (b.RetailPrice * (' + convert(varchar, 25) + ' *  0.01)))
		from ' + dbo.GetDbMD(@CompanyCode, @BranchCode) + '..spMstItemPrice b 
		where b.CompanyCode =''' + dbo.GetCompanyMD(@CompanyCode, @BranchCode) + '''
		and b.BranchCode =''' + dbo.GetBranchMD(@CompanyCode, @BranchCode) + '''
		and b.PartNo =''' + @PartNo + ''''
	END
	ELSE
	BEGIN
	set @sql = 'select @CostPrice = b.RetailPrice
		from ' + dbo.GetDbMD(@CompanyCode, @BranchCode) + '..spMstItemPrice b 
		where b.CompanyCode =''' + dbo.GetCompanyMD(@CompanyCode, @BranchCode) + '''
		and b.BranchCode =''' + dbo.GetBranchMD(@CompanyCode, @BranchCode) + '''
		and b.PartNo =''' + @PartNo + ''''
	END
	END
	ELSE
	BEGIN
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
	set @sql = 'select @CostPrice = (b.RetailPrice - (b.RetailPrice * (' + convert(varchar, @Discount) + ' *  0.01)))
		from ' + dbo.GetDbMD(@CompanyCode, @BranchCode) + '..spMstItemPrice b 
		where b.CompanyCode =''' + dbo.GetCompanyMD(@CompanyCode, @BranchCode) + '''
		and b.BranchCode =''' + dbo.GetBranchMD(@CompanyCode, @BranchCode) + '''
		and b.PartNo =''' + @PartNo + ''''


		--print @sql
		END

	execute sp_executesql @sql, N'@CostPrice numeric(18,2) OUTPUT', @CostPrice = @CostPrice OUTPUT

END