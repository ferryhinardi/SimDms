IF Object_id ('usprpt_OmRpSalRgs017Web') IS NOT NULL DROP procedure usprpt_OmRpSalRgs017Web
GO
CREATE procedure [dbo].[usprpt_OmRpSalRgs017Web] 
(
	@CompanyCode VARCHAR(15),
	@BranchCode	VARCHAR(15),
	@ReqDateFrom VARCHAR(8),
	@ReqDateTo VARCHAR(8)
)
AS

DECLARE
	@QRYTmp		AS varchar(max),
	@DBMD		AS varchar(25),
	@CompanyMD  AS varchar(25)


BEGIN

set @CompanyMD = (SELECT TOP 1 CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
set @DBMD = (SELECT TOP 1 DbMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode)

IF @DBMD IS NULL
begin
	select right (1000000 + (row_number () OVER (ORDER BY reqDtl.FakturPolisiNo)), 4)  AS No
		, orgCust.CustomerName
		, reqHdr.ReqDate
		, reqDtl.FakturPolisiNo
		, reqDtl.FakturPolisiDate
		, veh.SalesModelCode
		, (case when (reqDtl.isCityTransport = '1') then 'V' else '' end) isCityTransport
		, (reqDtl.ChassisCode + ' ' + convert(varchar, reqDtl.ChassisNo, 10)) as Chassis
		, (veh.EngineCode + ' ' + convert(varchar, veh.EngineNo, 10)) as Engine
		, reqDtl.FakturPolisiName
		, reqDtl.FakturPolisiAddress1
		, reqDtl.FakturPolisiAddress2
		, reqDtl.FakturPolisiAddress3
		, @ReqDateFrom as ReqDateFrom
		, @ReqDateTo as ReqDateTo
		, lookupDtl.LookupValueName City
		, (select SignName from gnMstSignature 
			where CompanyCode=reqDtl.CompanyCode and BranchCode=reqDtl.BranchCode
				and ProfitCenterCode = '100' and DocumentType = 'RFP' and SeqNo = 1) as SignName
		, color.RefferenceDesc1 color
		, veh.SalesModelYear
	from omTrSalesReqDetail reqDtl with(nolock, nowait)
	inner join omTrSalesReq reqHdr with(nolock, nowait) on 
		reqHdr.CompanyCode = reqDtl.CompanyCode
		and reqHdr.BranchCode = reqDtl.BranchCode	
		and reqHdr.ReqNo = reqDtl.ReqNo	
	left join gnMstCustomer orgCust with(nolock, nowait) on 
		orgCust.CustomerCode = reqHdr.SubDealerCode
		and orgCust.CompanyCode = reqHdr.CompanyCode
	left join omMstVehicle veh with(nolock, nowait) on
		veh.CompanyCode = reqDtl.CompanyCode
		and veh.ChassisCode = reqDtl.ChassisCode
		and veh.ChassisNo = reqDtl.ChassisNo
	left join gnMstCoProfile pfl with (nolock, nowait) on
		pfl.CompanyCode = reqDtl.CompanyCode
		and pfl.BranchCode = reqDtl.BranchCode
	left join gnMstLookupDtl lookupDtl with(nolock, nowait) on
		lookupDtl.CompanyCode = pfl.CompanyCode 
		and lookupDtl.LookupValue = pfl.CityCode
		and lookupDtl.CodeID = 'CITY'
	left join omMstRefference color on color.CompanyCode = reqDtl.CompanyCode
		and color.RefferenceType = 'COLO'
		AND color.RefferenceCode =  veh.ColourCode
	where reqDtl.CompanyCode = @CompanyCode
		and reqDtl.BranchCode = @BranchCode
		and convert(varchar, reqHdr.ReqDate, 112) between @ReqDateFrom and @ReqDateTo	
end
else
	set @QRYTmp = 
	'select right (1000000 + (row_number () OVER (ORDER BY reqDtl.FakturPolisiNo)), 4)  AS No
		, orgCust.CustomerName
		, reqHdr.ReqDate
		, reqDtl.FakturPolisiNo
		, reqDtl.FakturPolisiDate
		, veh.SalesModelCode
		, (case when (reqDtl.isCityTransport = ''1'') then ''V'' else '''' end) isCityTransport
		, (reqDtl.ChassisCode + '' '' + convert(varchar, reqDtl.ChassisNo, 10)) as Chassis
		, (veh.EngineCode + '' '' + convert(varchar, veh.EngineNo, 10)) as Engine
		, reqDtl.FakturPolisiName
		, reqDtl.FakturPolisiAddress1
		, reqDtl.FakturPolisiAddress2
		, reqDtl.FakturPolisiAddress3
		, ''' + @ReqDateFrom + ''' as ReqDateFrom
		, ''' + @ReqDateTo + ''' as ReqDateTo
		, lookupDtl.LookupValueName City
		, (select SignName from gnMstSignature 
			where CompanyCode=reqDtl.CompanyCode and BranchCode=reqDtl.BranchCode
				and ProfitCenterCode = ''100'' and DocumentType = ''RFP'' and SeqNo = 1) as SignName
		, color.RefferenceDesc1 color
		, veh.SalesModelYear
	from omTrSalesReqDetail reqDtl with(nolock, nowait)
	inner join omTrSalesReq reqHdr with(nolock, nowait) on 
		reqHdr.CompanyCode = reqDtl.CompanyCode
		and reqHdr.BranchCode = reqDtl.BranchCode	
		and reqHdr.ReqNo = reqDtl.ReqNo	
	left join gnMstCustomer orgCust with(nolock, nowait) on 
		orgCust.CustomerCode = reqHdr.SubDealerCode
		and orgCust.CompanyCode = reqHdr.CompanyCode
	left join ' + @DBMD + '..omMstVehicle veh with(nolock, nowait) on
		veh.CompanyCode = ''' + @CompanyMD + '''
		and veh.ChassisCode = reqDtl.ChassisCode
		and veh.ChassisNo = reqDtl.ChassisNo
	left join gnMstCoProfile pfl with (nolock, nowait) on
		pfl.CompanyCode = reqDtl.CompanyCode
		and pfl.BranchCode = reqDtl.BranchCode
	left join gnMstLookupDtl lookupDtl with(nolock, nowait) on
		lookupDtl.CompanyCode = pfl.CompanyCode 
		and lookupDtl.LookupValue = pfl.CityCode
		and lookupDtl.CodeID = ''CITY''
	left join omMstRefference color on color.CompanyCode = reqDtl.CompanyCode
		and color.RefferenceType = ''COLO''
		AND color.RefferenceCode =  veh.ColourCode
	where reqDtl.CompanyCode = ''' + @CompanyCode + '''
		and reqDtl.BranchCode = ''' + @BranchCode + '''
		and convert(varchar, reqHdr.ReqDate, 112) between ''' + @ReqDateFrom + ''' and ''' + @ReqDateTo + ''''
	exec (@QRYTmp);
end

