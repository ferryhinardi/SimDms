/****** Object:  StoredProcedure [dbo].[uspfn_LookupNoDnPINVS]    Script Date: 9/15/2015 10:42:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[uspfn_LookupNoDnPINVS]
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@TypeOfGoods varchar(1)
AS
BEGIN
	SELECT DISTINCT a.CompanyCode, BranchCode,
		a.DeliveryNo 
		,a.SupplierCode
		,ISNULL(b.SupplierName,'') as SupplierName 
		FROM spUtlPINVDDTL a
		LEFT JOIN GnMstSupplier b ON a.CompanyCode=b.CompanyCode 
		AND a.SupplierCode = b.SupplierCode
		WHERE a.CompanyCode = @CompanyCode AND a.BranchCode = @BranchCode
		and a.TypeOfGoods = @TypeOfGoods and a.Status in('0', '1')
END