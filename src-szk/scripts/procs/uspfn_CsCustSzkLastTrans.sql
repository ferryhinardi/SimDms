alter procedure uspfn_CsCustSzkLastTrans
    @CompanyCode varchar(15),                
    @BranchCode varchar(15),                  
    @Year int,                  
    @Month int
as                  

;with x as (                
select b.ShortName as CompanyAbbr
     , a.CompanyCode
     , a.CompanyName as BranchName
     , c.NoOfUnitService
     , c.NoOfUnit
     , c.NoOfService
  from gnMstCoProfile a
  left join DealerInfo b
    on b.DealerCode = a.CompanyCode
  left join gnMstCustDealer c
    on c.CompanyCode = a.CompanyCode
   and c.BranchCode = a.BranchCode
   and c.SelectCode = 'D'
 where 1 = 1
   and a.CompanyCode like @CompanyCode
   and a.BranchCode like @BranchCode
   and Year = @Year
   and Month = @Month
)
select * into #t1 from x

select * from #t1
select a.CompanyAbbr
     , b.DealerName as CompanyName
     , sum(a.NoOfUnitService) NoOfUnitService
     , sum(a.NoOfUnit) NoOfUnit
     , sum(a.NoOfService) NoOfService 
  from #t1 a
  left join DealerInfo b
    on b.DealerCode = a.CompanyCode
 group by a.CompanyCode, a.CompanyAbbr, b.DealerName

go


exec uspfn_CsCustSzkLastTrans '%', '%', 2014, 8
