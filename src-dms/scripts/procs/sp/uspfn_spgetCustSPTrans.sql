create procedure uspfn_spgetCustSPTrans --'PLS/13/016078','1000557'  
@PickingSlipNo varchar(25),  
@CustomerCode varchar(25)  
as  
SELECT a.CustomerCode, a.PickingSlipNo, b.CustomerCode, b.CustomerName, b.Address1, b.Address2, b.Address3 FROM spTrnSInvoiceHdr a  
               inner join gnMstCustomer b on a.CompanyCode = b.CompanyCode  
               AND a.CustomerCode = a.CustomerCode  
               where a.pickingSlipNo = @PickingSlipNo --'PLS/13/016078'  
               AND b.CustomerCode = @CustomerCode --'1000557'  