alter procedure [dbo].[uspfn_spTrnPOrderBalance] (  @CompanyCode varchar(15) ,  @BranchCode varchar(15) )
 as

 		    SELECT 
                DISTINCT a.POSNo, a.SupplierCode, b.SupplierName ,a.CompanyCode,a.BranchCode
            FROM 
                spTrnPOrderBalance a 
            INNER JOIN gnMstSupplier b 
               ON b.SupplierCode = a.SupplierCode 
              AND b.CompanyCode  = a.CompanyCode 
            WHERE a.OrderQty > a.Received
              AND a.CompanyCode = @CompanyCode
              AND a.BranchCode  = @BranchCode
            
            ORDER BY POSNo DESC