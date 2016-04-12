
go
if object_id('SvFmView') is not null
	drop view SvFmView

go
create view SvFmView
as 
select a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName  from GnMstEmployee a
