IF OBJECT_ID ('[dbo].[uspfn_pmEmployeeJoinBranch]') IS NOT NULL DROP PROCEDURE [dbo].[uspfn_pmEmployeeJoinBranch]
GO
-- CREATED BY Benedict 18-May-2015 LAST UPDATE 15-Sep-2015

CREATE PROCEDURE [dbo].[uspfn_pmEmployeeJoinBranch]
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@EmployeeID varchar(20),
	@LeaderID varchar(20),
	@UserID varchar(20)
AS

--DECLARE @CompanyCode varchar(15) SET @CompanyCode = '6006406'
--DECLARE @BranchCode varchar(15) SET @BranchCode = ''
--DECLARE @EmployeeID varchar(20) SET @EmployeeID = '52470'
--DECLARE @LeaderID varchar(20) SET @LeaderID = '3'
--DECLARE @UserID varchar(20) SET @UserID = 'BENT'
	
	
SET NOCOUNT ON
SET XACT_ABORT ON

DECLARE @EmpBranch varchar(15),
		@LeaderBranch varchar(15),
		@ErrorNo int
		
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
ELSE 
BEGIN 
	SELECT @LeaderBranch = @BranchCode, @EmpBranch = @BranchCode
END

BEGIN TRY
	BEGIN TRANSACTION

		IF(@EmpBranch <> @LeaderBranch) 
		BEGIN
			INSERT INTO HrEmployeeMutation (CompanyCode, BranchCode, EmployeeID, CreatedBy, CreatedDate, IsDeleted, IsJoinDate, MutationDate, UpdatedBy, UpdatedDate)
			VALUES (@CompanyCode, @LeaderBranch, @EmployeeID, @UserID, GETDATE(), 0, 0, GETDATE(), @UserID, GETDATE())
		END
	
		UPDATE HrEmployee 
		SET TeamLeader = @LeaderID, IsAssigned = 1, UpdatedBy = @UserID, UpdatedDate = GETDATE()
		WHERE CompanyCode = @CompanyCode AND EmployeeID = @EmployeeID

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

		DECLARE @i int SET @i = 1 
		DECLARE @inquiryNumber varchar(20)
		DECLARE @count int SET @count = (SELECT COUNT(*) FROM #Inquiries)
	
		DECLARE @KDPBranchCount INT
		    SET @KDPBranchCount = (SELECT COUNT(*) FROM (SELECT BranchCode FROM pmKDP WHERE BranchCode = @EmpBranch AND CompanyCode = @CompanyCode AND EmployeeID = @EmployeeID GROUP BY BranchCode) a)
		IF (@KDPBranchCount > 1) RAISERROR(N'Ada data KDP dobel pada employee emp_id %s, hubungi IT Support untuk bantuan', 15, -1, @EmployeeID)

		DECLARE @ActBranchCount INT
		    SET @ActBranchCount = (SELECT COUNT(*) FROM (SELECT BranchCode FROM pmKdpAdditional WHERE InquiryNumber IN (SELECT InquiryNumber FROM #Inquiries) AND BranchCode = @EmpBranch AND CompanyCode = @CompanyCode GROUP BY BranchCode) a)
		IF (@ActBranchCount > 1) RAISERROR(N'Ada data Activity dobel pada employee emp_id %s, hubungi IT Support untuk bantuan', 15, -1, @EmployeeID)

		DECLARE @HistBranchCount INT
		    SET @HistBranchCount = (SELECT COUNT(*) FROM (SELECT BranchCode FROM pmStatusHistory WHERE InquiryNumber IN (SELECT InquiryNumber FROM #Inquiries) AND BranchCode = @EmpBranch AND CompanyCode = @CompanyCode GROUP BY BranchCode) a)
		IF (@HistBranchCount > 1) RAISERROR(N'Ada data Status History dobel pada employee emp_id %s, hubungi IT Support untuk bantuan', 15, -1, @EmployeeID)

		DECLARE @AddBranchCount INT
			SET @AddBranchCount = (SELECT COUNT(*) FROM (SELECT BranchCode FROM pmKdpAdditional WHERE InquiryNumber IN (SELECT InquiryNumber FROM #Inquiries) AND BranchCode = @EmpBranch AND CompanyCode = @CompanyCode GROUP BY BranchCode) a)
		IF (@AddBranchCount > 1) RAISERROR(N'Ada data Additional KDP dobel pada employee emp_id %s, hubungi IT Support untuk bantuan', 15, -1, @EmployeeID)

		DECLARE @KDPBranchCode VARCHAR(15)
		    SET @KDPBranchCode = (SELECT DISTINCT BranchCode FROM pmKDP WHERE BranchCode = @EmpBranch AND CompanyCode = @CompanyCode AND EmployeeID = @EmployeeID )

		--SELECT @KdpBranchCount KdpBranchCount, @ActBranchCount ActBranchCount, @HistBranchCount HistBranchCount, @AddBranchCount AddBranchCount

		IF(@KDPBranchCode <> @LeaderBranch)
		BEGIN
			WHILE (@i <= (@count))
			BEGIN
				SET @inquiryNumber = (SELECT InquiryNumber FROM #Inquiries WHERE ID = @i)
				DECLARE @OldBranch varchar(15) SET @OldBranch = (SELECT TOP 1 BranchCode FROM pmKDP WHERE InquiryNumber = @inquiryNumber AND CompanyCode = @CompanyCode AND EmployeeID = @EmployeeID)
		
				;WITH KDP1 AS (
					SELECT 
					InquiryNumber,BranchCode = @LeaderBranch
					,CompanyCode,EmployeeID
					,SpvEmployeeID = @LeaderID
					,InquiryDate,OutletID,StatusProspek,PerolehanData,NamaProspek,AlamatProspek,TelpRumah,CityID,NamaPerusahaan,AlamatPerusahaan,Jabatan,Handphone,Faximile,Email,TipeKendaraan,Variant,Transmisi,ColourCode,CaraPembayaran,TestDrive,QuantityInquiry
					,LastProgress,LastUpdateStatus,SPKDate,LostCaseDate,LostCaseCategory,LostCaseReasonID,LostCaseOtherReason,LostCaseVoiceOfCustomer,CreationDate,CreatedBy
					,LastUpdateBy = @UserID
					,LastUpdateDate = GETDATE()
					,Leasing,DownPayment,Tenor,MerkLain
					FROM pmKDP
					WHERE InquiryNumber = @inquiryNumber AND CompanyCode = @CompanyCode AND EmployeeID = @EmployeeID 
				)
				INSERT INTO pmKDP SELECT * FROM KDP1
				
				-- Update by Rijal 11/11/2015
				insert INTO pmLogTransferHistoryByBranch
				VALUES (@CompanyCode, @KDPBranchCode, @LeaderBranch, @inquiryNumber, @EmployeeID, GETDATE(), @UserID)
				-----------------------------
				
				ALTER TABLE pmActivities NOCHECK CONSTRAINT RefpmKDP1
				UPDATE pmActivities
				SET BranchCode = @LeaderBranch
				WHERE CompanyCode = @CompanyCode AND InquiryNumber = @inquiryNumber
				ALTER TABLE pmActivities CHECK CONSTRAINT RefpmKDP1
				
				ALTER TABLE pmStatusHistory NOCHECK CONSTRAINT RefpmKDP35
				UPDATE pmStatusHistory
				SET BranchCode = @LeaderBranch
				WHERE InquiryNumber = @inquiryNumber AND CompanyCode = @CompanyCode
				ALTER TABLE pmStatusHistory CHECK CONSTRAINT RefpmKDP35

				UPDATE pmKdpAdditional
				SET BranchCode = @LeaderBranch
				WHERE CompanyCode = @CompanyCode AND InquiryNumber = @inquiryNumber

				DELETE FROM pmKDP WHERE InquiryNumber = @inquiryNumber AND CompanyCode = @CompanyCode AND EmployeeID = @EmployeeID AND BranchCode = @OldBranch
		
				SET @i = @i + 1
			END
		END	
	COMMIT TRANSACTION

END TRY BEGIN CATCH
	DECLARE @error INT, @message VARCHAR(4000), @xstate INT
	SELECT @error = ERROR_NUMBER(), @message = ERROR_MESSAGE(), @xstate = XACT_STATE();
    
	ROLLBACK TRAN
    RAISERROR ('SP Error: %d: %s', 16, 1, @error, @message) ;
END CATCH
IF OBJECT_ID('tempdb..#Inquiries') IS NOT NULL DROP TABLE #Inquiries

