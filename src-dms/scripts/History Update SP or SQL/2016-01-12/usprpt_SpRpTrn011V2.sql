ALTER procedure [dbo].[usprpt_SpRpTrn011V2]
--declare  
   @CompanyCode  VARCHAR (15),  
   @BranchCode   VARCHAR (15),  
   @FPJNoStart   VARCHAR (15),  
   @FPJNoEnd     VARCHAR (15),  
   @ProfitCenter VARCHAR (15),  
   @CounterDiv   DECIMAL,  
   @TypeOfGoods  VARCHAR (2)   
AS  

--select @CompanyCode= '6039401',@BranchCode='603940100',@FPJNoStart='FPJ/16/000355',@FPJNoEnd='FPJ/16/000355',
--@ProfitCenter='300',@CounterDiv=1,@TypeOfGoods='0'
  
declare @IsBranch as bit  
set @IsBranch=(select isBranch from gnMstOrganizationDtl where CompanyCode=@CompanyCode and BranchCode=@BranchCode)  
  
BEGIN  
SELECT   
 m.ParaValue,
 c.FPJNo,  
 c.FPJDate,  
 c.InvoiceNo,  
 c.PickingSlipNo,   
 c.FpjGovNo as fakturFPJGovNo,  
 isnull((  
  select top 1 xa.OrderNo   
    from spTrnSORDHdr xa  
   inner join spTrnSORDDtl xb  
   on xb.CompanyCode = xa.CompanyCode  
     and xb.BranchCode = xa.BranchCode  
     and xb.DocNo = xa.DocNo  
   inner join SpTrnSFPJDtl xf  
   on xf.CompanyCode = xb.CompanyCode  
     and xf.BranchCode = xb.BranchCode  
     and xf.DocNo = xb.DocNo  
     and xf.FPJNo = c.FPJNo  
   where 1 = 1  
     and xa.CompanyCode = @CompanyCode  
     and xa.BranchCode = @BranchCode  
  ), '') OrderNo,  
-- CASE WHEN  
--  ISNULL((SELECT COUNT(DISTINCT ReferenceNo) FROM spTrnSFpjDtl WHERE   
--   CompanyCode = @CompanyCode   
--   AND BranchCode = @BranchCode  
--   AND FPJNo = c.FpjNo),0) = 1   
--    THEN ISNULL((select top 1 ReferenceNo from spTrnSInvoiceDtl  
--  THEN ISNULL((SELECT DISTINCT ReferenceNo FROM spTrnSFpjDtl WHERE   
--   CompanyCode = @CompanyCode   
--   AND BranchCode = @BranchCode  
--   AND FPJNo = c.FpjNo),'')  
--  ELSE ''  
-- END OrderNo,  
 c.CustomerCode,   
 c.DueDate,  
 c.TotDiscAmt,  
 c.TotDppAmt,   
 c.TotPPNAmt,  
 c.TotFinalSalesAmt,  
 --x.CustomerName + ' / '+ d.CustomerName + ' [' + c.CustomerCode + ']'as CustomerName,   
 --x.Address1,   
 --x.Address2,   
 --x.Address3,   
 --x.Address4, 

 CASE WHEN l.ParaValue = '0' THEN d.CustomerName + ' [' + c.CustomerCode + ']' ELSE x.CustomerName + ' / '+ d.CustomerName + ' [' + c.CustomerCode + ']' END as CustomerName, -- update
 CASE WHEN m.ParaValue = '0' THEN d.Address1 ELSE x.Address1 END as Address1, -- update
 CASE WHEN m.ParaValue = '0' THEN d.Address2 ELSE x.Address2 END as Address2, -- update
 CASE WHEN m.ParaValue = '0' THEN d.Address3 ELSE x.Address3 END as Address3, -- update
 CASE WHEN m.ParaValue = '0' THEN d.Address4 ELSE x.Address4 END as Address4, -- update
 d.PhoneNo,   
 d.FaxNo,  
 d.NPWPNo,   
 a.DocNo,    
 a.PartNo,   
 a.PartNoOriginal,  
 a.QtyBill,  
 a.RetailPrice,  
 a.DiscPct,  
 a.DiscAmt,   
 a.RetailPrice - ((a.RetailPrice * a.DiscPct) / 100) NetRetailPrice,  
 a.NetSalesAmt,   
 b.PartName,  
 e.LookUpValueName  TOPC,   
 h.LookUpValueName  CITY,  
 i.TaxPct,   
 RIGHT(replicate('0', 2) + convert(varchar(2), c.PrintSeq), 2) PrintSeq,  
 c.PrintSeq,  
 c.TypeOfGoods,  
 case   
  when @IsBranch=0 then f.CompanyGovName  
  else f.CompanyGovName+' ('+ substring(a.BranchCode,len(a.branchCode)-1,2)+')'   
 end CompanyName,  
 f.Address1 AS AddressCo1,  
 f.Address2 AS AddressCo2,  
 f.Address3 AS AddressCo3,  
 k.Remark,  
 UPPER (j.SignName)  SignName,   
 UPPER (j.TitleSign)  TitleSign  
 , (SELECT LookUpValueName FROM gnMstLookUpDtl WHERE CompanyCode = a.CompanyCode AND CodeID = 'NOTE' AND LookUpValue = 'SPFP01') Note1  
 , (SELECT LookUpValueName FROM gnMstLookUpDtl WHERE CompanyCode = a.CompanyCode AND CodeID = 'NOTE' AND LookUpValue = 'SPFP02') Note2  
 , (SELECT LookUpValueName FROM gnMstLookUpDtl WHERE CompanyCode = a.CompanyCode AND CodeID = 'NOTE' AND LookUpValue = 'SPFP03') Note3  
FROM                           
(  
   SELECT CompanyCode,  
          BranchCode,   
    FPJNo,  
          PartNo,  
    DocNo,  
    PartNoOriginal,  
          RetailPrice,  
          DiscPct,  
          SUM(DiscAmt) AS DiscAmt,  
          SUM(NetSalesAmt) AS NetSalesAmt,  
          SUM(QtyBill) AS QtyBill  
   FROM SpTrnSFPJDtl WITH (NOLOCK, NOWAIT)  
   GROUP BY CompanyCode, BranchCode,FPJNo, DocNo,PartNo,PartNoOriginal,  
          RetailPrice, DiscPct  
   HAVING CompanyCode = @CompanyCode  
      AND BranchCode = @BranchCode  
      AND FPJNo BETWEEN @FPJNoStart AND @FPJNoEnd  
) a   
 INNER JOIN SpMstItemInfo b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode  
  AND a.PartNo = b.PartNo  
    INNER JOIN SpTrnSFPJHdr c WITH (NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode  
      AND a.BranchCode = c.BranchCode  
      AND a.FPJNo = c.FPJNo  
    INNER JOIN GnMstCustomer d WITH (NOLOCK, NOWAIT) ON c.CompanyCode = d.CompanyCode  
       AND c.CustomerCode = d.CustomerCode  
 INNER JOIN SpTrnSFPJInfo x WITH (NOLOCK, NOWAIT) ON c.CompanyCode = x.CompanyCode   
    AND c.BranchCode = x.BranchCode  
    AND c.FPJNo = x.FPJNo  
 INNER JOIN GnMstLookUpDtl e WITH (NOLOCK, NOWAIT) ON c.CompanyCode = e.CompanyCode  
  AND c.TOPCode = e.LookUpValue  
  AND e.CodeID = 'TOPC'  
 INNER JOIN GnMstCoProfile f WITH (NOLOCK, NOWAIT) ON a.CompanyCode = f.CompanyCode  
  AND a.BranchCode = f.BranchCode  
 LEFT JOIN GnMstLookUpDtl h WITH (NOLOCK, NOWAIT) ON f.CompanyCode = h.CompanyCode  
  AND f.CItyCode = h.LookUpValue  
  AND h.CodeID = 'CITY'  
 INNER JOIN GnMstTax i WITH (NOLOCK, NOWAIT) ON a.CompanyCode = i.CompanyCode   
  AND i.TaxCode = 'PPN'  
 LEFT JOIN GnMstSignature j WITH (NOLOCK, NOWAIT) ON a.CompanyCode = j.CompanyCode  
  AND a.BranchCode = j.BranchCode  
  AND j.ProfitCenterCode = @ProfitCenter  
  AND j.DocumentType = CONVERT (VARCHAR (3), 'SDH')  
  AND j.SeqNo = @CounterDiv  
 LEFT JOIN spTrnSPickingHdr k ON a.CompanyCode = k.CompanyCode  
  AND a.BranchCode = k.BranchCode  
  AND c.PickingSlipNo = k.PickingSlipNo 
  LEFT JOIN gnMstLookUpDtl l ON a.CompanyCode = l.CompanyCode
  AND l.CodeID = 'FLPG'
  AND l.LookUpValue = 'FPJ_NAME'
 LEFT JOIN gnMstLookUpDtl m ON a.CompanyCode = m.CompanyCode
  AND m.CodeID = 'FLPG'
  AND m.LookUpValue = 'FPJ_ADDR'    
WHERE a.CompanyCode = @CompanyCode  
 AND a.BranchCode = @BranchCode  
 AND a.FPJNo BETWEEN @FPJNoStart AND @FPJNoEnd  
 AND c.TypeOfGoods = @TypeOfGoods  
ORDER BY a.FPJNo, a.PartNo ASC  
  
END  
