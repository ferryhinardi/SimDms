
go
if object_id('SvSaView') is not null
	drop view SvSaView

go
create view SvSaView
as 
select a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName  from GnMstEmployee a
