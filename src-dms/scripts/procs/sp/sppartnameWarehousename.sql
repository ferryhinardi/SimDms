create view sppartnameWarehousename
as
SELECT  distinct      a.PartNo, a.PartName, b.WarehouseCode, c.LookUpValueName [Warehousename]
FROM            spMstItemInfo AS a INNER JOIN
                         spMstItemLoc AS b ON a.CompanyCode = b.CompanyCode AND a.PartNo = b.PartNo INNER JOIN
                         gnMstLookUpDtl AS c ON a.CompanyCode = c.CompanyCode 
						 AND b.WarehouseCode = c.LookUpValue
						 where c.CodeID='WRCD'
				
						  
