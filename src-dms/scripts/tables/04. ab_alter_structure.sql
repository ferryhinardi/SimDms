
go
alter table HrEmployee 
  add IsDeleted bit not null default 0

go
alter table HrEmployeeAchievement 
  add IsDeleted bit not null default 0	

go
alter table HrEmployeeAdditionalBranch 
  add IsDeleted bit not null default 0

go
alter table HrEmployeeAdditionalJob 
  add IsDeleted bit not null default 0
  
go
alter table HrEmployeeEducation 
  add IsDeleted bit not null default 0

go
alter table HrEmployeeExperience 
  add IsDeleted bit not null default 0

go
alter table HrEmployeeMutation 
  add IsDeleted bit not null default 0

go
alter table HrEmployeeSales 
  add IsDeleted bit not null default 0

go
alter table HrEmployeeService 
  add IsDeleted bit not null default 0

go
alter table HrEmployeeShift 
  add IsDeleted bit not null default 0

go
alter table HrEmployeeTraining 
  add IsDeleted bit not null default 0

go
alter table HrEmployeeVehicle 
  add IsDeleted bit not null default 0

go
alter table HrEmployeeVehicle 
  add IsDeleted bit not null default 0

go
alter table HrHoliday 
  add IsDeleted bit not null default 0

go
alter table HrLookupMapping
  add IsDeleted bit not null default 0

go
alter table HrMstTraining
  add IsDeleted bit not null default 0

go
alter table HrSetting
  add IsDeleted bit not null default 0

go
alter table HrShift
  add IsDeleted bit not null default 0

go
alter table HrTrnAttendanceFileDtl
  add IsDeleted bit not null default 0

go
alter table HrTrnAttendanceFileHdr
  add IsDeleted bit not null default 0

go
alter table HrUploadedFile
  add IsDeleted bit not null default 0

go
alter table HrDepartmentTraining
  add IsDeleted bit not null default 0

go
alter table HrEmployee 
  add ResignCategory varchar(25) null
 
go 
alter table SysRole   
  add IsChangeBranchCode bit default 0

go
alter table HrTrnAttendanceFileDtl	
alter column FileID varchar(64) null 

go
alter table HrEmployeeSales
add UpdatedBy varchar(50) null

go
alter table SysRole
  add IsActive bit null default 0
 
go
alter table SysRole
  add IsAdmin bit null default 0

go
alter table SysRole
  add Themes varchar(50) null default ''

go
alter table HrEmployeeSales
  add UpdatedDate datetime null

go
alter table HrEmployee 
  add IsDeleted bit null default 0

go
alter table HrEmployeeAchievement 
  add IsDeleted bit null default 0	

go
alter table HrEmployeeAchievement 
alter column IsDeleted bit null 

go
alter table HrEmployeeMutation 
  add IsDeleted bit null default 0

go
alter table HrEmployeeTraining
  add IsDeleted bit null default 0

go
alter table SysRole
  add IsChangeBranchCode bit null default 0

go
alter table gnMstPosition
  add IsGrade bit null 

go
alter table SysUser
  add RequiredChange bit null default 0

go
alter table HrEmployeeSales
  add UpdatedBy varchar(25) null 

go
alter table HrEmployeeSales
  add UpdatedDate datetime null

go
alter table HrEmployeeSales
  add IsTransfered bit null