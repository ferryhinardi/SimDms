/*
	uspfn_CsDlvryOutstanding '6006400001', '6006400106'
*/
CREATE PROCEDURE uspfn_CsDlvryOutstanding
	@CompanyCode nvarchar(20),
	@BranchCode varchar(20)
AS
BEGIN
	SELECT DISTINCT a.CustomerCode, a.CustomerName, a.Chassis, PoliceRegNo
	FROM CsLkuTDaysCallView a
	RIGHT JOIN omTrSalesBPK b ON a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.CustomerCode = b.CustomerCode
	WHERE a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND b.LockingDate <  (SELECT SettingParam1 FROM CsSettings WHERE SettingCode = 'REMDELIVERY')
	AND (b.LockingDate < b.BPKDate OR b.LockingDate IS NULL OR b.LockingDate = '1900-01-01 00:00:00.000')
END