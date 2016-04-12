

declare @CompanyCode varchar(25);
set @CompanyCode = (
	select top 1 CompanyCode from gnMstOrganizationHdr
)


delete gnMstLookUpHdr where CodeID = 'TRNPREALT';
delete gnMstLookUpHdr where CodeID = 'TRNPOSTALT';

insert into gnMstLookUpHdr
values
	(@CompanyCode, 'TRNPREALT', 'PRE ALT', '50', 0, 'System', getdate(), 'System', getDate(), 0, '', null)

insert into gnMstLookUpHdr
values
	(@CompanyCode, 'TRNPOSTALT', 'POST ALT', '50', 0, 'System', getdate(), 'System', getDate(), 0, '', null)





delete gnMstLookUpDtl where CodeID = 'TRNPREALT';
delete gnMstLookUpDtl where CodeID = 'TRNPOSTALT';

insert into gnMstLookUpDtl 
values
	((
		@CompanyCode
	), 'TRNPREALT', '1', 1, '1', 'GOOD', 'System', GETDATE(), 'System', GETDATE())

insert into gnMstLookUpDtl 
values
	((
		@CompanyCode
	), 'TRNPREALT', '2', 2, '2', 'BAD', 'System', GETDATE(), 'System', GETDATE())

insert into gnMstLookUpDtl 
values
	((
		@CompanyCode
	), 'TRNPOSTALT', '1', 1, '1', 'GOOD', 'System', GETDATE(), 'System', GETDATE())

insert into gnMstLookUpDtl 
values
	((
		@CompanyCode
	), 'TRNPOSTALT', '2', 2, '2', 'BAD', 'System', GETDATE(), 'System', GETDATE())
	
