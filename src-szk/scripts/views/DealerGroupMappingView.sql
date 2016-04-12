if object_id('DealerGroupMappingView') is not null
	drop view DealerGroupMappingView

go
create view DealerGroupMappingView
as
select b.GroupNo
     , a.CompanyCode
     , a.BranchCode
     , a.CompanyGovName as CompanyName
	 , a.CompanyName as BranchName
	 , c.AreaDealer as Area
  from gnMstCoProfile a
 inner join DealerGroupMapping b
    on b.DealerCode = a.CompanyCode
 inner join GroupArea c
    on c.GroupNo = b.GroupNo




go

select * from DealerGroupMappingView