ALTER PROCEDURE [dbo].[uspfn_GetSPKForApprovalPdiFsc]
--Declare
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@ProductType varchar(15),
	@IsPdi bit
--select @CompanyCode = '6083401002', @BranchCode = '6083401002', @ProductType = '4W', @IsPdi = 0
AS
BEGIN
	declare @sql varchar(8000)
	declare @joinType4FSC varchar(20)
	declare @joinVal4FSC varchar(20)

	if(@IsPdi = 1) begin
		set @joinType4FSC = ' left join '
		set @joinVal4FSC = ''
	end
	else begin
		set @joinType4FSC = ' left join '
		set @joinVal4FSC = char(13) + ' and b.IsLocked = 0 '
	end

	set @sql = 'select convert(bit, 0) IsSelected, row_number() over(order by a.JobOrderNo asc) as No,a.BranchCode, a.ServiceNo, a.JobOrderNo, a.JobOrderDate
		, a.ServiceBookNo, a.ChassisNo, a.BasicModel, a.JobType, (isnull(srv.ValItem, 0) + isnull(task.valTask,0)) TotalApprove
		from SvTrnService a
		left join (select CompanyCode, BranchCode, ServiceNo, sum((OperationHour * OperationCost)) valTask
			from svTrnSrvTask
			where BillType = ''F''
			group by CompanyCode, BranchCode, ServiceNo) task on task.CompanyCode = a.CompanyCode
												and task.BranchCode = a.BranchCode
												and task.ServiceNo = a.ServiceNo
		left join (select CompanyCode, BranchCode, ServiceNo, sum(((SupplyQty - ReturnQty) * RetailPrice)) valItem 
			from svTrnSrvItem 
			where BillType = ''F''
			group by CompanyCode, BranchCode, ServiceNo) srv on srv.CompanyCode = a.CompanyCode
												and srv.BranchCode = a.BranchCode
												and srv.ServiceNo = a.ServiceNo' 
		  + @joinType4FSC + 'svTrnInvoice b on 
		    a.CompanyCode = b.CompanyCode and
			a.BranchCode = b.BranchCode and
			a.JobOrderNo = b.JobOrderNo ' + @joinVal4FSC + '
		
		left join (select CompanyCode, BranchCode, ServiceNo, COUNT(BillType) Bill
		from svTrnSrvItem
		where BillType <> ''F''
		group by CompanyCode, BranchCode, ServiceNo) item on item.CompanyCode = a.CompanyCode
											and item.BranchCode = a.BranchCode
											and item.ServiceNo = a.ServiceNo
		
		where a.CompanyCode = ''' + @CompanyCode + '''
		and a.BranchCode = ''' + @BranchCode + '''
		and a.ProductType = ''' + @ProductType + '''
		and a.ServiceStatus = ''5 ''
		and a.IsLocked = 0
		and a.JobType like (case when ' + convert(varchar, @IsPDI) + ' = 1 then ''PDI%'' else ''FS%'' end )
		--and a.InvoiceNo is not null
		and (case when ' + convert(varchar, @IsPDI) + ' = 0 then a.InvoiceNo else a.InvoiceNo end ) is not null
		and ((case when item.Bill > 0 then a.InvoiceNo else a.InvoiceNo end) <> ''''					-- Perubahan
			OR (case when item.Bill IS NULL then a.InvoiceNo else a.InvoiceNo end) = '''' )				-- Perubahan
		Order by a.JobOrderNo'																			-- Perubahan
		

		--print (@sql);
		exec (@sql)
END