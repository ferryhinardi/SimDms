IF object_id('ITSBrowseSO4') IS NOT NULL
	DROP VIEW ITSBrowseSO4
GO

CREATE VIEW [dbo].[ITSBrowseSO4]  
as  
select distinct a.CompanyCode,a.BranchCode,convert(varchar,a.InquiryNumber) InquiryNo,a.InquiryDate,b.EmployeeName,a.NamaProspek,a.TipeKendaraan,  
 a.EmployeeID, a.LastProgress, a.CreatedBy  
from pmKDP a  
 left join HrEmployee b on a.CompanyCode=b.CompanyCode  
  and a.EmployeeID=b.EmployeeID  
  where a.LastProgress='SPK'

GO


