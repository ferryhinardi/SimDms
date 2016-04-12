CREATE PROCEDURE uspfn_gnCheckPostingStatus
AS
BEGIN
	declare @checklasttrans datetime, @retValue int, @procStatus int
	SELECT top 1 @checklasttrans=[DocDate], @procStatus = ProcessStatus 
	FROM [svSDMovement] with(nolock,nowait) order by docdate desc

	IF (@checklasttrans IS NULL)
		set @retValue=1
	ELSE
	BEGIN
		IF  (convert(varchar(10),@checklasttrans,120) <  convert(varchar(10),getdate(),120)) AND @procStatus=0
			set @retValue=0
		ELSE
			set @retValue=1
	END
	SELECT @retValue [SUCCESS], (select top 1 replace(ParamDescription, char(13),'</BR>') from sysParameter where ParamId='POSTING_STATUS') INFO

END