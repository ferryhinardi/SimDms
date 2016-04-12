ALTER procedure [dbo].[usprpt_OmRpSalRgs004]   
(  
      @CompanyCode VARCHAR(15),  
      @BranchCode VARCHAR(15),  
      @FromDate VARCHAR(15),  
      @ToDate VARCHAR(15),  
      @SalesType VARCHAR(2),  
      @CustomerFrom VARCHAR(15),  
      @CustomerTo VARCHAR(15),  
      @ModelFrom VARCHAR(15),  
      @ModelTo VARCHAR(15),  
      @InvoiceFrom VARCHAR(15),  
      @InvoiceTo VARCHAR(15)  
--    @param CHAR(2)  
)  
AS  
BEGIN  
  
--usprpt_OmRpSalRgs004 '6093401','609340101','','','','','','','','IBU/13/001023','IBU/13/001023'  
  
--declare   @CompanyCode VARCHAR(15),  
--          @BranchCode VARCHAR(15),  
--          @FromDate VARCHAR(15),  
--          @ToDate VARCHAR(15),  
--          @SalesType VARCHAR(2),  
--          @CustomerFrom VARCHAR(15),  
--          @CustomerTo VARCHAR(15),  
--          @ModelFrom VARCHAR(15),  
--          @ModelTo VARCHAR(15),  
--          @InvoiceFrom VARCHAR(15),  
--          @InvoiceTo VARCHAR(15)  
--  
--set @CompanyCode = '6093401'  
--set @BranchCode = '609340101'  
--set @FromDate = ''  
--set @ToDate = ''  
--set @SalesType = ''  
--set @CustomerFrom = ''  
--set @CustomerTo = ''  
--set @ModelFrom = ''  
--set @ModelTo = ''  
--set @InvoiceFrom = 'IBU/13/001023'  
--set @InvoiceTo = 'IBU/13/001023'  
  
SELECT   
      a.CompanyCode,a.BranchCode  
      ,(  
            select CompanyName from gnMstCoProfile  
            where CompanyCode=a.CompanyCode and BranchCode=a.BranchCode  
      ) BranchName  
      ,a.InvoiceDate,a.InvoiceNo,a.FakturPajakNo  
      ,case when convert(varchar, a.FakturPajakDate, 112) = '19000101' then '' else a.FakturPajakDate end FakturPajakDate  
      ,g.SKPKNo + '/' + g.RefferenceNo as SKPKNo,  
      f.CustomerName + ' [' + a.CustomerCode + ']' as pCustomer,e.DONo,a.SONo,b.BPKNo,c.SalesModelCode  
      , h.SalesModelDesc, c.SalesModelYear,  
      d.ColourCode  
      ,(    select RefferenceDesc1   
            from omMstRefference   
            where CompanyCode=a.CompanyCode and RefferenceType='COLO' and RefferenceCode=d.ColourCode  
      ) ColourName  
      ,d.ChassisNo,d.EngineNo  
      ,(  
            select count(ChassisNo) from omTrSalesInvoiceVin  
            where CompanyCode=a.CompanyCode and BranchCode=a.BranchCode and InvoiceNo=a.InvoiceNo  
                  and ChassisCode=d.ChassisCode and ChassisNo=d.ChassisNo  
      ) Qty  
      ,c.BeforeDiscDPP,c.DiscExcludePPn,c.AfterDiscDPP  
      --,((ISNULL((SELECT SUM(AfterDiscDPP) FROM omTrSalesInvoiceOthers   
      --    WHERE CompanyCode = @CompanyCode  
      --          AND BranchCode = @BranchCode  
      --          AND InvoiceNo = a.InvoiceNo), 0) + c.AfterDiscDPP) * 0.10) AfterDiscPPn, c.AfterDiscPPnBM, v.PPnBMBuyPaid  
      ,((ISNULL((SELECT SUM(AfterDiscPPn) FROM omTrSalesInvoiceOthers --Perubahan DIMAS  
            WHERE CompanyCode = @CompanyCode --Perubahan  
                  AND BranchCode = @BranchCode --Perubahan  
                  AND InvoiceNo = a.InvoiceNo), 0) + c.AfterDiscPPn)) AfterDiscPPn, c.AfterDiscPPnBM, v.PPnBMBuyPaid --Perubahan  
      , ISNULL((SELECT SUM(AfterDiscDPP) FROM omTrSalesInvoiceOthers  
            WHERE CompanyCode = @CompanyCode  
                  AND BranchCode = @BranchCode  
                  AND InvoiceNo = a.InvoiceNo), 0) TotalOthers  
      --, ((ISNULL((SELECT SUM(AfterDiscDPP) FROM omTrSalesInvoiceOthers   
      --    WHERE CompanyCode = @CompanyCode  
      --          AND BranchCode = @BranchCode  
      --          AND InvoiceNo = a.InvoiceNo), 0) + c.AfterDiscDPP) * 0.10) +  
      , ((ISNULL((SELECT SUM(AfterDiscPPn) FROM omTrSalesInvoiceOthers --Perubahan DIMAS  
            WHERE CompanyCode = @CompanyCode --Perubahan  
                  AND BranchCode = @BranchCode --Perubahan  
                  AND InvoiceNo = a.InvoiceNo), 0) + c.AfterDiscPPn)) + --Perubahan  
        (ISNULL((SELECT SUM(AfterDiscDPP) FROM omTrSalesInvoiceOthers   
            WHERE CompanyCode = @CompanyCode  
                  AND BranchCode = @BranchCode  
                  AND InvoiceNo = a.InvoiceNo), 0) + c.AfterDiscDPP) AfterDiscTotal  
      ,case @SalesType when '0' then 'WHOLE SALES' when '1' then 'DIRECT' else 'All' end as pSALESTYPE,  
      case when @CustomerFrom = '' then 'All'  
            else @CustomerFrom + ' s/d ' + @CustomerTo   
      end as pCUST,  
      case when @ModelFrom = '' then 'All'  
            else @ModelFrom + ' s/d ' + @ModelTo   
      end as pMODEL,  
      case when @InvoiceFrom= '' then 'All'  
            else @InvoiceFrom + ' s/d ' + @InvoiceTo   
      end as pINV  
FROM   
      omTrSalesInvoice a  
      INNER JOIN omTrSalesInvoiceBPK b ON a.CompanyCode = b.CompanyCode  
            AND a.BranchCode = b.BranchCode AND a.InvoiceNo = b.InvoiceNo  
      INNER JOIN omTrSalesInvoiceModel c ON a.CompanyCode = c.CompanyCode  
            AND a.BranchCode = c.BranchCode AND a.InvoiceNo = c.InvoiceNo  
            AND b.BPKNo = c.BPKNo  
      LEFT JOIN omTrSalesInvoiceVin d ON a.CompanyCode = d.CompanyCode  
            AND a.BranchCode = d.BranchCode AND a.InvoiceNo = d.InvoiceNo  
            AND b.BPKNo = d.BPKNo AND c.SalesModelCode = d.SalesModelCode  
            AND c.SalesModelYear = d.SalesModelYear  
      LEFT JOIN omTrSalesBPK e ON a.CompanyCode = e.CompanyCode  
            AND a.BranchCode = e.BranchCode AND b.BPKNo = e.BPKNo  
      LEFT JOIN gnMstCustomer f ON a.CompanyCode = f.CompanyCode  
            AND a.CustomerCode = f.CustomerCode  
      LEFT JOIN omTrSalesSO g ON a.CompanyCode = g.CompanyCode  
            AND a.BranchCode = g.BranchCode AND a.SONo = g.SONo  
      LEFT JOIN omMstModel h ON a.CompanyCode = h.CompanyCode  
            AND c.SalesModelCode = h.SalesModelCode     
      LEFT JOIN OmMstVehicle v ON d.CompanyCode = v.CompanyCode   
            AND d.ChassisCode = v.ChassisCode AND d.ChassisNo = v.ChassisNo  
WHERE   
      a.CompanyCode = @CompanyCode  
      AND ((CASE WHEN @BranchCode = '' THEN a.BranchCode END) <> ''  
                  OR (CASE WHEN @BranchCode <> '' THEN a.BranchCode END) = @BranchCode)  
      AND ((CASE WHEN @FromDate = '' THEN a.InvoiceDate END) <> ''  
                  OR (CASE WHEN @FromDate <> '' THEN CONVERT(VARCHAR, a.InvoiceDate, 112) END) BETWEEN @FromDate AND @ToDate)  
      AND ((CASE WHEN @InvoiceFrom = '' THEN a.InvoiceNo END) <> ''  
                  OR (CASE WHEN @InvoiceFrom <> '' THEN a.InvoiceNo END) BETWEEN @InvoiceFrom AND @InvoiceTo)  
      AND ((CASE WHEN @CustomerFrom = '' THEN a.CustomerCode END) <> ''  
                  OR (CASE WHEN @CustomerFrom <> '' THEN a.CustomerCode END) BETWEEN @CustomerFrom AND @CustomerTo)  
      AND ((CASE WHEN @ModelFrom = '' THEN c.SalesModelCode END) <> ''  
                  OR (CASE WHEN @ModelFrom <> '' THEN c.SalesModelCode END) BETWEEN @ModelFrom AND @ModelTo)  
      AND ((CASE WHEN @SalesType = '' THEN g.SalesType END) <> ''  
                  OR (CASE WHEN @SalesType <> '' THEN g.SalesType END) = @SalesType)  
                             
ORDER BY   
      a.BranchCode,a.InvoiceNo  
  
SELECT DISTINCT  
      a.BranchCode  
      , f.CompanyName  
      , c.SalesModelCode  
      , h.SalesModelDesc  
      , d.ColourCode  
      , e.RefferenceDesc1 ColourName  
      ,COUNT(d.ChassisNo) Unit  
      ,SUM(c.AfterDiscTotal) AfterDiscTotal  
FROM   
      omTrSalesInvoice a  
      INNER JOIN omTrSalesInvoiceBPK b ON a.CompanyCode = b.CompanyCode  
            AND a.BranchCode = b.BranchCode AND a.InvoiceNo = b.InvoiceNo  
      INNER JOIN omTrSalesInvoiceModel c ON a.CompanyCode = c.CompanyCode  
            AND a.BranchCode = c.BranchCode AND a.InvoiceNo = c.InvoiceNo  
            AND b.BPKNo = c.BPKNo  
      LEFT JOIN omTrSalesInvoiceVin d ON a.CompanyCode = d.CompanyCode  
            AND a.BranchCode = d.BranchCode AND a.InvoiceNo = d.InvoiceNo  
            AND b.BPKNo = d.BPKNo AND c.SalesModelCode = d.SalesModelCode  
            AND c.SalesModelYear = d.SalesModelYear  
      LEFT JOIN omMstRefference e ON a.CompanyCode = e.CompanyCode  
            AND RefferenceType='COLO' AND RefferenceCode=d.ColourCode  
      LEFT JOIN gnMstCoProfile f ON a.CompanyCode = a.CompanyCode  
            AND a.BranchCode = f.BranchCode  
      LEFT JOIN omTrSalesSO g ON a.CompanyCode = g.CompanyCode  
            AND a.BranchCode = g.BranchCode AND a.SONo = g.SONo  
      LEFT JOIN omMstModel h ON a.CompanyCode = h.CompanyCode  
            AND c.SalesModelCode = h.SalesModelCode  
WHERE   
      a.CompanyCode = @CompanyCode  
      AND ((CASE WHEN @BranchCode = '' THEN a.BranchCode END) <> ''  
                  OR (CASE WHEN @BranchCode <> '' THEN a.BranchCode END) = @BranchCode)  
      AND ((CASE WHEN @FromDate = '' THEN a.InvoiceDate END) <> ''  
                  OR (CASE WHEN @FromDate <> '' THEN CONVERT(VARCHAR, a.InvoiceDate, 112) END) BETWEEN @FromDate AND @ToDate)  
      AND ((CASE WHEN @InvoiceFrom = '' THEN a.InvoiceNo END) <> ''  
                  OR (CASE WHEN @InvoiceFrom <> '' THEN a.InvoiceNo END) BETWEEN @InvoiceFrom AND @InvoiceTo)  
      AND ((CASE WHEN @CustomerFrom = '' THEN a.CustomerCode END) <> ''  
                  OR (CASE WHEN @CustomerFrom <> '' THEN a.CustomerCode END) BETWEEN @CustomerFrom AND @CustomerTo)  
      AND ((CASE WHEN @ModelFrom = '' THEN c.SalesModelCode END) <> ''  
                  OR (CASE WHEN @ModelFrom <> '' THEN c.SalesModelCode END) BETWEEN @ModelFrom AND @ModelTo)  
      AND ((CASE WHEN @SalesType = '' THEN g.SalesType END) <> ''  
                  OR (CASE WHEN @SalesType <> '' THEN g.SalesType END) = @SalesType)  
GROUP BY  
      c.SalesModelCode  
      , h.SalesModelDesc  
      , d.ColourCode      
      , e.RefferenceDesc1  
      , a.BranchCode  
      , f.CompanyName  
ORDER BY a.BranchCode, c.SalesModelCode, d.ColourCode  
  
  
END  
