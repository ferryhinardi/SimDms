Create procedure [dbo].[uspfn_spTrnPPOSHdrView] 
@CompanyCode varchar(10),@BranchCode varchar(10),
@TypeOfGoods varchar(10),
 @Status  int
as               
				
				SELECT a.POSNo, a.PosDate , a.Status ,a.SupplierCode ,b.SupplierName
                FROM spTrnPPOSHdr a
                INNER JOIN gnMstSupplier b ON b.SupplierCode = a.SupplierCode and b.CompanyCode = a.CompanyCode
                WHERE a.CompanyCode=@CompanyCode 
                AND a.BranchCode=@BranchCode
                AND a.TypeOfGoods=@TypeOfGoods