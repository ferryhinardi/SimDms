ALTER PROCEDURE [dbo].[uspfn_GetSPKForApprovalPdiFsc]
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@ProductType varchar(15),
	@IsPdi bit
AS
BEGIN

	
	select convert(bit, 0) IsSelected, row_number() over(order by a.ServiceNo asc) as No,a.BranchCode, a.ServiceNo, a.JobOrderNo, a.JobOrderDate
	, a.ServiceBookNo, a.ChassisNo, a.BasicModel, a.JobType, (isnull(srv.ValItem, 0) + isnull(task.valTask,0)) TotalApprove
	from SvTrnService a
	left join (select CompanyCode, BranchCode, ServiceNo, sum((OperationHour * OperationCost)) valTask
		from svTrnSrvTask
		where BillType = 'F'
		group by CompanyCode, BranchCode, ServiceNo) task on task.CompanyCode = a.CompanyCode
											and task.BranchCode = a.BranchCode
											and task.ServiceNo = a.ServiceNo
	left join (select CompanyCode, BranchCode, ServiceNo, sum(((SupplyQty - ReturnQty) * RetailPrice)) valItem 
		from svTrnSrvItem 
		where BillType = 'F'
		group by CompanyCode, BranchCode, ServiceNo) srv on srv.CompanyCode = a.CompanyCode
											and srv.BranchCode = a.BranchCode
											and srv.ServiceNo = a.ServiceNo
	left join svTrnInvoice b on a.CompanyCode = b.CompanyCode and
		a.BranchCode = b.BranchCode and
		a.JobOrderNo = b.JobOrderNo
	where a.CompanyCode = @CompanyCode
	and a.BranchCode = @BranchCode
	and a.ProductType = @ProductType
	and a.ServiceStatus = '5 '
	and a.IsLocked = 0
	and a.JobType like (case when @IsPDI = 1 then 'PDI%' else 'FS%' end )
	--and a.InvoiceNo is not null
	and (case when @IsPDI = 0 then a.InvoiceNo else a.InvoiceNo end ) is not null	-- Perubahan
END

