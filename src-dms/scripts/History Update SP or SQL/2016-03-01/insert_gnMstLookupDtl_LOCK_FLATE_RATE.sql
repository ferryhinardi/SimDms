if not exists (select * from gnMstLookUpDtl where CodeID = 'SPK_FLAG' and LookUpValue = 'LOCK_FLATE_RATE')
insert into gnMstLookUpDtl
select (select top 1 CompanyCode from gnMstCoProfile), 'SPK_FLAG', 'LOCK_FLATE_RATE', 
(select MAX(SeqNo)+1 from gnMstLookUpDtl where CodeID = 'SPK_FLAG'), '1', 
'0 / 1 (0 : ENABLED FLATERATETIME FOR JOB TYPE FS; 1 : DISABLED FLATERATETIME FOR JOB TYPE FS)',
'ga', GETDATE(), 'ga', GETDATE()


