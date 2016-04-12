if object_id('SvCustomerView') is not null
	drop procedure SvCustomerView
GO
CREATE view [dbo].[SvCustomerView]
as 
select a.CompanyCode
     , b.CompanyName
     , a.CustomerCode
     , a.CustomerName
     , a.Address1 as Address1, a.Address1 as Address2, a.Address3 as Address3, a.Address4 as Address4
     , a.FaxNo, a.PhoneNo, a.HPNo, a.NPWPNo, isnull(a.NPWPDate,'19000101') NPWPDate, a.SKPNo, isnull(a.SKPDate,'19000101')SKPDate
     , a.BirthDate
  from gnMstCustomer a
  left join gnMstOrganizationHdr b
    on b.CompanyCode = a.CompanyCode



GO


