
go
if object_id('DealerListView') is not null
	drop view DealerListView

go
create view DealerListView
as
select *
  from gnMstOrganizationHdr a




go
select * from DealerListView