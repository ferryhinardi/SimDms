SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspfn_SelectLMPDtl] @CompanyCode varchar(15), @BranchCode varchar(15), @LmpNo varchar(15)  
as  
SELECT     
	row_number () OVER (ORDER BY spTrnSLmpDtl.CreatedDate ASC) AS NoUrut,
	spTrnSLmpDtl.PartNo,
	spTrnSLmpDtl.PartNoOriginal,
	spTrnSLmpDtl.DocNo, 
	CONVERT(VARCHAR, spTrnSLmpDtl.DocDate, 106) AS DocDate, 
	spTrnSLmpDtl.ReferenceNo, 
	spTrnSLmpDtl.QtyBill
	FROM spTrnSLmpDtl
	WHERE
	spTrnSLmpDtl.CompanyCode = @CompanyCode AND
	spTrnSLmpDtl.BranchCode = @BranchCode AND
	spTrnSLmpDtl.LmpNo = @LmpNo