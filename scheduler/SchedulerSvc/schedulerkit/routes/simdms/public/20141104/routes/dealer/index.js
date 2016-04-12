exports.start = function () 
{
	require("../../lib/JobSvc").serviceio();
	require("../../lib/sqliteBackup").service();
	require("../../lib/schedulerTasks").service();
}
