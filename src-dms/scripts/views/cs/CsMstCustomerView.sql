if object_id('CsMstCustomerView') is not null
	drop view CsMstCustomerView

go
create view CsMstCustomerView
as
select distinct
       a.CompanyCode
	 , b.BranchCode
     , a.CustomerCode
	 , a.CustomerName
	 , a.BirthDate
	 , Address = (a.Address1 + ltrim(a.Address2) + ltrim(a.Address3) + ltrim(a.Address4))
	 , Outstanding = (
			case  
				when isnull(d.CustomerCode, '') != '' and d.CustomerCode = '' then '1'
				else '0'
			end
	   )
  from GnMstCustomer a
 inner join omTrSalesInvoice b
    on b.CompanyCode = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
 --inner join gnMstOrganizationDtl c
 --   on c.CompanyCode = a.CompanyCode
 --  and c.BranchCode = b.BranchCode 
  left join CsCustBirthday d
    on d.CompanyCode = a.CompanyCode
   and d.CustomerCode = a.CustomerCode
 where exists (
			select x.CustomerCode
			  from svMstCustomerVehicle x
			 where x.CompanyCode = a.CompanyCode
			   and x.CustomerCode = a.CustomerCode
	   )

go

select * from CsMstCustomerView where Outstanding='1'
--select * from CsCustBirthDay where CustomerCode in (
--	select CustomerCode from CsMstCustomerView
--)