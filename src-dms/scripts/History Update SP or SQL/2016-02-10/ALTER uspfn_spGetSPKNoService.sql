ALTER procedure [dbo].[uspfn_spGetSPKNoService] @CompanyCode varchar(15), @BranchCode varchar(15), @LmpNo varchar(20)    
as    
SELECT c.*,     
d.CustomerCode, d.CustomerName, d.CustomerGovName, d.Address1, d.Address2, d.Address3, d.Address4,    
d.CityCode ,e.LookUpValueName as City, d.HpNo, d.FaxNo, c.VIN, c.EngineNo, x.LmpNo, x.DocDate as LmpDate  FROM     
(    
 SELECT TOP 1 a.*     
 FROM SpTrnSLmpDtl a     
 WHERE a.companyCode =   @CompanyCode    
   AND a.branchCode    = @BranchCode    
   AND a.lmpno         = @LmpNo     
) x     
INNER JOIN spTrnSOrdHdr b ON b.CompanyCode = x.CompanyCode     
  AND b.BranchCode = x.BranchCode     
  AND b.DocNo = x.DocNo     
INNER JOIN svTrnService c ON c.CompanyCode = b.CompanyCode    
  AND c.BranchCode = b.BranchCode    
  AND c.JobOrderNo = b.UsageDocNo    
INNER JOIN GnMstCustomer d on c.CompanyCode = d.CompanyCode    
  AND c.CustomerCode = d.CustomerCode    
LEFT JOIN GnMstLookupDtl e on e.CompanyCode = d.CompanyCode    
and e.CodeID = 'City'    
and e.LookupValue = d.CityCode    
WHERE x.CompanyCode = @CompanyCode     
  AND x.BranchCode  = @BranchCode     
  AND x.Lmpno       = @LmpNo 