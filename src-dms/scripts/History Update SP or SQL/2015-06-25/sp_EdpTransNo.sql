/****** Object:  StoredProcedure [dbo].[sp_EdpTransNo]    Script Date: 6/25/2015 2:42:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER procedure [dbo].[sp_EdpTransNo] (  

@CompanyCode varchar(10),
@BranchCode varchar(10),
@TypeOfGoods varchar(10),
@isTrex bit
--@LampiranNo varchar(10)
)

as

SELECT * INTO #t1 FROM ( 
SELECT
    a.LampiranNo
    , a.DealerCode as SupplierCode
    , ISNULL(b.SupplierName, '') as SupplierName
    , ISNULL(c.TypeOfGoods, '') TypeofGoods
FROM spUtlStockTrfHdr a
LEFT JOIN GnMstSupplier b ON b.CompanyCode = a.CompanyCode 
    AND b.SupplierCode = a.DealerCode
LEFT JOIN SpTrnSLmpHdr c ON c.CompanyCode = a.CompanyCode 
    AND c.BranchCode = a.DealerCode
    AND c.LmpNo = a.LampiranNo
WHERE a.CompanyCode = @CompanyCode
  AND a.BranchCode = @BranchCode
  AND a.Status in ('0','1') 
  AND c.isLocked = CASE @isTrex WHEN 1 THEN 1 ELSE 0 END
  ) #t1

SELECT * FROM #t1 WHERE TypeofGoods = @TypeOfGoods 

DROP TABLE #t1

