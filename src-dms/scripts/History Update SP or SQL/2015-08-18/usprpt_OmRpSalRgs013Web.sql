IF Object_id ('usprpt_OmRpSalRgs013Web') IS NOT NULL DROP procedure usprpt_OmRpSalRgs013Web
GO
--usprpt_OmRpSalRgs013 '6092401','609240100','2011/01/01','2011/01/31','','','0'

CREATE procedure [dbo].[usprpt_OmRpSalRgs013Web] 
(
	@CompanyCode VARCHAR(15),
	@BranchCode	VARCHAR(15),
	@FromDate DATETIME,
	@ToDate DATETIME,
	@ReqFrom VARCHAR(15),
	@ReqTo VARCHAR(15),
	@pReport CHAR(1)
)
--select * from omTrSalesReqDetail
AS

DECLARE
	@QRYTmp		AS varchar(max),
	@DBMD		AS varchar(25),
	@CompanyMD  AS varchar(25),
	@BranchMD	AS varchar(25)


BEGIN

set @CompanyMD = (SELECT TOP 1 CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
set @BranchMD = (SELECT TOP 1 UnitBranchMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode)
set @DBMD = (SELECT TOP 1 DbMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode)

IF @DBMD IS NULL
begin
	SELECT c.ReqNo
		, a.FakturPolisiNo
		, a.ChassisCode
		, convert(varchar,a.ChassisNo) AS ChassisNo
		, convert(varchar,b.EngineNo) AS EngineNo
		, b.ColourCode
		, b.SalesModelCode
		, a.FakturPolisiName
		, (SELECT c.CustomerName 
				FROM GnMstCustomer c 
				INNER JOIN omTrSalesReq d 
					ON c.CompanyCode = d.CompanyCode
					AND c.CustomerCode = d.SubDealerCode 
					AND d.ReqNo = a.ReqNo 
					AND d.BranchCode = a.BranchCode	
		  ) AS SubDealer
		, isnull(d.RefferenceDONo,'') DONo
		, isnull(d.RefferenceDODate,'') DODate
		, a.FakturPolisiDate
	FROM omTrSalesReqDetail a
		LEFT JOIN omMstVehicle b
			ON a.CompanyCode = b.CompanyCode
			AND a.ChassisCode = b.ChassisCode
			AND a.ChassisNo = b.ChassisNo
		INNER JOIN omTrSalesReq c
			ON a.CompanyCode = c.CompanyCode
			AND a.BranchCode = c.BranchCode
			AND a.ReqNo = c.ReqNo
		LEFT JOIN omTrPurchaseBPU d
			on d.CompanyCode = a.CompanyCode
			and d.BranchCode = a.BranchCode
			and d.BPUNo = b.BPUNo
			and d.PoNo = b.PoNo
	WHERE a.CompanyCode = @CompanyCode
		AND a.BranchCode = @BranchCode
		AND ((case when @pReport='0' then c.ReqDate end)= c.ReqDate
				or (case when @pReport='1' then CONVERT(VARCHAR, c.ReqDate, 112) end) BETWEEN CONVERT(VARCHAR, @FromDate, 112) AND CONVERT(VARCHAR, @ToDate, 112))
		AND ((CASE WHEN @ReqFrom = '' THEN a.ReqNo END) <> ''
			OR (CASE WHEN @ReqFrom <> '' THEN a.ReqNo END) BETWEEN @ReqFrom AND @ReqTo)
	ORDER BY a.ReqNo
end
else
	set @QRYTmp =
	'SELECT c.ReqNo
		, a.FakturPolisiNo
		, a.ChassisCode
		, a.ChassisNo
		, b.EngineNo
		, b.ColourCode
		, b.SalesModelCode
		, a.FakturPolisiName
		, (SELECT c.CustomerName 
				FROM GnMstCustomer c 
				INNER JOIN omTrSalesReq d 
					ON c.CompanyCode = d.CompanyCode
					AND c.CustomerCode = d.SubDealerCode 
					AND d.ReqNo = a.ReqNo 
					AND d.BranchCode = a.BranchCode	
		  ) AS SubDealer
		, isnull(d.RefferenceDONo,'''') DONo
		, isnull(d.RefferenceDODate,'''') DODate
		, a.FakturPolisiDate
	FROM omTrSalesReqDetail a
		LEFT JOIN ' + @DBMD + '..omMstVehicle b
			ON b.CompanyCode = ''' + @CompanyMD + '''
			AND a.ChassisCode = b.ChassisCode
			AND a.ChassisNo = b.ChassisNo
		INNER JOIN omTrSalesReq c
			ON a.CompanyCode = c.CompanyCode
			AND a.BranchCode = c.BranchCode
			AND a.ReqNo = c.ReqNo
		LEFT JOIN ' + @DBMD + '..omTrPurchaseBPU d
			on d.CompanyCode = ''' + @CompanyMD + '''
			and d.BranchCode = ''' + @BranchMD + '''
			and d.BPUNo = b.BPUNo
			and d.PoNo = b.PoNo
	WHERE a.CompanyCode = ''' + @CompanyCode + '''
		AND a.BranchCode = ''' + @BranchCode + '''
		AND ((case when ''' + @pReport + '''=''0'' then c.ReqDate end)= c.ReqDate
				or (case when ''' + @pReport + '''=''1'' then CONVERT(VARCHAR, c.ReqDate, 112) end) BETWEEN ''' + CONVERT(VARCHAR, @FromDate, 112) + ''' AND ''' + CONVERT(VARCHAR, @ToDate, 112) + ''')
		AND ((CASE WHEN ''' + @ReqFrom + ''' = '''' THEN a.ReqNo END) <> ''''
			OR (CASE WHEN ''' + @ReqFrom + ''' <> '''' THEN a.ReqNo END) BETWEEN ''' + @ReqFrom + ''' AND ''' + @ReqTo + ''')
	ORDER BY a.ReqNo'

	EXEC (@QRYTmp);
end
--------------------------------------------------- BATAS ----------------------------------------------------------
/*
IF @pReport = '0'
BEGIN
	SELECT c.ReqNo
	, a.FakturPolisiNo
	, a.ChassisCode
	, convert(varchar,a.ChassisNo)   AS ChassisNo
    , b.SalesModelCode
	, a.FakturPolisiName
	, (SELECT c.CustomerName 
		FROM GnMstCustomer c 
			INNER JOIN omTrSalesReq d 
			ON c.CompanyCode = d.CompanyCode
			AND c.CustomerCode = d.SubDealerCode 
			AND d.ReqNo = a.ReqNo 
			AND d.BranchCode = a.BranchCode	
	   ) AS SubDealer
	, isnull(d.RefferenceDONo,'') DONo
	, isnull(d.RefferenceDODate,'') DODate
	, a.FakturPolisiDate
	FROM omTrSalesReqDetail a
	LEFT JOIN omMstVehicle b
	ON a.CompanyCode = b.CompanyCode
	AND a.ChassisCode = b.ChassisCode
	AND a.ChassisNo = b.ChassisNo
	INNER JOIN omTrSalesReq c
	ON a.CompanyCode = c.CompanyCode
	AND a.BranchCode = c.BranchCode
	AND a.ReqNo = c.ReqNo
	LEFT JOIN omTrPurchaseBPU d
	on d.CompanyCode = a.CompanyCode
	and d.BranchCode = a.BranchCode
	and d.BPUNo = b.BPUNo
	WHERE a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND ((CASE WHEN @ReqFrom = '' THEN a.ReqNo END) <> ''
	OR (CASE WHEN @ReqFrom <> '' THEN a.ReqNo END) BETWEEN @ReqFrom AND @ReqTo)
	ORDER BY a.ReqNo
END
ELSE
BEGIN
	SELECT c.ReqNo
	, a.FakturPolisiNo
	, a.ChassisCode
	, convert(varchar,a.ChassisNo) AS ChassisNo
    , b.SalesModelCode
	, a.FakturPolisiName
	, (SELECT c.CustomerName 
			FROM GnMstCustomer c 
			INNER JOIN omTrSalesReq d 
				ON c.CompanyCode = d.CompanyCode
				AND c.CustomerCode = d.SubDealerCode 
				AND d.ReqNo = a.ReqNo 
				AND d.BranchCode = a.BranchCode	
	  ) AS SubDealer
	, isnull(d.RefferenceDONo,'') DONo
	, isnull(d.RefferenceDODate,'') DODate
	, a.FakturPolisiDate
	FROM omTrSalesReqDetail a
	LEFT JOIN omMstVehicle b
	ON a.CompanyCode = b.CompanyCode
	AND a.ChassisCode = b.ChassisCode
	AND a.ChassisNo = b.ChassisNo
	INNER JOIN omTrSalesReq c
	ON a.CompanyCode = c.CompanyCode
	AND a.BranchCode = c.BranchCode
	AND a.ReqNo = c.ReqNo
	LEFT JOIN omTrPurchaseBPU d
	on d.CompanyCode = a.CompanyCode
	and d.BranchCode = a.BranchCode
	and d.BPUNo = b.BPUNo
	WHERE a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND CONVERT(VARCHAR, c.ReqDate, 112) BETWEEN CONVERT(VARCHAR, @FromDate, 112) AND CONVERT(VARCHAR, @ToDate, 112) 
	AND ((CASE WHEN @ReqFrom = '' THEN a.ReqNo END) <> ''
	OR (CASE WHEN @ReqFrom <> '' THEN a.ReqNo END) BETWEEN @ReqFrom AND @ReqTo)
	ORDER BY a.ReqNo
END
*/
