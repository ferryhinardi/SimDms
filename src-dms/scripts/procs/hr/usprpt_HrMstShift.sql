
go
if object_id('usprpt_HrMstShift') is not null
	drop procedure usprpt_HrMstShift

go
create procedure usprpt_HrMstShift
	@CompanyCode varchar(50)

as

select CompanyCode, ShiftCode, ShiftName
     , OnDutyTime, OffDutyTime
     , OnRestTime, OffRestTime
     , WorkingHour, IsActive
  from HrShift
 where CompanyCode = @CompanyCode
