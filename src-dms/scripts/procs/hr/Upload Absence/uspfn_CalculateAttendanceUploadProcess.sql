
go
if object_id('uspfn_CalculateAttendanceUploadProcess') is not null
	drop procedure uspfn_CalculateAttendanceUploadProcess


GO
CREATE procedure uspfn_CalculateAttendanceUploadProcess
as
begin
	declare @percentage decimal(5, 2);
	declare @processed decimal(5, 0);
	declare @total decimal(5, 0);

	set @processed = isnull(( select top 1 a.AttendanceFlatFileExtractionProcessed from SimDmsIterator a ), 0.00);
	set @total = ( select top 1 a.AttendanceFlatFileExtractionTotal from SimDmsIterator a );

	set @percentage = @processed * 100 / @total;

	select @percentage as Processed;
end




GO


