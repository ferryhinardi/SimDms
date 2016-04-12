ALTER procedure [dbo].[uspfn_SvInqFpjData]    
		 @CompanyCode nvarchar(20),    
		 @BranchCode nvarchar(20),    
		 @DocPrefix nvarchar(20),    
		 @IsPdi bit,    
		 @IsFsc bit,    
		 @IsFSCCampaign bit,
		 @IsSprClaim bit = 0,    
		 @CustBill nvarchar(20)    
    
as    

--set @CompanyCode = '6006406'
--set @BranchCode = '6006402'
--set	@DocPrefix = 'INF'
--set	@IsPdi = 0
--set	@IsFsc = 0
--set	@IsFSCCampaign = 1
--set	@IsSprClaim = 0
--set	@CustBill = ''
    
select row_number() over(order by a.InvoiceNo) RowNum    
     , convert(bit, 1) IsSelected    
     , a.CompanyCode    
     , a.BranchCode    
     , a.InvoiceNo    
     , a.InvoiceDate    
     , a.JobOrderNo    
     , a.JobOrderDate    
     , a.TotalDPPAmt    
     , isnull(a.TotalPPHAmt, 0) + isnull(TotalPPNAmt, 0) as TotalPpnAmt         
     , a.TotalSrvAmt    
     , a.JobType    
     , a.PoliceRegNo    
     , a.BasicModel    
     , a.ChassisCode    
     , a.ChassisNo    
     , a.EngineCode    
     , a.EngineNo    
     , a.TOPCode    
     , a.TOPDays    
     , a.DueDate    
     , a.FPJNo    
     , a.FPJDate    
     , a.CustomerCodeBill    
     , a.Odometer    
     , a.IsPkp    
     , a.CustomerCode    
     , a.CustomerCode + ' - ' + b.CustomerName Pelanggan    
     , a.CustomerCodeBill + ' - ' + c.CustomerName Pembayar    
     , a.DueDate    
     , isnull(d.IsSparepartClaim, 0) IsSparepartClaim    
     , isnull(e.CampaignFlag, 0) CampaignFlag
  from svTrnInvoice a with(nolock, nowait)    
	left join gnMstCustomer b with(nolock, nowait)    
		on b.CompanyCode = a.CompanyCode    
		and b.CustomerCode = a.CustomerCode    
	left join gnMstCustomer c with(nolock, nowait)    
		on c.CompanyCode = a.CompanyCode    
		and c.CustomerCode = a.CustomerCodeBill    
	left join svTrnService d with(nolock, nowait)    
		on d.CompanyCode = a.CompanyCode    
		and d.BranchCode = a.BranchCode    
		and d.JobOrderNo = a.JobOrderNo    
	left join svMstFscCampaign e 
		on a.ChassisCode = e.ChassisCode
		and a.ChassisNo = e.ChassisNo
	--left join svTrnSrvTask f							--Penambahan
	--	on f.CompanyCode = d.CompanyCode				--Penambahan
	--	and f.BranchCode = d.BranchCode					--Penambahan
	--	and f.ServiceNo = d.ServiceNo					--Penambahan
	--left join svMstBillingType g						--Penambahan
	--	on g.CompanyCode = f.CompanyCode				--Penambahan
	--	and g.BillType = f.BillType						--Penambahan
	--	and g.CustomerCode = a.CustomerCodeBill			--Penambahan
 where 1 = 1    
		and a.CompanyCode = @CompanyCode     
		and a.BranchCode = @BranchCode     
		and isnull(a.InvoiceStatus, '0') = '0'    
		and isnull(a.FPJNo, '') = ''    
		and left(a.InvoiceNo, 3) in (@DocPrefix, case @DocPrefix when 'INC' then 'INP' else '' end)
		--and g.CustomerCode = a.CustomerCodeBill			--Penambahan
   -- if INF check PDI or FSC or all    
		and a.JobType like (    
       case     
         when (@DocPrefix = 'INF' and isnull(@IsPdi, 0) = 1) then 'PDI%'    
         when (@DocPrefix = 'INF' and isnull(@IsFsc, 0) = 1) then 'FSC%'    
         when (@DocPrefix = 'INF' and isnull(@IsFscCampaign, 0) = 1) then 'FSC%'    
         else '%'    
       end)       
   -- if INF check FSC Campaign       
   and isnull(e.CampaignFlag,0) =
	(case when (@DocPrefix = 'INF' and isnull(@IsFscCampaign, 0) = 1) then e.CampaignFlag else 0 end)
   -- if INW check SrvClaim or SprClaim    
   and isnull(d.IsSparepartClaim, 0) = (    
       case     
         when @DocPrefix = 'INW' then @IsSprClaim    
         else isnull(d.IsSparepartClaim, 0)    
       end)    
   -- if INC check customer bill    
   and a.CustomerCodeBill like (    
       case     
         when @DocPrefix in  ('INC','INP') then @CustBill    
         else '%'    
       end)    
