if object_id('uspfn_SelectChassis') is not null
	drop procedure uspfn_SelectChassis

go
create procedure uspfn_SelectChassis
	@CompanyCode varchar(25),
	@BranchCode varchar(25),
	@SalesModelCode varchar(25),
	@SalesModelYear int,
	@ChassisCode varchar(25),
	@ColourCode varchar(25),
	@WarehouseCode varchar(25)
as

begin
   SELECT ltrim(convert(varchar,a.ChassisNo) + '') ChassisNo
        , a.EngineCode, ltrim(convert(varchar, a.EngineNo) + '') EngineNo
		, a.ServiceBookNo
		, a.KeyNo
	 FROM omMstVehicle a
	INNER JOIN (
		  	SELECT TOP 1 * 
		  	 FROM omTrInventQtyVehicle a
		  	 WHERE a.CompanyCode = @CompanyCode 
		  		   AND a.BranchCode = @BranchCode
		  		   AND a.SalesModelCode = @SalesModelCode
		  		   AND a.SalesModelYear = @SalesModelYear 
		  		   AND a.ColourCode = @ColourCode
		  		   AND a.WarehouseCode = @WareHouseCode
		  	ORDER BY a.[Year] DESC, a.[Month] DESC
		  ) b on a.CompanyCode=b.CompanyCode 
	  AND a.SalesModelCode=b.SalesModelCode AND a.SalesModelYear=b.SalesModelYear 
	  AND a.ColourCode=b.ColourCode AND a.WarehouseCode=b.WarehouseCode
	WHERE a.CompanyCode = @CompanyCode 
	  AND b.BranchCode=@BranchCode
	  AND a.Status = '0'
	  AND a.IsActive = '1'
	  AND a.SalesModelCode = @SalesModelCode
	  AND a.SalesModelYear = @SalesModelYear 
	  AND a.ChassisCode = @ChassisCode
	  AND a.ColourCode = @ColourCode
	  AND a.WareHouseCode = @WareHouseCode
	  AND b.EndingAV > 0
	  and not exists (
			select 1 from omTrSalesSOVin x 
				left join omTrSalesSO y on x.CompanyCode=y.CompanyCode and x.BranchCode=y.BranchCode
					and x.SONo=y.SONo
			where y.Status in ('0','1') and x.ChassisCode=a.ChassisCode and x.ChassisNo=a.ChassisNo
		  )
	ORDER BY a.ChassisCode ASC
end