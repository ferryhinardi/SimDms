--insert SysReportDms VALUES ('SvRptCancelInvoice','sv/svCancelInvoice.rdlc','uspfn_RptCancelInvoice')
--GO

GO
drop procedure uspfn_RptCancelInvoice 
GO
create procedure uspfn_RptCancelInvoice 
@CompanyCode varchar(25), 
@BranchCode varchar(25), 
@PeriodStart datetime, 
@PeriodEnd DateTime
as
SELECT a.BranchCode, a.JobOrderNo, isNull(b.InvoiceNo,'-') as InvoiceNo, isnull(convert(varchar(25),b.InvoiceDate,13),'-') as InvoiceDate,isnull(convert(varchar,convert(MONEY,b.TotalSrvAmt),1),'-') as TotalInvoice, isnull(d.FullName,'-') as UpdatedBy,
a.InvoiceNo as InvoiceCancel, isnull(convert(varchar(25),a.InvoiceDate,113),'-') as InvoiceDateCancel, convert(varchar,convert(MONEY,a.TotalSrvAmt),1) as TotalInvoiceCancel,c.FullName as UpdatedByCancel
FROM svTrnInvoiceLog a
left JOIN svTrnInvoice b ON a.CompanyCode = b.CompanyCode
AND a.BranchCode = b.BranchCode AND substring(a.InvoiceNo,0,4) = substring(b.InvoiceNo,0,4) and a.JobOrderNo = b.JobOrderNo 
left JOIN sysUser c 
ON c.UserId = substring(a.CreatedBy,0,CHARINDEX ('^', a.CreatedBy, 1))
left join Sysuser d
on d.UserID = b.CreatedBy
where  a.CompanyCode = @CompanyCode
and a.BranchCode = @BranchCode
and a.InvoiceDate between @PeriodStart AND @PeriodEnd


select co.CompanyName as BranchName, co.Address1, co.Address2, co.Address3, co.Address4 , Convert(varchar(15),@PeriodStart,106) +' s/d '+Convert(varchar(15),@PeriodEnd,106) as PeriodStartEnd from gnMstCoProfile co
where co.CompanyCode = @CompanyCode
and co.BranchCode = @BranchCode