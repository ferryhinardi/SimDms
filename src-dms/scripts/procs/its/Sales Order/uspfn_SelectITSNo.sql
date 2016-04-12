ALTER PROCEDURE [dbo].[uspfn_SelectITSNo]
	@CompanyCode varchar(15),
	@BranchCode varchar(15)
AS
BEGIN
	
select a.CompanyCode,a.BranchCode,convert(varchar,a.InquiryNumber) InquiryNo,a.InquiryDate,b.EmployeeName,a.NamaProspek,a.TipeKendaraan,
	a.EmployeeID, a.LastProgress, c.ProspectNo, e.ReturnNo
from pmKDP a
	left join gnMstEmployee b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode
		and a.EmployeeID=b.EmployeeID
	left join omTrSalesSO c on c.CompanyCode = a.CompanyCode 
		and c.BranchCode = a.BranchCode
		and c.ProspectNo = a.InquiryNumber
	left join omTrSalesInvoice d on d.CompanyCode = a.CompanyCode
		and d.BranchCode = a.BranchCode
		and d.SONo = c.SONo
	left join omTrSalesReturn e on e.CompanyCode = a.CompanyCode
		and e.BranchCode = a.BranchCode
		and e.InvoiceNo = d.InvoiceNo
where a.CompanyCode= @CompanyCode
	and a.BranchCode= @BranchCode
	and a.LastProgress in ('SPK', 'DO', 'DELIVERY')
	and e.ReturnNo is not null
union	
select a.CompanyCode,a.BranchCode,convert(varchar,a.InquiryNumber) InquiryNo,a.InquiryDate,b.EmployeeName,a.NamaProspek,a.TipeKendaraan,
	a.EmployeeID, a.LastProgress, c.ProspectNo, e.ReturnNo
from pmKDP a
	left join gnMstEmployee b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode
		and a.EmployeeID=b.EmployeeID
	left join omTrSalesSO c on c.CompanyCode = a.CompanyCode 
		and c.BranchCode = a.BranchCode
		and c.ProspectNo = a.InquiryNumber
	left join omTrSalesInvoice d on d.CompanyCode = a.CompanyCode
		and d.BranchCode = a.BranchCode
		and d.SONo = c.SONo
	left join omTrSalesReturn e on e.CompanyCode = a.CompanyCode
		and e.BranchCode = a.BranchCode
		and e.InvoiceNo = d.InvoiceNo
where a.CompanyCode= @CompanyCode
	and a.BranchCode= @BranchCode
	and a.LastProgress = 'SPK'
	and c.ProspectNo is null
END
