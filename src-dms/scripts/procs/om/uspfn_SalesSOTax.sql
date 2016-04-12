
go
if object_id('uspfn_SalesSOTax') is not null
	drop procedure uspfn_SalesSOTax

go
create procedure uspfn_SalesSOTax
	@CompanyCode varchar(13),
	@CustomerCode varchar(17)
as
begin
	SELECT a.TaxPct
      FROM gnMstTax a
     INNER JOIN gnMstCustomerProfitCenter b
        ON a.CompanyCode = b.CompanyCode
       AND a.TaxCode = b.TaxCode
       AND a.CompanyCode = @CompanyCode
       AND b.CustomerCode = @CustomerCode
       AND b.ProfitCenterCode = '100'
end



--go
--exec uspfn_SalesSOTax '6115204', '0000001'

