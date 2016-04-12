alter procedure uspfn_SpPartSalesFilter
	@ProductType varchar(20) = '4W'
as 

declare @t_dealer as table (CompanyCode varchar(20))
 insert into @t_dealer 
 select CompanyCode from SpHstPartSales group by CompanyCode

 select c.Area as value, c.Area as text
   from DealerInfo b
   join GnMstDealerMapping c
     on c.DealerCode = b.DealerCode
  where b.ProductType = @ProductType
  group by c.GroupNo, c.Area
  order by c.GroupNo


 select b.DealerCode as value, b.DealerCode + ' - ' + b.DealerName as text, c.Area as 'group', b.ProductType as 'group2'
   from DealerInfo b
   join GnMstDealerMapping c
     on c.DealerCode = b.DealerCode
  where b.ProductType = @ProductType
  group by b.DealerCode, b.DealerName, c.Area, b.ProductType

select LookUpValue as value
     , LookUpValue + ' - ' + LookUpValueName as text
  from MstLookUpDtl
 where CodeID = 'TPGO'

go

uspfn_SpPartSalesFilter '4W'

