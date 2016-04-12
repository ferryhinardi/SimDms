
DECLARE
	  @DealerCode			VARCHAR(15)
	, @DealerAbbr			VARCHAR(15)
	, @Year					INT
	, @CurrentItem			NVARCHAR(40)
	, @CurrentCode			VARCHAR(10)
	, @Seq					INT
	, @GroupNo				INT
	, @Area					VARCHAR(40)

SELECT @Year = 2015

DECLARE @ResultTable TABLE (
	  GroupNo		INT
	, DealerCode    VARCHAR(15)
	, DealerAbbr	VARCHAR(15)
	, Seq			INT
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

DECLARE @DealerOutlet TABLE
(
	GroupNo		INT,
	DealerCode	VARCHAR(15),
	OutletCode	VARCHAR(15),
	DealerAbbr	VARCHAR(15)
)

INSERT INTO @DealerOutlet
SELECT a.GroupNo, a.DealerCode, a.OutletCode, b.DealerAbbreviation
FROM gnMstDealerOutletMapping a
JOIN gnMstDealerMapping b ON a.DealerCode = b.DealerCode AND a.GroupNo = b.GroupNo
WHERE a.isActive = 1 AND b.isActive = 1

INSERT INTO @ItemTable VALUES (1, 'BMDP Basic', 'BMDPB')
INSERT INTO @ItemTable VALUES (2, 'BMDP Intermediate', 'BMDPI')
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

DECLARE curDealer CURSOR LOCAL FAST_FORWARD FOR SELECT GroupNo, DealerCode, DealerAbbr FROM @DealerOutlet GROUP BY GroupNo, DealerCode, DealerAbbr OPEN curDealer
FETCH NEXT FROM curDealer INTO @GroupNo, @DealerCode, @DealerAbbr
WHILE @@FETCH_STATUS = 0 BEGIN
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
			DECLARE @Query NVARCHAR(4000), @Params NVARCHAR(1000), @value INT
			DECLARE @Date VARCHAR(15) 

			SELECT @Params = N'@GroupNo INT, @DealerCode VARCHAR(15), @CurrentCode VARCHAR(10), @Date VARCHAR(15), @Year INT, @value INT OUTPUT'
			
			SET @Query = N';WITH _1 AS
							(
								SELECT a.CompanyCode, a.EmployeeID, MAX(b.MutationDate) AS MutationDate
								FROM HrEmployee a
								JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
								WHERE a.CompanyCode = @DealerCode AND a.Department = ''SALES'' AND a.PersonnelStatus = ''1''
								GROUP BY a.CompanyCode, a.EmployeeID
							), _2 AS
							(
								SELECT c.GroupNo, a.CompanyCode, a.EmployeeID
								FROM _1 a
								JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
								JOIN gnMstDealerOutletMapping c ON b.BranchCode = c.OutletCode
							), _3 AS
							(
								SELECT b.GroupNo, a.CompanyCode, a.EmployeeID, a.TrainingCode, a.TrainingDate
								FROM HrEmployeeTraining a
								JOIN _2 b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
								WHERE a.CompanyCode = @DealerCode AND a.TrainingCode = @CurrentCode AND b.GroupNo = @GroupNo
							)
							SELECT @value = COUNT(*) FROM _3 a WHERE 1 = 1 '
			
			IF(@i <= 12) BEGIN
				IF(@i > MONTH(GETDATE())) BEGIN
					SET @Query = N'SELECT @value = 0'
				END ELSE BEGIN
					SELECT @Date = CONVERT(DATE, DATEADD(SECOND,-1,DATEADD(MONTH,DATEDIFF(MONTH,0, CONVERT(VARCHAR(15), @i) + '/01/' + CONVERT(VARCHAR(15), @Year))+1,0)))
					SET @Query += N'AND CONVERT(date, a.TrainingDate) <= @Date'
				END
			END ELSE IF (@i = 13) BEGIN -- Last Year
				SET @Query += N'AND YEAR(a.TrainingDate) = @Year - 1'
			END ELSE IF (@i = 14) BEGIN -- YTD Last Year
				SET @Query += N'AND YEAR(a.TrainingDate) = @Year - 1 AND MONTH(a.TrainingDate) BETWEEN 1 AND MONTH(GETDATE())'
			END ELSE IF (@i = 15) BEGIN -- YTD
				SET @Query += N'AND YEAR(a.TrainingDate) = @Year AND MONTH(a.TrainingDate) BETWEEN 1 AND MONTH(GETDATE())'
			END
			
			EXEC sp_executesql @Query, @Params, @GroupNo=@GroupNo, @DealerCode=@DealerCode, @CurrentCode=@CurrentCode, @Date=@Date, @Year=@Year, @value=@value OUTPUT 

			INSERT INTO @RowTable VALUES (@i, @value)
			SET @i = @i + 1
		END
		
		IF(SUBSTRING(@CurrentCode, 1, 2) <> 'T-') BEGIN
			INSERT INTO @ResultTable (GroupNo, DealerCode, Seq, Item, LastYear, YtdLastYear, Ytd, Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, [Dec])
				VALUES (
					@GroupNo
					, @DealerCode
					, @Seq
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
				INSERT INTO @ResultTable (GroupNo, DealerCode, Seq, Item, LastYear, YtdLastYear, Ytd, Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, [Dec])
				VALUES (
					@GroupNo
					, @DealerCode
					, @Seq
					, @CurrentItem
					, (SELECT SUM(LastYear	 )	FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(1,2))
					, (SELECT SUM(YtdLastYear)	FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(1,2))
					, (SELECT SUM(Ytd		 )	FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(1,2))
					, (SELECT SUM([Jan]		 )	FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(1,2))
					, (SELECT SUM([Feb]		 )	FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(1,2))
					, (SELECT SUM([Mar]		 )	FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(1,2))
					, (SELECT SUM([Apr]		 )	FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(1,2))
					, (SELECT SUM([May]		 )	FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(1,2))
					, (SELECT SUM([Jun]		 )	FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(1,2))
					, (SELECT SUM([Jul]		 )	FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(1,2))
					, (SELECT SUM([Aug]		 )	FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(1,2))
					, (SELECT SUM([Sep]		 )	FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(1,2))
					, (SELECT SUM([Oct]		 )	FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(1,2))
					, (SELECT SUM([Nov]		 )	FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(1,2))
					, (SELECT SUM([Dec]		 )	FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(1,2))
				)
			END ELSE IF (@CurrentCode = 'T-SH') BEGIN
				INSERT INTO @ResultTable (GroupNo, DealerCode, Seq, Item, LastYear, YtdLastYear, Ytd, Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, [Dec])
				VALUES (
					@GroupNo
					, @DealerCode
					, @Seq
					, @CurrentItem
					, (SELECT SUM(LastYear   ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(4,5))
					, (SELECT SUM(YtdLastYear) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(4,5))
					, (SELECT SUM(Ytd        ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(4,5))
					, (SELECT SUM([Jan]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(4,5))	
					, (SELECT SUM([Feb]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(4,5))	
					, (SELECT SUM([Mar]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(4,5))	
					, (SELECT SUM([Apr]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(4,5))	
					, (SELECT SUM([May]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(4,5))	
					, (SELECT SUM([Jun]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(4,5))	
					, (SELECT SUM([Jul]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(4,5))	
					, (SELECT SUM([Aug]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(4,5))	
					, (SELECT SUM([Sep]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(4,5))	
					, (SELECT SUM([Oct]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(4,5))
					, (SELECT SUM([Nov]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(4,5))
					, (SELECT SUM([Dec]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(4,5))
				)
			END ELSE IF (@CurrentCode = 'T-S') BEGIN
				INSERT INTO @ResultTable (GroupNo, DealerCode, Seq, Item, LastYear, YtdLastYear, Ytd, Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, [Dec])
				VALUES (
					@GroupNo
					, @DealerCode
					, @Seq
					, @CurrentItem
					, (SELECT SUM(LastYear   ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(7,8,9,10,11,12,13,14,15,16))
					, (SELECT SUM(YtdLastYear) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(7,8,9,10,11,12,13,14,15,16))
					, (SELECT SUM(Ytd        ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(7,8,9,10,11,12,13,14,15,16))
					, (SELECT SUM([Jan]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(7,8,9,10,11,12,13,14,15,16))
					, (SELECT SUM([Feb]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(7,8,9,10,11,12,13,14,15,16))
					, (SELECT SUM([Mar]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(7,8,9,10,11,12,13,14,15,16))
					, (SELECT SUM([Apr]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(7,8,9,10,11,12,13,14,15,16))
					, (SELECT SUM([May]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(7,8,9,10,11,12,13,14,15,16))
					, (SELECT SUM([Jun]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(7,8,9,10,11,12,13,14,15,16))
					, (SELECT SUM([Jul]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(7,8,9,10,11,12,13,14,15,16))
					, (SELECT SUM([Aug]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(7,8,9,10,11,12,13,14,15,16))
					, (SELECT SUM([Sep]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(7,8,9,10,11,12,13,14,15,16))
					, (SELECT SUM([Oct]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(7,8,9,10,11,12,13,14,15,16))
					, (SELECT SUM([Nov]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(7,8,9,10,11,12,13,14,15,16))
					, (SELECT SUM([Dec]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(7,8,9,10,11,12,13,14,15,16))
				)
			END ELSE IF (@CurrentCode = 'T-CRO') BEGIN
				INSERT INTO @ResultTable (GroupNo, DealerCode, Seq, Item, LastYear, YtdLastYear, Ytd, Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, [Dec])
				VALUES (
					@GroupNo
					, @DealerCode
					, @Seq
					, @CurrentItem
					, (SELECT SUM(LastYear)    FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN (18, 19))
					, (SELECT SUM(YtdLastYear) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN (18, 19))
					, (SELECT SUM(Ytd  )       FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN (18, 19))      
					, (SELECT SUM([Jan])       FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN (18, 19))      
					, (SELECT SUM([Feb])       FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN (18, 19))      
					, (SELECT SUM([Mar])       FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN (18, 19))      
					, (SELECT SUM([Apr])       FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN (18, 19))      
					, (SELECT SUM([May])       FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN (18, 19))      
					, (SELECT SUM([Jun])       FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN (18, 19))      
					, (SELECT SUM([Jul])       FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN (18, 19))      
					, (SELECT SUM([Aug])       FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN (18, 19))      
					, (SELECT SUM([Sep])       FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN (18, 19))      
					, (SELECT SUM([Oct])       FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN (18, 19))      
					, (SELECT SUM([Nov])       FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN (18, 19))      
					, (SELECT SUM([Dec])       FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN (18, 19))      
				)

			END ELSE IF (@CurrentCode = 'T-ST') BEGIN
				INSERT INTO @ResultTable (GroupNo, DealerCode, Seq, Item, LastYear, YtdLastYear, Ytd, Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, [Dec])
				VALUES (
					@GroupNo
					, @DealerCode
					, @Seq
					, @CurrentItem
					, (SELECT SUM(LastYear   ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(21,22,23))
					, (SELECT SUM(YtdLastYear) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(21,22,23))
					, (SELECT SUM(Ytd        ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(21,22,23))	
					, (SELECT SUM([Jan]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(21,22,23))
					, (SELECT SUM([Feb]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(21,22,23))
					, (SELECT SUM([Mar]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(21,22,23))
					, (SELECT SUM([Apr]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(21,22,23))
					, (SELECT SUM([May]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(21,22,23))
					, (SELECT SUM([Jun]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(21,22,23))
					, (SELECT SUM([Jul]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(21,22,23))
					, (SELECT SUM([Aug]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(21,22,23))
					, (SELECT SUM([Sep]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(21,22,23))
					, (SELECT SUM([Oct]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(21,22,23))
					, (SELECT SUM([Nov]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(21,22,23))
					, (SELECT SUM([Dec]      ) FROM @ResultTable WHERE GroupNo = @GroupNo AND @DealerCode = DealerCode AND Seq IN(21,22,23))
				)
			END
		END

		DELETE FROM @RowTable
		FETCH NEXT FROM curItem INTO @Seq, @CurrentItem, @CurrentCode
	END CLOSE curItem DEALLOCATE curItem
FETCH NEXT FROM curDealer INTO @GroupNo, @DealerCode, @DealerAbbr
END CLOSE curDealer DEALLOCATE curDealer

;WITH _1 AS
(
	SELECT GroupNo, DealerCode, DealerAbbr FROM @DealerOutlet GROUP BY GroupNo, DealerCode, DealerAbbr
)
SELECT a.GroupNo, a.DealerCode, b.DealerAbbr, a.Seq, a.Item, a.LastYear, a.YtdLastYear, a.Ytd, a.Jan, a.Feb, a.Mar, a.Apr, a.May, a.Jun, a.Jul, a.Aug, a.Sep, a.Oct, a.Nov, a.[Dec]
FROM @ResultTable a
JOIN _1 b ON a.GroupNo = b.GroupNo AND a.DealerCode = b.DealerCode
ORDER BY a.DealerCode, b.DealerAbbr, a.Seq