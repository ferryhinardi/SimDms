ALTER PROCEDURE [dbo].[usprpt_InquiryITSProd]
--DECLARE 
	@StartDate			DATETIME,
	@EndDate			DATETIME,
	@Area				varchar(100),
	@DealerCode			varchar(100),
	@OutletCode			varchar(100),
	@BranchHead			varchar(100),
	@SalesHead			varchar(100),
	@Salesman				varchar(100),
	@TypeReport			varchar(1),
	@ProductivityBy		varchar(1)
AS
BEGIN

--set @StartDate = '20150301'
--set @EndDate	= '20150331'
--set @Area = 'JAWA TIMUR / BALI / LOMBOK'
--set @DealerCode = '6093401'
--set @OutletCode = '609340101'
--set @BranchHead = '010138'
--set @SalesHead  = '010135'
--set @Salesman = '010047'
--set @TypeReport = '0' -- 0 : SUMMARY, 1 : SALDO
--set @ProductivityBy = '2'	-- 0 : SALESMAN, 1 : VEHICLE TYPE, 2 : SOURCE DATA		
	
IF @ProductivityBy = '0'
BEGIN
SELECT * INTO #t1 FROM (
	SELECT * FROM (
	SELECT '0' TypeReport, Hist.CompanyCode, Hist.BranchCode, ISNULL(Emp.EmployeeID, '') EmployeeID, ISNULL(Emp.EmployeeName , '') Wiraniaga, 
	Hist.LastProgress, COUNT(Hist.LastProgress) countLastProg, SpvEmp.EmployeeName SalesCoordinator, SpvEmp.EmployeeID SpvEmployeeID, Emp.Position
	FROM  pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
	INNER JOIN  pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
		HstITS.CompanyCode = Hist.CompanyCode AND
		HstITS.BranchCode = Hist.BranchCode AND
		HstITS.InquiryNumber = Hist.InquiryNumber 
	LEFT JOIN pmKDP KDP WITH (NOLOCK, NOWAIT) ON 
		KDP.CompanyCode = Hist.CompanyCode AND
		KDP.BranchCode = Hist.BranchCode AND
		KDP.InquiryNumber = Hist.InquiryNumber
	LEFT JOIN HrEmployee Emp WITH (NOLOCK, NOWAIT) ON 
		Emp.CompanyCode = KDP.CompanyCode AND
		Emp.EmployeeID = KDP.EmployeeID
	LEFT JOIN HrEmployee SpvEmp WITH (NOLOCK, NOWAIT) ON 
		SpvEmp.CompanyCode = Emp.CompanyCode AND
		SpvEmp.EmployeeID = Emp.TeamLeader
	WHERE Hist.LastProgress = 'P'
		AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
		or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
		and (Hist.LastProgress='P'   and not exists (select top 1 1 from  pmStatusHistory h
	where h.CompanyCode=Hist.CompanyCode
		and h.BranchCode=Hist.BranchCode
		and h.InquiryNumber=Hist.InquiryNumber
		and h.LastProgress<>'P'
		and convert(varchar,h.UpdateDate,112)<@StartDate)
		or  Hist.LastProgress='HP'  and not exists (select top 1 1 from  pmStatusHistory h
		where h.CompanyCode=Hist.CompanyCode
			and h.BranchCode=Hist.BranchCode
			and h.InquiryNumber=Hist.InquiryNumber
			and h.LastProgress not in ('P','HP')
			and convert(varchar,h.UpdateDate,112)<@StartDate)
			or Hist.LastProgress='SPK' and not exists (select top 1 1 from  pmStatusHistory h
	where h.CompanyCode=Hist.CompanyCode
	and h.BranchCode=Hist.BranchCode
	and h.InquiryNumber=Hist.InquiryNumber
	and h.LastProgress not in ('P','HP','SPK')
	and convert(varchar,h.UpdateDate,112)<@StartDate))))	
	AND	CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)
	and (CONVERT(VARCHAR, HstITS.InquiryDate , 112) between CONVERT(VARCHAR, @StartDate, 112) and @EndDate or HstITS.InquiryDate > CONVERT(VARCHAR, @EndDate, 112))											 	
	GROUP BY Hist.CompanyCode, Hist.BranchCode, Emp.EmployeeID, Emp.EmployeeName, Hist.LastProgress, SpvEmp.EmployeeName, SpvEmp.EmployeeID, Emp.Position
	UNION ALL
	SELECT '0' TypeReport, Hist.CompanyCode, Hist.BranchCode, ISNULL(Emp.EmployeeID, '') EmployeeID, ISNULL(Emp.EmployeeName , '') Wiraniaga, 
		Hist.LastProgress, CASE WHEN Hist.LastProgress IN ('DO','P','DELIVERY', 'LOST') THEN 0 ELSE COUNT(Hist.LastProgress) END countLastProg, 
		SpvEmp.EmployeeName SalesCoordinator, SpvEmp.EmployeeID SpvEmployeeID, Emp.Position
	FROM  pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
	INNER JOIN  pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
		HstITS.CompanyCode = Hist.CompanyCode AND
		HstITS.BranchCode = Hist.BranchCode AND
		HstITS.InquiryNumber = Hist.InquiryNumber 
	LEFT JOIN pmKDP KDP WITH (NOLOCK, NOWAIT) ON 
		KDP.CompanyCode = Hist.CompanyCode AND
		KDP.BranchCode = Hist.BranchCode AND
		KDP.InquiryNumber = Hist.InquiryNumber
	LEFT JOIN HrEmployee Emp WITH (NOLOCK, NOWAIT) ON 
		Emp.CompanyCode = KDP.CompanyCode AND
		Emp.EmployeeID = KDP.EmployeeID
	LEFT JOIN HrEmployee SpvEmp WITH (NOLOCK, NOWAIT) ON 
		SpvEmp.CompanyCode = Emp.CompanyCode AND
		SpvEmp.EmployeeID = Emp.TeamLeader
	WHERE (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
		or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
		and (Hist.LastProgress='P'   and not exists (select top 1 1 from  pmStatusHistory h
	where h.CompanyCode=Hist.CompanyCode
		and h.BranchCode=Hist.BranchCode
		and h.InquiryNumber=Hist.InquiryNumber
		and h.LastProgress<>'P'
		and convert(varchar,h.UpdateDate,112)<@StartDate)
		or  Hist.LastProgress='HP'  and not exists (select top 1 1 from  pmStatusHistory h
		where h.CompanyCode=Hist.CompanyCode
			and h.BranchCode=Hist.BranchCode
			and h.InquiryNumber=Hist.InquiryNumber
			and h.LastProgress not in ('P','HP')
			and convert(varchar,h.UpdateDate,112)<@StartDate)
			or Hist.LastProgress='SPK' and not exists (select top 1 1 from  pmStatusHistory h
	where h.CompanyCode=Hist.CompanyCode
	and h.BranchCode=Hist.BranchCode
	and h.InquiryNumber=Hist.InquiryNumber
	and h.LastProgress not in ('P','HP','SPK')
	and convert(varchar,h.UpdateDate,112)<@StartDate))))	
	AND	CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)
	AND (CONVERT(VARCHAR, HstITS.InquiryDate , 112) between CONVERT(VARCHAR, @StartDate, 112) and @EndDate or CONVERT(VARCHAR, HstITS.InquiryDate , 112) > CONVERT(VARCHAR, @EndDate, 112)
	OR (CONVERT(VARCHAR, HstITS.InquiryDate , 112) < CONVERT(VARCHAR, @StartDate, 112)  AND CONVERT(VARCHAR, Hist.UpdateDate , 112) between CONVERT(VARCHAR, @StartDate, 112) and CONVERT(VARCHAR, @EndDate, 112)))											 	
	GROUP BY Hist.CompanyCode, Hist.BranchCode, Emp.EmployeeID, Emp.EmployeeName, Hist.LastProgress, SpvEmp.EmployeeName, SpvEmp.EmployeeID, Emp.Position
	UNION ALL						
	SELECT '1' TypeReport, Hist.CompanyCode, Hist.BranchCode, ISNULL(Emp.EmployeeID, '') EmployeeID, ISNULL(Emp.EmployeeName , '') Wiraniaga,
		Hist.LastProgress, CASE WHEN Hist.LastProgress IN ('DO','DELIVERY', 'LOST') THEN 0 ELSE COUNT(Hist.LastProgress) END countLastProg, 
		SpvEmp.EmployeeName SalesCoordinator, SpvEmp.EmployeeID SpvEmployeeID, Emp.Position
		FROM  pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
		INNER JOIN  pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
			HstITS.CompanyCode = Hist.CompanyCode AND
			HstITS.BranchCode = Hist.BranchCode AND
			HstITS.InquiryNumber = Hist.InquiryNumber 
		LEFT JOIN pmKDP KDP WITH (NOLOCK, NOWAIT) ON 
			KDP.CompanyCode = Hist.CompanyCode AND
			KDP.BranchCode = Hist.BranchCode AND
			KDP.InquiryNumber = Hist.InquiryNumber
		LEFT JOIN HrEmployee Emp WITH (NOLOCK, NOWAIT) ON 
			Emp.CompanyCode = KDP.CompanyCode AND
			Emp.EmployeeID = KDP.EmployeeID
		LEFT JOIN HrEmployee SpvEmp WITH (NOLOCK, NOWAIT) ON 
			SpvEmp.CompanyCode = Emp.CompanyCode AND
			SpvEmp.EmployeeID = Emp.TeamLeader
		 WHERE (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
			 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
			and (Hist.LastProgress='P'   and not exists (select top 1 1 from  pmStatusHistory h
													   where h.CompanyCode=Hist.CompanyCode
														 and h.BranchCode=Hist.BranchCode
														 and h.InquiryNumber=Hist.InquiryNumber
														 and h.LastProgress<>'P'
														 and convert(varchar,h.UpdateDate,112)<@StartDate)
			 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from  pmStatusHistory h
													   where h.CompanyCode=Hist.CompanyCode
														 and h.BranchCode=Hist.BranchCode
														 and h.InquiryNumber=Hist.InquiryNumber
														 and h.LastProgress not in ('P','HP')
														 and convert(varchar,h.UpdateDate,112)<@StartDate)
			 or Hist.LastProgress='SPK' and not exists (select top 1 1 from  pmStatusHistory h
													   where h.CompanyCode=Hist.CompanyCode
														 and h.BranchCode=Hist.BranchCode
														 and h.InquiryNumber=Hist.InquiryNumber
														 and h.LastProgress not in ('P','HP','SPK')
														 and convert(varchar,h.UpdateDate,112)<@StartDate))))	
		AND	CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)
		AND (CONVERT(VARCHAR, HstITS.InquiryDate , 112) between CONVERT(VARCHAR, @StartDate, 112) and @EndDate or CONVERT(VARCHAR, HstITS.InquiryDate , 112) > CONVERT(VARCHAR, @EndDate, 112)
		OR (CONVERT(VARCHAR, HstITS.InquiryDate , 112) < CONVERT(VARCHAR, @StartDate, 112)  AND CONVERT(VARCHAR, Hist.UpdateDate , 112) between CONVERT(VARCHAR, @StartDate, 112) and CONVERT(VARCHAR, @EndDate, 112)))											 	
		AND CONVERT(VARCHAR, HstITS.LastUpdateDate, 112) <= CONVERT(VARCHAR, @EndDate, 112) 
		and HstITS.LastProgress not in ('DO','DELIVERY','LOST')															 
		GROUP BY Hist.CompanyCode, Hist.BranchCode, Emp.EmployeeID, Emp.EmployeeName, Hist.LastProgress, SpvEmp.EmployeeName, SpvEmp.EmployeeID, Emp.Position
	UNION ALL
	SELECT '' TypeReport, DOH.CompanyCode, DOH.BranchCode, ISNULL(Emp.EmployeeID, '') EmployeeID, ISNULL(Emp.EmployeeName , '') Wiraniaga, 'DO' LastProgress, 
		COUNT(DOD.DONo) countLastProg, SpvEmp.EmployeeName SalesCoordinator, SpvEmp.EmployeeID SpvEmployeeID, Emp.Position			
	FROM omTrSalesDO DOH WITH (NOLOCK, NOWAIT)
	INNER JOIN omTrSalesDODetail DOD WITH (NOLOCK, NOWAIT) ON 
		DOH.CompanyCode = DOD.CompanyCode AND
		DOH.BranchCode = DOD.BranchCode AND
		DOH.DONo = DOD.DONo
	LEFT JOIN omMstModel Model WITH (NOLOCK, NOWAIT) ON 
		Model.CompanyCode = DOD.CompanyCode AND
		Model.SalesModelCode = DOD.SalesModelCode 
	 INNER JOIN omTrSalesSO SOH WITH (NOLOCK, NOWAIT) ON 
		SOH.CompanyCode = DOH.CompanyCode AND
		SOH.BranchCode = DOH.BranchCode AND
		SOH.SONo = DOH.SONo
	 LEFT JOIN HrEmployee Emp WITH (NOLOCK, NOWAIT) ON 
		Emp.CompanyCode = SOH.CompanyCode AND
		Emp.EmployeeID = SOH.Salesman
	 LEFT JOIN HrEmployee SpvEmp WITH (NOLOCK, NOWAIT) ON 
		SpvEmp.CompanyCode = SOH.CompanyCode AND
		SpvEmp.EmployeeID = Emp.TeamLeader
	 WHERE  DOH.Status = '2'
		AND CONVERT(VARCHAR, DOH.DODate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 
	GROUP BY DOH.CompanyCode, DOH.BranchCode, Emp.EmployeeID, Emp.EmployeeName,SpvEmp.EmployeeName, SpvEmp.EmployeeID, Emp.Position
	UNION ALL
		SELECT '' TypeReport, SJH.CompanyCode, SJH.BranchCode, ISNULL(Emp.EmployeeID, '') EmployeeID, ISNULL(Emp.EmployeeName , '') Wiraniaga,
		'DELIVERY' LastProgress, COUNT(SJD.BPKNo) countLastProg, SpvEmp.EmployeeName SalesCoordinator, SpvEmp.EmployeeID SpvEmployeeID, Emp.Position	
		FROM omTrSalesBPK SJH WITH (NOLOCK, NOWAIT)
		INNER JOIN omTrSalesBPKDetail SJD WITH (NOLOCK, NOWAIT) ON 
			SJH.CompanyCode = SJD.CompanyCode AND
			SJH.BranchCode = SJD.BranchCode AND
			SJH.BPKNo = SJD.BPKNo 
		INNER JOIN omMstModel Model WITH (NOLOCK, NOWAIT) ON 
			Model.CompanyCode = SJD.CompanyCode AND
			Model.SalesModelCode = SJD.SalesModelCode 
		INNER JOIN omTrSalesSO SOH WITH (NOLOCK, NOWAIT) ON 
			SOH.CompanyCode = SJH.CompanyCode AND
			SOH.BranchCode = SJH.BranchCode AND
			SOH.SONo = SJH.SONo 
		LEFT JOIN HrEmployee Emp WITH (NOLOCK, NOWAIT) ON 
			Emp.CompanyCode = SOH.CompanyCode AND
			Emp.EmployeeID = SOH.Salesman
		LEFT JOIN HrEmployee SpvEmp WITH (NOLOCK, NOWAIT) ON 
			SpvEmp.CompanyCode = SOH.CompanyCode AND
			SpvEmp.EmployeeID = Emp.TeamLeader
		 WHERE  SJH.Status = '2'
			AND CONVERT(VARCHAR, SJH.BPKDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 
		GROUP BY SJH.CompanyCode, SJH.BranchCode, Emp.EmployeeID, Emp.EmployeeName,SpvEmp.EmployeeName, SpvEmp.EmployeeID, Emp.Position
	UNION ALL
	SELECT '' TypeReport, Hist.CompanyCode, Hist.BranchCode, ISNULL(Emp.EmployeeID, '') EmployeeID, ISNULL(Emp.EmployeeName , '') Wiraniaga, 
	'LOST' LastProgress, COUNT(Hist.LastProgress) countLastProg, SpvEmp.EmployeeName SalesCoordinator, SpvEmp.EmployeeID SpvEmployeeID, Emp.Position
	FROM  pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
	INNER JOIN  pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
		HstITS.CompanyCode = Hist.CompanyCode AND
		HstITS.BranchCode = Hist.BranchCode AND
		HstITS.InquiryNumber = Hist.InquiryNumber 
	LEFT JOIN pmKDP KDP WITH (NOLOCK, NOWAIT) ON 
		KDP.CompanyCode = Hist.CompanyCode AND
		KDP.BranchCode = Hist.BranchCode AND
		KDP.InquiryNumber = Hist.InquiryNumber
	LEFT JOIN HrEmployee Emp WITH (NOLOCK, NOWAIT) ON 
		Emp.CompanyCode = KDP.CompanyCode AND
		Emp.EmployeeID = KDP.EmployeeID
	LEFT JOIN HrEmployee SpvEmp WITH (NOLOCK, NOWAIT) ON 
		SpvEmp.CompanyCode = Emp.CompanyCode AND
		SpvEmp.EmployeeID = Emp.TeamLeader
	WHERE Hist.LastProgress='LOST' 
		AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
		or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
		and (Hist.LastProgress='P'   and not exists (select top 1 1 from  pmStatusHistory h
	where h.CompanyCode=Hist.CompanyCode
		and h.BranchCode=Hist.BranchCode
		and h.InquiryNumber=Hist.InquiryNumber
		and h.LastProgress<>'P'
		and convert(varchar,h.UpdateDate,112)<@StartDate)
		or  Hist.LastProgress='HP'  and not exists (select top 1 1 from  pmStatusHistory h
		where h.CompanyCode=Hist.CompanyCode
			and h.BranchCode=Hist.BranchCode
			and h.InquiryNumber=Hist.InquiryNumber
			and h.LastProgress not in ('P','HP')
			and convert(varchar,h.UpdateDate,112)<@StartDate)
			or Hist.LastProgress='SPK' and not exists (select top 1 1 from  pmStatusHistory h
	where h.CompanyCode=Hist.CompanyCode
	and h.BranchCode=Hist.BranchCode
	and h.InquiryNumber=Hist.InquiryNumber
	and h.LastProgress not in ('P','HP','SPK')
	and convert(varchar,h.UpdateDate,112)<@StartDate))))	
	AND	CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)
	AND (CONVERT(VARCHAR, HstITS.InquiryDate , 112) between CONVERT(VARCHAR, @StartDate, 112) and @EndDate or CONVERT(VARCHAR, HstITS.InquiryDate , 112) > CONVERT(VARCHAR, @EndDate, 112)
	OR (CONVERT(VARCHAR, HstITS.InquiryDate , 112) < CONVERT(VARCHAR, @StartDate, 112)  AND CONVERT(VARCHAR, Hist.UpdateDate , 112) between CONVERT(VARCHAR, @StartDate, 112) and CONVERT(VARCHAR, @EndDate, 112)))											 	
	GROUP BY Hist.CompanyCode, Hist.BranchCode, Emp.EmployeeID, Emp.EmployeeName, Hist.LastProgress, SpvEmp.EmployeeName, SpvEmp.EmployeeID, Emp.Position
	) a
	INNER JOIN  gnMstDealerMapping b WITH (NOLOCK, NOWAIT) on a.CompanyCode = b.DealerCode		
	WHERE (CASE WHEN @Area = '' THEN '' ELSE b.Area END) = @Area
			AND (CASE WHEN @DealerCode = '' THEN '' ELSE a.CompanyCode END) = @DealerCode
			AND (CASE WHEN @OutletCode = '' THEN '' ELSE a.BranchCode END) = @OutletCode
			AND (CASE WHEN @SalesHead = '' THEN '' ELSE (SELECT TeamLeader FROM HrEmployee WHERE CompanyCode = @DealerCode AND EmployeeID = a.SpvEmployeeID) END) = @SalesHead
			AND (CASE WHEN @Salesman = '' THEN '' ELSE a.EmployeeID END) = @Salesman
) #t1
SELECT CASE WHEN @TypeReport = '0' THEN 'Summary' ELSE 'Saldo' END TypeReport,
CASE @ProductivityBy 
	WHEN '0' THEN 'Salesman'
	WHEN '1' THEN 'Vehicle Type'
	WHEN '2' THEN 'Source Data'
END ProductivityBy,
CONVERT(VARCHAR, @EndDate, 105) PerDate, 
CASE WHEN @Area = '' THEN 'ALL' ELSE @Area END Area,
CONVERT(VARCHAR, @StartDate, 105) + ' s/d ' + CONVERT(VARCHAR, @EndDate, 105) PeriodeDO,
CASE WHEN @SalesHead = '' THEN 'ALL' ELSE (SELECT EmployeeName FROM HrEmployee WHERE CompanyCode = @DealerCode AND EmployeeID = @SalesHead) END SalesHead,
CASE WHEN @Salesman = '' THEN 'ALL' ELSE (SELECT EmployeeName FROM HrEmployee WHERE CompanyCode = @DealerCode AND EmployeeID = @Salesman) END Salesman,			
ISNULL((SELECT TOP 1 PosName FROM GnMstPosition where DeptCode = 'SALES' AND PosCode = a.Position), '') Position,
CASE WHEN @DealerCode = '' THEN 'ALL' ELSE (SELECT DealerName FROM  gnMstDealerMapping WHERE DealerCode = @DealerCode and Area = @Area ) END Dealer,
CASE WHEN @OutletCode = '' THEN 'ALL' ELSE (SELECT OutletName FROM  gnMstDealerOutletMapping WHERE DealerCode = @DealerCode AND OutletCode = @OutletCode) END Outlet,
ISNULL(a.EmployeeID, '') EmployeeID, ISNULL(a.Wiraniaga, '') EmployeeName, 
(SELECT ISNULL(SUM(countLastProg), 0) FROM #t1 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND EmployeeID = a.EmployeeID AND LastProgress = 'P' AND TypeReport = @TypeReport) NEW,
(SELECT ISNULL(SUM(countLastProg), 0) FROM #t1 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND EmployeeID = a.EmployeeID AND LastProgress = 'P' AND TypeReport = @TypeReport) P,
(SELECT ISNULL(SUM(countLastProg), 0) FROM #t1 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND EmployeeID = a.EmployeeID AND LastProgress = 'HP' AND TypeReport = @TypeReport) HP,
(SELECT ISNULL(SUM(countLastProg), 0) FROM #t1 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND EmployeeID = a.EmployeeID AND LastProgress = 'SPK' AND TypeReport = @TypeReport) SPK,
((SELECT ISNULL(SUM(countLastProg), 0) FROM #t1 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND EmployeeID = a.EmployeeID AND LastProgress = 'P' AND TypeReport = @TypeReport) +
(SELECT ISNULL(SUM(countLastProg), 0) FROM #t1 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND EmployeeID = a.EmployeeID AND LastProgress = 'HP' AND TypeReport = @TypeReport) +
(SELECT ISNULL(SUM(countLastProg), 0) FROM #t1 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND EmployeeID = a.EmployeeID AND LastProgress = 'SPK' AND TypeReport = @TypeReport)) SumOuts,
(SELECT ISNULL(SUM(countLastProg), 0) FROM #t1 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND EmployeeID = a.EmployeeID AND LastProgress = 'DO') DO,
(SELECT ISNULL(SUM(countLastProg), 0) FROM #t1 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND EmployeeID = a.EmployeeID AND LastProgress = 'DELIVERY') DELIVERY,
(SELECT ISNULL(SUM(countLastProg), 0) FROM #t1 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND EmployeeID = a.EmployeeID AND LastProgress = 'LOST') LOST
FROM #t1 a
GROUP BY a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Wiraniaga
ORDER BY  a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Wiraniaga
DROP TABLE #t1
END
ELSE IF @ProductivityBy = '1'
BEGIN
SELECT * INTO #t2 FROM (
	SELECT * FROM (
	SELECT '0' TypeReport, Hist.CompanyCode, Hist.BranchCode, ISNULL(HstITS.TipeKendaraan, '') TipeKendaraan, ISNULL(HstITS.Variant, '') Variant,
	Hist.LastProgress, COUNT(Hist.LastProgress) countLastProg, Emp.EmployeeID, SpvEmp.EmployeeName SalesCoordinator, SpvEmp.EmployeeID SpvEmployeeID, Emp.Position
	FROM  pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
	INNER JOIN  pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
		HstITS.CompanyCode = Hist.CompanyCode AND
		HstITS.BranchCode = Hist.BranchCode AND
		HstITS.InquiryNumber = Hist.InquiryNumber 
	LEFT JOIN pmKDP KDP WITH (NOLOCK, NOWAIT) ON 
		KDP.CompanyCode = Hist.CompanyCode AND
		KDP.BranchCode = Hist.BranchCode AND
		KDP.InquiryNumber = Hist.InquiryNumber
	LEFT JOIN HrEmployee Emp WITH (NOLOCK, NOWAIT) ON 
		Emp.CompanyCode = KDP.CompanyCode AND
		Emp.EmployeeID = KDP.EmployeeID
	LEFT JOIN HrEmployee SpvEmp WITH (NOLOCK, NOWAIT) ON 
		SpvEmp.CompanyCode = Emp.CompanyCode AND
		SpvEmp.EmployeeID = Emp.TeamLeader
	WHERE Hist.LastProgress = 'P'
		AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
		or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
		and (Hist.LastProgress='P'   and not exists (select top 1 1 from  pmStatusHistory h
	where h.CompanyCode=Hist.CompanyCode
		and h.BranchCode=Hist.BranchCode
		and h.InquiryNumber=Hist.InquiryNumber
		and h.LastProgress<>'P'
		and convert(varchar,h.UpdateDate,112)<@StartDate)
		or  Hist.LastProgress='HP'  and not exists (select top 1 1 from  pmStatusHistory h
		where h.CompanyCode=Hist.CompanyCode
			and h.BranchCode=Hist.BranchCode
			and h.InquiryNumber=Hist.InquiryNumber
			and h.LastProgress not in ('P','HP')
			and convert(varchar,h.UpdateDate,112)<@StartDate)
			or Hist.LastProgress='SPK' and not exists (select top 1 1 from  pmStatusHistory h
	where h.CompanyCode=Hist.CompanyCode
	and h.BranchCode=Hist.BranchCode
	and h.InquiryNumber=Hist.InquiryNumber
	and h.LastProgress not in ('P','HP','SPK')
	and convert(varchar,h.UpdateDate,112)<@StartDate))))	
	AND	CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)
	and (CONVERT(VARCHAR, HstITS.InquiryDate , 112) between CONVERT(VARCHAR, @StartDate, 112) and @EndDate or HstITS.InquiryDate > CONVERT(VARCHAR, @EndDate, 112))											 	
	GROUP BY Hist.CompanyCode, Hist.BranchCode, Hist.LastProgress, Emp.EmployeeID, SpvEmp.EmployeeName, SpvEmp.EmployeeID, Emp.Position, HstITS.TipeKendaraan, HstITS.Variant
	UNION ALL
	SELECT '0' TypeReport, Hist.CompanyCode, Hist.BranchCode,  
		ISNULL(HstITS.TipeKendaraan, '') TipeKendaraan, ISNULL(HstITS.Variant, '') Variant, Hist.LastProgress, 
		CASE WHEN Hist.LastProgress IN ('DO','P','DELIVERY','LOST') THEN 0 ELSE COUNT(Hist.LastProgress) END countLastProg, Emp.EmployeeID, SpvEmp.EmployeeName SalesCoordinator, 
		SpvEmp.EmployeeID SpvEmployeeID, Emp.Position
	FROM  pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
	INNER JOIN  pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
		HstITS.CompanyCode = Hist.CompanyCode AND
		HstITS.BranchCode = Hist.BranchCode AND
		HstITS.InquiryNumber = Hist.InquiryNumber 
	LEFT JOIN pmKDP KDP WITH (NOLOCK, NOWAIT) ON 
		KDP.CompanyCode = Hist.CompanyCode AND
		KDP.BranchCode = Hist.BranchCode AND
		KDP.InquiryNumber = Hist.InquiryNumber
	LEFT JOIN HrEmployee Emp WITH (NOLOCK, NOWAIT) ON 
		Emp.CompanyCode = KDP.CompanyCode AND
		Emp.EmployeeID = KDP.EmployeeID
	LEFT JOIN HrEmployee SpvEmp WITH (NOLOCK, NOWAIT) ON 
		SpvEmp.CompanyCode = Emp.CompanyCode AND
		SpvEmp.EmployeeID = Emp.TeamLeader
	WHERE (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
		or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
		and (Hist.LastProgress='P'   and not exists (select top 1 1 from  pmStatusHistory h
	where h.CompanyCode=Hist.CompanyCode
		and h.BranchCode=Hist.BranchCode
		and h.InquiryNumber=Hist.InquiryNumber
		and h.LastProgress<>'P'
		and convert(varchar,h.UpdateDate,112)<@StartDate)
		or  Hist.LastProgress='HP'  and not exists (select top 1 1 from  pmStatusHistory h
		where h.CompanyCode=Hist.CompanyCode
			and h.BranchCode=Hist.BranchCode
			and h.InquiryNumber=Hist.InquiryNumber
			and h.LastProgress not in ('P','HP')
			and convert(varchar,h.UpdateDate,112)<@StartDate)
			or Hist.LastProgress='SPK' and not exists (select top 1 1 from  pmStatusHistory h
	where h.CompanyCode=Hist.CompanyCode
	and h.BranchCode=Hist.BranchCode
	and h.InquiryNumber=Hist.InquiryNumber
	and h.LastProgress not in ('P','HP','SPK')
	and convert(varchar,h.UpdateDate,112)<@StartDate))))	
	AND	CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)
	AND (CONVERT(VARCHAR, HstITS.InquiryDate , 112) between CONVERT(VARCHAR, @StartDate, 112) and @EndDate or CONVERT(VARCHAR, HstITS.InquiryDate , 112) > CONVERT(VARCHAR, @EndDate, 112)
	OR (CONVERT(VARCHAR, HstITS.InquiryDate , 112) < CONVERT(VARCHAR, @StartDate, 112)  AND CONVERT(VARCHAR, Hist.UpdateDate , 112) between CONVERT(VARCHAR, @StartDate, 112) and CONVERT(VARCHAR, @EndDate, 112)))											 	
	GROUP BY Hist.CompanyCode, Hist.BranchCode, Hist.LastProgress, Emp.EmployeeID, SpvEmp.EmployeeName, SpvEmp.EmployeeID, Emp.Position, HstITS.TipeKendaraan, HstITS.Variant
	UNION ALL
	SELECT '1' TypeReport, Hist.CompanyCode, Hist.BranchCode, ISNULL(HstITS.TipeKendaraan, '') TipeKendaraan, ISNULL(HstITS.Variant, '') Variant,
	Hist.LastProgress, CASE WHEN Hist.LastProgress IN ('DO','DELIVERY','LOST') THEN 0 ELSE COUNT(Hist.LastProgress) END countLastProg, Emp.EmployeeID, 
	SpvEmp.EmployeeName SalesCoordinator, SpvEmp.EmployeeID SpvEmployeeID, Emp.Position
		FROM  pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
		INNER JOIN  pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
			HstITS.CompanyCode = Hist.CompanyCode AND
			HstITS.BranchCode = Hist.BranchCode AND
			HstITS.InquiryNumber = Hist.InquiryNumber 
		LEFT JOIN pmKDP KDP WITH (NOLOCK, NOWAIT) ON 
			KDP.CompanyCode = Hist.CompanyCode AND
			KDP.BranchCode = Hist.BranchCode AND
			KDP.InquiryNumber = Hist.InquiryNumber
		LEFT JOIN HrEmployee Emp WITH (NOLOCK, NOWAIT) ON 
			Emp.CompanyCode = KDP.CompanyCode AND
			Emp.EmployeeID = KDP.EmployeeID
		LEFT JOIN HrEmployee SpvEmp WITH (NOLOCK, NOWAIT) ON 
			SpvEmp.CompanyCode = Emp.CompanyCode AND
			SpvEmp.EmployeeID = Emp.TeamLeader
	 WHERE (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
			 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
			and (Hist.LastProgress='P'   and not exists (select top 1 1 from  pmStatusHistory h
													   where h.CompanyCode=Hist.CompanyCode
														 and h.BranchCode=Hist.BranchCode
														 and h.InquiryNumber=Hist.InquiryNumber
														 and h.LastProgress<>'P'
														 and convert(varchar,h.UpdateDate,112)<@StartDate)
			 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from  pmStatusHistory h
													   where h.CompanyCode=Hist.CompanyCode
														 and h.BranchCode=Hist.BranchCode
														 and h.InquiryNumber=Hist.InquiryNumber
														 and h.LastProgress not in ('P','HP')
														 and convert(varchar,h.UpdateDate,112)<@StartDate)
			 or Hist.LastProgress='SPK' and not exists (select top 1 1 from  pmStatusHistory h
													   where h.CompanyCode=Hist.CompanyCode
														 and h.BranchCode=Hist.BranchCode
														 and h.InquiryNumber=Hist.InquiryNumber
														 and h.LastProgress not in ('P','HP','SPK')
														 and convert(varchar,h.UpdateDate,112)<@StartDate))))	
		AND	CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)
		AND (CONVERT(VARCHAR, HstITS.InquiryDate , 112) between CONVERT(VARCHAR, @StartDate, 112) and @EndDate or CONVERT(VARCHAR, HstITS.InquiryDate , 112) > CONVERT(VARCHAR, @EndDate, 112)
		OR (CONVERT(VARCHAR, HstITS.InquiryDate , 112) < CONVERT(VARCHAR, @StartDate, 112)  AND CONVERT(VARCHAR, Hist.UpdateDate , 112) between CONVERT(VARCHAR, @StartDate, 112) and CONVERT(VARCHAR, @EndDate, 112)))											 	
		AND CONVERT(VARCHAR, HstITS.LastUpdateDate, 112) <= CONVERT(VARCHAR, @EndDate, 112) 
		and HstITS.LastProgress not in ('DO','DELIVERY','LOST')															 
	GROUP BY Hist.CompanyCode, Hist.BranchCode, Hist.LastProgress, Emp.EmployeeID, SpvEmp.EmployeeName, SpvEmp.EmployeeID, Emp.Position, HstITS.TipeKendaraan, HstITS.Variant
	UNION ALL		
	SELECT '' TypeReport, DOH.CompanyCode, DOH.BranchCode, ISNULL(Model.GroupCode, '') TipeKendaraan, ISNULL(Model.TypeCode, '') Variant, 'DO' LastProgress, 
		COUNT(DOD.DONo) countLastProg, Emp.EmployeeID, SpvEmp.EmployeeName SalesCoordinator, SpvEmp.EmployeeID SpvEmployeeID, Emp.Position			
	FROM omTrSalesDO DOH WITH (NOLOCK, NOWAIT)
	INNER JOIN omTrSalesDODetail DOD WITH (NOLOCK, NOWAIT) ON 
		DOH.CompanyCode = DOD.CompanyCode AND
		DOH.BranchCode = DOD.BranchCode AND
		DOH.DONo = DOD.DONo
	INNER JOIN omMstModel Model WITH (NOLOCK, NOWAIT) ON 
		Model.CompanyCode = DOD.CompanyCode AND
		Model.SalesModelCode = DOD.SalesModelCode 
	 INNER JOIN omTrSalesSO SOH WITH (NOLOCK, NOWAIT) ON 
		SOH.CompanyCode = DOH.CompanyCode AND
		SOH.BranchCode = DOH.BranchCode AND
		SOH.SONo = DOH.SONo
	 LEFT JOIN HrEmployee Emp WITH (NOLOCK, NOWAIT) ON 
		Emp.CompanyCode = SOH.CompanyCode AND
		Emp.EmployeeID = SOH.Salesman
	 LEFT JOIN HrEmployee SpvEmp WITH (NOLOCK, NOWAIT) ON 
		SpvEmp.CompanyCode = SOH.CompanyCode AND
		SpvEmp.EmployeeID = Emp.TeamLeader
	 WHERE  DOH.Status = '2'
		AND CONVERT(VARCHAR, DOH.DODate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 
	GROUP BY DOH.CompanyCode, DOH.BranchCode, Emp.EmployeeID, Emp.EmployeeName,SpvEmp.EmployeeName, SpvEmp.EmployeeID, Emp.Position, Model.GroupCode,  Model.TypeCode
	UNION ALL
	SELECT '' TypeReport, SJH.CompanyCode, SJH.BranchCode, ISNULL(Model.GroupCode, '') TipeKendaraan, ISNULL(Model.TypeCode, '') Variant, 
	'DELIVERY' LastProgress,COUNT(SJD.BPKNo) countLastProg, Emp.EmployeeID, SpvEmp.EmployeeName SalesCoordinator, SpvEmp.EmployeeID SpvEmployeeID, Emp.Position	
	FROM omTrSalesBPK SJH WITH (NOLOCK, NOWAIT)
	INNER JOIN omTrSalesBPKDetail SJD WITH (NOLOCK, NOWAIT) ON 
		SJH.CompanyCode = SJD.CompanyCode AND
		SJH.BranchCode = SJD.BranchCode AND
		SJH.BPKNo = SJD.BPKNo 
	INNER JOIN omMstModel Model WITH (NOLOCK, NOWAIT) ON 
		Model.CompanyCode = SJD.CompanyCode AND
		Model.SalesModelCode = SJD.SalesModelCode 
	INNER JOIN omTrSalesSO SOH WITH (NOLOCK, NOWAIT) ON 
		SOH.CompanyCode = SJH.CompanyCode AND
		SOH.BranchCode = SJH.BranchCode AND
		SOH.SONo = SJH.SONo 
	LEFT JOIN HrEmployee Emp WITH (NOLOCK, NOWAIT) ON 
		Emp.CompanyCode = SOH.CompanyCode AND
		Emp.EmployeeID = SOH.Salesman
	LEFT JOIN HrEmployee SpvEmp WITH (NOLOCK, NOWAIT) ON 
		SpvEmp.CompanyCode = SOH.CompanyCode AND
		SpvEmp.EmployeeID = Emp.TeamLeader
	 WHERE  SJH.Status = '2'
		AND CONVERT(VARCHAR, SJH.BPKDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 
	GROUP BY SJH.CompanyCode, SJH.BranchCode, Emp.EmployeeID, SpvEmp.EmployeeName, SpvEmp.EmployeeID, Emp.Position, Model.GroupCode,  Model.TypeCode
	UNION ALL
	SELECT '' TypeReport, Hist.CompanyCode, Hist.BranchCode, ISNULL(HstITS.TipeKendaraan, '') TipeKendaraan, ISNULL(HstITS.Variant, '') Variant,
	'LOST' LastProgress, COUNT(Hist.LastProgress) countLastProg, Emp.EmployeeID, SpvEmp.EmployeeName SalesCoordinator, SpvEmp.EmployeeID SpvEmployeeID, 
	Emp.Position
	FROM  pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
	INNER JOIN  pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
		HstITS.CompanyCode = Hist.CompanyCode AND
		HstITS.BranchCode = Hist.BranchCode AND
		HstITS.InquiryNumber = Hist.InquiryNumber 
	INNER JOIN pmKDP KDP WITH (NOLOCK, NOWAIT) ON 
		KDP.CompanyCode = Hist.CompanyCode AND
		KDP.BranchCode = Hist.BranchCode AND
		KDP.InquiryNumber = Hist.InquiryNumber
	LEFT JOIN HrEmployee Emp WITH (NOLOCK, NOWAIT) ON 
		Emp.CompanyCode = KDP.CompanyCode AND
		Emp.EmployeeID = KDP.EmployeeID
	LEFT JOIN HrEmployee SpvEmp WITH (NOLOCK, NOWAIT) ON 
		SpvEmp.CompanyCode = Emp.CompanyCode AND
		SpvEmp.EmployeeID = Emp.TeamLeader
	WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
		AND Hist.LastProgress = 'LOST'
		AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
		or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
		and (Hist.LastProgress='P'   and not exists (select top 1 1 from  pmStatusHistory h
	where h.CompanyCode=Hist.CompanyCode
		and h.BranchCode=Hist.BranchCode
		and h.InquiryNumber=Hist.InquiryNumber
		and h.LastProgress<>'P'
		and convert(varchar,h.UpdateDate,112)<@StartDate)
		or  Hist.LastProgress='HP'  and not exists (select top 1 1 from  pmStatusHistory h
		where h.CompanyCode=Hist.CompanyCode
			and h.BranchCode=Hist.BranchCode
			and h.InquiryNumber=Hist.InquiryNumber
			and h.LastProgress not in ('P','HP')
			and convert(varchar,h.UpdateDate,112)<@StartDate)
			or Hist.LastProgress='SPK' and not exists (select top 1 1 from  pmStatusHistory h
	where h.CompanyCode=Hist.CompanyCode
	and h.BranchCode=Hist.BranchCode
	and h.InquiryNumber=Hist.InquiryNumber
	and h.LastProgress not in ('P','HP','SPK')
	and convert(varchar,h.UpdateDate,112)<@StartDate))))	
	AND	CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)
	AND (CONVERT(VARCHAR, HstITS.InquiryDate , 112) between CONVERT(VARCHAR, @StartDate, 112) and @EndDate or CONVERT(VARCHAR, HstITS.InquiryDate , 112) > CONVERT(VARCHAR, @EndDate, 112)
	OR (CONVERT(VARCHAR, HstITS.InquiryDate , 112) < CONVERT(VARCHAR, @StartDate, 112)  AND CONVERT(VARCHAR, Hist.UpdateDate , 112) between CONVERT(VARCHAR, @StartDate, 112) and CONVERT(VARCHAR, @EndDate, 112)))											 	
	GROUP BY Hist.CompanyCode, Hist.BranchCode, Hist.LastProgress, Emp.EmployeeID, SpvEmp.EmployeeName, SpvEmp.EmployeeID, Emp.Position, HstITS.TipeKendaraan, HstITS.Variant
	) a
	INNER JOIN  gnMstDealerMapping b WITH (NOLOCK, NOWAIT) on a.CompanyCode = b.DealerCode		
	WHERE (CASE WHEN @Area = '' THEN '' ELSE b.Area END) = @Area
			AND (CASE WHEN @DealerCode = '' THEN '' ELSE a.CompanyCode END) = @DealerCode
			AND (CASE WHEN @OutletCode = '' THEN '' ELSE a.BranchCode END) = @OutletCode
			AND (CASE WHEN @SalesHead = '' THEN '' ELSE (SELECT TeamLeader FROM HrEmployee WHERE CompanyCode = @DealerCode AND EmployeeID = a.SpvEmployeeID) END) = @SalesHead
			AND (CASE WHEN @Salesman = '' THEN '' ELSE a.EmployeeID END) = @Salesman
) #t2
SELECT TypeReport, ProductivityBy,PerDate,Area,PeriodeDO,SalesHead,Salesman,Dealer,Outlet,TipeKendaraan,SUM(NEW)NEW,SUM(P)P,SUM(HP)HP,SUM(SPK)SPK,SUM(DO)DO,
SUM(DELIVERY)DELIVERY,SUM(LOST)LOST FROM(
SELECT CASE WHEN @TypeReport = '0' THEN 'Summary' ELSE 'Saldo' END TypeReport,
	CASE @ProductivityBy 
		WHEN '0' THEN 'Salesman'
		WHEN '1' THEN 'Vehicle Type'
		WHEN '2' THEN 'Source Data'
	END ProductivityBy,
	CONVERT(VARCHAR, @EndDate, 105) PerDate, CASE WHEN @Area = '' THEN 'ALL' ELSE @Area END Area, CONVERT(VARCHAR, @StartDate, 105) + ' s/d ' + CONVERT(VARCHAR, @EndDate, 105) PeriodeDO,
	CASE WHEN @SalesHead = '' THEN 'ALL' ELSE (SELECT EmployeeName FROM HrEmployee WHERE CompanyCode = @DealerCode AND EmployeeID = @SalesHead) END SalesHead,
	CASE WHEN @Salesman = '' THEN 'ALL' ELSE (SELECT EmployeeName FROM HrEmployee WHERE CompanyCode = @DealerCode AND EmployeeID = @Salesman) END Salesman,
	CASE WHEN @DealerCode = '' THEN 'ALL' ELSE (SELECT DealerName FROM  gnMstDealerMapping WHERE DealerCode = @DealerCode and Area = @Area) END Dealer,
	CASE WHEN @OutletCode = '' THEN 'ALL' ELSE (SELECT OutletName FROM  gnMstDealerOutletMapping WHERE DealerCode = @DealerCode AND OutletCode = @OutletCode) END Outlet,
	a.TipeKendaraan, 
	(SELECT ISNULL(SUM(countLastProg), 0) FROM #t2 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = a.TipeKendaraan AND Variant = a.Variant AND LastProgress = 'P' AND TypeReport = @TypeReport) NEW,
	(SELECT ISNULL(SUM(countLastProg), 0) FROM #t2 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = a.TipeKendaraan AND Variant = a.Variant AND LastProgress = 'P' AND TypeReport = @TypeReport) P,
	(SELECT ISNULL(SUM(countLastProg), 0) FROM #t2 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = a.TipeKendaraan AND Variant = a.Variant AND LastProgress = 'HP' AND TypeReport = @TypeReport) HP,
	(SELECT ISNULL(SUM(countLastProg), 0) FROM #t2 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = a.TipeKendaraan AND Variant = a.Variant AND LastProgress = 'SPK' AND TypeReport = @TypeReport) SPK,
	((SELECT ISNULL(SUM(countLastProg), 0) FROM #t2 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = a.TipeKendaraan AND Variant = a.Variant AND LastProgress = 'P' AND TypeReport = @TypeReport) +
	(SELECT ISNULL(SUM(countLastProg), 0) FROM #t2 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = a.TipeKendaraan AND Variant = a.Variant AND LastProgress = 'HP' AND TypeReport = @TypeReport) +
	(SELECT ISNULL(SUM(countLastProg), 0) FROM #t2 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = a.TipeKendaraan AND Variant = a.Variant AND LastProgress = 'SPK' AND TypeReport = @TypeReport)) SumOuts,
	(SELECT ISNULL(SUM(countLastProg), 0) FROM #t2 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = a.TipeKendaraan AND Variant = a.Variant AND LastProgress = 'DO') DO,
	(SELECT ISNULL(SUM(countLastProg), 0) FROM #t2 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = a.TipeKendaraan AND Variant = a.Variant AND LastProgress = 'DELIVERY') DELIVERY,
	(SELECT ISNULL(SUM(countLastProg), 0) FROM #t2 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = a.TipeKendaraan AND Variant = a.Variant AND LastProgress = 'LOST') LOST
FROM #t2 a	
GROUP BY a.CompanyCode, a.BranchCode, a.TipeKendaraan, a.Variant
) a
GROUP BY TypeReport, ProductivityBy,PerDate,Area,PeriodeDO,SalesHead,Salesman,Dealer,Outlet,TipeKendaraan
ORDER BY  TipeKendaraan
DROP TABLE #t2
END
ELSE IF @ProductivityBy = '2'
BEGIN
SELECT * INTO #t3 FROM (
	SELECT * FROM (
	SELECT '0' TypeReport, Hist.CompanyCode, Hist.BranchCode, ISNULL(HstIts.PerolehanData, '') PerolehanData,Hist.LastProgress, 
	COUNT(Hist.LastProgress) countLastProg, Emp.EmployeeID, SpvEmp.EmployeeName SalesCoordinator, SpvEmp.EmployeeID SpvEmployeeID, Emp.Position
	FROM  pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
	INNER JOIN  pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
		HstITS.CompanyCode = Hist.CompanyCode AND
		HstITS.BranchCode = Hist.BranchCode AND
		HstITS.InquiryNumber = Hist.InquiryNumber 
	LEFT JOIN pmKDP KDP WITH (NOLOCK, NOWAIT) ON 
		KDP.CompanyCode = Hist.CompanyCode AND
		KDP.BranchCode = Hist.BranchCode AND
		KDP.InquiryNumber = Hist.InquiryNumber
	LEFT JOIN HrEmployee Emp WITH (NOLOCK, NOWAIT) ON 
		Emp.CompanyCode = KDP.CompanyCode AND
		Emp.EmployeeID = KDP.EmployeeID
	LEFT JOIN HrEmployee SpvEmp WITH (NOLOCK, NOWAIT) ON 
		SpvEmp.CompanyCode = Emp.CompanyCode AND
		SpvEmp.EmployeeID = Emp.TeamLeader
	INNER JOIN  gnMstDealerMapping b WITH (NOLOCK, NOWAIT) on Hist.CompanyCode = b.DealerCode		
	WHERE Hist.LastProgress = 'P'
		AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
		or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
		and (Hist.LastProgress='P'   and not exists (select top 1 1 from  pmStatusHistory h
	where h.CompanyCode=Hist.CompanyCode
		and h.BranchCode=Hist.BranchCode
		and h.InquiryNumber=Hist.InquiryNumber
		and h.LastProgress<>'P'
		and convert(varchar,h.UpdateDate,112)<@StartDate)
		or  Hist.LastProgress='HP'  and not exists (select top 1 1 from  pmStatusHistory h
		where h.CompanyCode=Hist.CompanyCode
			and h.BranchCode=Hist.BranchCode
			and h.InquiryNumber=Hist.InquiryNumber
			and h.LastProgress not in ('P','HP')
			and convert(varchar,h.UpdateDate,112)<@StartDate)
			or Hist.LastProgress='SPK' and not exists (select top 1 1 from  pmStatusHistory h
	where h.CompanyCode=Hist.CompanyCode
	and h.BranchCode=Hist.BranchCode
	and h.InquiryNumber=Hist.InquiryNumber
	and h.LastProgress not in ('P','HP','SPK')
	and convert(varchar,h.UpdateDate,112)<@StartDate))))	
	AND	CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)
	and (CONVERT(VARCHAR, HstITS.InquiryDate , 112) between CONVERT(VARCHAR, @StartDate, 112) and @EndDate or HstITS.InquiryDate > CONVERT(VARCHAR, @EndDate, 112))											 	
	GROUP BY Hist.CompanyCode, Hist.BranchCode, Hist.LastProgress, Emp.EmployeeID, SpvEmp.EmployeeName, SpvEmp.EmployeeID, Emp.Position, HstIts.PerolehanData
	UNION ALL
	SELECT '0' TypeReport, Hist.CompanyCode, Hist.BranchCode,ISNULL(HstIts.PerolehanData, '') PerolehanData, Hist.LastProgress, 
		CASE WHEN Hist.LastProgress IN ('DO','P','DELIVERY', 'LOST') THEN 0 ELSE COUNT(Hist.LastProgress) END countLastProg, Emp.EmployeeID, SpvEmp.EmployeeName SalesCoordinator, 
		SpvEmp.EmployeeID SpvEmployeeID, Emp.Position
	FROM  pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
	INNER JOIN  pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
		HstITS.CompanyCode = Hist.CompanyCode AND
		HstITS.BranchCode = Hist.BranchCode AND
		HstITS.InquiryNumber = Hist.InquiryNumber 
	LEFT JOIN pmKDP KDP WITH (NOLOCK, NOWAIT) ON 
		KDP.CompanyCode = Hist.CompanyCode AND
		KDP.BranchCode = Hist.BranchCode AND
		KDP.InquiryNumber = Hist.InquiryNumber
	LEFT JOIN HrEmployee Emp WITH (NOLOCK, NOWAIT) ON 
		Emp.CompanyCode = KDP.CompanyCode AND
		Emp.EmployeeID = KDP.EmployeeID
	LEFT JOIN HrEmployee SpvEmp WITH (NOLOCK, NOWAIT) ON 
		SpvEmp.CompanyCode = Emp.CompanyCode AND
		SpvEmp.EmployeeID = Emp.TeamLeader
	WHERE (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
		or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
		and (Hist.LastProgress='P'   and not exists (select top 1 1 from  pmStatusHistory h
	where h.CompanyCode=Hist.CompanyCode
		and h.BranchCode=Hist.BranchCode
		and h.InquiryNumber=Hist.InquiryNumber
		and h.LastProgress<>'P'
		and convert(varchar,h.UpdateDate,112)<@StartDate)
		or  Hist.LastProgress='HP'  and not exists (select top 1 1 from  pmStatusHistory h
		where h.CompanyCode=Hist.CompanyCode
			and h.BranchCode=Hist.BranchCode
			and h.InquiryNumber=Hist.InquiryNumber
			and h.LastProgress not in ('P','HP')
			and convert(varchar,h.UpdateDate,112)<@StartDate)
			or Hist.LastProgress='SPK' and not exists (select top 1 1 from  pmStatusHistory h
	where h.CompanyCode=Hist.CompanyCode
	and h.BranchCode=Hist.BranchCode
	and h.InquiryNumber=Hist.InquiryNumber
	and h.LastProgress not in ('P','HP','SPK')
	and convert(varchar,h.UpdateDate,112)<@StartDate))))	
	AND	CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)
	AND (CONVERT(VARCHAR, HstITS.InquiryDate , 112) between CONVERT(VARCHAR, @StartDate, 112) and @EndDate or CONVERT(VARCHAR, HstITS.InquiryDate , 112) > CONVERT(VARCHAR, @EndDate, 112)
	OR (CONVERT(VARCHAR, HstITS.InquiryDate , 112) < CONVERT(VARCHAR, @StartDate, 112)  AND CONVERT(VARCHAR, Hist.UpdateDate , 112) between CONVERT(VARCHAR, @StartDate, 112) and CONVERT(VARCHAR, @EndDate, 112)))											 	
	GROUP BY Hist.CompanyCode, Hist.BranchCode, Hist.LastProgress, Emp.EmployeeID, SpvEmp.EmployeeName, SpvEmp.EmployeeID, Emp.Position, HstIts.PerolehanData
	UNION ALL
	SELECT '1' TypeReport, Hist.CompanyCode, Hist.BranchCode, ISNULL(HstIts.PerolehanData, '') PerolehanData,
		Hist.LastProgress, CASE WHEN Hist.LastProgress IN ('DO','DELIVERY','LOST') THEN 0 ELSE COUNT(Hist.LastProgress) END countLastProg, Emp.EmployeeID, 
		SpvEmp.EmployeeName SalesCoordinator, SpvEmp.EmployeeID SpvEmployeeID, Emp.Position
		FROM  pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
		INNER JOIN  pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
			HstITS.CompanyCode = Hist.CompanyCode AND
			HstITS.BranchCode = Hist.BranchCode AND
			HstITS.InquiryNumber = Hist.InquiryNumber 
		LEFT JOIN pmKDP KDP WITH (NOLOCK, NOWAIT) ON 
			KDP.CompanyCode = Hist.CompanyCode AND
			KDP.BranchCode = Hist.BranchCode AND
			KDP.InquiryNumber = Hist.InquiryNumber
		LEFT JOIN HrEmployee Emp WITH (NOLOCK, NOWAIT) ON 
			Emp.CompanyCode = KDP.CompanyCode AND
			Emp.EmployeeID = KDP.EmployeeID
		LEFT JOIN HrEmployee SpvEmp WITH (NOLOCK, NOWAIT) ON 
			SpvEmp.CompanyCode = Emp.CompanyCode AND
			SpvEmp.EmployeeID = Emp.TeamLeader
		 WHERE (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
			 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
			and (Hist.LastProgress='P'   and not exists (select top 1 1 from  pmStatusHistory h
													   where h.CompanyCode=Hist.CompanyCode
														 and h.BranchCode=Hist.BranchCode
														 and h.InquiryNumber=Hist.InquiryNumber
														 and h.LastProgress<>'P'
														 and convert(varchar,h.UpdateDate,112)<@StartDate)
			 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from  pmStatusHistory h
													   where h.CompanyCode=Hist.CompanyCode
														 and h.BranchCode=Hist.BranchCode
														 and h.InquiryNumber=Hist.InquiryNumber
														 and h.LastProgress not in ('P','HP')
														 and convert(varchar,h.UpdateDate,112)<@StartDate)
			 or Hist.LastProgress='SPK' and not exists (select top 1 1 from  pmStatusHistory h
													   where h.CompanyCode=Hist.CompanyCode
														 and h.BranchCode=Hist.BranchCode
														 and h.InquiryNumber=Hist.InquiryNumber
														 and h.LastProgress not in ('P','HP','SPK')
														 and convert(varchar,h.UpdateDate,112)<@StartDate))))	
		AND	CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)
		AND (CONVERT(VARCHAR, HstITS.InquiryDate , 112) between CONVERT(VARCHAR, @StartDate, 112) and @EndDate or CONVERT(VARCHAR, HstITS.InquiryDate , 112) > CONVERT(VARCHAR, @EndDate, 112)
		OR (CONVERT(VARCHAR, HstITS.InquiryDate , 112) < CONVERT(VARCHAR, @StartDate, 112)  AND CONVERT(VARCHAR, Hist.UpdateDate , 112) between CONVERT(VARCHAR, @StartDate, 112) and CONVERT(VARCHAR, @EndDate, 112)))											 	
		AND CONVERT(VARCHAR, HstITS.LastUpdateDate, 112) <= CONVERT(VARCHAR, @EndDate, 112) 
		and HstITS.LastProgress not in ('DO','DELIVERY','LOST')															 
		GROUP BY Hist.CompanyCode, Hist.BranchCode, Hist.LastProgress, Emp.EmployeeID, SpvEmp.EmployeeName, SpvEmp.EmployeeID, Emp.Position, HstIts.PerolehanData
	UNION ALL
	SELECT '' TypeReport, DOH.CompanyCode, DOH.BranchCode, ISNULL(HstIts.PerolehanData, '') PerolehanData, 'DO' LastProgress, 
		COUNT(DOD.DONo) countLastProg, Emp.EmployeeID, SpvEmp.EmployeeName SalesCoordinator, SpvEmp.EmployeeID SpvEmployeeID, Emp.Position			
	FROM omTrSalesDO DOH WITH (NOLOCK, NOWAIT)
	INNER JOIN omTrSalesDODetail DOD WITH (NOLOCK, NOWAIT) ON 
		DOH.CompanyCode = DOD.CompanyCode AND
		DOH.BranchCode = DOD.BranchCode AND
		DOH.DONo = DOD.DONo
	 INNER JOIN omTrSalesSO SOH WITH (NOLOCK, NOWAIT) ON 
		SOH.CompanyCode = DOH.CompanyCode AND
		SOH.BranchCode = DOH.BranchCode AND
		SOH.SONo = DOH.SONo
	LEFT JOIN  pmKDP KDP WITH (NOLOCK, NOWAIT) ON 
		KDP.CompanyCode = SOH.CompanyCode AND
		KDP.BranchCode = SOH.BranchCode AND
		KDP.InquiryNumber = SOH.ProspectNo 
	LEFT JOIN  pmHstITS hstITS WITH (NOLOCK, NOWAIT) ON 
		hstITS.CompanyCode = KDP.CompanyCode AND
		hstITS.BranchCode = KDP.BranchCode AND
		hstITS.InquiryNumber = KDP.InquiryNumber 
	LEFT JOIN HrEmployee Emp WITH (NOLOCK, NOWAIT) ON 
		Emp.CompanyCode = SOH.CompanyCode AND
		Emp.EmployeeID = SOH.Salesman
	LEFT JOIN HrEmployee SpvEmp WITH (NOLOCK, NOWAIT) ON 
		SpvEmp.CompanyCode = SOH.CompanyCode AND
		SpvEmp.EmployeeID = Emp.TeamLeader
	 WHERE  DOH.Status = '2'
		AND CONVERT(VARCHAR, DOH.DODate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 
	GROUP BY DOH.CompanyCode, DOH.BranchCode, Emp.EmployeeID, Emp.EmployeeName,SpvEmp.EmployeeName, SpvEmp.EmployeeID, Emp.Position, HstIts.PerolehanData
		UNION ALL
		SELECT '' TypeReport, SJH.CompanyCode, SJH.BranchCode, ISNULL(HstIts.PerolehanData, '') PerolehanData, 
		'DELIVERY' LastProgress,COUNT(SJD.BPKNo) countLastProg, Emp.EmployeeID, SpvEmp.EmployeeName SalesCoordinator, SpvEmp.EmployeeID SpvEmployeeID, Emp.Position	
		FROM omTrSalesBPK SJH WITH (NOLOCK, NOWAIT)
		INNER JOIN omTrSalesBPKDetail SJD WITH (NOLOCK, NOWAIT) ON 
			SJH.CompanyCode = SJD.CompanyCode AND
			SJH.BranchCode = SJD.BranchCode AND
			SJH.BPKNo = SJD.BPKNo 
		INNER JOIN omTrSalesSO SOH WITH (NOLOCK, NOWAIT) ON 
			SOH.CompanyCode = SJH.CompanyCode AND
			SOH.BranchCode = SJH.BranchCode AND
			SOH.SONo = SJH.SONo 
		LEFT JOIN  pmKDP KDP WITH (NOLOCK, NOWAIT) ON 
			KDP.CompanyCode = SOH.CompanyCode AND
			KDP.BranchCode = SOH.BranchCode AND
			KDP.InquiryNumber = SOH.ProspectNo 
		LEFT JOIN  pmHstITS hstITS WITH (NOLOCK, NOWAIT) ON 
			hstITS.CompanyCode = KDP.CompanyCode AND
			hstITS.BranchCode = KDP.BranchCode AND
			hstITS.InquiryNumber = KDP.InquiryNumber 	
		LEFT JOIN HrEmployee Emp WITH (NOLOCK, NOWAIT) ON 
			Emp.CompanyCode = SOH.CompanyCode AND
			Emp.EmployeeID = SOH.Salesman
		LEFT JOIN HrEmployee SpvEmp WITH (NOLOCK, NOWAIT) ON 
			SpvEmp.CompanyCode = SOH.CompanyCode AND
			SpvEmp.EmployeeID = Emp.TeamLeader
		 WHERE  SJH.Status = '2'
			AND CONVERT(VARCHAR, SJH.BPKDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 
		GROUP BY SJH.CompanyCode, SJH.BranchCode, Emp.EmployeeID, SpvEmp.EmployeeName, SpvEmp.EmployeeID, Emp.Position, HstIts.PerolehanData
		UNION ALL
		SELECT '' TypeReport, Hist.CompanyCode, Hist.BranchCode, ISNULL(HstIts.PerolehanData, '') PerolehanData, 'LOST' LastProgress, 
		COUNT(Hist.LastProgress) countLastProg, Emp.EmployeeID, SpvEmp.EmployeeName SalesCoordinator, SpvEmp.EmployeeID SpvEmployeeID, Emp.Position
		FROM  pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
		INNER JOIN  pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
			HstITS.CompanyCode = Hist.CompanyCode AND
			HstITS.BranchCode = Hist.BranchCode AND
			HstITS.InquiryNumber = Hist.InquiryNumber 
		LEFT JOIN pmKDP KDP WITH (NOLOCK, NOWAIT) ON 
			KDP.CompanyCode = Hist.CompanyCode AND
			KDP.BranchCode = Hist.BranchCode AND
			KDP.InquiryNumber = Hist.InquiryNumber
		LEFT JOIN HrEmployee Emp WITH (NOLOCK, NOWAIT) ON 
			Emp.CompanyCode = KDP.CompanyCode AND
			Emp.EmployeeID = KDP.EmployeeID
		LEFT JOIN HrEmployee SpvEmp WITH (NOLOCK, NOWAIT) ON 
			SpvEmp.CompanyCode = Emp.CompanyCode AND
			SpvEmp.EmployeeID = Emp.TeamLeader
		 WHERE Hist.LastProgress='LOST' 
			AND CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 
		GROUP BY Hist.CompanyCode, Hist.BranchCode, Hist.LastProgress, Emp.EmployeeID, SpvEmp.EmployeeName, SpvEmp.EmployeeID, Emp.Position, HstIts.PerolehanData
	) a
	INNER JOIN  gnMstDealerMapping b WITH (NOLOCK, NOWAIT) on a.CompanyCode = b.DealerCode		
	WHERE (CASE WHEN @Area = '' THEN '' ELSE b.Area END) = @Area
			AND (CASE WHEN @DealerCode = '' THEN '' ELSE a.CompanyCode END) = @DealerCode
			AND (CASE WHEN @OutletCode = '' THEN '' ELSE a.BranchCode END) = @OutletCode
			AND (CASE WHEN @SalesHead = '' THEN '' ELSE (SELECT TeamLeader FROM HrEmployee WHERE CompanyCode = @DealerCode AND EmployeeID = a.SpvEmployeeID) END) = @SalesHead
			AND (CASE WHEN @Salesman = '' THEN '' ELSE a.EmployeeID END) = @Salesman
) #t3
SELECT TypeReport, ProductivityBy,PerDate,Area,PeriodeDO,SalesHead,Salesman,Dealer,Outlet,Source,SUM(NEW)NEW,SUM(P)P,SUM(HP)HP,SUM(SPK)SPK,SUM(DO)DO,
SUM(DELIVERY)DELIVERY,SUM(LOST)LOST FROM(
SELECT CASE WHEN @TypeReport = '0' THEN 'Summary' ELSE 'Saldo' END TypeReport,
	CASE @ProductivityBy 
		WHEN '0' THEN 'Salesman'
		WHEN '1' THEN 'Vehicle Type'
		WHEN '2' THEN 'Source Data'
	END ProductivityBy,
	CONVERT(VARCHAR, @EndDate, 105) PerDate, CASE WHEN @Area = '' THEN 'ALL' ELSE @Area END Area, CONVERT(VARCHAR, @StartDate, 105) + ' s/d ' + CONVERT(VARCHAR, @EndDate, 105) PeriodeDO,
	CASE WHEN @SalesHead = '' THEN 'ALL' ELSE (SELECT EmployeeName FROM HrEmployee WHERE CompanyCode = @DealerCode AND EmployeeID = @SalesHead) END SalesHead,
	CASE WHEN @Salesman = '' THEN 'ALL' ELSE (SELECT EmployeeName FROM HrEmployee WHERE CompanyCode = @DealerCode AND EmployeeID = @Salesman) END Salesman,
	CASE WHEN @DealerCode = '' THEN 'ALL' ELSE (SELECT DealerName FROM  gnMstDealerMapping WHERE DealerCode = @DealerCode and Area = @Area) END Dealer,
	CASE WHEN @OutletCode = '' THEN 'ALL' ELSE (SELECT OutletName FROM  gnMstDealerOutletMapping WHERE DealerCode = @DealerCode AND OutletCode = @OutletCode) END Outlet,
	ISNULL(a.PerolehanData, '') Source,
	(SELECT ISNULL(SUM(countLastProg), 0) FROM #t3 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = a.PerolehanData AND LastProgress = 'P' AND TypeReport = @TypeReport) NEW,
	(SELECT ISNULL(SUM(countLastProg), 0) FROM #t3 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = a.PerolehanData AND LastProgress = 'P' AND TypeReport = @TypeReport) P,
	(SELECT ISNULL(SUM(countLastProg), 0) FROM #t3 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = a.PerolehanData AND LastProgress = 'HP' AND TypeReport = @TypeReport) HP,
	(SELECT ISNULL(SUM(countLastProg), 0) FROM #t3 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = a.PerolehanData AND LastProgress = 'SPK' AND TypeReport = @TypeReport) SPK,
	((SELECT ISNULL(SUM(countLastProg), 0) FROM #t3 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = a.PerolehanData AND LastProgress = 'P' AND TypeReport = @TypeReport) +
	(SELECT ISNULL(SUM(countLastProg), 0) FROM #t3 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = a.PerolehanData AND LastProgress = 'HP' AND TypeReport = @TypeReport) +
	(SELECT ISNULL(SUM(countLastProg), 0) FROM #t3 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = a.PerolehanData AND LastProgress = 'SPK' AND TypeReport = @TypeReport)) SumOuts,
	(SELECT ISNULL(SUM(countLastProg), 0) FROM #t3 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = a.PerolehanData AND LastProgress = 'DO') DO,
	(SELECT ISNULL(SUM(countLastProg), 0) FROM #t3 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = a.PerolehanData AND LastProgress = 'DELIVERY') DELIVERY,
	(SELECT ISNULL(SUM(countLastProg), 0) FROM #t3 WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = a.PerolehanData AND LastProgress = 'LOST') LOST
FROM #t3 a	
GROUP BY a.CompanyCode, a.BranchCode, a.PerolehanData
) a
GROUP BY TypeReport, ProductivityBy,PerDate,Area,PeriodeDO,SalesHead,Salesman,Dealer,Outlet,Source
ORDER BY  Source
DROP TABLE #t3
END
END