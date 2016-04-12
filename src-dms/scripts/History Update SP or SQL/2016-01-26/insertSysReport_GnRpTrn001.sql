insert into SysReport
select 'GnRpTrn001','Isi.Dms.Report.GeneralModule,ReportSource.Transaction.GnRpTrn001Rpt',	'SP',	'uspfn_GnRpTrn001',	'SVLTR',	'NOTA RETUR', '',	''
where not exists (select * from SysReport where ReportID = 'GnRpTrn001')
