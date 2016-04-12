alter procedure uspfn_PmStatusHistorySend
--declare 
	@LastUpdateDate datetime,
	@Segment int  
as  
--select @LastUpdateDate='1990-01-01 00:00:00',@Segment=500  
  
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
select top 100 InquiryNumber, CompanyCode, BranchCode, SequenceNo, LastProgress, UpdateDate, UpdateUser
  from pmStatusHistory  
 where UpdateDate = @LastUpdateQry  
   
  drop table #t1  