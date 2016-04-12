Create PROCEDURE uspfn_GetTrnPickingHdr @CompanyCode varchar(15),
@BranchCode varchar(15), 
@TypeOfGoods varchar(2),
@isBORelease bit
as
SELECT 
	DISTINCT(a.PickingSlipNo)
    , a.PickingSlipDate
    , ISNULL(b.CustomerCode,  c.BranchCodeTo) CustomerCode  
    , ISNULL(b.CustomerName,  c.BranchCodeToDesc) CustomerName
    --, spTrnSPickingDtl.ExPickingSlipNo
FROM 
	spTrnSPickingHdr a
LEFT JOIN gnMstCustomer b ON b.CompanyCode = a.CompanyCode
    AND b.CustomerCode = a.CustomerCode 		
LEFT JOIN spMstCompanyAccount c ON c.CompanyCode = a.CompanyCode
    AND c.BranchCodeTo = a.CustomerCode
LEFT JOIN gnMstCustomerProfitCenter d ON d.CompanyCode = a.CompanyCode 
	AND d.BranchCode = a.BranchCode
    AND d.CustomerCode = b.CustomerCode 
LEFT JOIN gnMstCustomerClass e ON e.CompanyCode = a.CompanyCode 
    AND e.BranchCode = a.BranchCode
    AND e.ProfitCenterCode = d.ProfitCenterCode 
	AND e.CustomerClass = d.CustomerClass 
LEFT JOIN spTrnSPickingDtl f ON f.CompanyCode = a.CompanyCode 
    AND f.BranchCode = a.BranchCode 
    AND f.PickingSlipNo = a.PickingSlipNo 
WHERE 
    a.CompanyCode= @CompanyCode
    AND a.BranchCode= @BranchCode
    AND (a.Status = 0 OR a.Status = 1) 
    AND a.TypeOfGoods = @TypeOfGoods AND a.isBORelease = @isBORelease  ORDER BY a.PickingSlipNo DESC