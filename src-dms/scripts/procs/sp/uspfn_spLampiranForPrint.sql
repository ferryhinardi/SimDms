USE [BITPKU]
GO
/****** Object:  StoredProcedure [dbo].[uspfn_spPickingSlipForPrint]    Script Date: 11/18/2014 3:13:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		David Leonardo
-- Create date: 6 November 2014
-- Description:	Select Lampiran for Print
-- =============================================
create PROCEDURE [dbo].[uspfn_spLampiranForPrint]
	-- Add the parameters for the stored procedure here
	@CompanyCode varchar(20),
	@BranchCode varchar(20),
	@JobOrderNo varchar(50)
AS
BEGIN
	SELECT a.LmpNo,a.PickingSlipNo, a.TypeOfGoods, a.TransType,
                SubString(a.TransType,1,1) SalesType,
                (SELECT TOP 1 DocNo FROM SpTrnSPickingDtl 
                    WHERE CompanyCode = a.CompanyCode 
                    AND BranchCode = a.BranchCode 
                    AND PickingSlipNo = a.PickingSlipNo) DocNo 
               FROM spTrnSLmpHdr a WHERE a.CompanyCode = @CompanyCode AND a.BranchCode = @BranchCode AND 
                    a.PickingSlipNo IN (@JobOrderNo)
END
