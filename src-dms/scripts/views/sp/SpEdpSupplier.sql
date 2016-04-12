
create view [dbo].[SpEdpSupplier]  as  
  
SELECT a.CompanyCode, BranchCode, 
a.SupplierCode, a.SupplierName, (a.address1+' '+a.address2+' '+a.address3+' '+a.address4) as Alamat,
b.DiscPct as Diskon, (Case a.Status when 0 then 'Tidak Aktif' else 'Aktif' end) as StatusStr, a.Status, ProfitCenterCode,
(SELECT Lookupvaluename FROM gnmstlookupdtl WHERE codeid='PFCN' 
AND lookupvalue = b.ProfitCentercode) as ProfitCenterCodeStr, isBlackList
FROM 
gnMstSupplier a
JOIN gnmstSupplierProfitCenter b ON a.CompanyCode= b.CompanyCode
AND a.SupplierCode = b.SupplierCode


GO


