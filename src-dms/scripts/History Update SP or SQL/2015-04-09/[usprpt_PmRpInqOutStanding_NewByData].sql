IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usprpt_PmRpInqOutStanding_NewByData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usprpt_PmRpInqOutStanding_NewByData]
GO

CREATE procedure [dbo].[usprpt_PmRpInqOutStanding_NewByData] 
(
	@CompanyCode		VARCHAR(15),
	@BranchCode			VARCHAR(15),
	@Period				DATETIME,
	@COO				VARCHAR(15),
	@BranchManager		VARCHAR(15),
	@SalesHead			VARCHAR(15),
	@SalesCoordinator	VARCHAR(15),
	@Salesman			VARCHAR(15)
	
)
AS 
BEGIN
SET NOCOUNT ON;
SELECT * INTO #dByTipe FROM(
			SELECT b.EmployeeID, b.PerolehanData, LastProgress, StatusProspek FROM PmKdp b 
			WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND b.LastUpdateDate <=  CONVERT(VARCHAR, @Period, 112)
		)#dByTipe

		SELECT * INTO #dSls FROM (
			SELECT 
				a.EmployeeID,
				c.Position,
				c.TeamLeader,
				a.PerolehanData Source,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'P' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'HP' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'SPK' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) SPK
			FROM #dByTipe a, HrEmployee c WHERE a.EmployeeID = c.EmployeeID
			GROUP BY a.EmployeeID, c.Position, c.TeamLeader, a.PerolehanData
		)#dSls

		IF @COO = ''
		BEGIN
		IF @SalesHead = '' AND @SalesCoordinator = '' AND @Salesman = ''
		BEGIN
			SELECT
					a.EmployeeID,
					a.Position,
					a.PerolehanData Source,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN
				(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @BranchManager))   
				AND a.PerolehanData <> ''
				GROUP BY a.EmployeeID, a.Position, a.PerolehanData
		END
		ELSE IF @SalesHead <> '' AND @SalesCoordinator = '' AND @Salesman = ''
		BEGIN
			SELECT
					a.EmployeeID,
					a.Position,
					a.PerolehanData Source,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.TeamLeader = @SalesHead
				AND a.PerolehanData <> ''
				GROUP BY a.EmployeeID, a.Position, a.PerolehanData
		END
		ELSE IF @SalesHead = '' AND @SalesCoordinator <> '' AND @Salesman = ''
		BEGIN
			SELECT
					a.EmployeeID,
					a.Position,
					a.PerolehanData Source,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.TeamLeader = @SalesCoordinator
				AND a.PerolehanData <> ''
				GROUP BY a.EmployeeID, a.Position, a.PerolehanData	
		END
		ELSE IF (@SalesHead <> '' OR @SalesCoordinator <> '') AND @Salesman <> ''
			SELECT
					a.EmployeeID,
					a.Position,
					a.PerolehanData,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.EmployeeID = @Salesman  
				AND a.PerolehanData <> ''
				GROUP BY a.EmployeeID, a.Position, a.PerolehanData

		DROP TABLE #dSls
		DROP TABLE #dByTipe
		END
END

