
go
if object_id('uspfn_PmItsByPerolehanDataFilter0') is not null
	drop procedure uspfn_PmItsByPerolehanDataFilter0

go
create procedure uspfn_PmItsByPerolehanDataFilter0
as 
begin
	select Area as value
		 , Area as text 
	  from PmITSByLostCase1
	 where Area is not null
	 group by Area
	 order by Area asc;

	select CompanyCode as value
		 , DealerAbbreviation as text
		 , Area as [group]  
	  from PmITSByLostCase1
	 group by Area, CompanyCode, DealerAbbreviation
	 order by DealerAbbreviation

	select BranchCode as value
		 , OutletAbbreviation as text
		 , CompanyCode as [group]  
	  from PmITSByLostCase1 a
	 group by CompanyCode, BranchCode, OutletAbbreviation
	 order by OutletAbbreviation
end

 go
 exec uspfn_PmItsByPerolehanDataFilter0 
 --exec uspfn_PmItsByPerolehanDataFilter1