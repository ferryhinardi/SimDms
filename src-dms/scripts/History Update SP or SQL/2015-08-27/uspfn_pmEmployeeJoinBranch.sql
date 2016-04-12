
IF OBJECT_ID ('[dbo].[uspfn_pmEmployeeJoinBranch]') IS NOT NULL DROP PROCEDURE [dbo].[uspfn_pmEmployeeJoinBranch]
GO

-- CREATED BY Benedict 18-May-2015 LAST UPDATE 27-Aug-2015

CREATE PROCEDURE [dbo].[uspfn_pmEmployeeJoinBranch]
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@EmployeeID varchar(20),
	@LeaderID varchar(20),
	@UserID varchar(20)

AS BEGIN

--DECLARE @CompanyCode varchar(15) SET @CompanyCode = '6354401'
--DECLARE @BranchCode varchar(15) SET @BranchCode = ''
--DECLARE @EmployeeID varchar(20) SET @EmployeeID = '123456 RD'
--DECLARE @LeaderID varchar(20) SET @LeaderID = '1356'
--DECLARE @UserID varchar(20) SET @UserID = 'IT-DEV'

SET NOCOUNT ON
SET XACT_ABORT ON

DECLARE @EmpBranch varchar(15),
		@LeaderBranch varchar(15),
		@trancount int


IF (@BranchCode = '')
BEGIN
	;WITH _1 AS (
		SELECT a.CompanyCode, a.EmployeeID, MAX(a.MutationDate) AS MutationDate
		FROM HrEmployeeMutation a
		WHERE a.CompanyCode = @CompanyCode AND a.EmployeeID = @EmployeeID
		GROUP BY a.CompanyCode, a.EmployeeID
	), _2 AS (
		SELECT b.BranchCode
		FROM _1 a
		INNER JOIN HrEmployeeMutation b 
		ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate    
	)
	SELECT @EmpBranch = (SELECT BranchCode FROM _2)

	;WITH _1 AS (
		SELECT a.CompanyCode, a.EmployeeID, MAX(a.MutationDate) AS MutationDate
		FROM HrEmployeeMutation a
		WHERE a.CompanyCode = @CompanyCode AND a.EmployeeID = @LeaderID
		GROUP BY a.CompanyCode, a.EmployeeID
	), _2 AS (
		SELECT b.BranchCode
		FROM _1 a
		INNER JOIN HrEmployeeMutation b 
		ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate    
	)
	SELECT @LeaderBranch = (SELECT BranchCode FROM _2)
END 
ELSE SET @LeaderBranch = @BranchCode


SET @trancount = @@TRANCOUNT


BEGIN TRY
	IF @trancount = 0
		BEGIN TRANSACTION
	ELSE SAVE TRAN uspfn_pmEmployeeJoinBranch;

	IF(@EmpBranch <> @LeaderBranch) 
	BEGIN
		INSERT INTO HrEmployeeMutation (CompanyCode, BranchCode, EmployeeID, CreatedBy, CreatedDate, IsDeleted, IsJoinDate, MutationDate, UpdatedBy, UpdatedDate)
		VALUES (@CompanyCode, @LeaderBranch, @EmployeeID, @UserID, GETDATE(), 0, 0, GETDATE(), @UserID, GETDATE())

		UPDATE HrEmployee 
		SET TeamLeader = @LeaderID, IsAssigned = 1, UpdatedBy = @UserID, UpdatedDate = GETDATE()
		WHERE CompanyCode = @CompanyCode AND EmployeeID = @EmployeeID
	END

	DECLARE @RelatedUser varchar(15)
	SET @RelatedUser = ISNULL((SELECT RelatedUser FROM HrEmployee WHERE CompanyCode = @CompanyCode AND EmployeeID = @EmployeeID AND Department = 'SALES'), '')
	
	IF(@RelatedUser <> '') 
	BEGIN
		UPDATE SysUser
		SET BranchCode = @LeaderBranch
		WHERE UserId = @RelatedUser
	END

	SELECT * INTO #Inquiries FROM (
		SELECT ROW_NUMBER() OVER(ORDER BY InquiryNumber) AS ID, BranchCode, InquiryNumber FROM pmKDP 
		WHERE CompanyCode = @CompanyCode AND EmployeeID = @EmployeeID
	) #Inquiries

--	SELECT * FROM pmKDP WHERE CompanyCode = @CompanyCode AND EmployeeID = @EmployeeID

	DECLARE @i int SET @i = 1 
	DECLARE @inquiryNumber varchar(20)
	DECLARE @count int SET @count = (SELECT COUNT(*) FROM #Inquiries)

	WHILE (@i <= (@count))
	BEGIN
		SET @inquiryNumber = (SELECT InquiryNumber FROM #Inquiries WHERE ID = @i)
		DECLARE @OldBranch varchar(15) SET @OldBranch = (SELECT TOP 1 BranchCode FROM pmKDP WHERE InquiryNumber = @inquiryNumber AND CompanyCode = @CompanyCode AND EmployeeID = @EmployeeID)
		
		;WITH KDP1 AS (
			SELECT 
			InquiryNumber,BranchCode = @LeaderBranch
			,CompanyCode,EmployeeID
			,SpvEmployeeID = @LeaderID
			,InquiryDate,OutletID,StatusProspek,PerolehanData,NamaProspek,AlamatProspek,TelpRumah,CityID,NamaPerusahaan,AlamatPerusahaan,Jabatan,Handphone,Faximile,Email,TipeKendaraan,Variant,Transmisi,ColourCode,CaraPembayaran,TestDrive,QuantityInquiry,LastProgress,LastUpdateStatus,SPKDate,LostCaseDate,LostCaseCategory,LostCaseReasonID,LostCaseOtherReason,LostCaseVoiceOfCustomer,CreationDate,CreatedBy
			,LastUpdateBy = @UserID
			,LastUpdateDate = GETDATE()
			,Leasing,DownPayment,Tenor,MerkLain
			FROM pmKDP
			WHERE InquiryNumber = @inquiryNumber AND CompanyCode = @CompanyCode AND EmployeeID = @EmployeeID 
		)
		INSERT INTO pmKDP SELECT * FROM KDP1

		;WITH ACT1 AS (
			SELECT CompanyCode
			, BranchCode = @LeaderBranch
			, InquiryNumber, ActivityID, ActivityDate, ActivityType, ActivityDetail, NextFollowUpDate, CreationDate, CreatedBy
			, LastUpdateBy = @UserID
			, LastUpdateDate = GETDATE()
			FROM pmActivities
			WHERE CompanyCode = @CompanyCode AND InquiryNumber = @inquiryNumber
		)
		INSERT INTO pmActivities SELECT * FROM ACT1
		DELETE FROM pmActivities WHERE CompanyCode = @CompanyCode AND InquiryNumber = @inquiryNumber AND BranchCode = @OldBranch
		
		
		;WITH HIST1 AS (
			SELECT 
			InquiryNumber,CompanyCode
			,BranchCode = @LeaderBranch,SequenceNo,LastProgress
			,UpdateDate = GETDATE()
			,UpdateUser = @UserID
			FROM pmStatusHistory WHERE InquiryNumber = @InquiryNumber AND CompanyCode = @CompanyCode
		)
		INSERT INTO pmStatusHistory SELECT * FROM HIST1
		DELETE FROM pmStatusHistory WHERE InquiryNumber = @InquiryNumber AND CompanyCode = @CompanyCode AND BranchCode = @OldBranch
			
		
		;WITH ADD1 AS (
			SELECT 
			CompanyCode
			,BranchCode = @LeaderBranch
			,InquiryNumber,StatusVehicle,OthersBrand,OthersType,CreatedBy,CreatedDate
			,LastUpdateBy = @UserID
			,LastUpdateDate = GETDATE()
			FROM pmKdpAdditional WHERE CompanyCode = @CompanyCode AND InquiryNumber = @inquiryNumber
		)
		INSERT INTO pmKdpAdditional SELECT * FROM ADD1
		DELETE FROM pmKdpAdditional WHERE CompanyCode = @CompanyCode AND InquiryNumber = @inquiryNumber AND BranchCode = @OldBranch

		DELETE FROM pmKDP WHERE InquiryNumber = @inquiryNumber AND CompanyCode = @CompanyCode AND EmployeeID = @EmployeeID AND BranchCode = @OldBranch
		
		SET @i = @i + 1
	END

	SELECT * FROM pmKDP WHERE CompanyCode = @CompanyCode AND EmployeeID = @EmployeeID
	
	IF @trancount = 0 COMMIT
END TRY


BEGIN CATCH
	DECLARE @error INT, @message VARCHAR(4000), @xstate INT
	SELECT @error = ERROR_NUMBER(), @message = ERROR_MESSAGE(), @xstate = XACT_STATE();
        IF @xstate = -1
            ROLLBACK;
        IF @xstate = 1 and @trancount = 0
            ROLLBACK
        IF @xstate = 1 and @trancount > 0
            ROLLBACK TRANSACTION uspfn_pmEmployeeJoinBranch;

        RAISERROR ('uspfn_pmEmployeeJoinBranch: %d: %s', 16, 1, @error, @message) ;
	
	ROLLBACK TRAN
END CATCH
IF OBJECT_ID('tempdb..#Inquiries') IS NOT NULL DROP TABLE #Inquiries

END