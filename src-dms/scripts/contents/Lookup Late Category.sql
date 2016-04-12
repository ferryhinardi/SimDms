




declare @CompanyCode varchar(25);
set @CompanyCode = (
	select top 1
		a.CompanyCode
	from
		gnMstOrganizationHdr a
);





delete gnMstLookUpHdr where CodeID = 'LATECTG';

insert into gnMstLookUpHdr
values
	(@CompanyCode, 'LATECTG', 'Late Category', '50', 0, 'System', getdate(), 'System', getDate(), 0, '', null)






delete gnMstLookUpDtl where CodeID = 'LATECTG';

insert into 
	gnMstLookUpDtl 
values
	(@CompanyCode, 'LATECTG', 'SKT', 1, 'SKT', 'SAKIT', 'system', getdate(), null, null)

insert into 
	gnMstLookUpDtl 
values
	(@CompanyCode, 'LATECTG', 'TRLMBT', 2, 'TRLMBT', 'TERLAMBAT', 'system', getdate(), null, null)

insert into 
	gnMstLookUpDtl 
values
	(@CompanyCode, 'LATECTG', 'IJNTRLMBT', 3, 'IJNTRLMBT', 'IJIN DATANG TERLAMBAT', 'system', getdate(), null, null)

insert into 
	gnMstLookUpDtl 
values
	(@CompanyCode, 'LATECTG', 'IJNPLGAWL', 4, 'IJNPLGAWL', 'IJIN PULANG LEBIH AWAL', 'system', getdate(), null, null)

insert into 
	gnMstLookUpDtl 
values
	(@CompanyCode, 'LATECTG', 'CUTI', 5, 'CUTI', 'CUTI', 'system', getdate(), null, null)

insert into 
	gnMstLookUpDtl 
values
	(@CompanyCode, 'LATECTG', 'SKTINAP', 6, 'SKTINAP', 'SAKIT RAWAT INAP', 'system', getdate(), null, null)

insert into 
	gnMstLookUpDtl 
values
	(@CompanyCode, 'LATECTG', 'IJNKHS', 7, 'IJNKHS', 'IJIN KHUSUS', 'system', getdate(), null, null)

	



select * from gnMstLookupDtl where CodeID='LATECTG'

