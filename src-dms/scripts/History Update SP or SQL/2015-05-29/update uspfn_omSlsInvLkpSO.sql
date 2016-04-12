ALTER procedure [dbo].[uspfn_omSlsInvLkpSO]     
(    
 @CompanyCode varchar(15),    
 @BranchCode varchar(15)   
)    
AS    
BEGIN    
-- exec uspfn_omSlsInvLkpSO 6006410,600641001  
    SELECT tableA.SONo,tableA.QtyBPK,tableA.QtyInvoice, tableB.CustomerCode, tableB.CustomerName, tableB.BillTo, tableB.BillName,  
    tableB.Address,tableB.SalesType,tableB.SalesTypeDsc,tableB.TOPDays, tableB.SKPKNo, tableB.RefferenceNo      
      FROM (SELECT a.SONo, sum (b.QuantityBPK)  AS QtyBPK, sum (b.QuantityInvoice)  AS QtyInvoice                     
              FROM omTrSalesBPK a, omTrSalesBPKModel b  
             WHERE a.CompanyCode = b.CompanyCode  
                   AND a.BranchCode = b.BranchCode  
                   AND a.BPKNo = b.BPKNo  
                   AND a.CompanyCode = @CompanyCode  
                   AND a.BranchCode = @BranchCode  
                   AND a.Status = '2'  
             GROUP BY a.SONo) tableA,  
           (SELECT a.SONo, a.CustomerCode, b.CustomerName, a.BillTo, b.CustomerName as BillName,  
   b.Address1 + ' ' + b.Address2 + ' ' + b.Address3 + ' ' + b.Address4 as Address,a.SalesType  
            , (CASE ISNULL(a.SalesType, 0) WHEN 0 THEN 'WholeSales' ELSE 'Direct' END) AS SalesTypeDsc  
            , ISNULL(a.TOPDays, 0) AS TOPDays, a.SKPKNo, a.RefferenceNo  
              FROM omTrSalesSO a  
     LEFT JOIN gnMstCustomer b ON a.CompanyCode = b.CompanyCode AND a.CustomerCode = b.CustomerCode  
             WHERE a.CompanyCode = @CompanyCode  
                   AND a.BranchCode = @BranchCode  
                   AND a.Status = '2') tableB  
    WHERE tableA.QtyBPK > tableA.QtyInvoice AND tableA.SONo = tableB.SONo 
    AND tableA.SONo NOT IN (SELECT z.SONo FROM omTrSalesInvoice z where z.Status <> 3) -- Tambahan Dimas
      
 ORDER BY tableA.SONo  
END
