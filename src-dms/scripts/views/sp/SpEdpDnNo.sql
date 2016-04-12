

create view [dbo].[SpEdpDnNo]  as    

SELECT DISTINCT a.CompanyCode, BranchCode,
a.DeliveryNo 
,a.SupplierCode
,ISNULL(b.SupplierName,'') as SupplierName 
FROM spUtlPINVDDTL a
LEFT JOIN GnMstSupplier b ON a.CompanyCode=b.CompanyCode 
AND a.SupplierCode=b.SupplierCode
WHERE a.Status in ('0','1')


GO


