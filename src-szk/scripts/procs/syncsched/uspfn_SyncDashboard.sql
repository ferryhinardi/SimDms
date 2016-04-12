alter procedure uspfn_SyncDashboard
as

begin try
	--begin transaction
	exec uspfn_SyncPmExecSummary
	exec uspfn_SyncPmExecSummary2
	exec uspfn_SyncPmExecSummary3
	exec uspfn_SyncPmMonitoring
	exec uspfn_SyncPmFakturPolisi
	exec uspfn_SyncPmMovingAvgChart
	exec uspfn_SyncPmItsByLeadTime
	--commit transaction
end try
begin catch
	select 'error' as status
end catch

go