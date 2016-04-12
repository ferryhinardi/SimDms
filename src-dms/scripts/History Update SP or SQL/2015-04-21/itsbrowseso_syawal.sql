ALTER view [dbo].[ITSBrowseSO]  
as  
select distinct a.CompanyCode,a.BranchCode,convert(varchar,a.InquiryNumber) InquiryNo,a.InquiryDate,b.EmployeeName,a.NamaProspek,a.TipeKendaraan,  
 a.EmployeeID, a.LastProgress, a.CreatedBy  
from pmKDP a  
 left join gnMstEmployee b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode  
  and a.EmployeeID=b.EmployeeID  
  where a.LastProgress='SPK'
  
GO