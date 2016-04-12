
go
if object_id('uspfn_SOLkuColourModelList') is not null
	drop procedure uspfn_SOLkuColourModelList

go
create procedure uspfn_SOLkuColourModelList
	@CompanyCode varchar(13),
	@BranchCode varchar(13),
	@SONumber varchar(35),
	@SalesModelCode varchar(35),
	@SalesModelYear decimal
as

begin
	select a.*
	     , b.SalesModelDesc
		 , (
				SELECT b.RefferenceDesc1
				  FROM omMstRefference b
				 WHERE b.RefferenceCode = a.ColourCode
				   AND b.CompanyCode = a.CompanyCode 
				   AND b.RefferenceType = 'COLO'
		   ) AS ColourDesc
	  from OmTrSalesSOModelColour a
	  inner join omMstModelYear b
      	on b.CompanyCode = a.CompanyCode 
	   and b.SalesModelCode = a.SalesModelCode
	   and b.SalesModelYear = a.SalesModelYear
	   and b.Status in ('1', '2') 
	 --inner join OmMstModelColour c
	 --   on c.CompanyCode = a.CompanyCode
	 --  and c.SalesModelCode = b.SalesModelCode
	 --  and c.ColourCode = a.ColourCode 
	 where a.CompanyCode = @CompanyCode
	   and a.BranchCode = @BranchCode
	   and a.SONo = @SONumber
	   and a.SalesModelCode = @SalesModelCode
	   and a.SalesModelYear = @SalesModelYear
end



go
exec uspfn_SOLkuColourModelList '6115204', '611520402', 'SOC/11/000001', 'FW110SC', 2011


--select * from omMstModel

--select * from gnMstCoProfile
--select * from omTr

--select * from omMstModelColour