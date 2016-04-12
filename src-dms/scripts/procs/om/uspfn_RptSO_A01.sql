
go
if object_id('uspfn_RptSO_A01_Rpt') is not null
	drop procedure uspfn_RptSO_A01_Rpt

go
create procedure uspfn_RptSO_A01_Rpt
	@CompanyCode varchar(13),
	@BranchCode varchar(13),
	@SONumber varchar(35)
as
begin
	select a.SONo as SONumber
	     , a.SODate 
		 , a.SKPKNo as SKPKNumber
		 , a.RefferenceNo as ReffNumber
		 , a.CustomerCode 
		 , b.CustomerName
		 , b.CustomerGovName
		 , Address = ( ltrim(b.Address1) + ' ' + ltrim(b.Address2) + ' ' + ltrim(b.Address3) + ltrim(b.Address4))
		 , b.Address1
		 , b.Address2
		 , b.Address3
		 , b.Address4
		 , b.NPWPNo 
		 , b.NPWPDate
		 , a.LeasingCo
		 , c.CustomerCode as LeasingCode
		 , c.CustomerName as LeasingName
		 , d.TOPCode
		 , e.LookUpValueName as TOPDesc
		 --, g.LookUpValueName as City
		 , City = (
				select top 1
				       y.LookupValueName
				  from gnMstCoProfile x
				 inner join gnMstLookupDtl y
				    on y.CompanyCode = x.CompanyCode
				   and y.CodeID = 'CITY'
				   and y.LookupValue = x.CityCode
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
		   )
	  from OmTrSalesSO a
	 inner join gnMstCustomer b
	    on b.CompanyCode = a.CompanyCode
	   and b.CustomerCode = a.CustomerCode
	  left join gnMstCustomer c
	    on c.CompanyCode = a.CompanyCode
	   and c.CategoryCode = 32
	   and c.CustomerCode = a.LeasingCo
	  left join gnMstCustomerProfitCenter d
	    on d.CompanyCode = a.CompanyCode
	   and d.BranchCode = a.BranchCode
	   and d.CustomerCode = c.CustomerCode
	  left join gnMstLookupDtl e
	    on e.CompanyCode = a.CompanyCode
	   and e.CodeID = 'TOPC'
	   and e.LookUpValue = d.TOPCode
	  left join gnMstCoProfile f
	    on f.CompanyCode = e.CompanyCode
	   and f.BranchCode = d.BranchCode
	  left join gnMstLookUpDtl g
	    on g.CompanyCode = f.CompanyCode
	   and g.CodeID = 'CITY'
	   and g.LookUpValue = f.CityCode
	 where a.CompanyCode = @CompanyCode
	   and a.BranchCode = @BranchCode
	   and a.SONo = @SONumber
end

go
exec uspfn_RptSO_A01_Rpt '6115204', '611520401', 'SOB/11/000009'





go
if object_id('uspfn_RptSO_A01_Dtl') is not null
	drop procedure uspfn_RptSO_A01_Dtl

go
create procedure uspfn_RptSO_A01_Dtl
	@CompanyCode varchar(13),
	@BranchCode varchar(13),
	@SONumber varchar(35)
as
begin
	select b.SalesModelCode
	     , b.SalesModelYear
		 , c.ColourCode
		 , e.RefferenceDesc1 as ColourName
		 , b.ChassisCode
		 , d.ChassisNo
		 , QuantitySO = (select sum(x.QuantitySO) from omTrSalesSoModel x where x.CompanyCode = a.CompanyCode and x.SONo = a.SONo)
	  from omTrSalesSo a
	 inner join omTrSalesSoModel b
	    on b.CompanyCode = a.CompanyCode
	   and b.BranchCode = a.BranchCode
	   and b.SONo = a.SONo
	 inner join OmTrSalesSOModelColour c
	    on c.CompanyCode = b.CompanyCode
	   and c.BranchCode = b.BranchCode
	   and c.SONo = b.SONo
	   and c.SalesModelCode = b.SalesModelCode
	   and c.SalesModelYear = b.SalesModelYear
	 inner join omTrSalesSOVin d
	    on d.CompanyCode = b.CompanyCode
	   and d.BranchCode = b.BranchCode
	   and d.SONo = b.SONo
	   and d.SalesModelCode = b.SalesModelCode
	   and d.SalesModelYear = b.SalesModelYear
	   and d.ChassisCode = b.ChassisCode
	   and d.ColourCode = c.ColourCode
	  left join omMstRefference e
	    on e.CompanyCode = d.CompanyCode
	   and e.RefferenceType = 'COLO'
	   and e.RefferenceCode = c.ColourCode
	 where a.CompanyCode = @CompanyCode
	   and a.BranchCode = @BranchCode
	   and a.SONo = @SONumber
end

go
exec uspfn_RptSO_A01_Dtl '6115204', '611520401', 'SOB/11/000001'



