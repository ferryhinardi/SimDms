
go
if object_id('uspfn_UpdateValidityCsBpkbRetrievalInformation') is not null
	drop procedure uspfn_UpdateValidityCsBpkbRetrievalInformation

go
create procedure uspfn_UpdateValidityCsBpkbRetrievalInformation
	@BpkbReadyDate datetime,
	@CompanyCode varchar(13),
	@CustomerCode varchar(13)
as
begin
	update CsBpkbRetrievalInformation
	   set IsDeleted = 1
	 where CompanyCode=@CompanyCode
	   and CustomerCode=@CustomerCode
	   and RetrievalEstimationDate < @BpkbReadyDate
end