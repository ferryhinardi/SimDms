
go
if object_id('uspfn_PmStatusHistory') is not null
	drop procedure uspfn_PmStatusHistory

go
create procedure uspfn_PmStatusHistory 
	@LastUpdateDate datetime,
	@Segment int
as

select * into #t1 from (
select top (@Segment) InquiryNumber, CompanyCode, BranchCode, SequenceNo, LastProgress, UpdateDate, UpdateUser
  from pmStatusHistory
 where UpdateDate is not null
   and UpdateDate > @LastUpdateDate
 order by UpdateDate asc )#t1
 
declare @LastUpdateQry datetime
    set @LastUpdateQry = (select top 1 UpdateDate from #t1 order by UpdateDate desc)

select * from #t1
 union
select InquiryNumber, CompanyCode, BranchCode, SequenceNo, LastProgress, UpdateDate, UpdateUser
  from pmStatusHistory
 where UpdateDate = @LastUpdateQry
 
  drop table #t1

--go

--uspfn_pmStatusHistory @LastUpdateDate='2013-10-10 00:00:00',@Segment=500

