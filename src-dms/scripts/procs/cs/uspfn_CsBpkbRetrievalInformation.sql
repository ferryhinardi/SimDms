
go
if object_id('uspfn_CsBpkbRetrievalInformation') is not null
	drop procedure uspfn_CsBpkbRetrievalInformation

go
create procedure uspfn_CsBpkbRetrievalInformation
	@CompanyCode varchar(17),
	@CustomerCode varchar(17)
as
begin
	select a.CompanyCode
		 , a.CustomerCode
		 , a.RetrievalEstimationDate
		 , a.Notes
	  from CsBpkbRetrievalInformation a
	 where a.CompanyCode = @CompanyCode
	   and a.CustomerCode = @CustomerCode
	   and ( a.IsDeleted = 0 or a.IsDeleted is null)
	 order by a.RetrievalEstimationDate desc
end




go
exec uspfn_CsBpkbRetrievalInformation @CompanyCode='6006406', @CustomerCode=''