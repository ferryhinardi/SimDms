IF(OBJECT_ID('uspfn_GetMonitoringCustBirthday') is not null)
	drop proc dbo.uspfn_GetMonitoringCustBirthday
GO

/****** Object:  StoredProcedure [dbo].[uspfn_GetMonitoringCustBirthday]    Script Date: 9/7/2015 11:21:04 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
	uspfn_GetMonitoringCustBirthday '6006400134', 2014, 1, 2, 0, 0
	[from uspfn_CsInqCustBirthday]
*/
ALTER PROCEDURE uspfn_GetMonitoringCustBirthday
	@BranchCode VARCHAR(20),
	@PeriodYear INT,
	@ParMonth1 INT,
	@ParMonth2 INT,
	@ParStatus INT,
	@type INT
AS
BEGIN

	DECLARE @tempTable TABLE (
		CompanyCode	VARCHAR(15),
		BranchCode VARCHAR(15),
		CustomerCode VARCHAR(15),
		BirthDate DATETIME,
		IsReminder VARCHAR(3),
		SentGiftDate DATETIME,
		SpouseTelephone VARCHAR(50),
		InputDate DATETIME
	)

	INSERT INTO @tempTable
	SELECT 
		a.CompanyCode
		, a.BranchCode
		, a.CustomerCode
		, a.BirthDate
		, (CASE WHEN c.CustomerCode IS NULL THEN 'N' ELSE 'Y' END) IsReminder
		, c.SentGiftDate
		, c.SpouseTelephone
		, c.CreatedDate as InputDate
	FROM CsCustomerView a
	LEFT JOIN CsCustData b
		ON b.CompanyCode = a.CompanyCode
		AND b.CustomerCode = a.CustomerCode
	LEFT JOIN CsCustBirthDay c
		ON c.CompanyCode = a.CompanyCode
		AND c.CustomerCode = a.CustomerCode
		AND c.PeriodYear = @PeriodYear
	WHERE a.BranchCode = (CASE ISNULL(@BranchCode, '') WHEN '' THEN a.BranchCode ELSE @BranchCode END)
	AND a.CustomerType = 'I'
	AND a.BirthDate IS NOT NULL
	AND a.BirthDate > '1900-01-01'
	AND (YEAR(GETDATE() - YEAR(a.BirthDate))) > 5
	AND MONTH(a.BirthDate) BETWEEN @ParMonth1 AND @ParMonth2
	AND ISNULL(c.CustomerCode, '1900-01-01') = (CASE @ParStatus
		WHEN 0 THEN ISNULL(c.CustomerCode, '1900-01-01')
		WHEN 1 THEN '1900-01-01'
		ELSE c.CustomerCode
		END)
	ORDER BY DAY(a.BirthDate)

	IF (@type = 0)
	BEGIN
		SELECT 
			a.Month,
			a.Periode, 
			ISNULL(TotalCustomer, 0) TotalCustomer, 
			ISNULL(Reminder, 0) Reminder, 
			ISNULL(Gift, 0) Gift, 
			ISNULL(Telephone, 0) Telephone
		FROM 
		(
			SELECT 
				DATEPART(MONTH, BirthDate) AS [Month],
				DATEPART(WEEK, BirthDate) AS [Periode], 
				COUNT(CompanyCode) AS TotalCustomer 
			FROM @tempTable a
			GROUP BY DATEPART(MONTH, BirthDate), DATEPART(WEEK, BirthDate)
		) a	
		LEFT JOIN (
			SELECT 
				DATEPART(MONTH, BirthDate) AS [Month],
				DATEPART(WEEK, BirthDate) AS [Periode], 
				COUNT(CompanyCode) AS [Reminder] 
			FROM @tempTable
			WHERE IsReminder = 'N'
			GROUP BY DATEPART(MONTH, BirthDate), DATEPART(WEEK, BirthDate)
		) b ON a.Month = b.Month AND a.Periode = b.Periode
		LEFT JOIN (
			SELECT 
				DATEPART(MONTH, BirthDate) AS [Month],
				DATEPART(WEEK, BirthDate) AS [Periode], 
				COUNT(CompanyCode) AS [Gift] 
			FROM @tempTable
			WHERE SentGiftDate IS NOT NULL
			GROUP BY DATEPART(MONTH, BirthDate), DATEPART(WEEK, BirthDate)
		) c ON a.Month = c.Month AND a.Periode = c.Periode
		LEFT JOIN (
			SELECT 
				DATEPART(MONTH, BirthDate) AS [Month],
				DATEPART(WEEK, BirthDate) AS [Periode], 
				COUNT(CompanyCode) AS [Telephone] 
			FROM @tempTable
			WHERE SpouseTelephone IS NOT NULL
			GROUP BY DATEPART(MONTH, BirthDate), DATEPART(WEEK, BirthDate)
		) d ON a.Month = d.Month AND a.Periode = d.Periode
		ORDER BY a.Month, b.Periode
	END
	---------------------------------------------------------------------
	ELSE
	BEGIN
		SELECT 
			a.Periode, 
			ISNULL(TotalCustomer, 0) TotalCustomer, 
			ISNULL(Reminder, 0) Reminder, 
			ISNULL(Gift, 0) Gift, 
			ISNULL(Telephone, 0) Telephone
		FROM 
		(
			SELECT 
				DATEPART(WEEK, BirthDate) AS [Periode], 
				COUNT(CompanyCode) AS TotalCustomer 
			FROM @tempTable a
			GROUP BY DATEPART(WEEK, BirthDate)
		) a	
		LEFT JOIN (
			SELECT 
				DATEPART(WEEK, BirthDate) AS [Periode], 
				COUNT(CompanyCode) AS [Reminder] 
			FROM @tempTable
			WHERE IsReminder = 'N'
			GROUP BY DATEPART(WEEK, BirthDate)
		) b ON a.Periode = b.Periode
		LEFT JOIN (
			SELECT 
				DATEPART(WEEK, BirthDate) AS [Periode], 
				COUNT(CompanyCode) AS [Gift] 
			FROM @tempTable
			WHERE SentGiftDate IS NOT NULL
			GROUP BY DATEPART(WEEK, BirthDate)
		) c ON a.Periode = c.Periode
		LEFT JOIN (
			SELECT 
				DATEPART(WEEK, BirthDate) AS [Periode], 
				COUNT(CompanyCode) AS [Telephone] 
			FROM @tempTable
			WHERE SpouseTelephone IS NOT NULL
			GROUP BY DATEPART(WEEK, BirthDate)
		) d ON a.Periode = d.Periode
		ORDER BY a.Periode
	END
END