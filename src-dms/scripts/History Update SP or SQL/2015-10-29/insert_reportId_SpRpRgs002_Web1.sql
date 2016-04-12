INSERT INTO [dbo].[sysReport]
           ([ReportID]
           ,[ReportPath]
           ,[ReportSource]
           ,[ReportProc]
           ,[ReportDeviceID]
           ,[ReportInfo]
           ,[DefaultParam])
     VALUES
           ('SpRpRgs002_Web1'
           ,'Isi.Dms.Report.Sparepart,ReportSource.Register.SpRpRgs002Rpt_Web1'
           ,'SP'
           ,'usprpt_SpRpRgs002_web1'
           ,'C03'
           ,'REGISTER SUPPLY SLIP OUTSTANDING'
           ,'''6006400001'',''6006400101'',''6006400101'',''20150101 00:00:00'',''20151027 23:59:59'',''ALL'','''',''false''')
GO