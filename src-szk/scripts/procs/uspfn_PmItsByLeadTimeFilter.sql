alter procedure uspfn_PmItsByLeadTimeFilter 
as 

select Area as value
     , Area as text 
  from PmItsByLeadTime
 where Area is not null
 group by Area
 order by Area asc;

select CompanyCode as value
     , DealerAbbreviation as text
	 , Area as [group]  
  from PmItsByLeadTime
 group by Area, CompanyCode, DealerAbbreviation
 order by DealerAbbreviation

select BranchCode as value
     , OutletAbbreviation as text
	 , CompanyCode as [group]  
  from PmItsByLeadTime
 group by CompanyCode, BranchCode, OutletAbbreviation
 order by OutletAbbreviation

 go

 uspfn_PmItsByLeadTimeFilter
