/****** Object:  StoredProcedure [dbo].[uspfn_spTrnIAdjustDtlview]    Script Date: 8/18/2014 8:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[uspfn_spTrnIAdjustDtlview]
@CompanyCode varchar(15),
@BranchCode varchar(15),
@AdjustmentNo varchar(25) 
 
AS
SELECT 
        row_number () OVER (ORDER BY spTrnIAdjustDtl.CreatedDate) AS No,
        spTrnIAdjustDtl.PartNo,
        spMstItemInfo.PartName,
        spTrnIAdjustDtl.WarehouseCode,	
        spTrnIAdjustDtl.LocationCode,
		spTrnIAdjustDtl.AdjustmentCode,
        gnMstLookUpDtl_1.LookUpValueName AS AdjustmentDesc,
	    gnMstLookUpDtl_2.LookUpValueName AS WarehouseName,
        spTrnIAdjustDtl.QtyAdjustment,
		spTrnIAdjustDtl.ReasonCode,
        gnMstLookUpDtl.LookUpValueName AS ReasonDesc
    FROM 
        spTrnIAdjustDtl
        INNER JOIN spTrnIAdjustHdr ON spTrnIAdjustHdr.AdjustmentNo = spTrnIAdjustDtl.AdjustmentNo 
            AND spTrnIAdjustHdr.CompanyCode =  spTrnIAdjustDtl.CompanyCode
            AND spTrnIAdjustHdr.BranchCode =  spTrnIAdjustDtl.BranchCode
        INNER JOIN spMstItemInfo ON spMstItemInfo.PartNo = spTrnIAdjustDtl.PartNo
            AND spMstItemInfo.CompanyCode = spTrnIAdjustDtl.CompanyCode
        INNER JOIN gnMstLookUpDtl ON gnMstLookUpDtl.LookUpValue = spTrnIAdjustDtl.ReasonCode
            AND gnMstLookUpDtl.CompanyCode = spTrnIAdjustDtl.CompanyCode
        INNER JOIN gnMstLookUpDtl AS gnMstLookUpDtl_1 ON gnMstLookUpDtl_1.LookUpValue = spTrnIAdjustDtl.AdjustmentCode
            AND gnMstLookUpDtl_1.CompanyCode = spTrnIAdjustDtl.CompanyCode
	    INNER JOIN gnMstLookUpDtl AS gnMstLookUpDtl_2 ON gnMstLookUpDtl_2.LookUpValue = spTrnIAdjustDtl.WarehouseCode
            AND gnMstLookUpDtl_2.CompanyCode = spTrnIAdjustDtl.CompanyCode
    WHERE 
        spTrnIAdjustDtl.CompanyCode = @CompanyCode
        AND spTrnIAdjustDtl.BranchCode = @BranchCode
        AND gnMstLookUpDtl.CodeId='RSAD'
	    AND gnMstLookUpDtl_2.CodeId='WRCD'
        AND gnMstLookUpDtl_1.CodeID = 'ADJS'
        AND spTrnIAdjustDtl.AdjustmentNo =  @AdjustmentNo
    ORDER BY spTrnIAdjustDtl.CreatedDate