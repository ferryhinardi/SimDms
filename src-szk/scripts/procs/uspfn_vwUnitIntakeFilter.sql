alter procedure [dbo].[uspfn_vwUnitIntakeFilter] 
as 

select distinct AreaCode as value
     , Area as text 
  from SvUnitIntake 
 where AreaCode is not null
 order by Area asc

select distinct convert(varchar, AreaCode, 3) + ' - ' + CompanyCode as value
     , CompanyCode + ' - '+ svMstDealerMapping.DealerName as text    
	 , AreaCode as [group]  
  from SvUnitIntake
  inner join svMstDealerMapping on svMstDealerMapping.DealerCode = SvUnitIntake.CompanyCode
 where not AreaCode is null 
  order by convert(varchar, AreaCode, 3) + ' - ' + CompanyCode

select distinct convert(varchar, b.GroupNo, 3) + ' - ' + a.CompanyCode as [group]	 
     , value = convert(varchar, b.GroupNo, 3) + ' - ' + a.BranchCode
     , text = a.BranchCode + ' - ' + isnull(c.OutletAbbreviation, '')
  from SvUnitIntake a
  inner join svMstDealerMapping b on
	b.DealerCode = a.CompanyCode
  inner join svMstDealerOutletMapping c
    on c.DealerCode = a.CompanyCode
   and c.OutletCode = a.BranchCode
   and c.GroupNo = b.GroupNo
 order by a.BranchCode + ' - ' + isnull(c.OutletAbbreviation, '')
