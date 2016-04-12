     create PROCEDURE [dbo].[uspfn_SpcekvalidTrnPOrderBalance]
@CompanyCode varchar(15),
@BranchCode varchar(15) ,
@TypeOfGoods varchar(15) ,
@POSNo varchar(15)

AS     
		 
		 
		    SELECT 
                DISTINCT a.POSNo
            FROM 
                spTrnPOrderBalance a 
            INNER JOIN gnMstSupplier b 
               ON b.SupplierCode = a.SupplierCode 
              AND b.CompanyCode  = a.CompanyCode 
            WHERE a.OrderQty > a.Received
              AND a.CompanyCode = @CompanyCode
              AND a.BranchCode  = @BranchCode
              AND a.TypeOfGoods = @TypeOfGoods
              AND a.POSNo       = @POSNo
            ORDER BY POSNo DESC