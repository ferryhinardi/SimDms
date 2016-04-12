
if (SELECT count(*) from sysReport WHERE reportID='OmRpSalesTrn007BWeb') = 0
	begin
	INSERT INTO SysReport (ReportID, ReportPath, ReportSource, ReportProc, ReportDeviceID, ReportInfo)
	Values('OmRpSalesTrn007BWeb','Isi.Dms.Report.Sales,ReportSource.Sales.OmRpSalesTrn007BRpt','SP','usprpt_OmRpSalesTrn007Web','TEST_LETTER','PERMOHONAN FAKTUR POLISI')
	end
if (SELECT count(*) from sysReport WHERE reportID='OmRpSalesTrn007CWeb') = 0
	begin
	INSERT INTO SysReport (ReportID, ReportPath, ReportSource, ReportProc, ReportDeviceID, ReportInfo)
	Values('OmRpSalesTrn007CWeb','Isi.Dms.Report.Sales,ReportSource.Sales.OmRpSalesTrn007CRpt','SP','usprpt_OmRpSalesTrn007BWeb','TEST_LETTER','PERMOHONAN FAKTUR POLISI') 
	end
if (SELECT count(*) from sysReport WHERE reportID='OmRpSalesTrn007DNewWeb') = 0
	begin
	INSERT INTO SysReport (ReportID, ReportPath, ReportSource, ReportProc, ReportDeviceID, ReportInfo)
	Values('OmRpSalesTrn007DNewWeb','Isi.Dms.Report.Sales,ReportSource.Sales.OmRpSalesTrn007DNewRpt','SP','usprpt_OmRpSalesTrn007Web','LetterP2','PERMOHONAN FAKTUR POLISI') 
	end