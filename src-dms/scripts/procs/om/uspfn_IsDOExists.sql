
go
if object_id('uspfn_IsDOExists') is not null
	drop procedure uspfn_IsDOExists

go
create procedure uspfn_IsDOExists
	@CompanyCode varchar(25),
	@BranchCode varchar(25),
	@SONumber varchar(50)
as

begin
	  SELECT count(*) as Jumlah
        FROM omTrSalesDO 
       WHERE CompanyCode = @CompanyCode
         AND BranchCode = @BranchCode
         AND SONo = @SONumber 
         AND Status NOT IN (3, 4);
end