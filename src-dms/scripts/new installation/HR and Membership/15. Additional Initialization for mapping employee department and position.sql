-- SCRIPT I DARI PAK WAWAN

;with x as (
select a.CompanyCode, a.EmployeeID, a.EmployeeName, UpdatedDate
     , a.Department, a.Position, a.Grade, a.Rank, a.TeamLeader
     , DepartmentX = isnull((select top 1 Department from gnMstEmployeeData where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID order by UpdatedDate desc), '')
     , PositionX = isnull((select top 1 PosCode from gnMstEmployeeData where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID order by UpdatedDate desc), '')
     , GradeX = isnull((select top 1 Grade from gnMstEmployeeData where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID order by UpdatedDate desc), '')
     , RankX = isnull((select top 1 RankCode from gnMstEmployeeData where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID order by UpdatedDate desc), '')
  from HrEmployee a
)
--select * from x where DepartmentX != ''
update x set Department = Department, Position = PositionX, Grade = GradeX, Rank = RankX where DepartmentX != ''

;with x as (select * from HrEmployee where isnull(Department, '') != '')
update x set UpdatedDate = getdate()





-- SCRIPT II DARI PAK WAWAN

insert into HrEmployeeMutation (CompanyCode, EmployeeID, MutationDate, BranchCode, IsJoinDate, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsDeleted)
select a.CompanyCode, a.EmployeeID
     , a.JoinDate as MutationDate
     , BranchCode = isnull((select top 1 BranchCode from gnMstEmployee where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and BranchCode != '' order by UpdatedDate desc), '')
     , IsJoinDate = 1
     , CreatedBy = 'konversi'
     , CreatedDate = getdate()
     , UpdatedBy = 'konversi'
     , UpdatedDate = getdate()
     , IsDeleted = 0
  from HrEmployee a
 where 1 = 1
   and Department = 'SALES'
   and not exists (
	select EmployeeID
	  from HrEmployeeMutation
	 where CompanyCode = a.CompanyCode
	   and EmployeeID = a.EmployeeID
	   and convert(varchar, MutationDate, 112) = convert(varchar, a.JoinDate, 112)
	   and IsJoinDate = 1) 
 union 
select CompanyCode, EmployeeID, MutationDate, BranchCode, IsJoinDate, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsDeleted
  from HrEmployeeMutation where IsJoinDate = 0





--SCRIPT III DARI PAK WAWAN

insert into HrEmployeeAchievement (CompanyCode, EmployeeID, AssignDate, Department, Position, Grade, IsJoinDate, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsDeleted)
select a.CompanyCode, a.EmployeeID
     , a.JoinDate as AssignDate
     , Department = isnull((select top 1 Department from gnMstEmployee where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and BranchCode != '' order by UpdatedDate desc), '')
     , Psition = isnull((select top 1 Position from gnMstEmployee where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and BranchCode != '' order by UpdatedDate desc), '')
     , Grade = isnull((select top 1 Grade from gnMstEmployee where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and BranchCode != '' order by UpdatedDate desc), '')
     , IsJoinDate = 1
     , CreatedBy = 'konversi'
     , CreatedDate = getdate()
     , UpdatedBy = 'konversi'
     , UpdatedDate = getdate()
     , IsDeleted = 0
  from HrEmployee a
 where 1 = 1
   and Department = 'SALES'
   and not exists (
	select EmployeeID
	  from HrEmployeeAchievement
	 where CompanyCode = a.CompanyCode
	   and EmployeeID = a.EmployeeID
	   and convert(varchar, AssignDate, 112) = convert(varchar, a.JoinDate, 112)
	   and IsJoinDate = 1) 
 union all
select top 0 CompanyCode, EmployeeID, AssignDate, Department, Position, Grade, IsJoinDate, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsDeleted
  from HrEmployeeAchievement where IsJoinDate = 1





--SCRIPT IV DARI PAK WAWAN

;with x as (
select a.CompanyCode
     , a.EmployeeID
     , SalesID = isnull((select top 1 SalesID from GnMstEMployeeData where CompanyCode = a.CompanyCode and EmployeeID = a.EMployeeID and SalesID != ''), '')
     , a.CreatedBy
     , CreatedDate = getdate()
  from HrEmployee a
 where not exists (select 1 from HrEmployeeSales where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID)
)
insert into HrEmployeeSales (CompanyCode, EmployeeID, SalesID, CreatedBy, CreatedDate)
select * from x where SalesID != ''






