alter view [dbo].[SvMechanicAvbView]  as  
  SELECT b.CompanyCode,a.EmployeeID, b.BranchCode, a.EmployeeName , /*CASE b.IsAvailable WHEN '1' THEN 'Hadir' ELSE 'Tidak Aktif' END AS IsAvailableString,*/     
  b.IsAvailable, CASE b.IsAvailable WHEN '1' THEN 'Hadir' ELSE 'Tidak Hadir' END as AttendStatus    
  FROM gnMstEmployee a, svMstAvailableMechanic b    
  where b.EmployeeID = a.EmployeeID    
  AND a.PersonnelStatus = '1'    
  and a.CompanyCode = b.CompanyCode    
  AND a.BranchCode  = b.BranchCode    
    