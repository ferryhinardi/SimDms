USE [SAT_UAT]
GO

/****** Object:  StoredProcedure [dbo].[uspfn_OmInquiryChassisDO]    Script Date: 3/12/2015 9:03:03 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- uspfn_OmInquiryChassisDO '6115202','611520200','SOA/11/000287','FU150 SCD',2011,'MH8BG41CABJ','COLO','00'
CREATE procedure [dbo].[uspfn_OmInquiryChassisDO]
	@CompanyCode varchar(15)
	,@BranchCode varchar(15)
	,@SONo varchar(15)
	,@SalesModelCode varchar(30)
	--,@SalesModelYear int
	,@SalesModelYear varchar(15)
	,@ChassisCode varchar(15)
	,@RefType varchar(15)
	,@WarehouseCode varchar(15)

as

--declare @CompanyCode varchar(15)
--,@BranchCode varchar(15)
--,@SONo varchar(15)
--,@SalesModelCode varchar(15)
--,@SalesModelYear int
--,@ChassisCode varchar(15)
--,@RefType varchar(15)
--,@WarehouseCode varchar(15)
--
--select @CompanyCode='6115202'
--,@BranchCode='611520200'
--,@SONo='SOA/11/000287' 
--,@SalesModelCode='FU150 SCD' 
--,@SalesModelYear=2011
--,@ChassisCode='MH8BG41CABJ' 
--,@RefType='COLO'
--,@WarehouseCode='00'

declare 
@val as int,
@CompanyMD as varchar(15)
,@DBMD as varchar(15)
,@QryTemp as varchar(max)


DECLARE @columnVal TABLE (columnVal int);

set @val=0

set @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
set @DBMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 

--set @val= (
set @QryTemp = 'SELECT isnull(count (a.ChassisNo),0) jml
FROM omTrSalesSOVin a 
	INNER JOIN ' + @DBMD + '.dbo.omMstVehicle b ON 
		a.ChassisCode = b.ChassisCode 
		AND a.ChassisNo = b.ChassisNo 
WHERE a.CompanyCode = ''' + @CompanyCode + '''
	AND a.BranchCode = ''' + @BranchCode + ''' 
	AND a.SONo = ''' + @SONo + '''
	AND a.SalesModelCode = ''' + @SalesModelCode + ''' 
	AND a.SalesModelYear = ''' + @SalesModelYear + '''
	AND a.ChassisCode = ''' + @ChassisCode + '''
	and b.Status in (0,3)
	AND not exists 
	( 
		SELECT 1 
		FROM omTrSalesDODetail x
			inner join omTrSalesDO y on x.CompanyCode=y.CompanyCode and x.BranchCode=y.BranchCode
				and x.DONo=y.DONo
		WHERE x.CompanyCode = a.CompanyCode 
		AND x.BranchCode = a.BranchCode 
		AND x.SalesModelCode = a.SalesModelCode 
		AND x.ChassisCode = a.ChassisCode 
		and x.ChassisNo= a.ChassisNo
		and y.Status in (0,1)
	) 
	AND a.ChassisNo != 0 ' 

INSERT INTO @columnVal EXEC (@QryTemp);

set @val= (SELECT * FROM @columnval);

if @val = 0 
begin
	set @QryTemp =
	'SELECT 
		a.ChassisCode , a.ChassisNo , a.EngineCode , a.EngineNo , 
		a.ColourCode , b.RefferenceDesc1 AS ColourName 
	FROM ' + @DBMD + '.dbo.omMstVehicle a 
		LEFT JOIN ' + @DBMD + '.dbo.omMstRefference b ON b.CompanyCode = a.CompanyCode 
			AND b.RefferenceType = ''' + @RefType + ''' 
			AND b.RefferenceCode = a.ColourCode 
	WHERE a.CompanyCode = ''' + @CompanyMD + '''  
		AND a.SalesModelCode = ''' + @SalesModelCode + ''' 
		AND a.ChassisCode = ''' + @ChassisCode + ''' 
		AND a.Status = 0 
		AND isnull (a.SONo,'''') = ''''  
		AND a.WarehouseCode = ''' + @WarehouseCode + '''
		AND exists 
		( 
			SELECT z.colourCode FROM OmTrSalesSOModelColour z 
			WHERE z.companyCode = a.CompanyCode 
			AND z.BranchCode = ''' + @BranchCode + '''
			AND z.SONo= ''' + @SONo + ''' 
			AND z.SalesModelCode = a.SalesModelCode 
			AND z.SalesModelYear = a.SalesModelYear 
			and z.ColourCode= a.ColourCode
		)'
		Exec (@QryTemp);
end
else
begin
	set @QryTemp =
	'SELECT 
		a.ChassisNo , a.EngineCode , a.EngineNo , 
		a.ColourCode , b.RefferenceDesc1 AS ColourName 
	FROM omTrSalesSOVin a 
		LEFT JOIN omMstRefference b ON b.CompanyCode = a.CompanyCode 
			AND b.RefferenceType = ''' + @RefType + ''' 
			AND b.RefferenceCode = a.ColourCode 
		INNER JOIN ' + @DBMD + '.dbo.omMstVehicle c ON 
			a.ChassisCode = c.ChassisCode 
			AND a.ChassisNo = c.ChassisNo 
	WHERE a.CompanyCode = ''' + @CompanyCode + ''' 
		AND a.BranchCode = ''' + @BranchCode + '''  
		AND a.SONo = ''' + @SONo + ''' 
		AND a.SalesModelCode = ''' + @SalesModelCode + '''
		AND a.SalesModelYear = ''' + @SalesModelYear + ''' 
		AND a.ChassisCode = ''' + @ChassisCode + ''' 
		AND c.WarehouseCode = ''' + @WarehouseCode + '''
		and c.Status in (0,3)
		AND not exists
		( 
			SELECT 1 
			FROM omTrSalesDODetail x
				inner join omTrSalesDO y on x.CompanyCode=y.CompanyCode and x.BranchCode=y.BranchCode
					and x.DONo=y.DONo
			WHERE x.CompanyCode = a.CompanyCode 
			AND x.BranchCode = a.BranchCode 
			AND x.SalesModelCode = a.SalesModelCode 
			AND x.ChassisCode = a.ChassisCode 
			and x.ChassisNo= a.ChassisNo
			and y.Status in (0,1)
		)'
		Exec (@QryTemp);
end

GO


