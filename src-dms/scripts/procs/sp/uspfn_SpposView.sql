
 ALTER procedure [dbo].[uspfn_SpposView] (  @CompanyCode varchar(15) ,@BranchCode varchar(15), @TypeOfGoods	 varchar(15))
   as
            SELECT 
                DISTINCT a.POSNo, a.SupplierCode, b.SupplierName 
            FROM 
                spTrnPOrderBalance a 
            INNER JOIN gnMstSupplier b 
               ON b.SupplierCode = a.SupplierCode 
              AND b.CompanyCode  = a.CompanyCode 
            WHERE a.OrderQty > a.Received
              AND a.CompanyCode = @CompanyCode
              AND a.BranchCode  = @BranchCode
			  AND a.TypeOfGoods = @TypeOfGoods	
            ORDER BY POSNo DESC