
go
if object_id('HrTrnAttendanceFileHdrView') is not null
	drop view HrTrnAttendanceFileHdrView

GO
create view HrTrnAttendanceFileHdrView
as
select 
	a.CompanyCode,
	a.FileID,
	IsTransfered = (
		case
			when a.IsTransfered='0' then 'Unprocessed'
			when a.IsTransfered='1' then 'Half Processed'
			when a.IsTransfered='2' then 'Processed'
		end
	),
	b.FileName,
	b.FileSize,
	b.FileType,
	Size = '',
	CreatedDate = b.UploadedDate,
	Unprocessed = (
		select 
			count(x.AttendanceTime)
		from 
			HrTrnAttendanceFileDtl x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.FileID = a.FileID
			and
			x.IsTransfered = 0
	),
	Processed = (
		select 
			count(x.AttendanceTime)
		from 
			HrTrnAttendanceFileDtl x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.FileID = a.FileID
			and
			x.IsTransfered = 1
	)
from 
	SDMS.dbo.HrTrnAttendanceFileHdr a
inner join
	sdms_document.dbo.HrAbsenceFile b
on
	a.FileID = b.FileID

GO


select * from [HrTrnAttendanceFileHdrView]
