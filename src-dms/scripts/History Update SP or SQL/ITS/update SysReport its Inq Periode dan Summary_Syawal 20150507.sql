if exists (select * from SysReport where ReportID='PmRpInqPeriodeWeb')
begin
	update sysreport 
	set ReportPath = 'Isi.Dms.Report.PreSales,ReportSource.Inquiry.PmRpInqFollowUpWebRpt'
	where ReportID='PmRpInqPeriodeWeb'
end
else
begin
	INSERT INTO SysReport
	VALUES ('PmRpInqPeriodeWeb','Isi.Dms.Report.PreSales,ReportSource.Inquiry.PmRpInqFollowUpWebRpt','SP','usprpt_PmRpInqPeriodeWeb','PMLetter1','PROSPECT INQUIRY LIST',NULL,	NULL)
end

if exists (select * from SysReport where ReportID='PmRpInqPeriodeWeb')
begin
	update sysreport 
set ReportPath = 'Isi.Dms.Report.PreSales,ReportSource.Inquiry.PmRpInqSummaryWebRpt'
where ReportID='PmRpInqSummaryWeb'
end
else
begin
	INSERT INTO SysReport
	VALUES ('PmRpInqSummaryWeb','Isi.Dms.Report.PreSales,ReportSource.Inquiry.PmRpInqSummaryWebRpt','SP','usprpt_PmRpInqSummaryWeb','SPLTP','INQUIRY SUMMARY','',NULL)
end



