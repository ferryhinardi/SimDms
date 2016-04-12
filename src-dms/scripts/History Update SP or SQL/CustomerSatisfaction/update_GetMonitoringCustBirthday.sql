

ALTER PROCEDURE [dbo].[uspfn_GetMonitoringCustBirthday]
	@BranchCode VARCHAR(20),
	@PeriodYear INT,
	@ParMonth1 INT,
	@ParMonth2 INT,
	@ParStatus INT
AS
BEGIN

	DECLARE @tempTable TABLE (
		CompanyCode	VARCHAR(15),
		BranchCode VARCHAR(15),
		OutletName VARCHAR(100),
		CustomerCode VARCHAR(15),
		BirthDate DATETIME,
		IsReminder VARCHAR(3),
		SentGiftDate DATETIME,
		SpouseTelephone VARCHAR(50),
		InputDate DATETIME,
		TypeOfGift VARCHAR(50)
	)

	INSERT INTO @tempTable
	SELECT 
		a.CompanyCode
		, a.BranchCode
		, d.OutletAbbreviation
		, a.CustomerCode
		, a.BirthDate
		, (CASE WHEN c.CustomerCode IS NULL THEN 'N' ELSE 'Y' END) IsReminder
		, c.SentGiftDate
		, c.SpouseTelephone
		, c.CreatedDate as InputDate
		, c.TypeOfGift
	FROM CsCustomerView a
	LEFT JOIN CsCustData b
		ON b.CompanyCode = a.CompanyCode
		AND b.CustomerCode = a.CustomerCode
	LEFT JOIN CsCustBirthDay c
		ON c.CompanyCode = a.CompanyCode
		AND c.CustomerCode = a.CustomerCode
		AND c.PeriodYear = @PeriodYear
	LEFT JOIN gnMstDealerOutletMapping d
		ON a.CompanyCode = d.DealerCode
		AND a.BranchCode = d.OutletCode
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

	SELECT 
		a.Month,
		a.OutletName,
		ISNULL(TotalCustomer, 0) TotalCustomer, 
		ISNULL(Reminder, 0) Reminder,
		ISNULL(Gift, 0) Gift,
		ISNULL(SMS, 0) SMS,
		ISNULL(Telephone, 0) Telephone,
		ISNULL(Letter, 0) Letter,
		ISNULL(Souvenir, 0) Souvenir
	FROM 
	(
		SELECT 
			DATEPART(MONTH, BirthDate) AS [Month],
			OutletName,
			COUNT(CompanyCode) AS TotalCustomer 
		FROM @tempTable a
		GROUP BY DATEPART(MONTH, BirthDate), OutletName
	) a	
	LEFT JOIN (
		SELECT 
			DATEPART(MONTH, BirthDate) AS [Month],
			OutletName,
			COUNT(CompanyCode) AS [Reminder] 
		FROM @tempTable
		WHERE IsReminder = 'N'
		GROUP BY DATEPART(MONTH, BirthDate), OutletName
	) b ON a.Month = b.Month AND a.OutletName = b.OutletName
	LEFT JOIN (
		SELECT 
			DATEPART(MONTH, BirthDate) AS [Month],
			OutletName,
			COUNT(CompanyCode) AS [Gift] 
		FROM @tempTable
		WHERE SentGiftDate IS NOT NULL
		GROUP BY DATEPART(MONTH, BirthDate), OutletName
	) c ON a.Month = c.Month AND a.OutletName = c.OutletName
	LEFT JOIN (
		SELECT 
			DATEPART(MONTH, BirthDate) AS [Month],
			OutletName,
			COUNT(CompanyCode) AS [SMS] 
		FROM @tempTable
		WHERE TypeOfGift IS NOT NULL and (SELECT Name FROM dbo.splitstring(TypeOfGift) where id = 5) = 'true' 
		GROUP BY DATEPART(MONTH, BirthDate), OutletName
	) d ON a.Month = d.Month AND a.OutletName = d.OutletName
	LEFT JOIN (
		SELECT 
			DATEPART(MONTH, BirthDate) AS [Month],
			OutletName,
			COUNT(CompanyCode) AS [Telephone] 
		FROM @tempTable
		WHERE TypeOfGift IS NOT NULL and (SELECT Name FROM dbo.splitstring(TypeOfGift) where id = 3) = 'true' 
		--WHERE SpouseTelephone IS NOT NULL
		GROUP BY DATEPART(MONTH, BirthDate), OutletName
	) e ON a.Month = e.Month AND a.OutletName = e.OutletName
	LEFT JOIN (
		SELECT 
			DATEPART(MONTH, BirthDate) AS [Month],
			OutletName,
			COUNT(CompanyCode) AS [Letter] 
		FROM @tempTable
		WHERE TypeOfGift IS NOT NULL and (SELECT Name FROM dbo.splitstring(TypeOfGift) where id = 2) = 'true' 
		GROUP BY DATEPART(MONTH, BirthDate), OutletName
	) f ON a.Month = f.Month AND a.OutletName = f.OutletName
	LEFT JOIN (
		SELECT 
			DATEPART(MONTH, BirthDate) AS [Month],
			OutletName,
			COUNT(CompanyCode) AS [Souvenir]
		FROM @tempTable
		WHERE TypeOfGift IS NOT NULL and (SELECT Name FROM dbo.splitstring(TypeOfGift) where id = 4) = 'true' 
		--WHERE SpouseTelephone IS NOT NULL
		GROUP BY DATEPART(MONTH, BirthDate), OutletName
	) g ON a.Month = g.Month AND a.OutletName = g.OutletName
	ORDER BY a.OutletName, a.Month
END

GO


