CREATE procedure uspfn_spGetSpTrnSFPJHdr @CompanyCode varchar(15), @BranchCode varchar(15)    
as    
select a.FPJNo, a.FPJdate, a.FPJGovNo, a.InvoiceNo, a.PickingSlipNo, a.CustomerCode, b.CustomerName, b.Address1, b.Address2, b.Address3, b.Address4, a.TOPDays, a.TOPCode from spTrnSFPJHdr a    
join gnMstCustomer b    
on b.CustomerCode = a.CustomerCode    
and b.CompanyCode = a.CompanyCode    
where a.CompanyCode = @CompanyCode    
and a.BranchCode = @BranchCode 
  