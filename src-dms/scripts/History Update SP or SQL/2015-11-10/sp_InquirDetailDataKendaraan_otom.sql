if object_id('sp_InquirDetailDataKendaraan') is not null	drop procedure sp_InquirDetailDataKendaraanGO
CREATE procedure [dbo].[sp_InquirDetailDataKendaraan] 
(
 @CompanyCode varchar(15),
 @BranchCode varchar(15),
 @ChassisCode varchar(100),
 @ChassisNo varchar(100)
)
AS

DECLARE
	@QRYTmp		AS varchar(max),
	@DBMD		AS varchar(25),
	@CompanyMD  AS varchar(25),
	@BranchMD AS varchar(25),
	@Otom		AS varchar(5)


BEGIN

set @CompanyMD = (SELECT TOP 1 CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode)
set @BranchMD = (SELECT TOP 1 UnitBranchMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
set @Otom = (SELECT ParaValue from gnMstLookUpDtl where CodeID='OTOM' AND LookUpValue='UNIT' AND CompanyCode=@CompanyCode)
set @DBMD = (SELECT TOP 1 DbMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 




if @DBMD IS NULL OR @Otom = '0'
begin
		select 
		(select b.refferenceDesc1 from ommstrefference b where b.companyCode = @CompanyCode
		and b.refferencetype = 'WARE' and b.refferenceCode = a.warehouseCode) as warehouseName
		,'(' + a.ColourCode+ ') ' +(select c.refferenceDesc1 from ommstrefference c where c.companyCode = @CompanyCode
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
		, h.bpuno+' ('+h.RefferenceDONo+') ' bpuno
		, convert(varchar, h.bpudate, 106) bpudate
		, a.sono
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
		dbo.ommstvehicle a
	left join omTrSalesInvoice b on b.CompanyCode = @CompanyCode and b.BranchCode like @BranchCode 
		and a.InvoiceNo = b.InvoiceNo
	left join gnMstCustomer c on c.CompanyCode = @CompanyCode and b.Customercode = c.CustomerCode
	left join omTrSalesSO d on d.CompanyCode = @CompanyCode and d.BranchCode like @BranchCode and a.SONo = d.SONo
	left join GnMstLookUpDtl e on e.CompanyCode = @CompanyCode and CodeID = 'GPAR' and e.LookUpValue = d.SalesCode
	left join gnMstEmployee f on f.Companycode = @CompanyCode and f.BranchCode like @BranchCode  
		and f.EmployeeID = d.Salesman
	left join gnMstCustomer g on g.CompanyCode = @CompanyCode and g.CustomerCode = d.LeasingCo
	left join omTrPurchaseBPU h on h.CompanyCode = @CompanyCode and h.BranchCode like @BranchCode 
		and a.PONo = h.PONo and a.BPUNo=h.BPUNo
	left join omTrPurchaseHPP i on i.CompanyCode = @CompanyCode and i.BranchCode like @BranchCode and a.HPPNo= i.HPPNo
	left join omTrPurchasePO j on j.CompanyCode = @CompanyCode and j.BranchCode like @BranchCode and a.PONo = j.PONo
	left join omTrSalesDO k on k.CompanyCode = @CompanyCode and k.BranchCode like @BranchCode and a.DONo = k.DONo and a.SONo= k.SONo
	left join omTrSalesBPK l on l.CompanyCode = @CompanyCode and l.BranchCode like @BranchCode and a.BPKNo = l.BPKNo
	left join omTrSalesInvoice m on m.CompanyCode = @CompanyCode and m.BranchCode like @BranchCode  
		and a.InvoiceNo = m.InvoiceNo
	left join omTrSalesReq n on n.CompanyCode = @CompanyCode and a.ReqOutNo = n.ReqNo
	left join omTrSalesSOModel o on o.CompanyCode = @CompanyCode and o.BranchCode like @BranchCode and a.SONo = o.SONo 
		and a.SalesModelCode = o.SalesModelCode and a.SalesModelYear = o.SalesModelYear and a.ChassisCode = o.ChassisCode
	left join omTrSalesSOVin p on p.CompanyCode = @CompanyCode and p.BranchCode like @BranchCode and a.SONo = p.SONo
		and a.SalesModelCode = p.SalesModelCode and a.SalesModelYear = p.SalesModelYear and a.ColourCode = p.ColourCode
		and a.ChassisNo = p.ChassisNo and a.ChassisCode = p.ChassisCode
	left join omTrPurchaseBPU q on q.CompanyCode = @CompanyCode and q.BranchCode like @BranchCode and q.PONo = j.PONO 
		and q.BPUNo = a.BPUNo
	left join omTrSalesSPKDetail s on s.CompanyCode = @CompanyCode
		and s.ChassisCode = a.ChassisCode and s.ChassisNo = a.ChassisNo and a.ReqOutNo = s.ReqInNo
	where 
		a.companyCode = @CompanyCode and a.chassisCode = @ChassisCode and a.chassisNo = @ChassisNo 
end
else
	set @QRYTmp = 
	'select 
		(select b.refferenceDesc1 from ' + @DBMD + '.dbo.ommstrefference b where b.companyCode = ''' + @CompanyMD + '''
		and b.refferencetype = ''WARE'' and b.refferenceCode = a.warehouseCode) as warehouseName
		,''('' + a.ColourCode+ '') '' +(select c.refferenceDesc1 from ommstrefference c where c.companyCode = ''' + @CompanyCode + '''
		and c.refferencetype = ''COLO'' and c.refferenceCode = a.ColourCode) as ColourName
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
		, h.bpuno+'' (''+h.RefferenceDONo+'')'' bpuno
		, convert(varchar, h.bpudate, 106) bpudate
		, aa.sono
		, convert(varchar, d.sodate, 106) sodate
		, k.dono
		, convert(varchar, k.dodate, 106) dodate
		, case d.SKPKNo when '''' then d.RefferenceNo else d.SKPKNo end as SKPKNo
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
		, isnull(b.CustomerCode + '' - '' + c.CustomerName, 
			k.CustomerCode + '' - '' + (select CustomerName from gnMstCustomer where CompanyCode = ''' + @CompanyCode + ''' and CustomerCode = k.CustomerCode)
			) as Customer
		, isnull(c.Address1 + '' '' + c.Address2 + '' '' + c.Address3 + '' '' + c.Address4,
			(select Address1 + '' '' + Address2 + '' '' + Address3 + '' '' + Address4 from gnMstCustomer where CompanyCode = ''' + @CompanyCode + ''' and CustomerCode = k.CustomerCode)
			) as Address
		, d.Salesman + '' - '' + f.EmployeeName as Salesman
		, d.LeasingCo + '' - '' + g.CustomerName as Leasing
		, d.SalesCode + '' - '' + e.LookUpValueName as KelAR
		, s.BPKBNo
		, s.SPKNo
		, a.ChassisCode
		, a.SalesModelCode
		, a.ChassisNo
		, a.EngineNo
	from 
		' + @DBMD + '.dbo.ommstvehicle a
	left join ommstvehicle aa on aa.CompanyCode = ''' + @CompanyCode + ''' and a.ChassisCode = aa.ChassisCode and a.ChassisNo = aa.ChassisNo and aa.ColourCode = a.ColourCode
	left join omTrSalesInvoice b on b.CompanyCode = ''' + @CompanyCode + ''' and b.BranchCode like ''' + @BranchCode + ''' 
		and a.InvoiceNo = b.InvoiceNo
	left join gnMstCustomer c on c.CompanyCode = ''' + @CompanyCode + ''' and b.Customercode = c.CustomerCode
	left join omTrSalesSO d on d.CompanyCode = ''' + @CompanyCode + ''' and d.BranchCode like ''' + @BranchCode + ''' and aa.SONo = d.SONo
	left join GnMstLookUpDtl e on e.CompanyCode = ''' + @CompanyCode + ''' and CodeID = ''GPAR'' and e.LookUpValue = d.SalesCode
	left join gnMstEmployee f on f.Companycode = ''' + @CompanyCode + ''' and f.BranchCode like ''' + @BranchCode + ''' 
		and f.EmployeeID = d.Salesman
	left join gnMstCustomer g on g.CompanyCode = ''' + @CompanyCode + ''' and g.CustomerCode = d.LeasingCo
	left join ' + @DBMD + '..omTrPurchaseBPU h on h.CompanyCode = ''' + @CompanyMD + ''' and h.BranchCode like ''' + @BranchMD + ''' 
		and a.PONo = h.PONo and a.BPUNo=h.BPUNo
	left join ' + @DBMD + '..omTrPurchaseHPP i on i.CompanyCode = ''' + @CompanyMD + ''' and i.BranchCode like ''' + @BranchMD + ''' and a.HPPNo= i.HPPNo
	left join ' + @DBMD + '..omTrPurchasePO j on j.CompanyCode = ''' + @CompanyMD + ''' and j.BranchCode like ''' + @BranchMD + ''' and a.PONo = j.PONo
	left join omTrSalesDO k on k.CompanyCode = ''' + @CompanyCode + ''' and k.BranchCode like ''' + @BranchCode + ''' and aa.DONo = k.DONo and aa.SONo= k.SONo
	left join omTrSalesBPK l on l.CompanyCode = ''' + @CompanyCode + ''' and l.BranchCode like ''' + @BranchCode + ''' and aa.BPKNo = l.BPKNo
	left join omTrSalesInvoice m on m.CompanyCode = ''' + @CompanyCode + ''' and m.BranchCode like ''' + @BranchCode + ''' 
		and aa.InvoiceNo = m.InvoiceNo
	left join omTrSalesReq n on n.CompanyCode = ''' + @CompanyCode + ''' and aa.ReqOutNo = n.ReqNo
	--and n.BranchCode like ''' + @BranchCode + ''' and aa.ReqOutNo = n.ReqNo
	left join omTrSalesSOModel o on o.CompanyCode = ''' + @CompanyCode + ''' and o.BranchCode like ''' + @BranchCode + ''' and aa.SONo = o.SONo 
		and a.SalesModelCode = o.SalesModelCode and a.SalesModelYear = o.SalesModelYear and a.ChassisCode = o.ChassisCode
	left join omTrSalesSOVin p on p.CompanyCode = ''' + @CompanyCode + ''' and p.BranchCode like ''' + @BranchCode + ''' and aa.SONo = p.SONo
		and a.SalesModelCode = p.SalesModelCode and a.SalesModelYear = p.SalesModelYear and a.ColourCode = p.ColourCode
		and a.ChassisNo = p.ChassisNo and a.ChassisCode = p.ChassisCode
	left join ' + @DBMD + '..omTrPurchaseBPU q on q.CompanyCode = ''' + @CompanyMD + ''' and q.BranchCode like ''' + @BranchMD + ''' and q.PONo = j.PONO 
		and q.BPUNo = a.BPUNo
	left join omTrSalesSPKDetail s on s.CompanyCode = ''' + @CompanyCode + ''' 
		--and s.BranchCode like ''' + @BranchCode + '''
		and s.ChassisCode = a.ChassisCode and s.ChassisNo = a.ChassisNo and aa.ReqOutNo = s.ReqInNo
	where 
		a.companyCode = ''' + @CompanyMD + ''' and a.chassisCode = ''' + @ChassisCode + ''' and a.chassisNo = ''' + @ChassisNo + ''''

		Exec (@QRYTmp);
	
END
GO
