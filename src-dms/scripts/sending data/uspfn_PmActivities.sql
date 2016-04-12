
go
if object_id('uspfn_PmActivities') is not null
	drop procedure uspfn_PmActivities

go
create procedure uspfn_PmActivities 
	@LastUpdateDate datetime,
	@Segment int
as

select * into #t1 from (
select top (@Segment) CompanyCode, BranchCode, InquiryNumber, ActivityID, ActivityDate, ActivityType, ActivityDetail, NextFollowUpDate, CreationDate, CreatedBy, LastUpdateBy, LastUpdateDate
  from pmActivities
 where LastUpdateDate is not null
   and LastUpdateDate > @LastUpdateDate
 order by LastUpdateDate asc )#t1
 
declare @LastUpdateQry datetime
    set @LastUpdateQry = (select top 1 LastUpdateDate from #t1 order by LastUpdateDate desc)

select * from #t1
 union
select CompanyCode, BranchCode, InquiryNumber, ActivityID, ActivityDate, ActivityType, ActivityDetail, NextFollowUpDate, CreationDate, CreatedBy, LastUpdateBy, LastUpdateDate
  from pmActivities
 where LastUpdateDate = @LastUpdateQry
 
  drop table #t1

--go

--uspfn_PmActivities @LastUpdateDate='2013-10-10 00:00:00',@Segment=500

