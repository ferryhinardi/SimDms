ALTER PROCEDURE [dbo].[uspfn_GetTotalInquiryAndSpk2]
	@CompanyCode VARCHAR(15),
	@BranchCode VARCHAR(15)
AS
BEGIN
IF @BranchCode = '' SET @BranchCode = NULL;

DECLARE @currInq INT, @prevInq INT, @prev2Inq INT, @currPrevInq INT,
		@currSPK INT, @prevSPK INT, @prev2SPK INT, @currPrevSPK INT,
		@currStock INT, @prevStock INT, @prev2Stock INT, @currPrevStock INT,
		@currInvoice INT, @prevInvoice INT, @prev2Invoice INT, @currPrevInvoice INT,
		--@startWorkingDay DATE, @startPrevWorkingDay DATE,
		@currentWorkingDay DATE, @currentPrevWorkingDay DATE,
		@currentDate DATE,
		@currentPrevDate DATE,
		@currentPrev2Date DATE,
		@startOfMonth DATE,
		@endOfMonth DATE,
		@startOfPrevMonth DATE,
		@endOfPrevMonth DATE,
		@startOfPrev2Month DATE,
		@endOfPrev2Month DATE

DECLARE @Temp TABLE (
	startOfMonth DATE,
	currentDate DATE,
	startOfPrevMonth DATE,
	PrevMonthOfCurrDate DATE,
	endOfMonth DATE,
	endOfPrevMonth DATE,
	startOfPrev2Month DATE,
	endOfPrev2Month DATE,
	currInq INT, prevInq INT, prev2Inq INT, currPrevINQ INT,
	currSPK INT, prevSPK INT, prev2SPK INT, currPrevSPK INT,
	currStock INT, prevStock INT, prev2Stock INT, currPrevStock INT,
	currInvoice INT, prevInvoice INT, prev2Invoice INT, currPrevInvoice INT
)

DECLARE @TempWorkingDay TABLE (
	[CurrentMonth] DATE,
	[PrevMonth] DATE
)

SELECT @currentDate = GETDATE(),
		@currentPrevDate = DATEADD(MONTH, -1, GETDATE()),
		@startOfMonth = CONVERT(VARCHAR(8), GETDATE(), 120) + '01',
		@endOfMonth = DATEADD(DAY, -1, DATEADD(MONTH, 1, @startOfMonth)),
		@startOfPrevMonth = DATEADD(MONTH, -1, @startOfMonth),
		@endOfPrevMonth = DATEADD(DAY, -1, @startOfMonth),
		@startOfPrev2Month = DATEADD(MONTH, -1, @startOfPrevMonth),
		@endOfPrev2Month = DATEADD(DAY, -1, @startOfPrevMonth)
		
INSERT INTO @TempWorkingDay
SELECT a.Date, b.Date
FROM (SELECT ROW_NUMBER() OVER(ORDER BY Tanggal) AS No, CAST(Tanggal AS DATE) AS [Date] FROM GetWorkingDays(CONVERT(VARCHAR(10), GETDATE(), 120))) a JOIN (SELECT ROW_NUMBER() OVER(ORDER BY Tanggal) AS No, CAST(Tanggal AS DATE) AS [Date] FROM GetWorkingDays(CONVERT(VARCHAR(10), @endOfPrevMonth, 120))) b ON a.No = b.No

--SELECT TOP 1 @startWorkingDay = [CurrentMonth] FROM @TempWorkingDay 
SELECT TOP 1 @currentWorkingDay = MAX([CurrentMonth]) FROM @TempWorkingDay

--SELECT TOP 1 @startPrevWorkingDay = [PrevMonth] FROM @TempWorkingDay
SELECT TOP 1 @currentPrevWorkingDay = MAX([PrevMonth]) FROM @TempWorkingDay

	-- SELECT @startWorkingDay, @currentWorkingDay, @startPrevWorkingDay, @currentPrevWorkingDay, @currentDate, @currentPrevDate, @startOfMonth, @endOfMonth, @startOfPrevMonth, @endOfPrevMonth, @startOfPrev2Month, @endOfPrev2Month
	
	SELECT @currInq = COUNT(*) FROM pmkdp WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(InquiryDate AS DATE) BETWEEN @startOfMonth AND @currentWorkingDay 

	SELECT @currPrevInq = COUNT(*) FROM pmkdp WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(InquiryDate AS DATE) BETWEEN @startOfPrevMonth AND @currentPrevWorkingDay

	SELECT @prevInq = COUNT(*) FROM pmkdp WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(InquiryDate AS DATE) BETWEEN @startOfPrevMonth AND @endOfPrevMonth

	SELECT @prev2Inq = COUNT(*) FROM pmkdp WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(InquiryDate AS DATE) BETWEEN @startOfPrev2Month AND @endOfPrev2Month
	
	SELECT @currSPK = COUNT(*) FROM ( SELECT DISTINCT [InquiryNumber],[CompanyCode],[BranchCode] 
	FROM pmStatusHistory  
	WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(updateDate AS DATE) BETWEEN @startOfMonth AND @currentWorkingDay AND LastProgress = 'spk') a

	SELECT @currPrevSPK = COUNT(*) FROM ( SELECT DISTINCT [InquiryNumber],[CompanyCode],[BranchCode] 
	FROM pmStatusHistory  
	WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(updateDate AS DATE) BETWEEN @startOfPrevMonth AND @currentPrevWorkingDay AND LastProgress = 'spk') a

	SELECT @prevSPK = COUNT(*) FROM ( SELECT DISTINCT [InquiryNumber],[CompanyCode],[BranchCode] 
	FROM pmStatusHistory  
	WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(updateDate AS DATE) BETWEEN @startOfPrevMonth AND @endOfPrevMonth AND LastProgress = 'spk') a
		
	SELECT @prev2SPK = COUNT(*) FROM ( SELECT DISTINCT [InquiryNumber],[CompanyCode],[BranchCode] 
	FROM pmStatusHistory  
	WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(updateDate AS DATE) BETWEEN @startOfPrev2Month AND @endOfPrev2Month AND LastProgress = 'spk') a
	
	SELECT @currStock = SUM(EndingOH) FROM omTrInventQtyVehicle WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(CreatedDate AS DATE) BETWEEN @startOfMonth AND @currentWorkingDay

	SELECT @currPrevStock = SUM(EndingOH) FROM omTrInventQtyVehicle WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(CreatedDate AS DATE) BETWEEN @startOfPrevMonth AND @currentPrevWorkingDay

	SELECT @prevStock = SUM(EndingOH) FROM omTrInventQtyVehicle WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(CreatedDate AS DATE) BETWEEN @startOfPrevMonth AND @endOfPrevMonth
		
	SELECT @prev2Stock = SUM(EndingOH) FROM omTrInventQtyVehicle WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(CreatedDate AS DATE) BETWEEN @startOfPrev2Month AND @endOfPrev2Month
	
	SELECT @currInvoice = COUNT(*) 
	FROM [omTrSalesInvoiceVin] a JOIN [omTrSalesInvoice] b 
	ON a.CompanyCode = b.CompanyCode 
	AND a.BranchCode = b.BranchCode
	AND a.InvoiceNo = b.InvoiceNo
	WHERE a.CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR a.BranchCode = @BranchCode) AND CAST(InvoiceDate AS DATE) BETWEEN @startOfMonth AND @currentWorkingDay
	
	SELECT @currPrevInvoice = COUNT(*) 
	FROM [omTrSalesInvoiceVin] a JOIN [omTrSalesInvoice] b 
	ON a.CompanyCode = b.CompanyCode 
	AND a.BranchCode = b.BranchCode
	AND a.InvoiceNo = b.InvoiceNo
	WHERE a.CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR a.BranchCode = @BranchCode) AND CAST(InvoiceDate AS DATE) BETWEEN @startOfPrevMonth AND @currentPrevWorkingDay
	
	SELECT @prevInvoice = COUNT(*) 
	FROM [omTrSalesInvoiceVin] a JOIN [omTrSalesInvoice] b 
	ON a.CompanyCode = b.CompanyCode 
	AND a.BranchCode = b.BranchCode
	AND a.InvoiceNo = b.InvoiceNo
	WHERE a.CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR a.BranchCode = @BranchCode) AND CAST(InvoiceDate AS DATE) BETWEEN @startOfPrevMonth AND @endOfPrevMonth
	
	SELECT @prev2Invoice = COUNT(*) 
	FROM [omTrSalesInvoiceVin] a JOIN [omTrSalesInvoice] b 
	ON a.CompanyCode = b.CompanyCode 
	AND a.BranchCode = b.BranchCode
	AND a.InvoiceNo = b.InvoiceNo
	WHERE a.CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR a.BranchCode = @BranchCode) AND CAST(InvoiceDate AS DATE) BETWEEN @startOfPrev2Month AND @endOfPrev2Month
	
	INSERT INTO @Temp
	SELECT  @startOfMonth, @currentWorkingDay, @startOfPrevMonth, @currentPrevWorkingDay, @endOfMonth, 
			@endOfPrevMonth, @startOfPrev2Month, @endOfPrev2Month,
			ISNULL(@currInq, 0), ISNULL(@prevInq, 0), ISNULL(@prev2Inq, 0), ISNULL(@currPrevInq, 0),
			ISNULL(@currSPK, 0), ISNULL(@prevSPK, 0), ISNULL(@prev2SPK, 0), ISNULL(@currPrevSPK, 0),
			ISNULL(@currStock, 0), ISNULL(@prevStock, 0), ISNULL(@prev2Stock, 0), ISNULL(@currPrevStock, 0),
			ISNULL(@currInvoice, 0), ISNULL(@prevInvoice, 0), ISNULL(@prev2Invoice, 0), ISNULL(@currPrevInvoice, 0)

	SELECT PeriodOfCurrMonth = CAST(DAY(startOfMonth) AS VARCHAR) + '-' + CAST(DAY(currentDate) AS VARCHAR),
			PeriodOfPrevMonth = CAST(DAY(startOfPrevMonth) AS VARCHAR) + '-' + CAST(DAY(PrevMonthOfCurrDate) AS VARCHAR),
			* 
	FROM @Temp;
END
GO

