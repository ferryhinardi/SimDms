IF object_id('uspfn_spInsertLiveStockDealerSales') IS NOT NULL DROP PROCEDURE uspfn_spInsertLiveStockDealerSales
Go
CREATE PROCEDURE [dbo].[uspfn_spInsertLiveStockDealerSales]
	@CompanyCode Varchar(15),
	@Type varchar(50),
	@Variant varchar(40),
	@IsVisible bit,
	@uid varchar(25)
AS
BEGIN
	IF NOT EXISTS(SELECT NULL FROM omMstLiveStockDealer WHERE CompanyCode=@CompanyCode AND Type = @Type AND Variant = @Variant)
	BEGIN
		INSERT INTO omMstLiveStockDealer (
			CompanyCode,
			Type,
			Variant,
			IsVisible,
			CreatedBy,
			CreatedDate,
			LastupdatedBy,
			LastupdatedDate
		)
		VALUES (
			@CompanyCode,
			@Type,
			@Variant,
			@IsVisible,
			@uid,
			GETDATE(),
			@uid, 
			GETDATE()
		)
	END
	ELSE
	BEGIN
		IF (
			(SELECT IsVisible FROM omMstLiveStockDealer 
			WHERE CompanyCode = @CompanyCode AND Type = @Type
			AND Variant = @Variant) <> @IsVisible
		)
		BEGIN
			UPDATE omMstLiveStockDealer
			SET IsVisible = @IsVisible, LastupdatedBy = @uid, LastupdatedDate = GETDATE()
			WHERE CompanyCode = @CompanyCode AND Type = @Type
			AND Variant = @Variant
		END
	END
END