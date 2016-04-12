USE [BITPKU]
GO
/****** Object:  StoredProcedure [dbo].[sp_EdpPartNo_Pembelian]    Script Date: 1/27/2015 6:20:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




ALTER procedure [dbo].[sp_EdpPartNo_Pembelian]  (  
@CompanyCode varchar(10),
@BranchCode varchar(10),
@DocNo varchar(20)
--@BinningNo varchar(20),
--@Opt varchar(10)
)
 as 
 
--SELECT DISTINCT a.POSNo, a.PartNo, c.PartName, CASE when e.ReceiveQty is null then a.OnOrder else a.OrderQty-e.ReceiveQty end as ReminQty,
--d.PurchasePrice, a.DiscPct, 
--a.OnOrder as MaxReceived
--from spTrnPOrderBalance a 
--LEFT JOIN spTrnPBinnDtl b ON
--a.CompanyCode = b.CompanyCode
--and a.BranchCode = b.BranchCode
--and a.POSNo = b.DocNo
--AND a.PartNo = b.PartNo
--LEFT JOIN spMstItemInfo c ON
--a.CompanyCode = c.CompanyCode
--and a.PartNo = c.PartNo
--LEFT JOIN SpMstItemPrice d ON
--a.CompanyCode = d.CompanyCode
--and a.BranchCode = d.BranchCode
--AND a.PartNo = d.PartNo LEFT JOIN
--(SELECT PartNo, DocNo, sum(ReceivedQty) as ReceiveQty from spTrnPBinnDtl WHERE CompanyCode = @CompanyCode
--AND BranchCode = @BranchCode
--and DocNo = @DocNo GROUP BY PartNo, DocNo) e
--ON a.PartNo = e.PartNo
--AND a.POSNo = e.DocNo
--where
--a.CompanyCode = @CompanyCode
--AND a.BranchCode = @BranchCode
--and a.POSNo = @DocNo

SELECT c.PartNo, c.PartName, c.PurchasePrice, c.DiscPct, SUM(c.OnOrder) AS MaxReceived
FROM
(
    SELECT
     a.PartNo
    ,b.PartName 
    ,ISNULL((SELECT round(x.PurchasePrice + (x.PurchasePrice * (select (TaxPct/100) from gnMstTax
where TaxCode = (select ParaValue from gnMstLookUpDtl where CodeID = 'BINS' and SeqNo = 3))),0)
FROM SpMstItemPrice x where x.CompanyCode = a.CompanyCode AND
     x.BranchCode = a.BranchCode AND x.PartNo = a.PartNo),0) AS PurchasePrice
    ,a.OnOrder
    ,a.DiscPct
    FROM spTrnPOrderBalance a 
    INNER JOIN spMstItemInfo b
       ON b.CompanyCode = a.CompanyCode
      AND b.PartNo      = a.PartNo
      WHERE a.CompanyCode = @CompanyCode
      AND a.BranchCode  = @BranchCode
      AND a.PosNo       = @DocNo      
) c
GROUP BY c.PartNo, c.PartName, c.PurchasePrice, c.DiscPct
HAVING sum(c.OnOrder) > 0



