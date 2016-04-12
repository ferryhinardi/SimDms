
DECLARE
	@Year					INT
	, @CurrentItem			NVARCHAR(40)
	, @CurrentCode			VARCHAR(10)
	, @Seq					INT

SET	@Year = 2012

DECLARE @ResultTable TABLE (
	Seq				INT
	, Item			NVARCHAR(40)
	, LastYear		INT
	, YtdLastYear	INT
	, Ytd			INT
	, Jan			INT
	, Feb			INT
	, Mar			INT
	, Apr			INT
	, May			INT
	, Jun			INT
	, Jul			INT
	, Aug			INT
	, Sep			INT
	, Oct			INT
	, Nov			INT
	, [Dec]			INT
)

DECLARE @ItemTable TABLE (
	Seq				INT
	, ItemName		NVARCHAR(40)
	, TrainingCode	VARCHAR(20)  -- '' = Sigma
)

INSERT INTO @ItemTable VALUES (1, 'BMDP Basic', 'BMDPB')
INSERT INTO @ItemTable VALUES (2, 'BMDP Intermediate', 'BMDP')
INSERT INTO @ItemTable VALUES (4, 'SH Basic', 'SHB')
INSERT INTO @ItemTable VALUES (5, 'SH Intermediate', 'SHI')
INSERT INTO @ItemTable VALUES (7, 'STDP1', 'STDP1')
INSERT INTO @ItemTable VALUES (8, 'STDP2', 'STDP2')
INSERT INTO @ItemTable VALUES (9, 'STDP3', 'STDP3')
INSERT INTO @ItemTable VALUES (10, 'STDP4', 'STDP4')
INSERT INTO @ItemTable VALUES (11, 'STDP5', 'STDP5')
INSERT INTO @ItemTable VALUES (12, 'STDP6', 'STDP6')
INSERT INTO @ItemTable VALUES (13, 'STDP7', 'STDP7')
INSERT INTO @ItemTable VALUES (14, 'SPS Silver', 'SPSS')
INSERT INTO @ItemTable VALUES (15, 'SPS Gold', 'SPSG')
INSERT INTO @ItemTable VALUES (16, 'SPS Platinum', 'SPSP')
INSERT INTO @ItemTable VALUES (18, 'CRO Basic', 'CROB')
INSERT INTO @ItemTable VALUES (19, 'CRO Intermediate', 'CROI')
INSERT INTO @ItemTable VALUES (21, 'TFT Basic', 'TFTB')
INSERT INTO @ItemTable VALUES (22, 'TFT Intermediate', 'TFTI')
INSERT INTO @ItemTable VALUES (23, 'TFT Advanced', 'TFTA')
INSERT INTO @ItemTable VALUES (0, NCHAR(931) + ' Trained Branch Manager', 'T-BM')
INSERT INTO @ItemTable VALUES (3, NCHAR(931) + ' Trained Sales Head', 'T-SH')
INSERT INTO @ItemTable VALUES (6, NCHAR(931) + ' Trained Wiraniaga', 'T-S')
INSERT INTO @ItemTable VALUES (17, NCHAR(931) + ' Trained CRO', 'T-CRO')
INSERT INTO @ItemTable VALUES (20, NCHAR(931) + ' Trained Suzuki Trainer', 'T-ST')

DECLARE curItem CURSOR LOCAL FAST_FORWARD FOR SELECT Seq, ItemName, TrainingCode FROM @ItemTable OPEN curItem
FETCH NEXT FROM curItem INTO @Seq, @CurrentItem, @CurrentCode
WHILE @@FETCH_STATUS = 0 BEGIN
	DECLARE @RowTable TABLE (
		Number			INT
		,Value			INT
	)
	DECLARE @i INT
	SET @i = 1
	WHILE (@i <= 15) BEGIN
		DECLARE @value INT
		IF(@i <= 12) BEGIN
			SET @value = (SELECT COUNT(EmployeeID) FROM HrEmployeeTraining WHERE TrainingCode = @CurrentCode
							AND YEAR(TrainingDate) = @Year AND MONTH(TrainingDate) = @i)
		END ELSE IF (@i = 13) BEGIN -- Last Year
			SET @value = (SELECT COUNT(EmployeeID) FROM HrEmployeeTraining WHERE TrainingCode = @CurrentCode
							AND YEAR(TrainingDate) = @Year - 1)
		END ELSE IF (@i = 14) BEGIN -- YTD Last Year
			SET @value = (SELECT COUNT(EmployeeID) FROM HrEmployeeTraining WHERE TrainingCode = @CurrentCode
							AND YEAR(TrainingDate) = @Year - 1 AND MONTH(TrainingDate) BETWEEN 1 AND MONTH(GETDATE()))
		END ELSE IF (@i = 15) BEGIN -- YTD
			SET @value = (SELECT COUNT(EmployeeID) FROM HrEmployeeTraining WHERE TrainingCode = @CurrentCode
							AND YEAR(TrainingDate) = @Year AND MONTH(TrainingDate) BETWEEN 1 AND MONTH(GETDATE()))
		END
		
		INSERT INTO @RowTable VALUES (@i, @value)
		SET @i = @i + 1
	END

	IF(SUBSTRING(@CurrentCode, 1, 2) <> 'T-') BEGIN
		INSERT INTO @ResultTable (Seq, Item, LastYear, YtdLastYear, Ytd, Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, [Dec])
			VALUES (
				@Seq
				, @CurrentItem
				, (SELECT Value FROM @RowTable WHERE Number = 13)
				, (SELECT Value FROM @RowTable WHERE Number = 14)
				, (SELECT Value FROM @RowTable WHERE Number = 15)
				, (SELECT Value FROM @RowTable WHERE Number = 1)	
				, (SELECT Value FROM @RowTable WHERE Number = 2)	
				, (SELECT Value FROM @RowTable WHERE Number = 3)	
				, (SELECT Value FROM @RowTable WHERE Number = 4)	
				, (SELECT Value FROM @RowTable WHERE Number = 5)	
				, (SELECT Value FROM @RowTable WHERE Number = 6)	
				, (SELECT Value FROM @RowTable WHERE Number = 7)	
				, (SELECT Value FROM @RowTable WHERE Number = 8)	
				, (SELECT Value FROM @RowTable WHERE Number = 9)	
				, (SELECT Value FROM @RowTable WHERE Number = 10)
				, (SELECT Value FROM @RowTable WHERE Number = 11)
				, (SELECT Value FROM @RowTable WHERE Number = 12)
			)
	END ELSE BEGIN
		IF (@CurrentCode = 'T-BM') BEGIN
			INSERT INTO @ResultTable (Seq, Item, LastYear, YtdLastYear, Ytd, Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, [Dec])
			VALUES (
				@Seq
				, @CurrentItem
				, (SELECT LastYear FROM @ResultTable WHERE Seq = 1) + (SELECT LastYear FROM @ResultTable WHERE Seq = 2)
				, (SELECT YtdLastYear FROM @ResultTable WHERE Seq = 1) + (SELECT YtdLastYear FROM @ResultTable WHERE Seq = 2)
				, (SELECT Ytd FROM @ResultTable WHERE Seq = 1) + (SELECT Ytd FROM @ResultTable WHERE Seq = 2)
				, (SELECT [Jan] FROM @ResultTable WHERE Seq = 1) + (SELECT [Jan] FROM @ResultTable WHERE Seq = 2)	
				, (SELECT [Feb] FROM @ResultTable WHERE Seq = 1) + (SELECT [Feb] FROM @ResultTable WHERE Seq = 2)	
				, (SELECT [Mar] FROM @ResultTable WHERE Seq = 1) + (SELECT [Mar] FROM @ResultTable WHERE Seq = 2)	
				, (SELECT [Apr] FROM @ResultTable WHERE Seq = 1) + (SELECT [Apr] FROM @ResultTable WHERE Seq = 2)	
				, (SELECT [May] FROM @ResultTable WHERE Seq = 1) + (SELECT [May] FROM @ResultTable WHERE Seq = 2)	
				, (SELECT [Jun] FROM @ResultTable WHERE Seq = 1) + (SELECT [Jun] FROM @ResultTable WHERE Seq = 2)	
				, (SELECT [Jul] FROM @ResultTable WHERE Seq = 1) + (SELECT [Jul] FROM @ResultTable WHERE Seq = 2)	
				, (SELECT [Aug] FROM @ResultTable WHERE Seq = 1) + (SELECT [Aug] FROM @ResultTable WHERE Seq = 2)	
				, (SELECT [Sep] FROM @ResultTable WHERE Seq = 1) + (SELECT [Sep] FROM @ResultTable WHERE Seq = 2)	
				, (SELECT [Oct] FROM @ResultTable WHERE Seq = 1) + (SELECT [Oct] FROM @ResultTable WHERE Seq = 2)
				, (SELECT [Nov] FROM @ResultTable WHERE Seq = 1) + (SELECT [Nov] FROM @ResultTable WHERE Seq = 2)
				, (SELECT [Dec] FROM @ResultTable WHERE Seq = 1) + (SELECT [Dec] FROM @ResultTable WHERE Seq = 2)
			)
		END ELSE IF (@CurrentCode = 'T-SH') BEGIN
			INSERT INTO @ResultTable (Seq, Item, LastYear, YtdLastYear, Ytd, Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, [Dec])
			VALUES (
				@Seq
				, @CurrentItem
				, (SELECT LastYear FROM @ResultTable WHERE Seq = 4) + (SELECT LastYear FROM @ResultTable WHERE Seq = 5)
				, (SELECT YtdLastYear FROM @ResultTable WHERE Seq = 4) + (SELECT YtdLastYear FROM @ResultTable WHERE Seq = 5)
				, (SELECT Ytd FROM @ResultTable WHERE Seq = 4) + (SELECT Ytd FROM @ResultTable WHERE Seq = 5)
				, (SELECT [Jan] FROM @ResultTable WHERE Seq = 4) + (SELECT [Jan] FROM @ResultTable WHERE Seq = 5)	
				, (SELECT [Feb] FROM @ResultTable WHERE Seq = 4) + (SELECT [Feb] FROM @ResultTable WHERE Seq = 5)	
				, (SELECT [Mar] FROM @ResultTable WHERE Seq = 4) + (SELECT [Mar] FROM @ResultTable WHERE Seq = 5)	
				, (SELECT [Apr] FROM @ResultTable WHERE Seq = 4) + (SELECT [Apr] FROM @ResultTable WHERE Seq = 5)	
				, (SELECT [May] FROM @ResultTable WHERE Seq = 4) + (SELECT [May] FROM @ResultTable WHERE Seq = 5)	
				, (SELECT [Jun] FROM @ResultTable WHERE Seq = 4) + (SELECT [Jun] FROM @ResultTable WHERE Seq = 5)	
				, (SELECT [Jul] FROM @ResultTable WHERE Seq = 4) + (SELECT [Jul] FROM @ResultTable WHERE Seq = 5)	
				, (SELECT [Aug] FROM @ResultTable WHERE Seq = 4) + (SELECT [Aug] FROM @ResultTable WHERE Seq = 5)	
				, (SELECT [Sep] FROM @ResultTable WHERE Seq = 4) + (SELECT [Sep] FROM @ResultTable WHERE Seq = 5)	
				, (SELECT [Oct] FROM @ResultTable WHERE Seq = 4) + (SELECT [Oct] FROM @ResultTable WHERE Seq = 5)
				, (SELECT [Nov] FROM @ResultTable WHERE Seq = 4) + (SELECT [Nov] FROM @ResultTable WHERE Seq = 5)
				, (SELECT [Dec] FROM @ResultTable WHERE Seq = 4) + (SELECT [Dec] FROM @ResultTable WHERE Seq = 5)
			)
		END ELSE IF (@CurrentCode = 'T-S') BEGIN
			INSERT INTO @ResultTable (Seq, Item, LastYear, YtdLastYear, Ytd, Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, [Dec])
			VALUES (
				@Seq
				, @CurrentItem
				, (SELECT LastYear FROM @ResultTable WHERE Seq = 7)		+ (SELECT LastYear FROM @ResultTable WHERE Seq = 8)		+ (SELECT LastYear FROM @ResultTable WHERE Seq = 9)		+ (SELECT LastYear FROM @ResultTable WHERE Seq = 10)	+ (SELECT LastYear FROM @ResultTable WHERE Seq = 11)	+ (SELECT LastYear FROM @ResultTable WHERE Seq = 12)	+ (SELECT LastYear FROM @ResultTable WHERE Seq = 13)	+ (SELECT LastYear FROM @ResultTable WHERE Seq = 14)	+ (SELECT LastYear FROM @ResultTable WHERE Seq = 15)	+ (SELECT LastYear FROM @ResultTable WHERE Seq = 16)
				, (SELECT YtdLastYear FROM @ResultTable WHERE Seq = 7)	+ (SELECT YtdLastYear FROM @ResultTable WHERE Seq = 8)	+ (SELECT YtdLastYear FROM @ResultTable WHERE Seq = 9)	+ (SELECT YtdLastYear FROM @ResultTable WHERE Seq = 10)	+ (SELECT YtdLastYear FROM @ResultTable WHERE Seq = 11)	+ (SELECT YtdLastYear FROM @ResultTable WHERE Seq = 12)	+ (SELECT YtdLastYear FROM @ResultTable WHERE Seq = 13)	+ (SELECT YtdLastYear FROM @ResultTable WHERE Seq = 14)	+ (SELECT YtdLastYear FROM @ResultTable WHERE Seq = 15)	+ (SELECT YtdLastYear FROM @ResultTable WHERE Seq = 16)
				, (SELECT Ytd FROM @ResultTable WHERE Seq = 7)			+ (SELECT Ytd FROM @ResultTable WHERE Seq = 8)			+ (SELECT Ytd FROM @ResultTable WHERE Seq = 9)			+ (SELECT Ytd FROM @ResultTable WHERE Seq = 10)			+ (SELECT Ytd FROM @ResultTable WHERE Seq = 11)			+ (SELECT Ytd FROM @ResultTable WHERE Seq = 12)			+ (SELECT Ytd FROM @ResultTable WHERE Seq = 13)			+ (SELECT Ytd FROM @ResultTable WHERE Seq = 14)			+ (SELECT Ytd FROM @ResultTable WHERE Seq = 15)			+ (SELECT Ytd FROM @ResultTable WHERE Seq = 16)
				, (SELECT [Jan] FROM @ResultTable WHERE Seq = 7)		+ (SELECT [Jan] FROM @ResultTable WHERE Seq = 8)		+ (SELECT [Jan] FROM @ResultTable WHERE Seq = 9)		+ (SELECT [Jan] FROM @ResultTable WHERE Seq = 10)		+ (SELECT [Jan] FROM @ResultTable WHERE Seq = 11)		+ (SELECT [Jan] FROM @ResultTable WHERE Seq = 12)		+ (SELECT [Jan] FROM @ResultTable WHERE Seq = 13)		+ (SELECT [Jan] FROM @ResultTable WHERE Seq = 14)		+ (SELECT [Jan] FROM @ResultTable WHERE Seq = 15)		+ (SELECT [Jan] FROM @ResultTable WHERE Seq = 16)	
				, (SELECT [Feb] FROM @ResultTable WHERE Seq = 7)		+ (SELECT [Feb] FROM @ResultTable WHERE Seq = 8)		+ (SELECT [Feb] FROM @ResultTable WHERE Seq = 9)		+ (SELECT [Feb] FROM @ResultTable WHERE Seq = 10)		+ (SELECT [Feb] FROM @ResultTable WHERE Seq = 11)		+ (SELECT [Feb] FROM @ResultTable WHERE Seq = 12)		+ (SELECT [Feb] FROM @ResultTable WHERE Seq = 13)		+ (SELECT [Feb] FROM @ResultTable WHERE Seq = 14)		+ (SELECT [Feb] FROM @ResultTable WHERE Seq = 15)		+ (SELECT [Feb] FROM @ResultTable WHERE Seq = 16)	
				, (SELECT [Mar] FROM @ResultTable WHERE Seq = 7)		+ (SELECT [Mar] FROM @ResultTable WHERE Seq = 8)		+ (SELECT [Mar] FROM @ResultTable WHERE Seq = 9)		+ (SELECT [Mar] FROM @ResultTable WHERE Seq = 10)		+ (SELECT [Mar] FROM @ResultTable WHERE Seq = 11)		+ (SELECT [Mar] FROM @ResultTable WHERE Seq = 12)		+ (SELECT [Mar] FROM @ResultTable WHERE Seq = 13)		+ (SELECT [Mar] FROM @ResultTable WHERE Seq = 14)		+ (SELECT [Mar] FROM @ResultTable WHERE Seq = 15)		+ (SELECT [Mar] FROM @ResultTable WHERE Seq = 16)	
				, (SELECT [Apr] FROM @ResultTable WHERE Seq = 7)		+ (SELECT [Apr] FROM @ResultTable WHERE Seq = 8)		+ (SELECT [Apr] FROM @ResultTable WHERE Seq = 9)		+ (SELECT [Apr] FROM @ResultTable WHERE Seq = 10)		+ (SELECT [Apr] FROM @ResultTable WHERE Seq = 11)		+ (SELECT [Apr] FROM @ResultTable WHERE Seq = 12)		+ (SELECT [Apr] FROM @ResultTable WHERE Seq = 13)		+ (SELECT [Apr] FROM @ResultTable WHERE Seq = 14)		+ (SELECT [Apr] FROM @ResultTable WHERE Seq = 15)		+ (SELECT [Apr] FROM @ResultTable WHERE Seq = 16)	
				, (SELECT [May] FROM @ResultTable WHERE Seq = 7)		+ (SELECT [May] FROM @ResultTable WHERE Seq = 8)		+ (SELECT [May] FROM @ResultTable WHERE Seq = 9)		+ (SELECT [May] FROM @ResultTable WHERE Seq = 10)		+ (SELECT [May] FROM @ResultTable WHERE Seq = 11)		+ (SELECT [May] FROM @ResultTable WHERE Seq = 12)		+ (SELECT [May] FROM @ResultTable WHERE Seq = 13)		+ (SELECT [May] FROM @ResultTable WHERE Seq = 14)		+ (SELECT [May] FROM @ResultTable WHERE Seq = 15)		+ (SELECT [May] FROM @ResultTable WHERE Seq = 16)	
				, (SELECT [Jun] FROM @ResultTable WHERE Seq = 7)		+ (SELECT [Jun] FROM @ResultTable WHERE Seq = 8)		+ (SELECT [Jun] FROM @ResultTable WHERE Seq = 9)		+ (SELECT [Jun] FROM @ResultTable WHERE Seq = 10)		+ (SELECT [Jun] FROM @ResultTable WHERE Seq = 11)		+ (SELECT [Jun] FROM @ResultTable WHERE Seq = 12)		+ (SELECT [Jun] FROM @ResultTable WHERE Seq = 13)		+ (SELECT [Jun] FROM @ResultTable WHERE Seq = 14)		+ (SELECT [Jun] FROM @ResultTable WHERE Seq = 15)		+ (SELECT [Jun] FROM @ResultTable WHERE Seq = 16)	
				, (SELECT [Jul] FROM @ResultTable WHERE Seq = 7)		+ (SELECT [Jul] FROM @ResultTable WHERE Seq = 8)		+ (SELECT [Jul] FROM @ResultTable WHERE Seq = 9)		+ (SELECT [Jul] FROM @ResultTable WHERE Seq = 10)		+ (SELECT [Jul] FROM @ResultTable WHERE Seq = 11)		+ (SELECT [Jul] FROM @ResultTable WHERE Seq = 12)		+ (SELECT [Jul] FROM @ResultTable WHERE Seq = 13)		+ (SELECT [Jul] FROM @ResultTable WHERE Seq = 14)		+ (SELECT [Jul] FROM @ResultTable WHERE Seq = 15)		+ (SELECT [Jul] FROM @ResultTable WHERE Seq = 16)	
				, (SELECT [Aug] FROM @ResultTable WHERE Seq = 7)		+ (SELECT [Aug] FROM @ResultTable WHERE Seq = 8)		+ (SELECT [Aug] FROM @ResultTable WHERE Seq = 9)		+ (SELECT [Aug] FROM @ResultTable WHERE Seq = 10)		+ (SELECT [Aug] FROM @ResultTable WHERE Seq = 11)		+ (SELECT [Aug] FROM @ResultTable WHERE Seq = 12)		+ (SELECT [Aug] FROM @ResultTable WHERE Seq = 13)		+ (SELECT [Aug] FROM @ResultTable WHERE Seq = 14)		+ (SELECT [Aug] FROM @ResultTable WHERE Seq = 15)		+ (SELECT [Aug] FROM @ResultTable WHERE Seq = 16)	
				, (SELECT [Sep] FROM @ResultTable WHERE Seq = 7)		+ (SELECT [Sep] FROM @ResultTable WHERE Seq = 8)		+ (SELECT [Sep] FROM @ResultTable WHERE Seq = 9)		+ (SELECT [Sep] FROM @ResultTable WHERE Seq = 10)		+ (SELECT [Sep] FROM @ResultTable WHERE Seq = 11)		+ (SELECT [Sep] FROM @ResultTable WHERE Seq = 12)		+ (SELECT [Sep] FROM @ResultTable WHERE Seq = 13)		+ (SELECT [Sep] FROM @ResultTable WHERE Seq = 14)		+ (SELECT [Sep] FROM @ResultTable WHERE Seq = 15)		+ (SELECT [Sep] FROM @ResultTable WHERE Seq = 16)	
				, (SELECT [Oct] FROM @ResultTable WHERE Seq = 7)		+ (SELECT [Oct] FROM @ResultTable WHERE Seq = 8)		+ (SELECT [Oct] FROM @ResultTable WHERE Seq = 9)		+ (SELECT [Oct] FROM @ResultTable WHERE Seq = 10)		+ (SELECT [Oct] FROM @ResultTable WHERE Seq = 11)		+ (SELECT [Oct] FROM @ResultTable WHERE Seq = 12)		+ (SELECT [Oct] FROM @ResultTable WHERE Seq = 13)		+ (SELECT [Oct] FROM @ResultTable WHERE Seq = 14)		+ (SELECT [Oct] FROM @ResultTable WHERE Seq = 15)		+ (SELECT [Oct] FROM @ResultTable WHERE Seq = 16)
				, (SELECT [Nov] FROM @ResultTable WHERE Seq = 7)		+ (SELECT [Nov] FROM @ResultTable WHERE Seq = 8)		+ (SELECT [Nov] FROM @ResultTable WHERE Seq = 9)		+ (SELECT [Nov] FROM @ResultTable WHERE Seq = 10)		+ (SELECT [Nov] FROM @ResultTable WHERE Seq = 11)		+ (SELECT [Nov] FROM @ResultTable WHERE Seq = 12)		+ (SELECT [Nov] FROM @ResultTable WHERE Seq = 13)		+ (SELECT [Nov] FROM @ResultTable WHERE Seq = 14)		+ (SELECT [Nov] FROM @ResultTable WHERE Seq = 15)		+ (SELECT [Nov] FROM @ResultTable WHERE Seq = 16)
				, (SELECT [Dec] FROM @ResultTable WHERE Seq = 7)		+ (SELECT [Dec] FROM @ResultTable WHERE Seq = 8)		+ (SELECT [Dec] FROM @ResultTable WHERE Seq = 9)		+ (SELECT [Dec] FROM @ResultTable WHERE Seq = 10)		+ (SELECT [Dec] FROM @ResultTable WHERE Seq = 11)		+ (SELECT [Dec] FROM @ResultTable WHERE Seq = 12)		+ (SELECT [Dec] FROM @ResultTable WHERE Seq = 13)		+ (SELECT [Dec] FROM @ResultTable WHERE Seq = 14)		+ (SELECT [Dec] FROM @ResultTable WHERE Seq = 15)		+ (SELECT [Dec] FROM @ResultTable WHERE Seq = 16)
			)
		END ELSE IF (@CurrentCode = 'T-CRO') BEGIN
			INSERT INTO @ResultTable (Seq, Item, LastYear, YtdLastYear, Ytd, Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, [Dec])
			VALUES (
				@Seq
				, @CurrentItem
				, (SELECT LastYear FROM @ResultTable WHERE Seq = 18) + (SELECT LastYear FROM @ResultTable WHERE Seq = 19)
				, (SELECT YtdLastYear FROM @ResultTable WHERE Seq = 18) + (SELECT YtdLastYear FROM @ResultTable WHERE Seq = 19)
				, (SELECT Ytd FROM @ResultTable WHERE Seq = 18) + (SELECT Ytd FROM @ResultTable WHERE Seq = 19)
				, (SELECT [Jan] FROM @ResultTable WHERE Seq = 18) + (SELECT [Jan] FROM @ResultTable WHERE Seq = 19)	
				, (SELECT [Feb] FROM @ResultTable WHERE Seq = 18) + (SELECT [Feb] FROM @ResultTable WHERE Seq = 19)	
				, (SELECT [Mar] FROM @ResultTable WHERE Seq = 18) + (SELECT [Mar] FROM @ResultTable WHERE Seq = 19)	
				, (SELECT [Apr] FROM @ResultTable WHERE Seq = 18) + (SELECT [Apr] FROM @ResultTable WHERE Seq = 19)	
				, (SELECT [May] FROM @ResultTable WHERE Seq = 18) + (SELECT [May] FROM @ResultTable WHERE Seq = 19)	
				, (SELECT [Jun] FROM @ResultTable WHERE Seq = 18) + (SELECT [Jun] FROM @ResultTable WHERE Seq = 19)	
				, (SELECT [Jul] FROM @ResultTable WHERE Seq = 18) + (SELECT [Jul] FROM @ResultTable WHERE Seq = 19)	
				, (SELECT [Aug] FROM @ResultTable WHERE Seq = 18) + (SELECT [Aug] FROM @ResultTable WHERE Seq = 19)	
				, (SELECT [Sep] FROM @ResultTable WHERE Seq = 18) + (SELECT [Sep] FROM @ResultTable WHERE Seq = 19)	
				, (SELECT [Oct] FROM @ResultTable WHERE Seq = 18) + (SELECT [Oct] FROM @ResultTable WHERE Seq = 19)
				, (SELECT [Nov] FROM @ResultTable WHERE Seq = 18) + (SELECT [Nov] FROM @ResultTable WHERE Seq = 19)
				, (SELECT [Dec] FROM @ResultTable WHERE Seq = 18) + (SELECT [Dec] FROM @ResultTable WHERE Seq = 19)
			)

		END ELSE IF (@CurrentCode = 'T-ST') BEGIN
			INSERT INTO @ResultTable (Seq, Item, LastYear, YtdLastYear, Ytd, Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, [Dec])
			VALUES (
				@Seq
				, @CurrentItem
				, (SELECT LastYear FROM @ResultTable WHERE Seq = 21)	+ (SELECT LastYear FROM @ResultTable WHERE Seq = 22)	+ (SELECT LastYear FROM @ResultTable WHERE Seq = 23)		
				, (SELECT YtdLastYear FROM @ResultTable WHERE Seq = 21)	+ (SELECT YtdLastYear FROM @ResultTable WHERE Seq = 22)	+ (SELECT YtdLastYear FROM @ResultTable WHERE Seq = 23)
				, (SELECT Ytd FROM @ResultTable WHERE Seq = 21)			+ (SELECT Ytd FROM @ResultTable WHERE Seq = 22)			+ (SELECT Ytd FROM @ResultTable WHERE Seq = 23)			
				, (SELECT [Jan] FROM @ResultTable WHERE Seq = 21)		+ (SELECT [Jan] FROM @ResultTable WHERE Seq = 22)		+ (SELECT [Jan] FROM @ResultTable WHERE Seq = 23)		
				, (SELECT [Feb] FROM @ResultTable WHERE Seq = 21)		+ (SELECT [Feb] FROM @ResultTable WHERE Seq = 22)		+ (SELECT [Feb] FROM @ResultTable WHERE Seq = 23)		
				, (SELECT [Mar] FROM @ResultTable WHERE Seq = 21)		+ (SELECT [Mar] FROM @ResultTable WHERE Seq = 22)		+ (SELECT [Mar] FROM @ResultTable WHERE Seq = 23)		
				, (SELECT [Apr] FROM @ResultTable WHERE Seq = 21)		+ (SELECT [Apr] FROM @ResultTable WHERE Seq = 22)		+ (SELECT [Apr] FROM @ResultTable WHERE Seq = 23)		
				, (SELECT [May] FROM @ResultTable WHERE Seq = 21)		+ (SELECT [May] FROM @ResultTable WHERE Seq = 22)		+ (SELECT [May] FROM @ResultTable WHERE Seq = 23)		
				, (SELECT [Jun] FROM @ResultTable WHERE Seq = 21)		+ (SELECT [Jun] FROM @ResultTable WHERE Seq = 22)		+ (SELECT [Jun] FROM @ResultTable WHERE Seq = 23)		
				, (SELECT [Jul] FROM @ResultTable WHERE Seq = 21)		+ (SELECT [Jul] FROM @ResultTable WHERE Seq = 22)		+ (SELECT [Jul] FROM @ResultTable WHERE Seq = 23)		
				, (SELECT [Aug] FROM @ResultTable WHERE Seq = 21)		+ (SELECT [Aug] FROM @ResultTable WHERE Seq = 22)		+ (SELECT [Aug] FROM @ResultTable WHERE Seq = 23)		
				, (SELECT [Sep] FROM @ResultTable WHERE Seq = 21)		+ (SELECT [Sep] FROM @ResultTable WHERE Seq = 22)		+ (SELECT [Sep] FROM @ResultTable WHERE Seq = 23)		
				, (SELECT [Oct] FROM @ResultTable WHERE Seq = 21)		+ (SELECT [Oct] FROM @ResultTable WHERE Seq = 22)		+ (SELECT [Oct] FROM @ResultTable WHERE Seq = 23)		
				, (SELECT [Nov] FROM @ResultTable WHERE Seq = 21)		+ (SELECT [Nov] FROM @ResultTable WHERE Seq = 22)		+ (SELECT [Nov] FROM @ResultTable WHERE Seq = 23)		
				, (SELECT [Dec] FROM @ResultTable WHERE Seq = 21)		+ (SELECT [Dec] FROM @ResultTable WHERE Seq = 22)		+ (SELECT [Dec] FROM @ResultTable WHERE Seq = 23)		
			)
		END
	END

	DELETE FROM @RowTable
	FETCH NEXT FROM curItem INTO @Seq, @CurrentItem, @CurrentCode
END CLOSE curItem DEALLOCATE curItem

SELECT Seq, Item, LastYear, YtdLastYear, Ytd, Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, [Dec] FROM @ResultTable ORDER BY Seq