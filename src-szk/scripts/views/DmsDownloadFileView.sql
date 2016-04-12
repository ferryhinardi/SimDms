
go
if object_id('DmsDownloadFileView') is not null
	drop view DmsDownloadFileView

go
create view DmsDownloadFileView
as
select a.*
     , b.DealerName AS DEALER_NAME
	 , b.ShortName AS SHORT_NAME
  from DMS_DOWNLOAD a
  left join DealerInfo b
	on a.CUSTOMER_CODE = b.DealerCode


go
select * from DmsDownloadFileView