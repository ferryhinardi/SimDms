go
if object_id('HrTrnAttendanceFileHdrView') is not null
	drop view HrTrnAttendanceFileHdrView

go
create view HrTrnAttendanceFileHdrView
as
select 
	a.CompanyCode,
	a.FileID,
	a.IsTransfered,
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
	HrTrnAttendanceFileHdr a
inner join
	HrUploadedFile b
on
	a.FileID = b.Checksum

go
select * from HrTrnAttendanceFileHdrView