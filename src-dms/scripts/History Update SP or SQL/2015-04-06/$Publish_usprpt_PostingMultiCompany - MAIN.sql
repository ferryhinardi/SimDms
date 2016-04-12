-- POSTING TRANSACTION MULTI COMPANY - MAIN PROCESS
-- ---------------------------------------------------------------
-- Created  by : HTO, 2014
-- Revision by : HTO, January 2015
-- ---------------------------------------------------------------
-- This procedure used for 
--		4-Wheeler : BIT-SBT, SBSM-SSBT, BAT-SAT, SIT-SST , UIB-SIT
--		2-Wheeler : IJMG,SVMG,ISG,ISMG-SMG
-- -------------------------------------------------------------------------------
-- execute [usprpt_PostingMultiCompanyMainProcess] '6006400001','2014/11/14','HTO'
-- update sysParaMeter set ParamValue='2014/11/01' where ParamId='POSTING_STATUS'
-- -------------------------------------------------------------------------------

ALTER procedure [dbo].[usprpt_PostingMultiCompanyMainProcess]
	@CompanyCode	varchar(15),
	@PostingDate	datetime,
	@UserId			varchar(20)
AS	

BEGIN TRANSACTION
BEGIN TRY

BEGIN
 -- Check Posting Multi Company Date in table sysParameter
	declare @PostDate	varchar(10)
	declare @PostStatus	integer
	set @PostDate   = (select ParamValue from sysParaMeter where ParamId='POSTING_STATUS')
	set @PostStatus = (case when @PostDate is null                             then 0
	                        when @PostDate < convert(varchar,@PostingDate,111) then 1
					        else                                                    2
					   end)
    if (select 1 from sysParaMeter where ParamId='POSTING_STATUS')=1 and @PostStatus=0
	    set @PostStatus=1

	if @PostStatus = 0
		insert sysParaMeter values('POSTING_STATUS',convert(varchar,@PostingDate,111),'Posting Multi Company')
	else
		if @PostStatus = 1
			update sysParaMeter set ParamValue=convert(varchar,@PostingDate,111) where ParamId='POSTING_STATUS'
		else
			begin
				select '0' [STATUS], 'Daily Posting tertanggal ' + convert(varchar,@PostingDate,106) + ' sudah pernah dilakukan sebelumnya....' [INFO]
				return
			end

	declare @Status	varchar(1)

	execute [usprpt_PostingMultiCompanySales] @CompanyCode, @PostingDate, @Status OUTPUT
	if @Status = '1'
		begin
			select '0' [STATUS], 'Daily Posting Sales fail...' [INFO]
			return
		end

	execute [usprpt_PostingMultiCompanySalesReturn] @CompanyCode, @PostingDate, @Status OUTPUT
	if @Status = '1'
		begin
			select '0' [STATUS], 'Daily Posting Sales Return fail...' [INFO]
			return
		end


	execute [usprpt_PostingMultiCompanySparepartService] @CompanyCode, @PostingDate, @Status OUTPUT
	if @Status = '1'
		begin
			select '0' [STATUS], 'Daily Posting SparePart & Service fail...' [INFO]
			return
		end
END		

END TRY

BEGIN CATCH
    --select ERROR_NUMBER()    AS ErrorNumber,    ERROR_SEVERITY() AS ErrorSeverity, ERROR_STATE()   AS ErrorState,
		  -- ERROR_PROCEDURE() AS ErrorProcedure, ERROR_LINE()     AS ErrorLine    , ERROR_MESSAGE() AS ErrorMessage
	if @@TRANCOUNT > 0
		begin
			select '0' [STATUS], 'Posting gagal !!!   ' + ERROR_MESSAGE() [INFO]
			rollback transaction
			return
		end
END CATCH

IF @@TRANCOUNT > 0
	begin
		select '1' [STATUS], 'Posting berhasil !!!' [INFO]
		--rollback transaction
		commit transaction
	end

