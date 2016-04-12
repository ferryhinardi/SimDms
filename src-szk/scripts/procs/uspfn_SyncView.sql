alter procedure uspfn_SyncView

as

set nocount on

if exists (select * from sys.tables where name = 'CsLkuTDayCallView') drop table CsLkuTDayCallView

select * into CsLkuTDayCallView from (select * from CsLkuTDayCallViewSource)x

select 'Finish Job'
