
go
if object_id('DmsUploadFileView') is not null
	drop view DmsUploadFileView

go
create view DmsUploadFileView
as
select a.*
     , b.DealerName AS DEALER_NAME
	 , b.ShortName AS SHORT_NAME
  from DMS_UPLOAD a
  left join DealerInfo b
	on a.CUSTOMER_CODE_BILLING = b.DealerCode


go
select * from DmsUploadFileView