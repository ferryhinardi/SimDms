ALTER procedure [dbo].[uspfn_SvTrnInvInquiryBatch]
	@CompanyCode  varchar(20),
	@BranchCode   varchar(20),
	@GroupJobType varchar(20)
as   

--declare	@CompanyCode  varchar(20)
--declare	@BranchCode   varchar(20)
--declare	@GroupJobType varchar(20)

--set @CompanyCode  = '6159401000' 
--set @BranchCode   = '6159401001'
--set @GroupJobType = 'FSC'

if @GroupJobType = 'ALL'
begin
	select convert(bit, 0) as IsSelect, a.JobOrderNo, a.JobOrderDate, a.JobType
		 , a.LaborDppAmt as LaborAmt
		 , a.PartsDppAmt as PartAmt
		 , a.MaterialDppAmt as MaterialAmt
		 , a.TotalDppAmount as TotalDppAmt
		 , a.TotalPPnAmount as TotalPPnAmt
		 , a.TotalSrvAmount as TotalAmt
		 , convert(varchar, '') as Remarks
	  from SvTrnService a
	 where a.CompanyCode = @CompanyCode
	   and a.BranchCode = @BranchCode
	   and a.ServiceStatus = '5'
	   and not exists (select 1 from svTrnInvoice where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and JobOrderNo = a.JobOrderNo)
end
else
if @GroupJobType = 'FSC'
begin
	select convert(bit, 0) as IsSelect, a.JobOrderNo, a.JobOrderDate, a.JobType
		 , a.LaborDppAmt as LaborAmt
		 , a.PartsDppAmt as PartAmt
		 , a.MaterialDppAmt as MaterialAmt
		 , a.TotalDppAmount as TotalDppAmt
		 , a.TotalPPnAmount as TotalPPnAmt
		 , a.TotalSrvAmount as TotalAmt
		 , convert(varchar, '') as Remarks
	  from SvTrnService a
	 where a.CompanyCode = @CompanyCode
	   and a.BranchCode = @BranchCode
	   and a.ServiceStatus = '5'
	   and a.IsLocked = '1'
	   and not exists (select 1 from svTrnInvoice where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and JobOrderNo = a.JobOrderNo)
end
else
begin
	select convert(bit, 0) as IsSelect, a.JobOrderNo, a.JobOrderDate, a.JobType
		 , a.LaborDppAmt as LaborAmt
		 , a.PartsDppAmt as PartAmt
		 , a.MaterialDppAmt as MaterialAmt
		 , a.TotalDppAmount as TotalDppAmt
		 , a.TotalPPnAmount as TotalPPnAmt
		 , a.TotalSrvAmount as TotalAmt
		 , convert(varchar, '') as Remarks
	  from SvTrnService a
	  left join SvMstJob b on b.JobType = a.JobType
	   and b.CompanyCode = a.CompanyCode
	   and b.ProductType = a.ProductType
	   and b.BasicModel = a.BasicModel
	 where a.CompanyCode = @CompanyCode
	   and a.BranchCode = @BranchCode
	   and a.ServiceStatus = '5'
	   and b.GroupJobType = @GroupJobType
	   and not exists (select 1 from svTrnInvoice where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and JobOrderNo = a.JobOrderNo)
end



select a.JobOrderNo, a.InvoiceNo, a.InvoiceDate, a.JobType
	 , a.LaborDppAmt as LaborAmt
	 , a.PartsDppAmt as PartAmt
	 , a.MaterialDppAmt as MaterialAmt
	 , a.TotalDppAmt
	 , a.TotalPPnAmt
	 , a.TotalSrvAmt as TotalAmt
	 , a.Remarks
  from SvTrnInvoice a, GnMstCoProfileService b, ArInterface c
 where 1 = 1
   and c.CompanyCode = a.CompanyCode
   and c.BranchCode = a.BranchCode
   and c.DocNo = a.InvoiceNo
   and c.StatusFlag < 3
   and b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode
   and a.CompanyCode = @CompanyCode
   and a.BranchCode = @BranchCode


