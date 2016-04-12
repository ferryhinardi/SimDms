alter procedure uspfn_CsCustDealer
    @CompanyCode varchar(20),                
    @BranchCode  varchar(20),
    @InqType     varchar(20),
    @Year        int,                  
    @Month       int
as                  

if @InqType = 'A'
begin
    ;with x as (                
    select a.CompanyCode
         , b.ShortName as CompanyAbbr
         , b.DealerName CompanyName
         , a.NoOfUnit
         , a.NoOfService
         , a.NoOfSparePart
      from gnMstCustDealer a
      left join DealerInfo b
        on b.DealerCode = a.CompanyCode
     where 1 = 1
       and a.CompanyCode like @CompanyCode
       and a.SelectCode = @InqType
       and Year = @Year
       and Month = @Month
    )
    select * from x
end
else 
begin
    ;with x as (                
    select b.ShortName as CompanyAbbr
         , a.CompanyCode
         , a.CompanyName as BranchName
         , c.NoOfUnitService
         , c.NoOfUnit
         , c.NoOfService
         , c.NoOfSparePart
      from gnMstCoProfile a
      left join DealerInfo b
        on b.DealerCode = a.CompanyCode
      left join gnMstCustDealer c
        on c.CompanyCode = a.CompanyCode
       and c.BranchCode = a.BranchCode
       and c.SelectCode = @InqType
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
         , sum(a.NoOfSparePart) NoOfSparePart
      from #t1 a
      left join DealerInfo b
        on b.DealerCode = a.CompanyCode
     group by a.CompanyCode, a.CompanyAbbr, b.DealerName
end

go


exec uspfn_CsCustDealer '%', '%', 'A', 2014, 9
