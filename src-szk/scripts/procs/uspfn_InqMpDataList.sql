
go
if object_id('uspfn_InqMpDataList') is not null
	drop procedure uspfn_InqMpDataList

go
CREATE procedure uspfn_InqMpDataList
	@type varchar(20) = ''
as



declare @t_list as table (
	text varchar(50), 
	value varchar(50), 
	type varchar(20), 
	seq int
)

insert into @t_list (type, seq, value, text) values ('mp', 401, 'HrEmployee', 'Hr Employee');
insert into @t_list (type, seq, value, text) values ('mp', 401, 'HrEmployeeAchievement', 'Hr Employee Achievement');
insert into @t_list (type, seq, value, text) values ('mp', 401, 'HrEmployeeSales', 'Hr Employee Sales');
insert into @t_list (type, seq, value, text) values ('mp', 401, 'HrEmployeeTraining', 'Hr Employee Training');
insert into @t_list (type, seq, value, text) values ('mp', 401, 'HrEmployeeMutation', 'Hr Employee  Mutation');

select * from @t_list 


