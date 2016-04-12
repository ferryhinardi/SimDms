create procedure [dbo].[sp_SpSrvLampLkp]
@CompanyCode varchar(15),  
@BranchCode varchar(15),   
@SalesType char(1)

as
SELECT
    DISTINCT LmpNo
    , LmpDate
    , (SELECT TOP 1 DocNo FROM spTrnSPickingDtl
		WHERE spTrnSPickingDtl.CompanyCode = spTrnSLmpHdr.CompanyCode
			AND spTrnSPickingDtl.BranchCode = spTrnSLmpHdr.BranchCode
			AND spTrnSPickingDtl.PickingSlipNo = spTrnSLmpHdr.PickingSlipNo
		) AS SSNo
	, (SELECT TOP 1 DocDate FROM spTrnSPickingDtl
		WHERE spTrnSPickingDtl.CompanyCode = spTrnSLmpHdr.CompanyCode
			AND spTrnSPickingDtl.BranchCode = spTrnSLmpHdr.BranchCode
			AND spTrnSPickingDtl.PickingSlipNo = spTrnSLmpHdr.PickingSlipNo
		) AS SSDate
    , spTrnSLmpHdr.BPSFNo
    , spTrnSLmpHdr.BPSFDate
    , spTrnSLmpHdr.PickingSlipNo
    , spTrnSLmpHdr.PickingSlipDate
FROM 
    spTrnSLmpHdr
    INNER JOIN spTrnSBPSFHdr ON spTrnSBPSFHdr.Companycode = spTrnSLmpHdr.CompanyCode 
            AND spTrnSBPSFHdr.BranchCode = spTrnSLmpHdr.BranchCode 
            AND spTrnSBPSFHdr.BPSFNo = spTrnSLmpHdr.BPSFNo 
WHERE 
    spTrnSLmpHdr.CompanyCode = @CompanyCode
    AND spTrnSLmpHdr.BranchCode = @BranchCode
    AND spTrnSBPSFHdr.SalesType = @SalesType
ORDER BY 
    LmpNo DESC
	
GO	
        