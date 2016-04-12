
go
if object_id('usprpt_HrMstEmployee') is not null
	drop procedure usprpt_HrMstEmployee

go
create procedure usprpt_HrMstEmployee
	@CompanyCode varchar(50),
	@HolidayYear int

as
select CompanyCode, HolidayYear, HolidayCode, HolidayDesc, DateFrom, DateTo
     , case IsHoliday when 1 then 'Y' else 'N' end IsHoliday
  from HrHoliday
 where CompanyCode = @CompanyCode
   and HolidayYear = @HolidayYear
