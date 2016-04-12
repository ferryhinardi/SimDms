create view [dbo].[SvFmView]

as 

select a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName  from GnMstEmployee a
where a.TitleCode IN ('8')
   AND PersonnelStatus = '1'
GO


