

-- =============================================
-- Author:		<yo>
-- Create date: <30 Oct 2013>
-- Last Update date: <01 Apr 2014> 
-- Description:	<Inquiry ITS Productivity>
-- =============================================

ALTER PROCEDURE [dbo].[usprpt_InquiryITSProd]
  @StartDate			DATETIME,
  @EndDate				DATETIME,
  @Area					varchar(100),
  @DealerCode			varchar(100),
  @OutletCode			varchar(100),
  @BranchHead			varchar(100),
  @SalesHead			varchar(100),
  @SalesCoordinator		varchar(100),
  @Salesman				varchar(100),
  @TypeReport			varchar(1),
  @ProductivityBy		varchar(1)
AS
BEGIN

--usprpt_InquiryITSProd '01-Nov-2013','30-Nov-2013','JAWA BARAT','6058401','605840100','','','','','1','1'
--declare @StartDate	DATETIME
--declare @EndDate	DATETIME
--declare @Area		varchar(100)
--declare @DealerCode	varchar(100)
--declare @OutletCode	varchar(100)
--declare @BranchHead	varchar(100)
--declare @SalesHead	varchar(100)
--declare @SalesCoordinator varchar(100)
--declare @Salesman	varchar(100)
--declare @TypeReport	varchar(1)
--declare @ProductivityBy varchar(1)

--set @StartDate = '01-Mar-2014'
--set @EndDate	= '19-Mar-2014'
--set @Area = 'CABANG'
--set @DealerCode = '6006406'
--set @OutletCode = '6006404'
--set @BranchHead = ''
--set @SalesHead  = ''
--set @SalesCoordinator = ''
--set @Salesman = ''
--set @TypeReport = '1' -- 0 : SUMMARY, 1 : SALDO
--set @ProductivityBy = '0'	-- 0 : SALESMAN, 1 : VEHICLE TYPE, 2 : SOURCE DATA			

DECLARE @National AS VARCHAR(1)
SET @National = (SELECT ParaValue FROM SuzukiR4..GnMstLookUpDtl WHERE CodeID = 'QSLS' AND LookUpValue = 'NATIONAL')					

--#region PRODUCTIVITY BY SALESMAN (0)	
IF @ProductivityBy = '0'
BEGIN	
--#region SELECT SALESMAN 	
		
	IF @National = '1' 
	BEGIN
	SELECT * INTO #tSPK1 FROM(
		SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(Wiraniaga, '[BLANK]') Wiraniaga, 
			COUNT(Hist.LastProgress) SPK, ISNULL(SalesCoordinator, '[BLANK]') SalesCoordinator
			FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
			LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
				HstITS.CompanyCode = Hist.CompanyCode AND
				HstITS.BranchCode = Hist.BranchCode AND
				HstITS.InquiryNumber = Hist.InquiryNumber 
			 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
				AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
				AND Hist.LastProgress = 'SPK' 
				AND HstITS.LastProgress = 'SPK' -- penambahan 1 April 2014
				AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
				 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
				and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress<>'P'
															 and convert(varchar,h.UpdateDate,112)<@StartDate)
				 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress not in ('P','HP')
															 and convert(varchar,h.UpdateDate,112)<@StartDate)
				 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress not in ('P','HP','SPK')
															 and convert(varchar,h.UpdateDate,112)<@StartDate))))	 
															 and HstITS.LastProgress not in ('DO','DELIVERY','LOST')	
			 AND CONVERT(VARCHAR, HstITS.LastUpdateStatus , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)																 
			 GROUP BY  Hist.CompanyCode, Hist.BranchCode, Wiraniaga, SalesCoordinator) #tSPK1

	SELECT * INTO #tSPK2 FROM(
		SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(Wiraniaga, '[BLANK]') Wiraniaga, 
			COUNT(Hist.LastProgress) SPK, ISNULL(SalesCoordinator, '[BLANK]') SalesCoordinator
			FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
			LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
				HstITS.CompanyCode = Hist.CompanyCode AND
				HstITS.BranchCode = Hist.BranchCode AND
				HstITS.InquiryNumber = Hist.InquiryNumber 
			 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
				AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
				AND Hist.LastProgress = 'SPK'
				AND HstITS.LastProgress = 'SPK' -- penambahan 1 April 2014
				AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)								
				 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
				 and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress<>'P'
															 and convert(varchar,h.UpdateDate,112)<@StartDate)
				 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress not in ('P','HP')
															 and convert(varchar,h.UpdateDate,112)<@StartDate)
				 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress not in ('P','HP','SPK')
															 and convert(varchar,h.UpdateDate,112)<@StartDate)))
			  AND CONVERT(VARCHAR, HstITS.LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112))
			   and HstITS.LastProgress not in ('DO','DELIVERY','LOST')	 
			 GROUP BY  Hist.CompanyCode, Hist.BranchCode, Wiraniaga, SalesCoordinator) #tSPK2
			 
	SELECT * INTO #tDO FROM (
		SELECT Hist.CompanyCode, Hist.BranchCode, Wiraniaga, COUNT(Hist.LastProgress) DO, SalesCoordinator
				FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
				INNER JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
					HstITS.CompanyCode = Hist.CompanyCode AND
					HstITS.BranchCode = Hist.BranchCode AND
					HstITS.InquiryNumber = Hist.InquiryNumber 
				 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
					AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
					AND Hist.LastProgress = 'DO'
					AND HstITS.LastProgress = 'DO' -- penambahan 1 April 2014
					AND CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 					
				GROUP BY Hist.CompanyCode, Hist.BranchCode, Wiraniaga, SalesCoordinator) #tDO
		
	SELECT * INTO #tDELIVERY FROM (
			SELECT Hist.CompanyCode, Hist.BranchCode, Wiraniaga, COUNT(Hist.LastProgress) DELIVERY, SalesCoordinator
					FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
					INNER JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
						HstITS.CompanyCode = Hist.CompanyCode AND
						HstITS.BranchCode = Hist.BranchCode AND
						HstITS.InquiryNumber = Hist.InquiryNumber 
					 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
						AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
						AND Hist.LastProgress = 'DELIVERY'
						AND HstITS.LastProgress = 'DELIVERY' -- penambahan 1 April 2014
						AND CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 
					GROUP BY Hist.CompanyCode, Hist.BranchCode, Wiraniaga, SalesCoordinator) #tDELIVERY

	SELECT * INTO #tHP1 FROM (
			SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(Wiraniaga, '[BLANK]') Wiraniaga, 
			COUNT(Hist.LastProgress) HP, ISNULL(SalesCoordinator, '[BLANK]') SalesCoordinator
			FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
			LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
				HstITS.CompanyCode = Hist.CompanyCode AND
				HstITS.BranchCode = Hist.BranchCode AND
				HstITS.InquiryNumber = Hist.InquiryNumber 
			 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
				AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
				AND Hist.LastProgress = 'HP'
				AND HstITS.LastProgress = 'HP' -- penambahan 1 April 2014
				AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
				 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
				and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress<>'P'
															 and convert(varchar,h.UpdateDate,112)<@StartDate)
				 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress not in ('P','HP')
															 and convert(varchar,h.UpdateDate,112)<@StartDate)
				 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress not in ('P','HP','SPK')
															 and convert(varchar,h.UpdateDate,112)<@StartDate))))	 
															  and HstITS.LastProgress not in ('DO','DELIVERY','LOST')	
			 AND CONVERT(VARCHAR, HstITS.LastUpdateStatus , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)	
			 GROUP BY  Hist.CompanyCode, Hist.BranchCode, Wiraniaga, SalesCoordinator) #tHP		

	SELECT * INTO #tHP2 FROM (
					SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(Wiraniaga, '[BLANK]') Wiraniaga, 
			COUNT(Hist.LastProgress) HP, ISNULL(SalesCoordinator, '[BLANK]') SalesCoordinator
			FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
			LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
				HstITS.CompanyCode = Hist.CompanyCode AND
				HstITS.BranchCode = Hist.BranchCode AND
				HstITS.InquiryNumber = Hist.InquiryNumber 
			 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
				AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
				AND Hist.LastProgress = 'HP'
				AND HstITS.LastProgress = 'HP' -- penambahan 1 April 2014
				AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
				 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
				and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress<>'P'
															 and convert(varchar,h.UpdateDate,112)<@StartDate)
				 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress not in ('P','HP')
															 and convert(varchar,h.UpdateDate,112)<@StartDate)
				 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress not in ('P','HP','SPK')
															 and convert(varchar,h.UpdateDate,112)<@StartDate)))
				AND CONVERT(VARCHAR, HstITS.LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112))				
				and HstITS.LastProgress not in ('DO','DELIVERY','LOST')											 	 
			 GROUP BY  Hist.CompanyCode, Hist.BranchCode, Wiraniaga, SalesCoordinator) #tHP2
		
	SELECT * INTO #tP1 FROM (
			SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(Wiraniaga, '[BLANK]') Wiraniaga, 
			COUNT(Hist.LastProgress) P, ISNULL(SalesCoordinator, '[BLANK]') SalesCoordinator
			FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
			LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
				HstITS.CompanyCode = Hist.CompanyCode AND
				HstITS.BranchCode = Hist.BranchCode AND
				HstITS.InquiryNumber = Hist.InquiryNumber 
			 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
				AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
				AND Hist.LastProgress = 'P'
				AND HstITS.LastProgress = 'P' -- penambahan 1 April 2014
				AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
				 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
				and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress<>'P'
															 and convert(varchar,h.UpdateDate,112)<@StartDate)
				 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress not in ('P','HP')
															 and convert(varchar,h.UpdateDate,112)<@StartDate)
				 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress not in ('P','HP','SPK')
															 and convert(varchar,h.UpdateDate,112)<@StartDate))))
															 and HstITS.LastProgress not in ('DO','DELIVERY','LOST')		
															  AND CONVERT(VARCHAR, HstITS.LastUpdateStatus , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)	 
			 GROUP BY  Hist.CompanyCode, Hist.BranchCode, Wiraniaga, SalesCoordinator) #tP	
	
	SELECT * INTO #tP2 FROM (
		SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(Wiraniaga, '[BLANK]') Wiraniaga, 
			COUNT(Hist.LastProgress) P, ISNULL(SalesCoordinator, '[BLANK]') SalesCoordinator
			FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
			LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
				HstITS.CompanyCode = Hist.CompanyCode AND
				HstITS.BranchCode = Hist.BranchCode AND
				HstITS.InquiryNumber = Hist.InquiryNumber 
			 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
				AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
				AND Hist.LastProgress = 'P'
				AND HstITS.LastProgress = 'P' -- penambahan 1 April 2014
				AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
				 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
				and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress<>'P'
															 and convert(varchar,h.UpdateDate,112)<@StartDate)
				 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress not in ('P','HP')
															 and convert(varchar,h.UpdateDate,112)<@StartDate)
				 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress not in ('P','HP','SPK')
															 and convert(varchar,h.UpdateDate,112)<@StartDate)))	 
				AND CONVERT(VARCHAR, HstITS.LastUpdateDate, 112) <= CONVERT(VARCHAR, @EndDate, 112))		
				 and HstITS.LastProgress not in ('DO','DELIVERY','LOST')															 
			 GROUP BY  Hist.CompanyCode, Hist.BranchCode, Wiraniaga, SalesCoordinator) #tP2

	SELECT * INTO #tLOST FROM (
			SELECT Hist.CompanyCode, Hist.BranchCode, Wiraniaga, COUNT(Hist.LastProgress) LOST, SalesCoordinator  
					FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
					INNER JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
						HstITS.CompanyCode = Hist.CompanyCode AND
						HstITS.BranchCode = Hist.BranchCode AND
						HstITS.InquiryNumber = Hist.InquiryNumber 
					 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
						AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
						AND Hist.LastProgress = 'LOST'
						AND HstITS.LastProgress = 'LOST' -- penambahan 1 April 2014
						AND CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 
					GROUP BY Hist.CompanyCode, Hist.BranchCode, Wiraniaga, SalesCoordinator) #tLOST		
	
	SELECT * INTO #tNEW FROM (
		SELECT CompanyCode, BranchCode, Wiraniaga, COUNT(LastProgress) NEW, SalesCoordinator
		FROM SuzukiR4..pmHstITS WITH (NOLOCK, NOWAIT)
		WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE CompanyCode END) = @DealerCode
			AND (CASE WHEN @OutletCode = '' THEN '' ELSE BranchCode END) = @OutletCode	 
			AND CONVERT(VARCHAR, InquiryDate, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)
		GROUP BY  CompanyCode, BranchCode, Wiraniaga, SalesCoordinator) #tNEW	
	
	SELECT * INTO #tSalesmanNational1 FROM(
		SELECT DISTINCT a.CompanyCode, a.BranchCode
		,ISNULL(case when HstITS.WiraNiaga = '' then 'SC-'+HstITS.SalesCoordinator else HstITS.Wiraniaga end, '') EmployeeID
		,'10' PositionID, ISNULL(HstITS.SalesCoordinator,'') SpvEmployeeID, ISNULL(HstITS.SalesHead, '') SalesHead, ISNULL(HstITS.BranchHead, '') BranchHead
		,ISNULL(NEW.NEW, 0) NEW		
		--, 0 NEW
		,ISNULL(CASE WHEN @TypeReport = '0' THEN P1.P ELSE P2.P END, (CASE WHEN HstITS.WiraNiaga IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT SUM(P) FROM #tP1 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND Wiraniaga = '[BLANK]'), 0) ELSE ISNULL((SELECT SUM(P) FROM #tP2 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND Wiraniaga = '[BLANK]'), 0) END) END)) P		 
		,ISNULL(CASE WHEN @TypeReport = '0' THEN HP1.HP ELSE HP2.HP END, (CASE WHEN HstITS.WiraNiaga IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT SUM(HP) FROM #tHP1 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND Wiraniaga = '[BLANK]'),0) ELSE ISNULL((SELECT SUM(HP) FROM #tHP2 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND Wiraniaga = '[BLANK]'),0) END) END)) HP		
		,ISNULL(CASE WHEN @TypeReport = '0' THEN SPK1.SPK ELSE SPK2.SPK END, (CASE WHEN HstITS.WiraNiaga IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT SUM(SPK) FROM #tSPK1 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND Wiraniaga = '[BLANK]'),0) ELSE ISNULL((SELECT SUM(SPK) FROM #tSPK2 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND Wiraniaga = '[BLANK]'),0) END) END)) SPK

		,(ISNULL(CASE WHEN @TypeReport = '0' THEN P1.P ELSE P2.P END, (CASE WHEN HstITS.WiraNiaga IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT SUM(P) FROM #tP1 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND Wiraniaga = '[BLANK]'), 0) ELSE ISNULL((SELECT SUM(P) FROM #tP2 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND Wiraniaga = '[BLANK]'), 0) END) END))
			+ISNULL(CASE WHEN @TypeReport = '0' THEN HP1.HP ELSE HP2.HP END, (CASE WHEN HstITS.WiraNiaga IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT SUM(HP) FROM #tHP1 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND Wiraniaga = '[BLANK]'),0) ELSE ISNULL((SELECT SUM(HP) FROM #tHP2 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND Wiraniaga = '[BLANK]'),0) END) END))
			+ISNULL(CASE WHEN @TypeReport = '0' THEN SPK1.SPK ELSE SPK2.SPK END, (CASE WHEN HstITS.WiraNiaga IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT SUM(SPK) FROM #tSPK1 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND Wiraniaga = '[BLANK]'),0) ELSE ISNULL((SELECT SUM(SPK) FROM #tSPK2 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND Wiraniaga = '[BLANK]'),0) END) END))) SumOuts

		,ISNULL(DO.DO, 0) DO, ISNULL(DELIVERY.DELIVERY, 0) DELIVERY, ISNULL(LOST.LOST, 0) LOST			
		FROM SuzukiR4..pmStatusHistory a WITH (NOLOCK, NOWAIT)
		LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON HstITS.CompanyCode = a.CompanyCode AND HstITS.BranchCode = a.BranchCode AND HstITS.InquiryNumber = a.InquiryNumber 
		LEFT JOIN SuzukiR4..gnMstDealerMapping b WITH (NOLOCK, NOWAIT) on a.CompanyCode = b.DealerCode	
		LEFT JOIN #tNEW NEW WITH (NOLOCK, NOWAIT) ON NEW.CompanyCode = a.CompanyCode AND NEW.BranchCode = a.BranchCode AND NEW.Wiraniaga = HstITS.Wiraniaga AND NEW.SalesCoordinator = HstITS.SalesCoordinator	
		LEFT JOIN #tP1 P1 WITH (NOLOCK, NOWAIT) ON P1.CompanyCode = a.CompanyCode AND P1.BranchCode = a.BranchCode AND P1.Wiraniaga = HstITS.Wiraniaga AND P1.SalesCoordinator = HstITS.SalesCoordinator	
		LEFT JOIN #tP2 P2 WITH (NOLOCK, NOWAIT)	ON P2.CompanyCode = a.CompanyCode AND P2.BranchCode = a.BranchCode AND P2.Wiraniaga = HstITS.Wiraniaga AND P2.SalesCoordinator = HstITS.SalesCoordinator				
		LEFT JOIN #tHP1 HP1 WITH (NOLOCK, NOWAIT) ON HP1.CompanyCode = a.CompanyCode AND HP1.BranchCode = a.BranchCode AND HP1.Wiraniaga = HstITS.Wiraniaga AND HP1.SalesCoordinator = HstITS.SalesCoordinator 			
		LEFT JOIN #tHP2 HP2 WITH (NOLOCK, NOWAIT) ON HP2.CompanyCode = a.CompanyCode AND HP2.BranchCode = a.BranchCode AND HP2.Wiraniaga = HstITS.Wiraniaga	AND HP2.SalesCoordinator = HstITS.SalesCoordinator 			
		LEFT JOIN #tSPK1 SPK1 WITH (NOLOCK, NOWAIT) ON SPK1.CompanyCode = a.CompanyCode AND SPK1.BranchCode = a.BranchCode AND SPK1.Wiraniaga = HstITS.Wiraniaga AND SPK1.SalesCoordinator = HstITS.SalesCoordinator 
		LEFT JOIN #tSPK2 SPK2 WITH (NOLOCK, NOWAIT) ON SPK2.CompanyCode = a.CompanyCode AND SPK2.BranchCode = a.BranchCode AND SPK2.Wiraniaga = HstITS.Wiraniaga AND SPK2.SalesCoordinator = HstITS.SalesCoordinator 	
		LEFT JOIN #tDO DO WITH (NOLOCK, NOWAIT) ON DO.CompanyCode = a.CompanyCode AND DO.BranchCode = a.BranchCode AND DO.Wiraniaga = HstITS.Wiraniaga AND DO.SalesCoordinator = HstITS.SalesCoordinator 	
		LEFT JOIN #tDELIVERY DELIVERY WITH (NOLOCK, NOWAIT) ON DELIVERY.CompanyCode = a.CompanyCode AND DELIVERY.BranchCode = a.BranchCode AND DELIVERY.Wiraniaga = HstITS.Wiraniaga AND DELIVERY.SalesCoordinator = HstITS.SalesCoordinator 			
		LEFT JOIN #tLOST LOST WITH (NOLOCK, NOWAIT) ON LOST.CompanyCode = a.CompanyCode AND LOST.BranchCode = a.BranchCode AND LOST.Wiraniaga = HstITS.Wiraniaga AND LOST.SalesCoordinator = HstITS.SalesCoordinator						   					   		   			   		
		WHERE (CASE WHEN @Area = '' THEN '' ELSE b.Area END) = @Area				
			  AND (CASE WHEN @DealerCode = '' THEN '' ELSE a.CompanyCode END) = @DealerCode
			  AND (CASE WHEN @OutletCode = '' THEN '' ELSE a.BranchCode END) = @OutletCode	 
	) #tSalesmanNational1
	
	SELECT * INTO #tSalesmanNational FROM(
		select NEW.CompanyCode, NEW.BranchCode, NEW.Wiraniaga EmployeeID, ISNULL(a.PositionID, '10') PositionID, NEW.SalesCoordinator SpvEmployeeID, isnull(a.SalesHead, '') SalesHead
		, isnull(a.BranchHead, '') BranchHead, ISNULL(NEW.NEW, 0) NEW, isnull(a.P, 0) P, isnull(a.HP, 0) HP, isnull(a.SPK, 0) SPK
		, ISNULL(a.SumOuts, 0) SumOuts, ISNULL(a.do, 0) DO, ISNULL(a.delivery, '') delivery, ISNULL(a.lost, 0) lost
		from #tNEW NEW
		left join #tSalesmanNational1 a WITH (NOLOCK, NOWAIT) ON NEW.CompanyCode = a.CompanyCode AND NEW.BranchCode = a.BranchCode AND NEW.Wiraniaga = a.EmployeeID AND NEW.SalesCoordinator = a.SpvEmployeeID
		where a.EmployeeID is null
		union
		select distinct CompanyCode, BranchCode, EmployeeID, PositionID, SpvEmployeeID,  
			(Select top 1 SalesHead from #tSalesmanNational1 a where a.CompanyCode = #tSalesmanNational1.CompanyCode and a.BranchCode = #tSalesmanNational1.BranchCode and a.EmployeeID = #tSalesmanNational1.EmployeeID) SalesHead, 
			BranchHead, NEW, P, HP, SPK,SumOuts, DO, Delivery, LOST
			from #tSalesmanNational1
			where BranchHead != ''
	) #tSalesmanNational	
	--select * from #tSalesmanNational
		
	END	 	
	ELSE
	BEGIN
	SELECT * INTO #tSalesman FROM(
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.PositionID, 
	(SELECT Distinct(EmployeeID) FROM SuzukiR4..PmMstTeamMembers 
	WHERE CompanyCode = a.CompanyCode AND 
		BranchCode = a.BranchCode AND 
		TeamID IN (SELECT Distinct(TeamID) FROM SuzukiR4..PmMstTeamMembers 
					WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE CompanyCode END) = @DealerCode
					AND (CASE WHEN @OutletCode = '' THEN '' ELSE BranchCode END) = @OutletCode	 
					AND EmployeeID = a.EmployeeID AND IsSupervisor = 0)
		AND IsSupervisor = 1) SpvEmployeeID,
	(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
	WHERE CompanyCode = a.CompanyCode AND
		BranchCode = a.BranchCode AND
		EmployeeID = a.EmployeeID AND
		CONVERT(VARCHAR, InquiryDate, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)) NEW,
		
	CASE WHEN @TypeReport = '0' THEN
		(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
		WHERE CompanyCode = a.CompanyCode AND
			BranchCode = a.BranchCode AND
			EmployeeID = a.EmployeeID AND
			LastProgress = 'P' AND 
			CONVERT(VARCHAR, LastUpdateStatus, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)) 
	ELSE
		(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
		WHERE CompanyCode = a.CompanyCode AND
			BranchCode = a.BranchCode AND
			EmployeeID = a.EmployeeID AND
			LastProgress = 'P' AND 
			CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) END P, 
			
	CASE WHEN @TypeReport = '0' THEN			
		(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
		WHERE CompanyCode = a.CompanyCode AND
			BranchCode = a.BranchCode AND
			EmployeeID = a.EmployeeID and
			LastProgress = 'HP' AND 
			CONVERT(VARCHAR, LastUpdateStatus, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112))
	ELSE
		(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
		WHERE CompanyCode = a.CompanyCode AND
			BranchCode = a.BranchCode AND
			EmployeeID = a.EmployeeID and
			LastProgress = 'HP' AND 
			CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) END HP, 
			
	(CASE WHEN @TypeReport = '0' THEN
		(SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
			INNER JOIN SuzukiR4..pmKdp KDP ON 
			KDP.CompanyCode = Hist.CompanyCode AND
			KDP.BranchCode = Hist.BranchCode AND
			KDP.InquiryNumber = Hist.InquiryNumber 
		 WHERE Hist.CompanyCode = a.CompanyCode AND
			Hist.BranchCode = a.BranchCode AND
			KDP.EmployeeID = a.EmployeeID AND
			CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
			Hist.LastProgress = 'SPK')
		ELSE
		(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
			WHERE CompanyCode = a.CompanyCode AND
			BranchCode = a.BranchCode AND
			EmployeeID = a.EmployeeID and
			LastProgress = 'SPK' AND 
			CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) 
			END) SPK,
			
	(CASE WHEN @TypeReport = '0' THEN			  
		(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
		WHERE CompanyCode = a.CompanyCode AND
			BranchCode = a.BranchCode AND
			EmployeeID = a.EmployeeID AND
			LastProgress = 'P' AND 
			CONVERT(VARCHAR, LastUpdateStatus, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)) +
		(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
		WHERE CompanyCode = a.CompanyCode AND
			BranchCode = a.BranchCode AND
			EmployeeID = a.EmployeeID and
			LastProgress = 'HP' AND 
			CONVERT(VARCHAR, LastUpdateStatus, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)) +		  
		(SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
			INNER JOIN SuzukiR4..pmKdp KDP ON 
			KDP.CompanyCode = Hist.CompanyCode AND
			KDP.BranchCode = Hist.BranchCode AND
			KDP.InquiryNumber = Hist.InquiryNumber 
		 WHERE Hist.CompanyCode = a.CompanyCode AND
			Hist.BranchCode = a.BranchCode AND
			KDP.EmployeeID = a.EmployeeID AND
			CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
			Hist.LastProgress = 'SPK')
	ELSE
		(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
		WHERE CompanyCode = a.CompanyCode AND
			BranchCode = a.BranchCode AND
			EmployeeID = a.EmployeeID AND
			LastProgress = 'P' AND 
			CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) +
		(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
		WHERE CompanyCode = a.CompanyCode AND
			BranchCode = a.BranchCode AND
			EmployeeID = a.EmployeeID AND
			LastProgress = 'HP' AND 
			CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) +				
		(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
		WHERE CompanyCode = a.CompanyCode AND
			BranchCode = a.BranchCode AND
			EmployeeID = a.EmployeeID and
			LastProgress = 'SPK' AND 
			CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) 
	 END) SumOuts
	 
	, (SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
		INNER JOIN SuzukiR4..pmKdp KDP ON 
		KDP.CompanyCode = Hist.CompanyCode AND
		KDP.BranchCode = Hist.BranchCode AND
		KDP.InquiryNumber = Hist.InquiryNumber 
	 WHERE Hist.CompanyCode = a.CompanyCode AND
		Hist.BranchCode = a.BranchCode AND
		KDP.EmployeeID = a.EmployeeID AND
		CONVERT(VARCHAR, Hist.UpdateDate, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
		Hist.LastProgress = 'DO') DO
	, (SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
		INNER JOIN SuzukiR4..pmKdp KDP ON 
		KDP.CompanyCode = Hist.CompanyCode AND
		KDP.BranchCode = Hist.BranchCode AND
		KDP.InquiryNumber = Hist.InquiryNumber 
	 WHERE Hist.CompanyCode = a.CompanyCode AND
		Hist.BranchCode = a.BranchCode AND
		KDP.EmployeeID = a.EmployeeID AND
		CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
		Hist.LastProgress = 'DELIVERY') DELIVERY
	, (SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
		INNER JOIN SuzukiR4..pmKdp KDP ON 
		KDP.CompanyCode = Hist.CompanyCode AND
		KDP.BranchCode = Hist.BranchCode AND
		KDP.InquiryNumber = Hist.InquiryNumber 
	 WHERE Hist.CompanyCode = a.CompanyCode AND
		Hist.BranchCode = a.BranchCode AND
		KDP.EmployeeID = a.EmployeeID AND
		CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
		Hist.LastProgress = 'LOST') LOST
FROM SuzukiR4..PmPosition a
LEFT JOIN SuzukiR4..gnMstDealerMapping b on a.CompanyCode = b.DealerCode	
WHERE (b.Area like Case when @Area <> ''
			then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
					then 'JABODETABEK'
					else @Area end
			else '' end
	   or b.Area like Case when @Area <> '' 
					then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
							then 'CABANG'
							else @Area end
					else '' end) 						
	  AND (CASE WHEN @DealerCode = '' THEN '' ELSE b.DealerCode END) = @DealerCode
	  AND (CASE WHEN @OutletCode = '' THEN '' ELSE a.BranchCode END) = @OutletCode	 
	  AND a.PositionID = 10 		
      ) #tSalesman
END  
--#endregion

--#region  SELECT COORD 
IF @National = '1'
BEGIN
	SELECT * INTO #tSalesCoNational  FROM(	 
	SELECT DISTINCT
		a.CompanyCode,
		a.BranchCode,
		a.SpvEmployeeID EmployeeID,
		'20' PositionID,
		SalesHead ShEmployeeID,
		BranchHead BMEmployeeID,
		ISNULL(SUM(a.NEW),0) NEW,
		ISNULL(SUM(a.P),0) P,
		ISNULL(SUM(a.HP),0) HP,
		ISNULL(SUM(a.SPK),0) SPK,
		ISNULL(SUM(a.SumOuts),0) SumOuts,
		ISNULL(SUM(a.DO),0) DO,
		ISNULL(SUM(a.DELIVERY),0) DELIVERY,
		ISNULL(SUM(a.LOST),0) LOST
	FROM #tSalesmanNational  a
	--WHERE a.SpvEmployeeID <> ''
	GROUP BY a.CompanyCode,
		a.BranchCode,
		a.SpvEmployeeID,
		a.SalesHead,
		a.BranchHead
	) #tSalesCoNational 		
END
ELSE
BEGIN
	SELECT * INTO #tSalesCo FROM(	 
	SELECT
		a.CompanyCode,
		a.BranchCode,
		a.SpvEmployeeID EmployeeID,
		'20' PositionID,
		(SELECT Distinct(EmployeeID) FROM SuzukiR4..PmMstTeamMembers 
		WHERE CompanyCode = a.CompanyCode AND 
			BranchCode = a.BranchCode AND 
			TeamID IN (SELECT Distinct(TeamID) FROM SuzukiR4..PmMstTeamMembers 
						WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE CompanyCode END) = @DealerCode
						AND (CASE WHEN @OutletCode = '' THEN '' ELSE BranchCode END) = @OutletCode	 
						AND EmployeeID = a.SpvEmployeeID AND IsSupervisor = 0)
			AND IsSupervisor = 1) ShEmployeeID,
		ISNULL(SUM(a.NEW),0) NEW,
		ISNULL(SUM(a.P),0) P,
		ISNULL(SUM(a.HP),0) HP,
		ISNULL(SUM(a.SPK),0) SPK,
		ISNULL(SUM(a.SumOuts),0) SumOuts,
		ISNULL(SUM(a.DO),0) DO,
		ISNULL(SUM(a.DELIVERY),0) DELIVERY,
		ISNULL(SUM(a.LOST),0) LOST
	FROM #tSalesman a
	GROUP BY a.CompanyCode,
		a.BranchCode,
		a.SpvEmployeeID
	) #tSalesCo
END	
--#endregion

--#region  SELECT SALES HEAD 	 
IF @National = '1'
BEGIN
	SELECT * INTO #tSalesHeadNational FROM(			
	SELECT DISTINCT
		a.CompanyCode,
		a.BranchCode,
		a.ShEmployeeID EmployeeID,
		'30' PositionID,
		a.BMEmployeeID,
		ISNULL(SUM(a.NEW),0) NEW,
		ISNULL(SUM(a.P),0) P,
		ISNULL(SUM(a.HP),0) HP,
		ISNULL(SUM(a.SPK),0) SPK,
		ISNULL(SUM(a.SumOuts),0) SumOuts,
		ISNULL(SUM(a.DO),0) DO,
		ISNULL(SUM(a.DELIVERY),0) DELIVERY,
		ISNULL(SUM(a.LOST),0) LOST
	FROM #tSalesCoNational a
	--WHERE a.ShEmployeeID <> ''
	GROUP BY a.CompanyCode,
		a.BranchCode,
		a.ShEmployeeID,
		a.BMEmployeeID
	) #tSalesHeadNational
END
ELSE
BEGIN
	SELECT * INTO #tSalesHead FROM(			
	SELECT
		a.CompanyCode,
		a.BranchCode,
		a.ShEmployeeID EmployeeID,
		'30' PositionID,
		(SELECT Distinct(EmployeeID) FROM SuzukiR4..PmMstTeamMembers 
		WHERE CompanyCode = a.CompanyCode AND 
			BranchCode = a.BranchCode AND 
			TeamID IN (SELECT Distinct(TeamID) FROM SuzukiR4..PmMstTeamMembers 
						WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE CompanyCode END) = @DealerCode
						AND (CASE WHEN @OutletCode = '' THEN '' ELSE BranchCode END) = @OutletCode	 
						AND EmployeeID = a.ShEmployeeID AND IsSupervisor = 0)
			AND IsSupervisor = 1) BMEmployeeID,
		ISNULL(SUM(a.NEW),0) NEW,
		ISNULL(SUM(a.P),0) P,
		ISNULL(SUM(a.HP),0) HP,
		ISNULL(SUM(a.SPK),0) SPK,
		ISNULL(SUM(a.SumOuts),0) SumOuts,
		ISNULL(SUM(a.DO),0) DO,
		ISNULL(SUM(a.DELIVERY),0) DELIVERY,
		ISNULL(SUM(a.LOST),0) LOST
	FROM #tSalesCo a
	GROUP BY a.CompanyCode,
		a.BranchCode,
		a.ShEmployeeID
	) #tSalesHead
END
--#endregion

--#region  SELECT BRANCH MANAGER 	 
IF @National = '1'
BEGIN
	SELECT * INTO #tSalesBMNational FROM(			
	SELECT DISTINCT
		a.CompanyCode,
		a.BranchCode,
		a.BMEmployeeID EmployeeID,
		'40' PositionID,
		'' SpvEmployeeID,
		ISNULL(SUM(a.NEW),0) NEW,
		ISNULL(SUM(a.P),0) P,
		ISNULL(SUM(a.HP),0) HP,
		ISNULL(SUM(a.SPK),0) SPK,
		ISNULL(SUM(a.SumOuts),0) SumOuts,
		ISNULL(SUM(a.DO),0) DO,
		ISNULL(SUM(a.DELIVERY),0) DELIVERY,
		ISNULL(SUM(a.LOST),0) LOST
		FROM #tSalesHeadNational a
		--WHERE a.BMEmployeeID <> ''
	GROUP BY a.CompanyCode,
		a.BranchCode,
		a.BMEmployeeID
	) #tSalesBMNational			
END
ELSE
BEGIN
	SELECT * INTO #tSalesBM FROM(			
	SELECT
		a.CompanyCode,
		a.BranchCode,
		a.BMEmployeeID EmployeeID,
		'40' PositionID,
		'' SpvEmployeeID,
		ISNULL(SUM(a.NEW),0) NEW,
		ISNULL(SUM(a.P),0) P,
		ISNULL(SUM(a.HP),0) HP,
		ISNULL(SUM(a.SPK),0) SPK,
		ISNULL(SUM(a.SumOuts),0) SumOuts,
		ISNULL(SUM(a.DO),0) DO,
		ISNULL(SUM(a.DELIVERY),0) DELIVERY,
		ISNULL(SUM(a.LOST),0) LOST
		FROM #tSalesHead a
	GROUP BY a.CompanyCode,
		a.BranchCode,
		a.BMEmployeeID
	) #tSalesBM	
END	
--#endregion

--#region  SELECT UNION ALL
IF @National = '1'
BEGIN
	SELECT * INTO #tUnionNational FROM(
	SELECT DISTINCT CompanyCode, BranchCode, EmployeeID, PositionID, NEW, P, HP, SPK, SumOuts, DO, DELIVERY, LOST FROM #tSalesBMNational
	WHERE EmployeeID != NULL UNION
	SELECT DISTINCT CompanyCode, BranchCode, EmployeeID, PositionID, NEW, P, HP, SPK, SumOuts, DO, DELIVERY, LOST FROM #tSalesHeadNational 
	WHERE (CASE WHEN @SalesHead  = '' THEN '' ELSE EmployeeID END) = @SalesHead UNION
	SELECT DISTINCT CompanyCode, BranchCode, EmployeeID, PositionID, NEW, P, HP, SPK, SumOuts, DO, DELIVERY, LOST FROM #tSalesCoNational 
	WHERE (CASE WHEN @SalesCoordinator  = '' THEN '' ELSE EmployeeID END) = @SalesCoordinator
		AND (CASE WHEN @SalesHead  = '' THEN '' ELSE ShEmployeeID END) = @SalesHead
	UNION
	SELECT DISTINCT CompanyCode, BranchCode, EmployeeID, PositionID, NEW, P, HP, SPK, SumOuts, DO, DELIVERY, LOST FROM #tSalesmanNational
	WHERE (CASE WHEN @SalesHead  = '' THEN '' ELSE SalesHead END) = @SalesHead
		AND (CASE WHEN @SalesCoordinator  = '' THEN '' ELSE SpvEmployeeID END) = @SalesCoordinator
		AND (CASE WHEN @Salesman  = '' THEN '' ELSE EmployeeID END) = @Salesman
	) #tUnionNational
	ORDER BY PositionID DESC		
END
ELSE
BEGIN	
	SELECT * INTO #tUnion FROM(
	SELECT * FROM #tSalesBM UNION
	SELECT * FROM #tSalesHead 
	WHERE (CASE WHEN @SalesHead  = '' THEN '' ELSE EmployeeID END) = @SalesHead UNION
	SELECT * FROM #tSalesCo 
	WHERE (CASE WHEN @SalesCoordinator  = '' THEN '' ELSE EmployeeID END) = @SalesCoordinator
	UNION
	SELECT * FROM #tSalesman
	WHERE (CASE WHEN @SalesCoordinator  = '' THEN '' ELSE SpvEmployeeID END) = @SalesCoordinator
		AND (CASE WHEN @Salesman  = '' THEN '' ELSE EmployeeID END) = @Salesman
	) #tUnion
	ORDER BY PositionID DESC
END
--#endregion

--#region SELECT ALL
IF @National = '1'
	BEGIN		
		SELECT CASE WHEN @TypeReport = '0' THEN 'Summary' ELSE 'Saldo' END TypeReport,
			CASE @ProductivityBy 
				WHEN '0' THEN 'Salesman'
				WHEN '1' THEN 'Vehicle Type'
				WHEN '2' THEN 'Source Data'
			END ProductivityBy,
			CONVERT(VARCHAR, @EndDate, 105) PerDate, 
			CASE WHEN @Area = '' THEN 'ALL' ELSE @Area END Area,
			CONVERT(VARCHAR, @StartDate, 105) + ' s/d ' + CONVERT(VARCHAR, @EndDate, 105) PeriodeDO,
			CASE WHEN @SalesHead = '' THEN 'ALL' ELSE @SalesHead END SalesHead,
			CASE WHEN @SalesCoordinator = '' THEN 'ALL' ELSE @SalesCoordinator END SalesCoordinator,
			CASE WHEN @Salesman = '' THEN 'ALL' ELSE @Salesman END Salesman,			
			(SELECT LookupValueName FROM SuzukiR4..GnMstLookUpDtl WHERE CodeID = 'PGRD' AND LookUpValue = a.PositionID) Position,
			CASE WHEN @DealerCode = '' THEN 'ALL' ELSE (SELECT DealerName FROM SuzukiR4..gnMstDealerMapping WHERE DealerCode = @DealerCode) END Dealer,
			CASE WHEN @OutletCode = '' THEN 'ALL' ELSE (SELECT OutletName FROM SuzukiR4..gnMstDealerOutletMapping WHERE CompanyCode = @DealerCode AND OutletCode = @OutletCode) END Outlet,
			a.EmployeeID, a.EmployeeID EmployeeName,		
			ISNULL(SUM(a.P), 0) NEW,
			ISNULL(SUM(a.P), 0) P,
			ISNULL(SUM(a.HP), 0) HP,
			ISNULL(SUM(a.SPK), 0) SPK,
			ISNULL(SUM(a.SumOuts), 0) SumOuts,
			ISNULL(SUM(a.DO), 0) DO,
			ISNULL(SUM(a.DELIVERY), 0) DELIVERY,
			ISNULL(SUM(a.LOST), 0) LOST	
			
			--ISNULL(CASE CAST(a.NEW AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(a.NEW AS VARCHAR) END, '-') NEW,
			--ISNULL(CASE CAST(a.P AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(a.P AS VARCHAR) END, '-') P,
			--ISNULL(CASE CAST(a.HP AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(a.HP AS VARCHAR) END, '-') HP,
			--ISNULL(CASE CAST(a.SPK AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(a.SPK AS VARCHAR) END, '-') SPK,
			--ISNULL(CASE CAST(a.SumOuts AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(a.SumOuts AS VARCHAR) END, '-') SumOuts,
			--ISNULL(CASE CAST(a.DO AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(a.DO AS VARCHAR) END, '-') DO,
			--ISNULL(CASE CAST(a.DELIVERY AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(a.DELIVERY AS VARCHAR) END, '-') DELIVERY,
			--ISNULL(CASE CAST(a.LOST AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(a.LOST AS VARCHAR) END, '-') LOST	
		FROM #tUnionNational a
		LEFT JOIN SuzukiR4..gnMstDealerMapping c on a.CompanyCode = c.DealerCode	
		WHERE 
		 (CASE WHEN @DealerCode = '' THEN '' ELSE c.DealerCode END) = @DealerCode
		  AND (CASE WHEN @OutletCode = '' THEN '' ELSE a.BranchCode END) = @OutletCode	 
		  AND a.PositionID <> 60 
		  AND a.EmployeeID != 'NULL' 
		GROUP BY a.CompanyCode, a.BranchCode, a.PositionID, a.EmployeeID
		ORDER BY  a.CompanyCode, a.BranchCode, a.PositionID DESC
		
		DROP TABLE #tSalesmanNational, #tSalesCoNational, #tSalesHeadNational, #tSalesBMNational, #tUnionNational, #tSPK1, #tSPK2, #tDO, #tDELIVERY, #tLOST, #tHP1, #tHP2, #tP1, #tNEW, #tP2, #tSalesmanNational1
	END
	ELSE 
	BEGIN
		SELECT CASE WHEN @TypeReport = '0' THEN 'Summary' ELSE 'Saldo' END TypeReport,
			CASE @ProductivityBy 
				WHEN '0' THEN 'Salesman'
				WHEN '1' THEN 'Vehicle Type'
				WHEN '2' THEN 'Source Data'
			END ProductivityBy,
			CONVERT(VARCHAR, @EndDate, 105) PerDate,  CASE WHEN @Area = '' THEN 'ALL' ELSE @Area END Area, CONVERT(VARCHAR, @StartDate, 105) + ' s/d ' + CONVERT(VARCHAR, @EndDate, 105) PeriodeDO,
			CASE WHEN @SalesHead = '' THEN 'ALL' ELSE (SELECT EmployeeName FROM SuzukiR4..GnMstEmployee WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND EmployeeID = @SalesHead) END SalesHead,
			CASE WHEN @SalesCoordinator = '' THEN 'ALL' ELSE (SELECT EmployeeName FROM SuzukiR4..GnMstEmployee WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND EmployeeID = @SalesCoordinator) END SalesCoordinator,
			CASE WHEN @Salesman = '' THEN 'ALL' ELSE (SELECT EmployeeName FROM SuzukiR4..GnMstEmployee WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND EmployeeID = @Salesman) END Salesman,
			(SELECT LookupValueName FROM SuzukiR4..GnMstLookUpDtl WHERE CompanyCode = a.CompanyCode AND CodeID = 'PGRD' AND LookUpValue = a.PositionID) Position,
			CASE WHEN @DealerCode = '' THEN 'ALL' ELSE (SELECT DealerName FROM SuzukiR4..gnMstDealerMapping WHERE DealerCode = @DealerCode) END Dealer,
			CASE WHEN @OutletCode = '' THEN 'ALL' ELSE (SELECT BranchName FROM SuzukiR4..gnMstOrganizationDtl WHERE CompanyCode = @DealerCode AND BranchCode = @OutletCode) END Outlet,
			b.EmployeeID,
			(SELECT EmployeeName FROM SuzukiR4..GnMstEmployee WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND EmployeeID = a.EmployeeID) EmployeeName,			
			ISNULL(SUM(a.P), 0) NEW,
			ISNULL(SUM(a.P), 0) P,
			ISNULL(SUM(a.HP), 0) HP,
			ISNULL(SUM(a.SPK), 0) SPK,
			ISNULL(SUM(a.SumOuts), 0) SumOuts,
			ISNULL(SUM(a.DO), 0) DO,
			ISNULL(SUM(a.DELIVERY), 0) DELIVERY,
			ISNULL(SUM(a.LOST), 0) LOST	
			
			--ISNULL(CASE CAST(b.NEW AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.NEW AS VARCHAR) END, '-') NEW,
			--ISNULL(CASE CAST(b.P AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.P AS VARCHAR) END, '-') P,
			--ISNULL(CASE CAST(b.HP AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.HP AS VARCHAR) END, '-') HP,
			--ISNULL(CASE CAST(b.SPK AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.SPK AS VARCHAR) END, '-') SPK,
			--ISNULL(CASE CAST(b.SumOuts AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.SumOuts AS VARCHAR) END, '-') SumOuts,
			--ISNULL(CASE CAST(b.DO AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.DO AS VARCHAR) END, '-') DO,
			--ISNULL(CASE CAST(b.DELIVERY AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.DELIVERY AS VARCHAR) END, '-') DELIVERY,
			--ISNULL(CASE CAST(b.LOST AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.LOST AS VARCHAR) END, '-') LOST	
		FROM SuzukiR4..PmPosition a
		INNER JOIN #tUnion b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.EmployeeID = b.EmployeeID AND a.PositionID = b.PositionID
		LEFT JOIN SuzukiR4..gnMstDealerMapping c on a.CompanyCode = c.DealerCode	
		WHERE 
		 (CASE WHEN @DealerCode = '' THEN '' ELSE c.DealerCode END) = @DealerCode
		  AND (CASE WHEN @OutletCode = '' THEN '' ELSE a.BranchCode END) = @OutletCode	 
		  AND a.PositionID <> 60  
		  AND b.SpvEmployeeID IS NOT NULL
		GROUP BY a.CompanyCode, a.BranchCode, a.PositionID, a.EmployeeID
		ORDER BY a.BranchCode, a.PositionID DESC
		DROP TABLE #tSalesman, #tSalesCo, #tSalesHead, #tSalesBM, #tUnion	
	END	
--#endregion

END
--#endregion

--#region PRODUCTIVITY BY VEHICLE TYPE (1)
ELSE IF @ProductivityBy	= '1'
BEGIN				
	IF @National = '1'
	BEGIN					
	select * into #tNEW2 from(
	SELECT CompanyCode, BranchCode, TipeKendaraan, Variant, COUNT(LastProgress) NEW
		FROM SuzukiR4..pmHstITS WITH (NOLOCK, NOWAIT)
		WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE CompanyCode END) = @DealerCode
			AND (CASE WHEN @OutletCode = '' THEN '' ELSE BranchCode END) = @OutletCode	 
			AND CONVERT(VARCHAR, InquiryDate, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)
		GROUP BY CompanyCode, BranchCode, TipeKendaraan, Variant) #tNEW2									
					
	SELECT * INTO #tP21 FROM (
		SELECT Hist.CompanyCode, Hist.BranchCode,  ISNULL(TipeKendaraan, '[BLANK]') TipeKendaraan, ISNULL(Variant, '[BLANK]') Variant,
		COUNT(Hist.LastProgress) P
		FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
		LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
		HstITS.CompanyCode = Hist.CompanyCode AND
		HstITS.BranchCode = Hist.BranchCode AND
		HstITS.InquiryNumber = Hist.InquiryNumber 
		WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
		AND Hist.LastProgress = 'P'
		AND HstITS.LastProgress = 'P' -- penambahan 1 April 2014
		AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
		 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
		and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
												   where h.CompanyCode=Hist.CompanyCode
													 and h.BranchCode=Hist.BranchCode
													 and h.InquiryNumber=Hist.InquiryNumber
													 and h.LastProgress<>'P'
													 and convert(varchar,h.UpdateDate,112)<@StartDate)
		 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
												   where h.CompanyCode=Hist.CompanyCode
													 and h.BranchCode=Hist.BranchCode
													 and h.InquiryNumber=Hist.InquiryNumber
													 and h.LastProgress not in ('P','HP')
													 and convert(varchar,h.UpdateDate,112)<@StartDate)
		 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
												   where h.CompanyCode=Hist.CompanyCode
													 and h.BranchCode=Hist.BranchCode
													 and h.InquiryNumber=Hist.InquiryNumber
													 and h.LastProgress not in ('P','HP','SPK')
													 and convert(varchar,h.UpdateDate,112)<@StartDate))))	 
													  and HstITS.LastProgress not in ('DO','DELIVERY','LOST')		
													  AND CONVERT(VARCHAR, HstITS.LastUpdateStatus , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)	
		GROUP BY  Hist.CompanyCode, Hist.BranchCode, TipeKendaraan, Variant) #tP21	

	SELECT * INTO #tP22 FROM(
		SELECT Hist.CompanyCode, Hist.BranchCode,  ISNULL(TipeKendaraan, '[BLANK]') TipeKendaraan, ISNULL(Variant, '[BLANK]') Variant,
		COUNT(Hist.LastProgress) P
		FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
		LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
		HstITS.CompanyCode = Hist.CompanyCode AND
		HstITS.BranchCode = Hist.BranchCode AND
		HstITS.InquiryNumber = Hist.InquiryNumber 
		WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
		AND Hist.LastProgress = 'P'
		AND HstITS.LastProgress = 'P' -- penambahan 1 April 2014
		AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
		 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
		and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
												   where h.CompanyCode=Hist.CompanyCode
													 and h.BranchCode=Hist.BranchCode
													 and h.InquiryNumber=Hist.InquiryNumber
													 and h.LastProgress<>'P'
													 and convert(varchar,h.UpdateDate,112)<@StartDate)
		 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
												   where h.CompanyCode=Hist.CompanyCode
													 and h.BranchCode=Hist.BranchCode
													 and h.InquiryNumber=Hist.InquiryNumber
													 and h.LastProgress not in ('P','HP')
													 and convert(varchar,h.UpdateDate,112)<@StartDate)
		 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
												   where h.CompanyCode=Hist.CompanyCode
													 and h.BranchCode=Hist.BranchCode
													 and h.InquiryNumber=Hist.InquiryNumber
													 and h.LastProgress not in ('P','HP','SPK')
													 and convert(varchar,h.UpdateDate,112)<@StartDate))))	 
		AND CONVERT(VARCHAR, HstITS.LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)
		 and HstITS.LastProgress not in ('DO','DELIVERY','LOST')												
		GROUP BY  Hist.CompanyCode, Hist.BranchCode, TipeKendaraan, Variant) #tP22

	SELECT * INTO #tHP21 FROM(
		SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(TipeKendaraan, '[BLANK]') TipeKendaraan, ISNULL(Variant, '[BLANK]') Variant,
				COUNT(Hist.LastProgress) HP
				FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
				LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
					HstITS.CompanyCode = Hist.CompanyCode AND
					HstITS.BranchCode = Hist.BranchCode AND
					HstITS.InquiryNumber = Hist.InquiryNumber 
				 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
					AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
					AND Hist.LastProgress = 'HP'
					AND HstITS.LastProgress = 'HP' -- penambahan 1 April 2014
					AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
					 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
					and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress<>'P'
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP')
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP','SPK')
																 and convert(varchar,h.UpdateDate,112)<@StartDate))))	
																  and HstITS.LastProgress not in ('DO','DELIVERY','LOST')		 
																  AND CONVERT(VARCHAR, HstITS.LastUpdateStatus , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)	
				 GROUP BY  Hist.CompanyCode, Hist.BranchCode, TipeKendaraan, Variant) #tHP21


	SELECT * INTO #tHP22 FROM(
		SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(TipeKendaraan, '[BLANK]') TipeKendaraan, ISNULL(Variant, '[BLANK]') Variant,
				COUNT(Hist.LastProgress) HP
				FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
				LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
					HstITS.CompanyCode = Hist.CompanyCode AND
					HstITS.BranchCode = Hist.BranchCode AND
					HstITS.InquiryNumber = Hist.InquiryNumber 
				 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
					AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
					AND Hist.LastProgress = 'HP'
					AND HstITS.LastProgress = 'HP' -- penambahan 1 April 2014
					AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
					 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
					and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress<>'P'
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP')
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP','SPK')
																 and convert(varchar,h.UpdateDate,112)<@StartDate))))	 
				 AND CONVERT(VARCHAR, HstITS.LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)			
				  and HstITS.LastProgress not in ('DO','DELIVERY','LOST')														 
				 GROUP BY  Hist.CompanyCode, Hist.BranchCode, TipeKendaraan, Variant) #tHP22

	SELECT * INTO #tSPK21 FROM(
	SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(TipeKendaraan, '[BLANK]') TipeKendaraan, ISNULL(Variant, '[BLANK]') Variant,
				COUNT(Hist.LastProgress) SPK
				FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
				LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
					HstITS.CompanyCode = Hist.CompanyCode AND
					HstITS.BranchCode = Hist.BranchCode AND
					HstITS.InquiryNumber = Hist.InquiryNumber 
				 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
					AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
					AND Hist.LastProgress = 'SPK'
					AND HstITS.LastProgress = 'SPK' -- penambahan 1 April 2014
					AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
					 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
					and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress<>'P'
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP')
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP','SPK')
																 and convert(varchar,h.UpdateDate,112)<@StartDate))))
																  and HstITS.LastProgress not in ('DO','DELIVERY','LOST')	 
																  AND CONVERT(VARCHAR, HstITS.LastUpdateStatus , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)		 
				 GROUP BY  Hist.CompanyCode, Hist.BranchCode, TipeKendaraan, Variant) #tSPK21

	SELECT * INTO #tSPK22 FROM(
		SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(TipeKendaraan, '[BLANK]') TipeKendaraan, ISNULL(Variant, '[BLANK]') Variant,
				COUNT(Hist.LastProgress) SPK
				FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
				LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
					HstITS.CompanyCode = Hist.CompanyCode AND
					HstITS.BranchCode = Hist.BranchCode AND
					HstITS.InquiryNumber = Hist.InquiryNumber 
				 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
					AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
					AND Hist.LastProgress = 'SPK'
					AND HstITS.LastProgress = 'SPK' -- penambahan 1 April 2014
					AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
					 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
					and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress<>'P'
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP')
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP','SPK')
																 and convert(varchar,h.UpdateDate,112)<@StartDate))))	 
					AND CONVERT(VARCHAR, HstITS.LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)			 
					 and HstITS.LastProgress not in ('DO','DELIVERY','LOST')	 
				 GROUP BY  Hist.CompanyCode, Hist.BranchCode, TipeKendaraan, Variant) #tSPK22

	SELECT * INTO #tDO2 FROM(
	SELECT Hist.CompanyCode, Hist.BranchCode, TipeKendaraan, Variant, COUNT(Hist.LastProgress) DO
	FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
	INNER JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
		HstITS.CompanyCode = Hist.CompanyCode AND
		HstITS.BranchCode = Hist.BranchCode AND
		HstITS.InquiryNumber = Hist.InquiryNumber 
	 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
		AND Hist.LastProgress = 'DO'
		AND HstITS.LastProgress = 'DO' -- penambahan 1 April 2014
		AND CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 
	GROUP BY Hist.CompanyCode, Hist.BranchCode, TipeKendaraan, Variant) #tDO2
		
	SELECT * INTO #tDELIVERY2 FROM(
	SELECT Hist.CompanyCode, Hist.BranchCode, TipeKendaraan, Variant, COUNT(Hist.LastProgress) DELIVERY
	FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
	INNER JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
		HstITS.CompanyCode = Hist.CompanyCode AND
		HstITS.BranchCode = Hist.BranchCode AND
		HstITS.InquiryNumber = Hist.InquiryNumber 
	 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
		AND Hist.LastProgress = 'DELIVERY'
		AND HstITS.LastProgress = 'DELIVERY' -- penambahan 1 April 2014
		AND CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 
	GROUP BY Hist.CompanyCode, Hist.BranchCode, TipeKendaraan, Variant) #tDELIVERY2
		
	SELECT * INTO #tLOST2 FROM(
	SELECT Hist.CompanyCode, Hist.BranchCode, TipeKendaraan, Variant, COUNT(Hist.LastProgress) LOST  
	FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
	INNER JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
		HstITS.CompanyCode = Hist.CompanyCode AND
		HstITS.BranchCode = Hist.BranchCode AND
		HstITS.InquiryNumber = Hist.InquiryNumber 
	 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
		AND Hist.LastProgress = 'LOST'
		AND HstITS.LastProgress = 'LOST' -- penambahan 1 April 2014
		AND CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 
	GROUP BY Hist.CompanyCode, Hist.BranchCode, TipeKendaraan, Variant) #tLOST2

	SELECT * INTO #Temp2 FROM(
	SELECT DISTINCT ISNULL(HstITS.TipeKendaraan, '') TipeKendaraan,
			ISNULL(HstITS.Variant,'') TypeCode,
			ISNULL(NEW.NEW, 0) NEW,		
			ISNULL(CASE WHEN @TypeReport = '0' THEN P1.P ELSE P2.P END, (CASE WHEN HstITS.TipeKendaraan IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT P FROM #tP21 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = '[BLANK]'), 0) ELSE ISNULL((SELECT P FROM #tP22 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = '[BLANK]'), 0) END) END)) P,		 
			ISNULL(CASE WHEN @TypeReport = '0' THEN HP1.HP ELSE HP2.HP END, (CASE WHEN HstITS.TipeKendaraan IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT HP FROM #tHP21 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = '[BLANK]'),0) ELSE ISNULL((SELECT HP FROM #tHP22 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = '[BLANK]'),0)  END) END)) HP,		
			ISNULL(CASE WHEN @TypeReport = '0' THEN SPK1.SPK ELSE SPK2.SPK END, (CASE WHEN HstITS.TipeKendaraan IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT SPK FROM #tSPK21 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = '[BLANK]'),0) ELSE ISNULL((SELECT SPK FROM #tSPK22 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = '[BLANK]'),0) END) END)) SPK,

			(ISNULL(CASE WHEN @TypeReport = '0' THEN P1.P ELSE P2.P END, (CASE WHEN HstITS.TipeKendaraan IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT P FROM #tP21 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = '[BLANK]'), 0) ELSE ISNULL((SELECT P FROM #tP22 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = '[BLANK]'), 0) END) END))
				+ISNULL(CASE WHEN @TypeReport = '0' THEN HP1.HP ELSE HP2.HP END, (CASE WHEN HstITS.TipeKendaraan IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT HP FROM #tHP21 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = '[BLANK]'),0) ELSE ISNULL((SELECT HP FROM #tHP22 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = '[BLANK]'),0) END) END))
				+ISNULL(CASE WHEN @TypeReport = '0' THEN SPK1.SPK ELSE SPK2.SPK END, (CASE WHEN HstITS.TipeKendaraan IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT SPK FROM #tSPK21 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = '[BLANK]'),0) ELSE ISNULL((SELECT SPK FROM #tSPK22 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = '[BLANK]'),0) END) END))) SumOuts,

			ISNULL(DO.DO, 0) DO, ISNULL(DELIVERY.DELIVERY, 0) DELIVERY, ISNULL(LOST.LOST, 0) LOST			
		FROM SuzukiR4..pmStatusHistory a WITH (NOLOCK, NOWAIT)
		LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON HstITS.CompanyCode = a.CompanyCode AND HstITS.BranchCode = a.BranchCode AND HstITS.InquiryNumber = a.InquiryNumber 
		LEFT JOIN SuzukiR4..gnMstDealerMapping b WITH (NOLOCK, NOWAIT) on a.CompanyCode = b.DealerCode	
		LEFT JOIN #tNEW2 NEW WITH (NOLOCK, NOWAIT) ON NEW.CompanyCode = a.CompanyCode AND NEW.BranchCode = a.BranchCode AND NEW.TipeKendaraan = HstITS.TipeKendaraan AND NEW.Variant = HstITS.Variant
		LEFT JOIN #tP21 P1 WITH (NOLOCK, NOWAIT) ON P1.CompanyCode = a.CompanyCode AND P1.BranchCode = a.BranchCode AND P1.TipeKendaraan = HstITS.TipeKendaraan AND P1.Variant = HstITS.Variant	
		LEFT JOIN #tP22 P2 WITH (NOLOCK, NOWAIT) ON P2.CompanyCode = a.CompanyCode AND P2.BranchCode = a.BranchCode AND P2.TipeKendaraan = HstITS.TipeKendaraan AND P2.Variant = HstITS.Variant				
		LEFT JOIN #tHP21 HP1 WITH (NOLOCK, NOWAIT) ON HP1.CompanyCode = a.CompanyCode AND HP1.BranchCode = a.BranchCode AND HP1.TipeKendaraan = HstITS.TipeKendaraan AND HP1.Variant = HstITS.Variant 			
		LEFT JOIN #tHP22 HP2 WITH (NOLOCK, NOWAIT) ON HP2.CompanyCode = a.CompanyCode AND HP2.BranchCode = a.BranchCode AND HP2.TipeKendaraan = HstITS.TipeKendaraan	AND HP2.Variant = HstITS.Variant 			
		LEFT JOIN #tSPK21 SPK1 WITH (NOLOCK, NOWAIT) ON SPK1.CompanyCode = a.CompanyCode AND SPK1.BranchCode = a.BranchCode AND SPK1.TipeKendaraan = HstITS.TipeKendaraan AND SPK1.Variant = HstITS.Variant 
		LEFT JOIN #tSPK22 SPK2 WITH (NOLOCK, NOWAIT) ON SPK2.CompanyCode = a.CompanyCode AND SPK2.BranchCode = a.BranchCode AND SPK2.TipeKendaraan = HstITS.TipeKendaraan AND SPK2.Variant = HstITS.Variant 	
		LEFT JOIN #tDO2 DO WITH (NOLOCK, NOWAIT) ON DO.CompanyCode = a.CompanyCode AND DO.BranchCode = a.BranchCode AND DO.TipeKendaraan = HstITS.TipeKendaraan AND DO.Variant = HstITS.Variant	 
		LEFT JOIN #tDELIVERY2 DELIVERY WITH (NOLOCK, NOWAIT) ON DELIVERY.CompanyCode = a.CompanyCode AND DELIVERY.BranchCode = a.BranchCode AND DELIVERY.TipeKendaraan = HstITS.TipeKendaraan AND DELIVERY.Variant = HstITS.Variant		
		LEFT JOIN #tLOST2 LOST WITH (NOLOCK, NOWAIT) ON LOST.CompanyCode = a.CompanyCode AND LOST.BranchCode = a.BranchCode AND LOST.TipeKendaraan = HstITS.TipeKendaraan AND LOST.Variant = HstITS.Variant						   					   		   			   		
		WHERE (CASE WHEN @Area = '' THEN '' ELSE b.Area END) = @Area
		AND (CASE WHEN @DealerCode = '' THEN '' ELSE a.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE a.BranchCode END) = @OutletCode) #Temp2	

	SELECT 
	CASE WHEN @TypeReport = '0' THEN 'Summary' ELSE 'Saldo' END TypeReport,
		CASE @ProductivityBy 
			WHEN '0' THEN 'Salesman'
			WHEN '1' THEN 'Vehicle Type'
			WHEN '2' THEN 'Source Data'
		END ProductivityBy,
		CONVERT(VARCHAR, @EndDate, 105) PerDate, CASE WHEN @Area = '' THEN 'ALL' ELSE @Area END Area, CONVERT(VARCHAR, @StartDate, 105) + ' s/d ' + CONVERT(VARCHAR, @EndDate, 105) PeriodeDO,
		CASE WHEN @SalesHead = '' THEN 'ALL' ELSE @SalesHead END SalesHead,
		CASE WHEN @SalesCoordinator = '' THEN 'ALL' ELSE @SalesCoordinator END SalesCoordinator,
		CASE WHEN @Salesman = '' THEN 'ALL' ELSE @Salesman END Salesman,
		CASE WHEN @DealerCode = '' THEN 'ALL' ELSE (SELECT DealerName FROM SuzukiR4..gnMstDealerMapping WHERE DealerCode = @DealerCode) END Dealer,
		CASE WHEN @OutletCode = '' THEN 'ALL' ELSE (SELECT OutletName FROM SuzukiR4..gnMstDealerOutletMapping WHERE DealerCode = @DealerCode AND OutletCode = @OutletCode) END Outlet,
		b.TipeKendaraan,
		ISNULL(SUM(b.P), 0) NEW,
		ISNULL(SUM(b.P), 0) P,
		ISNULL(SUM(b.HP), 0) HP,
		ISNULL(SUM(b.SPK), 0) SPK,
		ISNULL(SUM(b.SumOuts), 0) SumOuts,
		ISNULL(SUM(b.DO), 0) DO,
		ISNULL(SUM(b.DELIVERY), 0) DELIVERY,
		ISNULL(SUM(b.LOST), 0) LOST	
			
	--CASE CAST(ISNULL(b.NEW, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.NEW AS VARCHAR) END NEW,
	--CASE CAST(ISNULL(b.P, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.P AS VARCHAR) END P,
	--CASE CAST(ISNULL(b.HP, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.HP AS VARCHAR) END HP,
	--CASE CAST(ISNULL(b.SPK, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.SPK AS VARCHAR) END SPK,
	--CASE CAST(ISNULL(b.SumOuts, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.SumOuts AS VARCHAR) END SumOuts,
	--CASE CAST(ISNULL(b.DO, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.DO AS VARCHAR) END DO,
	--CASE CAST(ISNULL(b.DELIVERY, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.DELIVERY AS VARCHAR) END DELIVERY,
	--CASE CAST(ISNULL(b.LOST, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.LOST AS VARCHAR) END LOST
	FROM #Temp2 b 	
	GROUP BY b.TipeKendaraan
	
	DROP TABLE #tNEW2, #tP21, #tP22, #tHP21, #tHP22, #tSPK21, #tSPK22, #tDO2, #tDELIVERY2, #tLOST2, #Temp2	
	END					
	ELSE
	BEGIN
		SELECT DISTINCT
	CASE WHEN @TypeReport = '0' THEN 'Summary' ELSE 'Saldo' END TypeReport,
		CASE @ProductivityBy 
			WHEN '0' THEN 'Salesman'
			WHEN '1' THEN 'Vehicle Type'
			WHEN '2' THEN 'Source Data'
		END ProductivityBy,
		CONVERT(VARCHAR, @EndDate, 105) PerDate, @Area Area, CONVERT(VARCHAR, @StartDate, 105) + ' s/d ' + CONVERT(VARCHAR, @EndDate, 105) PeriodeDO,
		CASE WHEN @SalesHead = '' THEN 'ALL' ELSE (SELECT EmployeeName FROM SuzukiR4..GnMstEmployee WHERE CompanyCode = a.CompanyCode AND BranchCode = @OutletCode AND EmployeeID = @SalesHead) END SalesHead,
		CASE WHEN @SalesCoordinator = '' THEN 'ALL' ELSE (SELECT EmployeeName FROM SuzukiR4..GnMstEmployee WHERE CompanyCode = a.CompanyCode AND BranchCode = @OutletCode AND EmployeeID = @SalesCoordinator) END SalesCoordinator,
		CASE WHEN @Salesman = '' THEN 'ALL' ELSE (SELECT EmployeeName FROM SuzukiR4..GnMstEmployee WHERE CompanyCode = a.CompanyCode AND BranchCode = @OutletCode AND EmployeeID = @Salesman) END Salesman,
		CASE WHEN @DealerCode = '' THEN 'ALL' ELSE (SELECT DealerName FROM SuzukiR4..gnMstDealerMapping WHERE DealerCode = @DealerCode) END Dealer,
		CASE WHEN @OutletCode = '' THEN 'ALL' ELSE (SELECT BranchName FROM SuzukiR4..gnMstOrganizationDtl WHERE CompanyCode = @DealerCode AND BranchCode = @OutletCode) END Outlet,
	(a.GroupCode + ' ' + a.TypeCode) TipeKendaraan,
	ISNULL(b.P, 0) NEW,
	ISNULL(b.P, 0) P,
	ISNULL(b.HP, 0) HP,
	ISNULL(b.SPK, 0) SPK,
	ISNULL(b.SumOuts, 0) SumOuts,
	ISNULL(b.DO, 0) DO,
	ISNULL(b.DELIVERY, 0) DELIVERY,
	ISNULL(b.LOST, 0) LOST	
			
	--CASE CAST(ISNULL(b.NEW, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.NEW AS VARCHAR) END NEW,
	--CASE CAST(ISNULL(b.P, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.P AS VARCHAR) END P,
	--CASE CAST(ISNULL(b.HP, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.HP AS VARCHAR) END HP,
	--CASE CAST(ISNULL(b.SPK, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.SPK AS VARCHAR) END SPK,
	--CASE CAST(ISNULL(b.SumOuts, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.SumOuts AS VARCHAR) END SumOuts,
	--CASE CAST(ISNULL(b.DO, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.DO AS VARCHAR) END DO,
	--CASE CAST(ISNULL(b.DELIVERY, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.DELIVERY AS VARCHAR) END DELIVERY,
	--CASE CAST(ISNULL(b.LOST, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.LOST AS VARCHAR) END LOST
	FROM SuzukiR4..pmGroupTypeSeq a
	LEFT JOIN 
	(
	SELECT a.TipeKendaraan, 
		   a.Variant TypeCode,
		   (SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
			WHERE CompanyCode = a.CompanyCode
				AND BranchCode = a.BranchCode
				AND TipeKendaraan = a.TipeKendaraan 
				AND Variant = a.Variant 
				AND CONVERT(VARCHAR, InquiryDate, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)) NEW,
			(CASE WHEN @TypeReport = '0' THEN
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = a.CompanyCode
					AND BranchCode = a.BranchCode
					AND TipeKendaraan = a.TipeKendaraan  
					AND Variant = a.Variant 
					AND LastProgress = 'P'  
					AND CONVERT(VARCHAR, LastUpdateStatus, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)) 
			ELSE		
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = a.CompanyCode
					AND BranchCode = a.BranchCode	
					AND TipeKendaraan = a.TipeKendaraan  
					AND Variant = a.Variant 
					AND LastProgress = 'P'  
					AND CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) END) P,	
									
			(CASE WHEN @TypeReport = '0' THEN			
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE  CompanyCode = a.CompanyCode
					AND BranchCode = a.BranchCode		
					AND TipeKendaraan = a.TipeKendaraan  
					AND Variant = a.Variant 
					AND LastProgress = 'HP'  
					AND CONVERT(VARCHAR, LastUpdateStatus, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112))
			ELSE	
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = a.CompanyCode
					AND BranchCode = a.BranchCode
					AND TipeKendaraan = a.TipeKendaraan  
					AND Variant = a.Variant 
					AND LastProgress = 'HP'  
					AND CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) END) HP,
					
			(CASE WHEN @TypeReport = '0' THEN
				(SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
					INNER JOIN SuzukiR4..pmKdp KDP ON 
					KDP.CompanyCode = Hist.CompanyCode AND
					KDP.BranchCode = Hist.BranchCode AND
					KDP.InquiryNumber = Hist.InquiryNumber 
				 WHERE  Hist.CompanyCode = a.CompanyCode
					AND Hist.BranchCode = a.BranchCode
					AND KDP.TipeKendaraan = a.TipeKendaraan  
					AND KDP.Variant = a.Variant 				
					AND CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 
					AND Hist.LastProgress = 'SPK')
			ELSE
			(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = a.CompanyCode
					AND BranchCode = a.BranchCode
					AND TipeKendaraan = a.TipeKendaraan  
					AND Variant = a.Variant 	
					AND LastProgress = 'SPK'  
					AND CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) 
				END) SPK,
			
			(CASE WHEN @TypeReport = '0' THEN	
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = a.CompanyCode
					AND BranchCode = a.BranchCode
					AND TipeKendaraan = a.TipeKendaraan  
					AND Variant = a.Variant 
					AND LastProgress = 'P'  
					AND CONVERT(VARCHAR, LastUpdateStatus, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)) +
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE  CompanyCode = a.CompanyCode
					AND BranchCode = a.BranchCode		
					AND TipeKendaraan = a.TipeKendaraan  
					AND Variant = a.Variant 
					AND LastProgress = 'HP'  
					AND CONVERT(VARCHAR, LastUpdateStatus, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)) +
				(SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
					INNER JOIN SuzukiR4..pmKdp KDP ON 
					KDP.CompanyCode = Hist.CompanyCode AND
					KDP.BranchCode = Hist.BranchCode AND
					KDP.InquiryNumber = Hist.InquiryNumber 
				 WHERE Hist.CompanyCode = @DealerCode AND
					Hist.BranchCode = @OutletCode AND
					KDP.TipeKendaraan = a.TipeKendaraan AND 
					KDP.Variant = a.Variant AND				
					CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
					Hist.LastProgress = 'SPK')
			ELSE
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = a.CompanyCode
					AND BranchCode = a.BranchCode				
					AND TipeKendaraan = a.TipeKendaraan  
					AND Variant = a.Variant 
					AND LastProgress = 'P'  
					AND CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) +
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = a.CompanyCode
					AND BranchCode = a.BranchCode			
					AND TipeKendaraan = a.TipeKendaraan  
					AND Variant = a.Variant 
					AND LastProgress = 'HP'  
					AND CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) + 			
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
					WHERE CompanyCode = a.CompanyCode
					AND BranchCode = a.BranchCode	
					AND TipeKendaraan = a.TipeKendaraan  
					AND Variant = a.Variant 	
					AND LastProgress = 'SPK'  
					AND CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) 
					END) SumOuts,
					
			(SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
			INNER JOIN SuzukiR4..pmKdp KDP ON 
				KDP.CompanyCode = Hist.CompanyCode AND
				KDP.BranchCode = Hist.BranchCode AND
				KDP.InquiryNumber = Hist.InquiryNumber 
			 WHERE Hist.CompanyCode = a.CompanyCode AND
				Hist.BranchCode = a.BranchCode AND
				KDP.TipeKendaraan = a.TipeKendaraan AND 
				KDP.Variant = a.Variant AND			
				CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
				Hist.LastProgress = 'DO') DO,
			(SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
				INNER JOIN SuzukiR4..pmKdp KDP ON 
				KDP.CompanyCode = Hist.CompanyCode AND
				KDP.BranchCode = Hist.BranchCode AND
				KDP.InquiryNumber = Hist.InquiryNumber 
			 WHERE Hist.CompanyCode = a.CompanyCode AND
				Hist.BranchCode = a.BranchCode AND
				KDP.TipeKendaraan = a.TipeKendaraan AND 
				KDP.Variant = a.Variant AND		
				CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
				Hist.LastProgress = 'DELIVERY') DELIVERY,
			(SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
				INNER JOIN SuzukiR4..pmKdp KDP ON 
				KDP.CompanyCode = Hist.CompanyCode AND
				KDP.BranchCode = Hist.BranchCode AND
				KDP.InquiryNumber = Hist.InquiryNumber 
			 WHERE Hist.CompanyCode = a.CompanyCode AND
				Hist.BranchCode = a.BranchCode AND
				KDP.TipeKendaraan = a.TipeKendaraan AND 
				KDP.Variant = a.Variant AND		
				CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
				Hist.LastProgress = 'LOST') LOST
	FROM SuzukiR4..pmKdp a
	WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE a.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE a.BranchCode END) = @OutletCode
	GROUP BY a.CompanyCode, a.BranchCode, a.TipeKendaraan, a.Variant
	) b ON a.GroupCode = b.TipeKendaraan AND a.TypeCode = b.TypeCode
	WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE a.CompanyCode END) = @DealerCode
	END	
END	
--#endregion

--#region PRODUCTIVITY BY SOURCE DATA (2)	
ELSE IF @ProductivityBy = '2'
BEGIN
	SELECT * INTO #employee_src FROM (
		SELECT CompanyCode, LookUpValue
		FROM SuzukiR4..GnMstLookUpDtl 
		WHERE CodeID = 'PSRC'
	) #employee_src	
				
	IF @National = '1' 
	BEGIN	
		SELECT * INTO #employee_src_2_n FROM
		(
			SELECT DISTINCT Saleshead
			FROM SuzukiR4..pmHstITS a
			LEFT JOIN SuzukiR4..gnMstDealerMapping b on a.CompanyCode = b.DealerCode	
			WHERE (CASE WHEN @Area = '' THEN '' ELSE b.Area END) = @Area				
				  AND (CASE WHEN @DealerCode = '' THEN '' ELSE b.DealerCode END) = @DealerCode
				  AND (CASE WHEN @OutletCode = '' THEN '' ELSE a.BranchCode END) = @OutletCode	
		) #employee_src_2_n
		
SELECT * INTO #employee_src_3_n FROM
		(
			SELECT DISTINCT SalesCoordinator
				FROM SuzukiR4..pmHstITS a
				LEFT JOIN SuzukiR4..gnMstDealerMapping b on a.CompanyCode = b.DealerCode	
				WHERE (CASE WHEN @Area = '' THEN '' ELSE b.Area END) = @Area				
					  AND (CASE WHEN @DealerCode = '' THEN '' ELSE b.DealerCode END) = @DealerCode
					  AND (CASE WHEN @OutletCode = '' THEN '' ELSE a.BranchCode END) = @OutletCode	
					  AND SalesHead in (SELECT SalesHead FROM #employee_src_2_n)
		)#employee_src_3_n

SELECT * INTO #tNEW3 FROM (
	SELECT CompanyCode, BranchCode, PerolehanData, SalesCoordinator, COUNT(LastProgress) NEW
	FROM SuzukiR4..pmHstITS WITH (NOLOCK, NOWAIT)
	WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE BranchCode END) = @OutletCode	 
		AND CONVERT(VARCHAR, InquiryDate, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)
	GROUP BY CompanyCode, BranchCode, PerolehanData, SalesCoordinator
) #tNEW3							  		

SELECT * INTO #tP31 FROM (
		SELECT Hist.CompanyCode, Hist.BranchCode,  ISNULL(PerolehanData, '[BLANK]') PerolehanData, ISNULL(SalesCoordinator, '[BLANK]') SalesCoordinator,
		COUNT(Hist.LastProgress) P
		FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
		LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
		HstITS.CompanyCode = Hist.CompanyCode AND
		HstITS.BranchCode = Hist.BranchCode AND
		HstITS.InquiryNumber = Hist.InquiryNumber 
		WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
		AND Hist.LastProgress = 'P'
		AND HstITS.LastProgress = 'P' -- penambahan 1 April 2014
		AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
		 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
		and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
												   where h.CompanyCode=Hist.CompanyCode
													 and h.BranchCode=Hist.BranchCode
													 and h.InquiryNumber=Hist.InquiryNumber
													 and h.LastProgress<>'P'
													 and convert(varchar,h.UpdateDate,112)<@StartDate)
		 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
												   where h.CompanyCode=Hist.CompanyCode
													 and h.BranchCode=Hist.BranchCode
													 and h.InquiryNumber=Hist.InquiryNumber
													 and h.LastProgress not in ('P','HP')
													 and convert(varchar,h.UpdateDate,112)<@StartDate)
		 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
												   where h.CompanyCode=Hist.CompanyCode
													 and h.BranchCode=Hist.BranchCode
													 and h.InquiryNumber=Hist.InquiryNumber
													 and h.LastProgress not in ('P','HP','SPK')
													 and convert(varchar,h.UpdateDate,112)<@StartDate))))	
													  and HstITS.LastProgress not in ('DO','DELIVERY','LOST')		
													  AND CONVERT(VARCHAR, HstITS.LastUpdateStatus , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)	 
		GROUP BY  Hist.CompanyCode, Hist.BranchCode, PerolehanData, SalesCoordinator) #tP31	

SELECT * INTO #tP32 FROM(
		SELECT Hist.CompanyCode, Hist.BranchCode,  ISNULL(PerolehanData, '[BLANK]') PerolehanData, ISNULL(SalesCoordinator, '[BLANK]') SalesCoordinator,
		COUNT(Hist.LastProgress) P
		FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
		LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
		HstITS.CompanyCode = Hist.CompanyCode AND
		HstITS.BranchCode = Hist.BranchCode AND
		HstITS.InquiryNumber = Hist.InquiryNumber 
		WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
		AND Hist.LastProgress = 'P'
		AND HstITS.LastProgress = 'P' -- penambahan 1 April 2014
		AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
		 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
		and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
												   where h.CompanyCode=Hist.CompanyCode
													 and h.BranchCode=Hist.BranchCode
													 and h.InquiryNumber=Hist.InquiryNumber
													 and h.LastProgress<>'P'
													 and convert(varchar,h.UpdateDate,112)<@StartDate)
		 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
												   where h.CompanyCode=Hist.CompanyCode
													 and h.BranchCode=Hist.BranchCode
													 and h.InquiryNumber=Hist.InquiryNumber
													 and h.LastProgress not in ('P','HP')
													 and convert(varchar,h.UpdateDate,112)<@StartDate)
		 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
												   where h.CompanyCode=Hist.CompanyCode
													 and h.BranchCode=Hist.BranchCode
													 and h.InquiryNumber=Hist.InquiryNumber
													 and h.LastProgress not in ('P','HP','SPK')
													 and convert(varchar,h.UpdateDate,112)<@StartDate))))	 
		AND CONVERT(VARCHAR, HstITS.LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)				
		 and HstITS.LastProgress not in ('DO','DELIVERY','LOST')								
		GROUP BY  Hist.CompanyCode, Hist.BranchCode, PerolehanData, SalesCoordinator) #tP32

SELECT * INTO #tHP31 FROM(
		SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(PerolehanData, '[BLANK]') PerolehanData, ISNULL(SalesCoordinator, '[BLANK]') SalesCoordinator,
				COUNT(Hist.LastProgress) HP
				FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
				LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
					HstITS.CompanyCode = Hist.CompanyCode AND
					HstITS.BranchCode = Hist.BranchCode AND
					HstITS.InquiryNumber = Hist.InquiryNumber 
				 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
					AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
					AND Hist.LastProgress = 'HP'
					AND HstITS.LastProgress = 'HP' -- penambahan 1 April 2014
					AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
					 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
					and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress<>'P'
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP')
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP','SPK')
																 and convert(varchar,h.UpdateDate,112)<@StartDate))))	 
																  and HstITS.LastProgress not in ('DO','DELIVERY','LOST')		
																  AND CONVERT(VARCHAR, HstITS.LastUpdateStatus , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)	
				 GROUP BY  Hist.CompanyCode, Hist.BranchCode, PerolehanData, SalesCoordinator) #tHP31

SELECT * INTO #tHP32 FROM(
		SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(PerolehanData, '[BLANK]') PerolehanData, ISNULL(SalesCoordinator, '[BLANK]') SalesCoordinator,
				COUNT(Hist.LastProgress) HP
				FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
				LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
					HstITS.CompanyCode = Hist.CompanyCode AND
					HstITS.BranchCode = Hist.BranchCode AND
					HstITS.InquiryNumber = Hist.InquiryNumber 
				 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
					AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
					AND Hist.LastProgress = 'HP'
					AND HstITS.LastProgress = 'HP' -- penambahan 1 April 2014
					AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
					 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
					and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress<>'P'
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP')
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP','SPK')
																 and convert(varchar,h.UpdateDate,112)<@StartDate))))	 
				 AND CONVERT(VARCHAR, HstITS.LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)					
				  and HstITS.LastProgress not in ('DO','DELIVERY','LOST')												 
				 GROUP BY  Hist.CompanyCode, Hist.BranchCode, PerolehanData, SalesCoordinator) #tHP32				 				 

	SELECT * INTO #tSPK31 FROM(
	SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(PerolehanData, '[BLANK]') PerolehanData, ISNULL(SalesCoordinator, '[BLANK]') SalesCoordinator,
				COUNT(Hist.LastProgress) SPK
				FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
				LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
					HstITS.CompanyCode = Hist.CompanyCode AND
					HstITS.BranchCode = Hist.BranchCode AND
					HstITS.InquiryNumber = Hist.InquiryNumber 
				 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
					AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
					AND Hist.LastProgress = 'SPK'
					AND HstITS.LastProgress = 'SPK' -- penambahan 1 April 2014
					AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
					 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
					and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress<>'P'
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP')
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP','SPK')
																 and convert(varchar,h.UpdateDate,112)<@StartDate))))	 
																  and HstITS.LastProgress not in ('DO','DELIVERY','LOST')
																  AND CONVERT(VARCHAR, HstITS.LastUpdateStatus , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)		 
				 GROUP BY  Hist.CompanyCode, Hist.BranchCode, PerolehanData, SalesCoordinator) #tSPK31				 


	SELECT * INTO #tSPK32 FROM(
		SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(PerolehanData, '[BLANK]') PerolehanData, ISNULL(SalesCoordinator, '[BLANK]') SalesCoordinator,
				COUNT(Hist.LastProgress) SPK
				FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
				LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
					HstITS.CompanyCode = Hist.CompanyCode AND
					HstITS.BranchCode = Hist.BranchCode AND
					HstITS.InquiryNumber = Hist.InquiryNumber 
				 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
					AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
					AND Hist.LastProgress = 'SPK'
					AND HstITS.LastProgress = 'SPK' -- penambahan 1 April 2014
					AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
					 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
					and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress<>'P'
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP')
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP','SPK')
																 and convert(varchar,h.UpdateDate,112)<@StartDate))))	 
					AND CONVERT(VARCHAR, HstITS.LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)			 
					 and HstITS.LastProgress not in ('DO','DELIVERY','LOST')	 
				 GROUP BY  Hist.CompanyCode, Hist.BranchCode, PerolehanData, SalesCoordinator) #tSPK32
				 

SELECT * INTO #tDO3 FROM(
SELECT Hist.CompanyCode, Hist.BranchCode, hstITS.PerolehanData, hstITS.SalesCoordinator, COUNT(Hist.LastProgress) DO
FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
INNER JOIN SuzukiR4..pmHstITS hstITS WITH (NOLOCK, NOWAIT) ON 
	hstITS.CompanyCode = Hist.CompanyCode AND
	hstITS.BranchCode = Hist.BranchCode AND
	hstITS.InquiryNumber = Hist.InquiryNumber 
 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	 	
		AND CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 
		AND Hist.LastProgress = 'DO'
		AND HstITS.LastProgress = 'DO' -- penambahan 1 April 2014
	GROUP BY Hist.CompanyCode, Hist.BranchCode, hstITS.PerolehanData, hstITS.SalesCoordinator) #tDO3

SELECT * INTO #tDELIVERY3 FROM(
SELECT Hist.CompanyCode, Hist.BranchCode, hstITS.PerolehanData, hstITS.SalesCoordinator, COUNT(Hist.LastProgress) DELIVERY
FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
INNER JOIN SuzukiR4..pmHstITS hstITS WITH (NOLOCK, NOWAIT) ON 
	hstITS.CompanyCode = Hist.CompanyCode AND
	hstITS.BranchCode = Hist.BranchCode AND
	hstITS.InquiryNumber = Hist.InquiryNumber 
 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	 	
		AND CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 
		AND Hist.LastProgress = 'DELIVERY'
		AND HstITS.LastProgress = 'DELIVERY' -- penambahan 1 April 2014
	GROUP BY Hist.CompanyCode, Hist.BranchCode, hstITS.PerolehanData, hstITS.SalesCoordinator) #tDELIVERY3

SELECT * INTO #tLOST3 FROM(
SELECT Hist.CompanyCode, Hist.BranchCode, hstITS.PerolehanData, hstITS.SalesCoordinator, COUNT(Hist.LastProgress) LOST
FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
INNER JOIN SuzukiR4..pmHstITS hstITS WITH (NOLOCK, NOWAIT) ON 
	hstITS.CompanyCode = Hist.CompanyCode AND
	hstITS.BranchCode = Hist.BranchCode AND
	hstITS.InquiryNumber = Hist.InquiryNumber 
 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	 	
		AND CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 
		AND Hist.LastProgress = 'LOST'
		AND HstITS.LastProgress = 'LOST' -- penambahan 1 April 2014
	GROUP BY Hist.CompanyCode, Hist.BranchCode, hstITS.PerolehanData, hstITS.SalesCoordinator) #tLOST3

SELECT * INTO #Temp3 FROM
(
SELECT DISTINCT HstITS.SalesCoordinator, ISNULL(HstITS.PerolehanData, '') PerolehanData,			
			ISNULL(NEW.NEW, 0) NEW,		
						
			ISNULL(CASE WHEN @TypeReport = '0' THEN P1.P ELSE P2.P END, (CASE WHEN HstITS.PerolehanData IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT P FROM #tP31 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = '[BLANK]'), 0) ELSE ISNULL((SELECT P FROM #tP32 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = '[BLANK]'), 0) END) END)) P,		 
			ISNULL(CASE WHEN @TypeReport = '0' THEN HP1.HP ELSE HP2.HP END, (CASE WHEN HstITS.PerolehanData IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT HP FROM #tHP31 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = '[BLANK]'),0) ELSE ISNULL((SELECT HP FROM #tHP32 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = '[BLANK]'),0)  END) END)) HP,		
			ISNULL(CASE WHEN @TypeReport = '0' THEN SPK1.SPK ELSE SPK2.SPK END, (CASE WHEN HstITS.PerolehanData IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT SPK FROM #tSPK31 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = '[BLANK]'),0) ELSE ISNULL((SELECT SPK FROM #tSPK32 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = '[BLANK]'),0) END) END)) SPK,

			(ISNULL(CASE WHEN @TypeReport = '0' THEN P1.P ELSE P2.P END, (CASE WHEN HstITS.PerolehanData IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT P FROM #tP31 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = '[BLANK]'), 0) ELSE ISNULL((SELECT P FROM #tP32 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = '[BLANK]'), 0) END) END))
				+ISNULL(CASE WHEN @TypeReport = '0' THEN HP1.HP ELSE HP2.HP END, (CASE WHEN HstITS.PerolehanData IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT HP FROM #tHP31 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = '[BLANK]'),0) ELSE ISNULL((SELECT HP FROM #tHP32 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = '[BLANK]'),0) END) END))
				+ISNULL(CASE WHEN @TypeReport = '0' THEN SPK1.SPK ELSE SPK2.SPK END, (CASE WHEN HstITS.PerolehanData IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT SPK FROM #tSPK31 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = '[BLANK]'),0) ELSE ISNULL((SELECT SPK FROM #tSPK32 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = '[BLANK]'),0) END) END))) SumOuts,

			ISNULL(DO.DO, 0) DO, ISNULL(DELIVERY.DELIVERY, 0) DELIVERY, ISNULL(LOST.LOST, 0) LOST			
		FROM SuzukiR4..pmStatusHistory a WITH (NOLOCK, NOWAIT)
		LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON HstITS.CompanyCode = a.CompanyCode AND HstITS.BranchCode = a.BranchCode AND HstITS.InquiryNumber = a.InquiryNumber 
		LEFT JOIN SuzukiR4..gnMstDealerMapping b WITH (NOLOCK, NOWAIT) on a.CompanyCode = b.DealerCode	
		LEFT JOIN #tNEW3 NEW WITH (NOLOCK, NOWAIT) ON NEW.CompanyCode = a.CompanyCode AND NEW.BranchCode = a.BranchCode AND NEW.PerolehanData = HstITS.PerolehanData AND NEW.SalesCoordinator = HstITS.SalesCoordinator
		LEFT JOIN #tP31 P1 WITH (NOLOCK, NOWAIT) ON P1.CompanyCode = a.CompanyCode AND P1.BranchCode = a.BranchCode AND P1.PerolehanData = HstITS.PerolehanData AND P1.SalesCoordinator = HstITS.SalesCoordinator	
		LEFT JOIN #tP32 P2 WITH (NOLOCK, NOWAIT) ON P2.CompanyCode = a.CompanyCode AND P2.BranchCode = a.BranchCode AND P2.PerolehanData = HstITS.PerolehanData AND P2.SalesCoordinator = HstITS.SalesCoordinator				
		LEFT JOIN #tHP31 HP1 WITH (NOLOCK, NOWAIT) ON HP1.CompanyCode = a.CompanyCode AND HP1.BranchCode = a.BranchCode AND HP1.PerolehanData = HstITS.PerolehanData AND HP1.SalesCoordinator = HstITS.SalesCoordinator 			
		LEFT JOIN #tHP32 HP2 WITH (NOLOCK, NOWAIT) ON HP2.CompanyCode = a.CompanyCode AND HP2.BranchCode = a.BranchCode AND HP2.PerolehanData = HstITS.PerolehanData	AND HP2.SalesCoordinator = HstITS.SalesCoordinator 			
		LEFT JOIN #tSPK31 SPK1 WITH (NOLOCK, NOWAIT) ON SPK1.CompanyCode = a.CompanyCode AND SPK1.BranchCode = a.BranchCode AND SPK1.PerolehanData = HstITS.PerolehanData AND SPK1.SalesCoordinator = HstITS.SalesCoordinator 
		LEFT JOIN #tSPK32 SPK2 WITH (NOLOCK, NOWAIT) ON SPK2.CompanyCode = a.CompanyCode AND SPK2.BranchCode = a.BranchCode AND SPK2.PerolehanData = HstITS.PerolehanData AND SPK2.SalesCoordinator = HstITS.SalesCoordinator 	
		LEFT JOIN #tDO3 DO WITH (NOLOCK, NOWAIT) ON DO.CompanyCode = a.CompanyCode AND DO.BranchCode = a.BranchCode AND DO.PerolehanData = HstITS.PerolehanData AND DO.SalesCoordinator = HstITS.SalesCoordinator	 
		LEFT JOIN #tDELIVERY3 DELIVERY WITH (NOLOCK, NOWAIT) ON DELIVERY.CompanyCode = a.CompanyCode AND DELIVERY.BranchCode = a.BranchCode AND DELIVERY.PerolehanData = HstITS.PerolehanData AND DELIVERY.SalesCoordinator = HstITS.SalesCoordinator		
		LEFT JOIN #tLOST3 LOST WITH (NOLOCK, NOWAIT) ON LOST.CompanyCode = a.CompanyCode AND LOST.BranchCode = a.BranchCode AND LOST.PerolehanData = HstITS.PerolehanData AND LOST.SalesCoordinator = HstITS.SalesCoordinator						   					   		   			   		
		WHERE (CASE WHEN @Area = '' THEN '' ELSE b.Area END) = @Area
		AND (CASE WHEN @DealerCode = '' THEN '' ELSE a.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE a.BranchCode END) = @OutletCode) #Temp3						
								
SELECT 
		CASE WHEN @TypeReport = '0' THEN 'Summary' ELSE 'Saldo' END TypeReport,
		CASE @ProductivityBy 
			WHEN '0' THEN 'Salesman'
			WHEN '1' THEN 'Vehicle Type'
			WHEN '2' THEN 'Source Data'
		END ProductivityBy,
		CONVERT(VARCHAR, @EndDate, 105) PerDate, CASE WHEN @Area = '' THEN 'ALL' ELSE @Area END Area, 
		CONVERT(VARCHAR, @StartDate, 105) + ' s/d ' + CONVERT(VARCHAR, @EndDate, 105) PeriodeDO,
		CASE WHEN @SalesHead = '' THEN 'ALL' ELSE @SalesHead END SalesHead,
		CASE WHEN @SalesCoordinator = '' THEN 'ALL' ELSE @SalesCoordinator END SalesCoordinator,
		CASE WHEN @Salesman = '' THEN 'ALL' ELSE @Salesman END Salesman,
		CASE WHEN @DealerCode = '' THEN 'ALL' ELSE (SELECT DealerName FROM SuzukiR4..gnMstDealerMapping WHERE DealerCode = @DealerCode) END Dealer,
		CASE WHEN @OutletCode = '' THEN 'ALL' ELSE (SELECT OutletName FROM SuzukiR4..gnMstDealerOutletMapping WHERE DealerCode = @DealerCode AND OutletCode = @OutletCode) END Outlet,
		PerolehanData Source,
		
		ISNULL(SUM(P), 0) NEW,
		ISNULL(SUM(P), 0) P,
		ISNULL(SUM(HP), 0) HP,
		ISNULL(SUM(SPK), 0) SPK,
		ISNULL(SUM(SumOuts), 0)  SumOuts,
		ISNULL(SUM(DO), 0) DO,
		ISNULL(SUM(DELIVERY), 0) DELIVERY,
		ISNULL(SUM(LOST), 0) LOST
		
		--CASE CAST(ISNULL(SUM(NEW), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(NEW) AS VARCHAR) END NEW,
		--CASE CAST(ISNULL(SUM(P), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(P) AS VARCHAR) END P,
		--CASE CAST(ISNULL(SUM(HP), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(HP) AS VARCHAR) END HP,
		--CASE CAST(ISNULL(SUM(SPK), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(SPK) AS VARCHAR) END SPK,
		--CASE CAST(ISNULL(SUM(SumOuts), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(SumOuts) AS VARCHAR) END SumOuts,
		--CASE CAST(ISNULL(SUM(DO), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(DO) AS VARCHAR) END DO,
		--CASE CAST(ISNULL(SUM(DELIVERY), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(DELIVERY) AS VARCHAR) END DELIVERY,
		--CASE CAST(ISNULL(SUM(LOST), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(LOST) AS VARCHAR) END LOST
	FROM #Temp3 a
	GROUP BY PerolehanData
						  		
	DROP TABLE #employee_src_2_n, #employee_src_3_n, #employee_src, #tNEW3, #tP31, #tP32, #tHP31, #tHP32, #tSPK31, #tSPK32, #tDO3, #tDELIVERY3, #tLOST3, #Temp3
	END
	ELSE
	BEGIN
		SELECT * INTO #employee_src_2 FROM
		(
			SELECT EmployeeID FROM SuzukiR4..PmMstTeamMembers WHERE 
				(CASE WHEN @DealerCode = '' THEN '' ELSE CompanyCode END) = @DealerCode 
				AND (CASE WHEN @OutletCode = '' THEN '' ELSE BranchCode END) = @OutletCode
				AND TeamID IN (SELECT TeamID FROM SuzukiR4..PmMstTeamMembers WHERE 
								(CASE WHEN @DealerCode = '' THEN '' ELSE CompanyCode END) = @DealerCode 
								AND (CASE WHEN @OutletCode = '' THEN '' ELSE BranchCode END) = @OutletCode
								AND EmployeeID = @BranchHead) 
				AND EmployeeID <> @BranchHead		
		) #employee_src_2
		
		SELECT * INTO #employee_src_3 FROM
		(
			SELECT EmployeeID FROM SuzukiR4..PmMstTeamMembers WHERE 
				(CASE WHEN @DealerCode = '' THEN '' ELSE CompanyCode END) = @DealerCode 
				AND (CASE WHEN @OutletCode = '' THEN '' ELSE BranchCode END) = @OutletCode
				AND TeamID IN (SELECT TeamID FROM SuzukiR4..PmMstTeamMembers WHERE 
								(CASE WHEN @DealerCode = '' THEN '' ELSE CompanyCode END) = @DealerCode 
								AND (CASE WHEN @OutletCode = '' THEN '' ELSE BranchCode END) = @OutletCode
								AND EmployeeID IN (SELECT EmployeeID FROM #employee_src_2) 
								AND IsSupervisor = 1) 
				AND EmployeeID NOT IN (SELECT EmployeeID FROM #employee_src_2)
		 )#employee_src_3
			
			SELECT 
		CASE WHEN @TypeReport = '0' THEN 'Summary' ELSE 'Saldo' END TypeReport,
		CASE @ProductivityBy 
			WHEN '0' THEN 'Salesman'
			WHEN '1' THEN 'Vehicle Type'
			WHEN '2' THEN 'Source Data'
		END ProductivityBy,
		CONVERT(VARCHAR, @EndDate, 105) PerDate, @Area Area, CONVERT(VARCHAR, @StartDate, 105) + ' s/d ' + CONVERT(VARCHAR, @EndDate, 105) PeriodeDO,
		CASE WHEN @SalesHead = '' THEN 'ALL' ELSE (SELECT EmployeeName FROM SuzukiR4..GnMstEmployee WHERE CompanyCode = a.CompanyCode AND BranchCode = @OutletCode AND EmployeeID = @SalesHead) END SalesHead,
		CASE WHEN @SalesCoordinator = '' THEN 'ALL' ELSE (SELECT EmployeeName FROM SuzukiR4..GnMstEmployee WHERE CompanyCode = a.CompanyCode AND BranchCode = @OutletCode AND EmployeeID = @SalesCoordinator) END SalesCoordinator,
		CASE WHEN @Salesman = '' THEN 'ALL' ELSE (SELECT EmployeeName FROM SuzukiR4..GnMstEmployee WHERE CompanyCode = a.CompanyCode AND BranchCode = @OutletCode AND EmployeeID = @Salesman) END Salesman,
		CASE WHEN @DealerCode = '' THEN 'ALL' ELSE (SELECT DealerName FROM SuzukiR4..gnMstDealerMapping WHERE DealerCode = @DealerCode) END Dealer,
		CASE WHEN @OutletCode = '' THEN 'ALL' ELSE (SELECT BranchName FROM SuzukiR4..gnMstOrganizationDtl WHERE CompanyCode = @DealerCode AND BranchCode = @OutletCode) END Outlet,
		a.LookupValue Source,
		CASE CAST(ISNULL(SUM(P), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(P) AS VARCHAR) END NEW,
		CASE CAST(ISNULL(SUM(P), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(P) AS VARCHAR) END P,
		CASE CAST(ISNULL(SUM(HP), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(HP) AS VARCHAR) END HP,
		CASE CAST(ISNULL(SUM(SPK), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(SPK) AS VARCHAR) END SPK,
		CASE CAST(ISNULL(SUM(SumOuts), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(SumOuts) AS VARCHAR) END SumOuts,
		CASE CAST(ISNULL(SUM(DO), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(DO) AS VARCHAR) END DO,
		CASE CAST(ISNULL(SUM(DELIVERY), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(DELIVERY) AS VARCHAR) END DELIVERY,
		CASE CAST(ISNULL(SUM(LOST), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(LOST) AS VARCHAR) END LOST
	FROM #employee_src a
	LEFT JOIN 
	(
		SELECT 
			a.CompanyCode,
			a.BranchCode,
			a.PerolehanData,

			(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
			WHERE CompanyCode = @DealerCode AND
				BranchCode = @OutletCode AND 
				PerolehanData = a.PerolehanData AND 
				SpvEmployeeID = a.SpvEmployeeID AND
				CONVERT(VARCHAR, InquiryDate, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)) NEW,

			(CASE WHEN @TypeReport = '0' THEN	
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = @DealerCode AND
					BranchCode = @OutletCode AND 
					PerolehanData = a.PerolehanData AND 
					SpvEmployeeID = a.SpvEmployeeID AND
					LastProgress = 'P' AND 
					CONVERT(VARCHAR, LastUpdateStatus, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112))
			ELSE													
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = @DealerCode AND
					BranchCode = @OutletCode AND 
					PerolehanData = a.PerolehanData AND 
					SpvEmployeeID = a.SpvEmployeeID AND
					LastProgress = 'P' AND 
					CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) END) P,
				
			(CASE WHEN @TypeReport = '0' THEN	
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = @DealerCode AND
					BranchCode = @OutletCode AND 
					PerolehanData = a.PerolehanData AND 
					SpvEmployeeID = a.SpvEmployeeID AND
					LastProgress = 'HP' AND 
					CONVERT(VARCHAR, LastUpdateStatus, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112))
			ELSE
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = @DealerCode AND
					BranchCode = @OutletCode AND 
					PerolehanData = a.PerolehanData AND 
					SpvEmployeeID = a.SpvEmployeeID AND
					LastProgress = 'HP' AND 
					CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) END) HP,
			
			(CASE WHEN @TypeReport = '0' THEN			
				(SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
					INNER JOIN SuzukiR4..pmKdp KDP ON 
					KDP.CompanyCode = Hist.CompanyCode AND
					KDP.BranchCode = Hist.BranchCode AND
					KDP.InquiryNumber = Hist.InquiryNumber 
				 WHERE Hist.CompanyCode = @DealerCode AND
					Hist.BranchCode = @OutletCode AND 
					KDP.PerolehanData = a.PerolehanData AND 
					KDP.SpvEmployeeID = a.SpvEmployeeID AND		
					CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
					Hist.LastProgress = 'SPK')
			ELSE
			(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = @DealerCode AND
				BranchCode = @OutletCode AND 
				PerolehanData = a.PerolehanData AND 
				SpvEmployeeID = a.SpvEmployeeID AND		
				LastProgress = 'SPK' AND 
				CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) 
				END) SPK,
				
			(CASE WHEN @TypeReport = '0' THEN	
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = @DealerCode AND
					BranchCode = @OutletCode AND 
					PerolehanData = a.PerolehanData AND 
					SpvEmployeeID = a.SpvEmployeeID AND
					LastProgress = 'P' AND 
					CONVERT(VARCHAR, LastUpdateStatus, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)) +
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = @DealerCode AND
					BranchCode = @OutletCode AND 
					PerolehanData = a.PerolehanData AND 
					SpvEmployeeID = a.SpvEmployeeID AND
					LastProgress = 'HP' AND 
					CONVERT(VARCHAR, LastUpdateStatus, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)) +
				(SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
					INNER JOIN SuzukiR4..pmKdp KDP ON 
					KDP.CompanyCode = Hist.CompanyCode AND
					KDP.BranchCode = Hist.BranchCode AND
					KDP.InquiryNumber = Hist.InquiryNumber 
				 WHERE Hist.CompanyCode = @DealerCode AND
					Hist.BranchCode = @OutletCode AND 
					KDP.PerolehanData = a.PerolehanData AND 
					KDP.SpvEmployeeID = a.SpvEmployeeID AND		
					CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
					Hist.LastProgress = 'SPK')					
			ELSE													
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = @DealerCode AND
					BranchCode = @OutletCode AND 
					PerolehanData = a.PerolehanData AND 
					SpvEmployeeID = a.SpvEmployeeID AND
					LastProgress = 'P' AND 
					CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) + 
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = @DealerCode AND
					BranchCode = @OutletCode AND 
					PerolehanData = a.PerolehanData AND 
					SpvEmployeeID = a.SpvEmployeeID AND
					LastProgress = 'HP' AND 
					CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) +				
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
					WHERE CompanyCode = @DealerCode AND
					BranchCode = @OutletCode AND 
					PerolehanData = a.PerolehanData AND 
					SpvEmployeeID = a.SpvEmployeeID AND		
					LastProgress = 'SPK' AND 
					CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) END) SumOuts,
				
			(SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
			INNER JOIN SuzukiR4..pmKdp KDP ON 
				KDP.CompanyCode = Hist.CompanyCode AND
				KDP.BranchCode = Hist.BranchCode AND
				KDP.InquiryNumber = Hist.InquiryNumber 
			 WHERE Hist.CompanyCode = @DealerCode AND
				Hist.BranchCode = @OutletCode AND 
				KDP.PerolehanData = a.PerolehanData AND 
				KDP.SpvEmployeeID = a.SpvEmployeeID AND				
				CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
				Hist.LastProgress = 'DO') DO,
				
			(SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
				INNER JOIN SuzukiR4..pmKdp KDP ON 
				KDP.CompanyCode = Hist.CompanyCode AND
				KDP.BranchCode = Hist.BranchCode AND
				KDP.InquiryNumber = Hist.InquiryNumber 
			 WHERE Hist.CompanyCode = @DealerCode AND
				Hist.BranchCode = @OutletCode AND 
				KDP.PerolehanData = a.PerolehanData AND 
				KDP.SpvEmployeeID = a.SpvEmployeeID AND			
				CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
				Hist.LastProgress = 'DELIVERY') DELIVERY,
				
			(SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
				INNER JOIN SuzukiR4..pmKdp KDP ON 
				KDP.CompanyCode = Hist.CompanyCode AND
				KDP.BranchCode = Hist.BranchCode AND
				KDP.InquiryNumber = Hist.InquiryNumber 
			 WHERE Hist.CompanyCode = @DealerCode AND
				Hist.BranchCode = @OutletCode AND 
				KDP.PerolehanData = a.PerolehanData AND 
				KDP.SpvEmployeeID = a.SpvEmployeeID AND			
				CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
				Hist.LastProgress = 'LOST') LOST					
		FROM SuzukiR4..pmKdp a
		WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE a.CompanyCode END) = @DealerCode
			AND (CASE WHEN @OutletCode = '' THEN '' ELSE a.BranchCode END) = @OutletCode
			AND a.SpvEmployeeID IN (SELECT EmployeeID FROM #employee_src_3)
		GROUP BY a.PerolehanData, a.CompanyCode,
			a.BranchCode, a.SpvEmployeeID
	) b ON a.CompanyCode = b.CompanyCode AND a.LookupValue = b.PerolehanData
	GROUP BY a.CompanyCode, a.LookupValue

	DROP TABLE #employee_src, #employee_src_2, #employee_src_3
	END		
END					 	 	     
--#endregion

END
