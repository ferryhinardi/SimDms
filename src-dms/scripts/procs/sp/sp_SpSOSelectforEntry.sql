 CREATE PROCEDURE sp_SpSOSelectforEntry
@CompanyCode varchar(15),  
@BranchCode varchar(15),   
@ProductType  varchar(15),
@WarehouseCode  varchar(15)   

AS  
 SELECT
 distinct
	    a.PartNo,
	    (SELECT PartName FROM spMstItemInfo WHERE CompanyCode = a.CompanyCode AND PartNo = a.PartNo) AS PartName
    FROM
	    spMstItems a
        INNER JOIN spMstItemLoc b ON a.CompanyCode = b.CompanyCode
            AND a.BranchCode = b.BranchCode
            AND a.PartNo = b.PartNo
    WHERE
	    a.CompanyCode = @CompanyCode
	    AND a.BranchCode = @BranchCode	
	    AND a.ProductType = @ProductType
        AND b.WarehouseCode = @WarehouseCode
    ORDER BY
        PartNo ASC