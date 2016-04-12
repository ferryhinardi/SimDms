--		Dengan query ini saya temuakan 1216 data yang tidak valid.
--		Harap dilakukan lagi validasi server sidenya, agar ini tidak jadi boomerang ke kita.
--		
--		Yang mesti di patch di dealer :
--			Car idi HREmployee yang personel statusnya = 1
--			Jika JoinDate = ResignDate -> ResignDate = null
--			Jika ResignDate > CurrentDate -> ResignDate = null
--			Jika JoinDate < ResignDate -> PersonnelStatus = 3
--			Check lagi di table HrEmployee
--			Jika ResignDescription ada -> 3
--			Jika ResignDescription tidak ada -> ResignDate = null
--		
--		Yang perlu diperhatikan juga, ketiksa “SAVE” dari Controller perhatikan nilai “ResignDate” dan “PersonnelStatus”


begin transaction

;with x as (
select a.CompanyCode, a.EmployeeID, a.EmployeeName, a.JoinDate, a.ResignDate, a.PersonnelStatus
  from HrEmployee a
 where 1 = 1
   and a.PersonnelStatus = '1'
   and a.ResignDate is not null
   --and a.JoinDate != a.ResignDate
)
select * from x

rollback transaction

