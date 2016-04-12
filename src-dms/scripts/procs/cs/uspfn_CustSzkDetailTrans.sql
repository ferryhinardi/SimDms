  
  
-- SP to get CSZKTRDET        
CREATE Procedure uspfn_CustSzkDetailTrans            
--uspfn_CustSzkDetailTrans '2012-01-01', 100            
@LastUpdateDate datetime,            
@segment int            
as       
DECLARE @LastUpdateBy varchar(8)            
SET @LastUpdateBy ='AUTOUPDT'            
declare @LastUpdateDateTmp datetime            
set @LastUpdateDateTmp = @LastUpdateDate            
-- Query all Suzuki customer (3 digits VIN : JS2, JS3, JS4, JSA, MHY, MA3 & MMS)            
-- depend on transaction period, examples: last transaction in 3 years            
-- Customer detail information             
   select * into #t14 from            
        ( select distinct 'UNIT' Kode, a.CompanyCode,            
                 a.BranchCode, b.CustomerCode, b.CustomerName, b.CustomerGovName, b.CustomerStatus,             
                 b.Address1+b.Address2+b.Address3+b.Address4 Address,            
                 isnull((select top 1 LookUpValueName from gnMstLookUpDtl             
                          where CompanyCode=a.CompanyCode and CodeID='PROV' and LookUpValue=b.ProvinceCode),'') ProvinceName,             
                 isnull((select top 1 LookUpValueName from gnMstLookUpDtl             
                          where CompanyCode=a.CompanyCode and CodeID='CITY' and LookUpValue=b.CityCode),'') CityName,            
                 b.ZipNo, b.KelurahanDesa, b.KecamatanDistrik, b.KotaKabupaten, b.IbuKota,             
                 b.PhoneNo, b.HPNo, b.FaxNo, b.OfficePhoneNo, b.Email, b.BirthDate, b.isPKP,             
                 b.NPWPNo, b.NPWPDate, b.SKPNo, b.SKPDate, b.CustomerType, b.Gender, b.Spare01 KTP, a.InvoiceDate            
            from omTrSalesInvoice a, gnMstCustomer b, gnMstDealerMapping c            
           where a.CompanyCode=b.CompanyCode and a.CustomerCode=b.CustomerCode and a.CompanyCode =c.DealerCode            
           --and a.InvoiceDate>=dateadd(year,-3,(select convert(date,dateadd(mm,datediff(mm,0,getdate()),0))))            
             and a.InvoiceDate> @LastUpdateDate            
             and exists (select top 1 1 from omTrSalesInvoiceVIN            
                          where CompanyCode=a.CompanyCode and BranchCode=a.BranchCode and InvoiceNo=a.InvoiceNo            
                            and substring(ChassisCode,1,3) in ('JS2','JS3','JS4','JSA','MHY','MA3','MMS')) ) #t14            
                
   select * into #t15 from            
        ( select distinct 'SERVICE' Kode,  a.CompanyCode,            
                 a.BranchCode, b.CustomerCode, b.CustomerName, b.CustomerGovName, b.CustomerStatus,             
                 b.Address1+b.Address2+b.Address3+b.Address4 Address,            
                 isnull((select top 1 LookUpValueName from gnMstLookUpDtl             
                          where CompanyCode=a.CompanyCode and CodeID='PROV' and LookUpValue=b.ProvinceCode),'') ProvinceName,             
                 isnull((select top 1 LookUpValueName from gnMstLookUpDtl             
                          where CompanyCode=a.CompanyCode and CodeID='CITY' and LookUpValue=b.CityCode),'') CityName,            
                 b.ZipNo, b.KelurahanDesa, b.KecamatanDistrik, b.KotaKabupaten, b.IbuKota,             
                 b.PhoneNo, b.HPNo, b.FaxNo, b.OfficePhoneNo, b.Email, b.BirthDate, b.isPKP,             
                 b.NPWPNo, b.NPWPDate, b.SKPNo, b.SKPDate, b.CustomerType, b.Gender, b.Spare01 KTP, a.InvoiceDate            
            from svTrnInvoice a, gnMstCustomer b, gnMstDealerMapping c            
           where a.CompanyCode=b.CompanyCode and a.CustomerCode=b.CustomerCode and a.CompanyCode =c.DealerCode            
           --and a.InvoiceDate>=dateadd(year,-3,(select convert(date,dateadd(mm,datediff(mm,0,getdate()),0))))            
             and a.InvoiceDate>@LastUpdateDate            
             and substring(a.ChassisCode,1,3) in ('JS2','JS3','JS4','JSA','MHY','MA3','MMS')) #t15            
            
   select * into #t16 from         
        ( select 'UNITSERVICE' Kode,  CompanyCode, BranchCode, CustomerCode, CustomerName,             
                 CustomerGovName, CustomerStatus, Address, ProvinceName, CityName, ZipNo, KelurahanDesa,             
                 KecamatanDistrik, KotaKabupaten, IbuKota, PhoneNo, HPNo, FaxNo, OfficePhoneNo, Email,             
                 BirthDate, isPKP, NPWPNo, NPWPDate, SKPNo, SKPDate, CustomerType, Gender, KTP, InvoiceDate from #t14            
    union             
          select 'UNITSERVICE' Kode, CompanyCode, BranchCode, CustomerCode, CustomerName,             
                 CustomerGovName, CustomerStatus, Address, ProvinceName, CityName, ZipNo, KelurahanDesa,             
                 KecamatanDistrik, KotaKabupaten, IbuKota, PhoneNo, HPNo, FaxNo, OfficePhoneNo, Email,             
                 BirthDate, isPKP, NPWPNo, NPWPDate, SKPNo, SKPDate, CustomerType, Gender, KTP, InvoiceDate from #t15 ) #t16            
            
             
 SELECT * into #t17 FROM             
 (            
   select distinct * from #t16            
   union all            
   select * from #t14            
   union all            
   select * from #t15) #t17            
             
 select * into #t18 from(            
 select TOP (@segment) * from #t17 a            
 where a.InvoiceDate > @LastUpdateDate order BY a.InvoiceDate asc)#t18            
             
 set @LastUpdateDate = (select TOP 1 InvoiceDate from #t18 a order BY a.InvoiceDate desc)            
  
begin try   
 insert INTO gnMstCustDealerDtl (CompanyCode, BranchCode, CustomerCode,             
         CustomerName, CustomerGovName, CustomerStatus,             
         Address, ProvinceName, CityName, ZipNo,             
         KelurahanDesa, KecamatanDistrik, KotaKabupaten,             
         IbuKota, PhoneNo, HpNo, FaxNo, OfficePhoneNo,            
         Email, BirthDate, isPKP, NPWPNo, NPWPDate, SKPNo,            
         SKPDate, CustomerType, Gender, KTP,             
         LastTransactionDate, TransType, LastUpdateBy, LastUpdateDate)            
 SELECT TOP (@segment) a.CompanyCode, a.BranchCode, a.CustomerCode, a.CustomerName, a.CustomerGovName,            
  a.CustomerStatus, a.Address, a.ProvinceName, a.CityName, a.ZipNo, a.KelurahanDesa,            
  a.KecamatanDistrik, a.KotaKabupaten, a.Ibukota, a.PhoneNo, a.HpNo, a.FaxNo,            
  a.OfficePhoneNo, a.Email, a.BirthDate, a.isPKP, a.NPWPNo, a.NPWPDate, a.SKPNo, a.SKPDate,            
  a.CustomerType, a.Gender, a.KTP, a.InvoiceDate, a.Kode, @LastUpdateBy, @LastUpdateDate            
 from #t18 a              
 end try            
 begin catch            
 end catch            
                       
 begin try            
 insert INTO gnMstCustDealerDtl (CompanyCode, BranchCode, CustomerCode,             
         CustomerName, CustomerGovName, CustomerStatus,             
         Address, ProvinceName, CityName, ZipNo,             
         KelurahanDesa, KecamatanDistrik, KotaKabupaten,             
         IbuKota, PhoneNo, HpNo, FaxNo, OfficePhoneNo,            
         Email, BirthDate, isPKP, NPWPNo, NPWPDate, SKPNo,            
         SKPDate, CustomerType, Gender, KTP,             
         LastTransactionDate, TransType, LastUpdateBy, LastUpdateDate)            
 SELECT TOP (@segment) a.CompanyCode, a.BranchCode, a.CustomerCode, a.CustomerName, a.CustomerGovName,            
  a.CustomerStatus, a.Address, a.ProvinceName, a.CityName, a.ZipNo, a.KelurahanDesa,            
  a.KecamatanDistrik, a.KotaKabupaten, a.Ibukota, a.PhoneNo, a.HpNo, a.FaxNo,            
  a.OfficePhoneNo, a.Email, a.BirthDate, a.isPKP, a.NPWPNo, a.NPWPDate, a.SKPNo, a.SKPDate,            
  a.CustomerType, a.Gender, a.KTP, a.InvoiceDate, a.Kode, @LastUpdateBy, @LastUpdateDate            
 from #t17 a            
 where a.Kode = (SELECT top 1 Kode from #t18 b order BY b.InvoiceDate desc)            
 and a.InvoiceDate = (SELECT top 1 InvoiceDate from #t18 b order BY b.InvoiceDate desc)            
 end try            
 begin catch            
 end catch            
             
 drop table #t14, #t15, #t16, #t17, #t18            
             
 select top(@segment) a.CompanyCode, a.BranchCode, a.CustomerCode, a.CustomerName, a.CustomerGovName,            
  a.CustomerStatus, a.Address, a.ProvinceName, a.CityName, a.ZipNo, a.KelurahanDesa,            
  a.KecamatanDistrik, a.KotaKabupaten, a.Ibukota, a.PhoneNo, a.HpNo, a.FaxNo,            
  a.OfficePhoneNo, a.Email, a.BirthDate, a.isPKP, a.NPWPNo, a.NPWPDate, a.SKPNo, a.SKPDate,            
  a.CustomerType, a.Gender, a.KTP, a.LastTransactionDate, a.TransType, a.LastUpdateBy, a.LastUpdateDate  from gnMstCustDealerDtl a where a.LastTransactionDate > @LastUpdateDateTmp            
    
    