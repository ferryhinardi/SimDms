create view [dbo].[SvGetMekanik]  as    
  SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName , a.PersonnelStatus IsAvailableStatus, /*CASE b.IsAvailable WHEN '1' THEN 'Hadir' ELSE 'Tidak Aktif' END AS IsAvailable,*/     
  CASE a.PersonnelStatus WHEN '1' THEN 'AKTIF' END as PersonnelStatus    
  FROM gnMstEmployee a  
  where 1=1   
  and a.PersonnelStatus = '1'    
  AND a.TitleCode in ('2','3','8','9') 