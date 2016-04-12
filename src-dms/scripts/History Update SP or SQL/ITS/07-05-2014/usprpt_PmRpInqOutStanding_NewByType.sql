USE [PMU]
GO
/****** Object:  StoredProcedure [dbo].[usprpt_PmRpInqOutStanding_NewByType]    Script Date: 5/6/2015 2:55:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[usprpt_PmRpInqOutStanding_NewByType] 
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
			SELECT b.EmployeeID, (b.TipeKendaraan + ' ' + b.Variant) ModelKendaraan, LastProgress, StatusProspek FROM PmKdp b 
			WHERE b.CompanyCode = @CompanyCode 
			--AND b.BranchCode = @BranchCode
			AND b.LastUpdateDate <=  CONVERT(VARCHAR, @Period, 112)
		)#dByTipe

		SELECT * INTO #dSls FROM (
			SELECT 
				a.EmployeeID,
				c.Position,
				c.TeamLeader,
				a.ModelKendaraan,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'P' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'HP' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'SPK' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) SPK
			FROM #dByTipe a, HrEmployee c WHERE a.EmployeeID = c.EmployeeID AND c.PersonnelStatus = '1' AND c.Position = 'S'
			GROUP BY a.EmployeeID, c.Position, c.TeamLeader, a.ModelKendaraan
		)#dSls

		IF @COO = '' AND @BranchManager != ''
		BEGIN
			IF @SalesHead = '' AND @SalesCoordinator = '' AND @Salesman = ''
			BEGIN
				SELECT
						--a.EmployeeID,
						--a.Position,
						a.ModelKendaraan TipeKendaraan,
						SUM(a.PROSPECT) PROSPECT,
						SUM(a.HOTPROSPECT) HOTPROSPECT,
						SUM(a.SPK) SPK
					FROM #dSls a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @BranchManager) 
					AND a.ModelKendaraan <> ''
					GROUP BY a.ModelKendaraan
					--GROUP BY a.EmployeeID, a.Position, a.ModelKendaraan
			END
			ELSE IF @SalesHead <> '' AND @SalesCoordinator = '' AND @Salesman = ''
			BEGIN
				SELECT
						--a.EmployeeID,
						--a.Position,
						a.ModelKendaraan TipeKendaraan,
						SUM(a.PROSPECT) PROSPECT,
						SUM(a.HOTPROSPECT) HOTPROSPECT,
						SUM(a.SPK) SPK
					FROM #dSls a WHERE a.TeamLeader = @SalesHead
						AND a.ModelKendaraan <> ''
					GROUP BY a.ModelKendaraan	
					--GROUP BY a.EmployeeID, a.Position, a.ModelKendaraan
			END
			ELSE IF @SalesHead = '' AND @SalesCoordinator <> '' AND @Salesman = ''
			BEGIN
				SELECT
						--a.EmployeeID,
						--a.Position,
						a.ModelKendaraan TipeKendaraan,
						SUM(a.PROSPECT) PROSPECT,
						SUM(a.HOTPROSPECT) HOTPROSPECT,
						SUM(a.SPK) SPK
					FROM #dSls a WHERE a.TeamLeader = @SalesCoordinator
						AND a.ModelKendaraan <> ''
					GROUP BY a.ModelKendaraan
					--GROUP BY a.EmployeeID, a.Position, a.ModelKendaraan	
			END
			ELSE IF (@SalesHead <> '' OR @SalesCoordinator <> '') AND @Salesman <> ''
			BEGIN
				SELECT
						--a.EmployeeID,
						--a.Position,
						a.ModelKendaraan TipeKendaraan,
						SUM(a.PROSPECT) PROSPECT,
						SUM(a.HOTPROSPECT) HOTPROSPECT,
						SUM(a.SPK) SPK
					FROM #dSls a WHERE a.EmployeeID = @Salesman
						AND a.ModelKendaraan <> ''   
					GROUP BY a.ModelKendaraan
					--GROUP BY a.EmployeeID, a.Position, a.ModelKendaraan
			END
		END
		ELSE
		BEGIN
			SELECT
						--a.EmployeeID,
						--a.Position,
						a.ModelKendaraan TipeKendaraan,
						SUM(a.PROSPECT) PROSPECT,
						SUM(a.HOTPROSPECT) HOTPROSPECT,
						SUM(a.SPK) SPK
					FROM #dSls a WHERE a.ModelKendaraan <> ''
					GROUP BY a.ModelKendaraan
					--GROUP BY a.EmployeeID, a.Position, a.ModelKendaraan
			END
		DROP TABLE #dSls
		DROP TABLE #dByTipe
END

