create view [dbo].[ITSBrowse]
as
select distinct a.CompanyCode,a.BranchCode,convert(varchar,a.InquiryNumber) InquiryNo,a.InquiryDate,b.EmployeeName,a.NamaProspek,a.TipeKendaraan,
	a.EmployeeID, a.LastProgress
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
where --a.CompanyCode= @CompanyCode
	--and a.BranchCode= @BranchCode
	--and 
	a.LastProgress in ('SPK', 'DO', 'DELIVERY')
	and (e.ReturnNo is not null or c.Status =3) and not exists (select * from omTrSalesSO c where c.CompanyCode = a.CompanyCode and c.BranchCode = a.BranchCode and c.ProspectNo = a.InquiryNumber and c.Status = 2)	 
union
select distinct a.CompanyCode,a.BranchCode,convert(varchar,a.InquiryNumber) InquiryNo,a.InquiryDate,b.EmployeeName,a.NamaProspek,a.TipeKendaraan,
	a.EmployeeID, a.LastProgress
from pmKDP a
	left join gnMstEmployee b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode
		and a.EmployeeID=b.EmployeeID
	left join omTrSalesSO c on c.CompanyCode = a.CompanyCode 
		and c.BranchCode = a.BranchCode
		and c.ProspectNo = a.InquiryNumber
		and c.Status = 2
	left join omTrSalesInvoice d on d.CompanyCode = a.CompanyCode
		and d.BranchCode = a.BranchCode
		and d.SONo = c.SONo
	left join omTrSalesReturn e on e.CompanyCode = a.CompanyCode
		and e.BranchCode = a.BranchCode
		and e.InvoiceNo = d.InvoiceNo
	left join omTrSalesSOVin f on f.CompanyCode = c.CompanyCode	--Penambahan
		and f.BranchCode = c.BranchCode	--Penambahan
		and f.SONo = c.SONo	--Penambahan
	left join omMstVehicle g on g.CompanyCode = f.CompanyCode --Penambahan
		and g.ChassisNo = f.ChassisNo	--Penambahan
where 
--a.CompanyCode= @CompanyCode
	--and a.BranchCode= @BranchCode
	--and 
	a.LastProgress in ('SPK', 'DO', 'DELIVERY')
	and (e.ReturnNo is not null or c.Status =3) 
--	and exists (select * from omTrSalesReturn c where c.CompanyCode = a.CompanyCode and c.BranchCode = a.BranchCode and c.InvoiceNo = d.InvoiceNo and c.Status = 2)		
	and exists (select * from omTrSalesReturn c where c.CompanyCode = a.CompanyCode and c.BranchCode = a.BranchCode and c.InvoiceNo = d.InvoiceNo and c.Status in (2,5))	--Perubahan	
	and g.InvoiceNo = '' --Penambahan

union
select distinct a.CompanyCode,a.BranchCode,convert(varchar,a.InquiryNumber) InquiryNo,a.InquiryDate,b.EmployeeName,a.NamaProspek,a.TipeKendaraan,
	a.EmployeeID, a.LastProgress
from pmKDP a
	left join gnMstEmployee b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode
		and a.EmployeeID=b.EmployeeID
	left join omTrSalesSO c on c.CompanyCode = a.CompanyCode 
		and c.BranchCode = a.BranchCode
		and c.ProspectNo = a.InquiryNumber
		and c.Status = 2
	left join omTrSalesInvoice d on d.CompanyCode = a.CompanyCode
		and d.BranchCode = a.BranchCode
		and d.SONo = c.SONo
	left join omTrSalesReturn e on e.CompanyCode = a.CompanyCode
		and e.BranchCode = a.BranchCode
		and e.InvoiceNo = d.InvoiceNo
	left join omTrSalesSOVin f on f.CompanyCode = c.CompanyCode	--Penambahan
		and f.BranchCode = c.BranchCode	--Penambahan
		and f.SONo = c.SONo	--Penambahan
	left join omMstVehicle g on g.CompanyCode = f.CompanyCode --Penambahan
		and g.ChassisNo = f.ChassisNo	--Penambahan
where 
	--a.CompanyCode= @CompanyCode
	--and a.BranchCode= @BranchCode
	--and 
	a.LastProgress in ('SPK', 'DO', 'DELIVERY')
	and (e.ReturnNo is not null or c.Status =3) 
--	and exists (select * from omTrSalesReturn c where c.CompanyCode = a.CompanyCode and c.BranchCode = a.BranchCode and c.InvoiceNo = d.InvoiceNo and c.Status = 2)	
	and exists (select * from omTrSalesReturn c where c.CompanyCode = a.CompanyCode and c.BranchCode = a.BranchCode and c.InvoiceNo = d.InvoiceNo and c.Status in (2,5))	--Perubahan		
	and (select count(*) from omTrSalesSO c where c.CompanyCode = a.CompanyCode and c.BranchCode = a.BranchCode and c.ProspectNo = a.InquiryNumber and c.Status = 2) = 1
	and g.InvoiceNo = ''	--Penambahan

union
select a.CompanyCode,a.BranchCode,convert(varchar,a.InquiryNumber) InquiryNo,a.InquiryDate,b.EmployeeName,a.NamaProspek,a.TipeKendaraan,
	a.EmployeeID, a.LastProgress
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
where --a.CompanyCode= @CompanyCode
	--and a.BranchCode= @BranchCode
	--and 
	a.LastProgress = 'SPK'
	and c.ProspectNo is null





GO


