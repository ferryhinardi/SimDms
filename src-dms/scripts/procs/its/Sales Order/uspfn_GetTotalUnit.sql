
go
if object_id('uspfn_GetTotalUnit') is not null
	drop procedure uspfn_GetTotalUnit

go
create procedure uspfn_GetTotalUnit
	@CompanyCode varchar(13),
	@BranchCode varchar(13),
	@SONumber varchar(25)	
as

begin
	select isnull(sum(quantity), 0) as TotalUnit
	  from omtrsalessomodelcolour 
	 where companycode=@CompanyCode and branchcode=@BranchCode and sono=@SONumber;
end