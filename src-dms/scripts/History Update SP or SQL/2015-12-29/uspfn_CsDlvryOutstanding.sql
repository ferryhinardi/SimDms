/*
	uspfn_CsDlvryOutstanding '6006400001', '6006400106'
*/
ALTER PROCEDURE uspfn_CsDlvryOutstanding
	@CompanyCode nvarchar(20),
	@BranchCode varchar(20)
AS
BEGIN
	SELECT a.CustomerCode, a.CustomerName, b.Chassis, b.PoliceRegNo, b.BPKDate
	FROM CsCustomerView a
	  LEFT JOIN CsCustomerVehicleView b
		on b.CompanyCode = a.CompanyCode
	   and b.BranchCode = a.BranchCode
	   and b.CustomerCode = a.CustomerCode
	WHERE a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND b.BPKDate >=  (SELECT SettingParam1 FROM CsSettings WHERE SettingCode = 'REMDELIVERY')
	AND (b.DeliveryDate < b.BPKDate OR b.DeliveryDate IS NULL OR b.DeliveryDate = '1900-01-01 00:00:00.000')
	ORDER BY b.BPKDate DESC
END