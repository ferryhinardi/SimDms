if object_id('usprpt_PostingMultiCompany4DocNo') is not null
	drop procedure usprpt_PostingMultiCompany4DocNo
GO
-- POSTING TRANSACTION MULTI COMPANY - DOCUMENT NUMBER
-- --------------------------------------------------------------
-- Created  by : HTO, 2014
-- Revision by : HTO, January 2015
-- --------------------------------------------------------------
-- This procedure used for 
--		4-Wheeler : BIT-SBT, SBSM-SSBT, BAT-SAT, SIT-SST, UIB-SIT
--		2-Wheeler : IJMG,SVMG,ISG,ISMG-SMG
-- -------------------------------------------------------------------------------------------------
-- declare @DocNo varchar(15)  
-- execute [usprpt_PostingMultiCompany4DocNo] '6006400001','6006400131','SBTSBY','INV',@DocNo output
-- -------------------------------------------------------------------------------------------------

CREATE procedure [dbo].[usprpt_PostingMultiCompany4DocNo]
	@Company	varchar(15),
	@Branch		varchar(15),
	@DBName		varchar(50),
	@DocID		varchar(15),
	@DocNo		varchar(15) output
AS	

--BEGIN TRANSACTION
--BEGIN TRY

BEGIN
 -- Document Number sequence
  --declare @Company		varchar(15)  = '6006400001'
  --declare @Branch			varchar(15)  = '6006400131'
  --declare @DBName			varchar(50)  = 'SBTSBY'
  --declare @DocID			varchar(15)  = 'INV'
  --declare @DocNo			varchar(15)
	declare @DocPrefix		varchar(15)
	declare @DocYear		integer
	declare @DocSeq			integer
	declare @sqlString		nvarchar(max)

	set @sqlString = N'select @DocPrefix=DocumentPrefix, @DocYear=DocumentYear, @DocSeq=DocumentSequence from '+@DBName+'..gnMstDocument ' +
						'where CompanyCode='''+@Company+''' and BranchCode='''+@Branch+''' and DocumentType='''+@DocID+''''

		execute sp_executesql @sqlString, 
			N'@DocPrefix varchar(15) output, @DocYear integer output, @DocSeq integer output', @DocPrefix output, @DocYear output, @DocSeq output

	set @DocSeq = @DocSeq + 1
	set @sqlString = 'update ' +@DBName+ '..gnMstDocument' +
						' set DocumentSequence = ' +convert(varchar,@DocSeq)+ 
						' where CompanyCode=''' +@Company+ ''' and BranchCode=''' +@Branch+ ''' and DocumentType='''+@DocID+''''
		execute sp_executesql @sqlString 

	set @DocNo = @DocPrefix + '/' + right(convert(varchar,@DocYear),2) + '/' + 
					replicate('0',6-len(convert(varchar,@DocSeq))) + convert(varchar,@DocSeq)
/*
END TRY

BEGIN CATCH
    select ERROR_NUMBER()    AS ErrorNumber,    ERROR_SEVERITY() AS ErrorSeverity, ERROR_STATE()   AS ErrorState,
		   ERROR_PROCEDURE() AS ErrorProcedure, ERROR_LINE()     AS ErrorLine    , ERROR_MESSAGE() AS ErrorMessage
	if @@TRANCOUNT > 0
		rollback transaction
	select '0' [STATUS], 'Posting fail !!!' [INFO]
	return
END CATCH

IF @@TRANCOUNT > 0
	begin
		select '1' [STATUS], 'Posting Done !!!' [INFO]
		--rollback transaction
		commit transaction
	end
*/
END