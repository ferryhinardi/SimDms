

go
if object_id('uspfn_SvInqFpjHQGet') is not null
	drop procedure uspfn_SvInqFpjHQGet

go
Create procedure uspfn_SvInqFpjHQGet
	@CompanyCode nvarchar(20),
	@ProductType nvarchar(20),
	@BranchCodeStart nvarchar(20),
	@BranchCodeEnd nvarchar(20),
	@FPJNo nvarchar(20)

as
begin

select	row_number() over(order by inv.BranchCode,inv.InvoiceDate,inv.InvoiceNo) RowNum
        , convert(bit, 1) IsSelected
        , inv.BranchCode
        , inv.InvoiceNo
        , inv.InvoiceDate
        , inv.JobOrderNo
        , inv.JobOrderDate
        , isnull(inv.TotalDPPAmt,0) TotalDPPAmt
        , isnull(inv.TotalPPHAmt,0) + isnull(TotalPPNAmt,0) as TotalPpnAmt
        , isnull(inv.TotalSrvAmt,0) TotalSrvAmt
        , inv.JobType
        , inv.PoliceRegNo
        , inv.BasicModel
        , inv.ChassisCode
        , inv.ChassisNo
        , inv.EngineCode
        , inv.EngineNo
        , inv.TOPCode
        , inv.TOPDays
        , inv.DueDate
        , inv.FPJNo
        , inv.FPJDate
        , inv.CustomerCodeBill
        , inv.Odometer
        , inv.IsPkp
        , inv.CustomerCode
        , inv.CustomerCode + ' - ' + cust.CustomerName Pelanggan
        , inv.CustomerCodeBill + ' - ' + custBill.CustomerName Pembayar
        , inv.DueDate
from	svTrnInvoice inv with(nolock, nowait)
        left join gnMstCustomer cust with(nolock, nowait) on inv.CompanyCode = cust.CompanyCode
            and inv.CustomerCode = cust.CustomerCode
        left join gnMstCustomer custBill with(nolock, nowait) on inv.CompanyCode = custBill.CompanyCode
            and inv.CustomerCodeBill = custBill.CustomerCode
where	inv.CompanyCode = @CompanyCode 
        and inv.BranchCode between @BranchCodeStart and @BranchCodeEnd
        and inv.ProductType = @ProductType                     
        and inv.CustomerCodeBill = isnull((select top 1 CustomerCodeBill from svTrnInvoice where InvoiceNo = inv.InvoiceNo),'')
        and inv.Islocked = 1
        and inv.FPJNo = @FPJNo
        and (inv.InvoiceNo like 'INW%' or inv.InvoiceNo like 'INF%')

end

GO


