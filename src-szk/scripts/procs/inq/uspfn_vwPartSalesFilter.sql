alter procedure uspfn_vwPartSalesFilter 
as 

select distinct Area as value, Area as text from vwPartSales
select distinct DealerCode as value, DealerName as text, Area as [group] from vwPartSales
select distinct a.TypeOfGoods as value, b.LookUpValueName as text
  from vwPartSales a
  left join GnMstLookupDtl b
    on b.CompanyCode = '6006406'
   and b.CodeId = 'TPGO'
   and b.LookUpValue = a.TypeOfGoods

go

uspfn_vwPartSalesFilter 

