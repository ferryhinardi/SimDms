update gnMstPosition 
set DeptCode = 'SALES', UpdatedDate = getdate()
where DeptCode = 'SALES & MKT' and PosCode not in (select PosCode from gnMstPosition where DeptCode = 'SALES')

update HrEmployee 
set Department = 'SALES', UpdatedDate = getdate()
where Department = 'SALES & MKT'

update HrEmployeeTemp 
set Department = 'SALES'
where Department = 'SALES & MKT'

update HrEmployeeAdditionalJob 
set Department = 'SALES', UpdatedDate = getdate()
where Department = 'SALES & MKT'

update HrDepartmentTraining 
set Department = 'SALES', UpdatedDate = getdate()
where Department = 'SALES & MKT'

update HrEmployeeAchievement 
set Department = 'SALES', UpdatedDate = getdate()
where Department = 'SALES & MKT'

update SysHrEmployeeTemp 
set Department = 'SALES' 
where Department = 'SALES & MKT'

delete from [dbo].[gnMstOrgGroup]
where OrgGroupCode='DEPT' and OrgCode='SALES & MKT'