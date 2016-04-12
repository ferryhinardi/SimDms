if object_id('uspfn_gnCheckPostingStatus') is not null
	drop procedure uspfn_gnCheckPostingStatus
GO
CREATE PROCEDURE uspfn_gnCheckPostingStatus
AS
BEGIN
	declare @checklasttrans datetime, @retValue int, @tax int,
			@procStatus int,@retValue2 int, @prmValue varchar(20),
			@sql nvarchar(2000)	

	select @prmValue=ParamValue from sysParameter where paramid='SPSRV'
	
	declare @dbmd varchar(20)
	select top 1 @dbmd=dbmd from gnMstCompanyMapping

	set @tax = 1

	IF @prmValue='ON'
	BEGIN

		set @sql = 'SELECT top 1 @checklasttrans=[DocDate], @procStatus = ProcessStatus 
		FROM ' + @dbmd + '..[svSDMovement] with(nolock,nowait) order by docdate desc'

		exec sp_Executesql @sql, N'@checklasttrans datetime output, @procStatus int output', @checklasttrans output, @procStatus output
	
		IF (@checklasttrans IS NULL)
			set @retValue=1
		ELSE
		BEGIN
			IF  (convert(varchar(10),@checklasttrans,120) <  convert(varchar(10),getdate(),120)) AND @procStatus=0
				select @retValue=0, @tax = 0
			ELSE
				set @retValue=1
		END
	END
	ELSE
		set @retValue=2

	SELECT @checklasttrans = NULL, @procStatus = 0

	select @prmValue=ParamValue from sysParameter where paramid='SLS'
	IF @prmValue='ON'
	BEGIN

		set @sql = 'SELECT top 1 @checklasttrans=[DocDate], @procStatus = ProcessStatus 
		FROM  ' + @dbmd + '..[omSDMovement] with(nolock,nowait) order by docdate desc'

		exec sp_Executesql @sql, N'@checklasttrans datetime output, @procStatus int output', @checklasttrans output, @procStatus output

		IF (@checklasttrans IS NULL)
			set @retValue2=1
		ELSE
		BEGIN
			IF  (convert(varchar(10),@checklasttrans,120) <  convert(varchar(10),getdate(),120)) AND @procStatus=0
				BEGIN
					select @retValue2=0, @tax = 0	
					IF @retValue <> 2
						set @retValue=0
				END
			ELSE
				BEGIN
					set @retValue2=1
				END
		END
		
		IF @retValue2=1 and @retValue=0 
			set @retValue2=0
		
	END
	ELSE
		set @retValue2=1
		
	IF @retValue = 2
		set @retValue=1
		
	SELECT @retValue [SPSRV], @retValue2 [SALES], 
	(select top 1 replace(ParamDescription, char(13),'</BR>') from sysParameter where ParamId='POSTING_STATUS') INFO, 
	(select top 1 case when ISNULL(ParamValue,'OFF')='OFF' then 1 else @tax end from sysParameter where paramid='PAJAK') TAX

END
