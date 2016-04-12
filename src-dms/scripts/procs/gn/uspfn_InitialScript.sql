
if object_id('uspfn_InitialScript') is not null
	drop procedure uspfn_InitialScript

go
create procedure uspfn_InitialScript
as
;with x as (
select a.UserId, a.Password, a.FullName, a.IsActive, a.RequiredChange
     , LastChanged = (isnull((select top 1 ChangeDate from SysUserPwdHist where UserId = a.UserId order by ChangeDate desc), '1900-01-01'))
	 , DelayDays = datediff(DAY, (isnull((select top 1 ChangeDate from SysUserPwdHist where UserId = a.UserId order by ChangeDate desc), '1900-01-01')), getdate())
  from SysUser a
 where 1 = 1
   and UserId not in ('sa', 'ga')
   and Isnull(RequiredChange, 0) = 0
   and (datediff(DAY, (isnull((select top 1 ChangeDate from SysUserPwdHist where UserId = a.UserId order by ChangeDate desc), '1900-01-01')), getdate())) > 90
)
--select * from x
update x set RequiredChange = 1



