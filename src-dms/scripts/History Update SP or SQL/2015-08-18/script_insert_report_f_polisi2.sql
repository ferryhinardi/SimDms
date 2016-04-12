if (SELECT count(*) from sysReport WHERE reportID='OmRpSalRgs013Web') = 0
	begin
	INSERT INTO SysReport (ReportID, ReportPath, ReportSource, ReportProc, ReportDeviceID, ReportInfo)
	Values('OmRpSalRgs013Web','Isi.Dms.Report.Sales,ReportSource.Sales.OmRpSalRgs013Rpt','SP','usprpt_OmRpSalRgs013Web','C04','FAKTUR-FAKTUR REQUEST YANG SUDAH DIGENERATE')
	end
if (SELECT count(*) from sysReport WHERE reportID='OmRpSalRgs017Web') = 0
	begin
	INSERT INTO SysReport (ReportID, ReportPath, ReportSource, ReportProc, ReportDeviceID, ReportInfo)
	Values('OmRpSalRgs017Web','Isi.Dms.Report.Sales,ReportSource.Sales.OmRpSalRgs017Rpt','SP','usprpt_OmRpSalRgs017Web','C03','LAPORAN HARIAN PENERBITAN FAKTUR POLISI ')
	end