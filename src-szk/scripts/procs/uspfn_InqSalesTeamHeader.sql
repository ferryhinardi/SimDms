go
if object_id('uspfn_InqSalesTeamHeader') is not null
	drop procedure uspfn_InqSalesTeamHeader

go
create procedure uspfn_InqSalesTeamHeader
	@GroupArea varchar(10),
	@CompanyCode varchar(17),
	@BranchCode varchar(17)

as
begin
	--if @GroupArea is null or @GroupArea = ''
	--	set @GroupArea = '%';

	--if @CompanyCode is null or @CompanyCode = ''
	--	set @CompanyCode = '%';

	if @BranchCode is null or @BranchCode = ''
		set @BranchCode = '%';
			
	select c.AreaDealer as Area
	     , a.CompanyGovName as CompanyName
	     , a.CompanyName as BranchName
		 , b.GroupNo as GroupArea
		 , a.CompanyCode
		 , a.BranchCode
	  from gnMstCoProfile a
	 inner join DealerGroupMapping b
	    on b.DealerCode = a.CompanyCode
	 inner join GroupArea c
	    on c.GroupNo = b.GroupNo
	 where c.GroupNo like @GroupArea
	   and a.CompanyCode like @CompanyCode 
	   and a.BranchCode like @BranchCode
	 order by c.AreaDealer asc 
	     , a.CompanyGovName asc
	     , a.CompanyName asc;
end





go
exec uspfn_InqSalesTeamHeader @GroupArea='100', @CompanyCode='', @BranchCode=''