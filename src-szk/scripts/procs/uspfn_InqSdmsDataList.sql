go
if object_id('uspfn_InqSdmsDataList') is not null
	drop procedure uspfn_InqSdmsDataList

go
CREATE procedure uspfn_InqSdmsDataList
	@type varchar(20) = ''
as

declare @t_list as table (text varchar(50), value varchar(50), type varchar(20), seq int)

insert into @t_list (type, seq, value, text) values ('gn', 101, 'GnMstCustomer', 'GnMstCustomer');
insert into @t_list (type, seq, value, text) values ('gn', 102, 'CsTDayCall', 'CsTDayCall');
insert into @t_list (type, seq, value, text) values ('gn', 103, 'CsCustBirthDay', 'CsCustBirthDay');
insert into @t_list (type, seq, value, text) values ('gn', 104, 'CsCustBpkb', 'CsCustBpkb');
insert into @t_list (type, seq, value, text) values ('gn', 105, 'CsStnkExt', 'CsStnkExt');
insert into @t_list (type, seq, value, text) values ('sl', 201, 'PmHstITS', 'PmHstITS');
insert into @t_list (type, seq, value, text) values ('mp', 401, 'HrEmployee', 'HrEmployee');
--insert into @t_list (type, seq, value, text) values ('mp', 402, 'HrEmployeeAchievement', 'HrEmployeeAchievement');
--insert into @t_list (type, seq, value, text) values ('mp', 403, 'HrEmployeeMutation', 'HrEmployeeMutation');
--insert into @t_list (type, seq, value, text) values ('mp', 404, 'HrEmployeeTraining', 'HrEmployeeTraining');
insert into @t_list (type, seq, value, text) values ('sv', 501, 'SvMstCustomerVehicle', 'SvMstCustomerVehicle');
insert into @t_list (type, seq, value, text) values ('sv', 501, 'SvTrnService', 'SvTrnService');
insert into @t_list (type, seq, value, text) values ('sv', 501, 'SvTrnInvoice', 'SvTrnInvoice');

select * from @t_list where type = (case when isnull(@type, '') = '' then type else @type end) order by seq


