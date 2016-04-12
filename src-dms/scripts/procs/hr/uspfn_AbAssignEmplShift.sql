
go
if object_id('uspfn_AbAssignEmplShift') is not null
	drop procedure uspfn_AbAssignEmplShift

go
create procedure uspfn_AbAssignEmplShift  
 @CompanyCode varchar(20),  
 @BranchCode varchar(20),  
 @Department varchar(20),  
 @Position varchar(20),   
 @DateFrom datetime,  
 @DateTo datetime,  
 @Shift varchar(50),  
 @TargetShift varchar(10),  
 @UserID varchar(20)  
as  
  
;with x as (  
select a.CompanyCode, a.AttdDate, b.Department, b.Position, a.EmployeeID, a.ShiftCode  
     , a.OnDutyTime, a.OffDutyTime, a.OnRestTime, a.OffRestTime  
     , a.UpdatedBy, a.UpdatedDate  
     , c.ShiftCode as ShiftCodeTarget  
     , c.OnDutyTime as OnDutyTimeTarget  
     , c.OffDutyTime as OffDutyTimeTarget  
     , c.OnRestTime as OnRestTimeTarget  
     , c.OffRestTime as OffRestTimeTarget  
  from HrEmployeeShift a  
  left join HrEmployee b  
    on b.CompanyCode = a.CompanyCode  
   and b.EmployeeID = a.EmployeeID  
  left join HrShift c  
    on 1=1
   and c.CompanyCode = a.CompanyCode  
   and c.ShiftCode = @TargetShift   
 where 1 = 1  
   and a.CompanyCode = @CompanyCode  
   and b.Department = @Department  
   and b.Position = (case @Position when '' then b.Position else @Position end)  
   and a.AttdDate between convert(varchar, @DateFrom, 112) and convert(varchar, @DateTo, 112)  
   and a.ShiftCode = (  
		 case   
		  when @Shift = '' then a.ShiftCode  
		  when @Shift = '-' then ''  
		  else @Shift  
		 end  
   )  
)  

--select * from x
update x set ShiftCode = @TargetShift  
           , OnDutyTime = OnDutyTimeTarget  
           , OffDutyTime = OffDutyTimeTarget  
           , OnRestTime = OnRestTimeTarget  
           , OffRestTime = OffRestTimeTarget  
           , UpdatedBy = @UserID  
           , UpdatedDate = getdate()  
  
  
  
  
  
  