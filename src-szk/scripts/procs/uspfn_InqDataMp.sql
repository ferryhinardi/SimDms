
go
if object_id('uspfn_InqDataMp') is not null
	drop procedure uspfn_InqDataMp

go
CREATE procedure uspfn_InqDataMp
	@TableName varchar(80) = ''
as

if @TableName = 'HrEmployee' exec uspfn_InqHrEmployee
if @TableName = 'HrEmployeeAchievement' exec uspfn_InqHrEmployeeAchievement
if @TableName = 'HrEmployeeSales' exec uspfn_InqHrEmployeeSales
if @TableName = 'HrEmployeeTraining' exec uspfn_InqHrEmployeeTraining
if @TableName = 'HrEmployeeMutation' exec uspfn_InqHrEmployeeMutation


