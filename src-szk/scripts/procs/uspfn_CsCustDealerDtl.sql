alter procedure uspfn_CsCustDealerDtl
    @CompanyCode varchar(15),                
    @BranchCode  varchar(15),    
    @CustType    varchar(20),              
    @DateFrom    date,                  
    @DateTo      date
as                  
         

;with x as (
 select distinct c.shortname
      , b.companyname
      , a.customercode
      , a.customername
      , a.customergovname
      , ( case a.customerstatus 
            when 'A' then 'UNIT&SERVICE' 
            when 'B' then 'UNIT ONLY' 
            when 'C' then 'SERVICE ONLY' 
            when 'D' then 'NEITHER UNIT&SERVICE' 
          end ) CustomerStatus
      , a.address
      , a.provincename
      , a.cityname
      , a.zipno
      , a.kelurahandesa
      , a.kecamatandistrik
      , a.kotakabupaten
      , a.ibukota
      , a.phoneno
      , a.hpno
      , a.faxno
      , a.officephoneno
      , a.email
      , a.birthdate
      , ( case a.ispkp 
            when 1 then 'Ya' 
            when 0 then 'Tidak' 
          end ) IsPkp
      , a.npwpno
      , a.npwpdate
      , a.skpno
      , a.skpdate
      , a.customertype
      , a.TransType
      , ( case a.gender
            when 'M' then 'Pria' 
            when 'L' then 'Pria' 
            else 'Wanita' 
          end ) as Gender
      , a.ktp
   from gnmstcustdealerdtl a 
   join gnmstcoprofile b 
     on a.companycode = b.companycode 
    and a.branchcode = b.branchcode 
   join dealerinfo c 
     on a.companycode = c.dealercode 
  where a.companycode like @CompanyCode 
    and a.branchcode like @BranchCode 
    --and a.transtype like @CustType 
    and a.lasttransactiondate >= @DateFrom
    and a.lasttransactiondate <= @DateTo
    and c.producttype = '4W' 
)
select x.ShortName [Company Name]
     , x.CompanyName [Branch Name]
     , x.CustomerName [Customer Name]
     , x.CustomerGovName [Customer Gov Name]
     , x.TransType [Customer Type]
     , x.Address
     , x.ProvinceName [Province]
     , x.CityName [Ciy]
     , x.ZipNo
     , x.KelurahanDesa [Kelurahan]
     , x.KecamatanDistrik [Kecamatan]
     , x.IbuKota [Ibu Kota]
     , x.PhoneNo [Phone No]
     , x.HpNo [HP No]
     , x.FaxNo [Fax No]
     , x.OfficePhoneNo [Office Phone No]
     , x.Email
     , x.BirthDate [BirthDate]
     , x.IsPkp
     , x.NPWPNo [NPWP No]
     , x.NPWPDate [NPWP Date]
     , x.SKPNo [SKP No]
     , x.SKPDate [SKP Date]
     , x.Gender
     , x.KTP
  from x

go

exec uspfn_CsCustDealerDtl '6021406','%','SERVICE','01-01-1900','12-12-2100'