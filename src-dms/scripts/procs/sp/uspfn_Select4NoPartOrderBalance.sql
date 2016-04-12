USE [BITPKU]
GO
/****** Object:  StoredProcedure [dbo].[uspfn_Select4NoPartOrderBalance]    Script Date: 2/11/2015 4:31:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create procedure [dbo].[uspfn_Select4NoPartOrderBalance] @CompanyCode varchar(15),
@BranchCode varchar(15), @PosNo varchar(20)
as
SELECT 
	a.POSNo, a.PartNo, b.PartName, CAST(a.OrderQty as decimal(18,2)) as OrderQty, 
	a.OnOrder, a.Intransit, a.Received,a.DiscPct, a.PurchasePrice, 
	Convert(varchar(10),a.SeqNo) SeqNo, a.SupplierCode, a.OnOrder, a.PartNoOriginal, 
	a.TypeOfGoods 
FROM 
	spTrnPOrderBalance a 
INNER JOIN spMstItemInfo b
   ON b.PartNo      = a.PartNo
  AND b.CompanyCode = a.CompanyCode
WHERE a.CompanyCode = @CompanyCode
  AND a.BranchCode  = @BranchCode
  AND a.PosNo    like @PosNo
ORDER BY a.POSNo DESC, a.SeqNo