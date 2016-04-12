if not exists (select * from gnMstLookUpDtl where CodeID = 'RFTP' and LookUpValue = 'LOCK_BASMODEL')
insert into gnMstLookUpDtl
select (select distinct CompanyCode from gnMstCoProfile),'RFTP','LOCK_BASMODEL',(select MAX(seqno) from gnMstLookUpDtl where CodeID = 'RFTP'),'LOCK_BASMODEL',
'LOCK BASIC MODEL',	'SIM',	GETDATE(), 'SIM',	GETDATE()

if not exists (select * from svMstRefferenceService where RefferenceType = 'LOCK_BASMODEL')
begin
insert into svMstRefferenceService
select (select distinct CompanyCode from gnMstCoProfile), (select distinct ProductType from gnMstCoProfile), 'LOCK_BASMODEL','A1J310','99000B00W20N001,99000B99010N010',
'WAGON R','1','SIM',	GETDATE(), 'SIM',	GETDATE(),0,'',''
insert into svMstRefferenceService
select (select distinct CompanyCode from gnMstCoProfile), (select distinct ProductType from gnMstCoProfile), 'LOCK_BASMODEL','AVB414','99000B00W20N001,99000B99010N010',
'CIAZ 1.4','1','SIM',	GETDATE(), 'SIM',	GETDATE(),0,'',''
insert into svMstRefferenceService
select (select distinct CompanyCode from gnMstCoProfile), (select distinct ProductType from gnMstCoProfile), 'LOCK_BASMODEL','AVB414AT','99000B00W20N001,99000B99010N010',
'CIAZ 1.4 MATIC','1','SIM',	GETDATE(), 'SIM',	GETDATE(),0,'',''
insert into svMstRefferenceService
select (select distinct CompanyCode from gnMstCoProfile), (select distinct ProductType from gnMstCoProfile), 'LOCK_BASMODEL','GC415','99000B10W40N010,99000B99010N010',
'APV','1','SIM',	GETDATE(), 'SIM',	GETDATE(),0,'',''
end