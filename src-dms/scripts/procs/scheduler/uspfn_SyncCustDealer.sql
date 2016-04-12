alter procedure uspfn_SyncCustDealer
    @interval int = 3
as

declare @periode as datetime
declare @TCUSTDLR as table (
    CompanyCode varchar(20),
    BranchCode  varchar(20),
    SelectCode  char(1),
    Year        int,
    Month       int,
    NoOfUnitSrv int,
    NoOfUnit    int,
    NoOfService int,
    NoOfPart    int,
    UpdateBy    varchar(20),
    UpdateDate  datetime 
)

-- Insert Data Customer Dealer
set @periode = isnull((select top 1 dateadd(M, -abs(@interval), LastUpdateDate) from gnMstCustDealer where LastUpdateBy = 'SCHED_DLR' and SelectCode = 'A' order by LastUpdateDate desc), 
                      (select top 1 CreatedDate from gnMstCustomer where CreatedDate is not null order by LastUpdateDate))
set @periode = left(convert(varchar, @periode, 112), 6) + '01'
while (@periode < getdate())
begin
    -- Query all customer with/without transaction
    ; with r as (
    select distinct c.CustomerCode, p.ProfitCenterCode 
      from gnMstCustomer c, gnMstCustomerProfitCenter p 
     where c.CompanyCode = p.CompanyCode
       and c.CustomerCode = p.CustomerCode
       and c.Status=1 
       and isnull(c.CreatedDate, '1900-01-01') < dateadd(month, 1, @periode)
    )
    , s as (
    select a.CompanyCode
         , '' as BranchCode
         , 'A' as SelectCode
         , year(@periode) as Year
         , month(@periode) as Month
         , 0 as NoOfUnitSrv
         , ( select count(*) from r where ProfitCenterCode='100' ) as NoOfUnit
         , ( select count(*) from r where ProfitCenterCode='200' ) as NoOfService
         , ( select count(*) from r where ProfitCenterCode='300' ) as NoOfPart
         , 'SCHED_DLR' as UpdateBy
         , getdate() as UpdateDate
      from gnMstOrganizationHdr a
    )
    insert into @TCUSTDLR
    select * from s

    delete gnMstCustDealer where SelectCode = 'A' and Year = year(@periode) and Month = month(@periode)
    set @periode = dateadd(month, 1, @periode);
end

-- Insert Data Customer with Transaction
set @periode = isnull((select top 1 dateadd(M, -abs(@interval), LastUpdateDate) from gnMstCustDealer where LastUpdateBy = 'SCHED_DLR' and SelectCode = 'B' order by LastUpdateDate desc), 
                      (select top 1 CreatedDate from gnMstCustomer where CreatedDate is not null order by LastUpdateDate))
set @periode = left(convert(varchar, @periode, 112), 6) + '01'
while (@periode < getdate())
begin
    -- Query customer with transaction
    ;with r as (
    select 'UNIT&SERVICE' Kode, a.CompanyCode, a.BranchCode, a.CustomerCode
      from omTrSalesInvoice a
     where exists (select 1 from gnMstCustomer where CompanyCode=a.CompanyCode and CustomerCode=a.CustomerCode)
       and a.InvoiceDate < dateadd(month, 1, @periode)
     union
    select 'UNIT&SERVICE' Kode, a.CompanyCode, a.BranchCode, a.CustomerCode
      from svTrnInvoice a
     where exists (select 1 from gnMstCustomer where CompanyCode=a.CompanyCode and CustomerCode=a.CustomerCode)
       and a.InvoiceDate < dateadd(month, 1, @periode)
    )
    , s as (
    select a.CompanyCode
         , a.BranchCode
         , 'B' as SelectCode
         , year(@periode) as Year
         , month(@periode) as Month
         , (select count(distinct CustomerCode) from r
             where r.CompanyCode = a.CompanyCode
               and r.BranchCode = a.BranchCode
             ) as NoOfUnitSrv
         , (select count(distinct x.CustomerCode) from omTrSalesInvoice x
             where x.CompanyCode = a.CompanyCode
               and x.BranchCode = a.BranchCode
               and exists (select 1 from gnMstCustomer where CompanyCode = x.CompanyCode and CustomerCode = x.CustomerCode)
               and x.InvoiceDate < dateadd(month, 1, @periode)
             ) as NoOfUnit
         , (select count(distinct x.CustomerCode) from svTrnInvoice x
             where x.CompanyCode = a.CompanyCode
               and x.BranchCode = a.BranchCode
               and exists (select 1 from gnMstCustomer where CompanyCode = x.CompanyCode and CustomerCode = x.CustomerCode)
               and x.InvoiceDate < dateadd(month, 1, @periode)
             ) as NoOfService
         , (select count(distinct CustomerCode) from spTrnSInvoiceHdr x
             where x.CompanyCode = a.CompanyCode
               and x.BranchCode = a.BranchCode
               and exists (select 1 from gnMstCustomer where CompanyCode = x.CompanyCode and CustomerCode = x.CustomerCode)
               and x.InvoiceDate < dateadd(month, 1, @periode)
             ) as NoOfPart
         , 'SCHED_DLR' as UpdateBy
         , getdate() as UpdateDate
      from gnMstCoProfile a
     group by a.CompanyCode, a.BranchCode
    )
    insert into @TCUSTDLR
    select * from s 

    delete gnMstCustDealer where SelectCode = 'B' and Year = year(@periode) and Month = month(@periode)
    set @periode = dateadd(month, 1, @periode);
end

-- Insert Data Customer Dealer Suzuki
set @periode = isnull((select top 1 dateadd(M, -abs(@interval), LastUpdateDate) from gnMstCustDealer where LastUpdateBy = 'SCHED_DLR' and SelectCode = 'C' order by LastUpdateDate desc), 
                      (select top 1 CreatedDate from gnMstCustomer where CreatedDate is not null order by LastUpdateDate))
set @periode = left(convert(varchar, @periode, 112), 6) + '01'
while (@periode < getdate())
begin
    ;with r as (
    select 'UNIT&SERVICE' Kode, a.CompanyCode, a.BranchCode, a.CustomerCode
      from omTrSalesInvoice a
     where exists (select 1 from gnMstCustomer where CompanyCode=a.CompanyCode and CustomerCode=a.CustomerCode)
       and a.InvoiceDate < dateadd(month, 1, @periode)
       and exists (select top 1 1 from omTrSalesInvoiceVIN
                    where CompanyCode = a.CompanyCode
                      and BranchCode = a.BranchCode
                      and InvoiceNo=a.InvoiceNo
                      and substring(ChassisCode,1,3) in ('JS2','JS3','JS4','JSA','MHY','MA3','MMS'))
     union 
    select 'UNIT&SERVICE' Kode, a.CompanyCode, a.BranchCode, a.CustomerCode
      from svTrnInvoice a
     where exists (select 1 from gnMstCustomer where CompanyCode=a.CompanyCode and CustomerCode=a.CustomerCode)
       and a.InvoiceDate < dateadd(month, 1, @periode)
       and substring(a.ChassisCode,1,3) in ('JS2','JS3','JS4','JSA','MHY','MA3','MMS')
    )
    , s as (
    select a.CompanyCode
         , a.BranchCode
         , 'C' as SelectCode
         , year(@periode) as Year
         , month(@periode) as Month
         , (select count(distinct CustomerCode) from r
             where r.CompanyCode = a.CompanyCode
               and r.BranchCode = a.BranchCode
             ) as NoOfUnitService
         , (select count(distinct x.CustomerCode) from omTrSalesInvoice x
             where x.CompanyCode = a.CompanyCode
               and x.BranchCode = a.BranchCode
               and x.InvoiceDate < dateadd(month, 1, @periode)
               and exists (select 1 from gnMstCustomer where CompanyCode = x.CompanyCode and CustomerCode = x.CustomerCode)
               and exists (select top 1 1 from omTrSalesInvoiceVIN
                              where CompanyCode = a.CompanyCode
                                and BranchCode = a.BranchCode
                                and InvoiceNo = x.InvoiceNo
                                and substring(ChassisCode,1,3) in ('JS2','JS3','JS4','JSA','MHY','MA3','MMS'))
             ) as NoOfUnit
         , (select count(distinct x.CustomerCode) from svTrnInvoice x
             where x.CompanyCode = a.CompanyCode
               and x.BranchCode = a.BranchCode
               and x.InvoiceDate < dateadd(month, 1, @periode)
               and exists (select 1 from gnMstCustomer where CompanyCode = x.CompanyCode and CustomerCode = x.CustomerCode)
               and substring(x.ChassisCode,1,3) in ('JS2','JS3','JS4','JSA','MHY','MA3','MMS')
             ) as NoOfService
         , 0 as NoOfPart
         , 'SCHED_DLR' as UpdateBy
         , getdate() as UpdateDate
      from gnMstCoProfile a
     group by a.CompanyCode, a.BranchCode
    )
    insert into @TCUSTDLR
    select * from s order by BranchCode

    delete gnMstCustDealer where SelectCode = 'C' and Year = year(@periode) and Month = month(@periode)
    set @periode = dateadd(month, 1, @periode);
end

-- Insert Data Customer Dealer Suzuki in Last 3 Years
set @periode = isnull((select top 1 dateadd(M, -abs(@interval), LastUpdateDate) from gnMstCustDealer where LastUpdateBy = 'SCHED_DLR' and SelectCode = 'D' order by LastUpdateDate desc), 
                      (select top 1 CreatedDate from gnMstCustomer where CreatedDate is not null order by LastUpdateDate))
set @periode = left(convert(varchar, @periode, 112), 6) + '01'
while (@periode < getdate())
begin
    ;with r as (
    select 'UNIT&SERVICE' Kode, a.CompanyCode, a.BranchCode, a.CustomerCode
      from omTrSalesInvoice a
     where exists (select 1 from gnMstCustomer where CompanyCode=a.CompanyCode and CustomerCode=a.CustomerCode)
       and a.InvoiceDate < dateadd(month, 1, @periode)
       and a.InvoiceDate >= dateadd(month, -36, @periode)
       and exists (select top 1 1 from omTrSalesInvoiceVIN
                    where CompanyCode = a.CompanyCode
                      and BranchCode = a.BranchCode
                      and InvoiceNo=a.InvoiceNo
                      and substring(ChassisCode,1,3) in ('JS2','JS3','JS4','JSA','MHY','MA3','MMS'))
     union 
    select 'UNIT&SERVICE' Kode, a.CompanyCode, a.BranchCode, a.CustomerCode
      from svTrnInvoice a
     where exists (select 1 from gnMstCustomer where CompanyCode=a.CompanyCode and CustomerCode=a.CustomerCode)
       and a.InvoiceDate < dateadd(month, 1, @periode)
       and a.InvoiceDate >= dateadd(month, -36, @periode)
       and substring(a.ChassisCode,1,3) in ('JS2','JS3','JS4','JSA','MHY','MA3','MMS')
    )
    , s as (
    select a.CompanyCode
         , a.BranchCode
         , 'D' as SelectCode
         , year(@periode) as Year
         , month(@periode) as Month
         , (select count(distinct CustomerCode) from r
             where r.CompanyCode = a.CompanyCode
               and r.BranchCode = a.BranchCode
             ) as NoOfUnitService
         , (select count(distinct x.CustomerCode) from omTrSalesInvoice x
             where x.CompanyCode = a.CompanyCode
               and x.BranchCode = a.BranchCode
               and x.InvoiceDate < dateadd(month, 1, @periode)
               and x.InvoiceDate >= dateadd(month, -36, @periode)
               and exists (select 1 from gnMstCustomer where CompanyCode = x.CompanyCode and CustomerCode = x.CustomerCode)
               and exists (select top 1 1 from omTrSalesInvoiceVIN
                              where CompanyCode = a.CompanyCode
                                and BranchCode = a.BranchCode
                                and InvoiceNo = x.InvoiceNo
                                and substring(ChassisCode,1,3) in ('JS2','JS3','JS4','JSA','MHY','MA3','MMS'))
             ) as NoOfUnit
         , (select count(distinct x.CustomerCode) from svTrnInvoice x
             where x.CompanyCode = a.CompanyCode
               and x.BranchCode = a.BranchCode
               and x.InvoiceDate < dateadd(month, 1, @periode)
               and x.InvoiceDate >= dateadd(month, -36, @periode)
               and exists (select 1 from gnMstCustomer where CompanyCode = x.CompanyCode and CustomerCode = x.CustomerCode)
               and substring(x.ChassisCode,1,3) in ('JS2','JS3','JS4','JSA','MHY','MA3','MMS')
             ) as NoOfService
         , 0 as NoOfPart
         , 'SCHED_DLR' as UpdateBy
         , getdate() as UpdateDate
      from gnMstCoProfile a
     group by a.CompanyCode, a.BranchCode
    )
    insert into @TCUSTDLR
    select * from s order by BranchCode

    delete gnMstCustDealer where SelectCode = 'D' and Year = year(@periode) and Month = month(@periode)
    set @periode = dateadd(month, 1, @periode);
end

-- insert data info gnMstCustDealer
insert into gnMstCustDealer
select * from @TCUSTDLR

-- Info Summary Data
select SelectCode, count(*) as Data from @TCUSTDLR group by SelectCode order by SelectCode
select SelectCode
     , sum(NoOfUnit) Unit
     , sum(NoOfService) Service
     , sum(NoOfSparePart) Sparepart
     , sum(NoOfUnitService) UnitService
  from GnMstCustDealer
 where Year = year(getdate()) and Month = month(getdate())
 group by SelectCode order by SelectCode

--go
--uspfn_SyncCustDealer
