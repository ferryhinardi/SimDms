
go
delete gnMstLookUpDtl where CodeID = 'ITSG'




declare @CompanyCode varchar(25);
set @CompanyCode = (select top 1 CompanyCode from gnMstOrganizationHdr)

insert into 
	gnMstLookupDtl 
values 
	(@CompanyCode, 'ITSG', '1', 1, '1', 'Trainee', 'system', getdate(), 'system', getdate())

insert into 
	gnMstLookupDtl 
values 
	(@CompanyCode, 'ITSG', '2', 2, '2', 'Silver', 'system', getdate(), 'system', getdate())

insert into 
	gnMstLookupDtl 
values 
	(@CompanyCode, 'ITSG', '3', 3, '3', 'Gold', 'system', getdate(), 'system', getdate())

insert into 
	gnMstLookupDtl 
values 
	(@CompanyCode, 'ITSG', '4', 4, '4', 'Platinum', 'system', getdate(), 'system', getdate())




select * from gnMstLookupDtl where CodeID = 'ITSG'
