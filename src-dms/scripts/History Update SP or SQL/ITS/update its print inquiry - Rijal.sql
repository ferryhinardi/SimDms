if exists (select * from SysReport where ReportID='PmRpInqFollowUpDet2015')
begin
	delete SysReport
	where ReportID='PmRpInqFollowUpDet2015'

	INSERT INTO SysReport
	VALUES ('PmRpInqFollowUpDet2015','Isi.Dms.Report.PreSales,ReportSource.Inquiry.PmRpInqFollowUpDet2015Rpt','SP','usprpt_PmRpInqFollowUpDtlNew','A4C','PROSPECT FOLLOW UP LIST DETAIL',NULL,	NULL)
end
else
begin
	INSERT INTO SysReport
	VALUES ('PmRpInqFollowUpDet2015','Isi.Dms.Report.PreSales,ReportSource.Inquiry.PmRpInqFollowUpDet2015Rpt','SP','usprpt_PmRpInqFollowUpDtlNew','A4C','PROSPECT FOLLOW UP LIST DETAIL',NULL,	NULL)
end

if exists (select * from SysReport where ReportID='PmRpInqFollowUpDet2015B')
begin
	delete SysReport
	where ReportID='PmRpInqFollowUpDet2015B'

	INSERT INTO SysReport
	VALUES ('PmRpInqFollowUpDet2015B','Isi.Dms.Report.PreSales,ReportSource.Inquiry.PmRpInqFollowUpDet2015BRpt','SP','usprpt_PmRpInqFollowUpDtlNew','A4C','PROSPECT FOLLOW UP LIST DETAIL',NULL,	NULL)
end
else
begin
	INSERT INTO SysReport
	VALUES ('PmRpInqFollowUpDet2015B','Isi.Dms.Report.PreSales,ReportSource.Inquiry.PmRpInqFollowUpDet2015BRpt','SP','usprpt_PmRpInqFollowUpDtlNew','A4C','PROSPECT FOLLOW UP LIST DETAIL',NULL,	NULL)
end

if exists (select * from SysReport where ReportID='PmRpInqLostCaseWeb')
begin
	delete SysReport
	where ReportID='PmRpInqLostCaseWeb'
	
	INSERT INTO SysReport
	VALUES ('PmRpInqLostCaseWeb','Isi.Dms.Report.PreSales,ReportSource.Inquiry.PmRpInqLostCaseWebRpt','SP','usprpt_PmRpInqLostCaseWeb','PMLetter1','LOST CASE','',	NULL)
end
else
begin
	INSERT INTO SysReport
	VALUES ('PmRpInqLostCaseWeb','Isi.Dms.Report.PreSales,ReportSource.Inquiry.PmRpInqLostCaseWebRpt','SP','usprpt_PmRpInqLostCaseWeb','PMLetter1','LOST CASE','',	NULL)
end

if exists (select * from SysReport where ReportID='PmRpInqOutStandingNew')
begin
	delete SysReport
	where ReportID='PmRpInqOutStandingNew'
	
	INSERT INTO SysReport
	VALUES ('PmRpInqOutStandingNew','Isi.Dms.Report.PreSales,ReportSource.Inquiry.PmRpInqOutStandingWebRpt','SP','usprpt_PmRpInqOutStanding_NewPrint','SPLTP','INQUIRY OUTSTANDING PROSPECT','',	NULL)
end
else
begin
	INSERT INTO SysReport
	VALUES ('PmRpInqOutStandingNew','Isi.Dms.Report.PreSales,ReportSource.Inquiry.PmRpInqOutStandingWebRpt','SP','usprpt_PmRpInqOutStanding_NewPrint','SPLTP','INQUIRY OUTSTANDING PROSPECT','',	NULL)
end

if exists (select * from SysReport where ReportID='PmRpInqSalesAchievementWeb')
begin
	delete SysReport
	where ReportID='PmRpInqSalesAchievementWeb'
	
	INSERT INTO SysReport
	VALUES ('PmRpInqSalesAchievementWeb','Isi.Dms.Report.PreSales,ReportSource.Inquiry.PmRpInqSalesAchievementWebRpt','SP','usprpt_PmRpSalesAchievementWeb','PMLetter1','SALES ACHIEVEMENT','',	NULL)
end
else
begin
	INSERT INTO SysReport
	VALUES ('PmRpInqSalesAchievementWeb','Isi.Dms.Report.PreSales,ReportSource.Inquiry.PmRpInqSalesAchievementWebRpt','SP','usprpt_PmRpSalesAchievementWeb','PMLetter1','SALES ACHIEVEMENT','',	NULL)
end