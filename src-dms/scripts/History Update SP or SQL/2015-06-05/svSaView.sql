DROP VIEW [dbo].[SvSaView]
GO

CREATE view [dbo].[SvSaView]
as 
select a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName  from GnMstEmployee a
where a.TitleCode IN ('7')
   AND PersonnelStatus = '1'

GO