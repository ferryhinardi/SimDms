
-- =============================================
-- Author:		fhy
-- Create date: 29122015
-- Description:	CsReportCustBirthdayexport
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_GetMonitoringCustBirthdayexport] 
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

DECLARE @t_CsReportCustBirthday TABLE (
		[Dealer Kode   ]	VARCHAR(15),
		[Bulan          ] VARCHAR(100),
		[Nama Outlet                     ] VARCHAR(150),
		[Jumlah Customer]  int,
		[Input by CRO]  int,
		[Gift]  int,
		[SMS]  int,
		[Telephone]  int,
		[Letter]  int,
		[Souvenir]  int
	)

DECLARE @t_CsReportCustBirthdayrpt TABLE (		
		No varchar(5),
		[Nama Outlet                     ] VARCHAR(150),
		[Bulan          ] VARCHAR(100),
		[Jumlah Customer]  Numeric(18,2),
		[Input by CRO]  Numeric(18,2),
		[Gift]  Numeric(18,2),
		[SMS]  Numeric(18,2),
		[Telephone]  Numeric(18,2),
		[Letter]  Numeric(18,2),
		[Souvenir]  Numeric(18,2)
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

insert into @t_CsReportCustBirthday
	SELECT 
		a.CompanyCode,
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
			CompanyCode,DATEPART(MONTH, BirthDate) AS [Month],
			OutletName,
			COUNT(CompanyCode) AS TotalCustomer 
		FROM @tempTable a
		GROUP BY DATEPART(MONTH, BirthDate), OutletName,CompanyCode
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
	ORDER BY a.OutletName, a.Month,a.CompanyCode

select * from @t_CsReportCustBirthday

insert into @t_CsReportCustBirthdayrpt
select 
	ROW_NUMBER() over (order by [Dealer Kode   ])
	, [Nama Outlet                     ] 
	, case when [Bulan          ]	=1 then 'Januari' 
		when [Bulan          ]	=2 then 'Februari' 
		when [Bulan          ]	=3 then 'Maret' 
		when [Bulan          ]	=4 then 'April' 
		when [Bulan          ]	=5 then 'Mei' 
		when [Bulan          ]	=6 then 'Juni' 
		when [Bulan          ]	=7 then 'Juli' 
		when [Bulan          ]	=8 then 'Agustus' 
		when [Bulan          ]	=9 then 'September' 
		when [Bulan          ]	=10 then 'Oktober' 
		when [Bulan          ]	=11 then 'Nopember' 
		else  'Desember'  end 
	, [Jumlah Customer]
	, [Input by CRO]
	, [Gift]
	, [SMS]
	, [Telephone]
	, [Letter]
	, [Souvenir]
from @t_CsReportCustBirthday

select * from @t_CsReportCustBirthdayrpt
Union all
select 
'Total'
, ''
, ''
, ISNULL(sum([Jumlah Customer]),0)
, ISNULL(sum([Input by CRO]),0)
, ISNULL(sum([Gift]),0)
, ISNULL(sum([SMS]),0)
, ISNULL(sum([Telephone]),0)
, ISNULL(sum([Letter]),0)
, ISNULL(sum([Souvenir]),0)
from @t_CsReportCustBirthdayrpt

Union all
select 
'Persentase  (%)'
, ''
, ''
, isnull(case when (sum([Jumlah Customer]))=0 then 0
		else (sum([Jumlah Customer]) *100) /sum([Jumlah Customer]) end,0)
, isnull(case when (sum([Jumlah Customer]))=0 then 0
		else (sum([Input by CRO]) *100) /sum([Jumlah Customer]) end,0)
, isnull(case when (sum([Jumlah Customer]))=0 then 0
		else (sum([Gift]) *100) /sum([Jumlah Customer]) end,0)
, isnull(case when (sum([Jumlah Customer]))=0 then 0
		else (sum([SMS]) *100) /sum([Jumlah Customer]) end,0)
, isnull(case when (sum([Jumlah Customer]))=0 then 0
		else (sum([Telephone]) *100) /sum([Jumlah Customer]) end,0)
, isnull(case when (sum([Jumlah Customer]))=0 then 0
		else (sum([Letter]) *100) /sum([Jumlah Customer]) end,0)
, isnull(case when (sum([Jumlah Customer]))=0 then 0
		else (sum([Souvenir]) *100) /sum([Jumlah Customer]) end,0)
from @t_CsReportCustBirthdayrpt

delete @tempTable
delete @t_CsReportCustBirthday
delete @t_CsReportCustBirthdayrpt
END

GO


