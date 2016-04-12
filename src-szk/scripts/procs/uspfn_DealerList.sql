
go
if object_id('uspfn_DealerList') is not null
	drop procedure uspfn_DealerList

go	
create procedure uspfn_DealerList
	@DealerType varchar(5),
	@LinkedModule varchar(5),
	@GroupArea varchar(10)
as
begin
	if @DealerType is null or @DealerType = ''
		set @DealerType = '%';
	else
		set @DealerType = '%' + @DealerType + '%'


	if @GroupArea is null or @GroupArea = ''
		set @GroupArea = '%';
	else
		set @GroupArea = '%' + @GroupArea + '%'

	if @LinkedModule = '' 
	begin
		select a.DealerCode as CompanyCode
		     , a.DealerName as CompanyName
			 , a.ShortName as CompanyShortName
			 , a.ProductType as ProductType
		  from DealerInfo a
		 inner join DealerGroupMapping b
		    on b.DealerCode = a.DealerCode
		 inner join GroupArea c
		    on c.GroupNo=b.GroupNo
		 where a.ProductType like @DealerType
		   and b.GroupNo like @GroupArea
		 order by a.DealerName asc
	end
	else
	begin
		select a.DealerCode as CompanyCode
		     , a.DealerName as CompanyName
			 , a.ShortName as CompanyShortName
			 , a.ProductType as ProductType
		  from DealerInfo a
		 inner join DealerGroupMapping b
		    on b.DealerCode = a.DealerCode
		 inner join GroupArea c
		    on c.GroupNo=b.GroupNo
		 inner join gnMstDealerInstalledModule d
		   on d.DealerCode = a.DealerCode
		where d.InstalledModule like @LinkedModule
		  and a.ProductType like @DealerType
		  and b.GroupNo like @GroupArea
	    order by a.DealerName asc
	end
end


--select * from gnMstDealerInstalledModule
--where InstalledModule = 'MP'




go
exec uspfn_DealerList '4W', 'MP', '100'


--select * from DealerGroupMapping