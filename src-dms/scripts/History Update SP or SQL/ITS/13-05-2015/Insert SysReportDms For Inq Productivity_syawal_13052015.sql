if not exists(select * from SysReportDms where ReportID='ItsSumInqProdBySales') begin
	INSERT INTO [dbo].[SysReportDms]
           ([ReportID]
           ,[ReportPath]
           ,[ReportProc])
	values ('ItsSumInqProdBySales',
			'its/ItsSumInqProdBySales.rdlc',
			'usprpt_InquiryITSProd')
end
else print 'Sudah Ada'

if not exists(select * from SysReportDms where ReportID='ItsSumInqProdBySource') begin
	INSERT INTO [dbo].[SysReportDms]
           ([ReportID]
           ,[ReportPath]
           ,[ReportProc])
	VALUES
           ('ItsSumInqProdBySource'
           ,'its/ItsSumInqProdBySource.rdlc'
           ,'usprpt_InquiryITSProd')
end
else print 'Sudah Ada'

if not exists(select * from SysReportDms where ReportID='ItsSumInqProdByVeh') begin
	INSERT INTO [dbo].[SysReportDms]
           ([ReportID]
           ,[ReportPath]
           ,[ReportProc])
	VALUES
           ('ItsSumInqProdByVeh'
           ,'its/ItsSumInqProdByVeh.rdlc'
           ,'usprpt_InquiryITSProd')
end
else print 'Sudah Ada'

if not exists(select * from SysReportDms where ReportID='ItsSaldoInqProdBySales') begin
	INSERT INTO [dbo].[SysReportDms]
           ([ReportID]
           ,[ReportPath]
           ,[ReportProc])
VALUES
           ('ItsSaldoInqProdBySales'
           ,'its/ItsSaldoInqProdBySales.rdlc'
           ,'usprpt_InquiryITSProd')
end
else print 'Sudah Ada'

if not exists(select * from SysReportDms where ReportID='ItsSaldoInqProdBySource') begin
	INSERT INTO [dbo].[SysReportDms]
           ([ReportID]
           ,[ReportPath]
           ,[ReportProc])
VALUES
           ('ItsSaldoInqProdBySource'
           ,'its/ItsSaldoInqProdBySource.rdlc'
           ,'usprpt_InquiryITSProd')
end
else print 'Sudah Ada'

if not exists(select * from SysReportDms where ReportID='ItsSaldoInqProdByVeh') begin
	INSERT INTO [dbo].[SysReportDms]
           ([ReportID]
           ,[ReportPath]
           ,[ReportProc])
VALUES
           ('ItsSaldoInqProdByVeh'
           ,'its/ItsSaldoInqProdByVeh.rdlc'
           ,'usprpt_InquiryITSProd')
end
else print 'Sudah Ada'