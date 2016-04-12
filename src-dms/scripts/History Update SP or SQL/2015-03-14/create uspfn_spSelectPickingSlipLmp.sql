if object_id('uspfn_spSelectPickingSlipLmp') is not null
	drop procedure uspfn_spSelectPickingSlipLmp
GO
create procedure [dbo].[uspfn_spSelectPickingSlipLmp] @CompanyCode varchar(15), @BranchCode varchar(15), 
@ProductType varchar(4), @JobOrderNo varchar(25)
as
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
                    AND b.PartSeq = (SELECT MAX(PartSeq) FROM SvTrnSrvItem WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
		                   AND ProductType = @ProductType AND ServiceNo = a.ServiceNo AND PartNo = b.PartNo)
                    AND d.Status <= 2
            )#t1
            SELECT Row_number() OVER(ORDER BY DocNo) No, * FROM #t1
            DROP TABLE #t1
GO
