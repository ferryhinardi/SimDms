CREATE procedure [dbo].[sp_EdpTransNo] (  

@CompanyCode varchar(10),
@BranchCode varchar(10),
@TypeOfGoods varchar(10),
@LampiranNo varchar(10)
)


as

SELECT * INTO #t1 FROM ( 
SELECT
a.LampiranNo
, a.DealerCode as SupplierCode
, ISNULL(d.BranchCodeToDesc, '') as SupplierName
, ISNULL(a.TypeOfGoods, '') TypeofGoods
FROM spUtlStockTrfHdr a
LEFT JOIN spMstCompanyAccount d ON d.CompanyCode = a.CompanyCode
AND d.BranchCodeTo = a.DealerCode
WHERE a.CompanyCode = @CompanyCode
AND a.BranchCode = @BranchCode
AND a.Status in ('0','1') ) #t1

SELECT * FROM #t1 WHERE TypeofGoods = @TypeOfGoods 
AND LampiranNo = CASE @LampiranNo WHEN '' THEN LampiranNo ELSE @LampiranNo END

DROP TABLE #t1
    







GO


