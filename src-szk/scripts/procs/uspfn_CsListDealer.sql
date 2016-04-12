alter procedure uspfn_CsListDealer
as

select a.CompanyCode as value, b.CompanyName as text
  from CsDealer a
  left join gnMstOrganizationHdr b
    on b.CompanyCode = a.CompanyCode
 where isnull(a.IsActive, 1) = 1

