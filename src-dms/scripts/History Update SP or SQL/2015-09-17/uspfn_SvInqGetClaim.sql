ALTER procedure [dbo].[uspfn_SvInqGetClaim]
	 @CompanyCode	varchar(20),
	 @InvoiceFrom	varchar(20),
	 @InvoiceTo		varchar(20),
	 @BranchFrom	varchar(20),
	 @BranchTo		varchar(20),
	 @IsSprClaim	bit = 0

as

if @BranchFrom = '[All Branch]' and @BranchTo = '[All Branch]'
begin
	select * into #t1 from (
	select (row_number() over (order by a.InvoiceNo)) as SeqNo
		 , a.BranchCode, a.InvoiceNo, isnull(b.JobOrderDate, c.InvoiceDate) InvoiceDate
		 , a.IsCbu, a.CategoryCode, a.ComplainCode, a.DefectCode
		 , a.SubletHour, a.SubletAmt, a.CausalPartNo, a.TroubleDescription
		 , a.ProblemExplanation, a.OperationNo, a.OperationHour, a.OperationAmt
		 , isnull(b.BasicModel, c.BasicModel) BasicModel
		 , isnull(b.ServiceBookNo, '') ServiceBookNo
		 , isnull(b.ChassisCode, c.ChassisCode) ChassisCode
		 , isnull(b.ChassisNo, c.ChassisNo) ChassisNo
		 , isnull(b.EngineCode, c.EngineCode) EngineCode
		 , isnull(b.EngineNo, c.EngineNo) EngineNo
		 , isnull(b.Odometer, 0) Odometer
		 , isnull(b.TotalSrvAmount, c.TotalSrvAmt) ClaimAmt
		 , isnull(b.TotalSrvAmount, c.TotalSrvAmt) TotalSrvAmt
	  from svTrnInvClaim a
	  left join svTrnService b
		on b.CompanyCode = a.CompanyCode
	   and b.BranchCode = a.BranchCode
	   and b.JobOrderNo = a.InvoiceNo
	  left join svTrnInvoice c
		on c.CompanyCode = a.CompanyCode
	   and c.BranchCode = a.BranchCode
	   and c.InvoiceNo = a.InvoiceNo
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode
	   and convert(varchar, b.JobOrderDate, 112) between @InvoiceFrom and @InvoiceTo
	   and isnull(b.IsSparepartClaim, 0) = @IsSprClaim
	   and not exists (
		select * from svTrnClaimApplication
		 where CompanyCode = a.CompanyCode
		   and BranchCode = a.BranchCode
		   and InvoiceNo = a.InvoiceNo
		)
	   and b.ServiceStatus != '6' --Penambahan
	)#
	
	select * from #t1

	select (row_number() over (order by a.BasicModel)) as SeqNo
		 , a.BasicModel, count(a.BasicModel) Qty, sum(a.TotalSrvAmt) as TotalSrvAmt
	  from #t1 a
	 group by a.BasicModel

	drop table #t1
end
else
begin
	select * into #t2 from (
	select (row_number() over (order by a.InvoiceNo)) as SeqNo
		 , a.BranchCode, a.InvoiceNo, isnull(b.JobOrderDate, c.InvoiceDate) InvoiceDate
		 , a.IsCbu, a.CategoryCode, a.ComplainCode, a.DefectCode
		 , a.SubletHour, a.SubletAmt, a.CausalPartNo, a.TroubleDescription
		 , a.ProblemExplanation, a.OperationNo, a.OperationHour, a.OperationAmt
		 , isnull(b.BasicModel, c.BasicModel) BasicModel
		 , isnull(b.ServiceBookNo, '') ServiceBookNo
		 , isnull(b.ChassisCode, c.ChassisCode) ChassisCode
		 , isnull(b.ChassisNo, c.ChassisNo) ChassisNo
		 , isnull(b.EngineCode, c.EngineCode) EngineCode
		 , isnull(b.EngineNo, c.EngineNo) EngineNo
		 , isnull(b.Odometer, 0) Odometer
		 , isnull(b.TotalSrvAmount, c.TotalSrvAmt) ClaimAmt
		 , isnull(b.TotalSrvAmount, c.TotalSrvAmt) TotalSrvAmt
	  from svTrnInvClaim a
	  left join svTrnService b
		on b.CompanyCode = a.CompanyCode
	   and b.BranchCode = a.BranchCode
	   and b.JobOrderNo = a.InvoiceNo
	  left join svTrnInvoice c
		on c.CompanyCode = a.CompanyCode
	   and c.BranchCode = a.BranchCode
	   and c.InvoiceNo = a.InvoiceNo
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode between @BranchFrom and @BranchTo
	   and a.InvoiceNo between @InvoiceFrom and @InvoiceTo
	   and isnull(b.IsSparepartClaim, 0) = @IsSprClaim
	   and not exists (
		select * from svTrnClaimApplication
		 where CompanyCode = a.CompanyCode
		   and BranchCode = a.BranchCode
		   and InvoiceNo = a.InvoiceNo
		)
       and b.ServiceStatus != '6' --Penambahan
	)#
		
	select * from #t2

	select (row_number() over (order by a.BasicModel)) as SeqNo
		 , a.BasicModel, count(a.BasicModel) Qty, sum(a.TotalSrvAmt) as TotalSrvAmt
	  from #t2 a
	 group by a.BasicModel

	drop table #t2
end


