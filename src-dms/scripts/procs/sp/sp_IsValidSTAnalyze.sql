

/****** Object:  StoredProcedure [dbo].[sp_IsValidSTAnalyze]    Script Date: 07/03/2014 14:21:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO






CREATE procedure [dbo].[sp_IsValidSTAnalyze] (  

@CompanyCode varchar(10) ,
@BranchCode varchar(10),
@STHdrNo varchar(20))


as

SELECT * INTO #a2 FROM(
SELECT
	a.PartNo, a.STNo, a.SEqNo,  CASE WHEN b.Partno is null THEN 'LOKASI UTAMA BELUM DIENTRY' ELSE '' END Status
FROM SpTrnStockTakingDtl a
LEFT JOIN 
(
	SELECT
		x.CompanyCode,
		x.BranchCode,
		x.StHdrNo,
		x.PartNo
	FROM SpTrnStockTakingDtl x
	WHERE x.CompanyCode = @CompanyCode
		AND x.BranchCode = @BranchCode
		AND x.StHdrNo = @StHdrNo
		AND x.IsMainLocation = 1

) b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.StHdrNo = b.StHdrNo AND a.PartNo = b.PartNo
WHERE a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.StHdrNo = @StHdrNo
GROUP BY a.PartNo, b.PartNo, a.STNo, a.SeqNo
) #a2

SELECT * FROM #a2 WHERE Status <> ''
UNION
SELECT
	a.PartNo, a.STNo, a.SEqNo,'BELUM TERDAFTAR PADA LIST TAG/FORM' Status
FROM SpTrnStockTakingTemp a
WHERE CompanyCode = @CompanyCode 
	AND BranchCode = @BranchCode
	AND StHdrNo = @StHdrNo
	AND PartNo NOT IN (
SELECT 
DISTINCT(a.PartNo)
FROM SpTrnStockTakingDtl a
WHERE CompanyCode = @CompanyCode
	  AND BranchCode = @BranchCode
	  AND StHdrNo = @StHdrNo)
	AND PartNo <> ''
GROUP BY a.PartNo, a.STNo, a.SeqNo
UNION
SELECT
	a.PartNo, a.STNo, a.SEqNo, 'BLANK TAG/FORM BELUM TERPAKAI/DIBATALKAN' Status
FROM SpTrnStockTakingTemp a
WHERE CompanyCode = @CompanyCode 
	AND BranchCode = @BranchCode
	AND StHdrNo = @StHdrNo
	AND STNo NOT IN (
SELECT 
DISTINCT(a.StNo)
FROM SpTrnStockTakingDtl a
WHERE CompanyCode = @CompanyCode
	  AND BranchCode = @BranchCode
	  AND StHdrNo = @StHdrNo)
	AND PartNo = '' AND a.Status IN ('0','1')
GROUP BY a.PartNo, a.STNo, a.SeqNo
UNION
SELECT PartNo, STNo, SeqNo, 'LOKASI UTAMA DI-ENTRY LEBIH DARI BATAS' Status
FROM SpTrnStocktakingDtl
WHERE CompanyCode = @CompanyCode 
	AND BranchCode = @BranchCode
	AND StHdrNo = @StHdrNo
	AND PartNo IN (
SELECT
	PartNo
FROM spTrnStockTakingDtl
WHERE CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND StHdrNo = @StHdrNo
	AND IsMainLocation = 1
Group By PartNo
HAVING
	Count(PartNo) > 1)
DROP TABLE #a2





GO


