IF object_id('uspfn_GetAOSByPeriode') IS NOT NULL DROP PROCEDURE uspfn_GetAOSByPeriode
Go
create procedure [dbo].[uspfn_GetAOSByPeriode]
	@Branch varchar(15),
	@Month varchar(15),
	@Year varchar(15)
as

SELECT  d.DealerAbbreviation DealerAbb, e.OutletAbbreviation OutletAbb, CONVERT(varchar, a.POSDate, 105) POSDate, a.POSNO, b.PartNo, c.PartName, b.SuggorQty,
CASE a.isGenPORDD WHEN 1 THEN 'SEND' ELSE 'NOT SEND' END SStatus FROM spTrnPPOSHdr a 
INNER JOIN spTrnPPOSDtl b ON b.POSNo = a.POSNo AND b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode
INNER JOIN spMstItemInfo c ON c.PartNo = b.PartNo
INNER JOIN gnMstDealerMapping d ON d.DealerCode = a.CompanyCode
INNER JOIN gnMstDealerOutletMapping e ON e.OutletCode = a.BranchCode
WHERE a.OrderType='8' AND a.isDeleted=0 AND a.BranchCode=@Branch AND MONTH(a.POSDate) = @Month