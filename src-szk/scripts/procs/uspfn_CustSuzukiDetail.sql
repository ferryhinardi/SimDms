USE [SimDms]
GO
/****** Object:  StoredProcedure [dbo].[uspfn_CustSuzukiDetail]    Script Date: 2014-09-01 11:26:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--SELECT * FROM gnMstCustDealerDtl          
--order by LastTransactionDate asc          
-- uspfn_CustSuzukiDetail '6006406','6006406','%','01-01-2000','12-12-2013'           
--SELECT * FROM gnMstCustDealerDtl          
ALTER procedure [dbo].[uspfn_CustSuzukiDetail]          
@CompanyCode varchar(15),        
@BranchCode varchar(15),          
@CustType varchar(15),          
@TransDateStart datetime,          
@TransDateEnd datetime          
as          
--declare @BranchCode varchar(15)          
--set @BranchCode = '%'          
--declare @CustomerCode varchar(15)          
--set @CustomerCode = '%'          
--declare @TransDateStart datetime          
--set @TransDateStart = '2013-03-25'          
--declare @TransDateEnd datetime          
--set @TransDateEnd = '2013-03-26'        
--select * from gnMstCoProfile      
--select * from DealerInfo    
IF @CustType != 'D'
begin
select DISTINCT c.ShortName CompanyName, b.CompanyName BranchName, a.CustomerCode, a.CustomerName, a.CustomerGovName,(case a.CustomerStatus WHEN 'A' THEN 'UNIT&SERVICE' WHEN 'B' THEN  'UNIT ONLY' WHEN 'C' THEN 'SERVICE ONLY' WHEN 'D' THEN  'NEITHER UNIT&SERVICE' end)CustomerStatus, a.Address, a.ProvinceName,          
 a.CityName, a.ZipNo, a.KelurahanDesa, a.KecamatanDistrik, a.KotaKabupaten, a.IbuKota, a.PhoneNo, a.HpNo,          
 a.FaxNo, a.OfficePhoneNo, a.Email, a.BirthDate, (case a.IsPkp WHEN 1 THEN 'Ya' WHEN 0 THEN 'Tidak' END)IsPkp, a.NPWPNo, a.NPWPDate, a.SKPNo, a.SKPDate, a.CustomerType,          
 (case a.Gender WHEN 'M' THEN 'Pria' else 'Wanita' END), a.KTP from gnMstCustDealerDtl a          
 join gnMstCoProfile b ON a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode       
 join DealerInfo c   ON a.CompanyCode = c.DealerCode      
 where a.CompanyCode like @CompanyCode        
 and a.BranchCode like @BranchCode          
 and a.TransType like @CustType
 and a.LastTransactionDate >= @TransDateStart and a.LastTransactionDate <= @TransDateEnd          
 and c.ProductType = '4W'  
 order BY a.CustomerCode        
end
else
begin
	select DISTINCT c.ShortName CompanyName, b.CompanyName BranchName, a.CustomerCode, a.CustomerName, a.CustomerGovName,(case a.CustomerStatus WHEN 'A' THEN 'UNIT&SERVICE' WHEN 'B' THEN  'UNIT ONLY' WHEN 'C' THEN 'SERVICE ONLY' WHEN 'D' THEN  'NEITHER UNIT&SERVICE' end)CustomerStatus, a.Address, a.ProvinceName,          
 a.CityName, a.ZipNo, a.KelurahanDesa, a.KecamatanDistrik, a.KotaKabupaten, a.IbuKota, a.PhoneNo, a.HpNo,          
 a.FaxNo, a.OfficePhoneNo, a.Email, a.BirthDate, (case a.IsPkp WHEN 1 THEN 'Ya' WHEN 0 THEN 'Tidak' END)IsPkp, a.NPWPNo, a.NPWPDate, a.SKPNo, a.SKPDate, a.CustomerType,          
 (case a.Gender WHEN 'M' THEN 'Pria' else 'Wanita' END), a.KTP from gnMstCustDealerDtl a          
 join gnMstCoProfile b ON a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode       
 join DealerInfo c   ON a.CompanyCode = c.DealerCode      
 where a.CompanyCode like @CompanyCode        
 and a.BranchCode like @BranchCode          
 and a.CustomerStatus like @CustType          
 and a.LastTransactionDate >= @TransDateStart and a.LastTransactionDate <= @TransDateEnd          
 and c.ProductType = '4W'  
 order BY a.CustomerCode        
end      
select case when @CompanyCode = '%' then 'ALL'  else h.CompanyName end Company, case when @BranchCode = '%' then 'ALL'  else d.CompanyName end Branch,      
case when @TransDateStart = '01-01-1900' THEN 'ALL PERIOD' else  CONVERT(CHAR(12), @TransDateEnd , 106)end PeriodeStart, case when @TransDateEnd = '12-12-2100' THEN 'ALL PERIOD' else  CONVERT(CHAR(12), @TransDateEnd , 106) end PeriodeEnd,      
 'Customer Suzuki Detail with last transaction' InquiryType,      
 CASE WHEN @CustType = '%' THEN 'ALL'       
 when @CustType = 'UNITSERVICE' then 'Buy Unit & Service'       
 when @CustType = 'UNIT' then 'Buy Unit Only'      
 when @CustType = 'D' then 'Not Buy Unit & Service'      
 when @CustType = 'SERVICE' then 'Service Only' end StatusInq      
from gnMstCoProfile d      
join gnMstOrganizationHdr h ON d.CompanyCode = h.CompanyCode      
where d.BranchCode like @BranchCode 
and d.CompanyCode like @CompanyCode