update sysreport 
set ReportPath = 'Isi.Dms.Report.PreSales,ReportSource.Inquiry.PmRpInqFollowUpWebRpt'
where ReportID='PmRpInqPeriodeWeb'

update sysreport 
set ReportPath = 'Isi.Dms.Report.PreSales,ReportSource.Inquiry.PmRpInqSummaryWebRpt'
where ReportID='PmRpInqSummaryWeb'