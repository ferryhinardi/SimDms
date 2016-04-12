/*
INSERT INTO CsSettings (CompanyCode,SettingCode,SettingDesc,SettingParam1,SettingParam2,SettingParam3,SettingParam4,SettingParam5,
SettingLink1,SettingLink2,SettingLink3,IsDeleted,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate)
VALUES('6006400001', 'REMDELIVERY', 'REMINDER DELIVERY OUTSTANDING', '2016-01-01', 'DAY', 'CUTOFF', NULL, NULL, 'DeliveryOutstanding', 'Delivery Outstanding', 'Delivery Outstanding Monitoring', NULL, NULL, GETDATE(), NULL, NULL)
	
	select * from gnMstDealerOutletMapping
*/

UPDATE CsSettings
SET SettingParam1 = '2015-10-01' WHERE SettingCode = 'REMDELIVERY'


INSERT INTO CsSettings (CompanyCode,SettingCode,SettingDesc,SettingParam1,SettingParam2,SettingParam3,SettingParam4,SettingParam5,
SettingLink1,SettingLink2,SettingLink3,IsDeleted,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate)
VALUES((select top 1 companycode from gnMstOrganizationHdr), 'REMDELIVERY', 'REMINDER DELIVERY OUTSTANDING', '2015-10-01', 'DAY', 'CUTOFF', NULL, NULL, 'DeliveryOutstanding', 'Delivery Outstanding', 'Delivery Outstanding Monitoring', NULL, 'system', GETDATE(), 'system', getdate())
	
	