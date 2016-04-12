if object_id('uspfn_HrListEmployeeSalesSend') is not null
	drop procedure uspfn_HrListEmployeeSalesSend

go
create procedure uspfn_HrListEmployeeSalesSend 
	@LastUpdateDate datetime,
	@Segment int

--select @LastUpdateDate='1990-01-01 00:00:00',@Segment=500
as

select * into #t1 from (
select top (@Segment) CompanyCode, EmployeeID, SalesID, CreatedBy, CreatedDate
  from HrEmployeeSales
 where CreatedDate is not null
   and CreatedDate > @LastUpdateDate
 order by CreatedDate asc )#t1
 
declare @LastUpdateQry datetime
    set @LastUpdateQry = (select top 1 CreatedDate from #t1 order by CreatedDate desc)

select * from #t1
 union
select CompanyCode, EmployeeID, SalesID, CreatedBy, CreatedDate
  from HrEmployeeSales
 where CreatedDate = @LastUpdateQry
 
  drop table #t1
