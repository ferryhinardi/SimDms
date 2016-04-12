
go
if object_id('uspfn_HrGetEducationByEmployeeId') is not null
	drop procedure uspfn_HrGetEducationByEmployeeId

go
create procedure uspfn_HrGetEducationByEmployeeId 
	@CompanyCode varchar(25),
	@EmployeeID varchar(25)
as
select EmployeeID
     , EduSeq
     , College
     , Education
     , YearBegin
     , YearFinish
  from HrEmployeeEducation
 where CompanyCode = @CompanyCode
   and EmployeeID = @EmployeeID
 order by yearbegin asc