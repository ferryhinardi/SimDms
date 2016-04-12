if object_id('sp_InquirDetailDataKendaraan') is not null
	drop procedure sp_InquirDetailDataKendaraan
GO
CREATE procedure [dbo].[sp_InquirDetailDataKendaraan] 
(
 @CompanyCode varchar(15),
 @BranchCode varchar(15),
 @ChassisCode varchar(100),
 @ChassisNo varchar(100)
)
AS

BEGIN

select 
	(select b.refferenceDesc1 from ommstrefference b where b.companyCode = a.companyCode
	and b.refferencetype = 'WARE' and b.refferenceCode = a.warehouseCode) as warehouseName
	,'('+a.ColourCode+') '+(select c.refferenceDesc1 from ommstrefference c where c.companyCode = a.companyCode
	and c.refferencetype = 'COLO' and c.refferenceCode = a.ColourCode) as ColourName
	, a.servicebookno
	, a.keyno
	, a.cogsunit
	, a.cogsOthers
	, a.cogsKaroseri
    , o.afterdiscdpp dpp
    , o.afterdiscppn ppn
    , p.bbn
	, j.pono
    , convert(varchar, j.podate, 106) podate
	, h.bpuno+' ('+h.RefferenceDONo+')' bpuno
    , convert(varchar, h.bpudate, 106) bpudate
	, d.sono
    , convert(varchar, d.sodate, 106) sodate
	, k.dono
    , convert(varchar, k.dodate, 106) dodate
    , case d.SKPKNo when '' then d.RefferenceNo else d.SKPKNo end as SKPKNo
    , convert(varchar, d.sodate, 106)  SKPKDate
    , l.bpkno
    , convert(varchar, l.bpkdate, 106) bpkdate
	, m.invoiceNo
    , convert(varchar, m.invoicedate, 106) invoicedate
    , q.RefferenceSJNo
	, convert(varchar, q.RefferenceSJDate, 106) RefferenceSJDate
    , i.hppno
    , convert(varchar, i.hppdate, 106) hppdate
	, n.reqNo reqOutNo
    , convert(varchar, n.reqDate, 106) reqdate
    , i.RefferenceInvoiceNo reffinv
    , convert(varchar, i.RefferenceInvoiceDate, 106) reffinvdate
    , i.RefferenceFakturPajakNo refffp
    , convert(varchar, i.RefferenceFakturPajakDate , 106) refffpdate
	, s.PoliceRegistrationNo policeregno
    , convert(varchar, s.PoliceRegistrationDate, 106) policeregdate
	, isnull(b.CustomerCode + ' - ' + c.CustomerName, 
		k.CustomerCode + ' - ' + (select CustomerName from gnMstCustomer where CompanyCode = @CompanyCode and CustomerCode = k.CustomerCode)
		) as Customer
	, isnull(c.Address1 + ' ' + c.Address2 + ' ' + c.Address3 + ' ' + c.Address4,
		(select Address1 + ' ' + Address2 + ' ' + Address3 + ' ' + Address4 from gnMstCustomer where CompanyCode = @CompanyCode and CustomerCode = k.CustomerCode)
		) as Address
	, d.Salesman + ' - ' + f.EmployeeName as Salesman
	, d.LeasingCo + ' - ' + g.CustomerName as Leasing
	, d.SalesCode + ' - ' + e.LookUpValueName as KelAR
    , s.BPKBNo
	, s.SPKNo
	, a.ChassisCode
	, a.SalesModelCode
	, a.ChassisNo
	, a.EngineNo
from 
	ommstvehicle a
left join omTrSalesInvoice b on a.CompanyCode = b.CompanyCode and b.BranchCode like @BranchCode 
    and a.InvoiceNo = b.InvoiceNo
left join gnMstCustomer c on a.CompanyCode = c.CompanyCode and b.Customercode = c.CustomerCode
left join omTrSalesSO d on a.CompanyCode = d.CompanyCode and d.BranchCode like @BranchCode and a.SONo = d.SONo
left join GnMstLookUpDtl e on a.CompanyCode = e.CompanyCode and CodeID = 'GPAR' and e.LookUpValue = d.SalesCode
left join gnMstEmployee f on a.CompanyCode  = f.Companycode and f.BranchCode like @BranchCode 
    and f.EmployeeID = d.Salesman
left join gnMstCustomer g on a.CompanyCode = g.CompanyCode and g.CustomerCode = d.LeasingCo
left join omTrPurchaseBPU h on a.CompanyCode= h.CompanyCode and h.BranchCode like @BranchCode 
    and a.PONo = h.PONo and a.BPUNo=h.BPUNo
left join omTrPurchaseHPP i on a.CompanyCode= i.CompanyCode and i.BranchCode like @BranchCode and a.HPPNo= i.HPPNo
left join omTrPurchasePO j on a.CompanyCode = j.CompanyCode and j.BranchCode like @BranchCode and a.PONo = j.PONo
left join omTrSalesDO k on a.CompanyCode = k.CompanyCode and k.BranchCode like @BranchCode and a.DONo = k.DONo and a.SONo= k.SONo
left join omTrSalesBPK l on a.CompanyCode = l.CompanyCode and l.BranchCode like @BranchCode and a.BPKNo = l.BPKNo
left join omTrSalesInvoice m on a.CompanyCode = m.CompanyCode and m.BranchCode like @BranchCode 
    and a.InvoiceNo = m.InvoiceNo
left join omTrSalesReq n on a.CompanyCode = n.CompanyCode and n.BranchCode like @BranchCode and a.ReqOutNo = n.ReqNo
left join omTrSalesSOModel o on a.CompanyCode = o.CompanyCode and o.BranchCode like @BranchCode and a.SONo = o.SONo 
    and a.SalesModelCode = o.SalesModelCode and a.SalesModelYear = o.SalesModelYear and a.ChassisCode = o.ChassisCode
left join omTrSalesSOVin p on a.CompanyCode = p.CompanyCode and p.BranchCode like @BranchCode and a.SONo = p.SONo
    and a.SalesModelCode = p.SalesModelCode and a.SalesModelYear = p.SalesModelYear and a.ColourCode = p.ColourCode
    and a.ChassisNo = p.ChassisNo and a.ChassisCode = p.ChassisCode
left join omTrPurchaseBPU q on a.CompanyCode = q.CompanyCode and q.BranchCode like @BranchCode and q.PONo = j.PONO 
	and q.BPUNo = a.BPUNo
left join omTrSalesSPKDetail s on s.CompanyCode = a.CompanyCode and s.BranchCode like @BranchCode
	and s.ChassisCode = a.ChassisCode and s.ChassisNo = a.ChassisNo
where 
	a.companyCode = @CompanyCode and a.chassisCode = @ChassisCode and a.chassisNo = @ChassisNo

END
GO
