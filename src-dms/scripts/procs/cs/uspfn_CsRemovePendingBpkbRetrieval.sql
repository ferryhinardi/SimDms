
go
if object_id('uspfn_CsRemovePendingBpkbRetrieval') is not null
	drop procedure uspfn_CsRemovePendingBpkbRetrieval

go
create procedure uspfn_CsRemovePendingBpkbRetrieval
	@CompanyCode varchar(13),
	@CustomerCode varchar(13)
as
	
begin
	update CsBpkbRetrievalInformation
	   set IsDeleted=1
	 where CompanyCode=@CompanyCode
	   and CustomerCode=@CustomerCode
end


go 
exec uspfn_CsRemovePendingBpkbRetrieval @CompanyCode='6+006406', @CustomerCode='2077297'

select * from CsBpkbRetrievalInformation

