-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		David Leonardo
-- Create date: 6 November 2014
-- Description:	Select Supply Slip for Print
-- =============================================
CREATE PROCEDURE uspfn_spPickingSlipForPrint
	-- Add the parameters for the stored procedure here
	@CompanyCode varchar(20),
	@BranchCode varchar(20),
	@ProductType varchar(5),
	@JobOrderNo varchar(50)
AS
BEGIN
	SELECT * INTO #t1 FROM (
                SELECT
                    DISTINCT c.DocNo, c.DocDate, d.PickingSlipNo, e.PartNo, e.PartNo PartNoOri, e.QtySupply, 
                    e.QtyPicked, e.QtyBill, d.Status, f.LookUpValueName TransTypeDesc, c.TransType, g.LmpNo,
                    d.PickedBy
                FROM
                    svTrnService a
                LEFT JOIN svTrnSrvItem b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode 
	                AND b.ProductType = a.ProductType AND b.ServiceNo=a.ServiceNo
                LEFT JOIN spTrnSOrdHdr c ON c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode 
	                AND c.DocNo = b.SupplySlipNo
                LEFT JOIN spTrnSPickingHdr d ON d.CompanyCode = a.CompanyCode AND d.BranchCode = a.BranchCode 
	                AND d.PickingSlipNo = c.ExPickingSlipNo
                LEFT JOIN spTrnSPickingDtl e ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode 
	                AND e.PickingSlipNo = d.PickingSlipNo
                LEFT JOIN gnMstLookUpDtl f ON f.CompanyCode = a.CompanyCode AND f.CodeId = 'TTSR' 
                    AND f.LookUpValue = c.TransType
                LEFT JOIN spTrnSLmpHdr g ON g.CompanyCode = a.CompanyCode AND g.BranchCode = a.BranchCode 
                    AND g.PickingSlipNo = d.PickingSlipNo
                WHERE 
                    a.CompanyCode     = @CompanyCode
                    AND a.BranchCode  = @BranchCode
                    AND a.ProductType = @ProductType
                    AND a.jobOrderNo  = @JobOrderNo
                    AND b.SupplySlipNo <> ''
                    AND b.PartSeq = (SELECT MAX(PartSeq) FROM SvTrnSrvItem WHERE CompanyCode =  @CompanyCode AND BranchCode = @BranchCode
		                   AND ProductType = '4W' AND ServiceNo = a.ServiceNo AND PartNo = b.PartNo)
                    AND d.Status < 2
            )#t1
            
            select a.PickingSlipNo, a.TypeOfGoods, a.TransType, a.SalesType,
            (SELECT TOP 1 DocNo FROM SpTrnSPickingDtl 
                WHERE CompanyCode = a.CompanyCode 
                    AND BranchCode = a.BranchCode 
                    AND PickingSlipNo = a.PickingSlipNo) DocNo 
            from spTrnSpickingHdr a
            where  CompanyCode =  @CompanyCode
				AND BranchCode = @BranchCode
				AND a.pickingSlipNo IN 
                (SELECT DISTINCT PickingSlipNo FROM #t1)
				AND Salestype = 2
            DROP TABLE #t1
END
GO
