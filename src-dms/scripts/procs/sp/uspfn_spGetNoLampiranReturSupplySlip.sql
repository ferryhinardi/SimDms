CREATE procedure uspfn_spGetNoLampiranReturSupplySlip @CompanyCode varchar(15), @BranchCode varchar(15),   
@TypeOfGoods varchar(1), @ProductType varchar(2)  
as  
SELECT * FROM   
(  
 -- SalesType = 2 (Service / SSS)  
 SELECT  
  a.LmpNo  
  , a.LmpDate  
  , c.DocNo  
  , c.UsageDocNo  
 FROM  
  spTrnSLmpHdr a WITH(NOLOCK, NOWAIT)  
  LEFT JOIN spTrnSLmpDtl b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode  
   AND a.BranchCode = b.BranchCode  
   AND a.LmpNo = b.LmpNo  
  INNER JOIN spTrnSORDHdr c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode  
   AND a.BranchCode = c.BranchCode  
   AND b.DocNo = c.DocNo  
  INNER JOIN svTrnService d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode  
   AND a.BranchCode = d.BranchCode  
   AND b.ProductType = d.ProductType   
   AND c.UsageDocNo = d.JobOrderNo  
 WHERE  
  a.CompanyCode = @CompanyCode  
  AND a.BranchCode = @BranchCode  
  AND a.TypeOfGoods = @TypeOfGoods  
  AND b.ProductType = @ProductType  
  AND c.SalesType = '2'  
  AND d.ServiceStatus < 5  
  AND b.QtyBill -   
  ISNULL((  
   SELECT SUM(ISNULL(QtyReturn, 0))  
   FROM spTrnSRturSSDtl dtl WITH(NOLOCK, NOWAIT)  
    LEFT JOIN spTrnSRturSSHdr hdr WITH(NOLOCK, NOWAIT) ON dtl.CompanyCode = hdr.CompanyCode  
     AND dtl.BranchCode = hdr.BranchCode  
     AND dtl.ReturnNo = hdr.ReturnNo   
   WHERE dtl.CompanyCode = a.CompanyCode  
    AND dtl.BranchCode = a.BranchCode  
    AND TypeOfGoods = a.TypeOfGoods  
    AND ProductType = b.ProductType  
    AND PartNo = b.PartNo  
    AND PartNoOriginal = b.PartNoOriginal  
    AND dtl.DocNo = b.DocNo  
  ), 0) > 0  
  
 UNION  
  
 -- SalesType = 3 (Unit / SSU)  
 SELECT  
  a.LmpNo  
  , a.LmpDate  
  , c.DocNo  
  , c.UsageDocNo  
 FROM  
  spTrnSLmpHdr a WITH(NOLOCK, NOWAIT)  
  LEFT JOIN spTrnSLmpDtl b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode  
   AND a.BranchCode = b.BranchCode  
   AND a.LmpNo = b.LmpNo  
  INNER JOIN spTrnSORDHdr c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode  
   AND a.BranchCode = c.BranchCode  
   AND b.DocNo = c.DocNo  
  INNER JOIN omTrSalesSO d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode  
   AND a.BranchCode = d.BranchCode  
   AND c.UsageDocNo = d.SONo  
 WHERE  
  a.CompanyCode = @CompanyCode  
  AND a.BranchCode = @BranchCode  
  AND a.TypeOfGoods = @TypeOfGoods  
  AND b.ProductType = @ProductType  
  AND c.SalesType = '3'  
  AND d.Status IN ('2', '4')  
  AND b.QtyBill -   
  ISNULL((  
   SELECT SUM(ISNULL(QtyReturn, 0))  
   FROM spTrnSRturSSDtl dtl WITH(NOLOCK, NOWAIT)  
    LEFT JOIN spTrnSRturSSHdr hdr WITH(NOLOCK, NOWAIT) ON dtl.CompanyCode = hdr.CompanyCode  
     AND dtl.BranchCode = hdr.BranchCode  
     AND dtl.ReturnNo = hdr.ReturnNo   
   WHERE dtl.CompanyCode = a.CompanyCode  
    AND dtl.BranchCode = a.BranchCode  
    AND TypeOfGoods = a.TypeOfGoods  
    AND ProductType = b.ProductType  
    AND PartNo = b.PartNo  
    AND PartNoOriginal = b.PartNoOriginal  
    AND dtl.DocNo = b.DocNo  
  ), 0) > 0  
) Lampiran  
ORDER BY  
 Lampiran.LmpNo DESC