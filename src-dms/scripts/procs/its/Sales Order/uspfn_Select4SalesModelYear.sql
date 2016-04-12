go
if object_id('uspfn_Select4SalesModelYear') is not null
	drop procedure uspfn_Select4SalesModelYear

go
create procedure uspfn_Select4SalesModelYear
	@CompanyCode varchar(17),
	@SalesModelCode varchar(25),
	@GroupPriceCode varchar(25)
as
	
begin
	SELECT DISTINCT 
		   a.*  
      FROM omMstModelYear a  
	  LEFT JOIN omMstPriceListSell b  
	    ON a.CompanyCode = b.CompanyCode  
	   AND a.SalesModelCode = b.SalesModelCode  
	   AND a.SalesModelYear = b.SalesModelYear  
	   AND b.GroupPriceCode = @GroupPriceCode  
	 WHERE a.CompanyCode = @CompanyCode  
	   AND a.SalesModelCode = @SalesModelCode  
	   AND a.Status = '1'  ORDER BY a.SalesModelYear ASC
end













go
exec uspfn_Select4SalesModelYear '6115204', 'A100XS/VR', 'DC'
