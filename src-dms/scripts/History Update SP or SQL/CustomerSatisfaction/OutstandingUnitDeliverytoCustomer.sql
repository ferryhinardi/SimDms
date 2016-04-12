/*
	uspfn_CsOutstandingDlvryReport '6006400106', '2015-12-01', '2015-12-14', 1
*/
ALTER PROCEDURE uspfn_CsOutstandingDlvryReport
	@BranchCode VARCHAR(15),
	@DateFrom DATETIME,
	@DateTo DATETIME,
	@Status INT -- NULL, 0, 1
AS
BEGIN
IF @BranchCode = '' SET @BranchCode = NULL

SELECT ROW_NUMBER() OVER(ORDER BY h.BranchCode, h.BPKNo) AS No,
       h.BranchCode, o.OutletAbbreviation, h.BPKNo, h.BPKDate,-- h.SONo, 
	   DeliveryDate = (CASE WHEN YEAR(h.LockingDate)<2000
	                             THEN '' ELSE CAST(CAST(h.LockingDate AS DATE) AS VARCHAR) END),
		h.LockingDate,
	   h.CustomerCode, c.CustomerName, d.SalesModelCode, d.SalesModelYear, 
	   d.ChassisCode, d.ChassisNo, d.EngineCode, d.EngineNo
  FROM omTrSalesBPK h, omTrSalesBPKDetail d, gnMstCustomer c, gnMstDealerOutletMapping o
 WHERE (@BranchCode IS NULL OR h.BranchCode = @BranchCode)                     -- per Branch
   AND CAST(h.BPKDate AS DATE) BETWEEN @DateFrom AND @DateTo  -- per periode
   AND h.CompanyCode=d.CompanyCode
   AND h.BranchCode=d.BranchCode
   AND h.BPKNo=d.BPKNo
   AND h.CustomerCode=c.CustomerCode
   AND d.BranchCode=o.OutletCode
   AND (@Status IS NULL OR (@Status = 1 OR h.LockingDate = '1900-01-01 00:00:00.000') AND (@Status = 0 OR h.LockingDate <> '1900-01-01 00:00:00.000'))
END