if not exists (select * from gnMstLookUpDtl where CodeID = 'FLAT_RATE')
begin
insert into gnMstLookUpHdr
select (select top 1 CompanyCode from gnMstCoProfile), 'FLAT_RATE', 'FLAT RATE', '5', '1', 'ga', GETDATE(), 'ga', GETDATE(),'0','',''
insert into gnMstLookUpDtl
select (select top 1 CompanyCode from gnMstCoProfile), 'FLAT_RATE', 'FSC 1000', '1', '1.0', 'FLATE RATE FSC 1000 KM','ga', GETDATE(), 'ga', GETDATE()
insert into gnMstLookUpDtl
select (select top 1 CompanyCode from gnMstCoProfile), 'FLAT_RATE', 'FSC 5000', '2', '0.7', 'FLATE RATE FSC 5000 KM','ga', GETDATE(), 'ga', GETDATE()
insert into gnMstLookUpDtl
select (select top 1 CompanyCode from gnMstCoProfile), 'FLAT_RATE', 'FSC 10000', '3', '1.7', 'FLATE RATE FSC 10000 KM','ga', GETDATE(), 'ga', GETDATE()
insert into gnMstLookUpDtl
select (select top 1 CompanyCode from gnMstCoProfile), 'FLAT_RATE', 'FSC 20000', '4', '2.1', 'FLATE RATE FSC 20000 KM','ga', GETDATE(), 'ga', GETDATE()
insert into gnMstLookUpDtl
select (select top 1 CompanyCode from gnMstCoProfile), 'FLAT_RATE', 'FSC 30000', '5', '1.7', 'FLATE RATE FSC 30000 KM','ga', GETDATE(), 'ga', GETDATE()
insert into gnMstLookUpDtl
select (select top 1 CompanyCode from gnMstCoProfile), 'FLAT_RATE', 'FSC 40000', '6', '2.7', 'FLATE RATE FSC 40000 KM','ga', GETDATE(), 'ga', GETDATE()
insert into gnMstLookUpDtl
select (select top 1 CompanyCode from gnMstCoProfile), 'FLAT_RATE', 'FSC 50000', '7', '1.7', 'FLATE RATE FSC 50000 KM','ga', GETDATE(), 'ga', GETDATE()
end
