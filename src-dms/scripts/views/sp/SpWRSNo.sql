

create view [dbo].[SpWRSNo]  as    
 SELECT * FROM (
SELECT a.CompanyCode, a.BranchCode,
	a.WRSNo, a.TypeOfGoods
	, WRSDate
	, COUNT(b.PartNo) CountPartNo
	, ISNULL(Claim.CountClaim, 0) CountClaim
FROM 
	spTrnPRcvHdr a WITH(NOLOCK, NOWAIT)
	INNER JOIN spTrnPRcvDtl b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
		AND a.BranchCode = b.BranchCode
		AND a.WRSNo = b.WRSNo
	LEFT JOIN (SELECT dtl.CompanyCode, dtl.BranchCode, hdr.WRSNo, ISNULL(COUNT(dtl.PartNo), 0) CountClaim FROM spTrnPClaimDtl dtl
				INNER JOIN spTrnPClaimHdr hdr ON dtl.CompanyCode = hdr.CompanyCode
				AND dtl.BranchCode = hdr.BranchCode
				AND dtl.ClaimNo = hdr.ClaimNo
                AND hdr.status <> '3'
				GROUP BY dtl.CompanyCode, dtl.BranchCode, hdr.WRSNo
	) Claim ON a.CompanyCode = Claim.CompanyCode
		AND a.BranchCode = Claim.BranchCode
		AND a.WRSNo = Claim.WRSNo
WHERE 
	--a.CompanyCode = '6006406'
	--AND a.BranchCode = '6006401'
	(a.Status = '2' OR a.Status = '4')
	--AND a.TypeOfGoods = '0' 
GROUP BY 
	a.WRSNo, WRSDate, CountClaim,a.TypeOfGoods, a.CompanyCode, a.BranchCode
) a
GROUP BY 
	a.WrsNo, a.WrsDate, a.CountPartNo, a.CountClaim,a.TypeOfGoods, a.CompanyCode, a.BranchCode
HAVING 
	a.CountPartNo > a.CountClaim


GO


