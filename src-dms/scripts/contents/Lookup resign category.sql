declare @CompanyCode varchar(25);
set @CompanyCode = (
	select top 1
		a.CompanyCode
	from
		gnMstOrganizationHdr a
);


delete gnMstLookUpHdr where CodeID = 'RESIGNCTG';

insert into gnMstLookUpHdr
values
	(@CompanyCode, 'RESIGNCTG', 'Resign Category', '50', 0, 'System', getdate(), 'System', getDate(), 0, '', null)





delete gnMstLookUpDtl where CodeID = 'RESIGNCTG';

insert into 
	gnMstLookUpDtl 
values
	(@CompanyCode, 'RESIGNCTG', 'TDKPRFM', 1, 'TDKPRFM', 'TIDAK PERFORM', 'system', getdate(), null, null)

insert into 
	gnMstLookUpDtl 
values
	(@CompanyCode, 'RESIGNCTG', 'PRBLMADM', 2, 'PRBLMADM', 'PROBLEM ADMINISTRASI', 'system', getdate(), null, null)

insert into 
	gnMstLookUpDtl 
values
	(@CompanyCode, 'RESIGNCTG', 'TDKDSPLN', 3, 'TDKDSPLN', 'TIDAK DISIPLIN', 'system', getdate(), null, null)

insert into 
	gnMstLookUpDtl 
values
	(@CompanyCode, 'RESIGNCTG', 'PNDHKRJ', 4, 'PNDHKRJ', 'PINDAH KERJA', 'system', getdate(), null, null)

insert into 
	gnMstLookUpDtl 
values
	(@CompanyCode, 'RESIGNCTG', 'LN2', 99, 'LN2', 'LAIN-LAIN', 'system', getdate(), null, null)



select * from gnMstLookupDtl where CodeID='RESIGNCTG'
