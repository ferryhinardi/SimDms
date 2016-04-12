/*
	[dbo].[uspfn_GetTotalInquiryAndSpk] '6006400001', '6006400106'
*/
ALTER PROCEDURE [dbo].[uspfn_GetTotalInquiryAndSpk]
	@CompanyCode VARCHAR(15),
	@BranchCode VARCHAR(15)
AS

IF @BranchCode = '' SET @BranchCode = NULL;

DECLARE @currInq INT, @prevInq INT, @prev2Inq INT, @currPrevInq INT, @currPrev2Inq INT, 
		@currSPK INT, @prevSPK INT, @prev2SPK INT, @currPrevSPK INT, @currPrev2SPK INT,
		@currStock INT, @prevStock INT, @prev2Stock INT, @currPrevStock INT, @currPrev2Stock INT,
		@currInvoice INT, @prevInvoice INT, @prev2Invoice INT, @currPrevInvoice INT, @currPrev2Invoice INT,
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
	CurrentDate DATE,
	PrevMonthOfCurrDate DATE,
	Prev2MonthOfCurrDate DATE,
	startOfMonth DATE,
	endOfMonth DATE,
	startOfPrevMonth DATE,
	endOfPrevMonth DATE,
	startOfPrev2Month DATE,
	endOfPrev2Month DATE,
	currInq INT, prevInq INT, prev2Inq INT, currPrevINQ INT, currPrev2Inq INT, 
	currSPK INT, prevSPK INT, prev2SPK INT, currPrevSPK INT, currPrev2SPK INT,
	currStock INT, prevStock INT, prev2Stock INT, currPrevStock INT, currPrev2Stock INT,
	currInvoice INT, prevInvoice INT, prev2Invoice INT, currPrevInvoice INT, currPrev2Invoice INT
)

SELECT @currentDate = GETDATE(),
		@currentPrevDate = DATEADD(MONTH, -1, GETDATE()),
		@currentPrev2Date = DATEADD(MONTH, -2, GETDATE()),
		@startOfMonth = CONVERT(VARCHAR(8), GETDATE(), 120) + '01',
		@endOfMonth = DATEADD(DAY, -1, DATEADD(MONTH, 1, @startOfMonth)),
		@startOfPrevMonth = DATEADD(MONTH, -1, @startOfMonth),
		@endOfPrevMonth = DATEADD(DAY, -1, @startOfMonth),
		@startOfPrev2Month = DATEADD(MONTH, -1, @startOfPrevMonth),
		@endOfPrev2Month = DATEADD(DAY, -1, @startOfPrevMonth)

	-- SELECT @currentDate, @currentPrevDate, @currentPrev2Date, @startOfMonth, @endOfMonth, @startOfPrevMonth, @endOfPrevMonth, @startOfPrev2Month, @endOfPrev2Month
	
	SELECT @currInq = COUNT(*) FROM pmkdp WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(InquiryDate AS DATE) BETWEEN @startOfMonth AND @currentDate
	SELECT @prevInq = COUNT(*) FROM pmkdp WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(InquiryDate AS DATE) BETWEEN @startOfPrevMonth AND @endOfPrevMonth
	SELECT @prev2Inq = COUNT(*) FROM pmkdp WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(InquiryDate AS DATE) BETWEEN @startOfPrev2Month AND @endOfPrev2Month
	SELECT @currPrevInq = COUNT(*) FROM pmkdp WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(InquiryDate AS DATE) BETWEEN @startOfPrevMonth AND @currentPrevDate
	SELECT @currPrev2Inq = COUNT(*) FROM pmkdp WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(InquiryDate AS DATE) BETWEEN @startOfPrev2Month AND @currentPrev2Date
	
	SELECT @currSPK = COUNT(*) FROM ( SELECT DISTINCT [InquiryNumber],[CompanyCode],[BranchCode] FROM pmStatusHistory  
		WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(updateDate AS DATE) BETWEEN @startOfMonth AND @currentDate AND LastProgress = 'spk') a
	SELECT @prevSPK = COUNT(*) FROM ( SELECT DISTINCT [InquiryNumber],[CompanyCode],[BranchCode] FROM pmStatusHistory  
		WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(updateDate AS DATE) BETWEEN @startOfPrevMonth AND @endOfPrevMonth AND LastProgress = 'spk') a
	SELECT @prev2SPK = COUNT(*) FROM ( SELECT DISTINCT [InquiryNumber],[CompanyCode],[BranchCode] FROM pmStatusHistory  
		WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(updateDate AS DATE) BETWEEN @startOfPrev2Month AND @endOfPrev2Month AND LastProgress = 'spk') a
	SELECT @currPrevSPK = COUNT(*) FROM ( SELECT DISTINCT [InquiryNumber],[CompanyCode],[BranchCode] FROM pmStatusHistory  
		WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(updateDate AS DATE) BETWEEN @startOfPrevMonth AND @currentPrevDate AND LastProgress = 'spk') a
	SELECT @currPrev2SPK = COUNT(*) FROM ( SELECT DISTINCT [InquiryNumber],[CompanyCode],[BranchCode] FROM pmStatusHistory  
		WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(updateDate AS DATE) BETWEEN @startOfPrev2Month AND @currentPrev2Date AND LastProgress = 'spk') a
	
	SELECT @currStock = SUM(EndingOH) FROM omTrInventQtyVehicle WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(CreatedDate AS DATE) BETWEEN @startOfMonth AND @currentDate
	SELECT @prevStock = SUM(EndingOH) FROM omTrInventQtyVehicle WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(CreatedDate AS DATE) BETWEEN @startOfPrevMonth AND @endOfPrevMonth
	SELECT @prev2Stock = SUM(EndingOH) FROM omTrInventQtyVehicle WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(CreatedDate AS DATE) BETWEEN @startOfPrev2Month AND @endOfPrev2Month
	SELECT @currPrevStock = SUM(EndingOH) FROM omTrInventQtyVehicle WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(CreatedDate AS DATE) BETWEEN @startOfPrevMonth AND @currentPrevDate
	SELECT @currPrev2Stock = SUM(EndingOH) FROM omTrInventQtyVehicle WHERE CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR BranchCode = @BranchCode) AND CAST(CreatedDate AS DATE) BETWEEN @startOfPrev2Month AND @currentPrev2Date
	
	SELECT @currInvoice = COUNT(*) 
	FROM [omTrSalesInvoiceVin] a JOIN [omTrSalesInvoice] b 
	ON a.CompanyCode = b.CompanyCode 
	AND a.BranchCode = b.BranchCode
	AND a.InvoiceNo = b.InvoiceNo
	WHERE a.CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR a.BranchCode = @BranchCode) AND CAST(InvoiceDate AS DATE) BETWEEN @startOfMonth AND @currentDate
	
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
	
	SELECT @currPrevInvoice = COUNT(*) 
	FROM [omTrSalesInvoiceVin] a JOIN [omTrSalesInvoice] b 
	ON a.CompanyCode = b.CompanyCode 
	AND a.BranchCode = b.BranchCode
	AND a.InvoiceNo = b.InvoiceNo
	WHERE a.CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR a.BranchCode = @BranchCode) AND CAST(InvoiceDate AS DATE) BETWEEN @startOfPrevMonth AND @currentPrevDate
	
	SELECT @currPrev2Invoice = COUNT(*) 
	FROM [omTrSalesInvoiceVin] a JOIN [omTrSalesInvoice] b 
	ON a.CompanyCode = b.CompanyCode 
	AND a.BranchCode = b.BranchCode
	AND a.InvoiceNo = b.InvoiceNo
	WHERE a.CompanyCode = @CompanyCode AND (@BranchCode IS NULL OR a.BranchCode = @BranchCode) AND CAST(InvoiceDate AS DATE) BETWEEN @startOfPrev2Month AND @currentPrev2Date
	
	INSERT INTO @Temp
	SELECT @currentDate, @currentPrevDate, @currentPrev2Date, @startOfMonth, @endOfMonth, 
			@startOfPrevMonth, @endOfPrevMonth, @startOfPrev2Month, @endOfPrev2Month,
			ISNULL(@currInq, 0), ISNULL(@prevInq, 0), ISNULL(@prev2Inq, 0), ISNULL(@currPrevInq, 0), ISNULL(@currPrev2Inq, 0), 
			ISNULL(@currSPK, 0), ISNULL(@prevSPK, 0), ISNULL(@prev2SPK, 0), ISNULL(@currPrevSPK, 0), ISNULL(@currPrev2SPK, 0), 
			ISNULL(@currStock, 0), ISNULL(@prevStock, 0), ISNULL(@prev2Stock, 0), ISNULL(@currPrevStock, 0), ISNULL(@currPrev2Stock, 0), 
			ISNULL(@currInvoice, 0), ISNULL(@prevInvoice, 0), ISNULL(@prev2Invoice, 0), ISNULL(@currPrevInvoice, 0), ISNULL(@currPrev2Invoice, 0)

	SELECT PeriodOfCurrMonth = CAST(DAY(@startOfMonth) AS VARCHAR) + '-' + CAST(DAY(@currentDate) AS VARCHAR),
			* 
	FROM @Temp;