
go
delete gnMstLookUpDtl where CodeID = 'SIZEALT'

declare @CompanyCode varchar(25);
set @CompanyCode = (select top 1 CompanyCode from gnMstOrganizationHdr)

insert into 
	gnMstLookupDtl 
values 
	(@CompanyCode, 'SIZEALT', 'L', 1, 'L', 'Large', 'system', getdate(), 'system', getdate())

insert into 
	gnMstLookupDtl 
values 
	(@CompanyCode, 'SIZEALT', 'M', 2, 'M', 'Medium', 'system', getdate(), 'system', getdate())

insert into 
	gnMstLookupDtl 
values 
	(@CompanyCode, 'SIZEALT', 'S', 3, 'S', 'Small', 'system', getdate(), 'system', getdate())

insert into 
	gnMstLookupDtl 
values 
	(@CompanyCode, 'SIZEALT', 'XL', 4, 'XL', 'Extra Large', 'system', getdate(), 'system', getdate())

insert into 
	gnMstLookupDtl 
values 
	(@CompanyCode, 'SIZEALT', 'XXL', 4, 'XL', 'Double Extra Large', 'system', getdate(), 'system', getdate())

insert into 
	gnMstLookupDtl 
values 
	(@CompanyCode, 'SIZEALT', 'XXXL', 4, 'XL', 'Triple Extra Large', 'system', getdate(), 'system', getdate())

insert into 
	gnMstLookupDtl 
values 
	(@CompanyCode, 'SIZEALT', 'XXXXL', 4, 'XL', 'XXXXL (4XL)', 'system', getdate(), 'system', getdate())

select * from gnMstLookupDtl where CodeID = 'SIZEALT'
