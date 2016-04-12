
go
if object_id('uspfn_PmItsByLostCaseFilter') is not null
	drop procedure uspfn_PmItsByLostCaseFilter

go
create procedure uspfn_PmItsByLostCaseFilter
	@Filter char(1) = '0' 
as 
begin
	select distinct 
	       [text] = a.Area
		 , [value] = a.AreaCode
	  from PmITSByLostCase0 a
	 order by a.Area asc;

	select distinct
	       [text] = b.DealerName
		 , [value] = a.CompanyCode
		 , [group] = a.AreaCode
      from PmITSByLostCase0 a
	 inner join DealerInfo b
	    on b.DealerCode = a.CompanyCode
     order by b.DealerName asc;

	 select distinct 
	        [text] = a.OutletAbbreviation
		  , [value] = a.BranchCode
		  , [group] = a.CompanyCode
	   from PmITSByLostCase0 a
	  order by a.OutletAbbreviation asc;
	
end





 go
 exec uspfn_PmItsByLostCaseFilter 1
