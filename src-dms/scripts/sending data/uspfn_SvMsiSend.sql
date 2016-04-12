
go
if object_id('uspfn_SvMsiSend') is not null
	drop procedure uspfn_SvMsiSend

go
create procedure uspfn_SvMsiSend 
	@LastUpdateDate datetime,
	@Segment int

--select @LastUpdateDate='1990-01-01 00:00:00',@Segment=500
as

select * into #t1 from (
select top (@Segment) CompanyCode, BranchCode, PeriodYear, PeriodMonth, SeqNo, MsiGroup, MsiDesc, Unit, MsiData, CreatedBy, CreatedDate
  from svHstSzkMSI
 where CreatedDate is not null
   and CreatedDate > @LastUpdateDate
 order by CreatedDate asc )#t1
 
declare @LastUpdateQry datetime
    set @LastUpdateQry = (select top 1 CreatedDate from #t1 order by CreatedDate desc)

select * from #t1
 union
select CompanyCode, BranchCode, PeriodYear, PeriodMonth, SeqNo, MsiGroup, MsiDesc, Unit, MsiData, CreatedBy, CreatedDate
  from svHstSzkMSI
 where CreatedDate = @LastUpdateQry
 
  drop table #t1
