alter procedure [dbo].[uspfn_GenerateLampiranNP]    
 @CompanyCode VARCHAR(MAX),    
 @BranchCode  VARCHAR(MAX),    
 @PickingSlipNo VARCHAR(MAX),    
 @LmpDate  DATETIME,    
 @ProductType VARCHAR(MAX),    
 @UserID   VARCHAR(MAX),    
 @TypeOfGoods VARCHAR(MAX)    
AS    
BEGIN    
  DECLARE @MaxLmpNo INT    
  ---remove where clause DocumentYear refer from GetNewDocNo
  SET @MaxLmpNo = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE     
   CompanyCode = @CompanyCode    
    AND BranchCode = @BranchCode    
    AND DocumentType = 'LMP'     
    AND ProfitCenterCode = '300'     
    --AND DocumentYear = YEAR(GetDate())
	),0)    
	
	declare @DocYearMaxLmpNo int
	SET @DocYearMaxLmpNo = ISNULL((SELECT DocumentYear FROM GnMstDocument WHERE     
   CompanyCode = @CompanyCode    
    AND BranchCode = @BranchCode    
    AND DocumentType = 'LMP'     
    AND ProfitCenterCode = '300'     
    --AND DocumentYear = YEAR(GetDate())
	),0)
    
  DECLARE @errmsg VARCHAR(MAX)    
  DECLARE @TempLmpNo VARCHAR(MAX)    
  DECLARE @TempBPSFNo VARCHAR(MAX)    
  DECLARE @CustomerCode VARCHAR(MAX)    
    
  SET @TempLmpNo  = ISNULL((SELECT 'LMP/' + RIGHT(@DocYearMaxLmpNo,2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxLmpNo, 1), 6)),'LMP/YY/XXXXXX')    
  SET @TempBPSFNo = ISNULL((SELECT BPSFNo FROM SpTrnSBPSFHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PickingSlipNo = @PickingSlipNo),'')    
  SET @CustomerCode = ISNULL((SELECT CustomerCode FROM SpTrnSPickingHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PickingSlipNo = @PickingSlipNo),'')    

  print @TempLmpNo
  
  IF (ISNULL((SELECT Status FROM SpTrnSBPSFhdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND BPSFNo = @TempBPSFno), '0') = '2')    
  BEGIN    
   SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Nomor picking ini sudah pernah di-proses, hubungi IT support untuk pemerikasaan data lebih lanjut !'    
   RAISERROR (@errmsg,16,1);    
   RETURN    
  END    
    
  UPDATE SpTrnSBPSFHdr    
  SET Status = '2'    
   , LastUpdateDate = GetDate()    
   , LastUpdateBy = @UserID    
  WHERE CompanyCode = @CompanyCode    
   AND BranchCode = @BranchCode    
   AND BPSFNo = @TempBPSFNo    
    
  --===============================================================================================================================    
  IF (ISNULL((SELECT LmpNo FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo), '') <> '')    
  BEGIN    
   SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Nomor lampiran sudah ada, periksa setting-an sequence dokumen (LMP) pada general module !'+@TempLmpNo
   RAISERROR (@errmsg,16,1);    
   RETURN    
  END    
    
  DECLARE @isLocked BIT    
  SET @isLocked = (SELECT IsLocked FROM SpTrnSPickingHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PickingSlipNo = @PickingSlipNo)    
    
  INSERT INTO SpTrnSLmpHdr    
  SELECT    
   CompanyCode    
   , BranchCode    
   , @TempLmpNo LmpNo     
   , GetDate() LmpDate    
   , @TempBPSFNo BPSFNo    
   , (SELECT BPSFDate FROM SpTrnSBPSFHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND BPSFNo = @TempBPSFNo)    
    BPSFDate    
   , PickingSlipNo    
   , PickingSlipDate    
   , TransType    
   , CustomerCode    
   , CustomerCodeBill    
   , CustomerCodeShip    
   , TotSalesQty    
   , TotSalesAmt    
   , TotDiscAmt    
   , TotDPPAmt    
   , TotPPNAmt    
   , TotFinalSalesAmt    
   , CONVERT(BIT, 0) isPKP    
   , '0' Status    
   , 0 PrintSeq    
   , TypeOfGoods    
   , @UserID CreatedBy    
   , GetDate() CreatedDate    
   , @UserID LastUpdateBy    
   , GetDate() LastUpdateDate    
   , @isLocked IsLocked     
   , NULL LockingBy    
   , NULL LockingDate    
  FROM SpTrnSPickingHdr     
  WHERE     
   1 = 1    
   AND CompanyCode = @CompanyCode    
   AND BranchCode = @BranchCode    
   AND PickingSlipNo = @PickingSlipNo    
    
  UPDATE GnMstDocument    
  SET DocumentSequence = DocumentSequence + 1    
   , LastUpdateDate = GetDate()    
   , LastUpdateBy = @UserID    
  WHERE    
   1 = 1    
   AND CompanyCode = @CompanyCode    
   AND BranchCode = @BranchCode    
   AND DocumentType = 'LMP'    
   AND ProfitCenterCode = '300'    
   --AND DocumentYear = Year(GetDate())    
    
  --===============================================================================================================================    
  -- INSERT LAMPIRAN DETAIL    
  --===============================================================================================================================    
  INSERT INTO SpTrnSLmpDtl    
  SELECT    
   a.CompanyCode    
   , a.BranchCode    
   , @TempLmpNo LmpNo    
   , a.WarehouseCode    
   , a.PartNo    
   , a.PartNoOriginal    
   , a.DocNo    
   , a.DocDate    
   , a.ReferenceNo    
   , a.ReferenceDate    
   , a.LocationCode    
   , a.QtyBill    
   , a.RetailPriceInclTax    
   , a.RetailPrice    
   , ISNULL((SELECT CostPrice FROM SpMstItemPrice WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PartNo = a.PartNo),0) CostPrice    
   , a.DiscPct    
   , a.SalesAmt    
   , a.DiscAmt    
   , a.NetSalesAmt    
   , 0 PPNAmt    
   , a.TotSalesAmt    
   , a.ProductType    
   , a.PartCategory     
   , a.MovingCode    
   , a.ABCClass    
   , @UserID CreatedBy    
   , GetDate() CreatedDate    
   , @UserID LastUpdateBy    
   , GetDate() LastUpdateDate    
  FROM SpTrnSPickingDtl a    
  WHERE     
   1 = 1    
   AND a.CompanyCode = @CompanyCode    
   AND a.BranchCode = @BranchCode    
   AND a.PickingSlipNo = @PickingSlipNo    
   AND a.QtyPicked > 0    
    
    
  --===============================================================================================================================    
  -- UPDATE STOCK    
  -- NOTES : Transtype = 11 --> + BorrowedQty    
  --         Transtype = 12 --> - BorrowQty    
  --===============================================================================================================================    
  DECLARE @TempTransType VARCHAR (MAX)     
  SET @TempTransType = ISNULL((SELECT SUBSTRING(TransType,1,1) FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0)    
    
  --===============================================================================================================================    
  -- VALIDATION QTY    
  --===============================================================================================================================    
  DECLARE @Onhand_Valid NUMERIC(18,2)     
  DECLARE @Allocation_SRValid NUMERIC(18,2)    
  DECLARE @Allocation_SLValid NUMERIC(18,2)    
  DECLARE @Allocation_SPValid NUMERIC(18,2)    
      
  SELECT * INTO #Valid_2 FROM(    
   SELECT a.PartNo    
    , b.AllocationSR - a.QtyBill QtyValidSR    
    , b.AllocationSL - a.QtyBill QtyValidSL    
    , b.AllocationSP - a.QtyBill QtyValidSP    
    , b.Onhand - a.QtyBill QtyValidOnhand    
   FROM SpTrnSPickingDtl a    
   INNER JOIN SpMstItems b ON b.CompanyCode = a.CompanyCode     
    AND b.BranchCode = a.BranchCode     
    AND b.PartNo = a.PartNo      
   WHERE 1 = 1    
    AND a.CompanyCode = @CompanyCode    
    AND a.BranchCode = @BranchCode    
    AND a.PickingSlipNo = @PickingSlipNo     
  ) #Valid_2    
      
  SET @Allocation_SRValid = ISNULL((SELECT TOP 1 QtyValidSR FROM #Valid_2 WHERE QtyValidSR < 0),0)    
  SET @Allocation_SPValid = ISNULL((SELECT TOP 1 QtyValidSP FROM #Valid_2 WHERE QtyValidSP < 0),0)    
  SET @Allocation_SLValid = ISNULL((SELECT TOP 1 QtyValidSL FROM #Valid_2 WHERE QtyValidSL < 0),0)    
  SET @Onhand_Valid = ISNULL((SELECT TOP 1 QtyValidOnhand FROM #Valid_2 WHERE QtyValidOnhand < 0),0)    
    
  DROP TABLE #Valid_2    
  IF (@TempTransType = '2')    
  BEGIN    
   IF (@Onhand_Valid < 0 OR @Allocation_SRValid < 0)    
   BEGIN    
    SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Terdapat part dengan quantity Onhand atau alokasi kurang dari nol !'    
    RAISERROR (@errmsg,16,1);    
    RETURN    
   END     
  END    
      
  IF (@TempTransType = '1')    
  BEGIN    
   IF (@Onhand_Valid < 0 OR @Allocation_SPValid < 0)    
   BEGIN    
    SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Terdapat part dengan quantity Onhand atau alokasi kurang dari nol !'    
    RAISERROR (@errmsg,16,1);    
    RETURN    
   END     
  END     
    
  IF (@TempTransType = '3')    
  BEGIN    
   IF (@Onhand_Valid < 0 OR @Allocation_SLValid < 0)    
   BEGIN    
    SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Terdapat part dengan quantity Onhand atau alokasi kurang dari nol !'    
    RAISERROR (@errmsg,16,1);    
    RETURN    
   END     
  END     
  --===============================================================================================================================    
  IF (ISNULL((SELECT TransType FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0) = '11')    
  BEGIN    
  UPDATE SpMstItems    
  SET    
   BorrowedQty = BorrowedQty + b.QtyBill    
  FROM SpMstItems a, SpTrnSPickingDtl b    
  WHERE    
   1 = 1    
   AND a.CompanyCode = @CompanyCode    
   AND a.BranchCode = @BranchCode    
   AND b.PickingSlipNo = @PickingSlipNo    
   AND a.CompanyCode = b.CompanyCode    
   AND a.BranchCode = b.BranchCode    
   AND a.PartNo = b.PartNo    
  END    
    
  IF (ISNULL((SELECT TransType FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0) = '12')    
  BEGIN    
   UPDATE SpMstItems    
   SET    
    BorrowQty = BorrowQty - b.QtyBill    
   FROM SpMstItems a, SpTrnSPickingDtl b    
   WHERE    
    1 = 1    
    AND a.CompanyCode = @CompanyCode    
    AND a.BranchCode = @BranchCode    
    AND b.PickingSlipNo = @PickingSlipNo    
    AND a.CompanyCode = b.CompanyCode    
    AND a.BranchCode = b.BranchCode    
    AND a.PartNo = b.PartNo    
  END    
    
  IF (@TempTransType = '2')    
  BEGIN    
  UPDATE SpMstItems    
  SET    
   AllocationSR = AllocationSR - b.QtyBill    
   , Onhand = Onhand - b.QtyBill    
   , LastUpdateBy = @UserID    
   , LastUpdateDate = GetDate()    
   , LastSalesDate = GetDate()    
  FROM SpMstItems a, SpTrnSPickingDtl b    
  WHERE    
   1 = 1    
   AND a.CompanyCode = @CompanyCode    
   AND a.BranchCode = @BranchCode    
   AND b.PickingSlipNo = @PickingSlipNo    
   AND a.CompanyCode = b.CompanyCode    
   AND a.BranchCode = b.BranchCode    
   AND a.PartNo = b.PartNo    
    
  UPDATE SpMstItemLoc    
  SET    
   AllocationSR = AllocationSR - b.QtyBill    
   , Onhand = Onhand - b.QtyBill    
   , LastUpdateBy = @UserID    
   , LastUpdateDate = GetDate()    
  FROM SpMstItemLoc a, SpTrnSPickingDtl b    
  WHERE    
   1 = 1    
   AND a.CompanyCode = @CompanyCode    
   AND a.BranchCode = @BranchCode    
   AND a.WarehouseCode = '00'    
   AND b.PickingSlipNo = @PickingSlipNo    
   AND a.CompanyCode = b.CompanyCode    
   AND a.BranchCode = b.BranchCode    
   AND a.PartNo = b.PartNo    
  END    
    
  IF (@TempTransType = '1')    
  BEGIN    
   UPDATE SpMstItems    
   SET    
    AllocationSP = AllocationSP - b.QtyBill    
    , Onhand = Onhand - b.QtyBill    
    , LastUpdateBy = @UserID    
    , LastUpdateDate = GetDate()    
    , LastSalesDate = GetDate()    
   FROM SpMstItems a, SpTrnSPickingDtl b    
   WHERE    
    1 = 1    
    AND a.CompanyCode = @CompanyCode    
    AND a.BranchCode = @BranchCode    
    AND b.PickingSlipNo = @PickingSlipNo    
    AND a.CompanyCode = b.CompanyCode    
    AND a.BranchCode = b.BranchCode    
    AND a.PartNo = b.PartNo    
    
   UPDATE SpMstItemLoc    
   SET    
    AllocationSP = AllocationSP - b.QtyBill    
    , Onhand = Onhand - b.QtyBill    
    , LastUpdateBy = @UserID    
    , LastUpdateDate = GetDate()    
   FROM SpMstItemLoc a, SpTrnSPickingDtl b    
   WHERE    
    1 = 1    
    AND a.CompanyCode = @CompanyCode    
    AND a.BranchCode = @BranchCode    
    AND a.WarehouseCode = '00'    
    AND b.PickingSlipNo = @PickingSlipNo    
    AND a.CompanyCode = b.CompanyCode    
    AND a.BranchCode = b.BranchCode    
   AND a.PartNo = b.PartNo    
  END    
    
  IF (@TempTransType = '3')    
  BEGIN    
   UPDATE SpMstItems    
   SET    
    AllocationSL = AllocationSL - b.QtyBill    
    , Onhand = Onhand - b.QtyBill    
    , LastUpdateBy = @UserID    
    , LastUpdateDate = GetDate()    
    , LastSalesDate = GetDate()    
   FROM SpMstItems a, SpTrnSPickingDtl b    
   WHERE    
    1 = 1    
    AND a.CompanyCode = @CompanyCode    
    AND a.BranchCode = @BranchCode    
    AND b.PickingSlipNo = @PickingSlipNo    
    AND a.CompanyCode = b.CompanyCode    
    AND a.BranchCode = b.BranchCode    
    AND a.PartNo = b.PartNo    
    
   UPDATE SpMstItemLoc    
   SET    
    AllocationSL = AllocationSL - b.QtyBill    
    , Onhand = Onhand - b.QtyBill    
    , LastUpdateBy = @UserID    
    , LastUpdateDate = GetDate()    
   FROM SpMstItemLoc a, SpTrnSPickingDtl b    
   WHERE    
    1 = 1    
    AND a.CompanyCode = @CompanyCode    
    AND a.BranchCode = @BranchCode    
    AND a.WarehouseCode = '00'    
    AND b.PickingSlipNo = @PickingSlipNo    
    AND a.CompanyCode = b.CompanyCode    
    AND a.BranchCode = b.BranchCode    
    AND a.PartNo = b.PartNo    
  END    
  --===============================================================================================================================    
  -- UPDATE DEMAND CUST AND DEMAND ITEM    
  --===============================================================================================================================    
    
  UPDATE SpHstDemandCust    
  SET SalesFreq = SalesFreq + 1    
   , SalesQty = SalesQty + b.QtyBill    
   , LastUpdateBy = @UserID     
   , LastUpdateDate = GetDate()    
  FROM SpHstDemandCust a, SpTrnSPickingDtl b    
  WHERE    
   1 = 1    
   AND a.CompanyCode = @CompanyCode    
   AND a.BranchCode = @BranchCode    
   AND b.CompanyCode = @CompanyCode    
   AND b.BranchCode = @BranchCode    
   AND b.PickingSlipNo = @PickingSlipNo    
   AND a.CompanyCode = b.CompanyCode    
   AND a.BranchCode = b.BranchCode    
   AND a.Year = Year(b.DocDate)    
   AND a.Month = Month(b.DocDate)    
   AND a.CustomerCode = @CustomerCode    
   AND a.PartNo = b.PartNo    
       
  UPDATE SpHstDemandItem    
  SET SalesFreq = SalesFreq + 1    
   , SalesQty = SalesQty + b.QtyBill    
   , LastUpdateBy = @UserID     
   , LastUpdateDate = GetDate()    
  FROM SpHstDemandItem a, SpTrnSPickingDtl b    
  WHERE    
   1 = 1    
   AND a.CompanyCode = @CompanyCode    
   AND a.BranchCode = @BranchCode    
   AND b.CompanyCode = @CompanyCode    
   AND b.BranchCode = @BranchCode    
   AND b.PickingSlipNo = @PickingSlipNo    
   AND a.CompanyCode = b.CompanyCode    
   AND a.BranchCode = b.BranchCode    
   AND a.Year = Year(b.DocDate)    
   AND a.Month = Month(b.DocDate)    
   AND a.PartNo = b.PartNo    
    
  ----=============================================================================================================================    
  ---- INSERT TO ITEM MOVEMENT    
  ----=============================================================================================================================    
  INSERT INTO SpTrnIMovement    
  SELECT    
   @CompanyCode CompanyCode    
   , @BranchCode BranchCode    
   , a.LmpNo DocNo    
   , (SELECT LmPDate FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode     
    AND BranchCode = @BranchCode AND LmpNo = a.LmpNo)     
     DocDate    
   , dateadd(s,ROW_NUMBER() OVER(Order by a.PartNo),getdate()) CreatedDate     
   , '00' WarehouseCode    
   , LocationCode     
   , a.PartNo    
   , 'OUT' SignCode    
   , 'LAMPIRAN' SubSignCode    
   , a.QtyBill    
   , a.RetailPrice    
   , a.CostPrice    
   , a.ABCClass    
   , a.MovingCode    
   , a.ProductType    
   , a.PartCategory    
   , @UserID CreatedBy    
  FROM SpTrnSLmpDtl a    
  WHERE    
   1 = 1    
   AND CompanyCode = @CompanyCode    
   AND BranchCode = @BranchCode    
   AND LmpNo = @TempLmpNo    
    
    
  --===============================================================================================================================    
  -- UPDATE AVERAGE COST    
  -- NOTES : Transtype = 2% (SERVICE) CHECK ISLINKTOSERVICE    
  --===============================================================================================================================    
  IF (ISNULL((SELECT SUBSTRING(TransType,1,1) FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0) = '2')    
  BEGIN    
   IF (CONVERT(VARCHAR,ISNULL((SELECT IsLinkToService FROM gnMstCoProfile WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode),0),0) = '1')    
   BEGIN    
    SELECT    
     a.CompanyCode    
     , a.BranchCode    
     , d.ProductType    
     , ISNULL(d.ServiceNo, 0) ServiceNo    
     , a.PartNo    
     , a.DocNo SupplySlipNo    
     , ISNULL(a.CostPrice, 0) CostPrice    
       , ISNULL(a.RetailPrice, 0) RetailPrice    
    INTO #1    
    FROM spTrnSLmpDtl a     
     INNER JOIN spTrnSORDHdr c ON a.CompanyCode = c.CompanyCode    
      AND a.BranchCode = c.BranchCode    
      AND a.DocNo = c.DocNo    
     INNER JOIN svTrnService d ON a.CompanyCode = d.CompanyCode    
      AND a.BranchCode = d.BranchCode    
    WHERE a.CompanyCode = @CompanyCode    
     AND a.BranchCode = @BranchCode    
     AND d.ProductType = @ProductType    
     AND c.UsageDocNo = d.JobOrderNo    
     AND a.LmpNo = @TempLmpNo    
    
    UPDATE svTrnSrvItem     
    SET CostPrice = b.CostPrice    
     , LastUpdateBy = @UserID    
     , LastUpdateDate = GETDATE()    
    FROM svTrnSrvItem a, #1 b    
    WHERE a.CompanyCode = b.CompanyCode    
     AND a.BranchCode = b.BranchCode    
     AND a.ProductType = b.ProductType    
     AND a.ServiceNo = b.ServiceNo      
     AND a.PartNo = b.PartNo    
     AND a.SupplySlipNo = b.SupplySlipNo     
    
    --===============================================================================================================================    
    -- SERVICE PART    
    --===============================================================================================================================    
    SELECT * INTO #TempServiceItem FROM (    
    SELECT     
     a.CompanyCode    
     , a.BranchCode    
     , a.ProductType    
     , a.ServiceNo    
     , a.PartNo    
     , a.PartSeq    
     , a.DemandQty    
     , a.SupplyQty    
     , b.QtyBill    
     , b.DocNo    
     , a.CostPrice    
     , a.RetailPrice    
     , a.TypeOfGoods    
     , a.BillType    
     , a.DiscPct    
    FROM SvTrnSrvItem a     
    INNER JOIN SpTrnSPickingDtl b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo AND a.SupplySlipNo = b.DocNo    
    WHERE    
     1 = 1    
     AND a.CompanyCode = @CompanyCode    
     AND a.BranchCode = @BranchCode    
     AND a.ProductType = @ProductType    
     AND a.ServiceNo IN (SELECT ServiceNo     
          FROM SvTrnService     
          WHERE 1 = 1 AND CompanyCode = @CompanyCode     
           AND BranchCode = @BranchCode     
           AND JobOrderNo IN (SELECT ReferenceNo     
                FROM SpTrnSPickingDtl     
                WHERE 1= 1 AND CompanyCode = @CompanyCode     
                 AND  BranchCode = @BranchCode     
                 AND PickingSlipNo = @PickingSlipNo))    
     AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode    
      AND ProductType = @ProductType AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo)    
     AND a.SupplySlipNo = b .DocNo    
    ) #TempServiceItem     
    
    UPDATE svTrnSrvItem    
    SET SupplyQty = (CASE WHEN b.QtyBill > b.DemandQty     
        THEN     
         CASE WHEN b.DemandQty = 0 THEN b.QtyBill ELSE b.DemandQty END    
        ELSE b.QtyBill END)    
     , LastUpdateBy = @UserID    
     , LastUpdateDate = Getdate()    
    FROM svTrnSrvItem a, #TempServiceItem b    
    WHERE    
     1 = 1    
     AND a.CompanyCode = b.CompanyCode    
     AND a.BranchCode = b.BranchCode    
     AND a.ProductType = b.ProductType    
     AND a.ServiceNo = b.ServiceNo    
     AND a.PartNo = b.PartNo    
     AND a.PartSeq = b.PartSeq    
     AND a.SupplySlipNo = b.DocNo    
    
    UPDATE svTrnSrvItem    
    SET CostPrice = b.CostPrice    
     , LastUpdateBy = @UserID    
     , LastUpdateDate = Getdate()    
    FROM svTrnSrvItem a, #TempServiceItem b    
    WHERE    
     1 = 1    
     AND a.CompanyCode = b.CompanyCode    
     AND a.BranchCode = b.BranchCode    
     AND a.ProductType = b.ProductType    
     AND a.ServiceNo = b.ServiceNo    
     AND a.PartNo = b.PartNo    
     AND a.SupplySlipNo = b.DocNo    
    
    --===============================================================================================================================    
    -- INSERT NEW SRV ITEM BASED PICKING LIST    
    --===============================================================================================================================    
    INSERT INTO SvTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate
,  
 LastupdateBy, LastupdateDate, DiscPct)    
    SELECT     
     a.CompanyCode    
     , a.BranchCode    
     , a.ProductType    
     , a.ServiceNo    
     , a.PartNo    
     , (select max(PartSeq)+1 from svTrnSrvItem b where b.CompanyCode = a.CompanyCode and b.BranchCode = a.BranchCode and     
      b.ProductType = a.ProductType and b.ServiceNo = a.ServiceNo) PartSeq    
     , 0 DemandQty    
     , a.QtyBill - a.DemandQty SupplyQty    
     , 0 ReturnQty    
     , a.CostPrice    
     , a.RetailPrice    
     , a.TypeOfGoods    
     , a.BillType    
     , a.DocNo SupplySlipNo    
     , (SELECT TOP 1 DocDate FROM SpTrnSORDHdr WHERE 1= 1 AND CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode    
      AND DocNo = a.DocNo) SupplySlipDate    
     , NULL SSReturnNo    
     , NULL SSReturnDate    
     , @UserID CreatedBy    
     , GetDate() CreatedDate    
     , @UserID LastUpdateBy    
     , GetDate() LastUpdateDate    
     , a.DiscPct    
    FROM #TempServiceItem a     
    WHERE    
     1 = 1    
     AND a.CompanyCode = @CompanyCode    
     AND a.BranchCode = @BranchCode    
     AND a.ProductType = @ProductType    
     AND a.DemandQty < a.QtyBill    
     AND a.QtyBill > 0    
     AND a.DemandQty > 0    
    
    DROP TABLE #TempServiceItem      
    DROP TABLE #1    
   END    
  END    
    
  --===============================================================================================================================    
  -- GENERATE JOURNAL AND AUTOMATE TRANSFER STOCK    
  -- NOTES : Transtype = 10 (TRANSFER STOCK)    
  --===============================================================================================================================    
  DECLARE @TempJournalPrefix VARCHAR(MAX)    
  DECLARE @MaxTempJournal  INT    
    
  DECLARE @TempJournal  VARCHAR(MAX)    
  DECLARE @Amount    NUMERIC(18,2)    
  DECLARE @TempFiscalMonth INT    
  DECLARE @TempFiscalYear  INT    
    
  DECLARE @PeriodeNum   NUMERIC(18,0)    
  DECLARE @Periode   VARCHAR(MAX)    
  DECLARE @PeriodeName  VARCHAR(MAX)    
  DECLARE @GLDate    DATETIME    
    
  SET @TempFiscalYear = ISNULL((SELECT FiscalYear FROM GnMstCoProfileSpare WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode),0)     
  SET @TempFiscalMonth  = ISNULL((SELECT FiscalMonth FROM GnMstCoProfileSpare WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode),0)     
    
  SET @PeriodeNum = ISNULL((SELECT  TOP 1 PeriodeNum    
    FROM gnMstPeriode     
    WHERE CompanyCode = @CompanyCode    
     AND BranchCode = @BranchCode AND FiscalYear = @TempFiscalYear    
     AND FiscalMonth = @TempFiscalMonth AND StatusSparepart = 1    
     AND (MONTH(FromDate) = MONTH(@LmpDate) AND YEAR(FromDate) = YEAR(@LmpDate))    
     AND FiscalStatus = 1 ), NULL)     
    
  SET @Periode = ISNULL((SELECT  TOP 1 CONVERT(varchar, FiscalYear) + RIGHT('00' + CONVERT(varchar, PeriodeNum), 2) AS Periode    
    FROM gnMstPeriode     
    WHERE CompanyCode = @CompanyCode    
     AND BranchCode = @BranchCode AND FiscalYear = @TempFiscalYear    
     AND FiscalMonth = @TempFiscalMonth AND StatusSparepart = 1    
     AND (MONTH(FromDate) = MONTH(@LmpDate) AND YEAR(FromDate) = YEAR(@LmpDate))    
     AND FiscalStatus = 1 ), NULL)     
    
  SET @PeriodeName =  ISNULL((SELECT  TOP 1 PeriodeName    
    FROM gnMstPeriode     
    WHERE CompanyCode = @CompanyCode    
     AND BranchCode = @BranchCode AND FiscalYear = @TempFiscalYear    
     AND FiscalMonth = @TempFiscalMonth AND StatusSparepart = 1    
     AND (MONTH(FromDate) = MONTH(@LmpDate) AND YEAR(FromDate) = YEAR(@LmpDate))    
     AND FiscalStatus = 1 ), NULL)    
    
  DECLARE @AccountTypeInTran VARCHAR(MAX)    
  DECLARE @AccountTypeInvent VARCHAR(MAX)    
    
  SET @AccountTypeInTran = ISNULL((SELECT b.AccountType FROM GnMstAccount b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode AND b.AccountNo =     
    ISNULL((SELECT InTransitAccNo FROM SpMstAccount WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND TypeOfGoods = @TypeOfGoods),'')    
    ),'')    
  SET @AccountTypeInvent = ISNULL((SELECT b.AccountType FROM GnMstAccount b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode AND b.AccountNo =     
    ISNULL((SELECT InventoryAccNo FROM SpMstAccount WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND TypeOfGoods = @TypeOfGoods),'')    
    ),'')    
    
  --===============================================================================================================================    
  -- SET ACCOUNT FOR GENERATE JOURNAL    
  --===============================================================================================================================    
  DECLARE @CustCode VARCHAR(MAX)    
  SET @CustCode = (SELECT CustomerCode FROM spTrnSPickingHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PickingSlipNo = @PickingSlipNo)    
      
  Declare @TPGO VARCHAR(MAX)    
  SET @TPGO = (SELECT TypeOfGoods FROM spTrnSPickingHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PickingSlipNo = @PickingSlipNo)    
    
  DECLARE @InventoryAccNo VARCHAR(MAX)    
  DECLARE @InTransitAccNo VARCHAR(MAX)    
    
  IF (@isLocked = '1')    
  BEGIN    
   DECLARE @CompTo VARCHAR(MAX)    
   SET @CompTo = (SELECT ISNULL(CompanyCodeTo,'') FROM spMstCompanyAccount WHERE CompanyCode = @CompanyCode AND BranchCodeTo = @CustCode)    
       
   SET @InTransitAccNo =  (SELECT ISNULL(IntercompanyAccNoTo,'') FROM spMstCompanyAccountDtl WHERE CompanyCode = @CompanyCode AND CompanyCodeTo = @CompTo AND TPGO = @TPGO)    
  END    
  ELSE    
   SET @InTransitAccNo =  ISNULL((SELECT InTransitAccNo FROM SpMstAccount WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND TypeOfGoods = @TypeOfGoods),'')    
    
  SET @InventoryAccNo = ISNULL((SELECT InventoryAccNo FROM SpMstAccount WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND TypeOfGoods = @TypeOfGoods),'')    
  SET @Amount = ISNULL((SELECT SUM(QtyBill * CostPrice) FROM SpTrnSLmpDtl WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0)    
    
  --===============================================================================================================================    
  -- GENERATE JOURNAL    
  -- NOTES : Transtype = 1O (TRANSFER STOCK)    
  --===============================================================================================================================    
  IF (ISNULL((SELECT TransType FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0) = '10')    
  BEGIN    
   IF (@isLocked != '1')    
   BEGIN    
    --===============================================================================================================================    
    -- AUTOMATE TRANSFER STOCK    
    --===============================================================================================================================    
    INSERT INTO SpUtlStockTrfHdr    
    SELECT    
     a.CompanyCode    
     , a.CustomerCode BranchCode    
     , a.BranchCode DealerCode    
     , a.LmpNo LampiranNo    
     , a.CustomerCode RcvDealerCode    
     , '' InvoiceNo     
     , '' BinningNo     
     , '1900-01-01 00:00:00.000' BinningDate    
     , 0 Status    
     , @UserID CreatedBy    
     , GetDate() CreatedDate    
     , @UserID LastUpdateBy    
     , GetDate() LastUpdateDate    
     , null TypeOfGoods    
    FROM SpTrnSLmpHdr a    
    WHERE a.CompanyCode = @CompanyCode    
     AND a.BranchCode = @BranchCode    
     AND a.LmpNo = @TempLmpNo    
    
    INSERT INTO SpUtlStockTrfDtl    
    SELECT    
     a.CompanyCode CompanyCode    
     , b.CustomerCode BranchCode    
     , a.BranchCode DealerCode    
     , a.LmpNo LampiranNo    
     , a.DocNo OrderNo    
     , a.PartNo PartNo    
     , b.BPSFno SalesNo    
     , a.PartNo PartNoShip    
     , a.QtyBill QtyShipped     
     , 1.00 SalesUnit    
     , a.CostPrice PurchasePrice    
     , a.CostPrice costPrice    
     , '1900-01-01 00:00:00.000' ProcessDate    
     , @ProductType Producttype    
     , '' partCategory    
     , @UserID CreatedBy    
     , GetDate() CreatedDate    
     , @UserID LastUpdateBy    
     , GetDate() LastUpdateDate     
    FROM SpTrnSLmpDtl a    
    INNER JOIN SpTrnSLmpHdr b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.LmpNo = b.LmpNo    
    WHERE a.CompanyCode = @CompanyCode    
     AND a.BranchCode = @BranchCode    
     AND a.LmpNo = @TempLmpNo    
   END    
    
   --===============================================================================================================================    
   -- GENERATE GLINTERFACE    
   --===============================================================================================================================    
   IF (ISNULL((SELECT TOP 1 DocNo FROM glInterface WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND DocNo = @TempLmpNo), '') <> '')    
   BEGIN    
    SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Nomor lampiran sudah ada dalam glInterface, periksa setting-an sequence dokumen (LMP) !'    
    RAISERROR (@errmsg,16,1);    
    RETURN    
   END    
        
   INSERT INTO GLInterface    
   SELECT    
    a.CompanyCode    
    , a.BranchCode    
    , a.LmpNo DocNo    
    , 1 SeqNo    
    , a.LmpDate DocDate    
    , '300' ProfitCenterCode    
    , GetDate() AccDate    
    , @InTransitAccNo AccountNo    
    , 'SPAREPART' JournalCode    
    , 'BPS' TypeJournal    
    , a.LmpNo ApplyTo    
    , @Amount AmountDB    
    , 0 AmountCR    
    , 'INTRANSIT' TypeTrans    
    , '' BatchNo    
    , '1900-01-01 00:00:00.000' BatchDate    
    , 3 StatusFlag    
    , @UserID CreateBy     
    , GetDate() CreateDate    
    , @UserID UpdateBy    
    , GetDate() LastUpdateDate    
   FROM SpTrnSLmpHdr a    
   WHERE a.CompanyCode = @CompanyCode    
    AND a.BranchCode = @BranchCode    
    AND a.LmpNo = @TempLmpNo    
    
   INSERT INTO GLInterface    
   SELECT    
    a.CompanyCode    
    , a.BranchCode    
    , a.LmpNo DocNo    
    , 2 SeqNo    
    , a.LmpDate DocDate    
    , '300' ProfitCenterCode    
    , GetDate() AccDate    
    , @InventoryAccNo AccountNo    
    , 'SPAREPART' JournalCode    
    , 'BPS' TypeJournal    
    , a.LmpNo ApplyTo    
    , 0 AmountDB     
    , @Amount AmountCR    
    , 'INVENTORY' TypeTrans    
    , '' BatchNo    
    , '1900-01-01 00:00:00.000' BatchDate    
    , 3 StatusFlag    
    , @UserID CreateBy     
    , GetDate() CreateDate    
    , @UserID UpdateBy    
    , GetDate() LastUpdateDate    
   FROM SpTrnSLmpHdr a    
   WHERE a.CompanyCode = @CompanyCode    
    AND a.BranchCode = @BranchCode    
    AND a.LmpNo = @TempLmpNo    
    
   SET @GLDate = ISNULL((SELECT TOP 1 DocDate FROM GlInterface WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND DocNo = @TempLmpNo), NULL)    
    
   --===============================================================================================================================    
   -- GENERATE GLJOURNAL    
   --===============================================================================================================================    
   SET @MaxTempJournal = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE     
    CompanyCode = @CompanyCode    
     AND BranchCode = @BranchCode    
     AND DocumentType = 'JTS'     
     AND ProfitCenterCode = '300'     
     --AND DocumentYear = YEAR(GetDate())
	 ),0)

	declare @DocYearMaxTempJournal int
	SET @DocYearMaxTempJournal = ISNULL((SELECT DocumentYear FROM GnMstDocument WHERE     
    CompanyCode = @CompanyCode    
     AND BranchCode = @BranchCode    
     AND DocumentType = 'JTS'     
     AND ProfitCenterCode = '300'     
     --AND DocumentYear = YEAR(GetDate())
	 ),0)
    
   SET @TempJournalPrefix = ISNULL((SELECT DocumentPrefix FROM GnMstDocument WHERE     
    CompanyCode = @CompanyCode    
     AND BranchCode = @BranchCode    
     AND DocumentType = 'JTS'     
     AND ProfitCenterCode = '300'     
     --AND DocumentYear = YEAR(GetDate())
	 ),'XXX')    
    
   SET @TempJournal = ISNULL((SELECT @TempJournalPrefix + '/' + RIGHT(@DocYearMaxTempJournal,2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxTempJournal, 1), 6)),'XXX/YY/XXXXXX')

	print @TempJournal
    
   IF (ISNULL((SELECT TOP 1 JournalNo FROM GlJournal WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND JournalNo = @TempJournal), '') <> '')    
   BEGIN    
    SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Nomor journal sudah terpakai, periksa setting-an sequence dokumen (JTS) !'    
    RAISERROR (@errmsg,16,1);    
    RETURN    
   END    
    
   INSERT INTO GLJournal    
   SELECT    
    CompanyCode    
    , BranchCode    
    , @TempFiscalYear FiscalYear    
    , '300' ProfitCenterCode    
    , @TempJournal JournalNo    
    , 'Harian' JournalType    
    , GetDate() JournalDate    
    , 'SP' DocSource    
    , @TempLmpNo + ',' + @TempBPSFNo ReffNo    
    , GetDate() ReffDate    
    , @TempFiscalMonth FiscalMonth    
    , @PeriodeNum    
    , @Periode    
    , @PeriodeName    
    , @GLDate    
    , 1 BalanceType    
    , @Amount AmountDB     
    , @Amount AmountCR    
    , 1 Status    
    , '' StatusRecon    
    , NULL BatchNo    
    , '1900-01-01 00:00:00.000' PostingDate     
    , 0 StatusReverse    
    , '1900-01-01 00:00:00.000' ReverseDate    
    , 1 Printseq    
    , 0 FSend    
    , '' SendBy    
    , '1900-01-01 00:00:00.000' SendDate    
    , @UserID CreatedBy    
    , GetDate() CreatedDate    
    , @UserID LastUpdateBy    
    , GetDate() LastUpdateDate    
   FROM SpTrnSLmpHdr a    
   WHERE a.CompanyCode = @CompanyCode    
    AND a.BranchCode = @BranchCode    
    AND a.LmpNo = @TempLmpNo    
    
   --===============================================================================================================================    
   -- GENERATE GLJOURNALDTL    
   --===============================================================================================================================    
   INSERT INTO GLJournalDtl    
   SELECT    
    a.CompanyCode CompanyCode    
    , a.BranchCode BranchCode    
    , @TempFiscalYear FiscalYear    
    , @TempJournal JournalNo    
    , 1 SeqNo    
    , @InTransitAccNo AccountNo    
    , @TempLmpNo + ',' + @TempBPSFNo Description    
    , 'Harian' JournalType    
    , @Amount AmountDB     
    , 0 AmountCR    
    , 'INTRANSIT' TypeTrans    
    , @AccountTypeIntran    
    , @TempLmpNo  DocNo    
    , 0 StatusReverse    
    , '1900-01-01 00:00:00.000' ReverseDate    
    , 0 FSend    
    , @UserID CreatedBy     
    , GetDate() CreatedDate    
   FROM SpTrnSLmpHdr a    
   WHERE a.CompanyCode = @CompanyCode    
    AND a.BranchCode = @BranchCode    
    AND a.LmpNo = @TempLmpNo    
    
   INSERT INTO GLJournalDtl    
   SELECT    
    a.CompanyCode CompanyCode    
    , a.BranchCode BranchCode    
    , @TempFiscalYear FiscalYear    
    , @TempJournal JournalNo    
    , 2 SeqNo    
    , @InventoryAccNo AccountNo    
    , @TempLmpNo + ',' + @TempBPSFNo Description    
    , 'Harian' JournalType    
    , 0 AmountDB    
    , @Amount AmountCR    
    , 'INVENTORY' TypeTrans    
    , @AccountTypeInvent    
    , @TempLmpNo  DocNo    
    , 0 StatusReverse    
    , '1900-01-01 00:00:00.000' ReverseDate    
    , 0 FSend    
    , @UserID CreatedBy     
    , GetDate() CreatedDate    
   FROM SpTrnSLmpHdr a    
   WHERE a.CompanyCode = @CompanyCode    
    AND a.BranchCode = @BranchCode    
    AND a.LmpNo = @TempLmpNo    
    
   UPDATE GnMstDocument    
   SET DocumentSequence = DocumentSequence + 1    
    , LastUpdateDate = GetDate()    
    , LastUpdateBy = @UserID    
   WHERE    
    1 = 1    
    AND CompanyCode = @CompanyCode    
    AND BranchCode = @BranchCode    
    AND DocumentType = 'JTS'    
    AND ProfitCenterCode = '300'    
    --AND DocumentYear = Year(GetDate())    
  END    
  --===============================================================================================================================    
  -- END GENERATE JOURNAL Transtype = 1O (TRANSFER STOCK) --    
  --===============================================================================================================================    
      
  --===============================================================================================================================    
  -- GENERATE JOURNAL    
  -- NOTES : Transtype = 14 (OTHERS)    
  --===============================================================================================================================    
  IF (ISNULL((SELECT TransType FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0) = '14')    
  BEGIN    
   --===============================================================================================================================    
   -- GENERATE GLINTERFACE    
   --===============================================================================================================================    
   IF (ISNULL((SELECT TOP 1 DocNo FROM GlInterface WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND DocNo = @TempLmpNo), '') <> '')    
   BEGIN    
    SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Nomor lampiran sudah ada dalam glInterface, periksa setting-an sequence dokumen (LMP) !'    
    RAISERROR (@errmsg,16,1);    
    RETURN    
   END    
    
   INSERT INTO GLInterface    
   SELECT    
    a.CompanyCode    
    , a.BranchCode    
    , a.LmpNo DocNo    
    , 1 SeqNo    
    , a.LmpDate DocDate    
    , '300' ProfitCenterCode    
    , GetDate() AccDate    
    , @InTransitAccNo AccountNo    
    , 'SPAREPART' JournalCode    
    , 'BPS' TypeJournal    
    , a.LmpNo ApplyTo    
    , @Amount AmountDB    
    , 0 AmountCR    
    , 'INTRANSIT' TypeTrans    
    , '' BatchNo    
    , '1900-01-01 00:00:00.000' BatchDate    
    , 3 StatusFlag    
    , @UserID CreateBy     
    , GetDate() CreateDate    
    , @UserID UpdateBy    
    , GetDate() LastUpdateDate    
   FROM SpTrnSLmpHdr a    
   WHERE a.CompanyCode = @CompanyCode    
    AND a.BranchCode = @BranchCode    
    AND a.LmpNo = @TempLmpNo    
    
   INSERT INTO GLInterface    
   SELECT    
    a.CompanyCode    
    , a.BranchCode    
    , a.LmpNo DocNo    
    , 2 SeqNo    
    , a.LmpDate DocDate    
    , '300' ProfitCenterCode    
    , GetDate() AccDate    
    , @InventoryAccNo AccountNo    
    , 'SPAREPART' JournalCode    
    , 'BPS' TypeJournal    
    , a.LmpNo ApplyTo    
    , 0 AmountDB     
    , @Amount AmountCR    
    , 'INVENTORY' TypeTrans    
    , '' BatchNo    
    , '1900-01-01 00:00:00.000' BatchDate    
    , 3 StatusFlag    
    , @UserID CreateBy     
    , GetDate() CreateDate    
    , @UserID UpdateBy    
    , GetDate() LastUpdateDate    
   FROM SpTrnSLmpHdr a    
   WHERE a.CompanyCode = @CompanyCode    
    AND a.BranchCode = @BranchCode    
    AND a.LmpNo = @TempLmpNo    
    
   --===============================================================================================================================    
   -- GENERATE GLJOURNAL    
   --===============================================================================================================================    
   SET @MaxTempJournal = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE     
    CompanyCode = @CompanyCode    
     AND BranchCode = @BranchCode    
     AND DocumentType = 'JTI'     
     AND ProfitCenterCode = '300'     
     --AND DocumentYear = YEAR(GetDate())
	 ),0)    
    
	--declare @DocYearMaxTempJournal int
	set @DocYearMaxTempJournal = ISNULL((SELECT DocumentYear FROM GnMstDocument WHERE     
    CompanyCode = @CompanyCode    
     AND BranchCode = @BranchCode    
     AND DocumentType = 'JTI'     
     AND ProfitCenterCode = '300'     
     --AND DocumentYear = YEAR(GetDate())
	 ),0)
	 
   SET @TempJournalPrefix = ISNULL((SELECT DocumentPrefix FROM GnMstDocument WHERE     
    CompanyCode = @CompanyCode    
     AND BranchCode = @BranchCode    
     AND DocumentType = 'JTI'     
     AND ProfitCenterCode = '300'     
     --AND DocumentYear = YEAR(GetDate())
	 ),'XXX')    
    
   SET @TempJournal = ISNULL((SELECT @TempJournalPrefix + '/' + RIGHT(@DocYearMaxTempJournal,2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxTempJournal, 1), 6)),'XXX/YY/XXXXXX')
	print @TempJournal
   SET @TempFiscalYear = ISNULL((SELECT FiscalYear FROM GnMstCoProfileSpare WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode),0)     
   SET @TempFiscalMonth  = ISNULL((SELECT FiscalMonth FROM GnMstCoProfileSpare WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode),0)     
   IF (ISNULL((SELECT TOP 1 JournalNo FROM GlJournal WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND JournalNo = @TempJournal), '') <> '')    
   BEGIN    
    SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Nomor journal sudah terpakai, periksa setting-an sequence dokumen (JTS) !'    
    RAISERROR (@errmsg,16,1);    
    RETURN    
   END    
    
   INSERT INTO GLJournal    
   SELECT    
    CompanyCode    
    , BranchCode    
    , @TempFiscalYear FiscalYear    
    , '300' ProfitCenterCode    
    , @TempJournal JournalNo    
    , 'Harian' JournalType    
    , GetDate() JournalDate    
    , 'SP' DocSource    
    , @TempLmpNo ReffNo    
    , GetDate() ReffDate    
    , @TempFiscalMonth FiscalMonth    
    , @PeriodeNum    
    , @Periode    
    , @PeriodeName    
    , @GLDate    
    , 1 BalanceType    
    , @Amount AmountDB     
    , @Amount AmountCR    
    , 1 Status    
    , '' StatusRecon    
    , NULL BatchNo    
    , '1900-01-01 00:00:00.000' PostingDate     
    , '' StatusReverse    
    , '1900-01-01 00:00:00.000' ReverseDate    
    , 1 Printseq    
    , 0 FSend    
    , '' SendBy    
    , '1900-01-01 00:00:00.000' SendDate    
    , @UserID CreatedBy    
    , GetDate() CreatedDate    
    , @UserID LastUpdateBy    
    , GetDate() LastUpdateDate    
   FROM SpTrnSLmpHdr a    
   WHERE a.CompanyCode = @CompanyCode    
    AND a.BranchCode = @BranchCode    
    AND a.LmpNo = @TempLmpNo    
    
   --===============================================================================================================================    
   -- GENERATE GLJOURNALDTL    
   --===============================================================================================================================    
   INSERT INTO GLJournalDtl    
   SELECT    
    a.CompanyCode CompanyCode    
    , a.BranchCode BranchCode    
    , @TempFiscalYear FiscalYear    
    , @TempJournal JournalNo    
    , 1 SeqNo    
    , @InTransitAccNo AccountNo    
    , @TempLmpNo Description    
    , 'Harian' JournalType    
    , @Amount AmountDB     
    , 0 AmountCR    
    , 'INTRANSIT' TypeTrans    
    , @AccountTypeIntran    
    , @TempLmpNo  DocNo    
    , 0 StatusReverse    
    , '1900-01-01 00:00:00.000' ReverseDate    
    , 0 FSend    
    , @UserID CreatedBy     
    , GetDate() CreatedDate    
   FROM SpTrnSLmpHdr a    
   WHERE a.CompanyCode = @CompanyCode    
    AND a.BranchCode = @BranchCode    
    AND a.LmpNo = @TempLmpNo    
    
   INSERT INTO GLJournalDtl    
   SELECT    
    a.CompanyCode CompanyCode    
    , a.BranchCode BranchCode    
    , @TempFiscalYear FiscalYear    
    , @TempJournal JournalNo    
    , 2 SeqNo    
    , @InventoryAccNo AccountNo    
    , @TempLmpNo  Description    
    , 'Harian' JournalType    
    , 0 AmountDB    
    , @Amount AmountCR    
    , 'INVENTORY' TypeTrans    
    , @AccountTypeInvent    
    , @TempLmpNo  DocNo    
    , 0 StatusReverse    
    , '1900-01-01 00:00:00.000' ReverseDate    
    , 0 FSend    
    , @UserID CreatedBy     
    , GetDate() CreatedDate    
   FROM SpTrnSLmpHdr a    
   WHERE a.CompanyCode = @CompanyCode    
    AND a.BranchCode = @BranchCode    
    AND a.LmpNo = @TempLmpNo    
    
   UPDATE GnMstDocument    
   SET DocumentSequence = DocumentSequence + 1    
    , LastUpdateDate = GetDate()    
    , LastUpdateBy = @UserID    
   WHERE    
    1 = 1    
    AND CompanyCode = @CompanyCode    
    AND BranchCode = @BranchCode    
    AND DocumentType = 'JTI'    
    AND ProfitCenterCode = '300'    
    --AND DocumentYear = Year(GetDate())    
  END    
  --===============================================================================================================================    
  -- END GENERATE JOURNAL Transtype = 14 (OTHERS) --    
  --===============================================================================================================================    
    
  --===============================================================================================================================    
  -- UPDATE TRANSDATE    
  --===============================================================================================================================    
  update gnMstCoProfileSpare    
  set TransDate=getdate()    
   , LastUpdateBy=@UserID    
   , LastUpdateDate=getdate()    
  where CompanyCode = @CompanyCode AND BranchCode = @BranchCode    
END