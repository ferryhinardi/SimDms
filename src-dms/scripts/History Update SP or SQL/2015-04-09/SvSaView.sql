alter view [dbo].[SvSaView]

as 

select a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName  from GnMstEmployee a
where a.TitleCode IN ('3')
   AND PersonnelStatus = '1'



GO


